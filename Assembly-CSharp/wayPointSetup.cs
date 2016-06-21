using System;
using TheForest.Utils;
using UnityEngine;

public class wayPointSetup : MonoBehaviour
{
	public Transform nextWaypoint;

	public bool stopAtWaypoint;

	public float minWaitTime = 5f;

	public float maxWaitTime = 12f;

	private void Start()
	{
		if (!Scene.SceneTracker.caveWayPoints.Contains(base.transform))
		{
			Scene.SceneTracker.caveWayPoints.Add(base.transform);
		}
	}
}
