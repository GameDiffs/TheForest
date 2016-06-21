using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Device), HutongGames.PlayMaker.Tooltip("Gets the rotation of the device around its z axis (into the screen). For example when you steer with the iPhone in a driving game.")]
	public class GetDeviceRoll : FsmStateAction
	{
		public enum BaseOrientation
		{
			Portrait,
			LandscapeLeft,
			LandscapeRight
		}

		[HutongGames.PlayMaker.Tooltip("How the user is expected to hold the device (where angle will be zero).")]
		public GetDeviceRoll.BaseOrientation baseOrientation;

		[UIHint(UIHint.Variable)]
		public FsmFloat storeAngle;

		public FsmFloat limitAngle;

		public FsmFloat smoothing;

		public bool everyFrame;

		private float lastZAngle;

		public override void Reset()
		{
			this.baseOrientation = GetDeviceRoll.BaseOrientation.LandscapeLeft;
			this.storeAngle = null;
			this.limitAngle = new FsmFloat
			{
				UseVariable = true
			};
			this.smoothing = 5f;
			this.everyFrame = true;
		}

		public override void OnEnter()
		{
			this.DoGetDeviceRoll();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoGetDeviceRoll();
		}

		private void DoGetDeviceRoll()
		{
			float x = Input.acceleration.x;
			float y = Input.acceleration.y;
			float num = 0f;
			switch (this.baseOrientation)
			{
			case GetDeviceRoll.BaseOrientation.Portrait:
				num = -Mathf.Atan2(x, -y);
				break;
			case GetDeviceRoll.BaseOrientation.LandscapeLeft:
				num = Mathf.Atan2(y, -x);
				break;
			case GetDeviceRoll.BaseOrientation.LandscapeRight:
				num = -Mathf.Atan2(y, x);
				break;
			}
			if (!this.limitAngle.IsNone)
			{
				num = Mathf.Clamp(57.29578f * num, -this.limitAngle.Value, this.limitAngle.Value);
			}
			if (this.smoothing.Value > 0f)
			{
				num = Mathf.LerpAngle(this.lastZAngle, num, this.smoothing.Value * Time.deltaTime);
			}
			this.lastZAngle = num;
			this.storeAngle.Value = num;
		}
	}
}
