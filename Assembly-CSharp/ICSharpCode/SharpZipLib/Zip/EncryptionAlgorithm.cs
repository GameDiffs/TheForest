using System;

namespace ICSharpCode.SharpZipLib.Zip
{
	public enum EncryptionAlgorithm
	{
		None,
		PkzipClassic,
		Des = 26113,
		RC2,
		TripleDes168,
		TripleDes112 = 26121,
		Aes128 = 26126,
		Aes192,
		Aes256,
		RC2Corrected = 26370,
		Blowfish = 26400,
		Twofish,
		RC4 = 26625,
		Unknown = 65535
	}
}
