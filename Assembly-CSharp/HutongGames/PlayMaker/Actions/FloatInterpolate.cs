using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Math), HutongGames.PlayMaker.Tooltip("Interpolates between 2 Float values over a specified Time.")]
	public class FloatInterpolate : FsmStateAction
	{
		[HutongGames.PlayMaker.Tooltip("Interpolation mode: Linear or EaseInOut.")]
		public InterpolationType mode;

		[RequiredField, HutongGames.PlayMaker.Tooltip("Interpolate from this value.")]
		public FsmFloat fromFloat;

		[RequiredField, HutongGames.PlayMaker.Tooltip("Interpolate to this value.")]
		public FsmFloat toFloat;

		[RequiredField, HutongGames.PlayMaker.Tooltip("Interpolate over this amount of time in seconds.")]
		public FsmFloat time;

		[RequiredField, HutongGames.PlayMaker.Tooltip("Store the current value in a float variable."), UIHint(UIHint.Variable)]
		public FsmFloat storeResult;

		[HutongGames.PlayMaker.Tooltip("Event to send when the interpolation is finished.")]
		public FsmEvent finishEvent;

		[HutongGames.PlayMaker.Tooltip("Ignore TimeScale. Useful if the game is paused (Time scaled to 0).")]
		public bool realTime;

		private float startTime;

		private float currentTime;

		public override void Reset()
		{
			this.mode = InterpolationType.Linear;
			this.fromFloat = null;
			this.toFloat = null;
			this.time = 1f;
			this.storeResult = null;
			this.finishEvent = null;
			this.realTime = false;
		}

		public override void OnEnter()
		{
			this.startTime = FsmTime.RealtimeSinceStartup;
			this.currentTime = 0f;
			if (this.storeResult == null)
			{
				base.Finish();
			}
			else
			{
				this.storeResult.Value = this.fromFloat.Value;
			}
		}

		public override void OnUpdate()
		{
			if (this.realTime)
			{
				this.currentTime = FsmTime.RealtimeSinceStartup - this.startTime;
			}
			else
			{
				this.currentTime += Time.deltaTime;
			}
			float num = this.currentTime / this.time.Value;
			InterpolationType interpolationType = this.mode;
			if (interpolationType != InterpolationType.Linear)
			{
				if (interpolationType == InterpolationType.EaseInOut)
				{
					this.storeResult.Value = Mathf.SmoothStep(this.fromFloat.Value, this.toFloat.Value, num);
				}
			}
			else
			{
				this.storeResult.Value = Mathf.Lerp(this.fromFloat.Value, this.toFloat.Value, num);
			}
			if (num > 1f)
			{
				if (this.finishEvent != null)
				{
					base.Fsm.Event(this.finishEvent);
				}
				base.Finish();
			}
		}
	}
}
