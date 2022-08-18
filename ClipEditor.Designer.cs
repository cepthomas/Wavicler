using NBagOfUis;
using AudioLib;

namespace Wavicler
{
    partial class ClipEditor
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
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.waveViewerNav = new AudioLib.WaveViewer();
            this.picClipDisplay = new System.Windows.Forms.PictureBox();
            this.hsbClipDisplay = new System.Windows.Forms.HScrollBar();
            ((System.ComponentModel.ISupportInitialize)(this.picClipDisplay)).BeginInit();
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
            // waveViewerNav
            // 
            this.waveViewerNav.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.waveViewerNav.DrawColor = System.Drawing.Color.Orange;
            this.waveViewerNav.Location = new System.Drawing.Point(12, 423);
            this.waveViewerNav.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.waveViewerNav.Marker = 0;
            this.waveViewerNav.Name = "waveViewerNav";
            this.waveViewerNav.Size = new System.Drawing.Size(1156, 71);
            this.waveViewerNav.TabIndex = 34;
            this.toolTip1.SetToolTip(this.waveViewerNav, "Navigation");
            this.waveViewerNav.YGain = 0.8F;
            // 
            // picClipDisplay
            // 
            this.picClipDisplay.Location = new System.Drawing.Point(12, 95);
            this.picClipDisplay.Name = "picClipDisplay";
            this.picClipDisplay.Size = new System.Drawing.Size(1156, 261);
            this.picClipDisplay.TabIndex = 39;
            this.picClipDisplay.TabStop = false;
            this.picClipDisplay.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ClipDisplay_KeyDown);
            this.picClipDisplay.Paint += new System.Windows.Forms.PaintEventHandler(this.ClipDisplay_Paint);
            this.picClipDisplay.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ClipDisplay_MouseDown);
            this.picClipDisplay.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.ClipDisplay_MouseWheel);
            // 
            // hsbClipDisplay
            // 
            this.hsbClipDisplay.Location = new System.Drawing.Point(12, 378);
            this.hsbClipDisplay.Name = "hsbClipDisplay";
            this.hsbClipDisplay.Size = new System.Drawing.Size(1153, 26);
            this.hsbClipDisplay.TabIndex = 40;
            // 
            // ClipEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1182, 507);
            this.Controls.Add(this.hsbClipDisplay);
            this.Controls.Add(this.picClipDisplay);
            this.Controls.Add(this.waveViewerNav);
            this.Controls.Add(this.timeBar);
            this.Controls.Add(this.gain);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ClipEditor";
            this.Text = "  ";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ClipEditor_KeyDown);
            this.Resize += new System.EventHandler(this.ClipEditor_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.picClipDisplay)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        
        private TimeBar timeBar;
        private Slider gain;
        private WaveViewer waveViewerNav;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.PictureBox picClipDisplay;
        private System.Windows.Forms.HScrollBar hsbClipDisplay;
    }
}