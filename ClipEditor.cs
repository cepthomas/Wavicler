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

//TODO1 need something like timebar that does three flavors.

namespace Wavicler
{
    [ToolboxItem(false), Browsable(false)] // not useable in designer
    public partial class ClipEditor : UserControl
    {
        #region Fields
        /// <summary>The bound input sample provider.</summary>
        ClipSampleProvider _prov = new(Array.Empty<float>());

        /// <summary>OK color.</summary>
        readonly Color _validColor = SystemColors.Window;

        /// <summary>Not OK color.</summary>
        readonly Color _invalidColor = Color.LightPink;
        #endregion

        #region Properties
        /// <summary>The bound input sample provider.</summary>
        public ClipSampleProvider SampleProvider { get { return _prov; } }

        /// <summary>Current file.</summary>
        public string FileName { get; private set; } = "";

        /// <summary>For styling.</summary>
        public Color DrawColor { set { wvData.DrawColor = value; wvNav.DrawColor = value; } }

        /// <summary>For styling.</summary>
        public Color GridColor { set { wvData.GridColor = value; } }

        /// <summary>Gain adjustment.</summary>
        public double Gain { get { return wvData.Gain; } set { wvData.Gain = (float)value; } }
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
            wvData.Init(_prov, false);
            wvNav.Init(_prov, true);

            // Viewer events.
            wvData.ViewerChangeEvent += ProcessViewerChangeEvent;
            wvNav.ViewerChangeEvent += ProcessViewerChangeEvent;

            // User inputs.
            txtGain.KeyPress += (object? sender, KeyPressEventArgs e) => TestValid_KeyPress(sender!, e);
            txtGain.LostFocus += (_, __) => txtGain.BackColor = wvData.UpdateProperty(Property.Gain, txtGain.Text) ? _validColor : _invalidColor;

            txtMarker.KeyPress += (object? sender, KeyPressEventArgs e) => TestValid_KeyPress(sender!, e);
            txtMarker.LostFocus += (_, __) => txtMarker.BackColor = wvData.UpdateProperty(Property.Marker, txtMarker.Text) ? _validColor : _invalidColor;

            txtSelStart.KeyPress += (object? sender, KeyPressEventArgs e) => TestValid_KeyPress(sender!, e);
            txtSelStart.LostFocus += (_, __) => txtSelStart.BackColor = wvData.UpdateProperty(Property.SelStart, txtSelStart.Text) ? _validColor : _invalidColor;

            txtSelLength.KeyPress += (object? sender, KeyPressEventArgs e) => TestValid_KeyPress(sender!, e);
            txtSelLength.LostFocus += (_, __) => txtSelLength.BackColor = wvData.UpdateProperty(Property.SelLength, txtSelLength.Text) ? _validColor : _invalidColor;

            // Context menu.
            contextMenu.Items.Clear();
            contextMenu.Items.Add("Fit Gain", null, (_, __) => wvData.FitGain());
            contextMenu.Items.Add("Reset Gain", null, (_, __) => wvData.ResetGain());
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


        #endregion

        #region Private functions







        /// <summary>
        /// Allows user to enter only potentially valid characters - numbers and dp.
        /// s</summary>
        /// <param name="sender">Sender control.</param>
        /// <param name="e">Event args.</param>
        void TestValid_KeyPress(object? sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            e.Handled = !((c >= '0' && c <= '9') || (c == '.') || (c == '\b'));
        }

        /// <summary>
        /// Process viewer UI changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ProcessViewerChangeEvent(object? sender, WaveViewer.ViewerChangeEventArgs e)
        {
            switch (e.Change)
            {
                case Property.Gain when sender == wvData:
                    txtGain.Text = $"{wvData.Gain:0.00}";
                    break;

                case Property.Marker when sender == wvData:
                    txtMarker.Text = wvData.Marker.ToString();
                    break;

                case Property.SelStart when sender == wvData:
                    txtSelStart.Text = wvData.SelStart.ToString();
                    break;

                case Property.SelLength when sender == wvData:
                    txtSelLength.Text = wvData.SelLength.ToString();
                    break;

                case Property.Marker when sender == wvNav:
                    wvData.Recenter(wvNav.Marker);
                    break;

                default:
                    break;
            };
        }
        #endregion
    }
}
