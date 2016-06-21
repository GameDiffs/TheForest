using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System;
using System.Collections.Generic;
using System.IO;

public static class CompressionHelper
{
	public static string technique = "ZipStream";

	public static string Compress(byte[] data)
	{
		string result;
		using (MemoryStream memoryStream = new MemoryStream())
		{
			string text = CompressionHelper.technique;
			if (text != null)
			{
				if (CompressionHelper.<>f__switch$map1F == null)
				{
					CompressionHelper.<>f__switch$map1F = new Dictionary<string, int>(1)
					{
						{
							"ZipStream",
							0
						}
					};
				}
				int num;
				if (CompressionHelper.<>f__switch$map1F.TryGetValue(text, out num))
				{
					if (num == 0)
					{
						BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
						DeflaterOutputStream deflaterOutputStream = new DeflaterOutputStream(memoryStream);
						binaryWriter.Write(data.Length);
						deflaterOutputStream.Write(data, 0, data.Length);
						deflaterOutputStream.Flush();
						deflaterOutputStream.Close();
					}
				}
			}
			result = CompressionHelper.technique + ":" + Convert.ToBase64String(memoryStream.GetBuffer());
		}
		return result;
	}

	public static byte[] Decompress(string data)
	{
		byte[] array = null;
		if (data.StartsWith("ZipStream:"))
		{
			MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(data.Substring(10)));
			InflaterInputStream inflaterInputStream = new InflaterInputStream(memoryStream);
			BinaryReader binaryReader = new BinaryReader(memoryStream);
			int num = binaryReader.ReadInt32();
			array = new byte[num];
			inflaterInputStream.Read(array, 0, num);
			inflaterInputStream.Close();
			memoryStream.Close();
		}
		return array;
	}
}
