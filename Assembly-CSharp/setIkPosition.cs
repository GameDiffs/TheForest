using System;
using System.Collections.Generic;
using TheForest.Utils;
using UnityEngine;

public class setIkPosition : MonoBehaviour
{
	public Transform ikTarget;

	public List<Transform> ikPos = new List<Transform>();

	private float closestDist;

	private Transform closestTr;

	private int closestPos;

	public float speed;

	public bool forcePos;

	public int setPos;

	private void Start()
	{
	}

	private void OnEnable()
	{
		if (!base.IsInvoking("doIKPos"))
		{
			base.InvokeRepeating("doIKPos", 0f, 2f);
		}
	}

	private void OnDisable()
	{
		base.CancelInvoke("doIKPos");
	}

	private void Update()
	{
		if (base.enabled)
		{
			if (this.forcePos)
			{
				this.ikTarget.position = Vector3.Slerp(this.ikTarget.position, this.ikPos[this.setPos].position, Time.deltaTime * this.speed);
			}
			else
			{
				this.ikTarget.position = Vector3.Slerp(this.ikTarget.position, this.ikPos[this.closestPos].position, Time.deltaTime * this.speed);
			}
		}
	}

	private void doIKPos()
	{
		this.closestDist = float.PositiveInfinity;
		for (int i = 0; i < LocalPlayer.AnimControl.starLocations.Count; i++)
		{
			if (LocalPlayer.AnimControl.starLocations[i] != null)
			{
				float sqrMagnitude = (LocalPlayer.Transform.position - LocalPlayer.AnimControl.starLocations[i].position).sqrMagnitude;
				if (sqrMagnitude < this.closestDist)
				{
					this.closestPos = i;
					this.closestTr = this.ikPos[i];
					this.closestDist = sqrMagnitude;
				}
			}
		}
	}
}
