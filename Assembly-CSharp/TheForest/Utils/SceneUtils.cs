using System;
using UnityEngine;

namespace TheForest.Utils
{
	public static class SceneUtils
	{
		public static long PositionToLongHash(Vector3 position)
		{
			return (long)(Mathf.RoundToInt(position.x * 10f) + Mathf.RoundToInt(position.y * 10f) * 1000000) + (long)Mathf.RoundToInt(position.y * 10f) * 1000000000000L;
		}
	}
}
