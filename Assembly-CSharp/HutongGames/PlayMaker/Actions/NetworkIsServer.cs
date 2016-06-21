using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Network), HutongGames.PlayMaker.Tooltip("Test if your peer type is server.")]
	public class NetworkIsServer : FsmStateAction
	{
		[HutongGames.PlayMaker.Tooltip("True if running as server."), UIHint(UIHint.Variable)]
		public FsmBool isServer;

		[HutongGames.PlayMaker.Tooltip("Event to send if running as server.")]
		public FsmEvent isServerEvent;

		[HutongGames.PlayMaker.Tooltip("Event to send if not running as server.")]
		public FsmEvent isNotServerEvent;

		public override void Reset()
		{
			this.isServer = null;
		}

		public override void OnEnter()
		{
			this.DoCheckIsServer();
			base.Finish();
		}

		private void DoCheckIsServer()
		{
			this.isServer.Value = Network.isServer;
			if (Network.isServer && this.isServerEvent != null)
			{
				base.Fsm.Event(this.isServerEvent);
			}
			else if (!Network.isServer && this.isNotServerEvent != null)
			{
				base.Fsm.Event(this.isNotServerEvent);
			}
		}
	}
}
