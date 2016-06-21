using System;
using UnityEngine;

public class GrassUpdate : MonoBehaviour
{
	public float Fresnel;

	public float CutOff = 0.5f;

	public float Shininess = 4f;

	public float SpecularIntensity = 1f;

	public Vector4 Diffuse = Vector4.one;

	private Material grassMaterial;

	private Material TerrainMaterial
	{
		get
		{
			if (this.grassMaterial == null)
			{
				Shader shader = Shader.Find("Hidden/TerrainEngine/Details/WavingDoublePass");
				if (shader)
				{
					this.grassMaterial = new Material(shader);
					this.grassMaterial.name = "Grass Material";
				}
				else
				{
					Debug.LogError("Failed to create IBL material. Missing shader?");
				}
			}
			return this.grassMaterial;
		}
	}

	private void Awake()
	{
	}

	private void Start()
	{
	}

	public void Update()
	{
		if (this.Fresnel >= 0f && this.Fresnel <= 1f)
		{
			Shader.SetGlobalFloat("_FresnelGrass", this.Fresnel);
		}
		if (this.CutOff >= 0f && this.CutOff <= 1f)
		{
			Shader.SetGlobalFloat("_CutoffGrass", this.CutOff);
		}
		Shader.SetGlobalFloat("_ShininessGrass", this.Shininess);
		Shader.SetGlobalFloat("_SpecIntGrass", this.SpecularIntensity);
		Shader.SetGlobalVector("_ColorGrass", this.Diffuse);
	}

	public void OnDestroy()
	{
		UnityEngine.Object.DestroyImmediate(this.grassMaterial, false);
		this.grassMaterial = null;
	}
}
