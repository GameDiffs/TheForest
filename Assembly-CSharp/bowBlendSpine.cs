using System;
using UnityEngine;

public class bowBlendSpine : StateMachineBehaviour
{
	private Animator thisAnimator;

	public bool doBlend;

	private AnimatorStateInfo info1;

	private float n;

	public new void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (stateInfo.IsTag("attacking"))
		{
			this.thisAnimator = animator;
			this.doBlend = true;
		}
	}

	public new void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (animator.IsInTransition(1) && this.doBlend)
		{
			this.n = this.thisAnimator.GetAnimatorTransitionInfo(1).normalizedTime;
			this.thisAnimator.SetLayerWeightReflected(4, this.n);
		}
		else if (!animator.IsInTransition(1) && this.doBlend)
		{
			this.thisAnimator.SetLayerWeightReflected(4, 1f);
			this.doBlend = false;
		}
	}
}
