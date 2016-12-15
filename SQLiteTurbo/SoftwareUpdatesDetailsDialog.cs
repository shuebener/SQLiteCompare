using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using AutomaticUpdates;

namespace SQLiteTurbo
{
    public partial class SoftwareUpdatesDetailsDialog : Form
    {
        public SoftwareUpdatesDetailsDialog()
        {
            InitializeComponent();
        }

        public void Prepare(List<VersionUpdateInfo> vlist)
        {
            StringBuilder sb = new StringBuilder();
            foreach (VersionUpdateInfo v in vlist)
            {
                sb.Append("\r\n\r\n[" + v.Version + "] release date: "+v.ReleaseDate.ToLongDateString());
                if (v.FixedBugs.Count > 0)
                {
                    sb.Append("\r\n\r\nThe following bugs were fixed:");
                    foreach (BugFixInfo bug in v.FixedBugs)
                    {
                        sb.Append("\r\n(" + bug.Severity.ToString() + " bug) -> " + bug.Description);
                    } // foreach                   
                }
                if (v.AddedFeatures.Count > 0)
                {
                    sb.Append("\r\n\r\nThe following features were added:");
                    foreach (FeatureInfo feature in v.AddedFeatures)
                    {
                        sb.Append("\r\n(" + feature.Impact.ToString() + " impact) -> " + feature.Description);
                    } // foreach
                }
            } // foreach
            txtUpdates.Text = sb.ToString();
            txtUpdates.SelectionStart = 0;
            txtUpdates.SelectionLength = 0;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}