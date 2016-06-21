using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Animator"), HelpUrl("https://hutonggames.fogbugz.com/default.asp?W1075"), HutongGames.PlayMaker.Tooltip("Sets the value of a vector3 parameter")]
	public class SetAnimatorVector3 : FsmStateAction
	{
		[CheckForComponent(typeof(Animator)), RequiredField, HutongGames.PlayMaker.Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("The animator parameter")]
		public FsmString parameter;

		[HutongGames.PlayMaker.Tooltip("The Vector3 value to assign to the animator parameter")]
		public FsmVector3 Value;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful for changing over time. Must be checked if you are using damptime")]
		public bool everyFrame;

		private PlayMakerAnimatorMoveProxy _animatorProxy;

		private Animator _animator;

		private int _paramID;

		public override void Reset()
		{
			this.gameObject = null;
			this.parameter = null;
			this.Value = null;
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
			this._paramID = Animator.StringToHash(this.parameter.Value);
			this.SetParameter();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public void OnAnimatorMoveEvent()
		{
			if (this._animatorProxy != null)
			{
				this.SetParameter();
			}
		}

		public override void OnUpdate()
		{
			if (this._animatorProxy == null)
			{
				this.SetParameter();
			}
		}

		private void SetParameter()
		{
			if (this._animator != null)
			{
				this._animator.SetVector(this._paramID, this.Value.Value);
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
