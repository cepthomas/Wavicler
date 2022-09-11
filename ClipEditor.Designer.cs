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
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.txtStart = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.txtLength = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStrip.SuspendLayout();
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
            this.wvData.Location = new System.Drawing.Point(3, 32);
            this.wvData.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.wvData.Name = "wvData";
            this.wvData.Size = new System.Drawing.Size(1051, 302);
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
            this.wvNav.Location = new System.Drawing.Point(3, 343);
            this.wvNav.Name = "wvNav";
            this.wvNav.Size = new System.Drawing.Size(1051, 33);
            this.wvNav.TabIndex = 41;
            // 
            // toolStrip
            // 
            this.toolStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel2,
            this.toolStripLabel1,
            this.txtStart,
            this.toolStripSeparator2,
            this.toolStripLabel3,
            this.txtLength,
            this.toolStripSeparator3});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(1057, 28);
            this.toolStrip.TabIndex = 42;
            this.toolStrip.Text = "toolStrip1";
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(52, 25);
            this.toolStripLabel2.Text = "Select:";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 28);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(43, 25);
            this.toolStripLabel1.Text = "Start:";
            // 
            // txtStart
            // 
            this.txtStart.AutoSize = false;
            this.txtStart.Name = "txtStart";
            this.txtStart.Size = new System.Drawing.Size(100, 28);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 28);
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(57, 25);
            this.toolStripLabel3.Text = "Length:";
            // 
            // txtLength
            // 
            this.txtLength.AutoSize = false;
            this.txtLength.Name = "txtLength";
            this.txtLength.Size = new System.Drawing.Size(100, 28);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 28);
            // 
            // ClipEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.wvNav);
            this.Controls.Add(this.wvData);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ClipEditor";
            this.Size = new System.Drawing.Size(1057, 386);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private WaveViewer wvData;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private WaveViewer wvNav;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripTextBox txtStart;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripTextBox txtLength;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
    }
}