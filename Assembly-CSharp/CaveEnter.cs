using System;
using UnityEngine;

public class CaveEnter : MonoBehaviour
{
	public Terrain MainTerrain;

	private float distance;

	private Transform MyLocation;

	private bool Open;

	private void Awake()
	{
		this.MyLocation = base.transform;
	}

	private void FixedUpdate()
	{
		this.distance = (this.MyLocation.position - PlayerCamLocation.PlayerLoc).magnitude;
		if (this.distance > 6f || !this.Open)
		{
		}
		if ((double)this.distance <= 6.0 || this.Open)
		{
		}
	}
}
