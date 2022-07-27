using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing.Design;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json.Serialization;
using NBagOfTricks;
using NBagOfUis;
using AudioLib; // TODO restore dll ref.
using NAudio.Wave;
using NBagOfTricks.Slog;
using NAudio.Wave.SampleProviders;

namespace Wavicler
{
    public partial class MainForm : Form
    {
        #region Types
        /// <summary>What are we doing.</summary>
        public enum AppState { Stop, Play, Rewind, Complete }
        #endregion

        #region Fields
        /// <summary>My logger.</summary>
        readonly Logger _logger = LogManager.CreateLogger("MainForm");

        /// <summary>Current file.</summary>
        string _fn = "";

        /// <summary>My settings.</summary>
        UserSettings _settings;

        /// <summary>The actual player.</summary>
        AudioPlayer _player;

        /// <summary>Input device for audio file.</summary>
        AudioFileReader? _audioFileReader;

        /// <summary>Stream read chunk.</summary>
        const int READ_BUFF_SIZE = 1000000;
        #endregion


        CheckBox chkPlay = new();
        Slider sldVolume = new();
        CheckBox btnLoop = new();


        #region Events
        /// <inheritdoc />
        public event EventHandler? PlaybackCompleted;
        #endregion


        #region Lifecycle
        /// <summary>
        /// 
        /// </summary>
        public MainForm()
        {
            // Must do this first before initializing.
            string appDir = MiscUtils.GetAppDataDir("Wavicler", "Ephemera");
            _settings = (UserSettings)Settings.Load(appDir, typeof(UserSettings));
            // Tell the libs about their settings.
            AudioSettings.LibSettings = _settings.AudioSettings;

            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
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



            //////////////////////// from demo/test /////////////////////////////

            ///// Misc controls.
            pot1.ValueChanged += Pot1_ValueChanged;

            pan1.ValueChanged += Pan1_ValueChanged;

            volume1.ValueChanged += Volume1_ValueChanged;

            volume2.ValueChanged += Volume2_ValueChanged;

            ///// Wave viewer.
            // Simple sin.
            float[] data1 = new float[150];
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
            waveViewer2.DrawColor = Color.Green;
            waveViewer2.Init(data2, 1.0f);
            waveViewer2.Marker1 = -1; // hide
            waveViewer2.Marker2 = data2.Length / 2;

            ///// Time bar.
            timeBar.SnapMsec = 10;
            timeBar.Length = new TimeSpan(0, 0, 1, 23, 456);
            timeBar.Start = new TimeSpan(0, 0, 0, 10, 333);
            timeBar.End = new TimeSpan(0, 0, 0, 44, 777);
            timeBar.CurrentTimeChanged += TimeBar_CurrentTimeChanged1;
            timeBar.ProgressColor = Color.CornflowerBlue;
            timeBar.BackColor = Color.Salmon;

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

            //if (!_audioExplorer.Valid)
            //{
            //    _logger.Error($"Something wrong with your audio output device:{Common.Settings.AudioSettings.WavOutDevice}");
            //}
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
        /// 
        /// </summary>
        void EditSettings()
        {
            PropertyGrid pg = new()
            {
                Dock = DockStyle.Fill,
                PropertySort = PropertySort.Categorized,
                SelectedObject = AudioSettings.LibSettings
            };

            using Form f = new()
            {
                ClientSize = new(450, 450),
                AutoScaleMode = AutoScaleMode.None,
                Location = Cursor.Position,
                StartPosition = FormStartPosition.Manual,
                FormBorderStyle = FormBorderStyle.SizableToolWindow,
                ShowIcon = false,
                ShowInTaskbar = false
            };

            // Check changes.
            timeBar.SnapMsec = _settings.AudioSettings.SnapMsec;

            f.Controls.Add(pg);

            f.ShowDialog();
        }

        /// <summary>
        /// Collect and save user settings.
        /// </summary>
        void SaveSettings()
        {
            _settings.FormGeometry = new Rectangle(Location.X, Location.Y, Width, Height);
            //Common.Settings.Volume = sldVolume.Value;
            //Common.Settings.Autoplay = btnAutoplay.Checked;
            //Common.Settings.Loop = btnLoop.Checked;
            _settings.Save();
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
            }

            PlaybackCompleted?.Invoke(this, new EventArgs());
        }

        #region Audio play event handlers
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
            if (_audioFileReader is not null)
            {
                timeBar.Current = _audioFileReader.CurrentTime;
            }
        }
        #endregion

        #region Private functions - draw waves
        /// <summary>
        /// Show a clip waveform.
        /// </summary>
        void ShowClip()
        {
            if (_audioFileReader is not null)
            {
                _audioFileReader.Position = 0; // rewind
                var sampleChannel = new SampleChannel(_audioFileReader, false);

                // Read all data.
                long len = _audioFileReader.Length / (_audioFileReader.WaveFormat.BitsPerSample / 8);
                var data = new float[len];
                int offset = 0;
                int num = -1;

                while (num != 0)
                {
                    // This throws for flac and m4a files for unknown reason but works ok.
                    try
                    {
                        num = _audioFileReader.Read(data, offset, READ_BUFF_SIZE);
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

                timeBar.Length = _audioFileReader.TotalTime;
                timeBar.Start = TimeSpan.Zero;
                timeBar.End = TimeSpan.Zero;
                timeBar.Current = TimeSpan.Zero;

                _audioFileReader.Position = 0; // rewind
            }
        }
        #endregion

        #region Play functions
        public void Play()
        {
            _player.Run(true);
        }

        public void Stop()
        {
            _player.Run(false);
        }

        public void Rewind()
        {
            if (_audioFileReader is not null)
            {
                _audioFileReader.Position = 0;
            }
            _player.Rewind();
            timeBar.Current = TimeSpan.Zero;
        }
        #endregion

        #region File manaagement
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

            using (new WaitCursor())
            {
                try
                {
                    var ext = Path.GetExtension(fn).ToLower();
                    if (AudioLibDefs.AUDIO_FILE_TYPES.Contains(ext))
                    {

                        if(!_player.Valid)
                        {
                            _logger.Error("Your audio device is invalid.");
                            ok = false;
                        }
                    }
                    else
                    {
                        _logger.Error($"Invalid file type: {fn}");
                        ok = false;
                    }

                    if (ok)
                    {
                        ///////ok = _explorer.OpenFile(fn);
                        // Clean up first.
                        _audioFileReader?.Dispose();
                        waveViewer1.Reset();
                        waveViewer2.Reset();

                        // Create input device.
                        _audioFileReader = new AudioFileReader(fn);

                        timeBar.Length = _audioFileReader.TotalTime;
                        timeBar.Start = TimeSpan.Zero;
                        timeBar.End = TimeSpan.Zero;
                        timeBar.Current = TimeSpan.Zero;

                        // Create reader.
                        var sampleChannel = new SampleChannel(_audioFileReader, false);
                        sampleChannel.PreVolumeMeter += SampleChannel_PreVolumeMeter;
                        var postVolumeMeter = new MeteringSampleProvider(sampleChannel);
                        postVolumeMeter.StreamVolume += PostVolumeMeter_StreamVolume;

                        _player.Init(postVolumeMeter);

                        ShowClip();

                        if (!ok)
                        {
                            _audioFileReader?.Dispose();
                            _audioFileReader = null;
                        }






                        _fn = fn;
                        _settings.RecentFiles.UpdateMru(fn);
                        SetText();
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error($"Couldn't open the file: {fn} because: {ex.Message}");
                    _fn = "";
                    SetText();
                    ok = false;
                }
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

        /// <summary>
        /// Organize the file menu item drop down.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void File_DropDownOpening(object? sender, EventArgs e)
        {
            //fileDropDownButton.DropDownItems.Clear();

            //// Always:
            //fileDropDownButton.DropDownItems.Add(new ToolStripMenuItem("Open...", null, Open_Click));
            //fileDropDownButton.DropDownItems.Add(new ToolStripSeparator());

            //Common.Settings.RecentFiles.ForEach(f =>
            //{
            //    ToolStripMenuItem menuItem = new(f, null, new EventHandler(Recent_Click));
            //    fileDropDownButton.DropDownItems.Add(menuItem);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Volume_ValueChanged(object? sender, EventArgs e)
        {
            float vol = (float)sldVolume.Value;
            _settings.Volume = vol;
            _player.Volume = vol;
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
        /// General state management.
        /// </summary>
        void UpdateState(AppState state)
        {
            if (_player.Valid)
            {
                // Unhook.
                chkPlay.CheckedChanged -= ChkPlay_CheckedChanged;

                try
                {
                    switch (state)
                    {
                        case AppState.Complete:
                            Rewind();
                            if (btnLoop.Checked)
                            {
                                chkPlay.Checked = true;
                                Play();
                            }
                            else
                            {
                                chkPlay.Checked = false;
                                Stop();
                            }
                            break;

                        case AppState.Play:
                            chkPlay.Checked = true;
                            Play();
                            break;

                        case AppState.Stop:
                            chkPlay.Checked = false;
                            Stop();
                            break;

                        case AppState.Rewind:
                            Rewind();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    // Rehook.
                    chkPlay.CheckedChanged += ChkPlay_CheckedChanged;
                }
            }
        }

        /// <summary>
        /// Play button handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChkPlay_CheckedChanged(object? sender, EventArgs e)
        {
            UpdateState(chkPlay.Checked ? AppState.Play : AppState.Stop);
        }
        #endregion

        #region TODO stuff
        void Timer1_Tick(object? sender, EventArgs e)
        {
            if (true)//chkRunBars.Checked)
            {
                // Update time bar.
                timeBar.IncrementCurrent(timer1.Interval + 3); // not-real time for testing
                if (timeBar.Current >= timeBar.End) // done/reset
                {
                    timeBar.Current = timeBar.Start;
                }
            }
        }
        void TimeBar_CurrentTimeChanged1(object? sender, EventArgs e)
        {
        }

        void Pot1_ValueChanged(object? sender, EventArgs e)
        {
            // 25 -> 50
            meterLog.AddValue(pot1.Value);
        }

        void Volume1_ValueChanged(object? sender, EventArgs e)
        {
            // meterLog -60 -> +3
            // meterLinear 0 -> 100
            // meterDots -10 -> +10

            meterLinear.AddValue(volume1.Value * 100.0);
        }

        void Volume2_ValueChanged(object? sender, EventArgs e)
        {
            meterDots.AddValue(volume2.Value * 20.0 - 10.0);
        }

        void Pan1_ValueChanged(object? sender, EventArgs e)
        {
            meterLog.AddValue(pan1.Value * 50.0 + 50.0);
        }

        void TimeBar_CurrentTimeChanged(object? sender, EventArgs e)
        {
            txtInfo.AppendText($"Current time:{timeBar.Current}");
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
