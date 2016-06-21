using System;
using UnityEngine;

public class FPSmeter : MonoBehaviour
{
	public float updateInterval = 0.5f;

	private float lastInterval;

	private int frames;

	public static float fps;

	public bool showFPS;

	private void Start()
	{
		this.lastInterval = Time.realtimeSinceStartup;
		this.frames = 0;
	}

	private void OnGUI()
	{
		if (this.showFPS)
		{
			GUI.Label(new Rect(10f, 10f, 100f, 20f), string.Empty + Mathf.Round(FPSmeter.fps * 100f) / 100f);
		}
	}

	private void Update()
	{
		this.frames++;
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if (realtimeSinceStartup > this.lastInterval + this.updateInterval)
		{
			FPSmeter.fps = (float)this.frames / (realtimeSinceStartup - this.lastInterval);
			this.frames = 0;
			this.lastInterval = realtimeSinceStartup;
		}
	}
}
