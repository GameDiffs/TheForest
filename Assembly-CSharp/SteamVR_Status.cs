using System;
using UnityEngine;

public abstract class SteamVR_Status : MonoBehaviour
{
	public enum Mode
	{
		OnTrue,
		OnFalse,
		WhileTrue,
		WhileFalse
	}

	public string message;

	public float duration;

	public float fade;

	protected float timer;

	protected bool status;

	public SteamVR_Status.Mode mode;

	protected abstract void SetAlpha(float a);

	private void OnEnable()
	{
		SteamVR_Utils.Event.Listen(this.message, new SteamVR_Utils.Event.Handler(this.OnEvent));
	}

	private void OnDisable()
	{
		SteamVR_Utils.Event.Remove(this.message, new SteamVR_Utils.Event.Handler(this.OnEvent));
	}

	private void OnEvent(params object[] args)
	{
		this.status = (bool)args[0];
		if (this.status)
		{
			if (this.mode == SteamVR_Status.Mode.OnTrue)
			{
				this.timer = this.duration;
			}
		}
		else if (this.mode == SteamVR_Status.Mode.OnFalse)
		{
			this.timer = this.duration;
		}
	}

	private void Update()
	{
		if (this.mode == SteamVR_Status.Mode.OnTrue || this.mode == SteamVR_Status.Mode.OnFalse)
		{
			this.timer -= Time.deltaTime;
			if (this.timer < 0f)
			{
				this.SetAlpha(0f);
			}
			else
			{
				float alpha = 1f;
				if (this.timer < this.fade)
				{
					alpha = this.timer / this.fade;
				}
				if (this.timer > this.duration - this.fade)
				{
					alpha = Mathf.InverseLerp(this.duration, this.duration - this.fade, this.timer);
				}
				this.SetAlpha(alpha);
			}
		}
		else
		{
			bool flag = (this.mode == SteamVR_Status.Mode.WhileTrue && this.status) || (this.mode == SteamVR_Status.Mode.WhileFalse && !this.status);
			this.timer = ((!flag) ? Mathf.Max(0f, this.timer - Time.deltaTime) : Mathf.Min(this.fade, this.timer + Time.deltaTime));
			this.SetAlpha(this.timer / this.fade);
		}
	}
}
