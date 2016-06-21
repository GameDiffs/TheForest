using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Animator"), HutongGames.PlayMaker.Tooltip("Returns true if the specified layer is in a transition. Can also send events")]
	public class GetAnimatorIsLayerInTransition : FsmStateAction
	{
		[CheckForComponent(typeof(Animator)), RequiredField, HutongGames.PlayMaker.Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
		public FsmOwnerDefault gameObject;

		[RequiredField, HutongGames.PlayMaker.Tooltip("The layer's index")]
		public FsmInt layerIndex;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful when value is subject to change over time.")]
		public bool everyFrame;

		[ActionSection("Results"), HutongGames.PlayMaker.Tooltip("True if automatic matching is active"), UIHint(UIHint.Variable)]
		public FsmBool isInTransition;

		[HutongGames.PlayMaker.Tooltip("Event send if automatic matching is active")]
		public FsmEvent isInTransitionEvent;

		[HutongGames.PlayMaker.Tooltip("Event send if automatic matching is not active")]
		public FsmEvent isNotInTransitionEvent;

		private PlayMakerAnimatorMoveProxy _animatorProxy;

		private Animator _animator;

		public override void Reset()
		{
			this.gameObject = null;
			this.isInTransition = null;
			this.isInTransitionEvent = null;
			this.isNotInTransitionEvent = null;
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
			this.DoCheckIsInTransition();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public void OnAnimatorMoveEvent()
		{
			if (this._animatorProxy != null)
			{
				this.DoCheckIsInTransition();
			}
		}

		public override void OnUpdate()
		{
			if (this._animatorProxy == null)
			{
				this.DoCheckIsInTransition();
			}
		}

		private void DoCheckIsInTransition()
		{
			if (this._animator == null)
			{
				return;
			}
			bool flag = this._animator.IsInTransition(this.layerIndex.Value);
			this.isInTransition.Value = flag;
			if (flag)
			{
				base.Fsm.Event(this.isInTransitionEvent);
			}
			else
			{
				base.Fsm.Event(this.isNotInTransitionEvent);
			}
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
