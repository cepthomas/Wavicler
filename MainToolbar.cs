using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Wavicler
{
    public partial class MainToolbar : UserControl
    {
        public MainToolbar()
        {
            InitializeComponent();
        }


        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.chkPlay = new System.Windows.Forms.CheckBox();
            this.btnRewind = new System.Windows.Forms.Button();
            this.txtInfo = new NBagOfUis.TextViewer();
            this.volumeMaster = new NBagOfUis.Slider();
            this.txtBPM = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // chkPlay
            // 
            this.chkPlay.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkPlay.BackgroundImage = global::Wavicler.Properties.Resources.glyphicons_174_play;
            this.chkPlay.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.chkPlay.Location = new System.Drawing.Point(3, 0);
            this.chkPlay.Name = "chkPlay";
            this.chkPlay.Size = new System.Drawing.Size(47, 46);
            this.chkPlay.TabIndex = 37;
            this.chkPlay.Text = ">";
            this.chkPlay.UseVisualStyleBackColor = true;
            // 
            // btnRewind
            // 
            this.btnRewind.BackColor = System.Drawing.Color.MistyRose;
            this.btnRewind.BackgroundImage = global::Wavicler.Properties.Resources.glyphicons_173_rewind;
            this.btnRewind.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnRewind.Location = new System.Drawing.Point(68, 0);
            this.btnRewind.Name = "btnRewind";
            this.btnRewind.Size = new System.Drawing.Size(54, 46);
            this.btnRewind.TabIndex = 36;
            this.btnRewind.Text = "<<";
            this.btnRewind.UseVisualStyleBackColor = false;
            // 
            // txtInfo
            // 
            this.txtInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtInfo.Location = new System.Drawing.Point(324, 0);
            this.txtInfo.MaxText = 50000;
            this.txtInfo.Name = "txtInfo";
            this.txtInfo.Prompt = "> ";
            this.txtInfo.Size = new System.Drawing.Size(274, 63);
            this.txtInfo.TabIndex = 35;
            this.txtInfo.WordWrap = true;
            // 
            // volumeMaster
            // 
            this.volumeMaster.BackColor = System.Drawing.Color.Gainsboro;
            this.volumeMaster.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.volumeMaster.DrawColor = System.Drawing.Color.Orange;
            this.volumeMaster.Label = "Volume";
            this.volumeMaster.Location = new System.Drawing.Point(142, 0);
            this.volumeMaster.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.volumeMaster.Maximum = 2D;
            this.volumeMaster.Minimum = 0D;
            this.volumeMaster.Name = "volumeMaster";
            this.volumeMaster.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.volumeMaster.Resolution = 0.05D;
            this.volumeMaster.Size = new System.Drawing.Size(175, 49);
            this.volumeMaster.TabIndex = 34;
            this.volumeMaster.Value = 1D;
            // 
            // txtBPM
            // 
            this.txtBPM.Location = new System.Drawing.Point(649, 5);
            this.txtBPM.Name = "txtBPM";
            this.txtBPM.Size = new System.Drawing.Size(125, 27);
            this.txtBPM.TabIndex = 39;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(604, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 20);
            this.label1.TabIndex = 40;
            this.label1.Text = "BPM";
            // 
            // MainToolbar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.MistyRose;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtBPM);
            this.Controls.Add(this.chkPlay);
            this.Controls.Add(this.btnRewind);
            this.Controls.Add(this.txtInfo);
            this.Controls.Add(this.volumeMaster);
            this.Name = "MainToolbar";
            this.Size = new System.Drawing.Size(1049, 66);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label1;

        public System.Windows.Forms.CheckBox chkPlay;
        public System.Windows.Forms.Button btnRewind;
        public System.Windows.Forms.TextBox txtBPM;
        public NBagOfUis.TextViewer txtInfo;
        public NBagOfUis.Slider volumeMaster;
    }
}
