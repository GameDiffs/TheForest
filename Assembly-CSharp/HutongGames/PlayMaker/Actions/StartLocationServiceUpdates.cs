using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Device), Tooltip("Starts location service updates. Last location coordinates can be retrieved with GetLocationInfo.")]
	public class StartLocationServiceUpdates : FsmStateAction
	{
		[Tooltip("Maximum time to wait in seconds before failing.")]
		public FsmFloat maxWait;

		public FsmFloat desiredAccuracy;

		public FsmFloat updateDistance;

		[Tooltip("Event to send when the location services have started.")]
		public FsmEvent successEvent;

		[Tooltip("Event to send if the location services fail to start.")]
		public FsmEvent failedEvent;

		public override void Reset()
		{
			this.maxWait = 20f;
			this.desiredAccuracy = 10f;
			this.updateDistance = 10f;
			this.successEvent = null;
			this.failedEvent = null;
		}

		public override void OnEnter()
		{
			base.Finish();
		}

		public override void OnUpdate()
		{
		}
	}
}
