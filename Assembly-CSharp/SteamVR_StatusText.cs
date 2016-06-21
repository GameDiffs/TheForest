using System;
using UnityEngine;

[RequireComponent(typeof(GUIText))]
public class SteamVR_StatusText : SteamVR_Status
{
	private GUIText text;

	private void Awake()
	{
		this.text = base.GetComponent<GUIText>();
		if (this.mode == SteamVR_Status.Mode.WhileTrue || this.mode == SteamVR_Status.Mode.WhileFalse)
		{
			this.timer = this.fade * this.text.color.a;
		}
	}

	protected override void SetAlpha(float a)
	{
		if (a > 0f)
		{
			this.text.enabled = true;
			this.text.color = new Color(this.text.color.r, this.text.color.g, this.text.color.b, a);
		}
		else
		{
			this.text.enabled = false;
		}
	}
}
