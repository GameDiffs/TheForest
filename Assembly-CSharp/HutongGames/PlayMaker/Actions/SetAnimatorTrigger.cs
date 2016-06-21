using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Animator"), HutongGames.PlayMaker.Tooltip("Sets a trigger parameter to active or inactive. Triggers are parameters that act mostly like booleans, but get resets to inactive when they are used in a transition.")]
	public class SetAnimatorTrigger : FsmStateAction
	{
		[CheckForComponent(typeof(Animator)), RequiredField, HutongGames.PlayMaker.Tooltip("The target. An Animator component is required")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("The trigger name")]
		public FsmString trigger;

		private Animator _animator;

		private int _paramID;

		public override void Reset()
		{
			this.gameObject = null;
			this.trigger = null;
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
			this.SetTrigger();
			base.Finish();
		}

		private void SetTrigger()
		{
			if (this._animator != null)
			{
				this._animator.SetTrigger(this.trigger.Value);
			}
		}
	}
}
