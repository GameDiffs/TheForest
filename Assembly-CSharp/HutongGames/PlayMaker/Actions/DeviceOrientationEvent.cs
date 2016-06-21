using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Device), HutongGames.PlayMaker.Tooltip("Sends an Event based on the Orientation of the mobile device.")]
	public class DeviceOrientationEvent : FsmStateAction
	{
		[HutongGames.PlayMaker.Tooltip("Note: If device is physically situated between discrete positions, as when (for example) rotated diagonally, system will report Unknown orientation.")]
		public DeviceOrientation orientation;

		[HutongGames.PlayMaker.Tooltip("The event to send if the device orientation matches Orientation.")]
		public FsmEvent sendEvent;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if you want to wait for the orientation to be true.")]
		public bool everyFrame;

		public override void Reset()
		{
			this.orientation = DeviceOrientation.Portrait;
			this.sendEvent = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoDetectDeviceOrientation();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoDetectDeviceOrientation();
		}

		private void DoDetectDeviceOrientation()
		{
			if (Input.deviceOrientation == this.orientation)
			{
				base.Fsm.Event(this.sendEvent);
			}
		}
	}
}
