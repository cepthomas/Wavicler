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
using AudioLib; // TODO restore dll ref.
using NBagOfUis;


namespace Wavicler
{
    public partial class WaveForm : Form
    {
        #region Fields
        /// <summary>My logger.</summary>
        readonly Logger _logger = LogManager.CreateLogger("WaveForm");

        /// <summary>Current file.</summary>
        string _fn = "";

        ///// <summary>The actual player.</summary>
        //readonly AudioPlayer _player;

        /// <summary>Input device for audio file.</summary>
        AudioFileReader? _reader;

        /// <summary>Stream read chunk.</summary>
        const int READ_BUFF_SIZE = 100000;
        #endregion

        bool _loop = false;//TODO

        float[] _dataL = Array.Empty<float>();
        float[] _dataR = Array.Empty<float>();

        //public void Play();
        //public void Stop();
        //public void Rewind();


        #region Lifecycle
        /// <summary>
        /// Normal constructor.
        /// </summary>
        public WaveForm()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);

            InitializeComponent();

            Icon = Properties.Resources.tiger;

            gain.ValueChanged += (_, __) => { }; // TODO

            //DummyData();

            timeBar.SnapMsec = UserSettings.TheSettings.AudioSettings.SnapMsec;
            timeBar.CurrentTimeChanged += TimeBar_CurrentTimeChanged;
            timeBar.ProgressColor = Color.CornflowerBlue;
            timeBar.BackColor = Color.Salmon;

            OpenFile(@"C:\Dev\repos\TestAudioFiles\Cave Ceremony 01.wav");
        }

        /// <summary>
        /// Form is legal now. Init things that want to log.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            _logger.Info($"OK to log now!!");
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



        void TimeBar_CurrentTimeChanged(object? sender, EventArgs e)
        {
            // txtInfo.AppendText($"Current time:{timeBar.Current}");
        }



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
                //UpdateState(AppState.Dead);
            }
            else
            {
                //UpdateState(AppState.Complete);
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

        #region Private functions
        /// <summary>
        /// Read the audio data from the file.
        /// </summary>
        void ReadData()
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

                if (sampleChannel.WaveFormat.Channels == 2) // stereo interleaved
                {
                    long stlen = len / 2;
                    _dataL = new float[stlen];
                    _dataR = new float[stlen];

                    for (long i = 0; i < stlen; i++)
                    {
                        _dataL[i] = data[i * 2];
                        _dataR[i] = data[i * 2 + 1];
                    }
                }
                else // mono
                {
                    _dataL = data;
                    _dataL = new float[data.Length];
                    Array.Copy(data, _dataL, data.Length);
                }

                _reader.Position = 0; // rewind
            }
        }

        /// <summary>
        /// Show a clip waveform.
        /// </summary>
        void InitUi()
        {
            waveViewerNav.Init(_dataL, 1.0f);
            waveViewerEdit.Init(_dataR, 1.0f);

            timeBar.Length = _reader.TotalTime;
            timeBar.Start = TimeSpan.Zero;
            timeBar.End = TimeSpan.Zero;
            timeBar.Current = TimeSpan.Zero;
        }
        #endregion

        /// <summary>
        /// Common file opener.
        /// </summary>
        /// <param name="fn">The file to open.</param>
        /// <returns>Status.</returns>
        public bool OpenFile(string fn)
        {
            bool ok = true;

            _logger.Info($"Opening file: {fn}");

            var ext = Path.GetExtension(fn).ToLower();
            if (AudioLibDefs.AUDIO_FILE_TYPES.Contains(ext))
            {
                // Clean up first.
                _reader?.Dispose();
                waveViewerNav.Reset();
                waveViewerEdit.Reset();
                waveViewerNav.Reset();

                // Read the file.
                // AudioFileReader : WaveStream, ISampleProvider
                _reader = new AudioFileReader(fn);

                // (IWaveProvider waveProvider, bool forceStereo)
                //var sampleChannel = new SampleChannel(_reader, false);
                //sampleChannel.PreVolumeMeter += SampleChannel_PreVolumeMeter;

                // (ch, smpls per notif)
                //var postVolumeMeter = new MeteringSampleProvider(sampleChannel, _reader.WaveFormat.SampleRate);
                var postVolumeMeter = new MeteringSampleProvider(_reader, _reader.WaveFormat.SampleRate / 10);
                postVolumeMeter.StreamVolume += PostVolumeMeter_StreamVolume;

                //_player.Init(postVolumeMeter);

                ReadData();
                InitUi();

                _fn = fn;

                var s = _fn == "" ? "No file loaded" : _fn;
                Text = $"Wavicler {MiscUtils.GetVersionString()} - {s}";
            }
            else
            {
                _logger.Error($"Unsupported file type: {fn}");
                _reader?.Dispose();
                _reader = null;
                ok = false;
            }

            //if (ok)
            //{
            //    if (_settingsTODO.Autoplay)
            //    {
            //        UpdateState(AppState.Rewind);
            //        UpdateState(AppState.Play);
            //    }
            //}

            return ok;
        }
    }
}
