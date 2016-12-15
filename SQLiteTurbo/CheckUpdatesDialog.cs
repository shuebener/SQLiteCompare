using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using AutomaticUpdates;

namespace SQLiteTurbo
{
    /// <summary>
    /// Checks if new updates are available
    /// </summary>
    public partial class CheckUpdatesDialog : Form
    {
        #region Constructors
        public CheckUpdatesDialog()
        {
            InitializeComponent();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Special method for opening the updates dialog directly in the downloads page.
        /// This is necessary when the main application already checked that there are pending
        /// updates so showing the first page is not necessary here.
        /// </summary>
        /// <param name="vlist">The list of pending software updates</param>
        public void ShowDownloadPage(IWin32Window owner, List<VersionUpdateInfo> vlist)
        {
            _startDownload = true;
            _vlist = vlist;
            pnlContents.SelectedPage = pgDownloadUpdates;
            btnCancel.Text = "Close";
            ShowDialog(owner);
        }
        #endregion

        #region Event Handlers
        private void CheckUpdatesDialog_Load(object sender, EventArgs e)
        {
            cbxCheckUpdates.Checked = Configuration.CheckUpdatesOnStartup;
        }

        private void CheckUpdatesDialog_Shown(object sender, EventArgs e)
        {
            UpdateEngine.DownloadUpdatesCompleted += 
                new DownloadUpdatesCompletedEventHandler(UpdateEngine_DownloadUpdatesCompleted);

            // When starting normally (not programmatically) - we'll first check if there are 
            // pending software updates
            if (!_startDownload)
            {
                UpdateEngine.CheckForUpdatesCompleted +=
                    new CheckForUpdatesCompletedEventHandler(UpdateEngine_CheckForUpdatesCompleted);
                UpdateEngine.CheckForUpdatesAsync();
            }
        }

        private void UpdateEngine_DownloadUpdatesCompleted(object sender, DownloadUpdatesCompletedEventArgs e)
        {
            if (InvokeRequired)
                Invoke(new DownloadUpdatesCompletedEventHandler(UpdateEngine_DownloadUpdatesCompleted), sender, e);
            else
            {
                pbrDownloadProgress.Visible = false;
                this.Cursor = Cursors.Default;

                if (e.Error != null)
                {
                    btnDownloadUpdates.Enabled = false;
                    lblDownloadingUpdates.Text = "Error: failed to download updates from server. Try again later.";
                    btnCancel.Text = "Close";
                }
                else if (e.Cancelled)
                {
                    btnDownloadUpdates.Enabled = false;
                    lblDownloadingUpdates.Text = "Operation was cancelled";
                    btnCancel.Text = "Close";
                }
                else
                {
                    lblDownloadingUpdates.Visible = false;

                    // Store the path to the downloaded updates file
                    _updatesFilePath = e.Result;

                    // Switch to the confirmation page
                    pnlContents.SelectedPage = pgConfirm;

                    btnCancel.Text = "Cancel";
                }                
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (UpdateEngine.IsBusy)
            {
                UpdateEngine.CancelAsync();
                Cursor = Cursors.WaitCursor;
            }
            else
            {
                this.Close();
            }
        }

        private void UpdateEngine_CheckForUpdatesCompleted(object sender, CheckForUpdatesCompletedEventArgs e)
        {
            if (InvokeRequired)
                Invoke(new CheckForUpdatesCompletedEventHandler(UpdateEngine_CheckForUpdatesCompleted), sender, e);
            else
            {
                Cursor = Cursors.Default;
                pbrCheckProgress.Visible = false;
                if (e.Error != null)
                    lblCheckMessage.Text = "Error: failed to connect to server. Please try again later.";
                else if (e.Cancelled)
                    lblCheckMessage.Text = "Operation was cancelled.";
                else
                {
                    if (e.Result == null)
                        lblCheckMessage.Text = "Your software is up to date.";
                    else
                    {
                        // Switch to the downloads page and continue the process
                        pnlContents.SelectedPage = pgDownloadUpdates;
                        _vlist = e.Result;
                    } // else
                } // else

                btnCancel.Text = "Close";
            } // else
        }

        private void CheckUpdatesDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (UpdateEngine.IsBusy)
            {
                UpdateEngine.CancelAsync();
                Cursor = Cursors.WaitCursor;
                e.Cancel = true;
            }
            else
            {
                UpdateEngine.DownloadUpdatesCompleted -=
                    new DownloadUpdatesCompletedEventHandler(UpdateEngine_DownloadUpdatesCompleted);
                if (!_startDownload)
                {
                    UpdateEngine.CheckForUpdatesCompleted -=
                        new CheckForUpdatesCompletedEventHandler(UpdateEngine_CheckForUpdatesCompleted);
                }
            } // else
        }

        private void btnNewUpdatesDetails_Click(object sender, EventArgs e)
        {
            if (UpdateEngine.IsBusy)
                return;

            SoftwareUpdatesDetailsDialog dlg = new SoftwareUpdatesDetailsDialog();
            dlg.Prepare(_vlist);
            dlg.ShowDialog(this);
        }

        private void btnInstallNewUpdates_Click(object sender, EventArgs e)
        {
            if (UpdateEngine.IsBusy)
                return;

            lblDownloadingUpdates.Visible = true;
            pbrDownloadProgress.Visible = true;

            btnCancel.Text = "Cancel";
            btnDownloadUpdates.Enabled = false;
            UpdateEngine.DownloadUpdatesAsync();
        }

        private void btnInstallUpdates_Click(object sender, EventArgs e)
        {
            UpdateEngine.PrepareForInstall(_updatesFilePath);

            // Exit the main application loop
            Application.Exit();
        }

        private void cbxCheckUpdates_CheckedChanged(object sender, EventArgs e)
        {
            Configuration.CheckUpdatesOnStartup = cbxCheckUpdates.Checked;
        }

        #endregion

        #region Private Variables
        private List<VersionUpdateInfo> _vlist;
        private string _updatesFilePath;
        private bool _startDownload = false;
        #endregion
    }
}