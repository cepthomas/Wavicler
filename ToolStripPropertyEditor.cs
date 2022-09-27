using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Windows.Forms.Design;
using NBagOfTricks;
using AudioLib;


namespace Wavicler
{
    /// <summary>Simple toolstrip container for the property editor.</summary>
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.StatusStrip)]
    public class ToolStripPropertyEditor : ToolStripControlHost
    {
        #region Fields
        /// <summary>Contained control.</summary>
        TextBox _tb;


        /// <summary>OK color.</summary>
        readonly Color _validColor = SystemColors.Window;

        /// <summary>Not OK color.</summary>
        readonly Color _invalidColor = Color.LightPink;

        #endregion

        #region Properties mapped to contained control
        /// <summary>Current value (sample) or -1 if invalid.</summary>
        public int Value { get; set; } = -1;

        /// <summary>For styling.</summary>
        public override Color BackColor { get { return _tb.BackColor; } set { _tb.BackColor = value; } }

        /// <summary>Main content.</summary>
        public override string Text { get { return _tb.Text; } set { _tb.Text = value; } }

        /// <summary>Optional border.</summary>
        public BorderStyle BorderStyle { get { return _tb.BorderStyle; } set { _tb.BorderStyle = value; } }
        #endregion

        #region Events
        /// <summary>TextBox value changed event.</summary>
        public event EventHandler? ValueChanged;
        #endregion

        /// <summary>
        /// Make one.
        /// </summary>
        public ToolStripPropertyEditor() : base(new TextBox())
        {
            _tb = (TextBox)Control;
            AutoSize = false;
            Width = _tb.Width;
        }

        /// <summary>
        /// Look at what the user entered.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ValidateProperty();
            }
            
            base.OnKeyDown(e);
        }

        /// <summary>
        /// Look at what the user entered.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLeave(EventArgs e)
        {
            ValidateProperty();
            base.OnLeave(e);
        }

        /// <summary>
        /// Allows user to enter only potentially valid characters - numbers and dot.
        /// s</summary>
        /// <param name="e">Event args.</param>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            e.Handled = !((c >= '0' && c <= '9') || (c == '.') || (c == '\b'));

            base.OnKeyPress(e);
        }

        /// <summary>
        /// Executed when done editing.
        /// </summary>
        void ValidateProperty()
        {
            int sample = Globals.ConverterOps.Parse(Text);

            if (sample >= 0)
            {
                Value = sample;
                ValueChanged?.Invoke(this, EventArgs.Empty);
                BackColor = _validColor;
            }
            else
            {
                Value = -1;
                BackColor = _invalidColor;
            }
        }
    }
}
