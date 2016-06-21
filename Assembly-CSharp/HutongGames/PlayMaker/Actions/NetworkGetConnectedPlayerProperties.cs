using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Network), HutongGames.PlayMaker.Tooltip("Get connected player properties.")]
	public class NetworkGetConnectedPlayerProperties : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("The player connection index.")]
		public FsmInt index;

		[ActionSection("Result"), HutongGames.PlayMaker.Tooltip("Get the IP address of this player."), UIHint(UIHint.Variable)]
		public FsmString IpAddress;

		[HutongGames.PlayMaker.Tooltip("Get the port of this player."), UIHint(UIHint.Variable)]
		public FsmInt port;

		[HutongGames.PlayMaker.Tooltip("Get the GUID for this player, used when connecting with NAT punchthrough."), UIHint(UIHint.Variable)]
		public FsmString guid;

		[HutongGames.PlayMaker.Tooltip("Get the external IP address of the network interface. This will only be populated after some external connection has been made."), UIHint(UIHint.Variable)]
		public FsmString externalIPAddress;

		[HutongGames.PlayMaker.Tooltip("Get the external port of the network interface. This will only be populated after some external connection has been made."), UIHint(UIHint.Variable)]
		public FsmInt externalPort;

		public override void Reset()
		{
			this.index = null;
			this.IpAddress = null;
			this.port = null;
			this.guid = null;
			this.externalIPAddress = null;
			this.externalPort = null;
		}

		public override void OnEnter()
		{
			this.getPlayerProperties();
			base.Finish();
		}

		private void getPlayerProperties()
		{
			int value = this.index.Value;
			if (value < 0 || value >= Network.connections.Length)
			{
				this.LogError("Player index out of range");
				return;
			}
			NetworkPlayer networkPlayer = Network.connections[value];
			this.IpAddress.Value = networkPlayer.ipAddress;
			this.port.Value = networkPlayer.port;
			this.guid.Value = networkPlayer.guid;
			this.externalIPAddress.Value = networkPlayer.externalIP;
			this.externalPort.Value = networkPlayer.externalPort;
		}
	}
}
