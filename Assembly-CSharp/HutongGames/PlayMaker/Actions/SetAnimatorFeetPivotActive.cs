using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Animator"), HelpUrl("https://hutonggames.fogbugz.com/default.asp?W1065"), HutongGames.PlayMaker.Tooltip("Activates feet pivot. At 0% blending point is body mass center. At 100% blending point is feet pivot")]
	public class SetAnimatorFeetPivotActive : FsmStateAction
	{
		[CheckForComponent(typeof(Animator)), RequiredField, HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("Activates feet pivot. At 0% blending point is body mass center. At 100% blending point is feet pivot")]
		public FsmFloat feetPivotActive;

		private Animator _animator;

		public override void Reset()
		{
			this.gameObject = null;
			this.feetPivotActive = null;
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
			this.DoFeetPivotActive();
			base.Finish();
		}

		private void DoFeetPivotActive()
		{
			if (this._animator == null)
			{
				return;
			}
			this._animator.feetPivotActive = this.feetPivotActive.Value;
		}
	}
}
