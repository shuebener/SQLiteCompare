using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteFromClause
    {
        public SQLiteFromClause AddJoin(SQLiteJoinOperator join)
        {
            _tables.Add(join);
            return this;
        }

        public SQLiteFromClause AddTable(SQLiteObjectName tableName, string asName, 
            SQLiteFromIndexed indexed, SQLiteExpression onExpr, List<string> usingOpt)
        {
            _tables.Add(new SQLiteFromTable(tableName, asName, indexed, onExpr, usingOpt));
            return this;
        }

        public SQLiteFromClause AddInternalTable(SQLiteFromInternalTable itable, string asName,
            SQLiteExpression onExpr, List<string> usingOpt)
        {
            _tables.Add(new SQLiteFromTable(itable, asName, onExpr, usingOpt));
            return this;
        }

        public List<object> FromTables
        {
            get { return _tables; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteFromClause dst = obj as SQLiteFromClause;
            if (dst == null)
                return false;

            if (!RefCompare.CompareList<object>(_tables, dst._tables))
                return false;

            return true;
        }

        public override string ToString()
        {
            if (_tables == null || _tables.Count == 0)
                return string.Empty;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _tables.Count; i++)
            {
                if (_tables[i] is SQLiteFromTable)
                    sb.Append(_tables[i].ToString());
                else
                    sb.Append(Utils.GetJoinOperatorString((SQLiteJoinOperator)_tables[i]));
                if (i < _tables.Count - 1)
                    sb.Append(" ");
            } // for
            return sb.ToString();
        }

        public virtual object Clone()
        {
            List<object> tlist = null;
            if (_tables != null)
            {
                tlist = new List<object>();
                foreach (object obj in _tables)
                {
                    if (obj is SQLiteFromTable)
                    {
                        SQLiteFromTable ft = (SQLiteFromTable)obj;
                        tlist.Add(ft.Clone());
                    }
                    else if (obj is SQLiteJoinOperator)
                        tlist.Add(obj);
                } // foreach
            }

            SQLiteFromClause res = new SQLiteFromClause();
            res._tables = tlist;
            return res;
        }

        private List<object> _tables = new List<object>();
    }
}
