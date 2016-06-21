using Bolt;
using System;
using TheForest.Utils;
using UnityEngine;

public class visRangeSetup : EntityBehaviour<IPlayerState>
{
	public float treeDensity = 1f;

	private float bushOffset;

	public float unscaledVisRange;

	public float modVisRange;

	public float justTreeDensity;

	private float stealthFactor;

	private float darkFactor = 0.75f;

	private float litWeaponOffset;

	private float movementPenalty;

	private float crouchOffset;

	public bool host;

	public float treeCount;

	public float testDist = 30f;

	public float offsetFactor = 1f;

	public bool isTarget;

	public bool currentlyTargetted;

	private playerTargetFunctions tf;

	private int frameOffset;

	public int frameInterval = 10;

	public override void Attached()
	{
		if (!this.entity.IsOwner())
		{
			base.state.AddCallback("TreeDensity", new PropertyCallbackSimple(this.TreeDensityChanged));
			base.state.AddCallback("isTargetting", new PropertyCallbackSimple(this.isTargettingChanged));
		}
	}

	private void TreeDensityChanged()
	{
		this.treeDensity = base.state.TreeDensity;
	}

	private void isTargettingChanged()
	{
		this.currentlyTargetted = base.state.isTargetting;
	}

	private void Start()
	{
		this.tf = LocalPlayer.ScriptSetup.targetFunctions;
		if (this.host && !base.IsInvoking("updateCloseTrees"))
		{
			base.InvokeRepeating("updateCloseTrees", 2f, 2f);
		}
		if (!base.IsInvoking("updateVisParams"))
		{
			base.InvokeRepeating("updateVisParams", 2f, 0.65f);
		}
		if (!BoltNetwork.isClient && !base.IsInvoking("updateTarget"))
		{
			base.InvokeRepeating("updateTarget", 2f, 1.5f);
		}
	}

	private void updateCloseTrees()
	{
		this.treeCount = 0f;
		if (Scene.SceneTracker.allPlayersInCave.Contains(base.gameObject))
		{
			this.treeCount = 9f;
		}
		else
		{
			if (Scene.SceneTracker.closeTrees.Count > 0)
			{
				Scene.SceneTracker.closeTrees.RemoveAll((GameObject o) => o == null);
			}
			for (int i = 0; i < Scene.SceneTracker.closeTrees.Count; i++)
			{
				float sqrMagnitude = (Scene.SceneTracker.closeTrees[i].transform.position - base.transform.position).sqrMagnitude;
				if (sqrMagnitude < this.testDist * this.testDist)
				{
					this.treeCount += 1f;
				}
			}
		}
	}

	private void updateTarget()
	{
		this.isTarget = false;
		base.Invoke("checkCurrentlyTargetted", 1f);
	}

	private void checkCurrentlyTargetted()
	{
		if (!this.isTarget)
		{
			this.currentlyTargetted = false;
		}
		else
		{
			this.currentlyTargetted = true;
		}
	}

	private void updateVisParams()
	{
		if (this.host)
		{
			if (LocalPlayer.FpCharacter.crouching)
			{
				this.crouchOffset = 30f;
			}
			else
			{
				this.crouchOffset = 0f;
			}
			if (LocalPlayer.Inventory.IsWeaponBurning)
			{
				if (Clock.Dark && !Clock.InCave)
				{
					this.litWeaponOffset = 60f;
				}
				else
				{
					this.litWeaponOffset = 30f;
				}
			}
			else
			{
				this.litWeaponOffset = 0f;
			}
			this.stealthFactor = 1f - LocalPlayer.Stats.Stealth / 75f;
			if ((double)LocalPlayer.AnimControl.overallSpeed > 0.5)
			{
				this.movementPenalty = 30f;
			}
			else
			{
				this.movementPenalty = 0f;
			}
			if (Clock.Dark && !Clock.InCave)
			{
				this.darkFactor = 0.65f;
			}
			else
			{
				this.darkFactor = 1f;
			}
			float num = 100f - this.crouchOffset + this.tf.lighterRange - this.bushOffset + this.movementPenalty + this.litWeaponOffset;
			float num2 = Mathf.Clamp(this.treeCount, 0f, 12f);
			num2 /= 12f;
			num2 = 1f - num2;
			num2 = Mathf.Clamp(num2, 0.4f, 0.8f);
			this.modVisRange = num * num2 * this.offsetFactor * this.stealthFactor * this.darkFactor;
			this.modVisRange = Mathf.Clamp(this.modVisRange, 8f, 90f);
			this.unscaledVisRange = this.modVisRange;
		}
		else
		{
			this.modVisRange = this.treeDensity;
		}
		if (BoltNetwork.isRunning && !this.host)
		{
			this.unscaledVisRange = this.treeDensity;
		}
		if (BoltNetwork.isRunning)
		{
			if (this.entity != null && this.entity.isAttached)
			{
				if (this.entity.IsOwner())
				{
					this.treeDensity = this.modVisRange;
					if (this.treeDensity != base.state.TreeDensity)
					{
						base.state.TreeDensity = this.modVisRange;
					}
				}
				else if (this.isTarget != base.state.isTargetting)
				{
					base.state.isTargetting = this.currentlyTargetted;
				}
			}
		}
		else
		{
			this.treeDensity = this.modVisRange;
		}
	}

	private void LateUpdate()
	{
		this.bushOffset = 0f;
	}

	private void OnTriggerStay(Collider other)
	{
		if (!other || !this.host || !LocalPlayer.GameObject)
		{
			return;
		}
		if (other.gameObject.CompareTag("SmallTree"))
		{
			if (LocalPlayer.FpCharacter.crouching)
			{
				this.bushOffset = 35f;
			}
			else
			{
				this.bushOffset = 10f;
			}
		}
	}
}
