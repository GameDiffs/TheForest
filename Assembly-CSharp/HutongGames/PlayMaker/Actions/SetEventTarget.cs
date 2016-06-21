using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine), Tooltip("Sets the target FSM for all subsequent events sent by this state. The default 'Self' sends events to this FSM.")]
	public class SetEventTarget : FsmStateAction
	{
		public FsmEventTarget eventTarget;

		public override void Reset()
		{
		}

		public override void OnEnter()
		{
			base.Fsm.EventTarget = this.eventTarget;
			base.Finish();
		}
	}
}
