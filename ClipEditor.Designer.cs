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
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.waveViewer = new AudioLib.WaveViewer();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.scrollDisplay = new System.Windows.Forms.HScrollBar();
            this.SuspendLayout();
            // 
            // waveViewer
            // 
            this.waveViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.waveViewer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.waveViewer.ContextMenuStrip = this.contextMenu;
            this.waveViewer.DrawColor = System.Drawing.Color.Orange;
            this.waveViewer.Gain = 1F;
            this.waveViewer.GainIncrement = 0.05F;
            this.waveViewer.GridColor = System.Drawing.Color.LightGray;
            this.waveViewer.Location = new System.Drawing.Point(13, 13);
            this.waveViewer.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.waveViewer.Name = "waveViewer";
            this.waveViewer.MarkColor = System.Drawing.Color.OrangeRed;
            this.waveViewer.Size = new System.Drawing.Size(1030, 323);
            this.waveViewer.TabIndex = 34;
            this.toolTip.SetToolTip(this.waveViewer, "Navigation");
            // 
            // contextMenu
            // 
            this.contextMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(61, 4);
            // 
            // scrollDisplay
            // 
            this.scrollDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scrollDisplay.Location = new System.Drawing.Point(13, 345);
            this.scrollDisplay.Name = "scrollDisplay";
            this.scrollDisplay.Size = new System.Drawing.Size(1027, 26);
            this.scrollDisplay.TabIndex = 40;
            // 
            // ClipEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.scrollDisplay);
            this.Controls.Add(this.waveViewer);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ClipEditor";
            this.Size = new System.Drawing.Size(1057, 386);
            this.ResumeLayout(false);

        }

        #endregion
        private WaveViewer waveViewer;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.HScrollBar scrollDisplay;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
    }
}