using System;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
	[Serializable]
	public abstract class MB3_MeshCombiner
	{
		public delegate void GenerateUV2Delegate(Mesh m, float hardAngle, float packMargin);

		protected MBVersion _MBVersion;

		[SerializeField]
		protected MB2_LogLevel _LOG_LEVEL = MB2_LogLevel.info;

		[SerializeField]
		protected MB2_ValidationLevel _validationLevel = MB2_ValidationLevel.robust;

		[SerializeField]
		protected string _name;

		[SerializeField]
		protected MB2_TextureBakeResults _textureBakeResults;

		[SerializeField]
		protected GameObject _resultSceneObject;

		[SerializeField]
		protected Renderer _targetRenderer;

		[SerializeField]
		protected MB_RenderType _renderType;

		[SerializeField]
		protected MB2_OutputOptions _outputOption;

		[SerializeField]
		protected MB2_LightmapOptions _lightmapOption = MB2_LightmapOptions.ignore_UV2;

		[SerializeField]
		protected bool _doNorm = true;

		[SerializeField]
		protected bool _doTan = true;

		[SerializeField]
		protected bool _doCol;

		[SerializeField]
		protected bool _doUV = true;

		[SerializeField]
		protected bool _doUV3;

		[SerializeField]
		protected bool _doUV4;

		[SerializeField]
		public float uv2UnwrappingParamsHardAngle = 60f;

		[SerializeField]
		public float uv2UnwrappingParamsPackMargin = 0.005f;

		protected bool _usingTemporaryTextureBakeResult;

		public static bool EVAL_VERSION
		{
			get
			{
				return false;
			}
		}

		public virtual MB2_LogLevel LOG_LEVEL
		{
			get
			{
				return this._LOG_LEVEL;
			}
			set
			{
				this._LOG_LEVEL = value;
			}
		}

		public virtual MB2_ValidationLevel validationLevel
		{
			get
			{
				return this._validationLevel;
			}
			set
			{
				this._validationLevel = value;
			}
		}

		public string name
		{
			get
			{
				return this._name;
			}
			set
			{
				this._name = value;
			}
		}

		public virtual MB2_TextureBakeResults textureBakeResults
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

		public virtual GameObject resultSceneObject
		{
			get
			{
				return this._resultSceneObject;
			}
			set
			{
				this._resultSceneObject = value;
			}
		}

		public virtual Renderer targetRenderer
		{
			get
			{
				return this._targetRenderer;
			}
			set
			{
				if (this._targetRenderer != null && this._targetRenderer != value)
				{
					Debug.LogWarning("Previous targetRenderer was not null. Combined mesh may be being used by more than one Renderer");
				}
				this._targetRenderer = value;
			}
		}

		public virtual MB_RenderType renderType
		{
			get
			{
				return this._renderType;
			}
			set
			{
				this._renderType = value;
			}
		}

		public virtual MB2_OutputOptions outputOption
		{
			get
			{
				return this._outputOption;
			}
			set
			{
				this._outputOption = value;
			}
		}

		public virtual MB2_LightmapOptions lightmapOption
		{
			get
			{
				return this._lightmapOption;
			}
			set
			{
				this._lightmapOption = value;
			}
		}

		public virtual bool doNorm
		{
			get
			{
				return this._doNorm;
			}
			set
			{
				this._doNorm = value;
			}
		}

		public virtual bool doTan
		{
			get
			{
				return this._doTan;
			}
			set
			{
				this._doTan = value;
			}
		}

		public virtual bool doCol
		{
			get
			{
				return this._doCol;
			}
			set
			{
				this._doCol = value;
			}
		}

		public virtual bool doUV
		{
			get
			{
				return this._doUV;
			}
			set
			{
				this._doUV = value;
			}
		}

		public virtual bool doUV1
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public virtual bool doUV3
		{
			get
			{
				return this._doUV3;
			}
			set
			{
				this._doUV3 = value;
			}
		}

		public virtual bool doUV4
		{
			get
			{
				return this._doUV4;
			}
			set
			{
				this._doUV4 = value;
			}
		}

		public virtual bool doUV2()
		{
			return this._lightmapOption == MB2_LightmapOptions.copy_UV2_unchanged || this._lightmapOption == MB2_LightmapOptions.preserve_current_lightmapping;
		}

		public abstract int GetLightmapIndex();

		public abstract void ClearBuffers();

		public abstract void ClearMesh();

		public abstract void DestroyMesh();

		public abstract void DestroyMeshEditor(MB2_EditorMethodsInterface editorMethods);

		public abstract List<GameObject> GetObjectsInCombined();

		public abstract int GetNumObjectsInCombined();

		public abstract int GetNumVerticesFor(GameObject go);

		public abstract int GetNumVerticesFor(int instanceID);

		public virtual void Apply()
		{
			this.Apply(null);
		}

		public abstract void Apply(MB3_MeshCombiner.GenerateUV2Delegate uv2GenerationMethod);

		public abstract void Apply(bool triangles, bool vertices, bool normals, bool tangents, bool uvs, bool uv2, bool uv3, bool uv4, bool colors, bool bones = false, MB3_MeshCombiner.GenerateUV2Delegate uv2GenerationMethod = null);

		public abstract void UpdateGameObjects(GameObject[] gos, bool recalcBounds = true, bool updateVertices = true, bool updateNormals = true, bool updateTangents = true, bool updateUV = false, bool updateUV2 = false, bool updateUV3 = false, bool updateUV4 = false, bool updateColors = false, bool updateSkinningInfo = false);

		public abstract bool AddDeleteGameObjects(GameObject[] gos, GameObject[] deleteGOs, bool disableRendererInSource = true);

		public abstract bool AddDeleteGameObjectsByID(GameObject[] gos, int[] deleteGOinstanceIDs, bool disableRendererInSource);

		public abstract bool CombinedMeshContains(GameObject go);

		public abstract void UpdateSkinnedMeshApproximateBounds();

		public abstract void UpdateSkinnedMeshApproximateBoundsFromBones();

		public abstract void CheckIntegrity();

		public abstract void UpdateSkinnedMeshApproximateBoundsFromBounds();

		public static void UpdateSkinnedMeshApproximateBoundsFromBonesStatic(Transform[] bs, SkinnedMeshRenderer smr)
		{
			Vector3 position = bs[0].position;
			Vector3 position2 = bs[0].position;
			for (int i = 1; i < bs.Length; i++)
			{
				Vector3 position3 = bs[i].position;
				if (position3.x < position2.x)
				{
					position2.x = position3.x;
				}
				if (position3.y < position2.y)
				{
					position2.y = position3.y;
				}
				if (position3.z < position2.z)
				{
					position2.z = position3.z;
				}
				if (position3.x > position.x)
				{
					position.x = position3.x;
				}
				if (position3.y > position.y)
				{
					position.y = position3.y;
				}
				if (position3.z > position.z)
				{
					position.z = position3.z;
				}
			}
			Vector3 v = (position + position2) / 2f;
			Vector3 v2 = position - position2;
			Matrix4x4 worldToLocalMatrix = smr.worldToLocalMatrix;
			Bounds localBounds = new Bounds(worldToLocalMatrix * v, worldToLocalMatrix * v2);
			smr.localBounds = localBounds;
		}

		public static void UpdateSkinnedMeshApproximateBoundsFromBoundsStatic(List<GameObject> objectsInCombined, SkinnedMeshRenderer smr)
		{
			Bounds bounds = default(Bounds);
			Bounds localBounds = default(Bounds);
			if (MB_Utility.GetBounds(objectsInCombined[0], out bounds))
			{
				localBounds = bounds;
				for (int i = 1; i < objectsInCombined.Count; i++)
				{
					if (!MB_Utility.GetBounds(objectsInCombined[i], out bounds))
					{
						Debug.LogError("Could not get bounds. Not updating skinned mesh bounds");
						return;
					}
					localBounds.Encapsulate(bounds);
				}
				smr.localBounds = localBounds;
				return;
			}
			Debug.LogError("Could not get bounds. Not updating skinned mesh bounds");
		}

		protected virtual bool _CheckIfAllObjsToAddUseSameMaterialsAndCreateTemporaryTextrueBakeResult(GameObject[] gos)
		{
			this._usingTemporaryTextureBakeResult = false;
			Renderer renderer = MB_Utility.GetRenderer(gos[0]);
			if (renderer != null)
			{
				Material[] gOMaterials = MB_Utility.GetGOMaterials(gos[0]);
				for (int i = 0; i < gos.Length; i++)
				{
					if (gos[i] == null)
					{
						Debug.LogError(string.Format("Game object {0} in list of objects to add was null", i));
						return false;
					}
					Material[] gOMaterials2 = MB_Utility.GetGOMaterials(gos[i]);
					if (gOMaterials2 == null)
					{
						Debug.LogError(string.Format("Game object {0} in list of objects to add no renderer", i));
						return false;
					}
					for (int j = 0; j < gOMaterials2.Length; j++)
					{
						bool flag = false;
						for (int k = 0; k < gOMaterials.Length; k++)
						{
							if (gOMaterials2[j] == gOMaterials[k])
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							Debug.LogError(string.Format("Material Bake Result is null and game object {0} in list of objects to add did not have a subset of the materials in on the first object. You need to bake textures or all objects must have a subset of materials on the first object.", i));
							return false;
						}
					}
				}
				this._usingTemporaryTextureBakeResult = true;
				this._textureBakeResults = MB2_TextureBakeResults.CreateForMaterialsOnRenderer(renderer);
				return true;
			}
			return false;
		}
	}
}
