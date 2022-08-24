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


//snap on/off
//public float Snap;
//public enum SelectionMode { Sample, BarBeat, Time };


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

        /// <summary>Log for the user.</summary>
        TextViewer _tvLog;

        /// <summary>The settings.</summary>
        UserSettings _settings;
        #endregion

        #region Lifecycle
        public MainForm()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);

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
            LogManager.LogEvent += (object? sender, LogEventArgs e) => { this.InvokeIfRequired(_ => { _tvLog.AppendLine($"{e.Message}"); }); };
            LogManager.Run();

            // Set up paths.
            _outPath = Path.Combine(appDir, "out");
            DirectoryInfo di = new(_outPath);
            di.Create();

            // Init main form from settings
            WindowState = FormWindowState.Normal;
            StartPosition = FormStartPosition.Manual;
            Location = new Point(_settings.FormGeometry.X, _settings.FormGeometry.Y);
            Size = new Size(_settings.FormGeometry.Width, _settings.FormGeometry.Height);
            KeyPreview = true; // for routing kbd strokes through OnKeyDown

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
            var postVolumeMeter = new MeteringSampleProvider(_waveOutSwapper, _waveOutSwapper.WaveFormat.SampleRate / 10);
            postVolumeMeter.StreamVolume += (object? sender, StreamVolumeEventArgs e) =>
            {
                // TODO timeBar.Current = _reader.CurrentTime;
            };
            _waveOutSwapper.SetInput(postVolumeMeter);

            // Tab pages. Make one into a log view.
            tabControl.TabPages.Clear();
            _tvLog = new TextViewer
            {
                MaxText = 5000,
                Prompt = "> ",
                Font = Font,
                WordWrap = true,
                Dock = DockStyle.Fill
            };
            _tvLog.MatchColors.Add("ERR", Color.LightPink);
            _tvLog.MatchColors.Add("WRN:", Color.Plum);

            TabPage tpg = new() { Text = "Log" };
            tpg.Controls.Add(_tvLog);
            tabControl.Controls.Add(tpg);
            tabControl.SelectedTab = tpg;

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

            sldBPM.DrawColor = _settings.ControlColor;
            sldBPM.Value = _settings.BPM;
            sldBPM.ValueChanged += (_, __) => { _settings.BPM = sldBPM.Value; };

            cmbSelMode.Items.Add(SelectionMode.Sample);
            cmbSelMode.Items.Add(SelectionMode.BarBeat);
            cmbSelMode.Items.Add(SelectionMode.Time);
            cmbSelMode.SelectedItem = _settings.SelectionMode;
            cmbSelMode.SelectedIndexChanged += (_, __) => { _settings.SelectionMode = (SelectionMode)cmbSelMode.SelectedItem; };

            btnRewind.Click += (_, __) => { UpdateState(AppState.Rewind); };
            btnPlay.Click += (_, __) => { UpdateState(btnPlay.Checked ? AppState.Play : AppState.Stop); };

            // File handling.
            NewMenuItem.Click += (_, __) => { OpenFile(); };
            OpenMenuItem.Click += (_, __) => { Open_Click(); };
            SaveMenuItem.Click += (_, __) => { SaveFile(tabControl.SelectedTab); };
            SaveAsMenuItem.Click += (_, __) => { SaveFileAs(tabControl.SelectedTab); };
            CloseMenuItem.Click += (_, __) => { Close(false); };
            CloseAllMenuItem.Click += (_, __) => { Close(true); };
            ExitMenuItem.Click += (_, __) => { Close(true); };
            menuStrip.MenuActivate += (_, __) => { UpdateMenu(); };
            FileMenuItem.DropDownOpening += Recent_DropDownOpening;
            ftree.FileSelectedEvent += (object? sender, string fn) => { OpenFile(fn); };

            // Edit menu.
            CutMenuItem.Click += (_, __) => { Cut(); };
            CopyMenuItem.Click += (_, __) => { Copy(); };
            PasteMenuItem.Click += (_, __) => { Paste(); };
            ReplaceMenuItem.Click += (_, __) => { Replace(); };

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
            OpenFile(@"C:\Dev\repos\TestAudioFiles\Cave Ceremony 01.wav");
            //OpenFile(@"C:\Dev\repos\TestAudioFiles\ref-stereo.wav");

            // Internal test stuff.
            //int max = int.MaxValue;
            //int smplPerSec = 441000;
            //int sec = max / smplPerSec;
            //TimeSpan tmax = new TimeSpan(0, 0, sec);

            //var bb = Utils.SampleToBarBeat(1000000, 100); // 9, 1
            //var smpl = Utils.BarBeatToSample(20, 3, 100); // 2196179
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
        void InitNavigator()
        {
            var s = AudioLibDefs.AUDIO_FILE_TYPES;
            ftree.FilterExts = s.SplitByTokens("|;*");
            ftree.RootDirs = _settings.RootDirs;
            ftree.SingleClickSelect = true;

            try
            {
                ftree.Init();
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

                case AppState.Play:
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
                _player.Rewind();
//                timeBar.Current = TimeSpan.Zero;
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

            var tpg = tabControl.SelectedTab;
            if (tpg.Controls[0] is ClipEditor)
            {
                var ed = tpg.Controls[0] as ClipEditor;
                anyOpen = true;
                dirty = ed.Dirty;
            }

            btnRewind.Enabled = anyOpen;
            btnPlay.Enabled = anyOpen;
            sldVolume.Enabled = true;
            NewMenuItem.Enabled = true;
            OpenMenuItem.Enabled = true;
            SaveMenuItem.Enabled = dirty;
            SaveAsMenuItem.Enabled = dirty;
            CloseMenuItem.Enabled = anyOpen;
            CloseAllMenuItem.Enabled = anyOpen;
            ExitMenuItem.Enabled = true;
            CutMenuItem.Enabled = anyOpen;
            CopyMenuItem.Enabled = anyOpen;
            PasteMenuItem.Enabled = hasClip;
            ReplaceMenuItem.Enabled = anyOpen && hasClip;
        }
        #endregion

        #region Cut/copy/paste TODO??
        void Cut()
        {

        }

        void Copy()
        {

        }

        void Paste()
        {

        }

        void Replace()
        {

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

            if (fn == "")
            {
                _logger.Info($"Creating new tab");
                ClipSampleProvider prov = new(Array.Empty<float>());
                CreateTab(prov, "*");
                ok = true;
            }
            else
            {
                var ext = Path.GetExtension(fn).ToLower();
                var baseFn = Path.GetFileName(fn);

                if (!File.Exists(fn))
                {
                    _logger.Error($"Invalid file: {fn}");
                }
                else if (AudioLibDefs.AUDIO_FILE_TYPES.Contains(ext))
                {
                    _logger.Info($"Opening file: {fn}");

                    // If sample rate doesn't match, create a resampled temp file.
                    var reader = new AudioFileReader(fn);
                    if (reader.WaveFormat.SampleRate != AudioLibDefs.SAMPLE_RATE)
                    {
                        reader = reader.Resample();
                    }
                    reader.Validate(false);

                    // Make controls for our data. Add to tab.
                    if (reader.WaveFormat.Channels == 2) // stereo interleaved
                    {
                        CreateTab(new ClipSampleProvider(fn, StereoCoercion.Left), baseFn + " LEFT");
                        CreateTab(new ClipSampleProvider(fn, StereoCoercion.Right), baseFn + " RIGHT");
                    }
                    else
                    {
                        CreateTab(new ClipSampleProvider(reader, StereoCoercion.Mono), baseFn);
                    }

                    ok = true;
                }
                else
                {
                    _logger.Error($"Unsupported file type: {fn}");
                }
            }

            if (ok && _settings.Autoplay)
            {
                UpdateState(AppState.Rewind);
                UpdateState(AppState.Play);
            }

            UpdateMenu();

            return ok;
        }

        /// <summary>
        /// Common file saver.
        /// </summary>
        /// <param name="tpg">Data source.</param>
        /// <param name="fn">The file to save to.</param>
        /// <returns>Status.</returns>
        bool SaveFile(TabPage tpg, string fn = "")
        {
            bool ok = false;

            if (tpg.Controls[0] is ClipEditor)
            {
                // TODO get all rendered data and save to audio file - to fn if specified else old.

            }

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
        /// Save the file in the current page.
        /// </summary>
        /// <param name="tpg">The page.</param>
        void SaveFileAs(TabPage tpg)
        {
            if (tpg.Controls[0] is ClipEditor)
            {
                using SaveFileDialog saveDlg = new()
                {
                    Filter = AudioLibDefs.AUDIO_FILE_TYPES,
                    Title = "Save as file"
                };

                if (saveDlg.ShowDialog() == DialogResult.OK)
                {
                    SaveFile(tpg, saveDlg.FileName);
                }
            }
        }

        /// <summary>
        /// General closer/saver.
        /// </summary>
        /// <param name="all"></param>
        /// <returns></returns>
        bool Close(bool all)
        {
            bool ok = true;

            if (all)
            {
                for (int i = 0; i < tabControl.TabCount; i++)
                {
                    var tpg = tabControl.TabPages[i];
                    if (tpg.Controls[0] is ClipEditor)
                    {
                        CloseOne(tpg);
                    }
                }
            }
            else
            {
                CloseOne(tabControl.SelectedTab);
            }

            // Local function.
            void CloseOne(TabPage tpg)
            {
                if (tpg.Controls[0] is ClipEditor)
                {
                    var ed = tpg.Controls[0] as ClipEditor;
                    if (ed.Dirty)
                    {
                        // TODO ask to save.
                    }

                    ed.Dispose();
                    tabControl.TabPages.Remove(tpg);
                    tpg.Dispose();
                }
                // else leave alone
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
            var changes = _settings.Edit("User Settings", 300);

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

            TabPage tpg = new()
            {
                Text = tabName
            };
            tpg.Controls.Add(ed);
            tabControl.Controls.Add(tpg);
            tabControl.SelectedTab = tpg;
        }
        #endregion
    }
}
