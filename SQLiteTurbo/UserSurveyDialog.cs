using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SQLiteTurbo
{
    public partial class UserSurveyDialog : Form
    {
        #region Constructors
        public UserSurveyDialog()
        {
            InitializeComponent();
        }
        #endregion

        #region Event Handlers
        private void btnSend_Click(object sender, EventArgs e)
        {
            string description = "Problems:\r\n" + txtProblems.Text +
                "\r\n\r\nMissing Features:\r\n" + txtMissingFeatures.Text +
                "\r\n\r\nImprovements:\r\n\r\n" + txtImprovements.Text;

            PostSubmitter ps = new PostSubmitter();
            ps.Url = "http://www.sqlitecompare.com/support_submit.php";
            ps.PostItems.Add("subject", "Survey Results");
            ps.PostItems.Add("description", description);
            ps.PostItems.Add("email", txtEmail.Text.Trim());
            ps.PostItems.Add("os_version", Environment.OSVersion.ToString());
            ps.PostItems.Add("sw_version", Utils.GetSoftwareVersion() + " build " + Utils.GetSoftwareBuild());
            ps.PostItems.Add("name", txtName.Text.Trim());
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
                    "Your survey was queued for review. Thanks!",
                    "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            } // else
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion
    }
}