using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing.Design;
using System.Text.Json.Serialization;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NBagOfTricks;
using NBagOfTricks.Slog;
using NBagOfUis;
using AudioLib; // TODO restore dll ref.


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

        /// <summary>Current file.</summary>
        string _fn = "";

        /// <summary>My settings.</summary>
        readonly UserSettings _settings;

        /// <summary>The actual player.</summary>
        readonly AudioPlayer _player;

        /// <summary>Input device for audio file.</summary>
        AudioFileReader? _reader;

        /// <summary>Where we be.</summary>
        AppState _currentState = AppState.Stop;

        /// <summary>Stream read chunk.</summary>
        const int READ_BUFF_SIZE = 100000;
        #endregion

        bool _loop = false;//TODO

        #region Lifecycle
        /// <summary>
        /// Normal constructor.
        /// </summary>
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
            LogManager.LogEvent += LogManager_LogEvent;
            LogManager.Run();

            // Init main form from settings
            WindowState = FormWindowState.Normal;
            StartPosition = FormStartPosition.Manual;
            Location = new Point(_settings.FormGeometry.X, _settings.FormGeometry.Y);
            Size = new Size(_settings.FormGeometry.Width, _settings.FormGeometry.Height);
            KeyPreview = true; // for routing kbd strokes through OnKeyDown

            // The text output.
            txtInfo.Font = Font;
            txtInfo.WordWrap = true;
            txtInfo.MatchColors.Add("ERR", Color.LightPink);
            txtInfo.MatchColors.Add("WRN:", Color.Plum);

            _player = new(_settings.AudioSettings.WavOutDevice, int.Parse(_settings.AudioSettings.Latency));
            _player.PlaybackStopped += Player_PlaybackStopped;

            btnRewind.Click += (_, __) => { UpdateState(AppState.Rewind); };

            btnSettings.Click += (_, __) => { EditSettings(); };

            ddFile.DropDownOpening += File_DropDownOpening;

            volumeMaster.ValueChanged += (_, __) => { _player.Volume = (float)volumeMaster.Value; };
            gain.ValueChanged += (_, __) => {  }; // TODO



            //////////////////////// from demo/test /////////////////////////////
            //pot1.ValueChanged += (_, __) => { meterLog.AddValue(pot1.Value); };
            //pan1.ValueChanged += (_, __) => { meterLog.AddValue(pan1.Value * 50.0 + 50.0); };
            //volume1.ValueChanged += (_, __) => { meterLinear.AddValue(volume1.Value * 100.0); };
            //volume2.ValueChanged += (_, __) => { meterDots.AddValue(volume2.Value * 20.0 - 10.0); };
            //////////////////////// from demo/test /////////////////////////////


            //DummyData();

            OpenFile(@"C:\Dev\repos\TestAudioFiles\Cave Ceremony 01.wav");

            // Go-go-go.
            timer1.Enabled = true;
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
        }

        /// <summary>
        /// 
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
            base.Dispose(disposing);
        }
        #endregion

        #region Settings
        /// <summary>
        /// Edit user settings.
        /// </summary>
        void EditSettings()
        {
            var changes = _settings.Edit("User Settings", 300);

            // Check changes.
            timeBar.SnapMsec = _settings.AudioSettings.SnapMsec;
        }

        /// <summary>
        /// Collect and save user settings.
        /// </summary>
        void SaveSettings()
        {
            _settings.FormGeometry = new Rectangle(Location.X, Location.Y, Width, Height);
            _settings.Volume = volumeMaster.Value;
            //Common.Settings.Autoplay = btnAutoplay.Checked;
            //Common.Settings.Loop = btnLoop.Checked;
            _settings.Save();
        }
        #endregion

        #region Audio play event handlers
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

        /// <summary>
        /// Hook for processing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SampleChannel_PreVolumeMeter(object? sender, StreamVolumeEventArgs e)
        {
        }

        /// <summary>
        /// Hook for processing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PostVolumeMeter_StreamVolume(object? sender, StreamVolumeEventArgs e)
        {
            if (_reader is not null)
            {
                timeBar.Current = _reader.CurrentTime;
            }
        }
        #endregion

        #region Private functions - draw waves
        /// <summary>
        /// Show a clip waveform.
        /// </summary>
        void ShowClip()
        {
            if (_reader is not null)
            {
                _reader.Position = 0; // rewind
                var sampleChannel = new SampleChannel(_reader, false);

                // Read all data.
                long len = _reader.Length / (_reader.WaveFormat.BitsPerSample / 8);
                var data = new float[len];
                int offset = 0;
                int num = -1;

                while (num != 0)
                {
                    // This throws for flac and m4a files for unknown reason but works ok.
                    try
                    {
                        num = _reader.Read(data, offset, READ_BUFF_SIZE);
                        offset += num;
                    }
                    catch (Exception)
                    {
                    }
                }

                if (sampleChannel.WaveFormat.Channels == 2) // stereo
                {
                    long stlen = len / 2;
                    var dataL = new float[stlen];
                    var dataR = new float[stlen];

                    for (long i = 0; i < stlen; i++)
                    {
                        dataL[i] = data[i * 2];
                        dataR[i] = data[i * 2 + 1];
                    }

                    waveViewer1.Init(dataL, 1.0f);
                    waveViewer2.Init(dataR, 1.0f);
                }
                else // mono
                {
                    waveViewer1.Init(data, 1.0f);
                    waveViewer2.Init(null, 0);
                }

                timeBar.Length = _reader.TotalTime;
                timeBar.Start = TimeSpan.Zero;
                timeBar.End = TimeSpan.Zero;
                timeBar.Current = TimeSpan.Zero;

                _reader.Position = 0; // rewind
            }
        }
        #endregion

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
                // Clean up first.
                _reader?.Dispose();
                waveViewer1.Reset();
                waveViewer2.Reset();
                waveViewerNav.Reset();

                // Read the file.
                // AudioFileReader : WaveStream, ISampleProvider
                _reader = new AudioFileReader(fn);

                // Create output.
                // (IWaveProvider waveProvider, bool forceStereo)
                var sampleChannel = new SampleChannel(_reader, false);
                //sampleChannel.PreVolumeMeter += SampleChannel_PreVolumeMeter;

                // (ch, smpls per notif)
                var postVolumeMeter = new MeteringSampleProvider(sampleChannel, _reader.WaveFormat.SampleRate);
                postVolumeMeter.StreamVolume += PostVolumeMeter_StreamVolume;

                timeBar.Length = _reader.TotalTime;
                timeBar.Start = TimeSpan.Zero;
                timeBar.End = TimeSpan.Zero;
                timeBar.Current = TimeSpan.Zero;

                _player.Init(postVolumeMeter);

                ShowClip();

                _fn = fn;
                _settings.RecentFiles.UpdateMru(fn);
                SetText();
            }
            else
            {
                _logger.Error($"Unsupported file type: {fn}");
                _reader?.Dispose();
                _reader = null;
                ok = false;
            }

            chkPlay.Enabled = ok;

            if (ok)
            {
                if (_settings.Autoplay)
                {
                    UpdateState(AppState.Rewind);
                    UpdateState(AppState.Play);
                }
            }

            return ok;
        }


        //public bool OpenFile_orig(string fn)
        //{
        //    bool ok = true;

        //    UpdateState(AppState.Stop);

        //    _logger.Info($"Opening file: {fn}");

        //    var ext = Path.GetExtension(fn).ToLower();
        //    if (AudioLibDefs.AUDIO_FILE_TYPES.Contains(ext))
        //    {
        //        // Clean up first.
        //        _reader?.Dispose();
        //        waveViewer1.Reset();
        //        waveViewer2.Reset();
        //        waveViewerNav.Reset();

        //        // Create reader.
        //        // AudioFileReader : WaveStream, ISampleProvider
        //        _reader = new AudioFileReader(fn);

        //        // Create output.
        //        // (IWaveProvider waveProvider, bool forceStereo)
        //        var sampleChannel = new SampleChannel(_reader, false);
        //        //sampleChannel.PreVolumeMeter += SampleChannel_PreVolumeMeter;

        //        // (ch, smpls per notif)
        //        var postVolumeMeter = new MeteringSampleProvider(sampleChannel, _reader.WaveFormat.SampleRate);
        //        postVolumeMeter.StreamVolume += PostVolumeMeter_StreamVolume;

        //        timeBar.Length = _reader.TotalTime;
        //        timeBar.Start = TimeSpan.Zero;
        //        timeBar.End = TimeSpan.Zero;
        //        timeBar.Current = TimeSpan.Zero;

        //        _player.Init(postVolumeMeter);

        //        ShowClip();

        //        _fn = fn;
        //        _settings.RecentFiles.UpdateMru(fn);
        //        SetText();
        //    }
        //    else
        //    {
        //        _logger.Error($"Unsupported file type: {fn}");
        //        _reader?.Dispose();
        //        _reader = null;
        //        ok = false;
        //    }

        //    chkPlay.Enabled = ok;

        //    if (ok)
        //    {
        //        if (_settings.Autoplay)
        //        {
        //            UpdateState(AppState.Rewind);
        //            UpdateState(AppState.Play);
        //        }
        //    }

        //    return ok;
        //}




        /// <summary>
        /// Organize the file menu item drop down.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void File_DropDownOpening(object? sender, EventArgs e)
        {
            ddFile.DropDownItems.Clear();

            // Always:
            ddFile.DropDownItems.Add(new ToolStripMenuItem("Open...", null, Open_Click));
            ddFile.DropDownItems.Add(new ToolStripSeparator());

            _settings.RecentFiles.ForEach(f =>
            {
                ToolStripMenuItem menuItem = new(f, null, new EventHandler(Recent_Click));
                ddFile.DropDownItems.Add(menuItem);
            });
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

            if (openDlg.ShowDialog() == DialogResult.OK && openDlg.FileName != _fn)
            {
                OpenFile(openDlg.FileName);
                _fn = openDlg.FileName;
            }
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
                    UpdateState(chkPlay.Checked ? AppState.Stop : AppState.Play);
                    e.Handled = true;
                    break;
            }
        }
        #endregion

        #region Misc
        /// <summary>
        /// All about me.
        /// </summary>
        void About_Click(object? sender, EventArgs e)
        {
            MiscUtils.ShowReadme("Wavicler");
        }

        /// <summary>
        /// Utility for header.
        /// </summary>
        void SetText()
        {
            var s = _fn == "" ? "No file loaded" : _fn;
            Text = $"Wavicler {MiscUtils.GetVersionString()} - {s}";
        }
        #endregion

        #region State management
        /// <summary>
        /// General state management. Everything goes through here.
        /// </summary>
        void UpdateState(AppState newState)
        {
            // Unhook.
            chkPlay.CheckedChanged -= ChkPlay_CheckedChanged;

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
            chkPlay.CheckedChanged += ChkPlay_CheckedChanged;

            ///////// Local funcs ////////
            void Play()
            {
                chkPlay.Checked = true;
                _player.Run(true);
            }

            void Stop()
            {
                chkPlay.Checked = false;
                _player.Run(false);
            }

            void Rewind()
            {
                if(_reader is not null)
                {
                    _reader.Position = 0;
                }
                _player.Rewind();
                timeBar.Current = TimeSpan.Zero;
            }
        }

        /// <summary>
        /// Play button handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ChkPlay_CheckedChanged(object? sender, EventArgs e)
        {
            UpdateState(chkPlay.Checked ? AppState.Play : AppState.Stop);
        }
        #endregion

        #region TODO stuff
        void Timer1_Tick(object? sender, EventArgs e)
        {
            //if (chkRunBars.Checked)
            //{
            //    // Update time bar.
            //    timeBar.IncrementCurrent(timer1.Interval + 3); // not-real time for testing
            //    if (timeBar.Current >= timeBar.End) // done/reset
            //    {
            //        timeBar.Current = timeBar.Start;
            //    }
            //}
        }

        void TimeBar_CurrentTimeChanged(object? sender, EventArgs e)
        {
            txtInfo.AppendText($"Current time:{timeBar.Current}");
        }

        void DummyData()
        {
            ///// Wave viewer.
            // Simple sin.
            float[] data1 = new float[waveViewer1.ClientSize.Width];
            for (int i = 0; i < data1.Length; i++)
            {
                data1[i] = (float)Math.Sin(Math.PI * i / 180.0);
            }
            waveViewer1.Mode = WaveViewer.DrawMode.Raw;
            waveViewer1.DrawColor = Color.Green;
            waveViewer1.Init(data1, 1.0f);
            waveViewer1.Marker1 = 20;
            waveViewer1.Marker2 = 130;

            // Real data.
            string[] sdata = File.ReadAllLines(@"..\..\todo\wav.txt");
            float[] data2 = new float[sdata.Length];
            for (int i = 0; i < sdata.Length; i++)
            {
                data2[i] = float.Parse(sdata[i]);
            }
            waveViewer2.Mode = WaveViewer.DrawMode.Envelope;
            waveViewer2.DrawColor = Color.Blue;
            waveViewer2.Init(data2, 1.0f);
            waveViewer2.Marker1 = -1; // hide
            waveViewer2.Marker2 = data2.Length / 2;

            ///// Time bar.
            timeBar.SnapMsec = _settings.AudioSettings.SnapMsec;
            timeBar.Length = new TimeSpan(0, 0, 1, 23, 456);
            timeBar.Start = new TimeSpan(0, 0, 0, 10, 333);
            timeBar.End = new TimeSpan(0, 0, 0, 44, 777);
            timeBar.CurrentTimeChanged += TimeBar_CurrentTimeChanged;
            timeBar.ProgressColor = Color.CornflowerBlue;
            timeBar.BackColor = Color.Salmon;
        }
        #endregion

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
                this.InvokeIfRequired(_ => { txtInfo.AppendLine($"{e.Message}"); });
            }
        }
    }
}
