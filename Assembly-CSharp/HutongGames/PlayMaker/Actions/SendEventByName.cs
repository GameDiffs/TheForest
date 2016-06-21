using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine), Tooltip("Sends an Event by name after an optional delay. NOTE: Use this over Send Event if you store events as string variables.")]
	public class SendEventByName : FsmStateAction
	{
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		[RequiredField, Tooltip("The event to send. NOTE: Events must be marked Global to send between FSMs.")]
		public FsmString sendEvent;

		[HasFloatSlider(0f, 10f), Tooltip("Optional delay in seconds.")]
		public FsmFloat delay;

		[Tooltip("Repeat every frame. Rarely needed.")]
		public bool everyFrame;

		private DelayedEvent delayedEvent;

		public override void Reset()
		{
			this.eventTarget = null;
			this.sendEvent = null;
			this.delay = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			if (this.delay.Value < 0.001f)
			{
				base.Fsm.Event(this.eventTarget, this.sendEvent.Value);
				base.Finish();
			}
			else
			{
				this.delayedEvent = base.Fsm.DelayedEvent(this.eventTarget, FsmEvent.GetFsmEvent(this.sendEvent.Value), this.delay.Value);
			}
		}

		public override void OnUpdate()
		{
			if (!this.everyFrame && DelayedEvent.WasSent(this.delayedEvent))
			{
				base.Finish();
			}
		}
	}
}
