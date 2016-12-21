using System;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SQLiteTurbo
{
    public partial class IndefiniteProgressDialog : Form
    {
        public IndefiniteProgressDialog()
        {
            InitializeComponent();
        }

        public void Start(IWin32Window parent, string message, WaitCallback handler, MethodInvoker cancelHandler)
        {
            btnCancel.Enabled = cancelHandler != null;                
            _handler = handler;
            _cancelHandler = cancelHandler;
            lblMessage.Text = message;
            ShowDialog(parent);
        }

        private void IndefiniteProgressDialog_Shown(object sender, EventArgs e)
        {
            WaitCallback wc = delegate
            {
                try
                {
                    _handler(null);
                    Invoke(new MethodInvoker(delegate { this.Close(); }));
                }
                catch (Exception ex)
                {
                    _error = ex;
                    Invoke(new MethodInvoker(delegate
                    {
                        this.Close();
                        System.Windows.Forms.Timer tm = new System.Windows.Forms.Timer();
                        tm.Interval = 500;
                        tm.Tick += new EventHandler(tm_Tick);
                        tm.Start();                        
                    }));
                } // catch
            };
            ThreadPool.QueueUserWorkItem(wc);
        }

        void tm_Tick(object sender, EventArgs e)
        {
            System.Windows.Forms.Timer tm = (System.Windows.Forms.Timer)sender;
            tm.Stop();
            tm.Dispose();
            MessageBox.Show(_error.Message, "Operation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);            
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (_cancelHandler != null)
                _cancelHandler();
            this.DialogResult = DialogResult.Cancel;
        }

        private WaitCallback _handler;
        private MethodInvoker _cancelHandler;
        private Exception _error;
    }
}