using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace SQLiteTurbo
{
    /// <summary>
    /// The purpose of this class is to take an asynchronous worker object and
    /// turn it into a synchronouns one. This is useful in contexts where the
    /// worker is integrated into a larger operation.
    /// </summary>
    public class SyncWorker : IWorker
    {
        #region Constructors
        public SyncWorker(IWorker worker)
        {
            _worker = worker;
            _worker.ProgressChanged += new EventHandler<ProgressEventArgs>(_worker_ProgressChanged);
        }
        #endregion

        #region IWorker Members

        public event EventHandler<ProgressEventArgs> ProgressChanged;

        public void BeginWork()
        {
            _worker.BeginWork();
            _event.WaitOne();
            _event.Close();
            if (_error != null)
                throw _error;
        }

        public void Cancel()
        {
            _worker.Cancel();
        }

        public object Result
        {
            get { return _worker.Result; }
        }

        public bool SupportsDualProgress
        {
            get { return _worker.SupportsDualProgress; }
        }

        #endregion

        #region Event Handlers
        private void _worker_ProgressChanged(object sender, ProgressEventArgs e)
        {
            // Delegate to my listeners
            if (this.ProgressChanged != null)
                this.ProgressChanged(this, e);

            if (e.IsDone || e.Error != null)
            {
                if (e.Error != null)
                    _error = e.Error;
                _worker.ProgressChanged -= new EventHandler<ProgressEventArgs>(_worker_ProgressChanged);
                _event.Set();
            }
        }
        #endregion

        #region Private Variables
        private IWorker _worker;
        private Exception _error;
        private AutoResetEvent _event = new AutoResetEvent(false);
        #endregion
    }
}
