using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace FastGridApp
{
    /// <summary>
    /// This control provides a grid like control and comes as a replacement
    /// for the standard DataGridView control. It is needed because the standard
    /// DataGridView control has very very bad performance when it needs to
    /// render millions of rows (even in virtual mode) which is a very likely
    /// scenario when working with large database files.
    /// </summary>
    public class FastGrid : Control
    {
        #region Events
        public event EventHandler EditStarted;
        public event RowNeededEventHandler RowNeeded;
        public event EventHandler LayoutChanged;
        public event EventHandler Scroll;
        public event ColumnResizedEventHandler ColumnResized;
        public event EventHandler SelectionChanged;
        public event EventHandler SearchRequested;
        #endregion

        #region Constructor
        public FastGrid()
        {
            this.DoubleBuffered = true;

            _selection.SelectionChanged += new EventHandler(_selection_SelectionChanged);
        }
        #endregion

        #region Public Properties
        public List<FastGridColumn> Columns
        {
            get { return _columns; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public long FirstDisplayedRowIndex
        {
            get { return _firstDisplayedRowIndex; }
            set
            {
                _firstDisplayedRowIndex = value;
                DoLayout();
                Invalidate();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public long RowsCount
        {
            get { return _rowsCount; }
            set
            {
                _rowsCount = value;

                _selection.DelSelection(_rowsCount, long.MaxValue);
                
                DoLayout();
                Invalidate();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int MaxHorizontalScroll
        {
            get { return _header.Width+_colsExtraPixels; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int HorizontalScrollPageSize
        {
            get { return _horizontalPageSize; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int HorizontalScrollPosition
        {
            get { return _startX; }
            set
            {
                int pos = value;
                if (pos > _colsExtraPixels)
                    pos = _colsExtraPixels;
                if (pos < 0)
                    pos = 0;

                _startX = pos;
                Invalidate();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int VerticalPageSize
        {
            get { return _pageSize; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public long FirstSelectedRowIndex
        {
            get
            {
                return _selection.GetSelectedRowId();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FastGridSelection Selection
        {
            get { return (FastGridSelection)_selection.Clone(); }
            set
            {
                _selection.SelectionChanged -= new EventHandler(_selection_SelectionChanged);
                _selection = value;
                _selection.SelectionChanged += new EventHandler(_selection_SelectionChanged);
                Invalidate();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FastGridLocation SelectedCellLocation
        {
            get { return _selectedCellLocation; }
            set
            {
                if (value.ColumnIndex >= _columns.Count)
                {
                    if (_columns.Count > 0)
                        value.ColumnIndex = _columns.Count - 1;
                    else
                        return;
                }
                _selectedCellLocation = value;
                EnsureCellIsShown(_selectedCellLocation);
            }
        }
        #endregion

        #region Public Methods
        public void RefreshLayout()
        {
            DoLayout();
            Invalidate();
        }

        public FastGridLocation PointToLocation(Point pt)
        {
            if (!_rowsRectangle.Contains(pt))
            {
                if (_header.Contains(pt))
                {
                    for (int k = 0; k < _columnRectangles.Count; k++)
                    {
                        Rectangle r = _columnRectangles[k];
                        if (pt.X + _startX >= r.Left - 1 && pt.X + _startX <= r.Right + 1)
                            return new FastGridLocation(-1, k);
                    } // for
                }
                else
                    return FastGridLocation.Empty;
            }
            else
            {
                int row = (int)Math.Floor(1.0*(pt.Y - _rowsRectangle.Y) / _rowHeight);
                if (row >= _rows.Count)
                    row = _rows.Count - 1;
                if (row < 0)
                    return FastGridLocation.Empty;

                for (int k = 0; k <= _cellRectangles.GetUpperBound(1); k++)
                {
                    Rectangle r = _cellRectangles[row, k];
                    if (pt.X+_startX >= r.Left-1 && pt.X+_startX <= r.Right+1)
                        return new FastGridLocation(_firstDisplayedRowIndex+row, k);
                }
            } // else

            return FastGridLocation.Empty;
        }
        #endregion

        #region Protected Overrided Methods

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            Invalidate();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            int increment = _pageSize / 10;
            if (increment <= 0)
                increment = 2;

            // Make movement faster than regular arrow keys
            if (e.Delta < 0)
                _firstDisplayedRowIndex += increment;
            else
                _firstDisplayedRowIndex -= increment;

            if (_firstDisplayedRowIndex > _rowsCount - _pageSize)
                _firstDisplayedRowIndex = _rowsCount - _pageSize;
            if (_firstDisplayedRowIndex < 0)
                _firstDisplayedRowIndex = 0;

            FetchRows();
            Invalidate();

            // Notify that the first displayed row index has changed
            if (Scroll != null)
                Scroll(this, EventArgs.Empty);
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            DoLayout();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (!_valid)
                return;

            Graphics g = e.Graphics;

            if (_needsLayout)
            {
                DoLayout();
                _needsLayout = false;
            }

            g.DrawRectangle(Pens.Black, _border);
            using (SolidBrush b = new SolidBrush(BackColor))
                g.FillRectangle(b, _content);

            if (_columnRectangles.Count == 0)
                return;

            // Compute the indexes of the first visible column and the last visible column
            // These are used to reduce the amount of drawing that will take place by drawing
            // only from the first visible column up to the last visible column.
            int firstColIndex = -1;
            int lastColIndex = -1;
            for (int i = 0; i < _columnRectangles.Count; i++)
            {
                Rectangle hrec = _columnRectangles[i];
                if (firstColIndex == -1 && (hrec.X >= _startX || hrec.Right >= _startX))
                {
                    firstColIndex = i;
                }
                else if (lastColIndex == -1 && firstColIndex != -1 && (hrec.X >= _startX + _content.Width || hrec.Right >= _startX +_content.Width))
                {
                    lastColIndex = i;
                    break;
                }
            } // for
            if (firstColIndex == -1)
                return;
            if (lastColIndex == -1)
                lastColIndex = _columnRectangles.Count - 1;

            g.SetClip(_content, CombineMode.Replace);
            g.TranslateTransform((float)-_startX, 0);            

            // Draw columns
            for (int i = firstColIndex; i <= lastColIndex; i++)
            {
                Rectangle hrect = _columnRectangles[i];
                ControlPaint.DrawButton(g, hrect, ButtonState.Normal);

                // If the column has an associated image - draw it first
                int offset = 0;
                FastGridColumn col = _columns[i];
                if (col.Image != null)
                {
                    offset = 2 + col.Image.Width + 2;
                    g.DrawImage(col.Image, new Rectangle(hrect.X + 2, hrect.Y + (_header.Height - col.Image.Height) / 2,
                         col.Image.Width, col.Image.Height));
                    using (SolidBrush b = new SolidBrush(this.ForeColor))
                        g.DrawString(_columns[i].Text, this.Font, b, new Rectangle(hrect.X + offset, hrect.Y + (_header.Height - _fontHeight) / 2,
                            hrect.Width - offset, (_header.Height - _fontHeight / 2)), _fmt);
                }
                else
                {
                    using (SolidBrush b = new SolidBrush(this.ForeColor))
                        g.DrawString(_columns[i].Text, this.Font, b, new Rectangle(hrect.X + offset, hrect.Y + (_header.Height - _fontHeight) / 2,
                            hrect.Width - offset, (_header.Height - _fontHeight / 2)), _fmt);
                } // else
            } // for

            // Draw the rows cells
            for (int i = 0; i < _rows.Count; i++)
            {
                for (int j = firstColIndex; j <= lastColIndex; j++)
                {
                    Rectangle crect = _cellRectangles[i, j];
                    _rows[i].Cells[j].OnPaint(g, crect, _selection.IsRowSelected(_firstDisplayedRowIndex+i));
                }
            }

            // Draw the rows grid
            for (int i = 0; i < _rows.Count; i++)
            {
                g.DrawLine(Pens.LightGray, _cellRectangles[i, 0].X - 1, _cellRectangles[i, 0].Bottom+1,
                    _cellRectangles[i, _columns.Count - 1].Right + 1, _cellRectangles[i, _columns.Count - 1].Bottom+1);
            }
            if (_rows.Count > 0)
            {
                for (int i = 0; i < _columns.Count; i++)
                {
                    g.DrawLine(Pens.LightGray, _cellRectangles[0, i].Right, _cellRectangles[0, i].Y - 1,
                        _cellRectangles[0, i].Right, _cellRectangles[_rows.Count - 1, i].Bottom + 1);
                }
            }

            // Draw the selected cell boundary
            if (_selectedCellLocation.RowIndex >= _firstDisplayedRowIndex && 
                _selectedCellLocation.RowIndex < _firstDisplayedRowIndex + _rows.Count &&
                _selectedCellLocation.ColumnIndex < _columns.Count)
            {
                Rectangle crect = _cellRectangles[_selectedCellLocation.RowIndex-_firstDisplayedRowIndex, _selectedCellLocation.ColumnIndex];

                Color pcolor = FOCUSED_CELL_BORDER_COLOR;
                if (!Focused)
                    pcolor = UNFOCUSED_CELL_BORDER_COLOR;
                using (Pen p = new Pen(pcolor, 3))
                {
                    p.DashStyle = DashStyle.Dash;
                    g.DrawRectangle(p, crect);
                }
            }

            g.ResetClip();
            g.ResetTransform();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            _mouseLeftPressed = false;

            if (!_valid)
                return;

            // Back to normal mode
            _selectionMode = SelectionMode.None;

            if (_resizeColumnsMode)
            {
                _viewResizeHint = false;
                _resizeColumnsMode = false;
                Cursor = Cursors.Default;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!_valid)
                return;

            if (_resizeColumnsMode)
            {
                // Measure X distance from the location where the resize was started
                int delta = _resizeColumnsPoint.X - e.Location.X;
                if (delta != 0)
                {
                    _resizeColumnsPoint = e.Location;

                    // Apply the difference to the column's width
                    _columns[_resizedColumnIndex].Width -= delta;
                    if (_columns[_resizedColumnIndex].Width < 10)
                        _columns[_resizedColumnIndex].Width = 10;

                    // Notify about this resize
                    if (ColumnResized != null)
                        ColumnResized(this, new ColumnResizedEventArgs(_resizedColumnIndex, _columns[_resizedColumnIndex].Width));

                    DoLayout();

                    Refresh();
                }
            }
            else
            {
                FastGridLocation loc = PointToLocation(e.Location);
                if (loc == FastGridLocation.Empty)
                {
                    if (_viewResizeHint)
                    {
                        _viewResizeHint = false;
                        Cursor = Cursors.Default;
                    }
                    return;
                }

                if (loc.RowIndex < 0)
                {
                    // We need to track the mouse location when it hovers above
                    // column cells in order to be able to change the cursor so that
                    // the user can resize the columns.
                    if ((e.X + _startX - _columnRectangles[loc.ColumnIndex].Left < 5 ||
                        _columnRectangles[loc.ColumnIndex].Right - e.X - _startX < 5))
                    {
                        if (e.X + _startX - _columnRectangles[loc.ColumnIndex].Left < 5)
                            _resizedColumnIndex = loc.ColumnIndex - 1;
                        else
                            _resizedColumnIndex = loc.ColumnIndex;

                        if (_resizedColumnIndex < 0)
                        {
                            _viewResizeHint = false;
                            Cursor = Cursors.Default;
                        }
                        else
                        {
                            _viewResizeHint = true;
                            if (Cursor == Cursors.Default)
                                Cursor = Cursors.VSplit;
                        } // else
                    }
                    else if (_viewResizeHint)
                    {
                        _viewResizeHint = false;
                        Cursor = Cursors.Default;
                    }
                }
                else
                {
                    if (_viewResizeHint)
                    {
                        _viewResizeHint = false;
                        Cursor = Cursors.Default;
                    }

                    if (_mouseLeftPressed && _selectionMode == SelectionMode.None)
                        _selectionMode = SelectionMode.RangeSelection;

                    if (e.Button == MouseButtons.Left && _selectionMode == SelectionMode.RangeSelection)
                    {
                        long rowid = loc.RowIndex;
                        _currentRowId = rowid;

                        _selection.SetSelection(_startRowId, _currentRowId);

                        Invalidate();
                    }
                } // else
            } // else
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (!_valid)
                return;

            this.Focus();

            FastGridLocation loc = PointToLocation(e.Location);
            if (loc == FastGridLocation.Empty)
                return;

            if (_viewResizeHint)
            {
                // Enter resize mode
                _resizeColumnsMode = true;

                // Mark the point where the resize begun
                _resizeColumnsPoint = e.Location;
            }
            else
            {
                if (e.Button == MouseButtons.Left)
                    _mouseLeftPressed = true;

                // Ignore column cells
                if (loc.RowIndex < 0)
                {

                }
                else if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
                {
                    long rowid = loc.RowIndex;
                    switch (_selectionMode)
                    {
                        case SelectionMode.None:
                            _selection.SetSelection(rowid, rowid);
                            _currentRowId = rowid;
                            _startRowId = rowid;
                            break;
                        case SelectionMode.SingleSelection:
                            if (_selection.IsRowSelected(rowid))
                                _selection.DelSelection(rowid, rowid);
                            else
                                _selection.AddSelection(rowid, rowid);
                            _currentRowId = rowid;
                            _startRowId = rowid;
                            break;
                        case SelectionMode.RangeSelection:
                            _selection.SetSelection(_startRowId, rowid);
                            break;
                        default:
                            throw new InvalidOperationException("illegal selection mode encountered");
                    } // switch

                    // Move the selected cell to the clicked cell
                    _selectedCellLocation = loc;

                    Invalidate();
                } // else
            } // else
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (!_valid)
                return;

            // Range/Single selection is turned OFF
            _selectionMode = SelectionMode.None;
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            if (!_valid)
                return;

            e.Handled = true;            
        }

        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (!_valid)
                return;

            if (e.KeyCode == Keys.Tab)
                return;

            e.IsInputKey = true;

            if ((e.Modifiers & Keys.Shift) != 0)
                _selectionMode = SelectionMode.RangeSelection;
            else if ((e.Modifiers & Keys.Control) != 0)
                _selectionMode = SelectionMode.SingleSelection;
            else
                _selectionMode = SelectionMode.None;

            if ((e.Modifiers & Keys.Control) != 0)
            {
                if ((e.Modifiers & Keys.Shift) != 0)
                {
                    if (e.KeyCode == Keys.End)
                    {
                        // Need to create a selection from the current location to the end
                        _selection.SetSelection(_currentRowId, _rowsCount-1);
                        _currentRowId = _rowsCount - 1;

                        // Change location to end of grid
                        _firstDisplayedRowIndex = _rowsCount - _pageSize;
                        if (_firstDisplayedRowIndex < 0)
                            _firstDisplayedRowIndex = 0;
                        FetchRows();
                        Invalidate();

                        // Notify that the first displayed row index has changed
                        if (Scroll != null)
                            Scroll(this, EventArgs.Empty);
                    }
                }
                else
                {
                    if (e.KeyCode == Keys.A)
                    {                        
                        _selection.SetSelection(0, _rowsCount-1);
                        Invalidate();
                    }
                    else if (e.KeyCode == Keys.F)
                    {
                        // Avoid having the selection caused by the search being extended as if the
                        // user pressed SHIFT key while clicking on the found row.
                        _selectionMode = SelectionMode.None;

                        if (SearchRequested != null)
                            SearchRequested(this, EventArgs.Empty);
                    }
                } // else
            }
            else
            {
                if ((e.Modifiers & Keys.Shift) != 0)
                {
                    if (e.KeyCode == Keys.Down)
                    {
                        if (_currentRowId + 1 < _rowsCount)
                        {
                            _currentRowId++;
                            _selection.SetSelection(_startRowId, _currentRowId);

                            if (_currentRowId >= _firstDisplayedRowIndex + _pageSize)
                                _firstDisplayedRowIndex++;
                            FetchRows();
                            _selectedCellLocation.RowIndex = _currentRowId;
                            Invalidate();

                            // Notify that the first displayed row index has changed
                            if (Scroll != null)
                                Scroll(this, EventArgs.Empty);
                        }
                    }
                    else if (e.KeyCode == Keys.Up)
                    {
                        if (_currentRowId - 1 >= 0)
                        {
                            _currentRowId--;
                            _selection.SetSelection(_startRowId, _currentRowId);

                            if (_currentRowId < _firstDisplayedRowIndex)
                                _firstDisplayedRowIndex--;
                            FetchRows();
                            _selectedCellLocation.RowIndex = _currentRowId;
                            Invalidate();

                            // Notify that the first displayed row index has changed
                            if (Scroll != null)
                                Scroll(this, EventArgs.Empty);
                        }
                    }
                    else if (e.KeyCode == Keys.PageDown)
                    {
                        if (_currentRowId + _pageSize >= _rowsCount)
                            _currentRowId = _rowsCount - 1;
                        else
                            _currentRowId += _pageSize;
                        _firstDisplayedRowIndex += _pageSize;
                        if (_firstDisplayedRowIndex > _rowsCount - _pageSize)
                            _firstDisplayedRowIndex = _rowsCount - _pageSize;
                        if (_firstDisplayedRowIndex < 0)
                            _firstDisplayedRowIndex = 0;

                        _selection.SetSelection(_startRowId, _currentRowId);

                        FetchRows();
                        _selectedCellLocation.RowIndex = _currentRowId;
                        Invalidate();

                        // Notify that the first displayed row index has changed
                        if (Scroll != null)
                            Scroll(this, EventArgs.Empty);
                    }
                    else if (e.KeyCode == Keys.PageUp)
                    {
                        if (_currentRowId - _pageSize < 0)
                            _currentRowId = 0;
                        else
                            _currentRowId -= _pageSize;
                        _firstDisplayedRowIndex -= _pageSize;
                        if (_firstDisplayedRowIndex < 0)
                            _firstDisplayedRowIndex = 0;

                        _selection.SetSelection(_startRowId, _currentRowId);

                        FetchRows();
                        _selectedCellLocation.RowIndex = _currentRowId;
                        Invalidate();

                        // Notify that the first displayed row index has changed
                        if (Scroll != null)
                            Scroll(this, EventArgs.Empty);
                    }
                }
                else
                {
                    if (e.KeyCode == Keys.Down)
                    {
                        if (_currentRowId + 1 < _rowsCount)
                        {
                            _currentRowId++;
                            _startRowId = _currentRowId;
                            _selection.SetSelection(_currentRowId, _currentRowId);

                            if (_currentRowId >= _firstDisplayedRowIndex + _pageSize)
                                _firstDisplayedRowIndex++;
                            FetchRows();
                            _selectedCellLocation.RowIndex = _currentRowId;
                            Invalidate();

                            // Notify that the first displayed row index has changed
                            if (Scroll != null)
                                Scroll(this, EventArgs.Empty);
                        }
                    }
                    else if (e.KeyCode == Keys.Up)
                    {
                        if (_currentRowId - 1 >= 0)
                        {
                            _currentRowId--;
                            _startRowId = _currentRowId;
                            _selection.SetSelection(_currentRowId, _currentRowId);

                            if (_currentRowId < _firstDisplayedRowIndex)
                                _firstDisplayedRowIndex--;
                            FetchRows();
                            _selectedCellLocation.RowIndex = _currentRowId;
                            Invalidate();

                            // Notify that the first displayed row index has changed
                            if (Scroll != null)
                                Scroll(this, EventArgs.Empty);
                        }
                    }
                    else if (e.KeyCode == Keys.PageDown)
                    {
                        if (_currentRowId + _pageSize >= _rowsCount)
                            _currentRowId = _rowsCount - 1;
                        else
                            _currentRowId += _pageSize;
                        _startRowId = _currentRowId;
                        _firstDisplayedRowIndex += _pageSize;
                        if (_firstDisplayedRowIndex > _rowsCount-_pageSize)
                            _firstDisplayedRowIndex = _rowsCount - _pageSize;
                        if (_firstDisplayedRowIndex < 0)
                            _firstDisplayedRowIndex = 0;

                        _selection.SetSelection(_currentRowId, _currentRowId);
                        
                        FetchRows();
                        _selectedCellLocation.RowIndex = _currentRowId;
                        Invalidate();

                        // Notify that the first displayed row index has changed
                        if (Scroll != null)
                            Scroll(this, EventArgs.Empty);
                    }
                    else if (e.KeyCode == Keys.PageUp)
                    {
                        if (_currentRowId - _pageSize < 0)
                            _currentRowId = 0;
                        else
                            _currentRowId -= _pageSize;
                        _startRowId = _currentRowId;
                        _firstDisplayedRowIndex -= _pageSize;
                        if (_firstDisplayedRowIndex < 0)
                            _firstDisplayedRowIndex = 0;

                        _selection.SetSelection(_currentRowId, _currentRowId);

                        FetchRows();
                        _selectedCellLocation.RowIndex = _currentRowId;
                        Invalidate();

                        // Notify that the first displayed row index has changed
                        if (Scroll != null)
                            Scroll(this, EventArgs.Empty);
                    }
                    else if (e.KeyCode == Keys.Right)
                    {
                        // Move selected cell one cell right
                        if (_selectedCellLocation.ColumnIndex < _columns.Count - 1)
                        {
                            _selectedCellLocation.ColumnIndex++;
                            EnsureCellIsShown(_selectedCellLocation);
                        }

                        Invalidate();
                    }
                    else if (e.KeyCode == Keys.Left)
                    {
                        // Move the selected cell one cell to the left
                        if (_selectedCellLocation.ColumnIndex > 0)
                        {
                            _selectedCellLocation.ColumnIndex--;
                            EnsureCellIsShown(_selectedCellLocation);
                        }

                        Invalidate();
                    }
                    else if (e.KeyCode == Keys.Enter)
                    {
                        // Issue a EditStarted event
                        if (EditStarted != null)
                            EditStarted(this, EventArgs.Empty);
                    }
                }
            }// else        
        }
        #endregion

        #region Event Handlers
        private void _selection_SelectionChanged(object sender, EventArgs e)
        {
            Invalidate();
            if (SelectionChanged != null)
                SelectionChanged(this, EventArgs.Empty);
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Ensure that the cell as specified in the loc parameter is shown
        /// in the grid.
        /// </summary>
        /// <param name="loc">The location of the cell to show</param>
        private void EnsureCellIsShown(FastGridLocation loc)
        {
            bool needInvalidate = true;
            bool needScroll = false;

            // Handle moving the row so the row is shown
            if (loc.RowIndex < _firstDisplayedRowIndex || loc.RowIndex >= _firstDisplayedRowIndex + _rows.Count)
            {
                if (loc.RowIndex < RowsCount && loc.RowIndex >= 0)
                {
                    _firstDisplayedRowIndex = loc.RowIndex;
                    FetchRows();
                    needScroll = true;
                }
            }

            try
            {
                Rectangle cell = _cellRectangles[loc.RowIndex - _firstDisplayedRowIndex, loc.ColumnIndex];
                if (cell.X >= _content.X + _startX && cell.Right <= _content.Right + _startX)
                {
                    needInvalidate = false;
                    return;
                }
                else
                {
                    // Move to the start of the column
                    if (cell.Left >= _content.Right + _startX)
                    {
                        _startX = cell.Right - _content.Width;
                    }
                    else if (cell.Left < _content.X + _startX)
                    {
                        int width = cell.Width;
                        _startX -= width;
                        if (_startX > cell.Left)
                            _startX = cell.Left;
                    }
                    else if (cell.Right > _content.Right + _startX)
                    {
                        if (cell.Width < _content.Width)
                            _startX += cell.Right - (_content.Right + _startX);
                        else
                            _startX = cell.Left;
                    }

                    if (_startX < 0)
                        _startX = 0;

                    needScroll = true;
                }
            }
            finally
            {
                if (needScroll)
                {
                    // Notify about the scroll
                    if (Scroll != null)
                        Scroll(this, EventArgs.Empty);
                }

                if (needInvalidate)
                    Invalidate();
            } // finally
        }

        private void FetchRows()
        {
            // Fetch rows from provider
            _rows.Clear();
            if (RowNeeded != null)
            {
                for (long i = _firstDisplayedRowIndex; i < _firstDisplayedRowIndex + _pageSize && i < _rowsCount; i++)
                {
                    FastGridRow row = new FastGridRow(this);
                    row.Cells = new FastGridCell[_columns.Count];
                    for (int k = 0; k < _columns.Count; k++)
                    {
                        if (_columns[k].ColumnType == FastGridColumnType.Text)
                            row.Cells[k] = new FastGridCell(this);
                        else if (_columns[k].ColumnType == FastGridColumnType.CheckBox)
                            row.Cells[k] = new FastGridCheckBoxCell(this);
                        else
                            throw new InvalidOperationException();
                    }

                    RowNeededEventArgs ev = new RowNeededEventArgs(i, row);
                    RowNeeded(this, ev);
                    _rows.Add(row);
                } // for
            }
        }

        private void DoLayout()
        {        
            using (Graphics g = CreateGraphics())
            {
                // Compute the maximum height of any image that is placed in the header column(s)
                int imgHeight = 0;
                foreach (FastGridColumn col in _columns)
                {
                    if (col.Image != null)
                    {
                        if (col.Image.Height > imgHeight)
                            imgHeight = col.Image.Height;
                    }
                } // foreach

                _border = new Rectangle(0, 0, Width - 1, Height - 1);
                _content = new Rectangle(1, 1, Width - 2, Height - 2);
                _fontHeight = (int)Math.Ceiling(this.Font.GetHeight(g));

                int colHeight;
                if (imgHeight > _fontHeight)
                    colHeight = imgHeight + 6;
                else
                    colHeight = _fontHeight + 6;

                _header = new Rectangle(_content.X, _content.Y, _content.Width, colHeight);
                _rowsRectangle = new Rectangle(_header.X, _header.Bottom, _header.Width, _content.Height - _header.Height);
                _rowHeight = _header.Height;
                _pageSize = (int)Math.Floor(1.0 * _rowsRectangle.Height / _rowHeight);

                if (_content.Width <= 0 || _pageSize < 0)
                {
                    _valid = false;
                    return;
                }
                else
                    _valid = true;

                // Adjust the index of the first row if a resize event caused the control to expand
                // and more rows can be displayed
                if (_rows.Count < _pageSize)
                {
                    _firstDisplayedRowIndex -= _pageSize - _rows.Count;
                    if (_firstDisplayedRowIndex < 0)
                        _firstDisplayedRowIndex = 0;
                }
                
                // Fetch rows from provider
                FetchRows();

                // Compute column rectangles            
                int xpos = _header.X;
                _columnRectangles.Clear();
                for (int i = 0; i < _columns.Count; i++)
                {
                    Rectangle hrect = new Rectangle(xpos, _header.Y, _columns[i].Width, _header.Height);
                    _columnRectangles.Add(hrect);
                    xpos += hrect.Width;
                } // for

                // Compute how many pixels are missing from the currently shown column headers rectangle
                // so that it can be shown in its entirety.
                _colsExtraPixels = xpos - _header.Width;
                if (_colsExtraPixels < 0)
                    _colsExtraPixels = 0;
                _horizontalPageSize = _header.Width;
                if (_startX > _colsExtraPixels)
                    _startX = _colsExtraPixels;

                // Compute cell rectangles                
                _cellRectangles = new Rectangle[_rows.Count, _columns.Count];
                xpos = _rowsRectangle.X;                
                for (int c = 0; c < _columns.Count; c++)
                {
                    int ypos = _rowsRectangle.Y;
                    for (int i = 0; i < _rows.Count; i++)
                    {
                        Rectangle crect = new Rectangle(xpos+1, ypos+1, _columns[c].Width - 1, _header.Height-1);
                        _cellRectangles[i, c] = crect;
                        ypos += _header.Height;
                    } // for

                    xpos += _columns[c].Width;
                } // for
            } // using

            if (LayoutChanged != null)
                LayoutChanged(this, EventArgs.Empty);
        }
        #endregion

        #region Constants
        private Color FOCUSED_CELL_BORDER_COLOR = Color.Black;
        private Color UNFOCUSED_CELL_BORDER_COLOR = Color.Gray;
        #endregion

        #region Private Variables
        private bool _needsLayout = true;
        private Rectangle _border;
        private Rectangle _content;
        private Rectangle _header;
        private int _rowHeight;
        private int _fontHeight;
        private Rectangle _rowsRectangle;
        private List<Rectangle> _columnRectangles = new List<Rectangle>();
        private Rectangle[,] _cellRectangles;
        private int _pageSize;
        private int _horizontalPageSize;
        private bool _valid;
        private bool _mouseLeftPressed;

        private int _startX;
        private int _colsExtraPixels;
        private long _rowsCount;
        private long _firstDisplayedRowIndex;
        private List<FastGridRow> _rows = new List<FastGridRow>();
        private List<FastGridColumn> _columns = new List<FastGridColumn>();
        private StringFormat _fmt = new StringFormat(StringFormatFlags.NoWrap);

        private bool _viewResizeHint = false;
        private bool _resizeColumnsMode = false;
        private Point _resizeColumnsPoint;
        private int _resizedColumnIndex;

        #region Selection related variables
        private FastGridSelection _selection = new FastGridSelection();
        private long _currentRowId;
        private long _startRowId;
        private SelectionMode _selectionMode = SelectionMode.None;
        private FastGridLocation _selectedCellLocation;
        #endregion

        #endregion
    }

    #region Event Related Types
    public delegate void RowNeededEventHandler(object sender, RowNeededEventArgs e);
    public class RowNeededEventArgs : EventArgs
    {
        public RowNeededEventArgs(long index, FastGridRow row)
        {
            _row = row;
            _index = index;
        }

        public FastGridRow NeededRow
        {
            get { return _row; }
        }

        public long RowIndex
        {
            get { return _index; }
        }

        private FastGridRow _row;
        private long _index;
    }

    public delegate void ColumnResizedEventHandler(object sender, ColumnResizedEventArgs e);
    public class ColumnResizedEventArgs : EventArgs
    {
        public ColumnResizedEventArgs(int columnIndex, int updatedWidth)
        {
            _columnIndex = columnIndex;
            _updatedWidth = updatedWidth;
        }

        public int ColumnIndex
        {
            get { return _columnIndex; }
        }

        public int UpdatedWidth
        {
            get { return _updatedWidth; }
        }

        private int _columnIndex;
        private int _updatedWidth;
    }
    #endregion
}
