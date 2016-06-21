using System;
using UnityEngine;
using Valve.VR;

public class SteamVR_Fade : MonoBehaviour
{
	private Color currentColor = new Color(0f, 0f, 0f, 0f);

	private Color targetColor = new Color(0f, 0f, 0f, 0f);

	private Color deltaColor = new Color(0f, 0f, 0f, 0f);

	private bool fadeOverlay;

	private static Material fadeMaterial;

	public static void Start(Color newColor, float duration, bool fadeOverlay = false)
	{
		SteamVR_Utils.Event.Send("fade", new object[]
		{
			newColor,
			duration,
			fadeOverlay
		});
	}

	public static void View(Color newColor, float duration)
	{
		CVRCompositor compositor = OpenVR.Compositor;
		if (compositor != null)
		{
			compositor.FadeToColor(duration, newColor.r, newColor.g, newColor.b, newColor.a, false);
		}
	}

	public void OnStartFade(params object[] args)
	{
		Color color = (Color)args[0];
		float num = (float)args[1];
		this.fadeOverlay = (args.Length > 2 && (bool)args[2]);
		if (num > 0f)
		{
			this.targetColor = color;
			this.deltaColor = (this.targetColor - this.currentColor) / num;
		}
		else
		{
			this.currentColor = color;
		}
	}

	private void OnEnable()
	{
		if (SteamVR_Fade.fadeMaterial == null)
		{
			SteamVR_Fade.fadeMaterial = new Material(Shader.Find("Custom/SteamVR_Fade"));
		}
		SteamVR_Utils.Event.Listen("fade", new SteamVR_Utils.Event.Handler(this.OnStartFade));
		SteamVR_Utils.Event.Send("fade_ready", new object[0]);
	}

	private void OnDisable()
	{
		SteamVR_Utils.Event.Remove("fade", new SteamVR_Utils.Event.Handler(this.OnStartFade));
	}

	private void OnPostRender()
	{
		if (this.currentColor != this.targetColor)
		{
			if (Mathf.Abs(this.currentColor.a - this.targetColor.a) < Mathf.Abs(this.deltaColor.a) * Time.deltaTime)
			{
				this.currentColor = this.targetColor;
				this.deltaColor = new Color(0f, 0f, 0f, 0f);
			}
			else
			{
				this.currentColor += this.deltaColor * Time.deltaTime;
			}
			if (this.fadeOverlay)
			{
				SteamVR_Overlay instance = SteamVR_Overlay.instance;
				if (instance != null)
				{
					instance.alpha = 1f - this.currentColor.a;
				}
			}
		}
		if (this.currentColor.a > 0f && SteamVR_Fade.fadeMaterial)
		{
			GL.PushMatrix();
			GL.LoadOrtho();
			SteamVR_Fade.fadeMaterial.SetPass(0);
			GL.Begin(7);
			GL.Color(this.currentColor);
			GL.Vertex3(0f, 0f, 0f);
			GL.Vertex3(1f, 0f, 0f);
			GL.Vertex3(1f, 1f, 0f);
			GL.Vertex3(0f, 1f, 0f);
			GL.End();
			GL.PopMatrix();
		}
	}
}
