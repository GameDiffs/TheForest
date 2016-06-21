using System;

namespace ICSharpCode.SharpZipLib.Zip
{
	internal interface ITaggedDataFactory
	{
		ITaggedData Create(short tag, byte[] data, int offset, int count);
	}
}
