using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheForest.Utils
{
	public static class MultipointUtils
	{
		public static Vector3 CenterOf(List<Vector3> multipoint)
		{
			float num = 0f;
			float num2 = 0f;
			for (int i = 0; i < multipoint.Count; i++)
			{
				Vector3 vector = multipoint[i];
				num += vector.x;
				num2 += vector.z;
			}
			float x = num / (float)multipoint.Count;
			float z = num2 / (float)multipoint.Count;
			return new Vector3(x, multipoint[0].y, z);
		}
	}
}
