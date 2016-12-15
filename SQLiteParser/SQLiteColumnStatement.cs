using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteColumnStatement : SQLiteDdlStatement
    {
        public SQLiteColumnStatement(SQLiteObjectName name)
            : base(name)
        {
        }

        public SQLiteColumnStatement(SQLiteObjectName name, SQLiteColumnType type, List<SQLiteColumnConstraint> conslist)
            : base(name)
        {
            _type = type;
            _conslist = conslist;
        }

        public SQLiteColumnType ColumnType
        {
            get { return _type; }
            set { _type = value; }
        }

        public List<SQLiteColumnConstraint> ColumnConstraints
        {
            get { return _conslist; }
            set { _conslist = value; }
        }

        public SQLiteDefaultColumnConstraint GetDefaultConstraint()
        {
            if (_conslist == null)
                return null;
            for (int i = 0; i < _conslist.Count; i++)
            {
                SQLiteDefaultColumnConstraint dc = _conslist[i] as SQLiteDefaultColumnConstraint;
                if (dc != null)
                    return dc;
            } // for
            return null;    
        }

        public bool HasNonNullConstDefault
        {
            get
            {
                if (_conslist == null)
                    return false;
                for (int i = 0; i < _conslist.Count; i++)
                {
                    SQLiteDefaultColumnConstraint dc = _conslist[i] as SQLiteDefaultColumnConstraint;
                    if (dc != null)
                    {
                        if (dc.Expression != null && dc.Expression.IsConstant(false))
                            return true;
                        else
                        {
                            // We don't allow using column names with a constant DEFAULT
                            if (dc.Id != null)
                                return false;

                            // Basically - terms that are not a time function (like CURRENT_DATE) are allowed
                            if (dc.Term != null)
                            {
                                if (dc.Term.AsTimeFunction.HasValue)
                                    return false;
                                else
                                    return true;
                            }
                        } // else
                    } // if
                } // for
                return false;
            } // get
        }

        public bool HasNonNullDefault
        {
            get
            {
                if (_conslist == null)
                    return false;
                for (int i = 0; i < _conslist.Count; i++)
                {
                    SQLiteDefaultColumnConstraint dc = _conslist[i] as SQLiteDefaultColumnConstraint;
                    if (dc != null)
                    {
                        if (dc.Expression != null && dc.Expression is SQLiteNullExpression)
                            return false;
                        else
                            return true;
                    } // if
                } // for
                return false;
            }
        }

        public bool IsNullable
        {
            get
            {
                if (_conslist != null)
                {
                    foreach (SQLiteColumnConstraint ccon in _conslist)
                    {
                        SQLiteNullColumnConstraint ncon = ccon as SQLiteNullColumnConstraint;
                        if (ncon != null)
                        {
                            if (ncon.IsNull)
                                return true;
                            else
                                return false;
                        }
                    } // foreach
                }

                // If no NULL / NOT NULL constraint was specified - the default behavior
                // is for the column to be NULLable.
                return true;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteColumnStatement dst = obj as SQLiteColumnStatement;
            if (dst == null)
                return false;

            if (!RefCompare.Compare(_type, dst._type))
                return false;
            if (!RefCompare.CompareList<SQLiteColumnConstraint>(_conslist, dst._conslist))
                return false;

            return base.Equals(obj);
        }

        public override string ToString()
        {
            StringBuilder clist = new StringBuilder();

            if (_conslist != null)
            {
                clist.Append(" ");
                for (int i = 0; i < _conslist.Count; i++)
                {
                    SQLiteColumnConstraint cc = _conslist[i];
                    clist.Append(cc.ToString());
                    if (i < _conslist.Count - 1)
                        clist.Append(" ");
                } // foreach
            }

            if (_type.ToString().Length > 0)
                return base.ObjectName.ToString() + " " + _type.ToString() + clist.ToString();
            else if (clist.ToString().Length > 0)
                return base.ObjectName.ToString() + clist.ToString();
            else
                return base.ObjectName.ToString();
        }

        public override object Clone()
        {
            SQLiteColumnType type = null;
            if (_type != null)
                type = (SQLiteColumnType)_type.Clone();
            List<SQLiteColumnConstraint> conslist = null;
            if (_conslist != null)
            {
                conslist = new List<SQLiteColumnConstraint>();
                foreach (SQLiteColumnConstraint cc in _conslist)
                    conslist.Add((SQLiteColumnConstraint)cc.Clone());
            }

            SQLiteObjectName name = null;
            if (this.ObjectName != null)
                name = (SQLiteObjectName)this.ObjectName.Clone();

            SQLiteColumnStatement res = new SQLiteColumnStatement(name);
            res._conslist = conslist;
            res._type = type;
            return res;
        }

        private SQLiteColumnType _type;
        private List<SQLiteColumnConstraint> _conslist;
    }
}
