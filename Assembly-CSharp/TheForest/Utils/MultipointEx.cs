using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheForest.Utils
{
	public static class MultipointEx
	{
		public static Vector3 ClosestPointToMultipoint(this List<Vector3> multipoint, Vector3 point)
		{
			Vector3 result = Vector3.zero;
			float num = 3.40282347E+38f;
			bool flag = false;
			for (int i = 1; i < multipoint.Count; i++)
			{
				Vector3 vector;
				if (MathEx.ProjectPointOnLineSegment(multipoint[i - 1], multipoint[i], point, out vector))
				{
					float num2 = Vector3.Distance(point, vector);
					if (num2 < num || flag)
					{
						num = num2;
						result = vector;
						flag = false;
					}
				}
				else if (num == 3.40282347E+38f)
				{
					num = Vector3.Distance(point, vector);
					result = vector;
					flag = true;
				}
			}
			return result;
		}
	}
}
