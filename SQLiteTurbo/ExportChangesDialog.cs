using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SQLiteTurbo
{
    public partial class ExportChangesDialog : Form
    {
        #region Constructors
        public ExportChangesDialog()
        {
            InitializeComponent();
        }
        #endregion

        #region Public Properties
        public TableChanges TableChanges
        {
            get { return _changes; }
            set { _changes = value; }
        }

        public List<SchemaComparisonItem> MultipleChanges
        {
            get { return _multiChanges; }
            set { _multiChanges = value; }
        }
        #endregion

        #region Event Handlers

        private void ExportChangesDialog_Load(object sender, EventArgs e)
        {
            UpdateState();
            if (_lastPath != null)
                txtFile.Text = _lastPath;
            if (_lastNew.HasValue)
                cbxExportAdded.Checked = _lastNew.Value;
            if (_lastUpdated.HasValue)
                cbxExportUpdates.Checked = _lastUpdated.Value;
            if (_lastDeleted.HasValue)
                cbxExportDeleted.Checked = _lastDeleted.Value;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            DialogResult res = saveFileDialog1.ShowDialog(this);
            if (res == DialogResult.Cancel)
                return;
            string fpath = saveFileDialog1.FileName;
            txtFile.Text = fpath;
            _lastPath = fpath;
        }

        private void txtFile_TextChanged(object sender, EventArgs e)
        {
            UpdateState();
        }

        private void cbxExportUpdates_CheckedChanged(object sender, EventArgs e)
        {
            _lastUpdated = cbxExportUpdates.Checked;
            UpdateState();
        }

        private void cbxExportAdded_CheckedChanged(object sender, EventArgs e)
        {
            _lastNew = cbxExportAdded.Checked;
            UpdateState();
        }

        private void cbxExportDeleted_CheckedChanged(object sender, EventArgs e)
        {
            _lastDeleted = cbxExportDeleted.Checked;
            UpdateState();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            ExportToFile(true);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnExportAndClose_Click(object sender, EventArgs e)
        {
            ExportToFile(false);
        }

        #endregion

        #region Private Methods

        private void ExportToFile(bool openInExternal)
        {
            AbstractWorker worker = null;
            if (_changes != null)
            {
                worker = new DiffExporter(_changes, txtFile.Text.Trim(), cbxExportUpdates.Checked,
                    cbxExportAdded.Checked, cbxExportDeleted.Checked);
            }
            else
            {
                worker = new DiffExporter(_multiChanges, txtFile.Text.Trim(), cbxExportUpdates.Checked,
                    cbxExportAdded.Checked, cbxExportDeleted.Checked);
            }
            ProgressDialog dlg = new ProgressDialog();
            dlg.Start(this, worker);

            if (dlg.Error == null)
            {
                this.Close();

                if (openInExternal)
                {
                    try
                    {
                        // Open in external application
                        Process p = new Process();
                        ProcessStartInfo psi = new ProcessStartInfo(txtFile.Text.Trim());
                        p.StartInfo = psi;
                        psi.UseShellExecute = true;
                        p.Start();
                    }
                    catch (Exception ex)
                    {
                        DialogResult res = MessageBox.Show(this, "There is no external viewer for this type of file. Do yo want to open the file using notepad?",
                            "No Viewer Available", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                        if (res == DialogResult.Yes)
                        {
                            // Try to open using notepad
                            OpenAsTextFile(txtFile.Text.Trim());
                        }                        
                    } // catch
                }
            } // if
        }

        private void OpenAsTextFile(string fpath)
        {
            try
            {
                Process p = new Process();
                ProcessStartInfo psi = new ProcessStartInfo("notepad", fpath);
                p.StartInfo = psi;
                psi.UseShellExecute = true;
                p.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Failed to open notepad", "Failed To Open Viewer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateState()
        {
            bool enable = txtFile.Text.Trim().Length > 0 && (cbxExportAdded.Checked || cbxExportDeleted.Checked || cbxExportUpdates.Checked);
            btnOK.Enabled = enable;
            btnExportAndClose.Enabled = enable;
        }
        #endregion

        #region Private Constants
        private Color FILE_EXISTS_COLOR = Color.White;
        private Color FILE_NOT_EXISTS_COLOR = Color.LightSalmon;
        #endregion

        #region Fields
        private TableChanges _changes;
        private List<SchemaComparisonItem> _multiChanges;
        private static string _lastPath = null;
        private static bool? _lastNew = null;
        private static bool? _lastUpdated = null;
        private static bool? _lastDeleted = null;
        #endregion
    }
}