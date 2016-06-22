using Bolt;
using System;
using TheForest.Utils;
using UnityEngine;

public class CoopRaftPusher2 : EntityBehaviour<IRaftState>
{
	private Transform enteredTransform;

	private int playersClose;

	[SerializeField]
	private GameObject billboardIcon;

	[SerializeField]
	private Rigidbody raftMainBody;

	[SerializeField]
	private Transform raftMainTransform;

	[SerializeField]
	private RaftPush raftPush;

	[SerializeField]
	private float pushForce = 20f;

	[SerializeField]
	private ForceMode pushForceMode = ForceMode.VelocityChange;

	[SerializeField]
	private float iconPositionAlpha = 0.5f;

	private void Awake()
	{
		base.enabled = !BoltNetwork.isRunning;
	}

	public override void Attached()
	{
		base.enabled = true;
	}

	private void Update()
	{
		if ((!BoltNetwork.isRunning || BoltNetwork.isServer) && this.raftPush.InWater && this.raftMainBody.constraints != RigidbodyConstraints.None)
		{
			this.raftMainBody.constraints = RigidbodyConstraints.None;
		}
		if (this.enteredTransform && (!BoltNetwork.isRunning || !base.state.GrabbedBy) && !LocalPlayer.AnimControl.swimming)
		{
			if (!this.billboardIcon.activeSelf)
			{
				this.billboardIcon.SetActive(true);
			}
			if (TheForest.Utils.Input.GetButtonDown("Take"))
			{
				Vector3 vector = this.raftMainTransform.position - this.enteredTransform.position;
				if (BoltNetwork.isRunning)
				{
					PushRaft pushRaft = global::PushRaft.Create(GlobalTargets.OnlyServer);
					pushRaft.Direction = vector;
					pushRaft.Raft = this.entity;
					pushRaft.Send();
				}
				else
				{
					this.PushRaft(vector);
				}
			}
		}
		else if (this.billboardIcon.activeSelf)
		{
			this.billboardIcon.SetActive(false);
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (base.enabled)
		{
			if (collider.gameObject.CompareTag("Player"))
			{
				this.enteredTransform = collider.transform;
				this.billboardIcon.transform.position = Vector3.Lerp(base.transform.position, collider.transform.position, this.iconPositionAlpha);
			}
			if ((collider.gameObject.CompareTag("Player") || collider.gameObject.CompareTag("PlayerNet")) && !BoltNetwork.isClient && ++this.playersClose > 0)
			{
				this.raftMainBody.constraints = RigidbodyConstraints.FreezeAll;
			}
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		if (base.enabled)
		{
			if (collider.gameObject.CompareTag("Player"))
			{
				this.enteredTransform = null;
				this.billboardIcon.SetActive(false);
			}
			if ((collider.gameObject.CompareTag("Player") || collider.gameObject.CompareTag("PlayerNet")) && !BoltNetwork.isClient && --this.playersClose < 1)
			{
				this.raftMainBody.constraints = RigidbodyConstraints.None;
			}
		}
	}

	public void PushRaft(Vector3 worldDirection)
	{
		this.raftMainBody.constraints = RigidbodyConstraints.None;
		if (BoltNetwork.isRunning && this.entity.IsAttached() && base.state.GrabbedBy)
		{
			return;
		}
		this.raftMainBody.AddForce(worldDirection.normalized * this.pushForce, this.pushForceMode);
	}
}
