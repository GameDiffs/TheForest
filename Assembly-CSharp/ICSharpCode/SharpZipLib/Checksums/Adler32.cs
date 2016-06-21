using System;

namespace ICSharpCode.SharpZipLib.Checksums
{
	public sealed class Adler32 : IChecksum
	{
		private const uint BASE = 65521u;

		private uint checksum;

		public long Value
		{
			get
			{
				return (long)((ulong)this.checksum);
			}
		}

		public Adler32()
		{
			this.Reset();
		}

		public void Reset()
		{
			this.checksum = 1u;
		}

		public void Update(int value)
		{
			uint num = this.checksum & 65535u;
			uint num2 = this.checksum >> 16;
			num = (num + (uint)(value & 255)) % 65521u;
			num2 = (num + num2) % 65521u;
			this.checksum = (num2 << 16) + num;
		}

		public void Update(byte[] buffer)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			this.Update(buffer, 0, buffer.Length);
		}

		public void Update(byte[] buffer, int offset, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "cannot be negative");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "cannot be negative");
			}
			if (offset >= buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset", "not a valid index into buffer");
			}
			if (offset + count > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("count", "exceeds buffer size");
			}
			uint num = this.checksum & 65535u;
			uint num2 = this.checksum >> 16;
			while (count > 0)
			{
				int num3 = 3800;
				if (num3 > count)
				{
					num3 = count;
				}
				count -= num3;
				while (--num3 >= 0)
				{
					num += (uint)(buffer[offset++] & 255);
					num2 += num;
				}
				num %= 65521u;
				num2 %= 65521u;
			}
			this.checksum = (num2 << 16 | num);
		}
	}
}
