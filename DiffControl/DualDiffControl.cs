using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DifferenceEngine;
using UndoRedo;

namespace DiffControl
{
    /// <summary>
    /// Provides the dual diff view in which the user can view the differences
    /// between two schema objects and edit these differences.
    /// </summary>
    public partial class DualDiffControl : UserControl
    {
        #region Events
        /// <summary>
        /// Fired whenever the UNDO/REDO stacks change
        /// </summary>
        public event EventHandler UndoStateChanged;

        /// <summary>
        /// Request to save the left diff box
        /// </summary>
        public event EventHandler LeftSaveRequested;

        /// <summary>
        /// Request to save the right diff box
        /// </summary>
        public event EventHandler RightSaveRequested;
        #endregion

        #region Constructors
        public DualDiffControl()
        {
            InitializeComponent();

            // Register to receive UNDO/REDO notifications
            _undoManager.UndoStateChanged += new EventHandler(_undoManager_UndoStateChanged);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Load the specified strings, compare them and update the diff controls.
        /// </summary>
        /// <param name="left">The left strings array to compare</param>
        /// <param name="right">The right strings array to compare</param>
        public void CompareTexts(string[] left, string[] right, string leftTitle, string rightTitle)
        {
            _undoManager.Clear();

            lblLeftPath.Text = leftTitle;
            lblRightPath.Text = rightTitle;

            List<LineRecord> leftrecs = new List<LineRecord>();
            List<LineRecord> rightrecs = new List<LineRecord>();

            PrepareDiff(left, right, leftrecs, rightrecs);

            ucLeftDiff.Lines = leftrecs;
            ucLeftDiff.ClearModified();
            ucRightDiff.Lines = rightrecs;
            ucRightDiff.ClearModified();

            UpdateDiffBar();
        }

        /// <summary>
        /// Replace the text lines in both the left and the right diff edit boxes.
        /// </summary>
        /// <param name="left">The text to place into the left diff edit box</param>
        /// <param name="right">The text to place into the right diff edit box</param>
        public void ReplaceText(string[] left, string[] right)
        {
            _leftSnapshot = ucLeftDiff.GetSnapshot();
            _rightSnapshot = ucRightDiff.GetSnapshot();

            List<LineRecord> leftrecs = new List<LineRecord>();
            List<LineRecord> rightrecs = new List<LineRecord>();

            PrepareDiff(left, right, leftrecs, rightrecs);

            ucLeftDiff.Lines = leftrecs;
            ucRightDiff.Lines = rightrecs;

            UpdateDiffBar();

            ChangeTextAction action = new ChangeTextAction(ucLeftDiff, ucRightDiff,
                _leftSnapshot, ucLeftDiff.GetSnapshot(),
                _rightSnapshot, ucRightDiff.GetSnapshot());
            _undoManager.Do(action);
        }

        /// <summary>
        /// Undo the last action that was performed in any of the two diff views
        /// </summary>
        public void Undo()
        {
            if (_undoManager.CanUndo)
                _undoManager.Undo();
        }

        /// <summary>
        /// Redo the last action that was performed in any of the two diff views
        /// </summary>
        public void Redo()
        {
            if (_undoManager.CanRedo)
                _undoManager.Redo();
        }

        /// <summary>
        /// Returns the text that resides in the left diff edit box.
        /// </summary>
        public string GetLeftText()
        {
            return ucLeftDiff.GetLinesText();
        }

        /// <summary>
        /// Returns the text that resides in the right diff edit box
        /// </summary>
        public string GetRightText()
        {
            return ucRightDiff.GetLinesText();
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Returns TRUE if there is an operation in the UNDO stack that can be undone
        /// </summary>
        public bool CanUndo
        {
            get { return _undoManager.CanUndo; }
        }

        /// <summary>
        /// Returns TRUE if the ere is an operation in the REDO stack that can be undone
        /// </summary>
        public bool CanRedo
        {
            get { return _undoManager.CanRedo; }
        }

        /// <summary>
        /// Returns TRUE if the left view was modified
        /// </summary>
        public bool IsLeftModified
        {
            get { return ucLeftDiff.IsModified; }
        }

        /// <summary>
        /// Returns TRUE if the right view was modified
        /// </summary>
        public bool IsRightModified
        {
            get { return ucRightDiff.IsModified; }
        }

        /// <summary>
        /// Returns TRUE if the left view is focused, FALSE otherwise
        /// </summary>
        public bool IsLeftFocused
        {
            get { return ucLeftDiff.Focused; }
        }
        #endregion

        #region Event Handlers

        private void ucLeftDiff_UndoRequested(object sender, EventArgs e)
        {
            Undo();
        }

        private void ucLeftDiff_RedoRequested(object sender, EventArgs e)
        {
            Redo();
        }

        private void ucRightDiff_UndoRequested(object sender, EventArgs e)
        {
            Undo();
        }

        private void ucRightDiff_RedoRequested(object sender, EventArgs e)
        {
            Redo();
        }

        private void _undoManager_UndoStateChanged(object sender, EventArgs e)
        {
            if (UndoStateChanged != null)
                UndoStateChanged(this, e);
        }

        private void ucLeftDiff_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                UpdateState();
        }

        private void ucRightDiff_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                UpdateState();
        }

        private void mniCopyFromRight_Click(object sender, EventArgs e)
        {
            if (!CheckSynchronized())
                return;

            // Get the change range from the source diff control
            DiffEditRange range = new DiffEditRange();
            ucRightDiff.GetChangeRange(ucLeftDiff.GetCursorPosition(), ref range);

            // Copy the change lines from the source diff control
            ucLeftDiff.ReplaceLines(range.MinPosition.LineIndex, range.MaxPosition.LineIndex, ucRightDiff);

            UpdateState();
        }

        private void mniCopyToRight_Click(object sender, EventArgs e)
        {
            if (!CheckSynchronized())
                return;

            // Get the change range from the source diff control
            DiffEditRange range = new DiffEditRange();
            ucLeftDiff.GetChangeRange(ucLeftDiff.GetCursorPosition(), ref range);

            // Copy the change lines from the source diff control
            ucRightDiff.ReplaceLines(range.MinPosition.LineIndex, range.MaxPosition.LineIndex, ucLeftDiff);

            UpdateState();
        }

        private void mniCopyFromLeft_Click(object sender, EventArgs e)
        {
            if (!CheckSynchronized())
                return;

            // Get the change range from the source diff control
            DiffEditRange range = new DiffEditRange();
            ucRightDiff.GetChangeRange(ucRightDiff.GetCursorPosition(), ref range);

            // Copy the change lines from the source diff control
            ucRightDiff.ReplaceLines(range.MinPosition.LineIndex, range.MaxPosition.LineIndex, ucLeftDiff);

            UpdateState();
        }

        private void mniCopyToLeft_Click(object sender, EventArgs e)
        {
            if (!CheckSynchronized())
                return;

            // Get the change range from the source diff control
            DiffEditRange range = new DiffEditRange();
            ucRightDiff.GetChangeRange(ucRightDiff.GetCursorPosition(), ref range);

            // Copy the change lines from the source diff control
            ucLeftDiff.ReplaceLines(range.MinPosition.LineIndex, range.MaxPosition.LineIndex, ucRightDiff);

            UpdateState();
        }

        private void btnCopyLeftToRight_Click(object sender, EventArgs e)
        {
            if (_lastCursorMove == ucLeftDiff)
                ucRightDiff.SetCursorPosition(ucLeftDiff.GetCursorPosition());
            else
                ucLeftDiff.SetCursorPosition(ucRightDiff.GetCursorPosition());
            mniCopyFromLeft_Click(mniCopyFromLeft, e);
            _lastCursorMove.Focus();
        }

        private void btnCopyRightToLeft_Click(object sender, EventArgs e)
        {
            if (_lastCursorMove == ucLeftDiff)
                ucRightDiff.SetCursorPosition(ucLeftDiff.GetCursorPosition());
            else
                ucLeftDiff.SetCursorPosition(ucRightDiff.GetCursorPosition());
            mniCopyFromRight_Click(mniCopyFromRight, e);
            _lastCursorMove.Focus();
        }

        private void ucLeftDiff_LinesChanged(object sender, EventArgs e)
        {
            lblLeftEdit.Visible = ucLeftDiff.IsModified;
            UpdateDiffBar();
        }

        private void ucRightDiff_LinesChanged(object sender, EventArgs e)
        {
            lblRightEdit.Visible = ucRightDiff.IsModified;
            UpdateDiffBar();
        }

        private void ucLeftDiff_SnapshotChanged(object sender, EventArgs e)
        {
            ChangeTextAction action = new ChangeTextAction(ucLeftDiff, ucRightDiff,
                _leftSnapshot, ucLeftDiff.GetSnapshot(),
                _rightSnapshot, ucRightDiff.GetSnapshot());
            _undoManager.Do(action);
        }

        private void ucLeftDiff_SnapshotChanging(object sender, EventArgs e)
        {
            _leftSnapshot = ucLeftDiff.GetSnapshot();
            _rightSnapshot = ucRightDiff.GetSnapshot();
        }

        private void ucRightDiff_SnapshotChanged(object sender, EventArgs e)
        {
            ChangeTextAction action = new ChangeTextAction(ucLeftDiff, ucRightDiff,
                _leftSnapshot, ucLeftDiff.GetSnapshot(),
                _rightSnapshot, ucRightDiff.GetSnapshot());
            _undoManager.Do(action);
        }

        private void ucRightDiff_SnapshotChanging(object sender, EventArgs e)
        {
            _rightSnapshot = ucRightDiff.GetSnapshot();
            _leftSnapshot = ucLeftDiff.GetSnapshot();
        }

        private void scbVertical_Scroll(object sender, ScrollEventArgs e)
        {
            ucLeftDiff.StartLine = scbVertical.Value;
            ucRightDiff.StartLine = scbVertical.Value;
            diffBar1.PageSize = scbVertical.LargeChange;
            diffBar1.PageStartRow = scbVertical.Value;
            Refresh();
        }

        private void scbHorizontal_Scroll(object sender, ScrollEventArgs e)
        {
            ucLeftDiff.StartColumn = scbHorizontal.Value;
            ucRightDiff.StartColumn = scbHorizontal.Value;
            Refresh();
        }

        private void ucLeftDiff_ScrollNeedsUpdate(object sender, EventArgs e)
        {
            if (_nested)
                return;

            _nested = true;
            try
            {                
                scbVertical.Minimum = 0;
                scbVertical.Maximum = ucLeftDiff.Lines.Count - 1 >= 0 ? ucLeftDiff.Lines.Count - 1 : 0;
                scbVertical.SmallChange = 1;
                scbVertical.LargeChange = ucLeftDiff.LineScrollPageSize;
                scbVertical.Value = ucLeftDiff.StartLine;
                ucRightDiff.StartLine = ucLeftDiff.StartLine;

                int lchange = ucRightDiff.ColumnScrollPageSize;
                if (lchange > ucLeftDiff.ColumnScrollPageSize)
                    lchange = ucLeftDiff.ColumnScrollPageSize;

                int maxhscroll = ucRightDiff.MaxDisplayedColumns;
                if (maxhscroll < ucLeftDiff.MaxDisplayedColumns)
                    maxhscroll = ucLeftDiff.MaxDisplayedColumns;
                maxhscroll++;

                scbHorizontal.Minimum = 0;
                scbHorizontal.Maximum = maxhscroll;
                scbHorizontal.SmallChange = 1;
                scbHorizontal.LargeChange = lchange;

                scbHorizontal.Value = ucLeftDiff.StartColumn;
                ucRightDiff.StartColumn = scbHorizontal.Value;

                diffBar1.PageSize = scbVertical.LargeChange;
                diffBar1.PageStartRow = scbVertical.Value;
            }
            finally
            {
                _nested = false;
            }
        }

        private void ucRightDiff_ScrollNeedsUpdate(object sender, EventArgs e)
        {
            if (_nested)
                return;

            _nested = true;
            try
            {
                scbVertical.Minimum = 0;
                scbVertical.Maximum = ucRightDiff.Lines.Count - 1 >= 0 ? ucRightDiff.Lines.Count - 1 : 0;
                scbVertical.SmallChange = 1;
                scbVertical.LargeChange = ucRightDiff.LineScrollPageSize;
                scbVertical.Value = ucRightDiff.StartLine;
                ucLeftDiff.StartLine = ucRightDiff.StartLine;

                int lchange = ucRightDiff.ColumnScrollPageSize;
                if (lchange > ucLeftDiff.ColumnScrollPageSize)
                    lchange = ucLeftDiff.ColumnScrollPageSize;
                int maxhscroll = ucRightDiff.MaxDisplayedColumns;
                if (maxhscroll < ucLeftDiff.MaxDisplayedColumns)
                    maxhscroll = ucLeftDiff.MaxDisplayedColumns;
                maxhscroll++;

                scbHorizontal.Minimum = 0;
                scbHorizontal.Maximum = maxhscroll;
                scbHorizontal.SmallChange = 1;
                scbHorizontal.LargeChange = lchange;

                scbHorizontal.Value = ucRightDiff.StartColumn;
                ucLeftDiff.StartColumn = scbHorizontal.Value;

                diffBar1.PageSize = scbVertical.LargeChange;
                diffBar1.PageStartRow = scbVertical.Value;
            }
            finally
            {
                _nested = false;
            }
        }

        private void diffBar1_MouseClick(object sender, MouseEventArgs e)
        {
            long row = diffBar1.GetRowIndexFromPoint(e.Location);
            long start = (long)(row - ucRightDiff.LineScrollPageSize / 2F);
            if (start < 0)
                start = 0;
            else if (start > scbVertical.Maximum + 1 - scbVertical.LargeChange)
                start = scbVertical.Maximum + 1 - scbVertical.LargeChange;

            scbVertical.Value = (int)start;
            scbVertical_Scroll(scbVertical, new ScrollEventArgs(ScrollEventType.EndScroll, (int)start));
        }

        private void ucLeftDiff_CursorMoved(object sender, EventArgs e)
        {
            _lastCursorMove = ucLeftDiff;
            UpdateState();
        }

        private void ucRightDiff_CursorMoved(object sender, EventArgs e)
        {
            _lastCursorMove = ucRightDiff;
            UpdateState();
        }

        private void ucRightDiff_SaveRequested(object sender, EventArgs e)
        {
            if (RightSaveRequested != null)
                RightSaveRequested(this, EventArgs.Empty);
        }

        private void ucLeftDiff_SaveRequested(object sender, EventArgs e)
        {
            if (LeftSaveRequested != null)
                LeftSaveRequested(this, EventArgs.Empty);
        }

        #endregion

        #region Private Methods

        private void UpdateState()
        {
            DiffEditBox ebox = null;
            if (ucLeftDiff.Focused)
                ebox = ucLeftDiff;
            else if (ucRightDiff.Focused)
                ebox = ucRightDiff;

            if (ebox != null)
            {
                DiffEditRange range = new DiffEditRange();

                // Enable the COPY menu items only if the cursor is placed on a 
                // change range.
                bool onChange = ucRightDiff.GetChangeRange(ebox.GetCursorPosition(), ref range);
                btnCopyLeftToRight.Enabled = onChange;
                btnCopyRightToLeft.Enabled = onChange;
                mniCopyFromLeft.Enabled = onChange;
                mniCopyFromRight.Enabled = onChange;
                mniCopyToLeft.Enabled = onChange;
                mniCopyToRight.Enabled = onChange;
            }
            else
            {
                btnCopyLeftToRight.Enabled = false;
                btnCopyRightToLeft.Enabled = false;
                mniCopyFromLeft.Enabled = false;
                mniCopyFromRight.Enabled = false;
                mniCopyToLeft.Enabled = false;
                mniCopyToRight.Enabled = false;
            }
        }

        private void UpdateDiffBar()
        {
            int rows = ucLeftDiff.Lines.Count;
            if (rows < ucRightDiff.Lines.Count)
                rows = ucRightDiff.Lines.Count;

            Dictionary<int, Color> left = new Dictionary<int, Color>();
            Dictionary<int, Color> right = new Dictionary<int, Color>();

            UpdateDiffColors(left, ucLeftDiff.Lines);
            UpdateDiffColors(right, ucRightDiff.Lines);

            diffBar1.Clear();
            diffBar1.AddColumn(left);
            diffBar1.AddColumn(right);

            diffBar1.RowsCount = rows;
            diffBar1.PageSize = scbVertical.LargeChange;
            diffBar1.PageStartRow = scbVertical.Value;
        }

        private void UpdateDiffColors(Dictionary<int, Color> dcolors, List<LineRecord> lines)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                LineRecord lrec = lines[i];
                if (lrec.BackColor != Color.Empty)
                {
                    Color mark = lrec.BackColor;
                    if (lrec.RangesCount > 0)
                        mark = lrec[0].BackColor;
                    dcolors.Add(i, mark);
                }
            } // for
        }

        private bool CheckSynchronized()
        {
            if (!ucLeftDiff.IsSynched || !ucRightDiff.IsSynched)
            {
                MessageBox.Show(this,
                    "The two schema views are not synchronized.\r\nPlease save your changes or refresh the schema\r\n" +
                    "in order to perform copy operations.",
                    "Operation cannot proceed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void PrepareDiff(string[] left, string[] right, List<LineRecord> leftrecs, List<LineRecord> rightrecs)
        {
            DiffList_TextFile file1 = new DiffList_TextFile(left);
            DiffList_TextFile file2 = new DiffList_TextFile(right);

            DiffEngine engine = new DiffEngine();
            engine.ProcessDiff(file1, file2, DiffEngineLevel.SlowPerfect);
            ArrayList report = engine.DiffReport();

            foreach (DiffResultSpan dres in report)
            {
                switch (dres.Status)
                {
                    case DiffResultSpanStatus.NoChange:
                        for (int i = 0; i < dres.Length; i++)
                        {
                            LineRecord lrec = new LineRecord(i + dres.SourceIndex, left[i + dres.SourceIndex]);
                            leftrecs.Add(lrec);
                            LineRecord rrec = new LineRecord(i + dres.DestIndex, right[i + dres.DestIndex]);
                            rightrecs.Add(rrec);
                        }
                        break;
                    case DiffResultSpanStatus.Replace:
                        for (int i = 0; i < dres.Length; i++)
                        {
                            LineRecord lrec = new LineRecord(i + dres.SourceIndex, left[i + dres.SourceIndex]);
                            lrec.BackColor = Color.Khaki;
                            leftrecs.Add(lrec);
                            LineRecord rrec = new LineRecord(i + dres.DestIndex, right[i + dres.DestIndex]);
                            rrec.BackColor = Color.Khaki;
                            rightrecs.Add(rrec);
                            ComputeLineDifferences(lrec, rrec);
                        }
                        break;
                    case DiffResultSpanStatus.DeleteSource:
                        for (int i = 0; i < dres.Length; i++)
                        {
                            LineRecord lrec = new LineRecord(i + dres.SourceIndex, left[i + dres.SourceIndex]);
                            lrec.BackColor = Color.Khaki;
                            leftrecs.Add(lrec);
                            LineRecord rrec = new LineRecord();
                            rrec.BackColor = Color.LightGray;
                            rightrecs.Add(rrec);
                        }
                        break;
                    case DiffResultSpanStatus.AddDestination:
                        for (int i = 0; i < dres.Length; i++)
                        {
                            LineRecord lrec = new LineRecord();
                            lrec.BackColor = Color.LightGray;
                            leftrecs.Add(lrec);
                            LineRecord rrec = new LineRecord(i + dres.DestIndex, right[i + dres.DestIndex]);
                            rrec.BackColor = Color.Khaki;
                            rightrecs.Add(rrec);
                        }
                        break;
                    default:
                        break;
                } // switch
            } // foreach
        }

        private void ComputeLineDifferences(LineRecord lrec, LineRecord rrec)
        {
            DiffList_CharData left = new DiffList_CharData(lrec.Text);
            DiffList_CharData right = new DiffList_CharData(rrec.Text);

            DiffEngine engine = new DiffEngine();
            engine.ProcessDiff(left, right, DiffEngineLevel.SlowPerfect);
            ArrayList report = engine.DiffReport();

            foreach (DiffResultSpan dres in report)
            {
                switch (dres.Status)
                {
                    case DiffResultSpanStatus.NoChange:
                        break;
                    case DiffResultSpanStatus.Replace:
                        lrec.AddRange(dres.SourceIndex, dres.SourceIndex + dres.Length, Color.LightSalmon, Color.Empty);
                        rrec.AddRange(dres.DestIndex, dres.DestIndex + dres.Length, Color.LightSalmon, Color.Empty);
                        break;
                    case DiffResultSpanStatus.DeleteSource:
                        lrec.AddRange(dres.SourceIndex, dres.SourceIndex + dres.Length, Color.LightSalmon, Color.Empty);
                        break;
                    case DiffResultSpanStatus.AddDestination:
                        rrec.AddRange(dres.DestIndex, dres.DestIndex + dres.Length, Color.LightSalmon, Color.Empty);
                        break;
                    default:
                        break;
                } // switch
            } // foreach
        }
        #endregion

        #region Private Variables
        private bool _nested = false;
        private UndoManager _undoManager = new UndoManager();
        private DiffSnapshot _leftSnapshot;
        private DiffSnapshot _rightSnapshot;
        private DiffEditBox _lastCursorMove;
        #endregion
    }
}
