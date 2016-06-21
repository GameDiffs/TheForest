using System;
using UnityEngine;

public class SteamVR_CameraFlip : MonoBehaviour
{
	private static Material blitMaterial;

	private void OnEnable()
	{
		if (SteamVR_CameraFlip.blitMaterial == null)
		{
			SteamVR_CameraFlip.blitMaterial = new Material(Shader.Find("Custom/SteamVR_BlitFlip"));
		}
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		Graphics.Blit(src, dest, SteamVR_CameraFlip.blitMaterial);
	}
}
