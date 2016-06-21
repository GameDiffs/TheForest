using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Animator"), HelpUrl("https://hutonggames.fogbugz.com/default.asp?W1070"), HutongGames.PlayMaker.Tooltip("If true, additionnal layers affects the mass center")]
	public class SetAnimatorLayersAffectMassCenter : FsmStateAction
	{
		[CheckForComponent(typeof(Animator)), RequiredField, HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("If true, additionnal layers affects the mass center")]
		public FsmBool affectMassCenter;

		private Animator _animator;

		public override void Reset()
		{
			this.gameObject = null;
			this.affectMassCenter = null;
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
			this.SetAffectMassCenter();
			base.Finish();
		}

		private void SetAffectMassCenter()
		{
			if (this._animator == null)
			{
				return;
			}
			this._animator.layersAffectMassCenter = this.affectMassCenter.Value;
		}
	}
}
