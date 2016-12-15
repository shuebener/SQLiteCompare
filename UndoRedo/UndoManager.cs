using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;

namespace UndoRedo
{
    /// <summary>
    /// Supports undoable actions.
    /// </summary>
    public class UndoManager
    {
        #region Events
        /// <summary>
        /// Fired whenever the undo manager changes state
        /// </summary>
        public event EventHandler UndoStateChanged;
        #endregion

        #region Public Properties
        /// <summary>
        /// Get TRUE if there are actions to be undone
        /// </summary>
        public bool CanUndo
        {
            get
            {
                return _undoList.Count > 0;
            }
        }

        /// <summary>
        /// Get TRUE if there are actions to be redone
        /// </summary>
        public bool CanRedo
        {
            get
            {
                return _redoList.Count > 0;
            }
        }

        /// <summary>
        /// Get the undo action description
        /// </summary>
        public string UndoActionDescription
        {
            get
            {
                if (_undoList.Count > 0)
                    return _undoList[_undoList.Count - 1].Name;
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// Get the redo action description
        /// </summary>
        public string RedoActionDescription
        {
            get
            {
                if (_redoList.Count > 0)
                    return _redoList[_redoList.Count - 1].Name;
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// Get the list of actions that were performed in the manager
        /// </summary>
        public List<ActionRecord> ActionsHistory
        {
            get { return _actionsHistory; }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Clear all undoable actions
        /// </summary>
        public void Clear()
        {
            _undoList.Clear();
            _redoList.Clear();
            _actionsHistory.Clear();

            if (UndoStateChanged != null)
                UndoStateChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Do the specified action while updating the internal undo/redo lists.
        /// </summary>
        /// <param name="action">The action to perform</param>
        public void Do(UndoableAction action)
        {
            // Do the action
            action.Do(false);

            // Clear the redo list
            _redoList.Clear();

            // Add the action to the undo list.
            _undoList.Add(action);

            // Add action record to the history log
            ActionRecord ar = new ActionRecord(DateTime.Now, action.Name, 
                action.Description, ActionType.Do);
            _actionsHistory.Add(ar);

            // Notify that the state of the undo manager has changed
            if (UndoStateChanged != null)
                UndoStateChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Undo the most recent action
        /// </summary>
        public void Undo()
        {
            if (_undoList.Count == 0)
                return;

            // Get the most recent action
            UndoableAction action = _undoList[_undoList.Count - 1];

            // Undo that action
            action.Undo();

            // Remove the action from the undo list and add it to the redo list.
            _undoList.Remove(action);
            _redoList.Add(action);

            // Add action record to the history log
            ActionRecord ar = new ActionRecord(DateTime.Now, action.Name, 
                action.Description, ActionType.Undo);
            _actionsHistory.Add(ar);

            // Notify that the state of the undo manager has changed
            if (UndoStateChanged != null)
                UndoStateChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Redo the last action.
        /// </summary>
        public void Redo()
        {
            if (_redoList.Count == 0)
                return;

            // Get the latest action to redo
            UndoableAction action = _redoList[_redoList.Count - 1];

            // Redo that action
            action.Do(true);

            // Remove the action from the redo list and add it to the undo list
            _redoList.Remove(action);
            _undoList.Add(action);

            // Add action record to the history log
            ActionRecord ar = new ActionRecord(DateTime.Now, action.Name, 
                action.Description, ActionType.Redo);
            _actionsHistory.Add(ar);

            // Notify that the state of the undo manager has changed
            if (UndoStateChanged != null)
                UndoStateChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Returns a list of actions that were saved into a history file.
        /// </summary>
        /// <param name="fpath">The path to the file that contains the actions.</param>
        /// <returns>The list of action records that were recorded in the file.</returns>
        public List<ActionRecord> LoadActionsHistory(string fpath)
        {
            BinaryFormatter s = new BinaryFormatter();
            using (FileStream fs = File.OpenRead(fpath))
            {
                object obj = s.Deserialize(fs);
                return obj as List<ActionRecord>;
            } // using
        }

        /// <summary>
        /// Saves the specified list of action records into the specified file.
        /// </summary>
        /// <param name="fpath">The path to the history file.</param>
        /// <param name="history">The list of actions history to save</param>
        public void SaveActionsHistory(string fpath, List<ActionRecord> history)
        {
            BinaryFormatter s = new BinaryFormatter();
            using (FileStream fs = File.Open(fpath, FileMode.Create))
            {
                s.Serialize(fs, history);
            } // using
        }
        #endregion

        #region Private Variables
        private List<UndoableAction> _undoList = new List<UndoableAction>();
        private List<UndoableAction> _redoList = new List<UndoableAction>();
        private List<ActionRecord> _actionsHistory = new List<ActionRecord>();
        #endregion
    }
}
