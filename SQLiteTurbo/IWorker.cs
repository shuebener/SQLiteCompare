using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteTurbo
{
    /// <summary>
    /// Used to provide unified interface for invoking long running processes.
    /// </summary>
    public interface IWorker
    {
        /// <summary>
        /// Fired whenever the operation has progressed
        /// </summary>
        event EventHandler<ProgressEventArgs> ProgressChanged;

        /// <summary>
        /// Begin the operation
        /// </summary>
        void BeginWork();

        /// <summary>
        /// Cancel the operation
        /// </summary>
        void Cancel();

        /// <summary>
        /// Get the operation's result (valid only after the work has finished successfully).
        /// </summary>
        object Result
        {
            get;
        }

        /// <summary>
        /// In case the worker supports dual progress notifications (primary/secondary)
        /// this property will return TRUE.
        /// </summary>
        bool SupportsDualProgress
        {
            get;
        }
    }
}
