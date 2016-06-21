using DigitalOpus.MB.Core;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class MB3_TextureBaker : MB3_MeshBakerRoot
{
	public MB2_LogLevel LOG_LEVEL = MB2_LogLevel.info;

	[SerializeField]
	protected MB2_TextureBakeResults _textureBakeResults;

	[SerializeField]
	protected int _atlasPadding = 1;

	[SerializeField]
	protected int _maxAtlasSize = 4096;

	[SerializeField]
	protected bool _resizePowerOfTwoTextures;

	[SerializeField]
	protected bool _fixOutOfBoundsUVs;

	[SerializeField]
	protected int _maxTilingBakeSize = 1024;

	[SerializeField]
	protected MB2_PackingAlgorithmEnum _packingAlgorithm = MB2_PackingAlgorithmEnum.MeshBakerTexturePacker;

	[SerializeField]
	protected bool _meshBakerTexturePackerForcePowerOfTwo = true;

	[SerializeField]
	protected List<ShaderTextureProperty> _customShaderProperties = new List<ShaderTextureProperty>();

	[SerializeField]
	protected List<string> _customShaderPropNames_Depricated = new List<string>();

	[SerializeField]
	protected bool _doMultiMaterial;

	[SerializeField]
	protected Material _resultMaterial;

	public MB_MultiMaterial[] resultMaterials = new MB_MultiMaterial[0];

	public List<GameObject> objsToMesh;

	public override MB2_TextureBakeResults textureBakeResults
	{
		get
		{
			return this._textureBakeResults;
		}
		set
		{
			this._textureBakeResults = value;
		}
	}

	public virtual int atlasPadding
	{
		get
		{
			return this._atlasPadding;
		}
		set
		{
			this._atlasPadding = value;
		}
	}

	public virtual int maxAtlasSize
	{
		get
		{
			return this._maxAtlasSize;
		}
		set
		{
			this._maxAtlasSize = value;
		}
	}

	public virtual bool resizePowerOfTwoTextures
	{
		get
		{
			return this._resizePowerOfTwoTextures;
		}
		set
		{
			this._resizePowerOfTwoTextures = value;
		}
	}

	public virtual bool fixOutOfBoundsUVs
	{
		get
		{
			return this._fixOutOfBoundsUVs;
		}
		set
		{
			this._fixOutOfBoundsUVs = value;
		}
	}

	public virtual int maxTilingBakeSize
	{
		get
		{
			return this._maxTilingBakeSize;
		}
		set
		{
			this._maxTilingBakeSize = value;
		}
	}

	public virtual MB2_PackingAlgorithmEnum packingAlgorithm
	{
		get
		{
			return this._packingAlgorithm;
		}
		set
		{
			this._packingAlgorithm = value;
		}
	}

	public bool meshBakerTexturePackerForcePowerOfTwo
	{
		get
		{
			return this._meshBakerTexturePackerForcePowerOfTwo;
		}
		set
		{
			this._meshBakerTexturePackerForcePowerOfTwo = value;
		}
	}

	public virtual List<ShaderTextureProperty> customShaderProperties
	{
		get
		{
			return this._customShaderProperties;
		}
		set
		{
			this._customShaderProperties = value;
		}
	}

	public virtual List<string> customShaderPropNames
	{
		get
		{
			return this._customShaderPropNames_Depricated;
		}
		set
		{
			this._customShaderPropNames_Depricated = value;
		}
	}

	public virtual bool doMultiMaterial
	{
		get
		{
			return this._doMultiMaterial;
		}
		set
		{
			this._doMultiMaterial = value;
		}
	}

	public virtual Material resultMaterial
	{
		get
		{
			return this._resultMaterial;
		}
		set
		{
			this._resultMaterial = value;
		}
	}

	public override List<GameObject> GetObjectsToCombine()
	{
		if (this.objsToMesh == null)
		{
			this.objsToMesh = new List<GameObject>();
		}
		return this.objsToMesh;
	}

	public MB_AtlasesAndRects[] CreateAtlases()
	{
		return this.CreateAtlases(null, false, null);
	}

	public MB_AtlasesAndRects[] CreateAtlases(ProgressUpdateDelegate progressInfo, bool saveAtlasesAsAssets = false, MB2_EditorMethodsInterface editorMethods = null)
	{
		MB_AtlasesAndRects[] array = null;
		try
		{
			array = this._CreateAtlases(progressInfo, saveAtlasesAsAssets, editorMethods);
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
		finally
		{
			if (saveAtlasesAsAssets && array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					MB_AtlasesAndRects mB_AtlasesAndRects = array[i];
					if (mB_AtlasesAndRects != null && mB_AtlasesAndRects.atlases != null)
					{
						for (int j = 0; j < mB_AtlasesAndRects.atlases.Length; j++)
						{
							if (mB_AtlasesAndRects.atlases[j] != null)
							{
								if (editorMethods != null)
								{
									editorMethods.Destroy(mB_AtlasesAndRects.atlases[j]);
								}
								else
								{
									MB_Utility.Destroy(mB_AtlasesAndRects.atlases[j]);
								}
							}
						}
					}
				}
			}
		}
		return array;
	}

	private MB_AtlasesAndRects[] _CreateAtlases(ProgressUpdateDelegate progressInfo, bool saveAtlasesAsAssets = false, MB2_EditorMethodsInterface editorMethods = null)
	{
		if (saveAtlasesAsAssets && editorMethods == null)
		{
			Debug.LogError("Error in CreateAtlases If saveAtlasesAsAssets = true then editorMethods cannot be null.");
			return null;
		}
		if (saveAtlasesAsAssets && !Application.isEditor)
		{
			Debug.LogError("Error in CreateAtlases If saveAtlasesAsAssets = true it must be called from the Unity Editor.");
			return null;
		}
		MB2_ValidationLevel validationLevel = (!Application.isPlaying) ? MB2_ValidationLevel.robust : MB2_ValidationLevel.quick;
		if (!MB3_MeshBakerRoot.DoCombinedValidate(this, MB_ObjsToCombineTypes.dontCare, editorMethods, validationLevel))
		{
			return null;
		}
		if (this._doMultiMaterial && !this._ValidateResultMaterials())
		{
			return null;
		}
		if (!this._doMultiMaterial)
		{
			if (this._resultMaterial == null)
			{
				Debug.LogError("Combined Material is null please create and assign a result material.");
				return null;
			}
			Shader shader = this._resultMaterial.shader;
			for (int i = 0; i < this.objsToMesh.Count; i++)
			{
				Material[] gOMaterials = MB_Utility.GetGOMaterials(this.objsToMesh[i]);
				for (int j = 0; j < gOMaterials.Length; j++)
				{
					Material material = gOMaterials[j];
					if (material != null && material.shader != shader)
					{
						Debug.LogWarning(string.Concat(new object[]
						{
							"Game object ",
							this.objsToMesh[i],
							" does not use shader ",
							shader,
							" it may not have the required textures. If not 2x2 clear textures will be generated."
						}));
					}
				}
			}
		}
		for (int k = 0; k < this.objsToMesh.Count; k++)
		{
			Material[] gOMaterials2 = MB_Utility.GetGOMaterials(this.objsToMesh[k]);
			for (int l = 0; l < gOMaterials2.Length; l++)
			{
				Material x = gOMaterials2[l];
				if (x == null)
				{
					Debug.LogError("Game object " + this.objsToMesh[k] + " has a null material. Can't build atlases");
					return null;
				}
			}
		}
		MB3_TextureCombiner mB3_TextureCombiner = new MB3_TextureCombiner();
		mB3_TextureCombiner.LOG_LEVEL = this.LOG_LEVEL;
		mB3_TextureCombiner.atlasPadding = this._atlasPadding;
		mB3_TextureCombiner.maxAtlasSize = this._maxAtlasSize;
		mB3_TextureCombiner.customShaderPropNames = this._customShaderProperties;
		mB3_TextureCombiner.fixOutOfBoundsUVs = this._fixOutOfBoundsUVs;
		mB3_TextureCombiner.maxTilingBakeSize = this._maxTilingBakeSize;
		mB3_TextureCombiner.packingAlgorithm = this._packingAlgorithm;
		mB3_TextureCombiner.meshBakerTexturePackerForcePowerOfTwo = this._meshBakerTexturePackerForcePowerOfTwo;
		mB3_TextureCombiner.resizePowerOfTwoTextures = this._resizePowerOfTwoTextures;
		mB3_TextureCombiner.saveAtlasesAsAssets = saveAtlasesAsAssets;
		if (!Application.isPlaying)
		{
			Material[] array;
			if (this._doMultiMaterial)
			{
				array = new Material[this.resultMaterials.Length];
				for (int m = 0; m < array.Length; m++)
				{
					array[m] = this.resultMaterials[m].combinedMaterial;
				}
			}
			else
			{
				array = new Material[]
				{
					this._resultMaterial
				};
			}
			mB3_TextureCombiner.SuggestTreatment(this.objsToMesh, array, mB3_TextureCombiner.customShaderPropNames);
		}
		int num = 1;
		if (this._doMultiMaterial)
		{
			num = this.resultMaterials.Length;
		}
		MB_AtlasesAndRects[] array2 = new MB_AtlasesAndRects[num];
		for (int n = 0; n < array2.Length; n++)
		{
			array2[n] = new MB_AtlasesAndRects();
		}
		for (int num2 = 0; num2 < array2.Length; num2++)
		{
			List<Material> allowedMaterialsFilter = null;
			Material material2;
			if (this._doMultiMaterial)
			{
				allowedMaterialsFilter = this.resultMaterials[num2].sourceMaterials;
				material2 = this.resultMaterials[num2].combinedMaterial;
			}
			else
			{
				material2 = this._resultMaterial;
			}
			Debug.Log("Creating atlases for result material " + material2);
			if (!mB3_TextureCombiner.CombineTexturesIntoAtlases(progressInfo, array2[num2], material2, this.objsToMesh, allowedMaterialsFilter, editorMethods))
			{
				return null;
			}
		}
		this.textureBakeResults.combinedMaterialInfo = array2;
		this.textureBakeResults.doMultiMaterial = this._doMultiMaterial;
		this.textureBakeResults.resultMaterial = this._resultMaterial;
		this.textureBakeResults.resultMaterials = this.resultMaterials;
		this.textureBakeResults.fixOutOfBoundsUVs = mB3_TextureCombiner.fixOutOfBoundsUVs;
		this.unpackMat2RectMap(this.textureBakeResults);
		MB3_MeshBakerCommon[] componentsInChildren = base.GetComponentsInChildren<MB3_MeshBakerCommon>();
		for (int num3 = 0; num3 < componentsInChildren.Length; num3++)
		{
			componentsInChildren[num3].textureBakeResults = this.textureBakeResults;
		}
		if (this.LOG_LEVEL >= MB2_LogLevel.info)
		{
			Debug.Log("Created Atlases");
		}
		return array2;
	}

	private void unpackMat2RectMap(MB2_TextureBakeResults resultAtlasesAndRects)
	{
		List<Material> list = new List<Material>();
		List<Rect> list2 = new List<Rect>();
		for (int i = 0; i < resultAtlasesAndRects.combinedMaterialInfo.Length; i++)
		{
			MB_AtlasesAndRects mB_AtlasesAndRects = resultAtlasesAndRects.combinedMaterialInfo[i];
			Dictionary<Material, Rect> mat2rect_map = mB_AtlasesAndRects.mat2rect_map;
			foreach (Material current in mat2rect_map.Keys)
			{
				list.Add(current);
				list2.Add(mat2rect_map[current]);
			}
		}
		resultAtlasesAndRects.materials = list.ToArray();
		resultAtlasesAndRects.prefabUVRects = list2.ToArray();
	}

	public static void ConfigureNewMaterialToMatchOld(Material newMat, Material original)
	{
		if (original == null)
		{
			Debug.LogWarning(string.Concat(new object[]
			{
				"Original material is null, could not copy properties to ",
				newMat,
				". Setting shader to ",
				newMat.shader
			}));
			return;
		}
		newMat.shader = original.shader;
		newMat.CopyPropertiesFromMaterial(original);
		ShaderTextureProperty[] shaderTexPropertyNames = MB3_TextureCombiner.shaderTexPropertyNames;
		for (int i = 0; i < shaderTexPropertyNames.Length; i++)
		{
			Vector2 one = Vector2.one;
			Vector2 zero = Vector2.zero;
			if (newMat.HasProperty(shaderTexPropertyNames[i].name))
			{
				newMat.SetTextureOffset(shaderTexPropertyNames[i].name, zero);
				newMat.SetTextureScale(shaderTexPropertyNames[i].name, one);
			}
		}
	}

	private string PrintSet(HashSet<Material> s)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (Material current in s)
		{
			stringBuilder.Append(current + ",");
		}
		return stringBuilder.ToString();
	}

	private bool _ValidateResultMaterials()
	{
		HashSet<Material> hashSet = new HashSet<Material>();
		for (int i = 0; i < this.objsToMesh.Count; i++)
		{
			if (this.objsToMesh[i] != null)
			{
				Material[] gOMaterials = MB_Utility.GetGOMaterials(this.objsToMesh[i]);
				for (int j = 0; j < gOMaterials.Length; j++)
				{
					if (gOMaterials[j] != null)
					{
						hashSet.Add(gOMaterials[j]);
					}
				}
			}
		}
		HashSet<Material> hashSet2 = new HashSet<Material>();
		for (int k = 0; k < this.resultMaterials.Length; k++)
		{
			MB_MultiMaterial mB_MultiMaterial = this.resultMaterials[k];
			if (mB_MultiMaterial.combinedMaterial == null)
			{
				Debug.LogError("Combined Material is null please create and assign a result material.");
				return false;
			}
			Shader shader = mB_MultiMaterial.combinedMaterial.shader;
			for (int l = 0; l < mB_MultiMaterial.sourceMaterials.Count; l++)
			{
				if (mB_MultiMaterial.sourceMaterials[l] == null)
				{
					Debug.LogError("There are null entries in the list of Source Materials");
					return false;
				}
				if (shader != mB_MultiMaterial.sourceMaterials[l].shader)
				{
					Debug.LogWarning(string.Concat(new object[]
					{
						"Source material ",
						mB_MultiMaterial.sourceMaterials[l],
						" does not use shader ",
						shader,
						" it may not have the required textures. If not empty textures will be generated."
					}));
				}
				if (hashSet2.Contains(mB_MultiMaterial.sourceMaterials[l]))
				{
					Debug.LogError("A Material " + mB_MultiMaterial.sourceMaterials[l] + " appears more than once in the list of source materials in the source material to combined mapping. Each source material must be unique.");
					return false;
				}
				hashSet2.Add(mB_MultiMaterial.sourceMaterials[l]);
			}
		}
		if (hashSet.IsProperSubsetOf(hashSet2))
		{
			hashSet2.ExceptWith(hashSet);
			Debug.LogWarning("There are materials in the mapping that are not used on your source objects: " + this.PrintSet(hashSet2));
		}
		if (hashSet2.IsProperSubsetOf(hashSet))
		{
			hashSet.ExceptWith(hashSet2);
			Debug.LogError("There are materials on the objects to combine that are not in the mapping: " + this.PrintSet(hashSet));
			return false;
		}
		return true;
	}
}
