using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof(UITexture))]
public class DownloadTexture : MonoBehaviour
{
	public string url = "http://www.yourwebsite.com/logo.png";

	public bool pixelPerfect = true;

	private Texture2D mTex;

	[DebuggerHidden]
	private IEnumerator Start()
	{
		DownloadTexture.<Start>c__Iterator108 <Start>c__Iterator = new DownloadTexture.<Start>c__Iterator108();
		<Start>c__Iterator.<>f__this = this;
		return <Start>c__Iterator;
	}

	private void OnDestroy()
	{
		if (this.mTex != null)
		{
			UnityEngine.Object.Destroy(this.mTex);
		}
	}
}
