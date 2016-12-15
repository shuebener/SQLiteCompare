using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SQLiteTurbo
{
    /// <summary>
    /// This dialog is used to prepare the comparison parameters object from the
    /// user's choices.
    /// </summary>
    public partial class CompareDialog : Form
    {
        #region Constructors
        public CompareDialog()
        {
            InitializeComponent();
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Returns the comparison parameters that were entered by the user.
        /// </summary>
        public CompareParams CompareParams
        {
            get { return _params; }
            set
            {
                if (value == null)
                {
                    string lpath = Configuration.LastUsedLeftDbPath;
                    string rpath = Configuration.LastUsedRightDbPath;
                    bool data = Configuration.LastComparisonWithData;
                    if (lpath == null)
                        lpath = string.Empty;
                    if (rpath == null)
                        rpath = string.Empty;
                    bool blobcompare = Configuration.LastCompareBlobFields && data;
                    _params = new CompareParams(lpath, rpath, data ? ComparisonType.CompareSchemaAndData : ComparisonType.CompareSchemaOnly, blobcompare);
                }
                else
                    _params = value;

                txtLeftFile.Text = _params.LeftDbPath;
                txtRightFile.Text = _params.RightDbPath;
                rbtnCompareSchemaAndData.Checked = _params.ComparisonType == ComparisonType.CompareSchemaAndData;
                cbxCompareBlobFields.Checked = _params.IsCompareBlobFields;
            }
        }
        #endregion

        #region Event Handlers

        private void CompareDialog_Load(object sender, EventArgs e)
        {
            UpdateState();
        }

        private void txtLeftFile_TextChanged(object sender, EventArgs e)
        {
            UpdateState();
        }

        private void txtRightFile_TextChanged(object sender, EventArgs e)
        {
            UpdateState();
        }

        private void btnBrowseLeft_Click(object sender, EventArgs e)
        {
            HandleFileSelected(txtLeftFile);
        }

        private void btnBrowseRight_Click(object sender, EventArgs e)
        {
            HandleFileSelected(txtRightFile);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            float v;
            try
            {
                // Make sure both files are SQLite version 3 databases
                v = Utils.GetSQLiteVersion(txtLeftFile.Text.Trim());
                if (v == -1)
                {
                    MessageBox.Show(this,
                        "The left file is not recognized as a valid SQLite database",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }
                else if (v < 3)
                {
                    MessageBox.Show(this,
                        "The left file has an older SQLite file format that is not supported by this utility.\r\n" +
                        "If you really want to compare this file then you'll have to convert it to the newer file\r\n" +
                        "format by following the instructions at http://www.sqlite.org/formatchng.html",
                        "Compatibility Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this,
                    ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            } // catch

            try
            {
                v = Utils.GetSQLiteVersion(txtRightFile.Text.Trim());
                if (v == -1)
                {
                    MessageBox.Show(this,
                        "The right file is not recognized as a valid SQLite database",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }
                else if (v < 3)
                {
                    MessageBox.Show(this,
                        "The right file has an older SQLite file format that is not supported by this utility.\r\n" +
                        "If you really want to compare this file then you'll have to convert it to the newer file\r\n" +
                        "format by following the instructions at http://www.sqlite.org/formatchng.html",
                        "Compatibility Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this,
                    ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            } // catch

            ComparisonType ctype = ComparisonType.None;
            if (rbtnCompareSchemaOnly.Checked)
                ctype = ComparisonType.CompareSchemaOnly;
            else if (rbtnCompareSchemaAndData.Checked)
                ctype = ComparisonType.CompareSchemaAndData;
            else
                throw new ApplicationException("illegal state");

            // Prepare the comparison parameters object
            _params = new CompareParams(txtLeftFile.Text.Trim(), txtRightFile.Text.Trim(), ctype, cbxCompareBlobFields.Checked && rbtnCompareSchemaAndData.Checked);

            // Save parameters in the registry
            Configuration.LastUsedLeftDbPath = _params.LeftDbPath;
            Configuration.LastUsedRightDbPath = _params.RightDbPath;
            Configuration.LastComparisonWithData = _params.ComparisonType == ComparisonType.CompareSchemaAndData;
            Configuration.LastCompareBlobFields = _params.IsCompareBlobFields;

            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void rbtnCompareSchemaOnly_CheckedChanged(object sender, EventArgs e)
        {
            UpdateState();
        }

        private void rbtnCompareSchemaAndData_CheckedChanged(object sender, EventArgs e)
        {
            UpdateState();
        }

        private void txtLeftFile_DragDrop(object sender, DragEventArgs e)
        {
            string fpath = GetDroppedFileName(e);
            if (fpath != null)
                txtLeftFile.Text = fpath;
        }

        private void txtRightFile_DragDrop(object sender, DragEventArgs e)
        {
            string fpath = GetDroppedFileName(e);
            if (fpath != null)
                txtRightFile.Text = fpath;
        }

        private void txtLeftFile_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void txtLeftFile_DragLeave(object sender, EventArgs e)
        {

        }

        private void txtLeftFile_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void txtLeftFile_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            e.UseDefaultCursors = true;
        }

        private void txtLeftFile_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {

        }

        private void txtRightFile_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            e.UseDefaultCursors = true;
        }

        private void txtRightFile_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        #endregion

        #region Private Methods

        private string GetDroppedFileName(DragEventArgs e)
        {
            IDataObject data = e.Data;
            if (data == null)
                return null;
            Array array = data.GetData(DataFormats.FileDrop) as Array;
            if (array == null || array.Length == 0)
                return null;
            object value = array.GetValue(0);
            if (value == null)
                return null;
            return value.ToString();
        }

        private void UpdateState()
        {
            UpdateFilePathTextBox(txtLeftFile);
            UpdateFilePathTextBox(txtRightFile);
            btnOK.Enabled = txtLeftFile.BackColor == FILE_EXISTS_COLOR && txtRightFile.BackColor == FILE_EXISTS_COLOR &&
                txtLeftFile.Text.Trim() != txtRightFile.Text.Trim();
            cbxCompareBlobFields.Enabled = rbtnCompareSchemaAndData.Checked;
        }

        private void HandleFileSelected(TextBox txt)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.AddExtension = true;
            dlg.DefaultExt = "db";
            dlg.FileName = string.Empty;
            dlg.Filter = "SQLite files|*.db;*.db3;*.sqlite|All files|*.*";
            DialogResult res = dlg.ShowDialog(this);
            if (res == DialogResult.Cancel)
                return;

            string fpath = dlg.FileName;
            txt.Text = fpath;
        }

        private void UpdateFilePathTextBox(TextBox txt)
        {
            string path = txt.Text.Trim();
            if (File.Exists(path))
                txt.BackColor = FILE_EXISTS_COLOR;
            else
                txt.BackColor = FILE_NOT_EXISTS_COLOR;
        }
        #endregion

        #region Private Constants
        private Color FILE_EXISTS_COLOR = Color.White;
        private Color FILE_NOT_EXISTS_COLOR = Color.LightSalmon;
        #endregion

        #region Private Variables
        private CompareParams _params;
        #endregion
    }
}