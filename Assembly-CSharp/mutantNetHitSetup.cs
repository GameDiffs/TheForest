using System;
using TheForest.Utils;
using UnityEngine;

public class mutantNetHitSetup : MonoBehaviour
{
	private Animator animator;

	private AnimatorStateInfo state;

	private AnimatorStateInfo state1;

	private enemyAnimEvents events;

	private int damagedHash = Animator.StringToHash("damaged");

	private int staggerHash = Animator.StringToHash("hitStagger");

	private int deathHash = Animator.StringToHash("death");

	private int deathOnGroundHash = Animator.StringToHash("deathOnGround");

	private int sleepHash = Animator.StringToHash("sleep");

	private int idleHash = Animator.StringToHash("idle");

	private int runHash = Animator.StringToHash("running");

	public bool playerCollideDisabled;

	private CapsuleCollider controller;

	private void Awake()
	{
		this.controller = base.transform.parent.GetComponent<CapsuleCollider>();
		this.animator = base.GetComponent<Animator>();
		this.events = base.GetComponent<enemyAnimEvents>();
	}

	private void Update()
	{
		this.state = this.animator.GetCurrentAnimatorStateInfo(0);
		this.state1 = this.animator.GetCurrentAnimatorStateInfo(1);
		if ((this.state.tagHash == this.deathHash || this.state.tagHash == this.deathOnGroundHash) && !this.playerCollideDisabled)
		{
			this.disablePlayerCollision();
		}
		else if (this.state.tagHash != this.deathHash && this.state.tagHash != this.deathOnGroundHash && this.playerCollideDisabled)
		{
			this.enablePlayerCollision();
		}
		if (this.state.tagHash == this.damagedHash || this.state.tagHash == this.staggerHash || this.state.tagHash == this.runHash || this.state.tagHash == this.idleHash || this.state.tagHash == this.sleepHash || this.state.tagHash == this.deathHash || this.state.tagHash == this.deathOnGroundHash || this.state1.tagHash == this.damagedHash || this.state1.tagHash == this.staggerHash || this.state1.tagHash == this.deathHash)
		{
			this.events.weaponsBlocked = true;
			this.events.disableClawsWeapon();
			this.events.disableWeapon();
		}
		else
		{
			this.events.weaponsBlocked = false;
		}
	}

	private void LateUpdate()
	{
		if (this.state.tagHash == this.damagedHash || this.state.tagHash == this.staggerHash || this.state.tagHash == this.runHash || this.state.tagHash == this.idleHash || this.state.tagHash == this.sleepHash || this.state.tagHash == this.deathHash || this.state.tagHash == this.deathOnGroundHash || this.state1.tagHash == this.damagedHash || this.state1.tagHash == this.staggerHash || this.state1.tagHash == this.deathHash)
		{
			this.events.weaponsBlocked = true;
			this.events.disableClawsWeapon();
			this.events.disableWeapon();
		}
		else
		{
			this.events.weaponsBlocked = false;
		}
	}

	private void disablePlayerCollision()
	{
		if (LocalPlayer.GameObject && this.controller.enabled)
		{
			if (LocalPlayer.AnimControl.playerCollider.enabled)
			{
				Physics.IgnoreCollision(this.controller, LocalPlayer.AnimControl.playerCollider, true);
			}
			if (LocalPlayer.AnimControl.playerHeadCollider.enabled)
			{
				Physics.IgnoreCollision(this.controller, LocalPlayer.AnimControl.playerHeadCollider, true);
			}
			if (LocalPlayer.AnimControl.enemyCollider.enabled)
			{
				Physics.IgnoreCollision(this.controller, LocalPlayer.AnimControl.enemyCollider, true);
			}
		}
		this.playerCollideDisabled = true;
	}

	private void enablePlayerCollision()
	{
		if (this.playerCollideDisabled)
		{
			if (LocalPlayer.GameObject && this.controller.enabled)
			{
				if (LocalPlayer.AnimControl.playerCollider.enabled)
				{
					Physics.IgnoreCollision(this.controller, LocalPlayer.AnimControl.playerCollider, false);
				}
				if (LocalPlayer.AnimControl.playerHeadCollider.enabled)
				{
					Physics.IgnoreCollision(this.controller, LocalPlayer.AnimControl.playerHeadCollider, false);
				}
				if (LocalPlayer.AnimControl.enemyCollider.enabled)
				{
					Physics.IgnoreCollision(this.controller, LocalPlayer.AnimControl.enemyCollider, false);
				}
			}
			this.playerCollideDisabled = false;
		}
	}
}
