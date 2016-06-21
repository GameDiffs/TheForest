using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
	[Serializable]
	public class MB3_MeshCombinerSingle : MB3_MeshCombiner
	{
		[Serializable]
		public class MB_DynamicGameObject : IComparable<MB3_MeshCombinerSingle.MB_DynamicGameObject>
		{
			public int instanceID;

			public string name;

			public int vertIdx;

			public int numVerts;

			public int[] indexesOfBonesUsed = new int[0];

			public int lightmapIndex = -1;

			public Vector4 lightmapTilingOffset = new Vector4(1f, 1f, 0f, 0f);

			public bool show = true;

			public bool invertTriangles;

			public int[] submeshTriIdxs;

			public int[] submeshNumTris;

			public int[] targetSubmeshIdxs;

			public Rect[] uvRects;

			public Rect[] obUVRects;

			public int[][] _submeshTris;

			public bool _beingDeleted;

			public int _triangleIdxAdjustment;

			public Transform[] _tmpCachedBones;

			public Matrix4x4[] _tmpCachedBindposes;

			public int CompareTo(MB3_MeshCombinerSingle.MB_DynamicGameObject b)
			{
				return this.vertIdx - b.vertIdx;
			}
		}

		public struct BoneAndBindpose
		{
			public Transform bone;

			public Matrix4x4 bindPose;

			public BoneAndBindpose(Transform t, Matrix4x4 bp)
			{
				this.bone = t;
				this.bindPose = bp;
			}

			public override bool Equals(object obj)
			{
				return obj is MB3_MeshCombinerSingle.BoneAndBindpose && this.bone == ((MB3_MeshCombinerSingle.BoneAndBindpose)obj).bone && this.bindPose == ((MB3_MeshCombinerSingle.BoneAndBindpose)obj).bindPose;
			}

			public override int GetHashCode()
			{
				return this.bone.GetInstanceID() % 2147483647 ^ (int)this.bindPose[0, 0];
			}
		}

		[SerializeField]
		protected List<GameObject> objectsInCombinedMesh = new List<GameObject>();

		[SerializeField]
		private int lightmapIndex = -1;

		[SerializeField]
		private List<MB3_MeshCombinerSingle.MB_DynamicGameObject> mbDynamicObjectsInCombinedMesh = new List<MB3_MeshCombinerSingle.MB_DynamicGameObject>();

		private Dictionary<int, MB3_MeshCombinerSingle.MB_DynamicGameObject> _instance2combined_map = new Dictionary<int, MB3_MeshCombinerSingle.MB_DynamicGameObject>();

		[SerializeField]
		private Vector3[] verts = new Vector3[0];

		[SerializeField]
		private Vector3[] normals = new Vector3[0];

		[SerializeField]
		private Vector4[] tangents = new Vector4[0];

		[SerializeField]
		private Vector2[] uvs = new Vector2[0];

		[SerializeField]
		private Vector2[] uv2s = new Vector2[0];

		[SerializeField]
		private Vector2[] uv3s = new Vector2[0];

		[SerializeField]
		private Vector2[] uv4s = new Vector2[0];

		[SerializeField]
		private Color[] colors = new Color[0];

		[SerializeField]
		private Matrix4x4[] bindPoses = new Matrix4x4[0];

		[SerializeField]
		private Transform[] bones = new Transform[0];

		[SerializeField]
		private Mesh _mesh;

		private int[][] submeshTris = new int[0][];

		private BoneWeight[] boneWeights = new BoneWeight[0];

		private GameObject[] empty = new GameObject[0];

		private int[] emptyIDs = new int[0];

		private Vector2 _HALF_UV = new Vector2(0.5f, 0.5f);

		public override MB2_TextureBakeResults textureBakeResults
		{
			set
			{
				if (this.objectsInCombinedMesh.Count > 0 && this._textureBakeResults != value && this._textureBakeResults != null && this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning("If material bake result is changed then objects currently in combined mesh may be invalid.");
				}
				this._textureBakeResults = value;
			}
		}

		public override MB_RenderType renderType
		{
			set
			{
				if (value == MB_RenderType.skinnedMeshRenderer && this._renderType == MB_RenderType.meshRenderer && this.boneWeights.Length != this.verts.Length)
				{
					Debug.LogError("Can't set the render type to SkinnedMeshRenderer without clearing the mesh first. Try deleteing the CombinedMesh scene object.");
				}
				this._renderType = value;
			}
		}

		public override GameObject resultSceneObject
		{
			set
			{
				if (this._resultSceneObject != value)
				{
					this._targetRenderer = null;
					if (this._mesh != null && this._LOG_LEVEL >= MB2_LogLevel.warn)
					{
						Debug.LogWarning("Result Scene Object was changed when this mesh baker component had a reference to a mesh. If mesh is being used by another object make sure to reset the mesh to none before baking to avoid overwriting the other mesh.");
					}
				}
				this._resultSceneObject = value;
			}
		}

		private MB3_MeshCombinerSingle.MB_DynamicGameObject instance2Combined_MapGet(int gameObjectID)
		{
			return this._instance2combined_map[gameObjectID];
		}

		private void instance2Combined_MapAdd(int gameObjectID, MB3_MeshCombinerSingle.MB_DynamicGameObject dgo)
		{
			this._instance2combined_map.Add(gameObjectID, dgo);
		}

		private void instance2Combined_MapRemove(int gameObjectID)
		{
			this._instance2combined_map.Remove(gameObjectID);
		}

		private bool instance2Combined_MapTryGetValue(int gameObjectID, out MB3_MeshCombinerSingle.MB_DynamicGameObject dgo)
		{
			return this._instance2combined_map.TryGetValue(gameObjectID, out dgo);
		}

		private int instance2Combined_MapCount()
		{
			return this._instance2combined_map.Count;
		}

		private void instance2Combined_MapClear()
		{
			this._instance2combined_map.Clear();
		}

		private bool instance2Combined_MapContainsKey(int gameObjectID)
		{
			return this._instance2combined_map.ContainsKey(gameObjectID);
		}

		public override int GetNumObjectsInCombined()
		{
			return this.objectsInCombinedMesh.Count;
		}

		public override List<GameObject> GetObjectsInCombined()
		{
			List<GameObject> list = new List<GameObject>();
			list.AddRange(this.objectsInCombinedMesh);
			return list;
		}

		public Mesh GetMesh()
		{
			if (this._mesh == null)
			{
				this._mesh = new Mesh();
			}
			return this._mesh;
		}

		public Transform[] GetBones()
		{
			return this.bones;
		}

		public override int GetLightmapIndex()
		{
			if (this.lightmapOption == MB2_LightmapOptions.generate_new_UV2_layout || this.lightmapOption == MB2_LightmapOptions.preserve_current_lightmapping)
			{
				return this.lightmapIndex;
			}
			return -1;
		}

		public override int GetNumVerticesFor(GameObject go)
		{
			return this.GetNumVerticesFor(go.GetInstanceID());
		}

		public override int GetNumVerticesFor(int instanceID)
		{
			MB3_MeshCombinerSingle.MB_DynamicGameObject mB_DynamicGameObject;
			if (this.instance2Combined_MapTryGetValue(instanceID, out mB_DynamicGameObject))
			{
				return mB_DynamicGameObject.numVerts;
			}
			return -1;
		}

		private void _initialize()
		{
			if (this.objectsInCombinedMesh.Count == 0)
			{
				this.lightmapIndex = -1;
			}
			if (this._mesh == null)
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.debug)
				{
					MB2_Log.LogDebug("_initialize Creating new Mesh", new object[0]);
				}
				this._mesh = this.GetMesh();
			}
			if (this.instance2Combined_MapCount() != this.objectsInCombinedMesh.Count)
			{
				this.instance2Combined_MapClear();
				for (int i = 0; i < this.objectsInCombinedMesh.Count; i++)
				{
					this.instance2Combined_MapAdd(this.objectsInCombinedMesh[i].GetInstanceID(), this.mbDynamicObjectsInCombinedMesh[i]);
				}
				this.boneWeights = this._mesh.boneWeights;
				this.submeshTris = new int[this._mesh.subMeshCount][];
				for (int j = 0; j < this.submeshTris.Length; j++)
				{
					this.submeshTris[j] = this._mesh.GetTriangles(j);
				}
			}
			if (this.mbDynamicObjectsInCombinedMesh.Count > 0 && this.mbDynamicObjectsInCombinedMesh[0].indexesOfBonesUsed.Length == 0 && this.renderType == MB_RenderType.skinnedMeshRenderer && this.boneWeights.Length > 0)
			{
				for (int k = 0; k < this.mbDynamicObjectsInCombinedMesh.Count; k++)
				{
					MB3_MeshCombinerSingle.MB_DynamicGameObject mB_DynamicGameObject = this.mbDynamicObjectsInCombinedMesh[k];
					HashSet<int> hashSet = new HashSet<int>();
					for (int l = mB_DynamicGameObject.vertIdx; l < mB_DynamicGameObject.vertIdx + mB_DynamicGameObject.numVerts; l++)
					{
						if (this.boneWeights[l].weight0 > 0f)
						{
							hashSet.Add(this.boneWeights[l].boneIndex0);
						}
						if (this.boneWeights[l].weight1 > 0f)
						{
							hashSet.Add(this.boneWeights[l].boneIndex1);
						}
						if (this.boneWeights[l].weight2 > 0f)
						{
							hashSet.Add(this.boneWeights[l].boneIndex2);
						}
						if (this.boneWeights[l].weight3 > 0f)
						{
							hashSet.Add(this.boneWeights[l].boneIndex3);
						}
					}
					mB_DynamicGameObject.indexesOfBonesUsed = new int[hashSet.Count];
					hashSet.CopyTo(mB_DynamicGameObject.indexesOfBonesUsed);
				}
				if (this.LOG_LEVEL >= MB2_LogLevel.debug)
				{
					Debug.Log("Baker used old systems that duplicated bones. Upgrading to new system by building indexesOfBonesUsed");
				}
			}
		}

		private bool _collectMaterialTriangles(Mesh m, MB3_MeshCombinerSingle.MB_DynamicGameObject dgo, Material[] sharedMaterials, OrderedDictionary sourceMats2submeshIdx_map)
		{
			int num = m.subMeshCount;
			if (sharedMaterials.Length < num)
			{
				num = sharedMaterials.Length;
			}
			dgo._submeshTris = new int[num][];
			dgo.targetSubmeshIdxs = new int[num];
			for (int i = 0; i < num; i++)
			{
				if (this.textureBakeResults.doMultiMaterial)
				{
					if (!sourceMats2submeshIdx_map.Contains(sharedMaterials[i]))
					{
						Debug.LogError(string.Concat(new object[]
						{
							"Object ",
							dgo.name,
							" has a material that was not found in the result materials maping. ",
							sharedMaterials[i]
						}));
						return false;
					}
					dgo.targetSubmeshIdxs[i] = (int)sourceMats2submeshIdx_map[sharedMaterials[i]];
				}
				else
				{
					dgo.targetSubmeshIdxs[i] = 0;
				}
				dgo._submeshTris[i] = m.GetTriangles(i);
				if (this.LOG_LEVEL >= MB2_LogLevel.debug)
				{
					MB2_Log.LogDebug(string.Concat(new object[]
					{
						"Collecting triangles for: ",
						dgo.name,
						" submesh:",
						i,
						" maps to submesh:",
						dgo.targetSubmeshIdxs[i],
						" added:",
						dgo._submeshTris[i].Length
					}), new object[]
					{
						this.LOG_LEVEL
					});
				}
			}
			return true;
		}

		private bool _collectOutOfBoundsUVRects2(Mesh m, MB3_MeshCombinerSingle.MB_DynamicGameObject dgo, Material[] sharedMaterials, OrderedDictionary sourceMats2submeshIdx_map, Dictionary<int, MB_Utility.MeshAnalysisResult[]> meshAnalysisResults)
		{
			if (this.textureBakeResults == null)
			{
				Debug.LogError("Need to bake textures into combined material");
				return false;
			}
			MB_Utility.MeshAnalysisResult[] array;
			if (meshAnalysisResults.TryGetValue(m.GetInstanceID(), out array))
			{
				dgo.obUVRects = new Rect[sharedMaterials.Length];
				for (int i = 0; i < dgo.obUVRects.Length; i++)
				{
					dgo.obUVRects[i] = array[i].uvRect;
				}
			}
			else
			{
				int subMeshCount = m.subMeshCount;
				int num = subMeshCount;
				if (sharedMaterials.Length < subMeshCount)
				{
					num = sharedMaterials.Length;
				}
				dgo.obUVRects = new Rect[num];
				array = new MB_Utility.MeshAnalysisResult[subMeshCount];
				for (int j = 0; j < subMeshCount; j++)
				{
					Rect rect = default(Rect);
					MB_Utility.hasOutOfBoundsUVs(m, ref rect, ref array[j], j);
					if (j < num)
					{
						dgo.obUVRects[j] = rect;
					}
					array[j].uvRect = rect;
				}
				meshAnalysisResults.Add(m.GetInstanceID(), array);
			}
			return true;
		}

		private bool _validateTextureBakeResults()
		{
			if (this.textureBakeResults == null)
			{
				Debug.LogError("Material Bake Results is null. Can't combine meshes.");
				return false;
			}
			if (this.textureBakeResults.materials == null || this.textureBakeResults.materials.Length == 0)
			{
				Debug.LogError("Material Bake Results has no materials in material to uvRect map. Try baking materials. Can't combine meshes.");
				return false;
			}
			if (this.textureBakeResults.doMultiMaterial)
			{
				if (this.textureBakeResults.resultMaterials == null || this.textureBakeResults.resultMaterials.Length == 0)
				{
					Debug.LogError("Material Bake Results has no result materials. Try baking materials. Can't combine meshes.");
					return false;
				}
			}
			else if (this.textureBakeResults.resultMaterial == null)
			{
				Debug.LogError("Material Bake Results has no result material. Try baking materials. Can't combine meshes.");
				return false;
			}
			return true;
		}

		private bool _validateMeshFlags()
		{
			if (this.objectsInCombinedMesh.Count > 0 && ((!this._doNorm && this.doNorm) || (!this._doTan && this.doTan) || (!this._doCol && this.doCol) || (!this._doUV && this.doUV) || (!this._doUV3 && this.doUV3) || (!this._doUV4 && this.doUV4)))
			{
				Debug.LogError("The channels have changed. There are already objects in the combined mesh that were added with a different set of channels.");
				return false;
			}
			this._doNorm = this.doNorm;
			this._doTan = this.doTan;
			this._doCol = this.doCol;
			this._doUV = this.doUV;
			this._doUV3 = this.doUV3;
			this._doUV4 = this.doUV4;
			return true;
		}

		private bool _showHide(GameObject[] goToShow, GameObject[] goToHide)
		{
			if (goToShow == null)
			{
				goToShow = this.empty;
			}
			if (goToHide == null)
			{
				goToHide = this.empty;
			}
			this._initialize();
			for (int i = 0; i < goToHide.Length; i++)
			{
				if (!this.instance2Combined_MapContainsKey(goToHide[i].GetInstanceID()))
				{
					if (this.LOG_LEVEL >= MB2_LogLevel.warn)
					{
						Debug.LogWarning("Trying to hide an object " + goToHide[i] + " that is not in combined mesh");
					}
					return false;
				}
			}
			for (int j = 0; j < goToShow.Length; j++)
			{
				if (!this.instance2Combined_MapContainsKey(goToShow[j].GetInstanceID()))
				{
					if (this.LOG_LEVEL >= MB2_LogLevel.warn)
					{
						Debug.LogWarning("Trying to show an object " + goToShow[j] + " that is not in combined mesh");
					}
					return false;
				}
			}
			for (int k = 0; k < goToHide.Length; k++)
			{
				this._instance2combined_map[goToHide[k].GetInstanceID()].show = false;
			}
			for (int l = 0; l < goToShow.Length; l++)
			{
				this._instance2combined_map[goToShow[l].GetInstanceID()].show = true;
			}
			return true;
		}

		private bool _addToCombined(GameObject[] goToAdd, int[] goToDelete, bool disableRendererInSource)
		{
			MB3_MeshCombinerSingle.<_addToCombined>c__AnonStorey25F <_addToCombined>c__AnonStorey25F = new MB3_MeshCombinerSingle.<_addToCombined>c__AnonStorey25F();
			if (!this._validateTextureBakeResults())
			{
				return false;
			}
			if (!this._validateMeshFlags())
			{
				return false;
			}
			if (!this.ValidateTargRendererAndMeshAndResultSceneObj())
			{
				return false;
			}
			if (this.outputOption != MB2_OutputOptions.bakeMeshAssetsInPlace && this.renderType == MB_RenderType.skinnedMeshRenderer)
			{
				if (this._targetRenderer == null || !(this._targetRenderer is SkinnedMeshRenderer))
				{
					Debug.LogError("Target renderer must be set and must be a SkinnedMeshRenderer");
					return false;
				}
				SkinnedMeshRenderer skinnedMeshRenderer = (SkinnedMeshRenderer)this.targetRenderer;
				if (skinnedMeshRenderer.sharedMesh != this._mesh)
				{
					Debug.LogError("The combined mesh was not assigned to the targetRenderer. Try using buildSceneMeshObject to set up the combined mesh correctly");
				}
			}
			if (goToAdd == null)
			{
				<_addToCombined>c__AnonStorey25F._goToAdd = this.empty;
			}
			else
			{
				<_addToCombined>c__AnonStorey25F._goToAdd = (GameObject[])goToAdd.Clone();
			}
			int[] array;
			if (goToDelete == null)
			{
				array = this.emptyIDs;
			}
			else
			{
				array = (int[])goToDelete.Clone();
			}
			if (this._mesh == null)
			{
				this.DestroyMesh();
			}
			Dictionary<Material, Rect> mat2RectMap = this.textureBakeResults.GetMat2RectMap();
			this._initialize();
			if (this._mesh.vertexCount > 0 && this._instance2combined_map.Count == 0)
			{
				Debug.LogWarning("There were vertices in the combined mesh but nothing in the MeshBaker buffers. If you are trying to bake in the editor and modify at runtime, make sure 'Clear Buffers After Bake' is unchecked.");
			}
			int num = 1;
			if (this.textureBakeResults.doMultiMaterial)
			{
				num = this.textureBakeResults.resultMaterials.Length;
			}
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				MB2_Log.LogDebug(string.Concat(new object[]
				{
					"==== Calling _addToCombined objs adding:",
					<_addToCombined>c__AnonStorey25F._goToAdd.Length,
					" objs deleting:",
					array.Length,
					" fixOutOfBounds:",
					this.textureBakeResults.fixOutOfBoundsUVs,
					" doMultiMaterial:",
					this.textureBakeResults.doMultiMaterial,
					" disableRenderersInSource:",
					disableRendererInSource
				}), new object[]
				{
					this.LOG_LEVEL
				});
			}
			OrderedDictionary orderedDictionary = null;
			if (this.textureBakeResults.doMultiMaterial)
			{
				orderedDictionary = new OrderedDictionary();
				for (int i2 = 0; i2 < num; i2++)
				{
					MB_MultiMaterial mB_MultiMaterial = this.textureBakeResults.resultMaterials[i2];
					for (int j = 0; j < mB_MultiMaterial.sourceMaterials.Count; j++)
					{
						if (mB_MultiMaterial.sourceMaterials[j] == null)
						{
							Debug.LogError("Found null material in source materials for combined mesh materials " + i2);
							return false;
						}
						if (!orderedDictionary.Contains(mB_MultiMaterial.sourceMaterials[j]))
						{
							orderedDictionary.Add(mB_MultiMaterial.sourceMaterials[j], i2);
						}
					}
				}
			}
			if (this.submeshTris.Length != num)
			{
				this.submeshTris = new int[num][];
				for (int k = 0; k < this.submeshTris.Length; k++)
				{
					this.submeshTris[k] = new int[0];
				}
			}
			int num2 = 0;
			int[] array2 = new int[num];
			List<MB3_MeshCombinerSingle.MB_DynamicGameObject>[] array3 = null;
			List<int> list = new List<int>();
			HashSet<MB3_MeshCombinerSingle.BoneAndBindpose> hashSet = new HashSet<MB3_MeshCombinerSingle.BoneAndBindpose>();
			if (this.renderType == MB_RenderType.skinnedMeshRenderer && array.Length > 0)
			{
				array3 = this._buildBoneIdx2dgoMap();
			}
			for (int l = 0; l < array.Length; l++)
			{
				MB3_MeshCombinerSingle.MB_DynamicGameObject mB_DynamicGameObject;
				if (this.instance2Combined_MapTryGetValue(array[l], out mB_DynamicGameObject))
				{
					num2 += mB_DynamicGameObject.numVerts;
					if (this.renderType == MB_RenderType.skinnedMeshRenderer)
					{
						for (int m = 0; m < mB_DynamicGameObject.indexesOfBonesUsed.Length; m++)
						{
							if (array3[mB_DynamicGameObject.indexesOfBonesUsed[m]].Contains(mB_DynamicGameObject))
							{
								array3[mB_DynamicGameObject.indexesOfBonesUsed[m]].Remove(mB_DynamicGameObject);
								if (array3[mB_DynamicGameObject.indexesOfBonesUsed[m]].Count == 0)
								{
									list.Add(mB_DynamicGameObject.indexesOfBonesUsed[m]);
								}
							}
						}
					}
					for (int n = 0; n < mB_DynamicGameObject.submeshNumTris.Length; n++)
					{
						array2[n] += mB_DynamicGameObject.submeshNumTris[n];
					}
				}
				else if (this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning("Trying to delete an object that is not in combined mesh");
				}
			}
			List<MB3_MeshCombinerSingle.MB_DynamicGameObject> list2 = new List<MB3_MeshCombinerSingle.MB_DynamicGameObject>();
			Dictionary<int, MB_Utility.MeshAnalysisResult[]> meshAnalysisResults = new Dictionary<int, MB_Utility.MeshAnalysisResult[]>();
			int num3 = 0;
			int[] array4 = new int[num];
			int i;
			for (i = 0; i < <_addToCombined>c__AnonStorey25F._goToAdd.Length; i++)
			{
				if (!this.instance2Combined_MapContainsKey(<_addToCombined>c__AnonStorey25F._goToAdd[i].GetInstanceID()) || Array.FindIndex<int>(array, (int o) => o == <_addToCombined>c__AnonStorey25F._goToAdd[i].GetInstanceID()) != -1)
				{
					MB3_MeshCombinerSingle.MB_DynamicGameObject mB_DynamicGameObject2 = new MB3_MeshCombinerSingle.MB_DynamicGameObject();
					GameObject gameObject = <_addToCombined>c__AnonStorey25F._goToAdd[i];
					Material[] gOMaterials = MB_Utility.GetGOMaterials(gameObject);
					if (gOMaterials == null)
					{
						Debug.LogError("Object " + gameObject.name + " does not have a Renderer");
						<_addToCombined>c__AnonStorey25F._goToAdd[i] = null;
						return false;
					}
					Mesh mesh = MB_Utility.GetMesh(gameObject);
					if (mesh == null)
					{
						Debug.LogError("Object " + gameObject.name + " MeshFilter or SkinedMeshRenderer had no mesh");
						<_addToCombined>c__AnonStorey25F._goToAdd[i] = null;
						return false;
					}
					if (MBVersion.IsRunningAndMeshNotReadWriteable(mesh))
					{
						Debug.LogError("Object " + gameObject.name + " Mesh Importer has read/write flag set to 'false'. This needs to be set to 'true' in order to read data from this mesh.");
						<_addToCombined>c__AnonStorey25F._goToAdd[i] = null;
						return false;
					}
					Rect[] array5 = new Rect[gOMaterials.Length];
					for (int num4 = 0; num4 < gOMaterials.Length; num4++)
					{
						if (!mat2RectMap.TryGetValue(gOMaterials[num4], out array5[num4]))
						{
							Debug.LogError(string.Concat(new object[]
							{
								"Object ",
								gameObject.name,
								" has an unknown material ",
								gOMaterials[num4],
								". Try baking textures"
							}));
							<_addToCombined>c__AnonStorey25F._goToAdd[i] = null;
						}
					}
					if (<_addToCombined>c__AnonStorey25F._goToAdd[i] != null)
					{
						list2.Add(mB_DynamicGameObject2);
						mB_DynamicGameObject2.name = string.Format("{0} {1}", <_addToCombined>c__AnonStorey25F._goToAdd[i].ToString(), <_addToCombined>c__AnonStorey25F._goToAdd[i].GetInstanceID());
						mB_DynamicGameObject2.instanceID = <_addToCombined>c__AnonStorey25F._goToAdd[i].GetInstanceID();
						mB_DynamicGameObject2.uvRects = array5;
						mB_DynamicGameObject2.numVerts = mesh.vertexCount;
						Renderer renderer = MB_Utility.GetRenderer(gameObject);
						if (this.renderType == MB_RenderType.skinnedMeshRenderer)
						{
							this._CollectBonesToAddForDGO(mB_DynamicGameObject2, list, hashSet, renderer);
						}
						if (this.lightmapIndex == -1)
						{
							this.lightmapIndex = renderer.lightmapIndex;
						}
						if (this.lightmapOption == MB2_LightmapOptions.preserve_current_lightmapping)
						{
							if (this.lightmapIndex != renderer.lightmapIndex && this.LOG_LEVEL >= MB2_LogLevel.warn)
							{
								Debug.LogWarning("Object " + gameObject.name + " has a different lightmap index. Lightmapping will not work.");
							}
							if (!MBVersion.GetActive(gameObject) && this.LOG_LEVEL >= MB2_LogLevel.warn)
							{
								Debug.LogWarning("Object " + gameObject.name + " is inactive. Can only get lightmap index of active objects.");
							}
							if (renderer.lightmapIndex == -1 && this.LOG_LEVEL >= MB2_LogLevel.warn)
							{
								Debug.LogWarning("Object " + gameObject.name + " does not have an index to a lightmap.");
							}
						}
						mB_DynamicGameObject2.lightmapIndex = renderer.lightmapIndex;
						mB_DynamicGameObject2.lightmapTilingOffset = MBVersion.GetLightmapTilingOffset(renderer);
						if (!this._collectMaterialTriangles(mesh, mB_DynamicGameObject2, gOMaterials, orderedDictionary))
						{
							return false;
						}
						mB_DynamicGameObject2.submeshNumTris = new int[num];
						mB_DynamicGameObject2.submeshTriIdxs = new int[num];
						if (this.textureBakeResults.fixOutOfBoundsUVs && !this._collectOutOfBoundsUVRects2(mesh, mB_DynamicGameObject2, gOMaterials, orderedDictionary, meshAnalysisResults))
						{
							return false;
						}
						num3 += mB_DynamicGameObject2.numVerts;
						for (int num5 = 0; num5 < mB_DynamicGameObject2._submeshTris.Length; num5++)
						{
							array4[mB_DynamicGameObject2.targetSubmeshIdxs[num5]] += mB_DynamicGameObject2._submeshTris[num5].Length;
						}
						mB_DynamicGameObject2.invertTriangles = this.IsMirrored(gameObject.transform.localToWorldMatrix);
					}
				}
				else
				{
					if (this.LOG_LEVEL >= MB2_LogLevel.warn)
					{
						Debug.LogWarning("Object " + <_addToCombined>c__AnonStorey25F._goToAdd[i].name + " has already been added");
					}
					<_addToCombined>c__AnonStorey25F._goToAdd[i] = null;
				}
			}
			for (int num6 = 0; num6 < <_addToCombined>c__AnonStorey25F._goToAdd.Length; num6++)
			{
				if (<_addToCombined>c__AnonStorey25F._goToAdd[num6] != null && disableRendererInSource)
				{
					MB_Utility.DisableRendererInSource(<_addToCombined>c__AnonStorey25F._goToAdd[num6]);
					if (this.LOG_LEVEL == MB2_LogLevel.trace)
					{
						Debug.Log(string.Concat(new object[]
						{
							"Disabling renderer on ",
							<_addToCombined>c__AnonStorey25F._goToAdd[num6].name,
							" id=",
							<_addToCombined>c__AnonStorey25F._goToAdd[num6].GetInstanceID()
						}));
					}
				}
			}
			int num7 = this.verts.Length + num3 - num2;
			int num8 = this.bindPoses.Length + hashSet.Count - list.Count;
			int[] array6 = new int[num];
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				MB2_Log.LogDebug(string.Concat(new object[]
				{
					"Verts adding:",
					num3,
					" deleting:",
					num2,
					" submeshes:",
					array6.Length,
					" bones:",
					num8
				}), new object[]
				{
					this.LOG_LEVEL
				});
			}
			for (int num9 = 0; num9 < array6.Length; num9++)
			{
				array6[num9] = this.submeshTris[num9].Length + array4[num9] - array2[num9];
				if (this.LOG_LEVEL >= MB2_LogLevel.debug)
				{
					MB2_Log.LogDebug(string.Concat(new object[]
					{
						"    submesh :",
						num9,
						" already contains:",
						this.submeshTris[num9].Length,
						" tris to be Added:",
						array4[num9],
						" tris to be Deleted:",
						array2[num9]
					}), new object[0]);
				}
			}
			if (num7 > 65534)
			{
				Debug.LogError("Cannot add objects. Resulting mesh will have more than 64k vertices. Try using a Multi-MeshBaker component. This will split the combined mesh into several meshes. You don't have to re-configure the MB2_TextureBaker. Just remove the MB2_MeshBaker component and add a MB2_MultiMeshBaker component.");
				return false;
			}
			Vector3[] destinationArray = null;
			Vector4[] destinationArray2 = null;
			Vector2[] destinationArray3 = null;
			Vector2[] destinationArray4 = null;
			Vector2[] array7 = null;
			Vector2[] array8 = null;
			Color[] array9 = null;
			Vector3[] array10 = new Vector3[num7];
			if (this._doNorm)
			{
				destinationArray = new Vector3[num7];
			}
			if (this._doTan)
			{
				destinationArray2 = new Vector4[num7];
			}
			if (this._doUV)
			{
				destinationArray3 = new Vector2[num7];
			}
			if (this._doUV3)
			{
				array7 = new Vector2[num7];
			}
			if (this._doUV4)
			{
				array8 = new Vector2[num7];
			}
			if (this.lightmapOption == MB2_LightmapOptions.copy_UV2_unchanged || this.lightmapOption == MB2_LightmapOptions.preserve_current_lightmapping)
			{
				destinationArray4 = new Vector2[num7];
			}
			if (this._doCol)
			{
				array9 = new Color[num7];
			}
			BoneWeight[] array11 = new BoneWeight[num7];
			Matrix4x4[] array12 = new Matrix4x4[num8];
			Transform[] array13 = new Transform[num8];
			int[][] array14 = new int[num][];
			for (int num10 = 0; num10 < array14.Length; num10++)
			{
				array14[num10] = new int[array6[num10]];
			}
			for (int num11 = 0; num11 < array.Length; num11++)
			{
				MB3_MeshCombinerSingle.MB_DynamicGameObject mB_DynamicGameObject3 = null;
				if (this.instance2Combined_MapTryGetValue(array[num11], out mB_DynamicGameObject3))
				{
					mB_DynamicGameObject3._beingDeleted = true;
				}
			}
			this.mbDynamicObjectsInCombinedMesh.Sort();
			int num12 = 0;
			int[] array15 = new int[num];
			int num13 = 0;
			for (int num14 = 0; num14 < this.mbDynamicObjectsInCombinedMesh.Count; num14++)
			{
				MB3_MeshCombinerSingle.MB_DynamicGameObject mB_DynamicGameObject4 = this.mbDynamicObjectsInCombinedMesh[num14];
				if (!mB_DynamicGameObject4._beingDeleted)
				{
					if (this.LOG_LEVEL >= MB2_LogLevel.debug)
					{
						MB2_Log.LogDebug("Copying obj in combined arrays idx:" + num14, new object[]
						{
							this.LOG_LEVEL
						});
					}
					Array.Copy(this.verts, mB_DynamicGameObject4.vertIdx, array10, num12, mB_DynamicGameObject4.numVerts);
					if (this._doNorm)
					{
						Array.Copy(this.normals, mB_DynamicGameObject4.vertIdx, destinationArray, num12, mB_DynamicGameObject4.numVerts);
					}
					if (this._doTan)
					{
						Array.Copy(this.tangents, mB_DynamicGameObject4.vertIdx, destinationArray2, num12, mB_DynamicGameObject4.numVerts);
					}
					if (this._doUV)
					{
						Array.Copy(this.uvs, mB_DynamicGameObject4.vertIdx, destinationArray3, num12, mB_DynamicGameObject4.numVerts);
					}
					if (this._doUV3)
					{
						Array.Copy(this.uv3s, mB_DynamicGameObject4.vertIdx, array7, num12, mB_DynamicGameObject4.numVerts);
					}
					if (this._doUV4)
					{
						Array.Copy(this.uv4s, mB_DynamicGameObject4.vertIdx, array8, num12, mB_DynamicGameObject4.numVerts);
					}
					if (this.doUV2())
					{
						Array.Copy(this.uv2s, mB_DynamicGameObject4.vertIdx, destinationArray4, num12, mB_DynamicGameObject4.numVerts);
					}
					if (this._doCol)
					{
						Array.Copy(this.colors, mB_DynamicGameObject4.vertIdx, array9, num12, mB_DynamicGameObject4.numVerts);
					}
					if (this.renderType == MB_RenderType.skinnedMeshRenderer)
					{
						Array.Copy(this.boneWeights, mB_DynamicGameObject4.vertIdx, array11, num12, mB_DynamicGameObject4.numVerts);
					}
					for (int num15 = 0; num15 < num; num15++)
					{
						int[] array16 = this.submeshTris[num15];
						int num16 = mB_DynamicGameObject4.submeshTriIdxs[num15];
						int num17 = mB_DynamicGameObject4.submeshNumTris[num15];
						if (this.LOG_LEVEL >= MB2_LogLevel.debug)
						{
							MB2_Log.LogDebug(string.Concat(new object[]
							{
								"    Adjusting submesh triangles submesh:",
								num15,
								" startIdx:",
								num16,
								" num:",
								num17
							}), new object[]
							{
								this.LOG_LEVEL
							});
						}
						for (int num18 = num16; num18 < num16 + num17; num18++)
						{
							array16[num18] -= num13;
						}
						Array.Copy(array16, num16, array14[num15], array15[num15], num17);
					}
					mB_DynamicGameObject4.vertIdx = num12;
					for (int num19 = 0; num19 < array15.Length; num19++)
					{
						mB_DynamicGameObject4.submeshTriIdxs[num19] = array15[num19];
						array15[num19] += mB_DynamicGameObject4.submeshNumTris[num19];
					}
					num12 += mB_DynamicGameObject4.numVerts;
				}
				else
				{
					if (this.LOG_LEVEL >= MB2_LogLevel.debug)
					{
						MB2_Log.LogDebug("Not copying obj: " + num14, new object[]
						{
							this.LOG_LEVEL
						});
					}
					num13 += mB_DynamicGameObject4.numVerts;
				}
			}
			if (this.renderType == MB_RenderType.skinnedMeshRenderer)
			{
				this._CopyBonesWeAreKeepingToNewBonesArrayAndAdjustBWIndexes(list, hashSet, array13, array12, array11, num2);
			}
			for (int num20 = this.mbDynamicObjectsInCombinedMesh.Count - 1; num20 >= 0; num20--)
			{
				if (this.mbDynamicObjectsInCombinedMesh[num20]._beingDeleted)
				{
					this.instance2Combined_MapRemove(this.mbDynamicObjectsInCombinedMesh[num20].instanceID);
					this.objectsInCombinedMesh.RemoveAt(num20);
					this.mbDynamicObjectsInCombinedMesh.RemoveAt(num20);
				}
			}
			this.verts = array10;
			if (this._doNorm)
			{
				this.normals = destinationArray;
			}
			if (this._doTan)
			{
				this.tangents = destinationArray2;
			}
			if (this._doUV)
			{
				this.uvs = destinationArray3;
			}
			if (this._doUV3)
			{
				this.uv3s = array7;
			}
			if (this._doUV4)
			{
				this.uv4s = array8;
			}
			if (this.doUV2())
			{
				this.uv2s = destinationArray4;
			}
			if (this._doCol)
			{
				this.colors = array9;
			}
			if (this.renderType == MB_RenderType.skinnedMeshRenderer)
			{
				this.boneWeights = array11;
			}
			int num21 = this.bones.Length - list.Count;
			this.bindPoses = array12;
			this.bones = array13;
			this.submeshTris = array14;
			int num22 = 0;
			foreach (MB3_MeshCombinerSingle.BoneAndBindpose current in hashSet)
			{
				array13[num21 + num22] = current.bone;
				array12[num21 + num22] = current.bindPose;
				num22++;
			}
			for (int num23 = 0; num23 < list2.Count; num23++)
			{
				MB3_MeshCombinerSingle.MB_DynamicGameObject mB_DynamicGameObject5 = list2[num23];
				GameObject gameObject2 = <_addToCombined>c__AnonStorey25F._goToAdd[num23];
				int num24 = num12;
				Mesh mesh2 = MB_Utility.GetMesh(gameObject2);
				Matrix4x4 localToWorldMatrix = gameObject2.transform.localToWorldMatrix;
				Matrix4x4 matrix4x = localToWorldMatrix;
				int arg_119A_1 = 0;
				int arg_119A_2 = 3;
				float num25 = 0f;
				matrix4x[2, 3] = num25;
				num25 = num25;
				matrix4x[1, 3] = num25;
				matrix4x[arg_119A_1, arg_119A_2] = num25;
				array10 = mesh2.vertices;
				Vector3[] array17 = null;
				Vector4[] array18 = null;
				if (this._doNorm)
				{
					array17 = this._getMeshNormals(mesh2);
				}
				if (this._doTan)
				{
					array18 = this._getMeshTangents(mesh2);
				}
				if (this.renderType != MB_RenderType.skinnedMeshRenderer)
				{
					for (int num26 = 0; num26 < array10.Length; num26++)
					{
						array10[num26] = localToWorldMatrix.MultiplyPoint3x4(array10[num26]);
						if (this._doNorm)
						{
							array17[num26] = matrix4x.MultiplyPoint3x4(array17[num26]);
							array17[num26] = array17[num26].normalized;
						}
						if (this._doTan)
						{
							float w = array18[num26].w;
							Vector3 v = matrix4x.MultiplyPoint3x4(array18[num26]);
							v.Normalize();
							array18[num26] = v;
							array18[num26].w = w;
						}
					}
				}
				if (this._doNorm)
				{
					array17.CopyTo(this.normals, num24);
				}
				if (this._doTan)
				{
					array18.CopyTo(this.tangents, num24);
				}
				array10.CopyTo(this.verts, num24);
				int num27 = mesh2.subMeshCount;
				if (mB_DynamicGameObject5.uvRects.Length < num27)
				{
					if (this.LOG_LEVEL >= MB2_LogLevel.debug)
					{
						MB2_Log.LogDebug("Mesh " + mB_DynamicGameObject5.name + " has more submeshes than materials", new object[0]);
					}
					num27 = mB_DynamicGameObject5.uvRects.Length;
				}
				else if (mB_DynamicGameObject5.uvRects.Length > num27 && this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning("Mesh " + mB_DynamicGameObject5.name + " has fewer submeshes than materials");
				}
				if (this._doUV)
				{
					this._copyAndAdjustUVsFromMesh(mB_DynamicGameObject5, mesh2, num24);
				}
				if (this.doUV2())
				{
					this._copyAndAdjustUV2FromMesh(mB_DynamicGameObject5, mesh2, num24);
				}
				if (this._doUV3)
				{
					array7 = MBVersion.GetMeshUV3orUV4(mesh2, true, this.LOG_LEVEL);
					array7.CopyTo(this.uv3s, num24);
				}
				if (this._doUV4)
				{
					array8 = MBVersion.GetMeshUV3orUV4(mesh2, false, this.LOG_LEVEL);
					array8.CopyTo(this.uv4s, num24);
				}
				if (this._doCol)
				{
					array9 = this._getMeshColors(mesh2);
					array9.CopyTo(this.colors, num24);
				}
				if (this.renderType == MB_RenderType.skinnedMeshRenderer)
				{
					Renderer renderer2 = MB_Utility.GetRenderer(gameObject2);
					this._AddBonesToNewBonesArrayAndAdjustBWIndexes(mB_DynamicGameObject5, renderer2, num24, array13, array11);
				}
				for (int num28 = 0; num28 < array15.Length; num28++)
				{
					mB_DynamicGameObject5.submeshTriIdxs[num28] = array15[num28];
				}
				for (int num29 = 0; num29 < mB_DynamicGameObject5._submeshTris.Length; num29++)
				{
					int[] array19 = mB_DynamicGameObject5._submeshTris[num29];
					for (int num30 = 0; num30 < array19.Length; num30++)
					{
						array19[num30] += num24;
					}
					if (mB_DynamicGameObject5.invertTriangles)
					{
						for (int num31 = 0; num31 < array19.Length; num31 += 3)
						{
							int num32 = array19[num31];
							array19[num31] = array19[num31 + 1];
							array19[num31 + 1] = num32;
						}
					}
					int num33 = mB_DynamicGameObject5.targetSubmeshIdxs[num29];
					array19.CopyTo(this.submeshTris[num33], array15[num33]);
					mB_DynamicGameObject5.submeshNumTris[num33] += array19.Length;
					array15[num33] += array19.Length;
				}
				mB_DynamicGameObject5.vertIdx = num12;
				this.instance2Combined_MapAdd(gameObject2.GetInstanceID(), mB_DynamicGameObject5);
				this.objectsInCombinedMesh.Add(gameObject2);
				this.mbDynamicObjectsInCombinedMesh.Add(mB_DynamicGameObject5);
				num12 += array10.Length;
				for (int num34 = 0; num34 < mB_DynamicGameObject5._submeshTris.Length; num34++)
				{
					mB_DynamicGameObject5._submeshTris[num34] = null;
				}
				mB_DynamicGameObject5._submeshTris = null;
				if (this.LOG_LEVEL >= MB2_LogLevel.debug)
				{
					MB2_Log.LogDebug(string.Concat(new object[]
					{
						"Added to combined:",
						mB_DynamicGameObject5.name,
						" verts:",
						array10.Length,
						" bindPoses:",
						array12.Length
					}), new object[]
					{
						this.LOG_LEVEL
					});
				}
			}
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				MB2_Log.LogDebug("===== _addToCombined completed. Verts in buffer: " + this.verts.Length, new object[]
				{
					this.LOG_LEVEL
				});
			}
			return true;
		}

		private void _copyAndAdjustUVsFromMesh(MB3_MeshCombinerSingle.MB_DynamicGameObject dgo, Mesh mesh, int vertsIdx)
		{
			Vector2[] array = this._getMeshUVs(mesh);
			bool flag = true;
			if (!this.textureBakeResults.fixOutOfBoundsUVs)
			{
				Rect rhs = new Rect(0f, 0f, 1f, 1f);
				bool flag2 = true;
				for (int i = 0; i < this.textureBakeResults.prefabUVRects.Length; i++)
				{
					if (this.textureBakeResults.prefabUVRects[i] != rhs)
					{
						flag2 = false;
						break;
					}
				}
				if (flag2)
				{
					flag = false;
					if (this.LOG_LEVEL >= MB2_LogLevel.debug)
					{
						Debug.Log("All atlases have only one texture in atlas UVs will be copied without adjusting");
					}
				}
			}
			if (flag)
			{
				int[] array2 = new int[array.Length];
				for (int j = 0; j < array2.Length; j++)
				{
					array2[j] = -1;
				}
				bool flag3 = false;
				Rect rect = default(Rect);
				for (int k = 0; k < dgo.targetSubmeshIdxs.Length; k++)
				{
					int[] array3;
					if (dgo._submeshTris != null)
					{
						array3 = dgo._submeshTris[k];
					}
					else
					{
						array3 = mesh.GetTriangles(k);
					}
					Rect rect2 = dgo.uvRects[k];
					if (this.textureBakeResults.fixOutOfBoundsUVs)
					{
						rect = dgo.obUVRects[k];
					}
					for (int l = 0; l < array3.Length; l++)
					{
						int num = array3[l];
						if (array2[num] == -1)
						{
							array2[num] = k;
							if (this.textureBakeResults.fixOutOfBoundsUVs)
							{
								array[num].x = array[num].x / rect.width - rect.x / rect.width;
								array[num].y = array[num].y / rect.height - rect.y / rect.height;
							}
							array[num].x = rect2.x + array[num].x * rect2.width;
							array[num].y = rect2.y + array[num].y * rect2.height;
						}
						if (array2[num] != k)
						{
							flag3 = true;
						}
					}
				}
				if (flag3 && this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning(dgo.name + "has submeshes which share verticies. Adjusted uvs may not map correctly in combined atlas.");
				}
			}
			array.CopyTo(this.uvs, vertsIdx);
			if (this.LOG_LEVEL >= MB2_LogLevel.trace)
			{
				Debug.Log(string.Format("_copyAndAdjustUVsFromMesh copied {0} verts", array.Length));
			}
		}

		private void _copyAndAdjustUV2FromMesh(MB3_MeshCombinerSingle.MB_DynamicGameObject dgo, Mesh mesh, int vertsIdx)
		{
			Vector2[] array = this._getMeshUV2s(mesh);
			if (this.lightmapOption == MB2_LightmapOptions.preserve_current_lightmapping)
			{
				Vector4 lightmapTilingOffset = dgo.lightmapTilingOffset;
				Vector2 vector = new Vector2(lightmapTilingOffset.x, lightmapTilingOffset.y);
				Vector2 a = new Vector2(lightmapTilingOffset.z, lightmapTilingOffset.w);
				for (int i = 0; i < array.Length; i++)
				{
					Vector2 b;
					b.x = vector.x * array[i].x;
					b.y = vector.y * array[i].y;
					array[i] = a + b;
				}
				if (this.LOG_LEVEL >= MB2_LogLevel.trace)
				{
					Debug.Log("_copyAndAdjustUV2FromMesh copied and modify for preserve current lightmapping " + array.Length);
				}
			}
			else if (this.LOG_LEVEL >= MB2_LogLevel.trace)
			{
				Debug.Log("_copyAndAdjustUV2FromMesh copied without modifying " + array.Length);
			}
			array.CopyTo(this.uv2s, vertsIdx);
		}

		private Color[] _getMeshColors(Mesh m)
		{
			Color[] array = m.colors;
			if (array.Length == 0)
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.debug)
				{
					MB2_Log.LogDebug("Mesh " + m + " has no colors. Generating", new object[0]);
				}
				if (this._doCol && this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning("Mesh " + m + " didn't have colors. Generating an array of white colors");
				}
				array = new Color[m.vertexCount];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = Color.white;
				}
			}
			return array;
		}

		private Vector3[] _getMeshNormals(Mesh m)
		{
			Vector3[] array = m.normals;
			if (array.Length == 0)
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.debug)
				{
					MB2_Log.LogDebug("Mesh " + m + " has no normals. Generating", new object[0]);
				}
				if (this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning("Mesh " + m + " didn't have normals. Generating normals.");
				}
				Mesh mesh = UnityEngine.Object.Instantiate<Mesh>(m);
				mesh.RecalculateNormals();
				array = mesh.normals;
				MB_Utility.Destroy(mesh);
			}
			return array;
		}

		private Vector4[] _getMeshTangents(Mesh m)
		{
			Vector4[] array = m.tangents;
			if (array.Length == 0)
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.debug)
				{
					MB2_Log.LogDebug("Mesh " + m + " has no tangents. Generating", new object[0]);
				}
				if (this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning("Mesh " + m + " didn't have tangents. Generating tangents.");
				}
				Vector3[] vertices = m.vertices;
				Vector2[] array2 = this._getMeshUVs(m);
				Vector3[] array3 = this._getMeshNormals(m);
				array = new Vector4[m.vertexCount];
				for (int i = 0; i < m.subMeshCount; i++)
				{
					int[] triangles = m.GetTriangles(i);
					this._generateTangents(triangles, vertices, array2, array3, array);
				}
			}
			return array;
		}

		private Vector2[] _getMeshUVs(Mesh m)
		{
			Vector2[] array = m.uv;
			if (array.Length == 0)
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.debug)
				{
					MB2_Log.LogDebug("Mesh " + m + " has no uvs. Generating", new object[0]);
				}
				if (this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning("Mesh " + m + " didn't have uvs. Generating uvs.");
				}
				array = new Vector2[m.vertexCount];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = this._HALF_UV;
				}
			}
			return array;
		}

		private Vector2[] _getMeshUV2s(Mesh m)
		{
			Vector2[] array = m.uv2;
			if (array.Length == 0)
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.debug)
				{
					MB2_Log.LogDebug("Mesh " + m + " has no uv2s. Generating", new object[0]);
				}
				if (this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning(string.Concat(new object[]
					{
						"Mesh ",
						m,
						" didn't have uv2s. Lightmapping option was set to ",
						this.lightmapOption,
						" Generating uv2s."
					}));
				}
				array = new Vector2[m.vertexCount];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = this._HALF_UV;
				}
			}
			return array;
		}

		public override void UpdateSkinnedMeshApproximateBounds()
		{
			this.UpdateSkinnedMeshApproximateBoundsFromBounds();
		}

		public override void UpdateSkinnedMeshApproximateBoundsFromBones()
		{
			if (this.outputOption == MB2_OutputOptions.bakeMeshAssetsInPlace)
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning("Can't UpdateSkinnedMeshApproximateBounds when output type is bakeMeshAssetsInPlace");
				}
				return;
			}
			if (this.bones.Length == 0)
			{
				if (this.verts.Length > 0 && this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning("No bones in SkinnedMeshRenderer. Could not UpdateSkinnedMeshApproximateBounds.");
				}
				return;
			}
			if (this._targetRenderer == null)
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning("Target Renderer is not set. No point in calling UpdateSkinnedMeshApproximateBounds.");
				}
				return;
			}
			if (!this._targetRenderer.GetType().Equals(typeof(SkinnedMeshRenderer)))
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning("Target Renderer is not a SkinnedMeshRenderer. No point in calling UpdateSkinnedMeshApproximateBounds.");
				}
				return;
			}
			MB3_MeshCombiner.UpdateSkinnedMeshApproximateBoundsFromBonesStatic(this.bones, (SkinnedMeshRenderer)this.targetRenderer);
		}

		public override void UpdateSkinnedMeshApproximateBoundsFromBounds()
		{
			if (this.outputOption == MB2_OutputOptions.bakeMeshAssetsInPlace)
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning("Can't UpdateSkinnedMeshApproximateBoundsFromBounds when output type is bakeMeshAssetsInPlace");
				}
				return;
			}
			if (this.verts.Length == 0 || this.objectsInCombinedMesh.Count == 0)
			{
				if (this.verts.Length > 0 && this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning("Nothing in SkinnedMeshRenderer. Could not UpdateSkinnedMeshApproximateBoundsFromBounds.");
				}
				return;
			}
			if (this._targetRenderer == null)
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning("Target Renderer is not set. No point in calling UpdateSkinnedMeshApproximateBoundsFromBounds.");
				}
				return;
			}
			if (!this._targetRenderer.GetType().Equals(typeof(SkinnedMeshRenderer)))
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning("Target Renderer is not a SkinnedMeshRenderer. No point in calling UpdateSkinnedMeshApproximateBoundsFromBounds.");
				}
				return;
			}
			MB3_MeshCombiner.UpdateSkinnedMeshApproximateBoundsFromBoundsStatic(this.objectsInCombinedMesh, (SkinnedMeshRenderer)this.targetRenderer);
		}

		private int _getNumBones(Renderer r)
		{
			if (this.renderType != MB_RenderType.skinnedMeshRenderer)
			{
				return 0;
			}
			if (r is SkinnedMeshRenderer)
			{
				return ((SkinnedMeshRenderer)r).bones.Length;
			}
			if (r is MeshRenderer)
			{
				return 1;
			}
			Debug.LogError("Could not _getNumBones. Object does not have a renderer");
			return 0;
		}

		private Transform[] _getBones(Renderer r)
		{
			return MBVersion.GetBones(r);
		}

		private Matrix4x4[] _getBindPoses(Renderer r)
		{
			if (r is SkinnedMeshRenderer)
			{
				return ((SkinnedMeshRenderer)r).sharedMesh.bindposes;
			}
			if (r is MeshRenderer)
			{
				Matrix4x4 identity = Matrix4x4.identity;
				return new Matrix4x4[]
				{
					identity
				};
			}
			Debug.LogError("Could not _getBindPoses. Object does not have a renderer");
			return null;
		}

		private BoneWeight[] _getBoneWeights(Renderer r, int numVertsInMeshBeingAdded)
		{
			if (r is SkinnedMeshRenderer)
			{
				return ((SkinnedMeshRenderer)r).sharedMesh.boneWeights;
			}
			if (r is MeshRenderer)
			{
				BoneWeight boneWeight = default(BoneWeight);
				int num = 0;
				boneWeight.boneIndex3 = num;
				num = num;
				boneWeight.boneIndex2 = num;
				num = num;
				boneWeight.boneIndex1 = num;
				boneWeight.boneIndex0 = num;
				boneWeight.weight0 = 1f;
				float num2 = 0f;
				boneWeight.weight3 = num2;
				num2 = num2;
				boneWeight.weight2 = num2;
				boneWeight.weight1 = num2;
				BoneWeight[] array = new BoneWeight[numVertsInMeshBeingAdded];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = boneWeight;
				}
				return array;
			}
			Debug.LogError("Could not _getBoneWeights. Object does not have a renderer");
			return null;
		}

		public override void Apply(MB3_MeshCombiner.GenerateUV2Delegate uv2GenerationMethod)
		{
			bool flag = false;
			if (this.renderType == MB_RenderType.skinnedMeshRenderer)
			{
				flag = true;
			}
			this.Apply(true, true, this._doNorm, this._doTan, this._doUV, this.doUV2(), this._doUV3, this._doUV4, this.doCol, flag, uv2GenerationMethod);
		}

		public virtual void ApplyShowHide()
		{
			if (this._validationLevel >= MB2_ValidationLevel.quick && !this.ValidateTargRendererAndMeshAndResultSceneObj())
			{
				return;
			}
			if (this._mesh != null)
			{
				if (this.renderType == MB_RenderType.meshRenderer)
				{
					MBVersion.MeshClear(this._mesh, true);
					this._mesh.vertices = this.verts;
				}
				int[][] submeshTrisWithShowHideApplied = this.GetSubmeshTrisWithShowHideApplied();
				if (this.textureBakeResults.doMultiMaterial)
				{
					this._mesh.subMeshCount = submeshTrisWithShowHideApplied.Length;
					for (int i = 0; i < submeshTrisWithShowHideApplied.Length; i++)
					{
						this._mesh.SetTriangles(submeshTrisWithShowHideApplied[i], i);
					}
				}
				else
				{
					this._mesh.triangles = submeshTrisWithShowHideApplied[0];
				}
				if (this.LOG_LEVEL >= MB2_LogLevel.trace)
				{
					Debug.Log("ApplyShowHide");
				}
			}
			else
			{
				Debug.LogError("Need to add objects to this meshbaker before calling ApplyShowHide");
			}
		}

		public override void Apply(bool triangles, bool vertices, bool normals, bool tangents, bool uvs, bool uv2, bool uv3, bool uv4, bool colors, bool bones = false, MB3_MeshCombiner.GenerateUV2Delegate uv2GenerationMethod = null)
		{
			if (this._validationLevel >= MB2_ValidationLevel.quick && !this.ValidateTargRendererAndMeshAndResultSceneObj())
			{
				return;
			}
			if (this._mesh != null)
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.trace)
				{
					Debug.Log(string.Format("Apply called tri={0} vert={1} norm={2} tan={3} uv={4} col={5} uv3={6} uv4={7} uv2={8} bone={9} meshID={10}", new object[]
					{
						triangles,
						vertices,
						normals,
						tangents,
						uvs,
						colors,
						uv3,
						uv4,
						uv2,
						bones,
						this._mesh.GetInstanceID()
					}));
				}
				if (triangles || this._mesh.vertexCount != this.verts.Length)
				{
					if (triangles && !vertices && !normals && !tangents && !uvs && !colors && !uv3 && !uv4 && !uv2 && !bones)
					{
						MBVersion.MeshClear(this._mesh, true);
					}
					else
					{
						MBVersion.MeshClear(this._mesh, false);
					}
				}
				if (vertices)
				{
					this._mesh.vertices = this.verts;
				}
				if (triangles && this._textureBakeResults)
				{
					if (this._textureBakeResults == null)
					{
						Debug.LogError("Material Bake Result was not set.");
					}
					else
					{
						int[][] submeshTrisWithShowHideApplied = this.GetSubmeshTrisWithShowHideApplied();
						if (this._textureBakeResults.doMultiMaterial)
						{
							this._mesh.subMeshCount = submeshTrisWithShowHideApplied.Length;
							for (int i = 0; i < submeshTrisWithShowHideApplied.Length; i++)
							{
								this._mesh.SetTriangles(submeshTrisWithShowHideApplied[i], i);
							}
						}
						else
						{
							this._mesh.triangles = submeshTrisWithShowHideApplied[0];
						}
					}
				}
				if (normals)
				{
					if (this._doNorm)
					{
						this._mesh.normals = this.normals;
					}
					else
					{
						Debug.LogError("normal flag was set in Apply but MeshBaker didn't generate normals");
					}
				}
				if (tangents)
				{
					if (this._doTan)
					{
						this._mesh.tangents = this.tangents;
					}
					else
					{
						Debug.LogError("tangent flag was set in Apply but MeshBaker didn't generate tangents");
					}
				}
				if (uvs)
				{
					if (this._doUV)
					{
						this._mesh.uv = this.uvs;
					}
					else
					{
						Debug.LogError("uv flag was set in Apply but MeshBaker didn't generate uvs");
					}
				}
				if (colors)
				{
					if (this._doCol)
					{
						this._mesh.colors = this.colors;
					}
					else
					{
						Debug.LogError("color flag was set in Apply but MeshBaker didn't generate colors");
					}
				}
				if (uv3)
				{
					if (this._doUV3)
					{
						MBVersion.MeshAssignUV3(this._mesh, this.uv3s);
					}
					else
					{
						Debug.LogError("uv3 flag was set in Apply but MeshBaker didn't generate uv3s");
					}
				}
				if (uv4)
				{
					if (this._doUV4)
					{
						MBVersion.MeshAssignUV4(this._mesh, this.uv4s);
					}
					else
					{
						Debug.LogError("uv4 flag was set in Apply but MeshBaker didn't generate uv4s");
					}
				}
				if (uv2)
				{
					if (this.doUV2())
					{
						this._mesh.uv2 = this.uv2s;
					}
					else
					{
						Debug.LogError("uv2 flag was set in Apply but lightmapping option was set to " + this.lightmapOption);
					}
				}
				bool flag = false;
				if (this.renderType != MB_RenderType.skinnedMeshRenderer && this.lightmapOption == MB2_LightmapOptions.generate_new_UV2_layout)
				{
					if (uv2GenerationMethod != null)
					{
						uv2GenerationMethod(this._mesh, this.uv2UnwrappingParamsHardAngle, this.uv2UnwrappingParamsPackMargin);
						if (this.LOG_LEVEL >= MB2_LogLevel.trace)
						{
							Debug.Log("generating new UV2 layout for the combined mesh ");
						}
					}
					else
					{
						Debug.LogError("No GenerateUV2Delegate method was supplied. UV2 cannot be generated.");
					}
					flag = true;
				}
				else if (this.renderType == MB_RenderType.skinnedMeshRenderer && this.lightmapOption == MB2_LightmapOptions.generate_new_UV2_layout && this.LOG_LEVEL >= MB2_LogLevel.warn)
				{
					Debug.LogWarning("UV2 cannot be generated for SkinnedMeshRenderer objects.");
				}
				if (this.renderType != MB_RenderType.skinnedMeshRenderer && this.lightmapOption == MB2_LightmapOptions.generate_new_UV2_layout && !flag)
				{
					Debug.LogError("Failed to generate new UV2 layout. Only works in editor.");
				}
				if (this.renderType == MB_RenderType.skinnedMeshRenderer)
				{
					if (this.verts.Length == 0)
					{
						this.targetRenderer.enabled = false;
					}
					else
					{
						this.targetRenderer.enabled = true;
					}
					bool updateWhenOffscreen = ((SkinnedMeshRenderer)this.targetRenderer).updateWhenOffscreen;
					((SkinnedMeshRenderer)this.targetRenderer).updateWhenOffscreen = true;
					((SkinnedMeshRenderer)this.targetRenderer).updateWhenOffscreen = updateWhenOffscreen;
				}
				if (bones)
				{
					this._mesh.bindposes = this.bindPoses;
					this._mesh.boneWeights = this.boneWeights;
				}
				if (triangles || vertices)
				{
					if (this.LOG_LEVEL >= MB2_LogLevel.trace)
					{
						Debug.Log("recalculating bounds on mesh.");
					}
					this._mesh.RecalculateBounds();
				}
			}
			else
			{
				Debug.LogError("Need to add objects to this meshbaker before calling Apply or ApplyAll");
			}
		}

		public int[][] GetSubmeshTrisWithShowHideApplied()
		{
			bool flag = false;
			for (int i = 0; i < this.mbDynamicObjectsInCombinedMesh.Count; i++)
			{
				if (!this.mbDynamicObjectsInCombinedMesh[i].show)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				int[] array = new int[this.submeshTris.Length];
				int[][] array2 = new int[this.submeshTris.Length][];
				for (int j = 0; j < this.mbDynamicObjectsInCombinedMesh.Count; j++)
				{
					MB3_MeshCombinerSingle.MB_DynamicGameObject mB_DynamicGameObject = this.mbDynamicObjectsInCombinedMesh[j];
					if (mB_DynamicGameObject.show)
					{
						for (int k = 0; k < mB_DynamicGameObject.submeshNumTris.Length; k++)
						{
							array[k] += mB_DynamicGameObject.submeshNumTris[k];
						}
					}
				}
				for (int l = 0; l < array2.Length; l++)
				{
					array2[l] = new int[array[l]];
				}
				int[] array3 = new int[array2.Length];
				for (int m = 0; m < this.mbDynamicObjectsInCombinedMesh.Count; m++)
				{
					MB3_MeshCombinerSingle.MB_DynamicGameObject mB_DynamicGameObject2 = this.mbDynamicObjectsInCombinedMesh[m];
					if (mB_DynamicGameObject2.show)
					{
						for (int n = 0; n < this.submeshTris.Length; n++)
						{
							int[] array4 = this.submeshTris[n];
							int num = mB_DynamicGameObject2.submeshTriIdxs[n];
							int num2 = num + mB_DynamicGameObject2.submeshNumTris[n];
							for (int num3 = num; num3 < num2; num3++)
							{
								array2[n][array3[n]] = array4[num3];
								array3[n]++;
							}
						}
					}
				}
				return array2;
			}
			return this.submeshTris;
		}

		public override void UpdateGameObjects(GameObject[] gos, bool recalcBounds = true, bool updateVertices = true, bool updateNormals = true, bool updateTangents = true, bool updateUV = false, bool updateUV2 = false, bool updateUV3 = false, bool updateUV4 = false, bool updateColors = false, bool updateSkinningInfo = false)
		{
			this._updateGameObjects(gos, recalcBounds, updateVertices, updateNormals, updateTangents, updateUV, updateUV2, updateUV3, updateUV4, updateColors, updateSkinningInfo);
		}

		private void _updateGameObjects(GameObject[] gos, bool recalcBounds, bool updateVertices, bool updateNormals, bool updateTangents, bool updateUV, bool updateUV2, bool updateUV3, bool updateUV4, bool updateColors, bool updateSkinningInfo)
		{
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				Debug.Log("UpdateGameObjects called on " + gos.Length + " objects.");
			}
			this._initialize();
			if (this._mesh.vertexCount > 0 && this._instance2combined_map.Count == 0)
			{
				Debug.LogWarning("There were vertices in the combined mesh but nothing in the MeshBaker buffers. If you are trying to bake in the editor and modify at runtime, make sure 'Clear Buffers After Bake' is unchecked.");
			}
			for (int i = 0; i < gos.Length; i++)
			{
				this._updateGameObject(gos[i], updateVertices, updateNormals, updateTangents, updateUV, updateUV2, updateUV3, updateUV4, updateColors, updateSkinningInfo);
			}
			if (recalcBounds)
			{
				this._mesh.RecalculateBounds();
			}
		}

		private void _updateGameObject(GameObject go, bool updateVertices, bool updateNormals, bool updateTangents, bool updateUV, bool updateUV2, bool updateUV3, bool updateUV4, bool updateColors, bool updateSkinningInfo)
		{
			MB3_MeshCombinerSingle.MB_DynamicGameObject mB_DynamicGameObject = null;
			if (!this.instance2Combined_MapTryGetValue(go.GetInstanceID(), out mB_DynamicGameObject))
			{
				Debug.LogError("Object " + go.name + " has not been added");
				return;
			}
			Mesh mesh = MB_Utility.GetMesh(go);
			if (mB_DynamicGameObject.numVerts != mesh.vertexCount)
			{
				Debug.LogError("Object " + go.name + " source mesh has been modified since being added. To update it must have the same number of verts");
				return;
			}
			if (this._doUV && updateUV)
			{
				this._copyAndAdjustUVsFromMesh(mB_DynamicGameObject, mesh, mB_DynamicGameObject.vertIdx);
			}
			if (this.doUV2() && updateUV2)
			{
				this._copyAndAdjustUV2FromMesh(mB_DynamicGameObject, mesh, mB_DynamicGameObject.vertIdx);
			}
			if (this.renderType == MB_RenderType.skinnedMeshRenderer && updateSkinningInfo)
			{
				Renderer renderer = MB_Utility.GetRenderer(go);
				BoneWeight[] array = this._getBoneWeights(renderer, mB_DynamicGameObject.numVerts);
				Transform[] array2 = this._getBones(renderer);
				int num = mB_DynamicGameObject.vertIdx;
				bool flag = false;
				for (int i = 0; i < array.Length; i++)
				{
					if (array2[array[i].boneIndex0] != this.bones[this.boneWeights[num].boneIndex0])
					{
						flag = true;
						break;
					}
					this.boneWeights[num].weight0 = array[i].weight0;
					this.boneWeights[num].weight1 = array[i].weight1;
					this.boneWeights[num].weight2 = array[i].weight2;
					this.boneWeights[num].weight3 = array[i].weight3;
					num++;
				}
				if (flag)
				{
					Debug.LogError("Detected that some of the boneweights reference different bones than when initial added. Boneweights must reference the same bones " + mB_DynamicGameObject.name);
				}
			}
			Matrix4x4 localToWorldMatrix = go.transform.localToWorldMatrix;
			if (updateVertices)
			{
				Vector3[] vertices = mesh.vertices;
				for (int j = 0; j < vertices.Length; j++)
				{
					this.verts[mB_DynamicGameObject.vertIdx + j] = localToWorldMatrix.MultiplyPoint3x4(vertices[j]);
				}
			}
			int arg_259_1 = 0;
			int arg_259_2 = 3;
			float num2 = 0f;
			localToWorldMatrix[2, 3] = num2;
			num2 = num2;
			localToWorldMatrix[1, 3] = num2;
			localToWorldMatrix[arg_259_1, arg_259_2] = num2;
			if (this._doNorm && updateNormals)
			{
				Vector3[] array3 = this._getMeshNormals(mesh);
				for (int k = 0; k < array3.Length; k++)
				{
					array3[k] = localToWorldMatrix.MultiplyPoint3x4(array3[k]);
					this.normals[mB_DynamicGameObject.vertIdx + k] = array3[k].normalized;
				}
			}
			if (this._doTan && updateTangents)
			{
				Vector4[] array4 = this._getMeshTangents(mesh);
				for (int l = 0; l < array4.Length; l++)
				{
					int num3 = mB_DynamicGameObject.vertIdx + l;
					float w = array4[l].w;
					Vector3 v = localToWorldMatrix.MultiplyPoint3x4(array4[l]);
					v.Normalize();
					this.tangents[num3] = v;
					this.tangents[num3].w = w;
				}
			}
			if (this._doCol && updateColors)
			{
				Color[] array5 = this._getMeshColors(mesh);
				for (int m = 0; m < array5.Length; m++)
				{
					this.colors[mB_DynamicGameObject.vertIdx + m] = array5[m];
				}
			}
			if (this._doUV3 && updateUV3)
			{
				Vector2[] meshUV3orUV = MBVersion.GetMeshUV3orUV4(mesh, true, this.LOG_LEVEL);
				for (int n = 0; n < meshUV3orUV.Length; n++)
				{
					this.uv3s[mB_DynamicGameObject.vertIdx + n] = meshUV3orUV[n];
				}
			}
			if (this._doUV4 && updateUV4)
			{
				Vector2[] meshUV3orUV2 = MBVersion.GetMeshUV3orUV4(mesh, false, this.LOG_LEVEL);
				for (int num4 = 0; num4 < meshUV3orUV2.Length; num4++)
				{
					this.uv4s[mB_DynamicGameObject.vertIdx + num4] = meshUV3orUV2[num4];
				}
			}
		}

		public bool ShowHideGameObjects(GameObject[] toShow, GameObject[] toHide)
		{
			return this._showHide(toShow, toHide);
		}

		public override bool AddDeleteGameObjects(GameObject[] gos, GameObject[] deleteGOs, bool disableRendererInSource = true)
		{
			int[] array = null;
			if (deleteGOs != null)
			{
				array = new int[deleteGOs.Length];
				for (int i = 0; i < deleteGOs.Length; i++)
				{
					if (deleteGOs[i] == null)
					{
						Debug.LogError("The " + i + "th object on the list of objects to delete is 'Null'");
					}
					else
					{
						array[i] = deleteGOs[i].GetInstanceID();
					}
				}
			}
			return this.AddDeleteGameObjectsByID(gos, array, disableRendererInSource);
		}

		public override bool AddDeleteGameObjectsByID(GameObject[] gos, int[] deleteGOinstanceIDs, bool disableRendererInSource)
		{
			if (this.validationLevel > MB2_ValidationLevel.none)
			{
				if (gos != null)
				{
					for (int i = 0; i < gos.Length; i++)
					{
						if (gos[i] == null)
						{
							Debug.LogError("The " + i + "th object on the list of objects to combine is 'None'. Use Command-Delete on Mac OS X; Delete or Shift-Delete on Windows to remove this one element.");
							return false;
						}
						if (this.validationLevel >= MB2_ValidationLevel.robust)
						{
							for (int j = i + 1; j < gos.Length; j++)
							{
								if (gos[i] == gos[j])
								{
									Debug.LogError("GameObject " + gos[i] + " appears twice in list of game objects to add");
									return false;
								}
							}
						}
					}
				}
				if (deleteGOinstanceIDs != null && this.validationLevel >= MB2_ValidationLevel.robust)
				{
					for (int k = 0; k < deleteGOinstanceIDs.Length; k++)
					{
						for (int l = k + 1; l < deleteGOinstanceIDs.Length; l++)
						{
							if (deleteGOinstanceIDs[k] == deleteGOinstanceIDs[l])
							{
								Debug.LogError("GameObject " + deleteGOinstanceIDs[k] + "appears twice in list of game objects to delete");
								return false;
							}
						}
					}
				}
			}
			if (this._usingTemporaryTextureBakeResult && gos != null && gos.Length > 0)
			{
				MB_Utility.Destroy(this._textureBakeResults);
				this._textureBakeResults = null;
				this._usingTemporaryTextureBakeResult = false;
			}
			if (this._textureBakeResults == null && gos != null && gos.Length > 0 && gos[0] != null && !this._CheckIfAllObjsToAddUseSameMaterialsAndCreateTemporaryTextrueBakeResult(gos))
			{
				return false;
			}
			this.BuildSceneMeshObject(gos, false);
			if (!this._addToCombined(gos, deleteGOinstanceIDs, disableRendererInSource))
			{
				Debug.LogError("Failed to add/delete objects to combined mesh");
				return false;
			}
			if (this.targetRenderer != null)
			{
				if (this.renderType == MB_RenderType.skinnedMeshRenderer)
				{
					SkinnedMeshRenderer skinnedMeshRenderer = (SkinnedMeshRenderer)this.targetRenderer;
					skinnedMeshRenderer.bones = this.bones;
					this.UpdateSkinnedMeshApproximateBoundsFromBounds();
				}
				this.targetRenderer.lightmapIndex = this.GetLightmapIndex();
			}
			return true;
		}

		public override bool CombinedMeshContains(GameObject go)
		{
			return this.objectsInCombinedMesh.Contains(go);
		}

		public override void ClearBuffers()
		{
			this.verts = new Vector3[0];
			this.normals = new Vector3[0];
			this.tangents = new Vector4[0];
			this.uvs = new Vector2[0];
			this.uv2s = new Vector2[0];
			this.uv3s = new Vector2[0];
			this.uv4s = new Vector2[0];
			this.colors = new Color[0];
			this.bones = new Transform[0];
			this.bindPoses = new Matrix4x4[0];
			this.boneWeights = new BoneWeight[0];
			this.submeshTris = new int[0][];
			this.mbDynamicObjectsInCombinedMesh.Clear();
			this.objectsInCombinedMesh.Clear();
			this.instance2Combined_MapClear();
			if (this._usingTemporaryTextureBakeResult)
			{
				MB_Utility.Destroy(this._textureBakeResults);
				this._textureBakeResults = null;
				this._usingTemporaryTextureBakeResult = false;
			}
			if (this.LOG_LEVEL >= MB2_LogLevel.trace)
			{
				MB2_Log.LogDebug("ClearBuffers called", new object[0]);
			}
		}

		public override void ClearMesh()
		{
			if (this._mesh != null)
			{
				MBVersion.MeshClear(this._mesh, false);
			}
			else
			{
				this._mesh = new Mesh();
			}
			this.ClearBuffers();
		}

		public override void DestroyMesh()
		{
			if (this._mesh != null)
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.debug)
				{
					MB2_Log.LogDebug("Destroying Mesh", new object[0]);
				}
				MB_Utility.Destroy(this._mesh);
			}
			this._mesh = new Mesh();
			this.ClearBuffers();
		}

		public override void DestroyMeshEditor(MB2_EditorMethodsInterface editorMethods)
		{
			if (this._mesh != null)
			{
				if (this.LOG_LEVEL >= MB2_LogLevel.debug)
				{
					MB2_Log.LogDebug("Destroying Mesh", new object[0]);
				}
				editorMethods.Destroy(this._mesh);
			}
			this._mesh = new Mesh();
			this.ClearBuffers();
		}

		private void _generateTangents(int[] triangles, Vector3[] verts, Vector2[] uvs, Vector3[] normals, Vector4[] outTangents)
		{
			int num = triangles.Length;
			int num2 = verts.Length;
			Vector3[] array = new Vector3[num2];
			Vector3[] array2 = new Vector3[num2];
			for (int i = 0; i < num; i += 3)
			{
				int num3 = triangles[i];
				int num4 = triangles[i + 1];
				int num5 = triangles[i + 2];
				Vector3 vector = verts[num3];
				Vector3 vector2 = verts[num4];
				Vector3 vector3 = verts[num5];
				Vector2 vector4 = uvs[num3];
				Vector2 vector5 = uvs[num4];
				Vector2 vector6 = uvs[num5];
				float num6 = vector2.x - vector.x;
				float num7 = vector3.x - vector.x;
				float num8 = vector2.y - vector.y;
				float num9 = vector3.y - vector.y;
				float num10 = vector2.z - vector.z;
				float num11 = vector3.z - vector.z;
				float num12 = vector5.x - vector4.x;
				float num13 = vector6.x - vector4.x;
				float num14 = vector5.y - vector4.y;
				float num15 = vector6.y - vector4.y;
				float num16 = num12 * num15 - num13 * num14;
				if (num16 == 0f)
				{
					Debug.LogError("Could not compute tangents. All UVs need to form a valid triangles in UV space. If any UV triangles are collapsed, tangents cannot be generated.");
					return;
				}
				float num17 = 1f / num16;
				Vector3 b = new Vector3((num15 * num6 - num14 * num7) * num17, (num15 * num8 - num14 * num9) * num17, (num15 * num10 - num14 * num11) * num17);
				Vector3 b2 = new Vector3((num12 * num7 - num13 * num6) * num17, (num12 * num9 - num13 * num8) * num17, (num12 * num11 - num13 * num10) * num17);
				array[num3] += b;
				array[num4] += b;
				array[num5] += b;
				array2[num3] += b2;
				array2[num4] += b2;
				array2[num5] += b2;
			}
			for (int j = 0; j < num2; j++)
			{
				Vector3 vector7 = normals[j];
				Vector3 vector8 = array[j];
				Vector3 normalized = (vector8 - vector7 * Vector3.Dot(vector7, vector8)).normalized;
				outTangents[j] = new Vector4(normalized.x, normalized.y, normalized.z);
				outTangents[j].w = ((Vector3.Dot(Vector3.Cross(vector7, vector8), array2[j]) >= 0f) ? 1f : -1f);
			}
		}

		public bool ValidateTargRendererAndMeshAndResultSceneObj()
		{
			if (this._resultSceneObject == null)
			{
				if (this._LOG_LEVEL >= MB2_LogLevel.error)
				{
					Debug.LogError("Result Scene Object was not set.");
				}
				return false;
			}
			if (this._targetRenderer == null)
			{
				if (this._LOG_LEVEL >= MB2_LogLevel.error)
				{
					Debug.LogError("Target Renderer was not set.");
				}
				return false;
			}
			if (this._targetRenderer.transform.parent != this._resultSceneObject.transform)
			{
				if (this._LOG_LEVEL >= MB2_LogLevel.error)
				{
					Debug.LogError("Target Renderer game object is not a child of Result Scene Object was not set.");
				}
				return false;
			}
			if (this._renderType == MB_RenderType.skinnedMeshRenderer)
			{
				if (!(this._targetRenderer is SkinnedMeshRenderer))
				{
					if (this._LOG_LEVEL >= MB2_LogLevel.error)
					{
						Debug.LogError("Render Type is skinned mesh renderer but Target Renderer is not.");
					}
					return false;
				}
				if (((SkinnedMeshRenderer)this._targetRenderer).sharedMesh != this._mesh)
				{
					if (this._LOG_LEVEL >= MB2_LogLevel.error)
					{
						Debug.LogError("Target renderer mesh is not equal to mesh.");
					}
					return false;
				}
			}
			if (this._renderType == MB_RenderType.meshRenderer)
			{
				if (!(this._targetRenderer is MeshRenderer))
				{
					if (this._LOG_LEVEL >= MB2_LogLevel.error)
					{
						Debug.LogError("Render Type is mesh renderer but Target Renderer is not.");
					}
					return false;
				}
				MeshFilter component = this._targetRenderer.GetComponent<MeshFilter>();
				if (this._mesh != component.sharedMesh)
				{
					if (this._LOG_LEVEL >= MB2_LogLevel.error)
					{
						Debug.LogError("Target renderer mesh is not equal to mesh.");
					}
					return false;
				}
			}
			return true;
		}

		public static Renderer BuildSceneHierarch(MB3_MeshCombinerSingle mom, GameObject root, Mesh m, bool createNewChild = false, GameObject[] objsToBeAdded = null)
		{
			if (mom._LOG_LEVEL >= MB2_LogLevel.trace)
			{
				Debug.Log("Building Scene Hierarchy createNewChild=" + createNewChild);
			}
			MeshFilter meshFilter = null;
			MeshRenderer meshRenderer = null;
			SkinnedMeshRenderer skinnedMeshRenderer = null;
			Transform transform = null;
			if (root == null)
			{
				Debug.LogError("root was null.");
				return null;
			}
			if (mom.textureBakeResults == null)
			{
				Debug.LogError("textureBakeResults must be set.");
				return null;
			}
			if (root.GetComponent<Renderer>() != null)
			{
				Debug.LogError("root game object cannot have a renderer component");
				return null;
			}
			if (!createNewChild)
			{
				if (mom.targetRenderer != null)
				{
					transform = mom.targetRenderer.transform;
				}
				else
				{
					Renderer[] componentsInChildren = root.GetComponentsInChildren<Renderer>();
					if (componentsInChildren.Length > 1)
					{
						Debug.LogError("Result Scene Object had multiple child objects with renderers attached. Only one allowed. Try using a game object with no children as the Result Scene Object.");
						return null;
					}
					if (componentsInChildren.Length == 1)
					{
						if (componentsInChildren[0].transform.parent != root.transform)
						{
							Debug.LogError("Target Renderer is not an immediate child of Result Scene Object. Try using a game object with no children as the Result Scene Object..");
							return null;
						}
						transform = componentsInChildren[0].transform;
					}
				}
			}
			if (transform != null && transform.parent != root.transform)
			{
				transform = null;
			}
			if (transform == null)
			{
				transform = new GameObject(mom.name + "-mesh")
				{
					transform = 
					{
						parent = root.transform
					}
				}.transform;
			}
			GameObject gameObject = transform.gameObject;
			if (mom.renderType == MB_RenderType.skinnedMeshRenderer)
			{
				MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
				if (component != null)
				{
					MB_Utility.Destroy(component);
				}
				MeshFilter component2 = gameObject.GetComponent<MeshFilter>();
				if (component2 != null)
				{
					MB_Utility.Destroy(component2);
				}
				skinnedMeshRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();
				if (skinnedMeshRenderer == null)
				{
					skinnedMeshRenderer = gameObject.AddComponent<SkinnedMeshRenderer>();
				}
			}
			else
			{
				SkinnedMeshRenderer component3 = gameObject.GetComponent<SkinnedMeshRenderer>();
				if (component3 != null)
				{
					MB_Utility.Destroy(component3);
				}
				meshFilter = gameObject.GetComponent<MeshFilter>();
				if (meshFilter == null)
				{
					meshFilter = gameObject.AddComponent<MeshFilter>();
				}
				meshRenderer = gameObject.GetComponent<MeshRenderer>();
				if (meshRenderer == null)
				{
					meshRenderer = gameObject.AddComponent<MeshRenderer>();
				}
			}
			if (mom.textureBakeResults.doMultiMaterial)
			{
				Material[] array = new Material[mom.textureBakeResults.resultMaterials.Length];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = mom.textureBakeResults.resultMaterials[i].combinedMaterial;
				}
				if (mom.renderType == MB_RenderType.skinnedMeshRenderer)
				{
					skinnedMeshRenderer.sharedMaterial = null;
					skinnedMeshRenderer.sharedMaterials = array;
					skinnedMeshRenderer.bones = mom.GetBones();
					skinnedMeshRenderer.updateWhenOffscreen = true;
					skinnedMeshRenderer.updateWhenOffscreen = false;
				}
				else
				{
					meshRenderer.sharedMaterial = null;
					meshRenderer.sharedMaterials = array;
				}
			}
			else if (mom.renderType == MB_RenderType.skinnedMeshRenderer)
			{
				skinnedMeshRenderer.sharedMaterials = new Material[]
				{
					mom.textureBakeResults.resultMaterial
				};
				skinnedMeshRenderer.sharedMaterial = mom.textureBakeResults.resultMaterial;
				skinnedMeshRenderer.bones = mom.GetBones();
			}
			else
			{
				meshRenderer.sharedMaterials = new Material[]
				{
					mom.textureBakeResults.resultMaterial
				};
				meshRenderer.sharedMaterial = mom.textureBakeResults.resultMaterial;
			}
			if (mom.renderType == MB_RenderType.skinnedMeshRenderer)
			{
				skinnedMeshRenderer.sharedMesh = m;
				skinnedMeshRenderer.lightmapIndex = mom.GetLightmapIndex();
			}
			else
			{
				meshFilter.sharedMesh = m;
				meshRenderer.lightmapIndex = mom.GetLightmapIndex();
			}
			if (mom.lightmapOption == MB2_LightmapOptions.preserve_current_lightmapping || mom.lightmapOption == MB2_LightmapOptions.generate_new_UV2_layout)
			{
				gameObject.isStatic = true;
			}
			if (objsToBeAdded != null && objsToBeAdded.Length > 0 && objsToBeAdded[0] != null)
			{
				bool flag = true;
				bool flag2 = true;
				string tag = objsToBeAdded[0].tag;
				int layer = objsToBeAdded[0].layer;
				for (int j = 0; j < objsToBeAdded.Length; j++)
				{
					if (objsToBeAdded[j] != null)
					{
						if (!objsToBeAdded[j].tag.Equals(tag))
						{
							flag = false;
						}
						if (objsToBeAdded[j].layer != layer)
						{
							flag2 = false;
						}
					}
				}
				if (flag)
				{
					root.tag = tag;
					gameObject.tag = tag;
				}
				if (flag2)
				{
					root.layer = layer;
					gameObject.layer = layer;
				}
			}
			gameObject.transform.parent = root.transform;
			if (mom.renderType == MB_RenderType.skinnedMeshRenderer)
			{
				return skinnedMeshRenderer;
			}
			return meshRenderer;
		}

		public void BuildSceneMeshObject(GameObject[] gos = null, bool createNewChild = false)
		{
			if (this._resultSceneObject == null)
			{
				this._resultSceneObject = new GameObject("CombinedMesh-" + base.name);
			}
			this._targetRenderer = MB3_MeshCombinerSingle.BuildSceneHierarch(this, this._resultSceneObject, this.GetMesh(), createNewChild, gos);
		}

		private bool IsMirrored(Matrix4x4 tm)
		{
			Vector3 lhs = tm.GetRow(0);
			Vector3 rhs = tm.GetRow(1);
			Vector3 rhs2 = tm.GetRow(2);
			lhs.Normalize();
			rhs.Normalize();
			rhs2.Normalize();
			float num = Vector3.Dot(Vector3.Cross(lhs, rhs), rhs2);
			return num < 0f;
		}

		public override void CheckIntegrity()
		{
			for (int i = 0; i < this.mbDynamicObjectsInCombinedMesh.Count; i++)
			{
				MB3_MeshCombinerSingle.MB_DynamicGameObject mB_DynamicGameObject = this.mbDynamicObjectsInCombinedMesh[i];
				HashSet<int> hashSet = new HashSet<int>();
				HashSet<int> hashSet2 = new HashSet<int>();
				for (int j = mB_DynamicGameObject.vertIdx; j < mB_DynamicGameObject.vertIdx + mB_DynamicGameObject.numVerts; j++)
				{
					hashSet.Add(this.boneWeights[j].boneIndex0);
					hashSet.Add(this.boneWeights[j].boneIndex1);
					hashSet.Add(this.boneWeights[j].boneIndex2);
					hashSet.Add(this.boneWeights[j].boneIndex3);
				}
				for (int k = 0; k < mB_DynamicGameObject.indexesOfBonesUsed.Length; k++)
				{
					hashSet2.Add(mB_DynamicGameObject.indexesOfBonesUsed[k]);
				}
				hashSet2.ExceptWith(hashSet);
				if (hashSet2.Count > 0)
				{
					Debug.LogError(string.Concat(new object[]
					{
						"The bone indexes were not the same. ",
						hashSet.Count,
						" ",
						hashSet2.Count
					}));
				}
				for (int l = 0; l < mB_DynamicGameObject.indexesOfBonesUsed.Length; l++)
				{
					if (l < 0 || l > this.bones.Length)
					{
						Debug.LogError("Bone index was out of bounds.");
					}
				}
				if (this.renderType == MB_RenderType.skinnedMeshRenderer && mB_DynamicGameObject.indexesOfBonesUsed.Length < 1)
				{
					Debug.Log("DGO had no bones");
				}
			}
		}

		private List<MB3_MeshCombinerSingle.MB_DynamicGameObject>[] _buildBoneIdx2dgoMap()
		{
			List<MB3_MeshCombinerSingle.MB_DynamicGameObject>[] array = new List<MB3_MeshCombinerSingle.MB_DynamicGameObject>[this.bones.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new List<MB3_MeshCombinerSingle.MB_DynamicGameObject>();
			}
			for (int j = 0; j < this.mbDynamicObjectsInCombinedMesh.Count; j++)
			{
				MB3_MeshCombinerSingle.MB_DynamicGameObject mB_DynamicGameObject = this.mbDynamicObjectsInCombinedMesh[j];
				for (int k = 0; k < mB_DynamicGameObject.indexesOfBonesUsed.Length; k++)
				{
					array[mB_DynamicGameObject.indexesOfBonesUsed[k]].Add(mB_DynamicGameObject);
				}
			}
			return array;
		}

		private void _CollectBonesToAddForDGO(MB3_MeshCombinerSingle.MB_DynamicGameObject dgo, List<int> boneIdxsToDelete, HashSet<MB3_MeshCombinerSingle.BoneAndBindpose> bonesToAdd, Renderer r)
		{
			Matrix4x4[] array = dgo._tmpCachedBindposes = this._getBindPoses(r);
			Transform[] array2 = dgo._tmpCachedBones = this._getBones(r);
			for (int i = 0; i < array2.Length; i++)
			{
				bool flag = false;
				for (int j = 0; j < this.bones.Length; j++)
				{
					if (!boneIdxsToDelete.Contains(j))
					{
						if (array2[i] == this.bones[j])
						{
							if (array[i] == this.bindPoses[j])
							{
								flag = true;
							}
							break;
						}
					}
				}
				if (!flag)
				{
					MB3_MeshCombinerSingle.BoneAndBindpose item = new MB3_MeshCombinerSingle.BoneAndBindpose(array2[i], array[i]);
					if (!bonesToAdd.Contains(item))
					{
						bonesToAdd.Add(item);
					}
				}
			}
		}

		private void _CopyBonesWeAreKeepingToNewBonesArrayAndAdjustBWIndexes(List<int> boneIdxsToDelete, HashSet<MB3_MeshCombinerSingle.BoneAndBindpose> bonesToAdd, Transform[] nbones, Matrix4x4[] nbindPoses, BoneWeight[] nboneWeights, int totalDeleteVerts)
		{
			if (boneIdxsToDelete.Count > 0)
			{
				int[] array = new int[this.bones.Length];
				boneIdxsToDelete.Sort();
				int num = 0;
				int num2 = 0;
				for (int i = 0; i < this.bones.Length; i++)
				{
					if (num2 < boneIdxsToDelete.Count && boneIdxsToDelete[num2] == i)
					{
						num2++;
						array[i] = -1;
					}
					else
					{
						array[i] = num;
						nbones[num] = this.bones[i];
						nbindPoses[num] = this.bindPoses[i];
						num++;
					}
				}
				int num3 = this.boneWeights.Length - totalDeleteVerts;
				for (int j = 0; j < num3; j++)
				{
					nboneWeights[j].boneIndex0 = array[nboneWeights[j].boneIndex0];
					nboneWeights[j].boneIndex1 = array[nboneWeights[j].boneIndex1];
					nboneWeights[j].boneIndex2 = array[nboneWeights[j].boneIndex2];
					nboneWeights[j].boneIndex3 = array[nboneWeights[j].boneIndex3];
				}
				for (int k = 0; k < this.mbDynamicObjectsInCombinedMesh.Count; k++)
				{
					MB3_MeshCombinerSingle.MB_DynamicGameObject mB_DynamicGameObject = this.mbDynamicObjectsInCombinedMesh[k];
					for (int l = 0; l < mB_DynamicGameObject.indexesOfBonesUsed.Length; l++)
					{
						mB_DynamicGameObject.indexesOfBonesUsed[l] = array[mB_DynamicGameObject.indexesOfBonesUsed[l]];
					}
				}
			}
			else
			{
				Array.Copy(this.bones, nbones, this.bones.Length);
				Array.Copy(this.bindPoses, nbindPoses, this.bindPoses.Length);
			}
		}

		private void _AddBonesToNewBonesArrayAndAdjustBWIndexes(MB3_MeshCombinerSingle.MB_DynamicGameObject dgo, Renderer r, int vertsIdx, Transform[] nbones, BoneWeight[] nboneWeights)
		{
			Transform[] tmpCachedBones = dgo._tmpCachedBones;
			Matrix4x4[] tmpCachedBindposes = dgo._tmpCachedBindposes;
			BoneWeight[] array = this._getBoneWeights(r, dgo.numVerts);
			int[] array2 = new int[tmpCachedBones.Length];
			for (int i = 0; i < tmpCachedBones.Length; i++)
			{
				for (int j = 0; j < nbones.Length; j++)
				{
					if (tmpCachedBones[i] == nbones[j] && tmpCachedBindposes[i] == this.bindPoses[j])
					{
						array2[i] = j;
						break;
					}
				}
			}
			HashSet<int> hashSet = new HashSet<int>();
			for (int k = 0; k < array.Length; k++)
			{
				int num = vertsIdx + k;
				nboneWeights[num].boneIndex0 = array2[array[k].boneIndex0];
				nboneWeights[num].boneIndex1 = array2[array[k].boneIndex1];
				nboneWeights[num].boneIndex2 = array2[array[k].boneIndex2];
				nboneWeights[num].boneIndex3 = array2[array[k].boneIndex3];
				nboneWeights[num].weight0 = array[k].weight0;
				nboneWeights[num].weight1 = array[k].weight1;
				nboneWeights[num].weight2 = array[k].weight2;
				nboneWeights[num].weight3 = array[k].weight3;
				hashSet.Add(nboneWeights[num].boneIndex0);
				hashSet.Add(nboneWeights[num].boneIndex1);
				hashSet.Add(nboneWeights[num].boneIndex2);
				hashSet.Add(nboneWeights[num].boneIndex3);
			}
			dgo.indexesOfBonesUsed = new int[hashSet.Count];
			hashSet.CopyTo(dgo.indexesOfBonesUsed);
			dgo._tmpCachedBones = null;
			dgo._tmpCachedBindposes = null;
		}
	}
}
