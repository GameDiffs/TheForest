using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
	[Serializable]
	public class MB3_TextureCombiner
	{
		public class MeshBakerMaterialTexture
		{
			public Vector2 offset = new Vector2(0f, 0f);

			public Vector2 scale = new Vector2(1f, 1f);

			public Vector2 obUVoffset = new Vector2(0f, 0f);

			public Vector2 obUVscale = new Vector2(1f, 1f);

			public Texture2D t;

			public Color colorIfNoTexture;

			public Color tintColor;

			public MeshBakerMaterialTexture()
			{
			}

			public MeshBakerMaterialTexture(Texture2D tx)
			{
				this.t = tx;
			}

			public MeshBakerMaterialTexture(Texture2D tx, Vector2 o, Vector2 s, Vector2 oUV, Vector2 sUV, Color c, Color tColor)
			{
				this.t = tx;
				this.offset = o;
				this.scale = s;
				this.obUVoffset = oUV;
				this.obUVscale = sUV;
				this.colorIfNoTexture = c;
				this.tintColor = tColor;
			}
		}

		public class MB_TexSet
		{
			public MB3_TextureCombiner.MeshBakerMaterialTexture[] ts;

			public List<Material> mats;

			public List<GameObject> gos;

			public int idealWidth;

			public int idealHeight;

			public MB_TexSet(MB3_TextureCombiner.MeshBakerMaterialTexture[] tss)
			{
				this.ts = tss;
				this.mats = new List<Material>();
				this.gos = new List<GameObject>();
			}

			public bool IsEqual(object obj, bool fixOutOfBoundsUVs)
			{
				if (!(obj is MB3_TextureCombiner.MB_TexSet))
				{
					return false;
				}
				MB3_TextureCombiner.MB_TexSet mB_TexSet = (MB3_TextureCombiner.MB_TexSet)obj;
				if (mB_TexSet.ts.Length != this.ts.Length)
				{
					return false;
				}
				for (int i = 0; i < this.ts.Length; i++)
				{
					if (this.ts[i].offset != mB_TexSet.ts[i].offset)
					{
						return false;
					}
					if (this.ts[i].scale != mB_TexSet.ts[i].scale)
					{
						return false;
					}
					if (this.ts[i].t != mB_TexSet.ts[i].t)
					{
						return false;
					}
					if (this.ts[i].colorIfNoTexture != mB_TexSet.ts[i].colorIfNoTexture)
					{
						return false;
					}
					if (fixOutOfBoundsUVs && this.ts[i].obUVoffset != mB_TexSet.ts[i].obUVoffset)
					{
						return false;
					}
					if (fixOutOfBoundsUVs && this.ts[i].obUVscale != mB_TexSet.ts[i].obUVscale)
					{
						return false;
					}
				}
				return true;
			}
		}

		public MB2_LogLevel LOG_LEVEL = MB2_LogLevel.info;

		public static ShaderTextureProperty[] shaderTexPropertyNames = new ShaderTextureProperty[]
		{
			new ShaderTextureProperty("_MainTex", false),
			new ShaderTextureProperty("_BumpMap", true),
			new ShaderTextureProperty("_Normal", true),
			new ShaderTextureProperty("_BumpSpecMap", false),
			new ShaderTextureProperty("_DecalTex", false),
			new ShaderTextureProperty("_Detail", false),
			new ShaderTextureProperty("_GlossMap", false),
			new ShaderTextureProperty("_Illum", false),
			new ShaderTextureProperty("_LightTextureB0", false),
			new ShaderTextureProperty("_ParallaxMap", false),
			new ShaderTextureProperty("_ShadowOffset", false),
			new ShaderTextureProperty("_TranslucencyMap", false),
			new ShaderTextureProperty("_SpecMap", false),
			new ShaderTextureProperty("_SpecGlossMap", false),
			new ShaderTextureProperty("_TranspMap", false),
			new ShaderTextureProperty("_MetallicGlossMap", false),
			new ShaderTextureProperty("_OcclusionMap", false),
			new ShaderTextureProperty("_EmissionMap", false),
			new ShaderTextureProperty("_DetailMask", false)
		};

		[SerializeField]
		protected MB2_TextureBakeResults _textureBakeResults;

		[SerializeField]
		protected int _atlasPadding = 1;

		[SerializeField]
		protected int _maxAtlasSize = 1;

		[SerializeField]
		protected bool _resizePowerOfTwoTextures;

		[SerializeField]
		protected bool _fixOutOfBoundsUVs;

		[SerializeField]
		protected int _maxTilingBakeSize = 1024;

		[SerializeField]
		protected bool _saveAtlasesAsAssets;

		[SerializeField]
		protected MB2_PackingAlgorithmEnum _packingAlgorithm;

		[SerializeField]
		protected bool _meshBakerTexturePackerForcePowerOfTwo = true;

		[SerializeField]
		protected List<ShaderTextureProperty> _customShaderPropNames = new List<ShaderTextureProperty>();

		protected List<Texture2D> _temporaryTextures = new List<Texture2D>();

		public MB2_TextureBakeResults textureBakeResults
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

		public int atlasPadding
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

		public int maxAtlasSize
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

		public bool resizePowerOfTwoTextures
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

		public bool fixOutOfBoundsUVs
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

		public int maxTilingBakeSize
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

		public bool saveAtlasesAsAssets
		{
			get
			{
				return this._saveAtlasesAsAssets;
			}
			set
			{
				this._saveAtlasesAsAssets = value;
			}
		}

		public MB2_PackingAlgorithmEnum packingAlgorithm
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

		public List<ShaderTextureProperty> customShaderPropNames
		{
			get
			{
				return this._customShaderPropNames;
			}
			set
			{
				this._customShaderPropNames = value;
			}
		}

		public bool CombineTexturesIntoAtlases(ProgressUpdateDelegate progressInfo, MB_AtlasesAndRects resultAtlasesAndRects, Material resultMaterial, List<GameObject> objsToMesh, List<Material> allowedMaterialsFilter, MB2_EditorMethodsInterface textureEditorMethods = null)
		{
			return this._CombineTexturesIntoAtlases(progressInfo, resultAtlasesAndRects, resultMaterial, objsToMesh, allowedMaterialsFilter, textureEditorMethods);
		}

		private bool _CollectPropertyNames(Material resultMaterial, List<ShaderTextureProperty> texPropertyNames)
		{
			MB3_TextureCombiner.<_CollectPropertyNames>c__AnonStorey26A <_CollectPropertyNames>c__AnonStorey26A = new MB3_TextureCombiner.<_CollectPropertyNames>c__AnonStorey26A();
			<_CollectPropertyNames>c__AnonStorey26A.texPropertyNames = texPropertyNames;
			int i;
			for (i = 0; i < <_CollectPropertyNames>c__AnonStorey26A.texPropertyNames.Count; i++)
			{
				ShaderTextureProperty shaderTextureProperty = this._customShaderPropNames.Find((ShaderTextureProperty x) => x.name.Equals(<_CollectPropertyNames>c__AnonStorey26A.texPropertyNames[i].name));
				if (shaderTextureProperty != null)
				{
					this._customShaderPropNames.Remove(shaderTextureProperty);
				}
			}
			if (resultMaterial == null)
			{
				UnityEngine.Debug.LogError("Please assign a result material. The combined mesh will use this material.");
				return false;
			}
			string str = string.Empty;
			for (int k = 0; k < MB3_TextureCombiner.shaderTexPropertyNames.Length; k++)
			{
				if (resultMaterial.HasProperty(MB3_TextureCombiner.shaderTexPropertyNames[k].name))
				{
					str = str + ", " + MB3_TextureCombiner.shaderTexPropertyNames[k].name;
					if (!<_CollectPropertyNames>c__AnonStorey26A.texPropertyNames.Contains(MB3_TextureCombiner.shaderTexPropertyNames[k]))
					{
						<_CollectPropertyNames>c__AnonStorey26A.texPropertyNames.Add(MB3_TextureCombiner.shaderTexPropertyNames[k]);
					}
					if (resultMaterial.GetTextureOffset(MB3_TextureCombiner.shaderTexPropertyNames[k].name) != new Vector2(0f, 0f) && this.LOG_LEVEL >= MB2_LogLevel.warn)
					{
						UnityEngine.Debug.LogWarning("Result material has non-zero offset. This is may be incorrect.");
					}
					if (resultMaterial.GetTextureScale(MB3_TextureCombiner.shaderTexPropertyNames[k].name) != new Vector2(1f, 1f) && this.LOG_LEVEL >= MB2_LogLevel.warn)
					{
						UnityEngine.Debug.LogWarning("Result material should have tiling of 1,1");
					}
				}
			}
			for (int j = 0; j < this._customShaderPropNames.Count; j++)
			{
				if (resultMaterial.HasProperty(this._customShaderPropNames[j].name))
				{
					str = str + ", " + this._customShaderPropNames[j].name;
					<_CollectPropertyNames>c__AnonStorey26A.texPropertyNames.Add(this._customShaderPropNames[j]);
					if (resultMaterial.GetTextureOffset(this._customShaderPropNames[j].name) != new Vector2(0f, 0f) && this.LOG_LEVEL >= MB2_LogLevel.warn)
					{
						UnityEngine.Debug.LogWarning("Result material has non-zero offset. This is probably incorrect.");
					}
					if (resultMaterial.GetTextureScale(this._customShaderPropNames[j].name) != new Vector2(1f, 1f) && this.LOG_LEVEL >= MB2_LogLevel.warn)
					{
						UnityEngine.Debug.LogWarning("Result material should probably have tiling of 1,1.");
					}
				}
				else if (this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					UnityEngine.Debug.LogWarning("Result material shader does not use property " + this._customShaderPropNames[j].name + " in the list of custom shader property names");
				}
			}
			return true;
		}

		private bool _CombineTexturesIntoAtlases(ProgressUpdateDelegate progressInfo, MB_AtlasesAndRects resultAtlasesAndRects, Material resultMaterial, List<GameObject> objsToMesh, List<Material> allowedMaterialsFilter, MB2_EditorMethodsInterface textureEditorMethods)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			bool result = false;
			try
			{
				this._temporaryTextures.Clear();
				if (textureEditorMethods != null)
				{
					textureEditorMethods.Clear();
				}
				if (objsToMesh == null || objsToMesh.Count == 0)
				{
					UnityEngine.Debug.LogError("No meshes to combine. Please assign some meshes to combine.");
					bool result2 = false;
					return result2;
				}
				if (this._atlasPadding < 0)
				{
					UnityEngine.Debug.LogError("Atlas padding must be zero or greater.");
					bool result2 = false;
					return result2;
				}
				if (this._maxTilingBakeSize < 2 || this._maxTilingBakeSize > 4096)
				{
					UnityEngine.Debug.LogError("Invalid value for max tiling bake size.");
					bool result2 = false;
					return result2;
				}
				if (progressInfo != null)
				{
					progressInfo("Collecting textures for " + objsToMesh.Count + " meshes.", 0.01f);
				}
				List<ShaderTextureProperty> texPropertyNames = new List<ShaderTextureProperty>();
				if (!this._CollectPropertyNames(resultMaterial, texPropertyNames))
				{
					bool result2 = false;
					return result2;
				}
				result = this.__CombineTexturesIntoAtlases(progressInfo, resultAtlasesAndRects, resultMaterial, texPropertyNames, objsToMesh, allowedMaterialsFilter, textureEditorMethods);
			}
			catch (MissingReferenceException message)
			{
				UnityEngine.Debug.LogError("Creating atlases failed a MissingReferenceException was thrown. This is normally only happens when trying to create very large atlases and Unity is running out of Memory. Try changing the 'Texture Packer' to a different option, it may work with an alternate packer. This error is sometimes intermittant. Try baking again.");
				UnityEngine.Debug.LogError(message);
			}
			catch (Exception message2)
			{
				UnityEngine.Debug.LogError(message2);
			}
			finally
			{
				this._destroyTemporaryTextures();
				if (textureEditorMethods != null)
				{
					textureEditorMethods.SetReadFlags(progressInfo);
				}
				if (this.LOG_LEVEL >= MB2_LogLevel.debug)
				{
					UnityEngine.Debug.Log("Total time to create atlases " + stopwatch.ElapsedMilliseconds.ToString("f5"));
				}
			}
			return result;
		}

		private bool __CombineTexturesIntoAtlases(ProgressUpdateDelegate progressInfo, MB_AtlasesAndRects resultAtlasesAndRects, Material resultMaterial, List<ShaderTextureProperty> texPropertyNames, List<GameObject> objsToMesh, List<Material> allowedMaterialsFilter, MB2_EditorMethodsInterface textureEditorMethods)
		{
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				UnityEngine.Debug.Log(string.Concat(new object[]
				{
					"__CombineTexturesIntoAtlases atlases:",
					texPropertyNames.Count,
					" objsToMesh:",
					objsToMesh.Count,
					" _fixOutOfBoundsUVs:",
					this._fixOutOfBoundsUVs
				}));
			}
			if (progressInfo != null)
			{
				progressInfo("Collecting textures ", 0.01f);
			}
			List<MB3_TextureCombiner.MB_TexSet> list = new List<MB3_TextureCombiner.MB_TexSet>();
			List<GameObject> usedObjsToMesh = new List<GameObject>();
			if (!this.__Step1_CollectDistinctMatTexturesAndUsedObjects(objsToMesh, allowedMaterialsFilter, texPropertyNames, textureEditorMethods, list, usedObjsToMesh))
			{
				return false;
			}
			if (MB3_MeshCombiner.EVAL_VERSION)
			{
				bool flag = true;
				for (int i = 0; i < list.Count; i++)
				{
					for (int j = 0; j < list[i].mats.Count; j++)
					{
						if (!list[i].mats[j].shader.name.EndsWith("Diffuse") && !list[i].mats[j].shader.name.EndsWith("Bumped Diffuse"))
						{
							UnityEngine.Debug.LogError("The free version of Mesh Baker only works with Diffuse and Bumped Diffuse Shaders. The full version can be used with any shader. Material " + list[i].mats[j].name + " uses shader " + list[i].mats[j].shader.name);
							flag = false;
						}
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			bool[] allTexturesAreNullAndSameColor = new bool[texPropertyNames.Count];
			int padding = this.__Step2_CalculateIdealSizesForTexturesInAtlasAndPadding(list, texPropertyNames, allTexturesAreNullAndSameColor);
			this.__Step3_BuildAndSaveAtlasesAndStoreResults(progressInfo, list, texPropertyNames, allTexturesAreNullAndSameColor, padding, textureEditorMethods, resultAtlasesAndRects, resultMaterial);
			return true;
		}

		private bool __Step1_CollectDistinctMatTexturesAndUsedObjects(List<GameObject> allObjsToMesh, List<Material> allowedMaterialsFilter, List<ShaderTextureProperty> texPropertyNames, MB2_EditorMethodsInterface textureEditorMethods, List<MB3_TextureCombiner.MB_TexSet> distinctMaterialTextures, List<GameObject> usedObjsToMesh)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			bool flag = false;
			Dictionary<int, MB_Utility.MeshAnalysisResult[]> dictionary = new Dictionary<int, MB_Utility.MeshAnalysisResult[]>();
			for (int i = 0; i < allObjsToMesh.Count; i++)
			{
				GameObject gameObject = allObjsToMesh[i];
				if (this.LOG_LEVEL >= MB2_LogLevel.debug)
				{
					UnityEngine.Debug.Log("Collecting textures for object " + gameObject);
				}
				if (gameObject == null)
				{
					UnityEngine.Debug.LogError("The list of objects to mesh contained nulls.");
					return false;
				}
				Mesh mesh = MB_Utility.GetMesh(gameObject);
				if (mesh == null)
				{
					UnityEngine.Debug.LogError("Object " + gameObject.name + " in the list of objects to mesh has no mesh.");
					return false;
				}
				Material[] gOMaterials = MB_Utility.GetGOMaterials(gameObject);
				if (gOMaterials == null)
				{
					UnityEngine.Debug.LogError("Object " + gameObject.name + " in the list of objects has no materials.");
					return false;
				}
				MB_Utility.MeshAnalysisResult[] array;
				if (!dictionary.TryGetValue(mesh.GetInstanceID(), out array))
				{
					array = new MB_Utility.MeshAnalysisResult[mesh.subMeshCount];
					for (int j = 0; j < mesh.subMeshCount; j++)
					{
						Rect rect = default(Rect);
						MB_Utility.hasOutOfBoundsUVs(mesh, ref rect, ref array[j], j);
					}
					dictionary.Add(mesh.GetInstanceID(), array);
				}
				for (int k = 0; k < gOMaterials.Length; k++)
				{
					Material material = gOMaterials[k];
					if (allowedMaterialsFilter == null || allowedMaterialsFilter.Contains(material))
					{
						flag = (flag || array[k].hasOutOfBoundsUVs);
						if (material.name.Contains("(Instance)"))
						{
							UnityEngine.Debug.LogError("The sharedMaterial on object " + gameObject.name + " has been 'Instanced'. This was probably caused by a script accessing the meshRender.material property in the editor.  The material to UV Rectangle mapping will be incorrect. To fix this recreate the object from its prefab or re-assign its material from the correct asset.");
							return false;
						}
						if (this._fixOutOfBoundsUVs && !MB_Utility.AreAllSharedMaterialsDistinct(gOMaterials) && this.LOG_LEVEL >= MB2_LogLevel.warn)
						{
							UnityEngine.Debug.LogWarning("Object " + gameObject.name + " uses the same material on multiple submeshes. This may generate strange resultAtlasesAndRects especially when used with fix out of bounds uvs. Try duplicating the material.");
						}
						MB3_TextureCombiner.MeshBakerMaterialTexture[] array2 = new MB3_TextureCombiner.MeshBakerMaterialTexture[texPropertyNames.Count];
						for (int l = 0; l < texPropertyNames.Count; l++)
						{
							Texture2D texture2D = null;
							Vector2 s = Vector2.one;
							Vector2 o = Vector2.zero;
							Vector2 one = Vector2.one;
							Vector2 zero = Vector2.zero;
							Color c = Color.clear;
							Color colorIfNoTexture = this.GetColorIfNoTexture(material, texPropertyNames[l]);
							if (material.HasProperty(texPropertyNames[l].name))
							{
								Texture texture = material.GetTexture(texPropertyNames[l].name);
								if (texture != null)
								{
									if (!(texture is Texture2D))
									{
										UnityEngine.Debug.LogError("Object " + gameObject.name + " in the list of objects to mesh uses a Texture that is not a Texture2D. Cannot build atlases.");
										return false;
									}
									texture2D = (Texture2D)texture;
									TextureFormat format = texture2D.format;
									bool flag2 = false;
									if (!Application.isPlaying && textureEditorMethods != null)
									{
										flag2 = textureEditorMethods.IsNormalMap(texture2D);
									}
									if ((format != TextureFormat.ARGB32 && format != TextureFormat.RGBA32 && format != TextureFormat.BGRA32 && format != TextureFormat.RGB24 && format != TextureFormat.Alpha8) || flag2)
									{
										if (Application.isPlaying && this._packingAlgorithm != MB2_PackingAlgorithmEnum.MeshBakerTexturePacker_Fast)
										{
											UnityEngine.Debug.LogError(string.Concat(new object[]
											{
												"Object ",
												gameObject.name,
												" in the list of objects to mesh uses Texture ",
												texture2D.name,
												" uses format ",
												format,
												" that is not in: ARGB32, RGBA32, BGRA32, RGB24, Alpha8 or DXT. These textures cannot be resized at runtime. Try changing texture format. If format says 'compressed' try changing it to 'truecolor'"
											}));
											return false;
										}
										if (textureEditorMethods != null && (this._packingAlgorithm != MB2_PackingAlgorithmEnum.MeshBakerTexturePacker_Fast || flag2))
										{
											textureEditorMethods.AddTextureFormat(texture2D, flag2);
										}
										texture2D = (Texture2D)material.GetTexture(texPropertyNames[l].name);
									}
								}
								else
								{
									c = colorIfNoTexture;
								}
								o = material.GetTextureOffset(texPropertyNames[l].name);
								s = material.GetTextureScale(texPropertyNames[l].name);
							}
							else
							{
								c = colorIfNoTexture;
							}
							if (array[k].hasOutOfBoundsUVs)
							{
								one = new Vector2(array[k].uvRect.width, array[k].uvRect.height);
								zero = new Vector2(array[k].uvRect.x, array[k].uvRect.y);
							}
							array2[l] = new MB3_TextureCombiner.MeshBakerMaterialTexture(texture2D, o, s, zero, one, c, colorIfNoTexture);
						}
						MB3_TextureCombiner.MB_TexSet setOfTexs = new MB3_TextureCombiner.MB_TexSet(array2);
						MB3_TextureCombiner.MB_TexSet mB_TexSet = distinctMaterialTextures.Find((MB3_TextureCombiner.MB_TexSet x) => x.IsEqual(setOfTexs, this._fixOutOfBoundsUVs));
						if (mB_TexSet != null)
						{
							setOfTexs = mB_TexSet;
						}
						else
						{
							distinctMaterialTextures.Add(setOfTexs);
						}
						if (!setOfTexs.mats.Contains(material))
						{
							setOfTexs.mats.Add(material);
						}
						if (!setOfTexs.gos.Contains(gameObject))
						{
							setOfTexs.gos.Add(gameObject);
							if (!usedObjsToMesh.Contains(gameObject))
							{
								usedObjsToMesh.Add(gameObject);
							}
						}
					}
				}
			}
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				UnityEngine.Debug.Log("Total time Step1_CollectDistinctTextures " + stopwatch.ElapsedMilliseconds.ToString("f5"));
			}
			return true;
		}

		private int __Step2_CalculateIdealSizesForTexturesInAtlasAndPadding(List<MB3_TextureCombiner.MB_TexSet> distinctMaterialTextures, List<ShaderTextureProperty> texPropertyNames, bool[] allTexturesAreNullAndSameColor)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			int num = this._atlasPadding;
			if (distinctMaterialTextures.Count == 1 && !this._fixOutOfBoundsUVs)
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.info)
				{
					UnityEngine.Debug.Log("All objects use the same textures in this set of atlases. Original textures will be reused instead of creating atlases.");
				}
				num = 0;
			}
			else
			{
				if (allTexturesAreNullAndSameColor.Length != texPropertyNames.Count)
				{
					UnityEngine.Debug.LogError("allTexturesAreNullAndSameColor array must be the same length of texPropertyNames.");
				}
				for (int i = 0; i < distinctMaterialTextures.Count; i++)
				{
					if (this.LOG_LEVEL >= MB2_LogLevel.debug)
					{
						UnityEngine.Debug.Log(string.Concat(new object[]
						{
							"Calculating ideal sizes for texSet TexSet ",
							i,
							" of ",
							distinctMaterialTextures.Count
						}));
					}
					MB3_TextureCombiner.MB_TexSet mB_TexSet = distinctMaterialTextures[i];
					mB_TexSet.idealWidth = 1;
					mB_TexSet.idealHeight = 1;
					int num2 = 1;
					int num3 = 1;
					if (mB_TexSet.ts.Length != texPropertyNames.Count)
					{
						UnityEngine.Debug.LogError("length of arrays in each element of distinctMaterialTextures must be texPropertyNames.Count");
					}
					for (int j = 0; j < texPropertyNames.Count; j++)
					{
						MB3_TextureCombiner.MeshBakerMaterialTexture meshBakerMaterialTexture = mB_TexSet.ts[j];
						if (!meshBakerMaterialTexture.scale.Equals(Vector2.one) && distinctMaterialTextures.Count > 1 && this.LOG_LEVEL >= MB2_LogLevel.warn)
						{
							UnityEngine.Debug.LogWarning(string.Concat(new object[]
							{
								"Texture ",
								meshBakerMaterialTexture.t,
								"is tiled by ",
								meshBakerMaterialTexture.scale,
								" tiling will be baked into a texture with maxSize:",
								this._maxTilingBakeSize
							}));
						}
						if (!meshBakerMaterialTexture.obUVscale.Equals(Vector2.one) && distinctMaterialTextures.Count > 1 && this._fixOutOfBoundsUVs && this.LOG_LEVEL >= MB2_LogLevel.warn)
						{
							UnityEngine.Debug.LogWarning(string.Concat(new object[]
							{
								"Texture ",
								meshBakerMaterialTexture.t,
								"has out of bounds UVs that effectively tile by ",
								meshBakerMaterialTexture.obUVscale,
								" tiling will be baked into a texture with maxSize:",
								this._maxTilingBakeSize
							}));
						}
						if (meshBakerMaterialTexture.t != null)
						{
							Vector2 adjustedForScaleAndOffset2Dimensions = this.GetAdjustedForScaleAndOffset2Dimensions(meshBakerMaterialTexture);
							if ((int)(adjustedForScaleAndOffset2Dimensions.x * adjustedForScaleAndOffset2Dimensions.y) > num2 * num3)
							{
								if (this.LOG_LEVEL >= MB2_LogLevel.trace)
								{
									UnityEngine.Debug.Log(string.Concat(new object[]
									{
										"    matTex ",
										meshBakerMaterialTexture.t,
										" ",
										adjustedForScaleAndOffset2Dimensions,
										" has a bigger size than ",
										num2,
										" ",
										num3
									}));
								}
								num2 = (int)adjustedForScaleAndOffset2Dimensions.x;
								num3 = (int)adjustedForScaleAndOffset2Dimensions.y;
							}
						}
					}
					if (this._resizePowerOfTwoTextures)
					{
						if (this.IsPowerOfTwo(num2))
						{
							num2 -= num * 2;
						}
						if (this.IsPowerOfTwo(num3))
						{
							num3 -= num * 2;
						}
						if (num2 < 1)
						{
							num2 = 1;
						}
						if (num3 < 1)
						{
							num3 = 1;
						}
					}
					if (this.LOG_LEVEL >= MB2_LogLevel.trace)
					{
						UnityEngine.Debug.Log(string.Concat(new object[]
						{
							"    Ideal size is ",
							num2,
							" ",
							num3
						}));
					}
					mB_TexSet.idealWidth = num2;
					mB_TexSet.idealHeight = num3;
				}
			}
			for (int k = 0; k < texPropertyNames.Count; k++)
			{
				bool flag = true;
				bool flag2 = true;
				for (int l = 0; l < distinctMaterialTextures.Count; l++)
				{
					if (distinctMaterialTextures[l].ts[k].t != null)
					{
						flag = false;
						break;
					}
					for (int m = l + 1; m < distinctMaterialTextures.Count; m++)
					{
						if (distinctMaterialTextures[l].ts[k].colorIfNoTexture != distinctMaterialTextures[m].ts[k].colorIfNoTexture)
						{
							flag2 = false;
						}
					}
				}
				allTexturesAreNullAndSameColor[k] = (flag && flag2);
			}
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				UnityEngine.Debug.Log("Total time Step2 Calculate Ideal Sizes " + stopwatch.ElapsedMilliseconds.ToString("f5"));
			}
			return num;
		}

		private void __Step3_BuildAndSaveAtlasesAndStoreResults(ProgressUpdateDelegate progressInfo, List<MB3_TextureCombiner.MB_TexSet> distinctMaterialTextures, List<ShaderTextureProperty> texPropertyNames, bool[] allTexturesAreNullAndSameColor, int _padding, MB2_EditorMethodsInterface textureEditorMethods, MB_AtlasesAndRects resultAtlasesAndRects, Material resultMaterial)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			int count = texPropertyNames.Count;
			StringBuilder stringBuilder = new StringBuilder();
			if (count > 0)
			{
				stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("Report");
				for (int i = 0; i < distinctMaterialTextures.Count; i++)
				{
					MB3_TextureCombiner.MB_TexSet mB_TexSet = distinctMaterialTextures[i];
					stringBuilder.AppendLine("----------");
					stringBuilder.Append(string.Concat(new object[]
					{
						"This set of textures will be resized to:",
						mB_TexSet.idealWidth,
						"x",
						mB_TexSet.idealHeight,
						"\n"
					}));
					for (int j = 0; j < mB_TexSet.ts.Length; j++)
					{
						if (mB_TexSet.ts[j].t != null)
						{
							stringBuilder.Append(string.Concat(new object[]
							{
								"   [",
								texPropertyNames[j].name,
								" ",
								mB_TexSet.ts[j].t.name,
								" ",
								mB_TexSet.ts[j].t.width,
								"x",
								mB_TexSet.ts[j].t.height,
								"]"
							}));
							if (mB_TexSet.ts[j].scale != Vector2.one || mB_TexSet.ts[j].offset != Vector2.zero)
							{
								stringBuilder.AppendFormat(" material scale {0} offset{1} ", mB_TexSet.ts[j].scale.ToString("G4"), mB_TexSet.ts[j].offset.ToString("G4"));
							}
							if (mB_TexSet.ts[j].obUVscale != Vector2.one || mB_TexSet.ts[j].obUVoffset != Vector2.zero)
							{
								stringBuilder.AppendFormat(" obUV scale {0} offset{1} ", mB_TexSet.ts[j].obUVscale.ToString("G4"), mB_TexSet.ts[j].obUVoffset.ToString("G4"));
							}
							stringBuilder.AppendLine(string.Empty);
						}
						else
						{
							stringBuilder.Append("   [" + texPropertyNames[j].name + " null ");
							if (allTexturesAreNullAndSameColor[j])
							{
								stringBuilder.Append("no atlas will be created all textures null]\n");
							}
							else
							{
								stringBuilder.AppendFormat("a 16x16 texture will be created with color {0}]\n", mB_TexSet.ts[j].colorIfNoTexture);
							}
						}
					}
					stringBuilder.AppendLine(string.Empty);
					stringBuilder.Append("Materials using:");
					for (int k = 0; k < mB_TexSet.mats.Count; k++)
					{
						stringBuilder.Append(mB_TexSet.mats[k].name + ", ");
					}
					stringBuilder.AppendLine(string.Empty);
				}
			}
			if (progressInfo != null)
			{
				progressInfo("Creating txture atlases.", 0.1f);
			}
			GC.Collect();
			Texture2D[] array = new Texture2D[count];
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				UnityEngine.Debug.Log("time Step 3 Create And Save Atlases part 1 " + stopwatch.ElapsedMilliseconds.ToString("f5"));
			}
			Rect[] array2;
			if (this._packingAlgorithm == MB2_PackingAlgorithmEnum.UnitysPackTextures)
			{
				array2 = this.__CreateAtlasesUnityTexturePacker(progressInfo, count, distinctMaterialTextures, texPropertyNames, allTexturesAreNullAndSameColor, resultMaterial, array, textureEditorMethods, _padding);
			}
			else if (this._packingAlgorithm == MB2_PackingAlgorithmEnum.MeshBakerTexturePacker)
			{
				array2 = this.__CreateAtlasesMBTexturePacker(progressInfo, count, distinctMaterialTextures, texPropertyNames, allTexturesAreNullAndSameColor, resultMaterial, array, textureEditorMethods, _padding);
			}
			else
			{
				array2 = this.__CreateAtlasesMBTexturePackerFast(progressInfo, count, distinctMaterialTextures, texPropertyNames, allTexturesAreNullAndSameColor, resultMaterial, array, textureEditorMethods, _padding);
			}
			float num = (float)stopwatch.ElapsedMilliseconds;
			this.AdjustNonTextureProperties(resultMaterial, texPropertyNames, distinctMaterialTextures, textureEditorMethods);
			if (progressInfo != null)
			{
				progressInfo("Building Report", 0.7f);
			}
			StringBuilder stringBuilder2 = new StringBuilder();
			stringBuilder2.AppendLine("---- Atlases ------");
			for (int l = 0; l < count; l++)
			{
				if (array[l] != null)
				{
					stringBuilder2.AppendLine(string.Concat(new object[]
					{
						"Created Atlas For: ",
						texPropertyNames[l].name,
						" h=",
						array[l].height,
						" w=",
						array[l].width
					}));
				}
				else if (allTexturesAreNullAndSameColor[l])
				{
					stringBuilder2.AppendLine("Did not create atlas for " + texPropertyNames[l].name + " because all source textures were null.");
				}
			}
			stringBuilder.Append(stringBuilder2.ToString());
			Dictionary<Material, Rect> dictionary = new Dictionary<Material, Rect>();
			for (int m = 0; m < distinctMaterialTextures.Count; m++)
			{
				List<Material> mats = distinctMaterialTextures[m].mats;
				for (int n = 0; n < mats.Count; n++)
				{
					if (!dictionary.ContainsKey(mats[n]))
					{
						dictionary.Add(mats[n], array2[m]);
					}
				}
			}
			resultAtlasesAndRects.atlases = array;
			resultAtlasesAndRects.texPropertyNames = ShaderTextureProperty.GetNames(texPropertyNames);
			resultAtlasesAndRects.mat2rect_map = dictionary;
			if (progressInfo != null)
			{
				progressInfo("Restoring Texture Formats & Read Flags", 0.8f);
			}
			this._destroyTemporaryTextures();
			if (textureEditorMethods != null)
			{
				textureEditorMethods.SetReadFlags(progressInfo);
			}
			if (stringBuilder != null && this.LOG_LEVEL >= MB2_LogLevel.info)
			{
				UnityEngine.Debug.Log(stringBuilder.ToString());
			}
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				UnityEngine.Debug.Log("Time Step 3 Create And Save Atlases part 3 " + ((float)stopwatch.ElapsedMilliseconds - num).ToString("f5"));
			}
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				UnityEngine.Debug.Log("Total time Step 3 Create And Save Atlases " + stopwatch.ElapsedMilliseconds.ToString("f5"));
			}
		}

		private Rect[] __CreateAtlasesMBTexturePacker(ProgressUpdateDelegate progressInfo, int numAtlases, List<MB3_TextureCombiner.MB_TexSet> distinctMaterialTextures, List<ShaderTextureProperty> texPropertyNames, bool[] allTexturesAreNullAndSameColor, Material resultMaterial, Texture2D[] atlases, MB2_EditorMethodsInterface textureEditorMethods, int _padding)
		{
			Rect[] array;
			if (distinctMaterialTextures.Count == 1 && !this._fixOutOfBoundsUVs)
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.debug)
				{
					UnityEngine.Debug.Log("Only one image per atlas. Will re-use original texture");
				}
				array = new Rect[]
				{
					new Rect(0f, 0f, 1f, 1f)
				};
				for (int i = 0; i < numAtlases; i++)
				{
					MB3_TextureCombiner.MeshBakerMaterialTexture meshBakerMaterialTexture = distinctMaterialTextures[0].ts[i];
					atlases[i] = meshBakerMaterialTexture.t;
					resultMaterial.SetTexture(texPropertyNames[i].name, atlases[i]);
					resultMaterial.SetTextureScale(texPropertyNames[i].name, meshBakerMaterialTexture.scale);
					resultMaterial.SetTextureOffset(texPropertyNames[i].name, meshBakerMaterialTexture.offset);
				}
			}
			else
			{
				List<Vector2> list = new List<Vector2>();
				for (int j = 0; j < distinctMaterialTextures.Count; j++)
				{
					list.Add(new Vector2((float)distinctMaterialTextures[j].idealWidth, (float)distinctMaterialTextures[j].idealHeight));
				}
				MB2_TexturePacker mB2_TexturePacker = new MB2_TexturePacker();
				mB2_TexturePacker.doPowerOfTwoTextures = this._meshBakerTexturePackerForcePowerOfTwo;
				int num = 1;
				int num2 = 1;
				int maxAtlasSize = this._maxAtlasSize;
				array = mB2_TexturePacker.GetRects(list, maxAtlasSize, _padding, out num, out num2);
				if (this.LOG_LEVEL >= MB2_LogLevel.debug)
				{
					UnityEngine.Debug.Log(string.Concat(new object[]
					{
						"Generated atlas will be ",
						num,
						"x",
						num2,
						" (Max atlas size for platform: ",
						maxAtlasSize,
						")"
					}));
				}
				for (int k = 0; k < numAtlases; k++)
				{
					Texture2D texture2D;
					if (allTexturesAreNullAndSameColor[k])
					{
						texture2D = null;
						if (this.LOG_LEVEL >= MB2_LogLevel.debug)
						{
							UnityEngine.Debug.Log("Not creating atlas for " + texPropertyNames[k].name + " because textures are null and default value parameters are the same.");
						}
					}
					else
					{
						GC.Collect();
						if (progressInfo != null)
						{
							progressInfo("Creating Atlas '" + texPropertyNames[k].name + "'", 0.01f);
						}
						Color[][] array2 = new Color[num2][];
						for (int l = 0; l < array2.Length; l++)
						{
							array2[l] = new Color[num];
						}
						bool isNormalMap = false;
						if (texPropertyNames[k].isNormalMap)
						{
							isNormalMap = true;
						}
						for (int m = 0; m < distinctMaterialTextures.Count; m++)
						{
							if (this.LOG_LEVEL >= MB2_LogLevel.trace)
							{
								MB2_Log.Trace("Adding texture {0} to atlas {1}", new object[]
								{
									(!(distinctMaterialTextures[m].ts[k].t == null)) ? distinctMaterialTextures[m].ts[k].t.ToString() : "null",
									texPropertyNames[k]
								});
							}
							Rect rect = array[m];
							Texture2D t = distinctMaterialTextures[m].ts[k].t;
							int targX = Mathf.RoundToInt(rect.x * (float)num);
							int targY = Mathf.RoundToInt(rect.y * (float)num2);
							int num3 = Mathf.RoundToInt(rect.width * (float)num);
							int num4 = Mathf.RoundToInt(rect.height * (float)num2);
							if (num3 == 0 || num4 == 0)
							{
								UnityEngine.Debug.LogError("Image in atlas has no height or width");
							}
							if (textureEditorMethods != null)
							{
								textureEditorMethods.SetReadWriteFlag(t, true, true);
							}
							if (progressInfo != null)
							{
								progressInfo("Copying to atlas: '" + distinctMaterialTextures[m].ts[k].t + "'", 0.02f);
							}
							this.CopyScaledAndTiledToAtlas(distinctMaterialTextures[m].ts[k], targX, targY, num3, num4, this._fixOutOfBoundsUVs, this._maxTilingBakeSize, array2, num, isNormalMap, progressInfo);
						}
						if (progressInfo != null)
						{
							progressInfo("Applying changes to atlas: '" + texPropertyNames[k].name + "'", 0.03f);
						}
						texture2D = new Texture2D(num, num2, TextureFormat.ARGB32, true);
						for (int n = 0; n < array2.Length; n++)
						{
							texture2D.SetPixels(0, n, num, 1, array2[n]);
						}
						texture2D.Apply();
						if (this.LOG_LEVEL >= MB2_LogLevel.debug)
						{
							UnityEngine.Debug.Log(string.Concat(new object[]
							{
								"Saving atlas ",
								texPropertyNames[k].name,
								" w=",
								texture2D.width,
								" h=",
								texture2D.height
							}));
						}
					}
					atlases[k] = texture2D;
					if (progressInfo != null)
					{
						progressInfo("Saving atlas: '" + texPropertyNames[k].name + "'", 0.04f);
					}
					if (this._saveAtlasesAsAssets && textureEditorMethods != null)
					{
						textureEditorMethods.SaveAtlasToAssetDatabase(atlases[k], texPropertyNames[k], k, resultMaterial);
					}
					else
					{
						resultMaterial.SetTexture(texPropertyNames[k].name, atlases[k]);
					}
					resultMaterial.SetTextureOffset(texPropertyNames[k].name, Vector2.zero);
					resultMaterial.SetTextureScale(texPropertyNames[k].name, Vector2.one);
					this._destroyTemporaryTextures();
				}
			}
			return array;
		}

		private Rect[] __CreateAtlasesMBTexturePackerFast(ProgressUpdateDelegate progressInfo, int numAtlases, List<MB3_TextureCombiner.MB_TexSet> distinctMaterialTextures, List<ShaderTextureProperty> texPropertyNames, bool[] allTexturesAreNullAndSameColor, Material resultMaterial, Texture2D[] atlases, MB2_EditorMethodsInterface textureEditorMethods, int _padding)
		{
			Rect[] array;
			if (distinctMaterialTextures.Count == 1 && !this._fixOutOfBoundsUVs)
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.debug)
				{
					UnityEngine.Debug.Log("Only one image per atlas. Will re-use original texture");
				}
				array = new Rect[]
				{
					new Rect(0f, 0f, 1f, 1f)
				};
				for (int i = 0; i < numAtlases; i++)
				{
					MB3_TextureCombiner.MeshBakerMaterialTexture meshBakerMaterialTexture = distinctMaterialTextures[0].ts[i];
					atlases[i] = meshBakerMaterialTexture.t;
					resultMaterial.SetTexture(texPropertyNames[i].name, atlases[i]);
					resultMaterial.SetTextureScale(texPropertyNames[i].name, meshBakerMaterialTexture.scale);
					resultMaterial.SetTextureOffset(texPropertyNames[i].name, meshBakerMaterialTexture.offset);
				}
			}
			else
			{
				List<Vector2> list = new List<Vector2>();
				for (int j = 0; j < distinctMaterialTextures.Count; j++)
				{
					list.Add(new Vector2((float)distinctMaterialTextures[j].idealWidth, (float)distinctMaterialTextures[j].idealHeight));
				}
				MB2_TexturePacker mB2_TexturePacker = new MB2_TexturePacker();
				mB2_TexturePacker.doPowerOfTwoTextures = this._meshBakerTexturePackerForcePowerOfTwo;
				int num = 1;
				int num2 = 1;
				int maxAtlasSize = this._maxAtlasSize;
				array = mB2_TexturePacker.GetRects(list, maxAtlasSize, _padding, out num, out num2);
				if (this.LOG_LEVEL >= MB2_LogLevel.debug)
				{
					UnityEngine.Debug.Log(string.Concat(new object[]
					{
						"Generated atlas will be ",
						num,
						"x",
						num2,
						" (Max atlas size for platform: ",
						maxAtlasSize,
						")"
					}));
				}
				GameObject gameObject = null;
				try
				{
					gameObject = new GameObject("MBrenderAtlasesGO");
					MB3_AtlasPackerRenderTexture mB3_AtlasPackerRenderTexture = gameObject.AddComponent<MB3_AtlasPackerRenderTexture>();
					gameObject.AddComponent<Camera>();
					for (int k = 0; k < numAtlases; k++)
					{
						Texture2D texture2D;
						if (allTexturesAreNullAndSameColor[k])
						{
							texture2D = null;
							if (this.LOG_LEVEL >= MB2_LogLevel.debug)
							{
								UnityEngine.Debug.Log("Not creating atlas for " + texPropertyNames[k].name + " because textures are null and default value parameters are the same.");
							}
						}
						else
						{
							GC.Collect();
							if (progressInfo != null)
							{
								progressInfo("Creating Atlas '" + texPropertyNames[k].name + "'", 0.01f);
							}
							if (this.LOG_LEVEL >= MB2_LogLevel.debug)
							{
								UnityEngine.Debug.Log(string.Concat(new object[]
								{
									"About to render ",
									texPropertyNames[k].name,
									" isNormal=",
									texPropertyNames[k].isNormalMap
								}));
							}
							mB3_AtlasPackerRenderTexture.LOG_LEVEL = this.LOG_LEVEL;
							mB3_AtlasPackerRenderTexture.width = num;
							mB3_AtlasPackerRenderTexture.height = num2;
							mB3_AtlasPackerRenderTexture.padding = _padding;
							mB3_AtlasPackerRenderTexture.rects = array;
							mB3_AtlasPackerRenderTexture.textureSets = distinctMaterialTextures;
							mB3_AtlasPackerRenderTexture.indexOfTexSetToRender = k;
							mB3_AtlasPackerRenderTexture.isNormalMap = texPropertyNames[k].isNormalMap;
							mB3_AtlasPackerRenderTexture.fixOutOfBoundsUVs = this._fixOutOfBoundsUVs;
							texture2D = mB3_AtlasPackerRenderTexture.OnRenderAtlas(this);
							if (this.LOG_LEVEL >= MB2_LogLevel.debug)
							{
								UnityEngine.Debug.Log(string.Concat(new object[]
								{
									"Saving atlas ",
									texPropertyNames[k].name,
									" w=",
									texture2D.width,
									" h=",
									texture2D.height,
									" id=",
									texture2D.GetInstanceID()
								}));
							}
						}
						atlases[k] = texture2D;
						if (progressInfo != null)
						{
							progressInfo("Saving atlas: '" + texPropertyNames[k].name + "'", 0.04f);
						}
						if (this._saveAtlasesAsAssets && textureEditorMethods != null)
						{
							textureEditorMethods.SaveAtlasToAssetDatabase(atlases[k], texPropertyNames[k], k, resultMaterial);
						}
						else
						{
							resultMaterial.SetTexture(texPropertyNames[k].name, atlases[k]);
						}
						resultMaterial.SetTextureOffset(texPropertyNames[k].name, Vector2.zero);
						resultMaterial.SetTextureScale(texPropertyNames[k].name, Vector2.one);
						this._destroyTemporaryTextures();
					}
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
				finally
				{
					if (gameObject != null)
					{
						MB_Utility.Destroy(gameObject);
					}
				}
			}
			return array;
		}

		private Rect[] __CreateAtlasesUnityTexturePacker(ProgressUpdateDelegate progressInfo, int numAtlases, List<MB3_TextureCombiner.MB_TexSet> distinctMaterialTextures, List<ShaderTextureProperty> texPropertyNames, bool[] allTexturesAreNullAndSameColor, Material resultMaterial, Texture2D[] atlases, MB2_EditorMethodsInterface textureEditorMethods, int _padding)
		{
			Rect[] array;
			if (distinctMaterialTextures.Count == 1 && !this._fixOutOfBoundsUVs)
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.debug)
				{
					UnityEngine.Debug.Log("Only one image per atlas. Will re-use original texture");
				}
				array = new Rect[]
				{
					new Rect(0f, 0f, 1f, 1f)
				};
				for (int i = 0; i < numAtlases; i++)
				{
					MB3_TextureCombiner.MeshBakerMaterialTexture meshBakerMaterialTexture = distinctMaterialTextures[0].ts[i];
					atlases[i] = meshBakerMaterialTexture.t;
					resultMaterial.SetTexture(texPropertyNames[i].name, atlases[i]);
					resultMaterial.SetTextureScale(texPropertyNames[i].name, meshBakerMaterialTexture.scale);
					resultMaterial.SetTextureOffset(texPropertyNames[i].name, meshBakerMaterialTexture.offset);
				}
			}
			else
			{
				long num = 0L;
				int w = 1;
				int h = 1;
				array = null;
				for (int j = 0; j < numAtlases; j++)
				{
					Texture2D texture2D;
					if (allTexturesAreNullAndSameColor[j])
					{
						texture2D = null;
					}
					else
					{
						if (this.LOG_LEVEL >= MB2_LogLevel.debug)
						{
							UnityEngine.Debug.LogWarning(string.Concat(new object[]
							{
								"Beginning loop ",
								j,
								" num temporary textures ",
								this._temporaryTextures.Count
							}));
						}
						for (int k = 0; k < distinctMaterialTextures.Count; k++)
						{
							MB3_TextureCombiner.MB_TexSet mB_TexSet = distinctMaterialTextures[k];
							int idealWidth = mB_TexSet.idealWidth;
							int idealHeight = mB_TexSet.idealHeight;
							Texture2D texture2D2 = mB_TexSet.ts[j].t;
							if (texture2D2 == null)
							{
								texture2D2 = (mB_TexSet.ts[j].t = this._createTemporaryTexture(idealWidth, idealHeight, TextureFormat.ARGB32, true));
							}
							if (progressInfo != null)
							{
								progressInfo("Adjusting for scale and offset " + texture2D2, 0.01f);
							}
							if (textureEditorMethods != null)
							{
								textureEditorMethods.SetReadWriteFlag(texture2D2, true, true);
							}
							texture2D2 = this.GetAdjustedForScaleAndOffset2(mB_TexSet.ts[j]);
							if (texture2D2.width != idealWidth || texture2D2.height != idealHeight)
							{
								if (progressInfo != null)
								{
									progressInfo("Resizing texture '" + texture2D2 + "'", 0.01f);
								}
								if (this.LOG_LEVEL >= MB2_LogLevel.debug)
								{
									UnityEngine.Debug.LogWarning(string.Concat(new object[]
									{
										"Copying and resizing texture ",
										texPropertyNames[j].name,
										" from ",
										texture2D2.width,
										"x",
										texture2D2.height,
										" to ",
										idealWidth,
										"x",
										idealHeight
									}));
								}
								if (textureEditorMethods != null)
								{
									textureEditorMethods.SetReadWriteFlag(texture2D2, true, true);
								}
								texture2D2 = this._resizeTexture(texture2D2, idealWidth, idealHeight);
							}
							mB_TexSet.ts[j].t = texture2D2;
						}
						Texture2D[] array2 = new Texture2D[distinctMaterialTextures.Count];
						for (int l = 0; l < distinctMaterialTextures.Count; l++)
						{
							Texture2D t = distinctMaterialTextures[l].ts[j].t;
							num += (long)(t.width * t.height);
							array2[l] = t;
						}
						if (textureEditorMethods != null)
						{
							textureEditorMethods.CheckBuildSettings(num);
						}
						if (Math.Sqrt((double)num) > 3500.0 && this.LOG_LEVEL >= MB2_LogLevel.warn)
						{
							UnityEngine.Debug.LogWarning("The maximum possible atlas size is 4096. Textures may be shrunk");
						}
						texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, true);
						if (progressInfo != null)
						{
							progressInfo("Packing texture atlas " + texPropertyNames[j].name, 0.25f);
						}
						if (j == 0)
						{
							if (progressInfo != null)
							{
								progressInfo("Estimated min size of atlases: " + Math.Sqrt((double)num).ToString("F0"), 0.1f);
							}
							if (this.LOG_LEVEL >= MB2_LogLevel.info)
							{
								UnityEngine.Debug.Log("Estimated atlas minimum size:" + Math.Sqrt((double)num).ToString("F0"));
							}
							this._addWatermark(array2);
							if (distinctMaterialTextures.Count == 1 && !this._fixOutOfBoundsUVs)
							{
								array = new Rect[]
								{
									new Rect(0f, 0f, 1f, 1f)
								};
								texture2D = this._copyTexturesIntoAtlas(array2, _padding, array, array2[0].width, array2[0].height);
							}
							else
							{
								int maximumAtlasSize = 4096;
								array = texture2D.PackTextures(array2, _padding, maximumAtlasSize, false);
							}
							if (this.LOG_LEVEL >= MB2_LogLevel.info)
							{
								UnityEngine.Debug.Log(string.Concat(new object[]
								{
									"After pack textures atlas size ",
									texture2D.width,
									" ",
									texture2D.height
								}));
							}
							w = texture2D.width;
							h = texture2D.height;
							texture2D.Apply();
						}
						else
						{
							if (progressInfo != null)
							{
								progressInfo("Copying Textures Into: " + texPropertyNames[j].name, 0.1f);
							}
							texture2D = this._copyTexturesIntoAtlas(array2, _padding, array, w, h);
						}
					}
					atlases[j] = texture2D;
					if (this._saveAtlasesAsAssets && textureEditorMethods != null)
					{
						textureEditorMethods.SaveAtlasToAssetDatabase(atlases[j], texPropertyNames[j], j, resultMaterial);
					}
					resultMaterial.SetTextureOffset(texPropertyNames[j].name, Vector2.zero);
					resultMaterial.SetTextureScale(texPropertyNames[j].name, Vector2.one);
					this._destroyTemporaryTextures();
					GC.Collect();
				}
			}
			return array;
		}

		private void _addWatermark(Texture2D[] texToPack)
		{
		}

		private Texture2D _addWatermark(Texture2D texToPack)
		{
			return texToPack;
		}

		private Texture2D _copyTexturesIntoAtlas(Texture2D[] texToPack, int padding, Rect[] rs, int w, int h)
		{
			Texture2D texture2D = new Texture2D(w, h, TextureFormat.ARGB32, true);
			MB_Utility.setSolidColor(texture2D, Color.clear);
			for (int i = 0; i < rs.Length; i++)
			{
				Rect rect = rs[i];
				Texture2D texture2D2 = texToPack[i];
				int x = Mathf.RoundToInt(rect.x * (float)w);
				int y = Mathf.RoundToInt(rect.y * (float)h);
				int num = Mathf.RoundToInt(rect.width * (float)w);
				int num2 = Mathf.RoundToInt(rect.height * (float)h);
				if (texture2D2.width != num && texture2D2.height != num2)
				{
					texture2D2 = MB_Utility.resampleTexture(texture2D2, num, num2);
					this._temporaryTextures.Add(texture2D2);
				}
				texture2D.SetPixels(x, y, num, num2, texture2D2.GetPixels());
			}
			texture2D.Apply();
			return texture2D;
		}

		private bool IsPowerOfTwo(int x)
		{
			return (x & x - 1) == 0;
		}

		private Vector2 GetAdjustedForScaleAndOffset2Dimensions(MB3_TextureCombiner.MeshBakerMaterialTexture source)
		{
			if (source.offset.x == 0f && source.offset.y == 0f && source.scale.x == 1f && source.scale.y == 1f)
			{
				if (!this._fixOutOfBoundsUVs)
				{
					return new Vector2((float)source.t.width, (float)source.t.height);
				}
				if (source.obUVoffset.x == 0f && source.obUVoffset.y == 0f && source.obUVscale.x == 1f && source.obUVscale.y == 1f)
				{
					return new Vector2((float)source.t.width, (float)source.t.height);
				}
			}
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				UnityEngine.Debug.LogWarning(string.Concat(new object[]
				{
					"GetAdjustedForScaleAndOffset2Dimensions: ",
					source.t,
					" ",
					source.obUVoffset,
					" ",
					source.obUVscale
				}));
			}
			float num = (float)source.t.width * source.scale.x;
			float num2 = (float)source.t.height * source.scale.y;
			if (this._fixOutOfBoundsUVs)
			{
				num *= source.obUVscale.x;
				num2 *= source.obUVscale.y;
			}
			if (num > (float)this._maxTilingBakeSize)
			{
				num = (float)this._maxTilingBakeSize;
			}
			if (num2 > (float)this._maxTilingBakeSize)
			{
				num2 = (float)this._maxTilingBakeSize;
			}
			if (num < 1f)
			{
				num = 1f;
			}
			if (num2 < 1f)
			{
				num2 = 1f;
			}
			return new Vector2(num, num2);
		}

		public Texture2D GetAdjustedForScaleAndOffset2(MB3_TextureCombiner.MeshBakerMaterialTexture source)
		{
			if (source.offset.x == 0f && source.offset.y == 0f && source.scale.x == 1f && source.scale.y == 1f)
			{
				if (!this._fixOutOfBoundsUVs)
				{
					return source.t;
				}
				if (source.obUVoffset.x == 0f && source.obUVoffset.y == 0f && source.obUVscale.x == 1f && source.obUVscale.y == 1f)
				{
					return source.t;
				}
			}
			Vector2 adjustedForScaleAndOffset2Dimensions = this.GetAdjustedForScaleAndOffset2Dimensions(source);
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				UnityEngine.Debug.LogWarning(string.Concat(new object[]
				{
					"GetAdjustedForScaleAndOffset2: ",
					source.t,
					" ",
					source.obUVoffset,
					" ",
					source.obUVscale
				}));
			}
			float x = adjustedForScaleAndOffset2Dimensions.x;
			float y = adjustedForScaleAndOffset2Dimensions.y;
			float num = source.scale.x;
			float num2 = source.scale.y;
			float num3 = source.offset.x;
			float num4 = source.offset.y;
			if (this._fixOutOfBoundsUVs)
			{
				num *= source.obUVscale.x;
				num2 *= source.obUVscale.y;
				num3 += source.obUVoffset.x;
				num4 += source.obUVoffset.y;
			}
			Texture2D texture2D = this._createTemporaryTexture((int)x, (int)y, TextureFormat.ARGB32, true);
			for (int i = 0; i < texture2D.width; i++)
			{
				for (int j = 0; j < texture2D.height; j++)
				{
					float u = (float)i / x * num + num3;
					float v = (float)j / y * num2 + num4;
					texture2D.SetPixel(i, j, source.t.GetPixelBilinear(u, v));
				}
			}
			texture2D.Apply();
			return texture2D;
		}

		public void CopyScaledAndTiledToAtlas(MB3_TextureCombiner.MeshBakerMaterialTexture source, int targX, int targY, int targW, int targH, bool _fixOutOfBoundsUVs, int maxSize, Color[][] atlasPixels, int atlasWidth, bool isNormalMap, ProgressUpdateDelegate progressInfo = null)
		{
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				UnityEngine.Debug.Log(string.Concat(new object[]
				{
					"CopyScaledAndTiledToAtlas: ",
					source.t,
					" inAtlasX=",
					targX,
					" inAtlasY=",
					targY,
					" inAtlasW=",
					targW,
					" inAtlasH=",
					targH
				}));
			}
			float num = (float)targW;
			float num2 = (float)targH;
			float num3 = source.scale.x;
			float num4 = source.scale.y;
			float num5 = source.offset.x;
			float num6 = source.offset.y;
			if (_fixOutOfBoundsUVs)
			{
				num3 *= source.obUVscale.x;
				num4 *= source.obUVscale.y;
				num5 += source.obUVoffset.x;
				num6 += source.obUVoffset.y;
			}
			int num7 = (int)num;
			int num8 = (int)num2;
			Texture2D texture2D = source.t;
			if (texture2D == null)
			{
				texture2D = this._createTemporaryTexture(16, 16, TextureFormat.ARGB32, true);
				MB_Utility.setSolidColor(texture2D, source.colorIfNoTexture);
			}
			texture2D = this._addWatermark(texture2D);
			for (int i = 0; i < num7; i++)
			{
				if (progressInfo != null && num7 > 0)
				{
					progressInfo("CopyScaledAndTiledToAtlas " + ((float)i / (float)num7 * 100f).ToString("F0"), 0.2f);
				}
				for (int j = 0; j < num8; j++)
				{
					float u = (float)i / num * num3 + num5;
					float v = (float)j / num2 * num4 + num6;
					atlasPixels[targY + j][targX + i] = texture2D.GetPixelBilinear(u, v);
				}
			}
			for (int k = 0; k < num7; k++)
			{
				for (int l = 1; l <= this.atlasPadding; l++)
				{
					atlasPixels[targY - l][targX + k] = atlasPixels[targY][targX + k];
					atlasPixels[targY + num8 - 1 + l][targX + k] = atlasPixels[targY + num8 - 1][targX + k];
				}
			}
			for (int m = 0; m < num8; m++)
			{
				for (int n = 1; n <= this._atlasPadding; n++)
				{
					atlasPixels[targY + m][targX - n] = atlasPixels[targY + m][targX];
					atlasPixels[targY + m][targX + num7 + n - 1] = atlasPixels[targY + m][targX + num7 - 1];
				}
			}
			for (int num9 = 1; num9 <= this._atlasPadding; num9++)
			{
				for (int num10 = 1; num10 <= this._atlasPadding; num10++)
				{
					atlasPixels[targY - num10][targX - num9] = atlasPixels[targY][targX];
					atlasPixels[targY + num8 - 1 + num10][targX - num9] = atlasPixels[targY + num8 - 1][targX];
					atlasPixels[targY + num8 - 1 + num10][targX + num7 + num9 - 1] = atlasPixels[targY + num8 - 1][targX + num7 - 1];
					atlasPixels[targY - num10][targX + num7 + num9 - 1] = atlasPixels[targY][targX + num7 - 1];
				}
			}
		}

		public Texture2D _createTemporaryTexture(int w, int h, TextureFormat texFormat, bool mipMaps)
		{
			Texture2D texture2D = new Texture2D(w, h, texFormat, mipMaps);
			MB_Utility.setSolidColor(texture2D, Color.clear);
			this._temporaryTextures.Add(texture2D);
			return texture2D;
		}

		private Texture2D _createTextureCopy(Texture2D t)
		{
			Texture2D texture2D = MB_Utility.createTextureCopy(t);
			this._temporaryTextures.Add(texture2D);
			return texture2D;
		}

		private Texture2D _resizeTexture(Texture2D t, int w, int h)
		{
			Texture2D texture2D = MB_Utility.resampleTexture(t, w, h);
			this._temporaryTextures.Add(texture2D);
			return texture2D;
		}

		private void _destroyTemporaryTextures()
		{
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				UnityEngine.Debug.Log("Destroying " + this._temporaryTextures.Count + " temporary textures");
			}
			for (int i = 0; i < this._temporaryTextures.Count; i++)
			{
				MB_Utility.Destroy(this._temporaryTextures[i]);
			}
			this._temporaryTextures.Clear();
		}

		public void SuggestTreatment(List<GameObject> objsToMesh, Material[] resultMaterials, List<ShaderTextureProperty> _customShaderPropNames)
		{
			this._customShaderPropNames = _customShaderPropNames;
			StringBuilder stringBuilder = new StringBuilder();
			Dictionary<int, MB_Utility.MeshAnalysisResult[]> dictionary = new Dictionary<int, MB_Utility.MeshAnalysisResult[]>();
			for (int i = 0; i < objsToMesh.Count; i++)
			{
				GameObject gameObject = objsToMesh[i];
				if (!(gameObject == null))
				{
					Material[] gOMaterials = MB_Utility.GetGOMaterials(objsToMesh[i]);
					if (gOMaterials.Length > 1)
					{
						stringBuilder.AppendFormat("\nObject {0} uses {1} materials. Possible treatments:\n", objsToMesh[i].name, gOMaterials.Length);
						stringBuilder.AppendFormat("  1) Collapse the submeshes together into one submesh in the combined mesh. Each of the original submesh materials will map to a different UV rectangle in the atlas(es) used by the combined material.\n", new object[0]);
						stringBuilder.AppendFormat("  2) Use the multiple materials feature to map submeshes in the source mesh to submeshes in the combined mesh.\n", new object[0]);
					}
					Mesh mesh = MB_Utility.GetMesh(gameObject);
					MB_Utility.MeshAnalysisResult[] array;
					if (!dictionary.TryGetValue(mesh.GetInstanceID(), out array))
					{
						array = new MB_Utility.MeshAnalysisResult[mesh.subMeshCount];
						MB_Utility.doSubmeshesShareVertsOrTris(mesh, ref array[0]);
						for (int j = 0; j < mesh.subMeshCount; j++)
						{
							Rect rect = default(Rect);
							MB_Utility.hasOutOfBoundsUVs(mesh, ref rect, ref array[j], j);
							array[j].hasOverlappingSubmeshTris = array[0].hasOverlappingSubmeshTris;
							array[j].hasOverlappingSubmeshVerts = array[0].hasOverlappingSubmeshVerts;
						}
						dictionary.Add(mesh.GetInstanceID(), array);
					}
					for (int k = 0; k < gOMaterials.Length; k++)
					{
						if (array[k].hasOutOfBoundsUVs)
						{
							Rect uvRect = array[k].uvRect;
							stringBuilder.AppendFormat("\nObject {0} submesh={1} material={2} uses UVs outside the range 0,0 .. 1,1 to create tiling that tiles the box {3},{4} .. {5},{6}. This is a problem because the UVs outside the 0,0 .. 1,1 rectangle will pick up neighboring textures in the atlas. Possible Treatments:\n", new object[]
							{
								gameObject,
								k,
								gOMaterials[k],
								uvRect.x.ToString("G4"),
								uvRect.y.ToString("G4"),
								(uvRect.x + uvRect.width).ToString("G4"),
								(uvRect.y + uvRect.height).ToString("G4")
							});
							stringBuilder.AppendFormat("    1) Ignore the problem. The tiling may not affect result significantly.\n", new object[0]);
							stringBuilder.AppendFormat("    2) Use the 'fix out of bounds UVs' feature to bake the tiling and scale the UVs to fit in the 0,0 .. 1,1 rectangle.\n", new object[0]);
							stringBuilder.AppendFormat("    3) Use the Multiple Materials feature to map the material on this submesh to its own submesh in the combined mesh. No other materials should map to this submesh. This will result in only one texture in the atlas(es) and the UVs should tile correctly.\n", new object[0]);
							stringBuilder.AppendFormat("    4) Combine only meshes that use the same (or subset of) the set of materials on this mesh. The original material(s) can be applied to the result\n", new object[0]);
						}
					}
					if (array[0].hasOverlappingSubmeshVerts)
					{
						stringBuilder.AppendFormat("\nObject {0} has submeshes that share vertices. This is a problem because each vertex can have only one UV coordinate and may be required to map to different positions in the various atlases that are generated. Possible treatments:\n", objsToMesh[i]);
						stringBuilder.AppendFormat(" 1) Ignore the problem. The vertices may not affect the result.\n", new object[0]);
						stringBuilder.AppendFormat(" 2) Use the Multiple Materials feature to map the submeshs that overlap to their own submeshs in the combined mesh. No other materials should map to this submesh. This will result in only one texture in the atlas(es) and the UVs should tile correctly.\n", new object[0]);
						stringBuilder.AppendFormat(" 3) Combine only meshes that use the same (or subset of) the set of materials on this mesh. The original material(s) can be applied to the result\n", new object[0]);
					}
				}
			}
			Dictionary<Material, List<GameObject>> dictionary2 = new Dictionary<Material, List<GameObject>>();
			for (int l = 0; l < objsToMesh.Count; l++)
			{
				if (objsToMesh[l] != null)
				{
					Material[] gOMaterials2 = MB_Utility.GetGOMaterials(objsToMesh[l]);
					for (int m = 0; m < gOMaterials2.Length; m++)
					{
						if (gOMaterials2[m] != null)
						{
							List<GameObject> list;
							if (!dictionary2.TryGetValue(gOMaterials2[m], out list))
							{
								list = new List<GameObject>();
								dictionary2.Add(gOMaterials2[m], list);
							}
							if (!list.Contains(objsToMesh[l]))
							{
								list.Add(objsToMesh[l]);
							}
						}
					}
				}
			}
			List<ShaderTextureProperty> list2 = new List<ShaderTextureProperty>();
			for (int n = 0; n < resultMaterials.Length; n++)
			{
				this._CollectPropertyNames(resultMaterials[n], list2);
				foreach (Material current in dictionary2.Keys)
				{
					for (int num = 0; num < list2.Count; num++)
					{
						if (current.HasProperty(list2[num].name))
						{
							Texture texture = current.GetTexture(list2[num].name);
							if (texture != null)
							{
								Vector2 textureOffset = current.GetTextureOffset(list2[num].name);
								Vector3 vector = current.GetTextureScale(list2[num].name);
								if (textureOffset.x < 0f || textureOffset.x + vector.x > 1f || textureOffset.y < 0f || textureOffset.y + vector.y > 1f)
								{
									stringBuilder.AppendFormat("\nMaterial {0} used by objects {1} uses texture {2} that is tiled (scale={3} offset={4}). If there is more than one texture in the atlas  then Mesh Baker will bake the tiling into the atlas. If the baked tiling is large then quality can be lost. Possible treatments:\n", new object[]
									{
										current,
										this.PrintList(dictionary2[current]),
										texture,
										vector,
										textureOffset
									});
									stringBuilder.AppendFormat("  1) Use the baked tiling.\n", new object[0]);
									stringBuilder.AppendFormat("  2) Use the Multiple Materials feature to map the material on this object/submesh to its own submesh in the combined mesh. No other materials should map to this submesh. The original material can be applied to this submesh.\n", new object[0]);
									stringBuilder.AppendFormat("  3) Combine only meshes that use the same (or subset of) the set of textures on this mesh. The original material can be applied to the result.\n", new object[0]);
								}
							}
						}
					}
				}
			}
			string message = string.Empty;
			if (stringBuilder.Length == 0)
			{
				message = "====== No problems detected. These meshes should combine well ====\n  If there are problems with the combined meshes please report the problem to digitalOpus.ca so we can improve Mesh Baker.";
			}
			else
			{
				message = "====== There are possible problems with these meshes that may prevent them from combining well. TREATMENT SUGGESTIONS (copy and paste to text editor if too big) =====\n" + stringBuilder.ToString();
			}
			UnityEngine.Debug.Log(message);
		}

		private void AdjustNonTextureProperties(Material mat, List<ShaderTextureProperty> texPropertyNames, List<MB3_TextureCombiner.MB_TexSet> distinctMaterialTextures, MB2_EditorMethodsInterface editorMethods)
		{
			if (mat == null || texPropertyNames == null)
			{
				return;
			}
			for (int i = 0; i < texPropertyNames.Count; i++)
			{
				string name = texPropertyNames[i].name;
				if (name.Equals("_MainTex") && mat.HasProperty("_Color"))
				{
					try
					{
						mat.SetColor("_Color", distinctMaterialTextures[0].ts[i].tintColor);
					}
					catch (Exception)
					{
					}
				}
				if (name.Equals("_BumpMap") && mat.HasProperty("_BumpScale"))
				{
					try
					{
						mat.SetFloat("_BumpScale", 1f);
					}
					catch (Exception)
					{
					}
				}
				if (name.Equals("_ParallaxMap") && mat.HasProperty("_Parallax"))
				{
					try
					{
						mat.SetFloat("_Parallax", 0.02f);
					}
					catch (Exception)
					{
					}
				}
				if (name.Equals("_OcclusionMap") && mat.HasProperty("_OcclusionStrength"))
				{
					try
					{
						mat.SetFloat("_OcclusionStrength", 1f);
					}
					catch (Exception)
					{
					}
				}
				if (name.Equals("_EmissionMap"))
				{
					if (mat.HasProperty("_EmissionColorUI"))
					{
						try
						{
							mat.SetColor("_EmissionColorUI", new Color(1f, 1f, 1f, 1f));
						}
						catch (Exception)
						{
						}
					}
					if (mat.HasProperty("_EmissionScaleUI"))
					{
						try
						{
							mat.SetFloat("_EmissionScaleUI", 1f);
						}
						catch (Exception)
						{
						}
					}
				}
			}
			if (editorMethods != null)
			{
				editorMethods.CommitChangesToAssets();
			}
		}

		private Color GetColorIfNoTexture(Material mat, ShaderTextureProperty texProperty)
		{
			if (texProperty.isNormalMap)
			{
				return new Color(0.5f, 0.5f, 1f);
			}
			if (texProperty.name.Equals("_MainTex"))
			{
				if (mat != null && mat.HasProperty("_Color"))
				{
					try
					{
						Color result = mat.GetColor("_Color");
						return result;
					}
					catch (Exception)
					{
					}
				}
			}
			else if (texProperty.name.Equals("_SpecGlossMap"))
			{
				if (mat != null && mat.HasProperty("_SpecColor"))
				{
					try
					{
						Color color = mat.GetColor("_SpecColor");
						if (mat.HasProperty("_Glossiness"))
						{
							try
							{
								color.a = mat.GetFloat("_Glossiness");
							}
							catch (Exception)
							{
							}
						}
						UnityEngine.Debug.LogWarning(color);
						Color result = color;
						return result;
					}
					catch (Exception)
					{
					}
				}
			}
			else if (texProperty.name.Equals("_MetallicGlossMap"))
			{
				if (mat != null && mat.HasProperty("_Metallic"))
				{
					try
					{
						float @float = mat.GetFloat("_Metallic");
						Color color2 = new Color(@float, @float, @float);
						if (mat.HasProperty("_Glossiness"))
						{
							try
							{
								color2.a = mat.GetFloat("_Glossiness");
							}
							catch (Exception)
							{
							}
						}
						Color result = color2;
						return result;
					}
					catch (Exception)
					{
					}
				}
			}
			else
			{
				if (texProperty.name.Equals("_ParallaxMap"))
				{
					return new Color(0f, 0f, 0f, 0f);
				}
				if (texProperty.name.Equals("_OcclusionMap"))
				{
					return new Color(1f, 1f, 1f, 1f);
				}
				if (texProperty.name.Equals("_EmissionMap"))
				{
					if (mat != null && mat.HasProperty("_EmissionScaleUI"))
					{
						if (mat.HasProperty("_EmissionColor") && mat.HasProperty("_EmissionColorUI"))
						{
							try
							{
								Color color3 = mat.GetColor("_EmissionColor");
								Color color4 = mat.GetColor("_EmissionColorUI");
								float float2 = mat.GetFloat("_EmissionScaleUI");
								Color result;
								if (color3 == new Color(0f, 0f, 0f, 0f) && color4 == new Color(1f, 1f, 1f, 1f))
								{
									result = new Color(float2, float2, float2, float2);
									return result;
								}
								result = color4;
								return result;
							}
							catch (Exception)
							{
							}
						}
						else
						{
							try
							{
								float float3 = mat.GetFloat("_EmissionScaleUI");
								Color result = new Color(float3, float3, float3, float3);
								return result;
							}
							catch (Exception)
							{
							}
						}
					}
				}
				else if (texProperty.name.Equals("_DetailMask"))
				{
					return new Color(0f, 0f, 0f, 0f);
				}
			}
			return new Color(1f, 1f, 1f, 0f);
		}

		private Color32 ConvertNormalFormatFromUnity_ToStandard(Color32 c)
		{
			Vector3 zero = Vector3.zero;
			zero.x = (float)c.a * 2f - 1f;
			zero.y = (float)c.g * 2f - 1f;
			zero.z = Mathf.Sqrt(1f - zero.x * zero.x - zero.y * zero.y);
			return new Color32
			{
				a = 1,
				r = (byte)((zero.x + 1f) * 0.5f),
				g = (byte)((zero.y + 1f) * 0.5f),
				b = (byte)((zero.z + 1f) * 0.5f)
			};
		}

		private string PrintList(List<GameObject> gos)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < gos.Count; i++)
			{
				stringBuilder.Append(gos[i] + ",");
			}
			return stringBuilder.ToString();
		}
	}
}
