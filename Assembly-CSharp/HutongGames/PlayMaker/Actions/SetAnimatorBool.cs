using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Animator"), HelpUrl("https://hutonggames.fogbugz.com/default.asp?W1063"), HutongGames.PlayMaker.Tooltip("Sets the value of a bool parameter")]
	public class SetAnimatorBool : FsmStateAction
	{
		[CheckForComponent(typeof(Animator)), RequiredField, HutongGames.PlayMaker.Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("The animator parameter")]
		public FsmString parameter;

		[HutongGames.PlayMaker.Tooltip("The Bool value to assign to the animator parameter")]
		public FsmBool Value;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful when value is changing over time.")]
		public bool everyFrame;

		private PlayMakerAnimatorMoveProxy _animatorProxy;

		private Animator _animator;

		private int _paramID;

		private GameObject _animatorGo;

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
			this._animatorGo = ownerDefaultTarget;
			if (ownerDefaultTarget == null)
			{
				base.Finish();
				return;
			}
			if (!this._animator)
			{
				this._animator = ownerDefaultTarget.GetComponent<Animator>();
			}
			if (this._animator == null)
			{
				base.Finish();
				return;
			}
			if (!this._animatorProxy)
			{
				this._animatorProxy = ownerDefaultTarget.GetComponent<PlayMakerAnimatorMoveProxy>();
			}
			if (this._animatorProxy != null)
			{
				this._animatorProxy.OnAnimatorMoveEvent += new Action(this.OnAnimatorMoveEvent);
			}
			if (this._paramID == 0)
			{
				this._paramID = Animator.StringToHash(this.parameter.Value);
			}
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
			if (this._animator != null && this._animatorGo.activeSelf)
			{
				this._animator.SetBoolReflected(this.parameter.Value, this.Value.Value);
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
