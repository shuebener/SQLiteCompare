using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteInExpression : SQLiteExpression
    {
        private SQLiteInExpression()
        {
        }

        public SQLiteInExpression(SQLiteExpression expr, bool isIn, List<SQLiteExpression> exprlist)
        {
            _expr = expr;
            _isIn = isIn;
            _exprlist = exprlist;
        }

        public SQLiteInExpression(SQLiteExpression expr, bool isIn, SQLiteSelectStatement select)
        {
            _expr = expr;
            _isIn = isIn;
            _select = select;
        }

        public SQLiteInExpression(SQLiteExpression expr, bool isIn, SQLiteObjectName table)
        {
            _expr = expr;
            _isIn = isIn;
            _table = table;
        }

        public SQLiteExpression Expression
        {
            get { return _expr; }
            set { _expr = value; }
        }

        public bool In
        {
            get { return _isIn; }
            set { _isIn = value; }
        }

        public List<SQLiteExpression> ExpressionsList
        {
            get { return _exprlist; }
            set { _exprlist = value; }
        }

        public SQLiteSelectStatement SelectStatement
        {
            get { return _select; }
            set { _select = value; }
        }

        public SQLiteObjectName TableName
        {
            get { return _table; }
            set { _table = value; }
        }

        public override bool IsConstant(bool allowNull)
        {
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteInExpression dst = obj as SQLiteInExpression;
            if (dst == null)
                return false;

            if (_isIn != dst._isIn)
                return false;

            if (!RefCompare.CompareMany(_expr, dst._expr, _select, dst._select, _table, dst._table))
                return false;
            if (!RefCompare.CompareList<SQLiteExpression>(_exprlist, dst._exprlist))
                return false;

            return base.Equals(obj);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(_expr.ToString());

            if (_isIn)
                sb.Append(" IN ");
            else
                sb.Append(" NOT IN ");

            if (_exprlist != null)
            {
                // expr in_op LP exprlist RP

                sb.Append("(");
                for (int i = 0; i < _exprlist.Count; i++)
                {
                    sb.Append(_exprlist[i].ToString());
                    if (i < _exprlist.Count - 1)
                        sb.Append(",");
                } // for
                sb.Append(")");
            } // if
            else if (_select != null)
            {
                // expr in_op LP select RP

                sb.Append("(");
                sb.Append(_select.ToString());
                sb.Append(")");
            } // else
            else if (_table != null)
            {
                // expr in_op nm dbnm

                sb.Append(_table.ToString());
            } // else
            else
                throw new ArgumentException("illegal SQLiteInExpression instance");

            return sb.ToString();
        }

        public override object Clone()
        {
            SQLiteExpression expr = null;
            if (_expr != null)
                expr = (SQLiteExpression)_expr.Clone();
            SQLiteSelectStatement select = null;
            if (_select != null)
                select = (SQLiteSelectStatement)_select.Clone();
            SQLiteObjectName table = null;
            if (_table != null)
                table = (SQLiteObjectName)_table.Clone();
            List<SQLiteExpression> exprlist = null;
            if (_exprlist != null)
            {
                exprlist = new List<SQLiteExpression>();
                foreach (SQLiteExpression e in _exprlist)
                    exprlist.Add((SQLiteExpression)e.Clone());
            }

            SQLiteInExpression res = new SQLiteInExpression();
            res._expr = expr;
            res._exprlist = exprlist;
            res._isIn = _isIn;
            res._select = select;
            res._table = table;
            return res;
        }

        private bool _isIn;
        private SQLiteExpression _expr;
        private SQLiteSelectStatement _select;
        private SQLiteObjectName _table;
        private List<SQLiteExpression> _exprlist;
    }
}
