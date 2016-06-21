using System;
using UnityEngine;

[AddComponentMenu("Image Effects/BloodOverlay"), ExecuteInEditMode]
public class BleedBehavior : MonoBehaviour
{
	public static float BloodAmount;

	public static float BloodReductionRatio = 1f;

	public float TestingBloodAmount = 0.5f;

	public static float minBloodAmount;

	public float EdgeSharpness = 1f;

	public float minAlpha;

	public float maxAlpha = 1f;

	public float distortion = 0.2f;

	public bool autoFadeOut = true;

	public float autoFadeOutAbsReduc = 0.05f;

	public float autoFadeOutRelReduc = 0.5f;

	public float updateSpeed = 20f;

	private float prevBloodAmount;

	public Texture2D Image;

	public Texture2D Normals;

	public Shader Shader;

	private Material _material;

	private void Start()
	{
		this._material = new Material(this.Shader);
		this._material.SetTexture("_BlendTex", this.Image);
		this._material.SetTexture("_BumpMap", this.Normals);
	}

	public void Update()
	{
		if (this.autoFadeOut && BleedBehavior.BloodAmount > 0f)
		{
			BleedBehavior.BloodAmount -= this.autoFadeOutAbsReduc * Time.deltaTime * BleedBehavior.BloodReductionRatio;
			BleedBehavior.BloodAmount *= 1f + (Mathf.Pow(1f - this.autoFadeOutRelReduc, Time.deltaTime) - 1f) * BleedBehavior.BloodReductionRatio;
			BleedBehavior.BloodAmount = Mathf.Max(BleedBehavior.BloodAmount, 0f);
		}
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!Application.isPlaying)
		{
			this._material.SetTexture("_BlendTex", this.Image);
			this._material.SetTexture("_BumpMap", this.Normals);
			float num = Mathf.Clamp01(this.TestingBloodAmount) * (1f - BleedBehavior.minBloodAmount) + BleedBehavior.minBloodAmount;
			num = Mathf.Clamp01(num * (this.maxAlpha - this.minAlpha) + this.minAlpha);
			num = Mathf.Lerp(this.prevBloodAmount, num, Mathf.Clamp01(this.updateSpeed * Time.deltaTime));
			this._material.SetFloat("_BlendAmount", num);
			this.prevBloodAmount = num;
		}
		else
		{
			float num2 = Mathf.Clamp01(BleedBehavior.BloodAmount) * (1f - BleedBehavior.minBloodAmount) + BleedBehavior.minBloodAmount;
			num2 = Mathf.Clamp01(num2 * (this.maxAlpha - this.minAlpha) + this.minAlpha);
			num2 = Mathf.Lerp(this.prevBloodAmount, num2, Mathf.Clamp01(this.updateSpeed * Time.deltaTime));
			this._material.SetFloat("_BlendAmount", num2);
			this.prevBloodAmount = num2;
		}
		this._material.SetFloat("_EdgeSharpness", this.EdgeSharpness);
		this._material.SetFloat("_Distortion", this.distortion);
		Graphics.Blit(source, destination, this._material);
	}
}
