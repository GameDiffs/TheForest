using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Animator"), HutongGames.PlayMaker.Tooltip("Gets the avatar delta position and rotation for the last evaluated frame.")]
	public class GetAnimatorDelta : FsmStateAction
	{
		[CheckForComponent(typeof(Animator)), RequiredField, HutongGames.PlayMaker.Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("The avatar delta position for the last evaluated frame"), UIHint(UIHint.Variable)]
		public FsmVector3 deltaPosition;

		[HutongGames.PlayMaker.Tooltip("The avatar delta position for the last evaluated frame"), UIHint(UIHint.Variable)]
		public FsmQuaternion deltaRotation;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful when changing over time.")]
		public bool everyFrame;

		private PlayMakerAnimatorMoveProxy _animatorProxy;

		private Transform _transform;

		private Animator _animator;

		public override void Reset()
		{
			this.gameObject = null;
			this.deltaPosition = null;
			this.deltaRotation = null;
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
			this.DoGetDeltaPosition();
			base.Finish();
		}

		public override void OnUpdate()
		{
			if (this._animatorProxy == null)
			{
				this.DoGetDeltaPosition();
			}
		}

		public void OnAnimatorMoveEvent()
		{
			this.DoGetDeltaPosition();
		}

		private void DoGetDeltaPosition()
		{
			if (this._animator == null)
			{
				return;
			}
			this.deltaPosition.Value = this._animator.deltaPosition;
			this.deltaRotation.Value = this._animator.deltaRotation;
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
