using System;
using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(Camera))]
public class AtmospherePost : MonoBehaviour
{
	public static AtmospherePost instance;

	public Material material;

	public Shader shader;

	private bool goodToGo;

	public static Vector3 L;

	private float mieDirectionalG;

	private float rayleighCoefficient;

	private float mieCoefficient;

	private float Molecules;

	private float turbidity;

	private float Exposure;

	private float sunE;

	private void Awake()
	{
		AtmospherePost.instance = this;
	}

	private void Start()
	{
		this.goodToGo = true;
		if (!this.shader)
		{
			this.goodToGo = false;
		}
		if (!this.material)
		{
			this.material = new Material(this.shader);
		}
		if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures)
		{
			this.goodToGo = false;
		}
		AtmospherePost.instance = this;
	}

	private void Update()
	{
		AtmospherePost.instance = this;
	}

	public void SetAtmosphereProperties(float mieDirectionalG, float rayleighCoefficient, float mieCoefficient, float Molecules, float turbidity, float Exposure, Vector3 L, float sunE)
	{
		this.material.SetFloat("turbidity", turbidity);
		this.material.SetFloat("Exposure", Exposure);
		this.material.SetVector("L", L);
		this.material.SetFloat("sunE", sunE);
	}

	private void OnPreRender()
	{
		Shader.SetGlobalMatrix("_InverseProjectionMatrix", base.GetComponent<Camera>().projectionMatrix.inverse);
		Shader.SetGlobalMatrix("_InverseViewMatrix", base.GetComponent<Camera>().cameraToWorldMatrix);
	}

	[ImageEffectOpaque]
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!this.goodToGo)
		{
			Graphics.Blit(source, destination);
			return;
		}
		if (!this.material)
		{
			this.material = new Material(this.shader);
		}
		this.material.hideFlags = HideFlags.HideAndDontSave;
		Graphics.Blit(source, destination, this.material);
	}
}
