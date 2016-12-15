using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteSingleSelectStatement : SQLiteSelectStatement
    {
        private SQLiteSingleSelectStatement()
        {
        }

        public SQLiteSingleSelectStatement(SQLiteDistinct distinct, List<SQLiteSelectColumn> columns, 
            SQLiteFromClause from, SQLiteExpression whereExpr, List<SQLiteExpression> groupBy,
            SQLiteExpression having, List<SQLiteSortItem> orderBy, SQLiteLimitClause limit)
        {
            _distinct = distinct;
            _columns = columns;
            _from = from;
            _whereExpr = whereExpr;
            _groupBy = groupBy;
            _having = having;
            _orderBy = orderBy;
            _limit = limit;
        }

        public SQLiteDistinct Distinct
        {
            get { return _distinct; }
            set { _distinct = value; }
        }

        public List<SQLiteSelectColumn> Columns
        {
            get { return _columns; }
            set { _columns = value; }
        }

        public SQLiteFromClause FromClause
        {
            get { return _from; }
            set { _from = value; }
        }

        public SQLiteExpression WhereExpression
        {
            get { return _whereExpr; }
            set { _whereExpr = value; }
        }

        public List<SQLiteExpression> GroupBy
        {
            get { return _groupBy; }
            set { _groupBy = value; }
        }

        public SQLiteExpression HavingExpression
        {
            get { return _having; }
            set { _having = value; }
        }

        public List<SQLiteSortItem> OrderBy
        {
            get { return _orderBy; }
            set { _orderBy = value; }
        }

        public SQLiteLimitClause LimitClause
        {
            get { return _limit; }
            set { _limit = value; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteSingleSelectStatement dst = obj as SQLiteSingleSelectStatement;
            if (dst == null)
                return false;

            if (_distinct != dst._distinct)
                return false;

            if (!RefCompare.CompareMany(_from, dst._from, _whereExpr, dst._whereExpr, _having, dst._having,
                _limit, dst._limit))
                return false;
            if (!RefCompare.CompareList<SQLiteSelectColumn>(_columns, dst._columns))
                return false;
            if (!RefCompare.CompareList<SQLiteExpression>(_groupBy, dst._groupBy))
                return false;
            if (!RefCompare.CompareList<SQLiteSortItem>(_orderBy, dst._orderBy))
                return false;

            return base.Equals(obj);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            // SELECT distinct selcollist from where_opt groupby_opt having_opt orderby_opt limit_opt

            sb.Append("SELECT");
            if (_distinct == SQLiteDistinct.Distinct)
                sb.Append(" DISTINCT");
            else if (_distinct == SQLiteDistinct.All)
                sb.Append(" ALL");

            sb.Append(" ");
            for (int i = 0; i < _columns.Count; i++)
            {
                sb.Append(_columns[i].ToString());
                if (i < _columns.Count - 1)
                    sb.Append(",");
            } // for

            if (_from != null)
                sb.Append("\r\n    FROM " + _from.ToString());

            if (_whereExpr != null)
                sb.Append("\r\n    WHERE " + _whereExpr.ToString());

            if (_groupBy != null)
            {
                sb.Append("\r\n    GROUP BY ");
                for (int i = 0; i < _groupBy.Count; i++)
                {
                    sb.Append(_groupBy[i].ToString());
                    if (i < _groupBy.Count - 1)
                        sb.Append(",");
                } // for
            }

            if (_having != null)
                sb.Append("\r\n    HAVING " + _having.ToString());

            if (_orderBy != null)
            {
                sb.Append("\r\n    ORDER BY ");
                for (int i = 0; i < _orderBy.Count; i++)
                {
                    sb.Append(_orderBy[i].ToString());
                    if (i < _orderBy.Count - 1)
                        sb.Append(",");
                } // for
            } // if

            if (_limit != null)
                sb.Append("\r\n    " + _limit.ToString());

            return sb.ToString();
        }

        public override object Clone()
        {
            SQLiteFromClause from = null;
            if (_from != null)
                from = (SQLiteFromClause)_from.Clone();
            SQLiteExpression whereExpr = null;
            if (_whereExpr != null)
                whereExpr = (SQLiteExpression)_whereExpr.Clone();
            SQLiteExpression having = null;
            if (_having != null)
                having = (SQLiteExpression)_having.Clone();
            SQLiteLimitClause limit = null;
            if (_limit != null)
                limit = (SQLiteLimitClause)_limit.Clone();
            List<SQLiteSelectColumn> columns = null;
            if (_columns != null)
            {
                columns = new List<SQLiteSelectColumn>();
                foreach (SQLiteSelectColumn c in _columns)
                    columns.Add((SQLiteSelectColumn)c.Clone());
            }
            List<SQLiteExpression> groupBy = null;
            if (_groupBy != null)
            {
                groupBy = new List<SQLiteExpression>();
                foreach (SQLiteExpression e in _groupBy)
                    groupBy.Add((SQLiteExpression)e.Clone());
            }
            List<SQLiteSortItem> orderBy = null;
            if (_orderBy != null)
            {
                orderBy = new List<SQLiteSortItem>();
                foreach (SQLiteSortItem i in _orderBy)
                    orderBy.Add((SQLiteSortItem)i.Clone());
            }

            SQLiteSingleSelectStatement res = new SQLiteSingleSelectStatement();
            res._distinct = _distinct;
            res._from = from;
            res._whereExpr = whereExpr;
            res._having = having;
            res._limit = limit;
            res._columns = columns;
            res._groupBy = groupBy;
            res._orderBy = orderBy;
            return res;
        }

        private SQLiteDistinct _distinct;        
        private SQLiteFromClause _from;
        private SQLiteExpression _whereExpr;
        private SQLiteExpression _having;
        private SQLiteLimitClause _limit;
        private List<SQLiteSelectColumn> _columns;
        private List<SQLiteExpression> _groupBy;
        private List<SQLiteSortItem> _orderBy;
    }
}
