using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Device), HutongGames.PlayMaker.Tooltip("Gets the last measured linear acceleration of a device and stores it in a Vector3 Variable.")]
	public class GetDeviceAcceleration : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmVector3 storeVector;

		[UIHint(UIHint.Variable)]
		public FsmFloat storeX;

		[UIHint(UIHint.Variable)]
		public FsmFloat storeY;

		[UIHint(UIHint.Variable)]
		public FsmFloat storeZ;

		public FsmFloat multiplier;

		public bool everyFrame;

		public override void Reset()
		{
			this.storeVector = null;
			this.storeX = null;
			this.storeY = null;
			this.storeZ = null;
			this.multiplier = 1f;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoGetDeviceAcceleration();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoGetDeviceAcceleration();
		}

		private void DoGetDeviceAcceleration()
		{
			Vector3 vector = new Vector3(Input.acceleration.x, Input.acceleration.y, Input.acceleration.z);
			if (!this.multiplier.IsNone)
			{
				vector *= this.multiplier.Value;
			}
			this.storeVector.Value = vector;
			this.storeX.Value = vector.x;
			this.storeY.Value = vector.y;
			this.storeZ.Value = vector.z;
		}
	}
}
