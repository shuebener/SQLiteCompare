using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using log4net;
using Common;

namespace SQLiteTurbo
{
    /// <summary>
    /// General purpose progress dialog that can be used to monitor the progress
    /// of IWorker derived classes.
    /// </summary>
    public partial class ProgressDialog : Form
    {
        public ProgressDialog()
        {
            InitializeComponent();
        }

        #region Public Methods

        /// <summary>
        /// Starts the dialog in a mode that does not allow the user to cancel
        /// the operation.
        /// </summary>
        /// <param name="parent">The parent form</param>
        /// <param name="worker">The worker object to start.</param>
        public void StartNonCancellable(IWin32Window parent, CompareWorker worker)
        {
            btnCancel.Enabled = false;
            Start(parent, worker);
        }

        /// <summary>
        /// Start the worker object and show the dialog
        /// </summary>
        /// <param name="parent">The parent form</param>
        /// <param name="worker">The worker object to start.</param>
        public void Start(IWin32Window parent, IWorker worker)
        {
            if (_worker != null)
                throw new ApplicationException("progress dialog was already started");

            _worker = worker;
            _worker.ProgressChanged += new EventHandler<ProgressEventArgs>(_worker_ProgressChanged);

            // Adjust the dialog to display two progress bars or just one according to the
            // type of the worker assigned to it.
            if (worker.SupportsDualProgress)
            {
                this.Height = 170;
                pbrSecondaryProgress.Visible = true;
                lblSecondaryMsg.Visible = true;
            }
            else
            {
                this.Height = 126;
                pbrSecondaryProgress.Visible = false;
                lblSecondaryMsg.Visible = false;
            }

            this.ShowDialog(parent);
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Return the result object of the worker object
        /// </summary>
        public object Result
        {
            get 
            {
                return _result; 
            }
        }

        /// <summary>
        /// Return the error object (in case of an error)
        /// </summary>
        public Exception Error
        {
            get { return _error; }
        }
        #endregion

        #region Protected Overrided Methods
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            _worker.BeginWork();
        }
        #endregion

        #region Event Handlers
        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (btnCancel.Enabled)
                _worker.Cancel();
        }

        private void ProgressDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_closed)
                e.Cancel = true;
        }

        private void _worker_ProgressChanged(object sender, ProgressEventArgs e)
        {
            if (_closed)
                return;

            if (InvokeRequired)
                Invoke(new EventHandler<ProgressEventArgs>(_worker_ProgressChanged), sender, e);
            else
            {
                if (this.IsDisposed)
                    return;

                if (e.Error != null)
                {
                    if (!(e.Error is UserCancellationException))
                    {
                        MessageBox.Show(this, e.Error.Message, "Operation Failed", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }

                    _worker.ProgressChanged -= new EventHandler<ProgressEventArgs>(_worker_ProgressChanged);
                    _error = e.Error;
                    _result = null;
                    _closed = true;

                    _log.Debug("Closing the progress dialog...");
                    this.Close();
                    _log.Debug("Closed the progress dialog.");
                }
                else
                {
                    lblPrimaryMsg.Text = e.Message;
                    pbrPrimaryProgress.Value = e.Progress;
                    if (e.NestedProgress != null)
                    {
                        lblSecondaryMsg.Text = e.NestedProgress.Message;
                        pbrSecondaryProgress.Value = e.NestedProgress.Progress;
                    }
                    else
                    {
                        if (pbrSecondaryProgress.Value != 0)
                        {
                            pbrSecondaryProgress.Value = 0;
                            lblSecondaryMsg.Text = string.Empty;
                        }
                    } // else

                    if (e.IsDone)
                    {
                        _worker.ProgressChanged -= new EventHandler<ProgressEventArgs>(_worker_ProgressChanged);
                        _result = _worker.Result;
                        _closed = true;

                        _log.Debug("Closing the progress dialog...");
                        this.Close();
                        _log.Debug("Closed the progress dialog.");
                    }
                } // else
            } // else
        }
        #endregion

        #region Private Variables
        private bool _closed = false;
        private IWorker _worker = null;
        private object _result = null;
        private Exception _error = null;
        private ILog _log = LogManager.GetLogger(typeof(ProgressDialog));
        #endregion
    }
}