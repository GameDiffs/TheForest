using System;
using System.Threading;
using UnityEngine;

[Serializable]
public class JPGEncoder_class
{
	private int[] ZigZag;

	private int[] YTable;

	private int[] UVTable;

	private float[] fdtbl_Y;

	private float[] fdtbl_UV;

	private BitString[] YDC_HT;

	private BitString[] UVDC_HT;

	private BitString[] YAC_HT;

	private BitString[] UVAC_HT;

	private int[] std_dc_luminance_nrcodes;

	private int[] std_dc_luminance_values;

	private int[] std_ac_luminance_nrcodes;

	private int[] std_ac_luminance_values;

	private int[] std_dc_chrominance_nrcodes;

	private int[] std_dc_chrominance_values;

	private int[] std_ac_chrominance_nrcodes;

	private int[] std_ac_chrominance_values;

	private BitString[] bitcode;

	private int[] category;

	private int bytenew;

	private int bytepos;

	private ByteArray byteout;

	private int[] DU;

	private float[] YDU;

	private float[] UDU;

	private float[] VDU;

	public bool isDone;

	private BitmapData image;

	private int sf;

	public JPGEncoder_class(Texture2D texture, float quality)
	{
		this.ZigZag = new int[]
		{
			0,
			1,
			5,
			6,
			14,
			15,
			27,
			28,
			2,
			4,
			7,
			13,
			16,
			26,
			29,
			42,
			3,
			8,
			12,
			17,
			25,
			30,
			41,
			43,
			9,
			11,
			18,
			24,
			31,
			40,
			44,
			53,
			10,
			19,
			23,
			32,
			39,
			45,
			52,
			54,
			20,
			22,
			33,
			38,
			46,
			51,
			55,
			60,
			21,
			34,
			37,
			47,
			50,
			56,
			59,
			61,
			35,
			36,
			48,
			49,
			57,
			58,
			62,
			63
		};
		this.YTable = new int[64];
		this.UVTable = new int[64];
		this.fdtbl_Y = new float[64];
		this.fdtbl_UV = new float[64];
		this.std_dc_luminance_nrcodes = new int[]
		{
			0,
			0,
			1,
			5,
			1,
			1,
			1,
			1,
			1,
			1,
			0,
			0,
			0,
			0,
			0,
			0,
			0
		};
		this.std_dc_luminance_values = new int[]
		{
			0,
			1,
			2,
			3,
			4,
			5,
			6,
			7,
			8,
			9,
			10,
			11
		};
		this.std_ac_luminance_nrcodes = new int[]
		{
			0,
			0,
			2,
			1,
			3,
			3,
			2,
			4,
			3,
			5,
			5,
			4,
			4,
			0,
			0,
			1,
			125
		};
		this.std_ac_luminance_values = new int[]
		{
			1,
			2,
			3,
			0,
			4,
			17,
			5,
			18,
			33,
			49,
			65,
			6,
			19,
			81,
			97,
			7,
			34,
			113,
			20,
			50,
			129,
			145,
			161,
			8,
			35,
			66,
			177,
			193,
			21,
			82,
			209,
			240,
			36,
			51,
			98,
			114,
			130,
			9,
			10,
			22,
			23,
			24,
			25,
			26,
			37,
			38,
			39,
			40,
			41,
			42,
			52,
			53,
			54,
			55,
			56,
			57,
			58,
			67,
			68,
			69,
			70,
			71,
			72,
			73,
			74,
			83,
			84,
			85,
			86,
			87,
			88,
			89,
			90,
			99,
			100,
			101,
			102,
			103,
			104,
			105,
			106,
			115,
			116,
			117,
			118,
			119,
			120,
			121,
			122,
			131,
			132,
			133,
			134,
			135,
			136,
			137,
			138,
			146,
			147,
			148,
			149,
			150,
			151,
			152,
			153,
			154,
			162,
			163,
			164,
			165,
			166,
			167,
			168,
			169,
			170,
			178,
			179,
			180,
			181,
			182,
			183,
			184,
			185,
			186,
			194,
			195,
			196,
			197,
			198,
			199,
			200,
			201,
			202,
			210,
			211,
			212,
			213,
			214,
			215,
			216,
			217,
			218,
			225,
			226,
			227,
			228,
			229,
			230,
			231,
			232,
			233,
			234,
			241,
			242,
			243,
			244,
			245,
			246,
			247,
			248,
			249,
			250
		};
		this.std_dc_chrominance_nrcodes = new int[]
		{
			0,
			0,
			3,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			0,
			0,
			0,
			0,
			0
		};
		this.std_dc_chrominance_values = new int[]
		{
			0,
			1,
			2,
			3,
			4,
			5,
			6,
			7,
			8,
			9,
			10,
			11
		};
		this.std_ac_chrominance_nrcodes = new int[]
		{
			0,
			0,
			2,
			1,
			2,
			4,
			4,
			3,
			4,
			7,
			5,
			4,
			4,
			0,
			1,
			2,
			119
		};
		this.std_ac_chrominance_values = new int[]
		{
			0,
			1,
			2,
			3,
			17,
			4,
			5,
			33,
			49,
			6,
			18,
			65,
			81,
			7,
			97,
			113,
			19,
			34,
			50,
			129,
			8,
			20,
			66,
			145,
			161,
			177,
			193,
			9,
			35,
			51,
			82,
			240,
			21,
			98,
			114,
			209,
			10,
			22,
			36,
			52,
			225,
			37,
			241,
			23,
			24,
			25,
			26,
			38,
			39,
			40,
			41,
			42,
			53,
			54,
			55,
			56,
			57,
			58,
			67,
			68,
			69,
			70,
			71,
			72,
			73,
			74,
			83,
			84,
			85,
			86,
			87,
			88,
			89,
			90,
			99,
			100,
			101,
			102,
			103,
			104,
			105,
			106,
			115,
			116,
			117,
			118,
			119,
			120,
			121,
			122,
			130,
			131,
			132,
			133,
			134,
			135,
			136,
			137,
			138,
			146,
			147,
			148,
			149,
			150,
			151,
			152,
			153,
			154,
			162,
			163,
			164,
			165,
			166,
			167,
			168,
			169,
			170,
			178,
			179,
			180,
			181,
			182,
			183,
			184,
			185,
			186,
			194,
			195,
			196,
			197,
			198,
			199,
			200,
			201,
			202,
			210,
			211,
			212,
			213,
			214,
			215,
			216,
			217,
			218,
			226,
			227,
			228,
			229,
			230,
			231,
			232,
			233,
			234,
			242,
			243,
			244,
			245,
			246,
			247,
			248,
			249,
			250
		};
		this.bitcode = new BitString[65535];
		this.category = new int[65535];
		this.bytepos = 7;
		this.byteout = new ByteArray();
		this.DU = new int[64];
		this.YDU = new float[64];
		this.UDU = new float[64];
		this.VDU = new float[64];
		this.image = new BitmapData(texture);
		if (quality <= (float)0)
		{
			quality = (float)1;
		}
		if (quality > (float)100)
		{
			quality = (float)100;
		}
		if (quality < (float)50)
		{
			this.sf = (int)((float)5000 / quality);
		}
		else
		{
			this.sf = (int)((float)200 - quality * (float)2);
		}
		Thread thread = new Thread(new ThreadStart(this.doEncoding));
		thread.Start();
	}

	private void initQuantTables(int sf)
	{
		int i = 0;
		float num = 0f;
		int[] array = new int[]
		{
			16,
			11,
			10,
			16,
			24,
			40,
			51,
			61,
			12,
			12,
			14,
			19,
			26,
			58,
			60,
			55,
			14,
			13,
			16,
			24,
			40,
			57,
			69,
			56,
			14,
			17,
			22,
			29,
			51,
			87,
			80,
			62,
			18,
			22,
			37,
			56,
			68,
			109,
			103,
			77,
			24,
			35,
			55,
			64,
			81,
			104,
			113,
			92,
			49,
			64,
			78,
			87,
			103,
			121,
			120,
			101,
			72,
			92,
			95,
			98,
			112,
			100,
			103,
			99
		};
		for (i = 0; i < 64; i++)
		{
			num = Mathf.Floor((float)((array[i] * sf + 50) / 100));
			if (num < (float)1)
			{
				num = (float)1;
			}
			else if (num > (float)255)
			{
				num = (float)255;
			}
			this.YTable[this.ZigZag[i]] = (int)num;
		}
		int[] array2 = new int[]
		{
			17,
			18,
			24,
			47,
			99,
			99,
			99,
			99,
			18,
			21,
			26,
			66,
			99,
			99,
			99,
			99,
			24,
			26,
			56,
			99,
			99,
			99,
			99,
			99,
			47,
			66,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99,
			99
		};
		for (i = 0; i < 64; i++)
		{
			num = Mathf.Floor((float)((array2[i] * sf + 50) / 100));
			if (num < (float)1)
			{
				num = (float)1;
			}
			else if (num > (float)255)
			{
				num = (float)255;
			}
			this.UVTable[this.ZigZag[i]] = (int)num;
		}
		float[] array3 = new float[]
		{
			1f,
			1.3870399f,
			1.306563f,
			1.17587554f,
			1f,
			0.785694957f,
			0.5411961f,
			0.27589938f
		};
		i = 0;
		for (int j = 0; j < 8; j++)
		{
			for (int k = 0; k < 8; k++)
			{
				this.fdtbl_Y[i] = 1f / ((float)this.YTable[this.ZigZag[i]] * array3[j] * array3[k] * 8f);
				this.fdtbl_UV[i] = 1f / ((float)this.UVTable[this.ZigZag[i]] * array3[j] * array3[k] * 8f);
				i++;
			}
		}
	}

	private BitString[] computeHuffmanTbl(int[] nrcodes, int[] std_table)
	{
		int num = 0;
		int num2 = 0;
		BitString[] array = new BitString[256];
		for (int i = 1; i <= 16; i++)
		{
			for (int j = 1; j <= nrcodes[i]; j++)
			{
				array[std_table[num2]] = new BitString();
				array[std_table[num2]].val = num;
				array[std_table[num2]].len = i;
				num2++;
				num++;
			}
			num *= 2;
		}
		return array;
	}

	private void initHuffmanTbl()
	{
		this.YDC_HT = this.computeHuffmanTbl(this.std_dc_luminance_nrcodes, this.std_dc_luminance_values);
		this.UVDC_HT = this.computeHuffmanTbl(this.std_dc_chrominance_nrcodes, this.std_dc_chrominance_values);
		this.YAC_HT = this.computeHuffmanTbl(this.std_ac_luminance_nrcodes, this.std_ac_luminance_values);
		this.UVAC_HT = this.computeHuffmanTbl(this.std_ac_chrominance_nrcodes, this.std_ac_chrominance_values);
	}

	private void initCategoryfloat()
	{
		int num = 1;
		int num2 = 2;
		int i = 0;
		for (int j = 1; j <= 15; j++)
		{
			for (i = num; i < num2; i++)
			{
				this.category[32767 + i] = j;
				BitString bitString = new BitString();
				bitString.len = j;
				bitString.val = i;
				this.bitcode[32767 + i] = bitString;
			}
			for (i = -(num2 - 1); i <= -num; i++)
			{
				this.category[32767 + i] = j;
				BitString bitString = new BitString();
				bitString.len = j;
				bitString.val = num2 - 1 + i;
				this.bitcode[32767 + i] = bitString;
			}
			num <<= 1;
			num2 <<= 1;
		}
	}

	public override byte[] GetBytes()
	{
		byte[] arg_28_0;
		if (!this.isDone)
		{
			Debug.LogError("JPEGEncoder not complete, cannot get bytes!");
			arg_28_0 = null;
		}
		else
		{
			arg_28_0 = this.byteout.GetAllBytes();
		}
		return arg_28_0;
	}

	private void writeBits(BitString bs)
	{
		int val = bs.val;
		int i = bs.len - 1;
		while (i >= 0)
		{
			if ((int)((long)val & (long)Convert.ToUInt32(1 << i)) != 0)
			{
				this.bytenew = (int)((long)this.bytenew | (long)Convert.ToUInt32(1 << this.bytepos));
			}
			i--;
			this.bytepos--;
			if (this.bytepos < 0)
			{
				if (this.bytenew == 255)
				{
					this.writeByte(255);
					this.writeByte(0);
				}
				else
				{
					this.writeByte((byte)this.bytenew);
				}
				this.bytepos = 7;
				this.bytenew = 0;
			}
		}
	}

	private void writeByte(byte value)
	{
		this.byteout.writeByte(value);
	}

	private void writeWord(int value)
	{
		this.writeByte((byte)(value >> 8 & 255));
		this.writeByte((byte)(value & 255));
	}

	private float[] fDCTQuant(float[] data, float[] fdtbl)
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		float num5 = 0f;
		float num6 = 0f;
		float num7 = 0f;
		float num8 = 0f;
		float num9 = 0f;
		float num10 = 0f;
		float num11 = 0f;
		float num12 = 0f;
		float num13 = 0f;
		float num14 = 0f;
		float num15 = 0f;
		float num16 = 0f;
		float num17 = 0f;
		float num18 = 0f;
		float num19 = 0f;
		int i = 0;
		int num20 = 0;
		for (i = 0; i < 8; i++)
		{
			num = data[num20 + 0] + data[num20 + 7];
			num8 = data[num20 + 0] - data[num20 + 7];
			num2 = data[num20 + 1] + data[num20 + 6];
			num7 = data[num20 + 1] - data[num20 + 6];
			num3 = data[num20 + 2] + data[num20 + 5];
			num6 = data[num20 + 2] - data[num20 + 5];
			num4 = data[num20 + 3] + data[num20 + 4];
			num5 = data[num20 + 3] - data[num20 + 4];
			num9 = num + num4;
			num12 = num - num4;
			num10 = num2 + num3;
			num11 = num2 - num3;
			data[num20 + 0] = num9 + num10;
			data[num20 + 4] = num9 - num10;
			num13 = (num11 + num12) * 0.707106769f;
			data[num20 + 2] = num12 + num13;
			data[num20 + 6] = num12 - num13;
			num9 = num5 + num6;
			num10 = num6 + num7;
			num11 = num7 + num8;
			num17 = (num9 - num11) * 0.382683426f;
			num14 = 0.5411961f * num9 + num17;
			num16 = 1.306563f * num11 + num17;
			num15 = num10 * 0.707106769f;
			num18 = num8 + num15;
			num19 = num8 - num15;
			data[num20 + 5] = num19 + num14;
			data[num20 + 3] = num19 - num14;
			data[num20 + 1] = num18 + num16;
			data[num20 + 7] = num18 - num16;
			num20 += 8;
		}
		num20 = 0;
		for (i = 0; i < 8; i++)
		{
			num = data[num20 + 0] + data[num20 + 56];
			num8 = data[num20 + 0] - data[num20 + 56];
			num2 = data[num20 + 8] + data[num20 + 48];
			num7 = data[num20 + 8] - data[num20 + 48];
			num3 = data[num20 + 16] + data[num20 + 40];
			num6 = data[num20 + 16] - data[num20 + 40];
			num4 = data[num20 + 24] + data[num20 + 32];
			num5 = data[num20 + 24] - data[num20 + 32];
			num9 = num + num4;
			num12 = num - num4;
			num10 = num2 + num3;
			num11 = num2 - num3;
			data[num20 + 0] = num9 + num10;
			data[num20 + 32] = num9 - num10;
			num13 = (num11 + num12) * 0.707106769f;
			data[num20 + 16] = num12 + num13;
			data[num20 + 48] = num12 - num13;
			num9 = num5 + num6;
			num10 = num6 + num7;
			num11 = num7 + num8;
			num17 = (num9 - num11) * 0.382683426f;
			num14 = 0.5411961f * num9 + num17;
			num16 = 1.306563f * num11 + num17;
			num15 = num10 * 0.707106769f;
			num18 = num8 + num15;
			num19 = num8 - num15;
			data[num20 + 40] = num19 + num14;
			data[num20 + 24] = num19 - num14;
			data[num20 + 8] = num18 + num16;
			data[num20 + 56] = num18 - num16;
			num20++;
		}
		for (i = 0; i < 64; i++)
		{
			data[i] = Mathf.Round(data[i] * fdtbl[i]);
		}
		return data;
	}

	private void writeAPP0()
	{
		this.writeWord(65504);
		this.writeWord(16);
		this.writeByte(74);
		this.writeByte(70);
		this.writeByte(73);
		this.writeByte(70);
		this.writeByte(0);
		this.writeByte(1);
		this.writeByte(1);
		this.writeByte(0);
		this.writeWord(1);
		this.writeWord(1);
		this.writeByte(0);
		this.writeByte(0);
	}

	private void writeSOF0(int width, int height)
	{
		this.writeWord(65472);
		this.writeWord(17);
		this.writeByte(8);
		this.writeWord(height);
		this.writeWord(width);
		this.writeByte(3);
		this.writeByte(1);
		this.writeByte(17);
		this.writeByte(0);
		this.writeByte(2);
		this.writeByte(17);
		this.writeByte(1);
		this.writeByte(3);
		this.writeByte(17);
		this.writeByte(1);
	}

	private void writeDQT()
	{
		this.writeWord(65499);
		this.writeWord(132);
		this.writeByte(0);
		int i = 0;
		for (i = 0; i < 64; i++)
		{
			this.writeByte((byte)this.YTable[i]);
		}
		this.writeByte(1);
		for (i = 0; i < 64; i++)
		{
			this.writeByte((byte)this.UVTable[i]);
		}
	}

	private void writeDHT()
	{
		this.writeWord(65476);
		this.writeWord(418);
		int i = 0;
		this.writeByte(0);
		for (i = 0; i < 16; i++)
		{
			this.writeByte((byte)this.std_dc_luminance_nrcodes[i + 1]);
		}
		for (i = 0; i <= 11; i++)
		{
			this.writeByte((byte)this.std_dc_luminance_values[i]);
		}
		this.writeByte(16);
		for (i = 0; i < 16; i++)
		{
			this.writeByte((byte)this.std_ac_luminance_nrcodes[i + 1]);
		}
		for (i = 0; i <= 161; i++)
		{
			this.writeByte((byte)this.std_ac_luminance_values[i]);
		}
		this.writeByte(1);
		for (i = 0; i < 16; i++)
		{
			this.writeByte((byte)this.std_dc_chrominance_nrcodes[i + 1]);
		}
		for (i = 0; i <= 11; i++)
		{
			this.writeByte((byte)this.std_dc_chrominance_values[i]);
		}
		this.writeByte(17);
		for (i = 0; i < 16; i++)
		{
			this.writeByte((byte)this.std_ac_chrominance_nrcodes[i + 1]);
		}
		for (i = 0; i <= 161; i++)
		{
			this.writeByte((byte)this.std_ac_chrominance_values[i]);
		}
	}

	private void writeSOS()
	{
		this.writeWord(65498);
		this.writeWord(12);
		this.writeByte(3);
		this.writeByte(1);
		this.writeByte(0);
		this.writeByte(2);
		this.writeByte(17);
		this.writeByte(3);
		this.writeByte(17);
		this.writeByte(0);
		this.writeByte(63);
		this.writeByte(0);
	}

	private float processDU(float[] CDU, float[] fdtbl, float DC, BitString[] HTDC, BitString[] HTAC)
	{
		BitString bs = HTAC[0];
		BitString bs2 = HTAC[240];
		int i = 0;
		float[] array = this.fDCTQuant(CDU, fdtbl);
		for (i = 0; i < 64; i++)
		{
			this.DU[this.ZigZag[i]] = (int)array[i];
		}
		int num = (int)((float)this.DU[0] - DC);
		DC = (float)this.DU[0];
		if (num == 0)
		{
			this.writeBits(HTDC[0]);
		}
		else
		{
			this.writeBits(HTDC[this.category[32767 + num]]);
			this.writeBits(this.bitcode[32767 + num]);
		}
		int num2 = 63;
		while (num2 > 0 && this.DU[num2] == 0)
		{
			num2--;
		}
		float arg_1A8_0;
		if (num2 == 0)
		{
			this.writeBits(bs);
			arg_1A8_0 = DC;
		}
		else
		{
			for (i = 1; i <= num2; i++)
			{
				int num3 = i;
				while (this.DU[i] == 0 && i <= num2)
				{
					i++;
				}
				int num4 = i - num3;
				if (num4 >= 16)
				{
					for (int j = 1; j <= num4 / 16; j++)
					{
						this.writeBits(bs2);
					}
					num4 &= 15;
				}
				this.writeBits(HTAC[num4 * 16 + this.category[32767 + this.DU[i]]]);
				this.writeBits(this.bitcode[32767 + this.DU[i]]);
			}
			if (num2 != 63)
			{
				this.writeBits(bs);
			}
			arg_1A8_0 = DC;
		}
		return arg_1A8_0;
	}

	private void RGB2YUV(BitmapData img, int xpos, int ypos)
	{
		int num = 0;
		for (int i = 0; i < 8; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				Color pixelColor = img.getPixelColor(xpos + j, img.height - (ypos + i));
				float num2 = pixelColor.r * (float)255;
				float num3 = pixelColor.g * (float)255;
				float num4 = pixelColor.b * (float)255;
				this.YDU[num] = 0.299f * num2 + 0.587f * num3 + 0.114f * num4 - (float)128;
				this.UDU[num] = -0.16874f * num2 + -0.33126f * num3 + 0.5f * num4;
				this.VDU[num] = 0.5f * num2 + -0.41869f * num3 + -0.08131f * num4;
				num++;
			}
		}
	}

	private void doEncoding()
	{
		this.isDone = false;
		Thread.Sleep(5);
		this.initHuffmanTbl();
		this.initCategoryfloat();
		this.initQuantTables(this.sf);
		this.encode();
		this.isDone = true;
		this.image = null;
		Thread.CurrentThread.Abort();
	}

	private void encode()
	{
		this.byteout = new ByteArray();
		this.bytenew = 0;
		this.bytepos = 7;
		this.writeWord(65496);
		this.writeAPP0();
		this.writeDQT();
		this.writeSOF0(this.image.width, this.image.height);
		this.writeDHT();
		this.writeSOS();
		float dC = (float)0;
		float dC2 = (float)0;
		float dC3 = (float)0;
		this.bytenew = 0;
		this.bytepos = 7;
		for (int i = 0; i < this.image.height; i += 8)
		{
			for (int j = 0; j < this.image.width; j += 8)
			{
				this.RGB2YUV(this.image, j, i);
				dC = this.processDU(this.YDU, this.fdtbl_Y, dC, this.YDC_HT, this.YAC_HT);
				dC2 = this.processDU(this.UDU, this.fdtbl_UV, dC2, this.UVDC_HT, this.UVAC_HT);
				dC3 = this.processDU(this.VDU, this.fdtbl_UV, dC3, this.UVDC_HT, this.UVAC_HT);
				Thread.Sleep(0);
			}
		}
		if (this.bytepos >= 0)
		{
			this.writeBits(new BitString
			{
				len = this.bytepos + 1,
				val = (1 << this.bytepos + 1) - 1
			});
		}
		this.writeWord(65497);
		this.isDone = true;
	}
}
