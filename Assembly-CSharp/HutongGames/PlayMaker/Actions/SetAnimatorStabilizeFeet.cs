using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Animator"), HelpUrl("https://hutonggames.fogbugz.com/default.asp?W1074"), HutongGames.PlayMaker.Tooltip("If true, automaticaly stabilize feet during transition and blending")]
	public class SetAnimatorStabilizeFeet : FsmStateAction
	{
		[CheckForComponent(typeof(Animator)), RequiredField, HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("If true, automaticaly stabilize feet during transition and blending")]
		public FsmBool stabilizeFeet;

		private Animator _animator;

		public override void Reset()
		{
			this.gameObject = null;
			this.stabilizeFeet = null;
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
			this.DoStabilizeFeet();
			base.Finish();
		}

		private void DoStabilizeFeet()
		{
			if (this._animator == null)
			{
				return;
			}
			this._animator.stabilizeFeet = this.stabilizeFeet.Value;
		}
	}
}
