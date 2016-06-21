using System;
using TheForest.Utils;
using UnityEngine;

public class addToDragMarker : MonoBehaviour
{
	private void Start()
	{
		if (!Scene.SceneTracker.dragMarkers.Contains(base.transform))
		{
			Scene.SceneTracker.dragMarkers.Add(base.transform);
		}
	}
}
