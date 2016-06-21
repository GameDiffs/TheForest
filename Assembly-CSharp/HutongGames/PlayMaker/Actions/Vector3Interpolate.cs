using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Vector3), HutongGames.PlayMaker.Tooltip("Interpolates between 2 Vector3 values over a specified Time.")]
	public class Vector3Interpolate : FsmStateAction
	{
		public InterpolationType mode;

		[RequiredField]
		public FsmVector3 fromVector;

		[RequiredField]
		public FsmVector3 toVector;

		[RequiredField]
		public FsmFloat time;

		[RequiredField, UIHint(UIHint.Variable)]
		public FsmVector3 storeResult;

		public FsmEvent finishEvent;

		[HutongGames.PlayMaker.Tooltip("Ignore TimeScale")]
		public bool realTime;

		private float startTime;

		private float currentTime;

		public override void Reset()
		{
			this.mode = InterpolationType.Linear;
			this.fromVector = new FsmVector3
			{
				UseVariable = true
			};
			this.toVector = new FsmVector3
			{
				UseVariable = true
			};
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
				this.storeResult.Value = this.fromVector.Value;
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
					num = Mathf.SmoothStep(0f, 1f, num);
				}
			}
			this.storeResult.Value = Vector3.Lerp(this.fromVector.Value, this.toVector.Value, num);
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
