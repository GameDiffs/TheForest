using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Network), HutongGames.PlayMaker.Tooltip("Launch a server.")]
	public class NetworkInitializeServer : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("The number of allowed incoming connections/number of players allowed in the game.")]
		public FsmInt connections;

		[RequiredField, HutongGames.PlayMaker.Tooltip("The port number we want to listen to.")]
		public FsmInt listenPort;

		[HutongGames.PlayMaker.Tooltip("Sets the password for the server. This must be matched in the NetworkConnect action.")]
		public FsmString incomingPassword;

		[HutongGames.PlayMaker.Tooltip("Sets the NAT punchthrough functionality.")]
		public FsmBool useNAT;

		[HutongGames.PlayMaker.Tooltip("Unity handles the network layer by providing secure connections if you wish to use them. \nMost games will want to use secure connections. However, they add up to 15 bytes per packet and take time to compute so you may wish to limit usage to deployed games only.")]
		public FsmBool useSecurityLayer;

		[HutongGames.PlayMaker.Tooltip("Run the server in the background, even if it doesn't have focus.")]
		public FsmBool runInBackground;

		[ActionSection("Errors"), HutongGames.PlayMaker.Tooltip("Event to send in case of an error creating the server.")]
		public FsmEvent errorEvent;

		[HutongGames.PlayMaker.Tooltip("Store the error string in a variable."), UIHint(UIHint.Variable)]
		public FsmString errorString;

		public override void Reset()
		{
			this.connections = 32;
			this.listenPort = 25001;
			this.incomingPassword = string.Empty;
			this.errorEvent = null;
			this.errorString = null;
			this.useNAT = false;
			this.useSecurityLayer = false;
			this.runInBackground = true;
		}

		public override void OnEnter()
		{
			Network.incomingPassword = this.incomingPassword.Value;
			if (this.useSecurityLayer.Value)
			{
				Network.InitializeSecurity();
			}
			if (this.runInBackground.Value)
			{
				Application.runInBackground = true;
			}
			NetworkConnectionError networkConnectionError = Network.InitializeServer(this.connections.Value, this.listenPort.Value, this.useNAT.Value);
			if (networkConnectionError != NetworkConnectionError.NoError)
			{
				this.errorString.Value = networkConnectionError.ToString();
				this.LogError(this.errorString.Value);
				base.Fsm.Event(this.errorEvent);
			}
			base.Finish();
		}
	}
}
