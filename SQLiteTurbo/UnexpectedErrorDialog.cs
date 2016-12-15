using System;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SQLiteTurbo
{
    public partial class UnexpectedErrorDialog : Form
    {
        public UnexpectedErrorDialog()
        {
            InitializeComponent();
        }

        #region Public Properties
        public Exception Error
        {
            get { return _error; }
            set { _error = value; }
        }
        #endregion

        #region Event Handlers

        private void UnexpectedErrorDialog_Load(object sender, EventArgs e)
        {
            if (_shown)
                this.Close();
            else
                _shown = true;
            txtErrorLog.Text = _error.ToString();
            txtLogFilePath.Text = Configuration.LogFilePath;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            PostSubmitter ps = new PostSubmitter();
            ps.Url = "http://www.sqlitecompare.com/support_submit.php";
            ps.PostItems.Add("subject", "Bug report");
            ps.PostItems.Add("description", _error.ToString()+"\r\nActions: "+txtLastActions.Text.Trim());
            ps.PostItems.Add("email", txtEmail.Text.Trim());
            ps.PostItems.Add("os_version", Environment.OSVersion.ToString());
            ps.PostItems.Add("sw_version", Utils.GetSoftwareVersion() + " build " + Utils.GetSoftwareBuild());
            ps.PostItems.Add("name", "unknown");
            ps.Type = PostSubmitter.PostTypeEnum.Post;

            HttpPostSenderDialog dlg = new HttpPostSenderDialog();
            dlg.Prepare(ps);
            DialogResult rs = dlg.ShowDialog(this);
            if (rs == DialogResult.Cancel)
                return;
            if (dlg.Error != null)
            {
                // Some kind of transmission error has occured - notify the user
                DialogResult res = MessageBox.Show(this,
                    "Failed to transfer the data to our servers. Do you want to navigate to our web support page?",
                    "Error", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                if (res == DialogResult.Yes)
                    WebSiteUtils.OpenBugFeaturePage();
            }
            else
            {
                MessageBox.Show(this,
                    "Thank you for sending us this bug report. We'll do our best to check it and incorporate a bugfix " +
                    "in the next software version.\r\nThe software will now exit.",
                    "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            } // else
        }
        #endregion

        #region Private Variables
        private Exception _error;
        private static bool _shown = false;
        #endregion
    }
}