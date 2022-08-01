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

        ///// <summary>The actual player.</summary>
        //readonly AudioPlayer _player;

        ///// <summary>Input device for audio file.</summary>
        //AudioFileReader? _reader;

        ///// <summary>Stream read chunk.</summary>
        //const int READ_BUFF_SIZE = 100000;
        #endregion

       // bool _loop = false;//TODO

        float[] _buff = Array.Empty<float>();
        
        //float[] _dataL = Array.Empty<float>();
        //float[] _dataR = Array.Empty<float>();


        #region Properties
        /// <summary>Current file.</summary>
        public string FileName { get; private set; } = "";

        /// <summary>Edit flag.</summary>
        public bool Dirty { get; set; } = false;
        #endregion

        /// <summary>The edited data for client playing.</summary>
        public ISampleProvider RenderedSampleProvider { get; private set; }


        //public void Play();

        //public void Stop();


        // TODO implement these.
        public void Rewind()
        {
        }

        public void Save(string newfn = "")
        {
        }

        public void Undo()
        {
        }

        public void Redo()
        {
        }



        #region Lifecycle
        /// <summary>
        /// Normal constructor.
        /// </summary>
        /// <param name="buff">Data to display.</param>
        /// <param name="fn">File to open or default if empty.</param>
        public WaveForm(float[] buff, string fn)
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);

            InitializeComponent();

            //bool ok = true;
            _buff = buff;
            FileName = fn;
            Text = fn;
            Icon = Properties.Resources.tiger;

            gain.ValueChanged += (_, __) => { }; // TODO



            //ReadData();

            waveViewerNav.Init(_buff, 1.0f);
            waveViewerEdit.Init(_buff, 1.0f);

            //TODO? timeBar.Length = _reader.TotalTime;
            timeBar.Start = TimeSpan.Zero;
            timeBar.End = TimeSpan.Zero;
            timeBar.Current = TimeSpan.Zero;


            timeBar.SnapMsec = UserSettings.TheSettings.AudioSettings.SnapMsec;
            timeBar.CurrentTimeChanged += TimeBar_CurrentTimeChanged;
            timeBar.ProgressColor = UserSettings.TheSettings.ControlColor;
            //timeBar.BackColor = Color.Salmon;
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
            //_reader?.Dispose();
            //_reader = null;

            base.Dispose(disposing);
        }
        #endregion


        void TimeBar_CurrentTimeChanged(object? sender, EventArgs e)
        {
            // txtInfo.AppendText($"Current time:{timeBar.Current}");
        }



        //#region Audio play event handlers
        ///// <summary>
        ///// Usually end of file but could be error.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //void Player_PlaybackStopped(object? sender, StoppedEventArgs e)
        //{
        //    if (e.Exception is not null)
        //    {
        //        _logger.Exception(e.Exception, "Other NAudio error");
        //        //UpdateState(AppState.Dead);
        //    }
        //    else
        //    {
        //        //UpdateState(AppState.Complete);
        //    }
        //}

        ///// <summary>
        ///// Hook for processing.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //void SampleChannel_PreVolumeMeter(object? sender, StreamVolumeEventArgs e)
        //{
        //}

        ///// <summary>
        ///// Hook for processing.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //void PostVolumeMeter_StreamVolume(object? sender, StreamVolumeEventArgs e)
        //{
        //    if (_reader is not null)
        //    {
        //        timeBar.Current = _reader.CurrentTime;
        //    }
        //}
        //#endregion

        #region Private functions

        #endregion
    }
}
