using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class MB2_TextureBakeResults : ScriptableObject
{
	public MB_AtlasesAndRects[] combinedMaterialInfo;

	public Material[] materials;

	public Rect[] prefabUVRects;

	public Material resultMaterial;

	public MB_MultiMaterial[] resultMaterials;

	public bool doMultiMaterial;

	public bool fixOutOfBoundsUVs;

	public static MB2_TextureBakeResults CreateForMaterialsOnRenderer(Renderer r)
	{
		MB2_TextureBakeResults mB2_TextureBakeResults = (MB2_TextureBakeResults)ScriptableObject.CreateInstance(typeof(MB2_TextureBakeResults));
		Material[] sharedMaterials = r.sharedMaterials;
		mB2_TextureBakeResults.resultMaterial = sharedMaterials[0];
		mB2_TextureBakeResults.fixOutOfBoundsUVs = false;
		mB2_TextureBakeResults.materials = sharedMaterials;
		mB2_TextureBakeResults.resultMaterials = new MB_MultiMaterial[sharedMaterials.Length];
		if (sharedMaterials.Length > 1)
		{
			mB2_TextureBakeResults.prefabUVRects = new Rect[sharedMaterials.Length];
			for (int i = 0; i < sharedMaterials.Length; i++)
			{
				mB2_TextureBakeResults.prefabUVRects[i] = new Rect(0f, 0f, 1f, 1f);
				mB2_TextureBakeResults.resultMaterials[i] = new MB_MultiMaterial();
				List<Material> list = new List<Material>();
				list.Add(sharedMaterials[i]);
				mB2_TextureBakeResults.resultMaterials[i].sourceMaterials = list;
				mB2_TextureBakeResults.resultMaterials[i].combinedMaterial = sharedMaterials[i];
			}
			mB2_TextureBakeResults.doMultiMaterial = true;
		}
		else
		{
			mB2_TextureBakeResults.doMultiMaterial = false;
			mB2_TextureBakeResults.prefabUVRects = new Rect[]
			{
				new Rect(0f, 0f, 1f, 1f)
			};
		}
		return mB2_TextureBakeResults;
	}

	public Dictionary<Material, Rect> GetMat2RectMap()
	{
		Dictionary<Material, Rect> dictionary = new Dictionary<Material, Rect>();
		if (this.materials == null || this.prefabUVRects == null || this.materials.Length != this.prefabUVRects.Length)
		{
			Debug.LogWarning("Bad TextureBakeResults could not build mat2UVRect map");
		}
		else
		{
			for (int i = 0; i < this.materials.Length; i++)
			{
				dictionary.Add(this.materials[i], this.prefabUVRects[i]);
			}
		}
		return dictionary;
	}

	public string GetDescription()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("Shaders:\n");
		HashSet<Shader> hashSet = new HashSet<Shader>();
		if (this.materials != null)
		{
			for (int i = 0; i < this.materials.Length; i++)
			{
				hashSet.Add(this.materials[i].shader);
			}
		}
		foreach (Shader current in hashSet)
		{
			stringBuilder.Append("  ").Append(current.name).AppendLine();
		}
		stringBuilder.Append("Materials:\n");
		if (this.materials != null)
		{
			for (int j = 0; j < this.materials.Length; j++)
			{
				stringBuilder.Append("  ").Append(this.materials[j].name).AppendLine();
			}
		}
		return stringBuilder.ToString();
	}
}
