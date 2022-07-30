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
using NAudio.Wave;
using AudioLib;
using NBagOfTricks;
using NBagOfTricks.Slog;
using NBagOfUis;
using NAudio.Wave.SampleProviders;

namespace Wavicler
{
    public partial class MainForm : Form
    {
        #region Types
        /// <summary>What are we doing.</summary>
        public enum AppState { Stop, Play, Rewind, Complete, Dead }
        #endregion

        #region Fields
        /// <summary>My logger.</summary>
        readonly Logger _logger = LogManager.CreateLogger("MainForm");

        /// <summary>The actual player.</summary>
        readonly AudioPlayer _player;

        /// <summary>Where we be.</summary>
        AppState _currentState = AppState.Stop;
        #endregion

        bool _loop = false;//TODO
        MainToolbar MT = new();

        // StereoToMonoSampleProvider to split stereo into 2 mono.

        #region Lifecycle
        public MainForm()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);

            // Must do this first before initializing.
            string appDir = MiscUtils.GetAppDataDir("Wavicler", "Ephemera");
            UserSettings.TheSettings = (UserSettings)Settings.Load(appDir, typeof(UserSettings));
            // Tell the libs about their settings.
            AudioSettings.LibSettings = UserSettings.TheSettings.AudioSettings;

            InitializeComponent();
            // This took way too long to find out:
            //https://stackoverflow.com/questions/12823400/statusstrip-hosting-a-usercontrol-fails-to-call-usercontrols-onpaint-event
            toolStrip1.Items.Add(new ToolStripControlHost(MT));
            //ttt.button1.Click += Button1_Click;

            Icon = Properties.Resources.tiger;

            // Init logging.
            LogManager.MinLevelFile = UserSettings.TheSettings.FileLogLevel;
            LogManager.MinLevelNotif = UserSettings.TheSettings.NotifLogLevel;
            LogManager.LogEvent += LogManager_LogEvent;
            LogManager.Run();

            // Init main form from settings
            WindowState = FormWindowState.Normal;
            StartPosition = FormStartPosition.Manual;
            Location = new Point(UserSettings.TheSettings.FormGeometry.X, UserSettings.TheSettings.FormGeometry.Y);
            Size = new Size(UserSettings.TheSettings.FormGeometry.Width, UserSettings.TheSettings.FormGeometry.Height);
            KeyPreview = true; // for routing kbd strokes through OnKeyDown

            // The text output. Maybe use OARS style?
            MT.txtInfo.Font = Font;
            MT.txtInfo.WordWrap = true;
            MT.txtInfo.MatchColors.Add("ERR", Color.LightPink);
            MT.txtInfo.MatchColors.Add("WRN", Color.Plum);

            _player = new(UserSettings.TheSettings.AudioSettings.WavOutDevice, int.Parse(UserSettings.TheSettings.AudioSettings.Latency));
            _player.PlaybackStopped += Player_PlaybackStopped;

            MT.btnRewind.Click += (_, __) => { UpdateState(AppState.Rewind); };
            MT.chkPlay.Click += (_, __) => { UpdateState(MT.chkPlay.Checked ? AppState.Play : AppState.Stop); };
            MT.volumeMaster.ValueChanged += (_, __) => { _player.Volume = (float)MT.volumeMaster.Value; };

            // TODO
            // mainToolbar.btnSettings.Click += (_, __) => { EditSettings(); };
            // mainToolbar.ddFile.DropDownOpening += File_DropDownOpening;


            // Create output.
            // (IWaveProvider waveProvider, bool forceStereo)
            //var sampleChannel = new SampleChannel(_reader, false);
            //sampleChannel.PreVolumeMeter += SampleChannel_PreVolumeMeter;
            // (ch, smpls per notif)
            //var postVolumeMeter = new MeteringSampleProvider(sampleChannel, _reader.WaveFormat.SampleRate);

            //// TODO
            //var postVolumeMeter = new MeteringSampleProvider(_reader, _reader.WaveFormat.SampleRate / 10);
            //postVolumeMeter.StreamVolume += PostVolumeMeter_StreamVolume;
            //_player.Init(postVolumeMeter);

            Text = $"Wavicler {MiscUtils.GetVersionString()}";

            // >>>>>>>>>>>>>>>>>>>
            OpenFile(@"C:\Dev\repos\TestAudioFiles\Cave Ceremony 01.wav");
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
                var s = $"Something wrong with your audio output device:{UserSettings.TheSettings.AudioSettings.WavOutDevice}";
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
        /// Usually end of file but could be error.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Player_PlaybackStopped(object? sender, StoppedEventArgs e)
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
        }


        //When an MDI child form has a MainMenu component(with, usually, a menu structure of menu items) and it is
        //opened within an MDI parent form that has a MainMenu component(with, usually, a menu structure of menu items),
        //the menu items will merge automatically if you have set the MergeType property(and optionally, the MergeOrder
        //property). Set the MergeType property of both MainMenu components and all of the menu items of the child form
        //to MergeItems.Additionally, set the MergeOrder property so that the menu items from both menus appear in the
        //desired order.Moreover, keep in mind that when you close an MDI parent form, each of the MDI child forms raises
        //a Closing event before the Closing event for the MDI parent is raised. Canceling an MDI child's Closing event
        //will not prevent the MDI parent's Closing event from being raised; however, the CancelEventArgs argument for
        //the MDI parent's Closing event will now be set to true. You can force the MDI parent and all MDI child forms
        //to close by setting the CancelEventArgs argument to false.
        void NewFile_Click(object sender, EventArgs e)
        {
            WaveForm child = new();
            child.MdiParent = this;
            child.Show();
        }


        #region File management
        /// <summary>
        /// Common file opener.
        /// </summary>
        /// <param name="fn">The file to open.</param>
        /// <returns>Status.</returns>
        public bool OpenFile(string fn)
        {
            bool ok = true;

            UpdateState(AppState.Stop);

            _logger.Info($"Opening file: {fn}");

            var ext = Path.GetExtension(fn).ToLower();
            if (AudioLibDefs.AUDIO_FILE_TYPES.Contains(ext))
            {
                // TODO Create a new child form. Give it file name.
            }
            else
            {
                _logger.Error($"Unsupported file type: {fn}");
                //_reader?.Dispose();
                //_reader = null;
                ok = false;
            }

            if (ok)
            {
                if (UserSettings.TheSettings.Autoplay)
                {
                    UpdateState(AppState.Rewind);
                    UpdateState(AppState.Play);
                }
            }

            MT.chkPlay.Enabled = ok;
            return ok;
        }

        /// <summary>
        /// Organize the file menu item drop down.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void File_DropDownOpening(object? sender, EventArgs e)//TODO
        {
            //ddFile.DropDownItems.Clear();

            //// Always:
            //ddFile.DropDownItems.Add(new ToolStripMenuItem("Open...", null, Open_Click));
            //ddFile.DropDownItems.Add(new ToolStripSeparator());

            //UserSettings.TheSettings.RecentFiles.ForEach(f =>
            //{
            //    ToolStripMenuItem menuItem = new(f, null, new EventHandler(Recent_Click));
            //    ddFile.DropDownItems.Add(menuItem);
            //});
        }

        /// <summary>
        /// The user has asked to open a recent file.
        /// </summary>
        void Recent_Click(object? sender, EventArgs e)
        {
            if (sender is not null)
            {
                string fn = sender.ToString()!;
                OpenFile(fn);
            }
        }

        /// <summary>
        /// Allows the user to select an audio clip or midi from file system.
        /// </summary>
        void Open_Click(object? sender, EventArgs e)
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
        #endregion

        #region Settings
        /// <summary>
        /// Edit user settings.
        /// </summary>
        void EditSettings()
        {
            var changes = UserSettings.TheSettings.Edit("User Settings", 300);

            // Check for meaningful changes.
            //timeBar.SnapMsec = UserSettings.TheSettings.AudioSettings.SnapMsec;
        }

        /// <summary>
        /// Collect and save user settings.
        /// </summary>
        void SaveSettings()
        {
            UserSettings.TheSettings.FormGeometry = new Rectangle(Location.X, Location.Y, Width, Height);
            UserSettings.TheSettings.Volume = MT.volumeMaster.Value;
            //Common.Settings.Autoplay = btnAutoplay.Checked;
            //Common.Settings.Loop = btnLoop.Checked;
            UserSettings.TheSettings.Save();
        }
        #endregion

        #region Misc handlers
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
        #endregion

        #region Misc
        /// <summary>
        /// All about me.
        /// </summary>
        void About_Click(object? sender, EventArgs e)//TODO
        {
            MiscUtils.ShowReadme("Wavicler");
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
        #endregion

        #region State management
        /// <summary>
        /// General state management. Everything goes through here.
        /// </summary>
        void UpdateState(AppState newState)
        {
            // Unhook.
            MT.chkPlay.CheckedChanged -= ChkPlay_CheckedChanged;

            if(newState != _currentState)
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

            ///////// Local funcs ////////
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
                //if(_reader is not null)
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
    }
}
