using System;
using UnityEngine;

public class GetThemeColor : MonoBehaviour
{
	public bool realTime = true;

	public static byte COLOR_R = 255;

	public static byte COLOR_G = 255;

	public static byte COLOR_B = 255;

	private UISprite spr;

	private UISprite sspr;

	private void Start()
	{
		this.spr = base.gameObject.GetComponent<UISprite>();
		this.sspr = base.gameObject.GetComponent<UISprite>();
		if (this.spr != null)
		{
			this.spr.color = new Color32(GetThemeColor.COLOR_R, GetThemeColor.COLOR_G, GetThemeColor.COLOR_B, 255);
		}
		if (this.sspr != null)
		{
			this.sspr.color = new Color32(GetThemeColor.COLOR_R, GetThemeColor.COLOR_G, GetThemeColor.COLOR_B, 255);
		}
	}

	private void Update()
	{
		if (this.realTime)
		{
			if (this.spr != null)
			{
				this.spr.color = new Color32(GetThemeColor.COLOR_R, GetThemeColor.COLOR_G, GetThemeColor.COLOR_B, 255);
			}
			if (this.sspr != null)
			{
				this.sspr.color = new Color32(GetThemeColor.COLOR_R, GetThemeColor.COLOR_G, GetThemeColor.COLOR_B, 255);
			}
		}
	}
}
