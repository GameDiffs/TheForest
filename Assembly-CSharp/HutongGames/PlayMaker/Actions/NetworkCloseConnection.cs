using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Network), HutongGames.PlayMaker.Tooltip("Close the connection to another system.\n\nConnection index defines which system to close the connection to (from the Network connections array).\nCan define connection to close via Guid if index is unknown. \nIf we are a client the only possible connection to close is the server connection, if we are a server the target player will be kicked off. \n\nSend Disconnection Notification enables or disables notifications being sent to the other end. If disabled the connection is dropped, if not a disconnect notification is reliably sent to the remote party and there after the connection is dropped.")]
	public class NetworkCloseConnection : FsmStateAction
	{
		[HutongGames.PlayMaker.Tooltip("Connection index to close"), UIHint(UIHint.Variable)]
		public FsmInt connectionIndex;

		[HutongGames.PlayMaker.Tooltip("Connection GUID to close. Used If Index is not set."), UIHint(UIHint.Variable)]
		public FsmString connectionGUID;

		[HutongGames.PlayMaker.Tooltip("If True, send Disconnection Notification")]
		public bool sendDisconnectionNotification;

		public override void Reset()
		{
			this.connectionIndex = 0;
			this.connectionGUID = null;
			this.sendDisconnectionNotification = true;
		}

		public override void OnEnter()
		{
			int num = 0;
			int num2;
			if (!this.connectionIndex.IsNone)
			{
				num = this.connectionIndex.Value;
			}
			else if (!this.connectionGUID.IsNone && this.getIndexFromGUID(this.connectionGUID.Value, out num2))
			{
				num = num2;
			}
			if (num < 0 || num > Network.connections.Length)
			{
				this.LogError("Connection index out of range: " + num);
			}
			else
			{
				Network.CloseConnection(Network.connections[num], this.sendDisconnectionNotification);
			}
			base.Finish();
		}

		private bool getIndexFromGUID(string guid, out int guidIndex)
		{
			for (int i = 0; i < Network.connections.Length; i++)
			{
				if (guid.Equals(Network.connections[i].guid))
				{
					guidIndex = i;
					return true;
				}
			}
			guidIndex = 0;
			return false;
		}
	}
}
