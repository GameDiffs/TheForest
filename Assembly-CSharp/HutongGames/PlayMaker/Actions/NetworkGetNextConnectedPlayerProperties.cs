using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Network), HutongGames.PlayMaker.Tooltip("Get the next connected player properties. \nEach time this action is called it gets the next child of a GameObject.This lets you quickly loop through all the connected player to perform actions on them.")]
	public class NetworkGetNextConnectedPlayerProperties : FsmStateAction
	{
		[ActionSection("Set up"), HutongGames.PlayMaker.Tooltip("Event to send for looping.")]
		public FsmEvent loopEvent;

		[HutongGames.PlayMaker.Tooltip("Event to send when there are no more children.")]
		public FsmEvent finishedEvent;

		[ActionSection("Result"), HutongGames.PlayMaker.Tooltip("The player connection index."), UIHint(UIHint.Variable)]
		public FsmInt index;

		[HutongGames.PlayMaker.Tooltip("Get the IP address of this player."), UIHint(UIHint.Variable)]
		public FsmString IpAddress;

		[HutongGames.PlayMaker.Tooltip("Get the port of this player."), UIHint(UIHint.Variable)]
		public FsmInt port;

		[HutongGames.PlayMaker.Tooltip("Get the GUID for this player, used when connecting with NAT punchthrough."), UIHint(UIHint.Variable)]
		public FsmString guid;

		[HutongGames.PlayMaker.Tooltip("Get the external IP address of the network interface. This will only be populated after some external connection has been made."), UIHint(UIHint.Variable)]
		public FsmString externalIPAddress;

		[HutongGames.PlayMaker.Tooltip("Get the external port of the network interface. This will only be populated after some external connection has been made."), UIHint(UIHint.Variable)]
		public FsmInt externalPort;

		private int nextItemIndex;

		public override void Reset()
		{
			this.finishedEvent = null;
			this.loopEvent = null;
			this.index = null;
			this.IpAddress = null;
			this.port = null;
			this.guid = null;
			this.externalIPAddress = null;
			this.externalPort = null;
		}

		public override void OnEnter()
		{
			this.DoGetNextPlayerProperties();
			base.Finish();
		}

		private void DoGetNextPlayerProperties()
		{
			if (this.nextItemIndex >= Network.connections.Length)
			{
				base.Fsm.Event(this.finishedEvent);
				this.nextItemIndex = 0;
				return;
			}
			NetworkPlayer networkPlayer = Network.connections[this.nextItemIndex];
			this.index.Value = this.nextItemIndex;
			this.IpAddress.Value = networkPlayer.ipAddress;
			this.port.Value = networkPlayer.port;
			this.guid.Value = networkPlayer.guid;
			this.externalIPAddress.Value = networkPlayer.externalIP;
			this.externalPort.Value = networkPlayer.externalPort;
			if (this.nextItemIndex >= Network.connections.Length)
			{
				base.Fsm.Event(this.finishedEvent);
				this.nextItemIndex = 0;
				return;
			}
			this.nextItemIndex++;
			if (this.loopEvent != null)
			{
				base.Fsm.Event(this.loopEvent);
			}
		}
	}
}
