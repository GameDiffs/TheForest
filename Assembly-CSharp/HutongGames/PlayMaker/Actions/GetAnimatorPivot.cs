using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Animator"), HelpUrl("https://hutonggames.fogbugz.com/default.asp?W1055"), HutongGames.PlayMaker.Tooltip("Returns the pivot weight and/or position. The pivot is the most stable point between the avatar's left and right foot.\n For a weight value of 0, the left foot is the most stable point For a value of 1, the right foot is the most stable point")]
	public class GetAnimatorPivot : FsmStateAction
	{
		[CheckForComponent(typeof(Animator)), RequiredField, HutongGames.PlayMaker.Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful when value is subject to change over time.")]
		public bool everyFrame;

		[ActionSection("Results"), HutongGames.PlayMaker.Tooltip("The pivot is the most stable point between the avatar's left and right foot.\n For a value of 0, the left foot is the most stable point For a value of 1, the right foot is the most stable point"), UIHint(UIHint.Variable)]
		public FsmFloat pivotWeight;

		[HutongGames.PlayMaker.Tooltip("The pivot is the most stable point between the avatar's left and right foot.\n For a value of 0, the left foot is the most stable point For a value of 1, the right foot is the most stable point"), UIHint(UIHint.Variable)]
		public FsmVector3 pivotPosition;

		private PlayMakerAnimatorMoveProxy _animatorProxy;

		private Animator _animator;

		public override void Reset()
		{
			this.gameObject = null;
			this.pivotWeight = null;
			this.pivotPosition = null;
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
			this.DoCheckPivot();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public void OnAnimatorMoveEvent()
		{
			if (this._animatorProxy != null)
			{
				this.DoCheckPivot();
			}
		}

		public override void OnUpdate()
		{
			if (this._animatorProxy == null)
			{
				this.DoCheckPivot();
			}
		}

		private void DoCheckPivot()
		{
			if (this._animator == null)
			{
				return;
			}
			this.pivotWeight.Value = this._animator.pivotWeight;
			this.pivotPosition.Value = this._animator.pivotPosition;
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
