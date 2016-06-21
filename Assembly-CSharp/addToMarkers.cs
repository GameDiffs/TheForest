using System;
using TheForest.Utils;
using UnityEngine;

public class addToMarkers : MonoBehaviour
{
	public bool addToSwim;

	public bool addToBeach;

	public bool addToEntrance;

	public bool addToWaypoint;

	private sceneTracker scene;

	private void Start()
	{
		this.scene = Scene.SceneTracker;
		if (this.addToSwim && !this.scene.swimMarkers.Contains(base.gameObject))
		{
			this.scene.swimMarkers.Add(base.gameObject);
		}
		if (this.addToBeach && !this.scene.beachMarkers.Contains(base.gameObject))
		{
			this.scene.beachMarkers.Add(base.gameObject);
		}
		if (this.addToEntrance && !this.scene.caveMarkers.Contains(base.gameObject))
		{
			this.scene.caveMarkers.Add(base.gameObject);
		}
		if (this.addToWaypoint && !this.scene.waypointMarkers.Contains(base.gameObject))
		{
			this.scene.waypointMarkers.Add(base.gameObject);
		}
	}
}
