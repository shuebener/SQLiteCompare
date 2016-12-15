using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Liron.Windows.Forms;
using SQLiteParser;

namespace SQLiteTurbo
{
    public partial class SearchDataRowsDialog : Form
    {
        #region Constructors
        public SearchDataRowsDialog()
        {
            InitializeComponent();

            txtGuid.ValidatingType = typeof(Guid);
            txtGuid.Mask = "AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA";

            numFloatingPoint.Minimum = decimal.MinValue;
            numFloatingPoint.Maximum = decimal.MaxValue;

            numIntValue.Minimum = long.MinValue;
            numIntValue.Maximum = long.MaxValue;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Prepares the search dialog by initializing it with the columns list that can be searched,
        /// a column name to be searched by default (optional), and the table changes object that will
        /// be used for searching the database.
        /// </summary>
        /// <param name="clist">A list of columns that can be searched</param>
        /// <param name="colName">A default column name to be searched, or NULL if no column is searched by default.</param>
        /// <param name="tchanges">The table changes object</param>
        public void PrepareDialog(List<SQLiteColumnStatement> clist, string colName, string diff, TableChanges tchanges, long rowIndex, bool isLeft)
        {
            _matchedRowIndex = -1;
            _diff = diff;
            _isLeft = isLeft;
            _columns = clist;
            _changes = tchanges;
            _rowIndex = rowIndex;

            bool lastused = false;
            cboColumnName.Items.Clear();
            foreach (SQLiteColumnStatement col in clist)
            {
                string cname = SQLiteParser.Utils.Chop(col.ObjectName.ToString());
                cboColumnName.Items.Add(cname);

                if (cname == _lastColName)
                    lastused = true;
            } // foreach


            if (lastused)
                cboColumnName.SelectedItem = _lastColName;
            else if (colName != null)
            {
                colName = SQLiteParser.Utils.Chop(colName);
                cboColumnName.SelectedItem = colName;
            }

            if (isLeft)
                this.Text = "Search Rows (left database)";
            else
                this.Text = "Search Rows (right database)";
        }
        #endregion

        #region Public Properties
        public long MatchedRowIndex
        {
            get { return _matchedRowIndex; }
        }
        #endregion

        #region Event Handlers

        private void btnOK_Click(object sender, EventArgs e)
        {
            SearchDataWorker worker = null;
            string sql;
            if (tbcSearch.SelectedTab == tbpExact)
            {
                string colName = (string)cboColumnName.SelectedItem;

                if (cbxValue.Checked)
                {
                    sql = FormatColumnValue(colName);
                }
                else
                {
                    sql = SQLiteParser.Utils.QuoteIfNeeded(colName) + " IS NULL";
                }
            }
            else
            {
                sql = txtSQL.Text.Trim();
            }
            worker = new SearchDataWorker(_isLeft, _diff, _changes, _rowIndex, -1, sql);

            ProgressDialog dlg = new ProgressDialog();
            dlg.Start(this, worker);

            if (dlg.Result != null)
                _matchedRowIndex = (long)dlg.Result;
            if (_matchedRowIndex == -1 && dlg.Error == null)
            {
                if (_rowIndex > 0)
                {
                    DialogResult res = MessageBox.Show(this, "No match was found, do you want to search from the beginning?",
                        "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                    if (res == DialogResult.Yes)
                    {
                        worker = new SearchDataWorker(_isLeft, _diff, _changes, 0, _rowIndex-1, sql);
                        dlg = new ProgressDialog();
                        dlg.Start(this, worker);
                        if (dlg.Result != null)
                            _matchedRowIndex = (long)dlg.Result;
                        if (_matchedRowIndex != -1 || dlg.Error != null)
                            this.DialogResult = DialogResult.OK;
                        else
                            MessageBox.Show(this, "No match was found", "Search Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                    MessageBox.Show(this, "No match was found", "Search Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                this.DialogResult = DialogResult.OK;
            }
        }

        private void txtGuid_KeyDown(object sender, KeyEventArgs e)
        {
            if (!
                (e.Modifiers == (Keys)0 &&
                (e.KeyCode == Keys.A ||
                e.KeyCode == Keys.B ||
                e.KeyCode == Keys.C ||
                e.KeyCode == Keys.D ||
                e.KeyCode == Keys.E ||
                e.KeyCode == Keys.F ||
                e.KeyCode == Keys.D0 ||
                e.KeyCode == Keys.D1 ||
                e.KeyCode == Keys.D2 ||
                e.KeyCode == Keys.D3 ||
                e.KeyCode == Keys.D4 ||
                e.KeyCode == Keys.D5 ||
                e.KeyCode == Keys.D6 ||
                e.KeyCode == Keys.D7 ||
                e.KeyCode == Keys.D8 ||
                e.KeyCode == Keys.D9 ||
                e.KeyCode == Keys.Back ||
                e.KeyCode == Keys.Left ||
                e.KeyCode == Keys.Right ||
                e.KeyCode == Keys.End ||
                e.KeyCode == Keys.Home))
                )
                e.SuppressKeyPress = true;
        }

        private void txtGuid_TextChanged(object sender, EventArgs e)
        {
            UpdateState();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void SearchDataRowsDialog_Shown(object sender, EventArgs e)
        {
            if (tbcSearch.SelectedTab == tbpExact)
            {
                pnlValues.Focus();
                MoveFocus();
            }
            else
                txtSQL.Focus();
        }

        private void cboColumnName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboColumnName.SelectedIndex != -1)
                _lastColName = (string)cboColumnName.SelectedItem;

            UpdateState();

            MoveFocus();
        }

        private void cbxValue_CheckedChanged(object sender, EventArgs e)
        {
            UpdateState();
        }

        private void txtBlob_TextChanged(object sender, EventArgs e)
        {
            int tmp = txtBlob.SelectionStart;
            txtBlob.Text = txtBlob.Text.ToUpper();
            txtBlob.SelectionStart = tmp;
            UpdateState();
        }

        private void txtBlob_KeyDown(object sender, KeyEventArgs e)
        {
            if (!
                (e.Modifiers == (Keys)0 &&
                (e.KeyCode == Keys.A ||
                e.KeyCode == Keys.B ||
                e.KeyCode == Keys.C ||
                e.KeyCode == Keys.D ||
                e.KeyCode == Keys.E ||
                e.KeyCode == Keys.F ||
                e.KeyCode == Keys.D0 ||
                e.KeyCode == Keys.D1 ||
                e.KeyCode == Keys.D2 ||
                e.KeyCode == Keys.D3 ||
                e.KeyCode == Keys.D4 ||
                e.KeyCode == Keys.D5 ||
                e.KeyCode == Keys.D6 ||
                e.KeyCode == Keys.D7 ||
                e.KeyCode == Keys.D8 ||
                e.KeyCode == Keys.D9 ||
                e.KeyCode == Keys.Back ||
                e.KeyCode == Keys.Left ||
                e.KeyCode == Keys.Right ||
                e.KeyCode == Keys.End ||
                e.KeyCode == Keys.Home))
                )
                e.SuppressKeyPress = true;
        }

        #endregion

        #region Private Methods

        private void MoveFocus()
        {
            if (pnlValues.SelectedPage == pnlBooleanValue)
                rbtnTrue.Focus();
            else if (pnlValues.SelectedPage == pnlDateTimeValue)
                dtpDate.Focus();
            else if (pnlValues.SelectedPage == pnlFloatingPoint)
                numFloatingPoint.Focus();
            else if (pnlValues.SelectedPage == pnlGuidValue)
            {
                txtGuid.Focus();
                txtGuid.SelectAll();
            }
            else if (pnlValues.SelectedPage == pnlIntegerValue)
                numIntValue.Focus();
            else if (pnlValues.SelectedPage == pnlTextValue)
            {
                txtFieldValue.Focus();
                txtFieldValue.SelectAll();
            }
            else if (pnlValues.SelectedPage == pnlBlobValue)
                txtBlob.Focus();
        }

        private string FormatColumnValue(string colName)
        {
            colName = SQLiteParser.Utils.QuoteIfNeeded(colName);

            MultiPanelPage page = pnlValues.SelectedPage;
            if (page == pnlBlobValue)
                return colName + " = x'" + txtBlob.Text.Trim() + "'";
            else if (page == pnlBooleanValue)
                return colName + " = " + (rbtnTrue.Checked ? "1" : "0");
            else if (page == pnlDateTimeValue)
            {
                DateTime dt = new DateTime(dtpDate.Value.Year, dtpDate.Value.Month, dtpDate.Value.Day);
                return "DATE(" + colName + ") = DATE('" + SQLiteParser.Utils.GetDateTimeLiteralString(dt, false) + "')";
            }
            else if (page == pnlFloatingPoint)
            {
                string colname = (string)cboColumnName.SelectedItem;
                SQLiteColumnStatement c = Utils.GetColumnByName(_columns, colname);
                if (Utils.GetDbType(c.ColumnType) == DbType.Double)
                {
                    double dval = (double)numFloatingPoint.Value;
                    return colName + " = " + dval.ToString();
                }
                else
                {
                    float fval = (float)numFloatingPoint.Value;
                    return colName + " = " + fval.ToString();
                }
            }
            else if (page == pnlGuidValue)
            {
                return colName + " = " + "X'" + txtGuid.Text.Replace("-",string.Empty) + "'";
            }
            else if (page == pnlIntegerValue)
            {
                return colName + " = " + numIntValue.Value.ToString();
            }
            else if (page == pnlTextValue)
            {
                return colName + " = " + SQLiteParser.Utils.QuoteLiteralString(txtFieldValue.Text);
            }
            throw new InvalidOperationException();
        }

        private void UpdateState()
        {
            if (!cbxValue.Checked)
                pnlValues.SelectedPage = pnlNull;
            else
            {
                if (cboColumnName.SelectedIndex != -1)
                {
                    SQLiteColumnStatement col = Utils.GetColumnByName(_columns, (string)cboColumnName.SelectedItem);
                    DbType dbtype = Utils.GetDbType(col.ColumnType);
                    if (dbtype == DbType.String)
                        pnlValues.SelectedPage = pnlTextValue;
                    else if (dbtype == DbType.DateTime)
                        pnlValues.SelectedPage = pnlDateTimeValue;
                    else if (dbtype == DbType.Double || dbtype == DbType.Single)
                        pnlValues.SelectedPage = pnlFloatingPoint;
                    else if (dbtype == DbType.Int64)
                        pnlValues.SelectedPage = pnlIntegerValue;
                    else if (dbtype == DbType.Boolean)
                        pnlValues.SelectedPage = pnlBooleanValue;
                    else if (dbtype == DbType.Binary)
                        pnlValues.SelectedPage = pnlBlobValue;
                    else if (dbtype == DbType.Guid)
                        pnlValues.SelectedPage = pnlGuidValue;
                    else
                        throw new InvalidOperationException("Internal software error");
                } // if
            } // else

            if (pnlValues.SelectedPage == pnlGuidValue)
            {
                Guid dummy;
                btnOK.Enabled = Utils.IsGuid(txtGuid.Text, out dummy);
            }
            else if (pnlValues.SelectedPage == pnlBlobValue)
            {
                btnOK.Enabled = (txtBlob.Text.Trim().Length % 2 == 0);
            }
            else
                btnOK.Enabled = true;
        }

        private string FormatColumnValue(string colName, string value)
        {
            SQLiteColumnStatement col = Utils.GetColumnByName(_columns, colName);
            DbType dbtype = Utils.GetDbType(col.ColumnType);
            if (dbtype == DbType.Int64 || dbtype == DbType.Double || dbtype == DbType.Single)
            {
                double dval;
                if (double.TryParse(value, out dval))
                    return dval.ToString();
            }

            if (value.Length >= 2)
            {
                if (value[0] == '\'' && value[value.Length - 1] == '\'')
                    value = value.Substring(1, value.Length - 2);                
            }
            value = value.Replace("'", "''");
            value = "'" + value + "'";
            return value;
        }
        #endregion

        #region Private Variables
        private string _diff;
        private bool _isLeft;
        private long _rowIndex;
        private TableChanges _changes;
        private List<SQLiteColumnStatement> _columns;
        private long _matchedRowIndex = -1;
        private byte[] _blob = null;
        private static string _lastColName = null;
        #endregion
    }
}