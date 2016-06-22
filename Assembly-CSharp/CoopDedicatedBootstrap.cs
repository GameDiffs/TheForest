using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class CoopDedicatedBootstrap : MonoBehaviour
{
	private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
	{
		Console.WriteLine(string.Concat(new object[]
		{
			type,
			": ",
			condition,
			"\r\n",
			stackTrace
		}));
	}

	[DebuggerHidden]
	private IEnumerator Start()
	{
		CoopDedicatedBootstrap.<Start>c__Iterator2B <Start>c__Iterator2B = new CoopDedicatedBootstrap.<Start>c__Iterator2B();
		<Start>c__Iterator2B.<>f__this = this;
		return <Start>c__Iterator2B;
	}
}
