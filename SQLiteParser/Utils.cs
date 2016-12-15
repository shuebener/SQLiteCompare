using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class Utils
    {
        static Utils()
        {
            string letters = "abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ_";
            foreach (char ch in letters)
                _validLetters.Add(ch, ch);
            foreach (string w in _words)
                _keywords.Add(w, w);
        }

        /// <summary>
        /// Format a BLOB string literal from the specified bytes array
        /// </summary>
        /// <param name="blob">The BLOB to format as literal</param>
        /// <returns></returns>
        public static string GetBlobLiteralString(byte[] blob)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("X'");
            for (int i = 0; i < blob.Length; i++)
            {
                sb.Append(string.Format("{0:X2}", blob[i]));
            } // for
            sb.Append("'");
            return sb.ToString();
        }

        /// <summary>
        /// Quote a literal string
        /// </summary>
        /// <param name="literal">The literal string to quote</param>
        /// <returns>The quoted literal string</returns>
        public static string QuoteLiteralString(string literal)
        {
            return "'" + literal.Replace("'", "''") + "'";
        }

        /// <summary>
        /// Format the specified date time value as a valid SQLite time literal
        /// </summary>
        /// <param name="dt">The date-time value to format</param>
        /// <returns></returns>
        public static string GetDateTimeLiteralString(DateTime dt, bool withMilliseconds)
        {
            // YYYY-MM-DD HH:MM:SS.SSS
            string ret = dt.Year.ToString().PadLeft(4, '0') + "-" +
                   dt.Month.ToString().PadLeft(2, '0') + "-" +
                   dt.Day.ToString().PadLeft(2, '0') + " " +
                   dt.Hour.ToString().PadLeft(2, '0') + ":" +
                   dt.Minute.ToString().PadLeft(2, '0') + ":" +
                   dt.Second.ToString().PadLeft(2, '0');
            if (withMilliseconds && dt.Millisecond > 0)
                ret += "." + dt.Millisecond.ToString();
            return ret;
        }

        /// <summary>
        /// Quotes the name of the object with brackets
        /// </summary>
        /// <param name="objName">Name of the object</param>
        /// <returns></returns>
        public static string QuoteObjectName(string objName)
        {
            if (objName == null)
                return null;
            objName = Chop(objName);
            objName = objName.Replace("[", "\\[");
            objName = objName.Replace("]", "\\]");
            return "[" + objName + "]";
        }

        /// <summary>
        /// Quote the specified object name but only if needed according to the
        /// contents of the object name
        /// </summary>
        /// <param name="objName">The object name to quote</param>
        /// <returns></returns>
        public static string QuoteIfNeeded(string objName)
        {
            if (objName == null)
                return null;

            objName = Utils.Chop(objName);
            if (Utils.NeedsQuoting(objName))
                return Utils.QuoteObjectName(objName);
            return objName;
        }

        /// <summary>
        /// Same as QuoteIfNeeded above but handles string arrays
        /// </summary>
        /// <param name="values">Array of strings to quote</param>
        /// <returns></returns>
        public static string[] QuoteIfNeeded(string[] values)
        {
            if (values == null)
                return null;
            string[] res = new string[values.Length];
            for (int i = 0; i < values.Length; i++)
                res[i] = Utils.QuoteIfNeeded(values[i]);
            return res;
        }

        /// <summary>
        /// Checks the specified string in order to check if it needs quoting.
        /// </summary>
        /// <param name="value">The value to check</param>
        /// <returns>TRUE if it needs quoting, false otherwise</returns>
        public static bool NeedsQuoting(string value)
        {
            if (value == null)
                return false;
            if (value.Length < 2)
                return false;

            for (int i = 0; i < value.Length; i++)
            {
                if (!_validLetters.ContainsKey(value[i]))
                    return true;
            }

            if (Char.IsDigit(value[0]))
                return true;

            // If the string is one of several reserved keywords - it also needs quoting
            string keyword = value.ToLower();
            if (_keywords.ContainsKey(keyword))
                return true;

            return false;
        }

        /// <summary>
        /// Chop delimiters from object names
        /// </summary>
        /// <param name="value">the name to chop</param>
        /// <returns></returns>
        public static string Chop(string value)
        {
            if (value == null)
                return null;
            if (value.Length == 0)
                return string.Empty;
            if (value.Length == 1)
                return value;
            int last = value.Length - 1;
            if ((value[0] == '[' || value[0] == '\"' || value[0] == '\'' || value[0] == '`') &&
                (value[0] == '[' && value[last] == ']' ||
                 value[0] == value[last]))
                return value.Substring(1, value.Length - 2);
            else
                return value;
        }

        /// <summary>
        /// Chop delimiters from a list of names
        /// </summary>
        /// <param name="list">The list of names to chop</param>
        /// <returns></returns>
        public static string[] Chop(string[] list)
        {
            string[] res = new string[list.Length];
            for (int i = 0; i < list.Length; i++)
            {
                res[i] = Chop(list[i]);
            }

            return res;
        }

        public static string GetConflictClauseString(SQLiteResolveAction conf)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("ON CONFLICT ");
            switch (conf)
            {
                case SQLiteResolveAction.Ignore:
                    sb.Append("IGNORE");
                    break;
                case SQLiteResolveAction.Replace:
                    sb.Append("REPLACE");
                    break;
                case SQLiteResolveAction.Rollback:
                    sb.Append("ROLLBACK");
                    break;
                case SQLiteResolveAction.Abort:
                    sb.Append("ABORT");
                    break;
                case SQLiteResolveAction.Fail:
                    sb.Append("FAIL");
                    break;
                default:
                    throw new ArgumentException("Illegal resolve action [" + conf.ToString() + "]");
            } // switch

            return sb.ToString();
        }

        public static string GetReferenceTriggerString(SQLiteReferenceTrigger trigger)
        {
            switch (trigger)
            {
                case SQLiteReferenceTrigger.OnDelete:
                    return "DELETE";
                case SQLiteReferenceTrigger.OnInsert:
                    return "INSERT";
                case SQLiteReferenceTrigger.OnUpdate:
                    return "UPDATE";
                case SQLiteReferenceTrigger.Match:
                    return "MATCH";
                default:
                    throw new ArgumentException("illegal trigger type [" + trigger.ToString() + "]");
            } // switch
        }

        public static string GetOperatorString(SQLiteOperator op)
        {
            switch (op)
            {
                case SQLiteOperator.And:
                    return "AND";
                case SQLiteOperator.Or:
                    return "OR";
                case SQLiteOperator.Lt:
                    return "<";
                case SQLiteOperator.Gt:
                    return ">";
                case SQLiteOperator.Ge:
                    return ">=";
                case SQLiteOperator.Le:
                    return "<=";
                case SQLiteOperator.Eq:
                    return "=";
                case SQLiteOperator.Ne:
                    return "<>";
                case SQLiteOperator.BitAnd:
                    return "&";
                case SQLiteOperator.BitOr:
                    return "|";
                case SQLiteOperator.Lshift:
                    return "<<";
                case SQLiteOperator.Rshift:
                    return ">>";
                case SQLiteOperator.Plus:
                    return "+";
                case SQLiteOperator.Minus:
                    return "-";
                case SQLiteOperator.Star:
                    return "*";
                case SQLiteOperator.Slash:
                    return "/";
                case SQLiteOperator.Rem:
                    return "%";
                case SQLiteOperator.Concat:
                    return "||";
                case SQLiteOperator.IsNull:
                    return "ISNULL";
                case SQLiteOperator.NotNull:
                    return "NOTNULL";
                case SQLiteOperator.Is_Null:
                    return "IS NULL";
                case SQLiteOperator.Not_Null:
                    return "NOT NULL";
                case SQLiteOperator.Is_Not_Null:
                    return "IS NOT NULL";
                case SQLiteOperator.Not:
                    return "NOT";
                case SQLiteOperator.BitNot:
                    return "~";
                default:
                    throw new ArgumentException("Illegal sqlite operator [" + op.ToString() + "]");
            } // switch
        }

        public static string GetTriggerTimeString(SQLiteTriggerTime triggerTime)
        {
            switch (triggerTime)
            {
                case SQLiteTriggerTime.Before:
                    return "BEFORE";
                case SQLiteTriggerTime.After:
                    return "AFTER";
                case SQLiteTriggerTime.InsteadOf:
                    return "INSTEAD OF";
                default:
                    return string.Empty;
            } // switch
        }

        public static string GetJoinOperatorString(SQLiteJoinOperator join)
        {
            StringBuilder sb = new StringBuilder();
            if ((join & SQLiteJoinOperator.Natural) != 0)
                sb.Append("NATURAL");
            if ((join & SQLiteJoinOperator.Inner) != 0)
                sb.Append(" INNER");
            if ((join & SQLiteJoinOperator.Cross) != 0)
                sb.Append(" CROSS");
            if ((join & SQLiteJoinOperator.Left) != 0)
                sb.Append(" LEFT");
            if ((join & SQLiteJoinOperator.Outer) != 0)
                sb.Append(" OUTER");

            if (sb.Length > 0)
            {
                sb.Append(" JOIN");
                return sb.ToString();
            }

            return ",";
        }

        public static string GetResolveActionString(SQLiteResolveAction action)
        {
            switch (action)
            {
                case SQLiteResolveAction.Ignore:
                    return "IGNORE";
                case SQLiteResolveAction.Replace:
                    return "REPLACE";
                case SQLiteResolveAction.Rollback:
                    return "ROLLBACK";
                case SQLiteResolveAction.Abort:
                    return "ABORT";
                case SQLiteResolveAction.Fail:
                    return "FAIL";
                default:
                    throw new ArgumentException("illegal action type [" + action.ToString() + "]");
            } // switch
        }

        public static string GetSelectOperatorString(SQLiteSelectOperator op)
        {
            switch (op)
            {
                case SQLiteSelectOperator.Union:
                    return "UNION";
                case SQLiteSelectOperator.UnionAll:
                    return "UNION ALL";
                case SQLiteSelectOperator.Except:
                    return "EXCEPT";
                case SQLiteSelectOperator.Intersect:
                    return "INTERSECT";
                default:
                    throw new ArgumentException("illegal select operator [" + op.ToString() + "]");
            } // switch
        }

        public static string FormatTriggerStatementsList(int tab, List<SQLiteStatement> list)
        {
            StringBuilder tabstr = new StringBuilder();
            for (int i = 0; i < tab; i++)
                tabstr.Append(" ");
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                SQLiteStatement stmt = list[i];
                string stmtstr = stmt.ToString();
                stmtstr = stmtstr.Replace("\r\n", "\r\n" + tabstr);
                sb.Append(tabstr.ToString() + stmtstr + ";\r\n");
            } // for
            return sb.ToString();
        }

        private static Dictionary<char, char> _validLetters = new Dictionary<char, char>();
        private static Dictionary<string, string> _keywords = new Dictionary<string, string>();

        private static string[] _words = new string[]
            {
                "create",
                "unique",
                "index",
                "if",
                "not",
                "exists",
                "on",
                "collate",
                "asc",
                "desc",
                "table",
                "primary",
                "key",
                "conflict",
                "autoincrement",
                "constraint",
                "rollback",
                "abort",
                "fail",
                "ignore",
                "replace",
                "as",
                "like",
                "glob",
                "regexp",
                "isnull",
                "notnull",
                "between",
                "match",
                "is",
                "escape",
                "in",
                "natural",
                "outer",
                "cross",
                "inner",
                "left",
                "default",
                "null",
                "references",
                "deferrable",
                "cascade",
                "restrict",
                "initially",
                "deferred",
                "immediate",
                "foreign",
                "or",
                "cast",
                "case",
                "raise",
                "begin",
                "end",
                "when",
                "then",
                "trigger",
                "before",
                "after",
                "instead",
                "of",
                "limit",
                "offset",
                "delete",
                "insert",
                "update",
                "for",
                "each",
                "row",
                "set",
                "else",
                "values",
                "into",
                "from",
                "union",
                "all",
                "except",
                "intersect",
                "distinct",
                "join",
                "indexed",
                "by",
                "using",
                "group",
                "order",
                "having",
                "check",
                "view",
                "where",
                "temp",
                "and",
                "temporary"
            };
    }
}
