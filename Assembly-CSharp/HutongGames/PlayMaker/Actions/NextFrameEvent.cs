using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine), Tooltip("Sends an Event in the next frame. Useful if you want to loop states every frame.")]
	public class NextFrameEvent : FsmStateAction
	{
		[RequiredField]
		public FsmEvent sendEvent;

		public override void Reset()
		{
			this.sendEvent = null;
		}

		public override void OnEnter()
		{
		}

		public override void OnUpdate()
		{
			base.Finish();
			base.Fsm.Event(this.sendEvent);
		}
	}
}
