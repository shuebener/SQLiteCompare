using System;
using System.Collections.Generic;
using System.Text;

namespace FastGridApp
{
    /// <summary>
    /// This class provides a space-efficient method of storing which
    /// rows are selected in the table diff control. We want to avoid
    /// a case where there are millions of selected rows where each
    /// row index is stored indvidually in the grid control (which is 
    /// the default behavior). This case can cause the software to run
    /// out of memory. So we use a more efficient means of storing
    /// selection information. Basically we store ranges of selection
    /// instead of individual row indexes.
    /// </summary>
    public class FastGridSelection : ICloneable
    {
        #region Events
        public event EventHandler SelectionChanged;
        #endregion

        #region Constructors
        public FastGridSelection()
        {
        }
        #endregion

        #region ICloneable Members

        public object Clone()
        {
            FastGridSelection res = new FastGridSelection();
            foreach (SelectionRange range in _selection)
                res._selection.Add((SelectionRange)range.Clone());
            return res;
        }

        #endregion

        #region Public Properties
        public List<SelectionRange> SelectionRanges
        {
            get { return _selection; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Clear all selection ranges
        /// </summary>
        public void Clear()
        {
            _selection.Clear();
            if (SelectionChanged != null)
                SelectionChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Combines clearing the selection with adding a new selection.
        /// </summary>
        /// <param name="startRowId">The start ROW ID of the selection range to add</param>
        /// <param name="endRowId">The end ROW ID of the selection range to add</param>
        public void SetSelection(long startRowId, long endRowId)
        {
            _selection.Clear();
            AddSelection(startRowId, endRowId);
        }

        /// <summary>
        /// Add a new selection range
        /// </summary>
        /// <param name="startRowId">The start ROW ID of the selection range to add</param>
        /// <param name="endRowId">The end ROW ID of the selection range to add</param>
        public void AddSelection(long startRowId, long endRowId)
        {
            if (startRowId > endRowId)
            {
                long temp = endRowId;
                endRowId = startRowId;
                startRowId = temp;
            }

            for (int i = 0; i < _selection.Count; i++)
            {
                SelectionRange range = _selection[i];
                if (range.Contains(startRowId))
                {
                    if (range.Contains(endRowId))
                        return;

                    // Adjust the current range to expand to the end-row-id
                    range.EndRowId = endRowId;

                    // Merge selection ranges if necessary
                    MergeRanges();

                    if (SelectionChanged != null)
                        SelectionChanged(this, EventArgs.Empty);

                    return;
                }
                else if (range.Contains(endRowId))
                {
                    // This range includes the end row id but not the start row id, so
                    // we need to adjust its start row id to expand to the new start-row-id
                    range.StartRowId = startRowId;

                    // Merge selection ranges if necessary
                    MergeRanges();

                    if (SelectionChanged != null)
                        SelectionChanged(this, EventArgs.Empty);

                    return;
                }
                else if (range.StartRowId > endRowId)
                {
                    _selection.Insert(i, new SelectionRange(startRowId, endRowId));
                    MergeRanges();

                    if (SelectionChanged != null)
                        SelectionChanged(this, EventArgs.Empty);

                    return;
                }
            } // for

            // Add a new selection range because if is not even partialy contained in any
            // of th existing selection ranges
            _selection.Add(new SelectionRange(startRowId, endRowId));

            // Merge ranges if necessary
            MergeRanges();

            if (SelectionChanged != null)
                SelectionChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Deletes the specified selection range
        /// </summary>
        /// <param name="startRowId">The start row id</param>
        /// <param name="endRowId">The end row id of the cleared selection range</param>
        public void DelSelection(long startRowId, long endRowId)
        {
            if (startRowId > endRowId)
            {
                long temp = endRowId;
                endRowId = startRowId;
                startRowId = temp;
            }

            List<SelectionRange> removed = new List<SelectionRange>();
            for(int i=0; i<_selection.Count; i++)
            {
                SelectionRange range = _selection[i];

                if (range.StartRowId > endRowId)
                {
                    // There is no selection range that need to be deleted
                    break;
                }
                else
                {
                    if (range.Contains(startRowId))
                    {
                        if (range.Contains(endRowId))
                        {
                            // The deleted selection range is fully contained in the current
                            // selection range so we can remove it from the range.

                            // Maybe we can remove the entire range ?
                            if (range.StartRowId == startRowId && range.EndRowId == endRowId)
                            {
                                removed.Add(range);
                                break;
                            }
                            else
                            {
                                if (range.StartRowId < startRowId && range.EndRowId > endRowId)
                                {
                                    // We need to split this range into two separate ranges
                                    if (i + 1 < _selection.Count)
                                        _selection.Insert(i + 1, new SelectionRange(endRowId + 1, range.EndRowId));
                                    else
                                        _selection.Add(new SelectionRange(endRowId + 1, range.EndRowId));
                                    range.EndRowId = startRowId - 1;

                                    break;
                                } // if
                                else if (range.StartRowId < startRowId)
                                {
                                    // We need to adjust the current range to exclude the cleared ids
                                    range.EndRowId = startRowId - 1;
                                }
                                else if (range.EndRowId > endRowId)
                                {
                                    // We need to adjust the current range to exclude the cleared ids
                                    range.StartRowId = endRowId + 1;
                                }
                            } // else

                            break;
                        } // if
                        else
                        {
                            // We'll need to adjust the current range to exclude the removed row ids
                            if (startRowId-1 >= range.StartRowId)
                                range.EndRowId = startRowId - 1;
                            else
                                removed.Add(range);

                            // Now move forward and look for more ranges that we may need to remove or
                            // adjust in order to remove the cleared row ids.

                        } // else
                    } // if
                    else
                    {
                        if (range.Contains(endRowId))
                        {
                            if (endRowId >= range.EndRowId)
                            {
                                // In this case we'll remove this range
                                removed.Add(range);
                            }
                            else
                            {
                                // In this case we'll adjust the current range
                                range.StartRowId = endRowId + 1;
                            } // else
                        }
                        else if (startRowId <= range.StartRowId && endRowId >= range.EndRowId)
                        {
                            // In this case the range is contained entirely within the deleted selection
                            // so we need to delete it
                            removed.Add(range);
                        }
                    } // else
                } // else
            } // foreach

            foreach (SelectionRange range in removed)
                _selection.Remove(range);

            if (removed.Count > 0)
            {
                if (SelectionChanged != null)
                    SelectionChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Check if the specified row ID should be selected
        /// </summary>
        /// <param name="rowId">The row ID to check</param>
        /// <returns>TRUE if the row is selected, FALSE if not.</returns>
        public bool IsRowSelected(long rowId)
        {
            foreach (SelectionRange range in _selection)
            {
                if (range.StartRowId > rowId)
                    return false;
                else if (range.Contains(rowId))
                    return true;
            } // foreach

            return false;
        }

        /// <summary>
        /// Returns the first selected row id in the selection
        /// </summary>
        /// <returns>The first selected row id in the selection or -1 if there is none.</returns>
        public long GetSelectedRowId()
        {
            if (_selection.Count > 0)
                return _selection[0].StartRowId;
            else
                return -1;
        }
        #endregion

        #region Private Methods
        private void MergeRanges()
        {
            int removed = 0;
            for (int i = 0; i < _selection.Count-removed; i++)
            {
                SelectionRange prev = _selection[i];
                if (i + 1 < _selection.Count-removed)
                {
                    SelectionRange next = _selection[i + 1];
                    if (prev.Intersects(next))
                    {
                        _selection.Remove(next);
                        prev.EndRowId = next.EndRowId;
                        removed++;
                        i--;
                    }
                }
            } // for

            if (removed > 0)
                MergeRanges();
        }
        #endregion

        #region Private Varaibles
        private List<SelectionRange> _selection = new List<SelectionRange>();
        #endregion
    }

    public class SelectionRange : ICloneable
    {
        public SelectionRange()
        {
        }

        public SelectionRange(long start, long end)
        {
            StartRowId = start;
            EndRowId = end;
        }

        public bool Contains(long point)
        {
            if (point >= StartRowId && point <= EndRowId)
                return true;
            return false;
        }

        public bool Intersects(SelectionRange range)
        {
            if (range.StartRowId >= StartRowId && range.StartRowId <= EndRowId)
                return true;
            if (StartRowId >= range.StartRowId && StartRowId <= range.EndRowId)
                return true;
            if (range.StartRowId == EndRowId + 1)
                return true;
            if (range.EndRowId == StartRowId - 1)
                return true;

            return false;
        }

        #region ICloneable Members

        public object Clone()
        {
            SelectionRange res = new SelectionRange(StartRowId, EndRowId);
            return res;
        }

        #endregion

        public long StartRowId;
        public long EndRowId;
    }

    /// <summary>
    /// Provides the possible selection modes.
    /// </summary>
    public enum SelectionMode
    {
        /// <summary>
        /// No selection mode
        /// </summary>
        None = 0,

        /// <summary>
        /// Single selection mode (e.g., pressing CTRL key and clicking with the mouse on 
        /// a grid row).
        /// </summary>
        SingleSelection = 1,

        /// <summary>
        /// Range selection mode (e.g., pressing SHIFT key and clicking with the mouse on
        /// a grid row).
        /// </summary>
        RangeSelection = 2,
    }
}
