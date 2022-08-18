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
using AudioLib;


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

        /// <summary>TODO loop?</summary>
        bool _loop = false;

        /// <summary>TODO kludgy?</summary>
        readonly MainToolbar MT = new();
        #endregion

        #region Lifecycle
        public MainForm()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);

            // Must do this first before initializing.
            string appDir = MiscUtils.GetAppDataDir("Wavicler", "Ephemera");
            Common.TheSettings = (UserSettings)Settings.Load(appDir, typeof(UserSettings));
            // Tell the libs about their settings.
            AudioSettings.LibSettings = Common.TheSettings.AudioSettings;

            // Build it.
            InitializeComponent();
            Icon = Properties.Resources.tiger;
            ToolStrip.Items.Add(new ToolStripControlHost(MT));

            // Init logging.
            LogManager.MinLevelFile = Common.TheSettings.FileLogLevel;
            LogManager.MinLevelNotif = Common.TheSettings.NotifLogLevel;
            LogManager.LogEvent += LogManager_LogEvent;
            LogManager.Run();

            // Init main form from settings
            WindowState = FormWindowState.Normal;
            StartPosition = FormStartPosition.Manual;
            Location = new Point(Common.TheSettings.FormGeometry.X, Common.TheSettings.FormGeometry.Y);
            Size = new Size(Common.TheSettings.FormGeometry.Width, Common.TheSettings.FormGeometry.Height);
            KeyPreview = true; // for routing kbd strokes through OnKeyDown

            // The text output. TODO Maybe use OARS style?
            MT.txtInfo.Font = Font;
            MT.txtInfo.WordWrap = true;
            MT.txtInfo.MatchColors.Add("ERR", Color.LightPink);
            MT.txtInfo.MatchColors.Add("WRN", Color.Plum);

            // Create output.
            _player = new(Common.TheSettings.AudioSettings.WavOutDevice, int.Parse(Common.TheSettings.AudioSettings.Latency), _waveOutSwapper);
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

            MT.btnRewind.Click += (_, __) => { UpdateState(AppState.Rewind); };
            MT.chkPlay.Click += (_, __) => { UpdateState(MT.chkPlay.Checked ? AppState.Play : AppState.Stop); };
            MT.volumeMaster.ValueChanged += (_, __) => { _player.Volume = (float)MT.volumeMaster.Value; };
            MT.volumeMaster.Minimum = AudioLibDefs.VOLUME_MIN;
            MT.volumeMaster.Maximum = AudioLibDefs.VOLUME_MAX;
            MT.volumeMaster.Value = Common.TheSettings.Volume;

            MT.txtBPM.KeyPress += (object? sender, KeyPressEventArgs e) => { KeyUtils.TestForNumber_KeyPress(sender!, e); };

            // Managing files. FileMenuItem
            FileMenuItem.DropDownOpening += Recent_DropDownOpening;
            NewMenuItem.Click += (_, __) => { OpenFile(); };
            OpenMenuItem.Click += (_, __) => { Open_Click(); };
            SaveMenuItem.Click += (_, __) => { SaveFile(ActiveEditor()); };
            SaveAsMenuItem.Click += (_, __) => { SaveFileAs(ActiveEditor()); };
            CloseMenuItem.Click += (_, __) => { Close(false); };
            CloseAllMenuItem.Click += (_, __) => { Close(true); };
            ExitMenuItem.Click += (_, __) => { Close(true); };

            // Editing. EditMenuItem
            CutMenuItem.Click += (_, __) => { Cut(); };
            CopyMenuItem.Click += (_, __) => { Copy(); };
            PasteMenuItem.Click += (_, __) => { Paste(); };
            ReplaceMenuItem.Click += (_, __) => { Replace(); };
            // RemoveEnvelopeMenuItem

            // Tools. ToolsMenuItem
            AboutMenuItem.Click += (_, __) => { MiscUtils.ShowReadme("Wavicler"); };
            SettingsMenuItem.Click += (_, __) => { EditSettings(); };
            // BpmMenuItem

            // Misc events.
            Menu.MenuActivate += (_, __) => { UpdateMenu(); };

            UpdateMenu();

            Text = $"Wavicler {MiscUtils.GetVersionString()}";

            // Debugging >>>>>>>>>>>>>>>>>>>
            OpenFile(@"C:\Dev\repos\TestAudioFiles\Cave Ceremony 01.wav");
            //OpenFile(@"C:\Dev\repos\TestAudioFiles\ref-stereo.wav");
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
                var s = $"Something wrong with your audio output device:{Common.TheSettings.AudioSettings.WavOutDevice}";
                _logger.Error(s);
                UpdateState(AppState.Dead);
            }
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
                    UpdateState(MT.chkPlay.Checked ? AppState.Stop : AppState.Play);
                    e.Handled = true;
                    break;
            }
        }




        #region State management
        /// <summary>
        /// General state management. Everything goes through here.
        /// </summary>
        void UpdateState(AppState newState)
        {
            // Unhook.
            MT.chkPlay.CheckedChanged -= ChkPlay_CheckedChanged;

            if (newState != _currentState)
            {
                _logger.Info($"State change:{newState}");
            }

            switch (newState)
            {
                case AppState.Complete:
                    Rewind();
                    if (_loop)
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
            MT.chkPlay.CheckedChanged += ChkPlay_CheckedChanged;

            // Local funcs
            void Play()
            {
                MT.chkPlay.Checked = true;
                _player.Run(true);
            }

            void Stop()
            {
                MT.chkPlay.Checked = false;
                _player.Run(false);
            }

            void Rewind()
            {
                //if ( is not null)
                //{
                //    _reader.Position = 0;
                //}
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
            UpdateState(MT.chkPlay.Checked ? AppState.Play : AppState.Stop);
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

            Common.TheSettings.RecentFiles.ForEach(f =>
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
        /// Set menu item enables according to system states.
        /// </summary>
        void UpdateMenu()
        {
            bool anyOpen = false;
            bool dirty = false;
            bool hasClip = false; // TODO

            var child = ActiveEditor();
            if (child is ClipEditor)
            {
                anyOpen = true;
                dirty = child.Dirty;
            }

            MT.btnRewind.Enabled = anyOpen;
            MT.chkPlay.Enabled = anyOpen;
            MT.volumeMaster.Enabled = true;
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

        #region Cut/copy/paste TODO
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
                _logger.Info($"Creating new child");
                ClipSampleProvider prov = new(Array.Empty<float>());
                ClipEditor childNew = new(prov);
                childNew.Show();
                ok = true;
            }
            else
            {
                var ext = Path.GetExtension(fn).ToLower();
                if (!File.Exists(fn))
                {
                    _logger.Error($"Invalid file: {fn}");
                }
                else if (AudioLibDefs.AUDIO_FILE_TYPES.Contains(ext))
                {
                    _logger.Info($"Opening file: {fn}");

                    // Read all data.
                    var reader = new AudioFileReader(fn);
                    // If it doesn't match, create a resampled temp file.
                    if (reader.WaveFormat.SampleRate != AudioLibDefs.SAMPLE_RATE)
                    {
                        reader = reader.Resample();
                    }
                    reader.Validate(false);

                    //long len = reader.Length / (reader.WaveFormat.BitsPerSample / 8);

                    // Make buffers for our data.
                    if (reader.WaveFormat.Channels == 2) // stereo interleaved
                    {
                        var provL = new ClipSampleProvider(fn, StereoCoercion.Left);
                        ClipEditor childL = new(provL);
                        childL.MdiParent = this;
                        childL.Show();

                        var provR = new ClipSampleProvider(fn, StereoCoercion.Right);
                        ClipEditor childR = new(provR);
                        childR.MdiParent = this;
                        childR.Show();
                    }
                    else
                    {
                        var provM = new ClipSampleProvider(reader, StereoCoercion.Mono);
                        ClipEditor childM = new(provM);
                        childM.MdiParent = this;
                        childM.Show();
                    }
                    ok = true;
                }
                else
                {
                    _logger.Error($"Unsupported file type: {fn}");
                }
            }

            if (ok && Common.TheSettings.Autoplay)
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
        /// <param name="ed">Data source.</param>
        /// <param name="fn">The file to save to.</param>
        /// <returns>Status.</returns>
        bool SaveFile(ClipEditor? ed, string fn = "")
        {
            bool ok = false;

            if (ed is not null)
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
        /// Save the file in the current child.
        /// </summary>
        /// <param name="child">Data source.</param>
        void SaveFileAs(ClipEditor? child)
        {
            if (child is ClipEditor)
            {
                using SaveFileDialog saveDlg = new()
                {
                    Filter = AudioLibDefs.AUDIO_FILE_TYPES,
                    Title = "Save as file"
                };

                if (saveDlg.ShowDialog() == DialogResult.OK)
                {
                    SaveFile(child, saveDlg.FileName);
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
                MdiChildren.Where(ch => ch is ClipEditor).ForEach(ch =>
                {
                    CloseOne(ch as ClipEditor);
                });
            }
            else
            {
                var ed = ActiveEditor();
                if (ed is not null)
                {
                    CloseOne(ed);
                }
            }

            void CloseOne(ClipEditor ed)
            {
                if (ed.Dirty)
                {
                    // TODO ask to save. If yes, save()

                }

                ed.Close();
                ed.Dispose();
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
            var changes = Common.TheSettings.Edit("User Settings", 300);

            // Check for meaningful changes.
            //timeBar.SnapMsec = Common.TheSettings.AudioSettings.SnapMsec;
        }

        /// <summary>
        /// Collect and save user settings.
        /// </summary>
        void SaveSettings()
        {
            Common.TheSettings.FormGeometry = new Rectangle(Location.X, Location.Y, Width, Height);
            Common.TheSettings.Volume = MT.volumeMaster.Value;
            //Common.Settings.Autoplay = btnAutoplay.Checked;
            //Common.Settings.Loop = btnLoop.Checked;
            Common.TheSettings.Save();
        }
        #endregion

        #region Misc stuff
        /// <summary>
        /// Get current focus editor.
        /// </summary>
        /// <returns>The editor or null if invalid.</returns>
        ClipEditor? ActiveEditor()
        {
            ClipEditor? ret = null;

            // Get the form with focus.
            var child = ActiveMdiChild;
            if (child is ClipEditor)
            {
                ret = child as ClipEditor;
            }

            return ret;
        }

        /// <summary>
        /// Show log events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LogManager_LogEvent(object? sender, LogEventArgs e)
        {
            // Usually come from a different thread.
            if (IsHandleCreated)
            {
                this.InvokeIfRequired(_ => { MT.txtInfo.AppendLine($"{e.Message}"); });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MainForm_MdiChildActivate(object sender, EventArgs e)
        {
            var child = ActiveMdiChild;
            
            _logger.Info($"MDI child:{(child is null ? "None" : child.Text)}");
        }

        /// <summary>
        /// Internal stuff.
        /// </summary>
        void DoCalcs()
        {
            // Test stuff.
            int max = int.MaxValue;
            int smplPerSec = 441000;
            int sec = max / smplPerSec;
            TimeSpan tmax = new TimeSpan(0, 0, sec);


        }
        #endregion
    }
}
