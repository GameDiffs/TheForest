using System;
using TheForest.Utils;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Input), HutongGames.PlayMaker.Tooltip("Gets a world direction Vector from 2 Input Axis. Typically used for a third person controller with Relative To set to the camera.")]
	public class GetAxisVector : FsmStateAction
	{
		public enum AxisPlane
		{
			XZ,
			XY,
			YZ
		}

		[HutongGames.PlayMaker.Tooltip("The name of the horizontal input axis. See Unity Input Manager.")]
		public FsmString horizontalAxis;

		[HutongGames.PlayMaker.Tooltip("The name of the vertical input axis. See Unity Input Manager.")]
		public FsmString verticalAxis;

		[HutongGames.PlayMaker.Tooltip("Input axis are reported in the range -1 to 1, this multiplier lets you set a new range.")]
		public FsmFloat multiplier;

		[RequiredField, HutongGames.PlayMaker.Tooltip("The world plane to map the 2d input onto.")]
		public GetAxisVector.AxisPlane mapToPlane;

		[HutongGames.PlayMaker.Tooltip("Make the result relative to a GameObject, typically the main camera.")]
		public FsmGameObject relativeTo;

		[RequiredField, HutongGames.PlayMaker.Tooltip("Store the direction vector."), UIHint(UIHint.Variable)]
		public FsmVector3 storeVector;

		[HutongGames.PlayMaker.Tooltip("Store the length of the direction vector."), UIHint(UIHint.Variable)]
		public FsmFloat storeMagnitude;

		public override void Reset()
		{
			this.horizontalAxis = "Horizontal";
			this.verticalAxis = "Vertical";
			this.multiplier = 1f;
			this.mapToPlane = GetAxisVector.AxisPlane.XZ;
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
				case GetAxisVector.AxisPlane.XZ:
					a = Vector3.forward;
					a2 = Vector3.right;
					break;
				case GetAxisVector.AxisPlane.XY:
					a = Vector3.up;
					a2 = Vector3.right;
					break;
				case GetAxisVector.AxisPlane.YZ:
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
				case GetAxisVector.AxisPlane.XZ:
					a = transform.TransformDirection(Vector3.forward);
					a.y = 0f;
					a = a.normalized;
					a2 = new Vector3(a.z, 0f, -a.x);
					break;
				case GetAxisVector.AxisPlane.XY:
				case GetAxisVector.AxisPlane.YZ:
					a = Vector3.up;
					a.z = 0f;
					a = a.normalized;
					a2 = transform.TransformDirection(Vector3.right);
					break;
				}
			}
			float d = (!this.horizontalAxis.IsNone && !string.IsNullOrEmpty(this.horizontalAxis.Value)) ? TheForest.Utils.Input.GetAxis(this.horizontalAxis.Value) : 0f;
			float d2 = (!this.verticalAxis.IsNone && !string.IsNullOrEmpty(this.verticalAxis.Value)) ? TheForest.Utils.Input.GetAxis(this.verticalAxis.Value) : 0f;
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
