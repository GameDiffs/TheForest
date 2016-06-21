using System;
using TheForest.Utils;
using UnityEngine;

public class addToDrinkMarker : MonoBehaviour
{
	private void Start()
	{
		if (!Scene.SceneTracker.drinkMarkers.Contains(base.transform))
		{
			Scene.SceneTracker.drinkMarkers.Add(base.transform);
		}
	}
}
