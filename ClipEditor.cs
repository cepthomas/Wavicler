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
using System.ComponentModel;
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
        /// <summary>The selected/rendered data for client playing or persisting.</summary>
        public ISampleProvider SelectionSampleProvider { get; private set; }

        /// <summary>Current file.</summary>
        public string FileName { get; private set; } = "";

        /// <summary>Edited flag.</summary>
        public bool Dirty { get; set; } = false;

        /// <summary>For styling.</summary>
        public Color DrawColor
        {
            get { return waveViewer.DrawColor; }
            set { waveViewer.DrawColor = value; waveNav.DrawColor = value; }
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

        /// <summary>Gain adjustment.</summary>
        public double Gain
        {
            get { return waveViewer.Gain; }
            set { waveViewer.Gain = (float)value; }
        }
        #endregion

        #region Events
        /// <summary>Ask the parent to do something.</summary>
        public event EventHandler<ServiceRequestEventArgs>? ServiceRequestEvent;
        public enum ServiceRequest { CopySelectionToNewClip, Close }

        public class ServiceRequestEventArgs
        {
            public ServiceRequest Request { get; set; }
        }
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

            // Main wave.
            SelectionSampleProvider = waveViewer;
            waveViewer.Init(prov, false);
            
            // Navigation.
            waveNav.Init(prov, true);
            waveNav.MarkerChangedEvent += (_, __) => waveViewer.Center(waveNav.Marker);

            contextMenu.Opening += (_, __) =>
            {
                contextMenu.Items.Clear();
                contextMenu.Items.Add("Fit Gain", null, (_, __) => waveViewer.FitGain());
                contextMenu.Items.Add("Reset Gain", null, (_, __) => waveViewer.Gain = 1.0f);
                contextMenu.Items.Add("Remove Marker", null, (_, __) => waveViewer.Marker = -1);
                contextMenu.Items.Add("Copy To New Clip", null, (_, __) =>
                {
                    ServiceRequestEvent?.Invoke(this, new() { Request = ServiceRequest.CopySelectionToNewClip });
                });
                contextMenu.Items.Add("Close", null, (_, __) =>
                {
                    ServiceRequestEvent?.Invoke(this, new() { Request = ServiceRequest.Close });
                });
            };
        }

        /// <summary>
        /// Form is legal now. Init things that want to log.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            // _logger.Info($"OK to log now!!");
            base.OnLoad(e);
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
                waveNav.Dispose();
                components.Dispose();
            }

            base.Dispose(disposing);
        }
        #endregion
    }
}
