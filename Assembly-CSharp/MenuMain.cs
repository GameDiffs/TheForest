using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Utils;
using UnityEngine;

public class MenuMain : MonoBehaviour
{
	private bool selected;

	public static bool exiting;

	public GameObject LoadingBar;

	public Transform MenuRoot;

	public UIButton PlayButton;

	public UIButton LoadButton;

	public UIButton ExitButton;

	public UIButton ExitButtonMainMenu;

	private PauseMenuAudio audio;

	private Texture2D texture_Overlay;

	private void OnEnable()
	{
		this.selected = false;
		EventDelegate.Add(this.PlayButton.onClick, new EventDelegate.Callback(this.OnPlay));
		EventDelegate.Add(this.ExitButton.onClick, new EventDelegate.Callback(this.OnExit));
		EventDelegate.Add(this.ExitButtonMainMenu.onClick, new EventDelegate.Callback(this.OnExitMenu));
		EventDelegate.Add(this.LoadButton.onClick, new EventDelegate.Callback(this.OnLoad));
		this.audio = this.MenuRoot.GetComponentInChildren<PauseMenuAudio>();
	}

	private void OnDisable()
	{
		this.selected = false;
		EventDelegate.Remove(this.PlayButton.onClick, new EventDelegate.Callback(this.OnPlay));
		EventDelegate.Remove(this.ExitButton.onClick, new EventDelegate.Callback(this.OnExit));
		EventDelegate.Remove(this.ExitButtonMainMenu.onClick, new EventDelegate.Callback(this.OnExitMenu));
		EventDelegate.Remove(this.LoadButton.onClick, new EventDelegate.Callback(this.OnLoad));
	}

	private void OnPlay()
	{
		if (this.selected)
		{
			return;
		}
		Time.timeScale = 1f;
		FirstPersonCharacter[] array = UnityEngine.Object.FindObjectsOfType<FirstPersonCharacter>();
		for (int i = 0; i < array.Length; i++)
		{
			FirstPersonCharacter firstPersonCharacter = array[i];
			firstPersonCharacter.UnLockView();
		}
		this.MenuRoot.gameObject.SetActive(false);
	}

	private void OnLoad()
	{
		if (this.selected)
		{
			return;
		}
		this.selected = true;
		if (LevelSerializer.CanResume)
		{
			this.LoadingBar.SetActive(true);
			TheForest.Utils.Input.LockMouse();
			if (this.audio != null)
			{
				this.audio.PrepareForLevelLoad();
			}
			base.Invoke("DelayedLoadLevel", 0.1f);
		}
		Time.timeScale = 1f;
		this.MenuRoot.gameObject.SetActive(false);
	}

	private void OnExitMenu()
	{
		if (this.selected)
		{
			return;
		}
		this.selected = true;
		Time.timeScale = 1f;
		if (this.audio != null)
		{
			this.audio.PrepareForLevelLoad();
		}
		if (BoltNetwork.isRunning)
		{
			if (CoopLobby.IsInLobby)
			{
				if (CoopLobby.Instance.Info.IsOwner)
				{
					CoopLobby.Instance.Destroy();
				}
				CoopLobby.LeaveActive();
			}
			base.StartCoroutine(this.WaitForBoltShutdown(delegate
			{
				CoopSteamServer.Shutdown();
				CoopSteamClient.Shutdown();
				CoopTreeGrid.Clear();
				TitleScreen.StartGameSetup.Type = TitleScreen.GameSetup.InitTypes.New;
				TitleScreen.StartGameSetup.Game = TitleScreen.GameSetup.GameModes.Standard;
				Application.LoadLevel("TitleSceneLoader");
			}));
		}
		else
		{
			CoopTreeGrid.Clear();
			TitleScreen.StartGameSetup.Type = TitleScreen.GameSetup.InitTypes.New;
			TitleScreen.StartGameSetup.Game = TitleScreen.GameSetup.GameModes.Standard;
			Application.LoadLevel("TitleSceneLoader");
		}
	}

	private void DelayedLoadLevel()
	{
		LevelSerializer.Resume();
	}

	private void OnExit()
	{
		if (this.selected)
		{
			return;
		}
		this.selected = true;
		if (BoltNetwork.isRunning)
		{
			UnityEngine.Object.DontDestroyOnLoad(base.transform.root.gameObject);
			MenuMain.exiting = true;
			if (CoopLobby.IsInLobby)
			{
				if (CoopLobby.Instance.Info.IsOwner)
				{
					CoopLobby.Instance.Destroy();
				}
				CoopLobby.LeaveActive();
			}
			base.StartCoroutine(this.WaitForBoltShutdown(delegate
			{
				Application.Quit();
			}));
		}
		else
		{
			Application.Quit();
		}
	}

	private void DrawOverlay()
	{
		if (this.texture_Overlay)
		{
			GUI.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), this.texture_Overlay, ScaleMode.StretchToFill, true);
		}
	}

	private void DrawLoader()
	{
		this.DrawOverlay();
		Matrix4x4 matrix = GUI.matrix;
		GUIUtility.RotateAroundPivot(Time.time * 360f, new Vector2((float)(Screen.width / 2), (float)(Screen.height / 2)));
		GUI.DrawTexture(new Rect((float)(Screen.width / 2 - 32), (float)(Screen.height / 2 - 32), 64f, 64f), Resources.Load("CoopLoaderTexture") as Texture2D);
		GUI.matrix = matrix;
	}

	private void OnGUI()
	{
		if (this.selected)
		{
			if (!this.texture_Overlay)
			{
				this.texture_Overlay = new Texture2D(2, 2);
				this.texture_Overlay.SetPixels(new Color[]
				{
					new Color(0f, 0f, 0f, 0.75f),
					new Color(0f, 0f, 0f, 0.75f),
					new Color(0f, 0f, 0f, 0.75f),
					new Color(0f, 0f, 0f, 0.75f)
				});
				this.texture_Overlay.Apply();
			}
			this.DrawOverlay();
			this.DrawLoader();
		}
	}

	[DebuggerHidden]
	private IEnumerator WaitForBoltShutdown(Action done)
	{
		MenuMain.<WaitForBoltShutdown>c__Iterator17D <WaitForBoltShutdown>c__Iterator17D = new MenuMain.<WaitForBoltShutdown>c__Iterator17D();
		<WaitForBoltShutdown>c__Iterator17D.done = done;
		<WaitForBoltShutdown>c__Iterator17D.<$>done = done;
		return <WaitForBoltShutdown>c__Iterator17D;
	}

	private void OnLevelWasLoaded()
	{
		if (MenuMain.exiting)
		{
			Application.Quit();
		}
	}
}
