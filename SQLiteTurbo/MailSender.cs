using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using Common;

namespace SQLiteTurbo
{
    /// <summary>
    /// This class provides the methods needed for sending an email 
    /// </summary>
    public class MailSender
    {
        /// <summary>
        /// Compute default timeout based on the size of the attachment files
        /// </summary>
        /// <param name="attachments">Listof attachment files to sends</param>
        /// <returns>Timeout in seconds</returns>
        public static int ComputeTimeout(List<string> attachments)
        {
            long total = 0;
            foreach (string fpath in attachments)
            {
                FileInfo fi = new FileInfo(fpath);
                total += fi.Length;
            } // foreach

            // Compute timeout (seconds) based on the assumption that it takes 1 seconds to pass 15KB of data (upload)
            int timeout = (int)(10 + total / (15 * 1024));
            return timeout;
        }

        /// <summary>
        /// Cancel an ongoing email sending
        /// </summary>
        public static void CancelSendingViaGmail()
        {
            if (_smtp != null)
                _smtp.SendAsyncCancel();
        }

        /// <summary>
        /// Send a bug report/improvement suggestion via GMAIL
        /// </summary>
        /// <param name="isBug"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="attachments"></param>
        /// <param name="retEmail"></param>
        public static void SendMailViaGmail(bool isBug, string subject, string body, List<string> attachments, string retEmail, int timeout)
        {
            _wasCancelled = false;
            _error = null;

            string nsubject = (isBug ? "BUG: " : "FEATURE: ") + subject;

            if (retEmail != null)
            {
                body += "\r\n\r\nPlease send me a notification email when progress is made on my report.\r\n" +
                    "My email address is: " + retEmail + "\r\n";
            }

            MailMessage mail = new MailMessage();
            mail.To.Add(TO_ADDRESS);
            mail.From = new MailAddress(TO_ADDRESS);
            mail.Subject = nsubject;
            mail.Body = body;
            foreach (string fpath in attachments)
                mail.Attachments.Add(new Attachment(fpath));

            if (_smtp == null)
            {
                _smtp = new SmtpClient();
                _smtp.Host = SMTP_HOST;
                _smtp.Port = SMTP_PORT;
                _smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                _smtp.Credentials = new NetworkCredential(USER_NAME, PASSWORD);
                _smtp.EnableSsl = true;
                _smtp.SendCompleted += new SendCompletedEventHandler(_smtp_SendCompleted);
            }

            _wait.Reset();
            _smtp.Timeout = timeout;
            _msg = mail;
            _smtp.SendAsync(mail, null);
            _wait.WaitOne();

            if (_error != null)
            {
                throw _error;
            }
            else if (_wasCancelled)
                throw new UserCancellationException();
        }

        /// <summary>
        /// Send a report/bug email using the default email client
        /// </summary>
        /// <param name="isBug"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="attachments"></param>
        /// <param name="retEmail"></param>
        public static void SendMail(bool isBug, string subject, string body, List<string> attachments, string retEmail)
        {
            subject = EncodeURL((isBug ? "BUG: " : "FEATURE: ") + subject);

            StringBuilder sb = new StringBuilder();
            sb.Append(body);
            if (retEmail != null)
            {
                sb.Append("\r\n\r\nPlease send me a notification email when progress is made on my report.\r\n" +
                    "My email address is: " + retEmail + "\r\n");
            }
            body = EncodeURL(sb.ToString());

            StringBuilder url = new StringBuilder();
            url.Append("mailto:"+TO_ADDRESS);
            url.Append("&subject=" + subject);
            url.Append("&body=" + body);
            foreach (string fpath in attachments)
                url.Append("&Attach=" + (Char)34 + EncodeURL(fpath) + (Char)34);

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = url.ToString();
            psi.UseShellExecute = true;
            psi.RedirectStandardOutput = false;

            Process proc = new Process();
            proc.StartInfo = psi;
            proc.Start();
            proc.Dispose();
        }

        #region Private Methods
        private static void _smtp_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (_msg != null)
            {
                // Dispose from the mail message
                IDisposable idsp = (IDisposable)_msg;
                idsp.Dispose();
            }

            if (e.Cancelled)
                _wasCancelled = true;
            if (e.Error != null)
                _error = e.Error;
            _wait.Set();
        }

        private static string EncodeURL(string url)
        {
            return url.Replace("\n", "%0a%0d");
        }
        #endregion

        #region Constants
        private const string TO_ADDRESS = "liron.levi@gmail.com";
        private const string SMTP_HOST = "smtp.gmail.com";
        private const int SMTP_PORT = 587;
        private const string PASSWORD = "601|604l";
        private const string USER_NAME = TO_ADDRESS;
        #endregion

        #region Private Variables
        private static MailMessage _msg;
        private static SmtpClient _smtp;
        private static AutoResetEvent _wait = new AutoResetEvent(false);
        private static bool _wasCancelled;
        private static Exception _error;
        #endregion
    }
}
