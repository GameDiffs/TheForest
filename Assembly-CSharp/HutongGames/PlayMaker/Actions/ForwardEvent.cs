using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine), Tooltip("Forward an event recieved by this FSM to another target.")]
	public class ForwardEvent : FsmStateAction
	{
		[Tooltip("Forward to this target.")]
		public FsmEventTarget forwardTo;

		[Tooltip("The events to forward.")]
		public FsmEvent[] eventsToForward;

		[Tooltip("Should this action eat the events or pass them on.")]
		public bool eatEvents;

		public override void Reset()
		{
			this.forwardTo = new FsmEventTarget
			{
				target = FsmEventTarget.EventTarget.FSMComponent
			};
			this.eventsToForward = null;
			this.eatEvents = true;
		}

		public override bool Event(FsmEvent fsmEvent)
		{
			if (this.eventsToForward != null)
			{
				FsmEvent[] array = this.eventsToForward;
				for (int i = 0; i < array.Length; i++)
				{
					FsmEvent fsmEvent2 = array[i];
					if (fsmEvent2 == fsmEvent)
					{
						base.Fsm.Event(this.forwardTo, fsmEvent);
						return this.eatEvents;
					}
				}
			}
			return false;
		}
	}
}
