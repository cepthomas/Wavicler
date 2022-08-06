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


//units are time or bars/beats.

//9,223,372,036,854,775,807 = max int
//2,646,000 vals per minute @ 44100


namespace Wavicler
{
    public partial class WaveEditor : Form, ISampleProvider
    {
        #region Fields
        /// <summary>My logger.</summary>
        readonly Logger _logger = LogManager.CreateLogger("WaveEditor");
        #endregion

        UndoStack _stack = new();

        /// <summary>The full buffer from client.</summary>
        float[] _vals = Array.Empty<float>();

        /// <summary>For drawing.</summary>
        readonly Pen _pen = new(Color.Black, 1);

        /// <summary>For drawing.</summary>
        readonly Pen _penMarker = new(Color.Black, 2);

        /// <summary>For drawing text.</summary>
        readonly Font _textFont = new("Cascadia", 12, FontStyle.Regular, GraphicsUnit.Point, 0);

        /// <summary>For drawing text.</summary>
        readonly StringFormat _format = new() { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center };


        #region Properties
        /// <summary>Gets the WaveFormat of this Sample Provider. ISampleProvider implementation.</summary>
        /// <value>The wave format.</value>
        public WaveFormat WaveFormat { get; } = WaveFormat.CreateIeeeFloatWaveFormat(44100, 1);

        /// <summary>The rendered data for client playing.</summary>
        public ISampleProvider RenderedSampleProvider { get; private set; }

        /// <summary>Current file.</summary>
        public string FileName { get; private set; } = "";

        /// <summary>Edit flag.</summary>
        public bool Dirty { get; set; } = false;

        /// <summary>For styling.</summary>
        public Color DrawColor { get { return _pen.Color; } set { _pen.Color = value; } }

        // /// <summary>For styling.</summary>
        // public Color MarkerColor { get { return _penMarker.Color; } set { _penMarker.Color = value; } }

        // /// <summary>Snap to this increment value.</summary>
        // public float SnapSamples { get; set; } = 0;

        /// <summary>Marker index.</summary>
        public int Marker
        {
            get { return _marker; }
            set { _marker = MathUtils.Constrain(value, 0, _vals.Length); Invalidate(); }
        }
        int _marker = -1;

        // /// <summary>Selection start index.</summary>
        // public int SelStart
        // {
        //     get { return _selStart; }
        //     set { _selStart = MathUtils.Constrain(value, 0, _rawVals.Length); Invalidate(); }
        // }
        // int _selStart = -1;

        // /// <summary>Selection length.</summary>
        // public int SelLength
        // {
        //     get { return _selLen; }
        //     set { _selLen = MathUtils.Constrain(value, 0, _rawVals.Length - _selStart); Invalidate(); }
        // }
        // int _selLen = 0;

        // /// <summary>Visible start index.</summary>
        // public int VisStart
        // {
        //     get { return _visStart; }
        //     set { _visStart = MathUtils.Constrain(value, 0, _rawVals.Length); Invalidate(); }
        // }
        // int _visStart = -1;

        // /// <summary>Selection length.</summary>
        // public int VisLength
        // {
        //     get { return _visLen; }
        //     set { _visLen = MathUtils.Constrain(value, 0, _rawVals.Length); Invalidate(); }
        // }
        // int _visLen = 0;
        #endregion

        /// <summary>
        /// Fill the buffer with 32 bit floating point samples. ISampleProvider implementation.
        /// </summary>
        /// <param name="vals">The buffer to fill with samples.</param>
        /// <param name="offset">Offset into buffer</param>
        /// <param name="count">The number of samples to read</param>
        /// <returns>the number of samples written to the buffer.</returns>
        public int Read(float[] vals, int offset, int count)
        {
            throw new NotImplementedException();
        }

        // TODO implement these.
        public void SetPosition()
        {
        }

        public float[] Cut()
        {
            var ret = Array.Empty<float>();



            return ret;
        }

        public float[] Copy()
        {
            var ret = Array.Empty<float>();



            return ret;
        }

        public void Paste(float[] data)
        {

        }

        public void Replace(float[] data)
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
        public WaveEditor(float[] buff, string fn)
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);

            InitializeComponent();

            //bool ok = true;
            _vals = vals;
            FileName = fn;
            Text = fn;
            Icon = Properties.Resources.tiger;

            gain.ValueChanged += (_, __) => { }; // TODO



            //ReadData();

            waveViewerNav.Init(_vals);
            waveViewerEdit.Init(_vals);

            //TODO? timeBar.Length = _reader.TotalTime;
            timeBar.Start = TimeSpan.Zero;
            timeBar.End = TimeSpan.Zero;
            timeBar.Current = TimeSpan.Zero;


            timeBar.SnapMsec = 10;
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


        void TimeBar_CurrentTimeChanged(object? sender, EventArgs e)
        {
            // txtInfo.AppendText($"Current time:{timeBar.Current}");
        }

        void txtBPM_KeyPress(object? sender, KeyPressEventArgs e)
        {
            KeyUtils.TestForNumber_KeyPress(sender!, e);
        }

        /// <summary>
        /// Paints the waveform.
        /// </summary>
        protected override void OnPaint(PaintEventArgs pe)
        {
            // Setup.
            pe.Graphics.Clear(BackColor);

            if (_vals is null || _vals.Length == 0)
            {
                pe.Graphics.DrawString("No data", _textFont, Brushes.Gray, ClientRectangle, _format);
            }
            else
            {
                // https://stackoverflow.com/a/1215472
                int border = 5;
                float fitWidth = Width - (2 * border);
                float fitHeight = Height - (2 * border);
                float numVals = _vals.Length;


                //float zoom = 0.01f;
                //size *= zoom;

                for (int index = 0; index < fitWidth; index++)
                {
                    // determine start and end points within WAV
                    float start = index * (numVals / fitWidth);
                    float end = (index + 1) * (numVals / fitWidth);
                    float min = float.MaxValue;
                    float max = float.MinValue;
                    for (int i = (int)start; i < end; i++)
                    {
                        float val = _vals[i];
                        min = val < min ? val : min;
                        max = val > max ? val : max;
                    }
                    float yMax = border + fitHeight - ((max + 1) * 0.5f * fitHeight);
                    float yMin = border + fitHeight - ((min + 1) * 0.5f * fitHeight);
                    pe.Graphics.DrawLine(_pen, index + border, yMax, index + border, yMin);
                }

                // Draw  marker.
                if (_marker > 0)
                {
                    int mpos = (int)(_marker * (fitWidth / numVals));
                    //int x = _smplPerPixel > 0 ? _selStart / _smplPerPixel : _selStart;
                    pe.Graphics.DrawLine(_penMarker, mpos, 0, mpos, Height);
                }
            }
        }




        /// <summary>
        /// Handle mouse position changes.
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
           if (e.Button == MouseButtons.Left)
           {
               _current = GetTimeFromMouse(e.X);
               CurrentTimeChanged?.Invoke(this, new EventArgs());
           }
           else
           {
               if (e.X != _lastXPos)
               {
                   TimeSpan ts = GetTimeFromMouse(e.X);
                   _toolTip.SetToolTip(this, ts.ToString(TS_FORMAT));
                   _lastXPos = e.X;
               }
           }

           Invalidate();
           base.OnMouseMove(e);
        }

        /// <summary>
        /// Handle dragging.
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
           if (ModifierKeys.HasFlag(Keys.Control))
           {
               _start = GetTimeFromMouse(e.X);
           }
           else if (ModifierKeys.HasFlag(Keys.Alt))
           {
               _end = GetTimeFromMouse(e.X);
           }
           else
           {
               _current = GetTimeFromMouse(e.X);
           }

           CurrentTimeChanged?.Invoke(this, new EventArgs());
           Invalidate();
           base.OnMouseDown(e);
        }


        /// <summary>
        /// Convert x pos to TimeSpan.
        /// </summary>
        /// <param name="x"></param>
        TimeSpan GetTimeFromMouse(int x)
        {
           int msec = 0;

           if (_current.TotalMilliseconds < _length.TotalMilliseconds)
           {
               msec = x * (int)_length.TotalMilliseconds / Width;
               msec = MathUtils.Constrain(msec, 0, (int)_length.TotalMilliseconds);
               msec = DoSnap(msec);
           }
           return new TimeSpan(0, 0, 0, 0, msec);
        }

        /// <summary>
        /// Snap to user preference.
        /// </summary>
        /// <param name="msec"></param>
        /// <returns></returns>
        int DoSnap(int msec)
        {
           int smsec = 0;
           if (SnapMsec > 0)
           {
               smsec = (msec / SnapMsec) * SnapMsec;
               if (SnapMsec > (msec % SnapMsec) / 2)
               {
                   smsec += SnapMsec;
               }
           }

           return smsec;
        }

        /// <summary>
        /// Utility helper function.
        /// </summary>
        /// <param name="val"></param>
        /// <param name="lower"></param>
        /// <param name="upper"></param>
        /// <returns></returns>
        TimeSpan Constrain(TimeSpan val, TimeSpan lower, TimeSpan upper)
        {
           return TimeSpan.FromMilliseconds(MathUtils.Constrain(val.TotalMilliseconds, lower.TotalMilliseconds, upper.TotalMilliseconds));
        }

        /// <summary>
        /// Map from time to UI pixels.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public int Scale(TimeSpan val)
        {
           return (int)(val.TotalMilliseconds * Width / _length.TotalMilliseconds);
        }




        #region Private functions

        #endregion
    }

}
