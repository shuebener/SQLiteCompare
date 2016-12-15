using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace SQLiteTurbo
{
    public class EtchedLine : Control
    {
        #region Constructors
        public EtchedLine()
        {
            // Avoid receiving the focus.
            SetStyle(ControlStyles.Selectable, false);
        }
        #endregion

        #region Public Properties
        [Category("Appearance")]
        Color DarkColor
        {

            get { return _darkColor; }

            set
            {
                _darkColor = value;
                Refresh();
            }
        }

        [Category("Appearance")]
        Color LightColor
        {
            get { return _lightColor; }

            set
            {
                _lightColor = value;
                Refresh();
            }
        }
        #endregion

        #region Overriden Methods
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Brush lightBrush = new SolidBrush(_lightColor);
            Brush darkBrush = new SolidBrush(_darkColor);
            Pen lightPen = new Pen(lightBrush, 1);
            Pen darkPen = new Pen(darkBrush, 1);

            e.Graphics.DrawLine(darkPen, 0, 0, this.Width, 0);
            e.Graphics.DrawLine(lightPen, 0, 1, this.Width, 1);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            Refresh();
        }
        #endregion

        #region Private Variables
        private Color _darkColor = SystemColors.ControlDark;
        private Color _lightColor = SystemColors.ControlLightLight;
        #endregion
    }
}
