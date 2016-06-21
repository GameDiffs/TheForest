using System;
using TheForest.Utils;
using UnityEngine;

public class TimmyStats : MonoBehaviour
{
	private float Fullness;

	private HudGui Hud;

	private void Start()
	{
		this.Hud = Scene.HudGui;
		base.InvokeRepeating("Life", 1f, 1f);
	}

	private void Life()
	{
		this.Fullness -= 0.0015f;
	}

	public void Fed()
	{
		this.Fullness += 1f;
	}

	private void Awake()
	{
	}

	private void GrabEnter()
	{
		this.Hud.TimmyStomach.enabled = true;
	}

	private void GrabExit()
	{
		this.Hud.TimmyStomach.enabled = false;
	}

	private void Update()
	{
		this.Hud.TimmyStomach.fillAmount = this.Fullness;
		if ((double)this.Fullness < 0.5)
		{
		}
		if (this.Fullness < 0.19f)
		{
			this.Fullness = 0.19f;
		}
		if (this.Fullness > 1f)
		{
			this.Fullness = 1f;
		}
	}
}
