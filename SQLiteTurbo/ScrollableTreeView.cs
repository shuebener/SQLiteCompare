using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;

namespace SQLiteTurbo
{
    public class ScrollableTreeView : TreeView
    {
        #region Events
        public event EventHandler GotMessage;
        #endregion

        #region Public Methods
        public void SetScrollPosition(int pos)
        {
            if (!this.IsHandleCreated)
                this.CreateControl();

            Message m = new Message();
            m.HWnd = this.Handle;
            m.LParam = IntPtr.Zero;
            m.Msg = 277;
            m.WParam = new IntPtr(0x00000005 | (pos << 16));

            base.WndProc(ref m);
        }

        public int GetScrollPosition()
        {
            return GetScrollPos(this.Handle.ToInt32(), SB_VERT);
        }
        #endregion

        #region Protected Methods
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 277 || m.Msg == 0x100 || m.Msg == WM_MOUSEWHEEL)
            {
                if (GotMessage != null)
                    GotMessage(this, EventArgs.Empty);
            }
        }
        #endregion

        #region Private Constants
        private const int SB_HORZ = 0x0;
        private const int SB_VERT = 0x1;
        private const int WM_MOUSEWHEEL = 0x020A;
        #endregion

        #region Private Methods
        [DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern int GetScrollPos(int hWnd, int nBar);

        [DllImport("user32.dll")]
        static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);
        #endregion
    }
}
