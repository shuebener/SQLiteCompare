using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;

namespace DiffControl
{
    /// <summary>
    /// Provides a special purpose edit box for viewing and editing differences between 
    /// text bodies.
    /// </summary>
    public class DiffEditBox : Control
    {
        #region Events
        public event EventHandler CursorMoved;
        public event EventHandler SelectionChanged;
        public event EventHandler LinesChanged;
        public event EventHandler ScrollNeedsUpdate;
        public event EventHandler SnapshotChanging;
        public event EventHandler SnapshotChanged;
        public event EventHandler UndoRequested;
        public event EventHandler RedoRequested;
        public event EventHandler SaveRequested;
        #endregion

        #region Constructors
        public DiffEditBox()
        {
            DoubleBuffered = true;

            // Cursor will blink every 500 milliseconds
            _cursorTimer.Interval = 500;
            _cursorTimer.Tick += new EventHandler(_cursorTimer_Tick);
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Set the new cursor position
        /// </summary>
        /// <param name="pos">The new position for the cursor</param>
        public void SetCursorPosition(DiffEditPosition pos)
        {
            SetCursorPosition(pos.LineIndex, pos.ColumnIndex);
        }

        /// <summary>
        /// Set a new position to the cursor
        /// </summary>
        /// <param name="lineIndex">The line index where the cursor should be placed</param>
        /// <param name="colIndex">The column index where the cursor should be placed</param>
        public void SetCursorPosition(int lineIndex, int colIndex)
        {
            // Prevent out of boundary conditions
            if (lineIndex >= _lines.Count)
                lineIndex = _lines.Count-1;
            if (lineIndex < 0)
                lineIndex = 0;
            if (colIndex < 0)
                colIndex = 0;

            if (_lines.Count > lineIndex && _lines[lineIndex].Text == null)
            {
                // In case this is a place holder line - set the column index to be 
                // to the beginning of the line.
                colIndex = 0;
            }
            else if (_lines.Count > lineIndex && _lines[lineIndex].Text.Length < colIndex)
            {
                // Adjust cursor index in case it is larger than the maximum number of
                // characters in the specified line
                colIndex = _lines[lineIndex].Text.Length;
            }
            else if (_lines.Count == 0)
                colIndex = 0;

            // Set the adjusted line and column indexes as the next cursor position
            _cursor.LineIndex = lineIndex;
            _cursor.ColumnIndex = colIndex;

            // Restart the cursor blink
            if (Focused)
            {
                _cursorTimer.Stop();
                _cursorTimer.Start();
                _cursorOn = true;
            }

            // Make sure that the new cursor position is visible
            EnsurePositionIsVisible(_cursor);

            // Notify that the cursor has moved
            if (CursorMoved != null)
                CursorMoved(this, EventArgs.Empty);

            Invalidate();
        }

        /// <summary>
        /// Returns the selection range
        /// </summary>
        /// <returns>The current selection range</returns>
        public DiffEditRange GetSelectionRange()
        {
            return _selection;
        }

        /// <summary>
        /// Get the current cursor position
        /// </summary>
        /// <param name="line">The cursor line</param>
        /// <param name="col">The cursor column</param>
        public void GetCursorPosition(out int line, out int col)
        {
            line = _cursor.LineIndex;
            col = _cursor.ColumnIndex;
        }

        /// <summary>
        /// Get the current cursor position
        /// </summary>
        /// <returns>The current cursor position</returns>
        public DiffEditPosition GetCursorPosition()
        {
            return _cursor;
        }

        /// <summary>
        /// Returns the complete range where a change was detected (the pos
        /// parameter can be anywhere within the range).
        /// </summary>
        /// <param name="pos">The position where to look for the change range</param>
        /// <param name="range">If the position is contained in a change range -
        /// return that range</param>
        /// <returns>TRUE means that the pos parameter is contained in a change range.
        /// In this case the range parameter will be initialized with the range data.</returns>
        public bool GetChangeRange(DiffEditPosition pos, ref DiffEditRange range)
        {
            // Deal with out of range line indexes
            if (pos.LineIndex >= _lines.Count)
                return false;

            // Check if the position is withing a line that has no change(s)
            if (_lines[pos.LineIndex].BackColor == Color.Empty)
                return false;

            // Try to spread in both ways (up/down)
            int prev = pos.LineIndex - 1;
            while (prev >= 0 && _lines[prev].BackColor != Color.Empty)
                prev--;
            prev++;
            int next = pos.LineIndex + 1;
            while (next < _lines.Count && _lines[next].BackColor != Color.Empty)
                next++;
            next--;

            if (next >= prev)
            {
                int col = 0;
                if (_lines[next].Text != null)
                    col = _lines[next].Text.Length;
                range.SetRange(new DiffEditPosition(prev, 0), new DiffEditPosition(next, col));
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Replace the lines specified in the lines range by the lines given
        /// in the specified lines list.
        /// </summary>
        /// <param name="from">The index of the line to copy from (including)</param>
        /// <param name="to">The index of the line until which to copy (including)</param>
        /// <param name="diff">The diff edit box from which lines are copied</param>
        public void ReplaceLines(int from, int to, DiffEditBox diff)
        {
            if (SnapshotChanging != null)
                SnapshotChanging(this, EventArgs.Empty);

            // Replace the lines in the from/to range by the corresponding
            // lines in the specified diff edit box.
            List<LineRecord> linesToRemoveFrom = new List<LineRecord>();
            List<LineRecord> linesToRemoveTo = new List<LineRecord>();
            bool renumber = false;
            for (int i = from; i <= to; i++)
            {
                LineRecord lrec = _lines[i];
                if (lrec.Text == null)
                    renumber = true;

                if (diff._lines[i].Text == null)
                {
                    renumber = true;
                    linesToRemoveTo.Add(lrec);
                    linesToRemoveFrom.Add(diff._lines[i]);
                }
                else
                {
                    lrec.Text = diff._lines[i].Text;
                    lrec.Clear();
                    lrec.IsModified = true;
                    diff._lines[i].Clear();
                    diff._lines[i].IsModified = false;
                } // else
            } // for

            // Remove all lines that represent place holder and their matching lines
            foreach (LineRecord lineFrom in linesToRemoveFrom)
                diff._lines.Remove(lineFrom);
            foreach (LineRecord lineTo in linesToRemoveTo)
                _lines.Remove(lineTo);

            // The diff edit box from which the lines were copied need also
            // to be repainted (all change markings were removed).
            diff.Invalidate();

            // Replacing lines may require adjusting the number of columns that can
            // be displayed in the current page (affects the horizontal scroll bar).
            RecalculateMaxDisplayedColumns();

            // We don't want to renumber unless we really have to
            if (renumber)
            {
                RenumberLines(_lines);
                RenumberLines(diff._lines);
            }

            // Invalidate myself
            Invalidate();

            // Notify that lines were modified
            _modified = true;
            if (LinesChanged != null)
                LinesChanged(this, EventArgs.Empty);

            if (SnapshotChanged != null)
                SnapshotChanged(this, EventArgs.Empty);

            // Maybe we need to change the horizontal scroll bar as a result
            // of replacing the text in the specified lines.
            if (ScrollNeedsUpdate != null)
                ScrollNeedsUpdate(this, EventArgs.Empty);
        }

        /// <summary>
        /// Create a snapshot object that stores the current state of the diff edit box
        /// and can be used later to restore its state
        /// </summary>
        /// <returns></returns>
        public DiffSnapshot GetSnapshot()
        {
            DiffSnapshot res = new DiffSnapshot(_lines, _cursor, _selection, _startLine, _startColumn, _modified, _synched);
            return res;
        }

        /// <summary>
        /// Restore the state of the diff edit box from the specified snapshot object
        /// </summary>
        /// <param name="snapshot">The snapshot object that contains the state of the
        /// diff edit box</param>
        public void SetSnapshot(DiffSnapshot snapshot)
        {
            _lines = snapshot.Lines;
            _cursor = snapshot.Cursor;
            _selection = snapshot.Selection;
            _modified = snapshot.IsModified;
            _synched = snapshot.IsSynched;
            StartLine = snapshot.LineIndex;
            StartColumn = snapshot.ColumnIndex;

            if (LinesChanged != null)
                LinesChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Returns the entire text string for the diff edit box
        /// </summary>
        public string GetLinesText()
        {
            DiffEditRange range = GetMaxRange();
            string res = GetTextFromRange(range);
            return res;
        }

        /// <summary>
        /// Reset the DIRTY flag
        /// </summary>
        public void ClearModified()
        {
            _modified = false;
            if (LinesChanged != null)
                LinesChanged(this, EventArgs.Empty);
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Set/Get the list of line records that should be displayed in the control
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<LineRecord> Lines
        {
            get { return _lines; }
            set
            {
                _modified = false;
                _synched = true;

                if (_lines != null && _lines.Count != 0)
                {
                    if (_lines.Count != value.Count)
                        _modified = true;
                    else
                    {
                        for (int i = 0; i < _lines.Count; i++)
                        {
                            LineRecord lrec = _lines[i];
                            LineRecord rrec = value[i];
                            if (lrec.BackColor != rrec.BackColor ||
                                lrec.ForeColor != rrec.ForeColor ||
                                lrec.IsModified != rrec.IsModified ||
                                lrec.LineNumber != rrec.LineNumber ||
                                lrec.Text != rrec.Text)
                            {
                                _modified = true;
                                break;
                            }
                        } // for
                    }
                }

                _lines = value;
                CalcLayout();
                if (_lines.Count > 0)
                {
                    StartLine = 0;
                    StartColumn = 0;
                }

                SetCursorPosition(0, 0);

                if (LinesChanged != null)
                    LinesChanged(this, EventArgs.Empty);

                Invalidate();
            }
        }

        /// <summary>
        /// Check if the text inside the control was modified.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsModified
        {
            get { return _modified; }
        }

        /// <summary>
        /// Checks if the diff control is synched to another diff control or not.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsSynched
        {
            get { return _synched; }
        }

        /// <summary>
        /// Set the start column for displaying the lines. This is used to
        /// scroll the text in the diff edit box horizontally (e.g., when
        /// using the horizontal scroll bar).
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int StartColumn
        {
            get { return _startColumn; }
            set
            {
                // Adjust the location of the start column in order to prevent
                // boundary conditions
                _startColumn = value;
                if (_startColumn < 0)
                    _startColumn = 0;
                else if (_startColumn > _maxDisplayedCols)
                    _startColumn = _maxDisplayedCols;

                // Issue a scroll event (allows listeners to update their GUI,
                // e.g., changing horizontal scroll values).
                if (ScrollNeedsUpdate != null)
                    ScrollNeedsUpdate(this, EventArgs.Empty);

                Invalidate();
            }
        }

        /// <summary>
        /// Set/Get the line that will be displayed first (top-most) in the
        /// diff-edit-box. This is used to perform vertical scrolling of the
        /// text in the control.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int StartLine
        {
            get { return _startLine; }
            set
            {
                // Handle boundary conditions
                if (value >= _lines.Count)
                    return;
                if (value < 0)
                    _startLine = 0;
                else
                    _startLine = value;

                RecalculateMaxDisplayedColumns();

                // Start the cursor if necessary
                if (!_cursorTimer.Enabled && Focused)
                    _cursorTimer.Enabled = true;

                // Notify about a scroll event
                if (ScrollNeedsUpdate != null)
                    ScrollNeedsUpdate(this, EventArgs.Empty);

                Invalidate();
            }
        }

        /// <summary>
        /// Determines the number of columns that appear
        /// in a single page (used to set LargeSize property
        /// of a scroll bar).
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ColumnScrollPageSize
        {
            get { return _colsPerLine; }
        }

        /// <summary>
        /// Determines the maximum number of columns that can be displayed
        /// at the current page.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int MaxDisplayedColumns
        {
            get { return _maxDisplayedCols; }
        }

        /// <summary>
        /// Determines the number of lines that are displayed in a single 
        /// page.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int LineScrollPageSize
        {
            get { return _linesPerPage; }
        }
        #endregion

        #region Protected Overrided Methods
        /// <summary>
        /// Paint the entire diff-edit box
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Draw the line numbers rectangle
            Graphics g = e.Graphics;
            g.FillRectangle(_lineNumbersBrush, _lineNumbersRectangle);

            // Draw the padding rectagle
            g.FillRectangle(_textColumnBrush, _paddingRectangle);

            // Draw the text rectangle
            g.FillRectangle(_textColumnBrush, _textRectangle);

            if (_lines == null)
                return;

            Rectangle rx = new Rectangle();
            for (int i=_startLine; i<_startLine+_linesPerPage && i < _lines.Count; i++)
            {
                LineRecord lrec = _lines[i];

                // Y position is determined by line height
                float ypos = (i-_startLine) * _lineHeight;

                // Draw the line number of the line
                string lnumText = string.Empty;
                if (lrec.LineNumber != -1)
                    lnumText = (lrec.LineNumber+1).ToString();
                g.DrawString(lnumText, this.Font, Brushes.Black,
                    new RectangleF(_lineNumbersRectangle.Left, ypos, _lineNumbersRectangle.Width, _lineHeight), _lineNumbersFormat);

                // In case the line is modified - draw a small colored rectangle to mark this fact
                if (lrec.IsModified)
                {
                    g.FillRectangle(_modifiedBrush, _lineNumbersRectangle.Right, ypos + _textRectangle.Y,
                        _textRectangle.Left - _lineNumbersRectangle.Right-1, _lineHeight);
                }

                // Draw the background color of the line
                using (Brush br = new SolidBrush(lrec.BackColor))
                {
                    g.FillRectangle(br, _textRectangle.X, ypos+_textRectangle.Y,
                        this.Width - _textRectangle.Left-2, _lineHeight);
                }

                // There are two options:
                // 1. A real text line (lrec.Text != null)
                // 2. A place holder line (lrec.Text == null). A place holder line marks
                //    the non-existence of a line for a corresponding line in the second
                //    file. The cannot have text in them and are marked differently.
                if (lrec.Text != null)
                {
                    // Draw the text of the line (starting from the start column)
                    string text = string.Empty;
                    if (lrec.Text.Length > _startColumn)
                        text = lrec.Text.Substring(_startColumn);

                    // Allow the line record to override the default foreground color
                    if (lrec.ForeColor == Color.Empty)
                        lrec.ForeColor = this.ForeColor;

                    g.SetClip(_textRectangle, System.Drawing.Drawing2D.CombineMode.Replace);

                    // Draw the text of the line
                    for (int k = 0; k < text.Length && k < _colsPerLine; k++)
                    {
                        // Compute X position where the text should be written
                        int xpos = (int)(_textRectangle.Left + k * _charWidth + 1);

                        // In case there is a selection - draw a background rectangle and
                        // use a different color for drawing the text.
                        Color foreColor;
                        if (_selection.ContainsPosition(i, _startColumn + k))
                        {
                            g.FillRectangle(_selectionBrush, xpos, ypos + _textRectangle.Y, _charWidth, _lineHeight);
                            foreColor = _selectionForeColor;
                        }
                        else
                        {
                            // There is no selection but maybe the text belongs to a text range
                            // that was defined in the line record.
                            // Note: Text ranges are used to mark intra-lines text differences and
                            //       so they need to be rendered using different background/foreground
                            //       color combinations.
                            TextRange range = lrec.GetRangeForCharIndex(_startColumn + k);
                            if (range != null)
                            {
                                // Allow the range to override the background color
                                Color backColor = range.BackColor;
                                if (backColor == Color.Empty)
                                    backColor = this.BackColor;

                                // Allow the range to override the foreground color
                                foreColor = range.ForeColor;
                                if (foreColor == Color.Empty)
                                    foreColor = this.ForeColor;

                                // Fill the range rectangle accordingly
                                using (Brush rb = new SolidBrush(backColor))
                                    g.FillRectangle(rb, xpos, ypos + _textRectangle.Y, _charWidth, _lineHeight);
                            }
                            else
                                foreColor = lrec.ForeColor;
                        } // else

                        string ch = string.Empty + text[k];

                        // Special handling for the ampersand character (otherwise it won't appear at all)
                        if (ch == "&")
                            ch = "&&";

                        rx.X = xpos;
                        rx.Y = (int)(ypos + _textRectangle.Top);
                        rx.Width = _charWidth;
                        rx.Height = _charHeight;
                        TextRenderer.DrawText(g, ch, this.Font,
                            rx, foreColor, Color.Transparent, TextFormatFlags.NoPadding);
                    } // for

                    g.ResetClip();
                } // for

                // If the cursor rests on this line - draw it in its proper position
                if (_cursor.LineIndex == i && _cursorOn)
                {
                    int index = _cursor.ColumnIndex - _startColumn;
                    g.DrawLine(Pens.Black, _textRectangle.Left + index * _charWidth, ypos,
                        _textRectangle.Left + index * _charWidth, ypos + _lineHeight);
                }
            } // for

            if (_lines.Count == 0)
            {
                // Special treatment when there are no lines in the lines buffer
                if (_cursorOn)
                {
                    int index = _cursor.ColumnIndex - _startColumn;
                    g.DrawLine(Pens.Black, _textRectangle.Left + index * _charWidth, 0,
                        _textRectangle.Left + index * _charWidth, _lineHeight);
                }
            }

            // Draw the border rectangle
            g.DrawRectangle(Pens.Black, _borderRectangle);
        }

        /// <summary>
        /// Enable the cursor when focus is received
        /// </summary>
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            _cursorTimer.Enabled = true;
            _cursorOn = true;
            Invalidate();
        }

        /// <summary>
        /// Disable the cursor when focus is lost
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);

            _cursorTimer.Enabled = false;
            _cursorOn = false;
            Invalidate();
        }

        /// <summary>
        /// Handle resize events by adjusting the start line and column
        /// if necessary
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            int prevColsPerLine = _colsPerLine;
            CalcLayout();
            int nextColsPerLine = _colsPerLine;

            bool needsNotify = false;

            // Adjust the start line if necessary.
            // Note: Always handle adjustments from the end of the lines
            //       towards the beginning.
            if (_startLine + _linesPerPage >= _lines.Count)
                StartLine = _lines.Count - _linesPerPage;
            else
                needsNotify = true;

            // Adjust the start column.
            int deltaColsPerLine = nextColsPerLine - prevColsPerLine;
            if (deltaColsPerLine > 0)
                StartColumn = _startColumn - deltaColsPerLine;
            else
                needsNotify = true;

            if (needsNotify)
            {
                // Notify that the scrollbars needs to be updated
                if (ScrollNeedsUpdate != null)
                    ScrollNeedsUpdate(this, EventArgs.Empty);
            }

            Invalidate();
        }

        /// <summary>
        /// Used to select a single word
        /// </summary>
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            Focus();

            // Determine cursor position based on the clicked point
            DiffEditPosition textpos = GetTextPosition(e.Location);

            DiffEditRange range = new DiffEditRange();
            bool found = FindWordRange(textpos, ref range);
            if (!found)
                SetCursorPosition(textpos);
            else
            {
                // In this case we'll select the word range and move the cursor
                // to the end of the selection
                _selection.SetRange(range.MinPosition, range.MaxPosition);
                SetCursorPosition(range.MaxPosition);

                // Notify
                if (SelectionChanged != null)
                    SelectionChanged(this, EventArgs.Empty);
            } // else
        }

        /// <summary>
        /// Handle mouse down by setting new location for the cursor and handling 
        /// selection.
        /// </summary>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            Focus();

            // Determine cursor position based on the clicked point
            DiffEditPosition textpos = GetTextPosition(e.Location);

            // Set these as the next cursor position
            SetCursorPosition(textpos);

            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Right)
            {
                // If a shortcut menu was set in the control - we'll open it now.
                if (this.ContextMenu != null)
                {
                    this.ContextMenu.Show(this, e.Location);
                }
            }
            else if (e.Button == MouseButtons.Left)
            {
                // If the user is in the middle of an active selection (e.g., holding the
                // SHIFT button while pressing arrow keys) - adjust the selection range
                // according to the new cursor position.
                if (_activeSelection)
                {
                    _selection.SetRange(_selectionBeginPosition, textpos);

                    // Notify
                    if (SelectionChanged != null)
                        SelectionChanged(this, EventArgs.Empty);

                    Invalidate();
                }
                else
                {
                    // Save the click position for later calculations
                    _selectionBeginPosition = textpos;

                    // Set the clicked point as the start of the new selection
                    _selection.StartPosition = textpos;
                    _selection.EndPosition = textpos;

                    // Mark that mouse selection is now active (will affect the handling of the OnMouseMove
                    // event).
                    _mouseSelectionIsActive = true;

                    // Notify
                    if (SelectionChanged != null)
                        SelectionChanged(this, EventArgs.Empty);
                } // else
            } // else
        }

        /// <summary>
        /// Handle mouse move by changing cursor position and adjusting the active
        /// selection range
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!_mouseSelectionIsActive)
                return;

            // Determine cursor position based on the dragged point
            DiffEditPosition textpos = GetTextPosition(e.Location);

            // Set these as the next cursor position
            SetCursorPosition(textpos.LineIndex, textpos.ColumnIndex);

            // Set the selection range
            _selection.SetRange(_selectionBeginPosition, textpos);

            // Notify
            if (SelectionChanged != null)
                SelectionChanged(this, EventArgs.Empty);

            Invalidate();
        }

        /// <summary>
        /// Exit selection mode
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            _mouseSelectionIsActive = false;
        }

        /// <summary>
        /// Handle mouse wheel movements
        /// </summary>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            DiffEditPosition curr = GetCursorPosition();

            if (e.Delta < 0)
                StartLine += 1;
            else
                StartLine -= 1;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            // Prevent CONTROL/ALT keys from being passed to the OnKeyPress method
            if (e.Control || e.Alt || e.KeyCode == Keys.Escape)
                e.SuppressKeyPress = true;
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            e.Handled = true;
            if (e.KeyChar == '\b' || e.KeyChar == '\r')
                return;

            string inserted = string.Empty + e.KeyChar;
            HandleTextInsert(inserted);
        }

        /// <summary>
        /// Handle control buttons
        /// </summary>
        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (e.KeyCode == Keys.Escape)
                return;

            e.IsInputKey = true;

            int line;
            int col;
            GetCursorPosition(out line, out col);

            if (e.KeyCode == Keys.Right)
                SetCursorPosition(line, col + 1);
            else if (e.KeyCode == Keys.Left)
                SetCursorPosition(line, col - 1);
            else if (e.KeyCode == Keys.Up)
                SetCursorPosition(line - 1, col);
            else if (e.KeyCode == Keys.Down)
                SetCursorPosition(line + 1, col);
            else if (e.KeyCode == Keys.End)
                SetCursorPosition(line, _lines[line].Text != null ? (_lines[line].Text.Length + 1) : 0);
            else if (e.KeyCode == Keys.Home)
                SetCursorPosition(line, 0);
            else if (e.KeyCode == Keys.PageUp)
                SetCursorPosition(line - _linesPerPage, col);
            else if (e.KeyCode == Keys.PageDown)
                SetCursorPosition(line + _linesPerPage, col);
            else if (e.KeyCode == Keys.Back)
            {
                HandleBackPressed(line, col);
                return;
            } // else
            else if (e.KeyCode == Keys.Enter)
            {
                HandleEnterPressed(line, col);
                return;
            }
            else if (e.KeyCode == Keys.C && e.Control)
            {
                HandleCopyToClipboard();
                return;
            }
            else if (e.KeyCode == Keys.S && e.Control)
            {
                HandleSave();
                return;
            }
            else if (e.KeyCode == Keys.V && e.Control)
            {
                HandlePasteFromClipboard();
                return;
            }
            else if (e.KeyCode == Keys.X && e.Control)
            {
                HandleCutToClipboard();
                return;
            }
            else if (e.KeyCode == Keys.A && e.Control)
            {
                HandleSelectAll();
                return;
            }
            else if (e.Control && (e.KeyCode == Keys.Z || e.KeyCode == Keys.Y))
            {
                if (e.KeyCode == Keys.Z)
                {
                    if (UndoRequested != null)
                        UndoRequested(this, EventArgs.Empty);
                }
                else
                {
                    if (RedoRequested != null)
                        RedoRequested(this, EventArgs.Empty);
                }
                return;
            }

            int afterLine;
            int afterCol;
            GetCursorPosition(out afterLine, out afterCol);

            switch (e.KeyCode)
            {
                case Keys.Right:
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                case Keys.End:
                case Keys.Home:
                case Keys.PageUp:
                case Keys.PageDown:
                    if ((e.Modifiers & Keys.Shift) != 0)
                    {
                        _selection.StartPosition = _selectionBeginPosition;
                        _selection.EndPosition = new DiffEditPosition(afterLine, afterCol);
                    }
                    else
                    {
                        // Clear the current selection
                        _selection.StartPosition = new DiffEditPosition(afterLine, afterCol);
                        _selection.EndPosition = new DiffEditPosition(afterLine, afterCol);
                    } // else                    

                    // Notify
                    if (SelectionChanged != null)
                        SelectionChanged(this, EventArgs.Empty);

                    Invalidate();
                    break;
            } // switch

            if (e.KeyCode == Keys.ShiftKey && !_activeSelection)
            {
                // Remember that the shift button is pressed, and also remember the
                // location of the cursor during the press
                _activeSelection = true;

                if (_selection.IsEmpty)
                    _selectionBeginPosition = GetCursorPosition();
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (e.KeyCode == Keys.ShiftKey)
            {
                _activeSelection = false;
            }
        }
        #endregion

        #region Event Handlers
        private void _cursorTimer_Tick(object sender, EventArgs e)
        {
            _cursorOn = !_cursorOn;
            Invalidate();
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Find a word range from the specified text position.
        /// </summary>
        /// <param name="textpos">The text position to begin the search for a word range</param>
        /// <param name="range">The range that was found</param>
        /// <returns>TRUE if a range was found, FALSE otherwise</returns>
        private bool FindWordRange(DiffEditPosition textpos, ref DiffEditRange range)
        {
            if (_lines.Count == 0)
                return false;
            string text = _lines[textpos.LineIndex].Text;
            if (text == null)
                return false;
            if (textpos.ColumnIndex >= text.Length)
                return false;

            int right = textpos.ColumnIndex;
            while (right < text.Length && Char.IsLetterOrDigit(text[right]))
                right++;

            int left = textpos.ColumnIndex - 1;
            while (left >= 0 && Char.IsLetterOrDigit(text[left]))
                left--;

            if (left < right)
            {
                range.SetRange(new DiffEditPosition(textpos.LineIndex, left+1), new DiffEditPosition(textpos.LineIndex, right));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Select all characters that appear in the diff box
        /// </summary>
        private void HandleSelectAll()
        {
            if (_lines.Count == 0)
                return;

            LineRecord last = _lines[_lines.Count - 1];
            if (last.Text == null || last.Text.Length == 0)
                _selection.SetRange(new DiffEditPosition(0, 0), new DiffEditPosition(_lines.Count - 1, 0));
            else
                _selection.SetRange(new DiffEditPosition(0, 0), new DiffEditPosition(_lines.Count - 1, _lines[_lines.Count - 1].Text.Length));

            // Notify
            if (SelectionChanged != null)
                SelectionChanged(this, EventArgs.Empty);

            Invalidate();
        }

        /// <summary>
        /// Returns the maximal text range in the diff edit box
        /// </summary>
        /// <returns></returns>
        private DiffEditRange GetMaxRange()
        {
            DiffEditRange range = new DiffEditRange();
            if (_lines.Count == 0)
            {
                range.SetRange(new DiffEditPosition(0, 0), new DiffEditPosition(0, 0));
                return range;
            }

            LineRecord last = _lines[_lines.Count - 1];
            if (last.Text == null || last.Text.Length == 0)
                range.SetRange(new DiffEditPosition(0, 0), new DiffEditPosition(_lines.Count - 1, 0));
            else
                range.SetRange(new DiffEditPosition(0, 0), new DiffEditPosition(_lines.Count - 1, _lines[_lines.Count - 1].Text.Length));

            return range;
        }

        /// <summary>
        /// Notifies externals listeners that a SAVE was requested.
        /// </summary>
        private void HandleSave()
        {
            Timer tm = new Timer();
            tm.Interval = 200;
            tm.Tick += HandleSaveTimer;
            tm.Start();
        }

        private void HandleSaveTimer(object sender, EventArgs e)
        {
            Timer tm = (Timer)sender;
            tm.Stop();
            tm.Tick -= HandleSaveTimer;            
            tm.Dispose();

            if (SaveRequested != null)
                SaveRequested(this, EventArgs.Empty);
        }

        /// <summary>
        /// Copies all characters in the current selection to the clipboard
        /// </summary>
        private void HandleCopyToClipboard()
        {
            string str = GetTextFromRange(_selection);
            Clipboard.SetData(DataFormats.StringFormat, str);
        }

        /// <summary>
        /// Cuts all characters in the current selection to the clipboard
        /// </summary>
        private void HandleCutToClipboard()
        {
            string str = GetTextFromRange(_selection);

            // Place this string in the clipboard
            Clipboard.SetData(DataFormats.StringFormat, str);

            // Delete all characters in the selection
            HandleBackPressed(_selection.MaxPosition.LineIndex, _selection.MaxPosition.ColumnIndex);
        }

        /// <summary>
        /// Copies all characters from the clipboard and into the control lines buffer
        /// </summary>
        private void HandlePasteFromClipboard()
        {
            string str = (string)Clipboard.GetData(DataFormats.StringFormat);
            HandleTextInsert(str);
        }

        /// <summary>
        /// Gathers the text string that reside in the specified range
        /// </summary>
        /// <param name="range">The text range</param>
        /// <returns>The text that resides in the specified range</returns>
        private string GetTextFromRange(DiffEditRange range)
        {
            if (range.MinPosition.LineIndex == range.MaxPosition.LineIndex)
            {
                string text = _lines[range.MinPosition.LineIndex].Text;
                if (text == null)
                    return string.Empty;
                else
                    return text.Substring(range.MinPosition.ColumnIndex, range.MaxPosition.ColumnIndex - range.MinPosition.ColumnIndex);
            }
            else
            {
                StringBuilder sb = new StringBuilder();

                string first = _lines[range.MinPosition.LineIndex].Text;
                if (first != null)
                {
                    if (first.Length > 0)
                    {
                        if (range.MinPosition.ColumnIndex < first.Length)
                            first = first.Remove(0, range.MinPosition.ColumnIndex);
                    }
                    sb.Append(first + "\r\n");
                }

                string last = _lines[range.MaxPosition.LineIndex].Text;
                if (last == null)
                    last = string.Empty;
                if (last.Length > 0)
                {
                    if (range.MaxPosition.ColumnIndex < last.Length)
                        last = last.Remove(range.MaxPosition.ColumnIndex);
                }

                for (int i = range.MinPosition.LineIndex+1; i < range.MaxPosition.LineIndex; i++)
                {
                    if (_lines[i].Text != null)
                        sb.Append(_lines[i].Text + "\r\n");
                } // for

                sb.Append(last);

                return sb.ToString();
            } // else
        }

        /// <summary>
        /// Handle the insertion of the specified text to the lines buffer
        /// </summary>
        /// <param name="inserted">The text to insert</param>
        private void HandleTextInsert(string inserted)
        {
            int line;
            int col;
            bool renumber = false;

            // Do nothing if there is nothing to insert.
            if (inserted == null || inserted.Length == 0)
                return;

            // Notify that the snapshot is in the process of changing
            // Note: in this point it is still possible to take a snapshot
            //       of this diff edit box BEFORE it has changed.
            if (SnapshotChanging != null)
                SnapshotChanging(this, EventArgs.Empty);

            // Expand TAB characters to spaces
            inserted = inserted.Replace("\t", _tabSpaces);

            // Split the specified string along newline boundaries
            string[] lines = _nlrx.Split(inserted);

            // Ignore empty line at the end (if exists)
            if (lines.Length > 1 && lines[lines.Length - 1] == string.Empty)
            {
                string[] tmp = new string[lines.Length - 1];
                for (int i = 0; i < lines.Length - 1; i++)
                    tmp[i] = lines[i];
                lines = tmp;
            }

            // Get the current cursor position
            GetCursorPosition(out line, out col);

            // In case there is a selection when the insert is performed - we'll first
            // delete it and then handle the insert as usuall
            if (!_selection.IsEmpty)
            {
                if (_selection.StartPosition.LineIndex != _selection.EndPosition.LineIndex)
                    renumber = true;

                HandleBackPressed(line, col);

                // Get the current cursor position
                GetCursorPosition(out line, out col);
            }

            // Handle the special case when the lines buffer is empty
            if (_lines.Count == 0)
            {
                // In case there are no lines in the lines buffer - add an empty one
                LineRecord lrec = new LineRecord();
                lrec.Text = string.Empty;
                lrec.LineNumber = 0;
                _lines.Add(lrec);
            }

            if (lines.Length == 1)
            {
                // Insert the text into the first line
                LineRecord first = _lines[line];
                if (first.Text != null)
                    first.Text = first.Text.Insert(col, lines[0]);
                else
                {
                    // This is a place holder line
                    first.Text = lines[0];
                    renumber = true;
                } 
                first.Clear();
                first.IsModified = true;

                if (renumber)
                    RenumberLines(_lines);

                RecalculateMaxDisplayedColumns();
                SetCursorPosition(line, col + lines[0].Length);
            }
            else
            {
                // Insert multi-line text

                int prevCount = _lines.Count;

                // The 'end' string will be appended to the last line
                string end = string.Empty;
                LineRecord first = _lines[line];
                if (first.Text != null)
                {
                    // Insert operations cause the diff control to get out of synch with its peer diff control
                    _synched = false;

                    if (first.Text.Length > 0)
                    {
                        if (col < first.Text.Length)
                        {
                            end = first.Text.Remove(0, col);
                            first.Text = first.Text.Remove(col) + lines[0];
                        }
                        else if (col == first.Text.Length)
                        {
                            first.Text += lines[0];
                        } // else
                    }
                    else
                    {
                        first.Text = lines[0];
                        first.Clear();
                        first.IsModified = true;
                    }
                }
                else
                {
                    first.Text = lines[0];
                    first.Clear();
                    first.IsModified = true;
                }

                // Add the rest of the lines 
                for (int i = 1; i < lines.Length; i++)
                {
                    LineRecord nl = new LineRecord();
                    nl.Text = lines[i];
                    nl.IsModified = true;
                    _lines.Insert(line + i, nl);
                } // for

                // Merge the last line record with the text from the last line.
                LineRecord last;
                if (line + lines.Length - 1 < _lines.Count)
                    last = _lines[line + lines.Length - 1];
                else
                {
                    last = new LineRecord();
                    last.Text = lines[lines.Length - 1];
                    last.IsModified = true;
                    _lines.Insert(line + lines.Length - 1, last);
                } // else
                if (last.Text.Length > 0)
                    last.Text += end;
                else
                    last.Text = lines[lines.Length - 1];
                last.IsModified = true;

                int afterCount = _lines.Count;

                if (prevCount.ToString().Length < afterCount.ToString().Length)
                    CalcLayout();

                RenumberLines(_lines);
                RecalculateMaxDisplayedColumns();
                SetCursorPosition(line + lines.Length - 1, lines[lines.Length - 1].Length);
            }

            // Mark that a modification has occured
            _modified = true;
            if (LinesChanged != null)
                LinesChanged(this, EventArgs.Empty);

            // Notify that the snapshot has changed
            if (SnapshotChanged != null)
                SnapshotChanged(this, EventArgs.Empty);
            
            Invalidate();
        }

        /// <summary>
        /// Handle ENTER keypress events
        /// </summary>
        /// <param name="line">The line where the ENTER was pressed</param>
        /// <param name="col">The column where the ENTER was pressed</param>
        private void HandleEnterPressed(int line, int col)
        {
            // Notify that the snapshot is in the process of changing
            if (SnapshotChanging != null)
                SnapshotChanging(this, EventArgs.Empty);

            // In case there is a selection when the ENTER is pressed - we'll first
            // delete it and then handle the press as usuall.
            if (!_selection.IsEmpty)
            {
                HandleBackPressed(line, col);
                GetCursorPosition(out line, out col);
            }

            if (col == 0)
            {
                // When the user clicks ENTER in the beginning of the line there 
                if (_lines[line].Text == null)
                {
                    // This is a placeholder line so we'll update its Text property
                    // and mark it as modified
                    _lines[line].Text = string.Empty;
                    _lines[line].Clear();
                    _lines[line].IsModified = true;
                }
                else
                {
                    // ENTER operations cause the diff control to get out of synch with its peer diff control
                    _synched = false;

                    // In this case we'll add a new line record with an empty text
                    LineRecord nl = new LineRecord();
                    nl.Text = string.Empty;
                    nl.IsModified = true;
                    _lines.Insert(line, nl);
                }
            }
            else
            {
                // ENTER operations cause the diff control to get out of synch with its peer diff control
                _synched = false;

                // There are two options:
                // 1. The ENTER was clicked in the middle of the line. In this case
                //    we'll create a new line and split the line text between the existing
                //    line and the newly created line.
                // 2. The ENTER was clicked in the end of the line. In this case we'll
                //    create a new empty line and that's it.
                LineRecord lrec = _lines[line];
                LineRecord nl = new LineRecord();
                if (lrec.Text == null || lrec.Text.Length == 0 || col == lrec.Text.Length)
                {
                    // In this case we'll create a new line and leave the original line intact
                    nl.Text = string.Empty;
                    nl.IsModified = true;
                }
                else                    
                {
                    // In this case we'll create a new line and split the contents of the original
                    // line with this one.
                    nl.Text = lrec.Text.Remove(0, col);
                    nl.IsModified = true;
                    lrec.Text = lrec.Text.Remove(col);
                    lrec.IsModified = true;
                    lrec.Clear();
                } // else

                // Add the new line to the lines list
                if (line == _lines.Count - 1)
                    _lines.Add(nl);
                else
                    _lines.Insert(line + 1, nl);
            } // else

            RenumberLines(_lines);
            SetCursorPosition(line + 1, 0);

            // Mark that a modification has occured
            _modified = true;
            if (LinesChanged != null)
                LinesChanged(this, EventArgs.Empty);

            // Notify that the snapshot has changed
            if (SnapshotChanged != null)
                SnapshotChanged(this, EventArgs.Empty);

            // Maybe we need to adjust the size of the numbers column on the left.
            CalcLayout();

            Invalidate();
        }

        /// <summary>
        /// Handle BACKSPACE keypress events
        /// </summary>
        /// <param name="line">The line where the BACKSPACE was clicked</param>
        /// <param name="col">The column where the BACKSPACE was clicked</param>
        private void HandleBackPressed(int line, int col)
        {
            // Notify that the snapshot is in the process of changing
            if (SnapshotChanging != null)
                SnapshotChanging(this, EventArgs.Empty);

            try
            {
                if (_selection.IsEmpty)
                {
                    // If selection is empty - the default behavior is to delete the previous character
                    // if there is any
                    LineRecord lrec = _lines[line];
                    if (col > 0)
                    {
                        // In this case we can delete the previous character and adjust the
                        // cursor position to point one character to the left
                        string text = lrec.Text.Remove(col - 1, 1);
                        lrec.Text = text;
                        lrec.Clear();
                        lrec.IsModified = true;
                        RecalculateMaxDisplayedColumns();
                        SetCursorPosition(line, col - 1);
                        Invalidate();
                    } // if
                    else
                    {
                        // In this case the current line should be appended to the previous line
                        if (line > 0)
                        {
                            // BACKSPACE operations cause the diff control to get out of synch with its peer diff control
                            _synched = false;

                            LineRecord prec = _lines[line - 1];
                            if (prec.Text == null || prec.Text.Length == 0)
                            {
                                // The previous line is either a place holder line or it is empty
                                // In either case we should simply remove it from the lines list
                                // and renumber
                                _lines.RemoveAt(line - 1);
                                lrec.Clear();
                                lrec.IsModified = true;
                                RecalculateMaxDisplayedColumns();
                                RenumberLines(_lines);
                                SetCursorPosition(new DiffEditPosition(line - 1, 0));
                                Invalidate();
                            }
                            else
                            {
                                // The pervious line is non-empty. In this case we need to append
                                // the contents of the current line into the previous line and remove
                                // the current line.
                                _lines.RemoveAt(line);
                                prec.Clear();
                                int last = prec.Text.Length;
                                if (lrec.Text != null && lrec.Text.Length > 0)
                                {
                                    prec.Text += lrec.Text;
                                    prec.IsModified = true;
                                }
                                RecalculateMaxDisplayedColumns();
                                RenumberLines(_lines);
                                SetCursorPosition(new DiffEditPosition(line - 1, last));
                                Invalidate();
                            } // else
                        }
                        else
                        {
                            // This is the first line and it is empty so there is nothing to 
                            // delete in this case
                            return;
                        } // else
                    } // else
                } // if
                else
                {
                    // These flags are used to determine if all lines in the range were removed.
                    // We use this information in order to determine the appropriate position
                    // for the cursor after the removal is completed.
                    bool remfirst = false;
                    bool remlast = false;

                    // Selection is not empty so all the characters in its range should be 
                    // deleted.
                    DiffEditPosition min = _selection.MinPosition;
                    DiffEditPosition max = _selection.MaxPosition;

                    LineRecord minline = _lines[min.LineIndex];
                    LineRecord maxline = _lines[max.LineIndex];
                    if (min.LineIndex == max.LineIndex)
                    {
                        // Note: selection is not empty so this line can't be a place holder
                        //       thus the Text property of this line record cannot be NULL.

                        // If the entire selection rests in a single line and is contained fully
                        // in the line - we can simply adjust the line record
                        minline.Text = minline.Text.Remove(min.ColumnIndex, max.ColumnIndex - min.ColumnIndex);
                        minline.Clear();
                        minline.IsModified = true;
                    }
                    else
                    {
                        // This selection contains multiple lines

                        // Determine which lines can be removed entirely
                        List<LineRecord> removed = new List<LineRecord>();

                        // If the selection starts at the beginning of the first
                        // selected line - that line will be removed
                        if (min.ColumnIndex == 0)
                        {
                            removed.Add(minline);
                            remfirst = true;

                            if (max.ColumnIndex > 0)
                            {
                                if (maxline.Text != null && max.ColumnIndex == maxline.Text.Length)
                                {
                                    removed.Add(maxline);
                                    remlast = true;
                                }
                                else if (maxline.Text != null && max.ColumnIndex < maxline.Text.Length)
                                    maxline.Text = maxline.Text.Remove(0, max.ColumnIndex);
                            }
                            else
                            {
                                // The last selected line will not be affected or merged
                            } // else
                        }
                        else
                        {
                            if (minline.Text != null && minline.Text.Length > 0 && min.ColumnIndex < minline.Text.Length)
                                minline.Text = minline.Text.Remove(min.ColumnIndex);

                            // In this case we'll remove the last line and merge its contents into 
                            // the first line
                            removed.Add(maxline);
                            remlast = true;

                            // .. But make sure there is something to merge 
                            if (maxline.Text != null && max.ColumnIndex < maxline.Text.Length)
                                minline.Text = minline.Text + maxline.Text.Remove(0, max.ColumnIndex);

                            minline.Clear();
                            minline.IsModified = true;
                        } // else

                        // Every line between (not including) the minimum selection line and
                        // the maximum selection line will be removed as well.
                        for (int i = min.LineIndex + 1; i < max.LineIndex; i++)
                            removed.Add(_lines[i]);

                        // Apply removal to the lines list
                        foreach (LineRecord lr in removed)
                            _lines.Remove(lr);

                        // The lines list needs renumbering
                        RenumberLines(_lines);
                    } // else

                    //RecalculateMaxDisplayedColumns();

                    if (remfirst && remlast)
                    {
                        // In the case that all the selected lines were removed - we'll
                        // place the cursor on the previous line's last character.
                        int prev = min.LineIndex - 1;
                        if (prev >= 0)
                        {
                            if (_lines[prev].Text != null)
                                SetCursorPosition(prev, _lines[prev].Text.Length);
                            else
                                SetCursorPosition(prev, 0);
                        }
                        else
                            SetCursorPosition(0, 0);
                    }
                    else
                        SetCursorPosition(min);

                    _selection.StartPosition = _cursor;
                    _selection.EndPosition = _cursor;

                    RecalculateMaxDisplayedColumns();

                    // BACKSPACE operations cause the diff control to get out of synch with its peer diff control
                    _synched = false;

                    // Notify
                    if (SelectionChanged != null)
                        SelectionChanged(this, EventArgs.Empty);

                    Invalidate();
                } // else

                // Mark that a modification has occured
                _modified = true;
                if (LinesChanged != null)
                    LinesChanged(this, EventArgs.Empty);                
            }
            finally
            {
                if (SnapshotChanged != null)
                    SnapshotChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Recalculate the maximum number of columns that can be displayed in the
        /// current page. Notify listeneres if this number has changed (should be
        /// used to adjust horizontal scrollbar if attached).
        /// </summary>
        private void RecalculateMaxDisplayedColumns()
        {
            int next;
            int prev = _maxDisplayedCols;

            // Compute the maximum number of displayed columns for this page
            next = 0;
            for (int i = _startLine; i < _startLine + _linesPerPage && i < _lines.Count; i++)
            {
                LineRecord le = _lines[i];
                if (le.Text != null)
                {
                    if (le.Text.Length > next)
                        next = le.Text.Length;
                }
            } // for

            if (next != prev)
            {
                _maxDisplayedCols = next;

                // Issue a scroll event
                if (ScrollNeedsUpdate != null)
                    ScrollNeedsUpdate(this, EventArgs.Empty);
            }
        }

        private void RenumberLines(List<LineRecord> lines)
        {
            int num = 0;
            foreach (LineRecord lrec in lines)
            {
                if (lrec.Text != null)
                {
                    lrec.LineNumber = num;
                    num++;
                }
                else
                    lrec.LineNumber = -1;
            } // foreach
        }

        private void EnsurePositionIsVisible(DiffEditPosition pos)
        {
            int line = pos.LineIndex;
            int col = pos.ColumnIndex;

            if (line < _startLine)
            {
                if (_lines.Count <= _linesPerPage)
                    StartLine = 0;
                else
                    StartLine = line;
            }
            else if (line >= _startLine + _linesPerPage)
                StartLine = line - _linesPerPage + 1;

            if (col > _startColumn + _colsPerLine)
                StartColumn = col - _colsPerLine;
            else if (col == _startColumn + _colsPerLine && _colsPerLine > 0)
                StartColumn = _startColumn + 1; // Needed to make the cursor visible
            else if (col < _startColumn)
                StartColumn = col;
        }

        private DiffEditPosition GetTextPosition(Point point)
        {
            DiffEditPosition res = new DiffEditPosition();

            if (_lines.Count == 0)
                return new DiffEditPosition(0, 0);

            // Determine the index of the clicked line
            int idx = (int)Math.Floor((double)(point.Y / _lineHeight));
            int line = _startLine + idx;
            if (line >= _lines.Count)
            {
                // In case the user clicked below the ending text line - adjust the
                // line index to be right after the end line.
                res.LineIndex = _lines.Count-1;
                if (_lines[res.LineIndex].Text != null)
                    res.ColumnIndex = _lines[res.LineIndex].Text.Length;
                else
                    res.ColumnIndex = 0;
            }
            else if (line >= 0)
            {
                res.LineIndex = line;

                // Determine the index of the column
                idx = (int)Math.Floor((double)((point.X-_textRectangle.Left+_startColumn*_charWidth) / _charWidth));

                // Adjust in case the user clicked in the line numbers column
                if (idx < 0)
                    idx = 0;
                
                // Find the text in the clicked line
                string text = _lines[line].Text;
                if (text == null)
                    res.ColumnIndex = 0;
                else
                {
                    if (idx >= text.Length)
                        res.ColumnIndex = text.Length;
                    else
                        res.ColumnIndex = idx;
                } // else
            } // else
            else
                return new DiffEditPosition(0, 0);

            return res;
        }

        private void CalcLayout()
        {
            // Compute the number of lines that can be displayed in the control
            // without scrolling.
            using (Graphics g = this.CreateGraphics())
            {
                int lcount = _lines.Count;
                SizeF sf = g.MeasureString("0"+lcount.ToString(), this.Font);

                _borderRectangle = new Rectangle(0, 0, this.Width-2, this.Height-2);
                _lineNumbersRectangle = new RectangleF(1, 1, sf.Width, this.Height - 2);
                _paddingRectangle = new RectangleF(_lineNumbersRectangle.Right, 1, LINE_NUMBERS_PADDING, this.Height - 2);
                _textRectangle = new RectangleF(_paddingRectangle.Right, 1, 
                    this.Width - _lineNumbersRectangle.Width-_paddingRectangle.Width-2, this.Height - 2);

                Size sz = TextRenderer.MeasureText(g, "M", this.Font, new Size(100, 100), TextFormatFlags.NoPadding);
                _colsPerLine = (int)Math.Floor(_textRectangle.Width / (sz.Width));
                if (_colsPerLine <= 0)
                    _colsPerLine = 1;

                _charWidth = sz.Width;
                _charHeight = sz.Height;

                _lineHeight = sz.Height;
                _linesPerPage = (int)(this.Height / _lineHeight);
                if (_linesPerPage < 0)
                    _linesPerPage = 0;
            } // using
        }
        #endregion

        #region Interop methods
        #endregion

        #region Private Constants
        private const int LINE_NUMBERS_PADDING = 4;
        #endregion

        #region Private Variables
        private int _charWidth;
        private int _charHeight;

        private DiffEditPosition _cursor;
        private Timer _cursorTimer = new Timer();
        private bool _cursorOn = false;

        private int _startLine;
        private int _startColumn;
        private int _linesPerPage;
        private int _colsPerLine;
        private int _maxDisplayedCols;
        private int _lineHeight;
        private RectangleF _lineNumbersRectangle;
        private RectangleF _textRectangle;
        private RectangleF _paddingRectangle;
        private Rectangle _borderRectangle;
        private Brush _lineNumbersBrush = new SolidBrush(Color.LemonChiffon);
        private Brush _textColumnBrush = new SolidBrush(Color.White);
        private Brush _selectionBrush = SystemBrushes.Highlight;
        private Brush _modifiedBrush = new SolidBrush(Color.Brown);
        private Color _selectionForeColor = SystemColors.HighlightText;
        private StringFormat _lineNumbersFormat = new StringFormat(StringFormatFlags.DirectionRightToLeft);
        private StringFormat _textStringFormat = new StringFormat(StringFormatFlags.NoWrap | StringFormatFlags.NoFontFallback);
        private List<LineRecord> _lines = new List<LineRecord>();
        private DiffEditRange _selection = new DiffEditRange();
        private bool _mouseSelectionIsActive = false;
        private bool _activeSelection = false;
        private DiffEditPosition _selectionBeginPosition;
        private bool _modified = false;
        private bool _synched = true;

        private string _tabSpaces = "    ";
        private Regex _nlrx = new Regex("\n|\r\n");
        #endregion
    }

    public struct DiffEditPosition
    {
        public DiffEditPosition(int lineIndex, int columnIndex)
        {
            _lineIndex = lineIndex;
            _colIndex = columnIndex;
            if (_colIndex < 0)
                throw new ArgumentException();
        }

        public int LineIndex
        {
            get { return _lineIndex; }
            set { _lineIndex = value; }
        }

        public int ColumnIndex
        {
            get { return _colIndex; }
            set 
            { 
                _colIndex = value;
                if (_colIndex < 0)
                    throw new ArgumentException();
            }
        }

        public static bool operator <=(DiffEditPosition pos1, DiffEditPosition pos2)
        {
            if (pos1 == pos2 || pos1 < pos2)
                return true;
            return false;
        }

        public static bool operator >=(DiffEditPosition pos1, DiffEditPosition pos2)
        {
            if (pos1 == pos2 || pos1 > pos2)
                return true;
            return false;
        }

        public static bool operator ==(DiffEditPosition pos1, DiffEditPosition pos2)
        {
            if (pos1.LineIndex == pos2.LineIndex && pos1.ColumnIndex == pos2.ColumnIndex)
                return true;
            return false;
        }

        public static bool operator !=(DiffEditPosition pos1, DiffEditPosition pos2)
        {
            if (pos1.LineIndex != pos2.LineIndex || pos1.ColumnIndex != pos2.ColumnIndex)
                return true;
            return false;
        }

        public static bool operator <(DiffEditPosition pos1, DiffEditPosition pos2)
        {
            if (pos1.LineIndex < pos2.LineIndex)
                return true;
            if (pos1.LineIndex == pos2.LineIndex)
            {
                if (pos1.ColumnIndex < pos2.ColumnIndex)
                    return true;
            }

            return false;
        }

        public static bool operator >(DiffEditPosition pos1, DiffEditPosition pos2)
        {
            if (pos1.LineIndex > pos2.LineIndex)
                return true;
            if (pos1.LineIndex == pos2.LineIndex)
            {
                if (pos1.ColumnIndex > pos2.ColumnIndex)
                    return true;
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is DiffEditPosition))
                return false;

            DiffEditPosition pos = (DiffEditPosition)obj;
            if (pos._colIndex != _colIndex)
                return false;
            if (pos._lineIndex != _lineIndex)
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            long hash = _lineIndex.GetHashCode() * _colIndex.GetHashCode();
            return hash.GetHashCode();
        }

        private int _lineIndex;
        private int _colIndex;
    }

    public struct DiffEditRange
    {
        public DiffEditRange(DiffEditPosition start, DiffEditPosition end)
        {
            _start = start;
            _end = end;
        }

        public bool IsEmpty
        {
            get { return _start.Equals(_end); }
        }

        public void Clear()
        {
            _start = new DiffEditPosition();
            _end = new DiffEditPosition();
        }

        public bool ContainsPosition(int lineIndex, int colIndex)
        {
            if (IsEmpty)
                return false;

            DiffEditPosition pos = new DiffEditPosition(lineIndex, colIndex);

            if (_start < _end && pos >= _start && pos < _end)
                return true;
            else if (_end < _start && pos >= _end && pos < _start)
                return true;
            return false;
        }

        public void SetRange(DiffEditPosition pt1, DiffEditPosition pt2)
        {
            _start = pt1;
            _end = pt2;
        }

        public DiffEditPosition MinPosition
        {
            get
            {
                if (_start < _end)
                    return _start;
                else
                    return _end;
            }
        }

        public DiffEditPosition MaxPosition
        {
            get
            {
                if (_end > _start)
                    return _end;
                else
                    return _start;
            }
        }

        public DiffEditPosition StartPosition
        {
            get { return _start; }
            set { _start = value; }
        }

        public DiffEditPosition EndPosition
        {
            get { return _end; }
            set { _end = value; }
        }

        private DiffEditPosition _start;
        private DiffEditPosition _end;
    }
}
