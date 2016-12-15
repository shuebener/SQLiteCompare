using System;
using System.Collections.Generic;
using System.Text;

namespace FastGridApp
{
    /// <summary>
    /// Represents a location in the grid (Row/Column)
    /// </summary>
    public struct FastGridLocation
    {
        public static readonly FastGridLocation Empty = new FastGridLocation(long.MinValue, int.MinValue);

        public FastGridLocation(long row, int col)
        {
            RowIndex = row;
            ColumnIndex = col;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (!(obj is FastGridLocation))
                return false;

            FastGridLocation loc = (FastGridLocation)obj;
            if (loc.RowIndex != RowIndex)
                return false;
            if (loc.ColumnIndex != ColumnIndex)
                return false;

            return true;
        }

        public static bool operator ==(FastGridLocation obj1, FastGridLocation obj2)
        {
            return obj1.Equals(obj2);
        }

        public static bool operator !=(FastGridLocation obj1, FastGridLocation obj2)
        {
            return !obj1.Equals(obj2);
        }

        public override int GetHashCode()
        {
            return RowIndex.GetHashCode() * ColumnIndex.GetHashCode();
        }

        public override string ToString()
        {
            return "Row=" + RowIndex + ",Col=" + ColumnIndex;
        }

        public long RowIndex;
        public int ColumnIndex;
    }
}
