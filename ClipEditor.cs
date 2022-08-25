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
using AudioLib;


namespace Wavicler
{
    public partial class ClipEditor : UserControl
    {
        #region Fields
        /// <summary>My logger.</summary>
        readonly Logger _logger = LogManager.CreateLogger("ClipEditor");

        // my new >>>>>
        UndoStack _stack = new(); //TODO
        #endregion

        #region Properties
        /// <summary>The selected/rendered data for client playing.</summary>
        public ISampleProvider SelectionSampleProvider { get; private set; }

        /// <summary>Current file.</summary>
        public string FileName { get; private set; } = "";

        /// <summary>Edited flag.</summary>
        public bool Dirty { get; set; } = false;

        /// <summary>For styling.</summary>
        public Color DrawColor
        {
            get { return waveViewer.DrawColor; }
            set { waveViewer.DrawColor = value; sldGain.DrawColor = value; }
        }

        /// <summary>For styling.</summary>
        public Color GridColor
        {
            get { return waveViewer.GridColor; }
            set { waveViewer.GridColor = value; }
        }

        /// <summary>Snap control.</summary>
        public bool Snap { get; set; } = true;

        /// <summary>How to select wave.</summary>
        public SelectionMode SelectionMode { get; set; } = SelectionMode.Sample;
        #endregion

        #region Lifecycle
        /// <summary>
        /// Normal constructor.
        /// </summary>
        /// <param name="prov">Data source.</param>
        public ClipEditor(ClipSampleProvider prov)
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);

            InitializeComponent();

            SelectionSampleProvider = waveViewer;
            Text = prov.GetInfoString();

            waveViewer.Init(prov);
            waveViewer.GainChangedEvent += (_, __) => { sldGain.Value = waveViewer.Gain; };

            sldGain.ValueChanged += (_, __) => { waveViewer.Gain = (float)sldGain.Value; };
        }

        /// <summary>
        /// Form is legal now. Init things that want to log.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            // _logger.Info($"OK to log now!!");

            waveViewer.SelColor = GraphicsUtils.HalfMix(BackColor, Color.White);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                waveViewer.Dispose();
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
