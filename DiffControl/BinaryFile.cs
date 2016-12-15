using System;
using System.IO;
using System.Collections;

namespace DifferenceEngine
{
	public class DiffList_BinaryFile : IDiffList
	{
		private byte[] _byteList;

		public DiffList_BinaryFile(string fileName)
		{
			FileStream fs = null;
			BinaryReader br = null;
			try
			{
				fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
				int len = (int)fs.Length;
				br = new BinaryReader(fs);
				_byteList = br.ReadBytes(len);
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				if (br != null) br.Close();
				if (fs != null) fs.Close();
			}

		}
		#region IDiffList Members

		public int Count()
		{
			return _byteList.Length;
		}

		public IComparable GetByIndex(int index)
		{
			return _byteList[index];
		}

		#endregion
	}
}