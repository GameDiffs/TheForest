using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Network), HutongGames.PlayMaker.Tooltip("Send Events based on the status of the network interface peer type: Disconneced, Server, Client, Connecting.")]
	public class NetworkPeerTypeSwitch : FsmStateAction
	{
		[HutongGames.PlayMaker.Tooltip("Event to send if no client connection running. Server not initialized.")]
		public FsmEvent isDisconnected;

		[HutongGames.PlayMaker.Tooltip("Event to send if running as server.")]
		public FsmEvent isServer;

		[HutongGames.PlayMaker.Tooltip("Event to send if running as client.")]
		public FsmEvent isClient;

		[HutongGames.PlayMaker.Tooltip("Event to send attempting to connect to a server.")]
		public FsmEvent isConnecting;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if you're waiting for a particular network state.")]
		public bool everyFrame;

		public override void Reset()
		{
			this.isDisconnected = null;
			this.isServer = null;
			this.isClient = null;
			this.isConnecting = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoNetworkPeerTypeSwitch();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoNetworkPeerTypeSwitch();
		}

		private void DoNetworkPeerTypeSwitch()
		{
			switch (Network.peerType)
			{
			case NetworkPeerType.Disconnected:
				base.Fsm.Event(this.isDisconnected);
				break;
			case NetworkPeerType.Server:
				base.Fsm.Event(this.isServer);
				break;
			case NetworkPeerType.Client:
				base.Fsm.Event(this.isClient);
				break;
			case NetworkPeerType.Connecting:
				base.Fsm.Event(this.isConnecting);
				break;
			}
		}
	}
}
