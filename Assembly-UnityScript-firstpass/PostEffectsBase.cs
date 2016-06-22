using System;
using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(Camera))]
[Serializable]
public class PostEffectsBase : MonoBehaviour
{
	protected bool supportHDRTextures;

	protected bool supportDX11;

	protected bool isSupported;

	public PostEffectsBase()
	{
		this.supportHDRTextures = true;
		this.isSupported = true;
	}

	public override Material CheckShaderAndCreateMaterial(Shader s, Material m2Create)
	{
		Material arg_D0_0;
		if (!s)
		{
			Debug.Log("Missing shader in " + this.ToString());
			this.enabled = false;
			arg_D0_0 = null;
		}
		else if (s.isSupported && m2Create && m2Create.shader == s)
		{
			arg_D0_0 = m2Create;
		}
		else if (!s.isSupported)
		{
			this.NotSupported();
			Debug.Log("The shader " + s.ToString() + " on effect " + this.ToString() + " is not supported on this platform!");
			arg_D0_0 = null;
		}
		else
		{
			m2Create = new Material(s);
			m2Create.hideFlags = HideFlags.DontSave;
			arg_D0_0 = ((!m2Create) ? null : m2Create);
		}
		return arg_D0_0;
	}

	public override Material CreateMaterial(Shader s, Material m2Create)
	{
		Material arg_8F_0;
		if (!s)
		{
			Debug.Log("Missing shader in " + this.ToString());
			arg_8F_0 = null;
		}
		else if (m2Create && m2Create.shader == s && s.isSupported)
		{
			arg_8F_0 = m2Create;
		}
		else if (!s.isSupported)
		{
			arg_8F_0 = null;
		}
		else
		{
			m2Create = new Material(s);
			m2Create.hideFlags = HideFlags.DontSave;
			arg_8F_0 = ((!m2Create) ? null : m2Create);
		}
		return arg_8F_0;
	}

	public override void OnEnable()
	{
		this.isSupported = true;
	}

	public override bool CheckSupport()
	{
		return this.CheckSupport(false);
	}

	public override bool CheckResources()
	{
		Debug.LogWarning("CheckResources () for " + this.ToString() + " should be overwritten.");
		return this.isSupported;
	}

	public override void Start()
	{
		this.CheckResources();
	}

	public override bool CheckSupport(bool needDepth)
	{
		this.isSupported = true;
		this.supportHDRTextures = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf);
		bool arg_2C_1;
		if (arg_2C_1 = (SystemInfo.graphicsShaderLevel >= 50))
		{
			arg_2C_1 = SystemInfo.supportsComputeShaders;
		}
		this.supportDX11 = arg_2C_1;
		bool arg_8D_0;
		if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures)
		{
			this.NotSupported();
			arg_8D_0 = false;
		}
		else if (needDepth && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
		{
			this.NotSupported();
			arg_8D_0 = false;
		}
		else
		{
			if (needDepth)
			{
				this.GetComponent<Camera>().depthTextureMode = (this.GetComponent<Camera>().depthTextureMode | DepthTextureMode.Depth);
			}
			arg_8D_0 = true;
		}
		return arg_8D_0;
	}

	public override bool CheckSupport(bool needDepth, bool needHdr)
	{
		bool arg_30_0;
		if (!this.CheckSupport(needDepth))
		{
			arg_30_0 = false;
		}
		else if (needHdr && !this.supportHDRTextures)
		{
			this.NotSupported();
			arg_30_0 = false;
		}
		else
		{
			arg_30_0 = true;
		}
		return arg_30_0;
	}

	public override bool Dx11Support()
	{
		return this.supportDX11;
	}

	public override void ReportAutoDisable()
	{
		Debug.LogWarning("The image effect " + this.ToString() + " has been disabled as it's not supported on the current platform.");
	}

	public override bool CheckShader(Shader s)
	{
		Debug.Log("The shader " + s.ToString() + " on effect " + this.ToString() + " is not part of the Unity 3.2+ effects suite anymore. For best performance and quality, please ensure you are using the latest Standard Assets Image Effects (Pro only) package.");
		bool arg_51_0;
		if (!s.isSupported)
		{
			this.NotSupported();
			arg_51_0 = false;
		}
		else
		{
			arg_51_0 = false;
		}
		return arg_51_0;
	}

	public override void NotSupported()
	{
		this.enabled = false;
		this.isSupported = false;
	}

	public override void DrawBorder(RenderTexture dest, Material material)
	{
		float x = 0f;
		float x2 = 0f;
		float y = 0f;
		float y2 = 0f;
		RenderTexture.active = dest;
		bool flag = true;
		GL.PushMatrix();
		GL.LoadOrtho();
		for (int i = 0; i < material.passCount; i++)
		{
			material.SetPass(i);
			float y3 = 0f;
			float y4 = 0f;
			if (flag)
			{
				y3 = 1f;
				y4 = (float)0;
			}
			else
			{
				y3 = (float)0;
				y4 = 1f;
			}
			x = (float)0;
			x2 = (float)0 + 1f / ((float)dest.width * 1f);
			y = (float)0;
			y2 = 1f;
			GL.Begin(7);
			GL.TexCoord2((float)0, y3);
			GL.Vertex3(x, y, 0.1f);
			GL.TexCoord2(1f, y3);
			GL.Vertex3(x2, y, 0.1f);
			GL.TexCoord2(1f, y4);
			GL.Vertex3(x2, y2, 0.1f);
			GL.TexCoord2((float)0, y4);
			GL.Vertex3(x, y2, 0.1f);
			x = 1f - 1f / ((float)dest.width * 1f);
			x2 = 1f;
			y = (float)0;
			y2 = 1f;
			GL.TexCoord2((float)0, y3);
			GL.Vertex3(x, y, 0.1f);
			GL.TexCoord2(1f, y3);
			GL.Vertex3(x2, y, 0.1f);
			GL.TexCoord2(1f, y4);
			GL.Vertex3(x2, y2, 0.1f);
			GL.TexCoord2((float)0, y4);
			GL.Vertex3(x, y2, 0.1f);
			x = (float)0;
			x2 = 1f;
			y = (float)0;
			y2 = (float)0 + 1f / ((float)dest.height * 1f);
			GL.TexCoord2((float)0, y3);
			GL.Vertex3(x, y, 0.1f);
			GL.TexCoord2(1f, y3);
			GL.Vertex3(x2, y, 0.1f);
			GL.TexCoord2(1f, y4);
			GL.Vertex3(x2, y2, 0.1f);
			GL.TexCoord2((float)0, y4);
			GL.Vertex3(x, y2, 0.1f);
			x = (float)0;
			x2 = 1f;
			y = 1f - 1f / ((float)dest.height * 1f);
			y2 = 1f;
			GL.TexCoord2((float)0, y3);
			GL.Vertex3(x, y, 0.1f);
			GL.TexCoord2(1f, y3);
			GL.Vertex3(x2, y, 0.1f);
			GL.TexCoord2(1f, y4);
			GL.Vertex3(x2, y2, 0.1f);
			GL.TexCoord2((float)0, y4);
			GL.Vertex3(x, y2, 0.1f);
			GL.End();
		}
		GL.PopMatrix();
	}

	public override void Main()
	{
	}
}
