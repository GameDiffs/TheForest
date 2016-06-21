using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Animator"), HelpUrl("https://hutonggames.fogbugz.com/default.asp?W1035"), HutongGames.PlayMaker.Tooltip("Gets the value of ApplyRootMotion of an avatar. If true, root is controlled by animations")]
	public class GetAnimatorApplyRootMotion : FsmStateAction
	{
		[CheckForComponent(typeof(Animator)), RequiredField, HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
		public FsmOwnerDefault gameObject;

		[ActionSection("Results"), RequiredField, HutongGames.PlayMaker.Tooltip("Is the rootMotionapplied. If true, root is controlled by animations"), UIHint(UIHint.Variable)]
		public FsmBool rootMotionApplied;

		[HutongGames.PlayMaker.Tooltip("Event send if the root motion is applied")]
		public FsmEvent rootMotionIsAppliedEvent;

		[HutongGames.PlayMaker.Tooltip("Event send if the root motion is not applied")]
		public FsmEvent rootMotionIsNotAppliedEvent;

		private Animator _animator;

		public override void Reset()
		{
			this.gameObject = null;
			this.rootMotionApplied = null;
			this.rootMotionIsAppliedEvent = null;
			this.rootMotionIsNotAppliedEvent = null;
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
			this.GetApplyMotionRoot();
			base.Finish();
		}

		private void GetApplyMotionRoot()
		{
			if (this._animator != null)
			{
				bool applyRootMotion = this._animator.applyRootMotion;
				this.rootMotionApplied.Value = applyRootMotion;
				if (applyRootMotion)
				{
					base.Fsm.Event(this.rootMotionIsAppliedEvent);
				}
				else
				{
					base.Fsm.Event(this.rootMotionIsNotAppliedEvent);
				}
			}
		}
	}
}
