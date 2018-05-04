using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using SQLiteParser;
using FastGridApp;
using log4net;

namespace SQLiteTurbo
{
    /// <summary>
    /// This control is responsible to provide a diff view for comparing table rows.
    /// </summary>
    public partial class TableDiffControl : UserControl
    {
        #region Events
        public event EventHandler RowsChanged;
        public event EventHandler StateChanged;
        public event EventHandler SearchRequested;
        #endregion

        #region Constructors
        public TableDiffControl()
        {
            InitializeComponent();

            _italic = new Font(this.Font, FontStyle.Italic);
            _searchDialog = new SearchDataRowsDialog();
        }
        #endregion

        #region Public Methods
        public void SetTableChanges(SchemaComparisonItem item, string leftdb, string rightdb, TableChanges changes, string diff)
        {
            _item = item;
            _leftdb = leftdb;
            _rightdb = rightdb;
            _diff = diff;
            _tableChanges = changes;

            PrepareDataTab();
        }

        /// <summary>
        /// Opens the search data dialog
        /// </summary>
        public void SearchData()
        {
            bool isLeft = grdLeft.Focused;
            SQLiteCreateTableStatement table = 
                isLeft ? (SQLiteCreateTableStatement)_item.LeftDdlStatement : (SQLiteCreateTableStatement)_item.RightDdlStatement;

            FastGrid grid = isLeft ? grdLeft : grdRight;
            FastGridLocation loc = grid.SelectedCellLocation;
            FastGridColumn fcol = grid.Columns[loc.ColumnIndex];
            string cname = (string)fcol.Tag;

            _searchDialog.PrepareDialog(table.Columns, cname, _diff, _tableChanges, loc.RowIndex+1, isLeft);
            DialogResult res = _searchDialog.ShowDialog(this);
            if (res == DialogResult.OK && _searchDialog.MatchedRowIndex != -1)
            {
                FastGridSelection sel = new FastGridSelection();
                sel.AddSelection(_searchDialog.MatchedRowIndex, _searchDialog.MatchedRowIndex);

                FastGridLocation nloc = new FastGridLocation(_searchDialog.MatchedRowIndex, loc.ColumnIndex);
                grdLeft.SelectedCellLocation = nloc;
                grdLeft.Selection = sel;
                grdRight.SelectedCellLocation = nloc;
                grdRight.Selection = (FastGridSelection)sel.Clone();
            }
        }

        #endregion

        #region Event Handlers
        private void grdLeft_ColumnResized(object sender, ColumnResizedEventArgs e)
        {
            if (e.ColumnIndex < grdRight.Columns.Count)
            {
                grdRight.Columns[e.ColumnIndex].Width = e.UpdatedWidth;
                grdRight.RefreshLayout();
                grdRight.Refresh();
            }
        }

        private void grdLeft_LayoutChanged(object sender, EventArgs e)
        {
            HandleGridLayout();
        }

        private void grdLeft_RowNeeded(object sender, RowNeededEventArgs e)
        {
            FillRow(grdLeft, e.RowIndex, e.NeededRow);
        }

        private void grdLeft_Scroll(object sender, EventArgs e)
        {
            scbHorizontal.Value = grdLeft.HorizontalScrollPosition;
            scbRight.Value = (int)grdLeft.FirstDisplayedRowIndex;
            grdRight.FirstDisplayedRowIndex = scbRight.Value;
            grdRight.HorizontalScrollPosition = scbHorizontal.Value;
            selectionBar1.CursorOffset = scbRight.Value;
            Refresh();
        }

        private void grdLeft_SelectionChanged(object sender, EventArgs e)
        {
            grdRight.Selection = grdLeft.Selection;
            selectionBar1.SetData(grdLeft.RowsCount-1, grdLeft.Selection);
            UpdateState();
        }

        private void grdRight_ColumnResized(object sender, ColumnResizedEventArgs e)
        {
            if (e.ColumnIndex < grdLeft.Columns.Count)
            {
                grdLeft.Columns[e.ColumnIndex].Width = e.UpdatedWidth;
                grdLeft.RefreshLayout();
                grdLeft.Refresh();
            }
        }

        private void grdRight_LayoutChanged(object sender, EventArgs e)
        {
            HandleGridLayout();
        }

        private void grdRight_RowNeeded(object sender, RowNeededEventArgs e)
        {
            FillRow(grdRight, e.RowIndex, e.NeededRow);
        }

        private void grdRight_Scroll(object sender, EventArgs e)
        {
            scbHorizontal.Value = grdRight.HorizontalScrollPosition;
            scbRight.Value = (int)grdRight.FirstDisplayedRowIndex;
            grdLeft.FirstDisplayedRowIndex = scbRight.Value;
            grdLeft.HorizontalScrollPosition = scbHorizontal.Value;
            selectionBar1.CursorOffset = scbRight.Value;
            Refresh();
        }

        private void grdRight_SelectionChanged(object sender, EventArgs e)
        {
            grdLeft.Selection = grdRight.Selection;
            selectionBar1.SetData(grdLeft.RowsCount-1, grdLeft.Selection);
            UpdateState();
        }

        private void scbRight_Scroll(object sender, ScrollEventArgs e)
        {
            grdLeft.FirstDisplayedRowIndex = scbRight.Value;
            grdRight.FirstDisplayedRowIndex = scbRight.Value;
            selectionBar1.CursorOffset = scbRight.Value;
            Refresh();
        }

        private void scbHorizontal_Scroll(object sender, ScrollEventArgs e)
        {
            grdLeft.HorizontalScrollPosition = scbHorizontal.Value;
            grdRight.HorizontalScrollPosition = scbHorizontal.Value;
            Refresh();
        }

        private void grdRight_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            HandleCellEdit(true);
        }

        private void grdLeft_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            HandleCellEdit(false);
        }

        private void grdLeft_Enter(object sender, EventArgs e)
        {
            btnEditRow.Image = imageList2.Images[0];
            btnDeleteRows.Image = imageList2.Images[2];
            _lastFocusedGrid = grdLeft;

            if (StateChanged != null)
                StateChanged(this, EventArgs.Empty);
        }

        private void grdRight_Enter(object sender, EventArgs e)
        {
            btnEditRow.Image = imageList2.Images[1];
            btnDeleteRows.Image = imageList2.Images[3];
            _lastFocusedGrid = grdRight;

            if (StateChanged != null)
                StateChanged(this, EventArgs.Empty);
        }

        private void btnCopyLeftToRight_Click(object sender, EventArgs e)
        {
            List<TableChangesRange> rows = new List<TableChangesRange>();
            foreach (FastGridApp.SelectionRange range in grdLeft.Selection.SelectionRanges)
                rows.Add(new TableChangesRange(range.StartRowId, range.EndRowId));

            RowsCopier copier = new RowsCopier(_tableChanges, _diff, rows, true);
            ProgressDialog dlg = new ProgressDialog();
            dlg.Start(this, copier);
            grdRight.RefreshLayout();
            grdLeft.RefreshLayout();

            if (RowsChanged != null)
                RowsChanged(this, EventArgs.Empty);

            if (StateChanged != null)
                StateChanged(this, EventArgs.Empty);
        }

        private void btnCopyRightToLeft_Click(object sender, EventArgs e)
        {
            List<TableChangesRange> rows = new List<TableChangesRange>();
            foreach (FastGridApp.SelectionRange range in grdLeft.Selection.SelectionRanges)
                rows.Add(new TableChangesRange(range.StartRowId, range.EndRowId));
            RowsCopier copier = new RowsCopier(_tableChanges, _diff, rows, false);
            ProgressDialog dlg = new ProgressDialog();
            dlg.Start(this, copier);
            grdRight.RefreshLayout();
            grdLeft.RefreshLayout();

            if (RowsChanged != null)
                RowsChanged(this, EventArgs.Empty);

            if (StateChanged != null)
                StateChanged(this, EventArgs.Empty);
        }

        private void btnEditRow_Click(object sender, EventArgs e)
        {
            HandleEdit();
        }

        private void btnDeleteRows_Click(object sender, EventArgs e)
        {
            if (_lastFocusedGrid == null)
                return;

            if (object.ReferenceEquals(_lastFocusedGrid, grdLeft))
                DeleteRows(false);
            else
                DeleteRows(true);

            _lastFocusedGrid.Focus();

            if (StateChanged != null)
                StateChanged(this, EventArgs.Empty);
        }

        private void grdLeft_EditStarted(object sender, EventArgs e)
        {
            HandleEdit();
        }

        private void grdRight_EditStarted(object sender, EventArgs e)
        {
            HandleEdit();
        }

        private void grdRight_SearchRequested(object sender, EventArgs e)
        {
            if (SearchRequested != null)
                SearchRequested(this, EventArgs.Empty);
        }

        private void grdLeft_SearchRequested(object sender, EventArgs e)
        {
            if (SearchRequested != null)
                SearchRequested(this, EventArgs.Empty);
        }

        #endregion

        #region Private Methods

        private bool CanEditGrid(bool right, out TableChangeItem citem, out FastGridLocation sloc)
        {
            citem = null;
            sloc = FastGridLocation.Empty;
            if (_item == null)
                return false;

            FastGrid grid;
            SQLiteCreateTableStatement table;
            string dbpath;
            if (right)
            {
                grid = grdRight;
                table = (SQLiteCreateTableStatement)_item.RightDdlStatement;
                dbpath = _rightdb;
            }
            else
            {
                grid = grdLeft;
                table = (SQLiteCreateTableStatement)_item.LeftDdlStatement;
                dbpath = _leftdb;
            }

            sloc = grid.SelectedCellLocation;

            string columnName = (string)grid.Columns[sloc.ColumnIndex].Tag;
            SQLiteColumnStatement column = Utils.FindColumn(table.Columns, columnName);
            if (column == null)
                return false;

            long total = _tableChanges.GetTotalChangesCount(new string[] { _diff });
            if (sloc.RowIndex >= total)
                return false;
            citem = _tableChanges.GetChangeItem(_diff, sloc.RowIndex);

            // Can't edit a cell that belongs to a row that doesn't exist
            if (right && citem.Result == ComparisonResult.ExistsInLeftDB)
                return false;
            else if (!right && citem.Result == ComparisonResult.ExistsInRightDB)
                return false;

            return true;
        }

        private void HandleEdit()
        {
            if (_lastFocusedGrid == null)
                return;

            if (object.ReferenceEquals(_lastFocusedGrid, grdLeft))
                HandleCellEdit(false);
            else
                HandleCellEdit(true);

            _lastFocusedGrid.Focus();
        }

        private void UpdateState()
        {
            bool canEdit = true;

            FastGrid grid = null;
            if (grdLeft.Focused)
                grid = grdLeft;
            else if (grdRight.Focused)
                grid = grdRight;

            if (grid == null || grid.RowsCount == 0)
                canEdit = false;

            btnEditRow.Enabled = canEdit;

            bool enabled = grid != null && grid.Selection.SelectionRanges.Count > 0;
            if (btnCopyRightToLeft.Enabled != enabled)
                btnCopyLeftToRight.Enabled = enabled;
            if (btnCopyRightToLeft.Enabled != enabled)
                btnCopyRightToLeft.Enabled = enabled;
            if (btnDeleteRows.Enabled != enabled)
                btnDeleteRows.Enabled = enabled;
        }

        private object GetRowFieldValue(TableChangeItem citem, bool right, string columnName)
        {
            if (right)
            {
                for (int i = 0; i < citem.RightColumnNames.Length; i++)
                {
                    if (citem.RightColumnNames[i].ToLower() == columnName.ToLower())
                        return citem.RightFields[i];
                } // for
            }
            else
            {
                for (int i = 0; i < citem.LeftColumnNames.Length; i++)
                {
                    if (citem.LeftColumnNames[i].ToLower() == columnName.ToLower())
                        return citem.LeftFields[i];
                } // for
            }

            throw new ArgumentException("Illegal row index, column index or column name");
        }

        private DialogResult OpenCellEditDialog(SQLiteCreateTableStatement table, SQLiteColumnStatement column, ref object value)
        {
            DialogResult res;
            CellValueEditorDialog dlg = new CellValueEditorDialog();
            res = dlg.ShowEditor(this, table, column, ref value);
            return res;
        }

        private void DeleteRows(bool right)
        {
            List<TableChangesRange> rows = new List<TableChangesRange>();
            FastGrid grid;

            long totalDeleted = 0;
            if (right)
            {
                grid = grdRight;
                foreach (FastGridApp.SelectionRange range in grdRight.Selection.SelectionRanges)
                {
                    rows.Add(new TableChangesRange(range.StartRowId, range.EndRowId));
                    totalDeleted += range.EndRowId - range.StartRowId + 1;
                }
            }
            else
            {
                grid = grdLeft;
                foreach (FastGridApp.SelectionRange range in grdLeft.Selection.SelectionRanges)
                {
                    rows.Add(new TableChangesRange(range.StartRowId, range.EndRowId));
                    totalDeleted += range.EndRowId - range.StartRowId + 1;
                }
            }

            DialogResult res = MessageBox.Show(this, 
                string.Format("Are you sure you want to delete {0} rows from the {1} database table?", totalDeleted, (right?"right":"left")),
                "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (res == DialogResult.No)
                return;

            RowsDeleter deleter = new RowsDeleter(_tableChanges, _diff, rows, !right);
            ProgressDialog dlg = new ProgressDialog();
            dlg.Start(this, deleter);

            GC.Collect();

            // Notify that row(s) were deleted
            if (RowsChanged != null)
                RowsChanged(this, EventArgs.Empty);

            // Refresh the right/left grids again from the database.
            grdRight.RefreshLayout();
            grdLeft.RefreshLayout();
        }

        private void HandleCellEdit(bool right)
        {
            FastGrid grid;
            SQLiteCreateTableStatement table;
            string dbpath;
            if (right)
            {
                grid = grdRight;
                table = (SQLiteCreateTableStatement)_item.RightDdlStatement;
                dbpath = _rightdb;
            }
            else
            {
                grid = grdLeft;
                table = (SQLiteCreateTableStatement)_item.LeftDdlStatement;
                dbpath = _leftdb;
            }

            FastGridLocation sloc = grid.SelectedCellLocation;

            string columnName = (string)grid.Columns[sloc.ColumnIndex].Tag;
            SQLiteColumnStatement column = Utils.FindColumn(table.Columns, columnName);
            if (column == null)
                return;

            TableChangeItem citem = null;
            try
            {
                citem = _tableChanges.GetChangeItem(_diff, sloc.RowIndex);
            }
            catch (IndexOutOfRangeException iox)
            {
                // Ignore (the user must have pressed on a cell when the table is actually empty)
                return;
            } // catch

            // Can't edit a cell that belongs to a row that doesn't exist
            if (right && citem.Result == ComparisonResult.ExistsInLeftDB)
                return;
            else if (!right && citem.Result == ComparisonResult.ExistsInRightDB)
                return;

            object otherBlob = null;

            // The row id is needed when displaying editor dialog for a BLOB field
            long rowId = -1;
            if (right)
            {
                rowId = citem.RightRowId;
                if (citem.LeftFields != null && TableContainsColumn((SQLiteCreateTableStatement)_item.LeftDdlStatement, column))
                    otherBlob = GetRowFieldValue(citem, false, columnName);
            }
            else
            {
                rowId = citem.LeftRowId;
                if (citem.RightFields != null && TableContainsColumn((SQLiteCreateTableStatement)_item.RightDdlStatement, column))
                    otherBlob = GetRowFieldValue(citem, true, columnName);
            }

            // Extract the current field value from the change item
            object value = GetRowFieldValue(citem, right, columnName);

            // Adjust the BLOB value since it was fetched as the IS NOT NULL expression
            // in order to avoid loading the BLOB field into main memory.
            if (Utils.GetDbType(column.ColumnType) == DbType.Binary)
            {
                //long v = (long)value;
                //if (v == 0) // means NULL
                if (value.Equals("0"))
                    value = DBNull.Value;
            }

            // When the user is clicking on a BLOB field - we need to extract its value
            // to a local file before opening the cell-edit dialog.
            Be.Windows.Forms.DynamicFileByteProvider origProvider = null;
            if (value != DBNull.Value && Utils.GetDbType(column.ColumnType) == DbType.Binary)
            {
                // In case of BLOBs - we have to first load them to the local file-system and
                // only then we can allow the user to edit their contents
                using (BlobLoader loader = new BlobLoader(dbpath,
                    table.ObjectName.ToString(), column.ObjectName.ToString(), rowId, Configuration.TempBlobFilePath))
                {
                    ProgressDialog dlg = new ProgressDialog();
                    dlg.Start(this, loader);

                    if (dlg.Error != null)
                        return;
                } // using

                // Instead of passing a byte[] array to the cell editor dialog - we'll pass
                // a reference to the dynamic file byte provider that is opened on the data file
                // that was written with the BLOB data. This allows us to conserve memory (BLOB
                // fields can be quite large).
                origProvider = new Be.Windows.Forms.DynamicFileByteProvider(Configuration.TempBlobFilePath);
                value = origProvider;
            }

            // Open the cell editor dialog and allow the user to edit the contents of the field.
            DialogResult res = OpenCellEditDialog(table, column, ref value);
            if (res == DialogResult.Cancel)
                return;

            // This part of the IF statement deals with BLOB fields - these require
            // a special (and lengthy) handling.
            if (Utils.GetDbType(column.ColumnType) == DbType.Binary || value is Be.Windows.Forms.DynamicFileByteProvider)
            {
                if (otherBlob != null && (citem.Result == ComparisonResult.Same || citem.Result == ComparisonResult.DifferentData))
                {
                    /*
                    if (otherBlob is long)
                    {
                        // Another adjustment for the other BLOB field value
                        long ob = (long)otherBlob;
                        if (ob == 0) // means NULL blob field
                            otherBlob = DBNull.Value;
                    }
                    */
                    if (Utils.GetDbType(column.ColumnType) == DbType.Binary)
                    {
                        if (otherBlob.Equals("0"))
                            otherBlob = DBNull.Value;
                    }

                }

                if (value != DBNull.Value)
                {
                    //string fpath = null;
                    string fpath = Configuration.TempBlobFilePath;
                    long blobLength = 0;
                    if (value is Be.Windows.Forms.DynamicFileByteProvider)
                    {
                        Be.Windows.Forms.DynamicFileByteProvider dp = (Be.Windows.Forms.DynamicFileByteProvider)value;
                        if (dp.HasChanges())
                            dp.ApplyChanges();
                        dp.Dispose();

						blobLength = dp.Length;

					}
                    else
                        throw new InvalidOperationException("cell editor returned unexpected value");

                    BlobSaver saver = null;
                    try
                    {
                        // In this case we need to store the updated file/buffer back to the BLOB field
                        // and run BLOB comparison in order to check if the BLOB is different then
                        // the one in the other database.

                        // Save the file specified in the call as a BLOB
                        saver = new BlobSaver(dbpath, table.ObjectName.ToString(), column.ObjectName.ToString(), rowId, fpath);

                        ProgressDialog dlg = new ProgressDialog();
                        dlg.Start(this, saver);
                        if (dlg.Error != null)
                        {
                            // Notify that a row has changed
                            if (RowsChanged != null)
                                RowsChanged(this, EventArgs.Empty);

                            return;
                        }
                    }
                    finally
                    {
                        saver.Dispose();
                    } // finally

                    // Update the table change item with an indication that the BLOB field that was just saved
                    // is not null.
                    //citem.SetField(column.ObjectName.ToString(), !right, (long)1);
                    citem.SetField(column.ObjectName.ToString(), !right, "1");

                    if (otherBlob != null && (citem.Result == ComparisonResult.DifferentData || citem.Result == ComparisonResult.Same))
                    {
                        // At this point we need to compare the BLOB that was saved with the BLOB field in the other 
                        // database in order to update the ChangedBlobsColumnNames field of the table change item so
                        // that the user can know if the BLOB is equal to the other BLOB or if the BLOB is different.
                        if (otherBlob != DBNull.Value)
                        {
                            // Run BLOB comparison
                            bool equalBlobs = false;
                            using (BlobCompareWorker bcw = new BlobCompareWorker(_leftdb, _rightdb,
                                SQLiteParser.Utils.Chop(table.ObjectName.ToString()),
                                SQLiteParser.Utils.Chop(column.ObjectName.ToString()),
                                citem.LeftRowId, citem.RightRowId))
                            {
                                ProgressDialog pdlg = new ProgressDialog();
                                pdlg.Start(this, bcw);
                                if (pdlg.Error != null)
                                {
                                    // Notify that a row has changed
                                    if (RowsChanged != null)
                                        RowsChanged(this, EventArgs.Empty);

                                    return;
                                }
                                equalBlobs = bcw.IsBlobsEqual;
                            } // using

                            if (equalBlobs)
                            {
                                // The two BLOBs are equal so remove any difference mark                           
                                if (citem.ChangedBlobsColumnNames != null && citem.ChangedBlobsColumnNames.Contains(column.ObjectName.ToString()))
                                    citem.ChangedBlobsColumnNames.Remove(column.ObjectName.ToString());
                            }
                            else
                            {
                                // The two BLOBs are different so add a difference mark if necessary
                                if (citem.ChangedBlobsColumnNames == null)
                                    citem.ChangedBlobsColumnNames = new List<string>();
                                if (!citem.ChangedBlobsColumnNames.Contains(column.ObjectName.ToString()))
                                    citem.ChangedBlobsColumnNames.Add(column.ObjectName.ToString());
                                citem.Result = ComparisonResult.DifferentData;
                            } // else
                        }
                        else
                        {
                            // If the other BLOB field is NULL - it means that the two BLOBs are different
                            // because one is NULL and the other is not.
                            if (citem.ChangedBlobsColumnNames == null)
                                citem.ChangedBlobsColumnNames = new List<string>();
                            if (!citem.ChangedBlobsColumnNames.Contains(column.ObjectName.ToString()))
                                citem.ChangedBlobsColumnNames.Add(column.ObjectName.ToString());
                            citem.Result = ComparisonResult.DifferentData;
                        } // else
                    } // if
                }
                else
                {
                    // Ask the table changes object to set this field to null
                    _tableChanges.SetColumnField(_diff, sloc.RowIndex, column.ObjectName.ToString(), right, DBNull.Value);
                    citem.SetField(column.ObjectName.ToString(), !right, (long)0);

                    if (otherBlob != null && (citem.Result == ComparisonResult.Same || citem.Result == ComparisonResult.DifferentData))
                    {
                        // A BLOB field was set to NULL - compare it again to the other BLOB field and decide
                        // if the ChangedBlobsColumnNames field should be updated to reflect this.
                        if (otherBlob == DBNull.Value)
                        {
                            // The other BLOB field is NULL so the two fields are equal
                            if (citem.ChangedBlobsColumnNames != null && citem.ChangedBlobsColumnNames.Contains(column.ObjectName.ToString()))
                                citem.ChangedBlobsColumnNames.Remove(column.ObjectName.ToString());
                        }
                        else
                        {
                            if (citem.ChangedBlobsColumnNames == null)
                                citem.ChangedBlobsColumnNames = new List<string>();

                            // The other BLOB field is not NULL so the two fields are not equal
                            if (!citem.ChangedBlobsColumnNames.Contains(column.ObjectName.ToString()))
                                citem.ChangedBlobsColumnNames.Add(column.ObjectName.ToString());
                            citem.Result = ComparisonResult.DifferentData;
                        } // else
                    } // if
                } // else
            }
            else
            {
                // The field that was edited is not a BLOB so we can deal with it normally
                try
                {                               
                    _tableChanges.SetColumnField(_diff, sloc.RowIndex, column.ObjectName.ToString(), right, value);
                }
                catch (Exception ex)
                {
                    _log.Error("Failed to set column field", ex);
                    MessageBox.Show(this, ex.Message, "Operation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                } // catch
            } // else

            GC.Collect();

            // Mark that the table changes object does not contain precise results
            _tableChanges.HasPreciseResults = false;

            // Notify that a row has changed
            if (RowsChanged != null)
                RowsChanged(this, EventArgs.Empty);

            // Refresh the right/left grids again from the database.
            grdRight.RefreshLayout();
            grdLeft.RefreshLayout();
        }

        /// <summary>
        /// Checks if the specified table contains a column with the specified name.
        /// </summary>
        /// <param name="table">The table to check</param>
        /// <param name="column">The column to chcek</param>
        /// <returns>TRUE if the table contains a column with the specified name, FALSE otherwise.</returns>
        private bool TableContainsColumn(SQLiteCreateTableStatement table, SQLiteColumnStatement column)
        {
            foreach (SQLiteColumnStatement tc in table.Columns)
            {
                if (tc.ObjectName.Equals(column.ObjectName))
                    return true;
            } // foreach
            return false;
        }

        /// <summary>
        /// Update the horizontal and vertical scrollbars according to changes
        /// in the left/right grids
        /// </summary>
        private void HandleGridLayout()
        {
            int maxhscroll = grdLeft.MaxHorizontalScroll;
            if (maxhscroll < grdRight.MaxHorizontalScroll)
                maxhscroll = grdRight.MaxHorizontalScroll;

            int lchange = grdLeft.HorizontalScrollPageSize;
            if (lchange > grdRight.HorizontalScrollPageSize)
                lchange = grdRight.HorizontalScrollPageSize;

            if (scbHorizontal.Maximum != maxhscroll ||
                scbHorizontal.LargeChange != lchange)
            {
                scbHorizontal.Maximum = maxhscroll;
                scbHorizontal.LargeChange = lchange;               
                scbHorizontal.SmallChange = (int)Math.Ceiling(lchange / 10F);
            }

            if (scbRight.Maximum != grdLeft.RowsCount - 1 ||
                scbRight.LargeChange != grdLeft.VerticalPageSize)
            {
                if (grdLeft.RowsCount - 1 >= 0)
                {
                    scbRight.Maximum = (int)(grdLeft.RowsCount - 1);
                    scbRight.LargeChange = grdLeft.VerticalPageSize;
                }
                else
                {
                    scbRight.Maximum = 100;
                    scbRight.LargeChange = 101;
                }
            }
        }

        /// <summary>
        /// This purpose of this method is to fill the needed row with data
        /// taken from the table-changes object. This is part of a virtual mode
        /// implementation of the FastGrid control designed to minimize memory
        /// requirements only to the bare essentials.
        /// </summary>
        /// <param name="grid">The grid that issued to the RowNeeded request</param>
        /// <param name="rowIndex">The index of the row to fill</param>
        /// <param name="row">The row to fill</param>
        private void FillRow(FastGrid grid, long rowIndex, FastGridRow row)
        {
            // Fetch the table change item from the cache/database
            TableChangeItem item = _tableChanges.GetChangeItem(_diff, rowIndex);

            bool empty;
            bool isLeft;
            SQLiteCreateTableStatement table = null;
            if (grid == grdLeft)
            {
                isLeft = true;
                empty = item.LeftFields == null;
                table = _tableChanges.LeftTable;
            }
            else
            {
                isLeft = false;
                empty = item.RightFields == null;
                table = _tableChanges.RightTable;
            } // else

            if (empty)
            {
                for (int i = 0; i < row.Cells.Length; i++)
                    row.Cells[i].Style.BackColor = EMPTY_ROW_BACK_COLOR;
            }
            else
            {
                for (int i = 0; i < row.Cells.Length; i++)
                {
                    string cname = (string)grid.Columns[i].Tag;
                    SQLiteColumnStatement col = Utils.GetColumnByName(table, cname);
                    object fval = item.GetField(cname, isLeft);
                    if (fval == DBNull.Value || fval == null)
                    {
                        row.Cells[i].Value = "NULL";
                        row.Cells[i].Style.Font = _italic;
                    }
                    else if (Utils.GetDbType(col.ColumnType) == DbType.Binary)
                    {
                        //long v = (long)fval;
                        //if (v == 1)
                        if (fval.Equals("1"))
                            row.Cells[i].Value = "BLOB";
                        else
                            row.Cells[i].Value = "NULL";
                        row.Cells[i].Style.Font = _italic;
                    }
                    else
                    {
                        if (fval is byte[])
                        {
                            string tmp = Encoding.ASCII.GetString((byte[])fval);
                            row.Cells[i].Value = tmp;
                        }
                        else
                        {
                            row.Cells[i].Value = fval;
                        }
                    } // else

                    // Mark different cells with special background color
                    if (item.Result == ComparisonResult.DifferentData)
                    {
                        if (i < item.LeftFields.Length && i < item.RightFields.Length)
                        {
                            if (item.ChangedBlobsColumnNames != null && item.ChangedBlobsColumnNames.Contains(col.ObjectName.ToString()))
                                row.Cells[i].Style.BackColor = DIFFERENT_CELL_BACK_COLOR;
                            else 
                            {
                                // Check only if the field appears in both tables and has different values
                                if (item.HasField(cname,true) && item.HasField(cname,false) && !item.GetField(cname, true).Equals(item.GetField(cname, false)))
                                {
                                    object tmp = item.GetField(cname, isLeft);
                                    if (tmp is long && ((long)tmp) == 0 && Utils.GetDbType(col.ColumnType) == DbType.Binary &&
                                        item.GetField(cname, !isLeft) == DBNull.Value)
                                    {
                                        // Ignore the case when the values are not equal when one of the fields is a BLOB and the
                                        // other is not, but both values indicate NULL content.
                                    }
                                    else
                                    {
                                        tmp = item.GetField(cname, !isLeft);
                                        SQLiteCreateTableStatement tbl;
                                        if (isLeft)
                                            tbl = _tableChanges.RightTable;
                                        else
                                            tbl = _tableChanges.LeftTable;
                                        object tmp2 = item.GetField(cname, isLeft);
                                        SQLiteColumnStatement ocol = Utils.GetColumnByName(tbl.Columns, cname);
                                        if (tmp2 == DBNull.Value && tmp != null && tmp is long && ((long)tmp) == 0)
                                        {
                                            // Ignore the reverse case when both fields are actually NULL, but one of them is BLOB
                                            // and ther other is not.
                                        }
                                        else
                                            row.Cells[i].Style.BackColor = DIFFERENT_CELL_BACK_COLOR;
                                    } // else
                                } // if
                            } // else
                        } // if
                    } // if
                } // for
            } // else
        }

        private void PrepareDataTab()
        {
            lblLeftPath.Text = _leftdb;
            lblRightPath.Text = _rightdb;

            _total = _tableChanges.GetTotalChangesCount(new string[] { _diff });

            grdLeft.RowsCount = 0;
            grdRight.RowsCount = 0;

            // Add columns to both grids
            FillColumns();

            // Set the total number of rows to display
            grdLeft.RowsCount = _total;
            grdRight.RowsCount = _total;

            // Start with first line
            if (_total > 0)
            {
                grdLeft.FirstDisplayedRowIndex = 0;
                grdRight.FirstDisplayedRowIndex = 0;
            }

            // Update the selection bar control
            selectionBar1.SetData(_total, grdLeft.Selection);
            selectionBar1.CursorOffset = 0;

            UpdateState();
        }

        private void FillColumns()
        {
            grdLeft.Columns.Clear();
            grdRight.Columns.Clear();

            // Add columns to both grids
            List<SQLiteColumnStatement> common = Utils.GetCommonColumns(
                (SQLiteCreateTableStatement)_item.LeftDdlStatement,
                (SQLiteCreateTableStatement)_item.RightDdlStatement);

            FillGridColumns(grdLeft, common, (SQLiteCreateTableStatement)_item.LeftDdlStatement);
            FillGridColumns(grdRight, common, (SQLiteCreateTableStatement)_item.RightDdlStatement);
        }

        private void FillGridColumns(FastGrid grid, List<SQLiteColumnStatement> common, SQLiteCreateTableStatement table)
        {
            // Construct a list of columns that starts with all the primary key columns
            // and followed by all other columns that are common to both tables.
            List<SQLiteColumnStatement> pkeys = Utils.GetPrimaryColumns(table);
            List<SQLiteColumnStatement> cols = new List<SQLiteColumnStatement>();
            foreach (SQLiteColumnStatement cs in common)
            {
                bool found = false;
                for (int i = 0; i < pkeys.Count; i++)
                {
                    if (pkeys[i].ObjectName.Equals(cs.ObjectName))
                    {
                        found = true;
                        break;
                    }
                } // for
                if (!found)
                    cols.Add(cs);
            } // foreach

            // Construct a list of columns that are unique to this table (not common with another table)
            List<SQLiteColumnStatement> mycols = new List<SQLiteColumnStatement>();
            foreach (SQLiteColumnStatement c in table.Columns)
            {
                bool found = false;
                foreach (SQLiteColumnStatement cm in common)
                {
                    if (c.ObjectName.Equals(cm.ObjectName))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                    mycols.Add(c);
            } // foreach

            grid.Columns.Clear();

            // Add primary key columns
            for (int i = 0; i < pkeys.Count; i++)
                grid.Columns.Add(MakeGridColumn(true, GetColumnFromTable(pkeys[i].ObjectName, table.Columns)));

            // Add common columns
            for (int i = 0; i < cols.Count; i++)
                grid.Columns.Add(MakeGridColumn(false, GetColumnFromTable(cols[i].ObjectName, table.Columns)));

            // Add non common columns
            for (int i = 0; i < mycols.Count; i++)
                grid.Columns.Add(MakeGridColumn(false, GetColumnFromTable(mycols[i].ObjectName, table.Columns)));

            grid.RefreshLayout();
        }

        private SQLiteColumnStatement GetColumnFromTable(SQLiteObjectName name, List<SQLiteColumnStatement> clist)
        {
            foreach (SQLiteColumnStatement col in clist)
            {
                if (col.ObjectName.Equals(name))
                    return col;
            } // foreach
            return null;
        }

        private FastGridColumn MakeGridColumn(bool pkey, SQLiteColumnStatement col)
        {
            string cname = SQLiteParser.Utils.Chop(col.ObjectName.ToString());
            FastGridColumn res = new FastGridColumn(cname);
            if (Utils.GetDbType(col.ColumnType) == DbType.Boolean)
                res.ColumnType = FastGridColumnType.CheckBox;
            if (pkey)
                res.Image = imageList1.Images[0];
            res.Tag = cname;
            return res;
        }
        #endregion

        #region Constants
        private Color EMPTY_ROW_BACK_COLOR = Color.LightGray;
        private Color DIFFERENT_CELL_BACK_COLOR = Color.Khaki;
        #endregion

        #region Private Variables
        private long _total;
        private string _diff;
        private string _leftdb;
        private string _rightdb;
        private Font _italic;
        private SchemaComparisonItem _item;
        private TableChanges _tableChanges;
        private FastGrid _lastFocusedGrid;
        private SearchDataRowsDialog _searchDialog;
        private ILog _log = LogManager.GetLogger(typeof(TableDiffControl));
        #endregion
    }
}
