using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Time), HutongGames.PlayMaker.Tooltip("Delays a State from finishing by the specified time. NOTE: Other actions continue, but FINISHED can't happen before Time.")]
	public class Wait : FsmStateAction
	{
		[RequiredField]
		public FsmFloat time;

		public FsmEvent finishEvent;

		public bool realTime;

		private float startTime;

		private float timer;

		public override void Reset()
		{
			this.time = 1f;
			this.finishEvent = null;
			this.realTime = false;
		}

		public override void OnEnter()
		{
			if (this.time.Value <= 0f)
			{
				base.Fsm.Event(this.finishEvent);
				base.Finish();
				return;
			}
			this.startTime = FsmTime.RealtimeSinceStartup;
			this.timer = 0f;
		}

		public override void OnUpdate()
		{
			if (this.realTime)
			{
				this.timer = FsmTime.RealtimeSinceStartup - this.startTime;
			}
			else
			{
				this.timer += Time.deltaTime;
			}
			if (this.timer >= this.time.Value)
			{
				base.Finish();
				if (this.finishEvent != null)
				{
					base.Fsm.Event(this.finishEvent);
				}
			}
		}
	}
}
