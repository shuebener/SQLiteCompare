using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;

namespace SQLiteTurbo
{
    public class LocalUserFileAppender : AppenderSkeleton
    {
        public LocalUserFileAppender()
        {
            // Compute the path to the local application data folder where the sqlite log file will be
            // placed.
            _logpath = Configuration.LogFilePath;
            if (File.Exists(_logpath))
                File.Delete(_logpath);
            _logwriter = new StreamWriter(File.Open(_logpath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read));
        }

        [System.Obsolete("Instead use the default constructor and set the Layout property")]
        public LocalUserFileAppender(ILayout layout)
            : this()
        {
            Layout = layout;           
        }

        protected override void Append(log4net.Core.LoggingEvent loggingEvent)
        {
            if (_logwriter.BaseStream.Length > 500 * 1024)
            {
                _logwriter.Close();
                File.Delete(_logpath);
                _logwriter = new StreamWriter(File.Open(_logpath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read));
            }

            _logwriter.WriteLine("{0}|{1}|{2}|{3}|{4}",
                loggingEvent.Level.DisplayName,
                loggingEvent.ThreadName,
                loggingEvent.TimeStamp.ToShortDateString() + " " + loggingEvent.TimeStamp.ToShortTimeString() + "," + loggingEvent.TimeStamp.Millisecond,
                loggingEvent.LocationInformation.ClassName + "." + loggingEvent.LocationInformation.MethodName + "(" + loggingEvent.LocationInformation.LineNumber + ")",
                (loggingEvent.ExceptionObject != null ? (loggingEvent.ExceptionObject.Message + "\r\n" + loggingEvent.ExceptionObject.StackTrace) :
                    (loggingEvent.MessageObject != null ? loggingEvent.MessageObject.ToString() : "null")));
            _logwriter.Flush();
        }

        protected override void OnClose()
        {
            base.OnClose();
            _logwriter.Flush();
            _logwriter.Close();
        }

        protected override bool RequiresLayout
        {
            get { return false; }
        }

        private string _logpath;
        private StreamWriter _logwriter;
    }
}
