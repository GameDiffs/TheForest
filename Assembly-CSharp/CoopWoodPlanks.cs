using System;
using UnityEngine;

public class CoopWoodPlanks : MonoBehaviour
{
	public static CoopWoodPlanks Instance;

	public BreakWoodSimple[] Planks;

	private void Awake()
	{
		CoopWoodPlanks.Instance = this;
	}

	public int GetIndex(BreakWoodSimple wood)
	{
		if (wood)
		{
			for (int i = 0; i < this.Planks.Length; i++)
			{
				if (object.ReferenceEquals(wood, this.Planks[i]))
				{
					return i;
				}
			}
		}
		return -1;
	}
}
