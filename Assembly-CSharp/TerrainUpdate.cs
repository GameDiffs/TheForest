using System;
using UnityEngine;

public class TerrainUpdate : MonoBehaviour
{
	private Matrix4x4 skyMatrix = Matrix4x4.identity;

	private float ripplesUpdate;

	private Material terrainMaterial;

	private Material TerrainMaterial
	{
		get
		{
			if (this.terrainMaterial == null)
			{
				Shader shader = Shader.Find("AdvTex4/Terrain/AT4ReliefDX11V6");
				if (shader)
				{
					this.terrainMaterial = new Material(shader);
					this.terrainMaterial.name = "Terrain Material V6";
				}
				else
				{
					Debug.LogError("Failed to create IBL Skybox material. Missing shader?");
				}
			}
			return this.terrainMaterial;
		}
	}

	private void Awake()
	{
	}

	private void Start()
	{
		this.skyMatrix.SetTRS(Vector3.zero, base.transform.rotation, Vector3.one);
		Shader.SetGlobalMatrix("_SkyMatrix", this.skyMatrix);
	}

	public void Update()
	{
		this.skyMatrix.SetTRS(Vector3.zero, base.transform.rotation, Vector3.one);
		Shader.SetGlobalMatrix("_SkyMatrix", this.skyMatrix);
		Shader.SetGlobalFloat("_RipplesMove", this.ripplesUpdate);
		this.ripplesUpdate += Time.deltaTime * 0.06f;
	}

	public void OnDestroy()
	{
		UnityEngine.Object.DestroyImmediate(this.terrainMaterial, false);
		this.terrainMaterial = null;
	}
}
