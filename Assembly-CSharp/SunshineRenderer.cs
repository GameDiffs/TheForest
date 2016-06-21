using System;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class SunshineRenderer : MonoBehaviour
{
	private bool _receiveShadows = true;

	private bool isDirty = true;

	private static readonly string[] disabledKeywords = new string[]
	{
		"SUNSHINE_DISABLED"
	};

	private Material[] originalSharedMaterials;

	private Renderer attachedRenderer;

	private void OnEnable()
	{
		this.attachedRenderer = base.GetComponent<Renderer>();
	}

	private void Update()
	{
		bool receiveShadows = this.attachedRenderer.receiveShadows;
		if (this._receiveShadows != receiveShadows)
		{
			this._receiveShadows = receiveShadows;
			this.isDirty = true;
		}
		if (this.isDirty)
		{
			if (receiveShadows)
			{
				if (this.originalSharedMaterials != null)
				{
					this.attachedRenderer.materials = this.originalSharedMaterials;
				}
			}
			else
			{
				this.originalSharedMaterials = this.attachedRenderer.sharedMaterials;
				Material[] materials = this.attachedRenderer.materials;
				for (int i = 0; i < materials.Length; i++)
				{
					Material material = materials[i];
					material.shaderKeywords = SunshineRenderer.disabledKeywords;
				}
			}
			this.isDirty = false;
		}
	}
}
