using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NBagOfTricks;
using NBagOfTricks.Slog;
using NBagOfUis;
using AudioLib; // TODO restore dll ref.
using static System.Collections.Specialized.BitVector32;


//TODO1 cmbSelMode: Sample, Beat, Time + Snap on/off
// - Beats mode:
//   - Establish timing by select two samples and identify corresponding number of beats.
//   - Show in waveform.
//   - Subsequent selections are by beat using snap.
// - Time mode:
//   - Select two times using ?? resolution.
//   - Shows number of samples and time in UI.
// - Sample mode:
//   - Select two samples using ?? resolution.
//   - Shows number of samples and time in UI.


namespace Wavicler
{
    public partial class MainForm : Form
    {
        #region Fields
        /// <summary>My logger.</summary>
        readonly Logger _logger = LogManager.CreateLogger("MainForm");

        /// <summary>The actual player.</summary>
        readonly AudioPlayer _player;

        /// <summary>Dynamically connect input providers to the player.</summary>
        SwappableSampleProvider _waveOutSwapper = new();

        /// <summary>Where we be.</summary>
        AppState _currentState = AppState.Stop;

        /// <summary>Where to put stuff.</summary>
        string _outPath;

        /// <summary>The settings.</summary>
        UserSettings _settings;
        #endregion

        #region Lifecycle
        public MainForm()
        {
            //SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);

            // Must do this first before initializing.
            string appDir = MiscUtils.GetAppDataDir("Wavicler", "Ephemera");
            _settings = (UserSettings)Settings.Load(appDir, typeof(UserSettings));
            // Tell the libs about their settings.
            AudioSettings.LibSettings = _settings.AudioSettings;

            InitializeComponent();

            Icon = Properties.Resources.tiger;

            // Init logging.
            LogManager.MinLevelFile = _settings.FileLogLevel;
            LogManager.MinLevelNotif = _settings.NotifLogLevel;
            LogManager.LogEvent += (object? sender, LogEventArgs e) => { this.InvokeIfRequired(_ => { tvLog.AppendLine($"{e.Message}"); }); };
            LogManager.Run();
            tvLog.Font = Font;
            tvLog.MatchColors.Add("ERR", Color.LightPink);
            tvLog.MatchColors.Add("WRN:", Color.Plum);

            // Set up paths.
            _outPath = Path.Combine(appDir, "out");
            DirectoryInfo di = new(_outPath);
            di.Create();

            // Init main form from settings
            WindowState = FormWindowState.Normal;
            StartPosition = FormStartPosition.Manual;
            Location = new Point(_settings.FormGeometry.X, _settings.FormGeometry.Y);
            Size = new Size(_settings.FormGeometry.Width, _settings.FormGeometry.Height);
            // TODO not working... KeyPreview = true; // for routing kbd strokes through OnKeyDown

            // Create output.
            _player = new(_settings.AudioSettings.WavOutDevice, int.Parse(_settings.AudioSettings.Latency), _waveOutSwapper);
            // Usually end of file but could be error.
            _player.PlaybackStopped += (object? sender, StoppedEventArgs e) =>
            {
                if (e.Exception is not null)
                {
                    _logger.Exception(e.Exception, "Other NAudio error");
                    UpdateState(AppState.Dead);
                }
                else
                {
                    UpdateState(AppState.Complete);
                }
            };

            // Hook up audio processing chain.
            //var postVolumeMeter = new MeteringSampleProvider(_waveOutSwapper, _waveOutSwapper.WaveFormat.SampleRate / 10);
            //postVolumeMeter.StreamVolume += (object? sender, StreamVolumeEventArgs e) =>
            //{
            //    // timeBar.Current = _reader.CurrentTime;
            //};

            // Other UI items.
            toolStrip.Renderer = new NBagOfUis.CheckBoxRenderer() { SelectedColor = _settings.ControlColor };

            btnAutoplay.Checked = _settings.Autoplay;
            btnAutoplay.Click += (_, __) => { _settings.Autoplay = btnAutoplay.Checked; };

            btnLoop.Checked = _settings.Loop;
            btnLoop.Click += (_, __) => { _settings.Loop = btnLoop.Checked; };

            btnSnap.Checked = _settings.Snap;
            btnSnap.Click += (_, __) => { _settings.Snap = btnSnap.Checked; };

            sldVolume.DrawColor = _settings.ControlColor;
            sldVolume.Value = _settings.Volume;
            sldVolume.ValueChanged += (_, __) => { _player.Volume = (float)sldVolume.Value; };

            //sldBPM.DrawColor = _settings.ControlColor;
            //sldBPM.Value = _settings.BPM;
            //sldBPM.ValueChanged += (_, __) => { _settings.BPM = sldBPM.Value; };

            txtBPM.Text = _settings.BPM.ToString();
            txtBPM.KeyPress += (object? sender, KeyPressEventArgs e) =>
            {
                KeyUtils.TestForNumber_KeyPress(sender!, e);
                _settings.BPM = double.Parse(txtBPM.Text);
            };

            cmbSelMode.Items.Add(SelectionMode.Time);
            cmbSelMode.Items.Add(SelectionMode.Beat);
            cmbSelMode.Items.Add(SelectionMode.Sample);
            cmbSelMode.SelectedItem = _settings.SelectionMode;
            cmbSelMode.SelectedIndexChanged += (_, __) => { _settings.SelectionMode = (SelectionMode)cmbSelMode.SelectedItem; };

            btnRewind.Click += (_, __) => { UpdateState(AppState.Rewind); };
            btnPlay.Click += (_, __) => { UpdateState(btnPlay.Checked ? AppState.Play : AppState.Stop); };

            // File handling.
            //NewMenuItem.Click += (_, __) => { OpenFile(); };
            OpenMenuItem.Click += (_, __) => { Open_Click(); };
            SaveMenuItem.Click += (_, __) => { SaveFile(ActiveClipEditor()); };
            SaveAsMenuItem.Click += (_, __) => { SaveFileAs(ActiveClipEditor()); };
            CloseMenuItem.Click += (_, __) => { Close(false); };
            CloseAllMenuItem.Click += (_, __) => { Close(true); };
            ExitMenuItem.Click += (_, __) => { Close(true); };
            menuStrip.MenuActivate += (_, __) => { UpdateMenu(); };
            FileMenuItem.DropDownOpening += Recent_DropDownOpening;
            ftree.FileSelectedEvent += (object? sender, string fn) => { OpenFile(fn); };

            // Tools.
            AboutMenuItem.Click += (_, __) => { MiscUtils.ShowReadme("Wavicler"); };
            SettingsMenuItem.Click += (_, __) => { EditSettings(); };

            UpdateMenu();

            Text = $"Wavicler {MiscUtils.GetVersionString()}";
        }

        /// <summary>
        /// Form is legal now. Init things that want to log.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            _logger.Info($"OK to log now!!");

            if (!_player.Valid)
            {
                var s = $"Something wrong with your audio output device:{_settings.AudioSettings.WavOutDevice}";
                _logger.Error(s);
                UpdateState(AppState.Dead);
            }

            // Initialize tree from user settings.
            InitNavigator();

            // TODO Debugging >>>>>>>>>>>>>>>>>>>
//            OpenFile(@"C:\Dev\repos\TestAudioFiles\Cave Ceremony 01.wav");
            //OpenFile(@"C:\Dev\repos\TestAudioFiles\ref-stereo.wav");

            // Internal test stuff.
            //int max = int.MaxValue;
            //int smplPerSec = 441000;
            //int sec = max / smplPerSec;
            //TimeSpan tmax = new TimeSpan(0, 0, sec);

            //var bb = Utils.SampleToBarBeat(1000000, 100); // 9, 1
            //var smpl = Utils.BarBeatToSample(20, 3, 100); // 2196179

            base.OnLoad(e);
        }

        /// <summary>
        /// Bye-bye.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Close(true);
            LogManager.Stop();
            UpdateState(AppState.Stop);
            SaveSettings();
            base.OnFormClosing(e);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            // My stuff here.
            _player.Run(false);
            _player.Dispose();

            base.Dispose(disposing);
        }
        #endregion

        #region Navigator
        /// <summary>
        /// Initialize tree from user settings.
        /// </summary>
        void InitNavigator() //TODO probably don't need this after all.
        {
            var s = AudioLibDefs.AUDIO_FILE_TYPES;
            ftree.FilterExts = s.SplitByTokens("|;*");
            ftree.RootDirs = _settings.RootDirs;
            ftree.SingleClickSelect = true;

            try
            {
                ftree.Init();
                ftree.Invalidate();
            }
            catch (DirectoryNotFoundException)
            {
                _logger.Warn("No tree directories");
            }
        }
        #endregion

        #region State management
        /// <summary>
        /// General state management. Everything goes through here.
        /// </summary>
        void UpdateState(AppState newState)
        {
            // Unhook.
            btnPlay.CheckedChanged -= ChkPlay_CheckedChanged;

            if (newState != _currentState)
            {
                _logger.Info($"State change:{newState}");
            }

            switch (newState)
            {
                case AppState.Complete:
                    Rewind();
                    if (_settings.Loop)
                    {
                        Play();
                    }
                    else
                    {
                        Stop();
                    }
                    break;

                case AppState.Play: //TODO1 support play selection or all
                    Play();
                    break;

                case AppState.Stop:
                    Stop();
                    break;

                case AppState.Rewind:
                    Rewind();
                    break;

                case AppState.Dead:
                    Stop();
                    break;
            }

            _currentState = newState;

            // Rehook.
            btnPlay.CheckedChanged += ChkPlay_CheckedChanged;

            // Local funcs
            void Play()
            {
                btnPlay.Checked = true;
                _player.Run(true);
            }

            void Stop()
            {
                btnPlay.Checked = false;
                _player.Run(false);
            }

            void Rewind()
            {
                _waveOutSwapper.Rewind();
                
                //_player.Rewind();

                //timeBar.Current = TimeSpan.Zero;
            }
        }

        /// <summary>
        /// Play button handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ChkPlay_CheckedChanged(object? sender, EventArgs e)
        {
            UpdateState(btnPlay.Checked ? AppState.Play : AppState.Stop);
        }

        /// <summary>
        /// Do some global key handling. Space bar is used for stop/start playing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Space:
                    // Toggle.
                    UpdateState(btnPlay.Checked ? AppState.Stop : AppState.Play);
                    e.Handled = true;
                    break;
            }
        }
        #endregion

        #region Menu management
        /// <summary>
        /// Organize the file menu item drop down.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Recent_DropDownOpening(object? sender, EventArgs e)
        {
            RecentMenuItem.DropDownItems.Clear();

            _settings.RecentFiles.ForEach(f =>
            {
                ToolStripMenuItem menuItem = new(f);
                menuItem.Click += (object? sender, EventArgs e) =>
                {
                    string fn = sender!.ToString()!;
                    OpenFile(fn);
                };

                RecentMenuItem.DropDownItems.Add(menuItem);
            });
        }

        /// <summary>
        /// Set menu item enables according to system states. TODO tweak later.
        /// </summary>
        void UpdateMenu()
        {
            bool anyOpen = false;
            bool dirty = false;
            bool hasClip = false;

            var cled = ActiveClipEditor();
            if (cled is not null)
            {
                anyOpen = true;
                dirty = cled.Dirty;
            }

            btnRewind.Enabled = anyOpen;
            btnPlay.Enabled = anyOpen;
            sldVolume.Enabled = true;
            OpenMenuItem.Enabled = true;
            SaveMenuItem.Enabled = dirty;
            SaveAsMenuItem.Enabled = dirty;
            CloseMenuItem.Enabled = anyOpen;
            CloseAllMenuItem.Enabled = anyOpen;
            ExitMenuItem.Enabled = true;
        }
        #endregion

        #region File I/O
        /// <summary>
        /// Common file opener.
        /// </summary>
        /// <param name="fn">The file to open or create new if empty.</param>
        /// <returns>Status.</returns>
        bool OpenFile(string fn = "")
        {
            bool ok = false;
            UpdateState(AppState.Stop);
            _logger.Info($"Opening file: {fn}");

            try
            {
                if (fn == "")
                {
                    _logger.Info($"Creating new tab");
                    ClipSampleProvider prov = new(Array.Empty<float>());
                    CreateTab(prov, "*");
                    ok = true;
                }
                else
                {
                    if (!File.Exists(fn))
                    {
                        throw new InvalidOperationException($"Invalid file.");
                    }

                    var ext = Path.GetExtension(fn).ToLower();
                    var baseFn = Path.GetFileName(fn);

                    if (!AudioLibDefs.AUDIO_FILE_TYPES.Contains(ext))
                    {
                        throw new InvalidOperationException($"Invalid file type.");
                    }

                    using (new WaitCursor())
                    {
                        _logger.Info($"Opening file: {fn}");

                        // Find out about the requested file.
                        using var reader = new AudioFileReader(fn);
                        reader.Validate(false);

                        // Make tab control(s) for our data.
                        if (reader.WaveFormat.Channels == 2)
                        {
                            CreateTab(new ClipSampleProvider(fn, StereoCoercion.Left), baseFn + " LEFT");
                            CreateTab(new ClipSampleProvider(fn, StereoCoercion.Right), baseFn + " RIGHT");
                        }
                        else
                        {
                            CreateTab(new ClipSampleProvider(reader, StereoCoercion.Mono), baseFn);
                        }

                        _settings.RecentFiles.UpdateMru(fn);
                        ok = true;

                        if (_settings.Autoplay)
                        {
                            UpdateState(AppState.Rewind);
                            UpdateState(AppState.Play);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Couldn't open the file: {fn} because: {ex.Message}");
                ok = false;
            }

            btnPlay.Enabled = ok;
            UpdateMenu();

            return ok;
        }

        /// <summary>
        /// Allows the user to select an audio clip or midi from file system.
        /// </summary>
        void Open_Click()
        {
            var fileTypes = $"Audio Files|{AudioLibDefs.AUDIO_FILE_TYPES}";
            using OpenFileDialog openDlg = new()
            {
                Filter = fileTypes,
                Title = "Select a file"
            };

            if (openDlg.ShowDialog() == DialogResult.OK)
            {
                OpenFile(openDlg.FileName);
            }
        }

        /// <summary>
        /// Common file saver.
        /// </summary>
        /// <param name="cled">Data source.</param>
        /// <param name="fn">The file to save to.</param>
        /// <returns>Status.</returns>
        bool SaveFile(ClipEditor? cled, string fn = "")
        {
            bool ok = false;

            if (cled is not null)
            {
                // TODO get all rendered data and save to audio file - to fn if specified else old.
            }

            return ok;
        }

        /// <summary>
        /// Save the file in the current page.
        /// </summary>
        /// <param name="cled">Data source.</param>
        void SaveFileAs(ClipEditor? cled)
        {
            if (cled is not null)
            {
                using SaveFileDialog saveDlg = new()
                {
                    Filter = AudioLibDefs.AUDIO_FILE_TYPES,
                    Title = "Save as file"
                };

                if (saveDlg.ShowDialog() == DialogResult.OK)
                {
                    SaveFile(cled, saveDlg.FileName);
                }
            }
        }

        /// <summary>
        /// General closer/saver.
        /// </summary>
        /// <param name="all">Close all if true otherwise just the current selected.</param>
        /// <returns></returns>
        bool Close(bool all)
        {
            bool ok = true;

            if (all)
            {
                while(tabControl.TabCount > 0)
                {
                    CloseOne(tabControl.TabPages[0]);
                }
            }
            else if (tabControl.TabCount > 0)
            {
                CloseOne(tabControl.SelectedTab);
            }

            // Local function.
            void CloseOne(TabPage tpg)
            {
                var cled = tpg.Controls[0] as ClipEditor;
                if (cled.Dirty)
                {
                    // TODO ask to save.
                }

                cled.Dispose();
                tabControl.TabPages.Remove(tpg);
                tpg.Dispose();
            }

            UpdateMenu();

            return ok;
        }
        #endregion

        #region Settings
        /// <summary>
        /// Edit user settings.
        /// </summary>
        void EditSettings()
        {
            var changes = _settings.Edit("User Settings", 450);

            // TODO Check for meaningful changes.

        }

        /// <summary>
        /// Collect and save user settings.
        /// </summary>
        void SaveSettings()
        {
            _settings.FormGeometry = new Rectangle(Location.X, Location.Y, Width, Height);
            _settings.Save();
        }
        #endregion

        #region Misc stuff

        #endregion


        /// <summary>
        /// Helper function.
        /// </summary>
        /// <returns></returns>
        ClipEditor? ActiveClipEditor()
        {
            ClipEditor? cled = tabControl.TabPages.Count > 0 ? tabControl.SelectedTab.Controls[0] as ClipEditor : null;
            return cled;
        }



        void ResampleMenuItem_Click(object sender, EventArgs e)
        {
            //TODO get two file names and execute.

        }

        void StereoSplitMenuItem_Click(object sender, EventArgs e)
        {
            //TODO get one file name and execute.

        }

        void StereoToMonoMenuItem_Click(object sender, EventArgs e)
        {
            //TODO get one file name and execute.

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ClipEditor_ServiceRequest(object? sender, ClipEditor.ServiceRequestEventArgs e)
        {
            var cled = sender as ClipEditor;

            switch(e.Request)
            {
                case ClipEditor.ServiceRequest.Close:
                    Close(false);
                    break;

                case ClipEditor.ServiceRequest.CopySelectionToNewClip: // TODO
                    break;
            }
        }

        /// <summary>
        /// Function to open a new tab.
        /// </summary>
        /// <param name="prov"></param>
        /// <param name="tabName"></param>
        void CreateTab(ClipSampleProvider prov, string tabName)
        {
            ClipEditor ed = new(prov)
            {
                Dock = DockStyle.Fill,
                DrawColor = _settings.ControlColor,
                GridColor = Color.LightGray
            };
            ed.ServiceRequestEvent += ClipEditor_ServiceRequest;
            
            TabPage page = new() { Text = tabName };
            page.Controls.Add(ed);
            tabControl.TabPages.Add(page);
            tabControl.SelectedTab = page;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TabControl_SelectedIndexChanged(object? sender, EventArgs e)
        {
            var cled = ActiveClipEditor();
            if (cled is not null)
            {
                _waveOutSwapper.SetInput(cled.SelectionSampleProvider);
                statusInfo.Text = cled.SelectionSampleProvider.GetInfoString();
            }
            else
            {
                _waveOutSwapper.SetInput(null);
                statusInfo.Text = "";
            }
        }
    }
}
