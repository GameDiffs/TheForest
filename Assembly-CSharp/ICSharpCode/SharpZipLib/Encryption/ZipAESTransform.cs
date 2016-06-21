using System;
using System.Security.Cryptography;

namespace ICSharpCode.SharpZipLib.Encryption
{
	internal class ZipAESTransform : IDisposable, ICryptoTransform
	{
		private const int PWD_VER_LENGTH = 2;

		private const int KEY_ROUNDS = 1000;

		private const int ENCRYPT_BLOCK = 16;

		private int _blockSize;

		private ICryptoTransform _encryptor;

		private readonly byte[] _counterNonce;

		private byte[] _encryptBuffer;

		private int _encrPos;

		private byte[] _pwdVerifier;

		private HMACSHA1 _hmacsha1;

		private bool _finalised;

		private bool _writeMode;

		public byte[] PwdVerifier
		{
			get
			{
				return this._pwdVerifier;
			}
		}

		public int InputBlockSize
		{
			get
			{
				return this._blockSize;
			}
		}

		public int OutputBlockSize
		{
			get
			{
				return this._blockSize;
			}
		}

		public bool CanTransformMultipleBlocks
		{
			get
			{
				return true;
			}
		}

		public bool CanReuseTransform
		{
			get
			{
				return true;
			}
		}

		public ZipAESTransform(string key, byte[] saltBytes, int blockSize, bool writeMode)
		{
			if (blockSize != 16 && blockSize != 32)
			{
				throw new Exception("Invalid blocksize " + blockSize + ". Must be 16 or 32.");
			}
			if (saltBytes.Length != blockSize / 2)
			{
				throw new Exception(string.Concat(new object[]
				{
					"Invalid salt len. Must be ",
					blockSize / 2,
					" for blocksize ",
					blockSize
				}));
			}
			this._blockSize = blockSize;
			this._encryptBuffer = new byte[this._blockSize];
			this._encrPos = 16;
			Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(key, saltBytes, 1000);
			RijndaelManaged rijndaelManaged = new RijndaelManaged();
			rijndaelManaged.Mode = CipherMode.ECB;
			this._counterNonce = new byte[this._blockSize];
			byte[] bytes = rfc2898DeriveBytes.GetBytes(this._blockSize);
			byte[] bytes2 = rfc2898DeriveBytes.GetBytes(this._blockSize);
			this._encryptor = rijndaelManaged.CreateEncryptor(bytes, bytes2);
			this._pwdVerifier = rfc2898DeriveBytes.GetBytes(2);
			this._hmacsha1 = new HMACSHA1(bytes2);
			this._writeMode = writeMode;
		}

		public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
		{
			if (!this._writeMode)
			{
				this._hmacsha1.TransformBlock(inputBuffer, inputOffset, inputCount, inputBuffer, inputOffset);
			}
			for (int i = 0; i < inputCount; i++)
			{
				if (this._encrPos == 16)
				{
					int num = 0;
					while (true)
					{
						byte[] expr_47_cp_0 = this._counterNonce;
						int expr_47_cp_1 = num;
						if ((expr_47_cp_0[expr_47_cp_1] += 1) != 0)
						{
							break;
						}
						num++;
					}
					this._encryptor.TransformBlock(this._counterNonce, 0, this._blockSize, this._encryptBuffer, 0);
					this._encrPos = 0;
				}
				outputBuffer[i + outputOffset] = (inputBuffer[i + inputOffset] ^ this._encryptBuffer[this._encrPos++]);
			}
			if (this._writeMode)
			{
				this._hmacsha1.TransformBlock(outputBuffer, outputOffset, inputCount, outputBuffer, outputOffset);
			}
			return inputCount;
		}

		public byte[] GetAuthCode()
		{
			if (!this._finalised)
			{
				byte[] inputBuffer = new byte[0];
				this._hmacsha1.TransformFinalBlock(inputBuffer, 0, 0);
				this._finalised = true;
			}
			return this._hmacsha1.Hash;
		}

		public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
		{
			throw new NotImplementedException("ZipAESTransform.TransformFinalBlock");
		}

		public void Dispose()
		{
			this._encryptor.Dispose();
		}
	}
}
