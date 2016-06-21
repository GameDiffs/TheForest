using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Network), HutongGames.PlayMaker.Tooltip("Get the next host data from the master server. \nEach time this action is called it gets the next connected host.This lets you quickly loop through all the connected hosts to get information on each one.")]
	public class MasterServerGetNextHostData : FsmStateAction
	{
		[ActionSection("Set up"), HutongGames.PlayMaker.Tooltip("Event to send for looping.")]
		public FsmEvent loopEvent;

		[HutongGames.PlayMaker.Tooltip("Event to send when there are no more hosts.")]
		public FsmEvent finishedEvent;

		[ActionSection("Result"), HutongGames.PlayMaker.Tooltip("The index into the MasterServer Host List"), UIHint(UIHint.Variable)]
		public FsmInt index;

		[HutongGames.PlayMaker.Tooltip("Does this server require NAT punchthrough?"), UIHint(UIHint.Variable)]
		public FsmBool useNat;

		[HutongGames.PlayMaker.Tooltip("The type of the game (e.g., 'MyUniqueGameType')"), UIHint(UIHint.Variable)]
		public FsmString gameType;

		[HutongGames.PlayMaker.Tooltip("The name of the game (e.g., 'John Does's Game')"), UIHint(UIHint.Variable)]
		public FsmString gameName;

		[HutongGames.PlayMaker.Tooltip("Currently connected players"), UIHint(UIHint.Variable)]
		public FsmInt connectedPlayers;

		[HutongGames.PlayMaker.Tooltip("Maximum players limit"), UIHint(UIHint.Variable)]
		public FsmInt playerLimit;

		[HutongGames.PlayMaker.Tooltip("Server IP address."), UIHint(UIHint.Variable)]
		public FsmString ipAddress;

		[HutongGames.PlayMaker.Tooltip("Server port"), UIHint(UIHint.Variable)]
		public FsmInt port;

		[HutongGames.PlayMaker.Tooltip("Does the server require a password?"), UIHint(UIHint.Variable)]
		public FsmBool passwordProtected;

		[HutongGames.PlayMaker.Tooltip("A miscellaneous comment (can hold data)"), UIHint(UIHint.Variable)]
		public FsmString comment;

		[HutongGames.PlayMaker.Tooltip("The GUID of the host, needed when connecting with NAT punchthrough."), UIHint(UIHint.Variable)]
		public FsmString guid;

		private int nextItemIndex;

		private bool noMoreItems;

		public override void Reset()
		{
			this.finishedEvent = null;
			this.loopEvent = null;
			this.index = null;
			this.useNat = null;
			this.gameType = null;
			this.gameName = null;
			this.connectedPlayers = null;
			this.playerLimit = null;
			this.ipAddress = null;
			this.port = null;
			this.passwordProtected = null;
			this.comment = null;
			this.guid = null;
		}

		public override void OnEnter()
		{
			this.DoGetNextHostData();
			base.Finish();
		}

		private void DoGetNextHostData()
		{
			if (this.nextItemIndex >= MasterServer.PollHostList().Length)
			{
				this.nextItemIndex = 0;
				base.Fsm.Event(this.finishedEvent);
				return;
			}
			HostData hostData = MasterServer.PollHostList()[this.nextItemIndex];
			this.index.Value = this.nextItemIndex;
			this.useNat.Value = hostData.useNat;
			this.gameType.Value = hostData.gameType;
			this.gameName.Value = hostData.gameName;
			this.connectedPlayers.Value = hostData.connectedPlayers;
			this.playerLimit.Value = hostData.playerLimit;
			this.ipAddress.Value = hostData.ip[0];
			this.port.Value = hostData.port;
			this.passwordProtected.Value = hostData.passwordProtected;
			this.comment.Value = hostData.comment;
			this.guid.Value = hostData.guid;
			if (this.nextItemIndex >= MasterServer.PollHostList().Length)
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
