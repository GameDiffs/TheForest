using System;
using System.IO;

[Serializable]
public class ByteArray
{
	private MemoryStream stream;

	private BinaryWriter writer;

	public ByteArray()
	{
		this.stream = new MemoryStream();
		this.writer = new BinaryWriter(this.stream);
	}

	public override void writeByte(byte value)
	{
		this.writer.Write(value);
	}

	public override byte[] GetAllBytes()
	{
		byte[] array = new byte[(int)this.stream.Length];
		this.stream.Position = 0L;
		this.stream.Read(array, 0, array.Length);
		return array;
	}
}
