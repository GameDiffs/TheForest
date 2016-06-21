using System;
using UnityEngine;

[AddComponentMenu("Image Effects/HBAO"), ExecuteInEditMode, RequireComponent(typeof(Camera))]
public class HBAO : MonoBehaviour
{
	public enum ShowType
	{
		Normal,
		AOOnly,
		ColorBleedingOnly,
		SplitWithoutAOAndWithAO,
		SplitWithAOAndAOOnly,
		SplitWithoutAOAndAOOnly
	}

	public enum Quality
	{
		Lowest,
		Low,
		Medium,
		High,
		Highest
	}

	public enum Resolution
	{
		Full,
		Half
	}

	public enum Blur
	{
		None,
		Narrow,
		Medium,
		Wide,
		ExtraWide
	}

	public enum NoiseType
	{
		Random,
		Dither
	}

	[Tooltip("The way the AO is shown on screen.")]
	public HBAO.ShowType showType;

	[Tooltip("The quality of the AO.")]
	public HBAO.Quality quality = HBAO.Quality.Medium;

	[Tooltip("The resolution at which the AO is calculated.")]
	public HBAO.Resolution resolution;

	[Range(0f, 2f), Tooltip("Eye-space AO radius: this is the distance outside which occluders are ignored.")]
	public float radius = 0.5f;

	[Range(32f, 256f), Tooltip("Maximum radius in pixels: this prevents the radius to grow too much with close-up object and impact on performances.")]
	public float maxRadiusPixels = 256f;

	[Range(0f, 0.5f), Tooltip("For low-tessellated geometry, occlusion variations tend to appear at creases and ridges, which betray the underlying tessellation. To remove these artifacts, we use an angle bias parameter which restricts the hemisphere.")]
	public float bias = 0.05f;

	[Range(0f, 10f), Tooltip("This value allows to scale up the ambient occlusion values.")]
	public float intensity = 1f;

	[Range(0f, 1f), Tooltip("This value allows to attenuate ambient occlusion depending on final color luminance.")]
	public float luminanceInfluence;

	[Tooltip("The type of blur to use.")]
	public HBAO.Blur blur = HBAO.Blur.Medium;

	[Range(0f, 16f), Tooltip("This parameter controls the depth-dependent weight of the bilateral filter, to avoid bleeding across edges. A zero sharpness is a pure Gaussian blur. Increasing the blur sharpness removes bleeding by using lower weights for samples with large depth delta from the current pixel.")]
	public float blurSharpness = 8f;

	[Tooltip("The type of noise to use.")]
	public HBAO.NoiseType noiseType = HBAO.NoiseType.Dither;

	[Range(0f, 4f), Tooltip("This value allows to control the saturation of the color bleeding.")]
	public float colorBleedSaturation;

	[Range(0f, 32f), Tooltip("This value allows to scale the contribution of the color bleeding samples.")]
	public float albedoMultiplier = 16f;

	public Texture2D noiseTex;

	public Shader hbaoShader;

	private HBAO.ShowType _showType;

	private HBAO.Quality _quality;

	private HBAO.Resolution _resolution;

	private float _radius;

	private float _maxRadiusPixels;

	private float _bias;

	private float _intensity;

	private float _luminanceInfluence;

	private HBAO.Blur _blur;

	private float _blurSharpness;

	private HBAO.NoiseType _noiseType;

	private float _colorBleedSaturation;

	private float _albedoMultiplier;

	private RenderingPath _renderingPath;

	private Material _hbaoMaterial;

	private Camera _hbaoCamera;

	private Matrix4x4 _projMatrix;

	private int[] _numSampleDirections = new int[]
	{
		3,
		4,
		6,
		8,
		8
	};

	private void Start()
	{
		if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures || !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
		{
			Debug.LogWarning("HBAO shader is not supported on this platform.");
			base.enabled = false;
			return;
		}
		if (this.hbaoShader != null && !this.hbaoShader.isSupported)
		{
			Debug.LogWarning("HBAO shader is not supported on this platform.");
			base.enabled = false;
			return;
		}
		if (this.hbaoShader == null)
		{
			return;
		}
		this.CreateMaterial();
	}

	private void OnDisable()
	{
		if (this._hbaoMaterial != null)
		{
			UnityEngine.Object.DestroyImmediate(this._hbaoMaterial);
		}
		if (this.noiseTex != null)
		{
			UnityEngine.Object.DestroyImmediate(this.noiseTex);
		}
	}

	[ImageEffectOpaque]
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (this.hbaoShader == null || this._hbaoCamera == null)
		{
			Graphics.Blit(source, destination);
			return;
		}
		this.CreateMaterial();
		this._hbaoCamera.depthTextureMode |= DepthTextureMode.Depth;
		Vector4 vector = new Vector4(-2f / ((float)Screen.width * this._projMatrix[0]), -2f / ((float)Screen.height * this._projMatrix[5]), (1f - this._projMatrix[2]) / this._projMatrix[0], (1f + this._projMatrix[6]) / this._projMatrix[5]);
		this._hbaoMaterial.SetVector("_ProjInfo", vector);
		this._hbaoMaterial.SetMatrix("_WorldToCameraMatrix", this._hbaoCamera.worldToCameraMatrix);
		if (this._showType != this.showType || this._quality != this.quality || this._resolution != this.resolution || this._radius != this.radius || this._maxRadiusPixels != this.maxRadiusPixels || this._bias != this.bias || this._intensity != this.intensity || this._luminanceInfluence != this.luminanceInfluence || this._blur != this.blur || this._blurSharpness != this.blurSharpness || this._noiseType != this.noiseType || this._colorBleedSaturation != this.colorBleedSaturation || this._albedoMultiplier != this.albedoMultiplier || this._renderingPath != this._hbaoCamera.renderingPath)
		{
			this.UpdateMaterialProperties();
		}
		this.RenderHBAO(source, destination);
	}

	private void RenderHBAO(RenderTexture source, RenderTexture destination)
	{
		int downsampling = this.GetDownsampling();
		RenderTexture temporary = RenderTexture.GetTemporary(source.width >> downsampling, source.height >> downsampling);
		Graphics.Blit(source, temporary, this._hbaoMaterial, this.GetAoPass());
		if (this._blur != HBAO.Blur.None)
		{
			RenderTexture temporary2 = RenderTexture.GetTemporary(source.width, source.height);
			Graphics.Blit(temporary, temporary2, this._hbaoMaterial, this.GetBlurXPass());
			Graphics.Blit(temporary2, temporary, this._hbaoMaterial, this.GetBlurYPass());
			RenderTexture.ReleaseTemporary(temporary2);
		}
		this._hbaoMaterial.SetTexture("_HBAOTex", temporary);
		Graphics.Blit(source, destination, this._hbaoMaterial, this.GetFinalPass());
		RenderTexture.ReleaseTemporary(temporary);
	}

	private void CreateMaterial()
	{
		if (this._hbaoMaterial == null)
		{
			this._hbaoMaterial = new Material(this.hbaoShader);
			this._hbaoMaterial.hideFlags = HideFlags.HideAndDontSave;
			this._hbaoCamera = base.GetComponent<Camera>();
			this._projMatrix = this._hbaoCamera.projectionMatrix;
			this.UpdateMaterialProperties();
		}
	}

	private void UpdateMaterialProperties()
	{
		if (this.noiseTex == null || this._quality != this.quality || this._noiseType != this.noiseType)
		{
			float num = (float)((this.noiseType != HBAO.NoiseType.Dither) ? 64 : 4);
			this.CreateRandomTexture((int)num);
			this._hbaoMaterial.SetTexture("_NoiseTex", this.noiseTex);
			this._hbaoMaterial.SetFloat("_NoiseTexSize", num);
		}
		this._showType = this.showType;
		this._quality = this.quality;
		this._resolution = this.resolution;
		this._radius = this.radius;
		this._maxRadiusPixels = this.maxRadiusPixels;
		this._bias = this.bias;
		this._intensity = this.intensity;
		this._luminanceInfluence = this.luminanceInfluence;
		this._blur = this.blur;
		this._blurSharpness = this.blurSharpness;
		this._noiseType = this.noiseType;
		this._colorBleedSaturation = this.colorBleedSaturation;
		this._albedoMultiplier = this.albedoMultiplier;
		this._renderingPath = this._hbaoCamera.renderingPath;
		this._hbaoMaterial.SetFloat("_Radius", this._radius);
		this._hbaoMaterial.SetFloat("_MaxRadiusPixels", this._maxRadiusPixels);
		this._hbaoMaterial.SetFloat("_NegInvRadius2", -1f / (this._radius * this._radius));
		this._hbaoMaterial.SetFloat("_AngleBias", this._bias);
		this._hbaoMaterial.SetFloat("_AOmultiplier", 2f * (1f / (1f - this._bias)));
		this._hbaoMaterial.SetFloat("_Intensity", this._intensity);
		this._hbaoMaterial.SetFloat("_LuminanceInfluence", this._luminanceInfluence);
		this._hbaoMaterial.SetFloat("_ColorBleedSaturation", this._colorBleedSaturation);
		this._hbaoMaterial.SetFloat("_AlbedoMultiplier", this._albedoMultiplier);
		this._hbaoMaterial.SetFloat("_BlurSharpness", this._blurSharpness);
	}

	private int GetDownsampling()
	{
		HBAO.Resolution resolution = this.resolution;
		if (resolution == HBAO.Resolution.Full)
		{
			return 0;
		}
		if (resolution != HBAO.Resolution.Half)
		{
			return 0;
		}
		return 1;
	}

	private int GetAoPass()
	{
		switch (this.quality)
		{
		case HBAO.Quality.Lowest:
			return 0;
		case HBAO.Quality.Low:
			return 1;
		case HBAO.Quality.Medium:
			return 2;
		case HBAO.Quality.High:
			return 3;
		case HBAO.Quality.Highest:
			return 4;
		default:
			return 2;
		}
	}

	private int GetBlurXPass()
	{
		switch (this.blur)
		{
		case HBAO.Blur.Narrow:
			return 5;
		case HBAO.Blur.Medium:
			return 7;
		case HBAO.Blur.Wide:
			return 9;
		case HBAO.Blur.ExtraWide:
			return 11;
		default:
			return 7;
		}
	}

	private int GetBlurYPass()
	{
		switch (this.blur)
		{
		case HBAO.Blur.Narrow:
			return 6;
		case HBAO.Blur.Medium:
			return 8;
		case HBAO.Blur.Wide:
			return 10;
		case HBAO.Blur.ExtraWide:
			return 12;
		default:
			return 8;
		}
	}

	private int GetFinalPass()
	{
		switch (this.showType)
		{
		case HBAO.ShowType.Normal:
			return 13;
		case HBAO.ShowType.AOOnly:
			return 14;
		case HBAO.ShowType.ColorBleedingOnly:
			return 15;
		case HBAO.ShowType.SplitWithoutAOAndWithAO:
			return 16;
		case HBAO.ShowType.SplitWithAOAndAOOnly:
			return 17;
		case HBAO.ShowType.SplitWithoutAOAndAOOnly:
			return 18;
		default:
			return 13;
		}
	}

	private void CreateRandomTexture(int size)
	{
		this.noiseTex = new Texture2D(size, size, TextureFormat.RGB24, false, true);
		this.noiseTex.filterMode = FilterMode.Point;
		this.noiseTex.wrapMode = TextureWrapMode.Repeat;
		for (int i = 0; i < size; i++)
		{
			for (int j = 0; j < size; j++)
			{
				float num = UnityEngine.Random.Range(0f, 1f);
				float b = UnityEngine.Random.Range(0f, 1f);
				float f = 6.28318548f * num / (float)this._numSampleDirections[this.GetAoPass()];
				Color color = new Color(Mathf.Cos(f), Mathf.Sin(f), b);
				this.noiseTex.SetPixel(i, j, color);
			}
		}
		this.noiseTex.Apply();
	}
}
