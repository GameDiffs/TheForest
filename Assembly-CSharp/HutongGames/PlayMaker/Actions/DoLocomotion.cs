using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("Animator"), HutongGames.PlayMaker.Tooltip("Bridge for the Locomotion system.")]
	public class DoLocomotion : FsmStateAction
	{
		[CheckForComponent(typeof(Animator)), RequiredField, HutongGames.PlayMaker.Tooltip("The target. An Animator component is required")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("The speed value")]
		public FsmFloat speed;

		[HutongGames.PlayMaker.Tooltip("Optional: The angle value")]
		public FsmFloat angle;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful when changing over time.")]
		public bool everyFrame;

		private Animator _animator;

		protected Locomotion _locomotion;

		public override void Reset()
		{
			this.gameObject = null;
			this.speed = null;
			this.angle = null;
			this.everyFrame = true;
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
			this._locomotion = new Locomotion(this._animator);
			this.SetLocomotion();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.SetLocomotion();
		}

		public void SetLocomotion()
		{
			this._locomotion.Do(this.speed.Value, this.angle.Value);
		}
	}
}
