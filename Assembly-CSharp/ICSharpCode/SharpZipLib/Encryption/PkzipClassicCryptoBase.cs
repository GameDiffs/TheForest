using ICSharpCode.SharpZipLib.Checksums;
using System;

namespace ICSharpCode.SharpZipLib.Encryption
{
	internal class PkzipClassicCryptoBase
	{
		private uint[] keys;

		protected byte TransformByte()
		{
			uint num = (this.keys[2] & 65535u) | 2u;
			return (byte)(num * (num ^ 1u) >> 8);
		}

		protected void SetKeys(byte[] keyData)
		{
			if (keyData == null)
			{
				throw new ArgumentNullException("keyData");
			}
			if (keyData.Length != 12)
			{
				throw new InvalidOperationException("Key length is not valid");
			}
			this.keys = new uint[3];
			this.keys[0] = (uint)((int)keyData[3] << 24 | (int)keyData[2] << 16 | (int)keyData[1] << 8 | (int)keyData[0]);
			this.keys[1] = (uint)((int)keyData[7] << 24 | (int)keyData[6] << 16 | (int)keyData[5] << 8 | (int)keyData[4]);
			this.keys[2] = (uint)((int)keyData[11] << 24 | (int)keyData[10] << 16 | (int)keyData[9] << 8 | (int)keyData[8]);
		}

		protected void UpdateKeys(byte ch)
		{
			this.keys[0] = Crc32.ComputeCrc32(this.keys[0], ch);
			this.keys[1] = this.keys[1] + (uint)((byte)this.keys[0]);
			this.keys[1] = this.keys[1] * 134775813u + 1u;
			this.keys[2] = Crc32.ComputeCrc32(this.keys[2], (byte)(this.keys[1] >> 24));
		}

		protected void Reset()
		{
			this.keys[0] = 0u;
			this.keys[1] = 0u;
			this.keys[2] = 0u;
		}
	}
}
