using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace UndoRedo
{
    /// <summary>
    /// Encapsulates the details a single action that was performed by the user.
    /// </summary>
    [Serializable]
    public class ActionRecord
    {
        #region Constructors
        public ActionRecord()
        {
        }

        public ActionRecord(DateTime time, string name, string description, ActionType type)
        {
            _actionTime = time;
            _actionName = name;
            _actionDescription = description;
            _actionType = type;
        }
        #endregion

        #region Public Properties
        public DateTime ActionTime
        {
            get { return _actionTime; }
        }

        public string ActionName
        {
            get { return _actionName; }
        }

        public string ActionDescription
        {
            get { return _actionDescription; }
        }

        public ActionType ActionType
        {
            get { return _actionType; }
        }
        #endregion

        #region Private Variables
        private DateTime _actionTime;
        private string _actionName;
        private string _actionDescription;
        private ActionType _actionType = ActionType.None;
        #endregion
    }

    /// <summary>
    /// The type of the action that was performed by the user
    /// </summary>
    public enum ActionType
    {
        /// <summary>
        /// Illegal value
        /// </summary>
        None = 0,

        /// <summary>
        /// Normal action
        /// </summary>
        Do = 1,

        /// <summary>
        /// Undo action
        /// </summary>
        Undo = 2,

        /// <summary>
        /// Redo action
        /// </summary>
        Redo = 3,
    }
}
