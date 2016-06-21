using System;
using System.IO;

namespace ICSharpCode.SharpZipLib.Zip
{
	public sealed class ZipExtraData : IDisposable
	{
		private int _index;

		private int _readValueStart;

		private int _readValueLength;

		private MemoryStream _newEntry;

		private byte[] _data;

		public int Length
		{
			get
			{
				return this._data.Length;
			}
		}

		public int ValueLength
		{
			get
			{
				return this._readValueLength;
			}
		}

		public int CurrentReadIndex
		{
			get
			{
				return this._index;
			}
		}

		public int UnreadCount
		{
			get
			{
				if (this._readValueStart > this._data.Length || this._readValueStart < 4)
				{
					throw new Exception("Find must be called before calling a Read method");
				}
				return this._readValueStart + this._readValueLength - this._index;
			}
		}

		public ZipExtraData()
		{
			this.Clear();
		}

		public ZipExtraData(byte[] data)
		{
			if (data == null)
			{
				this._data = new byte[0];
			}
			else
			{
				this._data = data;
			}
		}

		public byte[] GetEntryData()
		{
			if (this.Length > 65535)
			{
				throw new Exception("Data exceeds maximum length");
			}
			return (byte[])this._data.Clone();
		}

		public void Clear()
		{
			if (this._data == null || this._data.Length != 0)
			{
				this._data = new byte[0];
			}
		}

		public Stream GetStreamForTag(int tag)
		{
			Stream result = null;
			if (this.Find(tag))
			{
				result = new MemoryStream(this._data, this._index, this._readValueLength, false);
			}
			return result;
		}

		private ITaggedData GetData(short tag)
		{
			ITaggedData result = null;
			if (this.Find((int)tag))
			{
				result = ZipExtraData.Create(tag, this._data, this._readValueStart, this._readValueLength);
			}
			return result;
		}

		private static ITaggedData Create(short tag, byte[] data, int offset, int count)
		{
			ITaggedData taggedData;
			if (tag != 10)
			{
				if (tag != 21589)
				{
					taggedData = new RawTaggedData(tag);
				}
				else
				{
					taggedData = new ExtendedUnixData();
				}
			}
			else
			{
				taggedData = new NTTaggedData();
			}
			taggedData.SetData(data, offset, count);
			return taggedData;
		}

		public bool Find(int headerID)
		{
			this._readValueStart = this._data.Length;
			this._readValueLength = 0;
			this._index = 0;
			int num = this._readValueStart;
			int num2 = headerID - 1;
			while (num2 != headerID && this._index < this._data.Length - 3)
			{
				num2 = this.ReadShortInternal();
				num = this.ReadShortInternal();
				if (num2 != headerID)
				{
					this._index += num;
				}
			}
			bool flag = num2 == headerID && this._index + num <= this._data.Length;
			if (flag)
			{
				this._readValueStart = this._index;
				this._readValueLength = num;
			}
			return flag;
		}

		public void AddEntry(ITaggedData taggedData)
		{
			if (taggedData == null)
			{
				throw new ArgumentNullException("taggedData");
			}
			this.AddEntry((int)taggedData.TagID, taggedData.GetData());
		}

		public void AddEntry(int headerID, byte[] fieldData)
		{
			if (headerID > 65535 || headerID < 0)
			{
				throw new ArgumentOutOfRangeException("headerID");
			}
			int num = (fieldData != null) ? fieldData.Length : 0;
			if (num > 65535)
			{
				throw new ArgumentOutOfRangeException("fieldData", "exceeds maximum length");
			}
			int num2 = this._data.Length + num + 4;
			if (this.Find(headerID))
			{
				num2 -= this.ValueLength + 4;
			}
			if (num2 > 65535)
			{
				throw new Exception("Data exceeds maximum length");
			}
			this.Delete(headerID);
			byte[] array = new byte[num2];
			this._data.CopyTo(array, 0);
			int index = this._data.Length;
			this._data = array;
			this.SetShort(ref index, headerID);
			this.SetShort(ref index, num);
			if (fieldData != null)
			{
				fieldData.CopyTo(array, index);
			}
		}

		public void StartNewEntry()
		{
			this._newEntry = new MemoryStream();
		}

		public void AddNewEntry(int headerID)
		{
			byte[] fieldData = this._newEntry.ToArray();
			this._newEntry = null;
			this.AddEntry(headerID, fieldData);
		}

		public void AddData(byte data)
		{
			this._newEntry.WriteByte(data);
		}

		public void AddData(byte[] data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			this._newEntry.Write(data, 0, data.Length);
		}

		public void AddLeShort(int toAdd)
		{
			this._newEntry.WriteByte((byte)toAdd);
			this._newEntry.WriteByte((byte)(toAdd >> 8));
		}

		public void AddLeInt(int toAdd)
		{
			this.AddLeShort((int)((short)toAdd));
			this.AddLeShort((int)((short)(toAdd >> 16)));
		}

		public void AddLeLong(long toAdd)
		{
			this.AddLeInt((int)(toAdd & (long)((ulong)-1)));
			this.AddLeInt((int)(toAdd >> 32));
		}

		public bool Delete(int headerID)
		{
			bool result = false;
			if (this.Find(headerID))
			{
				result = true;
				int num = this._readValueStart - 4;
				byte[] array = new byte[this._data.Length - (this.ValueLength + 4)];
				Array.Copy(this._data, 0, array, 0, num);
				int num2 = num + this.ValueLength + 4;
				Array.Copy(this._data, num2, array, num, this._data.Length - num2);
				this._data = array;
			}
			return result;
		}

		public long ReadLong()
		{
			this.ReadCheck(8);
			return ((long)this.ReadInt() & (long)((ulong)-1)) | (long)this.ReadInt() << 32;
		}

		public int ReadInt()
		{
			this.ReadCheck(4);
			int result = (int)this._data[this._index] + ((int)this._data[this._index + 1] << 8) + ((int)this._data[this._index + 2] << 16) + ((int)this._data[this._index + 3] << 24);
			this._index += 4;
			return result;
		}

		public int ReadShort()
		{
			this.ReadCheck(2);
			int result = (int)this._data[this._index] + ((int)this._data[this._index + 1] << 8);
			this._index += 2;
			return result;
		}

		public int ReadByte()
		{
			int result = -1;
			if (this._index < this._data.Length && this._readValueStart + this._readValueLength > this._index)
			{
				result = (int)this._data[this._index];
				this._index++;
			}
			return result;
		}

		public void Skip(int amount)
		{
			this.ReadCheck(amount);
			this._index += amount;
		}

		private void ReadCheck(int length)
		{
			if (this._readValueStart > this._data.Length || this._readValueStart < 4)
			{
				throw new Exception("Find must be called before calling a Read method");
			}
			if (this._index > this._readValueStart + this._readValueLength - length)
			{
				throw new Exception("End of extra data");
			}
			if (this._index + length < 4)
			{
				throw new Exception("Cannot read before start of tag");
			}
		}

		private int ReadShortInternal()
		{
			if (this._index > this._data.Length - 2)
			{
				throw new Exception("End of extra data");
			}
			int result = (int)this._data[this._index] + ((int)this._data[this._index + 1] << 8);
			this._index += 2;
			return result;
		}

		private void SetShort(ref int index, int source)
		{
			this._data[index] = (byte)source;
			this._data[index + 1] = (byte)(source >> 8);
			index += 2;
		}

		public void Dispose()
		{
			if (this._newEntry != null)
			{
				this._newEntry.Close();
			}
		}
	}
}
