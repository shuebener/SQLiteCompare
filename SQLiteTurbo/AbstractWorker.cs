using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using log4net;
using Common;

namespace SQLiteTurbo
{
    /// <summary>
    /// Provides basic worker implementation for derived classes
    /// </summary>
    public class AbstractWorker : IWorker
    {
        #region Constructors
        public AbstractWorker(string workerName)
        {
            _workerName = workerName;
            _pevent = new ProgressEventArgs(false, 0, null, null);
            _pevent.NestedProgress = new ProgressEventArgs(false, 0, null, null);
        }
        #endregion

        #region IWorker Members

        /// <summary>
        /// Fired whenever the operation has progressed
        /// </summary>
        public event EventHandler<ProgressEventArgs> ProgressChanged;

        /// <summary>
        /// Begin the operation
        /// </summary>
        public void BeginWork()
        {
            if (_worker != null && _worker.IsAlive)
                throw new ApplicationException("work is already in progress");

            _result = null;
            _cancelled = false;
            ThreadStart ts = delegate
            {
                try
                {
                    NotifyPrimaryProgress(false, 0, "operation was started", null);
                    DoWork();
                    NotifyPrimaryProgress(true, 100, "operation has finished", null);
                }
                catch (UserCancellationException cex)
                {
                    _log.Debug("The user chose to cancel the operation");
                    NotifyPrimaryProgress(true, 100, null, cex);
                }
                catch (Exception ex)
                {
                    _log.Error("operation failed", ex);
                    NotifyPrimaryProgress(true, 100, null, ex);
                } // catch
            };
            _worker = new Thread(ts);
            _worker.IsBackground = true;
            _worker.Name = _workerName;

            _worker.Start();
        }

        /// <summary>
        /// Cancel the operation
        /// </summary>
        public void Cancel()
        {
            _cancelled = true;
        }

        /// <summary>
        /// Get the operation's result (valid only after the work has finished successfully).
        /// </summary>
        /// <value></value>
        public object Result
        {
            get { return _result; }
        }

        /// <summary>
        /// In case the worker supports dual progress notifications (primary/secondary)
        /// this property will return TRUE.
        /// </summary>
        /// <value></value>
        public bool SupportsDualProgress
        {
            get { return IsDualProgress; }
        }

        #endregion

        #region Protected Methods
        /// <summary>
        /// This method should be overrided in sub-classes in order to add the specific
        /// code that needs to run in order to perform the worker's job
        /// </summary>
        protected virtual void DoWork()
        {
            // Derived classes should add their implementation here
        }

        /// <summary>
        /// Derived classes should call this method in order to inform about their
        /// primary progress
        /// </summary>
        /// <param name="done">TRUE means the work is done</param>
        /// <param name="progress">The progress (scale of 0-100)</param>
        /// <param name="msg">A message if there is any, or NULL otherwise</param>
        /// <param name="error">An error exception or NULL if there is none</param>
        protected void NotifyPrimaryProgress(bool done, int progress, string msg, Exception error)
        {
            _pevent.IsDone = done;
            _pevent.Progress = progress;
            _pevent.Message = msg;
            _pevent.Error = error;

            if (ProgressChanged != null)
                ProgressChanged(this, _pevent);
        }

        /// <summary>
        /// Derived classes should call this method in order to inform about their
        /// secondary progress
        /// </summary>
        /// <param name="done">TRUE means the work is done</param>
        /// <param name="progress">The progress (scale of 0-100)</param>
        /// <param name="msg">A message if there is any, or NULL otherwise</param>
        /// <param name="error">An error exception or NULL if there is none</param>
        protected void NotifySecondaryProgress(bool done, int progress, string msg, Exception error)
        {
            _pevent.NestedProgress.IsDone = done;
            _pevent.NestedProgress.Progress = progress;
            _pevent.NestedProgress.Message = msg;
            _pevent.NestedProgress.Error = error;

            if (ProgressChanged != null)
                ProgressChanged(this, _pevent);
        }

        /// <summary>
        /// Convenience method for checking if the user cancelled the operation.
        /// </summary>
        protected void CheckCancelled()
        {
            if (_cancelled)
                throw new UserCancellationException();
        }
        #endregion

        #region Protected Properties
        /// <summary>
        /// Derived classes should call this property in order to set/get the result
        /// of the work
        /// </summary>
        protected object WorkResult
        {
            get { return _result; }
            set { _result = value; }
        }

        /// <summary>
        /// Derived classes should override this property in order to determine if the
        /// concrete worker object supports dual progress notifications or not.
        /// </summary>
        protected virtual bool IsDualProgress
        {
            get { return false; }
        }

        /// <summary>
        /// Derived classes should call this property in order to determine if the 
        /// worker object was cancelled.
        /// </summary>
        protected bool WasCancelled
        {
            get { return _cancelled; }
        }
        #endregion

        #region Private Variables
        private ProgressEventArgs _pevent;
        private bool _cancelled;
        private Thread _worker;
        private object _result;
        private string _workerName;
        private ILog _log = LogManager.GetLogger(typeof(CompareWorker));
        #endregion
    }
}
