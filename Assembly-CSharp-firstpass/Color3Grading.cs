using System;
using UnityEngine;

[AddComponentMenu("Image Effects/Color3 Grading"), ExecuteInEditMode]
public sealed class Color3Grading : MonoBehaviour
{
	public delegate void OnFinishBlend();

	private ColorSpace colorSpace = ColorSpace.Uninitialized;

	private Shader shaderGrading;

	private Shader shaderBlend;

	private Shader shaderMask;

	private Shader shaderBlendMask;

	public float BlendAmount;

	public Texture2D LutTexture;

	public Texture2D LutBlendTexture;

	public Texture2D MaskTexture;

	private bool use3d;

	private Texture lutTexture3d;

	private Texture lutBlendTexture3d;

	private Material materialGrading;

	private Material materialBlend;

	private Material materialMask;

	private Material materialBlendMask;

	private bool blending;

	private float blendingTime;

	private float blendingTimeCountdown;

	private Color3Grading.OnFinishBlend onFinishBlend;

	internal bool JustCopy;

	public bool IsBlending
	{
		get
		{
			return this.blending;
		}
	}

	public bool WillItBlend
	{
		get
		{
			return this.LutTexture != null && this.LutBlendTexture != null && !this.blending;
		}
	}

	private void ReportMissingShaders()
	{
		Debug.LogError("[Color3] Error initializing shaders. Please reinstall Color3.");
		base.enabled = false;
	}

	private void ReportNotSupported()
	{
		Debug.LogError("[Color3] This image effect is not supported on this platform. Please make sure your Unity license supports Full-Screen Post-Processing Effects which is usually reserved forn Pro licenses.");
		base.enabled = false;
	}

	private bool CheckShader(Shader s)
	{
		if (s == null)
		{
			this.ReportMissingShaders();
			return false;
		}
		if (!s.isSupported)
		{
			this.ReportNotSupported();
			return false;
		}
		return true;
	}

	private bool CheckShaders()
	{
		return this.CheckShader(this.shaderGrading) && this.CheckShader(this.shaderBlend) && this.CheckShader(this.shaderMask) && this.CheckShader(this.shaderBlendMask);
	}

	private bool CheckSupport()
	{
		if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures)
		{
			this.ReportNotSupported();
			return false;
		}
		return true;
	}

	private void OnEnable()
	{
		if (!this.CheckSupport())
		{
			return;
		}
		this.CreateMaterials(false);
		if ((this.LutTexture != null && this.LutTexture.mipmapCount > 1) || (this.LutBlendTexture != null && this.LutBlendTexture.mipmapCount > 1))
		{
			Debug.LogError("[Color3] Please disable \"Generate Mip Maps\" import settings on all LUT textures to avoid visual glitches. Change Texture Type to \"Advanced\" to access Mip settings.");
		}
	}

	private void OnDisable()
	{
		this.ReleaseShaders();
		this.ReleaseTextures();
	}

	public void BlendTo(Texture2D blendTargetLUT, float blendTimeInSec, Color3Grading.OnFinishBlend onFinishBlend)
	{
		this.LutBlendTexture = blendTargetLUT;
		this.BlendAmount = 0f;
		this.onFinishBlend = onFinishBlend;
		this.blendingTime = blendTimeInSec;
		this.blendingTimeCountdown = blendTimeInSec;
		this.blending = true;
	}

	private void Update()
	{
		if (this.blending)
		{
			this.BlendAmount = (this.blendingTime - this.blendingTimeCountdown) / this.blendingTime;
			this.blendingTimeCountdown -= Time.smoothDeltaTime;
			if (this.BlendAmount >= 1f)
			{
				this.LutTexture = this.LutBlendTexture;
				this.BlendAmount = 0f;
				this.blending = false;
				this.LutBlendTexture = null;
				if (this.onFinishBlend != null)
				{
					this.onFinishBlend();
				}
			}
		}
		else
		{
			this.BlendAmount = Mathf.Clamp01(this.BlendAmount);
		}
	}

	private void SetupShader(bool fallback)
	{
		this.colorSpace = QualitySettings.activeColorSpace;
		string str = (this.colorSpace != ColorSpace.Linear) ? string.Empty : "Linear";
		string empty = string.Empty;
		this.shaderGrading = Shader.Find("Hidden/Color3" + str + empty);
		this.shaderBlend = Shader.Find("Hidden/Color3Blend" + str + empty);
		this.shaderMask = Shader.Find("Hidden/Color3Mask" + str + empty);
		this.shaderBlendMask = Shader.Find("Hidden/Color3BlendMask" + str + empty);
	}

	private void ReleaseShaders()
	{
		if (this.materialGrading != null)
		{
			UnityEngine.Object.DestroyImmediate(this.materialGrading);
			this.materialGrading = null;
		}
		if (this.materialBlend != null)
		{
			UnityEngine.Object.DestroyImmediate(this.materialBlend);
			this.materialBlend = null;
		}
		if (this.materialMask != null)
		{
			UnityEngine.Object.DestroyImmediate(this.materialMask);
			this.materialMask = null;
		}
		if (this.materialBlendMask != null)
		{
			UnityEngine.Object.DestroyImmediate(this.materialBlendMask);
			this.materialBlendMask = null;
		}
	}

	private void CreateMaterials(bool fallback)
	{
		this.SetupShader(fallback);
		if (!this.CheckShaders())
		{
			return;
		}
		this.ReleaseShaders();
		this.materialGrading = new Material(this.shaderGrading)
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		this.materialBlend = new Material(this.shaderBlend)
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		this.materialMask = new Material(this.shaderMask)
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		this.materialBlendMask = new Material(this.shaderBlendMask)
		{
			hideFlags = HideFlags.HideAndDontSave
		};
	}

	private void ReleaseTextures()
	{
	}

	public static bool ValidateLutDimensions(Texture2D lut)
	{
		bool result = true;
		if (lut != null)
		{
			if (lut.width / lut.height != lut.height)
			{
				Debug.LogWarning("[Color3] Lut " + lut.name + " has invalid dimensions.");
				result = false;
			}
			else if (lut.anisoLevel != 0)
			{
				lut.anisoLevel = 0;
			}
		}
		return result;
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		this.BlendAmount = Mathf.Clamp01(this.BlendAmount);
		if (this.colorSpace != QualitySettings.activeColorSpace)
		{
			this.CreateMaterials(false);
		}
		bool flag = Color3Grading.ValidateLutDimensions(this.LutTexture);
		bool flag2 = Color3Grading.ValidateLutDimensions(this.LutBlendTexture);
		if (this.JustCopy || !flag || !flag2)
		{
			Graphics.Blit(source, destination);
			return;
		}
		if (this.LutTexture == null && this.lutTexture3d == null)
		{
			Graphics.Blit(source, destination);
		}
		else
		{
			Material material;
			if (this.LutBlendTexture != null || this.lutBlendTexture3d != null)
			{
				if (this.MaskTexture != null)
				{
					material = this.materialBlendMask;
				}
				else
				{
					material = this.materialBlend;
				}
			}
			else if (this.MaskTexture != null)
			{
				material = this.materialMask;
			}
			else
			{
				material = this.materialGrading;
			}
			int pass = (base.GetComponent<Camera>().hdr || this.BlendAmount != 0f) ? 1 : 0;
			material.SetFloat("_lerpAmount", this.BlendAmount);
			if (this.MaskTexture != null)
			{
				material.SetTexture("_MaskTex", this.MaskTexture);
			}
			if (!this.use3d)
			{
				if (this.LutTexture != null)
				{
					material.SetTexture("_RgbTex", this.LutTexture);
				}
				if (this.LutBlendTexture != null)
				{
					material.SetTexture("_LerpRgbTex", this.LutBlendTexture);
				}
			}
			Graphics.Blit(source, destination, material, pass);
		}
	}
}
