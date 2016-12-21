using System;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Common;

namespace SQLiteTurbo
{
    public partial class HttpPostSenderDialog : Form
    {
        public HttpPostSenderDialog()
        {
            InitializeComponent();
        }

        public void Prepare(PostSubmitter ps)
        {
            _postSubmitter = ps;
        }

        public Exception Error
        {
            get { return _error; }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _postSubmitter.CancelPost();
        }

        private void HttpPostSenderDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_exit)
            {
                _postSubmitter.CancelPost();
                e.Cancel = true;
            }
        }

        private void HttpPostSenderDialog_Shown(object sender, EventArgs e)
        {
            WaitCallback wc = delegate
            {
                bool cancelled = false;
                _error = null;
                try
                {
                    _postSubmitter.Post();
                }
                catch (UserCancellationException uce)
                {
                    cancelled = true;
                }
                catch (Exception ex)
                {
                    _error = ex;
                } // catch

                try
                {
                    _exit = true;
                    if (cancelled)
                        Invoke(new MethodInvoker(delegate { this.DialogResult = DialogResult.Cancel; }));
                    else
                        Invoke(new MethodInvoker(delegate { this.DialogResult = DialogResult.OK; }));
                }
                catch (ObjectDisposedException ode)
                {
                     // Ignore
                } // catch
            };
            ThreadPool.QueueUserWorkItem(wc);
        }

        private PostSubmitter _postSubmitter;
        private Exception _error;
        private bool _exit;
    }
}