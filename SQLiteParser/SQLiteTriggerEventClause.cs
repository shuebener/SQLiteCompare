using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteTriggerEventClause
    {
        private SQLiteTriggerEventClause()
        {
        }

        public SQLiteTriggerEventClause(SQLiteTriggerEvent e)
        {
            _triggerEvent = e;
        }

        public SQLiteTriggerEventClause(List<string> columns)
        {
            _triggerEvent = SQLiteTriggerEvent.Update;
            Columns = columns;
        }

        public SQLiteTriggerEvent TriggerEvent
        {
            get { return _triggerEvent; }
            set { _triggerEvent = value; }
        }

        public List<string> Columns
        {
            get { return _columns; }
            set 
            {
                if (value != null)
                {
                    _columns = new List<string>();
                    foreach (string cname in value)
                        _columns.Add(Utils.QuoteIfNeeded(cname));
                }
                _triggerEvent = SQLiteTriggerEvent.Update; 
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteTriggerEventClause dst = obj as SQLiteTriggerEventClause;
            if (dst == null)
                return false;

            if (!RefCompare.CompareList<string>(_columns, dst._columns))
                return false;
            if (_triggerEvent != dst._triggerEvent)
                return false;
            return true;
        }

        public override string ToString()
        {
            switch (_triggerEvent)
            {
                case SQLiteTriggerEvent.Delete:
                    return "DELETE";
                case SQLiteTriggerEvent.Insert:
                    return "INSERT";
                case SQLiteTriggerEvent.Update:
                    if (_columns != null)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("UPDATE OF ");
                        for (int i = 0; i < _columns.Count; i++)
                        {
                            sb.Append(_columns[i]);
                            if (i < _columns.Count - 1)
                                sb.Append(",");
                        } // for
                        return sb.ToString();
                    }
                    else
                        return "UPDATE";
                default:
                    throw new ArgumentException("illegal trigger event type [" + _triggerEvent.ToString() + "]");
            } // switch
        }

        public virtual object Clone()
        {
            List<string> columns = null;
            if (_columns != null)
            {
                columns = new List<string>();
                foreach (string str in _columns)
                    columns.Add(str);
            }

            SQLiteTriggerEventClause res = new SQLiteTriggerEventClause();
            res._triggerEvent = _triggerEvent;
            res._columns = columns;
            return res;
        }

        private List<string> _columns;
        private SQLiteTriggerEvent _triggerEvent;        
    }

    public enum SQLiteTriggerEvent
    {
        None = 0,

        Delete = 1,

        Insert = 2,

        Update = 3,
    }
}
