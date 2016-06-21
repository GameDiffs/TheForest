using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform), HutongGames.PlayMaker.Tooltip("Smoothly Rotates a Game Object so its forward vector points in the specified Direction.")]
	public class SmoothLookAtDirection : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject to rotate.")]
		public FsmOwnerDefault gameObject;

		[RequiredField, HutongGames.PlayMaker.Tooltip("The direction to smoothly rotate towards.")]
		public FsmVector3 targetDirection;

		[HutongGames.PlayMaker.Tooltip("Only rotate if Target Direction Vector length is greater than this threshold.")]
		public FsmFloat minMagnitude;

		[HutongGames.PlayMaker.Tooltip("Keep this vector pointing up as the GameObject rotates.")]
		public FsmVector3 upVector;

		[RequiredField, HutongGames.PlayMaker.Tooltip("Eliminate any tilt up/down as the GameObject rotates.")]
		public FsmBool keepVertical;

		[HasFloatSlider(0.5f, 15f), RequiredField, HutongGames.PlayMaker.Tooltip("How quickly to rotate.")]
		public FsmFloat speed;

		[HutongGames.PlayMaker.Tooltip("Perform in LateUpdate. This can help eliminate jitters in some situations.")]
		public bool lateUpdate;

		private GameObject previousGo;

		private Quaternion lastRotation;

		private Quaternion desiredRotation;

		public override void Reset()
		{
			this.gameObject = null;
			this.targetDirection = new FsmVector3
			{
				UseVariable = true
			};
			this.minMagnitude = 0.1f;
			this.upVector = new FsmVector3
			{
				UseVariable = true
			};
			this.keepVertical = true;
			this.speed = 5f;
			this.lateUpdate = true;
		}

		public override void OnEnter()
		{
			this.previousGo = null;
		}

		public override void OnUpdate()
		{
			if (!this.lateUpdate)
			{
				this.DoSmoothLookAtDirection();
			}
		}

		public override void OnLateUpdate()
		{
			if (this.lateUpdate)
			{
				this.DoSmoothLookAtDirection();
			}
		}

		private void DoSmoothLookAtDirection()
		{
			if (this.targetDirection.IsNone)
			{
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			if (this.previousGo != ownerDefaultTarget)
			{
				this.lastRotation = ownerDefaultTarget.transform.rotation;
				this.desiredRotation = this.lastRotation;
				this.previousGo = ownerDefaultTarget;
			}
			Vector3 value = this.targetDirection.Value;
			if (this.keepVertical.Value)
			{
				value.y = 0f;
			}
			if (value.sqrMagnitude > this.minMagnitude.Value)
			{
				this.desiredRotation = Quaternion.LookRotation(value, (!this.upVector.IsNone) ? this.upVector.Value : Vector3.up);
			}
			this.lastRotation = Quaternion.Slerp(this.lastRotation, this.desiredRotation, this.speed.Value * Time.deltaTime);
			ownerDefaultTarget.transform.rotation = this.lastRotation;
		}
	}
}
