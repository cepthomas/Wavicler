using NBagOfUis;
using AudioLib;

namespace Wavicler
{
    partial class MainForm
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
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timeBar = new AudioLib.TimeBar();
            this.meterDots = new NBagOfUis.Meter();
            this.pan1 = new NBagOfUis.Pan();
            this.meterLinear = new NBagOfUis.Meter();
            this.pot1 = new NBagOfUis.Pot();
            this.volumeMaster = new NBagOfUis.Slider();
            this.gain = new NBagOfUis.Slider();
            this.waveViewer1 = new AudioLib.WaveViewer();
            this.waveViewer2 = new AudioLib.WaveViewer();
            this.txtInfo = new NBagOfUis.TextViewer();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.ddFile = new System.Windows.Forms.ToolStripDropDownButton();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnSettings = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnRewind = new System.Windows.Forms.Button();
            this.chkPlay = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.waveViewerNav = new AudioLib.WaveViewer();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // timeBar
            // 
            this.timeBar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.timeBar.FontLarge = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.timeBar.FontSmall = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.timeBar.Location = new System.Drawing.Point(14, 101);
            this.timeBar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.timeBar.MarkerColor = System.Drawing.Color.Black;
            this.timeBar.Name = "timeBar";
            this.timeBar.ProgressColor = System.Drawing.Color.Orange;
            this.timeBar.Size = new System.Drawing.Size(327, 64);
            this.timeBar.SnapMsec = 0;
            this.timeBar.TabIndex = 24;
            // 
            // meterDots
            // 
            this.meterDots.BackColor = System.Drawing.Color.Gainsboro;
            this.meterDots.DrawColor = System.Drawing.Color.Violet;
            this.meterDots.Label = "meter dots";
            this.meterDots.Location = new System.Drawing.Point(667, 78);
            this.meterDots.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.meterDots.Maximum = 10D;
            this.meterDots.MeterType = NBagOfUis.MeterType.ContinuousDots;
            this.meterDots.Minimum = -10D;
            this.meterDots.Name = "meterDots";
            this.meterDots.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.meterDots.Size = new System.Drawing.Size(117, 36);
            this.meterDots.TabIndex = 23;
            // 
            // pan1
            // 
            this.pan1.BackColor = System.Drawing.Color.Gainsboro;
            this.pan1.DrawColor = System.Drawing.Color.Crimson;
            this.pan1.Location = new System.Drawing.Point(667, 124);
            this.pan1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pan1.Name = "pan1";
            this.pan1.Size = new System.Drawing.Size(117, 36);
            this.pan1.TabIndex = 20;
            this.pan1.Value = 0.5D;
            // 
            // meterLinear
            // 
            this.meterLinear.BackColor = System.Drawing.Color.Gainsboro;
            this.meterLinear.DrawColor = System.Drawing.Color.Orange;
            this.meterLinear.Label = "meter lin";
            this.meterLinear.Location = new System.Drawing.Point(667, 32);
            this.meterLinear.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.meterLinear.Maximum = 100D;
            this.meterLinear.MeterType = NBagOfUis.MeterType.Linear;
            this.meterLinear.Minimum = 0D;
            this.meterLinear.Name = "meterLinear";
            this.meterLinear.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.meterLinear.Size = new System.Drawing.Size(117, 36);
            this.meterLinear.TabIndex = 19;
            // 
            // pot1
            // 
            this.pot1.BackColor = System.Drawing.Color.Gainsboro;
            this.pot1.DrawColor = System.Drawing.Color.Green;
            this.pot1.ForeColor = System.Drawing.Color.Black;
            this.pot1.Label = "p99";
            this.pot1.Location = new System.Drawing.Point(351, 30);
            this.pot1.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.pot1.Maximum = 50D;
            this.pot1.Minimum = 25D;
            this.pot1.Name = "pot1";
            this.pot1.Resolution = 0.5D;
            this.pot1.Size = new System.Drawing.Size(67, 66);
            this.pot1.TabIndex = 17;
            this.pot1.Taper = NBagOfUis.Taper.Linear;
            this.pot1.Value = 35D;
            // 
            // volumeMaster
            // 
            this.volumeMaster.BackColor = System.Drawing.Color.Gainsboro;
            this.volumeMaster.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.volumeMaster.DrawColor = System.Drawing.Color.Orange;
            this.volumeMaster.Label = "Volume";
            this.volumeMaster.Location = new System.Drawing.Point(153, 30);
            this.volumeMaster.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.volumeMaster.Maximum = 2D;
            this.volumeMaster.Minimum = 0D;
            this.volumeMaster.Name = "volumeMaster";
            this.volumeMaster.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.volumeMaster.Resolution = 0.05D;
            this.volumeMaster.Size = new System.Drawing.Size(175, 49);
            this.volumeMaster.TabIndex = 18;
            this.toolTip1.SetToolTip(this.volumeMaster, "Volume");
            this.volumeMaster.Value = 1D;
            // 
            // gain
            // 
            this.gain.BackColor = System.Drawing.Color.Gainsboro;
            this.gain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.gain.DrawColor = System.Drawing.Color.Blue;
            this.gain.Label = "Gain";
            this.gain.Location = new System.Drawing.Point(457, 30);
            this.gain.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.gain.Maximum = 2D;
            this.gain.Minimum = 0D;
            this.gain.Name = "gain";
            this.gain.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.gain.Resolution = 0.01D;
            this.gain.Size = new System.Drawing.Size(49, 130);
            this.gain.TabIndex = 36;
            this.toolTip1.SetToolTip(this.gain, "Gain");
            this.gain.Value = 1D;
            // 
            // waveViewer1
            // 
            this.waveViewer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.waveViewer1.DrawColor = System.Drawing.Color.Orange;
            this.waveViewer1.Location = new System.Drawing.Point(23, 328);
            this.waveViewer1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.waveViewer1.Marker1 = -1;
            this.waveViewer1.Marker2 = -1;
            this.waveViewer1.MarkerColor = System.Drawing.Color.Black;
            this.waveViewer1.Mode = AudioLib.WaveViewer.DrawMode.Envelope;
            this.waveViewer1.Name = "waveViewer1";
            this.waveViewer1.Size = new System.Drawing.Size(1156, 143);
            this.waveViewer1.TabIndex = 26;
            // 
            // waveViewer2
            // 
            this.waveViewer2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.waveViewer2.DrawColor = System.Drawing.Color.Orange;
            this.waveViewer2.Location = new System.Drawing.Point(23, 479);
            this.waveViewer2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.waveViewer2.Marker1 = -1;
            this.waveViewer2.Marker2 = -1;
            this.waveViewer2.MarkerColor = System.Drawing.Color.Black;
            this.waveViewer2.Mode = AudioLib.WaveViewer.DrawMode.Envelope;
            this.waveViewer2.Name = "waveViewer2";
            this.waveViewer2.Size = new System.Drawing.Size(1156, 143);
            this.waveViewer2.TabIndex = 27;
            // 
            // txtInfo
            // 
            this.txtInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtInfo.Location = new System.Drawing.Point(791, 32);
            this.txtInfo.MaxText = 50000;
            this.txtInfo.Name = "txtInfo";
            this.txtInfo.Prompt = "> ";
            this.txtInfo.Size = new System.Drawing.Size(382, 133);
            this.txtInfo.TabIndex = 29;
            this.toolTip1.SetToolTip(this.txtInfo, "There\'s something you should know");
            this.txtInfo.WordWrap = true;
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ddFile,
            this.toolStripSeparator1,
            this.btnSettings,
            this.toolStripSeparator2});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1191, 27);
            this.toolStrip1.TabIndex = 30;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // ddFile
            // 
            this.ddFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.ddFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem});
            this.ddFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ddFile.Name = "ddFile";
            this.ddFile.Size = new System.Drawing.Size(46, 24);
            this.ddFile.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(128, 26);
            this.openToolStripMenuItem.Text = "Open";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // btnSettings
            // 
            this.btnSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(66, 24);
            this.btnSettings.Text = "Settings";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // btnRewind
            // 
            this.btnRewind.Location = new System.Drawing.Point(79, 30);
            this.btnRewind.Name = "btnRewind";
            this.btnRewind.Size = new System.Drawing.Size(54, 46);
            this.btnRewind.TabIndex = 32;
            this.btnRewind.Text = "<<";
            this.toolTip1.SetToolTip(this.btnRewind, "Rewind");
            this.btnRewind.UseVisualStyleBackColor = true;
            // 
            // chkPlay
            // 
            this.chkPlay.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkPlay.Location = new System.Drawing.Point(14, 30);
            this.chkPlay.Name = "chkPlay";
            this.chkPlay.Size = new System.Drawing.Size(47, 46);
            this.chkPlay.TabIndex = 33;
            this.chkPlay.Text = ">";
            this.toolTip1.SetToolTip(this.chkPlay, "Play");
            this.chkPlay.UseVisualStyleBackColor = true;
            // 
            // waveViewerNav
            // 
            this.waveViewerNav.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.waveViewerNav.DrawColor = System.Drawing.Color.Orange;
            this.waveViewerNav.Location = new System.Drawing.Point(23, 177);
            this.waveViewerNav.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.waveViewerNav.Marker1 = -1;
            this.waveViewerNav.Marker2 = -1;
            this.waveViewerNav.MarkerColor = System.Drawing.Color.Black;
            this.waveViewerNav.Mode = AudioLib.WaveViewer.DrawMode.Envelope;
            this.waveViewerNav.Name = "waveViewerNav";
            this.waveViewerNav.Size = new System.Drawing.Size(1156, 143);
            this.waveViewerNav.TabIndex = 34;
            this.toolTip1.SetToolTip(this.waveViewerNav, "Navigation");
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1191, 632);
            this.Controls.Add(this.waveViewerNav);
            this.Controls.Add(this.chkPlay);
            this.Controls.Add(this.btnRewind);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.txtInfo);
            this.Controls.Add(this.waveViewer2);
            this.Controls.Add(this.waveViewer1);
            this.Controls.Add(this.timeBar);
            this.Controls.Add(this.meterDots);
            this.Controls.Add(this.pan1);
            this.Controls.Add(this.meterLinear);
            this.Controls.Add(this.pot1);
            this.Controls.Add(this.volumeMaster);
            this.Controls.Add(this.gain);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "MainForm";
            this.Text = "  ";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        
        private System.Windows.Forms.Timer timer1;
        private TimeBar timeBar;
        private Meter meterDots;
        private Meter meterLinear;
        private Slider volumeMaster;
        private Slider gain;
        private Pan pan1;
        private Pot pot1;
        private WaveViewer waveViewer1;
        private WaveViewer waveViewer2;
        private TextViewer txtInfo;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripDropDownButton ddFile;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton btnSettings;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.Button btnRewind;
        private System.Windows.Forms.CheckBox chkPlay;
        private System.Windows.Forms.ToolTip toolTip1;
        private WaveViewer waveViewerNav;
    }
}