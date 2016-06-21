using Bolt;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class CoopRaft : EntityBehaviour<IRaftState>
{
	private bool interpolate;

	private GameObject client_transform;

	[SerializeField]
	private Rigidbody raft_rigidbody;

	public override void Attached()
	{
		if (this.entity.IsOwner())
		{
			base.state.Transform.SetTransforms(base.transform);
		}
		else
		{
			this.raft_rigidbody.isKinematic = true;
			this.client_transform = new GameObject("client_raft_transform");
			base.state.Transform.SetTransforms(this.client_transform.transform);
			base.StartCoroutine(this.ClientPushEnabler(1f));
		}
	}

	public override void Detached()
	{
		if (this.client_transform)
		{
			UnityEngine.Object.Destroy(this.client_transform);
		}
	}

	private bool IsBeingDriven()
	{
		return this.entity.IsAttached() && base.state.GrabbedBy;
	}

	private void Start()
	{
		base.enabled = BoltNetwork.isRunning;
	}

	private void LateUpdate()
	{
		if (this.entity.IsAttached() && !this.entity.IsOwner())
		{
			if (this.interpolate)
			{
				this.raft_rigidbody.isKinematic = this.IsBeingDriven();
				base.transform.position = Vector3.Slerp(base.transform.position, this.client_transform.transform.position, Time.deltaTime);
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, this.client_transform.transform.rotation, Time.deltaTime);
			}
			else
			{
				base.transform.position = this.client_transform.transform.position;
				base.transform.rotation = this.client_transform.transform.rotation;
			}
		}
	}

	[DebuggerHidden]
	private IEnumerator ClientPushEnabler(float waitTime)
	{
		CoopRaft.<ClientPushEnabler>c__Iterator20 <ClientPushEnabler>c__Iterator = new CoopRaft.<ClientPushEnabler>c__Iterator20();
		<ClientPushEnabler>c__Iterator.waitTime = waitTime;
		<ClientPushEnabler>c__Iterator.<$>waitTime = waitTime;
		<ClientPushEnabler>c__Iterator.<>f__this = this;
		return <ClientPushEnabler>c__Iterator;
	}
}
