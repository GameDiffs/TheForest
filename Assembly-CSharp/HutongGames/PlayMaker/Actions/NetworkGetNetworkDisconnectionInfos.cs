using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Network), HutongGames.PlayMaker.Tooltip("Get the network OnDisconnectedFromServer.")]
	public class NetworkGetNetworkDisconnectionInfos : FsmStateAction
	{
		[HutongGames.PlayMaker.Tooltip("Disconnection label"), UIHint(UIHint.Variable)]
		public FsmString disconnectionLabel;

		[HutongGames.PlayMaker.Tooltip("The connection to the system has been lost, no reliable packets could be delivered.")]
		public FsmEvent lostConnectionEvent;

		[HutongGames.PlayMaker.Tooltip("The connection to the system has been closed.")]
		public FsmEvent disConnectedEvent;

		public override void Reset()
		{
			this.disconnectionLabel = null;
			this.lostConnectionEvent = null;
			this.disConnectedEvent = null;
		}

		public override void OnEnter()
		{
			this.doGetNetworkDisconnectionInfo();
			base.Finish();
		}

		private void doGetNetworkDisconnectionInfo()
		{
			NetworkDisconnection disconnectionInfo = Fsm.EventData.DisconnectionInfo;
			this.disconnectionLabel.Value = disconnectionInfo.ToString();
			NetworkDisconnection networkDisconnection = disconnectionInfo;
			if (networkDisconnection != NetworkDisconnection.Disconnected)
			{
				if (networkDisconnection == NetworkDisconnection.LostConnection)
				{
					if (this.lostConnectionEvent != null)
					{
						base.Fsm.Event(this.lostConnectionEvent);
					}
				}
			}
			else if (this.disConnectedEvent != null)
			{
				base.Fsm.Event(this.disConnectedEvent);
			}
		}
	}
}
