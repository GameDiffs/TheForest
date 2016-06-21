using System;
using UnityEngine;

public class SkinUpdate : MonoBehaviour
{
	private float sknUpdate;

	private Material sknMaterial;

	private Material SkinBleedMaterial
	{
		get
		{
			if (this.sknMaterial == null)
			{
				Shader shader = Shader.Find("Skin/PreIntegratedSkinShaderV1.2_SM3");
				if (shader)
				{
					this.sknMaterial = new Material(shader);
					this.sknMaterial.name = "Skin Bleed Material";
				}
				else
				{
					Debug.LogError("Failed to create IBL material. Missing shader?");
				}
			}
			return this.sknMaterial;
		}
	}

	private void Awake()
	{
	}

	private void Start()
	{
		this.sknUpdate = 0f;
	}

	public void Update()
	{
		Shader.SetGlobalFloat("_MoveStep", this.sknUpdate);
		this.sknUpdate += Time.deltaTime * 0.3f;
	}

	public void OnDestroy()
	{
		UnityEngine.Object.DestroyImmediate(this.sknMaterial, false);
		this.sknMaterial = null;
	}
}
