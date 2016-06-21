using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Animator"), HutongGames.PlayMaker.Tooltip("Returns The current gravity weight based on current animations that are played")]
	public class GetAnimatorGravityWeight : FsmStateAction
	{
		[CheckForComponent(typeof(Animator)), RequiredField, HutongGames.PlayMaker.Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful when value is subject to change over time.")]
		public bool everyFrame;

		[ActionSection("Results"), HutongGames.PlayMaker.Tooltip("The current gravity weight based on current animations that are played"), UIHint(UIHint.Variable)]
		public FsmFloat gravityWeight;

		private PlayMakerAnimatorMoveProxy _animatorProxy;

		private Animator _animator;

		public override void Reset()
		{
			this.gameObject = null;
			this.gravityWeight = null;
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
			this._animatorProxy = ownerDefaultTarget.GetComponent<PlayMakerAnimatorMoveProxy>();
			if (this._animatorProxy != null)
			{
				this._animatorProxy.OnAnimatorMoveEvent += new Action(this.OnAnimatorMoveEvent);
			}
			this.DoGetGravityWeight();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public void OnAnimatorMoveEvent()
		{
			if (this._animatorProxy != null)
			{
				this.DoGetGravityWeight();
			}
		}

		public override void OnUpdate()
		{
			if (this._animatorProxy == null)
			{
				this.DoGetGravityWeight();
			}
		}

		private void DoGetGravityWeight()
		{
			if (this._animator == null)
			{
				return;
			}
			this.gravityWeight.Value = this._animator.gravityWeight;
		}

		public override void OnExit()
		{
			if (this._animatorProxy != null)
			{
				this._animatorProxy.OnAnimatorMoveEvent -= new Action(this.OnAnimatorMoveEvent);
			}
		}
	}
}
