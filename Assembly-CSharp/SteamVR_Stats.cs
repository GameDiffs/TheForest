using System;
using System.Runtime.InteropServices;
using UnityEngine;
using Valve.VR;

public class SteamVR_Stats : MonoBehaviour
{
	public GUIText text;

	public Color fadeColor = Color.black;

	public float fadeDuration = 1f;

	private double lastUpdate;

	private void Awake()
	{
		if (this.text == null)
		{
			this.text = base.GetComponent<GUIText>();
			this.text.enabled = false;
		}
		if (this.fadeDuration > 0f)
		{
			SteamVR_Fade.Start(this.fadeColor, 0f, false);
			SteamVR_Fade.Start(Color.clear, this.fadeDuration, false);
		}
	}

	private void Update()
	{
		if (this.text != null)
		{
			if (Input.GetKeyDown(KeyCode.I))
			{
				this.text.enabled = !this.text.enabled;
			}
			if (this.text.enabled)
			{
				CVRCompositor compositor = OpenVR.Compositor;
				if (compositor != null)
				{
					Compositor_FrameTiming compositor_FrameTiming = default(Compositor_FrameTiming);
					compositor_FrameTiming.m_nSize = (uint)Marshal.SizeOf(typeof(Compositor_FrameTiming));
					compositor.GetFrameTiming(ref compositor_FrameTiming, 0u);
					double flSystemTimeInSeconds = compositor_FrameTiming.m_flSystemTimeInSeconds;
					if (flSystemTimeInSeconds > this.lastUpdate)
					{
						double num = (this.lastUpdate <= 0.0) ? 0.0 : (1.0 / (flSystemTimeInSeconds - this.lastUpdate));
						this.lastUpdate = flSystemTimeInSeconds;
						this.text.text = string.Format("framerate: {0:N0}\ndropped frames: {1}", num, (int)compositor_FrameTiming.m_nNumDroppedFrames);
					}
					else
					{
						this.lastUpdate = flSystemTimeInSeconds;
					}
				}
			}
		}
	}
}
