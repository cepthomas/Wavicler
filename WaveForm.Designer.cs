using NBagOfUis;
using AudioLib;

namespace Wavicler
{
    partial class WaveForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timeBar = new AudioLib.TimeBar();
            this.gain = new NBagOfUis.Slider();
            this.waveViewerEdit = new AudioLib.WaveViewer();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.waveViewerNav = new AudioLib.WaveViewer();
            this.SuspendLayout();
            // 
            // timeBar
            // 
            this.timeBar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.timeBar.FontLarge = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.timeBar.FontSmall = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.timeBar.Location = new System.Drawing.Point(12, 13);
            this.timeBar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.timeBar.MarkerColor = System.Drawing.Color.Black;
            this.timeBar.Name = "timeBar";
            this.timeBar.ProgressColor = System.Drawing.Color.Orange;
            this.timeBar.Size = new System.Drawing.Size(327, 64);
            this.timeBar.SnapMsec = 0;
            this.timeBar.TabIndex = 24;
            // 
            // gain
            // 
            this.gain.BackColor = System.Drawing.Color.Gainsboro;
            this.gain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.gain.DrawColor = System.Drawing.Color.Blue;
            this.gain.Label = "Gain";
            this.gain.Location = new System.Drawing.Point(346, 14);
            this.gain.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.gain.Maximum = 2D;
            this.gain.Minimum = 0D;
            this.gain.Name = "gain";
            this.gain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.gain.Resolution = 0.01D;
            this.gain.Size = new System.Drawing.Size(176, 63);
            this.gain.TabIndex = 36;
            this.toolTip1.SetToolTip(this.gain, "Gain");
            this.gain.Value = 1D;
            // 
            // waveViewerEdit
            // 
            this.waveViewerEdit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.waveViewerEdit.DrawColor = System.Drawing.Color.Orange;
            this.waveViewerEdit.Location = new System.Drawing.Point(12, 165);
            this.waveViewerEdit.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.waveViewerEdit.Marker1 = -1;
            this.waveViewerEdit.Marker2 = -1;
            this.waveViewerEdit.MarkerColor = System.Drawing.Color.Black;
            this.waveViewerEdit.Mode = AudioLib.WaveViewer.DrawMode.Envelope;
            this.waveViewerEdit.Name = "waveViewerEdit";
            this.waveViewerEdit.Size = new System.Drawing.Size(1156, 176);
            this.waveViewerEdit.TabIndex = 26;
            // 
            // waveViewerNav
            // 
            this.waveViewerNav.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.waveViewerNav.DrawColor = System.Drawing.Color.Orange;
            this.waveViewerNav.Location = new System.Drawing.Point(12, 86);
            this.waveViewerNav.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.waveViewerNav.Marker1 = -1;
            this.waveViewerNav.Marker2 = -1;
            this.waveViewerNav.MarkerColor = System.Drawing.Color.Black;
            this.waveViewerNav.Mode = AudioLib.WaveViewer.DrawMode.Envelope;
            this.waveViewerNav.Name = "waveViewerNav";
            this.waveViewerNav.Size = new System.Drawing.Size(1156, 71);
            this.waveViewerNav.TabIndex = 34;
            this.toolTip1.SetToolTip(this.waveViewerNav, "Navigation");
            // 
            // WaveForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1182, 355);
            this.Controls.Add(this.waveViewerNav);
            this.Controls.Add(this.waveViewerEdit);
            this.Controls.Add(this.timeBar);
            this.Controls.Add(this.gain);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "WaveForm";
            this.Text = "  ";
            this.ResumeLayout(false);

        }

        #endregion
        
        private TimeBar timeBar;
        private Slider gain;
        private WaveViewer waveViewerNav;
        private WaveViewer waveViewerEdit;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}