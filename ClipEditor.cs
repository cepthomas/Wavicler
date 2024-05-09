using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing.Design;
using System.ComponentModel;
using System.Xml.Linq;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Ephemera.NBagOfTricks;
using Ephemera.NBagOfUis;
using Ephemera.AudioLib;


namespace Wavicler
{
    [ToolboxItem(false), Browsable(false)] // not useable in designer
    public partial class ClipEditor : UserControl
    {
        #region Fields

        #endregion
        
        #region Backing fields
        readonly ClipSampleProvider _prov = new(Array.Empty<float>());
        #endregion

        #region Properties
        /// <summary>The bound input sample provider.</summary>
        public ClipSampleProvider SampleProvider { get { return _prov; } }

        /// <summary>Current file.</summary>
        public string FileName { get; private set; } = "";
        #endregion

        #region Events
        /// <summary>Ask the owner to do something.</summary>
        public event EventHandler<ServiceRequestEventArgs>? ServiceRequest;
        public enum ServiceRequestType { CopySelectionToNewClip, Close }

        public class ServiceRequestEventArgs : EventArgs
        {
            public ServiceRequestType Request { get; set; }
        }
        #endregion

        #region Lifecycle
        /// <summary>
        /// Normal constructor.
        /// </summary>
        /// <param name="prov">The bound input sample provider.</param>
        /// <param name="fn">Associated filename.</param>
        public ClipEditor(ClipSampleProvider prov, string fn)
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);

            InitializeComponent();

            FileName = fn;

            // Hook up provider and UI.
            _prov = prov;
            wvData.Init(_prov);
            wvData.WaveColor = Globals.WaveColor;
            wvData.GridColor = Globals.GridColor;
            wvData.MarkColor = Globals.MarkColor;
            wvData.TextColor = Globals.TextColor;

            _prov.Rewind();
            //_prov.ClipProgress += (object? sender, ClipSampleProvider.ClipProgressEventArgs e) => progBar.Current = (int)e.Position;
            timer.Tick += (_, __) => { progBar.Current = _prov.SampleIndex; };
            timer.Enabled = true;

            // Viewer events.
            wvData.ViewerChange += ProcessViewerChange;

            // Add some stuff to viewer context menu.
            wvData.ExtraMenuItems.Add(new ToolStripSeparator());
            wvData.ExtraMenuItems.Add(new ToolStripMenuItem(
                "Copy To New Clip",
                null,
                (_, __) => { ServiceRequest?.Invoke(this, new() { Request = ServiceRequestType.CopySelectionToNewClip }); }
            ));
            wvData.ExtraMenuItems.Add(new ToolStripMenuItem(
                "Close",
                null,
                (_, __) => { ServiceRequest?.Invoke(this, new() { Request = ServiceRequestType.Close }); }
            ));

            // Progress bar.
            progBar.ProgressColor = Globals.ControlColor;
            progBar.Length = _prov.SamplesPerChannel;
            progBar.Current = 0;
            var thumb = wvData.RenderThumbnail(progBar.Width, progBar.Height, Globals.WaveColor, SystemColors.Control, true);
            progBar.Thumbnail = thumb;
            progBar.CurrentChanged += (_, __) => { _prov.SampleIndex = progBar.Current; };
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                wvData.Dispose();
                components.Dispose();
            }

            base.Dispose(disposing);
        }
        #endregion

        #region Public functions
        /// <summary>
        /// 
        /// </summary>
        public void Rewind()
        {
            SampleProvider.SampleIndex = SampleProvider.SelStart;
            progBar.Current = SampleProvider.SelStart;
        }
        #endregion

        #region Private functions
        /// <summary>
        /// Process viewer UI changes. Update the bound sample provider.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ProcessViewerChange(object? sender, WaveViewer.ViewerChangeEventArgs e)
        {
            switch (e.Change)
            {
                case ParamChange.SelStart when sender == wvData:
                    SampleProvider.SelStart = wvData.SelStart;
                    SampleProvider.Rewind();
                    break;

                case ParamChange.SelLength when sender == wvData:
                    SampleProvider.SelLength = wvData.SelLength;
                    break;

                case ParamChange.Gain when sender == wvData:
                    SampleProvider.Gain = wvData.Gain;
                    break;

                case ParamChange.Marker when sender == wvData:
                    break;
            };
        }
        #endregion
    }
}
