using AmplifyColor;
using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("")]
public class AmplifyColorBase : MonoBehaviour
{
	public const int LutSize = 32;

	public const int LutWidth = 1024;

	public const int LutHeight = 32;

	public Quality QualityLevel = Quality.Standard;

	public float BlendAmount;

	public Texture LutTexture;

	public Texture LutBlendTexture;

	public Texture MaskTexture;

	public bool UseVolumes;

	public float ExitVolumeBlendTime = 1f;

	public Transform TriggerVolumeProxy;

	public LayerMask VolumeCollisionMask = -1;

	private Shader shaderBase;

	private Shader shaderBlend;

	private Shader shaderBlendCache;

	private Shader shaderMask;

	private Shader shaderBlendMask;

	private RenderTexture blendCacheLut;

	private Texture2D defaultLut;

	private ColorSpace colorSpace = ColorSpace.Uninitialized;

	private Quality qualityLevel = Quality.Standard;

	private Material materialBase;

	private Material materialBlend;

	private Material materialBlendCache;

	private Material materialMask;

	private Material materialBlendMask;

	private bool blending;

	private float blendingTime;

	private float blendingTimeCountdown;

	private Action onFinishBlend;

	private bool volumesBlending;

	private float volumesBlendingTime;

	private float volumesBlendingTimeCountdown;

	private Texture volumesLutBlendTexture;

	private float volumesBlendAmount;

	private Texture worldLUT;

	private AmplifyColorVolumeBase currentVolumeLut;

	private RenderTexture midBlendLUT;

	private bool blendingFromMidBlend;

	private VolumeEffect worldVolumeEffects;

	private VolumeEffect currentVolumeEffects;

	private VolumeEffect blendVolumeEffects;

	private float effectVolumesBlendAdjust;

	private List<AmplifyColorVolumeBase> enteredVolumes = new List<AmplifyColorVolumeBase>();

	private AmplifyColorTriggerProxy actualTriggerProxy;

	[HideInInspector]
	public VolumeEffectFlags EffectFlags = new VolumeEffectFlags();

	public Texture2D DefaultLut
	{
		get
		{
			return (!(this.defaultLut == null)) ? this.defaultLut : this.CreateDefaultLut();
		}
	}

	public bool IsBlending
	{
		get
		{
			return this.blending;
		}
	}

	private float effectVolumesBlendAdjusted
	{
		get
		{
			return Mathf.Clamp01((this.effectVolumesBlendAdjust >= 0.99f) ? 1f : ((this.volumesBlendAmount - this.effectVolumesBlendAdjust) / (1f - this.effectVolumesBlendAdjust)));
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
		Debug.LogError("[AmplifyColor] Failed to initialize shaders. Please attempt to re-enable the Amplify Color Effect component. If that fails, please reinstall Amplify Color.");
		base.enabled = false;
	}

	private void ReportNotSupported()
	{
		Debug.LogError("[AmplifyColor] This image effect is not supported on this platform. Please make sure your Unity license supports Full-Screen Post-Processing Effects which is usually reserved forn Pro licenses.");
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
		return this.CheckShader(this.shaderBase) && this.CheckShader(this.shaderBlend) && this.CheckShader(this.shaderBlendCache) && this.CheckShader(this.shaderMask) && this.CheckShader(this.shaderBlendMask);
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
		if (!this.CreateMaterials())
		{
			return;
		}
		Texture2D texture2D = this.LutTexture as Texture2D;
		Texture2D texture2D2 = this.LutBlendTexture as Texture2D;
		if ((texture2D != null && texture2D.mipmapCount > 1) || (texture2D2 != null && texture2D2.mipmapCount > 1))
		{
			Debug.LogError("[AmplifyColor] Please disable \"Generate Mip Maps\" import settings on all LUT textures to avoid visual glitches. Change Texture Type to \"Advanced\" to access Mip settings.");
		}
	}

	private void OnDisable()
	{
		if (this.actualTriggerProxy != null)
		{
			UnityEngine.Object.DestroyImmediate(this.actualTriggerProxy.gameObject);
			this.actualTriggerProxy = null;
		}
		this.ReleaseMaterials();
		this.ReleaseTextures();
	}

	private void VolumesBlendTo(Texture blendTargetLUT, float blendTimeInSec)
	{
		this.volumesLutBlendTexture = blendTargetLUT;
		this.volumesBlendAmount = 0f;
		this.volumesBlendingTime = blendTimeInSec;
		this.volumesBlendingTimeCountdown = blendTimeInSec;
		this.volumesBlending = true;
	}

	public void BlendTo(Texture blendTargetLUT, float blendTimeInSec, Action onFinishBlend)
	{
		this.LutBlendTexture = blendTargetLUT;
		this.BlendAmount = 0f;
		this.onFinishBlend = onFinishBlend;
		this.blendingTime = blendTimeInSec;
		this.blendingTimeCountdown = blendTimeInSec;
		this.blending = true;
	}

	private void Start()
	{
		this.worldLUT = this.LutTexture;
		this.worldVolumeEffects = this.EffectFlags.GenerateEffectData(this);
		this.blendVolumeEffects = (this.currentVolumeEffects = this.worldVolumeEffects);
	}

	private void Update()
	{
		if (this.volumesBlending)
		{
			this.volumesBlendAmount = (this.volumesBlendingTime - this.volumesBlendingTimeCountdown) / this.volumesBlendingTime;
			this.volumesBlendingTimeCountdown -= Time.smoothDeltaTime;
			if (this.volumesBlendAmount >= 1f)
			{
				this.LutTexture = this.volumesLutBlendTexture;
				this.volumesBlendAmount = 0f;
				this.volumesBlending = false;
				this.volumesLutBlendTexture = null;
				this.effectVolumesBlendAdjust = 0f;
				this.currentVolumeEffects = this.blendVolumeEffects;
				this.currentVolumeEffects.SetValues(this);
				if (this.blendingFromMidBlend && this.midBlendLUT != null)
				{
					this.midBlendLUT.DiscardContents();
				}
				this.blendingFromMidBlend = false;
			}
		}
		else
		{
			this.volumesBlendAmount = Mathf.Clamp01(this.volumesBlendAmount);
		}
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
		if (this.UseVolumes)
		{
			if (this.actualTriggerProxy == null)
			{
				GameObject gameObject = new GameObject(base.name + "+ACVolumeProxy")
				{
					hideFlags = HideFlags.HideAndDontSave
				};
				this.actualTriggerProxy = gameObject.AddComponent<AmplifyColorTriggerProxy>();
				this.actualTriggerProxy.OwnerEffect = this;
			}
			this.UpdateVolumes();
		}
		else if (this.actualTriggerProxy != null)
		{
			UnityEngine.Object.DestroyImmediate(this.actualTriggerProxy.gameObject);
			this.actualTriggerProxy = null;
		}
	}

	public void EnterVolume(AmplifyColorVolumeBase volume)
	{
		if (!this.enteredVolumes.Contains(volume))
		{
			this.enteredVolumes.Insert(0, volume);
		}
	}

	public void ExitVolume(AmplifyColorVolumeBase volume)
	{
		if (this.enteredVolumes.Contains(volume))
		{
			this.enteredVolumes.Remove(volume);
		}
	}

	private void UpdateVolumes()
	{
		if (this.volumesBlending)
		{
			this.currentVolumeEffects.BlendValues(this, this.blendVolumeEffects, this.effectVolumesBlendAdjusted);
		}
		Transform transform = (!(this.TriggerVolumeProxy == null)) ? this.TriggerVolumeProxy : base.transform;
		if (this.actualTriggerProxy.transform.parent != transform)
		{
			this.actualTriggerProxy.Reference = transform;
			this.actualTriggerProxy.gameObject.layer = transform.gameObject.layer;
		}
		AmplifyColorVolumeBase amplifyColorVolumeBase = null;
		int num = -2147483648;
		foreach (AmplifyColorVolumeBase current in this.enteredVolumes)
		{
			if (current.Priority > num)
			{
				amplifyColorVolumeBase = current;
				num = current.Priority;
			}
		}
		if (amplifyColorVolumeBase != this.currentVolumeLut)
		{
			this.currentVolumeLut = amplifyColorVolumeBase;
			Texture texture = (!(amplifyColorVolumeBase == null)) ? amplifyColorVolumeBase.LutTexture : this.worldLUT;
			float num2 = (!(amplifyColorVolumeBase == null)) ? amplifyColorVolumeBase.EnterBlendTime : this.ExitVolumeBlendTime;
			if (this.volumesBlending && !this.blendingFromMidBlend && texture == this.LutTexture)
			{
				this.LutTexture = this.volumesLutBlendTexture;
				this.volumesLutBlendTexture = texture;
				this.volumesBlendingTimeCountdown = num2 * ((this.volumesBlendingTime - this.volumesBlendingTimeCountdown) / this.volumesBlendingTime);
				this.volumesBlendingTime = num2;
				this.currentVolumeEffects = VolumeEffect.BlendValuesToVolumeEffect(this.EffectFlags, this.currentVolumeEffects, this.blendVolumeEffects, this.effectVolumesBlendAdjusted);
				this.effectVolumesBlendAdjust = 1f - this.volumesBlendAmount;
				this.volumesBlendAmount = 1f - this.volumesBlendAmount;
			}
			else
			{
				if (this.volumesBlending)
				{
					this.materialBlendCache.SetFloat("_lerpAmount", this.volumesBlendAmount);
					if (this.blendingFromMidBlend)
					{
						Graphics.Blit(this.midBlendLUT, this.blendCacheLut);
						this.materialBlendCache.SetTexture("_RgbTex", this.blendCacheLut);
					}
					else
					{
						this.materialBlendCache.SetTexture("_RgbTex", this.LutTexture);
					}
					this.materialBlendCache.SetTexture("_LerpRgbTex", (!(this.volumesLutBlendTexture != null)) ? this.defaultLut : this.volumesLutBlendTexture);
					Graphics.Blit(this.midBlendLUT, this.midBlendLUT, this.materialBlendCache);
					this.blendCacheLut.DiscardContents();
					this.currentVolumeEffects = VolumeEffect.BlendValuesToVolumeEffect(this.EffectFlags, this.currentVolumeEffects, this.blendVolumeEffects, this.effectVolumesBlendAdjusted);
					this.effectVolumesBlendAdjust = 0f;
					this.blendingFromMidBlend = true;
				}
				this.VolumesBlendTo(texture, num2);
			}
			this.blendVolumeEffects = ((!(amplifyColorVolumeBase == null)) ? amplifyColorVolumeBase.EffectContainer.GetVolumeEffect(this) : this.worldVolumeEffects);
			if (this.blendVolumeEffects == null)
			{
				this.blendVolumeEffects = this.worldVolumeEffects;
			}
		}
	}

	private void SetupShader()
	{
		this.colorSpace = QualitySettings.activeColorSpace;
		this.qualityLevel = this.QualityLevel;
		string str = (this.colorSpace != ColorSpace.Linear) ? string.Empty : "Linear";
		this.shaderBase = Shader.Find("Hidden/Amplify Color/Base" + str);
		this.shaderBlend = Shader.Find("Hidden/Amplify Color/Blend" + str);
		this.shaderBlendCache = Shader.Find("Hidden/Amplify Color/BlendCache");
		this.shaderMask = Shader.Find("Hidden/Amplify Color/Mask" + str);
		this.shaderBlendMask = Shader.Find("Hidden/Amplify Color/BlendMask" + str);
	}

	private void ReleaseMaterials()
	{
		if (this.materialBase != null)
		{
			UnityEngine.Object.DestroyImmediate(this.materialBase);
			this.materialBase = null;
		}
		if (this.materialBlend != null)
		{
			UnityEngine.Object.DestroyImmediate(this.materialBlend);
			this.materialBlend = null;
		}
		if (this.materialBlendCache != null)
		{
			UnityEngine.Object.DestroyImmediate(this.materialBlendCache);
			this.materialBlendCache = null;
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

	private Texture2D CreateDefaultLut()
	{
		this.defaultLut = new Texture2D(1024, 32, TextureFormat.RGB24, false, true)
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		this.defaultLut.name = "DefaultLut";
		this.defaultLut.hideFlags = HideFlags.DontSave;
		this.defaultLut.anisoLevel = 1;
		this.defaultLut.filterMode = FilterMode.Bilinear;
		Color32[] array = new Color32[32768];
		for (int i = 0; i < 32; i++)
		{
			int num = i * 32;
			for (int j = 0; j < 32; j++)
			{
				int num2 = num + j * 1024;
				for (int k = 0; k < 32; k++)
				{
					float num3 = (float)k / 31f;
					float num4 = (float)j / 31f;
					float num5 = (float)i / 31f;
					byte r = (byte)(num3 * 255f);
					byte g = (byte)(num4 * 255f);
					byte b = (byte)(num5 * 255f);
					array[num2 + k] = new Color32(r, g, b, 255);
				}
			}
		}
		this.defaultLut.SetPixels32(array);
		this.defaultLut.Apply();
		return this.defaultLut;
	}

	private void CreateHelperTextures()
	{
		this.ReleaseTextures();
		this.blendCacheLut = new RenderTexture(1024, 32, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear)
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		this.blendCacheLut.name = "BlendCacheLut";
		this.blendCacheLut.wrapMode = TextureWrapMode.Clamp;
		this.blendCacheLut.useMipMap = false;
		this.blendCacheLut.anisoLevel = 0;
		this.blendCacheLut.Create();
		this.midBlendLUT = new RenderTexture(1024, 32, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear)
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		this.midBlendLUT.name = "MidBlendLut";
		this.midBlendLUT.wrapMode = TextureWrapMode.Clamp;
		this.midBlendLUT.useMipMap = false;
		this.midBlendLUT.anisoLevel = 0;
		this.midBlendLUT.Create();
		this.CreateDefaultLut();
	}

	private bool CheckMaterialAndShader(Material material, string name)
	{
		if (material == null || material.shader == null)
		{
			Debug.LogError("[AmplifyColor] Error creating " + name + " material. Effect disabled.");
			base.enabled = false;
		}
		else if (!material.shader.isSupported)
		{
			Debug.LogError("[AmplifyColor] " + name + " shader not supported on this platform. Effect disabled.");
			base.enabled = false;
		}
		else
		{
			material.hideFlags = HideFlags.HideAndDontSave;
		}
		return base.enabled;
	}

	private void SwitchToMobile(Material mat)
	{
	}

	private void SwitchToStandard(Material mat)
	{
	}

	private bool CreateMaterials()
	{
		this.SetupShader();
		if (!this.CheckShaders())
		{
			return false;
		}
		this.ReleaseMaterials();
		this.materialBase = new Material(this.shaderBase);
		this.materialBlend = new Material(this.shaderBlend);
		this.materialBlendCache = new Material(this.shaderBlendCache);
		this.materialMask = new Material(this.shaderMask);
		this.materialBlendMask = new Material(this.shaderBlendMask);
		this.CheckMaterialAndShader(this.materialBase, "BaseMaterial");
		this.CheckMaterialAndShader(this.materialBlend, "BlendMaterial");
		this.CheckMaterialAndShader(this.materialBlendCache, "BlendCacheMaterial");
		this.CheckMaterialAndShader(this.materialMask, "MaskMaterial");
		this.CheckMaterialAndShader(this.materialBlendMask, "BlendMaskMaterial");
		if (!base.enabled)
		{
			return false;
		}
		if (this.QualityLevel == Quality.Mobile)
		{
			this.SwitchToMobile(this.materialBase);
			this.SwitchToMobile(this.materialBlend);
			this.SwitchToMobile(this.materialBlendCache);
			this.SwitchToMobile(this.materialMask);
			this.SwitchToMobile(this.materialBlendMask);
		}
		else
		{
			this.SwitchToStandard(this.materialBase);
			this.SwitchToStandard(this.materialBlend);
			this.SwitchToStandard(this.materialBlendCache);
			this.SwitchToStandard(this.materialMask);
			this.SwitchToStandard(this.materialBlendMask);
		}
		this.CreateHelperTextures();
		return true;
	}

	private void ReleaseTextures()
	{
		if (this.blendCacheLut != null)
		{
			UnityEngine.Object.DestroyImmediate(this.blendCacheLut);
			this.blendCacheLut = null;
		}
		if (this.midBlendLUT != null)
		{
			UnityEngine.Object.DestroyImmediate(this.midBlendLUT);
			this.midBlendLUT = null;
		}
		if (this.defaultLut != null)
		{
			UnityEngine.Object.DestroyImmediate(this.defaultLut);
			this.defaultLut = null;
		}
	}

	public static bool ValidateLutDimensions(Texture lut)
	{
		bool result = true;
		if (lut != null)
		{
			if (lut.width / lut.height != lut.height)
			{
				Debug.LogWarning("[AmplifyColor] Lut " + lut.name + " has invalid dimensions.");
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
		if (this.colorSpace != QualitySettings.activeColorSpace || this.qualityLevel != this.QualityLevel)
		{
			this.CreateMaterials();
		}
		bool flag = AmplifyColorBase.ValidateLutDimensions(this.LutTexture);
		bool flag2 = AmplifyColorBase.ValidateLutDimensions(this.LutBlendTexture);
		bool flag3 = this.LutTexture == null && this.LutBlendTexture == null && this.volumesLutBlendTexture == null;
		if (!flag || !flag2 || flag3)
		{
			Graphics.Blit(source, destination);
			return;
		}
		Texture texture = (!(this.LutTexture == null)) ? this.LutTexture : this.defaultLut;
		Texture lutBlendTexture = this.LutBlendTexture;
		int pass = base.GetComponent<Camera>().hdr ? 1 : 0;
		bool flag4 = this.BlendAmount != 0f || this.blending;
		bool flag5 = flag4 || (flag4 && lutBlendTexture != null);
		bool flag6 = flag5;
		Material material;
		if (flag5 || this.volumesBlending)
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
			material = this.materialBase;
		}
		material.SetFloat("_lerpAmount", this.BlendAmount);
		if (this.MaskTexture != null)
		{
			material.SetTexture("_MaskTex", this.MaskTexture);
		}
		if (this.volumesBlending)
		{
			this.volumesBlendAmount = Mathf.Clamp01(this.volumesBlendAmount);
			this.materialBlendCache.SetFloat("_lerpAmount", this.volumesBlendAmount);
			if (this.blendingFromMidBlend)
			{
				this.materialBlendCache.SetTexture("_RgbTex", this.midBlendLUT);
			}
			else
			{
				this.materialBlendCache.SetTexture("_RgbTex", texture);
			}
			this.materialBlendCache.SetTexture("_LerpRgbTex", (!(this.volumesLutBlendTexture != null)) ? this.defaultLut : this.volumesLutBlendTexture);
			Graphics.Blit(texture, this.blendCacheLut, this.materialBlendCache);
		}
		if (flag6)
		{
			this.materialBlendCache.SetFloat("_lerpAmount", this.BlendAmount);
			RenderTexture renderTexture = null;
			if (this.volumesBlending)
			{
				renderTexture = RenderTexture.GetTemporary(this.blendCacheLut.width, this.blendCacheLut.height, this.blendCacheLut.depth, this.blendCacheLut.format, RenderTextureReadWrite.Linear);
				Graphics.Blit(this.blendCacheLut, renderTexture);
				this.materialBlendCache.SetTexture("_RgbTex", renderTexture);
			}
			else
			{
				this.materialBlendCache.SetTexture("_RgbTex", texture);
			}
			this.materialBlendCache.SetTexture("_LerpRgbTex", (!(lutBlendTexture != null)) ? this.defaultLut : lutBlendTexture);
			Graphics.Blit(texture, this.blendCacheLut, this.materialBlendCache);
			if (renderTexture != null)
			{
				RenderTexture.ReleaseTemporary(renderTexture);
			}
			material.SetTexture("_RgbBlendCacheTex", this.blendCacheLut);
		}
		else if (this.volumesBlending)
		{
			material.SetTexture("_RgbBlendCacheTex", this.blendCacheLut);
		}
		else
		{
			if (texture != null)
			{
				material.SetTexture("_RgbTex", texture);
			}
			if (lutBlendTexture != null)
			{
				material.SetTexture("_LerpRgbTex", lutBlendTexture);
			}
		}
		Graphics.Blit(source, destination, material, pass);
		if (flag6 || this.volumesBlending)
		{
			this.blendCacheLut.DiscardContents();
		}
	}
}
