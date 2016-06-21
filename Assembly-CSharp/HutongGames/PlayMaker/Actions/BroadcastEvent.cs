using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine), Tooltip("Sends an Event to all FSMs in the scene or to all FSMs on a Game Object.\nNOTE: This action won't work on the very first frame of the game..."), Obsolete("This action is obsolete; use Send Event with Event Target instead.")]
	public class BroadcastEvent : FsmStateAction
	{
		[RequiredField]
		public FsmString broadcastEvent;

		[Tooltip("Optionally specify a game object to broadcast the event to all FSMs on that game object.")]
		public FsmGameObject gameObject;

		[Tooltip("Broadcast to all FSMs on the game object's children.")]
		public FsmBool sendToChildren;

		public FsmBool excludeSelf;

		public override void Reset()
		{
			this.broadcastEvent = null;
			this.gameObject = null;
			this.sendToChildren = false;
			this.excludeSelf = false;
		}

		public override void OnEnter()
		{
			if (!string.IsNullOrEmpty(this.broadcastEvent.Value))
			{
				if (this.gameObject.Value != null)
				{
					base.Fsm.BroadcastEventToGameObject(this.gameObject.Value, this.broadcastEvent.Value, this.sendToChildren.Value, this.excludeSelf.Value);
				}
				else
				{
					base.Fsm.BroadcastEvent(this.broadcastEvent.Value, this.excludeSelf.Value);
				}
			}
			base.Finish();
		}
	}
}
