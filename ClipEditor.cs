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

        /// <summary>OK color.</summary>
        readonly Color _validColor = SystemColors.Window;

        /// <summary>Not OK color.</summary>
        readonly Color _invalidColor = Color.LightPink;
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

        /// <summary>Gain adjustment.</summary>
        public double Gain { get { return wvData.Gain; } set { wvData.Gain = (float)value; } }
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
        // /// <summary>
        // /// Update main viewer with user settings.
        // /// </summary>
        // /// <param name="selectionMode"></param>
        // /// <param name="snap"></param>
        // /// <param name="bpm"></param>
        // public void UpdateSettings(WaveSelectionMode selectionMode, double bpm)
        // {
        //     wvData.SelectionMode = selectionMode;
        //     wvNav.SelectionMode = selectionMode;
        //     wvData.BPM = (float)bpm;
        // }
        #endregion

        #region Private functions



        // // user changed ui field  - tell wv
        // void ProcessPropertyChange(Property prop)
        // {
        //     //public enum ViewerChange { Gain, Marker, SelStart, SelLength }

        //     switch (prop)
        //     {
        //         case (Property.Marker):
        //             txtMarker.BackColor = wvData.UpdateProperty(Property.Marker, txtMarker.Text) ? _validColor : _invalidColor;
        //             break;

        //         case (Property.SelStart):
        //             txtSelStart.BackColor = wvData.UpdateProperty(Property.SelStart, txtSelStart.Text) ? _validColor : _invalidColor;
        //             break;

        //         case (Property.SelLength):
        //             txtSelLength.BackColor = wvData.UpdateProperty(Property.SelLength, txtSelLength.Text) ? _validColor : _invalidColor;
        //             break;

        //         case (Property.Gain):
        //             txtGain.BackColor = wvData.UpdateProperty(Property.Gain, txtGain.Text) ? _validColor : _invalidColor;
        //             break;
        //     }
        // }



        // void ProcessUiChange(UiChange change)
        // {
        //     //public enum ViewerChange { Gain, Marker, SelStart, SelLength }

        //     switch (change, wvData.SelectionMode)
        //     {
        //         case (UiChange.Marker, WaveSelectionMode.Sample):
        //             if(int.TryParse(txtMarker.Text, out int marker))
        //             {
        //                 wvData.Marker = marker;
        //                 txtMarker.BackColor = _validColor;
        //             }
        //             else
        //             {
        //                 txtMarker.BackColor = _invalidColor;
        //             }
        //             break;

        //         case (UiChange.Marker, WaveSelectionMode.Time):
        //             var time = AudioTime.Parse(txtMarker.Text);
        //             if (time is not null)
        //             {
        //                 wvData.Marker = marker;
        //                 txtMarker.BackColor = _validColor;
        //             }
        //             else
        //             {
        //                 txtMarker.BackColor = _invalidColor;
        //             }

        //             break;

        //         case (UiChange.Marker, WaveSelectionMode.Beat):

        //             break;


        //         case (UiChange.SelStart, WaveSelectionMode.Sample):
        //             if (int.TryParse(txtSelStart.Text, out int selstart))
        //             {
        //                 wvData.SelStart = selstart;
        //                 txtSelStart.BackColor = _validColor;
        //             }
        //             else
        //             {
        //                 txtSelStart.BackColor = _invalidColor;
        //             }
        //             break;

        //         case (UiChange.SelStart, WaveSelectionMode.Time):

        //             break;

        //         case (UiChange.SelStart, WaveSelectionMode.Beat):

        //             break;


        //         case (UiChange.SelLength, WaveSelectionMode.Sample):
        //             if (int.TryParse(txtSelLength.Text, out int sellen))
        //             {
        //                 wvData.SelLength = sellen;
        //                 txtSelLength.BackColor = _validColor;
        //             }
        //             else
        //             {
        //                 txtSelLength.BackColor = _invalidColor;
        //             }
        //             break;

        //         case (UiChange.SelLength, WaveSelectionMode.Time):

        //             break;

        //         case (UiChange.SelLength, WaveSelectionMode.Beat):

        //             break;





        //         //case WaveSelectionMode.Sample:
        //         //    // Determine whether the keystroke is a number.
        //         //    char c = e.KeyChar;
        //         //    e.Handled = !((c >= '0' && c <= '9') || (c == '\b') || (c == '-'));

        //         //    break;

        //         //case WaveSelectionMode.Time:
        //         //    //mm:ss.fff
        //         //    AudioTime.TryParse(string input);
        //         //    break;

        //         //case WaveSelectionMode.Beat:
        //         //    //Bar+1.Beat+1.PartBeat:00-99
        //         //    BarBeat.TryParse(string input);
        //         //    break;


        //         case (UiChange.Gain, WaveSelectionMode.Sample):
        //         case (UiChange.Gain, WaveSelectionMode.Time):
        //         case (UiChange.Gain, WaveSelectionMode.Beat):
        //             // Later.
        //             break;
        //     }
        // }




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
            //switch (e.Change)
            //{
            //    case Property.Gain when sender == wvData:
            //        sldGain.Value = wvData.Gain;
            //        break;

            //    case Property.Marker when sender == wv2:
            //        wv1.Recenter(wv2.Marker);
            //        break;

            //    default:
            //        break;
            //};


            switch ((sender as WaveViewer)!.Name, e.Change)
            {
                case ("wvData", Property.Gain):
                    txtGain.Text = $"{wvData.Gain:0.00}";
                    break;

                case ("wvData", Property.Marker):
                    txtMarker.Text = wvData.Marker.ToString();
                    break;

                case ("wvData", Property.SelStart):
                    txtSelStart.Text = wvData.SelStart.ToString();
                    break;

                case ("wvData", Property.SelLength):
                    txtSelLength.Text = wvData.SelLength.ToString();
                    break;

                case ("wvNav", Property.Marker):
                    wvData.Recenter(wvNav.Marker);
                    break;

                default:
                    break;
            };
        }


        #endregion
    }
}
