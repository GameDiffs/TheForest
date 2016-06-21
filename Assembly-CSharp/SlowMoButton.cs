using System;
using UnityEngine;

public class SlowMoButton : MonoBehaviour
{
	public Texture FullSpeedTex;

	public Texture SlowSpeedTex;

	public float fullSpeed = 1f;

	public float slowSpeed = 0.3f;

	public GUITexture guiTexture;

	public bool alsoScalePhysicsTimestep = true;

	private bool slowMo;

	private float targetTime;

	private float lastRealTime;

	private float fixedTimeRatio;

	private void Start()
	{
		this.targetTime = this.fullSpeed;
		this.lastRealTime = Time.realtimeSinceStartup;
		this.fixedTimeRatio = Time.fixedDeltaTime / Time.timeScale;
	}

	private void Update()
	{
		float num = Time.realtimeSinceStartup - this.lastRealTime;
		if (CrossPlatformInput.GetButtonDown("Speed"))
		{
			this.slowMo = !this.slowMo;
			this.guiTexture.texture = ((!this.slowMo) ? this.FullSpeedTex : this.SlowSpeedTex);
			this.targetTime = ((!this.slowMo) ? this.fullSpeed : this.slowSpeed);
		}
		if (Time.timeScale != this.targetTime)
		{
			Time.timeScale = Mathf.Lerp(Time.timeScale, this.targetTime, num * 2f);
			if (this.alsoScalePhysicsTimestep)
			{
				Time.fixedDeltaTime = this.fixedTimeRatio * Time.timeScale;
			}
			if (Mathf.Abs(Time.timeScale - this.targetTime) < 0.01f)
			{
				Time.timeScale = this.targetTime;
			}
		}
		this.lastRealTime = Time.realtimeSinceStartup;
	}
}
