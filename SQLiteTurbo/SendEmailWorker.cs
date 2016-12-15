using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace SQLiteTurbo
{
    public class SendEmailWorker : AbstractWorker
    {
        public SendEmailWorker(bool isBug, string subject, string body, List<string> attachments, string retEmail, bool useDefaultEmailClient)
            : base("SendEmailWorker")
        {
            _isbug = isBug;
            _subject = subject;
            _body = body;
            _attachments = attachments;
            _retemail = retEmail;
            _timeout = MailSender.ComputeTimeout(attachments);
            _useDefaultEmailClient = useDefaultEmailClient;
        }


        #region Protected Overrided Methods
        protected override void DoWork()
        {
            System.Timers.Timer tm = new System.Timers.Timer(1000);
            tm.Elapsed += new System.Timers.ElapsedEventHandler(tm_Elapsed);
            tm.AutoReset = true;
            tm.Start();

            try
            {
                if (_useDefaultEmailClient)
                    MailSender.SendMail(_isbug, _subject, _body, _attachments, _retemail);
                else
                    MailSender.SendMailViaGmail(_isbug, _subject, _body, _attachments, _retemail, _timeout * 1000);
            }
            catch (Exception ex)
            {
                // General error
                throw;
            }
            finally
            {
                tm.Stop();
                tm.Dispose();
            }
        }

        

        private void tm_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // Cause the internal thread to exit if possible
            if (WasCancelled && _first)
            {
                _first = false;
                MailSender.CancelSendingViaGmail();
                return;
            }

            _elapsed++;
            int progress = 0;
            if (_timeout > 0)
                progress = (int)(100.0*_elapsed / _timeout);
            if (progress > 100)
                return;
            NotifyPrimaryProgress(false, progress, 
                string.Format("Sending email to server (estimated {0} seconds left)", _timeout-_elapsed), null);
        }

        protected override bool IsDualProgress
        {
            get
            {
                return false;
            }
        }        
        #endregion

        #region Private Variables
        private bool _isbug;
        private string _subject;
        private string _body;
        private List<string> _attachments;
        private string _retemail;
        private int _timeout;
        private int _elapsed = 0;
        private bool _first = true;
        private bool _useDefaultEmailClient;
        #endregion
    }
}
