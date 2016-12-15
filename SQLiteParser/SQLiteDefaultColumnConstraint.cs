using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteDefaultColumnConstraint : SQLiteColumnConstraint
    {
        #region Constructors
        private SQLiteDefaultColumnConstraint(string name)
            : base(name)
        {
        }

        public SQLiteDefaultColumnConstraint(string name, string id)
            : base(name)
        {
            this.Id = id;
        }

        public SQLiteDefaultColumnConstraint(string name, bool minus, SQLiteTerm term)
            : base(name)
        {
            this._minus = minus;
            this.Term = term;
        }

        public SQLiteDefaultColumnConstraint(string name, SQLiteExpression expr)
            : base(name)
        {
            _withParens = true;
            this.Expression = expr;
        }

        public SQLiteDefaultColumnConstraint(string name, SQLiteExpression expr, bool withParens)
            : base(name)
        {
            _withParens = withParens;
            this.Expression = expr;
        }
        #endregion

        #region Public Properties
        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
                _term = null;
                _expr = null;
            }
        }

        public SQLiteTerm Term
        {
            get { return _term; }
            set
            {
                _id = null;
                _expr = null;
                _term = value;
            }
        }

        public bool IsMinus
        {
            get { return _minus; }
            set { _minus = value; }
        }

        public SQLiteExpression Expression
        {
            get { return _expr; }
            set
            {
                _id = null;
                _term = null;
                _expr = value;
            }
        }

        public string ValueString
        {
            get
            {
                if (_id != null)
                    return _id;
                else if (_term != null)
                {
                    if (_minus)
                        return "-"+_term.ToString();
                    else
                        return _term.ToString();
                }
                else
                    return _expr.ToString();
            }
        }

        public bool WithParens
        {
            get { return _withParens; }
            set { _withParens = value; }
        }
        #endregion

        #region Public Overrided Methods
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteDefaultColumnConstraint dst = obj as SQLiteDefaultColumnConstraint;
            if (dst == null)
                return false;

            if (_id != dst._id)
                return false;
            if (_minus != dst._minus)
                return false;
            if (_withParens != dst._withParens)
                return false;
            if (!RefCompare.CompareMany(_term, dst._term, _expr, dst._expr))
                return false;

            return base.Equals(obj);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("DEFAULT ");
            if (_minus)
                sb.Append("-");
            if (_id != null)
                sb.Append(_id);
            else if (_term != null)
                sb.Append(_term.ToString());
            else
            {
                if (_withParens)
                    sb.Append("(" + _expr.ToString() + ")");
                else
                    sb.Append(_expr.ToString());
            }

            if (ConstraintName != null)
                return "CONSTRAINT " + ConstraintName + " " + sb.ToString();
            else
                return sb.ToString();
        }
        #endregion

        public override object Clone()
        {
            SQLiteTerm term = null;
            if (_term != null)
                term = (SQLiteTerm)_term.Clone();
            SQLiteExpression expr = null;
            if (_expr != null)
                expr = (SQLiteExpression)_expr.Clone();

            SQLiteDefaultColumnConstraint res = new SQLiteDefaultColumnConstraint(this.ConstraintName);
            res._expr = expr;
            res._id = _id;
            res._minus = _minus;
            res._term = term;
            res._withParens = _withParens;
            return res;
        }

        #region Private Variables
        private string _id;
        private bool _minus;
        private SQLiteTerm _term;
        private SQLiteExpression _expr;
        private bool _withParens;
        #endregion
    }
}

