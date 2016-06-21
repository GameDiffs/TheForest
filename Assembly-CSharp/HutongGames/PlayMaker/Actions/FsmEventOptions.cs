using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.StateMachine), Tooltip("Sets how subsequent events sent in this state are handled.")]
	public class FsmEventOptions : FsmStateAction
	{
		public PlayMakerFSM sendToFsmComponent;

		public FsmGameObject sendToGameObject;

		public FsmString fsmName;

		public FsmBool sendToChildren;

		public FsmBool broadcastToAll;

		public override void Reset()
		{
			this.sendToFsmComponent = null;
			this.sendToGameObject = null;
			this.fsmName = string.Empty;
			this.sendToChildren = false;
			this.broadcastToAll = false;
		}

		public override void OnUpdate()
		{
		}
	}
}
