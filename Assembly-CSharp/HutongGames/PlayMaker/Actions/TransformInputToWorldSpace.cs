using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Input), HutongGames.PlayMaker.Tooltip("Transforms 2d input into a 3d world space vector. E.g., can be used to transform input from a touch joystick to a movement vector.")]
	public class TransformInputToWorldSpace : FsmStateAction
	{
		public enum AxisPlane
		{
			XZ,
			XY,
			YZ
		}

		[HutongGames.PlayMaker.Tooltip("The horizontal input."), UIHint(UIHint.Variable)]
		public FsmFloat horizontalInput;

		[HutongGames.PlayMaker.Tooltip("The vertical input."), UIHint(UIHint.Variable)]
		public FsmFloat verticalInput;

		[HutongGames.PlayMaker.Tooltip("Input axis are reported in the range -1 to 1, this multiplier lets you set a new range.")]
		public FsmFloat multiplier;

		[RequiredField, HutongGames.PlayMaker.Tooltip("The world plane to map the 2d input onto.")]
		public TransformInputToWorldSpace.AxisPlane mapToPlane;

		[HutongGames.PlayMaker.Tooltip("Make the result relative to a GameObject, typically the main camera.")]
		public FsmGameObject relativeTo;

		[RequiredField, HutongGames.PlayMaker.Tooltip("Store the direction vector."), UIHint(UIHint.Variable)]
		public FsmVector3 storeVector;

		[HutongGames.PlayMaker.Tooltip("Store the length of the direction vector."), UIHint(UIHint.Variable)]
		public FsmFloat storeMagnitude;

		public override void Reset()
		{
			this.horizontalInput = null;
			this.verticalInput = null;
			this.multiplier = 1f;
			this.mapToPlane = TransformInputToWorldSpace.AxisPlane.XZ;
			this.storeVector = null;
			this.storeMagnitude = null;
		}

		public override void OnUpdate()
		{
			Vector3 a = default(Vector3);
			Vector3 a2 = default(Vector3);
			if (this.relativeTo.Value == null)
			{
				switch (this.mapToPlane)
				{
				case TransformInputToWorldSpace.AxisPlane.XZ:
					a = Vector3.forward;
					a2 = Vector3.right;
					break;
				case TransformInputToWorldSpace.AxisPlane.XY:
					a = Vector3.up;
					a2 = Vector3.right;
					break;
				case TransformInputToWorldSpace.AxisPlane.YZ:
					a = Vector3.up;
					a2 = Vector3.forward;
					break;
				}
			}
			else
			{
				Transform transform = this.relativeTo.Value.transform;
				switch (this.mapToPlane)
				{
				case TransformInputToWorldSpace.AxisPlane.XZ:
					a = transform.TransformDirection(Vector3.forward);
					a.y = 0f;
					a = a.normalized;
					a2 = new Vector3(a.z, 0f, -a.x);
					break;
				case TransformInputToWorldSpace.AxisPlane.XY:
				case TransformInputToWorldSpace.AxisPlane.YZ:
					a = Vector3.up;
					a.z = 0f;
					a = a.normalized;
					a2 = transform.TransformDirection(Vector3.right);
					break;
				}
			}
			float d = (!this.horizontalInput.IsNone) ? this.horizontalInput.Value : 0f;
			float d2 = (!this.verticalInput.IsNone) ? this.verticalInput.Value : 0f;
			Vector3 vector = d * a2 + d2 * a;
			vector *= this.multiplier.Value;
			this.storeVector.Value = vector;
			if (!this.storeMagnitude.IsNone)
			{
				this.storeMagnitude.Value = vector.magnitude;
			}
		}
	}
}
