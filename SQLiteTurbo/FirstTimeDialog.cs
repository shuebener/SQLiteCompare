using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SQLiteTurbo
{
    public partial class FirstTimeDialog : Form
    {
        public FirstTimeDialog()
        {
            InitializeComponent();
        }

        #region Public Properties
        public bool IsAutomaticUpdates
        {
            get { return _automaticUpdates; }
        }
        #endregion

        #region Event Handlers
        private void cbxAutomaticUpdates_CheckedChanged(object sender, EventArgs e)
        {
            _automaticUpdates = cbxAutomaticUpdates.Checked;
        }

        private void btnCheckNow_Click(object sender, EventArgs e)
        {
            _automaticUpdates = cbxAutomaticUpdates.Checked;
            this.DialogResult = DialogResult.OK;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            _automaticUpdates = cbxAutomaticUpdates.Checked;
            this.DialogResult = DialogResult.Cancel;
        }
        #endregion

        #region Private Variables
        private bool _automaticUpdates = false;
        #endregion
    }
}