using System;
using UnityEngine;

namespace PathologicalGames
{
	public static class PoolManagerUtils
	{
		internal static void SetActive(GameObject obj, bool state)
		{
			obj.SetActive(state);
		}

		internal static bool activeInHierarchy(GameObject obj)
		{
			return obj.activeInHierarchy;
		}
	}
}
