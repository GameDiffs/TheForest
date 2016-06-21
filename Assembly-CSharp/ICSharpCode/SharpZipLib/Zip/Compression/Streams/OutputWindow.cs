using System;

namespace ICSharpCode.SharpZipLib.Zip.Compression.Streams
{
	public class OutputWindow
	{
		private const int WindowSize = 32768;

		private const int WindowMask = 32767;

		private byte[] window = new byte[32768];

		private int windowEnd;

		private int windowFilled;

		public void Write(int value)
		{
			if (this.windowFilled++ == 32768)
			{
				throw new InvalidOperationException("Window full");
			}
			this.window[this.windowEnd++] = (byte)value;
			this.windowEnd &= 32767;
		}

		private void SlowRepeat(int repStart, int length, int distance)
		{
			while (length-- > 0)
			{
				this.window[this.windowEnd++] = this.window[repStart++];
				this.windowEnd &= 32767;
				repStart &= 32767;
			}
		}

		public void Repeat(int length, int distance)
		{
			if ((this.windowFilled += length) > 32768)
			{
				throw new InvalidOperationException("Window full");
			}
			int num = this.windowEnd - distance & 32767;
			int num2 = 32768 - length;
			if (num <= num2 && this.windowEnd < num2)
			{
				if (length <= distance)
				{
					Array.Copy(this.window, num, this.window, this.windowEnd, length);
					this.windowEnd += length;
				}
				else
				{
					while (length-- > 0)
					{
						this.window[this.windowEnd++] = this.window[num++];
					}
				}
			}
			else
			{
				this.SlowRepeat(num, length, distance);
			}
		}

		public int CopyStored(StreamManipulator input, int length)
		{
			length = Math.Min(Math.Min(length, 32768 - this.windowFilled), input.AvailableBytes);
			int num = 32768 - this.windowEnd;
			int num2;
			if (length > num)
			{
				num2 = input.CopyBytes(this.window, this.windowEnd, num);
				if (num2 == num)
				{
					num2 += input.CopyBytes(this.window, 0, length - num);
				}
			}
			else
			{
				num2 = input.CopyBytes(this.window, this.windowEnd, length);
			}
			this.windowEnd = (this.windowEnd + num2 & 32767);
			this.windowFilled += num2;
			return num2;
		}

		public void CopyDict(byte[] dictionary, int offset, int length)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException("dictionary");
			}
			if (this.windowFilled > 0)
			{
				throw new InvalidOperationException();
			}
			if (length > 32768)
			{
				offset += length - 32768;
				length = 32768;
			}
			Array.Copy(dictionary, offset, this.window, 0, length);
			this.windowEnd = (length & 32767);
		}

		public int GetFreeSpace()
		{
			return 32768 - this.windowFilled;
		}

		public int GetAvailable()
		{
			return this.windowFilled;
		}

		public int CopyOutput(byte[] output, int offset, int len)
		{
			int num = this.windowEnd;
			if (len > this.windowFilled)
			{
				len = this.windowFilled;
			}
			else
			{
				num = (this.windowEnd - this.windowFilled + len & 32767);
			}
			int num2 = len;
			int num3 = len - num;
			if (num3 > 0)
			{
				Array.Copy(this.window, 32768 - num3, output, offset, num3);
				offset += num3;
				len = num;
			}
			Array.Copy(this.window, num - len, output, offset, len);
			this.windowFilled -= num2;
			if (this.windowFilled < 0)
			{
				throw new InvalidOperationException();
			}
			return num2;
		}

		public void Reset()
		{
			this.windowFilled = (this.windowEnd = 0);
		}
	}
}
