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
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NBagOfTricks;
using NBagOfUis;
using AudioLib;
using System.Xml.Linq;


namespace Wavicler
{
    [ToolboxItem(false), Browsable(false)] // not useable in designer
    public partial class ClipEditor : UserControl
    {
        #region Fields
        /// <summary>The bound input sample provider.</summary>
        ClipSampleProvider _prov = new(Array.Empty<float>());
        #endregion

        #region Properties
        /// <summary>The bound input sample provider.</summary>
        public ISampleProvider SampleProvider { get { return _prov; } }

        /// <summary>Current file.</summary>
        public string FileName { get; private set; } = "";

        /// <summary>For styling.</summary>
        public Color DrawColor { set { wvData.DrawColor = value; wvNav.DrawColor = value; } }

        /// <summary>For styling.</summary>
        public Color GridColor { set { wvData.GridColor = value; } }

        ///// <summary>Gain adjustment.</summary>
        //public double Gain { get { return wvData.Gain; } set { wvData.Gain = (float)value; } }
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
        /// <param name="prov">The bound input sample provider.</param>
        public ClipEditor(ClipSampleProvider prov)
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);

            InitializeComponent();

            _prov = prov;

            // Hook up provider and ui.
            wvData.Init(_prov, false);
            wvNav.Init(_prov, true);
            UpdateSettings(WaveSelectionMode.Sample, 100); // set some defaults
            // Viewer events.
            wvData.ViewerChangeEvent += ProcessViewerChangeEvent;
            wvNav.ViewerChangeEvent += ProcessViewerChangeEvent;

            // User inputs.
            txtGain.KeyPress += (object? sender, KeyPressEventArgs e) => KeyUtils.TestForNumber_KeyPress(sender!, e);
            txtGain.LostFocus += (_, __) => ProcessUiChange(UiChange.Gain);
//>>>>>>>others...



            // Context menu.
            contextMenu.Items.Clear();
            contextMenu.Items.Add("Fit Gain", null, (_, __) => wvData.FitGain());
            contextMenu.Items.Add("Reset Gain", null, (_, __) => wvData.Gain = 1.0f);
            contextMenu.Items.Add("Remove Marker", null, (_, __) => wvData.Marker = 0);
            contextMenu.Items.Add("Copy To New Clip", null, (_, __) =>
            {
                ServiceRequestEvent?.Invoke(this, new() { Request = ServiceRequest.CopySelectionToNewClip });
            });
            contextMenu.Items.Add("Close", null, (_, __) =>
            {
                ServiceRequestEvent?.Invoke(this, new() { Request = ServiceRequest.Close });
            });
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
                wvNav.Dispose();
                components.Dispose();
            }

            base.Dispose(disposing);
        }
        #endregion

        #region Public functions
        /// <summary>
        /// Update main viewer with user settings.
        /// </summary>
        /// <param name="selectionMode"></param>
        /// <param name="snap"></param>
        /// <param name="bpm"></param>
        public void UpdateSettings(WaveSelectionMode selectionMode, double bpm)
        {
            wvData.SelectionMode = selectionMode;
            wvNav.SelectionMode = selectionMode;
            wvData.BPM = (float)bpm;
        }
        #endregion

        #region Private functions


        void ProcessUiChange(UiChange change)
        {
                //public enum ViewerChange { Gain, Marker, SelStart, SelLength }


        }

        /// <summary>
        /// Process viewer UI changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ProcessViewerChangeEvent(object? sender, WaveViewer.ViewerChangeEventArgs e)
        {
            switch ((sender as WaveViewer)!.Name, e.Change)
            {
                case ("wvData", UiChange.Gain):
                    txtGain.Text = $"{wvData.Gain:0.00}";
                    break;

                case ("wvData", UiChange.Marker):
                    txtMarker.Text = wvData.Marker.ToString();
                    break;

                case ("wvData", UiChange.SelStart):
                    txtSelStart.Text = wvData.SelStart.ToString();
                    break;

                case ("wvData", UiChange.SelLength):
                    txtSelLength.Text = wvData.SelLength.ToString();
                    break;

                case ("wvNav", UiChange.Marker):
                    wvData.Recenter(wvNav.Marker);
                    break;

                default:
                    break;
            };
        }

        /// <summary>
        /// Generic UI helper. Limits input to mode specific values.
        /// s</summary>
        /// <param name="sender">Sender control.</param>
        /// <param name="e">Event args.</param>
        void TestForPositional_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (wvData.SelectionMode)
            {
                case WaveSelectionMode.Sample:
                    // Determine whether the keystroke is a number.
                    char c = e.KeyChar;
                    e.Handled = !((c >= '0' && c <= '9') || (c == '\b') || (c == '-'));

                    break;

                case WaveSelectionMode.Time:
                    //mm:ss.fff
                    //public static bool TryParse (string? input, IFormatProvider? formatProvider, out TimeSpan result);
    //TODO1                AudioTime.TryParse(string? input);
                    break;

                case WaveSelectionMode.Beat:
                    //Bar+1.Beat+1.PartBeat:00-99
                    break;
            }

            //// Determine whether the keystroke is a number.
            //char c = e.KeyChar;
            //e.Handled = !((c >= '0' && c <= '9') || (c == '.') || (c == '\b') || (c == '-'));
        }
        #endregion

    }
}
