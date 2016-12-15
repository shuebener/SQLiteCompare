using System;
using System.Collections.Generic;
using System.Text;

namespace AutomaticUpdates
{
    /// <summary>
    /// Contains information regarding a bug that was fixed
    /// </summary>
    [Serializable]
    public class BugFixInfo
    {
        public BugFixInfo()
        {
        }

        public BugFixInfo(BugSeverity severity, string description)
        {
            _severity = severity;
            _description = description;
        }

        /// <summary>
        /// Get the severity level of the bug
        /// </summary>
        public BugSeverity Severity
        {
            get { return _severity; }
            set { _severity = value; }
        }

        /// <summary>
        /// Get the description of the bug
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        private BugSeverity _severity;
        private string _description;
    }

    /// <summary>
    /// Enumerates the possible severity levels for bugs
    /// </summary>
    public enum BugSeverity
    {
        None = 0,

        Minor = 1,

        Major = 2,

        Critical = 3,
    }
}
