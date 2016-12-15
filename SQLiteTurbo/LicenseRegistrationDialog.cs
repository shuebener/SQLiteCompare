using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Security;
using Common;

namespace SQLiteTurbo
{
    public partial class LicenseRegistrationDialog : Form
    {
        public LicenseRegistrationDialog()
        {
            InitializeComponent();
        }

        #region Event Handlers
        private void LicenseRegistrationDialog_Load(object sender, EventArgs e)
        {
            UpdateState();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                LicenseManager.InstallLicense(txtLicenseFile.Text);
                this.DialogResult = DialogResult.OK;
            }
            catch (SecurityException sex)
            {
                MessageBox.Show(this,
                    "Installing a license requires administrator permissions"
                    , "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (UnauthorizedAccessException aex)
            {
                MessageBox.Show(this,
                    "Installing a license requires administrator permissions"
                    , "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this,
                    ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } // catch
        }

        private void lnkPurchase_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            WebSiteUtils.OpenBuyPage();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.AddExtension = true;
            dlg.DefaultExt = "lic";
            dlg.Filter = "License Files|*.lic|All Files|*.*";
            DialogResult res = dlg.ShowDialog(this);
            if (res == DialogResult.Cancel)
                return;

            string fpath = dlg.FileName;
            try
            {
                _license = LicenseManager.DecodeLicense(fpath);
                txtLicenseFile.Text = fpath;
                txtLicensedTo.Text = _license.CustomerName;
                if (_license.IsSiteLicense)
                    txtLicenseType.Text = LicenseManager.GetLicenseUsage(_license.Usage) + ", site license";
                else
                    txtLicenseType.Text = LicenseManager.GetLicenseUsage(_license.Usage) + ", " + _license.NumberOfSeats + " seats";
                UpdateState();
            }
            catch (Exception ex)
            {
                txtLicensedTo.Text = string.Empty;
                txtLicenseType.Text = string.Empty;
                _license = null;
                UpdateState();
                MessageBox.Show(this, "Invalid license file", "License Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } // catch
        }

        #endregion

        #region Private Methods
        private void UpdateState()
        {
            pbxLicense.Image = imageList1.Images["initial"];
            if (_license != null)
            {
                if (_license.Usage == LicenseUsage.Demo)
                {
                    if (LicenseManager.OkForEvaluation(_license))
                    {
                        pbxLicense.Image = imageList1.Images["valid"];
                        lblLicenseHasExpired.Visible = false;
                        btnOK.Enabled = true;
                    }
                    else
                    {
                        pbxLicense.Image = imageList1.Images["not-valid"];
                        lblLicenseHasExpired.Visible = true;
                        btnOK.Enabled = false;
                    }
                } // if
                else
                {
                    lblLicenseHasExpired.Visible = false;
                    pbxLicense.Image = imageList1.Images["valid"];
                    btnOK.Enabled = true;
                }
            } // if
            else
            {
                lblLicenseHasExpired.Visible = false;
                btnOK.Enabled = false;
            }
        }
        #endregion

        #region Private Variables
        private LicenseDetails _license;
        #endregion
    }
}