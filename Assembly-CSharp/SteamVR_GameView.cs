using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SteamVR_GameView : MonoBehaviour
{
	public float scale = 1.5f;

	public bool drawOverlay = true;

	private static Material overlayMaterial;

	private void OnEnable()
	{
		if (SteamVR_GameView.overlayMaterial == null)
		{
			SteamVR_GameView.overlayMaterial = new Material(Shader.Find("Custom/SteamVR_Overlay"));
		}
	}

	private void OnPostRender()
	{
		SteamVR instance = SteamVR.instance;
		Camera component = base.GetComponent<Camera>();
		float num = this.scale * component.aspect / instance.aspect;
		float x = -this.scale;
		float x2 = this.scale;
		float y = num;
		float y2 = -num;
		Material blitMaterial = SteamVR_Camera.blitMaterial;
		blitMaterial.mainTexture = SteamVR_Camera.GetSceneTexture(component.hdr);
		GL.PushMatrix();
		GL.LoadOrtho();
		blitMaterial.SetPass(0);
		GL.Begin(7);
		GL.TexCoord2(0f, 0f);
		GL.Vertex3(x, y, 0f);
		GL.TexCoord2(1f, 0f);
		GL.Vertex3(x2, y, 0f);
		GL.TexCoord2(1f, 1f);
		GL.Vertex3(x2, y2, 0f);
		GL.TexCoord2(0f, 1f);
		GL.Vertex3(x, y2, 0f);
		GL.End();
		GL.PopMatrix();
		SteamVR_Overlay instance2 = SteamVR_Overlay.instance;
		if (instance2 && instance2.texture && SteamVR_GameView.overlayMaterial && this.drawOverlay)
		{
			Texture texture = instance2.texture;
			SteamVR_GameView.overlayMaterial.mainTexture = texture;
			float x3 = 0f;
			float y3 = 1f - (float)Screen.height / (float)texture.height;
			float x4 = (float)Screen.width / (float)texture.width;
			float y4 = 1f;
			GL.PushMatrix();
			GL.LoadOrtho();
			SteamVR_GameView.overlayMaterial.SetPass((QualitySettings.activeColorSpace != ColorSpace.Linear) ? 0 : 1);
			GL.Begin(7);
			GL.TexCoord2(x3, y3);
			GL.Vertex3(-1f, -1f, 0f);
			GL.TexCoord2(x4, y3);
			GL.Vertex3(1f, -1f, 0f);
			GL.TexCoord2(x4, y4);
			GL.Vertex3(1f, 1f, 0f);
			GL.TexCoord2(x3, y4);
			GL.Vertex3(-1f, 1f, 0f);
			GL.End();
			GL.PopMatrix();
		}
	}
}
