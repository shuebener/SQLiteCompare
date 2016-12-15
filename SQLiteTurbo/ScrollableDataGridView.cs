using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace SQLiteTurbo
{
    /// <summary>
    /// Provides the ability to synchronize the location of the horizontal scrollbar
    /// between two data grids.
    /// </summary>
    public class ScrollableDataGridView : DataGridView
    {
        #region Public Methods

        /// <summary>
        /// Returns the maximum horizontal scroll position
        /// </summary>
        /// <returns></returns>
        public int GetMaxScrollPosition()
        {
            HScrollBar sbr = GetHScrollBar();
            return sbr.Maximum;
        }

        /// <summary>
        /// Set the position of the horizontal scrollbar
        /// </summary>
        /// <param name="pos">The position to set</param>
        public void SetScrollPosition(int pos)
        {
            HScrollBar sbr = (HScrollBar)HorizontalScrollBar;
            if (pos > sbr.Maximum)
                pos = sbr.Maximum;
            sbr.Value = pos;
            SendMessage(this.Handle, 276, (IntPtr)(4 + 0x10000 * pos), sbr.Handle);
        }

        /// <summary>
        /// Get the position of the horizontal scrollbar
        /// </summary>
        /// <returns></returns>
        public int GetScrollPosition()
        {
            HScrollBar sb = GetHScrollBar();
            return sb.Value;
        }
        #endregion

        #region Private Methods
        [DllImport("user32.dll")]
        static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, Int32 Msg, IntPtr wParam, IntPtr lParam);

        private HScrollBar GetHScrollBar()
        {
            if (_hscrollbar != null)
                return _hscrollbar;
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is HScrollBar)
                {
                    _hscrollbar = ctrl as HScrollBar;
                    return _hscrollbar;
                }
            } // foreach

            return null;
        }
        #endregion

        private HScrollBar _hscrollbar;
    }
}
