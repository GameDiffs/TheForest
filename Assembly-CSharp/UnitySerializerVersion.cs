using System;
using UnityEngine;

public class UnitySerializerVersion : MonoBehaviour
{
	public static int[] version;

	static UnitySerializerVersion()
	{
		// Note: this type is marked as 'beforefieldinit'.
		int[] expr_06 = new int[3];
		expr_06[0] = 2;
		expr_06[1] = 5;
		UnitySerializerVersion.version = expr_06;
	}
}
