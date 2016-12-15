using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using SQLiteParser;

namespace SQLiteTurbo
{
    public partial class SchemaComparisonView : UserControl
    {
        #region Events
        /// <summary>
        /// Fired whenever the user changes selection in the grid.
        /// </summary>
        public event EventHandler SelectionChanged;
        #endregion

        #region Constructors
        public SchemaComparisonView()
        {
            InitializeComponent();
            _strikeout = new Font(this.Font, FontStyle.Strikeout);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Used to set the comparison results into the view
        /// </summary>
        /// <param name="results">The results object</param>
        /// <param name="leftDb">The path to the left DB file</param>
        /// <param name="rightDb">The path to the right DB file</param>
        public void ShowComparisonResults(
            Dictionary<SchemaObject, List<SchemaComparisonItem>> results,
            string leftDb, string rightDb, Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> leftSchema,
            Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> rightSchema,
            bool dataWasCompared)
        {
            _leftdb = leftDb;
            _rightdb = rightDb;
            _leftSchema = leftSchema;
            _rightSchema = rightSchema;
            colLeft.HeaderText = leftDb;
            colRight.HeaderText = rightDb;
            lblTableDataIsDifferent.Visible = dataWasCompared;
            _results = results;
            UpdateView();
            ShowMatchingRows();
        }
        
        /// <summary>
        /// Returns TRUE if there is another difference in the grid after the selected row.
        /// </summary>
        public bool HasNextDiff()
        {
            if (_adding || grdSchemaDiffs.SelectedRows.Count == 0)
                return false;
            DataGridViewRow row = grdSchemaDiffs.SelectedRows[0];
            for (int index = row.Index + 1; index < grdSchemaDiffs.Rows.Count; index++)
            {
                row = grdSchemaDiffs.Rows[index];
                if (!row.Visible)
                    continue;
                SchemaComparisonItem item = (SchemaComparisonItem)row.Tag;
                if (item.Result != ComparisonResult.Same || (item.TableChanges != null && !item.TableChanges.SameTables))
                    return true;
            } // for

            return false;
        }

        /// <summary>
        /// Move selection to the next difference in the grid
        /// </summary>
        public void MoveToNextDiff()
        {
            if (_adding || grdSchemaDiffs.SelectedRows.Count == 0)
                return;
            DataGridViewRow row = grdSchemaDiffs.SelectedRows[0];
            for (int index = row.Index+1; index < grdSchemaDiffs.Rows.Count; index++)
            {
                row = grdSchemaDiffs.Rows[index];
                if (!row.Visible)
                    continue;
                SchemaComparisonItem item = (SchemaComparisonItem)row.Tag;
                if (item.Result != ComparisonResult.Same || (item.TableChanges != null && !item.TableChanges.SameTables))
                {
                    grdSchemaDiffs.ClearSelection();
                    row.Selected = true;
                    grdSchemaDiffs.FirstDisplayedScrollingRowIndex = row.Index;
                    return;
                }                
            } // for
        }

        /// <summary>
        /// Returns TRUE if there is a previous difference before the 
        /// selected row.
        /// </summary>
        public bool HasPreviousDiff()
        {
            if (_adding || grdSchemaDiffs.SelectedRows.Count == 0)
                return false;
            DataGridViewRow row = grdSchemaDiffs.SelectedRows[0];
            for (int index = row.Index - 1; index >= 0; index--)
            {
                row = grdSchemaDiffs.Rows[index];
                if (!row.Visible)
                    continue;
                SchemaComparisonItem item = (SchemaComparisonItem)row.Tag;
                if (item.Result != ComparisonResult.Same || (item.TableChanges != null && !item.TableChanges.SameTables))
                    return true;
            } // for            
            return false;
        }

        /// <summary>
        /// Move selection to the previous difference in the grid
        /// </summary>
        public void MoveToPreviousDiff()
        {
            if (_adding || grdSchemaDiffs.SelectedRows.Count == 0)
                return;
            DataGridViewRow row = grdSchemaDiffs.SelectedRows[0];
            for (int index = row.Index - 1; index >= 0; index--)
            {
                row = grdSchemaDiffs.Rows[index];
                if (!row.Visible)
                    continue;

                SchemaComparisonItem item = (SchemaComparisonItem)row.Tag;
                if (item.Result != ComparisonResult.Same || (item.TableChanges != null && !item.TableChanges.SameTables))
                {
                    grdSchemaDiffs.ClearSelection();
                    row.Selected = true;                    
                    grdSchemaDiffs.FirstDisplayedScrollingRowIndex = row.Index;
                    return;
                }
            } // for            
        }

        /// <summary>
        /// Returns TRUE if the user can copy the DB entity from the left database
        /// to the right database.
        /// </summary>
        public bool CanCopyFromLeftDB()
        {
            if (_adding || grdSchemaDiffs.SelectedRows.Count == 0)
                return false;
            DataGridViewRow row = grdSchemaDiffs.SelectedRows[0];
            if (!row.Visible)
                return false;

            SchemaComparisonItem item = (SchemaComparisonItem)row.Tag;
            if (item.LeftDdlStatement == null && item.RightDdlStatement == null)
                return false;

            return true;
        }

        /// <summary>
        /// Copies the DB entity stored in the left DB to the right DB
        /// </summary>
        public void CopyFromLeftDB()
        {
            if (_adding || grdSchemaDiffs.SelectedRows.Count == 0)
                return;
            DataGridViewRow row = grdSchemaDiffs.SelectedRows[0];
            SchemaComparisonItem item = (SchemaComparisonItem)row.Tag;

            if (item.Result == ComparisonResult.ExistsInLeftDB ||
                item.Result == ComparisonResult.DifferentSchema ||
                item.Result == ComparisonResult.DifferentData ||
                item.Result == ComparisonResult.Same)
            {
                DialogResult res = MessageBox.Show(this,
                    "Are you sure you want to copy " + Utils.GetItemObjectTypeName(item) +
                    " " + item.ObjectName + " " +
                    "from the left database to the right datbase?",
                    "Confirm copying",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);
                if (res == DialogResult.No)
                    return;
            }
            else if (item.Result == ComparisonResult.ExistsInRightDB)
            {
                DialogResult res = MessageBox.Show(this,
                    "Are you sure you want to delete " + Utils.GetItemObjectTypeName(item) +
                    " " + item.ObjectName + " " +
                    "from the right database?", "Confirm deletion",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);
                if (res == DialogResult.No)
                    return;
            } // else
            else if (item.Result == ComparisonResult.Deleted)
                return;

            ProgressDialog dlg = new ProgressDialog();
            ItemCopier copier = new ItemCopier(_leftSchema, _rightSchema, item, _leftdb, _rightdb, true);
            dlg.Start(this, copier);
            if (dlg.Error == null)
            {
                // Update the comparison view to reflect the change that was done.
                FixView(item, row, true);

                // Notify about the change so the main form can update its sensitivity
                if (SelectionChanged != null)
                    SelectionChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Copies the DB entity stored in the right DB to the left DB
        /// </summary>
        public void CopyFromRightDB()
        {
            if (_adding || grdSchemaDiffs.SelectedRows.Count == 0)
                return;
            DataGridViewRow row = grdSchemaDiffs.SelectedRows[0];
            SchemaComparisonItem item = (SchemaComparisonItem)row.Tag;

            if (item.Result == ComparisonResult.ExistsInRightDB ||
                item.Result == ComparisonResult.DifferentSchema ||
                item.Result == ComparisonResult.DifferentData ||
                item.Result == ComparisonResult.Same)
            {
                DialogResult res = MessageBox.Show(this,
                    "Are you sure you want to copy " + Utils.GetItemObjectTypeName(item) +
                    " " + item.ObjectName + " " +
                    "from the right database to the left datbase?",
                    "Confirm copying",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);
                if (res == DialogResult.No)
                    return;
            }
            else if (item.Result == ComparisonResult.ExistsInLeftDB)
            {
                DialogResult res = MessageBox.Show(this,
                    "Are you sure you want to delete " + Utils.GetItemObjectTypeName(item) +
                    " " + item.ObjectName + " " +
                    "from the left database?", "Confirm deletion",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);
                if (res == DialogResult.No)
                    return;
            } // else
            else if (item.Result == ComparisonResult.Deleted)
                return;

            ProgressDialog dlg = new ProgressDialog();
            ItemCopier copier = new ItemCopier(_leftSchema, _rightSchema, item, _leftdb, _rightdb, false);
            dlg.Start(this, copier);
            if (dlg.Error == null)
            {
                // Update the comparison view
                FixView(item, row, false);

                // Notify about the change so the main form can update its sensitivity
                if (SelectionChanged != null)
                    SelectionChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Returns TRUE if the user can copy the DB entity from the right database
        /// to the left database.
        /// </summary>
        public bool CanCopyFromRightDB()
        {
            if (_adding || grdSchemaDiffs.SelectedRows.Count == 0)
                return false;
            DataGridViewRow row = grdSchemaDiffs.SelectedRows[0];
            if (!row.Visible)
                return false;

            SchemaComparisonItem item = (SchemaComparisonItem)row.Tag;
            if (item.LeftDdlStatement == null && item.RightDdlStatement == null)
                return false;

            return true;
        }

        /// <summary>
        /// Returns TRUE if the user can edit the selected different DB entities
        /// </summary>
        public bool CanEditSelectedDifference()
        {
            if (_adding || grdSchemaDiffs.SelectedRows.Count == 0)
                return false;
            DataGridViewRow row = grdSchemaDiffs.SelectedRows[0];
            if (!row.Visible)
                return false;

            SchemaComparisonItem item = (SchemaComparisonItem)row.Tag;
            if (item.LeftDdlStatement == null && item.RightDdlStatement == null)
                return false;

            if (!(item.Result == ComparisonResult.ExistsInLeftDB || item.Result == ComparisonResult.ExistsInRightDB ||
                item.Result == ComparisonResult.Deleted))
                return true;
            return false;
        }

        /// <summary>
        /// Returns TRUE if there are any differences
        /// </summary>
        public bool HasDiffs()
        {
            foreach (DataGridViewRow row in grdSchemaDiffs.Rows)
            {
                SchemaComparisonItem item = row.Tag as SchemaComparisonItem;
                if (item != null && (item.Result != ComparisonResult.Same || (item.TableChanges != null && !item.TableChanges.SameTables)))
                    return true;
            } // foreach
            return false;
        }

        /// <summary>
        /// Returns TRUE if there are any data differences
        /// </summary>
        public bool HasDataDiffs()
        {
            foreach (DataGridViewRow row in grdSchemaDiffs.Rows)
            {
                SchemaComparisonItem item = row.Tag as SchemaComparisonItem;
                if (item != null && ((item.TableChanges != null && !item.TableChanges.SameTables)))
                    return true;
            } // foreach
            return false;
        }

        /// <summary>
        /// Opens the compare-dialog in order to compare two DB entities
        /// </summary>
        public void OpenCompareDialog()
        {
            if (grdSchemaDiffs.SelectedRows.Count == 0)
                return;

            DataGridViewRow row = grdSchemaDiffs.SelectedRows[0];
            SchemaComparisonItem item = (SchemaComparisonItem)row.Tag;
            if (item.LeftDdlStatement == null || item.RightDdlStatement == null)
            {
                MessageBox.Show(this,
                    "Can't compare objects because one of the objects exist in only one of the database files.",
                    "Comparison Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return;
            }

            TwoWayCompareEditDialog dlg = new TwoWayCompareEditDialog();
            dlg.Prepare(item, _leftSchema, _rightSchema, _leftdb, _rightdb);
            dlg.SchemaChanged += new EventHandler(dlg_SchemaChanged);
            dlg.ShowDialog(this);
            dlg.SchemaChanged -= new EventHandler(dlg_SchemaChanged);
        }

        public void ExportDataDifferences()
        {
            ExportChangesDialog dlg = new ExportChangesDialog();
            dlg.MultipleChanges = _results[SchemaObject.Table];
            dlg.ShowDialog(this);
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Return the results object
        /// </summary>
        public Dictionary<SchemaObject, List<SchemaComparisonItem>> Results
        {
            get { return _results; }
        }
        #endregion

        #region Event Handlers
        private void dlg_SchemaChanged(object sender, EventArgs e)
        {
            DataGridViewRow row = grdSchemaDiffs.SelectedRows[0];
            SchemaComparisonItem item = (SchemaComparisonItem)row.Tag;
            FormatRow(row);

            bool hasData = false;
            foreach (DataGridViewRow r in grdSchemaDiffs.Rows)
            {
                SchemaComparisonItem i = (SchemaComparisonItem)r.Tag;
                if (i.TableChanges != null)
                {
                    hasData = true;
                    break;
                }
            } // foreach
            lblTableDataIsDifferent.Visible = hasData;
            UpdateChangesCount();
        }

        private void cbxShowOnlyDifferences_CheckedChanged(object sender, EventArgs e)
        {
            ShowMatchingRows();
        }

        private void cbxShowTableDifferences_CheckedChanged(object sender, EventArgs e)
        {
            ShowMatchingRows();
        }

        private void cbxShowIndexDifferences_CheckedChanged(object sender, EventArgs e)
        {
            ShowMatchingRows();
        }

        private void cbxShowViewDifferences_CheckedChanged(object sender, EventArgs e)
        {
            ShowMatchingRows();
        }

        private void cbxShowTriggerDifferences_CheckedChanged(object sender, EventArgs e)
        {
            ShowMatchingRows();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            ShowMatchingRows();
        }

        private void grdSchemaDiffs_SelectionChanged(object sender, EventArgs e)
        {
            if (SelectionChanged != null)
                SelectionChanged(this, EventArgs.Empty);
        }

        private void grdSchemaDiffs_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row = grdSchemaDiffs.Rows[e.RowIndex];
            SchemaComparisonItem item = (SchemaComparisonItem)row.Tag;
            if (item.LeftDdlStatement == null || item.RightDdlStatement == null)
            {
                MessageBox.Show(this,
                    "Can't compare objects because one of the objects exist in only one of the database files.",
                    "Comparison Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return;
            }

            OpenCompareDialog();
        }
        #endregion

        #region Private Methods

        private void UpdateChangesCount()
        {
            int count = 0;
            foreach (SchemaObject objtype in _results.Keys)
            {
                foreach (SchemaComparisonItem item in _results[objtype])
                {
                    if (item.Result != ComparisonResult.Same || (item.TableChanges != null && !item.TableChanges.SameTables))
                        count++;
                } // foreach
            } // foreach

            // Update the number of changes still left                
            lblTotalFound.Text = "" + count + " changes found";
        }

        private void FixView(SchemaComparisonItem item, DataGridViewRow row, bool leftToRight)
        {
            if (leftToRight)
                FixLeftToRight(item, row);
            else
                FixRightToLeft(item, row);
        }

        private void FixRightToLeft(SchemaComparisonItem item, DataGridViewRow row)
        {
            SQLiteDdlStatement orig = item.LeftDdlStatement;

            if (item.RightDdlStatement != null)
                item.LeftDdlStatement = (SQLiteDdlStatement)item.RightDdlStatement.Clone();
            else
                item.LeftDdlStatement = null;

            if (item.Result != ComparisonResult.Same || (item.TableChanges != null && !item.TableChanges.SameTables))
            {
                item.TableChanges = null;
                item.Result = ComparisonResult.Same;
            }

            // Replace the existing schema object with the new one (taken from the right
            // database schema).
            if (orig != null)
            {
                if (orig is SQLiteCreateIndexStatement)
                    _leftSchema[SchemaObject.Index].Remove(orig.ObjectName.ToString().ToLower());
                else if (orig is SQLiteCreateTableStatement)
                    _leftSchema[SchemaObject.Table].Remove(orig.ObjectName.ToString().ToLower());
                else if (orig is SQLiteCreateTriggerStatement)
                    _leftSchema[SchemaObject.Trigger].Remove(orig.ObjectName.ToString().ToLower());
                else if (orig is SQLiteCreateViewStatement)
                    _leftSchema[SchemaObject.View].Remove(orig.ObjectName.ToString().ToLower());
            }
            if (item.LeftDdlStatement != null)
            {
                if (item.LeftDdlStatement is SQLiteCreateIndexStatement)
                {
                    _leftSchema[SchemaObject.Index].Add(item.LeftDdlStatement.ObjectName.ToString().ToLower(),
                        item.RightDdlStatement);
                }
                else if (item.LeftDdlStatement is SQLiteCreateTableStatement)
                {
                    _leftSchema[SchemaObject.Table].Add(item.LeftDdlStatement.ObjectName.ToString().ToLower(),
                        item.LeftDdlStatement);
                }
                else if (item.LeftDdlStatement is SQLiteCreateTriggerStatement)
                {
                    _leftSchema[SchemaObject.Trigger].Add(item.LeftDdlStatement.ObjectName.ToString().ToLower(),
                        item.LeftDdlStatement);
                }
                else if (item.LeftDdlStatement is SQLiteCreateViewStatement)
                {
                    _leftSchema[SchemaObject.View].Add(item.LeftDdlStatement.ObjectName.ToString().ToLower(),
                        item.LeftDdlStatement);
                }
            }

            // Re-format the comparison row
            FormatRow(row);

            // Special treatment is required if the copied entity was a table. This is so
            // because when deleting a table - all of its associated triggers and indexes
            // are deleted as well, and when copying a table - all of its associated 
            // triggers and indexes are replacing the indexes and triggers in the target schema.

            if (item.LeftDdlStatement == null && orig != null && orig is SQLiteCreateTableStatement)
            {
                // The table originally existed in the right database schema but was deleted (the
                // left database don't contain this table nor any of its indexes or triggers).

                SQLiteCreateTableStatement origTable = orig as SQLiteCreateTableStatement;

                // The table was deleted - mark all associated triggers and indexes as deleted
                foreach (DataGridViewRow crow in grdSchemaDiffs.Rows)
                {
                    SchemaComparisonItem sci = (SchemaComparisonItem)crow.Tag;
                    SQLiteCreateIndexStatement index = sci.LeftDdlStatement as SQLiteCreateIndexStatement;
                    SQLiteCreateTriggerStatement trigger = sci.LeftDdlStatement as SQLiteCreateTriggerStatement;
                    if (index != null && index.OnTable.ToLower() == origTable.ObjectName.ToString().ToLower())
                    {
                        // This index should be marked as removed from the schema in the right database
                        sci.Result = ComparisonResult.Same;
                        sci.RightDdlStatement = null;
                        FormatRow(crow);

                        // Remove the index from the right schema
                        _leftSchema[SchemaObject.Index].Remove(index.ObjectName.ToString().ToLower());
                    } // if
                    else if (trigger != null && trigger.TableName.ToString().ToLower() == origTable.ObjectName.ToString().ToLower())
                    {
                        // This trigger should be marked as removed from the schema in the right database
                        sci.Result = ComparisonResult.Same;
                        sci.RightDdlStatement = null;
                        FormatRow(crow);

                        // Remove the trigger from the right schema
                        _leftSchema[SchemaObject.Trigger].Remove(trigger.ObjectName.ToString().ToLower());
                    } // else
                } // foreach
            }
            else if (item.LeftDdlStatement != null && orig != null && orig is SQLiteCreateTableStatement)
            {
                // The table originally existed in the right database schema and was replaced by the table
                // from the left database schema. In this case we need to add all indexes/triggers that
                // belong exclusively to the table from the left database schema and remove all indexes/triggers that
                // belong exclusively to the original table in the right database.

                SQLiteCreateTableStatement origTable = orig as SQLiteCreateTableStatement;
                SQLiteCreateTableStatement updTable = item.LeftDdlStatement as SQLiteCreateTableStatement;

                List<SQLiteCreateIndexStatement> indexesToRemove =
                    ComputeIndexesExclusiveToTable(origTable, _leftSchema, _rightSchema);
                foreach (DataGridViewRow crow in grdSchemaDiffs.Rows)
                {
                    SchemaComparisonItem sci = (SchemaComparisonItem)crow.Tag;
                    SQLiteCreateIndexStatement index = sci.LeftDdlStatement as SQLiteCreateIndexStatement;
                    if (index != null && FindIndexInList(index, indexesToRemove))
                    {
                        // Mark this index as removed.
                        sci.Result = ComparisonResult.Same;
                        sci.LeftDdlStatement = null;
                        FormatRow(crow);

                        // Remove the index from the right database schema
                        _leftSchema[SchemaObject.Index].Remove(index.ObjectName.ToString().ToLower());
                    }

                } // foreach

                List<SQLiteCreateIndexStatement> indexesToAdd =
                    ComputeIndexesExclusiveToTable(updTable, _rightSchema, _leftSchema);
                foreach (DataGridViewRow crow in grdSchemaDiffs.Rows)
                {
                    SchemaComparisonItem sci = (SchemaComparisonItem)crow.Tag;
                    SQLiteCreateIndexStatement index = sci.RightDdlStatement as SQLiteCreateIndexStatement;
                    if (index != null && FindIndexInList(index, indexesToAdd))
                    {
                        // Mark this index as added to the right database
                        sci.Result = ComparisonResult.Same;
                        sci.LeftDdlStatement = (SQLiteDdlStatement)index.Clone();
                        FormatRow(crow);

                        // Add the index to the right database schema
                        SQLiteCreateIndexStatement indexCopy = (SQLiteCreateIndexStatement)sci.LeftDdlStatement;
                        _leftSchema[SchemaObject.Index].Add(indexCopy.ObjectName.ToString().ToLower(), indexCopy);
                    }
                } // foreach

                List<SQLiteCreateTriggerStatement> triggersToRemove =
                    ComputeTriggersExclusiveToTable(origTable, _leftSchema, _rightSchema);
                foreach (DataGridViewRow crow in grdSchemaDiffs.Rows)
                {
                    SchemaComparisonItem sci = (SchemaComparisonItem)crow.Tag;
                    SQLiteCreateTriggerStatement trigger = sci.LeftDdlStatement as SQLiteCreateTriggerStatement;
                    if (trigger != null && FindTriggerInList(trigger, triggersToRemove))
                    {
                        // Mark this trigger as removed from the right database schema
                        sci.Result = ComparisonResult.Same;
                        sci.LeftDdlStatement = null;
                        FormatRow(crow);

                        // Remove the table trigger from the right database schema
                        _leftSchema[SchemaObject.Trigger].Remove(trigger.ObjectName.ToString().ToLower());
                    }

                } // foreach

                List<SQLiteCreateTriggerStatement> triggersToAdd =
                    ComputeTriggersExclusiveToTable(updTable, _rightSchema, _leftSchema);
                foreach (DataGridViewRow crow in grdSchemaDiffs.Rows)
                {
                    SchemaComparisonItem sci = (SchemaComparisonItem)crow.Tag;
                    SQLiteCreateTriggerStatement trigger = sci.RightDdlStatement as SQLiteCreateTriggerStatement;
                    if (trigger != null && FindTriggerInList(trigger, triggersToAdd))
                    {
                        // Mark this trigger as added to the right database schema
                        sci.Result = ComparisonResult.Same;
                        sci.LeftDdlStatement = (SQLiteDdlStatement)trigger.Clone();
                        FormatRow(crow);

                        // Add this trigger to the right database schema
                        SQLiteCreateTriggerStatement triggerCopy = (SQLiteCreateTriggerStatement)sci.LeftDdlStatement;
                        _leftSchema[SchemaObject.Trigger].Add(triggerCopy.ObjectName.ToString().ToLower(), triggerCopy);
                    }
                } // foreach
            } // else
            else if (orig == null && item.LeftDdlStatement != null && item.LeftDdlStatement is SQLiteCreateTableStatement)
            {
                // The table did not exist in the right database, but was added from the left database.
                // In this case - we need to mark that all of the indexes and triggers of the left database
                // were added to the right database.

                SQLiteCreateTableStatement updTable = item.LeftDdlStatement as SQLiteCreateTableStatement;
                foreach (DataGridViewRow crow in grdSchemaDiffs.Rows)
                {
                    SchemaComparisonItem sci = (SchemaComparisonItem)crow.Tag;
                    SQLiteCreateIndexStatement index = sci.RightDdlStatement as SQLiteCreateIndexStatement;
                    SQLiteCreateTriggerStatement trigger = sci.RightDdlStatement as SQLiteCreateTriggerStatement;
                    if (index != null && index.OnTable.ToLower() == updTable.ObjectName.ToString().ToLower())
                    {
                        // This index should be mared as added to the right database
                        sci.Result = ComparisonResult.Same;
                        sci.LeftDdlStatement = (SQLiteDdlStatement)index.Clone();
                        FormatRow(crow);

                        // Add the index to the right database schema
                        SQLiteCreateIndexStatement indexCopy = (SQLiteCreateIndexStatement)sci.LeftDdlStatement;
                        _leftSchema[SchemaObject.Index].Add(indexCopy.ObjectName.ToString().ToLower(), indexCopy);
                    }
                    else if (trigger != null && trigger.TableName.ToString().ToLower() == updTable.ObjectName.ToString().ToLower())
                    {
                        // This trigger should be marked as added to the right database
                        sci.Result = ComparisonResult.Same;
                        sci.LeftDdlStatement = (SQLiteDdlStatement)trigger.Clone();
                        FormatRow(crow);

                        // Add the trigger to the right database schema
                        SQLiteCreateTriggerStatement triggerCopy = (SQLiteCreateTriggerStatement)sci.LeftDdlStatement;
                        _leftSchema[SchemaObject.Trigger].Add(triggerCopy.ObjectName.ToString().ToLower(), triggerCopy);
                    } // else                        
                } // foreach
            } // else

            // Update the number of changes still left                
            UpdateChangesCount();
        }

        private void FixLeftToRight(SchemaComparisonItem item, DataGridViewRow row)
        {
            SQLiteDdlStatement orig = item.RightDdlStatement;

            if (item.LeftDdlStatement != null)
                item.RightDdlStatement = (SQLiteDdlStatement)item.LeftDdlStatement.Clone();
            else
                item.RightDdlStatement = null;

            if (item.Result != ComparisonResult.Same || (item.TableChanges != null && !item.TableChanges.SameTables))
            {
                item.TableChanges = null;
                item.Result = ComparisonResult.Same;
            }

            // Replace the existing schema object with the new one (taken from the left
            // database schema).
            if (orig != null)
            {
                if (orig is SQLiteCreateIndexStatement)
                    _rightSchema[SchemaObject.Index].Remove(orig.ObjectName.ToString().ToLower());
                else if (orig is SQLiteCreateTableStatement)
                    _rightSchema[SchemaObject.Table].Remove(orig.ObjectName.ToString().ToLower());
                else if (orig is SQLiteCreateTriggerStatement)
                    _rightSchema[SchemaObject.Trigger].Remove(orig.ObjectName.ToString().ToLower());
                else if (orig is SQLiteCreateViewStatement)
                    _rightSchema[SchemaObject.View].Remove(orig.ObjectName.ToString().ToLower());
            }
            if (item.RightDdlStatement != null)
            {
                if (item.RightDdlStatement is SQLiteCreateIndexStatement)
                {
                    _rightSchema[SchemaObject.Index].Add(item.RightDdlStatement.ObjectName.ToString().ToLower(),
                        item.RightDdlStatement);
                }
                else if (item.RightDdlStatement is SQLiteCreateTableStatement)
                {
                    _rightSchema[SchemaObject.Table].Add(item.RightDdlStatement.ObjectName.ToString().ToLower(),
                        item.RightDdlStatement);
                }
                else if (item.RightDdlStatement is SQLiteCreateTriggerStatement)
                {
                    _rightSchema[SchemaObject.Trigger].Add(item.RightDdlStatement.ObjectName.ToString().ToLower(),
                        item.RightDdlStatement);
                }
                else if (item.RightDdlStatement is SQLiteCreateViewStatement)
                {
                    _rightSchema[SchemaObject.View].Add(item.RightDdlStatement.ObjectName.ToString().ToLower(),
                        item.RightDdlStatement);
                }
            }

            // Re-format the comparison row
            FormatRow(row);

            // Special treatment is required if the copied entity was a table. This is so
            // because when deleting a table - all of its associated triggers and indexes
            // are deleted as well, and when copying a table - all of its associated 
            // triggers and indexes are replacing the indexes and triggers in the target schema.

            if (item.RightDdlStatement == null && orig != null && orig is SQLiteCreateTableStatement)
            {
                // The table originally existed in the right database schema but was deleted (the
                // left database don't contain this table nor any of its indexes or triggers).

                SQLiteCreateTableStatement origTable = orig as SQLiteCreateTableStatement;

                // The table was deleted - mark all associated triggers and indexes as deleted
                foreach (DataGridViewRow crow in grdSchemaDiffs.Rows)
                {
                    SchemaComparisonItem sci = (SchemaComparisonItem)crow.Tag;
                    SQLiteCreateIndexStatement index = sci.RightDdlStatement as SQLiteCreateIndexStatement;
                    SQLiteCreateTriggerStatement trigger = sci.RightDdlStatement as SQLiteCreateTriggerStatement;
                    if (index != null && index.OnTable.ToLower() == origTable.ObjectName.ToString().ToLower())
                    {
                        // This index should be marked as removed from the schema in the right database
                        sci.Result = ComparisonResult.Same;
                        sci.RightDdlStatement = null;
                        FormatRow(crow);

                        // Remove the index from the right schema
                        _rightSchema[SchemaObject.Index].Remove(index.ObjectName.ToString().ToLower());
                    } // if
                    else if (trigger != null && trigger.TableName.ToString().ToLower() == origTable.ObjectName.ToString().ToLower())
                    {
                        // This trigger should be marked as removed from the schema in the right database
                        sci.Result = ComparisonResult.Same;
                        sci.RightDdlStatement = null;
                        FormatRow(crow);

                        // Remove the trigger from the right schema
                        _rightSchema[SchemaObject.Trigger].Remove(trigger.ObjectName.ToString().ToLower());
                    } // else
                } // foreach
            }
            else if (item.RightDdlStatement != null && orig != null && orig is SQLiteCreateTableStatement)
            {
                // The table originally existed in the right database schema and was replaced by the table
                // from the left database schema. In this case we need to add all indexes/triggers that
                // belong exclusively to the table from the left database schema and remove all indexes/triggers that
                // belong exclusively to the original table in the right database.

                SQLiteCreateTableStatement origTable = orig as SQLiteCreateTableStatement;
                SQLiteCreateTableStatement updTable = item.RightDdlStatement as SQLiteCreateTableStatement;

                List<SQLiteCreateIndexStatement> indexesToRemove =
                    ComputeIndexesExclusiveToTable(origTable, _rightSchema, _leftSchema);
                foreach (DataGridViewRow crow in grdSchemaDiffs.Rows)
                {
                    SchemaComparisonItem sci = (SchemaComparisonItem)crow.Tag;
                    SQLiteCreateIndexStatement index = sci.RightDdlStatement as SQLiteCreateIndexStatement;
                    if (index != null && FindIndexInList(index, indexesToRemove))
                    {
                        // Mark this index as removed.
                        sci.Result = ComparisonResult.Same;
                        sci.RightDdlStatement = null;
                        FormatRow(crow);

                        // Remove the index from the right database schema
                        _rightSchema[SchemaObject.Index].Remove(index.ObjectName.ToString().ToLower());
                    }

                } // foreach

                List<SQLiteCreateIndexStatement> indexesToAdd =
                    ComputeIndexesExclusiveToTable(updTable, _leftSchema, _rightSchema);
                foreach (DataGridViewRow crow in grdSchemaDiffs.Rows)
                {
                    SchemaComparisonItem sci = (SchemaComparisonItem)crow.Tag;
                    SQLiteCreateIndexStatement index = sci.LeftDdlStatement as SQLiteCreateIndexStatement;
                    if (index != null && FindIndexInList(index, indexesToAdd))
                    {
                        // Mark this index as added to the right database
                        sci.Result = ComparisonResult.Same;
                        sci.RightDdlStatement = (SQLiteDdlStatement)index.Clone();
                        FormatRow(crow);

                        // Add the index to the right database schema
                        SQLiteCreateIndexStatement indexCopy = (SQLiteCreateIndexStatement)sci.RightDdlStatement;
                        _rightSchema[SchemaObject.Index].Add(indexCopy.ObjectName.ToString().ToLower(), indexCopy);
                    }
                } // foreach

                List<SQLiteCreateTriggerStatement> triggersToRemove =
                    ComputeTriggersExclusiveToTable(origTable, _rightSchema, _leftSchema);
                foreach (DataGridViewRow crow in grdSchemaDiffs.Rows)
                {
                    SchemaComparisonItem sci = (SchemaComparisonItem)crow.Tag;
                    SQLiteCreateTriggerStatement trigger = sci.RightDdlStatement as SQLiteCreateTriggerStatement;
                    if (trigger != null && FindTriggerInList(trigger, triggersToRemove))
                    {
                        // Mark this trigger as removed from the right database schema
                        sci.Result = ComparisonResult.Same;
                        sci.RightDdlStatement = null;
                        FormatRow(crow);

                        // Remove the table trigger from the right database schema
                        _rightSchema[SchemaObject.Trigger].Remove(trigger.ObjectName.ToString().ToLower());
                    }

                } // foreach

                List<SQLiteCreateTriggerStatement> triggersToAdd =
                    ComputeTriggersExclusiveToTable(updTable, _leftSchema, _rightSchema);
                foreach (DataGridViewRow crow in grdSchemaDiffs.Rows)
                {
                    SchemaComparisonItem sci = (SchemaComparisonItem)crow.Tag;
                    SQLiteCreateTriggerStatement trigger = sci.LeftDdlStatement as SQLiteCreateTriggerStatement;
                    if (trigger != null && FindTriggerInList(trigger, triggersToAdd))
                    {
                        // Mark this trigger as added to the right database schema
                        sci.Result = ComparisonResult.Same;
                        sci.RightDdlStatement = (SQLiteDdlStatement)trigger.Clone();
                        FormatRow(crow);

                        // Add this trigger to the right database schema
                        SQLiteCreateTriggerStatement triggerCopy = (SQLiteCreateTriggerStatement)sci.RightDdlStatement;
                        _rightSchema[SchemaObject.Trigger].Add(triggerCopy.ObjectName.ToString().ToLower(), triggerCopy);
                    }
                } // foreach
            } // else
            else if (orig == null && item.RightDdlStatement != null && item.RightDdlStatement is SQLiteCreateTableStatement)
            {
                // The table did not exist in the right database, but was added from the left database.
                // In this case - we need to mark that all of the indexes and triggers of the left database
                // were added to the right database.

                SQLiteCreateTableStatement updTable = item.RightDdlStatement as SQLiteCreateTableStatement;
                foreach (DataGridViewRow crow in grdSchemaDiffs.Rows)
                {
                    SchemaComparisonItem sci = (SchemaComparisonItem)crow.Tag;
                    SQLiteCreateIndexStatement index = sci.LeftDdlStatement as SQLiteCreateIndexStatement;
                    SQLiteCreateTriggerStatement trigger = sci.LeftDdlStatement as SQLiteCreateTriggerStatement;
                    if (index != null && index.OnTable.ToLower() == updTable.ObjectName.ToString().ToLower())
                    {
                        // This index should be mared as added to the right database
                        sci.Result = ComparisonResult.Same;
                        sci.RightDdlStatement = (SQLiteDdlStatement)index.Clone();
                        FormatRow(crow);

                        // Add the index to the right database schema
                        SQLiteCreateIndexStatement indexCopy = (SQLiteCreateIndexStatement)sci.RightDdlStatement;
                        _rightSchema[SchemaObject.Index].Add(indexCopy.ObjectName.ToString().ToLower(), indexCopy);
                    }
                    else if (trigger != null && trigger.TableName.ToString().ToLower() == updTable.ObjectName.ToString().ToLower())
                    {
                        // This trigger should be marked as added to the right database
                        sci.Result = ComparisonResult.Same;
                        sci.RightDdlStatement = (SQLiteDdlStatement)trigger.Clone();
                        FormatRow(crow);

                        // Add the trigger to the right database schema
                        SQLiteCreateTriggerStatement triggerCopy = (SQLiteCreateTriggerStatement)sci.RightDdlStatement;
                        _rightSchema[SchemaObject.Trigger].Add(triggerCopy.ObjectName.ToString().ToLower(), triggerCopy);
                    } // else                        
                } // foreach
            } // else

            // Update the number of changes still left                
            UpdateChangesCount();
        }

        private List<SQLiteCreateIndexStatement> ComputeIndexesExclusiveToTable(
            SQLiteCreateTableStatement table1, 
            Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> schema1,             
            Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> schema2)
        {
            List<SQLiteCreateIndexStatement> res = new List<SQLiteCreateIndexStatement>();
            foreach (SQLiteCreateIndexStatement index in schema1[SchemaObject.Index].Values)
            {
                if (index.OnTable.ToLower() == table1.ObjectName.ToString().ToLower())
                {
                    bool found = false;
                    foreach (SQLiteCreateIndexStatement index2 in schema2[SchemaObject.Index].Values)
                    {
                        if (index2.OnTable.ToLower() == table1.ObjectName.ToString().ToLower() &&
                            index2.ObjectName.ToString().ToLower() == index.ObjectName.ToString().ToLower())
                        {
                            found = true;
                            break;
                        }
                    } // foreach
                    if (!found)
                        res.Add(index);
                } // if
            } // foreach
            return res;
        }

        private List<SQLiteCreateTriggerStatement> ComputeTriggersExclusiveToTable(
            SQLiteCreateTableStatement table1,
            Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> schema1,
            Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> schema2)
        {
            List<SQLiteCreateTriggerStatement> res = new List<SQLiteCreateTriggerStatement>();
            foreach (SQLiteCreateTriggerStatement trigger in schema1[SchemaObject.Trigger].Values)
            {
                if (trigger.TableName.ToString().ToLower() == table1.ObjectName.ToString().ToLower())
                {
                    bool found = false;
                    foreach (SQLiteCreateTriggerStatement trigger2 in schema2[SchemaObject.Trigger].Values)
                    {
                        if (trigger2.TableName.ToString().ToLower() == table1.ObjectName.ToString().ToLower() &&
                            trigger2.ObjectName.ToString().ToLower() == trigger.ObjectName.ToString().ToLower())
                        {
                            found = true;
                            break;
                        }
                    } // foreach
                    if (!found)
                        res.Add(trigger);
                } // if
            } // foreach
            return res;
        }

        private bool FindIndexInList(SQLiteCreateIndexStatement index, List<SQLiteCreateIndexStatement> ilist)
        {
            foreach (SQLiteCreateIndexStatement ci in ilist)
            {
                if (ci.ObjectName.ToString().ToLower() == index.ObjectName.ToString().ToLower())
                    return true;
            } // foreach
            return false;
        }

        private bool FindTriggerInList(SQLiteCreateTriggerStatement trigger, List<SQLiteCreateTriggerStatement> ilist)
        {
            foreach (SQLiteCreateTriggerStatement ci in ilist)
            {
                if (ci.ObjectName.ToString().ToLower() == trigger.ObjectName.ToString().ToLower())
                    return true;
            } // foreach
            return false;
        }

        private void UpdateView()
        {
            ItemComparer comparer = new ItemComparer();
            grdSchemaDiffs.Rows.Clear();

            List<SchemaComparisonItem> items = _results[SchemaObject.Table];
            items.Sort(comparer);
            AddComparisonItemsToGrid(items);
            items = _results[SchemaObject.Index];
            items.Sort(comparer);
            AddComparisonItemsToGrid(items);
            items = _results[SchemaObject.View];
            items.Sort(comparer);
            AddComparisonItemsToGrid(items);
            items = _results[SchemaObject.Trigger];
            items.Sort(comparer);
            AddComparisonItemsToGrid(items);

            // Clear selection
            grdSchemaDiffs.ClearSelection();

            UpdateChangesCount();
        }

        private void FormatRow(DataGridViewRow row)
        {
            SchemaComparisonItem item = (SchemaComparisonItem)row.Tag;

            row.Cells[2].Style.Font = this.Font;
            row.Cells[3].Style.Font = this.Font;
            row.Cells[2].Style.BackColor = NORMAL_BGCOLOR;
            row.Cells[3].Style.BackColor = NORMAL_BGCOLOR;
            row.Cells[2].Value = item.ObjectName;
            row.Cells[3].Value = item.ObjectName;

            if (item.ErrorMessage != null)
            {
                row.Cells[2].Style.BackColor = COMPARISON_ERROR_COLOR;
                row.Cells[3].Style.BackColor = COMPARISON_ERROR_COLOR;
            }
            else if (item.TableChanges != null && !item.TableChanges.SameTables)
            {
                row.Cells[2].Style.BackColor = DIFFERENT_DATA_COLOR;
                row.Cells[3].Style.BackColor = DIFFERENT_DATA_COLOR;
            }
            else if (item.Result == ComparisonResult.ExistsInLeftDB)
            {
                row.Cells[3].Style.Font = _strikeout;
                row.Cells[3].Style.BackColor = NOT_EXIST_COLOR;
            }
            else if (item.Result == ComparisonResult.ExistsInRightDB)
            {
                row.Cells[2].Style.Font = _strikeout;
                row.Cells[2].Style.BackColor = NOT_EXIST_COLOR;
            }
            else if (item.Result == ComparisonResult.DifferentSchema)
            {
                row.Cells[2].Style.BackColor = DIFFERENT_SCHEMA_COLOR;
                row.Cells[3].Style.BackColor = DIFFERENT_SCHEMA_COLOR;
            }
            else if (item.Result == ComparisonResult.Same)
            {
                if (item.LeftDdlStatement == null)
                {
                    row.Cells[2].Style.Font = _strikeout;
                    row.Cells[2].Style.BackColor = NOT_EXIST_COLOR;
                    row.Cells[3].Style.Font = _strikeout;
                    row.Cells[3].Style.BackColor = NOT_EXIST_COLOR;
                }
            }
        }

        private void AddComparisonItemsToGrid(List<SchemaComparisonItem> items)
        {
            try
            {
                _adding = true;

                foreach (SchemaComparisonItem item in items)
                {
                    SQLiteDdlStatement stmt = item.LeftDdlStatement;
                    if (stmt == null)
                        stmt = item.RightDdlStatement;

                    grdSchemaDiffs.Rows.Add(GetItemImage(item), GetItemType(item), SQLiteParser.Utils.Chop(item.ObjectName),
                        SQLiteParser.Utils.Chop(item.ObjectName));                    
                    DataGridViewRow row = grdSchemaDiffs.Rows[grdSchemaDiffs.Rows.Count - 1];
                    row.Tag = item;

                    FormatRow(row);
                } // foreach
            }
            finally
            {
                _adding = false;
            } // finally
        }

        private void ShowMatchingRows()
        {
            string value = txtSearch.Text.Trim();
            foreach (DataGridViewRow row in grdSchemaDiffs.Rows)
            {
                SchemaComparisonItem item = (SchemaComparisonItem)row.Tag;

                SQLiteDdlStatement stmt = item.LeftDdlStatement;
                if (stmt == null)
                    stmt = item.RightDdlStatement;

                bool match = stmt is SQLiteCreateTableStatement && cbxShowTableDifferences.Checked ||
                    stmt is SQLiteCreateIndexStatement && cbxShowIndexDifferences.Checked ||
                    stmt is SQLiteCreateViewStatement && cbxShowViewDifferences.Checked ||
                    stmt is SQLiteCreateTriggerStatement && cbxShowTriggerDifferences.Checked;
                bool diff = (cbxShowOnlyDifferences.Checked && 
                    (item.Result != ComparisonResult.Same || (item.TableChanges != null && !item.TableChanges.SameTables))) || 
                    !cbxShowOnlyDifferences.Checked;
                match = match && diff;

                string name = (string)row.Cells[2].Value;
                if (value == string.Empty)
                    row.Visible = match;
                row.Visible = name.ToLower().Contains(value.ToLower()) && match;
            } // foreach

            if (SelectionChanged != null)
                SelectionChanged(this, EventArgs.Empty);
        }

        private string GetItemType(SchemaComparisonItem item)
        {
            SQLiteDdlStatement stmt = item.LeftDdlStatement;
            if (stmt == null)
                stmt = item.RightDdlStatement;

            if (stmt is SQLiteCreateTableStatement)
                return "Table";
            else if (stmt is SQLiteCreateIndexStatement)
                return "Index";
            else if (stmt is SQLiteCreateViewStatement)
                return "View";
            else if (stmt is SQLiteCreateTriggerStatement)
                return "Trigger";
            else
                throw new ArgumentException("Illegal item type");
        }

        private Image GetItemImage(SchemaComparisonItem item)
        {
            SQLiteDdlStatement stmt = item.LeftDdlStatement;
            if (stmt == null)
                stmt = item.RightDdlStatement;

            if (stmt is SQLiteCreateTableStatement)
                return imageList1.Images["db_table"];
            else if (stmt is SQLiteCreateIndexStatement)
                return imageList1.Images["db_index"];
            else if (stmt is SQLiteCreateViewStatement)
                return imageList1.Images["db_view"];
            else if (stmt is SQLiteCreateTriggerStatement)
                return imageList1.Images["db_trigger"];
            else
                throw new ArgumentException("illegal item type");
        }
        #endregion

        #region Private Constants
        private Color NORMAL_BGCOLOR = SystemColors.Window;
        private Color NOT_EXIST_COLOR = Color.LightGray;
        private Color DIFFERENT_SCHEMA_COLOR = Color.Khaki;
        private Color DIFFERENT_DATA_COLOR = Color.LightBlue;
        private Color COMPARISON_ERROR_COLOR = Color.LightCoral;
        #endregion

        #region Private Classes
        private class ItemComparer : Comparer<SchemaComparisonItem>
        {
            public override int Compare(SchemaComparisonItem x, SchemaComparisonItem y)
            {
                string xname, yname;

                xname = x.ObjectName;
                yname = y.ObjectName;

                return xname.ToLower().CompareTo(yname.ToLower());
            }
        }
        #endregion

        #region Private Variables
        private bool _adding = false;
        private string _leftdb = null;
        private string _rightdb = null;
        private Font _strikeout;
        private Dictionary<SchemaObject, List<SchemaComparisonItem>> _results;
        private Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> _leftSchema;
        private Dictionary<SchemaObject, Dictionary<string, SQLiteDdlStatement>> _rightSchema;
        #endregion
    }
}
