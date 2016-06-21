using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Animator"), HutongGames.PlayMaker.Tooltip("Force the normalized time of a state to a user defined value. Only works on base layer. Should not be used in transition.")]
	public class AnimatorForceStateNormalizedTime : FsmStateAction
	{
		[CheckForComponent(typeof(Animator)), RequiredField, HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("The normalized time for the base layer")]
		public FsmFloat normalizedTime;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful for changing over time.")]
		public bool everyFrame;

		private Animator _animator;

		public override void Reset()
		{
			this.gameObject = null;
			this.normalizedTime = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				base.Finish();
				return;
			}
			this._animator = ownerDefaultTarget.GetComponent<Animator>();
			if (this._animator == null)
			{
				base.Finish();
				return;
			}
			this.ForceNormalizedTime();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.ForceNormalizedTime();
		}

		private void ForceNormalizedTime()
		{
			if (this._animator == null)
			{
				return;
			}
			this._animator.ForceStateNormalizedTime(this.normalizedTime.Value);
		}
	}
}
