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
    public partial class ClipEditor : Form //, ISampleProvider
    {
        #region Fields
        /// <summary>My logger.</summary>
        readonly Logger _logger = LogManager.CreateLogger("ClipEditor");
        #endregion

        // From WaveViewer
        /// <summary>Y adjustment.</summary>
        public float YGain { get { return _yGain; } set { _yGain = value; picClipDisplay.Invalidate(); } }
        float _yGain = 1.0f;

        /// <summary>Maximum Y gain.</summary>
        float _maxGain = 5.0f;

        /// <summary>Grid Y resolution. Assumes +-1.0f range.</summary>
        float _gridStep = 0.25f;



        // my new >>>>>
        UndoStack _stack = new();

        /// <summary>The full buffer from client. TODO or do it dynamically using Read()? Also sampling?</summary>
        float[] _vals = Array.Empty<float>();

        /// <summary>For drawing.</summary>
        readonly Pen _pen = new(Color.Black, 1);

        /// <summary>For drawing.</summary>
        readonly Pen _penMarker = new(Color.Black, 2);

        /// <summary>For drawing text.</summary>
        readonly Font _textFont = new("Cascadia", 12, FontStyle.Regular, GraphicsUnit.Point, 0);

        /// <summary>For drawing text.</summary>
        readonly StringFormat _format = new() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center };

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

        /// <summary>For styling.</summary>
        public Color DrawColor { get { return _pen.Color; } set { _pen.Color = value; } }

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



        /// <summary>One marker.</summary>
        public int Marker1
        {
            get { return _marker1; }
            set { _marker1 = MathUtils.Constrain(value, 0, _vals.Length); picClipDisplay.Invalidate(); }
        }
        int _marker1 = new();

        /// <summary>Other marker.</summary>
        public int Marker2
        {
            get { return _marker2; }
            set { _marker2 = MathUtils.Constrain(value, 0, _vals.Length); picClipDisplay.Invalidate(); }
        }
        int _marker2 = new();


        /// <summary>Visible start sample.</summary>
        public int VisStart
        {
            get { return _visStart; }
            set { _visStart = MathUtils.Constrain(value, 0, _vals.Length); picClipDisplay.Invalidate(); }
        }
        int _visStart = -1;

        /// <summary>Visible length.</summary>
        public int VisLength
        {
            get { return _visLen; }
            set { _visLen = MathUtils.Constrain(value, 0, _vals.Length); picClipDisplay.Invalidate(); }
        }
        int _visLen = 0;

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
            Text = NAudioEx.GetInfo(prov);
            Icon = Properties.Resources.tiger;

            gain.ValueChanged += (_, __) => { SelectionSampleProvider.MasterGain = (float)gain.Value; };

            //ReadData();
            _vals = prov.ReadAll();

            waveViewerNav.SampleProvider = prov;

            // TODO all time stuff
            timeBar.Marker1 = TimeSpan.Zero;
            timeBar.Marker2 = TimeSpan.Zero;
            timeBar.Current = TimeSpan.Zero;
            timeBar.Length = prov.TotalTime;
            timeBar.SnapMsec = 10;
            timeBar.CurrentTimeChanged += TimeBar_CurrentTimeChanged;
            timeBar.ProgressColor = Common.TheSettings.ControlColor;
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
                _pen.Dispose();
                _textFont.Dispose();
                _format.Dispose();

                components.Dispose();
            }
            //_reader?.Dispose();
            //_reader = null;

            base.Dispose(disposing);
        }
        #endregion

        #region Draw
        /// <summary>
        /// Paints the waveform.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ClipDisplay_Paint(object sender, PaintEventArgs e)
        {
            // Setup.
            e.Graphics.Clear(BackColor);

            if (_vals is null || _vals.Length == 0)
            {
                e.Graphics.DrawString("No data", _textFont, Brushes.Gray, ClientRectangle, _format);
            }
            else
            {
                int border = 5;
                int fitWidth = Width - (2 * border);
                int fitHeight = Height - (2 * border);
                int numVals = _vals.Length;
                int samplesPerPixel = _vals.Length / fitWidth;

                var peaks = PeakProvider.GetPeaks(_vals, 0, samplesPerPixel, fitWidth); // TODO for visible range!

                for (int i = 0; i < peaks.Count; i++)
                {
                    float yMax = border + fitHeight - ((peaks[i].max + 1) * 0.5f * fitHeight);
                    float yMin = border + fitHeight - ((peaks[i].min + 1) * 0.5f * fitHeight);
                    e.Graphics.DrawLine(_pen, i + border, yMax, i + border, yMin);
                }

                //// Draw  marker.
                //_marker = MathUtils.Constrain(_marker, 0, _vals.Length);
                //if (_marker > 0)
                //{
                //    int mpos = (int)(_marker * (fitWidth / numVals));
                //    pe.Graphics.DrawLine(_penMarker, mpos, 0, mpos, Height);
                //}
            }
        }

        #endregion

        // #region UI handlers
        // /// <summary>
        // /// 
        // /// </summary>
        // /// <param name="sender"></param>
        // /// <param name="e"></param>
        // void ClipDisplay_MouseWheel(object? sender, MouseEventArgs e)
        // {
        //     HandledMouseEventArgs hme = (HandledMouseEventArgs)e;
        //     hme.Handled = true; // This prevents the mouse wheel event from getting back to the parent.

        //     if (ModifierKeys == Keys.Control) // x zoom TODO
        //     {

        //     }
        //     else if (ModifierKeys == Keys.Shift) // y gain
        //     {
        //         _yGain += hme.Delta > 0 ? 0.1f : -0.1f;
        //         _yGain = (float)MathUtils.Constrain(_yGain, 0.0f, _maxGain);
        //         picClipDisplay.Invalidate();
        //     }
        //     else if (ModifierKeys == Keys.None) // no mods = x shift TODO
        //     {

        //     }
        // }

        // /// <summary>
        // /// 
        // /// </summary>
        // /// <param name="sender"></param>
        // /// <param name="e"></param>
        // void ClipDisplay_MouseDown(object? sender, MouseEventArgs e)
        // {
        //     switch (e.Button)
        //     {
        //         case MouseButtons.Left:

        //             if (ModifierKeys == Keys.Control) // end pos
        //             {

        //                 picClipDisplay.Invalidate();
        //             }
        //             else if (ModifierKeys == Keys.None) // start pos
        //             {
        //                 picClipDisplay.Invalidate();
        //             }
        //             break;
        //     }
        // }

        // /// <summary>
        // /// 
        // /// </summary>
        // /// <param name="sender"></param>
        // /// <param name="e"></param>
        // void ClipDisplay_KeyDown(object? sender, KeyEventArgs e)
        // {
        //     switch (e.KeyCode)
        //     {
        //         case Keys.Escape: // reset gain
        //             //_firstPaint = true;
        //             _yGain = 1.0f;
        //             picClipDisplay.Invalidate();
        //             break;
        //     }
        // }


        // private void ClipEditor_Resize(object? sender, EventArgs e)
        // {
        //     //_firstPaint = true; // Need to recalc the grid too.
        //     picClipDisplay.Invalidate();
        // }


        // private void ClipEditor_KeyDown(object? sender, KeyEventArgs e)
        // {
        //     switch(e.KeyCode)
        //     {
        //         //case Keys.Escape:
        //         case Keys.G:
        //             //_firstPaint = true;
        //             _yGain = 1.0f;
        //             picClipDisplay.Invalidate();
        //             break;

        //         case Keys.H:
        //             //_firstPaint = true;
        //             _visStart = 0;
        //             _visLen = _vals.Length;
        //             picClipDisplay.Invalidate();
        //             break;

        //         default:
        //             break;
        //     }
        // }

        // /// <summary>
        // /// 
        // /// </summary>
        // /// <param name="sender"></param>
        // /// <param name="e"></param>
        // void TimeBar_CurrentTimeChanged(object? sender, EventArgs e)
        // {
        //     // txtInfo.AppendText($"Current time:{timeBar.Current}");
        // }
        // #endregion

        // #region Private functions TODO all these
        // /// <summary>
        // /// Convert x pos to sample index.
        // /// </summary>
        // /// <param name="x"></param>
        // int GetSampleFromMouse(int x)
        // {
        //     int sample = MathUtils.Map(x, 0, Width, _visStart, _visStart + _visLen);
        //     return sample;
        // }

        // /// <summary>
        // /// Convert x pos to TimeSpan.
        // /// </summary>
        // /// <param name="x"></param>
        // TimeSpan GetTimeFromMouse(int x)
        // {
        //     int msec = 0;

        //     //if (_current.TotalMilliseconds < _length.TotalMilliseconds)
        //     //{
        //     //    msec = x * (int)_length.TotalMilliseconds / Width;
        //     //    msec = MathUtils.Constrain(msec, 0, (int)_length.TotalMilliseconds);
        //     //    msec = DoSnap(msec);
        //     //}

        //     return new TimeSpan(0, 0, 0, 0, msec);
        // }

        // /// <summary>
        // /// Snap to user preference.
        // /// </summary>
        // /// <param name="sample"></param>
        // /// <returns></returns>
        // int DoSnap(int sample)
        // {
        //     //int smsec = 0;

        //     //if (SnapMsec > 0)
        //     //{
        //     //    smsec = (msec / SnapMsec) * SnapMsec;
        //     //    if (SnapMsec > (msec % SnapMsec) / 2)
        //     //    {
        //     //        smsec += SnapMsec;
        //     //    }
        //     //}

        //     return sample;
        // }

        // ///// <summary>
        // ///// Utility helper function.
        // ///// </summary>
        // ///// <param name="val"></param>
        // ///// <param name="lower"></param>
        // ///// <param name="upper"></param>
        // ///// <returns></returns>
        // //TimeSpan Constrain(TimeSpan val, TimeSpan lower, TimeSpan upper)
        // //{
        // //    return TimeSpan.FromMilliseconds(MathUtils.Constrain(val.TotalMilliseconds, lower.TotalMilliseconds, upper.TotalMilliseconds));
        // //}

        // ///// <summary>
        // ///// Map from time to UI pixels.
        // ///// </summary>
        // ///// <param name="val"></param>
        // ///// <returns></returns>
        // //int Scale(TimeSpan val)
        // //{
        // //    return (int)(val.TotalMilliseconds * Width / _length.TotalMilliseconds);
        // //}

        // #endregion
    }
}
