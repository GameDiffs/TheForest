using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Character), HutongGames.PlayMaker.Tooltip("Modify various character controller settings.\n'None' leaves the setting unchanged.")]
	public class ControllerSettings : FsmStateAction
	{
		[CheckForComponent(typeof(CharacterController)), RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject that owns the CharacterController.")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("The height of the character's capsule.")]
		public FsmFloat height;

		[HutongGames.PlayMaker.Tooltip("The radius of the character's capsule.")]
		public FsmFloat radius;

		[HutongGames.PlayMaker.Tooltip("The character controllers slope limit in degrees.")]
		public FsmFloat slopeLimit;

		[HutongGames.PlayMaker.Tooltip("The character controllers step offset in meters.")]
		public FsmFloat stepOffset;

		[HutongGames.PlayMaker.Tooltip("The center of the character's capsule relative to the transform's position")]
		public FsmVector3 center;

		[HutongGames.PlayMaker.Tooltip("Should other rigidbodies or character controllers collide with this character controller (By default always enabled).")]
		public FsmBool detectCollisions;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;

		private GameObject previousGo;

		private CharacterController controller;

		public override void Reset()
		{
			this.gameObject = null;
			this.height = new FsmFloat
			{
				UseVariable = true
			};
			this.radius = new FsmFloat
			{
				UseVariable = true
			};
			this.slopeLimit = new FsmFloat
			{
				UseVariable = true
			};
			this.stepOffset = new FsmFloat
			{
				UseVariable = true
			};
			this.center = new FsmVector3
			{
				UseVariable = true
			};
			this.detectCollisions = new FsmBool
			{
				UseVariable = true
			};
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoControllerSettings();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoControllerSettings();
		}

		private void DoControllerSettings()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			if (ownerDefaultTarget != this.previousGo)
			{
				this.controller = ownerDefaultTarget.GetComponent<CharacterController>();
				this.previousGo = ownerDefaultTarget;
			}
			if (this.controller != null)
			{
				if (!this.height.IsNone)
				{
					this.controller.height = this.height.Value;
				}
				if (!this.radius.IsNone)
				{
					this.controller.radius = this.radius.Value;
				}
				if (!this.slopeLimit.IsNone)
				{
					this.controller.slopeLimit = this.slopeLimit.Value;
				}
				if (!this.stepOffset.IsNone)
				{
					this.controller.stepOffset = this.stepOffset.Value;
				}
				if (!this.center.IsNone)
				{
					this.controller.center = this.center.Value;
				}
				if (!this.detectCollisions.IsNone)
				{
					this.controller.detectCollisions = this.detectCollisions.Value;
				}
			}
		}
	}
}
