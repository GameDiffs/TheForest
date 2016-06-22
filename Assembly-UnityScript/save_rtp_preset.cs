using System;
using UnityEngine;

[Serializable]
public class save_rtp_preset : MonoBehaviour
{
	public Texture2D[] NormalGlobal;

	public Texture2D[] TreesGlobal;

	public Texture2D[] ColorGlobal;

	public Texture2D BumpGlobalCombined;

	public Texture2D controlA;

	public Texture2D controlB;

	public Texture2D controlC;

	public float distance_start;

	public float distance_transition;

	public float RTP_MIP_BIAS;

	public Color _SpecColor;

	public Color rtp_customAmbientCorrection;

	public Vector4 RTP_LightDefVector;

	public Color RTP_ReflexLightDiffuseColor;

	public Color RTP_ReflexLightSpecColor;

	public float RTP_AOsharpness;

	public float RTP_AOamp;

	public float blendMultiplier;

	public Texture2D VerticalTexture;

	public float VerticalTextureTiling;

	public float VerticalTextureGlobalBumpInfluence;

	public Vector3 GlobalColorMapBlendValues;

	public float _GlobalColorMapNearMIP;

	public float GlobalColorMapSaturation;

	public float GlobalColorMapBrightness;

	public float global_normalMap_multiplier;

	public float trees_pixel_distance_start;

	public float trees_pixel_distance_transition;

	public float trees_pixel_blend_val;

	public float trees_shadow_distance_start;

	public float trees_shadow_distance_transition;

	public float trees_shadow_value;

	public float _snow_strength;

	public float _global_color_brightness_to_snow;

	public float _snow_slope_factor;

	public float _snow_height_treshold;

	public float _snow_height_transition;

	public Color _snow_color;

	public float _snow_specular;

	public float _snow_gloss;

	public float _snow_reflectivness;

	public float _snow_edge_definition;

	public float _snow_deep_factor;

	public float BumpMapGlobalScale;

	public float rtp_mipoffset_globalnorm;

	public float distance_start_bumpglobal;

	public float rtp_perlin_start_val;

	public float distance_transition_bumpglobal;

	public float _FarNormalDamp;

	public float TERRAIN_GlobalWetness;

	public float TERRAIN_WaterSpecularity;

	public float TERRAIN_FlowSpeed;

	public float TERRAIN_FlowScale;

	public float TERRAIN_FlowMipOffset;

	public float TERRAIN_mipoffset_flowSpeed;

	public float TERRAIN_WetDarkening;

	public float TERRAIN_RainIntensity;

	public float TERRAIN_WetDropletsStrength;

	public float TERRAIN_DropletsSpeed;

	public float TERRAIN_RippleScale;

	public float TERRAIN_CausticsAnimSpeed;

	public Color TERRAIN_CausticsColor;

	public float TERRAIN_CausticsWaterLevel;

	public float TERRAIN_CausticsWaterLevelByAngle;

	public float TERRAIN_CausticsWaterShallowFadeLength;

	public float TERRAIN_CausticsWaterDeepFadeLength;

	public float TERRAIN_CausticsTilingScale;

	public float _SuperDetailTiling;

	public Color TERRAIN_ReflColorA;

	public Color TERRAIN_ReflColorB;

	public float TERRAIN_ReflDistortion;

	public float TERRAIN_ReflectionRotSpeed;

	public float TERRAIN_FresnelPow;

	public float TERRAIN_FresnelOffset;

	public Texture2D[] splats;

	public Texture2D[] splat_atlases;

	public Texture2D[] Bumps;

	public Texture2D Bump01;

	public Texture2D Bump23;

	public Texture2D Bump45;

	public Texture2D Bump67;

	public Texture2D Bump89;

	public Texture2D BumpAB;

	public Texture2D[] Heights;

	public Texture2D HeightMap;

	public Texture2D HeightMap2;

	public Texture2D HeightMap3;

	public ProceduralMaterial[] Substances;

	public float[] Spec;

	public float[] FarGlossCorrection;

	public float[] PER_LAYER_HEIGHT_MODIFIER;

	public float[] MIPmult;

	public float[] GlobalColorPerLayer;

	public float[] MixScale;

	public float[] MixBlend;

	public float[] MixSaturation;

	public float[] _BumpMapGlobalStrength;

	public float[] _SuperDetailStrengthNormal;

	public float[] _SuperDetailStrengthMultA;

	public float[] _SuperDetailStrengthMultASelfMaskNear;

	public float[] _SuperDetailStrengthMultASelfMaskFar;

	public float[] _SuperDetailStrengthMultB;

	public float[] _SuperDetailStrengthMultBSelfMaskNear;

	public float[] _SuperDetailStrengthMultBSelfMaskFar;

	public float[] VerticalTextureStrength;

	public float[] _snow_strength_per_layer;

	public float[] TERRAIN_LayerWetStrength;

	public Color[] TERRAIN_WaterColor;

	public float[] TERRAIN_WaterLevel;

	public float[] TERRAIN_WaterLevelSlopeDamp;

	public float[] TERRAIN_WaterEdge;

	public float[] TERRAIN_WaterOpacity;

	public float[] TERRAIN_WaterGloss;

	public float[] TERRAIN_Refraction;

	public float[] TERRAIN_Flow;

	public float[] TERRAIN_WetSpecularity;

	public float[] TERRAIN_WetReflection;

	public float[] TERRAIN_WetRefraction;

	public Vector4 ReliefTransform;

	public save_rtp_preset()
	{
		this.splat_atlases = new Texture2D[2];
	}

	public override void Main()
	{
	}
}
