using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteTurbo
{
    /// <summary>
    /// Conveys progress informatio
    /// </summary>
    public class ProgressEventArgs : EventArgs
    {
        #region Constructors
        public ProgressEventArgs(bool done, int progress, string msg, Exception error)
        {
            _done = done;
            _progress = progress;
            _msg = msg;
            _error = error;
        }
        #endregion

        #region Public Properties
        public bool IsDone
        {
            get { return _done; }
            set { _done = value; }
        }

        public int Progress
        {
            get { return _progress; }
            set { _progress = value; }
        }

        public string Message
        {
            get { return _msg; }
            set { _msg = value; }
        }

        public Exception Error
        {
            get { return _error; }
            set { _error = value; }
        }

        public ProgressEventArgs NestedProgress
        {
            get { return _nested; }
            set { _nested = value; }
        }
        #endregion

        #region Private Variables
        private bool _done;
        private int _progress;
        private string _msg;
        private Exception _error;
        private ProgressEventArgs _nested;
        #endregion
    }
}
