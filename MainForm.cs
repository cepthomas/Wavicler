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
using System.Diagnostics;

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

        /// <summary>Stream read chunk.</summary>
        const int READ_BUFF_SIZE = 100000;

        /// <summary>TODO loop?</summary>
        bool _loop = false;

        /// <summary>TODO kludgy?</summary>
        readonly MainToolbar MT = new();

        /// <summary>Dynamically connect input providers to the player.</summary>
        SwappableSampleProvider _swapper = new(WaveFormat.CreateIeeeFloatWaveFormat(44100, 1));
        #endregion

        #region Lifecycle
        public MainForm()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);

            // Must do this first before initializing.
            string appDir = MiscUtils.GetAppDataDir("Wavicler", "Ephemera");
            UserSettings.TheSettings = (UserSettings)Settings.Load(appDir, typeof(UserSettings));
            // Tell the libs about their settings.
            AudioSettings.LibSettings = UserSettings.TheSettings.AudioSettings;

            // Build it.
            InitializeComponent();
            Icon = Properties.Resources.tiger;
            ToolStrip.Items.Add(new ToolStripControlHost(MT));

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

            MT.btnRewind.Click += (_, __) => { UpdateState(AppState.Rewind); };
            MT.chkPlay.Click += (_, __) => { UpdateState(MT.chkPlay.Checked ? AppState.Play : AppState.Stop); };
            MT.volumeMaster.ValueChanged += (_, __) => { _player.Volume = (float)MT.volumeMaster.Value; };

            // Managing files. FileMenuItem
            FileMenuItem.DropDownOpening += Recent_DropDownOpening;
            NewMenuItem.Click += (_, __) => { OpenFile(); };
            OpenMenuItem.Click += (_, __) => { Open_Click(); };
            // - RecentMenuItem
            // - SaveMenuItem
            // - SaveAsMenuItem
            // - CloseMenuItem
            // - ExitMenuItem

            // Editing. EditMenuItem
            // - CutMenuItem
            // - CopyMenuItem
            // - PasteMenuItem
            // - ReplaceMenuItem
            // - RemoveEnvelopeMenuItem

            // Tools. ToolsMenuItem
            // - BpmMenuItem
            // - AboutMenuItem
            // - SettingsMenuItem

            //this.ActivateMdiChild(form)
            //            var child = ActiveMdiChild;


            // Create output.
            _player = new(UserSettings.TheSettings.AudioSettings.WavOutDevice, int.Parse(UserSettings.TheSettings.AudioSettings.Latency));
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
            var postVolumeMeter = new MeteringSampleProvider(_swapper, _swapper.WaveFormat.SampleRate / 10);
            postVolumeMeter.StreamVolume += (object? sender, StreamVolumeEventArgs e) =>
            {
               // TODO timeBar.Current = _reader.CurrentTime;
            };
            _player.Init(postVolumeMeter);

            Text = $"Wavicler {MiscUtils.GetVersionString()}";

            // >>>>>>>>>>>>>>>>>>>
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





        #region File management
        /// <summary>
        /// Common file opener.
        /// </summary>
        /// <param name="fn">The file to open.</param>
        /// <returns>Status.</returns>
        public bool OpenFile(string fn = "")
        {
            bool ok = true;

            UpdateState(AppState.Stop);

            if(fn == "")
            {
                _logger.Info($"Creating new child");
                WaveEditor childNew = new(Array.Empty<float>(), fn) { MdiParent = this };
                childNew.Show();
            }
            else
            {
                var ext = Path.GetExtension(fn).ToLower();
                if(!File.Exists(fn))
                {
                    _logger.Error($"Invalid file: {fn}");
                    ok = false;
                }
                else if (AudioLibDefs.AUDIO_FILE_TYPES.Contains(ext))
                {
                    _logger.Info($"Opening file: {fn}");

                    // Read all data.
                    var reader = new AudioFileReader(fn);

                    // public float Volume
                    // public override long Length => length;
                    // public override long Position
                    // public string FileName { get; }
                    // public override WaveFormat WaveFormat => sampleChannel.WaveFormat;
                    // >>>>
                    //public WaveFormatEncoding Encoding => waveFormatTag;
                    //public int Channels => channels;
                    //public int SampleRate => sampleRate;
                    //public int AverageBytesPerSecond => averageBytesPerSecond;
                    //public virtual int BlockAlign => blockAlign;
                    //public int BitsPerSample => bitsPerSample;
                    //public int ExtraSize => extraSize;


                    //sampleRate: 44100 smplPerSec:: 22.67573696145125 usec
                    //tempo: 100 beatsPerMin == 100 / 60 beatsPerSec == 60 / 100 secPerBeat
                    //smpl per beat = smplPerSec * secPerBeat
                    // Debug.WriteLine($"tempo,secPerBeat,samplesPerBeat");
                    // for (float tempo = 60.0f; tempo < 200.0f; tempo += 2.5f)
                    // {
                    //     float secPerBeat = 60.0f / tempo;
                    //     float samplesPerBeat = secPerBeat * reader.WaveFormat.SampleRate;
                    //     Debug.WriteLine($"{tempo},{secPerBeat},{samplesPerBeat}");
                    // }


                    long len = reader.Length / (reader.WaveFormat.BitsPerSample / 8);
                    var data = new float[len];
                    int offset = 0;
                    int num = -1;

                    while (num != 0)
                    {
                        // This throws for flac and m4a files for unknown reason but works ok.
                        try
                        {
                            num = reader.Read(data, offset, READ_BUFF_SIZE);
                            offset += num;
                        }
                        catch (Exception)
                        {
                        }
                    }

                    if (reader.WaveFormat.Channels == 2) // stereo interleaved
                    {
                        // TODO ask user if they want L/R/both-separate
                        // StereoToMonoSampleProvider to split stereo into 2 mono?

                        long stlen = len / 2;
                        var dataL = new float[stlen];
                        var dataR = new float[stlen];

                        for (long i = 0; i < stlen; i++)
                        {
                            dataL[i] = data[i * 2];
                            dataR[i] = data[i * 2 + 1];
                        }
                        WaveEditor childL = new(dataL, $"{fn}.left") { MdiParent = this };
                        childL.Show();
                        WaveEditor childR = new(dataR, $"{fn}.right") { MdiParent = this };
                        childR.Show();
                    }
                    else // mono
                    {
                        var buff = new float[data.Length];
                        Array.Copy(data, buff, data.Length);
                        WaveEditor childM = new(buff, fn) { MdiParent = this };
                        childM.Show();
                    }
                }
                else
                {
                    _logger.Error($"Unsupported file type: {fn}");
                    ok = false;
                }
            }

            if (ok && UserSettings.TheSettings.Autoplay)
            {
                UpdateState(AppState.Rewind);
                UpdateState(AppState.Play);
            }

            MT.chkPlay.Enabled = ok;
            return ok;
        }

        /// <summary>
        /// Organize the file menu item drop down.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Recent_DropDownOpening(object? sender, EventArgs e)//TODO
        {
            RecentMenuItem.DropDownItems.Clear();

            UserSettings.TheSettings.RecentFiles.ForEach(f =>
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

        void MainForm_MdiChildActivate(object sender, EventArgs e)
        {
            var child = (sender as MainForm).ActiveMdiChild;
            _logger.Info($"MDI child:{child.Text}");
        }
    }
}
