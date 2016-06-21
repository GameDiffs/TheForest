using System;

namespace ICSharpCode.SharpZipLib.Zip
{
	public enum CompressionMethod
	{
		Stored,
		Deflated = 8,
		Deflate64,
		BZip2 = 11,
		WinZipAES = 99
	}
}
