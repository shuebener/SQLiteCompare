using System;
using System.Collections.Generic;
using System.Text;

namespace UndoRedo
{
    /// <summary>
    /// This class serves as the basis for all undoable actions in the software.
    /// </summary>
    public abstract class UndoableAction
    {
        /// <summary>
        /// Get the name of the action
        /// </summary>
        public abstract string Name
        {
            get;
        }

        /// <summary>
        /// Get the description of the undoable action
        /// </summary>
        public abstract string Description
        {
            get;
        }

        /// <summary>
        /// Performs the action
        /// </summary>
        /// <param name="redo">TRUE indicates that the action is redone</param>
        public abstract void Do(bool redo);

        /// <summary>
        /// Undo the action
        /// </summary>
        public abstract void Undo();
    }
}
