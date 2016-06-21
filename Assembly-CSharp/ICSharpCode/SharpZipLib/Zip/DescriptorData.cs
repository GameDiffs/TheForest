using System;

namespace ICSharpCode.SharpZipLib.Zip
{
	public class DescriptorData
	{
		private long size;

		private long compressedSize;

		private long crc;

		public long CompressedSize
		{
			get
			{
				return this.compressedSize;
			}
			set
			{
				this.compressedSize = value;
			}
		}

		public long Size
		{
			get
			{
				return this.size;
			}
			set
			{
				this.size = value;
			}
		}

		public long Crc
		{
			get
			{
				return this.crc;
			}
			set
			{
				this.crc = (value & (long)((ulong)-1));
			}
		}
	}
}
