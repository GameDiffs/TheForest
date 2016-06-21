using System;
using UnityEngine;

[ExecuteInEditMode]
public class SetupDeferredSkin : MonoBehaviour
{
	[Header("Area Lights"), Space(2f)]
	public bool enableAreaLights;

	[Header("Skin Lighting"), Space(2f)]
	public Texture BRDFTexture;

	[Space(5f)]
	public Color SubsurfaceColor = new Color(1f, 0.4f, 0.25f, 1f);

	public float Power = 2f;

	[Range(0f, 1f)]
	public float Distortion = 0.1f;

	public float Scale = 2f;

	[Range(1f, 3f)]
	public float SpecularMultiplier = 1f;

	[Header("Fur Lighting Rough Material"), Range(0f, 1f)]
	public float PrimarySmoothnessR = 0.5f;

	[Range(0f, 2f)]
	public float PrimaryStrengthR = 1.5f;

	[Range(-1f, 1f)]
	public float PrimaryShiftR = 0.1f;

	[Range(0f, 1f), Space(5f)]
	public float SecondarySmoothnessR = 0.3f;

	[Range(0f, 20f)]
	public float SecondaryStrengthR = 10f;

	[Range(-1f, 1f)]
	public float SecondaryShiftR = 0.15f;

	[Range(0f, 1f), Space(5f)]
	public float RimStrengthR = 0.15f;

	[Header("Fur Lighting Smooth Material"), Range(0f, 1f)]
	public float PrimarySmoothnessS = 0.7f;

	[Range(0f, 2f)]
	public float PrimaryStrengthS = 1.5f;

	private float PrimaryNormalizationS;

	[Range(-1f, 1f)]
	public float PrimaryShiftS = 0.1f;

	[Range(0f, 1f), Space(5f)]
	public float SecondarySmoothnessS = 0.5f;

	[Range(0f, 20f)]
	public float SecondaryStrengthS = 10f;

	[Range(-1f, 1f)]
	public float SecondaryShiftS = 0.15f;

	private float SecondaryNormalizationS;

	[Range(0f, 1f), Space(5f)]
	public float RimStrengthS = 0.15f;

	private float PrimarySP_R;

	private float SecondarySP_R;

	private float PrimarySP_S;

	private float SecondarySP_S;

	private float Smoothness2SpecularPower(float smoothness)
	{
		smoothness = 1f - smoothness;
		smoothness *= smoothness;
		smoothness = 2f / (smoothness * smoothness) - 2f;
		return smoothness;
	}

	private void Start()
	{
		Shader.SetGlobalTexture("_BRDFTex", this.BRDFTexture);
		Shader.SetGlobalColor("_SubColor", this.SubsurfaceColor.linear);
		Shader.SetGlobalVector("_Lux_Skin_DeepSubsurface", new Vector4(this.Power, this.Distortion, this.Scale, this.SpecularMultiplier));
		Shader.SetGlobalFloat("_Lux_Skin_SpecularMultiplier", this.SpecularMultiplier);
		Shader.SetGlobalVector("_Lux_Fur_Exponents_R", new Vector4(this.Smoothness2SpecularPower(this.PrimarySmoothnessR), this.Smoothness2SpecularPower(this.SecondarySmoothnessR), this.PrimaryStrengthR, this.SecondaryStrengthR));
		Shader.SetGlobalVector("_Lux_Fur_Shift_Rim_R", new Vector4(this.PrimaryShiftR, this.SecondaryShiftR, this.RimStrengthR, (this.PrimarySmoothnessR + this.SecondarySmoothnessR) * 0.5f));
		Shader.SetGlobalVector("_Lux_Fur_Exponents_S", new Vector4(this.Smoothness2SpecularPower(this.PrimarySmoothnessS), this.Smoothness2SpecularPower(this.SecondarySmoothnessS), this.PrimaryStrengthS, this.SecondaryStrengthS));
		Shader.SetGlobalVector("_Lux_Fur_Shift_Rim_S", new Vector4(this.PrimaryShiftS, this.SecondaryShiftS, this.RimStrengthS, this.PrimarySmoothnessS + this.SecondarySmoothnessS * 0.5f));
	}

	private void Update()
	{
	}
}
