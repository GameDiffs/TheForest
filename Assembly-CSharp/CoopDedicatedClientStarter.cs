using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class CoopDedicatedClientStarter : MonoBehaviour
{
	internal static string Password;

	internal static string EndPoint;

	private CoopSteamClientStarter clientStarter;

	public LoadAsync loadAsync;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(this.loadAsync.gameObject);
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	[DebuggerHidden]
	private IEnumerator Start()
	{
		CoopDedicatedClientStarter.<Start>c__Iterator2B <Start>c__Iterator2B = new CoopDedicatedClientStarter.<Start>c__Iterator2B();
		<Start>c__Iterator2B.<>f__this = this;
		return <Start>c__Iterator2B;
	}

	private void Connect()
	{
		this.clientStarter = base.gameObject.AddComponent<CoopSteamClientStarter>();
		this.clientStarter.dedicatedHostEndPoint = CoopDedicatedClientStarter.EndPoint;
		this.clientStarter._async = this.loadAsync;
	}
}
