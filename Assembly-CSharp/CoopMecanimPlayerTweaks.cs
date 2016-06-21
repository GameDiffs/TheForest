using Bolt;
using System;
using UnityEngine;

public class CoopMecanimPlayerTweaks : EntityBehaviour<IPlayerState>
{
	[SerializeField]
	private Transform targetTransform;

	[SerializeField]
	private CoopMecanimReplicator Replicator;

	[SerializeField]
	private float remoteSpeedAdjustment = 1.2f;

	[SerializeField]
	private float remoteMovementLimit = 0.05f;

	[HideInInspector]
	private Vector3 last_position;

	private int idleHash = Animator.StringToHash("Base Layer.idle");

	private int locoHash = Animator.StringToHash("Base Layer.locomotion");

	private void Awake()
	{
		if (!this.targetTransform)
		{
			this.targetTransform = base.transform;
		}
		if (!BoltNetwork.isRunning)
		{
			base.enabled = false;
		}
	}

	public override void Attached()
	{
		IPlayerState expr_06 = base.state;
		expr_06.OnknockBackTrigger = (Action)Delegate.Combine(expr_06.OnknockBackTrigger, new Action(this.OnKnockbackTrigger));
		this.last_position = this.targetTransform.position;
	}

	private void OnKnockbackTrigger()
	{
		this.Replicator.TargetAnimator.CrossFade("upperBody.hitKnockBack", 0f, 1);
		this.Replicator.TargetAnimator.CrossFade("fullBodyActions.hitKnockBack", 0f, 2);
		for (int i = 0; i < this.Replicator.LayersToSync.Length; i++)
		{
			if (this.Replicator.LayersToSync[i].Name == "upperBody" || this.Replicator.LayersToSync[i].Name == "fullBodyActions")
			{
				this.Replicator.LayersToSync[i].Hash = this.Replicator.LayersToSync[i].Hash_Recv;
			}
		}
	}

	private void Update()
	{
		if (this.entity.IsAttached())
		{
			if (this.entity.IsOwner())
			{
				if (this.Replicator.TargetAnimator.GetBool("knockBackTrigger"))
				{
					base.state.knockBackTrigger();
				}
				base.state.hSpeed = this.Replicator.TargetAnimator.GetFloat("hSpeed");
				base.state.vSpeed = this.Replicator.TargetAnimator.GetFloat("vSpeed");
			}
			else
			{
				if (Vector3.Distance(this.last_position, this.targetTransform.position) > this.remoteMovementLimit)
				{
					this.Replicator.TargetAnimator.SetFloat("hSpeed", Mathf.Clamp(base.state.hSpeed * this.remoteSpeedAdjustment, -2.3f, 2.3f));
					this.Replicator.TargetAnimator.SetFloat("vSpeed", Mathf.Clamp(base.state.vSpeed * this.remoteSpeedAdjustment, -2.3f, 2.3f));
					if (this.Replicator.TargetAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash == this.idleHash && !this.Replicator.TargetAnimator.IsInTransition(0))
					{
						this.Replicator.TargetAnimator.CrossFade("Base Layer.locomotion", 0.05f, 0);
						this.Replicator.LayersToSync[0].Hash_Recv = this.locoHash;
					}
				}
				else
				{
					this.Replicator.TargetAnimator.SetFloat("hSpeed", base.state.hSpeed);
					this.Replicator.TargetAnimator.SetFloat("vSpeed", base.state.vSpeed);
				}
				this.last_position = this.targetTransform.position;
			}
		}
	}

	private void forcePlayerBlock()
	{
		this.Replicator.TargetAnimator.SetBool("stickBlock", true);
		base.Invoke("resetBlock", 0.5f);
	}

	private void resetBlock()
	{
		this.Replicator.TargetAnimator.SetBool("stickBlock", false);
	}
}
