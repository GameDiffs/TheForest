using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ceto.Common.Unity.Utility
{
	public static class RTUtility
	{
		public static void Blit(RenderTexture des, Material mat, int pass = 0)
		{
			Graphics.SetRenderTarget(des);
			GL.PushMatrix();
			GL.LoadOrtho();
			mat.SetPass(pass);
			GL.Begin(7);
			GL.TexCoord2(0f, 0f);
			GL.Vertex3(0f, 0f, 0.1f);
			GL.TexCoord2(1f, 0f);
			GL.Vertex3(1f, 0f, 0.1f);
			GL.TexCoord2(1f, 1f);
			GL.Vertex3(1f, 1f, 0.1f);
			GL.TexCoord2(0f, 1f);
			GL.Vertex3(0f, 1f, 0.1f);
			GL.End();
			GL.PopMatrix();
		}

		public static void Blit(RenderTexture des, Material mat, Vector3[] verts, int pass = 0)
		{
			Graphics.SetRenderTarget(des);
			GL.PushMatrix();
			GL.LoadOrtho();
			mat.SetPass(pass);
			GL.Begin(7);
			GL.TexCoord2(0f, 0f);
			GL.Vertex(verts[0]);
			GL.TexCoord2(1f, 0f);
			GL.Vertex(verts[1]);
			GL.TexCoord2(1f, 1f);
			GL.Vertex(verts[2]);
			GL.TexCoord2(0f, 1f);
			GL.Vertex(verts[3]);
			GL.End();
			GL.PopMatrix();
		}

		public static void Blit(RenderTexture des, Material mat, Vector3[] verts, Vector2[] uvs, int pass = 0)
		{
			Graphics.SetRenderTarget(des);
			GL.PushMatrix();
			GL.LoadOrtho();
			mat.SetPass(pass);
			GL.Begin(7);
			GL.TexCoord(uvs[0]);
			GL.Vertex(verts[0]);
			GL.TexCoord(uvs[1]);
			GL.Vertex(verts[1]);
			GL.TexCoord(uvs[2]);
			GL.Vertex(verts[2]);
			GL.TexCoord(uvs[3]);
			GL.Vertex(verts[3]);
			GL.End();
			GL.PopMatrix();
		}

		public static void MultiTargetBlit(IList<RenderTexture> des, Material mat, int pass = 0)
		{
			RenderBuffer[] array = new RenderBuffer[des.Count];
			for (int i = 0; i < des.Count; i++)
			{
				array[i] = des[i].colorBuffer;
			}
			Graphics.SetRenderTarget(array, des[0].depthBuffer);
			GL.PushMatrix();
			GL.LoadOrtho();
			mat.SetPass(pass);
			GL.Begin(7);
			GL.TexCoord2(0f, 0f);
			GL.Vertex3(0f, 0f, 0.1f);
			GL.TexCoord2(1f, 0f);
			GL.Vertex3(1f, 0f, 0.1f);
			GL.TexCoord2(1f, 1f);
			GL.Vertex3(1f, 1f, 0.1f);
			GL.TexCoord2(0f, 1f);
			GL.Vertex3(0f, 1f, 0.1f);
			GL.End();
			GL.PopMatrix();
		}

		public static void MultiTargetBlit(RenderBuffer[] des_rb, RenderBuffer des_db, Material mat, int pass = 0)
		{
			Graphics.SetRenderTarget(des_rb, des_db);
			GL.PushMatrix();
			GL.LoadOrtho();
			mat.SetPass(pass);
			GL.Begin(7);
			GL.TexCoord2(0f, 0f);
			GL.Vertex3(0f, 0f, 0.1f);
			GL.TexCoord2(1f, 0f);
			GL.Vertex3(1f, 0f, 0.1f);
			GL.TexCoord2(1f, 1f);
			GL.Vertex3(1f, 1f, 0.1f);
			GL.TexCoord2(0f, 1f);
			GL.Vertex3(0f, 1f, 0.1f);
			GL.End();
			GL.PopMatrix();
		}

		public static void ClearColor(RenderTexture tex, Color col)
		{
			if (tex == null)
			{
				return;
			}
			if (!SystemInfo.SupportsRenderTextureFormat(tex.format))
			{
				return;
			}
			Graphics.SetRenderTarget(tex);
			GL.Clear(false, true, col);
		}

		public static void Release(RenderTexture tex)
		{
			if (tex == null)
			{
				return;
			}
			tex.Release();
		}

		public static void Release(IList<RenderTexture> texList)
		{
			if (texList == null)
			{
				return;
			}
			int count = texList.Count;
			for (int i = 0; i < count; i++)
			{
				if (!(texList[i] == null))
				{
					texList[i].Release();
				}
			}
		}

		public static void ReleaseAndDestroy(RenderTexture tex)
		{
			if (tex == null)
			{
				return;
			}
			tex.Release();
			UnityEngine.Object.Destroy(tex);
		}

		public static void ReleaseAndDestroy(IList<RenderTexture> texList)
		{
			if (texList == null)
			{
				return;
			}
			int count = texList.Count;
			for (int i = 0; i < count; i++)
			{
				if (!(texList[i] == null))
				{
					texList[i].Release();
					UnityEngine.Object.Destroy(texList[i]);
				}
			}
		}

		public static void ReleaseTemporary(RenderTexture tex)
		{
			if (tex == null)
			{
				return;
			}
			RenderTexture.ReleaseTemporary(tex);
		}

		public static void ReleaseTemporary(IList<RenderTexture> texList)
		{
			if (texList == null)
			{
				return;
			}
			int count = texList.Count;
			for (int i = 0; i < count; i++)
			{
				if (!(texList[i] == null))
				{
					RenderTexture.ReleaseTemporary(texList[i]);
				}
			}
		}

		private static RenderTextureFormat CheckFormat(RTSettings setting)
		{
			RenderTextureFormat renderTextureFormat = setting.format;
			if (!SystemInfo.SupportsRenderTextureFormat(renderTextureFormat))
			{
				Debug.Log("System does not support " + renderTextureFormat + " render texture format.");
				bool flag = false;
				int count = setting.fallbackFormats.Count;
				for (int i = 0; i < count; i++)
				{
					if (SystemInfo.SupportsRenderTextureFormat(setting.fallbackFormats[i]))
					{
						renderTextureFormat = setting.fallbackFormats[i];
						Debug.Log("Found fallback format: " + renderTextureFormat);
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					throw new InvalidOperationException("Could not find fallback render texture format");
				}
			}
			return renderTextureFormat;
		}

		public static RenderTexture CreateRenderTexture(RTSettings setting)
		{
			if (setting == null)
			{
				throw new NullReferenceException("RTSettings is null");
			}
			if (!SystemInfo.supportsRenderTextures)
			{
				throw new InvalidOperationException("This system does not support render textures");
			}
			RenderTextureFormat format = RTUtility.CheckFormat(setting);
			return new RenderTexture(setting.width, setting.height, setting.depth, format, setting.readWrite)
			{
				name = setting.name,
				wrapMode = setting.wrap,
				filterMode = setting.filer,
				useMipMap = setting.mipmaps,
				anisoLevel = setting.ansioLevel,
				enableRandomWrite = setting.randomWrite
			};
		}

		public static RenderTexture CreateTemporyRenderTexture(RTSettings setting)
		{
			if (setting == null)
			{
				throw new NullReferenceException("RTSettings is null");
			}
			if (!SystemInfo.supportsRenderTextures)
			{
				throw new InvalidOperationException("This system does not support render textures");
			}
			RenderTextureFormat format = RTUtility.CheckFormat(setting);
			RenderTexture temporary = RenderTexture.GetTemporary(setting.width, setting.height, setting.depth, format, setting.readWrite);
			temporary.name = setting.name;
			temporary.wrapMode = setting.wrap;
			temporary.filterMode = setting.filer;
			temporary.anisoLevel = setting.ansioLevel;
			return temporary;
		}
	}
}
