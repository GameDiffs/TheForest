using System;

namespace ICSharpCode.SharpZipLib.Zip
{
	public class RawTaggedData : ITaggedData
	{
		private short _tag;

		private byte[] _data;

		public short TagID
		{
			get
			{
				return this._tag;
			}
			set
			{
				this._tag = value;
			}
		}

		public byte[] Data
		{
			get
			{
				return this._data;
			}
			set
			{
				this._data = value;
			}
		}

		public RawTaggedData(short tag)
		{
			this._tag = tag;
		}

		public void SetData(byte[] data, int offset, int count)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			this._data = new byte[count];
			Array.Copy(data, offset, this._data, 0, count);
		}

		public byte[] GetData()
		{
			return this._data;
		}
	}
}
