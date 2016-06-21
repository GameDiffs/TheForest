using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Time), HutongGames.PlayMaker.Tooltip("Gets various useful Time measurements.")]
	public class GetTimeInfo : FsmStateAction
	{
		public enum TimeInfo
		{
			DeltaTime,
			TimeScale,
			SmoothDeltaTime,
			TimeInCurrentState,
			TimeSinceStartup,
			TimeSinceLevelLoad,
			RealTimeSinceStartup,
			RealTimeInCurrentState
		}

		public GetTimeInfo.TimeInfo getInfo;

		[RequiredField, UIHint(UIHint.Variable)]
		public FsmFloat storeValue;

		public bool everyFrame;

		public override void Reset()
		{
			this.getInfo = GetTimeInfo.TimeInfo.TimeSinceLevelLoad;
			this.storeValue = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoGetTimeInfo();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoGetTimeInfo();
		}

		private void DoGetTimeInfo()
		{
			switch (this.getInfo)
			{
			case GetTimeInfo.TimeInfo.DeltaTime:
				this.storeValue.Value = Time.deltaTime;
				break;
			case GetTimeInfo.TimeInfo.TimeScale:
				this.storeValue.Value = Time.timeScale;
				break;
			case GetTimeInfo.TimeInfo.SmoothDeltaTime:
				this.storeValue.Value = Time.smoothDeltaTime;
				break;
			case GetTimeInfo.TimeInfo.TimeInCurrentState:
				this.storeValue.Value = base.State.StateTime;
				break;
			case GetTimeInfo.TimeInfo.TimeSinceStartup:
				this.storeValue.Value = Time.time;
				break;
			case GetTimeInfo.TimeInfo.TimeSinceLevelLoad:
				this.storeValue.Value = Time.timeSinceLevelLoad;
				break;
			case GetTimeInfo.TimeInfo.RealTimeSinceStartup:
				this.storeValue.Value = FsmTime.RealtimeSinceStartup;
				break;
			case GetTimeInfo.TimeInfo.RealTimeInCurrentState:
				this.storeValue.Value = FsmTime.RealtimeSinceStartup - base.State.RealStartTime;
				break;
			default:
				this.storeValue.Value = 0f;
				break;
			}
		}
	}
}
