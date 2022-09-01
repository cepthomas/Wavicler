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
            this.waveNav = new AudioLib.WaveViewer();
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
            this.waveViewer.MarkColor = System.Drawing.Color.OrangeRed;
            this.waveViewer.Name = "waveViewer";
            this.waveViewer.ShiftIncrement = 10;
            this.waveViewer.Size = new System.Drawing.Size(1030, 321);
            this.waveViewer.TabIndex = 34;
            this.waveViewer.WheelResolution = 8;
            this.waveViewer.ZoomFactor = 20;
            // 
            // contextMenu
            // 
            this.contextMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(61, 4);
            // 
            // waveNav
            // 
            this.waveNav.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.waveNav.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.waveNav.DrawColor = System.Drawing.Color.Black;
            this.waveNav.Gain = 1F;
            this.waveNav.GainIncrement = 0.05F;
            this.waveNav.GridColor = System.Drawing.Color.LightGray;
            this.waveNav.Location = new System.Drawing.Point(13, 343);
            this.waveNav.MarkColor = System.Drawing.Color.Red;
            this.waveNav.Name = "waveNav";
            this.waveNav.ShiftIncrement = 10;
            this.waveNav.Size = new System.Drawing.Size(1030, 33);
            this.waveNav.TabIndex = 41;
            this.waveNav.WheelResolution = 8;
            this.waveNav.ZoomFactor = 20;
            // 
            // ClipEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.waveNav);
            this.Controls.Add(this.waveViewer);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ClipEditor";
            this.Size = new System.Drawing.Size(1057, 386);
            this.ResumeLayout(false);

        }

        #endregion
        private WaveViewer waveViewer;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private WaveViewer waveNav;
    }
}