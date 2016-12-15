using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteReferenceHandler
    {
        private SQLiteReferenceHandler()
        {
        }

        public SQLiteReferenceHandler(SQLiteReferenceTrigger trigger, SQLiteReferenceAction action)
        {
            _trigger = trigger;
            _action = action;
            _matchName = null;
        }

        public SQLiteReferenceHandler(string matchName)
        {
            _trigger = SQLiteReferenceTrigger.Match;
            _matchName = matchName;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteReferenceHandler dst = obj as SQLiteReferenceHandler;
            if (dst == null)
                return false;

            if (_trigger != dst._trigger || _matchName != dst._matchName || _action != dst._action)
                return false;

            return true;
        }

        public override string ToString()
        {
            if (_matchName != null)
                return "MATCH " + _matchName;

            StringBuilder sb = new StringBuilder();
            sb.Append("ON "+Utils.GetReferenceTriggerString(_trigger)+" ");
            switch (_action)
            {
                case SQLiteReferenceAction.SetNull:
                    sb.Append("SET NULL");
                    break;
                case SQLiteReferenceAction.SetDefault:
                    sb.Append("SET DEFAULT");
                    break;
                case SQLiteReferenceAction.Cascade:
                    sb.Append("CASCADE");
                    break;
                case SQLiteReferenceAction.Restrict:
                    sb.Append("RESTRICT");
                    break;
                default:
                    throw new ArgumentException("illegal reference action [" + _action.ToString() + "]");
            } // switch

            return sb.ToString();
        }

        public virtual object Clone()
        {
            SQLiteReferenceHandler res = new SQLiteReferenceHandler();
            res._trigger = _trigger;
            res._action = _action;
            res._matchName = _matchName;
            return res;
        }

        private SQLiteReferenceTrigger _trigger;
        private SQLiteReferenceAction _action;
        private string _matchName;
    }

    public enum SQLiteReferenceAction
    {
        None = 0,

        SetNull = 1,

        SetDefault = 2,

        Cascade = 3,

        Restrict = 4,
    }

    public enum SQLiteReferenceTrigger
    {
        None = 0,

        OnDelete = 1,

        OnInsert = 2,

        OnUpdate = 3,

        Match = 4,
    }
}
