using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using SQLiteParser;
using log4net;
using Common;

namespace SQLiteTurbo
{
    public partial class TwoWayCompareEditDialog : Form
    {
        #region Events
        /// <summary>
        /// Fired when the user changes the schema of any one of the two databases.
        /// This is used by the schema comparison view in order to update its grid.
        /// </summary>
        public event EventHandler SchemaChanged;
        #endregion

        #region Constructors
        public TwoWayCompareEditDialog()
        {
            InitializeComponent();
            _italic = new Font(this.Font, FontStyle.Italic);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Prepare the dialog with the comparison item
        /// </summary>
        /// <param name="item">The comparison item</param>
        /// <param name="leftSchema">The left schema.</param>
        /// <param name="rightSchema">The right schema.</param>
        /// <param name="leftdb">The path to the left database file</param>
        /// <param name="rightdb">the path to the right database file</param>
        public void Prepare(SchemaComparisonItem item,
            Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> leftSchema,
            Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> rightSchema,
            string leftdb, string rightdb)
        {
            _item = item;
            _leftdb = leftdb;
            _rightdb = rightdb;
            _leftSchema = leftSchema;
            _rightSchema = rightSchema;

            this.Text = FormatTitle(item);

            // In case no data comparison took place - hide the data tab
            if (item.TableChanges == null)
                tbcViews.TabPages.Remove(tbpData);
            else
            {
                _tableChanges = item.TableChanges;
                tbcViews.SelectTab(tbpData);
                UpdateDataTab();
            }

            // Load the two schema texts into the diff control            
            CompareSchema(item);

            // Display error title if there was any error
            if (item.ErrorMessage != null)
            {
                string errmsg = item.ErrorMessage.Replace("\r\n", " ");
                panel1.Visible = true;
                lblErrorMessage.Text = "ERROR: data comparison failed (" + errmsg + ")";
            }
        }
        #endregion

        #region Event Handlers

        private void btnClearAllChanges_Click(object sender, EventArgs e)
        {
            CompareSchema(_item);
        }

        private void ucDiff_UndoStateChanged(object sender, EventArgs e)
        {
            UpdateState();
        }


        private void TwoWayCompareEditDialog_Shown(object sender, EventArgs e)
        {
            ucDiff.Focus();
        }

        private void ucTableDiff_SearchRequested(object sender, EventArgs e)
        {
            if (btnSearchData.Enabled)
                ucTableDiff.SearchData();
        }

        private void btnCompareData_Click(object sender, EventArgs e)
        {
            // Before comparing data we have to check if there are any BLOB columns
            // in the any common columns of the tables. If there are any - we have to
            // ask the user if he wants to compare BLOB fields or not.
            SQLiteCreateTableStatement leftTable = _item.LeftDdlStatement as SQLiteCreateTableStatement;
            SQLiteCreateTableStatement rightTable = _item.RightDdlStatement as SQLiteCreateTableStatement;
            List<SQLiteColumnStatement> common = Utils.GetCommonColumns(leftTable, rightTable);
            bool allowBlobComparison = false;
            if (Utils.ContainsBlobColumn(common))
            {
                DialogResult res = MessageBox.Show(this,
                    "At least one column that will be compared is a BLOB.\r\nComparing BLOB fields can potentially take " +
                    "a lot of time to perform.\r\nDo you want to disable BLOB content comparison in order\r\nto make " +
                    "the comparison go faster?",
                    "Disable BLOB Contents Comparison?",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);
                if (res == DialogResult.No)
                    allowBlobComparison = true;
            }

            string errmsg;
            if (!Utils.IsTableComparisonAllowed(leftTable, rightTable,
                out errmsg, allowBlobComparison))
            {
                MessageBox.Show(this, errmsg, "Data comparison is not allowed",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            TableCompareWorker worker = new TableCompareWorker(
                leftTable, rightTable, _leftdb, _rightdb, allowBlobComparison);
            ProgressDialog dlg = new ProgressDialog();
            dlg.Start(this, worker);
            if (dlg.Error != null)
            {
                if (dlg.Error.GetType() != typeof(UserCancellationException))
                    _item.ErrorMessage = dlg.Error.Message;
                return;
            }
            _tableChanges = (TableChanges)dlg.Result;
            if (!tbcViews.TabPages.Contains(tbpData))
                tbcViews.TabPages.Add(tbpData);

            // Update the schema comparison item
            panel1.Visible = false;
            _item.ErrorMessage = null;
            _item.TableChanges = _tableChanges;
            if (SchemaChanged != null)
                SchemaChanged(this, EventArgs.Empty);

            // Set the table changes object into the table diff control
            UpdateDataTab();

            tbcViews.SelectedTab = tbpData;
        }


        private void btnExistsInLeft_Click(object sender, EventArgs e)
        {
            if (_nested)
                return;

            _nested = true;
            btnExistsInLeft.Checked = true;
            btnExistsInRight.Checked = btnDifferent.Checked = btnSame.Checked = false;
            _nested = false;

            // Request the table diff control to show only rows that exist in the left database table
            //ucTableDiff.SetTableChanges(_item, _leftdb, _rightdb, _tableChanges, GetDiff());
            UpdateDataTab();
        }

        private void btnExistsInRight_Click(object sender, EventArgs e)
        {
            if (_nested)
                return;

            _nested = true;
            btnExistsInRight.Checked = true;
            btnExistsInLeft.Checked = btnDifferent.Checked = btnSame.Checked = false;
            _nested = false;

            // Request the table diff control to show only rows that exist in the right database table
            UpdateDataTab();
        }

        private void btnDifferent_Click(object sender, EventArgs e)
        {
            if (_nested)
                return;

            _nested = true;
            btnDifferent.Checked = true;
            btnExistsInLeft.Checked = btnExistsInRight.Checked = btnSame.Checked = false;
            _nested = false;

            // Request the table diff control to show only rows that are different in both databases
            UpdateDataTab();
        }

        private void btnSame_Click(object sender, EventArgs e)
        {
            if (_nested)
                return;

            _nested = true;
            btnSame.Checked = true;
            btnExistsInLeft.Checked = btnExistsInRight.Checked = btnDifferent.Checked = false;
            _nested = false;

            // Request the table diff control to show only rows that are the same in both databases
            UpdateDataTab();
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            ucDiff.Undo();
        }

        private void btnRedo_Click(object sender, EventArgs e)
        {
            ucDiff.Redo();
        }

        private void btnUpdateSchema_Click(object sender, EventArgs e)
        {
            // Run the schema updater object in order to make the necessary changes
            // to the two schema objects according to the text that was updated
            // in the two diff views.

            // We'll pass NULL for a schema object that did not change. This is a hint
            // to the schema object updater NOT to update that object in the database.
            string leftSQL = null;
            string rightSQL = null;
            if (ucDiff.IsLeftModified)
                leftSQL = ucDiff.GetLeftText();
            if (ucDiff.IsRightModified)
                rightSQL = ucDiff.GetRightText();

            StartTableUpdate(leftSQL, rightSQL, false);
        }

        private void btnRefreshComparison_Click(object sender, EventArgs e)
        {
            btnCompareData_Click(btnRefreshComparison, e);
        }

        private void ucTableDiff_RowsChanged(object sender, EventArgs e)
        {
            // When rows are changed - this means that the comparison results are not
            // necessarily valid so we mark them as such
            UpdateDiffCounters(_tableChanges.HasPreciseResults);            
        }

        private void btnReorderColumns_Click(object sender, EventArgs e)
        {
            if (ucDiff.IsLeftFocused)
            {
                SQLiteCreateTableStatement reordered = Utils.ReOrderTableColumns(
                    (SQLiteCreateTableStatement)_item.RightDdlStatement,
                    (SQLiteCreateTableStatement)_item.LeftDdlStatement);

                string[] left = _nlrx.Split(reordered.ToString());
                string[] right = _nlrx.Split(_item.RightDdlStatement.ToString());
                ucDiff.ReplaceText(left, right);
            }
            else
            {
                SQLiteCreateTableStatement reordered = Utils.ReOrderTableColumns(
                    (SQLiteCreateTableStatement)_item.LeftDdlStatement,
                    (SQLiteCreateTableStatement)_item.RightDdlStatement);

                string[] left = _nlrx.Split(_item.LeftDdlStatement.ToString());
                string[] right = _nlrx.Split(reordered.ToString());
                ucDiff.ReplaceText(left, right);
            }

            UpdateState();
        }

        private void btnSearchData_Click(object sender, EventArgs e)
        {
            ucTableDiff.SearchData();
        }

        private void ucTableDiff_StateChanged(object sender, EventArgs e)
        {
            UpdateState();
        }

        private void ucDiff_LeftSaveRequested(object sender, EventArgs e)
        {
            if (btnUpdateSchema.Enabled)
                btnUpdateSchema_Click(btnUpdateSchema, EventArgs.Empty);
        }

        private void ucDiff_RightSaveRequested(object sender, EventArgs e)
        {
            if (btnUpdateSchema.Enabled)
                btnUpdateSchema_Click(btnUpdateSchema, EventArgs.Empty);
        }

        private void TwoWayCompareEditDialog_Load(object sender, EventArgs e)
        {

        }

        private void btnExportDifferences_Click(object sender, EventArgs e)
        {
            ExportChangesDialog dlg = new ExportChangesDialog();
            dlg.TableChanges = _tableChanges;
            dlg.ShowDialog(this);
        }

        #endregion

        #region Protected Methods
        #endregion

        #region Private Methods

        private void UpdateDataTab()
        {
            // Request the table diff control to show the required changes
            ucTableDiff.SetTableChanges(_item, _leftdb, _rightdb, _tableChanges, GetDiff());

            UpdateDiffCounters(_tableChanges.HasPreciseResults);

            UpdateState();
        }

        private void UpdateDiffCounters(bool precise)
        {
            // Update the counters for all change types
            long leftCount = _tableChanges.GetTotalChangesCount(new string[] { TableChanges.EXISTS_IN_LEFT_TABLE_NAME });
            long rightCount = _tableChanges.GetTotalChangesCount(new string[] { TableChanges.EXISTS_IN_RIGHT_TABLE_NAME });
            long diffCount = _tableChanges.GetTotalChangesCount(new string[] { TableChanges.DIFFERENT_ROWS_TABLE_NAME });
            long sameCount = _tableChanges.GetTotalChangesCount(new string[] { TableChanges.SAME_ROWS_TABLE_NAME });

            string qm = string.Empty;
            if (!precise)
            {
                qm = "?";

                btnRefreshComparison.BackColor = UNPRECISE_COMPARISON_COLOR;
            }
            else
            {
                btnRefreshComparison.BackColor = Color.Transparent;
            }

            btnExistsInLeft.Text = "(" + leftCount + qm+ ")";
            btnExistsInRight.Text = "(" + rightCount + qm+")";
            btnDifferent.Text = "(" + diffCount + qm+")";
            btnSame.Text = "(" + sameCount + qm+")";
        }

        private void StartTableUpdate(string leftSQL, string rightSQL, bool skipNullRows)
        {
            // Create and start the updater object
            SchemaObjectUpdater updater = new SchemaObjectUpdater(_item,
                _leftSchema, _rightSchema, _leftdb, _rightdb, leftSQL, rightSQL, skipNullRows);
            ProgressDialog dlg = new ProgressDialog();
            dlg.Start(this, updater);

            if (dlg.Error == null)
            {
                // Compare again based on the updated schema objects found in the item object.
                CompareSchema(_item);

                // Notify about the change
                if (SchemaChanged != null)
                    SchemaChanged(this, EventArgs.Empty);
            } // else
            else
            {
                if (dlg.Error is UpdateTableException)
                {
                    UpdateTableException ue = (UpdateTableException)dlg.Error;
                    if (ue.CanRestart)
                    {
                        DialogResult res = MessageBox.Show(this,
                            "Press [Yes] to restart the operation and skip problem rows\r\n" +
                            "Press [No] to abort the operation",
                            "Information",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning,
                            MessageBoxDefaultButton.Button2);
                        if (res == DialogResult.No)
                            return;

                        StartTableUpdate(leftSQL, rightSQL, true);
                    }
                } // if
            } // else

            // Remove the data tab
            tbcViews.TabPages.Remove(tbpData);
        }

        private void CompareSchema(SchemaComparisonItem item)
        {
            string[] left = _nlrx.Split(item.LeftDdlStatement.ToString());
            string[] right = _nlrx.Split(item.RightDdlStatement.ToString());
            ucDiff.CompareTexts(left, right, _leftdb, _rightdb);

            UpdateState();
        }

        /// <summary>
        /// Update the button's sensitivity and visibility according to the state of the dialog box
        /// </summary>
        private void UpdateState()
        {
            btnUndo.Enabled = ucDiff.CanUndo;
            btnRedo.Enabled = ucDiff.CanRedo;
            btnClearAllChanges.Enabled = ucDiff.CanUndo;
            btnCompareData.Visible = _item.LeftDdlStatement is SQLiteCreateTableStatement;
            btnUpdateSchema.Enabled = ucDiff.IsLeftModified || ucDiff.IsRightModified;
            btnReorderColumns.Visible = _item.LeftDdlStatement is SQLiteCreateTableStatement;
            btnSearchData.Enabled = true;

            bool allowedGenerateScripts = true;
            btnExportDifferences.Enabled = allowedGenerateScripts;
        }

        private string GetDiff()
        {
            if (btnDifferent.Checked)
                return TableChanges.DIFFERENT_ROWS_TABLE_NAME;
            if (btnSame.Checked)
                return TableChanges.SAME_ROWS_TABLE_NAME;
            if (btnExistsInLeft.Checked)
                return TableChanges.EXISTS_IN_LEFT_TABLE_NAME;
            if (btnExistsInRight.Checked)
                return TableChanges.EXISTS_IN_RIGHT_TABLE_NAME;
            throw new InvalidOperationException();
        }

        private string FormatTitle(SchemaComparisonItem item)
        {
            string etype;
            if (item.LeftDdlStatement is SQLiteCreateTableStatement)
                etype = "Table";
            else if (item.LeftDdlStatement is SQLiteCreateIndexStatement)
                etype = "Index";
            else if (item.LeftDdlStatement is SQLiteCreateTriggerStatement)
                etype = "Trigger";
            else if (item.LeftDdlStatement is SQLiteCreateViewStatement)
                etype = "View";
            else
                throw new ArgumentException("illegal SQL entity type");

            return etype + " " + item.LeftDdlStatement.ObjectName.ToString();
        }
        #endregion

        #region Constants
        private readonly Color UNPRECISE_COMPARISON_COLOR = Color.LightSalmon;
        #endregion

        #region Private Variables
        private bool _nested;
        private SchemaComparisonItem _item;
        private string _leftdb;
        private string _rightdb;
        private TableChanges _tableChanges;
        private Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> _leftSchema;
        private Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> _rightSchema;
        private Font _italic;
        private Regex _nlrx = new Regex("\r\n|\n");
        private ILog _log = LogManager.GetLogger(typeof(TwoWayCompareEditDialog));
        #endregion
    }
}