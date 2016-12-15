using System;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SQLiteParser;
using Common;
using AutomaticUpdates;
using log4net;

namespace SQLiteTurbo
{
    /// <summary>
    /// This class is the main form of the application.
    /// </summary>
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        #region Event Handlers

        private void MainForm_Load(object sender, EventArgs e)
        {
            UpdateState();
        }

        private void btnCompare_Click(object sender, EventArgs e)
        {
            HandleCompareDialog();
        }

        private void mniCompare_Click(object sender, EventArgs e)
        {
            HandleCompareDialog();
        }

        private void mniCloseComparison_Click(object sender, EventArgs e)
        {
            CleanupSchemaView();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CleanupSchemaView();
        }

        private void btnNextDiff_Click(object sender, EventArgs e)
        {
            _schemaView.MoveToNextDiff();
        }

        private void btnPreviousDiff_Click(object sender, EventArgs e)
        {
            _schemaView.MoveToPreviousDiff();
        }

        private void _schemaView_SelectionChanged(object sender, EventArgs e)
        {
            UpdateState();
        }

        private void mniCopyFromLeftDB_Click(object sender, EventArgs e)
        {
            _schemaView.CopyFromLeftDB();
        }

        private void mniCopyFromRightDB_Click(object sender, EventArgs e)
        {
            _schemaView.CopyFromRightDB();
        }

        private void btnEditSelectedDifference_Click(object sender, EventArgs e)
        {
            _schemaView.OpenCompareDialog();
        }

        private void mniEditSelection_Click(object sender, EventArgs e)
        {
            _schemaView.OpenCompareDialog();
        }

        private void mniGotoNextDifference_Click(object sender, EventArgs e)
        {
            _schemaView.MoveToNextDiff();
        }

        private void mniGotoPreviousDifference_Click(object sender, EventArgs e)
        {
            _schemaView.MoveToPreviousDiff();
        }

        private void btnCopyFromLeftDB_Click(object sender, EventArgs e)
        {
            _schemaView.CopyFromLeftDB();
        }

        private void btnCopyFromRightDB_Click(object sender, EventArgs e)
        {
            _schemaView.CopyFromRightDB();
        }

        private void mniAbout_Click(object sender, EventArgs e)
        {
            AboutDialog dlg = new AboutDialog();
            dlg.ShowDialog(this);
        }

        private void mniExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void mniReport_Click(object sender, EventArgs e)
        {
            WebSiteUtils.OpenBugFeaturePage();
        }

        private void mniGenerateChangeScriptLeftToRight_Click(object sender, EventArgs e)
        {
            string sql = ChangeScriptBuilder.Generate(_leftdb, _rightdb, _leftSchema, _rightSchema, _results, ChangeDirection.LeftToRight);

            ChangeScriptDialog dlg = new ChangeScriptDialog();
            dlg.Prepare(sql);
            dlg.ShowDialog(this);
        }

        private void mniGenerateChangeScriptRightToLeft_Click(object sender, EventArgs e)
        {
            string sql = ChangeScriptBuilder.Generate(_leftdb, _rightdb, _leftSchema, _rightSchema, _results, ChangeDirection.RightToLeft);

            ChangeScriptDialog dlg = new ChangeScriptDialog();
            dlg.Prepare(sql);
            dlg.ShowDialog(this);
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            UpdateTitle();

            // The first time the software is ran - it should prompt the user if he wants to enable
            // checking for software updates upon system startup
            if (Configuration.FirstTime)
            {
                Configuration.FirstTime = false;

                FirstTimeDialog fdlg = new FirstTimeDialog();
                DialogResult res = fdlg.ShowDialog(this);
                Configuration.CheckUpdatesOnStartup = fdlg.IsAutomaticUpdates;
                if (res == DialogResult.OK)
                {
                    CheckUpdatesDialog cdlg = new CheckUpdatesDialog();
                    cdlg.ShowDialog(this);
                } // else
            }
            else
            {
                // Check for software updates if necessary
                CheckSoftwareUpdates();
            }
        }

        private void mniProductWebsite_Click(object sender, EventArgs e)
        {
            WebSiteUtils.OpenProductPage();
        }

        private void mniUserGuide_Click(object sender, EventArgs e)
        {
            WebSiteUtils.OpenUserGuidePage();
        }

        private void mniCheckForUpdates_Click(object sender, EventArgs e)
        {
            CheckUpdatesDialog dlg = new CheckUpdatesDialog();
            dlg.ShowDialog(this);
        }

        private void pbxFeedback_MouseDown(object sender, MouseEventArgs e)
        {
            pbxFeedback.Image = Properties.Resources.pressed_feedback_button;
        }

        private void pbxFeedback_MouseUp(object sender, MouseEventArgs e)
        {
            pbxFeedback.Image = Properties.Resources.normal_feedback_button;
            if (pbxFeedback.ClientRectangle.Contains(e.Location))
            {
                pbxFeedback.Visible = false;
                FeedbackDialog dlg = new FeedbackDialog();
                dlg.ShowDialog(this);
                pbxFeedback.Visible = true;
            }
        }

        private void btnExportDataDifferences_Click(object sender, EventArgs e)
        {
            _schemaView.ExportDataDifferences();
        }

        #endregion

        #region Private Methods

        private void UpdateTitle()
        {
            this.Text = "SQLite Compare";
        }

        /// <summary>
        /// Allows to refresh the comparison results
        /// </summary>
        /// <param name="cancellable">TRUE means that the user will be allowed to cancel
        /// the comparison process. FALSE prevents this from happening (by making the CANCEL
        /// button disabled).</param>
        private void RefreshComparison(bool cancellable)
        {
            CompareWorker worker = new CompareWorker(_compareParams);
            ProgressDialog pdlg = new ProgressDialog();
            if (cancellable)
                pdlg.Start(this, worker);
            else
                pdlg.StartNonCancellable(this, worker);

            Dictionary<SchemaObject, List<SchemaComparisonItem>> results =
                (Dictionary<SchemaObject, List<SchemaComparisonItem>>)pdlg.Result;
            if (results != null)
            {
                // Create the schema comparison view and populate it with the results
                if (_schemaView == null)
                {
                    _schemaView = new SchemaComparisonView();
                    _schemaView.BackColor = SystemColors.Control;
                    pnlContents.Controls.Add(_schemaView);
                    _schemaView.Dock = DockStyle.Fill;
                    _schemaView.SelectionChanged += new EventHandler(_schemaView_SelectionChanged);
                }

                _leftSchema = worker.LeftSchema;
                _rightSchema = worker.RightSchema;
                _results = results;
                _leftdb = _compareParams.LeftDbPath;
                _rightdb = _compareParams.RightDbPath;

                _schemaView.ShowComparisonResults(results, _compareParams.LeftDbPath, _compareParams.RightDbPath,
                    worker.LeftSchema, worker.RightSchema, _compareParams.ComparisonType == ComparisonType.CompareSchemaAndData);
            }

            UpdateState();
        }

        private void CleanupSchemaView()
        {
            if (_schemaView == null)
                return;

            // Delete all temporary comparison files.
            foreach (SchemaComparisonItem item in _schemaView.Results[SchemaObject.Table])
            {
                if (item.TableChanges != null)
                    item.TableChanges.Dispose();
            } // foreach

            // Hide the schema view control
            pnlContents.Controls.Remove(_schemaView);
            _schemaView.SelectionChanged -= new EventHandler(_schemaView_SelectionChanged);
            _schemaView.Dispose();
            _schemaView = null;            

            _leftSchema = _rightSchema = null;
            _results = null;

            UpdateState();
        }

        private void UpdateState()
        {
            btnNextDiff.Enabled = _schemaView != null && _schemaView.HasNextDiff();
            btnPreviousDiff.Enabled = _schemaView != null && _schemaView.HasPreviousDiff();
            btnCopyFromLeftDB.Enabled = _schemaView != null && _schemaView.CanCopyFromLeftDB();
            btnCopyFromRightDB.Enabled = _schemaView != null && _schemaView.CanCopyFromRightDB();
            btnEditSelectedDifference.Enabled = _schemaView != null && _schemaView.CanEditSelectedDifference();
            mniCopyFromLeftDB.Enabled = _schemaView != null && _schemaView.CanCopyFromLeftDB();
            mniCopyFromRightDB.Enabled = _schemaView != null && _schemaView.CanCopyFromRightDB();
            mniEditSelection.Enabled = _schemaView != null && _schemaView.CanEditSelectedDifference();
            mniGotoNextDifference.Enabled = _schemaView != null && _schemaView.HasNextDiff();
            mniGotoPreviousDifference.Enabled = _schemaView != null && _schemaView.HasPreviousDiff();
            pbxFeedback.Visible = _schemaView == null;            

            #region Allow change script generation only to commercial/evaluation users

            bool allowedGenerateScripts = false;
            allowedGenerateScripts = true;

            mniGenerateChangeScriptLeftToRight.Enabled = _schemaView != null && allowedGenerateScripts;
            mniGenerateChangeScriptRightToLeft.Enabled = _schemaView != null && allowedGenerateScripts;
            mniCloseComparison.Enabled = _schemaView != null;
            btnExportDataDifferences.Enabled = _schemaView != null && _schemaView.HasDataDiffs() && allowedGenerateScripts;

            #endregion
        }

        private void HandleCompareDialog()
        {
            CompareDialog dlg = new CompareDialog();
            dlg.CompareParams = _compareParams;
            DialogResult res = dlg.ShowDialog(this);
            if (res == DialogResult.Cancel)
                return;

            // Collect the comparison parameters from the user and do the comparison.
            _compareParams = dlg.CompareParams;
            RefreshComparison(true);
        }

        /// <summary>
        /// Check if there are software updates and show the updates dialog if necessary
        /// </summary>
        private void CheckSoftwareUpdates()
        {
            // Check for software updates if necessary
            if (Configuration.CheckUpdatesOnStartup)
            {
                mniCheckForUpdates.Enabled = false;
                WaitCallback wc = delegate
                {
                    try
                    {
                        //List<VersionUpdateInfo> vlist = UpdateEngine.CheckForUpdates();
                        List<VersionUpdateInfo> vlist = null;
                        if (vlist != null)
                        {
                            // There are updates waiting
                            Invoke(new MethodInvoker(delegate
                            {
                                CheckUpdatesDialog cdlg = new CheckUpdatesDialog();
                                cdlg.ShowDownloadPage(this, vlist);
                            }));
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Warn("failed to check for software updates", ex);
                    } // catch
                    finally
                    {
                        // The window may have been disposed as a result of shutting down the application.
                        if (!UpdateEngine.IsReInstalling)
                        {
                            try
                            {
                                Invoke(new MethodInvoker(delegate
                                {
                                    mniCheckForUpdates.Enabled = true;
                                }));
                            }
                            catch (ObjectDisposedException ode)
                            {
                                // Ignore
                            }
                        } // if
                    } // finally
                };
                ThreadPool.QueueUserWorkItem(wc);
            }
        }
        #endregion

        #region Private Variables       
        private SchemaComparisonView _schemaView = null;
        private CompareParams _compareParams = null;
        private string _leftdb;
        private string _rightdb;
        private ILog _log = LogManager.GetLogger(typeof(MainForm));
        private Dictionary<SchemaObject, List<SchemaComparisonItem>> _results;
        private Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> _leftSchema;
        private Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> _rightSchema;
        #endregion
    }
}