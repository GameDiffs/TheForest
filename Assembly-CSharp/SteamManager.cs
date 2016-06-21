using Steamworks;
using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

internal class SteamManager : MonoBehaviour
{
	private static SteamManager s_instance;

	private bool m_bInitialized;

	private SteamAPIWarningMessageHook_t m_SteamAPIWarningMessageHook;

	public static AppId_t AppId
	{
		get
		{
			return new AppId_t(242760u);
		}
	}

	public static int BuildId
	{
		get;
		private set;
	}

	public static string BetaName
	{
		get;
		private set;
	}

	private static SteamManager Instance
	{
		get
		{
			return SteamManager.s_instance ?? new GameObject("SteamManager").AddComponent<SteamManager>();
		}
	}

	public static bool Initialized
	{
		get
		{
			return SteamManager.Instance.m_bInitialized;
		}
	}

	[DllImport("kernel32.dll")]
	private static extern void SetDllDirectory(string dir);

	private static void AddSearchDir(string dir)
	{
		Debug.LogFormat("Added {0} to DLL search path", new object[]
		{
			dir
		});
		SteamManager.SetDllDirectory(dir);
	}

	private static void SteamAPIDebugTextHook(int nSeverity, StringBuilder pchDebugText)
	{
		Debug.LogWarning(pchDebugText);
	}

	private void Awake()
	{
		if (SteamManager.s_instance != null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		SteamManager.s_instance = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		if (!Packsize.Test())
		{
			Debug.LogError("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.", this);
		}
		if (!DllCheck.Test())
		{
			Debug.LogError("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.", this);
		}
		try
		{
			if (SteamAPI.RestartAppIfNecessary(SteamManager.AppId))
			{
				Application.Quit();
				return;
			}
		}
		catch (DllNotFoundException arg)
		{
			Debug.LogError("[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib. It's likely not in the correct location. Refer to the README for more details.\n" + arg, this);
			Application.Quit();
			return;
		}
		this.m_bInitialized = SteamAPI.Init();
		if (!this.m_bInitialized)
		{
			Debug.LogError("[Steamworks.NET] SteamAPI_Init() failed. Refer to Valve's documentation or the comment above this line for more information.", this);
			return;
		}
		SteamManager.BuildId = SteamApps.GetAppBuildId();
		string betaName;
		if (SteamApps.GetCurrentBetaName(out betaName, 50))
		{
			SteamManager.BetaName = betaName;
		}
		Debug.Log("Steam Started");
		CoopSteamManager.Initialize();
	}

	private void OnEnable()
	{
		if (SteamManager.s_instance == null)
		{
			SteamManager.s_instance = this;
		}
		if (!this.m_bInitialized)
		{
			return;
		}
		if (this.m_SteamAPIWarningMessageHook == null)
		{
			this.m_SteamAPIWarningMessageHook = new SteamAPIWarningMessageHook_t(SteamManager.SteamAPIDebugTextHook);
			SteamClient.SetWarningMessageHook(this.m_SteamAPIWarningMessageHook);
		}
	}

	private void OnDestroy()
	{
		if (SteamManager.s_instance != this)
		{
			return;
		}
		SteamManager.s_instance = null;
		if (!this.m_bInitialized)
		{
			return;
		}
		CoopSteamServer.Shutdown();
		CoopSteamClient.Shutdown();
		CoopSteamManager.Shutdown();
		CoopLobbyManager.Shutdown();
		SteamAPI.Shutdown();
	}

	private void Update()
	{
		if (!this.m_bInitialized)
		{
			return;
		}
		SteamAPI.RunCallbacks();
		CoopSteamServer.Update();
		CoopSteamClient.Update();
	}
}
