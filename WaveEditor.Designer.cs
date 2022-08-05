using NBagOfUis;
using AudioLib;

namespace Wavicler
{
    partial class WaveEditor
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
            this.waveViewerEdit = new AudioLib.WaveViewer();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.waveViewerNav = new AudioLib.WaveViewer();
            this.label1 = new System.Windows.Forms.Label();
            this.txtBPM = new System.Windows.Forms.TextBox();
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
            // waveViewerEdit
            // 
            this.waveViewerEdit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.waveViewerEdit.DrawColor = System.Drawing.Color.Orange;
            this.waveViewerEdit.Location = new System.Drawing.Point(12, 165);
            this.waveViewerEdit.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.waveViewerEdit.Marker = -1;
            this.waveViewerEdit.Name = "waveViewerEdit";
            this.waveViewerEdit.Size = new System.Drawing.Size(1156, 176);
            this.waveViewerEdit.TabIndex = 26;
            // 
            // waveViewerNav
            // 
            this.waveViewerNav.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.waveViewerNav.DrawColor = System.Drawing.Color.Orange;
            this.waveViewerNav.Location = new System.Drawing.Point(12, 86);
            this.waveViewerNav.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.waveViewerNav.Marker = -1;
            this.waveViewerNav.Name = "waveViewerNav";
            this.waveViewerNav.Size = new System.Drawing.Size(1156, 71);
            this.waveViewerNav.TabIndex = 34;
            this.toolTip1.SetToolTip(this.waveViewerNav, "Navigation");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(559, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 20);
            this.label1.TabIndex = 37;
            this.label1.Text = "BPM";
            // 
            // txtBPM
            // 
            this.txtBPM.Location = new System.Drawing.Point(603, 25);
            this.txtBPM.Name = "txtBPM";
            this.txtBPM.Size = new System.Drawing.Size(125, 27);
            this.txtBPM.TabIndex = 38;
            this.txtBPM.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtBPM_KeyPress);
            // 
            // WaveEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1182, 355);
            this.Controls.Add(this.txtBPM);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.waveViewerNav);
            this.Controls.Add(this.waveViewerEdit);
            this.Controls.Add(this.timeBar);
            this.Controls.Add(this.gain);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "WaveEditor";
            this.Text = "  ";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        
        private TimeBar timeBar;
        private Slider gain;
        private WaveViewer waveViewerNav;
        private WaveViewer waveViewerEdit;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtBPM;
    }
}