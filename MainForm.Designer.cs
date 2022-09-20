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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.MenuStrip = new System.Windows.Forms.MenuStrip();
            this.FileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RecentMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.SaveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveAsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.CloseMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CloseAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ExitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resampleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitStereoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stereoToMonoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.SettingsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStrip = new System.Windows.Forms.ToolStrip();
            this.btnAutoplay = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.btnLoop = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.sldVolume = new NBagOfUis.ToolStripSlider();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.btnPlay = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.btnRewind = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.txtBPM = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.cmbSelMode = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.ftree = new NBagOfUis.FilTree();
            this.TabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.tvLog = new NBagOfUis.TextViewer();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.MenuStrip.SuspendLayout();
            this.ToolStrip.SuspendLayout();
            this.TabControl.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // MenuStrip
            // 
            this.MenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.MenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileMenuItem,
            this.ToolsMenuItem});
            this.MenuStrip.Location = new System.Drawing.Point(0, 0);
            this.MenuStrip.MdiWindowListItem = this.FileMenuItem;
            this.MenuStrip.Name = "MenuStrip";
            this.MenuStrip.Size = new System.Drawing.Size(1242, 28);
            this.MenuStrip.TabIndex = 0;
            this.MenuStrip.Text = "menuStrip";
            // 
            // FileMenuItem
            // 
            this.FileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OpenMenuItem,
            this.RecentMenuItem,
            this.toolStripSeparator3,
            this.SaveMenuItem,
            this.SaveAsMenuItem,
            this.toolStripSeparator2,
            this.CloseMenuItem,
            this.CloseAllMenuItem,
            this.toolStripSeparator1,
            this.ExitMenuItem});
            this.FileMenuItem.Name = "FileMenuItem";
            this.FileMenuItem.Size = new System.Drawing.Size(46, 24);
            this.FileMenuItem.Text = "File";
            // 
            // OpenMenuItem
            // 
            this.OpenMenuItem.Name = "OpenMenuItem";
            this.OpenMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.OpenMenuItem.Size = new System.Drawing.Size(246, 26);
            this.OpenMenuItem.Text = "Open";
            // 
            // RecentMenuItem
            // 
            this.RecentMenuItem.Name = "RecentMenuItem";
            this.RecentMenuItem.Size = new System.Drawing.Size(246, 26);
            this.RecentMenuItem.Text = "Recent";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(243, 6);
            // 
            // SaveMenuItem
            // 
            this.SaveMenuItem.Name = "SaveMenuItem";
            this.SaveMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.SaveMenuItem.Size = new System.Drawing.Size(246, 26);
            this.SaveMenuItem.Text = "Save";
            // 
            // SaveAsMenuItem
            // 
            this.SaveAsMenuItem.Name = "SaveAsMenuItem";
            this.SaveAsMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.SaveAsMenuItem.Size = new System.Drawing.Size(246, 26);
            this.SaveAsMenuItem.Text = "Save As";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(243, 6);
            // 
            // CloseMenuItem
            // 
            this.CloseMenuItem.Name = "CloseMenuItem";
            this.CloseMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
            this.CloseMenuItem.Size = new System.Drawing.Size(246, 26);
            this.CloseMenuItem.Text = "Close";
            // 
            // CloseAllMenuItem
            // 
            this.CloseAllMenuItem.Name = "CloseAllMenuItem";
            this.CloseAllMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.W)));
            this.CloseAllMenuItem.Size = new System.Drawing.Size(246, 26);
            this.CloseAllMenuItem.Text = "Close All";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(243, 6);
            // 
            // ExitMenuItem
            // 
            this.ExitMenuItem.Name = "ExitMenuItem";
            this.ExitMenuItem.Size = new System.Drawing.Size(246, 26);
            this.ExitMenuItem.Text = "Exit";
            // 
            // ToolsMenuItem
            // 
            this.ToolsMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resampleToolStripMenuItem,
            this.splitStereoToolStripMenuItem,
            this.stereoToMonoToolStripMenuItem,
            this.toolStripSeparator8,
            this.SettingsMenuItem,
            this.AboutMenuItem});
            this.ToolsMenuItem.Name = "ToolsMenuItem";
            this.ToolsMenuItem.Size = new System.Drawing.Size(58, 24);
            this.ToolsMenuItem.Text = "Tools";
            // 
            // resampleToolStripMenuItem
            // 
            this.resampleToolStripMenuItem.Name = "resampleToolStripMenuItem";
            this.resampleToolStripMenuItem.Size = new System.Drawing.Size(196, 26);
            this.resampleToolStripMenuItem.Text = "Resample";
            this.resampleToolStripMenuItem.Click += new System.EventHandler(this.ResampleMenuItem_Click);
            // 
            // splitStereoToolStripMenuItem
            // 
            this.splitStereoToolStripMenuItem.Name = "splitStereoToolStripMenuItem";
            this.splitStereoToolStripMenuItem.Size = new System.Drawing.Size(196, 26);
            this.splitStereoToolStripMenuItem.Text = "Stereo Split";
            this.splitStereoToolStripMenuItem.Click += new System.EventHandler(this.StereoSplitMenuItem_Click);
            // 
            // stereoToMonoToolStripMenuItem
            // 
            this.stereoToMonoToolStripMenuItem.Name = "stereoToMonoToolStripMenuItem";
            this.stereoToMonoToolStripMenuItem.Size = new System.Drawing.Size(196, 26);
            this.stereoToMonoToolStripMenuItem.Text = "Stereo to Mono";
            this.stereoToMonoToolStripMenuItem.Click += new System.EventHandler(this.StereoToMonoMenuItem_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(193, 6);
            // 
            // SettingsMenuItem
            // 
            this.SettingsMenuItem.Name = "SettingsMenuItem";
            this.SettingsMenuItem.Size = new System.Drawing.Size(196, 26);
            this.SettingsMenuItem.Text = "Settings";
            // 
            // AboutMenuItem
            // 
            this.AboutMenuItem.Name = "AboutMenuItem";
            this.AboutMenuItem.Size = new System.Drawing.Size(196, 26);
            this.AboutMenuItem.Text = "About";
            // 
            // ToolStrip
            // 
            this.ToolStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAutoplay,
            this.toolStripSeparator5,
            this.btnLoop,
            this.toolStripSeparator6,
            this.sldVolume,
            this.toolStripSeparator7,
            this.btnPlay,
            this.toolStripSeparator9,
            this.btnRewind,
            this.toolStripSeparator11,
            this.toolStripLabel1,
            this.txtBPM,
            this.toolStripSeparator12,
            this.toolStripLabel2,
            this.cmbSelMode,
            this.toolStripSeparator4,
            this.toolStripLabel3});
            this.ToolStrip.Location = new System.Drawing.Point(0, 28);
            this.ToolStrip.Name = "ToolStrip";
            this.ToolStrip.Size = new System.Drawing.Size(1242, 43);
            this.ToolStrip.TabIndex = 1;
            this.ToolStrip.Text = "toolStrip";
            // 
            // btnAutoplay
            // 
            this.btnAutoplay.AutoSize = false;
            this.btnAutoplay.CheckOnClick = true;
            this.btnAutoplay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAutoplay.Image = global::Wavicler.Properties.Resources.glyphicons_221_play_button;
            this.btnAutoplay.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnAutoplay.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAutoplay.Name = "btnAutoplay";
            this.btnAutoplay.Size = new System.Drawing.Size(40, 40);
            this.btnAutoplay.Text = "toolStripButton1";
            this.btnAutoplay.ToolTipText = "Autoplay the selection";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 43);
            // 
            // btnLoop
            // 
            this.btnLoop.AutoSize = false;
            this.btnLoop.CheckOnClick = true;
            this.btnLoop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnLoop.Image = global::Wavicler.Properties.Resources.glyphicons_82_refresh;
            this.btnLoop.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnLoop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLoop.Name = "btnLoop";
            this.btnLoop.Size = new System.Drawing.Size(40, 40);
            this.btnLoop.Text = "toolStripButton1";
            this.btnLoop.ToolTipText = "Loop forever";
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 43);
            // 
            // sldVolume
            // 
            this.sldVolume.AccessibleName = "sldVolume";
            this.sldVolume.AutoSize = false;
            this.sldVolume.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.sldVolume.DrawColor = System.Drawing.Color.White;
            this.sldVolume.Label = "Volume";
            this.sldVolume.Maximum = 2D;
            this.sldVolume.Minimum = 0D;
            this.sldVolume.Name = "sldVolume";
            this.sldVolume.Resolution = 0.05D;
            this.sldVolume.Size = new System.Drawing.Size(150, 40);
            this.sldVolume.Text = "vol";
            this.sldVolume.ToolTipText = "Master volume";
            this.sldVolume.Value = 1D;
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(6, 43);
            // 
            // btnPlay
            // 
            this.btnPlay.AutoSize = false;
            this.btnPlay.CheckOnClick = true;
            this.btnPlay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPlay.Image = global::Wavicler.Properties.Resources.glyphicons_174_play;
            this.btnPlay.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnPlay.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(40, 40);
            this.btnPlay.Text = "toolStripButton1";
            this.btnPlay.ToolTipText = "Play or stop";
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(6, 43);
            // 
            // btnRewind
            // 
            this.btnRewind.AutoSize = false;
            this.btnRewind.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRewind.Image = global::Wavicler.Properties.Resources.glyphicons_173_rewind;
            this.btnRewind.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnRewind.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRewind.Name = "btnRewind";
            this.btnRewind.Size = new System.Drawing.Size(40, 40);
            this.btnRewind.Text = "toolStripButton1";
            this.btnRewind.ToolTipText = "Rewind";
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(6, 43);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(42, 40);
            this.toolStripLabel1.Text = "BPM:";
            // 
            // txtBPM
            // 
            this.txtBPM.AutoSize = false;
            this.txtBPM.Name = "txtBPM";
            this.txtBPM.Size = new System.Drawing.Size(70, 43);
            this.txtBPM.ToolTipText = "BPM for beat mode";
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Size = new System.Drawing.Size(6, 43);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(52, 40);
            this.toolStripLabel2.Text = "Select:";
            // 
            // cmbSelMode
            // 
            this.cmbSelMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSelMode.Name = "cmbSelMode";
            this.cmbSelMode.Size = new System.Drawing.Size(90, 43);
            this.cmbSelMode.ToolTipText = "Selection mode";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 43);
            // 
            // ftree
            // 
            this.ftree.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.ftree.Location = new System.Drawing.Point(8, 80);
            this.ftree.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.ftree.Name = "ftree";
            this.ftree.SingleClickSelect = false;
            this.ftree.Size = new System.Drawing.Size(435, 379);
            this.ftree.TabIndex = 89;
            this.toolTip.SetToolTip(this.ftree, "File selection");
            this.ftree.FileSelectedEvent += new System.EventHandler<string>(this.Navigator_FileSelectedEvent);
            // 
            // TabControl
            // 
            this.TabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TabControl.Controls.Add(this.tabPage1);
            this.TabControl.Location = new System.Drawing.Point(445, 80);
            this.TabControl.Name = "TabControl";
            this.TabControl.SelectedIndex = 0;
            this.TabControl.Size = new System.Drawing.Size(797, 381);
            this.TabControl.TabIndex = 90;
            this.TabControl.SelectedIndexChanged += new System.EventHandler(this.TabControl_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(789, 348);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusInfo});
            this.statusStrip.Location = new System.Drawing.Point(0, 550);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1242, 26);
            this.statusStrip.TabIndex = 91;
            this.statusStrip.Text = "statusStrip1";
            // 
            // statusInfo
            // 
            this.statusInfo.Name = "statusInfo";
            this.statusInfo.Size = new System.Drawing.Size(151, 20);
            this.statusInfo.Text = "toolStripStatusLabel1";
            // 
            // tvLog
            // 
            this.tvLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tvLog.Location = new System.Drawing.Point(8, 467);
            this.tvLog.MaxText = 5000;
            this.tvLog.Name = "tvLog";
            this.tvLog.Prompt = "> ";
            this.tvLog.Size = new System.Drawing.Size(1230, 80);
            this.tvLog.TabIndex = 92;
            this.tvLog.TabStop = false;
            this.tvLog.WordWrap = true;
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(45, 40);
            this.toolStripLabel3.Text = "Snap:";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1242, 576);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.TabControl);
            this.Controls.Add(this.ToolStrip);
            this.Controls.Add(this.MenuStrip);
            this.Controls.Add(this.tvLog);
            this.Controls.Add(this.ftree);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.MenuStrip;
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.MenuStrip.ResumeLayout(false);
            this.MenuStrip.PerformLayout();
            this.ToolStrip.ResumeLayout(false);
            this.ToolStrip.PerformLayout();
            this.TabControl.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ToolStrip ToolStrip;
        private System.Windows.Forms.MenuStrip MenuStrip;
        private System.Windows.Forms.TabControl TabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ToolStripStatusLabel statusInfo;

        private System.Windows.Forms.ToolStripMenuItem FileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ToolsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AboutMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SettingsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem OpenMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RecentMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SaveMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SaveAsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem CloseMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ExitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem CloseAllMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resampleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem splitStereoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stereoToMonoToolStripMenuItem;

        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;

        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;

        private System.Windows.Forms.ToolStripTextBox txtBPM;
        private System.Windows.Forms.ToolStripButton btnPlay;
        private System.Windows.Forms.ToolStripComboBox cmbSelMode;
        private System.Windows.Forms.ToolStripButton btnAutoplay;
        private System.Windows.Forms.ToolStripButton btnLoop;
        private System.Windows.Forms.ToolStripButton btnRewind;
        private System.Windows.Forms.StatusStrip statusStrip;
        private NBagOfUis.FilTree ftree;
        private NBagOfUis.TextViewer tvLog;
        private NBagOfUis.ToolStripSlider sldVolume;
    }
}