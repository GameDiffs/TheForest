using System;
using UnityEngine;

[ExecuteInEditMode]
public class SunshinePostprocess : MonoBehaviour
{
	private SunshineCamera sunshineCamera;

	private void OnEnable()
	{
		this.sunshineCamera = base.GetComponent<SunshineCamera>();
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (this.sunshineCamera == null)
		{
			this.sunshineCamera = base.GetComponent<SunshineCamera>();
		}
		if (this.sunshineCamera != null && this.sunshineCamera.enabled)
		{
			this.sunshineCamera.OnPostProcess(source, destination);
		}
		else
		{
			Graphics.Blit(source, destination);
			base.enabled = false;
		}
	}

	public static void Blit(RenderTexture source, RenderTexture destination, Material material, int pass)
	{
		Graphics.Blit(source, destination, material, pass);
	}
}
