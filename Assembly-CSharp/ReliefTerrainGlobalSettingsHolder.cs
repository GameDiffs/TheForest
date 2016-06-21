using System;
using UnityEngine;

[Serializable]
public class ReliefTerrainGlobalSettingsHolder
{
	public bool useTerrainMaterial = true;

	public int numTiles;

	public int numLayers;

	[NonSerialized]
	public bool dont_check_weak_references;

	[NonSerialized]
	public bool dont_check_for_interfering_terrain_replacement_shaders;

	[NonSerialized]
	public Texture2D[] splats_glossBaked = new Texture2D[12];

	[NonSerialized]
	public Texture2D[] atlas_glossBaked = new Texture2D[3];

	public RTPGlossBaked[] gloss_baked = new RTPGlossBaked[12];

	public Texture2D[] splats;

	public Texture2D[] splat_atlases = new Texture2D[3];

	public string save_path_atlasA = string.Empty;

	public string save_path_atlasB = string.Empty;

	public string save_path_atlasC = string.Empty;

	public string save_path_terrain_steepness = string.Empty;

	public string save_path_terrain_height = string.Empty;

	public string save_path_terrain_direction = string.Empty;

	public string save_path_Bump01 = string.Empty;

	public string save_path_Bump23 = string.Empty;

	public string save_path_Bump45 = string.Empty;

	public string save_path_Bump67 = string.Empty;

	public string save_path_Bump89 = string.Empty;

	public string save_path_BumpAB = string.Empty;

	public string save_path_HeightMap = string.Empty;

	public string save_path_HeightMap2 = string.Empty;

	public string save_path_HeightMap3 = string.Empty;

	public string save_path_SSColorCombinedA = string.Empty;

	public string save_path_SSColorCombinedB = string.Empty;

	public string newPresetName = "a preset name...";

	public Texture2D activateObject;

	private GameObject _RTP_LODmanager;

	public RTP_LODmanager _RTP_LODmanagerScript;

	public bool super_simple_active;

	public float RTP_MIP_BIAS;

	public Color _SpecColor;

	public float RTP_DeferredAddPassSpec = 0.5f;

	public float MasterLayerBrightness = 1f;

	public float MasterLayerSaturation = 1f;

	public float EmissionRefractFiltering = 4f;

	public float EmissionRefractAnimSpeed = 4f;

	public RTPColorChannels SuperDetailA_channel;

	public RTPColorChannels SuperDetailB_channel;

	public Texture2D Bump01;

	public Texture2D Bump23;

	public Texture2D Bump45;

	public Texture2D Bump67;

	public Texture2D Bump89;

	public Texture2D BumpAB;

	public Texture2D BumpGlobal;

	public int BumpGlobalCombinedSize = 1024;

	public Texture2D SSColorCombinedA;

	public Texture2D SSColorCombinedB;

	public Texture2D VerticalTexture;

	public float BumpMapGlobalScale;

	public Vector3 GlobalColorMapBlendValues;

	public float _GlobalColorMapNearMIP;

	public float GlobalColorMapSaturation;

	public float GlobalColorMapSaturationFar = 1f;

	public float GlobalColorMapDistortByPerlin = 0.005f;

	public float GlobalColorMapBrightness;

	public float GlobalColorMapBrightnessFar = 1f;

	public float _FarNormalDamp;

	public float blendMultiplier;

	public Vector3 terrainTileSize;

	public Texture2D HeightMap;

	public Vector4 ReliefTransform;

	public float DIST_STEPS;

	public float WAVELENGTH;

	public float ReliefBorderBlend;

	public float ExtrudeHeight;

	public float LightmapShading;

	public float RTP_AOsharpness;

	public float RTP_AOamp;

	public bool colorSpaceLinear;

	public float SHADOW_STEPS;

	public float WAVELENGTH_SHADOWS;

	public float SelfShadowStrength;

	public float ShadowSmoothing;

	public float ShadowSoftnessFade = 0.8f;

	public float distance_start;

	public float distance_transition;

	public float distance_start_bumpglobal;

	public float distance_transition_bumpglobal;

	public float rtp_perlin_start_val;

	public float _Phong;

	public float tessHeight = 300f;

	public float _TessSubdivisions = 1f;

	public float _TessSubdivisionsFar = 1f;

	public float _TessYOffset;

	public float trees_shadow_distance_start;

	public float trees_shadow_distance_transition;

	public float trees_shadow_value;

	public float trees_pixel_distance_start;

	public float trees_pixel_distance_transition;

	public float trees_pixel_blend_val;

	public float global_normalMap_multiplier;

	public float global_normalMap_farUsage;

	public float _AmbientEmissiveMultiplier = 1f;

	public float _AmbientEmissiveRelief = 0.5f;

	public Texture2D HeightMap2;

	public Texture2D HeightMap3;

	public int rtp_mipoffset_globalnorm;

	public float _SuperDetailTiling;

	public Texture2D SuperDetailA;

	public Texture2D SuperDetailB;

	public Texture2D TERRAIN_ReflectionMap;

	public RTPColorChannels TERRAIN_ReflectionMap_channel;

	public Color TERRAIN_ReflColorA;

	public Color TERRAIN_ReflColorB;

	public Color TERRAIN_ReflColorC;

	public float TERRAIN_ReflColorCenter = 0.5f;

	public float TERRAIN_ReflGlossAttenuation = 0.5f;

	public float TERRAIN_ReflectionRotSpeed;

	public float TERRAIN_GlobalWetness;

	public Texture2D TERRAIN_RippleMap;

	public float TERRAIN_RippleScale;

	public float TERRAIN_FlowScale;

	public float TERRAIN_FlowSpeed;

	public float TERRAIN_FlowCycleScale;

	public float TERRAIN_FlowMipOffset;

	public float TERRAIN_WetDarkening;

	public float TERRAIN_WetDropletsStrength;

	public float TERRAIN_WetHeight_Treshold;

	public float TERRAIN_WetHeight_Transition;

	public float TERRAIN_RainIntensity;

	public float TERRAIN_DropletsSpeed;

	public float TERRAIN_mipoffset_flowSpeed;

	public float TERRAIN_CausticsAnimSpeed;

	public Color TERRAIN_CausticsColor;

	public GameObject TERRAIN_CausticsWaterLevelRefObject;

	public float TERRAIN_CausticsWaterLevel;

	public float TERRAIN_CausticsWaterLevelByAngle;

	public float TERRAIN_CausticsWaterDeepFadeLength;

	public float TERRAIN_CausticsWaterShallowFadeLength;

	public float TERRAIN_CausticsTilingScale;

	public Texture2D TERRAIN_CausticsTex;

	public Color rtp_customAmbientCorrection;

	public Cubemap _CubemapDiff;

	public float TERRAIN_IBL_DiffAO_Damp = 0.25f;

	public Cubemap _CubemapSpec;

	public float TERRAIN_IBLRefl_SpecAO_Damp = 0.5f;

	public Vector4 RTP_LightDefVector;

	public Color RTP_ReflexLightDiffuseColor;

	public Color RTP_ReflexLightDiffuseColor2;

	public Color RTP_ReflexLightSpecColor;

	public Texture2D[] Bumps;

	public float[] Spec;

	public float[] FarSpecCorrection;

	public float[] MIPmult;

	public float[] MixScale;

	public float[] MixBlend;

	public float[] MixSaturation;

	public float[] RTP_gloss2mask;

	public float[] RTP_gloss_mult;

	public float[] RTP_gloss_shaping;

	public float[] RTP_Fresnel;

	public float[] RTP_FresnelAtten;

	public float[] RTP_DiffFresnel;

	public float[] RTP_IBL_bump_smoothness;

	public float[] RTP_IBL_DiffuseStrength;

	public float[] RTP_IBL_SpecStrength;

	public float[] _DeferredSpecDampAddPass;

	public float[] GlobalColorBottom;

	public float[] GlobalColorTop;

	public float[] GlobalColorColormapLoSat;

	public float[] GlobalColorColormapHiSat;

	public float[] GlobalColorLayerLoSat;

	public float[] GlobalColorLayerHiSat;

	public float[] GlobalColorLoBlend;

	public float[] GlobalColorHiBlend;

	public float[] MixBrightness;

	public float[] MixReplace;

	public float[] LayerBrightness;

	public float[] LayerBrightness2Spec;

	public float[] LayerAlbedo2SpecColor;

	public float[] LayerSaturation;

	public float[] LayerEmission;

	public Color[] LayerEmissionColor;

	public float[] LayerEmissionRefractStrength;

	public float[] LayerEmissionRefractHBedge;

	public float[] GlobalColorPerLayer;

	public float[] PER_LAYER_HEIGHT_MODIFIER;

	public float[] _SuperDetailStrengthMultA;

	public float[] _SuperDetailStrengthMultASelfMaskNear;

	public float[] _SuperDetailStrengthMultASelfMaskFar;

	public float[] _SuperDetailStrengthMultB;

	public float[] _SuperDetailStrengthMultBSelfMaskNear;

	public float[] _SuperDetailStrengthMultBSelfMaskFar;

	public float[] _SuperDetailStrengthNormal;

	public float[] _BumpMapGlobalStrength;

	public float[] AO_strength = new float[]
	{
		1f,
		1f,
		1f,
		1f,
		1f,
		1f,
		1f,
		1f,
		1f,
		1f,
		1f,
		1f
	};

	public float[] VerticalTextureStrength;

	public float VerticalTextureGlobalBumpInfluence;

	public float VerticalTextureTiling;

	public Texture2D[] Heights;

	public float[] _snow_strength_per_layer;

	public ProceduralMaterial[] Substances;

	public float[] TERRAIN_LayerWetStrength;

	public float[] TERRAIN_WaterLevel;

	public float[] TERRAIN_WaterLevelSlopeDamp;

	public float[] TERRAIN_WaterEdge;

	public float[] TERRAIN_WaterSpecularity;

	public float[] TERRAIN_WaterGloss;

	public float[] TERRAIN_WaterGlossDamper;

	public float[] TERRAIN_WaterOpacity;

	public float[] TERRAIN_Refraction;

	public float[] TERRAIN_WetRefraction;

	public float[] TERRAIN_Flow;

	public float[] TERRAIN_WetFlow;

	public float[] TERRAIN_WetSpecularity;

	public float[] TERRAIN_WetGloss;

	public Color[] TERRAIN_WaterColor;

	public float[] TERRAIN_WaterIBL_SpecWetStrength;

	public float[] TERRAIN_WaterIBL_SpecWaterStrength;

	public float[] TERRAIN_WaterEmission;

	public float _snow_strength;

	public float _global_color_brightness_to_snow;

	public float _snow_slope_factor;

	public float _snow_edge_definition;

	public float _snow_height_treshold;

	public float _snow_height_transition;

	public Color _snow_color;

	public float _snow_specular;

	public float _snow_gloss;

	public float _snow_reflectivness;

	public float _snow_deep_factor;

	public float _snow_fresnel;

	public float _snow_diff_fresnel;

	public float _snow_IBL_DiffuseStrength;

	public float _snow_IBL_SpecStrength;

	public bool _4LAYERS_SHADER_USED;

	public bool flat_dir_ref = true;

	public bool flip_dir_ref = true;

	public GameObject direction_object;

	public bool show_details;

	public bool show_details_main;

	public bool show_details_atlasing;

	public bool show_details_layers;

	public bool show_details_uv_blend;

	public bool show_controlmaps;

	public bool show_controlmaps_build;

	public bool show_controlmaps_helpers;

	public bool show_controlmaps_highcost;

	public bool show_controlmaps_splats;

	public bool show_vert_texture;

	public bool show_global_color;

	public bool show_snow;

	public bool show_global_bump;

	public bool show_global_bump_normals;

	public bool show_global_bump_superdetail;

	public ReliefTerrainMenuItems submenu;

	public ReliefTerrainSettingsItems submenu_settings;

	public ReliefTerrainDerivedTexturesItems submenu_derived_textures;

	public ReliefTerrainControlTexturesItems submenu_control_textures;

	public bool show_global_wet_settings;

	public bool show_global_reflection_settings;

	public int show_active_layer;

	public bool show_derivedmaps;

	public bool show_settings;

	public bool undo_flag;

	public bool paint_flag;

	public float paint_size = 0.5f;

	public float paint_smoothness;

	public float paint_opacity = 1f;

	public Color paintColor = new Color(0.5f, 0.3f, 0f, 0f);

	public bool preserveBrightness = true;

	public bool paint_alpha_flag;

	public bool paint_wetmask;

	public RaycastHit paintHitInfo;

	public bool paintHitInfo_flag;

	public bool cut_holes;

	private Texture2D dumb_tex;

	public Color[] paintColorSwatches;

	public Material use_mat;

	public ReliefTerrainGlobalSettingsHolder()
	{
		this.gloss_baked = new RTPGlossBaked[12];
		this.Bumps = new Texture2D[12];
		this.Heights = new Texture2D[12];
		this.Spec = new float[12];
		this.FarSpecCorrection = new float[12];
		this.MIPmult = new float[12];
		this.MixScale = new float[12];
		this.MixBlend = new float[12];
		this.MixSaturation = new float[12];
		this.RTP_gloss2mask = new float[12];
		this.RTP_gloss_mult = new float[12];
		this.RTP_gloss_shaping = new float[12];
		this.RTP_Fresnel = new float[12];
		this.RTP_FresnelAtten = new float[12];
		this.RTP_DiffFresnel = new float[12];
		this.RTP_IBL_bump_smoothness = new float[12];
		this.RTP_IBL_DiffuseStrength = new float[12];
		this.RTP_IBL_SpecStrength = new float[12];
		this._DeferredSpecDampAddPass = new float[12];
		this.MixBrightness = new float[12];
		this.MixReplace = new float[12];
		this.LayerBrightness = new float[12];
		this.LayerBrightness2Spec = new float[12];
		this.LayerAlbedo2SpecColor = new float[12];
		this.LayerSaturation = new float[12];
		this.LayerEmission = new float[12];
		this.LayerEmissionColor = new Color[12];
		this.LayerEmissionRefractStrength = new float[12];
		this.LayerEmissionRefractHBedge = new float[12];
		this.GlobalColorPerLayer = new float[12];
		this.GlobalColorBottom = new float[12];
		this.GlobalColorTop = new float[12];
		this.GlobalColorColormapLoSat = new float[12];
		this.GlobalColorColormapHiSat = new float[12];
		this.GlobalColorLayerLoSat = new float[12];
		this.GlobalColorLayerHiSat = new float[12];
		this.GlobalColorLoBlend = new float[12];
		this.GlobalColorHiBlend = new float[12];
		this.PER_LAYER_HEIGHT_MODIFIER = new float[12];
		this._snow_strength_per_layer = new float[12];
		this.Substances = new ProceduralMaterial[12];
		this._SuperDetailStrengthMultA = new float[12];
		this._SuperDetailStrengthMultASelfMaskNear = new float[12];
		this._SuperDetailStrengthMultASelfMaskFar = new float[12];
		this._SuperDetailStrengthMultB = new float[12];
		this._SuperDetailStrengthMultBSelfMaskNear = new float[12];
		this._SuperDetailStrengthMultBSelfMaskFar = new float[12];
		this._SuperDetailStrengthNormal = new float[12];
		this._BumpMapGlobalStrength = new float[12];
		this.AO_strength = new float[12];
		this.VerticalTextureStrength = new float[12];
		this.TERRAIN_LayerWetStrength = new float[12];
		this.TERRAIN_WaterLevel = new float[12];
		this.TERRAIN_WaterLevelSlopeDamp = new float[12];
		this.TERRAIN_WaterEdge = new float[12];
		this.TERRAIN_WaterSpecularity = new float[12];
		this.TERRAIN_WaterGloss = new float[12];
		this.TERRAIN_WaterGlossDamper = new float[12];
		this.TERRAIN_WaterOpacity = new float[12];
		this.TERRAIN_Refraction = new float[12];
		this.TERRAIN_WetRefraction = new float[12];
		this.TERRAIN_Flow = new float[12];
		this.TERRAIN_WetFlow = new float[12];
		this.TERRAIN_WetSpecularity = new float[12];
		this.TERRAIN_WetGloss = new float[12];
		this.TERRAIN_WaterColor = new Color[12];
		this.TERRAIN_WaterIBL_SpecWetStrength = new float[12];
		this.TERRAIN_WaterIBL_SpecWaterStrength = new float[12];
		this.TERRAIN_WaterEmission = new float[12];
	}

	public void ReInit(Terrain terrainComp)
	{
		if (terrainComp.terrainData.splatPrototypes.Length > this.numLayers)
		{
			Texture2D[] array = new Texture2D[terrainComp.terrainData.splatPrototypes.Length];
			for (int i = 0; i < this.splats.Length; i++)
			{
				array[i] = this.splats[i];
			}
			this.splats = array;
			this.splats[terrainComp.terrainData.splatPrototypes.Length - 1] = terrainComp.terrainData.splatPrototypes[(terrainComp.terrainData.splatPrototypes.Length - 2 < 0) ? 0 : (terrainComp.terrainData.splatPrototypes.Length - 2)].texture;
		}
		else if (terrainComp.terrainData.splatPrototypes.Length < this.numLayers)
		{
			Texture2D[] array2 = new Texture2D[terrainComp.terrainData.splatPrototypes.Length];
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j] = this.splats[j];
			}
			this.splats = array2;
		}
		this.numLayers = terrainComp.terrainData.splatPrototypes.Length;
	}

	public void SetShaderParam(string name, Texture2D tex)
	{
		if (!tex)
		{
			return;
		}
		if (this.use_mat)
		{
			this.use_mat.SetTexture(name, tex);
		}
		else
		{
			Shader.SetGlobalTexture(name, tex);
		}
	}

	public void SetShaderParam(string name, Cubemap tex)
	{
		if (!tex)
		{
			return;
		}
		if (this.use_mat)
		{
			this.use_mat.SetTexture(name, tex);
		}
		else
		{
			Shader.SetGlobalTexture(name, tex);
		}
	}

	public void SetShaderParam(string name, Matrix4x4 mtx)
	{
		if (this.use_mat)
		{
			this.use_mat.SetMatrix(name, mtx);
		}
		else
		{
			Shader.SetGlobalMatrix(name, mtx);
		}
	}

	public void SetShaderParam(string name, Vector4 vec)
	{
		if (this.use_mat)
		{
			this.use_mat.SetVector(name, vec);
		}
		else
		{
			Shader.SetGlobalVector(name, vec);
		}
	}

	public void SetShaderParam(string name, float val)
	{
		if (this.use_mat)
		{
			this.use_mat.SetFloat(name, val);
		}
		else
		{
			Shader.SetGlobalFloat(name, val);
		}
	}

	public void SetShaderParam(string name, Color col)
	{
		if (this.use_mat)
		{
			this.use_mat.SetColor(name, col);
		}
		else
		{
			Shader.SetGlobalColor(name, col);
		}
	}

	public RTP_LODmanager Get_RTP_LODmanagerScript()
	{
		return this._RTP_LODmanagerScript;
	}

	public void ApplyGlossBakedTexture(string shaderParamName, int i)
	{
		if (this.gloss_baked == null || this.gloss_baked.Length == 0)
		{
			this.gloss_baked = new RTPGlossBaked[12];
		}
		if (this.splats_glossBaked[i] == null)
		{
			if (this.gloss_baked[i] != null && !this.gloss_baked[i].used_in_atlas && this.gloss_baked[i].CheckSize(this.splats[i]))
			{
				this.splats_glossBaked[i] = this.gloss_baked[i].MakeTexture(this.splats[i]);
				this.SetShaderParam(shaderParamName, this.splats_glossBaked[i]);
			}
			else
			{
				this.SetShaderParam(shaderParamName, this.splats[i]);
			}
		}
		else
		{
			this.SetShaderParam(shaderParamName, this.splats_glossBaked[i]);
		}
	}

	public void ApplyGlossBakedAtlas(string shaderParamName, int atlasNum)
	{
		if (this.gloss_baked == null || this.gloss_baked.Length == 0)
		{
			this.gloss_baked = new RTPGlossBaked[12];
		}
		if (this.atlas_glossBaked[atlasNum] == null)
		{
			if (this.splat_atlases[atlasNum] == null)
			{
				return;
			}
			bool flag = false;
			for (int i = 0; i < 4; i++)
			{
				int num = atlasNum * 4 + i;
				if (this.gloss_baked[num] != null && this.gloss_baked[num].used_in_atlas && this.gloss_baked[num].CheckSize(this.splats[num]))
				{
					flag = true;
				}
			}
			if (flag)
			{
				RTPGlossBaked[] array = new RTPGlossBaked[4];
				for (int j = 0; j < 4; j++)
				{
					int num2 = atlasNum * 4 + j;
					if (this.gloss_baked[num2] != null && this.gloss_baked[num2].used_in_atlas && this.gloss_baked[num2].CheckSize(this.splats[num2]))
					{
						array[j] = this.gloss_baked[num2];
					}
					else
					{
						array[j] = (ScriptableObject.CreateInstance(typeof(RTPGlossBaked)) as RTPGlossBaked);
						array[j].Init(this.splats[num2].width);
						array[j].GetMIPGlossMapsFromAtlas(this.splat_atlases[atlasNum], j);
						array[j].used_in_atlas = true;
					}
				}
				this.atlas_glossBaked[atlasNum] = RTPGlossBaked.MakeTexture(this.splat_atlases[atlasNum], array);
				this.SetShaderParam(shaderParamName, this.atlas_glossBaked[atlasNum]);
			}
			else
			{
				this.SetShaderParam(shaderParamName, this.splat_atlases[atlasNum]);
			}
		}
		else
		{
			this.SetShaderParam(shaderParamName, this.atlas_glossBaked[atlasNum]);
		}
	}

	private void CheckLightScriptForDefered()
	{
		Light[] array = UnityEngine.Object.FindObjectsOfType<Light>();
		Light light = null;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].type == LightType.Directional)
			{
				if (!(array[i].gameObject.GetComponent<ReliefShaders_applyLightForDeferred>() == null))
				{
					return;
				}
				light = array[i];
			}
		}
		if (light)
		{
			ReliefShaders_applyLightForDeferred reliefShaders_applyLightForDeferred = light.gameObject.AddComponent(typeof(ReliefShaders_applyLightForDeferred)) as ReliefShaders_applyLightForDeferred;
			reliefShaders_applyLightForDeferred.lightForSelfShadowing = light;
		}
	}

	public void RefreshAll()
	{
		this.CheckLightScriptForDefered();
		ReliefTerrain[] array = UnityEngine.Object.FindObjectsOfType(typeof(ReliefTerrain)) as ReliefTerrain[];
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].globalSettingsHolder != null)
			{
				Terrain terrain = array[i].GetComponent(typeof(Terrain)) as Terrain;
				if (terrain)
				{
					array[i].globalSettingsHolder.Refresh(terrain.materialTemplate, null);
				}
				else
				{
					array[i].globalSettingsHolder.Refresh(array[i].GetComponent<Renderer>().sharedMaterial, null);
				}
				array[i].RefreshTextures(null, false);
			}
		}
	}

	public void Refresh(Material mat = null, ReliefTerrain rt_caller = null)
	{
		if (this.splats == null)
		{
			return;
		}
		if (mat == null && rt_caller != null && rt_caller.globalSettingsHolder == this)
		{
			Terrain terrain = rt_caller.GetComponent(typeof(Terrain)) as Terrain;
			if (terrain)
			{
				rt_caller.globalSettingsHolder.Refresh(terrain.materialTemplate, null);
			}
			else if (rt_caller.GetComponent<Renderer>() != null && rt_caller.GetComponent<Renderer>().sharedMaterial != null)
			{
				rt_caller.globalSettingsHolder.Refresh(rt_caller.GetComponent<Renderer>().sharedMaterial, null);
			}
		}
		this.use_mat = mat;
		for (int i = 0; i < this.numLayers; i++)
		{
			if (i < 4)
			{
				this.ApplyGlossBakedTexture("_SplatA" + i, i);
			}
			else if (i < 8)
			{
				if (this._4LAYERS_SHADER_USED)
				{
					this.ApplyGlossBakedTexture("_SplatC" + (i - 4), i);
					this.ApplyGlossBakedTexture("_SplatB" + (i - 4), i);
				}
				else
				{
					this.ApplyGlossBakedTexture("_SplatB" + (i - 4), i);
				}
			}
			else if (i < 12)
			{
				this.ApplyGlossBakedTexture("_SplatC" + (i - 8), i);
			}
		}
		if (this.CheckAndUpdate(ref this.RTP_gloss2mask, 0.5f, this.numLayers))
		{
			for (int j = 0; j < this.numLayers; j++)
			{
				this.Spec[j] = 1f;
			}
		}
		this.CheckAndUpdate(ref this.RTP_gloss_mult, 1f, this.numLayers);
		this.CheckAndUpdate(ref this.RTP_gloss_shaping, 0.5f, this.numLayers);
		this.CheckAndUpdate(ref this.RTP_Fresnel, 0f, this.numLayers);
		this.CheckAndUpdate(ref this.RTP_FresnelAtten, 0f, this.numLayers);
		this.CheckAndUpdate(ref this.RTP_DiffFresnel, 0f, this.numLayers);
		this.CheckAndUpdate(ref this.RTP_IBL_bump_smoothness, 0.7f, this.numLayers);
		this.CheckAndUpdate(ref this.RTP_IBL_DiffuseStrength, 0.5f, this.numLayers);
		this.CheckAndUpdate(ref this.RTP_IBL_SpecStrength, 0.5f, this.numLayers);
		this.CheckAndUpdate(ref this._DeferredSpecDampAddPass, 1f, this.numLayers);
		this.CheckAndUpdate(ref this.TERRAIN_WaterSpecularity, 0.5f, this.numLayers);
		this.CheckAndUpdate(ref this.TERRAIN_WaterGloss, 0.1f, this.numLayers);
		this.CheckAndUpdate(ref this.TERRAIN_WaterGlossDamper, 0f, this.numLayers);
		this.CheckAndUpdate(ref this.TERRAIN_WetSpecularity, 0.2f, this.numLayers);
		this.CheckAndUpdate(ref this.TERRAIN_WetGloss, 0.05f, this.numLayers);
		this.CheckAndUpdate(ref this.TERRAIN_WetFlow, 0.05f, this.numLayers);
		this.CheckAndUpdate(ref this.MixBrightness, 2f, this.numLayers);
		this.CheckAndUpdate(ref this.MixReplace, 0f, this.numLayers);
		this.CheckAndUpdate(ref this.LayerBrightness, 1f, this.numLayers);
		this.CheckAndUpdate(ref this.LayerBrightness2Spec, 0f, this.numLayers);
		this.CheckAndUpdate(ref this.LayerAlbedo2SpecColor, 0f, this.numLayers);
		this.CheckAndUpdate(ref this.LayerSaturation, 1f, this.numLayers);
		this.CheckAndUpdate(ref this.LayerEmission, 0f, this.numLayers);
		this.CheckAndUpdate(ref this.FarSpecCorrection, 0f, this.numLayers);
		this.CheckAndUpdate(ref this.LayerEmissionColor, Color.black, this.numLayers);
		this.CheckAndUpdate(ref this.LayerEmissionRefractStrength, 0f, this.numLayers);
		this.CheckAndUpdate(ref this.LayerEmissionRefractHBedge, 0f, this.numLayers);
		this.CheckAndUpdate(ref this.TERRAIN_WaterIBL_SpecWetStrength, 0.1f, this.numLayers);
		this.CheckAndUpdate(ref this.TERRAIN_WaterIBL_SpecWaterStrength, 0.5f, this.numLayers);
		this.CheckAndUpdate(ref this.TERRAIN_WaterEmission, 0f, this.numLayers);
		if (RenderSettings.fog)
		{
			Shader.SetGlobalFloat("_Fdensity", RenderSettings.fogDensity);
			if (this.colorSpaceLinear)
			{
				Shader.SetGlobalColor("_FColor", RenderSettings.fogColor.linear);
			}
			else
			{
				Shader.SetGlobalColor("_FColor", RenderSettings.fogColor);
			}
			Shader.SetGlobalFloat("_Fstart", RenderSettings.fogStartDistance);
			Shader.SetGlobalFloat("_Fend", RenderSettings.fogEndDistance);
		}
		else
		{
			Shader.SetGlobalFloat("_Fdensity", 0f);
			Shader.SetGlobalFloat("_Fstart", 1000000f);
			Shader.SetGlobalFloat("_Fend", 2000000f);
		}
		this.SetShaderParam("terrainTileSize", this.terrainTileSize);
		this.SetShaderParam("RTP_AOamp", this.RTP_AOamp);
		this.SetShaderParam("RTP_AOsharpness", this.RTP_AOsharpness);
		this.SetShaderParam("EmissionRefractFiltering", this.EmissionRefractFiltering);
		this.SetShaderParam("EmissionRefractAnimSpeed", this.EmissionRefractAnimSpeed);
		this.SetShaderParam("_VerticalTexture", this.VerticalTexture);
		this.SetShaderParam("_GlobalColorMapBlendValues", this.GlobalColorMapBlendValues);
		this.SetShaderParam("_GlobalColorMapSaturation", this.GlobalColorMapSaturation);
		this.SetShaderParam("_GlobalColorMapSaturationFar", this.GlobalColorMapSaturationFar);
		this.SetShaderParam("_GlobalColorMapDistortByPerlin", this.GlobalColorMapDistortByPerlin);
		this.SetShaderParam("_GlobalColorMapBrightness", this.GlobalColorMapBrightness);
		this.SetShaderParam("_GlobalColorMapBrightnessFar", this.GlobalColorMapBrightnessFar);
		this.SetShaderParam("_GlobalColorMapNearMIP", this._GlobalColorMapNearMIP);
		this.SetShaderParam("_RTP_MIP_BIAS", this.RTP_MIP_BIAS);
		this.SetShaderParam("_BumpMapGlobalScale", this.BumpMapGlobalScale);
		this.SetShaderParam("_FarNormalDamp", this._FarNormalDamp);
		this.SetShaderParam("_SpecColor", this._SpecColor);
		this.SetShaderParam("RTP_DeferredAddPassSpec", this.RTP_DeferredAddPassSpec);
		this.SetShaderParam("_blend_multiplier", this.blendMultiplier);
		this.SetShaderParam("_TERRAIN_ReliefTransform", this.ReliefTransform);
		this.SetShaderParam("_TERRAIN_ReliefTransformTriplanarZ", this.ReliefTransform.x);
		this.SetShaderParam("_TERRAIN_DIST_STEPS", this.DIST_STEPS);
		this.SetShaderParam("_TERRAIN_WAVELENGTH", this.WAVELENGTH);
		this.SetShaderParam("_TERRAIN_ExtrudeHeight", this.ExtrudeHeight);
		this.SetShaderParam("_TERRAIN_LightmapShading", this.LightmapShading);
		this.SetShaderParam("_TERRAIN_SHADOW_STEPS", this.SHADOW_STEPS);
		this.SetShaderParam("_TERRAIN_WAVELENGTH_SHADOWS", this.WAVELENGTH_SHADOWS);
		this.SetShaderParam("_TERRAIN_SelfShadowStrength", this.SelfShadowStrength);
		this.SetShaderParam("_TERRAIN_ShadowSmoothing", (1f - this.ShadowSmoothing) * 6f);
		this.SetShaderParam("_TERRAIN_ShadowSoftnessFade", this.ShadowSoftnessFade);
		this.SetShaderParam("_TERRAIN_distance_start", this.distance_start);
		this.SetShaderParam("_TERRAIN_distance_transition", this.distance_transition);
		this.SetShaderParam("_TERRAIN_distance_start_bumpglobal", this.distance_start_bumpglobal);
		this.SetShaderParam("_TERRAIN_distance_transition_bumpglobal", this.distance_transition_bumpglobal);
		this.SetShaderParam("rtp_perlin_start_val", this.rtp_perlin_start_val);
		Shader.SetGlobalVector("_TERRAIN_trees_shadow_values", new Vector4(this.trees_shadow_distance_start, this.trees_shadow_distance_transition, this.trees_shadow_value, this.global_normalMap_multiplier));
		Shader.SetGlobalVector("_TERRAIN_trees_pixel_values", new Vector4(this.trees_pixel_distance_start, this.trees_pixel_distance_transition, this.trees_pixel_blend_val, this.global_normalMap_farUsage));
		this.SetShaderParam("_Phong", this._Phong);
		this.SetShaderParam("_TessSubdivisions", this._TessSubdivisions);
		this.SetShaderParam("_TessSubdivisionsFar", this._TessSubdivisionsFar);
		this.SetShaderParam("_TessYOffset", this._TessYOffset);
		Shader.SetGlobalFloat("_AmbientEmissiveMultiplier", this._AmbientEmissiveMultiplier);
		Shader.SetGlobalFloat("_AmbientEmissiveRelief", this._AmbientEmissiveRelief);
		this.SetShaderParam("_SuperDetailTiling", this._SuperDetailTiling);
		Shader.SetGlobalFloat("rtp_snow_strength", this._snow_strength);
		Shader.SetGlobalFloat("rtp_global_color_brightness_to_snow", this._global_color_brightness_to_snow);
		Shader.SetGlobalFloat("rtp_snow_slope_factor", this._snow_slope_factor);
		Shader.SetGlobalFloat("rtp_snow_edge_definition", this._snow_edge_definition);
		Shader.SetGlobalFloat("rtp_snow_height_treshold", this._snow_height_treshold);
		Shader.SetGlobalFloat("rtp_snow_height_transition", this._snow_height_transition);
		Shader.SetGlobalColor("rtp_snow_color", this._snow_color);
		Shader.SetGlobalFloat("rtp_snow_specular", this._snow_specular);
		Shader.SetGlobalFloat("rtp_snow_gloss", this._snow_gloss);
		Shader.SetGlobalFloat("rtp_snow_reflectivness", this._snow_reflectivness);
		Shader.SetGlobalFloat("rtp_snow_deep_factor", this._snow_deep_factor);
		Shader.SetGlobalFloat("rtp_snow_fresnel", this._snow_fresnel);
		Shader.SetGlobalFloat("rtp_snow_diff_fresnel", this._snow_diff_fresnel);
		Shader.SetGlobalFloat("rtp_snow_IBL_DiffuseStrength", this._snow_IBL_DiffuseStrength);
		Shader.SetGlobalFloat("rtp_snow_IBL_SpecStrength", this._snow_IBL_SpecStrength);
		this.SetShaderParam("TERRAIN_CausticsAnimSpeed", this.TERRAIN_CausticsAnimSpeed);
		this.SetShaderParam("TERRAIN_CausticsColor", this.TERRAIN_CausticsColor);
		if (this.TERRAIN_CausticsWaterLevelRefObject)
		{
			this.TERRAIN_CausticsWaterLevel = this.TERRAIN_CausticsWaterLevelRefObject.transform.position.y;
		}
		Shader.SetGlobalFloat("TERRAIN_CausticsWaterLevel", this.TERRAIN_CausticsWaterLevel);
		Shader.SetGlobalFloat("TERRAIN_CausticsWaterLevelByAngle", this.TERRAIN_CausticsWaterLevelByAngle);
		Shader.SetGlobalFloat("TERRAIN_CausticsWaterDeepFadeLength", this.TERRAIN_CausticsWaterDeepFadeLength);
		Shader.SetGlobalFloat("TERRAIN_CausticsWaterShallowFadeLength", this.TERRAIN_CausticsWaterShallowFadeLength);
		this.SetShaderParam("TERRAIN_CausticsTilingScale", this.TERRAIN_CausticsTilingScale);
		this.SetShaderParam("TERRAIN_CausticsTex", this.TERRAIN_CausticsTex);
		if (this.numLayers > 0)
		{
			int num = 512;
			for (int k = 0; k < this.numLayers; k++)
			{
				if (this.splats[k])
				{
					num = this.splats[k].width;
					break;
				}
			}
			this.SetShaderParam("rtp_mipoffset_color", -Mathf.Log(1024f / (float)num) / Mathf.Log(2f));
			if (this.Bump01 != null)
			{
				num = this.Bump01.width;
			}
			this.SetShaderParam("rtp_mipoffset_bump", -Mathf.Log(1024f / (float)num) / Mathf.Log(2f));
			if (this.HeightMap)
			{
				num = this.HeightMap.width;
			}
			else if (this.HeightMap2)
			{
				num = this.HeightMap2.width;
			}
			else if (this.HeightMap3)
			{
				num = this.HeightMap3.width;
			}
			this.SetShaderParam("rtp_mipoffset_height", -Mathf.Log(1024f / (float)num) / Mathf.Log(2f));
			num = this.BumpGlobalCombinedSize;
			this.SetShaderParam("rtp_mipoffset_globalnorm", -Mathf.Log(1024f / ((float)num * this.BumpMapGlobalScale)) / Mathf.Log(2f) + (float)this.rtp_mipoffset_globalnorm);
			this.SetShaderParam("rtp_mipoffset_superdetail", -Mathf.Log(1024f / ((float)num * this._SuperDetailTiling)) / Mathf.Log(2f));
			this.SetShaderParam("rtp_mipoffset_flow", -Mathf.Log(1024f / ((float)num * this.TERRAIN_FlowScale)) / Mathf.Log(2f) + this.TERRAIN_FlowMipOffset);
			if (this.TERRAIN_RippleMap)
			{
				num = this.TERRAIN_RippleMap.width;
			}
			this.SetShaderParam("rtp_mipoffset_ripple", -Mathf.Log(1024f / ((float)num * this.TERRAIN_RippleScale)) / Mathf.Log(2f));
			if (this.TERRAIN_CausticsTex)
			{
				num = this.TERRAIN_CausticsTex.width;
			}
			this.SetShaderParam("rtp_mipoffset_caustics", -Mathf.Log(1024f / ((float)num * this.TERRAIN_CausticsTilingScale)) / Mathf.Log(2f));
		}
		this.SetShaderParam("TERRAIN_ReflectionMap", this.TERRAIN_ReflectionMap);
		this.SetShaderParam("TERRAIN_ReflColorA", this.TERRAIN_ReflColorA);
		this.SetShaderParam("TERRAIN_ReflColorB", this.TERRAIN_ReflColorB);
		this.SetShaderParam("TERRAIN_ReflColorC", this.TERRAIN_ReflColorC);
		this.SetShaderParam("TERRAIN_ReflColorCenter", this.TERRAIN_ReflColorCenter);
		this.SetShaderParam("TERRAIN_ReflGlossAttenuation", this.TERRAIN_ReflGlossAttenuation);
		this.SetShaderParam("TERRAIN_ReflectionRotSpeed", this.TERRAIN_ReflectionRotSpeed);
		this.SetShaderParam("TERRAIN_GlobalWetness", this.TERRAIN_GlobalWetness);
		Shader.SetGlobalFloat("TERRAIN_GlobalWetness", this.TERRAIN_GlobalWetness);
		this.SetShaderParam("TERRAIN_RippleMap", this.TERRAIN_RippleMap);
		this.SetShaderParam("TERRAIN_RippleScale", this.TERRAIN_RippleScale);
		this.SetShaderParam("TERRAIN_FlowScale", this.TERRAIN_FlowScale);
		this.SetShaderParam("TERRAIN_FlowMipOffset", this.TERRAIN_FlowMipOffset);
		this.SetShaderParam("TERRAIN_FlowSpeed", this.TERRAIN_FlowSpeed);
		this.SetShaderParam("TERRAIN_FlowCycleScale", this.TERRAIN_FlowCycleScale);
		Shader.SetGlobalFloat("TERRAIN_RainIntensity", this.TERRAIN_RainIntensity);
		this.SetShaderParam("TERRAIN_DropletsSpeed", this.TERRAIN_DropletsSpeed);
		this.SetShaderParam("TERRAIN_WetDropletsStrength", this.TERRAIN_WetDropletsStrength);
		this.SetShaderParam("TERRAIN_WetDarkening", this.TERRAIN_WetDarkening);
		this.SetShaderParam("TERRAIN_mipoffset_flowSpeed", this.TERRAIN_mipoffset_flowSpeed);
		this.SetShaderParam("TERRAIN_WetHeight_Treshold", this.TERRAIN_WetHeight_Treshold);
		this.SetShaderParam("TERRAIN_WetHeight_Transition", this.TERRAIN_WetHeight_Transition);
		Shader.SetGlobalVector("rtp_customAmbientCorrection", new Vector4(this.rtp_customAmbientCorrection.r - 0.2f, this.rtp_customAmbientCorrection.g - 0.2f, this.rtp_customAmbientCorrection.b - 0.2f, 0f) * 0.1f);
		this.SetShaderParam("_CubemapDiff", this._CubemapDiff);
		this.SetShaderParam("_CubemapSpec", this._CubemapSpec);
		Shader.SetGlobalFloat("TERRAIN_IBL_DiffAO_Damp", this.TERRAIN_IBL_DiffAO_Damp);
		Shader.SetGlobalFloat("TERRAIN_IBLRefl_SpecAO_Damp", this.TERRAIN_IBLRefl_SpecAO_Damp);
		Shader.SetGlobalVector("RTP_LightDefVector", this.RTP_LightDefVector);
		Shader.SetGlobalFloat("RTP_BackLightStrength", this.RTP_LightDefVector.x);
		Shader.SetGlobalFloat("RTP_ReflexLightDiffuseSoftness", this.RTP_LightDefVector.y);
		Shader.SetGlobalFloat("RTP_ReflexLightSpecSoftness", this.RTP_LightDefVector.z);
		Shader.SetGlobalFloat("RTP_ReflexLightSpecularity", this.RTP_LightDefVector.w);
		Shader.SetGlobalColor("RTP_ReflexLightDiffuseColor1", this.RTP_ReflexLightDiffuseColor);
		Shader.SetGlobalColor("RTP_ReflexLightDiffuseColor2", this.RTP_ReflexLightDiffuseColor2);
		Shader.SetGlobalColor("RTP_ReflexLightSpecColor", this.RTP_ReflexLightSpecColor);
		this.SetShaderParam("_VerticalTextureGlobalBumpInfluence", this.VerticalTextureGlobalBumpInfluence);
		this.SetShaderParam("_VerticalTextureTiling", this.VerticalTextureTiling);
		float[] array = new float[this.RTP_gloss_mult.Length];
		for (int l = 0; l < array.Length; l++)
		{
			if (this.gloss_baked[l] != null && this.gloss_baked[l].baked)
			{
				array[l] = 1f;
			}
			else
			{
				array[l] = this.RTP_gloss_mult[l];
			}
		}
		float[] array2 = new float[this.RTP_gloss_shaping.Length];
		for (int m = 0; m < array2.Length; m++)
		{
			if (this.gloss_baked[m] != null && this.gloss_baked[m].baked)
			{
				array2[m] = 0.5f;
			}
			else
			{
				array2[m] = this.RTP_gloss_shaping[m];
			}
		}
		this.SetShaderParam("_Spec0123", this.getVector(this.Spec, 0, 3));
		this.SetShaderParam("_FarSpecCorrection0123", this.getVector(this.FarSpecCorrection, 0, 3));
		this.SetShaderParam("_MIPmult0123", this.getVector(this.MIPmult, 0, 3));
		this.SetShaderParam("_MixScale0123", this.getVector(this.MixScale, 0, 3));
		this.SetShaderParam("_MixBlend0123", this.getVector(this.MixBlend, 0, 3));
		this.SetShaderParam("_MixSaturation0123", this.getVector(this.MixSaturation, 0, 3));
		this.SetShaderParam("RTP_gloss2mask0123", this.getVector(this.RTP_gloss2mask, 0, 3));
		this.SetShaderParam("RTP_gloss_mult0123", this.getVector(array, 0, 3));
		this.SetShaderParam("RTP_gloss_shaping0123", this.getVector(array2, 0, 3));
		this.SetShaderParam("RTP_Fresnel0123", this.getVector(this.RTP_Fresnel, 0, 3));
		this.SetShaderParam("RTP_FresnelAtten0123", this.getVector(this.RTP_FresnelAtten, 0, 3));
		this.SetShaderParam("RTP_DiffFresnel0123", this.getVector(this.RTP_DiffFresnel, 0, 3));
		this.SetShaderParam("RTP_IBL_bump_smoothness0123", this.getVector(this.RTP_IBL_bump_smoothness, 0, 3));
		this.SetShaderParam("RTP_IBL_DiffuseStrength0123", this.getVector(this.RTP_IBL_DiffuseStrength, 0, 3));
		this.SetShaderParam("RTP_IBL_SpecStrength0123", this.getVector(this.RTP_IBL_SpecStrength, 0, 3));
		this.SetShaderParam("_MixBrightness0123", this.getVector(this.MixBrightness, 0, 3));
		this.SetShaderParam("_MixReplace0123", this.getVector(this.MixReplace, 0, 3));
		this.SetShaderParam("_LayerBrightness0123", this.MasterLayerBrightness * this.getVector(this.LayerBrightness, 0, 3));
		this.SetShaderParam("_LayerSaturation0123", this.MasterLayerSaturation * this.getVector(this.LayerSaturation, 0, 3));
		this.SetShaderParam("_LayerEmission0123", this.getVector(this.LayerEmission, 0, 3));
		this.SetShaderParam("_LayerEmissionColorR0123", this.getColorVector(this.LayerEmissionColor, 0, 3, 0));
		this.SetShaderParam("_LayerEmissionColorG0123", this.getColorVector(this.LayerEmissionColor, 0, 3, 1));
		this.SetShaderParam("_LayerEmissionColorB0123", this.getColorVector(this.LayerEmissionColor, 0, 3, 2));
		this.SetShaderParam("_LayerEmissionColorA0123", this.getColorVector(this.LayerEmissionColor, 0, 3, 3));
		this.SetShaderParam("_LayerBrightness2Spec0123", this.getVector(this.LayerBrightness2Spec, 0, 3));
		this.SetShaderParam("_LayerAlbedo2SpecColor0123", this.getVector(this.LayerAlbedo2SpecColor, 0, 3));
		this.SetShaderParam("_LayerEmissionRefractStrength0123", this.getVector(this.LayerEmissionRefractStrength, 0, 3));
		this.SetShaderParam("_LayerEmissionRefractHBedge0123", this.getVector(this.LayerEmissionRefractHBedge, 0, 3));
		this.SetShaderParam("_GlobalColorPerLayer0123", this.getVector(this.GlobalColorPerLayer, 0, 3));
		this.SetShaderParam("_GlobalColorBottom0123", this.getVector(this.GlobalColorBottom, 0, 3));
		this.SetShaderParam("_GlobalColorTop0123", this.getVector(this.GlobalColorTop, 0, 3));
		this.SetShaderParam("_GlobalColorColormapLoSat0123", this.getVector(this.GlobalColorColormapLoSat, 0, 3));
		this.SetShaderParam("_GlobalColorColormapHiSat0123", this.getVector(this.GlobalColorColormapHiSat, 0, 3));
		this.SetShaderParam("_GlobalColorLayerLoSat0123", this.getVector(this.GlobalColorLayerLoSat, 0, 3));
		this.SetShaderParam("_GlobalColorLayerHiSat0123", this.getVector(this.GlobalColorLayerHiSat, 0, 3));
		this.SetShaderParam("_GlobalColorLoBlend0123", this.getVector(this.GlobalColorLoBlend, 0, 3));
		this.SetShaderParam("_GlobalColorHiBlend0123", this.getVector(this.GlobalColorHiBlend, 0, 3));
		this.SetShaderParam("PER_LAYER_HEIGHT_MODIFIER0123", this.getVector(this.PER_LAYER_HEIGHT_MODIFIER, 0, 3));
		this.SetShaderParam("rtp_snow_strength_per_layer0123", this.getVector(this._snow_strength_per_layer, 0, 3));
		this.SetShaderParam("_SuperDetailStrengthMultA0123", this.getVector(this._SuperDetailStrengthMultA, 0, 3));
		this.SetShaderParam("_SuperDetailStrengthMultB0123", this.getVector(this._SuperDetailStrengthMultB, 0, 3));
		this.SetShaderParam("_SuperDetailStrengthNormal0123", this.getVector(this._SuperDetailStrengthNormal, 0, 3));
		this.SetShaderParam("_BumpMapGlobalStrength0123", this.getVector(this._BumpMapGlobalStrength, 0, 3));
		this.SetShaderParam("_SuperDetailStrengthMultASelfMaskNear0123", this.getVector(this._SuperDetailStrengthMultASelfMaskNear, 0, 3));
		this.SetShaderParam("_SuperDetailStrengthMultASelfMaskFar0123", this.getVector(this._SuperDetailStrengthMultASelfMaskFar, 0, 3));
		this.SetShaderParam("_SuperDetailStrengthMultBSelfMaskNear0123", this.getVector(this._SuperDetailStrengthMultBSelfMaskNear, 0, 3));
		this.SetShaderParam("_SuperDetailStrengthMultBSelfMaskFar0123", this.getVector(this._SuperDetailStrengthMultBSelfMaskFar, 0, 3));
		this.SetShaderParam("TERRAIN_LayerWetStrength0123", this.getVector(this.TERRAIN_LayerWetStrength, 0, 3));
		this.SetShaderParam("TERRAIN_WaterLevel0123", this.getVector(this.TERRAIN_WaterLevel, 0, 3));
		this.SetShaderParam("TERRAIN_WaterLevelSlopeDamp0123", this.getVector(this.TERRAIN_WaterLevelSlopeDamp, 0, 3));
		this.SetShaderParam("TERRAIN_WaterEdge0123", this.getVector(this.TERRAIN_WaterEdge, 0, 3));
		this.SetShaderParam("TERRAIN_WaterSpecularity0123", this.getVector(this.TERRAIN_WaterSpecularity, 0, 3));
		this.SetShaderParam("TERRAIN_WaterGloss0123", this.getVector(this.TERRAIN_WaterGloss, 0, 3));
		this.SetShaderParam("TERRAIN_WaterGlossDamper0123", this.getVector(this.TERRAIN_WaterGlossDamper, 0, 3));
		this.SetShaderParam("TERRAIN_WaterOpacity0123", this.getVector(this.TERRAIN_WaterOpacity, 0, 3));
		this.SetShaderParam("TERRAIN_Refraction0123", this.getVector(this.TERRAIN_Refraction, 0, 3));
		this.SetShaderParam("TERRAIN_WetRefraction0123", this.getVector(this.TERRAIN_WetRefraction, 0, 3));
		this.SetShaderParam("TERRAIN_Flow0123", this.getVector(this.TERRAIN_Flow, 0, 3));
		this.SetShaderParam("TERRAIN_WetFlow0123", this.getVector(this.TERRAIN_WetFlow, 0, 3));
		this.SetShaderParam("TERRAIN_WetSpecularity0123", this.getVector(this.TERRAIN_WetSpecularity, 0, 3));
		this.SetShaderParam("TERRAIN_WetGloss0123", this.getVector(this.TERRAIN_WetGloss, 0, 3));
		this.SetShaderParam("TERRAIN_WaterColorR0123", this.getColorVector(this.TERRAIN_WaterColor, 0, 3, 0));
		this.SetShaderParam("TERRAIN_WaterColorG0123", this.getColorVector(this.TERRAIN_WaterColor, 0, 3, 1));
		this.SetShaderParam("TERRAIN_WaterColorB0123", this.getColorVector(this.TERRAIN_WaterColor, 0, 3, 2));
		this.SetShaderParam("TERRAIN_WaterColorA0123", this.getColorVector(this.TERRAIN_WaterColor, 0, 3, 3));
		this.SetShaderParam("TERRAIN_WaterIBL_SpecWetStrength0123", this.getVector(this.TERRAIN_WaterIBL_SpecWetStrength, 0, 3));
		this.SetShaderParam("TERRAIN_WaterIBL_SpecWaterStrength0123", this.getVector(this.TERRAIN_WaterIBL_SpecWaterStrength, 0, 3));
		this.SetShaderParam("TERRAIN_WaterEmission0123", this.getVector(this.TERRAIN_WaterEmission, 0, 3));
		this.SetShaderParam("RTP_AO_0123", this.getVector(this.AO_strength, 0, 3));
		this.SetShaderParam("_VerticalTexture0123", this.getVector(this.VerticalTextureStrength, 0, 3));
		if (this.numLayers > 4 && this._4LAYERS_SHADER_USED)
		{
			this.SetShaderParam("_Spec89AB", this.getVector(this.Spec, 4, 7));
			this.SetShaderParam("_FarSpecCorrection89AB", this.getVector(this.FarSpecCorrection, 4, 7));
			this.SetShaderParam("_MIPmult89AB", this.getVector(this.MIPmult, 4, 7));
			this.SetShaderParam("_MixScale89AB", this.getVector(this.MixScale, 4, 7));
			this.SetShaderParam("_MixBlend89AB", this.getVector(this.MixBlend, 4, 7));
			this.SetShaderParam("_MixSaturation89AB", this.getVector(this.MixSaturation, 4, 7));
			this.SetShaderParam("RTP_gloss2mask89AB", this.getVector(this.RTP_gloss2mask, 4, 7));
			this.SetShaderParam("RTP_gloss_mult89AB", this.getVector(array, 4, 7));
			this.SetShaderParam("RTP_gloss_shaping89AB", this.getVector(array2, 4, 7));
			this.SetShaderParam("RTP_Fresnel89AB", this.getVector(this.RTP_Fresnel, 4, 7));
			this.SetShaderParam("RTP_FresnelAtten89AB", this.getVector(this.RTP_FresnelAtten, 4, 7));
			this.SetShaderParam("RTP_DiffFresnel89AB", this.getVector(this.RTP_DiffFresnel, 4, 7));
			this.SetShaderParam("RTP_IBL_bump_smoothness89AB", this.getVector(this.RTP_IBL_bump_smoothness, 4, 7));
			this.SetShaderParam("RTP_IBL_DiffuseStrength89AB", this.getVector(this.RTP_IBL_DiffuseStrength, 4, 7));
			this.SetShaderParam("RTP_IBL_SpecStrength89AB", this.getVector(this.RTP_IBL_SpecStrength, 4, 7));
			this.SetShaderParam("_DeferredSpecDampAddPass89AB", this.getVector(this._DeferredSpecDampAddPass, 4, 7));
			this.SetShaderParam("_MixBrightness89AB", this.getVector(this.MixBrightness, 4, 7));
			this.SetShaderParam("_MixReplace89AB", this.getVector(this.MixReplace, 4, 7));
			this.SetShaderParam("_LayerBrightness89AB", this.MasterLayerBrightness * this.getVector(this.LayerBrightness, 4, 7));
			this.SetShaderParam("_LayerSaturation89AB", this.MasterLayerSaturation * this.getVector(this.LayerSaturation, 4, 7));
			this.SetShaderParam("_LayerEmission89AB", this.getVector(this.LayerEmission, 4, 7));
			this.SetShaderParam("_LayerEmissionColorR89AB", this.getColorVector(this.LayerEmissionColor, 4, 7, 0));
			this.SetShaderParam("_LayerEmissionColorG89AB", this.getColorVector(this.LayerEmissionColor, 4, 7, 1));
			this.SetShaderParam("_LayerEmissionColorB89AB", this.getColorVector(this.LayerEmissionColor, 4, 7, 2));
			this.SetShaderParam("_LayerEmissionColorA89AB", this.getColorVector(this.LayerEmissionColor, 4, 7, 3));
			this.SetShaderParam("_LayerBrightness2Spec89AB", this.getVector(this.LayerBrightness2Spec, 4, 7));
			this.SetShaderParam("_LayerAlbedo2SpecColor89AB", this.getVector(this.LayerAlbedo2SpecColor, 4, 7));
			this.SetShaderParam("_LayerEmissionRefractStrength89AB", this.getVector(this.LayerEmissionRefractStrength, 4, 7));
			this.SetShaderParam("_LayerEmissionRefractHBedge89AB", this.getVector(this.LayerEmissionRefractHBedge, 4, 7));
			this.SetShaderParam("_GlobalColorPerLayer89AB", this.getVector(this.GlobalColorPerLayer, 4, 7));
			this.SetShaderParam("_GlobalColorBottom89AB", this.getVector(this.GlobalColorBottom, 4, 7));
			this.SetShaderParam("_GlobalColorTop89AB", this.getVector(this.GlobalColorTop, 4, 7));
			this.SetShaderParam("_GlobalColorColormapLoSat89AB", this.getVector(this.GlobalColorColormapLoSat, 4, 7));
			this.SetShaderParam("_GlobalColorColormapHiSat89AB", this.getVector(this.GlobalColorColormapHiSat, 4, 7));
			this.SetShaderParam("_GlobalColorLayerLoSat89AB", this.getVector(this.GlobalColorLayerLoSat, 4, 7));
			this.SetShaderParam("_GlobalColorLayerHiSat89AB", this.getVector(this.GlobalColorLayerHiSat, 4, 7));
			this.SetShaderParam("_GlobalColorLoBlend89AB", this.getVector(this.GlobalColorLoBlend, 4, 7));
			this.SetShaderParam("_GlobalColorHiBlend89AB", this.getVector(this.GlobalColorHiBlend, 4, 7));
			this.SetShaderParam("PER_LAYER_HEIGHT_MODIFIER89AB", this.getVector(this.PER_LAYER_HEIGHT_MODIFIER, 4, 7));
			this.SetShaderParam("rtp_snow_strength_per_layer89AB", this.getVector(this._snow_strength_per_layer, 4, 7));
			this.SetShaderParam("_SuperDetailStrengthMultA89AB", this.getVector(this._SuperDetailStrengthMultA, 4, 7));
			this.SetShaderParam("_SuperDetailStrengthMultB89AB", this.getVector(this._SuperDetailStrengthMultB, 4, 7));
			this.SetShaderParam("_SuperDetailStrengthNormal89AB", this.getVector(this._SuperDetailStrengthNormal, 4, 7));
			this.SetShaderParam("_BumpMapGlobalStrength89AB", this.getVector(this._BumpMapGlobalStrength, 4, 7));
			this.SetShaderParam("_SuperDetailStrengthMultASelfMaskNear89AB", this.getVector(this._SuperDetailStrengthMultASelfMaskNear, 4, 7));
			this.SetShaderParam("_SuperDetailStrengthMultASelfMaskFar89AB", this.getVector(this._SuperDetailStrengthMultASelfMaskFar, 4, 7));
			this.SetShaderParam("_SuperDetailStrengthMultBSelfMaskNear89AB", this.getVector(this._SuperDetailStrengthMultBSelfMaskNear, 4, 7));
			this.SetShaderParam("_SuperDetailStrengthMultBSelfMaskFar89AB", this.getVector(this._SuperDetailStrengthMultBSelfMaskFar, 4, 7));
			this.SetShaderParam("TERRAIN_LayerWetStrength89AB", this.getVector(this.TERRAIN_LayerWetStrength, 4, 7));
			this.SetShaderParam("TERRAIN_WaterLevel89AB", this.getVector(this.TERRAIN_WaterLevel, 4, 7));
			this.SetShaderParam("TERRAIN_WaterLevelSlopeDamp89AB", this.getVector(this.TERRAIN_WaterLevelSlopeDamp, 4, 7));
			this.SetShaderParam("TERRAIN_WaterEdge89AB", this.getVector(this.TERRAIN_WaterEdge, 4, 7));
			this.SetShaderParam("TERRAIN_WaterSpecularity89AB", this.getVector(this.TERRAIN_WaterSpecularity, 4, 7));
			this.SetShaderParam("TERRAIN_WaterGloss89AB", this.getVector(this.TERRAIN_WaterGloss, 4, 7));
			this.SetShaderParam("TERRAIN_WaterGlossDamper89AB", this.getVector(this.TERRAIN_WaterGlossDamper, 4, 7));
			this.SetShaderParam("TERRAIN_WaterOpacity89AB", this.getVector(this.TERRAIN_WaterOpacity, 4, 7));
			this.SetShaderParam("TERRAIN_Refraction89AB", this.getVector(this.TERRAIN_Refraction, 4, 7));
			this.SetShaderParam("TERRAIN_WetRefraction89AB", this.getVector(this.TERRAIN_WetRefraction, 4, 7));
			this.SetShaderParam("TERRAIN_Flow89AB", this.getVector(this.TERRAIN_Flow, 4, 7));
			this.SetShaderParam("TERRAIN_WetFlow89AB", this.getVector(this.TERRAIN_WetFlow, 4, 7));
			this.SetShaderParam("TERRAIN_WetSpecularity89AB", this.getVector(this.TERRAIN_WetSpecularity, 4, 7));
			this.SetShaderParam("TERRAIN_WetGloss89AB", this.getVector(this.TERRAIN_WetGloss, 4, 7));
			this.SetShaderParam("TERRAIN_WaterColorR89AB", this.getColorVector(this.TERRAIN_WaterColor, 4, 7, 0));
			this.SetShaderParam("TERRAIN_WaterColorG89AB", this.getColorVector(this.TERRAIN_WaterColor, 4, 7, 1));
			this.SetShaderParam("TERRAIN_WaterColorB89AB", this.getColorVector(this.TERRAIN_WaterColor, 4, 7, 2));
			this.SetShaderParam("TERRAIN_WaterColorA89AB", this.getColorVector(this.TERRAIN_WaterColor, 4, 7, 3));
			this.SetShaderParam("TERRAIN_WaterIBL_SpecWetStrength89AB", this.getVector(this.TERRAIN_WaterIBL_SpecWetStrength, 4, 7));
			this.SetShaderParam("TERRAIN_WaterIBL_SpecWaterStrength89AB", this.getVector(this.TERRAIN_WaterIBL_SpecWaterStrength, 4, 7));
			this.SetShaderParam("TERRAIN_WaterEmission89AB", this.getVector(this.TERRAIN_WaterEmission, 4, 7));
			this.SetShaderParam("RTP_AO_89AB", this.getVector(this.AO_strength, 4, 7));
			this.SetShaderParam("_VerticalTexture89AB", this.getVector(this.VerticalTextureStrength, 4, 7));
		}
		else
		{
			this.SetShaderParam("_Spec4567", this.getVector(this.Spec, 4, 7));
			this.SetShaderParam("_FarSpecCorrection4567", this.getVector(this.FarSpecCorrection, 4, 7));
			this.SetShaderParam("_MIPmult4567", this.getVector(this.MIPmult, 4, 7));
			this.SetShaderParam("_MixScale4567", this.getVector(this.MixScale, 4, 7));
			this.SetShaderParam("_MixBlend4567", this.getVector(this.MixBlend, 4, 7));
			this.SetShaderParam("_MixSaturation4567", this.getVector(this.MixSaturation, 4, 7));
			this.SetShaderParam("RTP_gloss2mask4567", this.getVector(this.RTP_gloss2mask, 4, 7));
			this.SetShaderParam("RTP_gloss_mult4567", this.getVector(array, 4, 7));
			this.SetShaderParam("RTP_gloss_shaping4567", this.getVector(array2, 4, 7));
			this.SetShaderParam("RTP_Fresnel4567", this.getVector(this.RTP_Fresnel, 4, 7));
			this.SetShaderParam("RTP_FresnelAtten4567", this.getVector(this.RTP_FresnelAtten, 4, 7));
			this.SetShaderParam("RTP_DiffFresnel4567", this.getVector(this.RTP_DiffFresnel, 4, 7));
			this.SetShaderParam("RTP_IBL_bump_smoothness4567", this.getVector(this.RTP_IBL_bump_smoothness, 4, 7));
			this.SetShaderParam("RTP_IBL_DiffuseStrength4567", this.getVector(this.RTP_IBL_DiffuseStrength, 4, 7));
			this.SetShaderParam("RTP_IBL_SpecStrength4567", this.getVector(this.RTP_IBL_SpecStrength, 4, 7));
			this.SetShaderParam("_MixBrightness4567", this.getVector(this.MixBrightness, 4, 7));
			this.SetShaderParam("_MixReplace4567", this.getVector(this.MixReplace, 4, 7));
			this.SetShaderParam("_LayerBrightness4567", this.MasterLayerBrightness * this.getVector(this.LayerBrightness, 4, 7));
			this.SetShaderParam("_LayerSaturation4567", this.MasterLayerSaturation * this.getVector(this.LayerSaturation, 4, 7));
			this.SetShaderParam("_LayerEmission4567", this.getVector(this.LayerEmission, 4, 7));
			this.SetShaderParam("_LayerEmissionColorR4567", this.getColorVector(this.LayerEmissionColor, 4, 7, 0));
			this.SetShaderParam("_LayerEmissionColorG4567", this.getColorVector(this.LayerEmissionColor, 4, 7, 1));
			this.SetShaderParam("_LayerEmissionColorB4567", this.getColorVector(this.LayerEmissionColor, 4, 7, 2));
			this.SetShaderParam("_LayerEmissionColorA4567", this.getColorVector(this.LayerEmissionColor, 4, 7, 3));
			this.SetShaderParam("_LayerBrightness2Spec4567", this.getVector(this.LayerBrightness2Spec, 4, 7));
			this.SetShaderParam("_LayerAlbedo2SpecColor4567", this.getVector(this.LayerAlbedo2SpecColor, 4, 7));
			this.SetShaderParam("_LayerEmissionRefractStrength4567", this.getVector(this.LayerEmissionRefractStrength, 4, 7));
			this.SetShaderParam("_LayerEmissionRefractHBedge4567", this.getVector(this.LayerEmissionRefractHBedge, 4, 7));
			this.SetShaderParam("_GlobalColorPerLayer4567", this.getVector(this.GlobalColorPerLayer, 4, 7));
			this.SetShaderParam("_GlobalColorBottom4567", this.getVector(this.GlobalColorBottom, 4, 7));
			this.SetShaderParam("_GlobalColorTop4567", this.getVector(this.GlobalColorTop, 4, 7));
			this.SetShaderParam("_GlobalColorColormapLoSat4567", this.getVector(this.GlobalColorColormapLoSat, 4, 7));
			this.SetShaderParam("_GlobalColorColormapHiSat4567", this.getVector(this.GlobalColorColormapHiSat, 4, 7));
			this.SetShaderParam("_GlobalColorLayerLoSat4567", this.getVector(this.GlobalColorLayerLoSat, 4, 7));
			this.SetShaderParam("_GlobalColorLayerHiSat4567", this.getVector(this.GlobalColorLayerHiSat, 4, 7));
			this.SetShaderParam("_GlobalColorLoBlend4567", this.getVector(this.GlobalColorLoBlend, 4, 7));
			this.SetShaderParam("_GlobalColorHiBlend4567", this.getVector(this.GlobalColorHiBlend, 4, 7));
			this.SetShaderParam("PER_LAYER_HEIGHT_MODIFIER4567", this.getVector(this.PER_LAYER_HEIGHT_MODIFIER, 4, 7));
			this.SetShaderParam("rtp_snow_strength_per_layer4567", this.getVector(this._snow_strength_per_layer, 4, 7));
			this.SetShaderParam("_SuperDetailStrengthMultA4567", this.getVector(this._SuperDetailStrengthMultA, 4, 7));
			this.SetShaderParam("_SuperDetailStrengthMultB4567", this.getVector(this._SuperDetailStrengthMultB, 4, 7));
			this.SetShaderParam("_SuperDetailStrengthNormal4567", this.getVector(this._SuperDetailStrengthNormal, 4, 7));
			this.SetShaderParam("_BumpMapGlobalStrength4567", this.getVector(this._BumpMapGlobalStrength, 4, 7));
			this.SetShaderParam("_SuperDetailStrengthMultASelfMaskNear4567", this.getVector(this._SuperDetailStrengthMultASelfMaskNear, 4, 7));
			this.SetShaderParam("_SuperDetailStrengthMultASelfMaskFar4567", this.getVector(this._SuperDetailStrengthMultASelfMaskFar, 4, 7));
			this.SetShaderParam("_SuperDetailStrengthMultBSelfMaskNear4567", this.getVector(this._SuperDetailStrengthMultBSelfMaskNear, 4, 7));
			this.SetShaderParam("_SuperDetailStrengthMultBSelfMaskFar4567", this.getVector(this._SuperDetailStrengthMultBSelfMaskFar, 4, 7));
			this.SetShaderParam("TERRAIN_LayerWetStrength4567", this.getVector(this.TERRAIN_LayerWetStrength, 4, 7));
			this.SetShaderParam("TERRAIN_WaterLevel4567", this.getVector(this.TERRAIN_WaterLevel, 4, 7));
			this.SetShaderParam("TERRAIN_WaterLevelSlopeDamp4567", this.getVector(this.TERRAIN_WaterLevelSlopeDamp, 4, 7));
			this.SetShaderParam("TERRAIN_WaterEdge4567", this.getVector(this.TERRAIN_WaterEdge, 4, 7));
			this.SetShaderParam("TERRAIN_WaterSpecularity4567", this.getVector(this.TERRAIN_WaterSpecularity, 4, 7));
			this.SetShaderParam("TERRAIN_WaterGloss4567", this.getVector(this.TERRAIN_WaterGloss, 4, 7));
			this.SetShaderParam("TERRAIN_WaterGlossDamper4567", this.getVector(this.TERRAIN_WaterGlossDamper, 4, 7));
			this.SetShaderParam("TERRAIN_WaterOpacity4567", this.getVector(this.TERRAIN_WaterOpacity, 4, 7));
			this.SetShaderParam("TERRAIN_Refraction4567", this.getVector(this.TERRAIN_Refraction, 4, 7));
			this.SetShaderParam("TERRAIN_WetRefraction4567", this.getVector(this.TERRAIN_WetRefraction, 4, 7));
			this.SetShaderParam("TERRAIN_Flow4567", this.getVector(this.TERRAIN_Flow, 4, 7));
			this.SetShaderParam("TERRAIN_WetFlow4567", this.getVector(this.TERRAIN_WetFlow, 4, 7));
			this.SetShaderParam("TERRAIN_WetSpecularity4567", this.getVector(this.TERRAIN_WetSpecularity, 4, 7));
			this.SetShaderParam("TERRAIN_WetGloss4567", this.getVector(this.TERRAIN_WetGloss, 4, 7));
			this.SetShaderParam("TERRAIN_WaterColorR4567", this.getColorVector(this.TERRAIN_WaterColor, 4, 7, 0));
			this.SetShaderParam("TERRAIN_WaterColorG4567", this.getColorVector(this.TERRAIN_WaterColor, 4, 7, 1));
			this.SetShaderParam("TERRAIN_WaterColorB4567", this.getColorVector(this.TERRAIN_WaterColor, 4, 7, 2));
			this.SetShaderParam("TERRAIN_WaterColorA4567", this.getColorVector(this.TERRAIN_WaterColor, 4, 7, 3));
			this.SetShaderParam("TERRAIN_WaterIBL_SpecWetStrength4567", this.getVector(this.TERRAIN_WaterIBL_SpecWetStrength, 4, 7));
			this.SetShaderParam("TERRAIN_WaterIBL_SpecWaterStrength4567", this.getVector(this.TERRAIN_WaterIBL_SpecWaterStrength, 4, 7));
			this.SetShaderParam("TERRAIN_WaterEmission4567", this.getVector(this.TERRAIN_WaterEmission, 4, 7));
			this.SetShaderParam("RTP_AO_4567", this.getVector(this.AO_strength, 4, 7));
			this.SetShaderParam("_VerticalTexture4567", this.getVector(this.VerticalTextureStrength, 4, 7));
			this.SetShaderParam("_Spec89AB", this.getVector(this.Spec, 8, 11));
			this.SetShaderParam("_FarSpecCorrection89AB", this.getVector(this.FarSpecCorrection, 8, 11));
			this.SetShaderParam("_MIPmult89AB", this.getVector(this.MIPmult, 8, 11));
			this.SetShaderParam("_MixScale89AB", this.getVector(this.MixScale, 8, 11));
			this.SetShaderParam("_MixBlend89AB", this.getVector(this.MixBlend, 8, 11));
			this.SetShaderParam("_MixSaturation89AB", this.getVector(this.MixSaturation, 8, 11));
			this.SetShaderParam("RTP_gloss2mask89AB", this.getVector(this.RTP_gloss2mask, 8, 11));
			this.SetShaderParam("RTP_gloss_mult89AB", this.getVector(array, 8, 11));
			this.SetShaderParam("RTP_gloss_shaping89AB", this.getVector(array2, 8, 11));
			this.SetShaderParam("RTP_Fresnel89AB", this.getVector(this.RTP_Fresnel, 8, 11));
			this.SetShaderParam("RTP_FresnelAtten89AB", this.getVector(this.RTP_FresnelAtten, 8, 11));
			this.SetShaderParam("RTP_DiffFresnel89AB", this.getVector(this.RTP_DiffFresnel, 8, 11));
			this.SetShaderParam("RTP_IBL_bump_smoothness89AB", this.getVector(this.RTP_IBL_bump_smoothness, 8, 11));
			this.SetShaderParam("RTP_IBL_DiffuseStrength89AB", this.getVector(this.RTP_IBL_DiffuseStrength, 8, 11));
			this.SetShaderParam("RTP_IBL_SpecStrength89AB", this.getVector(this.RTP_IBL_SpecStrength, 8, 11));
			this.SetShaderParam("_DeferredSpecDampAddPass89AB", this.getVector(this._DeferredSpecDampAddPass, 8, 11));
			this.SetShaderParam("_MixBrightness89AB", this.getVector(this.MixBrightness, 8, 11));
			this.SetShaderParam("_MixReplace89AB", this.getVector(this.MixReplace, 8, 11));
			this.SetShaderParam("_LayerBrightness89AB", this.MasterLayerBrightness * this.getVector(this.LayerBrightness, 8, 11));
			this.SetShaderParam("_LayerSaturation89AB", this.MasterLayerSaturation * this.getVector(this.LayerSaturation, 8, 11));
			this.SetShaderParam("_LayerEmission89AB", this.getVector(this.LayerEmission, 8, 11));
			this.SetShaderParam("_LayerEmissionColorR89AB", this.getColorVector(this.LayerEmissionColor, 8, 11, 0));
			this.SetShaderParam("_LayerEmissionColorG89AB", this.getColorVector(this.LayerEmissionColor, 8, 11, 1));
			this.SetShaderParam("_LayerEmissionColorB89AB", this.getColorVector(this.LayerEmissionColor, 8, 11, 2));
			this.SetShaderParam("_LayerEmissionColorA89AB", this.getColorVector(this.LayerEmissionColor, 8, 11, 3));
			this.SetShaderParam("_LayerBrightness2Spec89AB", this.getVector(this.LayerBrightness2Spec, 8, 11));
			this.SetShaderParam("_LayerAlbedo2SpecColor89AB", this.getVector(this.LayerAlbedo2SpecColor, 8, 11));
			this.SetShaderParam("_LayerEmissionRefractStrength89AB", this.getVector(this.LayerEmissionRefractStrength, 8, 11));
			this.SetShaderParam("_LayerEmissionRefractHBedge89AB", this.getVector(this.LayerEmissionRefractHBedge, 8, 11));
			this.SetShaderParam("_GlobalColorPerLayer89AB", this.getVector(this.GlobalColorPerLayer, 8, 11));
			this.SetShaderParam("_GlobalColorBottom89AB", this.getVector(this.GlobalColorBottom, 8, 11));
			this.SetShaderParam("_GlobalColorTop89AB", this.getVector(this.GlobalColorTop, 8, 11));
			this.SetShaderParam("_GlobalColorColormapLoSat89AB", this.getVector(this.GlobalColorColormapLoSat, 8, 11));
			this.SetShaderParam("_GlobalColorColormapHiSat89AB", this.getVector(this.GlobalColorColormapHiSat, 8, 11));
			this.SetShaderParam("_GlobalColorLayerLoSat89AB", this.getVector(this.GlobalColorLayerLoSat, 8, 11));
			this.SetShaderParam("_GlobalColorLayerHiSat89AB", this.getVector(this.GlobalColorLayerHiSat, 8, 11));
			this.SetShaderParam("_GlobalColorLoBlend89AB", this.getVector(this.GlobalColorLoBlend, 8, 11));
			this.SetShaderParam("_GlobalColorHiBlend89AB", this.getVector(this.GlobalColorHiBlend, 8, 11));
			this.SetShaderParam("PER_LAYER_HEIGHT_MODIFIER89AB", this.getVector(this.PER_LAYER_HEIGHT_MODIFIER, 8, 11));
			this.SetShaderParam("rtp_snow_strength_per_layer89AB", this.getVector(this._snow_strength_per_layer, 8, 11));
			this.SetShaderParam("_SuperDetailStrengthMultA89AB", this.getVector(this._SuperDetailStrengthMultA, 8, 11));
			this.SetShaderParam("_SuperDetailStrengthMultB89AB", this.getVector(this._SuperDetailStrengthMultB, 8, 11));
			this.SetShaderParam("_SuperDetailStrengthNormal89AB", this.getVector(this._SuperDetailStrengthNormal, 8, 11));
			this.SetShaderParam("_BumpMapGlobalStrength89AB", this.getVector(this._BumpMapGlobalStrength, 8, 11));
			this.SetShaderParam("_SuperDetailStrengthMultASelfMaskNear89AB", this.getVector(this._SuperDetailStrengthMultASelfMaskNear, 8, 11));
			this.SetShaderParam("_SuperDetailStrengthMultASelfMaskFar89AB", this.getVector(this._SuperDetailStrengthMultASelfMaskFar, 8, 11));
			this.SetShaderParam("_SuperDetailStrengthMultBSelfMaskNear89AB", this.getVector(this._SuperDetailStrengthMultBSelfMaskNear, 8, 11));
			this.SetShaderParam("_SuperDetailStrengthMultBSelfMaskFar89AB", this.getVector(this._SuperDetailStrengthMultBSelfMaskFar, 8, 11));
			this.SetShaderParam("TERRAIN_LayerWetStrength89AB", this.getVector(this.TERRAIN_LayerWetStrength, 8, 11));
			this.SetShaderParam("TERRAIN_WaterLevel89AB", this.getVector(this.TERRAIN_WaterLevel, 8, 11));
			this.SetShaderParam("TERRAIN_WaterLevelSlopeDamp89AB", this.getVector(this.TERRAIN_WaterLevelSlopeDamp, 8, 11));
			this.SetShaderParam("TERRAIN_WaterEdge89AB", this.getVector(this.TERRAIN_WaterEdge, 8, 11));
			this.SetShaderParam("TERRAIN_WaterSpecularity89AB", this.getVector(this.TERRAIN_WaterSpecularity, 8, 11));
			this.SetShaderParam("TERRAIN_WaterGloss89AB", this.getVector(this.TERRAIN_WaterGloss, 8, 11));
			this.SetShaderParam("TERRAIN_WaterGlossDamper89AB", this.getVector(this.TERRAIN_WaterGlossDamper, 8, 11));
			this.SetShaderParam("TERRAIN_WaterOpacity89AB", this.getVector(this.TERRAIN_WaterOpacity, 8, 11));
			this.SetShaderParam("TERRAIN_Refraction89AB", this.getVector(this.TERRAIN_Refraction, 8, 11));
			this.SetShaderParam("TERRAIN_WetRefraction89AB", this.getVector(this.TERRAIN_WetRefraction, 8, 11));
			this.SetShaderParam("TERRAIN_Flow89AB", this.getVector(this.TERRAIN_Flow, 8, 11));
			this.SetShaderParam("TERRAIN_WetFlow89AB", this.getVector(this.TERRAIN_WetFlow, 8, 11));
			this.SetShaderParam("TERRAIN_WetSpecularity89AB", this.getVector(this.TERRAIN_WetSpecularity, 8, 11));
			this.SetShaderParam("TERRAIN_WetGloss89AB", this.getVector(this.TERRAIN_WetGloss, 8, 11));
			this.SetShaderParam("TERRAIN_WaterColorR89AB", this.getColorVector(this.TERRAIN_WaterColor, 8, 11, 0));
			this.SetShaderParam("TERRAIN_WaterColorG89AB", this.getColorVector(this.TERRAIN_WaterColor, 8, 11, 1));
			this.SetShaderParam("TERRAIN_WaterColorB89AB", this.getColorVector(this.TERRAIN_WaterColor, 8, 11, 2));
			this.SetShaderParam("TERRAIN_WaterColorA89AB", this.getColorVector(this.TERRAIN_WaterColor, 8, 11, 3));
			this.SetShaderParam("TERRAIN_WaterIBL_SpecWetStrength89AB", this.getVector(this.TERRAIN_WaterIBL_SpecWetStrength, 8, 11));
			this.SetShaderParam("TERRAIN_WaterIBL_SpecWaterStrength89AB", this.getVector(this.TERRAIN_WaterIBL_SpecWaterStrength, 8, 11));
			this.SetShaderParam("TERRAIN_WaterEmission89AB", this.getVector(this.TERRAIN_WaterEmission, 8, 11));
			this.SetShaderParam("RTP_AO_89AB", this.getVector(this.AO_strength, 8, 11));
			this.SetShaderParam("_VerticalTexture89AB", this.getVector(this.VerticalTextureStrength, 8, 11));
		}
		if (this.splat_atlases.Length == 2)
		{
			Texture2D texture2D = this.splat_atlases[0];
			Texture2D texture2D2 = this.splat_atlases[1];
			this.splat_atlases = new Texture2D[3];
			this.splat_atlases[0] = texture2D;
			this.splat_atlases[1] = texture2D2;
		}
		this.ApplyGlossBakedAtlas("_SplatAtlasA", 0);
		this.SetShaderParam("_BumpMap01", this.Bump01);
		this.SetShaderParam("_BumpMap23", this.Bump23);
		this.SetShaderParam("_TERRAIN_HeightMap", this.HeightMap);
		this.SetShaderParam("_SSColorCombinedA", this.SSColorCombinedA);
		if (this.numLayers > 4)
		{
			this.ApplyGlossBakedAtlas("_SplatAtlasB", 1);
			this.ApplyGlossBakedAtlas("_SplatAtlasC", 1);
			this.SetShaderParam("_TERRAIN_HeightMap2", this.HeightMap2);
			this.SetShaderParam("_SSColorCombinedB", this.SSColorCombinedB);
		}
		if (this.numLayers > 8)
		{
			this.ApplyGlossBakedAtlas("_SplatAtlasC", 2);
		}
		if (this.numLayers > 4 && this._4LAYERS_SHADER_USED)
		{
			this.SetShaderParam("_BumpMap89", this.Bump45);
			this.SetShaderParam("_BumpMapAB", this.Bump67);
			this.SetShaderParam("_TERRAIN_HeightMap3", this.HeightMap2);
			this.SetShaderParam("_BumpMap45", this.Bump45);
			this.SetShaderParam("_BumpMap67", this.Bump67);
		}
		else
		{
			this.SetShaderParam("_BumpMap45", this.Bump45);
			this.SetShaderParam("_BumpMap67", this.Bump67);
			this.SetShaderParam("_BumpMap89", this.Bump89);
			this.SetShaderParam("_BumpMapAB", this.BumpAB);
			this.SetShaderParam("_TERRAIN_HeightMap3", this.HeightMap3);
		}
		this.use_mat = null;
	}

	public Vector4 getVector(float[] vec, int idxA, int idxB)
	{
		if (vec == null)
		{
			return Vector4.zero;
		}
		Vector4 zero = Vector4.zero;
		for (int i = idxA; i <= idxB; i++)
		{
			if (i < vec.Length)
			{
				zero[i - idxA] = vec[i];
			}
		}
		return zero;
	}

	public Vector4 getColorVector(Color[] vec, int idxA, int idxB, int channel)
	{
		if (vec == null)
		{
			return Vector4.zero;
		}
		Vector4 zero = Vector4.zero;
		for (int i = idxA; i <= idxB; i++)
		{
			if (i < vec.Length)
			{
				zero[i - idxA] = vec[i][channel];
			}
		}
		return zero;
	}

	public Texture2D get_dumb_tex()
	{
		if (!this.dumb_tex)
		{
			this.dumb_tex = new Texture2D(32, 32, TextureFormat.RGB24, false);
			Color[] pixels = this.dumb_tex.GetPixels();
			for (int i = 0; i < pixels.Length; i++)
			{
				pixels[i] = Color.white;
			}
			this.dumb_tex.SetPixels(pixels);
			this.dumb_tex.Apply();
		}
		return this.dumb_tex;
	}

	public void SyncGlobalPropsAcrossTerrainGroups()
	{
		ReliefTerrain[] array = (ReliefTerrain[])UnityEngine.Object.FindObjectsOfType(typeof(ReliefTerrain));
		ReliefTerrainGlobalSettingsHolder[] array2 = new ReliefTerrainGlobalSettingsHolder[array.Length];
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			bool flag = false;
			for (int j = 0; j < num; j++)
			{
				if (array2[j] == array[i].globalSettingsHolder)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				array2[num++] = array[i].globalSettingsHolder;
			}
		}
		if (num > 1)
		{
			for (int k = 0; k < num; k++)
			{
				array2[k].useTerrainMaterial = true;
			}
		}
		for (int l = 0; l < num; l++)
		{
			if (array2[l] != this)
			{
				array2[l].trees_shadow_distance_start = this.trees_shadow_distance_start;
				array2[l].trees_shadow_distance_transition = this.trees_shadow_distance_transition;
				array2[l].trees_shadow_value = this.trees_shadow_value;
				array2[l].global_normalMap_multiplier = this.global_normalMap_multiplier;
				array2[l].trees_pixel_distance_start = this.trees_pixel_distance_start;
				array2[l].trees_pixel_distance_transition = this.trees_pixel_distance_transition;
				array2[l].trees_pixel_blend_val = this.trees_pixel_blend_val;
				array2[l].global_normalMap_farUsage = this.global_normalMap_farUsage;
				array2[l]._AmbientEmissiveMultiplier = this._AmbientEmissiveMultiplier;
				array2[l]._AmbientEmissiveRelief = this._AmbientEmissiveRelief;
				array2[l]._snow_strength = this._snow_strength;
				array2[l]._global_color_brightness_to_snow = this._global_color_brightness_to_snow;
				array2[l]._snow_slope_factor = this._snow_slope_factor;
				array2[l]._snow_edge_definition = this._snow_edge_definition;
				array2[l]._snow_height_treshold = this._snow_height_treshold;
				array2[l]._snow_height_transition = this._snow_height_transition;
				array2[l]._snow_color = this._snow_color;
				array2[l]._snow_specular = this._snow_specular;
				array2[l]._snow_gloss = this._snow_gloss;
				array2[l]._snow_reflectivness = this._snow_reflectivness;
				array2[l]._snow_deep_factor = this._snow_deep_factor;
				array2[l]._snow_fresnel = this._snow_fresnel;
				array2[l]._snow_diff_fresnel = this._snow_diff_fresnel;
				array2[l]._snow_IBL_DiffuseStrength = this._snow_IBL_DiffuseStrength;
				array2[l]._snow_IBL_SpecStrength = this._snow_IBL_SpecStrength;
				array2[l].TERRAIN_CausticsWaterLevel = this.TERRAIN_CausticsWaterLevel;
				array2[l].TERRAIN_CausticsWaterLevelByAngle = this.TERRAIN_CausticsWaterLevelByAngle;
				array2[l].TERRAIN_CausticsWaterDeepFadeLength = this.TERRAIN_CausticsWaterDeepFadeLength;
				array2[l].TERRAIN_CausticsWaterShallowFadeLength = this.TERRAIN_CausticsWaterShallowFadeLength;
				array2[l].TERRAIN_GlobalWetness = this.TERRAIN_GlobalWetness;
				array2[l].TERRAIN_RainIntensity = this.TERRAIN_RainIntensity;
				array2[l].rtp_customAmbientCorrection = this.rtp_customAmbientCorrection;
				array2[l].TERRAIN_IBL_DiffAO_Damp = this.TERRAIN_IBL_DiffAO_Damp;
				array2[l].TERRAIN_IBLRefl_SpecAO_Damp = this.TERRAIN_IBLRefl_SpecAO_Damp;
				array2[l].RTP_LightDefVector = this.RTP_LightDefVector;
				array2[l].RTP_LightDefVector = this.RTP_LightDefVector;
				array2[l].RTP_ReflexLightDiffuseColor = this.RTP_ReflexLightDiffuseColor;
				array2[l].RTP_ReflexLightDiffuseColor2 = this.RTP_ReflexLightDiffuseColor2;
				array2[l].RTP_ReflexLightSpecColor = this.RTP_ReflexLightSpecColor;
			}
		}
	}

	public void RestorePreset(ReliefTerrainPresetHolder holder)
	{
		this.numLayers = holder.numLayers;
		this.splats = new Texture2D[holder.splats.Length];
		for (int i = 0; i < holder.splats.Length; i++)
		{
			this.splats[i] = holder.splats[i];
		}
		this.splat_atlases = new Texture2D[3];
		for (int j = 0; j < this.splat_atlases.Length; j++)
		{
			this.splat_atlases[j] = holder.splat_atlases[j];
		}
		this.gloss_baked = holder.gloss_baked;
		this.splats_glossBaked = new Texture2D[12];
		this.atlas_glossBaked = new Texture2D[3];
		this.RTP_MIP_BIAS = holder.RTP_MIP_BIAS;
		this._SpecColor = holder._SpecColor;
		this.RTP_DeferredAddPassSpec = holder.RTP_DeferredAddPassSpec;
		this.MasterLayerBrightness = holder.MasterLayerBrightness;
		this.MasterLayerSaturation = holder.MasterLayerSaturation;
		this.SuperDetailA_channel = holder.SuperDetailA_channel;
		this.SuperDetailB_channel = holder.SuperDetailB_channel;
		this.Bump01 = holder.Bump01;
		this.Bump23 = holder.Bump23;
		this.Bump45 = holder.Bump45;
		this.Bump67 = holder.Bump67;
		this.Bump89 = holder.Bump89;
		this.BumpAB = holder.BumpAB;
		this.SSColorCombinedA = holder.SSColorCombinedA;
		this.SSColorCombinedB = holder.SSColorCombinedB;
		this.BumpGlobal = holder.BumpGlobal;
		this.VerticalTexture = holder.VerticalTexture;
		this.BumpMapGlobalScale = holder.BumpMapGlobalScale;
		this.GlobalColorMapBlendValues = holder.GlobalColorMapBlendValues;
		this.GlobalColorMapSaturation = holder.GlobalColorMapSaturation;
		this.GlobalColorMapSaturationFar = holder.GlobalColorMapSaturationFar;
		this.GlobalColorMapDistortByPerlin = holder.GlobalColorMapDistortByPerlin;
		this.GlobalColorMapBrightness = holder.GlobalColorMapBrightness;
		this.GlobalColorMapBrightnessFar = holder.GlobalColorMapBrightnessFar;
		this._GlobalColorMapNearMIP = holder._GlobalColorMapNearMIP;
		this._FarNormalDamp = holder._FarNormalDamp;
		this.blendMultiplier = holder.blendMultiplier;
		this.HeightMap = holder.HeightMap;
		this.HeightMap2 = holder.HeightMap2;
		this.HeightMap3 = holder.HeightMap3;
		this.ReliefTransform = holder.ReliefTransform;
		this.DIST_STEPS = holder.DIST_STEPS;
		this.WAVELENGTH = holder.WAVELENGTH;
		this.ReliefBorderBlend = holder.ReliefBorderBlend;
		this.ExtrudeHeight = holder.ExtrudeHeight;
		this.LightmapShading = holder.LightmapShading;
		this.SHADOW_STEPS = holder.SHADOW_STEPS;
		this.WAVELENGTH_SHADOWS = holder.WAVELENGTH_SHADOWS;
		this.SelfShadowStrength = holder.SelfShadowStrength;
		this.ShadowSmoothing = holder.ShadowSmoothing;
		this.ShadowSoftnessFade = holder.ShadowSoftnessFade;
		this.distance_start = holder.distance_start;
		this.distance_transition = holder.distance_transition;
		this.distance_start_bumpglobal = holder.distance_start_bumpglobal;
		this.distance_transition_bumpglobal = holder.distance_transition_bumpglobal;
		this.rtp_perlin_start_val = holder.rtp_perlin_start_val;
		this._Phong = holder._Phong;
		this.tessHeight = holder.tessHeight;
		this._TessSubdivisions = holder._TessSubdivisions;
		this._TessSubdivisionsFar = holder._TessSubdivisionsFar;
		this._TessYOffset = holder._TessYOffset;
		this.trees_shadow_distance_start = holder.trees_shadow_distance_start;
		this.trees_shadow_distance_transition = holder.trees_shadow_distance_transition;
		this.trees_shadow_value = holder.trees_shadow_value;
		this.trees_pixel_distance_start = holder.trees_pixel_distance_start;
		this.trees_pixel_distance_transition = holder.trees_pixel_distance_transition;
		this.trees_pixel_blend_val = holder.trees_pixel_blend_val;
		this.global_normalMap_multiplier = holder.global_normalMap_multiplier;
		this.global_normalMap_farUsage = holder.global_normalMap_farUsage;
		this._AmbientEmissiveMultiplier = holder._AmbientEmissiveMultiplier;
		this._AmbientEmissiveRelief = holder._AmbientEmissiveRelief;
		this.rtp_mipoffset_globalnorm = holder.rtp_mipoffset_globalnorm;
		this._SuperDetailTiling = holder._SuperDetailTiling;
		this.SuperDetailA = holder.SuperDetailA;
		this.SuperDetailB = holder.SuperDetailB;
		this.TERRAIN_ReflectionMap = holder.TERRAIN_ReflectionMap;
		this.TERRAIN_ReflectionMap_channel = holder.TERRAIN_ReflectionMap_channel;
		this.TERRAIN_ReflColorA = holder.TERRAIN_ReflColorA;
		this.TERRAIN_ReflColorB = holder.TERRAIN_ReflColorB;
		this.TERRAIN_ReflColorC = holder.TERRAIN_ReflColorC;
		this.TERRAIN_ReflColorCenter = holder.TERRAIN_ReflColorCenter;
		this.TERRAIN_ReflGlossAttenuation = holder.TERRAIN_ReflGlossAttenuation;
		this.TERRAIN_ReflectionRotSpeed = holder.TERRAIN_ReflectionRotSpeed;
		this.TERRAIN_GlobalWetness = holder.TERRAIN_GlobalWetness;
		this.TERRAIN_RippleMap = holder.TERRAIN_RippleMap;
		this.TERRAIN_RippleScale = holder.TERRAIN_RippleScale;
		this.TERRAIN_FlowScale = holder.TERRAIN_FlowScale;
		this.TERRAIN_FlowSpeed = holder.TERRAIN_FlowSpeed;
		this.TERRAIN_FlowCycleScale = holder.TERRAIN_FlowCycleScale;
		this.TERRAIN_FlowMipOffset = holder.TERRAIN_FlowMipOffset;
		this.TERRAIN_WetDarkening = holder.TERRAIN_WetDarkening;
		this.TERRAIN_WetDropletsStrength = holder.TERRAIN_WetDropletsStrength;
		this.TERRAIN_WetHeight_Treshold = holder.TERRAIN_WetHeight_Treshold;
		this.TERRAIN_WetHeight_Transition = holder.TERRAIN_WetHeight_Transition;
		this.TERRAIN_RainIntensity = holder.TERRAIN_RainIntensity;
		this.TERRAIN_DropletsSpeed = holder.TERRAIN_DropletsSpeed;
		this.TERRAIN_mipoffset_flowSpeed = holder.TERRAIN_mipoffset_flowSpeed;
		this.TERRAIN_CausticsAnimSpeed = holder.TERRAIN_CausticsAnimSpeed;
		this.TERRAIN_CausticsColor = holder.TERRAIN_CausticsColor;
		this.TERRAIN_CausticsWaterLevel = holder.TERRAIN_CausticsWaterLevel;
		this.TERRAIN_CausticsWaterLevelByAngle = holder.TERRAIN_CausticsWaterLevelByAngle;
		this.TERRAIN_CausticsWaterDeepFadeLength = holder.TERRAIN_CausticsWaterDeepFadeLength;
		this.TERRAIN_CausticsWaterShallowFadeLength = holder.TERRAIN_CausticsWaterShallowFadeLength;
		this.TERRAIN_CausticsTilingScale = holder.TERRAIN_CausticsTilingScale;
		this.TERRAIN_CausticsTex = holder.TERRAIN_CausticsTex;
		this.rtp_customAmbientCorrection = holder.rtp_customAmbientCorrection;
		this.TERRAIN_IBL_DiffAO_Damp = holder.TERRAIN_IBL_DiffAO_Damp;
		this.TERRAIN_IBLRefl_SpecAO_Damp = holder.TERRAIN_IBLRefl_SpecAO_Damp;
		this._CubemapDiff = holder._CubemapDiff;
		this._CubemapSpec = holder._CubemapSpec;
		this.RTP_AOsharpness = holder.RTP_AOsharpness;
		this.RTP_AOamp = holder.RTP_AOamp;
		this.RTP_LightDefVector = holder.RTP_LightDefVector;
		this.RTP_ReflexLightDiffuseColor = holder.RTP_ReflexLightDiffuseColor;
		this.RTP_ReflexLightDiffuseColor2 = holder.RTP_ReflexLightDiffuseColor2;
		this.RTP_ReflexLightSpecColor = holder.RTP_ReflexLightSpecColor;
		this.EmissionRefractFiltering = holder.EmissionRefractFiltering;
		this.EmissionRefractAnimSpeed = holder.EmissionRefractAnimSpeed;
		this.VerticalTextureGlobalBumpInfluence = holder.VerticalTextureGlobalBumpInfluence;
		this.VerticalTextureTiling = holder.VerticalTextureTiling;
		this._snow_strength = holder._snow_strength;
		this._global_color_brightness_to_snow = holder._global_color_brightness_to_snow;
		this._snow_slope_factor = holder._snow_slope_factor;
		this._snow_edge_definition = holder._snow_edge_definition;
		this._snow_height_treshold = holder._snow_height_treshold;
		this._snow_height_transition = holder._snow_height_transition;
		this._snow_color = holder._snow_color;
		this._snow_specular = holder._snow_specular;
		this._snow_gloss = holder._snow_gloss;
		this._snow_reflectivness = holder._snow_reflectivness;
		this._snow_deep_factor = holder._snow_deep_factor;
		this._snow_fresnel = holder._snow_fresnel;
		this._snow_diff_fresnel = holder._snow_diff_fresnel;
		this._snow_IBL_DiffuseStrength = holder._snow_IBL_DiffuseStrength;
		this._snow_IBL_SpecStrength = holder._snow_IBL_SpecStrength;
		this.Bumps = new Texture2D[holder.Bumps.Length];
		this.Spec = new float[holder.Bumps.Length];
		this.FarSpecCorrection = new float[holder.Bumps.Length];
		this.MixScale = new float[holder.Bumps.Length];
		this.MixBlend = new float[holder.Bumps.Length];
		this.MixSaturation = new float[holder.Bumps.Length];
		this.RTP_gloss2mask = new float[holder.Bumps.Length];
		this.RTP_gloss_mult = new float[holder.Bumps.Length];
		this.RTP_gloss_shaping = new float[holder.Bumps.Length];
		this.RTP_Fresnel = new float[holder.Bumps.Length];
		this.RTP_FresnelAtten = new float[holder.Bumps.Length];
		this.RTP_DiffFresnel = new float[holder.Bumps.Length];
		this.RTP_IBL_bump_smoothness = new float[holder.Bumps.Length];
		this.RTP_IBL_DiffuseStrength = new float[holder.Bumps.Length];
		this.RTP_IBL_SpecStrength = new float[holder.Bumps.Length];
		this._DeferredSpecDampAddPass = new float[holder.Bumps.Length];
		this.MixBrightness = new float[holder.Bumps.Length];
		this.MixReplace = new float[holder.Bumps.Length];
		this.LayerBrightness = new float[holder.Bumps.Length];
		this.LayerBrightness2Spec = new float[holder.Bumps.Length];
		this.LayerAlbedo2SpecColor = new float[holder.Bumps.Length];
		this.LayerSaturation = new float[holder.Bumps.Length];
		this.LayerEmission = new float[holder.Bumps.Length];
		this.LayerEmissionColor = new Color[holder.Bumps.Length];
		this.LayerEmissionRefractStrength = new float[holder.Bumps.Length];
		this.LayerEmissionRefractHBedge = new float[holder.Bumps.Length];
		this.GlobalColorPerLayer = new float[holder.Bumps.Length];
		this.GlobalColorBottom = new float[holder.Bumps.Length];
		this.GlobalColorTop = new float[holder.Bumps.Length];
		this.GlobalColorColormapLoSat = new float[holder.Bumps.Length];
		this.GlobalColorColormapHiSat = new float[holder.Bumps.Length];
		this.GlobalColorLayerLoSat = new float[holder.Bumps.Length];
		this.GlobalColorLayerHiSat = new float[holder.Bumps.Length];
		this.GlobalColorLoBlend = new float[holder.Bumps.Length];
		this.GlobalColorHiBlend = new float[holder.Bumps.Length];
		this.PER_LAYER_HEIGHT_MODIFIER = new float[holder.Bumps.Length];
		this._SuperDetailStrengthMultA = new float[holder.Bumps.Length];
		this._SuperDetailStrengthMultASelfMaskNear = new float[holder.Bumps.Length];
		this._SuperDetailStrengthMultASelfMaskFar = new float[holder.Bumps.Length];
		this._SuperDetailStrengthMultB = new float[holder.Bumps.Length];
		this._SuperDetailStrengthMultBSelfMaskNear = new float[holder.Bumps.Length];
		this._SuperDetailStrengthMultBSelfMaskFar = new float[holder.Bumps.Length];
		this._SuperDetailStrengthNormal = new float[holder.Bumps.Length];
		this._BumpMapGlobalStrength = new float[holder.Bumps.Length];
		this.AO_strength = new float[holder.Bumps.Length];
		this.VerticalTextureStrength = new float[holder.Bumps.Length];
		this.Heights = new Texture2D[holder.Bumps.Length];
		this._snow_strength_per_layer = new float[holder.Bumps.Length];
		this.Substances = new ProceduralMaterial[holder.Bumps.Length];
		this.TERRAIN_LayerWetStrength = new float[holder.Bumps.Length];
		this.TERRAIN_WaterLevel = new float[holder.Bumps.Length];
		this.TERRAIN_WaterLevelSlopeDamp = new float[holder.Bumps.Length];
		this.TERRAIN_WaterEdge = new float[holder.Bumps.Length];
		this.TERRAIN_WaterSpecularity = new float[holder.Bumps.Length];
		this.TERRAIN_WaterGloss = new float[holder.Bumps.Length];
		this.TERRAIN_WaterGlossDamper = new float[holder.Bumps.Length];
		this.TERRAIN_WaterOpacity = new float[holder.Bumps.Length];
		this.TERRAIN_Refraction = new float[holder.Bumps.Length];
		this.TERRAIN_WetRefraction = new float[holder.Bumps.Length];
		this.TERRAIN_Flow = new float[holder.Bumps.Length];
		this.TERRAIN_WetFlow = new float[holder.Bumps.Length];
		this.TERRAIN_WetSpecularity = new float[holder.Bumps.Length];
		this.TERRAIN_WetGloss = new float[holder.Bumps.Length];
		this.TERRAIN_WaterColor = new Color[holder.Bumps.Length];
		this.TERRAIN_WaterIBL_SpecWetStrength = new float[holder.Bumps.Length];
		this.TERRAIN_WaterIBL_SpecWaterStrength = new float[holder.Bumps.Length];
		this.TERRAIN_WaterEmission = new float[holder.Bumps.Length];
		for (int k = 0; k < holder.Bumps.Length; k++)
		{
			this.Bumps[k] = holder.Bumps[k];
			this.Spec[k] = holder.Spec[k];
			this.FarSpecCorrection[k] = holder.FarSpecCorrection[k];
			this.MixScale[k] = holder.MixScale[k];
			this.MixBlend[k] = holder.MixBlend[k];
			this.MixSaturation[k] = holder.MixSaturation[k];
			if (this.CheckAndUpdate(ref holder.RTP_gloss2mask, 0.5f, holder.Bumps.Length))
			{
				for (int l = 0; l < this.numLayers; l++)
				{
					this.Spec[l] = 1f;
				}
			}
			this.CheckAndUpdate(ref holder.RTP_gloss_mult, 1f, holder.Bumps.Length);
			this.CheckAndUpdate(ref holder.RTP_gloss_shaping, 0.5f, holder.Bumps.Length);
			this.CheckAndUpdate(ref holder.RTP_Fresnel, 0f, holder.Bumps.Length);
			this.CheckAndUpdate(ref holder.RTP_FresnelAtten, 0f, holder.Bumps.Length);
			this.CheckAndUpdate(ref holder.RTP_DiffFresnel, 0f, holder.Bumps.Length);
			this.CheckAndUpdate(ref holder.RTP_IBL_bump_smoothness, 0.7f, holder.Bumps.Length);
			this.CheckAndUpdate(ref holder.RTP_IBL_DiffuseStrength, 0.5f, holder.Bumps.Length);
			this.CheckAndUpdate(ref holder.RTP_IBL_SpecStrength, 0.5f, holder.Bumps.Length);
			this.CheckAndUpdate(ref holder._DeferredSpecDampAddPass, 1f, holder.Bumps.Length);
			this.CheckAndUpdate(ref holder.TERRAIN_WaterSpecularity, 0.5f, holder.Bumps.Length);
			this.CheckAndUpdate(ref holder.TERRAIN_WaterGloss, 0.1f, holder.Bumps.Length);
			this.CheckAndUpdate(ref holder.TERRAIN_WaterGlossDamper, 0f, holder.Bumps.Length);
			this.CheckAndUpdate(ref holder.TERRAIN_WetSpecularity, 0.2f, holder.Bumps.Length);
			this.CheckAndUpdate(ref holder.TERRAIN_WetGloss, 0.05f, holder.Bumps.Length);
			this.CheckAndUpdate(ref holder.TERRAIN_WetFlow, 0.05f, holder.Bumps.Length);
			this.CheckAndUpdate(ref holder.MixBrightness, 2f, holder.Bumps.Length);
			this.CheckAndUpdate(ref holder.MixReplace, 0f, holder.Bumps.Length);
			this.CheckAndUpdate(ref holder.LayerBrightness, 1f, holder.Bumps.Length);
			this.CheckAndUpdate(ref holder.LayerBrightness2Spec, 0f, holder.Bumps.Length);
			this.CheckAndUpdate(ref holder.LayerAlbedo2SpecColor, 0f, holder.Bumps.Length);
			this.CheckAndUpdate(ref holder.LayerSaturation, 1f, holder.Bumps.Length);
			this.CheckAndUpdate(ref holder.LayerEmission, 1f, holder.Bumps.Length);
			this.CheckAndUpdate(ref holder.LayerEmissionColor, Color.black, holder.Bumps.Length);
			this.CheckAndUpdate(ref holder.FarSpecCorrection, 0f, holder.Bumps.Length);
			this.CheckAndUpdate(ref holder.LayerEmissionRefractStrength, 0f, holder.Bumps.Length);
			this.CheckAndUpdate(ref holder.LayerEmissionRefractHBedge, 0f, holder.Bumps.Length);
			this.CheckAndUpdate(ref holder.TERRAIN_WaterIBL_SpecWetStrength, 0.1f, holder.Bumps.Length);
			this.CheckAndUpdate(ref holder.TERRAIN_WaterIBL_SpecWaterStrength, 0.5f, holder.Bumps.Length);
			this.CheckAndUpdate(ref holder.TERRAIN_WaterEmission, 0f, holder.Bumps.Length);
			this.RTP_gloss2mask[k] = holder.RTP_gloss2mask[k];
			this.RTP_gloss_mult[k] = holder.RTP_gloss_mult[k];
			this.RTP_gloss_shaping[k] = holder.RTP_gloss_shaping[k];
			this.RTP_Fresnel[k] = holder.RTP_Fresnel[k];
			this.RTP_FresnelAtten[k] = holder.RTP_FresnelAtten[k];
			this.RTP_DiffFresnel[k] = holder.RTP_DiffFresnel[k];
			this.RTP_IBL_bump_smoothness[k] = holder.RTP_IBL_bump_smoothness[k];
			this.RTP_IBL_DiffuseStrength[k] = holder.RTP_IBL_DiffuseStrength[k];
			this.RTP_IBL_SpecStrength[k] = holder.RTP_IBL_SpecStrength[k];
			this._DeferredSpecDampAddPass[k] = holder._DeferredSpecDampAddPass[k];
			this.MixBrightness[k] = holder.MixBrightness[k];
			this.MixReplace[k] = holder.MixReplace[k];
			this.LayerBrightness[k] = holder.LayerBrightness[k];
			this.LayerBrightness2Spec[k] = holder.LayerBrightness2Spec[k];
			this.LayerAlbedo2SpecColor[k] = holder.LayerAlbedo2SpecColor[k];
			this.LayerSaturation[k] = holder.LayerSaturation[k];
			this.LayerEmission[k] = holder.LayerEmission[k];
			this.LayerEmissionColor[k] = holder.LayerEmissionColor[k];
			this.LayerEmissionRefractStrength[k] = holder.LayerEmissionRefractStrength[k];
			this.LayerEmissionRefractHBedge[k] = holder.LayerEmissionRefractHBedge[k];
			this.GlobalColorPerLayer[k] = holder.GlobalColorPerLayer[k];
			this.GlobalColorBottom[k] = holder.GlobalColorBottom[k];
			this.GlobalColorTop[k] = holder.GlobalColorTop[k];
			this.GlobalColorColormapLoSat[k] = holder.GlobalColorColormapLoSat[k];
			this.GlobalColorColormapHiSat[k] = holder.GlobalColorColormapHiSat[k];
			this.GlobalColorLayerLoSat[k] = holder.GlobalColorLayerLoSat[k];
			this.GlobalColorLayerHiSat[k] = holder.GlobalColorLayerHiSat[k];
			this.GlobalColorLoBlend[k] = holder.GlobalColorLoBlend[k];
			this.GlobalColorHiBlend[k] = holder.GlobalColorHiBlend[k];
			this.PER_LAYER_HEIGHT_MODIFIER[k] = holder.PER_LAYER_HEIGHT_MODIFIER[k];
			this._SuperDetailStrengthMultA[k] = holder._SuperDetailStrengthMultA[k];
			this._SuperDetailStrengthMultASelfMaskNear[k] = holder._SuperDetailStrengthMultASelfMaskNear[k];
			this._SuperDetailStrengthMultASelfMaskFar[k] = holder._SuperDetailStrengthMultASelfMaskFar[k];
			this._SuperDetailStrengthMultB[k] = holder._SuperDetailStrengthMultB[k];
			this._SuperDetailStrengthMultBSelfMaskNear[k] = holder._SuperDetailStrengthMultBSelfMaskNear[k];
			this._SuperDetailStrengthMultBSelfMaskFar[k] = holder._SuperDetailStrengthMultBSelfMaskFar[k];
			this._SuperDetailStrengthNormal[k] = holder._SuperDetailStrengthNormal[k];
			this._BumpMapGlobalStrength[k] = holder._BumpMapGlobalStrength[k];
			this.VerticalTextureStrength[k] = holder.VerticalTextureStrength[k];
			this.AO_strength[k] = holder.AO_strength[k];
			this.Heights[k] = holder.Heights[k];
			this._snow_strength_per_layer[k] = holder._snow_strength_per_layer[k];
			this.Substances[k] = holder.Substances[k];
			this.TERRAIN_LayerWetStrength[k] = holder.TERRAIN_LayerWetStrength[k];
			this.TERRAIN_WaterLevel[k] = holder.TERRAIN_WaterLevel[k];
			this.TERRAIN_WaterLevelSlopeDamp[k] = holder.TERRAIN_WaterLevelSlopeDamp[k];
			this.TERRAIN_WaterEdge[k] = holder.TERRAIN_WaterEdge[k];
			this.TERRAIN_WaterSpecularity[k] = holder.TERRAIN_WaterSpecularity[k];
			this.TERRAIN_WaterGloss[k] = holder.TERRAIN_WaterGloss[k];
			this.TERRAIN_WaterGlossDamper[k] = holder.TERRAIN_WaterGlossDamper[k];
			this.TERRAIN_WaterOpacity[k] = holder.TERRAIN_WaterOpacity[k];
			this.TERRAIN_Refraction[k] = holder.TERRAIN_Refraction[k];
			this.TERRAIN_WetRefraction[k] = holder.TERRAIN_WetRefraction[k];
			this.TERRAIN_Flow[k] = holder.TERRAIN_Flow[k];
			this.TERRAIN_WetFlow[k] = holder.TERRAIN_WetFlow[k];
			this.TERRAIN_WetSpecularity[k] = holder.TERRAIN_WetSpecularity[k];
			this.TERRAIN_WetGloss[k] = holder.TERRAIN_WetGloss[k];
			this.TERRAIN_WaterColor[k] = holder.TERRAIN_WaterColor[k];
			this.TERRAIN_WaterIBL_SpecWetStrength[k] = holder.TERRAIN_WaterIBL_SpecWetStrength[k];
			this.TERRAIN_WaterIBL_SpecWaterStrength[k] = holder.TERRAIN_WaterIBL_SpecWaterStrength[k];
			this.TERRAIN_WaterEmission[k] = holder.TERRAIN_WaterEmission[k];
		}
	}

	public void SavePreset(ref ReliefTerrainPresetHolder holder)
	{
		holder.numLayers = this.numLayers;
		holder.splats = new Texture2D[this.splats.Length];
		for (int i = 0; i < holder.splats.Length; i++)
		{
			holder.splats[i] = this.splats[i];
		}
		holder.splat_atlases = new Texture2D[3];
		for (int j = 0; j < this.splat_atlases.Length; j++)
		{
			holder.splat_atlases[j] = this.splat_atlases[j];
		}
		holder.gloss_baked = this.gloss_baked;
		holder.RTP_MIP_BIAS = this.RTP_MIP_BIAS;
		holder._SpecColor = this._SpecColor;
		holder.RTP_DeferredAddPassSpec = this.RTP_DeferredAddPassSpec;
		holder.MasterLayerBrightness = this.MasterLayerBrightness;
		holder.MasterLayerSaturation = this.MasterLayerSaturation;
		holder.SuperDetailA_channel = this.SuperDetailA_channel;
		holder.SuperDetailB_channel = this.SuperDetailB_channel;
		holder.Bump01 = this.Bump01;
		holder.Bump23 = this.Bump23;
		holder.Bump45 = this.Bump45;
		holder.Bump67 = this.Bump67;
		holder.Bump89 = this.Bump89;
		holder.BumpAB = this.BumpAB;
		holder.SSColorCombinedA = this.SSColorCombinedA;
		holder.SSColorCombinedB = this.SSColorCombinedB;
		holder.BumpGlobal = this.BumpGlobal;
		holder.VerticalTexture = this.VerticalTexture;
		holder.BumpMapGlobalScale = this.BumpMapGlobalScale;
		holder.GlobalColorMapBlendValues = this.GlobalColorMapBlendValues;
		holder.GlobalColorMapSaturation = this.GlobalColorMapSaturation;
		holder.GlobalColorMapSaturationFar = this.GlobalColorMapSaturationFar;
		holder.GlobalColorMapDistortByPerlin = this.GlobalColorMapDistortByPerlin;
		holder.GlobalColorMapBrightness = this.GlobalColorMapBrightness;
		holder.GlobalColorMapBrightnessFar = this.GlobalColorMapBrightnessFar;
		holder._GlobalColorMapNearMIP = this._GlobalColorMapNearMIP;
		holder._FarNormalDamp = this._FarNormalDamp;
		holder.blendMultiplier = this.blendMultiplier;
		holder.HeightMap = this.HeightMap;
		holder.HeightMap2 = this.HeightMap2;
		holder.HeightMap3 = this.HeightMap3;
		holder.ReliefTransform = this.ReliefTransform;
		holder.DIST_STEPS = this.DIST_STEPS;
		holder.WAVELENGTH = this.WAVELENGTH;
		holder.ReliefBorderBlend = this.ReliefBorderBlend;
		holder.ExtrudeHeight = this.ExtrudeHeight;
		holder.LightmapShading = this.LightmapShading;
		holder.SHADOW_STEPS = this.SHADOW_STEPS;
		holder.WAVELENGTH_SHADOWS = this.WAVELENGTH_SHADOWS;
		holder.SelfShadowStrength = this.SelfShadowStrength;
		holder.ShadowSmoothing = this.ShadowSmoothing;
		holder.ShadowSoftnessFade = this.ShadowSoftnessFade;
		holder.distance_start = this.distance_start;
		holder.distance_transition = this.distance_transition;
		holder.distance_start_bumpglobal = this.distance_start_bumpglobal;
		holder.distance_transition_bumpglobal = this.distance_transition_bumpglobal;
		holder.rtp_perlin_start_val = this.rtp_perlin_start_val;
		holder._Phong = this._Phong;
		holder.tessHeight = this.tessHeight;
		holder._TessSubdivisions = this._TessSubdivisions;
		holder._TessSubdivisionsFar = this._TessSubdivisionsFar;
		holder._TessYOffset = this._TessYOffset;
		holder.trees_shadow_distance_start = this.trees_shadow_distance_start;
		holder.trees_shadow_distance_transition = this.trees_shadow_distance_transition;
		holder.trees_shadow_value = this.trees_shadow_value;
		holder.trees_pixel_distance_start = this.trees_pixel_distance_start;
		holder.trees_pixel_distance_transition = this.trees_pixel_distance_transition;
		holder.trees_pixel_blend_val = this.trees_pixel_blend_val;
		holder.global_normalMap_multiplier = this.global_normalMap_multiplier;
		holder.global_normalMap_farUsage = this.global_normalMap_farUsage;
		holder._AmbientEmissiveMultiplier = this._AmbientEmissiveMultiplier;
		holder._AmbientEmissiveRelief = this._AmbientEmissiveRelief;
		holder.rtp_mipoffset_globalnorm = this.rtp_mipoffset_globalnorm;
		holder._SuperDetailTiling = this._SuperDetailTiling;
		holder.SuperDetailA = this.SuperDetailA;
		holder.SuperDetailB = this.SuperDetailB;
		holder.TERRAIN_ReflectionMap = this.TERRAIN_ReflectionMap;
		holder.TERRAIN_ReflectionMap_channel = this.TERRAIN_ReflectionMap_channel;
		holder.TERRAIN_ReflColorA = this.TERRAIN_ReflColorA;
		holder.TERRAIN_ReflColorB = this.TERRAIN_ReflColorB;
		holder.TERRAIN_ReflColorC = this.TERRAIN_ReflColorC;
		holder.TERRAIN_ReflColorCenter = this.TERRAIN_ReflColorCenter;
		holder.TERRAIN_ReflGlossAttenuation = this.TERRAIN_ReflGlossAttenuation;
		holder.TERRAIN_ReflectionRotSpeed = this.TERRAIN_ReflectionRotSpeed;
		holder.TERRAIN_GlobalWetness = this.TERRAIN_GlobalWetness;
		holder.TERRAIN_RippleMap = this.TERRAIN_RippleMap;
		holder.TERRAIN_RippleScale = this.TERRAIN_RippleScale;
		holder.TERRAIN_FlowScale = this.TERRAIN_FlowScale;
		holder.TERRAIN_FlowSpeed = this.TERRAIN_FlowSpeed;
		holder.TERRAIN_FlowCycleScale = this.TERRAIN_FlowCycleScale;
		holder.TERRAIN_FlowMipOffset = this.TERRAIN_FlowMipOffset;
		holder.TERRAIN_WetDarkening = this.TERRAIN_WetDarkening;
		holder.TERRAIN_WetDropletsStrength = this.TERRAIN_WetDropletsStrength;
		holder.TERRAIN_WetHeight_Treshold = this.TERRAIN_WetHeight_Treshold;
		holder.TERRAIN_WetHeight_Transition = this.TERRAIN_WetHeight_Transition;
		holder.TERRAIN_RainIntensity = this.TERRAIN_RainIntensity;
		holder.TERRAIN_DropletsSpeed = this.TERRAIN_DropletsSpeed;
		holder.TERRAIN_mipoffset_flowSpeed = this.TERRAIN_mipoffset_flowSpeed;
		holder.TERRAIN_CausticsAnimSpeed = this.TERRAIN_CausticsAnimSpeed;
		holder.TERRAIN_CausticsColor = this.TERRAIN_CausticsColor;
		holder.TERRAIN_CausticsWaterLevel = this.TERRAIN_CausticsWaterLevel;
		holder.TERRAIN_CausticsWaterLevelByAngle = this.TERRAIN_CausticsWaterLevelByAngle;
		holder.TERRAIN_CausticsWaterDeepFadeLength = this.TERRAIN_CausticsWaterDeepFadeLength;
		holder.TERRAIN_CausticsWaterShallowFadeLength = this.TERRAIN_CausticsWaterShallowFadeLength;
		holder.TERRAIN_CausticsTilingScale = this.TERRAIN_CausticsTilingScale;
		holder.TERRAIN_CausticsTex = this.TERRAIN_CausticsTex;
		holder.rtp_customAmbientCorrection = this.rtp_customAmbientCorrection;
		holder.TERRAIN_IBL_DiffAO_Damp = this.TERRAIN_IBL_DiffAO_Damp;
		holder.TERRAIN_IBLRefl_SpecAO_Damp = this.TERRAIN_IBLRefl_SpecAO_Damp;
		holder._CubemapDiff = this._CubemapDiff;
		holder._CubemapSpec = this._CubemapSpec;
		holder.RTP_AOsharpness = this.RTP_AOsharpness;
		holder.RTP_AOamp = this.RTP_AOamp;
		holder.RTP_LightDefVector = this.RTP_LightDefVector;
		holder.RTP_ReflexLightDiffuseColor = this.RTP_ReflexLightDiffuseColor;
		holder.RTP_ReflexLightDiffuseColor2 = this.RTP_ReflexLightDiffuseColor2;
		holder.RTP_ReflexLightSpecColor = this.RTP_ReflexLightSpecColor;
		holder.EmissionRefractFiltering = this.EmissionRefractFiltering;
		holder.EmissionRefractAnimSpeed = this.EmissionRefractAnimSpeed;
		holder.VerticalTextureGlobalBumpInfluence = this.VerticalTextureGlobalBumpInfluence;
		holder.VerticalTextureTiling = this.VerticalTextureTiling;
		holder._snow_strength = this._snow_strength;
		holder._global_color_brightness_to_snow = this._global_color_brightness_to_snow;
		holder._snow_slope_factor = this._snow_slope_factor;
		holder._snow_edge_definition = this._snow_edge_definition;
		holder._snow_height_treshold = this._snow_height_treshold;
		holder._snow_height_transition = this._snow_height_transition;
		holder._snow_color = this._snow_color;
		holder._snow_specular = this._snow_specular;
		holder._snow_gloss = this._snow_gloss;
		holder._snow_reflectivness = this._snow_reflectivness;
		holder._snow_deep_factor = this._snow_deep_factor;
		holder._snow_fresnel = this._snow_fresnel;
		holder._snow_diff_fresnel = this._snow_diff_fresnel;
		holder._snow_IBL_DiffuseStrength = this._snow_IBL_DiffuseStrength;
		holder._snow_IBL_SpecStrength = this._snow_IBL_SpecStrength;
		holder.Bumps = new Texture2D[this.numLayers];
		holder.Spec = new float[this.numLayers];
		holder.FarSpecCorrection = new float[this.numLayers];
		holder.MixScale = new float[this.numLayers];
		holder.MixBlend = new float[this.numLayers];
		holder.MixSaturation = new float[this.numLayers];
		holder.RTP_gloss2mask = new float[this.numLayers];
		holder.RTP_gloss_mult = new float[this.numLayers];
		holder.RTP_gloss_shaping = new float[this.numLayers];
		holder.RTP_Fresnel = new float[this.numLayers];
		holder.RTP_FresnelAtten = new float[this.numLayers];
		holder.RTP_DiffFresnel = new float[this.numLayers];
		holder.RTP_IBL_bump_smoothness = new float[this.numLayers];
		holder.RTP_IBL_DiffuseStrength = new float[this.numLayers];
		holder.RTP_IBL_SpecStrength = new float[this.numLayers];
		holder._DeferredSpecDampAddPass = new float[this.numLayers];
		holder.MixBrightness = new float[this.numLayers];
		holder.MixReplace = new float[this.numLayers];
		holder.LayerBrightness = new float[this.numLayers];
		holder.LayerBrightness2Spec = new float[this.numLayers];
		holder.LayerAlbedo2SpecColor = new float[this.numLayers];
		holder.LayerSaturation = new float[this.numLayers];
		holder.LayerEmission = new float[this.numLayers];
		holder.LayerEmissionColor = new Color[this.numLayers];
		holder.LayerEmissionRefractStrength = new float[this.numLayers];
		holder.LayerEmissionRefractHBedge = new float[this.numLayers];
		holder.GlobalColorPerLayer = new float[this.numLayers];
		holder.GlobalColorBottom = new float[this.numLayers];
		holder.GlobalColorTop = new float[this.numLayers];
		holder.GlobalColorColormapLoSat = new float[this.numLayers];
		holder.GlobalColorColormapHiSat = new float[this.numLayers];
		holder.GlobalColorLayerLoSat = new float[this.numLayers];
		holder.GlobalColorLayerHiSat = new float[this.numLayers];
		holder.GlobalColorLoBlend = new float[this.numLayers];
		holder.GlobalColorHiBlend = new float[this.numLayers];
		holder.PER_LAYER_HEIGHT_MODIFIER = new float[this.numLayers];
		holder._SuperDetailStrengthMultA = new float[this.numLayers];
		holder._SuperDetailStrengthMultASelfMaskNear = new float[this.numLayers];
		holder._SuperDetailStrengthMultASelfMaskFar = new float[this.numLayers];
		holder._SuperDetailStrengthMultB = new float[this.numLayers];
		holder._SuperDetailStrengthMultBSelfMaskNear = new float[this.numLayers];
		holder._SuperDetailStrengthMultBSelfMaskFar = new float[this.numLayers];
		holder._SuperDetailStrengthNormal = new float[this.numLayers];
		holder._BumpMapGlobalStrength = new float[this.numLayers];
		holder.VerticalTextureStrength = new float[this.numLayers];
		holder.AO_strength = new float[this.numLayers];
		holder.Heights = new Texture2D[this.numLayers];
		holder._snow_strength_per_layer = new float[this.numLayers];
		holder.Substances = new ProceduralMaterial[this.numLayers];
		holder.TERRAIN_LayerWetStrength = new float[this.numLayers];
		holder.TERRAIN_WaterLevel = new float[this.numLayers];
		holder.TERRAIN_WaterLevelSlopeDamp = new float[this.numLayers];
		holder.TERRAIN_WaterEdge = new float[this.numLayers];
		holder.TERRAIN_WaterSpecularity = new float[this.numLayers];
		holder.TERRAIN_WaterGloss = new float[this.numLayers];
		holder.TERRAIN_WaterGlossDamper = new float[this.numLayers];
		holder.TERRAIN_WaterOpacity = new float[this.numLayers];
		holder.TERRAIN_Refraction = new float[this.numLayers];
		holder.TERRAIN_WetRefraction = new float[this.numLayers];
		holder.TERRAIN_Flow = new float[this.numLayers];
		holder.TERRAIN_WetFlow = new float[this.numLayers];
		holder.TERRAIN_WetSpecularity = new float[this.numLayers];
		holder.TERRAIN_WetGloss = new float[this.numLayers];
		holder.TERRAIN_WaterColor = new Color[this.numLayers];
		holder.TERRAIN_WaterIBL_SpecWetStrength = new float[this.numLayers];
		holder.TERRAIN_WaterIBL_SpecWaterStrength = new float[this.numLayers];
		holder.TERRAIN_WaterEmission = new float[this.numLayers];
		for (int k = 0; k < this.numLayers; k++)
		{
			holder.Bumps[k] = this.Bumps[k];
			holder.Spec[k] = this.Spec[k];
			holder.FarSpecCorrection[k] = this.FarSpecCorrection[k];
			holder.MixScale[k] = this.MixScale[k];
			holder.MixBlend[k] = this.MixBlend[k];
			holder.MixSaturation[k] = this.MixSaturation[k];
			if (this.CheckAndUpdate(ref this.RTP_gloss2mask, 0.5f, this.numLayers))
			{
				for (int l = 0; l < this.numLayers; l++)
				{
					this.Spec[l] = 1f;
				}
			}
			this.CheckAndUpdate(ref this.RTP_gloss_mult, 1f, this.numLayers);
			this.CheckAndUpdate(ref this.RTP_gloss_shaping, 0.5f, this.numLayers);
			this.CheckAndUpdate(ref this.RTP_Fresnel, 0f, this.numLayers);
			this.CheckAndUpdate(ref this.RTP_FresnelAtten, 0f, this.numLayers);
			this.CheckAndUpdate(ref this.RTP_DiffFresnel, 0f, this.numLayers);
			this.CheckAndUpdate(ref this.RTP_IBL_bump_smoothness, 0.7f, this.numLayers);
			this.CheckAndUpdate(ref this.RTP_IBL_DiffuseStrength, 0.5f, this.numLayers);
			this.CheckAndUpdate(ref this.RTP_IBL_SpecStrength, 0.5f, this.numLayers);
			this.CheckAndUpdate(ref this._DeferredSpecDampAddPass, 1f, this.numLayers);
			this.CheckAndUpdate(ref this.TERRAIN_WaterSpecularity, 0.5f, this.numLayers);
			this.CheckAndUpdate(ref this.TERRAIN_WaterGloss, 0.1f, this.numLayers);
			this.CheckAndUpdate(ref this.TERRAIN_WaterGlossDamper, 0f, this.numLayers);
			this.CheckAndUpdate(ref this.TERRAIN_WetSpecularity, 0.2f, this.numLayers);
			this.CheckAndUpdate(ref this.TERRAIN_WetGloss, 0.05f, this.numLayers);
			this.CheckAndUpdate(ref this.TERRAIN_WetFlow, 0.05f, this.numLayers);
			this.CheckAndUpdate(ref this.MixBrightness, 2f, this.numLayers);
			this.CheckAndUpdate(ref this.MixReplace, 0f, this.numLayers);
			this.CheckAndUpdate(ref this.LayerBrightness, 1f, this.numLayers);
			this.CheckAndUpdate(ref this.LayerBrightness2Spec, 0f, this.numLayers);
			this.CheckAndUpdate(ref this.LayerAlbedo2SpecColor, 0f, this.numLayers);
			this.CheckAndUpdate(ref this.LayerSaturation, 1f, this.numLayers);
			this.CheckAndUpdate(ref this.LayerEmission, 0f, this.numLayers);
			this.CheckAndUpdate(ref this.LayerEmissionColor, Color.black, this.numLayers);
			this.CheckAndUpdate(ref this.LayerEmissionRefractStrength, 0f, this.numLayers);
			this.CheckAndUpdate(ref this.LayerEmissionRefractHBedge, 0f, this.numLayers);
			this.CheckAndUpdate(ref this.TERRAIN_WaterIBL_SpecWetStrength, 0.1f, this.numLayers);
			this.CheckAndUpdate(ref this.TERRAIN_WaterIBL_SpecWaterStrength, 0.5f, this.numLayers);
			this.CheckAndUpdate(ref this.TERRAIN_WaterEmission, 0.5f, this.numLayers);
			holder.RTP_gloss2mask[k] = this.RTP_gloss2mask[k];
			holder.RTP_gloss_mult[k] = this.RTP_gloss_mult[k];
			holder.RTP_gloss_shaping[k] = this.RTP_gloss_shaping[k];
			holder.RTP_Fresnel[k] = this.RTP_Fresnel[k];
			holder.RTP_FresnelAtten[k] = this.RTP_FresnelAtten[k];
			holder.RTP_DiffFresnel[k] = this.RTP_DiffFresnel[k];
			holder.RTP_IBL_bump_smoothness[k] = this.RTP_IBL_bump_smoothness[k];
			holder.RTP_IBL_DiffuseStrength[k] = this.RTP_IBL_DiffuseStrength[k];
			holder.RTP_IBL_SpecStrength[k] = this.RTP_IBL_SpecStrength[k];
			holder._DeferredSpecDampAddPass[k] = this._DeferredSpecDampAddPass[k];
			holder.TERRAIN_WaterIBL_SpecWetStrength[k] = this.TERRAIN_WaterIBL_SpecWetStrength[k];
			holder.TERRAIN_WaterIBL_SpecWaterStrength[k] = this.TERRAIN_WaterIBL_SpecWaterStrength[k];
			holder.TERRAIN_WaterEmission[k] = this.TERRAIN_WaterEmission[k];
			holder.MixBrightness[k] = this.MixBrightness[k];
			holder.MixReplace[k] = this.MixReplace[k];
			holder.LayerBrightness[k] = this.LayerBrightness[k];
			holder.LayerBrightness2Spec[k] = this.LayerBrightness2Spec[k];
			holder.LayerAlbedo2SpecColor[k] = this.LayerAlbedo2SpecColor[k];
			holder.LayerSaturation[k] = this.LayerSaturation[k];
			holder.LayerEmission[k] = this.LayerEmission[k];
			holder.LayerEmissionColor[k] = this.LayerEmissionColor[k];
			holder.LayerEmissionRefractStrength[k] = this.LayerEmissionRefractStrength[k];
			holder.LayerEmissionRefractHBedge[k] = this.LayerEmissionRefractHBedge[k];
			holder.GlobalColorPerLayer[k] = this.GlobalColorPerLayer[k];
			holder.GlobalColorBottom[k] = this.GlobalColorBottom[k];
			holder.GlobalColorTop[k] = this.GlobalColorTop[k];
			holder.GlobalColorColormapLoSat[k] = this.GlobalColorColormapLoSat[k];
			holder.GlobalColorColormapHiSat[k] = this.GlobalColorColormapHiSat[k];
			holder.GlobalColorLayerLoSat[k] = this.GlobalColorLayerLoSat[k];
			holder.GlobalColorLayerHiSat[k] = this.GlobalColorLayerHiSat[k];
			holder.GlobalColorLoBlend[k] = this.GlobalColorLoBlend[k];
			holder.GlobalColorHiBlend[k] = this.GlobalColorHiBlend[k];
			holder.PER_LAYER_HEIGHT_MODIFIER[k] = this.PER_LAYER_HEIGHT_MODIFIER[k];
			holder._SuperDetailStrengthMultA[k] = this._SuperDetailStrengthMultA[k];
			holder._SuperDetailStrengthMultASelfMaskNear[k] = this._SuperDetailStrengthMultASelfMaskNear[k];
			holder._SuperDetailStrengthMultASelfMaskFar[k] = this._SuperDetailStrengthMultASelfMaskFar[k];
			holder._SuperDetailStrengthMultB[k] = this._SuperDetailStrengthMultB[k];
			holder._SuperDetailStrengthMultBSelfMaskNear[k] = this._SuperDetailStrengthMultBSelfMaskNear[k];
			holder._SuperDetailStrengthMultBSelfMaskFar[k] = this._SuperDetailStrengthMultBSelfMaskFar[k];
			holder._SuperDetailStrengthNormal[k] = this._SuperDetailStrengthNormal[k];
			holder._BumpMapGlobalStrength[k] = this._BumpMapGlobalStrength[k];
			holder.VerticalTextureStrength[k] = this.VerticalTextureStrength[k];
			holder.AO_strength[k] = this.AO_strength[k];
			holder.Heights[k] = this.Heights[k];
			holder._snow_strength_per_layer[k] = this._snow_strength_per_layer[k];
			holder.Substances[k] = this.Substances[k];
			holder.TERRAIN_LayerWetStrength[k] = this.TERRAIN_LayerWetStrength[k];
			holder.TERRAIN_WaterLevel[k] = this.TERRAIN_WaterLevel[k];
			holder.TERRAIN_WaterLevelSlopeDamp[k] = this.TERRAIN_WaterLevelSlopeDamp[k];
			holder.TERRAIN_WaterEdge[k] = this.TERRAIN_WaterEdge[k];
			holder.TERRAIN_WaterSpecularity[k] = this.TERRAIN_WaterSpecularity[k];
			holder.TERRAIN_WaterGloss[k] = this.TERRAIN_WaterGloss[k];
			holder.TERRAIN_WaterGlossDamper[k] = this.TERRAIN_WaterGlossDamper[k];
			holder.TERRAIN_WaterOpacity[k] = this.TERRAIN_WaterOpacity[k];
			holder.TERRAIN_Refraction[k] = this.TERRAIN_Refraction[k];
			holder.TERRAIN_WetRefraction[k] = this.TERRAIN_WetRefraction[k];
			holder.TERRAIN_Flow[k] = this.TERRAIN_Flow[k];
			holder.TERRAIN_WetFlow[k] = this.TERRAIN_WetFlow[k];
			holder.TERRAIN_WetSpecularity[k] = this.TERRAIN_WetSpecularity[k];
			holder.TERRAIN_WetGloss[k] = this.TERRAIN_WetGloss[k];
			holder.TERRAIN_WaterColor[k] = this.TERRAIN_WaterColor[k];
		}
	}

	public void InterpolatePresets(ReliefTerrainPresetHolder holderA, ReliefTerrainPresetHolder holderB, float t)
	{
		this.RTP_MIP_BIAS = Mathf.Lerp(holderA.RTP_MIP_BIAS, holderB.RTP_MIP_BIAS, t);
		this._SpecColor = Color.Lerp(holderA._SpecColor, holderB._SpecColor, t);
		this.RTP_DeferredAddPassSpec = Mathf.Lerp(holderA.RTP_DeferredAddPassSpec, holderB.RTP_DeferredAddPassSpec, t);
		this.MasterLayerBrightness = Mathf.Lerp(holderA.MasterLayerBrightness, holderB.MasterLayerBrightness, t);
		this.MasterLayerSaturation = Mathf.Lerp(holderA.MasterLayerSaturation, holderB.MasterLayerSaturation, t);
		this.BumpMapGlobalScale = Mathf.Lerp(holderA.BumpMapGlobalScale, holderB.BumpMapGlobalScale, t);
		this.GlobalColorMapBlendValues = Vector3.Lerp(holderA.GlobalColorMapBlendValues, holderB.GlobalColorMapBlendValues, t);
		this.GlobalColorMapSaturation = Mathf.Lerp(holderA.GlobalColorMapSaturation, holderB.GlobalColorMapSaturation, t);
		this.GlobalColorMapSaturationFar = Mathf.Lerp(holderA.GlobalColorMapSaturationFar, holderB.GlobalColorMapSaturationFar, t);
		this.GlobalColorMapDistortByPerlin = Mathf.Lerp(holderA.GlobalColorMapDistortByPerlin, holderB.GlobalColorMapDistortByPerlin, t);
		this.GlobalColorMapBrightness = Mathf.Lerp(holderA.GlobalColorMapBrightness, holderB.GlobalColorMapBrightness, t);
		this.GlobalColorMapBrightnessFar = Mathf.Lerp(holderA.GlobalColorMapBrightnessFar, holderB.GlobalColorMapBrightnessFar, t);
		this._GlobalColorMapNearMIP = Mathf.Lerp(holderA._GlobalColorMapNearMIP, holderB._GlobalColorMapNearMIP, t);
		this._FarNormalDamp = Mathf.Lerp(holderA._FarNormalDamp, holderB._FarNormalDamp, t);
		this.blendMultiplier = Mathf.Lerp(holderA.blendMultiplier, holderB.blendMultiplier, t);
		this.ReliefTransform = Vector4.Lerp(holderA.ReliefTransform, holderB.ReliefTransform, t);
		this.DIST_STEPS = Mathf.Lerp(holderA.DIST_STEPS, holderB.DIST_STEPS, t);
		this.WAVELENGTH = Mathf.Lerp(holderA.WAVELENGTH, holderB.WAVELENGTH, t);
		this.ReliefBorderBlend = Mathf.Lerp(holderA.ReliefBorderBlend, holderB.ReliefBorderBlend, t);
		this.ExtrudeHeight = Mathf.Lerp(holderA.ExtrudeHeight, holderB.ExtrudeHeight, t);
		this.LightmapShading = Mathf.Lerp(holderA.LightmapShading, holderB.LightmapShading, t);
		this.SHADOW_STEPS = Mathf.Lerp(holderA.SHADOW_STEPS, holderB.SHADOW_STEPS, t);
		this.WAVELENGTH_SHADOWS = Mathf.Lerp(holderA.WAVELENGTH_SHADOWS, holderB.WAVELENGTH_SHADOWS, t);
		this.SelfShadowStrength = Mathf.Lerp(holderA.SelfShadowStrength, holderB.SelfShadowStrength, t);
		this.ShadowSmoothing = Mathf.Lerp(holderA.ShadowSmoothing, holderB.ShadowSmoothing, t);
		this.ShadowSoftnessFade = Mathf.Lerp(holderA.ShadowSoftnessFade, holderB.ShadowSoftnessFade, t);
		this.distance_start = Mathf.Lerp(holderA.distance_start, holderB.distance_start, t);
		this.distance_transition = Mathf.Lerp(holderA.distance_transition, holderB.distance_transition, t);
		this.distance_start_bumpglobal = Mathf.Lerp(holderA.distance_start_bumpglobal, holderB.distance_start_bumpglobal, t);
		this.distance_transition_bumpglobal = Mathf.Lerp(holderA.distance_transition_bumpglobal, holderB.distance_transition_bumpglobal, t);
		this.rtp_perlin_start_val = Mathf.Lerp(holderA.rtp_perlin_start_val, holderB.rtp_perlin_start_val, t);
		this.trees_shadow_distance_start = Mathf.Lerp(holderA.trees_shadow_distance_start, holderB.trees_shadow_distance_start, t);
		this.trees_shadow_distance_transition = Mathf.Lerp(holderA.trees_shadow_distance_transition, holderB.trees_shadow_distance_transition, t);
		this.trees_shadow_value = Mathf.Lerp(holderA.trees_shadow_value, holderB.trees_shadow_value, t);
		this.trees_pixel_distance_start = Mathf.Lerp(holderA.trees_pixel_distance_start, holderB.trees_pixel_distance_start, t);
		this.trees_pixel_distance_transition = Mathf.Lerp(holderA.trees_pixel_distance_transition, holderB.trees_pixel_distance_transition, t);
		this.trees_pixel_blend_val = Mathf.Lerp(holderA.trees_pixel_blend_val, holderB.trees_pixel_blend_val, t);
		this.global_normalMap_multiplier = Mathf.Lerp(holderA.global_normalMap_multiplier, holderB.global_normalMap_multiplier, t);
		this.global_normalMap_farUsage = Mathf.Lerp(holderA.global_normalMap_farUsage, holderB.global_normalMap_farUsage, t);
		this._AmbientEmissiveMultiplier = Mathf.Lerp(holderA._AmbientEmissiveMultiplier, holderB._AmbientEmissiveMultiplier, t);
		this._AmbientEmissiveRelief = Mathf.Lerp(holderA._AmbientEmissiveRelief, holderB._AmbientEmissiveRelief, t);
		this._SuperDetailTiling = Mathf.Lerp(holderA._SuperDetailTiling, holderB._SuperDetailTiling, t);
		this.TERRAIN_ReflColorA = Color.Lerp(holderA.TERRAIN_ReflColorA, holderB.TERRAIN_ReflColorA, t);
		this.TERRAIN_ReflColorB = Color.Lerp(holderA.TERRAIN_ReflColorB, holderB.TERRAIN_ReflColorB, t);
		this.TERRAIN_ReflColorC = Color.Lerp(holderA.TERRAIN_ReflColorC, holderB.TERRAIN_ReflColorC, t);
		this.TERRAIN_ReflColorCenter = Mathf.Lerp(holderA.TERRAIN_ReflColorCenter, holderB.TERRAIN_ReflColorCenter, t);
		this.TERRAIN_ReflGlossAttenuation = Mathf.Lerp(holderA.TERRAIN_ReflGlossAttenuation, holderB.TERRAIN_ReflGlossAttenuation, t);
		this.TERRAIN_ReflectionRotSpeed = Mathf.Lerp(holderA.TERRAIN_ReflectionRotSpeed, holderB.TERRAIN_ReflectionRotSpeed, t);
		this.TERRAIN_GlobalWetness = Mathf.Lerp(holderA.TERRAIN_GlobalWetness, holderB.TERRAIN_GlobalWetness, t);
		this.TERRAIN_RippleScale = Mathf.Lerp(holderA.TERRAIN_RippleScale, holderB.TERRAIN_RippleScale, t);
		this.TERRAIN_FlowScale = Mathf.Lerp(holderA.TERRAIN_FlowScale, holderB.TERRAIN_FlowScale, t);
		this.TERRAIN_FlowSpeed = Mathf.Lerp(holderA.TERRAIN_FlowSpeed, holderB.TERRAIN_FlowSpeed, t);
		this.TERRAIN_FlowCycleScale = Mathf.Lerp(holderA.TERRAIN_FlowCycleScale, holderB.TERRAIN_FlowCycleScale, t);
		this.TERRAIN_FlowMipOffset = Mathf.Lerp(holderA.TERRAIN_FlowMipOffset, holderB.TERRAIN_FlowMipOffset, t);
		this.TERRAIN_WetDarkening = Mathf.Lerp(holderA.TERRAIN_WetDarkening, holderB.TERRAIN_WetDarkening, t);
		this.TERRAIN_WetDropletsStrength = Mathf.Lerp(holderA.TERRAIN_WetDropletsStrength, holderB.TERRAIN_WetDropletsStrength, t);
		this.TERRAIN_WetHeight_Treshold = Mathf.Lerp(holderA.TERRAIN_WetHeight_Treshold, holderB.TERRAIN_WetHeight_Treshold, t);
		this.TERRAIN_WetHeight_Transition = Mathf.Lerp(holderA.TERRAIN_WetHeight_Transition, holderB.TERRAIN_WetHeight_Transition, t);
		this.TERRAIN_RainIntensity = Mathf.Lerp(holderA.TERRAIN_RainIntensity, holderB.TERRAIN_RainIntensity, t);
		this.TERRAIN_DropletsSpeed = Mathf.Lerp(holderA.TERRAIN_DropletsSpeed, holderB.TERRAIN_DropletsSpeed, t);
		this.TERRAIN_mipoffset_flowSpeed = Mathf.Lerp(holderA.TERRAIN_mipoffset_flowSpeed, holderB.TERRAIN_mipoffset_flowSpeed, t);
		this.TERRAIN_CausticsAnimSpeed = Mathf.Lerp(holderA.TERRAIN_CausticsAnimSpeed, holderB.TERRAIN_CausticsAnimSpeed, t);
		this.TERRAIN_CausticsColor = Color.Lerp(holderA.TERRAIN_CausticsColor, holderB.TERRAIN_CausticsColor, t);
		this.TERRAIN_CausticsWaterLevel = Mathf.Lerp(holderA.TERRAIN_CausticsWaterLevel, holderB.TERRAIN_CausticsWaterLevel, t);
		this.TERRAIN_CausticsWaterLevelByAngle = Mathf.Lerp(holderA.TERRAIN_CausticsWaterLevelByAngle, holderB.TERRAIN_CausticsWaterLevelByAngle, t);
		this.TERRAIN_CausticsWaterDeepFadeLength = Mathf.Lerp(holderA.TERRAIN_CausticsWaterDeepFadeLength, holderB.TERRAIN_CausticsWaterDeepFadeLength, t);
		this.TERRAIN_CausticsWaterShallowFadeLength = Mathf.Lerp(holderA.TERRAIN_CausticsWaterShallowFadeLength, holderB.TERRAIN_CausticsWaterShallowFadeLength, t);
		this.TERRAIN_CausticsTilingScale = Mathf.Lerp(holderA.TERRAIN_CausticsTilingScale, holderB.TERRAIN_CausticsTilingScale, t);
		this.rtp_customAmbientCorrection = Color.Lerp(holderA.rtp_customAmbientCorrection, holderB.rtp_customAmbientCorrection, t);
		this.TERRAIN_IBL_DiffAO_Damp = Mathf.Lerp(holderA.TERRAIN_IBL_DiffAO_Damp, holderB.TERRAIN_IBL_DiffAO_Damp, t);
		this.TERRAIN_IBLRefl_SpecAO_Damp = Mathf.Lerp(holderA.TERRAIN_IBLRefl_SpecAO_Damp, holderB.TERRAIN_IBLRefl_SpecAO_Damp, t);
		this.RTP_AOsharpness = Mathf.Lerp(holderA.RTP_AOsharpness, holderB.RTP_AOsharpness, t);
		this.RTP_AOamp = Mathf.Lerp(holderA.RTP_AOamp, holderB.RTP_AOamp, t);
		this.RTP_LightDefVector = Vector4.Lerp(holderA.RTP_LightDefVector, holderB.RTP_LightDefVector, t);
		this.RTP_ReflexLightDiffuseColor = Color.Lerp(holderA.RTP_ReflexLightDiffuseColor, holderB.RTP_ReflexLightDiffuseColor, t);
		this.RTP_ReflexLightDiffuseColor2 = Color.Lerp(holderA.RTP_ReflexLightDiffuseColor2, holderB.RTP_ReflexLightDiffuseColor2, t);
		this.RTP_ReflexLightSpecColor = Color.Lerp(holderA.RTP_ReflexLightSpecColor, holderB.RTP_ReflexLightSpecColor, t);
		this.EmissionRefractFiltering = Mathf.Lerp(holderA.EmissionRefractFiltering, holderB.EmissionRefractFiltering, t);
		this.EmissionRefractAnimSpeed = Mathf.Lerp(holderA.EmissionRefractAnimSpeed, holderB.EmissionRefractAnimSpeed, t);
		this.VerticalTextureGlobalBumpInfluence = Mathf.Lerp(holderA.VerticalTextureGlobalBumpInfluence, holderB.VerticalTextureGlobalBumpInfluence, t);
		this.VerticalTextureTiling = Mathf.Lerp(holderA.VerticalTextureTiling, holderB.VerticalTextureTiling, t);
		this._snow_strength = Mathf.Lerp(holderA._snow_strength, holderB._snow_strength, t);
		this._global_color_brightness_to_snow = Mathf.Lerp(holderA._global_color_brightness_to_snow, holderB._global_color_brightness_to_snow, t);
		this._snow_slope_factor = Mathf.Lerp(holderA._snow_slope_factor, holderB._snow_slope_factor, t);
		this._snow_edge_definition = Mathf.Lerp(holderA._snow_edge_definition, holderB._snow_edge_definition, t);
		this._snow_height_treshold = Mathf.Lerp(holderA._snow_height_treshold, holderB._snow_height_treshold, t);
		this._snow_height_transition = Mathf.Lerp(holderA._snow_height_transition, holderB._snow_height_transition, t);
		this._snow_color = Color.Lerp(holderA._snow_color, holderB._snow_color, t);
		this._snow_specular = Mathf.Lerp(holderA._snow_specular, holderB._snow_specular, t);
		this._snow_gloss = Mathf.Lerp(holderA._snow_gloss, holderB._snow_gloss, t);
		this._snow_reflectivness = Mathf.Lerp(holderA._snow_reflectivness, holderB._snow_reflectivness, t);
		this._snow_deep_factor = Mathf.Lerp(holderA._snow_deep_factor, holderB._snow_deep_factor, t);
		this._snow_fresnel = Mathf.Lerp(holderA._snow_fresnel, holderB._snow_fresnel, t);
		this._snow_diff_fresnel = Mathf.Lerp(holderA._snow_diff_fresnel, holderB._snow_diff_fresnel, t);
		this._snow_IBL_DiffuseStrength = Mathf.Lerp(holderA._snow_IBL_DiffuseStrength, holderB._snow_IBL_DiffuseStrength, t);
		this._snow_IBL_SpecStrength = Mathf.Lerp(holderA._snow_IBL_SpecStrength, holderB._snow_IBL_SpecStrength, t);
		for (int i = 0; i < holderA.Spec.Length; i++)
		{
			if (i < this.Spec.Length)
			{
				this.Spec[i] = Mathf.Lerp(holderA.Spec[i], holderB.Spec[i], t);
				this.FarSpecCorrection[i] = Mathf.Lerp(holderA.FarSpecCorrection[i], holderB.FarSpecCorrection[i], t);
				this.MixScale[i] = Mathf.Lerp(holderA.MixScale[i], holderB.MixScale[i], t);
				this.MixBlend[i] = Mathf.Lerp(holderA.MixBlend[i], holderB.MixBlend[i], t);
				this.MixSaturation[i] = Mathf.Lerp(holderA.MixSaturation[i], holderB.MixSaturation[i], t);
				this.RTP_gloss2mask[i] = Mathf.Lerp(holderA.RTP_gloss2mask[i], holderB.RTP_gloss2mask[i], t);
				this.RTP_gloss_mult[i] = Mathf.Lerp(holderA.RTP_gloss_mult[i], holderB.RTP_gloss_mult[i], t);
				this.RTP_gloss_shaping[i] = Mathf.Lerp(holderA.RTP_gloss_shaping[i], holderB.RTP_gloss_shaping[i], t);
				this.RTP_Fresnel[i] = Mathf.Lerp(holderA.RTP_Fresnel[i], holderB.RTP_Fresnel[i], t);
				this.RTP_FresnelAtten[i] = Mathf.Lerp(holderA.RTP_FresnelAtten[i], holderB.RTP_FresnelAtten[i], t);
				this.RTP_DiffFresnel[i] = Mathf.Lerp(holderA.RTP_DiffFresnel[i], holderB.RTP_DiffFresnel[i], t);
				this.RTP_IBL_bump_smoothness[i] = Mathf.Lerp(holderA.RTP_IBL_bump_smoothness[i], holderB.RTP_IBL_bump_smoothness[i], t);
				this.RTP_IBL_DiffuseStrength[i] = Mathf.Lerp(holderA.RTP_IBL_DiffuseStrength[i], holderB.RTP_IBL_DiffuseStrength[i], t);
				this.RTP_IBL_SpecStrength[i] = Mathf.Lerp(holderA.RTP_IBL_SpecStrength[i], holderB.RTP_IBL_SpecStrength[i], t);
				this._DeferredSpecDampAddPass[i] = Mathf.Lerp(holderA._DeferredSpecDampAddPass[i], holderB._DeferredSpecDampAddPass[i], t);
				this.MixBrightness[i] = Mathf.Lerp(holderA.MixBrightness[i], holderB.MixBrightness[i], t);
				this.MixReplace[i] = Mathf.Lerp(holderA.MixReplace[i], holderB.MixReplace[i], t);
				this.LayerBrightness[i] = Mathf.Lerp(holderA.LayerBrightness[i], holderB.LayerBrightness[i], t);
				this.LayerBrightness2Spec[i] = Mathf.Lerp(holderA.LayerBrightness2Spec[i], holderB.LayerBrightness2Spec[i], t);
				this.LayerAlbedo2SpecColor[i] = Mathf.Lerp(holderA.LayerAlbedo2SpecColor[i], holderB.LayerAlbedo2SpecColor[i], t);
				this.LayerSaturation[i] = Mathf.Lerp(holderA.LayerSaturation[i], holderB.LayerSaturation[i], t);
				this.LayerEmission[i] = Mathf.Lerp(holderA.LayerEmission[i], holderB.LayerEmission[i], t);
				this.LayerEmissionColor[i] = Color.Lerp(holderA.LayerEmissionColor[i], holderB.LayerEmissionColor[i], t);
				this.LayerEmissionRefractStrength[i] = Mathf.Lerp(holderA.LayerEmissionRefractStrength[i], holderB.LayerEmissionRefractStrength[i], t);
				this.LayerEmissionRefractHBedge[i] = Mathf.Lerp(holderA.LayerEmissionRefractHBedge[i], holderB.LayerEmissionRefractHBedge[i], t);
				this.GlobalColorPerLayer[i] = Mathf.Lerp(holderA.GlobalColorPerLayer[i], holderB.GlobalColorPerLayer[i], t);
				this.GlobalColorBottom[i] = Mathf.Lerp(holderA.GlobalColorBottom[i], holderB.GlobalColorBottom[i], t);
				this.GlobalColorTop[i] = Mathf.Lerp(holderA.GlobalColorTop[i], holderB.GlobalColorTop[i], t);
				this.GlobalColorColormapLoSat[i] = Mathf.Lerp(holderA.GlobalColorColormapLoSat[i], holderB.GlobalColorColormapLoSat[i], t);
				this.GlobalColorColormapHiSat[i] = Mathf.Lerp(holderA.GlobalColorColormapHiSat[i], holderB.GlobalColorColormapHiSat[i], t);
				this.GlobalColorLayerLoSat[i] = Mathf.Lerp(holderA.GlobalColorLayerLoSat[i], holderB.GlobalColorLayerLoSat[i], t);
				this.GlobalColorLayerHiSat[i] = Mathf.Lerp(holderA.GlobalColorLayerHiSat[i], holderB.GlobalColorLayerHiSat[i], t);
				this.GlobalColorLoBlend[i] = Mathf.Lerp(holderA.GlobalColorLoBlend[i], holderB.GlobalColorLoBlend[i], t);
				this.GlobalColorHiBlend[i] = Mathf.Lerp(holderA.GlobalColorHiBlend[i], holderB.GlobalColorHiBlend[i], t);
				this.PER_LAYER_HEIGHT_MODIFIER[i] = Mathf.Lerp(holderA.PER_LAYER_HEIGHT_MODIFIER[i], holderB.PER_LAYER_HEIGHT_MODIFIER[i], t);
				this._SuperDetailStrengthMultA[i] = Mathf.Lerp(holderA._SuperDetailStrengthMultA[i], holderB._SuperDetailStrengthMultA[i], t);
				this._SuperDetailStrengthMultASelfMaskNear[i] = Mathf.Lerp(holderA._SuperDetailStrengthMultASelfMaskNear[i], holderB._SuperDetailStrengthMultASelfMaskNear[i], t);
				this._SuperDetailStrengthMultASelfMaskFar[i] = Mathf.Lerp(holderA._SuperDetailStrengthMultASelfMaskFar[i], holderB._SuperDetailStrengthMultASelfMaskFar[i], t);
				this._SuperDetailStrengthMultB[i] = Mathf.Lerp(holderA._SuperDetailStrengthMultB[i], holderB._SuperDetailStrengthMultB[i], t);
				this._SuperDetailStrengthMultBSelfMaskNear[i] = Mathf.Lerp(holderA._SuperDetailStrengthMultBSelfMaskNear[i], holderB._SuperDetailStrengthMultBSelfMaskNear[i], t);
				this._SuperDetailStrengthMultBSelfMaskFar[i] = Mathf.Lerp(holderA._SuperDetailStrengthMultBSelfMaskFar[i], holderB._SuperDetailStrengthMultBSelfMaskFar[i], t);
				this._SuperDetailStrengthNormal[i] = Mathf.Lerp(holderA._SuperDetailStrengthNormal[i], holderB._SuperDetailStrengthNormal[i], t);
				this._BumpMapGlobalStrength[i] = Mathf.Lerp(holderA._BumpMapGlobalStrength[i], holderB._BumpMapGlobalStrength[i], t);
				this.AO_strength[i] = Mathf.Lerp(holderA.AO_strength[i], holderB.AO_strength[i], t);
				this.VerticalTextureStrength[i] = Mathf.Lerp(holderA.VerticalTextureStrength[i], holderB.VerticalTextureStrength[i], t);
				this._snow_strength_per_layer[i] = Mathf.Lerp(holderA._snow_strength_per_layer[i], holderB._snow_strength_per_layer[i], t);
				this.TERRAIN_LayerWetStrength[i] = Mathf.Lerp(holderA.TERRAIN_LayerWetStrength[i], holderB.TERRAIN_LayerWetStrength[i], t);
				this.TERRAIN_WaterLevel[i] = Mathf.Lerp(holderA.TERRAIN_WaterLevel[i], holderB.TERRAIN_WaterLevel[i], t);
				this.TERRAIN_WaterLevelSlopeDamp[i] = Mathf.Lerp(holderA.TERRAIN_WaterLevelSlopeDamp[i], holderB.TERRAIN_WaterLevelSlopeDamp[i], t);
				this.TERRAIN_WaterEdge[i] = Mathf.Lerp(holderA.TERRAIN_WaterEdge[i], holderB.TERRAIN_WaterEdge[i], t);
				this.TERRAIN_WaterSpecularity[i] = Mathf.Lerp(holderA.TERRAIN_WaterSpecularity[i], holderB.TERRAIN_WaterSpecularity[i], t);
				this.TERRAIN_WaterGloss[i] = Mathf.Lerp(holderA.TERRAIN_WaterGloss[i], holderB.TERRAIN_WaterGloss[i], t);
				this.TERRAIN_WaterGlossDamper[i] = Mathf.Lerp(holderA.TERRAIN_WaterGlossDamper[i], holderB.TERRAIN_WaterGlossDamper[i], t);
				this.TERRAIN_WaterOpacity[i] = Mathf.Lerp(holderA.TERRAIN_WaterOpacity[i], holderB.TERRAIN_WaterOpacity[i], t);
				this.TERRAIN_Refraction[i] = Mathf.Lerp(holderA.TERRAIN_Refraction[i], holderB.TERRAIN_Refraction[i], t);
				this.TERRAIN_WetRefraction[i] = Mathf.Lerp(holderA.TERRAIN_WetRefraction[i], holderB.TERRAIN_WetRefraction[i], t);
				this.TERRAIN_Flow[i] = Mathf.Lerp(holderA.TERRAIN_Flow[i], holderB.TERRAIN_Flow[i], t);
				this.TERRAIN_WetFlow[i] = Mathf.Lerp(holderA.TERRAIN_WetFlow[i], holderB.TERRAIN_WetFlow[i], t);
				this.TERRAIN_WetSpecularity[i] = Mathf.Lerp(holderA.TERRAIN_WetSpecularity[i], holderB.TERRAIN_WetSpecularity[i], t);
				this.TERRAIN_WetGloss[i] = Mathf.Lerp(holderA.TERRAIN_WetGloss[i], holderB.TERRAIN_WetGloss[i], t);
				this.TERRAIN_WaterColor[i] = Color.Lerp(holderA.TERRAIN_WaterColor[i], holderB.TERRAIN_WaterColor[i], t);
				this.TERRAIN_WaterIBL_SpecWetStrength[i] = Mathf.Lerp(holderA.TERRAIN_WaterIBL_SpecWetStrength[i], holderB.TERRAIN_WaterIBL_SpecWetStrength[i], t);
				this.TERRAIN_WaterIBL_SpecWaterStrength[i] = Mathf.Lerp(holderA.TERRAIN_WaterIBL_SpecWaterStrength[i], holderB.TERRAIN_WaterIBL_SpecWaterStrength[i], t);
				this.TERRAIN_WaterEmission[i] = Mathf.Lerp(holderA.TERRAIN_WaterEmission[i], holderB.TERRAIN_WaterEmission[i], t);
			}
		}
	}

	public void ReturnToDefaults(string what = "", int layerIdx = -1)
	{
		if (what == string.Empty || what == "main")
		{
			this.ReliefTransform = new Vector4(3f, 3f, 0f, 0f);
			this.distance_start = 5f;
			this.distance_transition = 20f;
			this._SpecColor = new Color(0.784313738f, 0.784313738f, 0.784313738f, 1f);
			this.RTP_DeferredAddPassSpec = 0.5f;
			this.rtp_customAmbientCorrection = new Color(0.2f, 0.2f, 0.2f, 1f);
			this.TERRAIN_IBL_DiffAO_Damp = 0.25f;
			this.TERRAIN_IBLRefl_SpecAO_Damp = 0.5f;
			this.RTP_LightDefVector = new Vector4(0.05f, 0.5f, 0.5f, 25f);
			this.RTP_ReflexLightDiffuseColor = new Color(0.7921569f, 0.9411765f, 1f, 0.2f);
			this.RTP_ReflexLightDiffuseColor2 = new Color(0.7921569f, 0.9411765f, 1f, 0.2f);
			this.RTP_ReflexLightSpecColor = new Color(0.9411765f, 0.9607843f, 1f, 0.15f);
			this.ReliefBorderBlend = 6f;
			this.LightmapShading = 0f;
			this.RTP_MIP_BIAS = 0f;
			this.RTP_AOsharpness = 1.5f;
			this.RTP_AOamp = 0.1f;
			this.MasterLayerBrightness = 1f;
			this.MasterLayerSaturation = 1f;
			this.EmissionRefractFiltering = 4f;
			this.EmissionRefractAnimSpeed = 4f;
		}
		if (what == string.Empty || what == "perlin")
		{
			this.BumpMapGlobalScale = 0.1f;
			this._FarNormalDamp = 0.2f;
			this.distance_start_bumpglobal = 30f;
			this.distance_transition_bumpglobal = 30f;
			this.rtp_perlin_start_val = 0f;
		}
		if (what == string.Empty || what == "global_color")
		{
			this.GlobalColorMapBlendValues = new Vector3(0.2f, 0.4f, 0.5f);
			this.GlobalColorMapSaturation = 1f;
			this.GlobalColorMapSaturationFar = 1f;
			this.GlobalColorMapDistortByPerlin = 0.005f;
			this.GlobalColorMapBrightness = 1f;
			this.GlobalColorMapBrightnessFar = 1f;
			this._GlobalColorMapNearMIP = 0f;
			this.trees_shadow_distance_start = 50f;
			this.trees_shadow_distance_transition = 10f;
			this.trees_shadow_value = 0.5f;
			this.trees_pixel_distance_start = 500f;
			this.trees_pixel_distance_transition = 10f;
			this.trees_pixel_blend_val = 2f;
			this.global_normalMap_multiplier = 1f;
			this.global_normalMap_farUsage = 0f;
			this._Phong = 0f;
			this.tessHeight = 300f;
			this._TessSubdivisions = 1f;
			this._TessSubdivisionsFar = 1f;
			this._TessYOffset = 0f;
			this._AmbientEmissiveMultiplier = 1f;
			this._AmbientEmissiveRelief = 0.5f;
		}
		if (what == string.Empty || what == "uvblend")
		{
			this.blendMultiplier = 1f;
		}
		if (what == string.Empty || what == "pom/pm")
		{
			this.ExtrudeHeight = 0.05f;
			this.DIST_STEPS = 20f;
			this.WAVELENGTH = 2f;
			this.SHADOW_STEPS = 20f;
			this.WAVELENGTH_SHADOWS = 2f;
			this.SelfShadowStrength = 0.8f;
			this.ShadowSmoothing = 1f;
			this.ShadowSoftnessFade = 0.8f;
		}
		if (what == string.Empty || what == "snow")
		{
			this._global_color_brightness_to_snow = 0.5f;
			this._snow_strength = 0f;
			this._snow_slope_factor = 2f;
			this._snow_edge_definition = 5f;
			this._snow_height_treshold = -200f;
			this._snow_height_transition = 1f;
			this._snow_color = Color.white;
			this._snow_specular = 0.5f;
			this._snow_gloss = 0.7f;
			this._snow_reflectivness = 0.7f;
			this._snow_deep_factor = 1.5f;
			this._snow_fresnel = 0.5f;
			this._snow_diff_fresnel = 0.5f;
			this._snow_IBL_DiffuseStrength = 0.5f;
			this._snow_IBL_SpecStrength = 0.5f;
		}
		if (what == string.Empty || what == "superdetail")
		{
			this._SuperDetailTiling = 8f;
		}
		if (what == string.Empty || what == "vertical")
		{
			this.VerticalTextureGlobalBumpInfluence = 0f;
			this.VerticalTextureTiling = 50f;
		}
		if (what == string.Empty || what == "reflection")
		{
			this.TERRAIN_ReflectionRotSpeed = 0.3f;
			this.TERRAIN_ReflGlossAttenuation = 0.5f;
			this.TERRAIN_ReflColorA = new Color(1f, 1f, 1f, 1f);
			this.TERRAIN_ReflColorB = new Color(0.392156869f, 0.470588237f, 0.509803951f, 1f);
			this.TERRAIN_ReflColorC = new Color(0.156862751f, 0.1882353f, 0.235294119f, 1f);
			this.TERRAIN_ReflColorCenter = 0.5f;
		}
		if (what == string.Empty || what == "water")
		{
			this.TERRAIN_GlobalWetness = 1f;
			this.TERRAIN_RippleScale = 4f;
			this.TERRAIN_FlowScale = 1f;
			this.TERRAIN_FlowSpeed = 0.5f;
			this.TERRAIN_FlowCycleScale = 1f;
			this.TERRAIN_RainIntensity = 1f;
			this.TERRAIN_DropletsSpeed = 10f;
			this.TERRAIN_mipoffset_flowSpeed = 1f;
			this.TERRAIN_FlowMipOffset = 0f;
			this.TERRAIN_WetDarkening = 0.5f;
			this.TERRAIN_WetDropletsStrength = 0f;
			this.TERRAIN_WetHeight_Treshold = -200f;
			this.TERRAIN_WetHeight_Transition = 5f;
		}
		if (what == string.Empty || what == "caustics")
		{
			this.TERRAIN_CausticsAnimSpeed = 2f;
			this.TERRAIN_CausticsColor = Color.white;
			this.TERRAIN_CausticsWaterLevel = 30f;
			this.TERRAIN_CausticsWaterLevelByAngle = 2f;
			this.TERRAIN_CausticsWaterDeepFadeLength = 50f;
			this.TERRAIN_CausticsWaterShallowFadeLength = 30f;
			this.TERRAIN_CausticsTilingScale = 1f;
		}
		if (what == string.Empty || what == "layer")
		{
			int num = 0;
			int num2 = (this.numLayers >= 12) ? 12 : this.numLayers;
			if (layerIdx >= 0)
			{
				num = layerIdx;
				num2 = layerIdx + 1;
			}
			for (int i = num; i < num2; i++)
			{
				this.Spec[i] = 1f;
				this.FarSpecCorrection[i] = 0f;
				this.MIPmult[i] = 0f;
				this.MixScale[i] = 0.2f;
				this.MixBlend[i] = 0.5f;
				this.MixSaturation[i] = 0.3f;
				this.RTP_gloss2mask[i] = 0.5f;
				this.RTP_gloss_mult[i] = 1f;
				this.RTP_gloss_shaping[i] = 0.5f;
				this.RTP_Fresnel[i] = 0f;
				this.RTP_FresnelAtten[i] = 0f;
				this.RTP_DiffFresnel[i] = 0f;
				this.RTP_IBL_bump_smoothness[i] = 0.7f;
				this.RTP_IBL_DiffuseStrength[i] = 0.5f;
				this.RTP_IBL_SpecStrength[i] = 0.5f;
				this._DeferredSpecDampAddPass[i] = 1f;
				this.MixBrightness[i] = 2f;
				this.MixReplace[i] = 0f;
				this.LayerBrightness[i] = 1f;
				this.LayerBrightness2Spec[i] = 0f;
				this.LayerAlbedo2SpecColor[i] = 0f;
				this.LayerSaturation[i] = 1f;
				this.LayerEmission[i] = 0f;
				this.LayerEmissionColor[i] = Color.black;
				this.LayerEmissionRefractStrength[i] = 0f;
				this.LayerEmissionRefractHBedge[i] = 0f;
				this.GlobalColorPerLayer[i] = 1f;
				this.GlobalColorBottom[i] = 0f;
				this.GlobalColorTop[i] = 1f;
				this.GlobalColorColormapLoSat[i] = 1f;
				this.GlobalColorColormapHiSat[i] = 1f;
				this.GlobalColorLayerLoSat[i] = 1f;
				this.GlobalColorLayerHiSat[i] = 1f;
				this.GlobalColorLoBlend[i] = 1f;
				this.GlobalColorHiBlend[i] = 1f;
				this.PER_LAYER_HEIGHT_MODIFIER[i] = 0f;
				this._SuperDetailStrengthMultA[i] = 0f;
				this._SuperDetailStrengthMultASelfMaskNear[i] = 0f;
				this._SuperDetailStrengthMultASelfMaskFar[i] = 0f;
				this._SuperDetailStrengthMultB[i] = 0f;
				this._SuperDetailStrengthMultBSelfMaskNear[i] = 0f;
				this._SuperDetailStrengthMultBSelfMaskFar[i] = 0f;
				this._SuperDetailStrengthNormal[i] = 0f;
				this._BumpMapGlobalStrength[i] = 0.3f;
				this._snow_strength_per_layer[i] = 1f;
				this.VerticalTextureStrength[i] = 0.5f;
				this.AO_strength[i] = 1f;
				this.TERRAIN_LayerWetStrength[i] = 1f;
				this.TERRAIN_WaterLevel[i] = 0.5f;
				this.TERRAIN_WaterLevelSlopeDamp[i] = 0.5f;
				this.TERRAIN_WaterEdge[i] = 2f;
				this.TERRAIN_WaterSpecularity[i] = 0.5f;
				this.TERRAIN_WaterGloss[i] = 0.1f;
				this.TERRAIN_WaterGlossDamper[i] = 0f;
				this.TERRAIN_WaterOpacity[i] = 0.3f;
				this.TERRAIN_Refraction[i] = 0.01f;
				this.TERRAIN_WetRefraction[i] = 0.2f;
				this.TERRAIN_Flow[i] = 0.3f;
				this.TERRAIN_WetFlow[i] = 0.05f;
				this.TERRAIN_WetSpecularity[i] = 0.2f;
				this.TERRAIN_WetGloss[i] = 0.05f;
				this.TERRAIN_WaterColor[i] = new Color(0.9f, 0.9f, 1f, 0.5f);
				this.TERRAIN_WaterIBL_SpecWetStrength[i] = 0.5f;
				this.TERRAIN_WaterIBL_SpecWaterStrength[i] = 0.5f;
				this.TERRAIN_WaterEmission[i] = 0f;
			}
		}
	}

	public bool CheckAndUpdate(ref float[] aLayerPropArray, float defVal, int len)
	{
		if (aLayerPropArray == null || aLayerPropArray.Length < len)
		{
			aLayerPropArray = new float[len];
			for (int i = 0; i < len; i++)
			{
				aLayerPropArray[i] = defVal;
			}
			return true;
		}
		return false;
	}

	public bool CheckAndUpdate(ref Color[] aLayerPropArray, Color defVal, int len)
	{
		if (aLayerPropArray == null || aLayerPropArray.Length < len)
		{
			aLayerPropArray = new Color[len];
			for (int i = 0; i < len; i++)
			{
				aLayerPropArray[i] = defVal;
			}
			return true;
		}
		return false;
	}
}
