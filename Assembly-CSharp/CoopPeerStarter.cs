using Bolt;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class CoopPeerStarter : GlobalEventListener
{
	public enum MapState
	{
		None,
		Begin,
		Loading,
		Done,
		Playing
	}

	public static bool Dedicated;

	public static bool DedicatedHost;

	[HideInInspector]
	public CoopPeerStarter.MapState mapState;

	[HideInInspector]
	public CoopSteamNGUI gui;

	[HideInInspector]
	public LoadAsync _async;

	private void Awake()
	{
		if (this.gui)
		{
			UnityEngine.Object.DontDestroyOnLoad(this.gui);
		}
	}

	protected BoltConfig GetConfig()
	{
		BoltConfig configCopy = BoltRuntimeSettings.instance.GetConfigCopy();
		configCopy.connectionTimeout = 500000;
		configCopy.connectionRequestAttempts = ((!CoopPeerStarter.Dedicated) ? 60 : 15);
		configCopy.connectionRequestTimeout = 1000;
		return configCopy;
	}

	protected void BoltSetup()
	{
		BoltNetwork.SetCanReceiveEntities(false);
		BoltNetwork.RegisterTokenClass<CoopConnectToken>();
		BoltNetwork.RegisterTokenClass<CoopConstructionExToken>();
		BoltNetwork.RegisterTokenClass<CoopFloorToken>();
		BoltNetwork.RegisterTokenClass<CoopRoofToken>();
		BoltNetwork.RegisterTokenClass<CoopWallChunkToken>();
		BoltNetwork.RegisterTokenClass<CoopBridgeToken>();
		BoltNetwork.RegisterTokenClass<CoopFoundationChunkTierToken>();
		BoltNetwork.RegisterTokenClass<CoopSingleAnchorToken>();
		BoltNetwork.RegisterTokenClass<CoopMutantDummyToken>();
		BoltNetwork.RegisterTokenClass<CoopDestroyTagToken>();
		BoltNetwork.RegisterTokenClass<CoopKickToken>();
		BoltNetwork.RegisterTokenClass<CoopCreateBreakToken>();
		BoltNetwork.RegisterTokenClass<CoopJoinDedicatedServerToken>();
	}

	protected virtual void OnLoadingDone()
	{
	}

	[DebuggerHidden]
	private IEnumerator LoadingDone()
	{
		CoopPeerStarter.<LoadingDone>c__Iterator25 <LoadingDone>c__Iterator = new CoopPeerStarter.<LoadingDone>c__Iterator25();
		<LoadingDone>c__Iterator.<>f__this = this;
		return <LoadingDone>c__Iterator;
	}

	private LoadAsync GetAsync()
	{
		if (this._async)
		{
			return this._async;
		}
		if (this.gui && this.gui._async)
		{
			return this.gui._async;
		}
		return null;
	}

	protected void Update()
	{
		switch (this.mapState)
		{
		case CoopPeerStarter.MapState.Begin:
		{
			CoopPlayerCallbacks.ClearTrees();
			LoadAsync async = this.GetAsync();
			if (async)
			{
				async.gameObject.SetActive(true);
				this.mapState = CoopPeerStarter.MapState.Loading;
			}
			break;
		}
		case CoopPeerStarter.MapState.Loading:
			if (!this.GetAsync() || this.GetAsync().isDone)
			{
				this.mapState = CoopPeerStarter.MapState.Done;
			}
			break;
		case CoopPeerStarter.MapState.Done:
		{
			BoltNetwork.UpdateSceneObjectsLookup();
			Camera componentInChildren = base.GetComponentInChildren<Camera>();
			if (componentInChildren)
			{
				componentInChildren.enabled = false;
			}
			base.StartCoroutine(this.LoadingDone());
			this.mapState = CoopPeerStarter.MapState.Playing;
			break;
		}
		}
	}
}
