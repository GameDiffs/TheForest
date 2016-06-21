using System;
using System.IO;
using System.Security.Cryptography;

namespace ICSharpCode.SharpZipLib.Encryption
{
	internal class ZipAESStream : CryptoStream
	{
		private const int AUTH_CODE_LENGTH = 10;

		private const int CRYPTO_BLOCK_SIZE = 16;

		private Stream _stream;

		private ZipAESTransform _transform;

		private byte[] _slideBuffer;

		private int _slideBufStartPos;

		private int _slideBufFreePos;

		private int _blockAndAuth;

		public ZipAESStream(Stream stream, ZipAESTransform transform, CryptoStreamMode mode) : base(stream, transform, mode)
		{
			this._stream = stream;
			this._transform = transform;
			this._slideBuffer = new byte[1024];
			this._blockAndAuth = 26;
			if (mode != CryptoStreamMode.Read)
			{
				throw new Exception("ZipAESStream only for read");
			}
		}

		public override int Read(byte[] outBuffer, int offset, int count)
		{
			int i = 0;
			while (i < count)
			{
				int num = this._slideBufFreePos - this._slideBufStartPos;
				int num2 = this._blockAndAuth - num;
				if (this._slideBuffer.Length - this._slideBufFreePos < num2)
				{
					int num3 = 0;
					int j = this._slideBufStartPos;
					while (j < this._slideBufFreePos)
					{
						this._slideBuffer[num3] = this._slideBuffer[j];
						j++;
						num3++;
					}
					this._slideBufFreePos -= this._slideBufStartPos;
					this._slideBufStartPos = 0;
				}
				int num4 = this._stream.Read(this._slideBuffer, this._slideBufFreePos, num2);
				this._slideBufFreePos += num4;
				num = this._slideBufFreePos - this._slideBufStartPos;
				if (num < this._blockAndAuth)
				{
					if (num > 10)
					{
						int num5 = num - 10;
						this._transform.TransformBlock(this._slideBuffer, this._slideBufStartPos, num5, outBuffer, offset);
						i += num5;
						this._slideBufStartPos += num5;
					}
					else if (num < 10)
					{
						throw new Exception("Internal error missed auth code");
					}
					byte[] authCode = this._transform.GetAuthCode();
					for (int k = 0; k < 10; k++)
					{
						if (authCode[k] != this._slideBuffer[this._slideBufStartPos + k])
						{
							throw new Exception("AES Authentication Code does not match. This is a super-CRC check on the data in the file after compression and encryption. \r\nThe file may be damaged.");
						}
					}
					break;
				}
				this._transform.TransformBlock(this._slideBuffer, this._slideBufStartPos, 16, outBuffer, offset);
				i += 16;
				offset += 16;
				this._slideBufStartPos += 16;
			}
			return i;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}
	}
}
