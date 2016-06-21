using Bolt;
using Steamworks;
using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Save;
using UdpKit;
using UnityEngine;

internal class CoopSteamClientStarter : CoopPeerStarter
{
	private bool _planePosArrived;

	private bool _planeRotArrived;

	private int _connectionAttempts;

	public static bool IsAdmin;

	[HideInInspector]
	public string dedicatedHostEndPoint = "127.0.0.1:27015";

	[DebuggerHidden]
	private IEnumerator Start()
	{
		CoopSteamClientStarter.<Start>c__Iterator26 <Start>c__Iterator = new CoopSteamClientStarter.<Start>c__Iterator26();
		<Start>c__Iterator.<>f__this = this;
		return <Start>c__Iterator;
	}

	protected new void Update()
	{
		if (this._planePosArrived && this._planeRotArrived && CoopAckChecker.ACKED && this.mapState == CoopPeerStarter.MapState.None)
		{
			base.CancelInvoke("OnDisconnected");
			CoopClientCallbacks.OnDisconnected = null;
			this.mapState = CoopPeerStarter.MapState.Begin;
		}
		base.Update();
	}

	public override void ConnectAttempt(UdpEndPoint endpoint, IProtocolToken token)
	{
		UnityEngine.Debug.Log("ConnectAttempt");
		base.CancelInvoke("OnDisconnected");
		base.Invoke("OnDisconnected", (float)(6 * this._connectionAttempts));
	}

	public void OnDisconnected()
	{
		base.CancelInvoke("OnDisconnected");
		CoopClientCallbacks.OnDisconnected = null;
		if (this._connectionAttempts < 3)
		{
			UnityEngine.Debug.Log("Client connection attempt #" + this._connectionAttempts);
			this._connectionAttempts++;
			base.StartCoroutine(this.RetryConnectingRoutine());
		}
		else
		{
			CoopPlayerCallbacks.WasDisconnectedFromServer = true;
			BoltLauncher.Shutdown();
			Application.LoadLevel("TitleScene");
		}
	}

	[DebuggerHidden]
	private IEnumerator RetryConnectingRoutine()
	{
		CoopSteamClientStarter.<RetryConnectingRoutine>c__Iterator27 <RetryConnectingRoutine>c__Iterator = new CoopSteamClientStarter.<RetryConnectingRoutine>c__Iterator27();
		<RetryConnectingRoutine>c__Iterator.<>f__this = this;
		return <RetryConnectingRoutine>c__Iterator;
	}

	public override void BoltStartDone()
	{
		BoltNetwork.AddGlobalEventListener(CoopAckChecker.Instance);
		if (!CoopPeerStarter.Dedicated)
		{
			CoopSteamClient.Start();
		}
		if (!PlayerSpawn.HasMPCharacterSave())
		{
			TitleScreen.StartGameSetup.Type = TitleScreen.GameSetup.InitTypes.New;
		}
		base.BoltSetup();
		this.Connect();
		PlayerSpawn.LoadSavedCharacter = (TitleScreen.StartGameSetup.Type == TitleScreen.GameSetup.InitTypes.Continue);
	}

	private void InitBolt()
	{
		if (this._connectionAttempts < 3)
		{
			CoopClientCallbacks.OnDisconnected = new Action(this.OnDisconnected);
		}
		if (CoopPeerStarter.Dedicated)
		{
			BoltLauncher.SetUdpPlatform(new DotNetPlatform());
			BoltLauncher.StartClient(base.GetConfig());
		}
		else
		{
			BoltLauncher.StartClient(SteamUser.GetSteamID().ToEndPoint(), base.GetConfig());
		}
		CoopAckChecker.ACKED = false;
	}

	private void Connect()
	{
		if (CoopPeerStarter.Dedicated)
		{
			UnityEngine.Debug.LogFormat("connecting to: {0}", new object[]
			{
				this.dedicatedHostEndPoint
			});
			BoltNetwork.Connect(UdpEndPoint.Parse(this.dedicatedHostEndPoint), new CoopJoinDedicatedServerToken
			{
				AdminPassword = CoopDedicatedServerStarter.AdminPassword,
				ServerPassword = CoopDedicatedServerStarter.ServerPassword
			});
		}
		else
		{
			BoltNetwork.Connect(CoopLobby.Instance.Info.ServerId.ToEndPoint());
		}
	}

	public override void ConnectFailed(UdpEndPoint endpoint, IProtocolToken token)
	{
		if (this.gui)
		{
			this.gui.SetErrorText("Could not connect to server (attempt " + this._connectionAttempts + "/3)");
		}
		else
		{
			UnityEngine.Debug.LogError("could not connect to dedicated server: " + this.dedicatedHostEndPoint);
		}
	}

	public override void EntityAttached(BoltEntity entity)
	{
		if (entity.StateIs<ICoopServerInfo>())
		{
			entity.GetState<ICoopServerInfo>().AddCallback("PlanePosition", new PropertyCallbackSimple(this.PlanePositionArrived));
			entity.GetState<ICoopServerInfo>().AddCallback("PlaneRotation", new PropertyCallbackSimple(this.PlaneRotationArrived));
		}
	}

	private void PlanePositionArrived()
	{
		base.CancelInvoke("OnDisconnected");
		UnityEngine.Debug.Log("PLANE Pos");
		this._planePosArrived = true;
	}

	private void PlaneRotationArrived()
	{
		base.CancelInvoke("OnDisconnected");
		UnityEngine.Debug.Log("PLANE Rot");
		this._planeRotArrived = true;
	}
}
