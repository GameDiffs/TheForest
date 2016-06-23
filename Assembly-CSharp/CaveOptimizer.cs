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
		CaveOptimizer.<LoadIn>c__Iterator15C <LoadIn>c__Iterator15C = new CaveOptimizer.<LoadIn>c__Iterator15C();
		<LoadIn>c__Iterator15C.<>f__this = this;
		return <LoadIn>c__Iterator15C;
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
