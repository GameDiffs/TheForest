using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class CaveOptimizer : MonoBehaviour
{
	private bool Loaded;

	public GameObject MyStreaming;

	[DebuggerHidden]
	private IEnumerator LoadIn()
	{
		CaveOptimizer.<LoadIn>c__Iterator153 <LoadIn>c__Iterator = new CaveOptimizer.<LoadIn>c__Iterator153();
		<LoadIn>c__Iterator.<>f__this = this;
		return <LoadIn>c__Iterator;
	}

	private void LoadOut()
	{
		if (this.MyStreaming != null)
		{
			UnityEngine.Object.Destroy(this.MyStreaming);
		}
	}

	private void Update()
	{
		if (Clock.InCave && !this.Loaded)
		{
			this.Loaded = true;
			base.StartCoroutine(this.LoadIn());
		}
		if (!Clock.InCave && this.Loaded && this.MyStreaming != null)
		{
			this.Loaded = false;
			UnityEngine.Object.Destroy(this.MyStreaming);
			this.MyStreaming = null;
		}
	}
}
