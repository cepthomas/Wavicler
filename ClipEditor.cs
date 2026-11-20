using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
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

        /// <summary>Selection mode.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public WaveSelectionMode SelectionMode { set { wvData.SelectionMode = value; timeBar.SelectionMode = value; } }
        #endregion

        #region Events
        /// <summary>One or more controls asks the owner to do something.</summary>
        public event EventHandler<ServiceRequestEventArgs>? ServiceRequest;
        public enum ServiceRequestType { CopySelectionToNewClip, Close }

        public class ServiceRequestEventArgs : EventArgs { public ServiceRequestType Request { get; set; } }
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

            _prov.Rewind();
            //_prov.ClipProgress += (object? sender, ClipSampleProvider.ClipProgressEventArgs e) => progBar.Current = (int)e.Position;
            timer.Tick += (_, __) => { timeBar.Current = _prov.SampleIndex; };
            timer.Enabled = true;

            // Viewer events.
            wvData.StateChange += ProcessViewerStateChange;

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
            timeBar.Length = _prov.SamplesPerChannel;
            timeBar.Current = 0;
            timeBar.CurrentChanged += (_, __) => { _prov.SampleIndex = timeBar.Current; };
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
            timeBar.Current = SampleProvider.SelStart;
        }

        /// <summary>
        /// Make a pretty.
        /// </summary>
        /// <param name="settings"></param>
        public void UpdateSettings(UserSettings settings)
        {
            wvData.WaveColor = settings.WaveColor;
            wvData.GridColor = settings.GridColor;
            wvData.MarkColor = settings.MarkColor;
            wvData.TextColor = settings.TextColor;
            timeBar.ProgressColor = settings.DrawColor;
            wvData.SelectionMode = settings.SelectionMode;
            timeBar.SelectionMode = settings.SelectionMode;
            var thumb = wvData.RenderThumbnail(timeBar.Width, timeBar.Height, settings.WaveColor, SystemColors.Control, true);
            timeBar.Thumbnail = thumb;
        }
        #endregion

        #region Private functions
        /// <summary>
        /// Process viewer UI changes. Update the bound sample provider.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ProcessViewerStateChange(object? sender, WaveViewer.StateChangeEventArgs e)
        {
            switch (e.Which)
            {
                case WaveViewer.UiChange.Gain:
                    SampleProvider.Gain = wvData.Gain;
                    break;

                case WaveViewer.UiChange.SelStart:
                    SampleProvider.SelStart = wvData.SelStart;
                    SampleProvider.Rewind();
                    break;

                case WaveViewer.UiChange.SelLength:
                    SampleProvider.SelLength = wvData.SelLength;
                    break;

                case WaveViewer.UiChange.Marker:
                    //timeBar.SelLength = wv1.SelLength;
                    break;

                default:
                    break;
            }
        }
        #endregion
    }
}
