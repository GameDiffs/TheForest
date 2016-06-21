using DigitalOpus.MB.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class MB_TextureCombinerRenderTexture
{
	public MB2_LogLevel LOG_LEVEL = MB2_LogLevel.info;

	private Material mat;

	private RenderTexture _destinationTexture;

	private Camera myCamera;

	private int _padding;

	private bool _isNormalMap;

	private bool _fixOutOfBoundsUVs;

	private bool _doRenderAtlas;

	private Rect[] rs;

	private List<MB3_TextureCombiner.MB_TexSet> textureSets;

	private int indexOfTexSetToRender;

	private Texture2D targTex;

	private MB3_TextureCombiner combiner;

	public Texture2D DoRenderAtlas(GameObject gameObject, int width, int height, int padding, Rect[] rss, List<MB3_TextureCombiner.MB_TexSet> textureSetss, int indexOfTexSetToRenders, bool isNormalMap, bool fixOutOfBoundsUVs, MB3_TextureCombiner texCombiner, MB2_LogLevel LOG_LEV)
	{
		this.LOG_LEVEL = LOG_LEV;
		this.textureSets = textureSetss;
		this.indexOfTexSetToRender = indexOfTexSetToRenders;
		this._padding = padding;
		this._isNormalMap = isNormalMap;
		this._fixOutOfBoundsUVs = fixOutOfBoundsUVs;
		this.combiner = texCombiner;
		this.rs = rss;
		Shader shader;
		if (this._isNormalMap)
		{
			shader = Shader.Find("MeshBaker/NormalMapShader");
		}
		else
		{
			shader = Shader.Find("MeshBaker/AlbedoShader");
		}
		if (shader == null)
		{
			UnityEngine.Debug.LogError("Could not find shader for RenderTexture. Try reimporting mesh baker");
			return null;
		}
		this.mat = new Material(shader);
		this._destinationTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
		this._destinationTexture.filterMode = FilterMode.Point;
		this.myCamera = gameObject.GetComponent<Camera>();
		this.myCamera.orthographic = true;
		this.myCamera.orthographicSize = (float)(height >> 1);
		this.myCamera.aspect = (float)(width / height);
		this.myCamera.targetTexture = this._destinationTexture;
		this.myCamera.clearFlags = CameraClearFlags.Color;
		Transform component = this.myCamera.GetComponent<Transform>();
		component.localPosition = new Vector3((float)width / 2f, (float)height / 2f, 3f);
		component.localRotation = Quaternion.Euler(0f, 180f, 180f);
		this._doRenderAtlas = true;
		if (this.LOG_LEVEL >= MB2_LogLevel.debug)
		{
			UnityEngine.Debug.Log(string.Format("Begin Camera.Render destTex w={0} h={1} camPos={2}", width, height, component.localPosition));
		}
		this.myCamera.Render();
		this._doRenderAtlas = false;
		MB_Utility.Destroy(this.mat);
		MB_Utility.Destroy(this._destinationTexture);
		if (this.LOG_LEVEL >= MB2_LogLevel.debug)
		{
			UnityEngine.Debug.Log("Finished Camera.Render ");
		}
		Texture2D result = this.targTex;
		this.targTex = null;
		return result;
	}

	public void OnRenderObject()
	{
		if (this._doRenderAtlas)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			for (int i = 0; i < this.rs.Length; i++)
			{
				MB3_TextureCombiner.MeshBakerMaterialTexture meshBakerMaterialTexture = this.textureSets[i].ts[this.indexOfTexSetToRender];
				if (this.LOG_LEVEL >= MB2_LogLevel.trace && meshBakerMaterialTexture.t != null)
				{
					UnityEngine.Debug.Log(string.Concat(new object[]
					{
						"Added ",
						meshBakerMaterialTexture.t,
						" to atlas w=",
						meshBakerMaterialTexture.t.width,
						" h=",
						meshBakerMaterialTexture.t.height,
						" offset=",
						meshBakerMaterialTexture.offset,
						" scale=",
						meshBakerMaterialTexture.scale,
						" rect=",
						this.rs[i],
						" padding=",
						this._padding
					}));
					this._printTexture(meshBakerMaterialTexture.t);
				}
				this.CopyScaledAndTiledToAtlas(meshBakerMaterialTexture, this.rs[i]);
			}
			stopwatch.Stop();
			stopwatch.Start();
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				UnityEngine.Debug.Log("Total time for Graphics.DrawTexture calls " + stopwatch.ElapsedMilliseconds.ToString("f5"));
			}
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				UnityEngine.Debug.Log("Copying RenderTexture to Texture2D.");
			}
			Texture2D texture2D = new Texture2D(this._destinationTexture.width, this._destinationTexture.height, TextureFormat.ARGB32, true);
			int num = this._destinationTexture.width / 512;
			int num2 = this._destinationTexture.height / 512;
			if (num == 0 || num2 == 0)
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.trace)
				{
					UnityEngine.Debug.Log("Copying all in one shot");
				}
				RenderTexture.active = this._destinationTexture;
				texture2D.ReadPixels(new Rect(0f, 0f, (float)this._destinationTexture.width, (float)this._destinationTexture.height), 0, 0, true);
				RenderTexture.active = null;
			}
			else if (this.IsOpenGL())
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.trace)
				{
					UnityEngine.Debug.Log("OpenGL copying blocks");
				}
				for (int j = 0; j < num; j++)
				{
					for (int k = 0; k < num2; k++)
					{
						RenderTexture.active = this._destinationTexture;
						texture2D.ReadPixels(new Rect((float)(j * 512), (float)(k * 512), 512f, 512f), j * 512, k * 512, true);
						RenderTexture.active = null;
					}
				}
			}
			else
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.trace)
				{
					UnityEngine.Debug.Log("Not OpenGL copying blocks");
				}
				for (int l = 0; l < num; l++)
				{
					for (int m = 0; m < num2; m++)
					{
						RenderTexture.active = this._destinationTexture;
						texture2D.ReadPixels(new Rect((float)(l * 512), (float)(this._destinationTexture.height - 512 - m * 512), 512f, 512f), l * 512, m * 512, true);
						RenderTexture.active = null;
					}
				}
			}
			texture2D.Apply();
			if (this.LOG_LEVEL >= MB2_LogLevel.trace)
			{
				this._printTexture(texture2D);
			}
			this.myCamera.targetTexture = null;
			RenderTexture.active = null;
			this.targTex = texture2D;
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				UnityEngine.Debug.Log("Total time to copy RenderTexture to Texture2D " + stopwatch.ElapsedMilliseconds.ToString("f5"));
			}
		}
	}

	private Color32 ConvertNormalFormatFromUnity_ToStandard(Color32 c)
	{
		Vector3 zero = Vector3.zero;
		zero.x = (float)c.a * 2f - 1f;
		zero.y = (float)c.g * 2f - 1f;
		zero.z = Mathf.Sqrt(1f - zero.x * zero.x - zero.y * zero.y);
		return new Color32
		{
			a = 1,
			r = (byte)((zero.x + 1f) * 0.5f),
			g = (byte)((zero.y + 1f) * 0.5f),
			b = (byte)((zero.z + 1f) * 0.5f)
		};
	}

	private bool IsOpenGL()
	{
		string graphicsDeviceVersion = SystemInfo.graphicsDeviceVersion;
		return graphicsDeviceVersion.StartsWith("OpenGL");
	}

	private void CopyScaledAndTiledToAtlas(MB3_TextureCombiner.MeshBakerMaterialTexture source, Rect rec)
	{
		Rect rect = rec;
		this.myCamera.backgroundColor = source.colorIfNoTexture;
		if (source.t == null)
		{
			source.t = this.combiner._createTemporaryTexture(16, 16, TextureFormat.ARGB32, true);
			UnityEngine.Debug.Log(string.Concat(new object[]
			{
				"Creating texture with color ",
				source.colorIfNoTexture,
				" isNormal",
				this._isNormalMap
			}));
			MB_Utility.setSolidColor(source.t, source.colorIfNoTexture);
		}
		rect.y = 1f - (rect.y + rect.height);
		rect.x *= (float)this._destinationTexture.width;
		rect.y *= (float)this._destinationTexture.height;
		rect.width *= (float)this._destinationTexture.width;
		rect.height *= (float)this._destinationTexture.height;
		Rect rect2 = rect;
		rect2.x -= (float)this._padding;
		rect2.y -= (float)this._padding;
		rect2.width += (float)(this._padding * 2);
		rect2.height += (float)(this._padding * 2);
		Rect rect3 = default(Rect);
		Rect screenRect = default(Rect);
		rect3.width = source.scale.x;
		rect3.height = source.scale.y;
		rect3.x = source.offset.x;
		rect3.y = source.offset.y;
		if (this._fixOutOfBoundsUVs)
		{
			rect3.width *= source.obUVscale.x;
			rect3.height *= source.obUVscale.y;
			rect3.x += source.obUVoffset.x;
			rect3.y += source.obUVoffset.y;
			if (this.LOG_LEVEL >= MB2_LogLevel.trace)
			{
				UnityEngine.Debug.Log("Fixing out of bounds UVs for tex " + source.t);
			}
		}
		Texture t = source.t;
		TextureWrapMode wrapMode = t.wrapMode;
		if (rect3.width == 1f && rect3.height == 1f && rect3.x == 0f && rect3.y == 0f)
		{
			t.wrapMode = TextureWrapMode.Clamp;
		}
		else
		{
			t.wrapMode = TextureWrapMode.Repeat;
		}
		if (this.LOG_LEVEL >= MB2_LogLevel.trace)
		{
			UnityEngine.Debug.Log(string.Concat(new object[]
			{
				"DrawTexture tex=",
				t.name,
				" destRect=",
				rect,
				" srcRect=",
				rect3,
				" Mat=",
				this.mat
			}));
		}
		Rect sourceRect = default(Rect);
		sourceRect.x = rect3.x;
		sourceRect.y = rect3.y + 1f - 1f / (float)t.height;
		sourceRect.width = rect3.width;
		sourceRect.height = 1f / (float)t.height;
		screenRect.x = rect.x;
		screenRect.y = rect2.y;
		screenRect.width = rect.width;
		screenRect.height = (float)this._padding;
		Graphics.DrawTexture(screenRect, t, sourceRect, 0, 0, 0, 0, this.mat);
		sourceRect.x = rect3.x;
		sourceRect.y = rect3.y;
		sourceRect.width = rect3.width;
		sourceRect.height = 1f / (float)t.height;
		screenRect.x = rect.x;
		screenRect.y = rect.y + rect.height;
		screenRect.width = rect.width;
		screenRect.height = (float)this._padding;
		Graphics.DrawTexture(screenRect, t, sourceRect, 0, 0, 0, 0, this.mat);
		sourceRect.x = rect3.x;
		sourceRect.y = rect3.y;
		sourceRect.width = 1f / (float)t.width;
		sourceRect.height = rect3.height;
		screenRect.x = rect2.x;
		screenRect.y = rect.y;
		screenRect.width = (float)this._padding;
		screenRect.height = rect.height;
		Graphics.DrawTexture(screenRect, t, sourceRect, 0, 0, 0, 0, this.mat);
		sourceRect.x = rect3.x + 1f - 1f / (float)t.width;
		sourceRect.y = rect3.y;
		sourceRect.width = 1f / (float)t.width;
		sourceRect.height = rect3.height;
		screenRect.x = rect.x + rect.width;
		screenRect.y = rect.y;
		screenRect.width = (float)this._padding;
		screenRect.height = rect.height;
		Graphics.DrawTexture(screenRect, t, sourceRect, 0, 0, 0, 0, this.mat);
		sourceRect.x = rect3.x;
		sourceRect.y = rect3.y + 1f - 1f / (float)t.height;
		sourceRect.width = 1f / (float)t.width;
		sourceRect.height = 1f / (float)t.height;
		screenRect.x = rect2.x;
		screenRect.y = rect2.y;
		screenRect.width = (float)this._padding;
		screenRect.height = (float)this._padding;
		Graphics.DrawTexture(screenRect, t, sourceRect, 0, 0, 0, 0, this.mat);
		sourceRect.x = rect3.x + 1f - 1f / (float)t.width;
		sourceRect.y = rect3.y + 1f - 1f / (float)t.height;
		sourceRect.width = 1f / (float)t.width;
		sourceRect.height = 1f / (float)t.height;
		screenRect.x = rect.x + rect.width;
		screenRect.y = rect2.y;
		screenRect.width = (float)this._padding;
		screenRect.height = (float)this._padding;
		Graphics.DrawTexture(screenRect, t, sourceRect, 0, 0, 0, 0, this.mat);
		sourceRect.x = rect3.x;
		sourceRect.y = rect3.y;
		sourceRect.width = 1f / (float)t.width;
		sourceRect.height = 1f / (float)t.height;
		screenRect.x = rect2.x;
		screenRect.y = rect.y + rect.height;
		screenRect.width = (float)this._padding;
		screenRect.height = (float)this._padding;
		Graphics.DrawTexture(screenRect, t, sourceRect, 0, 0, 0, 0, this.mat);
		sourceRect.x = rect3.x + 1f - 1f / (float)t.width;
		sourceRect.y = rect3.y;
		sourceRect.width = 1f / (float)t.width;
		sourceRect.height = 1f / (float)t.height;
		screenRect.x = rect.x + rect.width;
		screenRect.y = rect.y + rect.height;
		screenRect.width = (float)this._padding;
		screenRect.height = (float)this._padding;
		Graphics.DrawTexture(screenRect, t, sourceRect, 0, 0, 0, 0, this.mat);
		Graphics.DrawTexture(rect, t, rect3, 0, 0, 0, 0, this.mat);
		t.wrapMode = wrapMode;
	}

	private void _printTexture(Texture2D t)
	{
		if (t.width * t.height > 100)
		{
			UnityEngine.Debug.Log("Not printing texture too large.");
		}
		Color32[] pixels = t.GetPixels32();
		string text = string.Empty;
		for (int i = 0; i < t.height; i++)
		{
			for (int j = 0; j < t.width; j++)
			{
				text = text + pixels[i * t.width + j] + ", ";
			}
			text += "\n";
		}
		UnityEngine.Debug.Log(text);
	}
}
