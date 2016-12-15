using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteFunctionExpression : SQLiteExpression
    {
        private SQLiteFunctionExpression()
        {
        }

        public SQLiteFunctionExpression(string id, SQLiteDistinct distinct, List<SQLiteExpression> exprlist)
        {
            _id = id;
            _distinct = distinct;
            _exprlist = exprlist;
        }

        public SQLiteFunctionExpression(string id)
        {
            _exprlist = null;
            _id = id;
            _distinct = SQLiteDistinct.None;
        }

        public bool IsCatchAll
        {
            get { return _exprlist == null; }
        }

        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public SQLiteDistinct Distinct
        {
            get { return _distinct; }
            set { _distinct = value; }
        }

        public List<SQLiteExpression> ExpressionsList
        {
            get { return _exprlist; }
            set { _exprlist = value; }
        }

        public override bool IsConstant(bool allowNull)
        {
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteFunctionExpression dst = obj as SQLiteFunctionExpression;
            if (dst == null)
                return false;

            if (_id != dst._id || _distinct != dst._distinct)
                return false;

            if (!RefCompare.CompareList<SQLiteExpression>(_exprlist, dst._exprlist))
                return false;

            return base.Equals(obj);
        }

        public override string ToString()
        {
            string res;
            if (IsCatchAll)
                res = _id + "(*)";
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(_id + "(");
                if (_distinct == SQLiteDistinct.All)
                    sb.Append("ALL ");
                else if (_distinct == SQLiteDistinct.Distinct)
                    sb.Append("DISTINCT ");                    
                for (int i = 0; i < _exprlist.Count; i++)
                {
                    sb.Append(_exprlist[i].ToString());
                    if (i < _exprlist.Count - 1)
                        sb.Append(",");
                } // for
                sb.Append(")");

                res = sb.ToString();
            } // else

            return res;
        }

        public override object Clone()
        {
            List<SQLiteExpression> elist = null;
            if (_exprlist != null)
            {
                elist = new List<SQLiteExpression>();
                foreach (SQLiteExpression e in _exprlist)
                    elist.Add((SQLiteExpression)e.Clone());
            }

            SQLiteFunctionExpression res = new SQLiteFunctionExpression();
            res._id = _id;
            res._distinct = _distinct;
            res._exprlist = elist;
            return res;
        }

        private string _id;
        private SQLiteDistinct _distinct;
        private List<SQLiteExpression> _exprlist;
    }
}
