using Bolt;
using System;
using TheForest.Buildings.World;
using UnityEngine;

public class coopRockThrower : EntityEventListener<IRockThrowerState>
{
	public MultiThrowerItemHolder Holder;

	public rockThrowerAnimEvents Anim;

	public Animator TargetAnimator;

	public GameObject triggerGo;

	public override void Attached()
	{
	}

	private void disableTrigger()
	{
		this.triggerGo.SetActive(false);
	}

	private void enableTrigger()
	{
		this.triggerGo.SetActive(true);
	}

	public void setAnimator(int var, bool onoff)
	{
		if (var == 0)
		{
			this.TargetAnimator.SetBoolReflected("load", onoff);
		}
		else if (var == 1)
		{
			this.TargetAnimator.SetBoolReflected("release", onoff);
		}
	}
}
