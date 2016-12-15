using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteInsertStatement : SQLiteStatement
    {
        private SQLiteInsertStatement()
        {
        }

        public SQLiteInsertStatement(SQLiteInsertPrefix prefix, string table, List<string> columns, SQLiteSelectStatement select)
        {
            _prefix = prefix;
            TableName = table;
            Columns = columns;
            _select = select;
        }

        public SQLiteInsertStatement(SQLiteInsertPrefix prefix, string table, List<string> columns,
            List<SQLiteExpression> values)
        {
            _prefix = prefix;
            TableName = table;
            Columns = columns;
            _values = values;
        }

        public SQLiteInsertPrefix Prefix
        {
            get { return _prefix; }
            set { _prefix = value; }
        }

        public string TableName
        {
            get { return _table; }
            set 
            {
                _table = Utils.QuoteIfNeeded(value);
            }
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
                else
                    _columns = null;
            }
        }

        public SQLiteSelectStatement SelectStatement
        {
            get { return _select; }
            set { _select = value; }
        }

        public List<SQLiteExpression> ValuesList
        {
            get { return _values; }
            set { _values = value; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteInsertStatement dst = obj as SQLiteInsertStatement;
            if (dst == null)
                return false;

            if (_table != dst._table)
                return false;

            if (!RefCompare.CompareMany(_prefix, dst._prefix, _select, dst._select))
                return false;
            if (!RefCompare.CompareList<string>(_columns, dst._columns))
                return false;
            if (!RefCompare.CompareList<SQLiteExpression>(_values, dst._values))
                return false;               

            return base.Equals(obj);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            // insert_cmd INTO nm inscollist_opt VALUES LP itemlist RP
            // insert_cmd INTO nm inscollist_opt select

            if (_prefix.IsInsert)
            {
                sb.Append("INSERT");
                if (_prefix.ResolveAction != SQLiteResolveAction.None)
                    sb.Append(" OR " + Utils.GetResolveActionString(_prefix.ResolveAction));
            }
            else if (_prefix.IsReplace)
                sb.Append("REPLACE");
            else
                throw new ArgumentException("illegal INSERT prefix");

            sb.Append(" INTO " + _table);
            if (_columns != null && _columns.Count > 0)
            {
                sb.Append(" (");
                for (int i = 0; i < _columns.Count; i++)
                {
                    sb.Append(_columns[i]);
                    if (i < _columns.Count - 1)
                        sb.Append(",");
                } // for
                sb.Append(")");
            }

            if (_values != null && _values.Count > 0)
            {
                sb.Append("\r\n    VALUES (");
                for (int i = 0; i < _values.Count; i++)
                {
                    sb.Append(_values[i].ToString());
                    if (i < _values.Count - 1)
                        sb.Append(",");
                } // for
                sb.Append(")");
            } // if
            else if (_select != null)
            {
                string select = _select.ToString();
                select = select.Replace("\r\n", "\r\n    ");
                sb.Append("\r\n    " + select);
            }

            return sb.ToString();
        }

        public override object Clone()
        {
            SQLiteInsertPrefix prefix = null;
            if (_prefix != null)
                prefix = (SQLiteInsertPrefix)_prefix.Clone();
            SQLiteSelectStatement select = null;
            if (_select != null)
                select = (SQLiteSelectStatement)_select.Clone();
            List<string> columns = null;
            if (_columns != null)
            {
                columns = new List<string>();
                foreach (string str in _columns)
                    columns.Add(str);
            }
            List<SQLiteExpression> values = null;
            if (_values != null)
            {
                values = new List<SQLiteExpression>();
                foreach (SQLiteExpression expr in _values)
                {
                    values.Add((SQLiteExpression)expr.Clone());
                } // foreach
            }

            SQLiteInsertStatement res = new SQLiteInsertStatement();
            res._table = _table;
            res._prefix = prefix;
            res._select = select;
            res._columns = columns;
            res._values = values;
            return res;
        }

        private string _table;
        private SQLiteInsertPrefix _prefix;
        private SQLiteSelectStatement _select;
        private List<string> _columns;
        private List<SQLiteExpression> _values;
    }
}

