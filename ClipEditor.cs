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
    [ToolboxItem(false), Browsable(false)] // not useable in designer
    public partial class ClipEditor : UserControl
    {
        #region Properties
        /// <summary>The selected/rendered data for client playing or persisting.</summary>
        public ISampleProvider SelectionSampleProvider { get { return waveViewer; } }

        /// <summary>Current file.</summary>
        public string FileName { get; private set; } = "";

        /// <summary>Edited flag.</summary>
        public bool Dirty { get; set; } = false;
        #endregion

        #region Properties - mainly pass through TODO just expose the wave viewer?
        /// <summary>For styling.</summary>
        public Color DrawColor { set { waveViewer.DrawColor = value; waveNav.DrawColor = value; } }

        /// <summary>For styling.</summary>
        public Color GridColor { set { waveViewer.GridColor = value; } }

        /// <summary>Snap control.</summary>
        public bool Snap { set { waveViewer.Snap = value; } }

        /// <summary>For beat mode.</summary>
        public float BPM { set { waveViewer.BPM = value; } }

        /// <summary>How to select wave.</summary>
        public WaveSelectionMode SelectionMode { set { waveViewer.SelectionMode = value; } }

        /// <summary>Gain adjustment.</summary>
        public double Gain { get { return waveViewer.Gain; } set { waveViewer.Gain = (float)value; } }
        #endregion

        #region Events
        /// <summary>Ask the owner to do something.</summary>
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
            //SelectionSampleProvider = waveViewer;
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
