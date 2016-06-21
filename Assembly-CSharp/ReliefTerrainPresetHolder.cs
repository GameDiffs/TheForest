using System;
using UnityEngine;

[Serializable]
public class ReliefTerrainPresetHolder : ScriptableObject
{
	public string PresetID;

	public string PresetName;

	public string type;

	public int numLayers;

	public Texture2D[] splats;

	public RTPGlossBaked[] gloss_baked = new RTPGlossBaked[12];

	public Texture2D[] splat_atlases;

	public Texture2D controlA;

	public Texture2D controlB;

	public Texture2D controlC;

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

	public Texture2D ColorGlobal;

	public Texture2D NormalGlobal;

	public Texture2D TreesGlobal;

	public Texture2D AmbientEmissiveMap;

	public Texture2D SSColorCombinedA;

	public Texture2D SSColorCombinedB;

	public bool globalColorModifed_flag;

	public bool globalCombinedModifed_flag;

	public bool globalWaterModifed_flag;

	public Texture2D BumpGlobal;

	public Texture2D BumpGlobalCombined;

	public Texture2D VerticalTexture;

	public float BumpMapGlobalScale;

	public Vector3 GlobalColorMapBlendValues;

	public float GlobalColorMapSaturation;

	public float GlobalColorMapSaturationFar;

	public float GlobalColorMapDistortByPerlin = 0.005f;

	public float GlobalColorMapBrightness;

	public float GlobalColorMapBrightnessFar = 1f;

	public float _GlobalColorMapNearMIP;

	public float _FarNormalDamp;

	public float blendMultiplier;

	public Texture2D HeightMap;

	public Texture2D HeightMap2;

	public Texture2D HeightMap3;

	public Vector4 ReliefTransform;

	public float DIST_STEPS;

	public float WAVELENGTH;

	public float ReliefBorderBlend;

	public float ExtrudeHeight;

	public float LightmapShading;

	public float SHADOW_STEPS;

	public float WAVELENGTH_SHADOWS;

	public float SelfShadowStrength;

	public float ShadowSmoothing;

	public float ShadowSoftnessFade = 0.7f;

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

	public float _AmbientEmissiveMultiplier;

	public float _AmbientEmissiveRelief;

	public int rtp_mipoffset_globalnorm;

	public float _SuperDetailTiling;

	public Texture2D SuperDetailA;

	public Texture2D SuperDetailB;

	public Texture2D TERRAIN_ReflectionMap;

	public RTPColorChannels TERRAIN_ReflectionMap_channel;

	public Color TERRAIN_ReflColorA;

	public Color TERRAIN_ReflColorB;

	public Color TERRAIN_ReflColorC;

	public float TERRAIN_ReflColorCenter;

	public float TERRAIN_ReflGlossAttenuation;

	public float TERRAIN_ReflectionRotSpeed;

	public float TERRAIN_GlobalWetness;

	public Texture2D TERRAIN_RippleMap;

	public Texture2D TERRAIN_WetMask;

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

	public float TERRAIN_CausticsWaterLevel;

	public float TERRAIN_CausticsWaterLevelByAngle;

	public float TERRAIN_CausticsWaterDeepFadeLength;

	public float TERRAIN_CausticsWaterShallowFadeLength;

	public float TERRAIN_CausticsTilingScale;

	public Texture2D TERRAIN_CausticsTex;

	public Color rtp_customAmbientCorrection;

	public float TERRAIN_IBL_DiffAO_Damp;

	public float TERRAIN_IBLRefl_SpecAO_Damp;

	public Cubemap _CubemapDiff;

	public Cubemap _CubemapSpec;

	public Vector4 RTP_LightDefVector;

	public Color RTP_ReflexLightDiffuseColor;

	public Color RTP_ReflexLightDiffuseColor2;

	public Color RTP_ReflexLightSpecColor;

	public float RTP_AOamp;

	public float RTP_AOsharpness;

	public Texture2D[] Bumps;

	public float[] Spec;

	public float[] FarSpecCorrection;

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

	public float[] GlobalColorBottom;

	public float[] GlobalColorTop;

	public float[] GlobalColorColormapLoSat;

	public float[] GlobalColorColormapHiSat;

	public float[] GlobalColorLayerLoSat;

	public float[] GlobalColorLayerHiSat;

	public float[] GlobalColorLoBlend;

	public float[] GlobalColorHiBlend;

	public float[] PER_LAYER_HEIGHT_MODIFIER;

	public float[] _SuperDetailStrengthMultA;

	public float[] _SuperDetailStrengthMultASelfMaskNear;

	public float[] _SuperDetailStrengthMultASelfMaskFar;

	public float[] _SuperDetailStrengthMultB;

	public float[] _SuperDetailStrengthMultBSelfMaskNear;

	public float[] _SuperDetailStrengthMultBSelfMaskFar;

	public float[] _SuperDetailStrengthNormal;

	public float[] _BumpMapGlobalStrength;

	public float[] VerticalTextureStrength;

	public float[] AO_strength;

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

	public void Init(string name)
	{
		this.PresetID = string.Empty + UnityEngine.Random.value + Time.realtimeSinceStartup;
		this.PresetName = name;
	}
}
