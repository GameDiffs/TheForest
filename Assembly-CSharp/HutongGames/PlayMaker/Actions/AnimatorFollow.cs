using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Animator"), HelpUrl("https://hutonggames.fogbugz.com/default.asp?W1033"), HutongGames.PlayMaker.Tooltip("Follow a target")]
	public class AnimatorFollow : FsmStateAction
	{
		[CheckForComponent(typeof(Animator)), RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject. An Animator component and a PlayMakerAnimatorProxy component are required")]
		public FsmOwnerDefault gameObject;

		[RequiredField, HutongGames.PlayMaker.Tooltip("The Game Object to target.")]
		public FsmGameObject target;

		[HutongGames.PlayMaker.Tooltip("The speed to follow target")]
		public FsmFloat speed;

		[HutongGames.PlayMaker.Tooltip("The minimum distance to follow.")]
		public FsmFloat minimumDistance;

		[HutongGames.PlayMaker.Tooltip("The damping for following up.")]
		public FsmFloat speedDampTime;

		[HutongGames.PlayMaker.Tooltip("The damping for turning.")]
		public FsmFloat directionDampTime;

		private GameObject _go;

		private PlayMakerAnimatorMoveProxy _animatorProxy;

		private Animator avatar;

		private CharacterController controller;

		public override void Reset()
		{
			this.gameObject = null;
			this.target = null;
			this.speed = 1f;
			this.speedDampTime = 0.25f;
			this.directionDampTime = 0.25f;
			this.minimumDistance = 1f;
		}

		public override void OnEnter()
		{
			this._go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this._go == null)
			{
				base.Finish();
				return;
			}
			this._animatorProxy = this._go.GetComponent<PlayMakerAnimatorMoveProxy>();
			if (this._animatorProxy != null)
			{
				this._animatorProxy.OnAnimatorMoveEvent += new Action(this.OnAnimatorMoveEvent);
			}
			this.avatar = this._go.GetComponent<Animator>();
			this.controller = this._go.GetComponent<CharacterController>();
		}

		public override void OnUpdate()
		{
			GameObject value = this.target.Value;
			float value2 = this.speedDampTime.Value;
			float value3 = this.directionDampTime.Value;
			float value4 = this.minimumDistance.Value;
			if (this.avatar && value)
			{
				if (Vector3.Distance(value.transform.position, this.avatar.rootPosition) > value4)
				{
					this.avatar.speed = this.speed.Value;
					this.avatar.SetFloatReflected("Speed", 1f, value2, Time.deltaTime);
					Vector3 lhs = this.avatar.rootRotation * Vector3.forward;
					Vector3 normalized = (value.transform.position - this.avatar.rootPosition).normalized;
					if (Vector3.Dot(lhs, normalized) > 0f)
					{
						this.avatar.SetFloatReflected("Direction", Vector3.Cross(lhs, normalized).y, value3, Time.deltaTime);
					}
					else
					{
						this.avatar.SetFloatReflected("Direction", (float)((Vector3.Cross(lhs, normalized).y <= 0f) ? -1 : 1), value3, Time.deltaTime);
					}
				}
				else
				{
					this.avatar.SetFloatReflected("Speed", 0f, value2, Time.deltaTime);
				}
				if (this._animatorProxy == null)
				{
					this.OnAnimatorMoveEvent();
				}
			}
		}

		public override void OnExit()
		{
			if (this._animatorProxy != null)
			{
				this._animatorProxy.OnAnimatorMoveEvent -= new Action(this.OnAnimatorMoveEvent);
			}
		}

		public void OnAnimatorMoveEvent()
		{
			this.controller.Move(this.avatar.deltaPosition);
			this._go.transform.rotation = this.avatar.rootRotation;
		}
	}
}
