using System;
using System.IO;

namespace ICSharpCode.SharpZipLib.Zip
{
	internal class ZipHelperStream : Stream
	{
		private bool isOwner_;

		private Stream stream_;

		public bool IsStreamOwner
		{
			get
			{
				return this.isOwner_;
			}
			set
			{
				this.isOwner_ = value;
			}
		}

		public override bool CanRead
		{
			get
			{
				return this.stream_.CanRead;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return this.stream_.CanSeek;
			}
		}

		public override bool CanTimeout
		{
			get
			{
				return this.stream_.CanTimeout;
			}
		}

		public override long Length
		{
			get
			{
				return this.stream_.Length;
			}
		}

		public override long Position
		{
			get
			{
				return this.stream_.Position;
			}
			set
			{
				this.stream_.Position = value;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this.stream_.CanWrite;
			}
		}

		public ZipHelperStream(string name)
		{
			this.stream_ = new FileStream(name, FileMode.Open, FileAccess.ReadWrite);
			this.isOwner_ = true;
		}

		public ZipHelperStream(Stream stream)
		{
			this.stream_ = stream;
		}

		public override void Flush()
		{
			this.stream_.Flush();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return this.stream_.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			this.stream_.SetLength(value);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return this.stream_.Read(buffer, offset, count);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			this.stream_.Write(buffer, offset, count);
		}

		public override void Close()
		{
			Stream stream = this.stream_;
			this.stream_ = null;
			if (this.isOwner_ && stream != null)
			{
				this.isOwner_ = false;
				stream.Close();
			}
		}

		private void WriteLocalHeader(ZipEntry entry, EntryPatchData patchData)
		{
			CompressionMethod compressionMethod = entry.CompressionMethod;
			bool flag = true;
			bool flag2 = false;
			this.WriteLEInt(67324752);
			this.WriteLEShort(entry.Version);
			this.WriteLEShort(entry.Flags);
			this.WriteLEShort((int)((byte)compressionMethod));
			this.WriteLEInt((int)entry.DosTime);
			if (flag)
			{
				this.WriteLEInt((int)entry.Crc);
				if (entry.LocalHeaderRequiresZip64)
				{
					this.WriteLEInt(-1);
					this.WriteLEInt(-1);
				}
				else
				{
					this.WriteLEInt((!entry.IsCrypted) ? ((int)entry.CompressedSize) : ((int)entry.CompressedSize + 12));
					this.WriteLEInt((int)entry.Size);
				}
			}
			else
			{
				if (patchData != null)
				{
					patchData.CrcPatchOffset = this.stream_.Position;
				}
				this.WriteLEInt(0);
				if (patchData != null)
				{
					patchData.SizePatchOffset = this.stream_.Position;
				}
				if (entry.LocalHeaderRequiresZip64 && flag2)
				{
					this.WriteLEInt(-1);
					this.WriteLEInt(-1);
				}
				else
				{
					this.WriteLEInt(0);
					this.WriteLEInt(0);
				}
			}
			byte[] array = ZipConstants.ConvertToArray(entry.Flags, entry.Name);
			if (array.Length > 65535)
			{
				throw new Exception("Entry name too long.");
			}
			ZipExtraData zipExtraData = new ZipExtraData(entry.ExtraData);
			if (entry.LocalHeaderRequiresZip64 && (flag || flag2))
			{
				zipExtraData.StartNewEntry();
				if (flag)
				{
					zipExtraData.AddLeLong(entry.Size);
					zipExtraData.AddLeLong(entry.CompressedSize);
				}
				else
				{
					zipExtraData.AddLeLong(-1L);
					zipExtraData.AddLeLong(-1L);
				}
				zipExtraData.AddNewEntry(1);
				if (!zipExtraData.Find(1))
				{
					throw new Exception("Internal error cant find extra data");
				}
				if (patchData != null)
				{
					patchData.SizePatchOffset = (long)zipExtraData.CurrentReadIndex;
				}
			}
			else
			{
				zipExtraData.Delete(1);
			}
			byte[] entryData = zipExtraData.GetEntryData();
			this.WriteLEShort(array.Length);
			this.WriteLEShort(entryData.Length);
			if (array.Length > 0)
			{
				this.stream_.Write(array, 0, array.Length);
			}
			if (entry.LocalHeaderRequiresZip64 && flag2)
			{
				patchData.SizePatchOffset += this.stream_.Position;
			}
			if (entryData.Length > 0)
			{
				this.stream_.Write(entryData, 0, entryData.Length);
			}
		}

		public long LocateBlockWithSignature(int signature, long endLocation, int minimumBlockSize, int maximumVariableData)
		{
			long num = endLocation - (long)minimumBlockSize;
			if (num < 0L)
			{
				return -1L;
			}
			long num2 = Math.Max(num - (long)maximumVariableData, 0L);
			while (num >= num2)
			{
				long expr_29 = num;
				num = expr_29 - 1L;
				this.Seek(expr_29, SeekOrigin.Begin);
				if (this.ReadLEInt() == signature)
				{
					return this.Position;
				}
			}
			return -1L;
		}

		public void WriteZip64EndOfCentralDirectory(long noOfEntries, long sizeEntries, long centralDirOffset)
		{
			long position = this.stream_.Position;
			this.WriteLEInt(101075792);
			this.WriteLELong(44L);
			this.WriteLEShort(51);
			this.WriteLEShort(45);
			this.WriteLEInt(0);
			this.WriteLEInt(0);
			this.WriteLELong(noOfEntries);
			this.WriteLELong(noOfEntries);
			this.WriteLELong(sizeEntries);
			this.WriteLELong(centralDirOffset);
			this.WriteLEInt(117853008);
			this.WriteLEInt(0);
			this.WriteLELong(position);
			this.WriteLEInt(1);
		}

		public void WriteEndOfCentralDirectory(long noOfEntries, long sizeEntries, long startOfCentralDirectory, byte[] comment)
		{
			if (noOfEntries >= 65535L || startOfCentralDirectory >= (long)((ulong)-1) || sizeEntries >= (long)((ulong)-1))
			{
				this.WriteZip64EndOfCentralDirectory(noOfEntries, sizeEntries, startOfCentralDirectory);
			}
			this.WriteLEInt(101010256);
			this.WriteLEShort(0);
			this.WriteLEShort(0);
			if (noOfEntries >= 65535L)
			{
				this.WriteLEUshort(65535);
				this.WriteLEUshort(65535);
			}
			else
			{
				this.WriteLEShort((int)((short)noOfEntries));
				this.WriteLEShort((int)((short)noOfEntries));
			}
			if (sizeEntries >= (long)((ulong)-1))
			{
				this.WriteLEUint(4294967295u);
			}
			else
			{
				this.WriteLEInt((int)sizeEntries);
			}
			if (startOfCentralDirectory >= (long)((ulong)-1))
			{
				this.WriteLEUint(4294967295u);
			}
			else
			{
				this.WriteLEInt((int)startOfCentralDirectory);
			}
			int num = (comment == null) ? 0 : comment.Length;
			if (num > 65535)
			{
				throw new Exception(string.Format("Comment length({0}) is too long can only be 64K", num));
			}
			this.WriteLEShort(num);
			if (num > 0)
			{
				this.Write(comment, 0, comment.Length);
			}
		}

		public int ReadLEShort()
		{
			int num = this.stream_.ReadByte();
			if (num < 0)
			{
				throw new EndOfStreamException();
			}
			int num2 = this.stream_.ReadByte();
			if (num2 < 0)
			{
				throw new EndOfStreamException();
			}
			return num | num2 << 8;
		}

		public int ReadLEInt()
		{
			return this.ReadLEShort() | this.ReadLEShort() << 16;
		}

		public long ReadLELong()
		{
			return (long)((ulong)this.ReadLEInt() | (ulong)((ulong)((long)this.ReadLEInt()) << 32));
		}

		public void WriteLEShort(int value)
		{
			this.stream_.WriteByte((byte)(value & 255));
			this.stream_.WriteByte((byte)(value >> 8 & 255));
		}

		public void WriteLEUshort(ushort value)
		{
			this.stream_.WriteByte((byte)(value & 255));
			this.stream_.WriteByte((byte)(value >> 8));
		}

		public void WriteLEInt(int value)
		{
			this.WriteLEShort(value);
			this.WriteLEShort(value >> 16);
		}

		public void WriteLEUint(uint value)
		{
			this.WriteLEUshort((ushort)(value & 65535u));
			this.WriteLEUshort((ushort)(value >> 16));
		}

		public void WriteLELong(long value)
		{
			this.WriteLEInt((int)value);
			this.WriteLEInt((int)(value >> 32));
		}

		public void WriteLEUlong(ulong value)
		{
			this.WriteLEUint((uint)(value & (ulong)-1));
			this.WriteLEUint((uint)(value >> 32));
		}

		public int WriteDataDescriptor(ZipEntry entry)
		{
			if (entry == null)
			{
				throw new ArgumentNullException("entry");
			}
			int num = 0;
			if ((entry.Flags & 8) != 0)
			{
				this.WriteLEInt(134695760);
				this.WriteLEInt((int)entry.Crc);
				num += 8;
				if (entry.LocalHeaderRequiresZip64)
				{
					this.WriteLELong(entry.CompressedSize);
					this.WriteLELong(entry.Size);
					num += 16;
				}
				else
				{
					this.WriteLEInt((int)entry.CompressedSize);
					this.WriteLEInt((int)entry.Size);
					num += 8;
				}
			}
			return num;
		}

		public void ReadDataDescriptor(bool zip64, DescriptorData data)
		{
			int num = this.ReadLEInt();
			if (num != 134695760)
			{
				throw new Exception("Data descriptor signature not found");
			}
			data.Crc = (long)this.ReadLEInt();
			if (zip64)
			{
				data.CompressedSize = this.ReadLELong();
				data.Size = this.ReadLELong();
			}
			else
			{
				data.CompressedSize = (long)this.ReadLEInt();
				data.Size = (long)this.ReadLEInt();
			}
		}
	}
}
