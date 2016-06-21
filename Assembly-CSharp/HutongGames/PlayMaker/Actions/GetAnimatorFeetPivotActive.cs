using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Animator"), HutongGames.PlayMaker.Tooltip("Returns the feet pivot. At 0% blending point is body mass center. At 100% blending point is feet pivot")]
	public class GetAnimatorFeetPivotActive : FsmStateAction
	{
		[CheckForComponent(typeof(Animator)), RequiredField, HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
		public FsmOwnerDefault gameObject;

		[RequiredField, HutongGames.PlayMaker.Tooltip("The feet pivot Blending. At 0% blending point is body mass center. At 100% blending point is feet pivot"), UIHint(UIHint.Variable)]
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
			this.DoGetFeetPivotActive();
			base.Finish();
		}

		private void DoGetFeetPivotActive()
		{
			if (this._animator == null)
			{
				return;
			}
			this.feetPivotActive.Value = this._animator.feetPivotActive;
		}
	}
}
