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
            this.progBar = new AudioLib.ProgressBar();
            this.wvData = new AudioLib.WaveViewer();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.edSelStart = new Wavicler.ToolStripPropertyEditor();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.edSelLength = new Wavicler.ToolStripPropertyEditor();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.edMarker = new Wavicler.ToolStripPropertyEditor();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // progBar
            // 
            this.progBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progBar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.progBar.FontLarge = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.progBar.FontSmall = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.progBar.Location = new System.Drawing.Point(3, 31);
            this.progBar.Name = "progBar";
            this.progBar.Size = new System.Drawing.Size(1051, 49);
            this.progBar.TabIndex = 43;
            this.progBar.Thumbnail = null;
            this.toolTip.SetToolTip(this.progBar, "Zoom zoom");
            // 
            // wvData
            // 
            this.wvData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.wvData.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.wvData.ContextMenuStrip = this.contextMenu;
            this.wvData.Gain = 1F;
            this.wvData.Location = new System.Drawing.Point(3, 87);
            this.wvData.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.wvData.Name = "wvData";
            this.wvData.Size = new System.Drawing.Size(1051, 295);
            this.wvData.TabIndex = 34;
            // 
            // contextMenu
            // 
            this.contextMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(61, 4);
            // 
            // toolStrip
            // 
            this.toolStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.edSelStart,
            this.toolStripSeparator2,
            this.toolStripLabel3,
            this.edSelLength,
            this.toolStripSeparator3,
            this.toolStripLabel2,
            this.edMarker,
            this.toolStripSeparator4});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(1057, 31);
            this.toolStrip.TabIndex = 42;
            this.toolStrip.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(43, 28);
            this.toolStripLabel1.Text = "Start:";
            // 
            // edSelStart
            // 
            this.edSelStart.AutoSize = false;
            this.edSelStart.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.edSelStart.Name = "edSelStart";
            this.edSelStart.Size = new System.Drawing.Size(100, 28);
            this.edSelStart.Value = -1;
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 31);
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(57, 28);
            this.toolStripLabel3.Text = "Length:";
            // 
            // edSelLength
            // 
            this.edSelLength.AutoSize = false;
            this.edSelLength.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.edSelLength.Name = "edSelLength";
            this.edSelLength.Size = new System.Drawing.Size(100, 28);
            this.edSelLength.Value = -1;
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 31);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(58, 28);
            this.toolStripLabel2.Text = "Marker:";
            // 
            // edMarker
            // 
            this.edMarker.AutoSize = false;
            this.edMarker.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.edMarker.Name = "edMarker";
            this.edMarker.Size = new System.Drawing.Size(100, 28);
            this.edMarker.Value = -1;
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 31);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 28);
            // 
            // ClipEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.progBar);
            this.Controls.Add(this.toolStrip);
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
        private ToolStripPropertyEditor edSelStart;
        private ToolStripPropertyEditor edSelLength;
        private ToolStripPropertyEditor edMarker;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private ProgressBar progBar;
    }
}