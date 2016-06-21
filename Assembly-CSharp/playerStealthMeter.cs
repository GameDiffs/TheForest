using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Utils;
using UnityEngine;

public class playerStealthMeter : MonoBehaviour
{
	private playerScriptSetup setup;

	private visRangeSetup vis;

	public float stealthValue = 100f;

	public float treeDensity = 1f;

	public UITexture tex;

	[DebuggerHidden]
	private IEnumerator Start()
	{
		playerStealthMeter.<Start>c__IteratorE1 <Start>c__IteratorE = new playerStealthMeter.<Start>c__IteratorE1();
		<Start>c__IteratorE.<>f__this = this;
		return <Start>c__IteratorE;
	}

	private void Update()
	{
		if (!LocalPlayer.Animator)
		{
			return;
		}
		if (LocalPlayer.Animator.GetFloat("crouch") > 5f && !this.vis.currentlyTargetted && PlayerPreferences.ShowStealthMeter)
		{
			Scene.HudGui.EyeIcon.SetActive(true);
		}
		else
		{
			Scene.HudGui.EyeIcon.SetActive(false);
		}
		if (Scene.HudGui.EyeIcon.activeSelf)
		{
			float num = (70f - this.vis.modVisRange) / 100f;
			num = Mathf.Clamp(num, 0.1f, 1f);
			if (this.vis.currentlyTargetted)
			{
				Scene.HudGui.EyeIconLine.transform.localScale = Vector3.Lerp(Scene.HudGui.EyeIconLine.transform.localScale, Vector3.zero, Time.deltaTime * 3f);
			}
			else
			{
				Scene.HudGui.EyeIconLine.transform.localScale = Vector3.Lerp(Scene.HudGui.EyeIconLine.transform.localScale, new Vector3(num * 1.6f, 0.85f, 1f), Time.deltaTime * 2f);
			}
		}
	}
}
