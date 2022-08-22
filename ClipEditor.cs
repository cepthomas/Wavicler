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
    public partial class ClipEditor : UserControl// Form //, ISampleProvider
    {
        #region Fields
        /// <summary>My logger.</summary>
        readonly Logger _logger = LogManager.CreateLogger("ClipEditor");
        #endregion

        /// <summary>Gain adjustment.</summary>
        public float Gain { get { return _gain; } set { _gain = value; waveViewer.Invalidate(); } }
        float _gain = 1.0f;

        ///// <summary>Maximum Y gain.</summary>
        //float _maxGain = 5.0f;

        ///// <summary>Grid Y resolution. Assumes +-1.0f range.</summary>
        //float _gridStep = 0.25f;

        //// my new >>>>>
        //UndoStack _stack = new();

        /// <summary>The full buffer from client. TODO or do it dynamically using Read()? Also sampling?</summary>
        float[] _vals = Array.Empty<float>();

        ///// <summary>For drawing.</summary>
        //readonly Pen _pen = new(Color.Black, 1);

        ///// <summary>For drawing.</summary>
        //readonly Pen _penMarker = new(Color.Black, 2);

        ///// <summary>For drawing text.</summary>
        //readonly Font _textFont = new("Cascadia", 12, FontStyle.Regular, GraphicsUnit.Point, 0);

        ///// <summary>For drawing text.</summary>
        //readonly StringFormat _format = new() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center };

        ///// <summary>The rendered data for client playing.</summary>
        //ClipSampleProvider _clipSampleProvider;

        #region Properties
        /// <summary>Gets the WaveFormat of this Sample Provider. ISampleProvider implementation.</summary>
        /// <value>The wave format.</value>
        //public WaveFormat WaveFormat { get; } = WaveFormat.CreateIeeeFloatWaveFormat(AudioLibDefs.SAMPLE_RATE, 1);

        /// <summary>The rendered data for client playing.</summary>
        public ClipSampleProvider SelectionSampleProvider { get; private set; }

        /// <summary>Current file.</summary>
        public string FileName { get; private set; } = "";

        /// <summary>Edit flag.</summary>
        public bool Dirty { get; set; } = false;




        ///// <summary>For styling.</summary>
        //public Color DrawColor { get { return _pen.Color; } set { _pen.Color = value; } }

        // /// <summary>For styling.</summary>
        // public Color MarkerColor { get { return _penMarker.Color; } set { _penMarker.Color = value; } }

        /// <summary>Snap to this increment value.</summary>
        public float SnapSamples { get; set; } = 0;

        // /// <summary>Selection start.</summary>
        // public int SelStart
        // {
        //     get { return _selStart; }
        //     set { _selStart = MathUtils.Constrain(value, 0, _vals.Length); picClipDisplay.Invalidate(); }
        // }
        // int _selStart = new();

        // /// <summary>Selection length.</summary>
        // public int SelLength
        // {
        //     get { return _selLength; }
        //     set { _selLength = MathUtils.Constrain(value, 0, _vals.Length); picClipDisplay.Invalidate(); }
        // }
        // int _selLength = new();

        ///// <summary>One marker.</summary>
        //public int Marker1
        //{
        //    get { return _marker1; }
        //    set { _marker1 = MathUtils.Constrain(value, 0, _vals.Length); picClipDisplay.Invalidate(); }
        //}
        //int _marker1 = new();

        ///// <summary>Other marker.</summary>
        //public int Marker2
        //{
        //    get { return _marker2; }
        //    set { _marker2 = MathUtils.Constrain(value, 0, _vals.Length); picClipDisplay.Invalidate(); }
        //}
        //int _marker2 = new();

        ///// <summary>Visible start sample.</summary>
        //public int VisStart
        //{
        //    get { return _visStart; }
        //    set { _visStart = MathUtils.Constrain(value, 0, _vals.Length); picClipDisplay.Invalidate(); }
        //}
        //int _visStart = -1;

        ///// <summary>Visible length.</summary>
        //public int VisLength
        //{
        //    get { return _visLen; }
        //    set { _visLen = MathUtils.Constrain(value, 0, _vals.Length); picClipDisplay.Invalidate(); }
        //}
        //int _visLen = 0;
        #endregion

        ///// <summary>
        ///// Fill the buffer with 32 bit floating point samples. ISampleProvider implementation.
        ///// </summary>
        ///// <param name="vals">The buffer to fill with samples.</param>
        ///// <param name="offset">Offset into buffer</param>
        ///// <param name="count">The number of samples to read</param>
        ///// <returns>the number of samples written to the buffer.</returns>
        //public int Read(float[] vals, int offset, int count)
        //{
        //    throw new NotImplementedException();
        //}



        #region Lifecycle
        /// <summary>
        /// Normal constructor.
        /// </summary>
        /// <param name="vals">Data to display.</param>
        /// <param name="waveFormat">Format.</param>
        /// <param name="fn">File to open or new if empty.</param>
        public ClipEditor(ClipSampleProvider prov)
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);

            InitializeComponent();

            SelectionSampleProvider = prov;
            Text = prov.GetInfoString();
            //Icon = Properties.Resources.tiger;

            sldGain.ValueChanged += (_, __) => { waveViewer.Gain = (float)sldGain.Value; };
            waveViewer.Init(prov);
            waveViewer.DrawColor = Common.TheSettings.ControlColor;
            waveViewer.SelColor = Color.Cyan; // TODO
            waveViewer.GridColor = Color.LightGray;
            waveViewer.StatusEvent += (_, __) => { sldGain.Value = waveViewer.Gain; };

            sldGain.DrawColor = Common.TheSettings.ControlColor;

            // TODO all time stuff
            timeBar.Marker1 = TimeSpan.Zero;
            timeBar.Marker2 = TimeSpan.Zero;
            timeBar.Current = TimeSpan.Zero;
            timeBar.Length = prov.TotalTime;
            timeBar.SnapMsec = 10;
            //timeBar.CurrentTimeChanged += TimeBar_CurrentTimeChanged;
            timeBar.ProgressColor = Common.TheSettings.ControlColor;
            //timeBar.BackColor = Color.Salmon;
        }

        /// <summary>
        /// Form is legal now. Init things that want to log.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
           // _logger.Info($"OK to log now!!");


        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                //_pen.Dispose();
                //_textFont.Dispose();
                //_format.Dispose();

                components.Dispose();
            }
            //_reader?.Dispose();
            //_reader = null;

            base.Dispose(disposing);
        }
        #endregion
    }
}
