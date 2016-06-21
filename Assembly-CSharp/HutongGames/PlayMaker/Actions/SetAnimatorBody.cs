using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Animator"), HelpUrl("https://hutonggames.fogbugz.com/default.asp?W1062"), HutongGames.PlayMaker.Tooltip("Sets the position and rotation of the body. A GameObject can be set to control the position and rotation, or it can be manually expressed.")]
	public class SetAnimatorBody : FsmStateAction
	{
		[CheckForComponent(typeof(Animator)), RequiredField, HutongGames.PlayMaker.Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("The gameObject target of the ik goal")]
		public FsmGameObject target;

		[HutongGames.PlayMaker.Tooltip("The position of the ik goal. If Goal GameObject set, position is used as an offset from Goal")]
		public FsmVector3 position;

		[HutongGames.PlayMaker.Tooltip("The rotation of the ik goal.If Goal GameObject set, rotation is used as an offset from Goal")]
		public FsmQuaternion rotation;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful when changing over time.")]
		public bool everyFrame;

		private PlayMakerAnimatorIKProxy _animatorProxy;

		private Animator _animator;

		private Transform _transform;

		public override void Reset()
		{
			this.gameObject = null;
			this.target = null;
			this.position = new FsmVector3
			{
				UseVariable = true
			};
			this.rotation = new FsmQuaternion
			{
				UseVariable = true
			};
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
			this._animatorProxy = ownerDefaultTarget.GetComponent<PlayMakerAnimatorIKProxy>();
			if (this._animatorProxy != null)
			{
				this._animatorProxy.OnAnimatorIKEvent += new Action<int>(this.OnAnimatorIKEvent);
			}
			GameObject value = this.target.Value;
			if (value != null)
			{
				this._transform = value.transform;
			}
			this.DoSetBody();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public void OnAnimatorIKEvent(int layer)
		{
			if (this._animatorProxy != null)
			{
				this.DoSetBody();
			}
		}

		public override void OnUpdate()
		{
			if (this._animatorProxy == null)
			{
				this.DoSetBody();
			}
		}

		private void DoSetBody()
		{
			if (this._animator == null)
			{
				return;
			}
			if (this._transform != null)
			{
				if (this.position.IsNone)
				{
					this._animator.bodyPosition = this._transform.position;
				}
				else
				{
					this._animator.bodyPosition = this._transform.position + this.position.Value;
				}
				if (this.rotation.IsNone)
				{
					this._animator.bodyRotation = this._transform.rotation;
				}
				else
				{
					this._animator.bodyRotation = this._transform.rotation * this.rotation.Value;
				}
			}
			else
			{
				if (!this.position.IsNone)
				{
					this._animator.bodyPosition = this.position.Value;
				}
				if (!this.rotation.IsNone)
				{
					this._animator.bodyRotation = this.rotation.Value;
				}
			}
		}

		public override void OnExit()
		{
			if (this._animatorProxy != null)
			{
				this._animatorProxy.OnAnimatorIKEvent -= new Action<int>(this.OnAnimatorIKEvent);
			}
		}
	}
}
