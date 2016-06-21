using System;
using UnityEngine;

internal class LCGRand
{
	public static uint Seed = (uint)(UnityEngine.Random.value * 2.14748365E+09f);

	public static float Value01
	{
		get
		{
			return LCGRand.Next() / 4.2949673E+09f;
		}
	}

	private static uint Next()
	{
		LCGRand.Seed = (uint)((ulong)LCGRand.Seed * 279470273uL % (ulong)-5);
		return LCGRand.Seed;
	}

	public static float Range(float min, float max)
	{
		return min + LCGRand.Value01 * (max - min);
	}

	public static int Range(int min, int max)
	{
		return min + (int)(LCGRand.Value01 * (float)(max - min));
	}
}
