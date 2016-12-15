using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SQLiteParser;
using Be.Windows.Forms;

namespace SQLiteTurbo
{
    /// <summary>
    /// This dialog provides a way for the user to edit the content of any table
    /// cell and update its value in the database.
    /// </summary>
    public partial class CellValueEditorDialog : Form
    {
        #region Constructorss
        public CellValueEditorDialog()
        {
            InitializeComponent();

            numInteger.Minimum = long.MinValue;
            numInteger.Maximum = long.MaxValue;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Show the editor for the specified column and field value.
        /// </summary>
        /// <param name="owner">The owner window</param>
        /// <param name="table">The table sql schema object</param>
        /// <param name="column">The column whose field is edited</param>
        /// <param name="value">The initial value to edit</param>
        /// <returns>
        /// If the user chose to apply his changes - DialogResult.OK (in which case the <paramref name="value"/>
        /// parameter is changed to the edited value). Otherwise a DialogResult.Cancel value is returned.
        /// </returns>
        public DialogResult ShowEditor(IWin32Window owner, SQLiteCreateTableStatement table, SQLiteColumnStatement column, ref object value)
        {
            _table = table;
            _column = column;
            this.Text = "Edit " + column.ObjectName.ToString();

            TabPage tbp = null;
            _nullable = column.IsNullable;
            if (value == DBNull.Value)
            {
                cbxSetAsNull.Checked = true;
                tbp = GetTabPageForColumnType(column);
            }
            else
            {
                cbxSetAsNull.Checked = false;
                tbp = GetTabPageForValueType(value);                
            }

            if (Utils.IsColumnActingAsRowIdAlias(_table, column.ObjectName.ToString()))
            {
                // This is a special case in which we allow the user to edit a INTEGER PRIMARY KEY column
                // that acts as an alias to the underlying RowID column. In this case we can't allow the user
                // to choose any other type other than INTEGER when editing the field.
                tbp = tbpEditInteger;
            }

            TabPage tbp2 = GetTabPageForColumnType(column);
            if (!object.ReferenceEquals(tbp, tbp2))
            {
                RemoveAllPagesExcept(tbp, tbp2);
                SetTabPageValue(tbp, value);
                SetTabPageValue(tbp2, value);
            }
            else
            {
                RemoveAllPagesExcept(tbp);
                SetTabPageValue(tbp, value);
            }            
            
            _origTabPage = tbp;
            tbcTypes.SelectedTab = tbp;

            UpdateState();

            DialogResult res = ShowDialog(owner);
            if (res == DialogResult.OK)
                value = _value;

            GC.Collect();

            return res;
        }
        #endregion

        #region Event Handlers

        private void CellValueEditorDialog_Load(object sender, EventArgs e)
        {
            txtGuid.ValidatingType = typeof(Guid);
            txtGuid.Mask = "AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA";
        }

        private void cbxSetAsNull_CheckedChanged(object sender, EventArgs e)
        {
            UpdateState();
        }

        private void dtpTime_ValueChanged(object sender, EventArgs e)
        {
            pnlDateTimeWarning.Visible = false;
        }

        private void dtpDate_ValueChanged(object sender, EventArgs e)
        {
            pnlDateTimeWarning.Visible = false;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (cbxSetAsNull.Checked)
            {
                // In case the user chose to Nullify a BLOB field - dispose of the dynamic file byte provider.
                // Otherwise - we'll have a resource leakage.
                if (_blobProvider != null && _blobProvider is Be.Windows.Forms.DynamicFileByteProvider)
                {
                    Be.Windows.Forms.DynamicFileByteProvider dp = _blobProvider as Be.Windows.Forms.DynamicFileByteProvider;
                    dp.Dispose();
                }

                _value = DBNull.Value;
            }
            else if (tbcTypes.SelectedTab == tbpEditBlob)
            {
                if (_column.ColumnType.TypeName == null ||
                    _column.ColumnType.TypeName == string.Empty ||
                    _column.ColumnType.TypeName.ToLower() == "blob")
                    _value = _blobProvider;
                else
                {
                    // Non BLOB column cannot be written as BLOB so I convert
                    // its value to string.
                    byte[] arr = new byte[_blobProvider.Length];
                    for (int i = 0; i < _blobProvider.Length; i++)
                        arr[i] = _blobProvider.ReadByte(i);
                    _value = Encoding.ASCII.GetString(arr);
                }
            }
            else if (tbcTypes.SelectedTab == tbpEditBoolean)
            {
                _value = rbtnTrue.Checked;
            }
            else if (tbcTypes.SelectedTab == tbpEditGuid)
            {
                _value = new Guid(txtGuid.Text);
            }
            else if (tbcTypes.SelectedTab == tbpEditFloatingPoint)
            {
                if (Utils.GetDbType(_column.ColumnType) == DbType.Single)
                    _value = float.Parse(txtFloatingPoint.Text);
                else
                    _value = double.Parse(txtFloatingPoint.Text);
            }
            else if (tbcTypes.SelectedTab == tbpEditInteger)
            {
                _value = (long)numInteger.Value;
            }
            else if (tbcTypes.SelectedTab == tbpEditText)
            {
                _value = txtValue.Text;
            }
            else if (tbcTypes.SelectedTab == tbpEditDateTime)
            {
                _value = new DateTime(dtpDate.Value.Year, dtpDate.Value.Month, dtpDate.Value.Day, dtpTime.Value.Hour,
                    dtpTime.Value.Minute, dtpTime.Value.Second, dtpTime.Value.Millisecond);
            }
            else
                throw new ArgumentException("Illegal tab page (name=" + tbcTypes.SelectedTab.Name + ")");

            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (_blobProvider != null)
            {
                Be.Windows.Forms.DynamicFileByteProvider dp = _blobProvider as Be.Windows.Forms.DynamicFileByteProvider;
                if (dp != null)
                    dp.Dispose();
            }

            this.DialogResult = DialogResult.Cancel;
        }

        private void CellValueEditorDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                Be.Windows.Forms.DynamicFileByteProvider dp = _blobProvider as Be.Windows.Forms.DynamicFileByteProvider;
                if (dp != null)
                    dp.Dispose();
            }
        }

        private void txtGuid_KeyDown(object sender, KeyEventArgs e)
        {
            if (!(e.KeyCode == Keys.A ||
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
                e.KeyCode == Keys.D9))
                e.SuppressKeyPress = true;
        }

        private void txtGuid_TextChanged(object sender, EventArgs e)
        {
            UpdateState();
        }

        private void txtFloatingPoint_TextChanged(object sender, EventArgs e)
        {
            UpdateState();
        }

        private void btnLoadFrom_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            DialogResult res = dlg.ShowDialog(this);
            if (res == DialogResult.Cancel)
                return;
            string fpath = dlg.FileName;

            FileInfo fi = new FileInfo(fpath);
            if (fi.Length > MAX_BLOB_FILESIZE_MB*1000*1024)
            {
                res = MessageBox.Show(this,
                    "SQLite does not handle BLOBs larger than "+MAX_BLOB_FILESIZE_MB+"MB very well.\r\n" +
                    "It is highly recommended that you don't add such large BLOB\r\n" +
                    "fields into the database due to performance reasons.\r\n" +
                    "Do you want to abort the operation?",
                    "Warning",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);
                if (res == DialogResult.Yes)
                    return;
            }

            if (_blobProvider != null)
            {
                Be.Windows.Forms.DynamicFileByteProvider dp = _blobProvider as Be.Windows.Forms.DynamicFileByteProvider;
                if (dp != null)
                    dp.Dispose();
            }

            try
            {
                // Copy the file to the temporary BLOB file
                if (File.Exists(Configuration.TempBlobFilePath))
                    File.Delete(Configuration.TempBlobFilePath);
                File.Copy(fpath, Configuration.TempBlobFilePath);
            }
            catch(Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Internal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            } // catch

            _blobProvider = new DynamicFileByteProvider(Configuration.TempBlobFilePath);
            ucHexEditor.ByteProvider = _blobProvider;
            lblBlobSize.Text = "Blob contains " + Utils.FormatMemSize(fi.Length, MemFormat.KB);
        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            DialogResult res = dlg.ShowDialog(this);
            if (res == DialogResult.Cancel)
                return;
            string fpath = dlg.FileName;

            try
            {
                if (File.Exists(fpath))
                    File.Delete(fpath);
                File.Copy(Configuration.TempBlobFilePath, fpath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Save Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } // catch
        }

        private void tbcTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateState();
        }
        #endregion

        #region Private Methods
        private void UpdateState()
        {
            cbxSetAsNull.Enabled = _nullable;
            tbcTypes.Enabled = !cbxSetAsNull.Checked;

            if (tbcTypes.SelectedTab == tbpEditFloatingPoint)
            {
                double value;
                bool legal = double.TryParse(txtFloatingPoint.Text, out value);
                btnOK.Enabled = legal;
            }
            else if (tbcTypes.SelectedTab == tbpEditGuid)
            {
                btnOK.Enabled = txtGuid.MaskCompleted;
            }
            else
                btnOK.Enabled = true;
        }

        private void SetTabPageValue(TabPage tbp, object value)
        {
            if (value == DBNull.Value || value == null)
            {
                lblBlobSize.Text = "Blob contains 0KB";
                if (tbp == tbpEditBlob)
                {
                    string fpath = Configuration.TempBlobFilePath;
                    if (File.Exists(fpath))
                        File.Delete(fpath);
                    File.Create(fpath).Dispose();
                    _blobProvider = new DynamicFileByteProvider(fpath);
                    ucHexEditor.ByteProvider = _blobProvider;
                }
                else if (tbp == tbpEditDateTime)
                    pnlDateTimeWarning.Visible = false;

                return;
            }

            if (tbp == tbpEditBlob)
            {
                if (value is Be.Windows.Forms.DynamicFileByteProvider)
                {
                    _blobProvider = (Be.Windows.Forms.DynamicFileByteProvider)value;
                    ucHexEditor.ByteProvider = _blobProvider;
                }
                else
                {
                    string fpath = Configuration.TempBlobFilePath;
                    if (File.Exists(fpath))
                        File.Delete(fpath);
                    File.Create(fpath).Dispose();
                    if (value is byte[])
                        File.WriteAllBytes(fpath, (byte[])value);
                    else
                        File.WriteAllText(fpath, value.ToString());
                    _blobProvider = new DynamicFileByteProvider(fpath);
                    ucHexEditor.ByteProvider = _blobProvider;                    
                }
                lblBlobSize.Text = "Blob contains " + Utils.FormatMemSize(_blobProvider.Length, MemFormat.KB);
            }
            else if (tbp == tbpEditBoolean)
            {
                rbtnTrue.Checked = (bool)value;
                rbtnFalse.Checked = !(bool)value;
            }
            else if (tbp == tbpEditGuid)
            {
                if (value is Guid)
                    txtGuid.Text = ((Guid)value).ToString("D");
                else if (value is string)
                {
                    // Issue a warning panel!
                }
                else
                    throw new ArgumentException("illegal value type [" + value.GetType().FullName + "]");
            }
            else if (tbp == tbpEditFloatingPoint)
            {
                if (value is float)
                    txtFloatingPoint.Text = ((float)value).ToString();
                else if (value is double)
                    txtFloatingPoint.Text = ((double)value).ToString();
                else if (value is decimal)
                    txtFloatingPoint.Text = ((decimal)value).ToString();
                else
                    throw new ArgumentException("Illegal value type [" + value.GetType().FullName + "]");
            }
            else if (tbp == tbpEditInteger)
            {
                if (value is sbyte)
                    numInteger.Value = (sbyte)value;
                else if (value is byte)
                    numInteger.Value = (byte)value;
                else if (value is short)
                    numInteger.Value = (short)value;
                else if (value is ushort)
                    numInteger.Value = (ushort)value;
                else if (value is int)
                    numInteger.Value = (int)value;
                else if (value is uint)
                    numInteger.Value = (uint)value;
                else if (value is long)
                    numInteger.Value = (long)value;
                else if (value is ulong)
                    numInteger.Value = (ulong)value;
                else
                    throw new ArgumentException("Illegal value type [" + value.GetType().FullName + "]");
            }
            else if (tbp == tbpEditText)
            {
                if (value is char)
                    txtValue.Text = string.Empty + (char)value;
                else if (value is string)
                    txtValue.Text = (string)value;
                else if (value is byte[])
                    txtValue.Text = Encoding.ASCII.GetString((byte[])value);
                else if (value is Guid)
                    txtValue.Text = ((Guid)value).ToString();
                else
                    throw new ArgumentException("Illegal value type [" + value.GetType().FullName + "]");
            }
            else if (tbp == tbpEditDateTime)
            {
                if (value is DateTime)
                {
                    DateTime dt = (DateTime)value;

                    if (dt >= dtpDate.MinDate)
                    {
                        dtpDate.Value = dt;
                        dtpTime.Value = dt;
                        pnlDateTimeWarning.Visible = false;
                    }
                    else
                        pnlDateTimeWarning.Visible = true;
                }
                else
                {
                    pnlDateTimeWarning.Visible = true;
                }
            }
            else
                throw new ArgumentException("Illegal tab page was provided (name=" + tbp.Name + ")");
        }

        private void RemoveAllPagesExcept(params TabPage[] tbp)
        {
            // Remove all other tab pages in order to prevent the user from editing using some other type.
            List<TabPage> remove = new List<TabPage>();
            foreach (TabPage page in tbcTypes.TabPages)
            {
                bool found = false;
                foreach (TabPage tp in tbp)
                {
                    if (Object.ReferenceEquals(tp, page))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                    remove.Add(page);
            } // foreach
            foreach (TabPage page in remove)
                tbcTypes.TabPages.Remove(page);
        }

        private TabPage GetTabPageForColumnType(SQLiteColumnStatement column)
        {
            TabPage res = null;
            DbType dbtype = Utils.GetDbType(column.ColumnType);
            switch (dbtype)
            {
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.String:
                case DbType.StringFixedLength:
                case DbType.Xml:
                    res = tbpEditText;
                    break;

                case DbType.Guid:
                    res = tbpEditGuid;
                    break;

                case DbType.Binary:
                case DbType.Object:
                    res = tbpEditBlob;
                    break;

                case DbType.Boolean:
                    res = tbpEditBoolean;
                    break;

                case DbType.Date:
                case DbType.DateTime:                   
                case DbType.DateTime2:
                case DbType.DateTimeOffset:
                case DbType.Time:
                    res = tbpEditDateTime;
                    break;

                case DbType.Byte:
                case DbType.Int16:
                case DbType.Int32:
                case DbType.Int64:
                case DbType.SByte:
                case DbType.UInt16:
                case DbType.UInt32:
                case DbType.UInt64:
                    res = tbpEditInteger;
                    break;

                case DbType.Currency:
                case DbType.Decimal:
                case DbType.Double:
                case DbType.Single:
                case DbType.VarNumeric:
                    res = tbpEditFloatingPoint;
                    break;

                default:
                    throw new ArgumentException("illegal column dbtype [" + dbtype.ToString() + "]");
            } // switch

            return res;
        }

        private TabPage GetTabPageForValueType(object value)
        {
            if (value is string || value is char)
                return tbpEditText;
            if (value is byte || value is sbyte || value is short || value is ushort || value is int ||
                value is uint || value is long || value is ulong)
                return tbpEditInteger;
            if (value is float || value is double || value is decimal)
                return tbpEditFloatingPoint;
            if (value is Be.Windows.Forms.DynamicFileByteProvider)
                return tbpEditBlob;
            if (value is bool)
                return tbpEditBoolean;
            if (value is DateTime)
                return tbpEditDateTime;
            if (value is Guid)
                return tbpEditGuid;
            return tbpEditText;
        }
        #endregion

        #region Constants
        /// <summary>
        /// The maximum size of BLOB files. Warn the user if he tries to
        /// store larger files into database BLOB fields.
        /// </summary>
        private const int MAX_BLOB_FILESIZE_MB = 100;
        #endregion

        #region Private Variables
        private bool _nullable;
        private TabPage _origTabPage;
        private object _value;
        private Be.Windows.Forms.IByteProvider _blobProvider;
        private SQLiteColumnStatement _column;
        private SQLiteCreateTableStatement _table;
        #endregion
    }
}