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
            this.sldGain = new NBagOfUis.Slider();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.waveViewer = new AudioLib.WaveViewer();
            this.hsbClipDisplay = new System.Windows.Forms.HScrollBar();
            this.SuspendLayout();
            // 
            // timeBar
            // 
            this.timeBar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.timeBar.FontLarge = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.timeBar.FontSmall = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.timeBar.Location = new System.Drawing.Point(70, 13);
            this.timeBar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.timeBar.MarkerColor = System.Drawing.Color.Black;
            this.timeBar.Name = "timeBar";
            this.timeBar.ProgressColor = System.Drawing.Color.Orange;
            this.timeBar.Size = new System.Drawing.Size(327, 41);
            this.timeBar.SnapMsec = 0;
            this.timeBar.TabIndex = 24;
            // 
            // sldGain
            // 
            this.sldGain.BackColor = System.Drawing.Color.Gainsboro;
            this.sldGain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.sldGain.DrawColor = System.Drawing.Color.Blue;
            this.sldGain.Label = "Gain";
            this.sldGain.Location = new System.Drawing.Point(12, 63);
            this.sldGain.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.sldGain.Maximum = 5D;
            this.sldGain.Minimum = 0D;
            this.sldGain.Name = "sldGain";
            this.sldGain.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.sldGain.Resolution = 0.01D;
            this.sldGain.Size = new System.Drawing.Size(38, 102);
            this.sldGain.TabIndex = 36;
            this.toolTip.SetToolTip(this.sldGain, "Gain");
            this.sldGain.Value = 1D;
            // 
            // waveViewer
            // 
            this.waveViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.waveViewer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.waveViewer.DrawColor = System.Drawing.Color.Orange;
            this.waveViewer.Gain = 0.8F;
            this.waveViewer.GridColor = System.Drawing.Color.LightGray;
            this.waveViewer.Location = new System.Drawing.Point(70, 62);
            this.waveViewer.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.waveViewer.Name = "waveViewer";
            this.waveViewer.SelColor = System.Drawing.Color.White;
            this.waveViewer.SelLength = 0;
            this.waveViewer.SelStart = -1;
            this.waveViewer.Size = new System.Drawing.Size(973, 274);
            this.waveViewer.SnapTODO = 0F;
            this.waveViewer.TabIndex = 34;
            this.toolTip.SetToolTip(this.waveViewer, "Navigation");
            this.waveViewer.ViewCursor = -1;
            this.waveViewer.VisLength = 0;
            this.waveViewer.VisStart = 0;
            // 
            // hsbClipDisplay
            // 
            this.hsbClipDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hsbClipDisplay.Location = new System.Drawing.Point(70, 345);
            this.hsbClipDisplay.Name = "hsbClipDisplay";
            this.hsbClipDisplay.Size = new System.Drawing.Size(970, 26);
            this.hsbClipDisplay.TabIndex = 40;
            // 
            // ClipEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.hsbClipDisplay);
            this.Controls.Add(this.waveViewer);
            this.Controls.Add(this.timeBar);
            this.Controls.Add(this.sldGain);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ClipEditor";
            this.Size = new System.Drawing.Size(1057, 386);
            this.ResumeLayout(false);

        }

        #endregion
        
        private TimeBar timeBar;
        private Slider sldGain;
        private WaveViewer waveViewer;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.HScrollBar hsbClipDisplay;
    }
}