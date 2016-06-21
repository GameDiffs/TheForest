using System;
using UnityEngine;

public static class UEx
{
	public static float SqrDistance(Vector3 first, Vector3 second)
	{
		return Vector3.SqrMagnitude(first - second);
	}

	public static Vector3 MidPoint(Vector3 first, Vector3 second)
	{
		return new Vector3((first.x + second.x) * 0.5f, (first.y + second.y) * 0.5f, (first.z + second.z) * 0.5f);
	}

	public static float SqrLineDistance(Vector3 lineP1, Vector3 lineP2, Vector3 point, out int closestPoint)
	{
		Vector3 vector = lineP2 - lineP1;
		Vector3 lhs = point - lineP1;
		float num = Vector3.Dot(lhs, vector);
		if (num <= 0f)
		{
			closestPoint = 1;
			return UEx.SqrDistance(point, lineP1);
		}
		float num2 = Vector3.Dot(vector, vector);
		if (num2 <= num)
		{
			closestPoint = 2;
			return UEx.SqrDistance(point, lineP2);
		}
		float d = num / num2;
		Vector3 second = lineP1 + d * vector;
		closestPoint = 4;
		return UEx.SqrDistance(point, second);
	}
}
