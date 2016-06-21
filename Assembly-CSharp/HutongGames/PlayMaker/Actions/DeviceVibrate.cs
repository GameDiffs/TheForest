using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Device), Tooltip("Causes the device to vibrate for half a second.")]
	public class DeviceVibrate : FsmStateAction
	{
		public override void Reset()
		{
		}

		public override void OnEnter()
		{
			base.Finish();
		}
	}
}
