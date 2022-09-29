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
using NBagOfTricks;
using NBagOfUis;
using AudioLib;


namespace Wavicler
{
    [ToolboxItem(false), Browsable(false)] // not useable in designer
    public partial class ClipEditor : UserControl
    {
        #region Fields
        /// <summary>The bound input sample provider.</summary>
        readonly ClipSampleProvider _prov = new(Array.Empty<float>());

        // /// <summary>OK color.</summary>
        // readonly Color _validColor = SystemColors.Window;

        // /// <summary>Not OK color.</summary>
        // readonly Color _invalidColor = Color.LightPink;
        #endregion

        #region Properties
        /// <summary>The bound input sample provider.</summary>
        public ClipSampleProvider SampleProvider { get { return _prov; } }

        ///// <summary>Current file.</summary>
        //public string FileName { get; private set; } = "";

        ///// <summary>Gain adjustment.</summary>
        //public double Gain { get { return wvData.Gain; } set { wvData.Gain = (float)value; } }
        #endregion

        #region Events
        /// <summary>Ask the owner to do something.</summary>
        public event EventHandler<ServiceRequestEventArgs>? ServiceRequestEvent;
        public enum ServiceRequest { CopySelectionToNewClip, Close }

        public class ServiceRequestEventArgs : EventArgs
        {
            public ServiceRequest Request { get; set; }
        }
        #endregion

        #region Lifecycle
        /// <summary>
        /// Normal constructor.
        /// </summary>
        /// <param name="prov">The bound input sample provider.</param>
        public ClipEditor(ClipSampleProvider prov)
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);

            InitializeComponent();

            // Hook up provider and ui.
            _prov = prov;
            wvData.Init(_prov);
            wvData.WaveColor = Globals.WaveColor;
            wvData.GridColor = Globals.GridColor;
            wvData.MarkColor = Globals.MarkColor;
            wvData.TextColor = Globals.TextColor;

            _prov.Rewind();
            _prov.ClipProgress += (object? sender, ClipSampleProvider.ClipProgressEventArgs e) => progBar.Current = (int)e.Position;

            // Viewer events.
            wvData.ViewerChangeEvent += ProcessViewerChangeEvent;

            // TODO Context menu.
            // contextMenu.Items.Clear();
            // contextMenu.Items.Add("Fit Gain", null, (_, __) => wvData.FitGain());
            // contextMenu.Items.Add("Reset Gain", null, (_, __) => wvData.ResetGain());
            // contextMenu.Items.Add("Remove Marker", null, (_, __) => wvData.Marker = 0);
            // contextMenu.Items.Add("Copy To New Clip", null, (_, __) => { ServiceRequestEvent?.Invoke(this, new() { Request = ServiceRequest.CopySelectionToNewClip }); });
            // contextMenu.Items.Add("Close", null, (_, __) => { ServiceRequestEvent?.Invoke(this, new() { Request = ServiceRequest.Close }); });

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
            SampleProvider.SampleIndex = 0;
            progBar.Current = 0;
        }
        #endregion

        //#region Private functions
        ///// <summary>
        ///// Process viewer UI changes.
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //void ProcessViewerChangeEvent(object? sender, WaveViewer.ViewerChangeEventArgs e)
        //{
        //    switch (e.Change)
        //    {
        //        case PropertyChange.Marker when sender == wvData:
        //            edMarker.Text = Globals.ConverterOps.Format(wvData.Marker);
        //            break;

        //        case PropertyChange.SelStart when sender == wvData:
        //            edSelStart.Text = Globals.ConverterOps.Format(wvData.SelStart);
        //            SampleProvider.SelStart = wvData.SelStart;
        //            break;

        //        case PropertyChange.SelLength when sender == wvData:
        //            edSelLength.Text = Globals.ConverterOps.Format(wvData.SelLength);
        //            SampleProvider.SelLength = wvData.SelLength;
        //            break;

        //        case PropertyChange.Gain when sender == wvData:
        //            SampleProvider.Gain = wvData.Gain;
        //            break;
        //    };
        //}
        //#endregion
    }
}
