using System;
using UnityEngine;
using UnityEngine.Rendering;

[AddComponentMenu("AFS/Setup Advanced Foliage Shader"), ExecuteInEditMode]
public class SetupAdvancedFoliageShader : MonoBehaviour
{
	private const float TwoPI = 6.28318548f;

	public bool isLinear;

	public bool AmbientLightingSH;

	public GameObject TerrianLight0;

	public GameObject TerrianLight1;

	public GameObject DirectionalLightReference;

	private Vector3 DirectlightDir;

	private Light Sunlight;

	private Vector3 SunLightCol;

	private float SunLuminance;

	public bool GrassApproxTrans;

	[Range(0f, 2f)]
	public float GrassTransStrength = 0.5f;

	public int AFSFog_Mode = 2;

	public string AFSShader_Folder = "Assets/Advanced Foliage Shader v4/Shaders/";

	public bool disableFoginShader;

	public Vector4 Wind = new Vector4(0.85f, 0.075f, 0.4f, 0.5f);

	[Range(0.01f, 2f)]
	public float WindFrequency = 0.25f;

	[Range(0.1f, 25f)]
	public float WaveSizeFoliageShader = 10f;

	[Range(0f, 10f)]
	public float LeafTurbulenceFoliageShader = 4f;

	[Range(0.01f, 1f)]
	public float WindMultiplierForGrassshader = 1f;

	[Range(0f, 10f)]
	public float WaveSizeForGrassshader = 1f;

	[Range(0f, 1f)]
	public float WindJitterFrequencyForGrassshader = 0.25f;

	[Range(0f, 1f)]
	public float WindJitterScaleForGrassshader = 0.15f;

	public AnimationCurve WindJitterScaleForGrassshaderCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 1f),
		new Keyframe(4f, 0.2f)
	});

	public bool SyncWindDir;

	[Range(0f, 10f)]
	public float WindMuliplierForTreeShaderPrimary = 1f;

	[Range(0f, 10f)]
	public float WindMuliplierForTreeShaderSecondary = 1f;

	public Vector4 WindMuliplierForTreeShader = new Vector4(1f, 1f, 1f, 1f);

	private float temp_WindFrequency = 0.25f;

	private float temp_WindJitterFrequency = 0.25f;

	private float freqSpeed = 0.05f;

	private float domainTime_Wind;

	private float domainTime_2ndBending;

	private float domainTime_Grass;

	[Range(0f, 1f)]
	public float RainAmount;

	[Range(0f, 1f)]
	public float VertexLitAlphaCutOff = 0.3f;

	public Color VertexLitTranslucencyColor = new Color(0.73f, 0.85f, 0.4f, 1f);

	[Range(0f, 1f)]
	public float VertexLitTranslucencyViewDependency = 0.7f;

	[Range(0f, 1f)]
	public float VertexLitShadowStrength = 0.8f;

	public Color VertexLitSpecularReflectivity = new Color(0.2f, 0.2f, 0.2f, 1f);

	[Range(0f, 100f)]
	public Vector2 AfsSpecFade = new Vector2(30f, 10f);

	public Texture TerrainFoliageNrmSpecMap;

	public bool AutoSyncToTerrain;

	public Terrain SyncedTerrain;

	public bool AutoSyncInPlaymode;

	public float DetailDistanceForGrassShader = 80f;

	[Range(0f, 1000f)]
	public float BillboardStart = 50f;

	[Range(0f, 30f)]
	public float BillboardFadeLenght = 5f;

	public bool GrassAnimateNormal;

	public Color GrassWavingTint = Color.white;

	public Color AFSTreeColor = Color.black;

	private Camera MainCam;

	public bool TreeBillboardShadows;

	[Range(10f, 100f)]
	public float BillboardFadeOutLength = 60f;

	public bool BillboardAdjustToCamera = true;

	[Range(10f, 90f)]
	public float BillboardAngleLimit = 30f;

	public Color BillboardShadowColor;

	[Range(0f, 4f)]
	public float BillboardAmbientLightFactor = 1f;

	[Range(0f, 2f)]
	public float BillboardAmbientLightDesaturationFactor = 0.7f;

	public bool AutosyncShadowColor;

	public bool EnableCameraLayerCulling;

	[Range(10f, 300f)]
	public int SmallDetailsDistance = 70;

	[Range(10f, 300f)]
	public int MediumDetailsDistance = 90;

	public bool AllGrassObjectsCombined;

	private Vector4 TempWind;

	private float GrassWindSpeed;

	private float SinusWave;

	private Vector4 TriangleWaves;

	private float Oscillation;

	private Vector3 CameraForward = new Vector3(0f, 0f, 0f);

	private Vector3 ShadowCameraForward = new Vector3(0f, 0f, 0f);

	private Vector3 CameraForwardVec;

	private float rollingX;

	private float rollingXShadow;

	private Vector3 lightDir;

	private Vector3 templightDir;

	private float CameraAngle;

	private Terrain[] allTerrains;

	private Vector3[] fLight = new Vector3[9];

	private Vector4[] vCoeff = new Vector4[3];

	private void Awake()
	{
		this.afsSyncFrequencies();
		this.afsCheckColorSpace();
		this.afsLightingSettings();
		this.afsUpdateWind();
		this.afsUpdateRain();
		this.afsSetupTerrainEngine();
		this.afsAutoSyncToTerrain();
		this.afsUpdateGrassTreesBillboards();
		this.afsSetupCameraLayerCulling();
	}

	public void Update()
	{
		this.afsLightingSettings();
		this.afsUpdateRain();
		this.afsAutoSyncToTerrain();
		this.afsUpdateGrassTreesBillboards();
	}

	public void FixedUpdate()
	{
		this.afsUpdateWind();
	}

	private void afsSyncFrequencies()
	{
		this.temp_WindFrequency = this.WindFrequency;
		this.temp_WindJitterFrequency = this.WindJitterFrequencyForGrassshader;
		this.domainTime_Wind = 0f;
		this.domainTime_2ndBending = 0f;
		this.domainTime_Grass = 0f;
	}

	private void afsCheckColorSpace()
	{
	}

	private void afsLightingSettings()
	{
		if (this.DirectionalLightReference != null)
		{
			this.DirectlightDir = this.DirectionalLightReference.transform.forward;
			if (this.Sunlight == null)
			{
				this.Sunlight = this.DirectionalLightReference.GetComponent<Light>();
			}
			if (!this.Sunlight.enabled)
			{
				Shader.SetGlobalVector("_AfsDirectSunDir", new Vector4(0f, 0f, 0f, 0f));
			}
			else
			{
				this.SunLightCol = new Vector3(this.Sunlight.color.r, this.Sunlight.color.g, this.Sunlight.color.b) * this.Sunlight.intensity;
				this.SunLuminance = Vector3.Dot(this.SunLightCol, new Vector3(0.22f, 0.707f, 0.071f));
				Shader.SetGlobalVector("_AfsDirectSunDir", new Vector4(this.DirectlightDir.x, this.DirectlightDir.y, this.DirectlightDir.z, this.SunLuminance));
				Shader.SetGlobalVector("_AfsDirectSunCol", this.SunLightCol);
			}
		}
		else
		{
			Shader.SetGlobalVector("_AfsDirectSunDir", new Vector4(0f, 0f, 0f, 0f));
			Shader.SetGlobalVector("_AfsDirectSunCol", new Vector3(0f, 0f, 0f));
		}
		Shader.SetGlobalVector("_AfsSpecFade", new Vector4(this.AfsSpecFade.x, this.AfsSpecFade.y, 1f, 1f));
	}

	private void afsSetupTerrainEngine()
	{
		Shader.SetGlobalFloat("_AfsAlphaCutOff", this.VertexLitAlphaCutOff);
		Shader.SetGlobalFloat("_AfsTranslucencyViewDependency", this.VertexLitTranslucencyViewDependency);
		Shader.SetGlobalFloat("_AfsShadowStrength", this.VertexLitShadowStrength);
		Shader.SetGlobalColor("_AfsTranslucencyColor", this.VertexLitTranslucencyColor);
		if (this.isLinear)
		{
			Shader.SetGlobalColor("_AfsSpecularReflectivity", this.VertexLitSpecularReflectivity.linear);
		}
		else
		{
			Shader.SetGlobalColor("_AfsSpecularReflectivity", this.VertexLitSpecularReflectivity);
		}
		if (this.TerrainFoliageNrmSpecMap != null)
		{
			Shader.SetGlobalTexture("_TerrianBumpTransSpecMap", this.TerrainFoliageNrmSpecMap);
		}
		else
		{
			Shader.SetGlobalTexture("_TerrianBumpTransSpecMap", null);
		}
	}

	private void afsUpdateWind()
	{
		if (this.SyncWindDir)
		{
			this.Wind = new Vector4(base.transform.forward.x, base.transform.forward.y, base.transform.forward.z, this.Wind.w);
		}
		this.TempWind = this.Wind;
		this.TempWind.w = this.TempWind.w * 2f;
		Shader.SetGlobalVector("_Wind", this.TempWind);
		Shader.SetGlobalVector("_WindStrengthTrees", new Vector2(this.TempWind.w * this.WindMuliplierForTreeShaderPrimary, this.TempWind.w * this.WindMuliplierForTreeShaderSecondary));
		Shader.SetGlobalFloat("_WindFrequency", this.WindFrequency);
		this.domainTime_Wind += this.temp_WindFrequency * Time.deltaTime * 2f;
		this.domainTime_2ndBending += Time.deltaTime;
		Shader.SetGlobalVector("_AfsTimeFrequency", new Vector4(this.domainTime_Wind, this.domainTime_2ndBending, 0.375f * (1f + this.temp_WindFrequency * this.LeafTurbulenceFoliageShader), 0.193f * (1f + this.temp_WindFrequency * this.LeafTurbulenceFoliageShader)));
		this.temp_WindFrequency = Mathf.MoveTowards(this.temp_WindFrequency, this.WindFrequency, this.freqSpeed);
		this.SinusWave = Mathf.Sin(this.domainTime_Wind);
		this.TriangleWaves = this.SmoothTriangleWave(new Vector4(this.SinusWave, this.SinusWave * 0.8f, 0f, 0f));
		this.Oscillation = this.TriangleWaves.x + this.TriangleWaves.y * this.TriangleWaves.y;
		this.Oscillation = (this.Oscillation + 8f) * 0.125f * this.TempWind.w;
		this.TempWind.x = this.TempWind.x * this.Oscillation;
		this.TempWind.z = this.TempWind.z * this.Oscillation;
		Shader.SetGlobalFloat("_AfsWaveSize", 0.5f / this.WaveSizeFoliageShader);
		float num = this.WindJitterScaleForGrassshaderCurve.Evaluate(this.TempWind.w);
		Shader.SetGlobalFloat("_AfsWindJitterScale", this.WindJitterScaleForGrassshader * (10f * num));
		Shader.SetGlobalVector("_AfsGrassWind", this.TempWind * this.WindMultiplierForGrassshader);
		this.domainTime_Grass += this.temp_WindJitterFrequency * Time.deltaTime;
		this.temp_WindJitterFrequency = Mathf.MoveTowards(this.temp_WindJitterFrequency, this.WindJitterFrequencyForGrassshader, this.freqSpeed);
		this.GrassWindSpeed = this.domainTime_Grass * 0.1f;
		Shader.SetGlobalVector("_AfsWaveAndDistance", new Vector4(this.GrassWindSpeed, this.WaveSizeForGrassshader, this.TempWind.w, this.DetailDistanceForGrassShader * this.DetailDistanceForGrassShader));
		Shader.SetGlobalVector("_AFSWindMuliplier", this.WindMuliplierForTreeShader);
	}

	private void afsUpdateRain()
	{
		Shader.SetGlobalFloat("_AfsRainamount", this.RainAmount);
	}

	private void afsAutoSyncToTerrain()
	{
		if (this.AutoSyncToTerrain && this.SyncedTerrain != null)
		{
			this.DetailDistanceForGrassShader = this.SyncedTerrain.detailObjectDistance;
			this.BillboardStart = this.SyncedTerrain.treeBillboardDistance;
			if (Application.isPlaying && !this.TreeBillboardShadows)
			{
				this.BillboardFadeLenght = this.SyncedTerrain.treeCrossFadeLength;
			}
			this.GrassWavingTint = this.SyncedTerrain.terrainData.wavingGrassTint;
		}
	}

	private void afsUpdateGrassTreesBillboards()
	{
		Shader.SetGlobalColor("_AfsWavingTint", this.GrassWavingTint);
	}

	private void afsSetupCameraLayerCulling()
	{
		if (this.EnableCameraLayerCulling)
		{
			for (int i = 0; i < Camera.allCameras.Length; i++)
			{
				float[] array = new float[32];
				array = Camera.allCameras[i].layerCullDistances;
				array[8] = (float)this.SmallDetailsDistance;
				array[9] = (float)this.MediumDetailsDistance;
				Camera.allCameras[i].layerCullDistances = array;
			}
		}
	}

	private void UpdateLightingForClassicBillboards()
	{
		if (RenderSettings.ambientMode == AmbientMode.Skybox)
		{
			this.AmbientLightingSH = true;
			this.UdpateSHLightingforBillboards();
			Shader.EnableKeyword("AFS_SH_AMBIENT");
			Shader.DisableKeyword("AFS_COLOR_AMBIENT");
			Shader.DisableKeyword("AFS_GRADIENT_AMBIENT");
		}
		else if (RenderSettings.ambientMode == AmbientMode.Trilight)
		{
			this.AmbientLightingSH = false;
			if (this.isLinear)
			{
				Shader.SetGlobalColor("_AfsSkyColor", RenderSettings.ambientSkyColor.linear);
				Shader.SetGlobalColor("_AfsGroundColor", RenderSettings.ambientGroundColor.linear);
				Shader.SetGlobalColor("_AfsEquatorColor", RenderSettings.ambientEquatorColor.linear);
			}
			else
			{
				Shader.SetGlobalColor("_AfsSkyColor", RenderSettings.ambientSkyColor);
				Shader.SetGlobalColor("_AfsGroundColor", RenderSettings.ambientGroundColor);
				Shader.SetGlobalColor("_AfsEquatorColor", RenderSettings.ambientEquatorColor);
			}
			Shader.DisableKeyword("AFS_SH_AMBIENT");
			Shader.DisableKeyword("AFS_COLOR_AMBIENT");
			Shader.EnableKeyword("AFS_GRADIENT_AMBIENT");
		}
		else if (RenderSettings.ambientMode == AmbientMode.Flat)
		{
			this.AmbientLightingSH = false;
			Shader.SetGlobalVector("_AfsAmbientColor", RenderSettings.ambientLight);
			Shader.DisableKeyword("AFS_SH_AMBIENT");
			Shader.EnableKeyword("AFS_COLOR_AMBIENT");
			Shader.DisableKeyword("AFS_GRADIENT_AMBIENT");
		}
	}

	private void UdpateSHLightingforBillboards()
	{
		this.fLight[0].x = RenderSettings.ambientProbe[0, 0];
		this.fLight[0].y = RenderSettings.ambientProbe[1, 0];
		this.fLight[0].z = RenderSettings.ambientProbe[2, 0];
		this.fLight[1].x = RenderSettings.ambientProbe[0, 1];
		this.fLight[1].y = RenderSettings.ambientProbe[1, 1];
		this.fLight[1].z = RenderSettings.ambientProbe[2, 1];
		this.fLight[2].x = RenderSettings.ambientProbe[0, 2];
		this.fLight[2].y = RenderSettings.ambientProbe[1, 2];
		this.fLight[2].z = RenderSettings.ambientProbe[2, 2];
		this.fLight[3].x = RenderSettings.ambientProbe[0, 3];
		this.fLight[3].y = RenderSettings.ambientProbe[1, 3];
		this.fLight[3].z = RenderSettings.ambientProbe[2, 3];
		this.fLight[4].x = RenderSettings.ambientProbe[0, 4];
		this.fLight[4].y = RenderSettings.ambientProbe[1, 4];
		this.fLight[4].z = RenderSettings.ambientProbe[2, 4];
		this.fLight[5].x = RenderSettings.ambientProbe[0, 5];
		this.fLight[5].y = RenderSettings.ambientProbe[1, 5];
		this.fLight[5].z = RenderSettings.ambientProbe[2, 5];
		this.fLight[6].x = RenderSettings.ambientProbe[0, 6];
		this.fLight[6].y = RenderSettings.ambientProbe[1, 6];
		this.fLight[6].z = RenderSettings.ambientProbe[2, 6];
		this.fLight[7].x = RenderSettings.ambientProbe[0, 7];
		this.fLight[7].y = RenderSettings.ambientProbe[1, 7];
		this.fLight[7].z = RenderSettings.ambientProbe[2, 7];
		this.fLight[8].x = RenderSettings.ambientProbe[0, 8];
		this.fLight[8].y = RenderSettings.ambientProbe[1, 8];
		this.fLight[8].z = RenderSettings.ambientProbe[2, 8];
		float num = 3.54490781f;
		float num2 = 1f / (2f * num);
		float num3 = Mathf.Sqrt(3f) / (3f * num);
		float num4 = Mathf.Sqrt(15f) / (8f * num);
		float num5 = Mathf.Sqrt(5f) / (16f * num);
		float num6 = 0.5f * num4;
		for (int i = 0; i < 3; i++)
		{
			this.vCoeff[i].x = -num3 * this.fLight[3][i];
			this.vCoeff[i].y = -num3 * this.fLight[1][i];
			this.vCoeff[i].z = num3 * this.fLight[2][i];
			this.vCoeff[i].w = num2 * this.fLight[0][i] - num5 * this.fLight[6][i];
		}
		Shader.SetGlobalVector("afs_SHAr", this.vCoeff[0] * RenderSettings.ambientIntensity);
		Shader.SetGlobalVector("afs_SHAg", this.vCoeff[1] * RenderSettings.ambientIntensity);
		Shader.SetGlobalVector("afs_SHAb", this.vCoeff[2] * RenderSettings.ambientIntensity);
		for (int i = 0; i < 3; i++)
		{
			this.vCoeff[i].x = num4 * this.fLight[4][i];
			this.vCoeff[i].y = -num4 * this.fLight[5][i];
			this.vCoeff[i].z = 3f * num5 * this.fLight[6][i];
			this.vCoeff[i].w = -num4 * this.fLight[7][i];
		}
		Shader.SetGlobalVector("afs_SHBr", this.vCoeff[0] * RenderSettings.ambientIntensity);
		Shader.SetGlobalVector("afs_SHBg", this.vCoeff[1] * RenderSettings.ambientIntensity);
		Shader.SetGlobalVector("afs_SHBb", this.vCoeff[2] * RenderSettings.ambientIntensity);
		this.vCoeff[0].x = num6 * this.fLight[8][0];
		this.vCoeff[0].y = num6 * this.fLight[8][1];
		this.vCoeff[0].z = num6 * this.fLight[8][2];
		this.vCoeff[0].w = 1f;
		Shader.SetGlobalVector("afs_SHC", this.vCoeff[0] * RenderSettings.ambientIntensity);
	}

	private Color Desaturate(float r, float g, float b)
	{
		float num = 0.3f * r + 0.59f * g + 0.11f * b;
		r = num * this.BillboardAmbientLightDesaturationFactor + r * (1f - this.BillboardAmbientLightDesaturationFactor);
		g = num * this.BillboardAmbientLightDesaturationFactor + g * (1f - this.BillboardAmbientLightDesaturationFactor);
		b = num * this.BillboardAmbientLightDesaturationFactor + b * (1f - this.BillboardAmbientLightDesaturationFactor);
		return new Color(r, g, b, 1f);
	}

	private float CubicSmooth(float x)
	{
		return x * x * (3f - 2f * x);
	}

	private float TriangleWave(float x)
	{
		return Mathf.Abs((x + 0.5f) % 1f * 2f - 1f);
	}

	private float SmoothTriangleWave(float x)
	{
		return this.CubicSmooth(this.TriangleWave(x));
	}

	private Vector4 CubicSmooth(Vector4 x)
	{
		x = Vector4.Scale(x, x);
		x = Vector4.Scale(x, new Vector4(3f, 3f, 3f, 3f) - 2f * x);
		return x;
	}

	private Vector4 TriangleWave(Vector4 x)
	{
		x = (x + new Vector4(0.5f, 0.5f, 0.5f, 0.5f)) * 2f - new Vector4(1f, 1f, 1f, 1f);
		return this.AbsVecFour(x);
	}

	private Vector4 SmoothTriangleWave(Vector4 x)
	{
		return this.CubicSmooth(this.TriangleWave(x));
	}

	private Vector4 FracVecFour(Vector4 a)
	{
		a.x %= 1f;
		a.y %= 1f;
		a.z %= 1f;
		a.w %= 1f;
		return a;
	}

	private Vector4 AbsVecFour(Vector4 a)
	{
		a.x = Mathf.Abs(a.x);
		a.y = Mathf.Abs(a.y);
		a.z = Mathf.Abs(a.z);
		a.w = Mathf.Abs(a.w);
		return a;
	}
}
