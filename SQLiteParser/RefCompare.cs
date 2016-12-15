using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteParser
{
    public class RefCompare
    {
        public static bool Compare(object t1, object t2)
        {
            if (t1 == null && t2 != null || t1 != null && t2 == null)
                return false;
            if (t1 == null && t2 == null)
                return true;
            if (!t1.Equals(t2))
                return false;
            return true;
        }

        public static bool CompareMany(params object[] args)
        {
            if (args == null || args.Length % 2 != 0)
                throw new ArgumentException();

            for (int i = 0; i < args.Length; )
            {
                if (!Compare(args[i], args[i + 1]))
                    return false;
                i += 2;
            } // for

            return true;
        }

        public static bool CompareList<I>(List<I> i1, List<I> i2) where I : class
        {
            if (i1 == null && i2 != null || i1 != null && i2 == null)
                return false;
            if (i1 == null && i2 == null)
                return true;
            if (i1.Count != i2.Count)
                return false;
            for (int i = 0; i < i1.Count; i++)
            {
                if (i1[i] == null && i2[i] != null)
                    return false;
                if (i1[i] != null && i2[i] == null)
                    return false;
                if (i1[i] != null && i2[i] != null)
                {
                    if (!i1[i].Equals(i2[i]))
                        return false;
                }
            } // for

            return true;
        }
    }
}
