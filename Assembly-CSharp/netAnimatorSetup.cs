using Bolt;
using System;
using UnityEngine;

public class netAnimatorSetup : EntityEventListener<IPlayerState>
{
	private Animator animator;

	private float lyr1;

	private float lyr2;

	public AnimationClip crouchIdle;

	private void Start()
	{
		if (BoltNetwork.isRunning)
		{
			base.Invoke("fixRootRotation", 1f);
		}
	}

	public override void Attached()
	{
		this.animator = base.transform.GetComponent<Animator>();
		this.animator.SetBool("net", true);
		if (BoltNetwork.isRunning)
		{
			base.Invoke("fixRootRotation", 1f);
		}
	}

	private void LateUpdate()
	{
	}

	private void ChangeClip(string currClip, AnimationClip newClip)
	{
		Animator component = base.GetComponent<Animator>();
		AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController();
		animatorOverrideController.runtimeAnimatorController = component.runtimeAnimatorController;
		animatorOverrideController[currClip] = newClip;
		component.runtimeAnimatorController = animatorOverrideController;
	}

	private void fixRootRotation()
	{
		base.transform.parent.localEulerAngles = new Vector3(0f, base.transform.parent.localEulerAngles.y, 0f);
	}
}
