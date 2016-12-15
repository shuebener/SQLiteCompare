using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteCreateTriggerStatement : SQLiteDdlStatement
    {
        private SQLiteCreateTriggerStatement(SQLiteObjectName triggerName)
            : base(triggerName)
        {
        }

        public SQLiteCreateTriggerStatement(bool isTemp, bool ifNotExists, SQLiteObjectName triggerName, 
            SQLiteTriggerTime triggerTime, SQLiteTriggerEventClause triggerEventClause, SQLiteObjectName table,
            bool foreachClause, SQLiteExpression whenExpr)
            : base(triggerName)
        {
            _isTemp = isTemp;
            _ifNotExists = ifNotExists;
            _triggerTime = triggerTime;
            _triggerEventClause = triggerEventClause;
            _table = table;
            _foreachClause = foreachClause;
            _whenExpr = whenExpr;
        }

        public bool IsTemp
        {
            get { return _isTemp; }
            set { _isTemp = value; }
        }

        public bool IfNotExists
        {
            get { return _ifNotExists; }
            set { _ifNotExists = value; }
        }

        public SQLiteTriggerTime TriggerTime
        {
            get { return _triggerTime; }
            set { _triggerTime = value; }
        }

        public SQLiteTriggerEventClause TriggerEvent
        {
            get { return _triggerEventClause; }
            set { _triggerEventClause = value; }
        }

        public SQLiteObjectName TableName
        {
            get { return _table; }
            set { _table = value; }
        }

        public bool HasForeachClause
        {
            get { return _foreachClause; }
            set { _foreachClause = value; }
        }

        public SQLiteExpression WhenExpression
        {
            get { return _whenExpr; }
            set { _whenExpr = value; }
        }

        public List<SQLiteStatement> StatementsList
        {
            get { return _slist; }
            set { _slist = value; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteCreateTriggerStatement dst = obj as SQLiteCreateTriggerStatement;
            if (dst == null)
                return false;

            if (this._isTemp != dst._isTemp ||
                this._ifNotExists != dst._ifNotExists ||
                this._triggerTime != dst._triggerTime ||
                this._foreachClause != dst._foreachClause)
                return false;

            if (!RefCompare.CompareMany(_triggerEventClause, dst._triggerEventClause,
                _table, dst._table, _whenExpr, dst._whenExpr))
                return false;
            if (!RefCompare.CompareList<SQLiteStatement>(_slist, dst._slist))
                return false;

            return base.Equals(obj);
        }

        public override string ToString()
        {
            // CREATE temp TRIGGER ifnotexists nm dbnm trigger_time trigger_event 
            // ON fullname 
            // foreach_clause 
            // when_clause
            // BEGIN
            // statements_list
            // END

            StringBuilder sb = new StringBuilder();
            sb.Append("CREATE");
            if (_isTemp)
                sb.Append(" TEMP");
            sb.Append(" TRIGGER");
            if (_ifNotExists)
                sb.Append(" IF NOT EXISTS");
            sb.Append(" " + base.ObjectName);
            if (_triggerTime != SQLiteTriggerTime.None)
                sb.Append("\r\n" + Utils.GetTriggerTimeString(_triggerTime));
            sb.Append(" " + _triggerEventClause.ToString());
            sb.Append("\r\nON " + _table.ToString());
            if (_foreachClause)
                sb.Append("\r\nFOR EACH ROW");
            if (_whenExpr != null)
                sb.Append("\r\nWHEN " + _whenExpr.ToString());
            sb.Append("\r\nBEGIN\r\n");
            sb.Append(Utils.FormatTriggerStatementsList(4, _slist));
            sb.Append("END");

            return sb.ToString();
        }

        public override object Clone()
        {
            SQLiteTriggerEventClause triggerEventClause = null;
            if (_triggerEventClause != null)
                triggerEventClause = (SQLiteTriggerEventClause)_triggerEventClause.Clone();
            SQLiteObjectName table = null;
            if (_table != null)
                table = (SQLiteObjectName)_table.Clone();
            SQLiteExpression whenExpr = null;
            if (_whenExpr != null)
                whenExpr = (SQLiteExpression)_whenExpr.Clone();
            List<SQLiteStatement> slist = null;
            if (_slist != null)
            {
                slist = new List<SQLiteStatement>();
                foreach (SQLiteStatement s in _slist)
                    slist.Add((SQLiteStatement)s.Clone());
            }
            SQLiteObjectName name = null;
            if (this.ObjectName != null)
                name = (SQLiteObjectName)this.ObjectName.Clone();

            SQLiteCreateTriggerStatement res = new SQLiteCreateTriggerStatement(name);
            res._foreachClause = _foreachClause;
            res._ifNotExists = _ifNotExists;
            res._isTemp = _isTemp;
            res._slist = slist;
            res._table = table;
            res._triggerEventClause = triggerEventClause;
            res._triggerTime = _triggerTime;
            res._whenExpr = whenExpr;
            return res;
        }

        private bool _isTemp;
        private bool _ifNotExists;
        private SQLiteTriggerTime _triggerTime;
        private bool _foreachClause;
        private SQLiteTriggerEventClause _triggerEventClause;
        private SQLiteObjectName _table;        
        private SQLiteExpression _whenExpr;
        private List<SQLiteStatement> _slist;
    }
}
