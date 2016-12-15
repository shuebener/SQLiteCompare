using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteCaseExpression : SQLiteExpression
    {
        private SQLiteCaseExpression()
        {
        }

        public SQLiteCaseExpression(SQLiteExpression caseOperand, List<SQLiteCaseItem> caseItems, SQLiteExpression caseElse)
        {
            _caseOperand = caseOperand;
            _caseItems = caseItems;
            _caseElse = caseElse;
        }

        public SQLiteExpression CaseOperand
        {
            get { return _caseOperand; }
            set { _caseOperand = value; }
        }

        public List<SQLiteCaseItem> CaseItems
        {
            get { return _caseItems; }
            set { _caseItems = value; }
        }

        public SQLiteExpression CaseElse
        {
            get { return _caseElse; }
            set { _caseElse = value; }
        }

        public override bool IsConstant(bool allowNull)
        {
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteCaseExpression dst = obj as SQLiteCaseExpression;
            if (dst == null)
                return false;

            if (!RefCompare.CompareMany(_caseOperand, dst._caseOperand, _caseElse, dst._caseElse))
                return false;
            if (!RefCompare.CompareList<SQLiteCaseItem>(_caseItems, dst._caseItems))
                return false;

            return base.Equals(obj);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("CASE");
            if (_caseOperand != null)
                sb.Append(" " + _caseOperand.ToString());
            if (_caseItems != null)
            {
                sb.Append(" ");
                for (int i = 0; i < _caseItems.Count; i++)
                {
                    sb.Append(_caseItems[i].ToString());
                    if (i < _caseItems.Count - 1)
                        sb.Append(" ");
                } // for
            } // if
            if (_caseElse != null)
                sb.Append(" ELSE " + _caseElse.ToString());
            sb.Append(" END");

            return sb.ToString();
        }

        public override object Clone()
        {
            SQLiteExpression caseOperand = null;
            if (_caseOperand != null)
                caseOperand = (SQLiteExpression)_caseOperand.Clone();
            List<SQLiteCaseItem> caseItems = null;
            if (_caseItems != null)
            {
                caseItems = new List<SQLiteCaseItem>();
                foreach (SQLiteCaseItem ci in _caseItems)
                    caseItems.Add((SQLiteCaseItem)ci.Clone());
            }
            SQLiteExpression caseElse = null;
            if (_caseElse != null)
                caseElse = (SQLiteExpression)_caseElse.Clone();

            SQLiteCaseExpression res = new SQLiteCaseExpression();
            res._caseElse = caseElse;
            res._caseItems = caseItems;
            res._caseOperand = caseOperand;
            return res;
        }

        private SQLiteExpression _caseOperand;
        private List<SQLiteCaseItem> _caseItems;
        private SQLiteExpression _caseElse;
    }
}
