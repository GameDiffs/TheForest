using Bolt;
using System;
using UnityEngine;

public class BoltHeldItem : EntityBehaviour
{
	[SerializeField]
	public int Id;

	private void OnEnable()
	{
		if (!BoltNetwork.isRunning || this.entity.isOwner)
		{
		}
	}

	private void OnDisable()
	{
		if (!BoltNetwork.isRunning || this.entity.isOwner)
		{
		}
	}
}
