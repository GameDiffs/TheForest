using Steamworks;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace TheForest.Networking
{
	public class AutoJoinAfterMPInvite : MonoBehaviour
	{
		[Serializable]
		public class InviteReceivedScreen
		{
			public CoopLobbyInfo _lobby;

			public GameObject _screen;

			public UILabel _gameName;

			public UIButton _continueSaveButton;
		}

		public AutoJoinAfterMPInvite.InviteReceivedScreen _inviteReceivedScreen;

		public TitleScreen _titleScreen;

		private Texture2D _textureOverlay;

		public static string LobbyID
		{
			get;
			set;
		}

		public ulong invitedTo
		{
			get;
			private set;
		}

		private static bool Done
		{
			get;
			set;
		}

		[DebuggerHidden]
		private IEnumerator Start()
		{
			AutoJoinAfterMPInvite.<Start>c__Iterator17F <Start>c__Iterator17F = new AutoJoinAfterMPInvite.<Start>c__Iterator17F();
			<Start>c__Iterator17F.<>f__this = this;
			return <Start>c__Iterator17F;
		}

		public void SetInvitedToGameId(ulong gameId)
		{
			CSteamID y = new CSteamID(gameId);
			if (y.IsValid() && (CoopLobby.Instance == null || CoopLobby.Instance.Info.LobbyId != y))
			{
				base.StartCoroutine(this.DelayedInviteReceived(new CoopLobbyInfo(gameId)));
			}
		}

		[DebuggerHidden]
		private IEnumerator DelayedInviteReceived(CoopLobbyInfo lobby)
		{
			AutoJoinAfterMPInvite.<DelayedInviteReceived>c__Iterator180 <DelayedInviteReceived>c__Iterator = new AutoJoinAfterMPInvite.<DelayedInviteReceived>c__Iterator180();
			<DelayedInviteReceived>c__Iterator.lobby = lobby;
			<DelayedInviteReceived>c__Iterator.<$>lobby = lobby;
			<DelayedInviteReceived>c__Iterator.<>f__this = this;
			return <DelayedInviteReceived>c__Iterator;
		}

		public void OnClientCancel()
		{
			this._inviteReceivedScreen._screen.SetActive(false);
			this._inviteReceivedScreen._lobby = null;
		}

		public void OnClientNewGame()
		{
			this.invitedTo = this._inviteReceivedScreen._lobby.LobbyId.m_SteamID;
			AutoJoinAfterMPInvite.LobbyID = this.invitedTo.ToString();
			this._titleScreen.OnCoOp();
			TitleScreen.StartGameSetup.Type = TitleScreen.GameSetup.InitTypes.New;
			this._titleScreen.OnStartMpClient();
		}

		public void OnClientContinueGame()
		{
			this.invitedTo = this._inviteReceivedScreen._lobby.LobbyId.m_SteamID;
			AutoJoinAfterMPInvite.LobbyID = this.invitedTo.ToString();
			this._titleScreen.OnCoOp();
			this._titleScreen.OnLoad();
			this._titleScreen.OnStartMpClient();
		}
	}
}
