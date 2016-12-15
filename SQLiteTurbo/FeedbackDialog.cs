using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SQLiteTurbo
{
    public partial class FeedbackDialog : Form
    {
        public FeedbackDialog()
        {
            InitializeComponent();
        }

        private void FeedbackDialog_Load(object sender, EventArgs e)
        {
            _selected = pbxIdea;
            UpdateState();
        }

        private void FeedbackDialog_MouseDown(object sender, MouseEventArgs e)
        {
            Rectangle rect = new Rectangle(0, 0, 50, 50);
            if (rect.Contains(e.Location))
                this.Close();
        }

        private void pbxProblem_Click(object sender, EventArgs e)
        {
            _selected = pbxProblem;
            UpdateState();
        }

        private void pbxQuestion_Click(object sender, EventArgs e)
        {
            _selected = pbxQuestion;
            UpdateState();
        }

        private void pbxIdea_Click(object sender, EventArgs e)
        {
            _selected = pbxIdea;
            UpdateState();
        }

        private void btnSendFeedback_Click(object sender, EventArgs e)
        {
            string type = "Unknown feedback type";
            if (_selected == pbxIdea)
                type = "IDEA";
            else if (_selected == pbxProblem)
                type = "PROBLEM";
            else if (_selected == pbxQuestion)
                type = "QUESTION";

            string email = string.Empty;
            if (_rx.IsMatch(txtEmail.Text.Trim()))
                email = txtEmail.Text.Trim();

            string licenseType = "Freeware";

            string customer = "Anonymous";

            PostSubmitter ps = new PostSubmitter();
            ps.Url = "http://www.sqlitecompare.com/support_submit.php";
            ps.PostItems.Add("subject", "Feedback ["+type+"]");
            ps.PostItems.Add("description", "FEEDBACK REPORT (license = ["+licenseType+"]):\r\n\r\n"+txtFeedback.Text);
            ps.PostItems.Add("email", email);
            ps.PostItems.Add("os_version", Environment.OSVersion.ToString());
            ps.PostItems.Add("sw_version", Utils.GetSoftwareVersion() + " build " + Utils.GetSoftwareBuild());
            ps.PostItems.Add("name", customer);
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
                    "Thank you for your feedback. We'll do our best to incorporate any useful idea into SQLite Compare "+
                    "and fix any reported bug as soon as possible.",
                    "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            } // els
        }

        private void UpdateState()
        {
            pbxIdea.Image = _selected == pbxIdea ? Properties.Resources.feedback_selected_idea : Properties.Resources.feedback_non_selected_idea;
            pbxProblem.Image = _selected == pbxProblem ? Properties.Resources.feedback_problem_selected : Properties.Resources.feedback_problem_non_selected;
            pbxQuestion.Image = _selected == pbxQuestion ? Properties.Resources.feedback_selected_question : Properties.Resources.feedback_non_selected_question;

            if (_selected == pbxIdea)
                lblMessage.Text = "Share an idea";
            else if (_selected == pbxProblem)
                lblMessage.Text = "Report a problem";
            else if (_selected == pbxQuestion)
                lblMessage.Text = "Ask a question and get help";
        }

        private PictureBox _selected = null;
        private Regex _rx = new Regex(@"\b[a-zA-Z0-9._%-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}\b");
    }
}