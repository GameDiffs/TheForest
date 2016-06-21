using System;
using System.IO;
using System.Security.Cryptography;

namespace ICSharpCode.SharpZipLib.Zip.Compression.Streams
{
	public class InflaterInputBuffer
	{
		private int rawLength;

		private byte[] rawData;

		private int clearTextLength;

		private byte[] clearText;

		private byte[] internalClearText;

		private int available;

		private ICryptoTransform cryptoTransform;

		private Stream inputStream;

		public int RawLength
		{
			get
			{
				return this.rawLength;
			}
		}

		public byte[] RawData
		{
			get
			{
				return this.rawData;
			}
		}

		public int ClearTextLength
		{
			get
			{
				return this.clearTextLength;
			}
		}

		public byte[] ClearText
		{
			get
			{
				return this.clearText;
			}
		}

		public int Available
		{
			get
			{
				return this.available;
			}
			set
			{
				this.available = value;
			}
		}

		public ICryptoTransform CryptoTransform
		{
			set
			{
				this.cryptoTransform = value;
				if (this.cryptoTransform != null)
				{
					if (this.rawData == this.clearText)
					{
						if (this.internalClearText == null)
						{
							this.internalClearText = new byte[this.rawData.Length];
						}
						this.clearText = this.internalClearText;
					}
					this.clearTextLength = this.rawLength;
					if (this.available > 0)
					{
						this.cryptoTransform.TransformBlock(this.rawData, this.rawLength - this.available, this.available, this.clearText, this.rawLength - this.available);
					}
				}
				else
				{
					this.clearText = this.rawData;
					this.clearTextLength = this.rawLength;
				}
			}
		}

		public InflaterInputBuffer(Stream stream) : this(stream, 4096)
		{
		}

		public InflaterInputBuffer(Stream stream, int bufferSize)
		{
			this.inputStream = stream;
			if (bufferSize < 1024)
			{
				bufferSize = 1024;
			}
			this.rawData = new byte[bufferSize];
			this.clearText = this.rawData;
		}

		public void SetInflaterInput(Inflater inflater)
		{
			if (this.available > 0)
			{
				inflater.SetInput(this.clearText, this.clearTextLength - this.available, this.available);
				this.available = 0;
			}
		}

		public void Fill()
		{
			this.rawLength = 0;
			int num;
			for (int i = this.rawData.Length; i > 0; i -= num)
			{
				num = this.inputStream.Read(this.rawData, this.rawLength, i);
				if (num <= 0)
				{
					break;
				}
				this.rawLength += num;
			}
			if (this.cryptoTransform != null)
			{
				this.clearTextLength = this.cryptoTransform.TransformBlock(this.rawData, 0, this.rawLength, this.clearText, 0);
			}
			else
			{
				this.clearTextLength = this.rawLength;
			}
			this.available = this.clearTextLength;
		}

		public int ReadRawBuffer(byte[] buffer)
		{
			return this.ReadRawBuffer(buffer, 0, buffer.Length);
		}

		public int ReadRawBuffer(byte[] outBuffer, int offset, int length)
		{
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			int num = offset;
			int i = length;
			while (i > 0)
			{
				if (this.available <= 0)
				{
					this.Fill();
					if (this.available <= 0)
					{
						return 0;
					}
				}
				int num2 = Math.Min(i, this.available);
				Array.Copy(this.rawData, this.rawLength - this.available, outBuffer, num, num2);
				num += num2;
				i -= num2;
				this.available -= num2;
			}
			return length;
		}

		public int ReadClearTextBuffer(byte[] outBuffer, int offset, int length)
		{
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			int num = offset;
			int i = length;
			while (i > 0)
			{
				if (this.available <= 0)
				{
					this.Fill();
					if (this.available <= 0)
					{
						return 0;
					}
				}
				int num2 = Math.Min(i, this.available);
				Array.Copy(this.clearText, this.clearTextLength - this.available, outBuffer, num, num2);
				num += num2;
				i -= num2;
				this.available -= num2;
			}
			return length;
		}

		public int ReadLeByte()
		{
			if (this.available <= 0)
			{
				this.Fill();
				if (this.available <= 0)
				{
					throw new Exception("EOF in header");
				}
			}
			byte result = this.rawData[this.rawLength - this.available];
			this.available--;
			return (int)result;
		}

		public int ReadLeShort()
		{
			return this.ReadLeByte() | this.ReadLeByte() << 8;
		}

		public int ReadLeInt()
		{
			return this.ReadLeShort() | this.ReadLeShort() << 16;
		}

		public long ReadLeLong()
		{
			return (long)((ulong)this.ReadLeInt() | (ulong)((ulong)((long)this.ReadLeInt()) << 32));
		}
	}
}
