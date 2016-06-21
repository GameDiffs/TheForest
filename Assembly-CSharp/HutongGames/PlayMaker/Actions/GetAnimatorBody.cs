using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Animator"), HelpUrl("https://hutonggames.fogbugz.com/default.asp?W1036"), HutongGames.PlayMaker.Tooltip("Gets the avatar body mass center position and rotation.Optionally accept a GameObject to get the body transform. \nThe position and rotation are local to the gameobject")]
	public class GetAnimatorBody : FsmStateAction
	{
		[CheckForComponent(typeof(Animator)), RequiredField, HutongGames.PlayMaker.Tooltip("The target.")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful when changing over time.")]
		public bool everyFrame;

		[ActionSection("Results"), HutongGames.PlayMaker.Tooltip("The avatar body mass center"), UIHint(UIHint.Variable)]
		public FsmVector3 bodyPosition;

		[HutongGames.PlayMaker.Tooltip("The avatar body mass center"), UIHint(UIHint.Variable)]
		public FsmQuaternion bodyRotation;

		[HutongGames.PlayMaker.Tooltip("If set, apply the body mass center position and rotation to this gameObject")]
		public FsmGameObject bodyGameObject;

		private PlayMakerAnimatorMoveProxy _animatorProxy;

		private Animator _animator;

		private Transform _transform;

		public override void Reset()
		{
			this.gameObject = null;
			this.bodyPosition = null;
			this.bodyRotation = null;
			this.bodyGameObject = null;
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
			GameObject value = this.bodyGameObject.Value;
			if (value != null)
			{
				this._transform = value.transform;
			}
			this.DoGetBodyPosition();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public void OnAnimatorMoveEvent()
		{
			if (this._animatorProxy != null)
			{
				this.DoGetBodyPosition();
			}
		}

		public override void OnUpdate()
		{
			if (this._animatorProxy == null)
			{
				this.DoGetBodyPosition();
			}
		}

		private void DoGetBodyPosition()
		{
			if (this._animator == null || !this._animator.enabled)
			{
				return;
			}
			this.bodyPosition.Value = this._animator.bodyPosition;
			this.bodyRotation.Value = this._animator.bodyRotation;
			if (this._transform != null)
			{
				this._transform.position = this._animator.bodyPosition;
				this._transform.rotation = this._animator.bodyRotation;
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
