using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Animator"), HelpUrl("https://hutonggames.fogbugz.com/default.asp?W1060"), HutongGames.PlayMaker.Tooltip("When turned on, animations will be executed in the physics loop. This is only useful in conjunction with kinematic rigidbodies.")]
	public class SetAnimatorAnimatePhysics : FsmStateAction
	{
		[CheckForComponent(typeof(Animator)), RequiredField, HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("If true, animations will be executed in the physics loop. This is only useful in conjunction with kinematic rigidbodies.")]
		public FsmBool animatePhysics;

		private Animator _animator;

		public override void Reset()
		{
			this.gameObject = null;
			this.animatePhysics = null;
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
			this.DoAnimatePhysics();
			base.Finish();
		}

		private void DoAnimatePhysics()
		{
			if (this._animator == null)
			{
				return;
			}
			this._animator.animatePhysics = this.animatePhysics.Value;
		}
	}
}
