using System;
using TheForest.Utils;
using UnityEngine;

public class TitleScreen : MonoBehaviour
{
	public struct GameSetup
	{
		public enum PlayerModes
		{
			SinglePlayer,
			Multiplayer
		}

		public enum GameModes
		{
			Standard,
			Horde
		}

		public enum InitTypes
		{
			New,
			Continue
		}

		public enum MpTypes
		{
			Server,
			Client
		}

		public enum Slots
		{
			Slot1 = 1,
			Slot2,
			Slot3,
			Slot4,
			Slot5
		}

		public TitleScreen.GameSetup.GameModes Game;

		public TitleScreen.GameSetup.PlayerModes Mode;

		public TitleScreen.GameSetup.InitTypes Type;

		public TitleScreen.GameSetup.MpTypes MpType;

		public TitleScreen.GameSetup.Slots Slot;
	}

	public static TitleScreen.GameSetup StartGameSetup = new TitleScreen.GameSetup
	{
		Slot = TitleScreen.GameSetup.Slots.Slot1
	};

	public UILabel BreadcumbLabel;

	public GameObject MyLoader;

	public Transform MenuRoot;

	public string CoopScene = "SteamStartScene";

	public GameObject PlayerPrefab;

	private void Awake()
	{
		TitleScreen.StartGameSetup.Type = TitleScreen.GameSetup.InitTypes.New;
		TitleScreen.StartGameSetup.Game = TitleScreen.GameSetup.GameModes.Standard;
		LoadSave.ShouldLoad = false;
		CoopAckChecker.ACKED = false;
	}

	private void OnEnable()
	{
		this.BreadCrumbLevel0();
		CoopSteamNGUI coopSteamNGUI = UnityEngine.Object.FindObjectOfType<CoopSteamNGUI>();
		if (coopSteamNGUI)
		{
			UnityEngine.Object.Destroy(coopSteamNGUI.gameObject);
		}
		if (BoltNetwork.isRunning)
		{
			BoltLauncher.Shutdown();
		}
		if (MenuMain.exiting)
		{
			Application.Quit();
		}
		TheForest.Utils.Input.UnLockMouse();
	}

	private void Update()
	{
		Application.targetFrameRate = 60;
	}

	private void OnDestroy()
	{
		Application.targetFrameRate = PlayerPreferences.MaxFrameRate;
		base.enabled = false;
	}

	public void BreadCrumbLevel0()
	{
		this.BreadcumbLabel.enabled = false;
	}

	public void BreadCrumbLevel1()
	{
		this.BreadcumbLabel.enabled = true;
		this.BreadcumbLabel.text = TitleScreen.StartGameSetup.Mode.ToString();
	}

	public void BreadCrumbLevel2()
	{
		this.BreadcumbLabel.enabled = true;
		this.BreadcumbLabel.text = TitleScreen.StartGameSetup.Mode + "." + TitleScreen.StartGameSetup.Type;
	}

	public void BreadCrumbLevel2Mp()
	{
		this.BreadcumbLabel.enabled = true;
		this.BreadcumbLabel.text = TitleScreen.StartGameSetup.Mode + "." + TitleScreen.StartGameSetup.MpType;
	}

	public void BreadCrumbLevel3Mp()
	{
		this.BreadcumbLabel.enabled = true;
		this.BreadcumbLabel.text = string.Concat(new object[]
		{
			TitleScreen.StartGameSetup.Mode,
			".",
			TitleScreen.StartGameSetup.MpType,
			".",
			TitleScreen.StartGameSetup.Type
		});
	}

	public void OnStartDedicated()
	{
		Application.LoadLevel("SteamStartSceneDedicatedServer");
	}

	public void OnJoinDedicated()
	{
		Application.LoadLevel("SteamStartSceneDedicatedServer_Client");
	}

	public void OnSinglePlayer()
	{
		TitleScreen.StartGameSetup.Mode = TitleScreen.GameSetup.PlayerModes.SinglePlayer;
		this.BreadCrumbLevel1();
	}

	public void OnCoOp()
	{
		TitleScreen.StartGameSetup.Mode = TitleScreen.GameSetup.PlayerModes.Multiplayer;
		this.BreadCrumbLevel1();
	}

	public void OnMpHost()
	{
		TitleScreen.StartGameSetup.MpType = TitleScreen.GameSetup.MpTypes.Server;
		this.BreadCrumbLevel2Mp();
	}

	public void OnNewGame()
	{
		TitleScreen.StartGameSetup.Type = TitleScreen.GameSetup.InitTypes.New;
		if (TitleScreen.StartGameSetup.Mode == TitleScreen.GameSetup.PlayerModes.SinglePlayer)
		{
			this.BreadCrumbLevel2();
			PlaneCrashAudioState.Spawn();
			LoadSave.ShouldLoad = (TitleScreen.StartGameSetup.Type == TitleScreen.GameSetup.InitTypes.Continue);
			if (!this.MyLoader)
			{
				this.FixMissingLoaderRef();
			}
			this.MyLoader.SetActive(true);
		}
		else
		{
			this.BreadCrumbLevel3Mp();
			Application.LoadLevel(this.CoopScene);
		}
		this.MenuRoot.gameObject.SetActive(false);
	}

	public void OnLoad()
	{
		TitleScreen.StartGameSetup.Type = TitleScreen.GameSetup.InitTypes.Continue;
		if (TitleScreen.StartGameSetup.Mode == TitleScreen.GameSetup.PlayerModes.SinglePlayer)
		{
			this.BreadCrumbLevel2();
		}
		else
		{
			this.BreadCrumbLevel3Mp();
		}
	}

	public void OnSlotSelection(int slotNum)
	{
		TitleScreen.StartGameSetup.Slot = (TitleScreen.GameSetup.Slots)slotNum;
		if (TitleScreen.StartGameSetup.Mode == TitleScreen.GameSetup.PlayerModes.SinglePlayer)
		{
			LoadSave.ShouldLoad = (TitleScreen.StartGameSetup.Type == TitleScreen.GameSetup.InitTypes.Continue);
			if (!this.MyLoader)
			{
				this.FixMissingLoaderRef();
			}
			this.MyLoader.SetActive(true);
		}
		else
		{
			Application.LoadLevel(this.CoopScene);
		}
		this.MenuRoot.gameObject.SetActive(false);
	}

	public void OnStartMpClient()
	{
		TitleScreen.StartGameSetup.MpType = TitleScreen.GameSetup.MpTypes.Client;
		Application.LoadLevel(this.CoopScene);
		this.MenuRoot.gameObject.SetActive(false);
	}

	private void OnCredits()
	{
		Application.LoadLevelAsync("CreditsScene");
	}

	public void OnExit()
	{
		PlayerPreferences.Save();
		Application.Quit();
	}

	private void FixMissingLoaderRef()
	{
		Debug.LogError("Missing 'loader' reference in title screen");
		foreach (Transform transform in base.transform.parent)
		{
			if (transform.name == "Loading")
			{
				this.MyLoader = transform.gameObject;
			}
		}
	}
}
