using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Common;

namespace SQLiteTurbo
{
    public partial class AboutDialog : Form
    {
        public AboutDialog()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            WebSiteUtils.OpenPage(e.LinkText);
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            WebSiteUtils.OpenProductPage();
        }

        private void AboutDialog_Load(object sender, EventArgs e)
        {
            lblVersion.Text = Utils.GetSoftwareVersion();
            lblBuild.Text = Utils.GetSoftwareBuild();
        }
    }
}