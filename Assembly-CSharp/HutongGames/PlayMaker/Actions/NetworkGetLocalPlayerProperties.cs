using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Network), HutongGames.PlayMaker.Tooltip("Get the local network player properties")]
	public class NetworkGetLocalPlayerProperties : FsmStateAction
	{
		[HutongGames.PlayMaker.Tooltip("The IP address of this player."), UIHint(UIHint.Variable)]
		public FsmString IpAddress;

		[HutongGames.PlayMaker.Tooltip("The port of this player."), UIHint(UIHint.Variable)]
		public FsmInt port;

		[HutongGames.PlayMaker.Tooltip("The GUID for this player, used when connecting with NAT punchthrough."), UIHint(UIHint.Variable)]
		public FsmString guid;

		[HutongGames.PlayMaker.Tooltip("The external IP address of the network interface. This will only be populated after some external connection has been made."), UIHint(UIHint.Variable)]
		public FsmString externalIPAddress;

		[HutongGames.PlayMaker.Tooltip("Returns the external port of the network interface. This will only be populated after some external connection has been made."), UIHint(UIHint.Variable)]
		public FsmInt externalPort;

		public override void Reset()
		{
			this.IpAddress = null;
			this.port = null;
			this.guid = null;
			this.externalIPAddress = null;
			this.externalPort = null;
		}

		public override void OnEnter()
		{
			this.IpAddress.Value = Network.player.ipAddress;
			this.port.Value = Network.player.port;
			this.guid.Value = Network.player.guid;
			this.externalIPAddress.Value = Network.player.externalIP;
			this.externalPort.Value = Network.player.externalPort;
			base.Finish();
		}
	}
}
