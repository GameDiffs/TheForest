using System;
using UnityEngine;

internal class CoopFPS : MonoBehaviour
{
	private int frames;

	private float dt;

	private float fps;

	private void Update()
	{
		this.frames++;
		this.dt += Time.deltaTime;
		if (this.dt > 1f)
		{
			this.fps = (float)this.frames;
			this.dt -= 1f;
			this.frames = 0;
		}
	}

	private void OnGUI()
	{
		GUI.Label(new Rect(0f, 0f, 100f, 100f), "FPS: " + this.fps);
	}
}
