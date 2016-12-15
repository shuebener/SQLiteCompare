using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteFromTable
    {
        private SQLiteFromTable()
        {
        }

        public SQLiteFromTable(SQLiteObjectName table, string asName, 
            SQLiteFromIndexed indexed, SQLiteExpression onExpr, List<string> usingOpt)
        {
            _table = table;
            AsName = asName;
            _indexed = indexed;
            _onExpr = onExpr;
            _usingOpt = usingOpt;
        }

        public SQLiteFromTable(SQLiteFromInternalTable itable, string asName,
            SQLiteExpression onExpr, List<string> usingOpt)
        {            
            _itable = itable;
            AsName = asName;
            _onExpr = onExpr;
            _usingOpt = usingOpt;
        }

        public SQLiteFromInternalTable InternalTable
        {
            get { return _itable; }
            set { _itable = value; }
        }

        public SQLiteObjectName TableName
        {
            get { return _table; }
            set { _table = value; }
        }

        public string AsName
        {
            get { return _asName; }
            set 
            { 
                _asName = Utils.QuoteIfNeeded(value); 
            }
        }

        public SQLiteFromIndexed Indexed
        {
            get { return _indexed; }
            set { _indexed = value; }
        }

        public SQLiteExpression OnExpression
        {
            get { return _onExpr; }
            set { _onExpr = value; }
        }

        public List<string> UsingOpt
        {
            get { return _usingOpt; }
            set { _usingOpt = value; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteFromTable dst = obj as SQLiteFromTable;
            if (dst == null)
                return false;

            if (_asName != dst._asName)
                return false;

            if (!RefCompare.CompareMany(_table, dst._table, _itable, dst._itable, _indexed, dst._indexed,
                _onExpr, dst._onExpr))
                return false;
            if (!RefCompare.CompareList<string>(_usingOpt, dst._usingOpt))
                return false;

            return true;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();            

            if (_itable != null)
            {
                // LP seltablist_paren RP as on_opt using_opt
                sb.Append(" ("+_itable.ToString()+") ");
            }
            else
            {
                // nm dbnm as indexed_opt on_opt using_opt            
                sb.Append(" " + _table.ToString());
            } // else

            if (_asName != null)
                sb.Append(" AS " + _asName);
            if (_indexed != null)
                sb.Append(" " + _indexed);
            if (_onExpr != null)
                sb.Append(" ON " + _onExpr.ToString());
            if (_usingOpt != null && _usingOpt.Count > 0)
            {
                sb.Append(" USING (");
                for (int i = 0; i < _usingOpt.Count; i++)
                {
                    sb.Append(_usingOpt[i]);
                    if (i < _usingOpt.Count - 1)
                        sb.Append(",");
                } // for
                sb.Append(")");
            }
            
            return sb.ToString();
        }

        public virtual object Clone()
        {
            SQLiteObjectName t = null;
            if (_table != null)
                t = (SQLiteObjectName)_table.Clone();
            SQLiteFromInternalTable fit = null;
            if (_itable != null)
                fit = (SQLiteFromInternalTable)_itable.Clone();
            SQLiteFromIndexed fi = null;
            if (_indexed != null)
                fi = (SQLiteFromIndexed)_indexed.Clone();
            SQLiteExpression expr = null;
            if (_onExpr != null)
                expr = (SQLiteExpression)_onExpr.Clone();
            List<string> us = null;
            if (_usingOpt != null)
            {
                us = new List<string>();
                foreach (string str in _usingOpt)
                    us.Add(str);
            }

            SQLiteFromTable res = new SQLiteFromTable();
            res._asName = _asName;
            res._indexed = fi;
            res._itable = fit;
            res._onExpr = expr;
            res._table = t;
            res._usingOpt = us;           
            return res;
        }

        private string _asName;
        private SQLiteObjectName _table;
        private SQLiteFromInternalTable _itable;
        private SQLiteFromIndexed _indexed;
        private SQLiteExpression _onExpr;
        private List<string> _usingOpt;        
    }
}
