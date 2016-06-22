using Bolt;
using System;
using UnityEngine;

public class CoopGridObject : EntityBehaviour
{
	private float updateTime;

	[HideInInspector]
	public int CurrentNode;

	[SerializeField]
	public bool Dynamic;

	[SerializeField]
	public float DynamicUpdateTime = 1f;

	private void Awake()
	{
		this.CurrentNode = -1;
		base.enabled = false;
	}

	private void Update()
	{
		if (this.Dynamic && BoltNetwork.isServer)
		{
			if ((this.updateTime -= Time.deltaTime) <= 0f)
			{
				CoopTreeGrid.UpdateObject(this);
				this.updateTime = this.DynamicUpdateTime;
			}
		}
		else
		{
			base.enabled = false;
		}
	}

	public override void Attached()
	{
		if (BoltNetwork.isServer)
		{
			CoopTreeGrid.RegisterObject(this);
			base.enabled = this.Dynamic;
		}
	}

	public override void Detached()
	{
		if (BoltNetwork.isServer)
		{
			CoopTreeGrid.RemoveObject(this);
		}
	}
}
