using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Device), Tooltip("Gets Location Info from a mobile device. NOTE: Use StartLocationService before trying to get location info.")]
	public class GetLocationInfo : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmVector3 vectorPosition;

		[UIHint(UIHint.Variable)]
		public FsmFloat longitude;

		[UIHint(UIHint.Variable)]
		public FsmFloat latitude;

		[UIHint(UIHint.Variable)]
		public FsmFloat altitude;

		[UIHint(UIHint.Variable)]
		public FsmFloat horizontalAccuracy;

		[UIHint(UIHint.Variable)]
		public FsmFloat verticalAccuracy;

		[Tooltip("Event to send if the location cannot be queried.")]
		public FsmEvent errorEvent;

		public override void Reset()
		{
			this.longitude = null;
			this.latitude = null;
			this.altitude = null;
			this.horizontalAccuracy = null;
			this.verticalAccuracy = null;
			this.errorEvent = null;
		}

		public override void OnEnter()
		{
			this.DoGetLocationInfo();
			base.Finish();
		}

		private void DoGetLocationInfo()
		{
		}
	}
}
