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
		DownloadTexture.<Start>c__Iterator10B <Start>c__Iterator10B = new DownloadTexture.<Start>c__Iterator10B();
		<Start>c__Iterator10B.<>f__this = this;
		return <Start>c__Iterator10B;
	}

	private void OnDestroy()
	{
		if (this.mTex != null)
		{
			UnityEngine.Object.Destroy(this.mTex);
		}
	}
}
