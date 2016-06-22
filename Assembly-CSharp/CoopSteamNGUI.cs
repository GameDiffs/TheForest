using Bolt;
using Rewired;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TheForest.Networking;
using TheForest.UI;
using TheForest.Utils;
using UniLinq;
using UnityEngine;

public class CoopSteamNGUI : MonoBehaviour
{
	private enum Screens
	{
		ModalScreen,
		LobbySetup,
		Lobby,
		GameBrowser,
		InviteReceivedScreen
	}

	[Serializable]
	public class ModalScreen
	{
		public GameObject _screen;

		public UILabel _text;
	}

	[Serializable]
	public class InviteReceivedScreen
	{
		public GameObject _screen;

		public UILabel _gameName;

		public UIButton _continueSaveButton;
	}

	[Serializable]
	public class LobbySetupScreen
	{
		public GameObject _screen;

		public UIInput _gameNameInput;

		public UIInput _playerCountInput;

		public UIToggle _privateOnlyToggle;
	}

	[Serializable]
	public class LobbyScreen
	{
		public GameObject _screen;

		public UILabel _gameNameLabel;

		public UILabel _playerCountLabel;

		public UILabel _playerListLabel;
	}

	[Serializable]
	public class GameBrowserScreen
	{
		public GameObject _screen;

		public MpGameRow _rowPrefab;

		public UIScrollView _scrollview;

		public UIGrid _grid;

		public UIInput _filter;
	}

	public bool _allowLegacyGames;

	public LoadAsync _async;

	public UILabel _errorLabel;

	public CoopSteamNGUI.ModalScreen _modalScreen;

	public CoopSteamNGUI.InviteReceivedScreen _inviteReceivedScreen;

	public CoopSteamNGUI.LobbySetupScreen _lobbySetupScreen;

	public CoopSteamNGUI.LobbyScreen _lobbyScreen;

	public CoopSteamNGUI.GameBrowserScreen _gameBrowserScreen;

	public GameObject[] _hostOnlyGOs;

	public GameObject[] _clientOnlyGOs;

	public CoopLobbyInfo _currentInvitelobby;

	private bool _hostFriendsOnly;

	private string _hostGameName = "The Forest Game";

	private string _hostMaxPlayers = "4";

	private string _browserFilter = string.Empty;

	private CoopSteamNGUI.Screens _currentScreen;

	private CoopSteamNGUI.Screens _prevScreen;

	private IEnumerable<CoopLobbyInfo> _lobbies = new List<CoopLobbyInfo>(0);

	private Dictionary<CoopLobbyInfo, MpGameRow> _gameRows = new Dictionary<CoopLobbyInfo, MpGameRow>();

	private HashSet<string> _previouslyPlayedServers;

	public ResourceRequest PrefabDbResource
	{
		get;
		private set;
	}

	private void Awake()
	{
		CoopPeerStarter.Dedicated = false;
		CoopPeerStarter.DedicatedHost = false;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		this.PrefabDbResource = Resources.LoadAsync<PrefabDatabase>("BoltPrefabDatabase");
		BoltLauncher.SetUdpPlatform(new SteamPlatform());
		TheForest.Utils.Input.player.controllers.maps.SetMapsEnabled(true, ControllerType.Joystick, "Menu");
		if (!SteamManager.Initialized)
		{
			if (TitleScreen.StartGameSetup.MpType == TitleScreen.GameSetup.MpTypes.Server)
			{
				this.OpenScreen(CoopSteamNGUI.Screens.LobbySetup);
			}
			else
			{
				this.OpenScreen(CoopSteamNGUI.Screens.GameBrowser);
			}
			this.SetLoadingText("Steam not initialized");
			return;
		}
		this.RefreshUI();
		if (TitleScreen.StartGameSetup.MpType == TitleScreen.GameSetup.MpTypes.Server)
		{
			this._hostGameName = PlayerPrefs.GetString("MpGameName", this._hostGameName);
			this._hostMaxPlayers = PlayerPrefs.GetInt("MpGamePlayerCount", this.GetHostPlayersMax()).ToString();
			this._hostFriendsOnly = (PlayerPrefs.GetInt("MpGameFriendsOnly", (!this._hostFriendsOnly) ? 0 : 1) == 1);
			this._lobbySetupScreen._gameNameInput.value = this._hostGameName;
			this._lobbySetupScreen._playerCountInput.value = this._hostMaxPlayers;
			this._lobbySetupScreen._privateOnlyToggle.value = this._hostFriendsOnly;
			this.OpenScreen(CoopSteamNGUI.Screens.LobbySetup);
		}
		else
		{
			CoopLobbyManager.QueryList();
			this.OpenScreen(CoopSteamNGUI.Screens.GameBrowser);
		}
		if (AutoJoinAfterMPInvite.LobbyID != null && (CoopLobby.Instance == null || CoopLobby.Instance.Info.LobbyId.ToString() != AutoJoinAfterMPInvite.LobbyID))
		{
			CoopLobbyInfo lobby = new CoopLobbyInfo(ulong.Parse(AutoJoinAfterMPInvite.LobbyID));
			AutoJoinAfterMPInvite.LobbyID = null;
			if (TitleScreen.StartGameSetup.Type == TitleScreen.GameSetup.InitTypes.Continue)
			{
				this.OnClientContinueGame(lobby);
			}
			else
			{
				this.OnClientNewGame(lobby);
			}
		}
	}

	private void LateUpdate()
	{
		switch (this._currentScreen)
		{
		case CoopSteamNGUI.Screens.Lobby:
			this.UpdateLobby();
			break;
		case CoopSteamNGUI.Screens.GameBrowser:
			this.UpdateGameBrowser();
			break;
		}
	}

	public void OnHostLobbySetup()
	{
		this._hostGameName = this._lobbySetupScreen._gameNameInput.value;
		this._hostMaxPlayers = this._lobbySetupScreen._playerCountInput.value;
		this._hostFriendsOnly = this._lobbySetupScreen._privateOnlyToggle.value;
		PlayerPrefs.SetString("MpGameName", this._hostGameName);
		PlayerPrefs.SetInt("MpGamePlayerCount", this.GetHostPlayersMax());
		PlayerPrefs.SetInt("MpGameFriendsOnly", (!this._hostFriendsOnly) ? 0 : 1);
		PlayerPrefs.Save();
		bool flag = CoopSteamServer.Start(delegate
		{
			this.SetLoadingText("Creating Lobby ...");
			CoopLobbyManager.Create(this.GetHostGameName(), this.GetHostPlayersMax(), this._hostFriendsOnly, delegate
			{
				if (string.IsNullOrEmpty(CoopLobby.Instance.Info.Guid) && TitleScreen.StartGameSetup.Type == TitleScreen.GameSetup.InitTypes.New)
				{
					CoopLobby.Instance.SetGuid(Guid.NewGuid().ToString());
				}
				this._lobbyScreen._gameNameLabel.text = "Lobby: " + CoopLobby.Instance.Info.Name;
				this.OpenScreen(CoopSteamNGUI.Screens.Lobby);
			}, delegate
			{
				this.ClearLoadingAndError();
				this.SetErrorText("Could not create Steam lobby.");
			});
		}, delegate
		{
			this.SetErrorText("Could not connect to Steam master server.");
		});
		if (flag)
		{
			this.SetLoadingText("Talking To Steam ...");
		}
		else
		{
			this.SetErrorText("Could not start Steam game server.");
		}
	}

	public void OnHostInviteFriends()
	{
		SteamFriends.ActivateGameOverlayInviteDialog(CoopLobby.Instance.Info.LobbyId);
	}

	public void OnHostStartGame()
	{
		this.SetLoadingText("Starting Server ...");
		if (TitleScreen.StartGameSetup.Type == TitleScreen.GameSetup.InitTypes.New)
		{
			PlaneCrashAudioState.Spawn();
		}
		base.gameObject.AddComponent<CoopSteamServerStarter>().gui = this;
	}

	public void OnClientUpdateFilter()
	{
		this._browserFilter = this._gameBrowserScreen._filter.value.ToLowerInvariant();
	}

	public void OnClientRefreshGameList()
	{
		this._lobbies = new List<CoopLobbyInfo>(0);
		foreach (MpGameRow current in this._gameRows.Values)
		{
			UnityEngine.Object.Destroy(current.gameObject);
		}
		this._gameRows.Clear();
		CoopLobbyManager.QueryList();
	}

	public void OnClientContinueGame(CoopLobbyInfo lobby)
	{
		if (CoopLobby.IsInLobby)
		{
			CoopLobby.LeaveActive();
		}
		TitleScreen.StartGameSetup.Type = TitleScreen.GameSetup.InitTypes.Continue;
		TitleScreen.StartGameSetup.MpType = TitleScreen.GameSetup.MpTypes.Client;
		this.RefreshUI();
		if (this._currentScreen == CoopSteamNGUI.Screens.InviteReceivedScreen)
		{
			this._currentScreen = this._prevScreen;
			this.OpenScreen(CoopSteamNGUI.Screens.GameBrowser);
		}
		this.SetLoadingText(string.Format("Joining Lobby {0} ...", lobby.Name));
		lobby.UpdateData();
		CoopLobbyManager.Join(lobby, delegate
		{
			lobby.UpdateData();
			this.ClearLoadingAndError();
		}, delegate
		{
			this.ClearLoadingAndError();
			this.SetErrorText("Could not join Steam lobby.");
		});
	}

	public void OnClientNewGame(CoopLobbyInfo lobby)
	{
		if (CoopLobby.IsInLobby)
		{
			CoopLobby.LeaveActive();
		}
		TitleScreen.StartGameSetup.Type = TitleScreen.GameSetup.InitTypes.New;
		TitleScreen.StartGameSetup.MpType = TitleScreen.GameSetup.MpTypes.Client;
		this.RefreshUI();
		if (this._currentScreen == CoopSteamNGUI.Screens.InviteReceivedScreen)
		{
			this._currentScreen = this._prevScreen;
			this.OpenScreen(CoopSteamNGUI.Screens.GameBrowser);
		}
		this.SetLoadingText(string.Format("Joining Lobby {0} ...", lobby.Name));
		lobby.UpdateData();
		CoopLobbyManager.Join(lobby, delegate
		{
			lobby.UpdateData();
			this.ClearLoadingAndError();
		}, delegate
		{
			this.ClearLoadingAndError();
			this.SetErrorText("Could not join Steam lobby.");
		});
	}

	public void OnBack()
	{
		switch (this._currentScreen)
		{
		case CoopSteamNGUI.Screens.ModalScreen:
			if (TitleScreen.StartGameSetup.MpType == TitleScreen.GameSetup.MpTypes.Client)
			{
				CoopLobby.LeaveActive();
				if (this._prevScreen == CoopSteamNGUI.Screens.Lobby)
				{
					this._prevScreen = CoopSteamNGUI.Screens.GameBrowser;
					this.OnClientRefreshGameList();
				}
			}
			else
			{
				if (CoopLobby.Instance != null)
				{
					CoopLobby.Instance.Destroy();
				}
				CoopLobby.LeaveActive();
				CoopSteamServer.Shutdown();
				if (this._prevScreen == CoopSteamNGUI.Screens.Lobby)
				{
					this._prevScreen = CoopSteamNGUI.Screens.LobbySetup;
				}
			}
			this.OpenScreen(this._prevScreen);
			this._prevScreen = this._currentScreen;
			return;
		case CoopSteamNGUI.Screens.LobbySetup:
			CoopSteamClient.Shutdown();
			UnityEngine.Object.Destroy(base.gameObject);
			Application.LoadLevel("TitleScene");
			return;
		case CoopSteamNGUI.Screens.Lobby:
			if (TitleScreen.StartGameSetup.MpType == TitleScreen.GameSetup.MpTypes.Client)
			{
				CoopLobby.LeaveActive();
				if (this._prevScreen == CoopSteamNGUI.Screens.GameBrowser)
				{
					this.OpenScreen(CoopSteamNGUI.Screens.GameBrowser);
					this.OnClientRefreshGameList();
				}
				else if (this._prevScreen == CoopSteamNGUI.Screens.LobbySetup)
				{
					this.OpenScreen(CoopSteamNGUI.Screens.LobbySetup);
				}
				else
				{
					UnityEngine.Object.Destroy(base.gameObject);
					Application.LoadLevel("TitleScene");
				}
			}
			else
			{
				CoopLobby.Instance.Destroy();
				CoopLobby.LeaveActive();
				CoopSteamServer.Shutdown();
				this.OpenScreen(CoopSteamNGUI.Screens.LobbySetup);
			}
			return;
		}
		UnityEngine.Object.Destroy(base.gameObject);
		Application.LoadLevel("TitleScene");
	}

	private void RefreshUI()
	{
		GameObject[] hostOnlyGOs = this._hostOnlyGOs;
		for (int i = 0; i < hostOnlyGOs.Length; i++)
		{
			GameObject gameObject = hostOnlyGOs[i];
			gameObject.SetActive(TitleScreen.StartGameSetup.MpType == TitleScreen.GameSetup.MpTypes.Server);
		}
		GameObject[] clientOnlyGOs = this._clientOnlyGOs;
		for (int j = 0; j < clientOnlyGOs.Length; j++)
		{
			GameObject gameObject2 = clientOnlyGOs[j];
			gameObject2.SetActive(TitleScreen.StartGameSetup.MpType == TitleScreen.GameSetup.MpTypes.Client);
		}
	}

	private void OpenScreen(CoopSteamNGUI.Screens screen)
	{
		this.ClearLoadingAndError();
		this._prevScreen = this._currentScreen;
		this._modalScreen._screen.SetActive(screen == CoopSteamNGUI.Screens.ModalScreen);
		this._inviteReceivedScreen._screen.SetActive(screen == CoopSteamNGUI.Screens.InviteReceivedScreen);
		this._lobbySetupScreen._screen.SetActive(screen == CoopSteamNGUI.Screens.LobbySetup);
		this._lobbyScreen._screen.SetActive(screen == CoopSteamNGUI.Screens.Lobby);
		this._gameBrowserScreen._screen.SetActive(screen == CoopSteamNGUI.Screens.GameBrowser);
		this._currentScreen = screen;
		if (screen == CoopSteamNGUI.Screens.GameBrowser)
		{
			this._previouslyPlayedServers = SaveSlotUtils.GetPreviouslyPlayedServers();
		}
	}

	private string GetHostGameName()
	{
		if (this._hostGameName == null || this._hostGameName.Trim().Length == 0)
		{
			return SteamFriends.GetPersonaName() + "'s game";
		}
		return this._hostGameName.Trim() + " (" + SteamFriends.GetPersonaName() + ")";
	}

	private int GetHostPlayersMax()
	{
		int result;
		try
		{
			result = Mathf.Clamp(int.Parse(this._hostMaxPlayers), 2, 8);
		}
		catch
		{
			result = 4;
		}
		return result;
	}

	public void SetJoinText(LobbyInvite_t param)
	{
		base.StartCoroutine(this.DelayedInviteReceived(new CoopLobbyInfo(param.m_ulSteamIDLobby)));
	}

	[DebuggerHidden]
	private IEnumerator DelayedInviteReceived(CoopLobbyInfo lobby)
	{
		CoopSteamNGUI.<DelayedInviteReceived>c__Iterator29 <DelayedInviteReceived>c__Iterator = new CoopSteamNGUI.<DelayedInviteReceived>c__Iterator29();
		<DelayedInviteReceived>c__Iterator.lobby = lobby;
		<DelayedInviteReceived>c__Iterator.<$>lobby = lobby;
		<DelayedInviteReceived>c__Iterator.<>f__this = this;
		return <DelayedInviteReceived>c__Iterator;
	}

	private void SetLoadingText(string text)
	{
		this._modalScreen._screen.SetActive(true);
		this._modalScreen._text.text = text;
	}

	public void SetErrorText(string text)
	{
		this._errorLabel.text = text;
	}

	private void ClearLoadingAndError()
	{
		this._modalScreen._screen.SetActive(false);
		this._errorLabel.text = string.Empty;
	}

	private void UpdateLobby()
	{
		if (!CoopLobby.IsInLobby)
		{
			this.OnBack();
			return;
		}
		if (CoopLobby.Instance == null || CoopLobby.Instance.Info == null || CoopLobby.Instance.Info.Destroyed)
		{
			this.SetErrorText("Lobby Destroyed");
			this.OnBack();
			CoopLobby.LeaveActive();
			return;
		}
		if (!CoopLobby.Instance.Info.IsOwner && CoopLobby.Instance.Info.ServerId.IsValid())
		{
			if (!BoltNetwork.isClient)
			{
				base.gameObject.AddComponent<CoopSteamClientStarter>().gui = this;
				if (TitleScreen.StartGameSetup.Type == TitleScreen.GameSetup.InitTypes.New)
				{
					PlaneCrashAudioState.Spawn();
				}
				this.OpenScreen(CoopSteamNGUI.Screens.ModalScreen);
				this.SetLoadingText("Starting Client ...");
			}
		}
		else
		{
			bool foundHost = false;
			ulong ownerId = SteamMatchmaking.GetLobbyOwner(CoopLobby.Instance.Info.LobbyId).m_SteamID;
			this._lobbyScreen._playerCountLabel.text = string.Format("Players: {0} / {1}", CoopLobby.Instance.MemberCount, CoopLobby.Instance.Info.MemberLimit);
			this._lobbyScreen._playerListLabel.text = CoopLobby.Instance.AllMembers.Select(delegate(CSteamID x)
			{
				string text = SteamFriends.GetFriendPersonaName(x);
				bool flag = x.m_SteamID == ownerId;
				if (flag)
				{
					text += " (Host)";
					foundHost = true;
				}
				return text;
			}).Aggregate((string a, string b) => a + "\n" + b);
			if (!foundHost)
			{
				this.OnBack();
			}
		}
	}

	private void UpdateGameBrowser()
	{
		bool flag = false;
		if (CoopLobby.IsInLobby)
		{
			this.OpenScreen(CoopSteamNGUI.Screens.Lobby);
			this._lobbyScreen._gameNameLabel.text = "Lobby: " + CoopLobby.Instance.Info.Name;
			return;
		}
		this._lobbies = (from l in this._lobbies
		where l != null && CoopLobbyManager.Lobbies.Any((CoopLobbyInfo al) => al.LobbyId.m_SteamID == l.LobbyId.m_SteamID)
		select l).ToList<CoopLobbyInfo>();
		IEnumerable<CoopLobbyInfo> enumerable = (from nl in CoopLobbyManager.Lobbies
		where !string.IsNullOrEmpty(nl.Name) && (this._allowLegacyGames || !string.IsNullOrEmpty(nl.Guid)) && !nl.IsOwner && !this._lobbies.Any((CoopLobbyInfo l) => nl.LobbyId.m_SteamID == l.LobbyId.m_SteamID)
		select nl).Take(5);
		if (enumerable != null && enumerable.Any<CoopLobbyInfo>())
		{
			Vector3 localScale = this._gameBrowserScreen._rowPrefab.transform.localScale;
			foreach (CoopLobbyInfo current in enumerable)
			{
				MpGameRow mpGameRow = UnityEngine.Object.Instantiate<MpGameRow>(this._gameBrowserScreen._rowPrefab);
				mpGameRow.transform.parent = this._gameBrowserScreen._grid.transform;
				mpGameRow.transform.localScale = localScale;
				mpGameRow._gameName.text = current.Name;
				mpGameRow._lobby = current;
				mpGameRow._playerLimit.text = string.Format("{0} / {1}", current.CurrentMembers, current.MemberLimit);
				this._gameRows[current] = mpGameRow;
				mpGameRow._previousPlayed = this._previouslyPlayedServers.Contains(mpGameRow._lobby.Guid);
				if (mpGameRow._previousPlayed)
				{
					mpGameRow.name = "0";
					mpGameRow._newButtonLabel.transform.parent.gameObject.SetActive(true);
					mpGameRow._continueButtonLabel.text = "Continue";
				}
				else
				{
					mpGameRow._newButtonLabel.transform.parent.gameObject.SetActive(false);
					mpGameRow.name = "1";
				}
				MpGameRow expr_1EA = mpGameRow;
				expr_1EA.name += current.Name.Substring(0, 6);
			}
			this._lobbies = this._lobbies.Union(enumerable).ToList<CoopLobbyInfo>();
			flag = true;
		}
		bool flag2 = !string.IsNullOrEmpty(this._browserFilter);
		foreach (MpGameRow current2 in this._gameRows.Values)
		{
			if (flag2)
			{
				bool flag3 = current2._lobby.Name.ToLowerInvariant().Contains(this._browserFilter);
				if (current2.gameObject.activeSelf != flag3)
				{
					current2.transform.parent = ((!flag3) ? this._gameBrowserScreen._grid.transform.parent : this._gameBrowserScreen._grid.transform);
					current2.gameObject.SetActive(flag3);
					flag = true;
				}
			}
			else if (!current2.gameObject.activeSelf)
			{
				current2.transform.parent = this._gameBrowserScreen._grid.transform;
				current2.gameObject.SetActive(true);
				flag = true;
			}
		}
		if (flag)
		{
			this._gameBrowserScreen._grid.repositionNow = true;
			this._gameBrowserScreen._scrollview.UpdateScrollbars();
		}
	}
}
