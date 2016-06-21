using System;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
	[Serializable]
	public class MB3_MultiMeshCombiner : MB3_MeshCombiner
	{
		[Serializable]
		public class CombinedMesh
		{
			public MB3_MeshCombinerSingle combinedMesh;

			public int extraSpace = -1;

			public int numVertsInListToDelete;

			public int numVertsInListToAdd;

			public List<GameObject> gosToAdd;

			public List<int> gosToDelete;

			public List<GameObject> gosToUpdate;

			public bool isDirty;

			public CombinedMesh(int maxNumVertsInMesh, GameObject resultSceneObject, MB2_LogLevel ll)
			{
				this.combinedMesh = new MB3_MeshCombinerSingle();
				this.combinedMesh.resultSceneObject = resultSceneObject;
				this.combinedMesh.LOG_LEVEL = ll;
				this.extraSpace = maxNumVertsInMesh;
				this.numVertsInListToDelete = 0;
				this.numVertsInListToAdd = 0;
				this.gosToAdd = new List<GameObject>();
				this.gosToDelete = new List<int>();
				this.gosToUpdate = new List<GameObject>();
			}

			public bool isEmpty()
			{
				List<GameObject> list = new List<GameObject>();
				list.AddRange(this.combinedMesh.GetObjectsInCombined());
				for (int i = 0; i < this.gosToDelete.Count; i++)
				{
					for (int j = 0; j < list.Count; j++)
					{
						if (list[j].GetInstanceID() == this.gosToDelete[i])
						{
							list.RemoveAt(j);
							break;
						}
					}
				}
				return list.Count == 0;
			}
		}

		private static GameObject[] empty = new GameObject[0];

		private static int[] emptyIDs = new int[0];

		public Dictionary<int, MB3_MultiMeshCombiner.CombinedMesh> obj2MeshCombinerMap = new Dictionary<int, MB3_MultiMeshCombiner.CombinedMesh>();

		[SerializeField]
		public List<MB3_MultiMeshCombiner.CombinedMesh> meshCombiners = new List<MB3_MultiMeshCombiner.CombinedMesh>();

		[SerializeField]
		private int _maxVertsInMesh = 65535;

		public override MB2_LogLevel LOG_LEVEL
		{
			get
			{
				return this._LOG_LEVEL;
			}
			set
			{
				this._LOG_LEVEL = value;
				for (int i = 0; i < this.meshCombiners.Count; i++)
				{
					this.meshCombiners[i].combinedMesh.LOG_LEVEL = value;
				}
			}
		}

		public override MB2_ValidationLevel validationLevel
		{
			get
			{
				return this._validationLevel;
			}
			set
			{
				this._validationLevel = value;
				for (int i = 0; i < this.meshCombiners.Count; i++)
				{
					this.meshCombiners[i].combinedMesh.validationLevel = this._validationLevel;
				}
			}
		}

		public int maxVertsInMesh
		{
			get
			{
				return this._maxVertsInMesh;
			}
			set
			{
				if (this.obj2MeshCombinerMap.Count > 0)
				{
					return;
				}
				if (value < 3)
				{
					Debug.LogError("Max verts in mesh must be greater than three.");
				}
				else if (value > 65535)
				{
					Debug.LogError("Meshes in unity cannot have more than 65535 vertices.");
				}
				else
				{
					this._maxVertsInMesh = value;
				}
			}
		}

		public override int GetNumObjectsInCombined()
		{
			return this.obj2MeshCombinerMap.Count;
		}

		public override int GetNumVerticesFor(GameObject go)
		{
			MB3_MultiMeshCombiner.CombinedMesh combinedMesh = null;
			if (this.obj2MeshCombinerMap.TryGetValue(go.GetInstanceID(), out combinedMesh))
			{
				return combinedMesh.combinedMesh.GetNumVerticesFor(go);
			}
			return -1;
		}

		public override int GetNumVerticesFor(int gameObjectID)
		{
			MB3_MultiMeshCombiner.CombinedMesh combinedMesh = null;
			if (this.obj2MeshCombinerMap.TryGetValue(gameObjectID, out combinedMesh))
			{
				return combinedMesh.combinedMesh.GetNumVerticesFor(gameObjectID);
			}
			return -1;
		}

		public override List<GameObject> GetObjectsInCombined()
		{
			List<GameObject> list = new List<GameObject>();
			for (int i = 0; i < this.meshCombiners.Count; i++)
			{
				list.AddRange(this.meshCombiners[i].combinedMesh.GetObjectsInCombined());
			}
			return list;
		}

		public override int GetLightmapIndex()
		{
			if (this.meshCombiners.Count > 0)
			{
				return this.meshCombiners[0].combinedMesh.GetLightmapIndex();
			}
			return -1;
		}

		public override bool CombinedMeshContains(GameObject go)
		{
			return this.obj2MeshCombinerMap.ContainsKey(go.GetInstanceID());
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

		public override void Apply(MB3_MeshCombiner.GenerateUV2Delegate uv2GenerationMethod)
		{
			for (int i = 0; i < this.meshCombiners.Count; i++)
			{
				if (this.meshCombiners[i].isDirty)
				{
					this.meshCombiners[i].combinedMesh.Apply(uv2GenerationMethod);
					this.meshCombiners[i].isDirty = false;
				}
			}
		}

		public override void Apply(bool triangles, bool vertices, bool normals, bool tangents, bool uvs, bool uv2, bool uv3, bool uv4, bool colors, bool bones = false, MB3_MeshCombiner.GenerateUV2Delegate uv2GenerationMethod = null)
		{
			for (int i = 0; i < this.meshCombiners.Count; i++)
			{
				if (this.meshCombiners[i].isDirty)
				{
					this.meshCombiners[i].combinedMesh.Apply(triangles, vertices, normals, tangents, uvs, uv2, uv3, uv4, colors, bones, uv2GenerationMethod);
					this.meshCombiners[i].isDirty = false;
				}
			}
		}

		public override void UpdateSkinnedMeshApproximateBounds()
		{
			for (int i = 0; i < this.meshCombiners.Count; i++)
			{
				this.meshCombiners[i].combinedMesh.UpdateSkinnedMeshApproximateBounds();
			}
		}

		public override void UpdateSkinnedMeshApproximateBoundsFromBones()
		{
			for (int i = 0; i < this.meshCombiners.Count; i++)
			{
				this.meshCombiners[i].combinedMesh.UpdateSkinnedMeshApproximateBoundsFromBones();
			}
		}

		public override void UpdateSkinnedMeshApproximateBoundsFromBounds()
		{
			for (int i = 0; i < this.meshCombiners.Count; i++)
			{
				this.meshCombiners[i].combinedMesh.UpdateSkinnedMeshApproximateBoundsFromBounds();
			}
		}

		public override void UpdateGameObjects(GameObject[] gos, bool recalcBounds = true, bool updateVertices = true, bool updateNormals = true, bool updateTangents = true, bool updateUV = false, bool updateUV2 = false, bool updateUV3 = false, bool updateUV4 = false, bool updateColors = false, bool updateSkinningInfo = false)
		{
			if (gos == null)
			{
				Debug.LogError("list of game objects cannot be null");
				return;
			}
			for (int i = 0; i < this.meshCombiners.Count; i++)
			{
				this.meshCombiners[i].gosToUpdate.Clear();
			}
			for (int j = 0; j < gos.Length; j++)
			{
				MB3_MultiMeshCombiner.CombinedMesh combinedMesh = null;
				this.obj2MeshCombinerMap.TryGetValue(gos[j].GetInstanceID(), out combinedMesh);
				if (combinedMesh != null)
				{
					combinedMesh.gosToUpdate.Add(gos[j]);
				}
				else
				{
					Debug.LogWarning("Object " + gos[j] + " is not in the combined mesh.");
				}
			}
			for (int k = 0; k < this.meshCombiners.Count; k++)
			{
				if (this.meshCombiners[k].gosToUpdate.Count > 0)
				{
					this.meshCombiners[k].isDirty = true;
					GameObject[] gos2 = this.meshCombiners[k].gosToUpdate.ToArray();
					this.meshCombiners[k].combinedMesh.UpdateGameObjects(gos2, recalcBounds, updateVertices, updateNormals, updateTangents, updateUV, updateUV2, updateUV3, updateUV4, updateColors, updateSkinningInfo);
				}
			}
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

		public override bool AddDeleteGameObjectsByID(GameObject[] gos, int[] deleteGOinstanceIDs, bool disableRendererInSource = true)
		{
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
			if (!this._validate(gos, deleteGOinstanceIDs))
			{
				return false;
			}
			this._distributeAmongBakers(gos, deleteGOinstanceIDs);
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				MB2_Log.LogDebug(string.Concat(new object[]
				{
					"MB2_MultiMeshCombiner.AddDeleteGameObjects numCombinedMeshes: ",
					this.meshCombiners.Count,
					" added:",
					gos,
					" deleted:",
					deleteGOinstanceIDs,
					" disableRendererInSource:",
					disableRendererInSource,
					" maxVertsPerCombined:",
					this._maxVertsInMesh
				}), new object[0]);
			}
			return this._bakeStep1(gos, deleteGOinstanceIDs, disableRendererInSource);
		}

		private bool _validate(GameObject[] gos, int[] deleteGOinstanceIDs)
		{
			if (this._validationLevel == MB2_ValidationLevel.none)
			{
				return true;
			}
			if (this._maxVertsInMesh < 3)
			{
				Debug.LogError("Invalid value for maxVertsInMesh=" + this._maxVertsInMesh);
			}
			this._validateTextureBakeResults();
			if (gos != null)
			{
				for (int i = 0; i < gos.Length; i++)
				{
					if (gos[i] == null)
					{
						Debug.LogError("The " + i + "th object on the list of objects to combine is 'None'. Use Command-Delete on Mac OS X; Delete or Shift-Delete on Windows to remove this one element.");
						return false;
					}
					if (this._validationLevel >= MB2_ValidationLevel.robust)
					{
						for (int j = i + 1; j < gos.Length; j++)
						{
							if (gos[i] == gos[j])
							{
								Debug.LogError("GameObject " + gos[i] + "appears twice in list of game objects to add");
								return false;
							}
						}
						if (this.obj2MeshCombinerMap.ContainsKey(gos[i].GetInstanceID()))
						{
							bool flag = false;
							if (deleteGOinstanceIDs != null)
							{
								for (int k = 0; k < deleteGOinstanceIDs.Length; k++)
								{
									if (deleteGOinstanceIDs[k] == gos[i].GetInstanceID())
									{
										flag = true;
									}
								}
							}
							if (!flag)
							{
								Debug.LogError(string.Concat(new object[]
								{
									"GameObject ",
									gos[i],
									" is already in the combined mesh ",
									gos[i].GetInstanceID()
								}));
								return false;
							}
						}
					}
				}
			}
			if (deleteGOinstanceIDs != null && this._validationLevel >= MB2_ValidationLevel.robust)
			{
				for (int l = 0; l < deleteGOinstanceIDs.Length; l++)
				{
					for (int m = l + 1; m < deleteGOinstanceIDs.Length; m++)
					{
						if (deleteGOinstanceIDs[l] == deleteGOinstanceIDs[m])
						{
							Debug.LogError("GameObject " + deleteGOinstanceIDs[l] + "appears twice in list of game objects to delete");
							return false;
						}
					}
					if (!this.obj2MeshCombinerMap.ContainsKey(deleteGOinstanceIDs[l]))
					{
						Debug.LogWarning("GameObject with instance ID " + deleteGOinstanceIDs[l] + " on the list of objects to delete is not in the combined mesh.");
					}
				}
			}
			return true;
		}

		private void _distributeAmongBakers(GameObject[] gos, int[] deleteGOinstanceIDs)
		{
			if (gos == null)
			{
				gos = MB3_MultiMeshCombiner.empty;
			}
			if (deleteGOinstanceIDs == null)
			{
				deleteGOinstanceIDs = MB3_MultiMeshCombiner.emptyIDs;
			}
			if (this.resultSceneObject == null)
			{
				this.resultSceneObject = new GameObject("CombinedMesh-" + base.name);
			}
			for (int i = 0; i < this.meshCombiners.Count; i++)
			{
				this.meshCombiners[i].extraSpace = this._maxVertsInMesh - this.meshCombiners[i].combinedMesh.GetMesh().vertexCount;
			}
			for (int j = 0; j < deleteGOinstanceIDs.Length; j++)
			{
				MB3_MultiMeshCombiner.CombinedMesh combinedMesh = null;
				if (this.obj2MeshCombinerMap.TryGetValue(deleteGOinstanceIDs[j], out combinedMesh))
				{
					if (this.LOG_LEVEL >= MB2_LogLevel.debug)
					{
						MB2_Log.LogDebug(string.Concat(new object[]
						{
							"MB2_MultiMeshCombiner.Removing ",
							deleteGOinstanceIDs[j],
							" from meshCombiner ",
							this.meshCombiners.IndexOf(combinedMesh)
						}), new object[0]);
					}
					combinedMesh.numVertsInListToDelete += combinedMesh.combinedMesh.GetNumVerticesFor(deleteGOinstanceIDs[j]);
					combinedMesh.gosToDelete.Add(deleteGOinstanceIDs[j]);
				}
				else
				{
					Debug.LogWarning("Object " + deleteGOinstanceIDs[j] + " in the list of objects to delete is not in the combined mesh.");
				}
			}
			for (int k = 0; k < gos.Length; k++)
			{
				GameObject gameObject = gos[k];
				int vertexCount = MB_Utility.GetMesh(gameObject).vertexCount;
				MB3_MultiMeshCombiner.CombinedMesh combinedMesh2 = null;
				for (int l = 0; l < this.meshCombiners.Count; l++)
				{
					if (this.meshCombiners[l].extraSpace + this.meshCombiners[l].numVertsInListToDelete - this.meshCombiners[l].numVertsInListToAdd > vertexCount)
					{
						combinedMesh2 = this.meshCombiners[l];
						if (this.LOG_LEVEL >= MB2_LogLevel.debug)
						{
							MB2_Log.LogDebug(string.Concat(new object[]
							{
								"MB2_MultiMeshCombiner.Added ",
								gos[k],
								" to combinedMesh ",
								l
							}), new object[]
							{
								this.LOG_LEVEL
							});
						}
						break;
					}
				}
				if (combinedMesh2 == null)
				{
					combinedMesh2 = new MB3_MultiMeshCombiner.CombinedMesh(this.maxVertsInMesh, this._resultSceneObject, this._LOG_LEVEL);
					this._setMBValues(combinedMesh2.combinedMesh);
					this.meshCombiners.Add(combinedMesh2);
					if (this.LOG_LEVEL >= MB2_LogLevel.debug)
					{
						MB2_Log.LogDebug("MB2_MultiMeshCombiner.Created new combinedMesh", new object[0]);
					}
				}
				combinedMesh2.gosToAdd.Add(gameObject);
				combinedMesh2.numVertsInListToAdd += vertexCount;
			}
		}

		private bool _bakeStep1(GameObject[] gos, int[] deleteGOinstanceIDs, bool disableRendererInSource)
		{
			for (int i = 0; i < this.meshCombiners.Count; i++)
			{
				MB3_MultiMeshCombiner.CombinedMesh combinedMesh = this.meshCombiners[i];
				if (combinedMesh.combinedMesh.targetRenderer == null)
				{
					combinedMesh.combinedMesh.resultSceneObject = this._resultSceneObject;
					combinedMesh.combinedMesh.BuildSceneMeshObject(gos, true);
					if (this._LOG_LEVEL >= MB2_LogLevel.debug)
					{
						MB2_Log.LogDebug("BuildSO combiner {0} goID {1} targetRenID {2} meshID {3}", new object[]
						{
							i,
							combinedMesh.combinedMesh.targetRenderer.gameObject.GetInstanceID(),
							combinedMesh.combinedMesh.targetRenderer.GetInstanceID(),
							combinedMesh.combinedMesh.GetMesh().GetInstanceID()
						});
					}
				}
				else if (combinedMesh.combinedMesh.targetRenderer.transform.parent != this.resultSceneObject.transform)
				{
					Debug.LogError("targetRender objects must be children of resultSceneObject");
					return false;
				}
				if (combinedMesh.gosToAdd.Count > 0 || combinedMesh.gosToDelete.Count > 0)
				{
					combinedMesh.combinedMesh.AddDeleteGameObjectsByID(combinedMesh.gosToAdd.ToArray(), combinedMesh.gosToDelete.ToArray(), disableRendererInSource);
					if (this._LOG_LEVEL >= MB2_LogLevel.debug)
					{
						MB2_Log.LogDebug("Baked combiner {0} obsAdded {1} objsRemoved {2} goID {3} targetRenID {4} meshID {5}", new object[]
						{
							i,
							combinedMesh.gosToAdd.Count,
							combinedMesh.gosToDelete.Count,
							combinedMesh.combinedMesh.targetRenderer.gameObject.GetInstanceID(),
							combinedMesh.combinedMesh.targetRenderer.GetInstanceID(),
							combinedMesh.combinedMesh.GetMesh().GetInstanceID()
						});
					}
				}
				Renderer targetRenderer = combinedMesh.combinedMesh.targetRenderer;
				Mesh mesh = combinedMesh.combinedMesh.GetMesh();
				if (targetRenderer is MeshRenderer)
				{
					MeshFilter component = targetRenderer.gameObject.GetComponent<MeshFilter>();
					component.sharedMesh = mesh;
				}
				else
				{
					SkinnedMeshRenderer skinnedMeshRenderer = (SkinnedMeshRenderer)targetRenderer;
					skinnedMeshRenderer.sharedMesh = mesh;
				}
			}
			for (int j = 0; j < this.meshCombiners.Count; j++)
			{
				MB3_MultiMeshCombiner.CombinedMesh combinedMesh2 = this.meshCombiners[j];
				for (int k = 0; k < combinedMesh2.gosToDelete.Count; k++)
				{
					this.obj2MeshCombinerMap.Remove(combinedMesh2.gosToDelete[k]);
				}
			}
			for (int l = 0; l < this.meshCombiners.Count; l++)
			{
				MB3_MultiMeshCombiner.CombinedMesh combinedMesh3 = this.meshCombiners[l];
				for (int m = 0; m < combinedMesh3.gosToAdd.Count; m++)
				{
					this.obj2MeshCombinerMap.Add(combinedMesh3.gosToAdd[m].GetInstanceID(), combinedMesh3);
				}
				if (combinedMesh3.gosToAdd.Count > 0 || combinedMesh3.gosToDelete.Count > 0)
				{
					combinedMesh3.gosToDelete.Clear();
					combinedMesh3.gosToAdd.Clear();
					combinedMesh3.numVertsInListToDelete = 0;
					combinedMesh3.numVertsInListToAdd = 0;
					combinedMesh3.isDirty = true;
				}
			}
			if (this.LOG_LEVEL >= MB2_LogLevel.debug)
			{
				string text = "Meshes in combined:";
				for (int n = 0; n < this.meshCombiners.Count; n++)
				{
					string text2 = text;
					text = string.Concat(new object[]
					{
						text2,
						" mesh",
						n,
						"(",
						this.meshCombiners[n].combinedMesh.GetObjectsInCombined().Count,
						")\n"
					});
				}
				text = text + "children in result: " + this.resultSceneObject.transform.childCount;
				MB2_Log.LogDebug(text, new object[]
				{
					this.LOG_LEVEL
				});
			}
			return this.meshCombiners.Count > 0;
		}

		public override void ClearBuffers()
		{
			for (int i = 0; i < this.meshCombiners.Count; i++)
			{
				this.meshCombiners[i].combinedMesh.ClearBuffers();
			}
		}

		public override void ClearMesh()
		{
			this.DestroyMesh();
		}

		public override void DestroyMesh()
		{
			for (int i = 0; i < this.meshCombiners.Count; i++)
			{
				if (this.meshCombiners[i].combinedMesh.targetRenderer != null)
				{
					MB_Utility.Destroy(this.meshCombiners[i].combinedMesh.targetRenderer.gameObject);
				}
				this.meshCombiners[i].combinedMesh.ClearMesh();
			}
			this.obj2MeshCombinerMap.Clear();
			this.meshCombiners.Clear();
		}

		public override void DestroyMeshEditor(MB2_EditorMethodsInterface editorMethods)
		{
			for (int i = 0; i < this.meshCombiners.Count; i++)
			{
				if (this.meshCombiners[i].combinedMesh.targetRenderer != null)
				{
					editorMethods.Destroy(this.meshCombiners[i].combinedMesh.targetRenderer.gameObject);
				}
				this.meshCombiners[i].combinedMesh.ClearMesh();
			}
			this.obj2MeshCombinerMap.Clear();
			this.meshCombiners.Clear();
		}

		private void _setMBValues(MB3_MeshCombinerSingle targ)
		{
			targ.validationLevel = this._validationLevel;
			targ.renderType = this.renderType;
			targ.outputOption = MB2_OutputOptions.bakeIntoSceneObject;
			targ.lightmapOption = this.lightmapOption;
			targ.textureBakeResults = this.textureBakeResults;
			targ.doNorm = this.doNorm;
			targ.doTan = this.doTan;
			targ.doCol = this.doCol;
			targ.doUV = this.doUV;
			targ.doUV3 = this.doUV3;
			targ.doUV4 = this.doUV4;
			targ.uv2UnwrappingParamsHardAngle = this.uv2UnwrappingParamsHardAngle;
			targ.uv2UnwrappingParamsPackMargin = this.uv2UnwrappingParamsPackMargin;
		}

		public override void CheckIntegrity()
		{
			for (int i = 0; i < this.meshCombiners.Count; i++)
			{
				this.meshCombiners[i].combinedMesh.CheckIntegrity();
			}
		}
	}
}
