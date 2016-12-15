using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class SQLiteTerm
    {
        private SQLiteTerm()
        {
        }

        public SQLiteTerm(string str)
        {
            _str = str;
        }

        public SQLiteTerm(double number)
        {
            _number = number;
        }

        public SQLiteTerm(SQLiteTimeFunction time)
        {
            _time = time;
        }

        public SQLiteTerm(byte[] blob)
        {
            _blob = blob;
        }

        public string AsString
        {
            get { return _str; }
            set { _str = value; }
        }

        public double? AsNumber
        {
            get { return _number; }
            set { _number = value; }
        }

        public SQLiteTimeFunction? AsTimeFunction
        {
            get { return _time; }
            set { _time = value; }
        }

        public byte[] AsBlob
        {
            get { return _blob; }
            set { _blob = value; }
        }

        public static byte[] CreateBlob(string blobLiteral)
        {
            int len = blobLiteral.Length - 3;
            byte[] res = new byte[len / 2];
            for (int i = 0; i < res.Length; i++)
            {
                char ch1 = blobLiteral[i * 2 + 2];
                char ch2 = blobLiteral[i * 2 + 3];
                string str = "" + ch1 + ch2;
                res[i] = byte.Parse(str, System.Globalization.NumberStyles.HexNumber);
            } // for
            return res;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            SQLiteTerm dst = obj as SQLiteTerm;
            if (dst == null)
                return false;

            if (_str != dst._str)
                return false;
            if (_number != dst._number)
                return false;
            if (_time != dst._time)
                return false;
            if (_blob == null && dst._blob != null ||
                _blob != null && dst._blob == null)
                return false;
            if (_blob == null && dst._blob == null)
                return true;
            if (_blob.Length != dst._blob.Length)
                return false;
            for (int i = 0; i < _blob.Length; i++)
            {
                if (_blob[i] != dst._blob[i])
                    return false;
            }

            return true;
        }

        public override string ToString()
        {
            if (_str != null)
                return _str;
            if (_number.HasValue)
                return _number.Value.ToString();
            if (_time.HasValue)
                return GetTimeFunctionName(_time.Value);
            if (_blob != null)
                return Utils.GetBlobLiteralString(_blob);
            throw new InvalidOperationException("The SQLiteTerm object is not initialized");
        }

        public virtual object Clone()
        {
            SQLiteTerm res = new SQLiteTerm();
            res._str = this._str;
            res._number = this._number;
            res._time = this._time;
            if (_blob != null)
            {
                res._blob = new byte[_blob.Length];
                for (int i = 0; i < _blob.Length; i++)
                    res._blob[i] = _blob[i];
            }
            return res;
        }

        private string GetTimeFunctionName(SQLiteTimeFunction func)
        {
            switch (func)
            {
                case SQLiteTimeFunction.CurrentTime:
                    return "CURRENT_TIME";
                case SQLiteTimeFunction.CurrentDate:
                    return "CURRENT_DATE";
                case SQLiteTimeFunction.CurrentTimestamp:
                    return "CURRENT_TIMESTAMP";
                default:
                    throw new ArgumentException("Illegal function enum value [" + func.ToString() + "]");
            } // switch
        }

        private string _str;
        private double? _number;
        private SQLiteTimeFunction? _time;
        private byte[] _blob;
    }

    public enum SQLiteTimeFunction
    {
        None = 0,

        CurrentTime = 1,

        CurrentDate = 2,

        CurrentTimestamp = 3,
    }
}
