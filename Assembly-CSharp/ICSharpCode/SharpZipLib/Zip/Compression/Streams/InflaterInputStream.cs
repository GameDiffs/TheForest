using System;
using System.IO;

namespace ICSharpCode.SharpZipLib.Zip.Compression.Streams
{
	public class InflaterInputStream : Stream
	{
		protected Inflater inf;

		protected InflaterInputBuffer inputBuffer;

		private Stream baseInputStream;

		protected long csize;

		private bool isClosed;

		private bool isStreamOwner = true;

		public bool IsStreamOwner
		{
			get
			{
				return this.isStreamOwner;
			}
			set
			{
				this.isStreamOwner = value;
			}
		}

		public virtual int Available
		{
			get
			{
				return (!this.inf.IsFinished) ? 1 : 0;
			}
		}

		public override bool CanRead
		{
			get
			{
				return this.baseInputStream.CanRead;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		public override long Length
		{
			get
			{
				return (long)this.inputBuffer.RawLength;
			}
		}

		public override long Position
		{
			get
			{
				return this.baseInputStream.Position;
			}
			set
			{
				throw new NotSupportedException("InflaterInputStream Position not supported");
			}
		}

		public InflaterInputStream(Stream baseInputStream) : this(baseInputStream, new Inflater(), 4096)
		{
		}

		public InflaterInputStream(Stream baseInputStream, Inflater inf) : this(baseInputStream, inf, 4096)
		{
		}

		public InflaterInputStream(Stream baseInputStream, Inflater inflater, int bufferSize)
		{
			if (baseInputStream == null)
			{
				throw new ArgumentNullException("baseInputStream");
			}
			if (inflater == null)
			{
				throw new ArgumentNullException("inflater");
			}
			if (bufferSize <= 0)
			{
				throw new ArgumentOutOfRangeException("bufferSize");
			}
			this.baseInputStream = baseInputStream;
			this.inf = inflater;
			this.inputBuffer = new InflaterInputBuffer(baseInputStream, bufferSize);
		}

		public long Skip(long count)
		{
			if (count <= 0L)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (this.baseInputStream.CanSeek)
			{
				this.baseInputStream.Seek(count, SeekOrigin.Current);
				return count;
			}
			int num = 2048;
			if (count < (long)num)
			{
				num = (int)count;
			}
			byte[] buffer = new byte[num];
			int num2 = 1;
			long num3 = count;
			while (num3 > 0L && num2 > 0)
			{
				if (num3 < (long)num)
				{
					num = (int)num3;
				}
				num2 = this.baseInputStream.Read(buffer, 0, num);
				num3 -= (long)num2;
			}
			return count - num3;
		}

		protected void StopDecrypting()
		{
			this.inputBuffer.CryptoTransform = null;
		}

		protected void Fill()
		{
			if (this.inputBuffer.Available <= 0)
			{
				this.inputBuffer.Fill();
				if (this.inputBuffer.Available <= 0)
				{
					throw new SharpZipBaseException("Unexpected EOF");
				}
			}
			this.inputBuffer.SetInflaterInput(this.inf);
		}

		public override void Flush()
		{
			this.baseInputStream.Flush();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException("Seek not supported");
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException("InflaterInputStream SetLength not supported");
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException("InflaterInputStream Write not supported");
		}

		public override void WriteByte(byte value)
		{
			throw new NotSupportedException("InflaterInputStream WriteByte not supported");
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			throw new NotSupportedException("InflaterInputStream BeginWrite not supported");
		}

		public override void Close()
		{
			if (!this.isClosed)
			{
				this.isClosed = true;
				if (this.isStreamOwner)
				{
					this.baseInputStream.Close();
				}
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (this.inf.IsNeedingDictionary)
			{
				throw new SharpZipBaseException("Need a dictionary");
			}
			int num = count;
			while (true)
			{
				int num2 = this.inf.Inflate(buffer, offset, num);
				offset += num2;
				num -= num2;
				if (num == 0 || this.inf.IsFinished)
				{
					break;
				}
				if (this.inf.IsNeedingInput)
				{
					this.Fill();
				}
				else if (num2 == 0)
				{
					goto Block_4;
				}
			}
			return count - num;
			Block_4:
			throw new Exception("Dont know what to do");
		}
	}
}
