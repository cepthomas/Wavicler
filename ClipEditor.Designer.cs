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
            this.wvData = new AudioLib.WaveViewer();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.wvNav = new AudioLib.WaveViewer();
            this.SuspendLayout();
            // 
            // wvData
            // 
            this.wvData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.wvData.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.wvData.ContextMenuStrip = this.contextMenu;
            this.wvData.Gain = 1F;
            this.wvData.Location = new System.Drawing.Point(13, 13);
            this.wvData.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.wvData.Name = "wvData";
            this.wvData.Size = new System.Drawing.Size(1030, 321);
            this.wvData.TabIndex = 34;
            // 
            // contextMenu
            // 
            this.contextMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(61, 4);
            // 
            // wvNav
            // 
            this.wvNav.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.wvNav.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.wvNav.Gain = 1F;
            this.wvNav.Location = new System.Drawing.Point(13, 343);
            this.wvNav.Name = "wvNav";
            this.wvNav.Size = new System.Drawing.Size(1030, 33);
            this.wvNav.TabIndex = 41;
            // 
            // ClipEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.wvNav);
            this.Controls.Add(this.wvData);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ClipEditor";
            this.Size = new System.Drawing.Size(1057, 386);
            this.ResumeLayout(false);

        }

        #endregion
        private WaveViewer wvData;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private WaveViewer wvNav;
    }
}