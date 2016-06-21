using System;
using UnityEngine;

public class RockUpdate : MonoBehaviour
{
	private float cavernUpdate;

	private Material rockMaterial;

	private Material TerrainMaterial
	{
		get
		{
			if (this.rockMaterial == null)
			{
				Shader shader = Shader.Find("EndNight/Marmoset Bumped Specular Cavern DX9");
				if (shader)
				{
					this.rockMaterial = new Material(shader);
					this.rockMaterial.name = "Cavern Material";
				}
				else
				{
					Debug.LogError("Failed to create IBL material. Missing shader?");
				}
			}
			return this.rockMaterial;
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
		Shader.SetGlobalFloat("_MoveStep", this.cavernUpdate);
		this.cavernUpdate += Time.deltaTime * 0.01f;
	}

	public void OnDestroy()
	{
		UnityEngine.Object.DestroyImmediate(this.rockMaterial, false);
		this.rockMaterial = null;
	}
}
