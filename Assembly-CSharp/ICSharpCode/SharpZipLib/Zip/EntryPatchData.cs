using System;

namespace ICSharpCode.SharpZipLib.Zip
{
	internal class EntryPatchData
	{
		private long sizePatchOffset_;

		private long crcPatchOffset_;

		public long SizePatchOffset
		{
			get
			{
				return this.sizePatchOffset_;
			}
			set
			{
				this.sizePatchOffset_ = value;
			}
		}

		public long CrcPatchOffset
		{
			get
			{
				return this.crcPatchOffset_;
			}
			set
			{
				this.crcPatchOffset_ = value;
			}
		}
	}
}
