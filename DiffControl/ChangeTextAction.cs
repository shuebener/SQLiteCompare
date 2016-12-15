using System;
using System.Collections.Generic;
using System.Text;
using UndoRedo;

namespace DiffControl
{
    /// <summary>
    /// Encapsulates the case when any element of text was changed
    /// in one of the two diff edit boxes (dual diff control)
    /// </summary>
    public class ChangeTextAction : UndoableAction
    {
        public ChangeTextAction(DiffEditBox left, DiffEditBox right, 
            DiffSnapshot beforeLeft, DiffSnapshot afterLeft,
            DiffSnapshot beforeRight, DiffSnapshot afterRight)
        {
            _left = left;
            _right = right;
            _beforeLeft = beforeLeft;
            _beforeRight = beforeRight;
            _afterLeft = afterLeft;
            _afterRight = afterRight;
        }

        public override string Name
        {
            get { return "Change Text"; }
        }

        public override string Description
        {
            get { return "Text was changed"; }
        }

        public override void Do(bool redo)
        {
            if (redo)
            {
                _left.SetSnapshot(_afterLeft);
                _right.SetSnapshot(_afterRight);
            }
        }

        public override void Undo()
        {
            _left.SetSnapshot(_beforeLeft);
            _right.SetSnapshot(_beforeRight);
        }

        private DiffEditBox _left;
        private DiffEditBox _right;
        private DiffSnapshot _beforeLeft;
        private DiffSnapshot _afterLeft;
        private DiffSnapshot _beforeRight;
        private DiffSnapshot _afterRight;
    }
}
