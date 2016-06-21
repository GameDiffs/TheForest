using System;
using System.Collections.Generic;
using TheForest.Utils;
using UnityEngine;

public class sendStarLocations : MonoBehaviour
{
	public List<Transform> caveStars = new List<Transform>();

	private void OnEnable()
	{
		LocalPlayer.AnimControl.starLocations.Clear();
		for (int i = 0; i < this.caveStars.Count; i++)
		{
			LocalPlayer.AnimControl.starLocations.Add(this.caveStars[i]);
		}
	}
}
