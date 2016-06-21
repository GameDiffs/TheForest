using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace OrbCreationExtensions
{
	public static class GameObjectExtensions
	{
		public static Bounds GetWorldBounds(this GameObject go)
		{
			if (go.transform == null)
			{
				return default(Bounds);
			}
			Bounds bounds = new Bounds(go.transform.position, Vector3.zero);
			Renderer[] componentsInChildren = go.GetComponentsInChildren<Renderer>(true);
			if (componentsInChildren != null && componentsInChildren.Length > 0)
			{
				bounds = componentsInChildren[0].bounds;
			}
			Renderer[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				Renderer renderer = array[i];
				Bounds bounds2 = renderer.bounds;
				bounds.Encapsulate(bounds2);
			}
			return bounds;
		}

		public static Vector3[] GetBoundsCorners(this Bounds bounds)
		{
			Vector3[] array = new Vector3[8];
			for (int i = 0; i < 8; i++)
			{
				array[i] = bounds.min;
				if ((i & 1) > 0)
				{
					Vector3[] expr_31_cp_0 = array;
					int expr_31_cp_1 = i;
					expr_31_cp_0[expr_31_cp_1].x = expr_31_cp_0[expr_31_cp_1].x + bounds.size.x;
				}
				if ((i & 2) > 0)
				{
					Vector3[] expr_5C_cp_0 = array;
					int expr_5C_cp_1 = i;
					expr_5C_cp_0[expr_5C_cp_1].y = expr_5C_cp_0[expr_5C_cp_1].y + bounds.size.y;
				}
				if ((i & 4) > 0)
				{
					Vector3[] expr_87_cp_0 = array;
					int expr_87_cp_1 = i;
					expr_87_cp_0[expr_87_cp_1].z = expr_87_cp_0[expr_87_cp_1].z + bounds.size.z;
				}
			}
			return array;
		}

		public static Vector3[] GetBoundsCenterAndCorners(this Bounds bounds)
		{
			Vector3[] array = new Vector3[9];
			array[0] = bounds.center;
			for (int i = 1; i < 9; i++)
			{
				array[i] = bounds.min;
				if ((i & 1) > 0)
				{
					Vector3[] expr_45_cp_0 = array;
					int expr_45_cp_1 = i;
					expr_45_cp_0[expr_45_cp_1].x = expr_45_cp_0[expr_45_cp_1].x + bounds.size.x;
				}
				if ((i & 2) > 0)
				{
					Vector3[] expr_70_cp_0 = array;
					int expr_70_cp_1 = i;
					expr_70_cp_0[expr_70_cp_1].y = expr_70_cp_0[expr_70_cp_1].y + bounds.size.y;
				}
				if ((i & 4) > 0)
				{
					Vector3[] expr_9B_cp_0 = array;
					int expr_9B_cp_1 = i;
					expr_9B_cp_0[expr_9B_cp_1].z = expr_9B_cp_0[expr_9B_cp_1].z + bounds.size.z;
				}
			}
			return array;
		}

		public static Vector3[] GetWorldBoundsCorners(this GameObject go)
		{
			return go.GetWorldBounds().GetBoundsCorners();
		}

		public static Vector3[] GetWorldBoundsCenterAndCorners(this GameObject go)
		{
			return go.GetWorldBounds().GetBoundsCenterAndCorners();
		}

		public static float GetModelComplexity(this GameObject go)
		{
			float num = 0f;
			MeshFilter[] componentsInChildren = go.GetComponentsInChildren<MeshFilter>(true);
			MeshFilter[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				MeshFilter meshFilter = array[i];
				Mesh sharedMesh = meshFilter.sharedMesh;
				float num2 = 1f;
				for (int j = 0; j < sharedMesh.subMeshCount; j++)
				{
					num += num2 * (float)sharedMesh.GetTriangles(j).Length / 3f / 65536f;
					num2 *= 1.1f;
				}
			}
			return num;
		}

		public static string GetModelInfoString(this GameObject go)
		{
			string arg = string.Empty;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			Bounds worldBounds = go.GetWorldBounds();
			MeshFilter[] componentsInChildren = go.GetComponentsInChildren<MeshFilter>(true);
			MeshFilter[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				MeshFilter meshFilter = array[i];
				Mesh sharedMesh = meshFilter.sharedMesh;
				num++;
				num2 += sharedMesh.subMeshCount;
				num3 += sharedMesh.vertices.Length;
				num4 += sharedMesh.triangles.Length / 3;
			}
			arg = arg + num + " meshes\n";
			arg = arg + num2 + " submeshes\n";
			arg = arg + num3 + " vertices\n";
			arg = arg + num4 + " triangles\n";
			return arg + worldBounds.size + " meters";
		}

		public static GameObject TopParent(this GameObject go)
		{
			Transform parent = go.transform.parent;
			if (parent == null)
			{
				return go;
			}
			return parent.gameObject.TopParent();
		}

		public static GameObject FindParentWithName(this GameObject go, string parentName)
		{
			if (go.name == parentName)
			{
				return go;
			}
			Transform parent = go.transform.parent;
			if (parent == null)
			{
				return null;
			}
			return parent.gameObject.FindParentWithName(parentName);
		}

		public static GameObject FindMutualParent(this GameObject go1, GameObject go2)
		{
			if (go2 == null || go1 == go2)
			{
				return null;
			}
			Transform transform = go2.transform;
			while (transform != null)
			{
				if (go1 == transform.gameObject)
				{
					return go1;
				}
				transform = transform.parent;
			}
			Transform parent = go1.transform.parent;
			if (parent == null)
			{
				return null;
			}
			return parent.gameObject.FindMutualParent(go2);
		}

		public static GameObject FindFirstChildWithName(this GameObject go, string childName)
		{
			if (go.name == childName)
			{
				return go;
			}
			foreach (Transform transform in go.transform)
			{
				GameObject gameObject = transform.gameObject.FindFirstChildWithName(childName);
				if (gameObject != null)
				{
					return gameObject;
				}
			}
			return null;
		}

		public static bool IsChildWithNameUnique(this GameObject go, string childName)
		{
			int num = 0;
			go.CountChildrenWithName(childName, ref num);
			return num <= 1;
		}

		public static void CountChildrenWithName(this GameObject go, string childName, ref int total)
		{
			if (go.name == childName)
			{
				total++;
			}
			foreach (Transform transform in go.transform)
			{
				transform.gameObject.CountChildrenWithName(childName, ref total);
			}
		}

		public static void DestroyChildren(this GameObject go, bool disabledOnly)
		{
			List<Transform> list = new List<Transform>();
			foreach (Transform transform in go.transform)
			{
				if (!transform.gameObject.activeSelf || !disabledOnly)
				{
					list.Add(transform);
				}
			}
			for (int i = list.Count - 1; i >= 0; i--)
			{
				list[i].SetParent(null);
				UnityEngine.Object.Destroy(list[i].gameObject);
			}
		}

		public static T GetFirstComponentInParents<T>(this GameObject go) where T : Component
		{
			T component = go.GetComponent<T>();
			if (component != null)
			{
				return component;
			}
			if (go.transform.parent != null && go.transform.parent.gameObject != go)
			{
				return go.transform.parent.gameObject.GetFirstComponentInParents<T>();
			}
			return (T)((object)null);
		}

		public static T GetFirstComponentInChildren<T>(this GameObject go) where T : Component
		{
			T[] componentsInChildren = go.GetComponentsInChildren<T>();
			if (componentsInChildren != null && componentsInChildren.Length > 0)
			{
				return componentsInChildren[0];
			}
			return (T)((object)null);
		}

		public static Mesh[] GetMeshes(this GameObject aGo)
		{
			return aGo.GetMeshes(true);
		}

		public static Mesh[] GetMeshes(this GameObject aGo, bool includeDisabled)
		{
			MeshFilter[] componentsInChildren = aGo.GetComponentsInChildren<MeshFilter>(includeDisabled);
			SkinnedMeshRenderer[] componentsInChildren2 = aGo.GetComponentsInChildren<SkinnedMeshRenderer>(includeDisabled);
			int num = 0;
			if (componentsInChildren != null)
			{
				num += componentsInChildren.Length;
			}
			if (componentsInChildren2 != null)
			{
				num += componentsInChildren2.Length;
			}
			if (num == 0)
			{
				return null;
			}
			Mesh[] array = new Mesh[num];
			int num2 = 0;
			while (componentsInChildren != null && num2 < componentsInChildren.Length)
			{
				array[num2] = componentsInChildren[num2].sharedMesh;
				num2++;
			}
			int num3 = num2;
			num2 = 0;
			while (componentsInChildren2 != null && num2 < componentsInChildren2.Length)
			{
				array[num2 + num3] = componentsInChildren2[num2].sharedMesh;
				num2++;
			}
			return array;
		}

		public static int GetTotalVertexCount(this GameObject aGo)
		{
			MeshFilter[] componentsInChildren = aGo.GetComponentsInChildren<MeshFilter>(false);
			SkinnedMeshRenderer[] componentsInChildren2 = aGo.GetComponentsInChildren<SkinnedMeshRenderer>(false);
			int num = 0;
			int num2 = 0;
			while (componentsInChildren != null && num2 < componentsInChildren.Length)
			{
				Mesh sharedMesh = componentsInChildren[num2].sharedMesh;
				if (sharedMesh != null)
				{
					num += sharedMesh.vertexCount;
				}
				num2++;
			}
			int num3 = 0;
			while (componentsInChildren2 != null && num3 < componentsInChildren2.Length)
			{
				Mesh sharedMesh2 = componentsInChildren2[num3].sharedMesh;
				if (sharedMesh2 != null)
				{
					num += sharedMesh2.vertexCount;
				}
				num3++;
			}
			return num;
		}

		public static Mesh Get1stSharedMesh(this GameObject aGo)
		{
			MeshFilter[] componentsInChildren = aGo.GetComponentsInChildren<MeshFilter>(false);
			int num = 0;
			while (componentsInChildren != null && num < componentsInChildren.Length)
			{
				if (componentsInChildren[num].sharedMesh != null)
				{
					return componentsInChildren[num].sharedMesh;
				}
				num++;
			}
			SkinnedMeshRenderer[] componentsInChildren2 = aGo.GetComponentsInChildren<SkinnedMeshRenderer>(false);
			int num2 = 0;
			while (componentsInChildren2 != null && num2 < componentsInChildren2.Length)
			{
				if (componentsInChildren2[num2].sharedMesh != null)
				{
					return componentsInChildren2[num2].sharedMesh;
				}
				num2++;
			}
			return null;
		}

		public static void SetMeshes(this GameObject aGo, Mesh[] meshes)
		{
			aGo.SetMeshes(meshes, true, 0);
		}

		public static void SetMeshes(this GameObject aGo, Mesh[] meshes, int lodLevel)
		{
			aGo.SetMeshes(meshes, true, lodLevel);
		}

		public static void SetMeshes(this GameObject aGo, Mesh[] meshes, bool includeDisabled, int lodLevel)
		{
			MeshFilter[] componentsInChildren = aGo.GetComponentsInChildren<MeshFilter>(includeDisabled);
			SkinnedMeshRenderer[] componentsInChildren2 = aGo.GetComponentsInChildren<SkinnedMeshRenderer>(includeDisabled);
			int num = 0;
			if (componentsInChildren != null)
			{
				num += componentsInChildren.Length;
			}
			if (componentsInChildren2 != null)
			{
				num += componentsInChildren2.Length;
			}
			if (num == 0)
			{
				return;
			}
			int num2 = 0;
			while (componentsInChildren != null && num2 < componentsInChildren.Length)
			{
				LODSwitcher lODSwitcher = componentsInChildren[num2].gameObject.GetComponent<LODSwitcher>();
				if (meshes != null && meshes.Length > num2)
				{
					if (lodLevel == 0)
					{
						componentsInChildren[num2].sharedMesh = meshes[num2];
					}
					if (lODSwitcher == null && lodLevel > 0)
					{
						lODSwitcher = componentsInChildren[num2].gameObject.AddComponent<LODSwitcher>();
						lODSwitcher.SetMesh(componentsInChildren[num2].sharedMesh, 0);
					}
					if (lODSwitcher != null)
					{
						lODSwitcher.SetMesh(meshes[num2], lodLevel);
					}
				}
				else
				{
					if (lODSwitcher != null)
					{
						lODSwitcher.SetMesh(null, lodLevel);
					}
					if (lodLevel == 0)
					{
						componentsInChildren[num2].sharedMesh = null;
					}
				}
				num2++;
			}
			int num3 = num2;
			num2 = 0;
			while (componentsInChildren2 != null && num2 < componentsInChildren2.Length)
			{
				LODSwitcher lODSwitcher2 = componentsInChildren2[num2].gameObject.GetComponent<LODSwitcher>();
				if (meshes != null && meshes.Length > num2 + num3)
				{
					if (lodLevel == 0)
					{
						componentsInChildren2[num2].sharedMesh = meshes[num2 + num3];
					}
					if (lODSwitcher2 == null && lodLevel > 0)
					{
						lODSwitcher2 = componentsInChildren2[num2].gameObject.AddComponent<LODSwitcher>();
						lODSwitcher2.SetMesh(componentsInChildren2[num2].sharedMesh, 0);
					}
					if (lODSwitcher2 != null)
					{
						lODSwitcher2.SetMesh(meshes[num2 + num3], lodLevel);
					}
				}
				else
				{
					if (lODSwitcher2 != null)
					{
						lODSwitcher2.SetMesh(null, lodLevel);
					}
					if (lodLevel == 0)
					{
						componentsInChildren2[num2].sharedMesh = null;
					}
				}
				num2++;
			}
		}

		public static Material[] GetMaterials(this GameObject aGo, bool includeDisabled)
		{
			List<Material> list = new List<Material>();
			MeshRenderer[] componentsInChildren = aGo.GetComponentsInChildren<MeshRenderer>(includeDisabled);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				list.AddRange(componentsInChildren[i].sharedMaterials);
			}
			SkinnedMeshRenderer[] componentsInChildren2 = aGo.GetComponentsInChildren<SkinnedMeshRenderer>(includeDisabled);
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				list.AddRange(componentsInChildren2[j].sharedMaterials);
			}
			return list.ToArray();
		}

		public static Mesh[] CombineMeshes(this GameObject aGO)
		{
			return aGO.CombineMeshes(new string[0], true);
		}

		public static Mesh[] CombineMeshes(this GameObject aGO, string[] skipSubmeshNames, bool makeNewGameObjectWhenRendererPresent = true)
		{
			List<Mesh> list = new List<Mesh>();
			MeshRenderer[] componentsInChildren = aGO.GetComponentsInChildren<MeshRenderer>(false);
			SkinnedMeshRenderer[] componentsInChildren2 = aGO.GetComponentsInChildren<SkinnedMeshRenderer>(false);
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = -999;
			bool flag = false;
			if (aGO.GetComponent<SkinnedMeshRenderer>() != null || aGO.GetComponent<MeshRenderer>() != null)
			{
				flag = makeNewGameObjectWhenRendererPresent;
			}
			if (componentsInChildren2 != null && componentsInChildren2.Length > 0)
			{
				SkinnedMeshRenderer[] array = componentsInChildren2;
				for (int i = 0; i < array.Length; i++)
				{
					SkinnedMeshRenderer skinnedMeshRenderer = array[i];
					if (skinnedMeshRenderer.sharedMesh != null)
					{
						num += skinnedMeshRenderer.sharedMesh.vertexCount;
						num2++;
						num3++;
					}
				}
			}
			if (componentsInChildren != null && componentsInChildren.Length > 0)
			{
				MeshRenderer[] array2 = componentsInChildren;
				for (int j = 0; j < array2.Length; j++)
				{
					MeshRenderer meshRenderer = array2[j];
					MeshFilter component = meshRenderer.gameObject.GetComponent<MeshFilter>();
					if (component != null && component.sharedMesh != null)
					{
						if (num4 == -999 && meshRenderer.lightmapIndex >= 0 && meshRenderer.lightmapIndex <= 253)
						{
							num4 = meshRenderer.lightmapIndex;
						}
						if (num4 < 0 || meshRenderer.lightmapIndex < 0 || meshRenderer.lightmapIndex > 253 || num4 == meshRenderer.lightmapIndex)
						{
							num += component.sharedMesh.vertexCount;
							num2++;
						}
					}
					num3++;
				}
			}
			if (num2 == 0)
			{
				throw new ApplicationException("No meshes found in children. There's nothing to combine.");
			}
			if (flag)
			{
				GameObject gameObject = new GameObject();
				string text = aGO.name + "_Merged";
				string name = text;
				int num5 = 0;
				while (GameObject.Find(name) != null)
				{
					name = text + "_" + num5;
					num5++;
				}
				gameObject.name = name;
				gameObject.transform.SetParent(aGO.transform.parent);
				gameObject.transform.localPosition = aGO.transform.localPosition;
				gameObject.transform.localRotation = aGO.transform.localRotation;
				gameObject.transform.localScale = aGO.transform.localScale;
				aGO = gameObject;
			}
			int num6 = 1;
			int num7 = -1;
			int k = 0;
			while (k < num2)
			{
				if (num7 == k)
				{
					break;
				}
				num7 = k;
				GameObject gameObject2 = aGO;
				if (num > 65534)
				{
					gameObject2 = new GameObject();
					gameObject2.name = "Merged part " + num6++;
					gameObject2.transform.SetParent(aGO.transform);
					gameObject2.transform.localPosition = Vector3.zero;
					gameObject2.transform.localRotation = Quaternion.identity;
					gameObject2.transform.localScale = Vector3.one;
				}
				Mesh mesh = null;
				List<Vector3> list2 = new List<Vector3>();
				List<Vector3> list3 = new List<Vector3>();
				List<Vector2> list4 = new List<Vector2>();
				List<Vector2> list5 = new List<Vector2>();
				List<Vector2> list6 = new List<Vector2>();
				List<Vector2> list7 = new List<Vector2>();
				List<Color32> list8 = new List<Color32>();
				List<Transform> list9 = new List<Transform>();
				List<Matrix4x4> list10 = new List<Matrix4x4>();
				List<BoneWeight> list11 = new List<BoneWeight>();
				Dictionary<Material, List<int>> dictionary = new Dictionary<Material, List<int>>();
				if (componentsInChildren2 != null && componentsInChildren2.Length > 0)
				{
					bool flag2 = false;
					bool flag3 = false;
					int num8 = 0;
					int num9 = -1;
					SkinnedMeshRenderer[] array3 = componentsInChildren2;
					for (int l = 0; l < array3.Length; l++)
					{
						SkinnedMeshRenderer skinnedMeshRenderer2 = array3[l];
						if (skinnedMeshRenderer2.sharedMesh != null)
						{
							if (list2.Count + skinnedMeshRenderer2.sharedMesh.vertexCount > 65534)
							{
								flag2 = true;
							}
							if (k <= num8 && !flag2)
							{
								if (num8 != num9)
								{
									bool flag4 = GameObjectExtensions.MergeMeshInto(skinnedMeshRenderer2.sharedMesh, skinnedMeshRenderer2.bones, skinnedMeshRenderer2.sharedMaterials, list2, list3, list4, list5, list6, list7, list8, list11, list9, list10, dictionary, skinnedMeshRenderer2.transform.localScale.x * skinnedMeshRenderer2.transform.localScale.y * skinnedMeshRenderer2.transform.localScale.z < 0f, new Vector4(1f, 1f, 0f, 0f), skinnedMeshRenderer2.transform, gameObject2.transform, skinnedMeshRenderer2.gameObject.name + "_" + skinnedMeshRenderer2.sharedMesh.name, skipSubmeshNames);
									if (flag4 && skinnedMeshRenderer2.gameObject != gameObject2)
									{
										skinnedMeshRenderer2.gameObject.SetActive(false);
									}
								}
								flag3 = true;
								k++;
							}
							num8++;
						}
					}
					if (componentsInChildren != null && componentsInChildren.Length > 0 && flag3)
					{
						MeshRenderer[] array4 = componentsInChildren;
						for (int m = 0; m < array4.Length; m++)
						{
							MeshRenderer meshRenderer2 = array4[m];
							MeshFilter component2 = meshRenderer2.gameObject.GetComponent<MeshFilter>();
							if (component2 != null && component2.sharedMesh != null && component2.gameObject != gameObject2)
							{
								if (list2.Count + component2.sharedMesh.vertexCount > 65534)
								{
									flag2 = true;
								}
								if (k <= num8 && !flag2)
								{
									bool flag5 = GameObjectExtensions.MergeMeshInto(component2.sharedMesh, null, meshRenderer2.sharedMaterials, list2, list3, list4, list5, list6, list7, list8, list11, list9, list10, dictionary, component2.transform.localScale.x * component2.transform.localScale.y * component2.transform.localScale.z < 0f, meshRenderer2.lightmapScaleOffset, component2.transform, gameObject2.transform, component2.gameObject.name + "_" + component2.sharedMesh.name, skipSubmeshNames);
									if (flag5)
									{
										meshRenderer2.enabled = false;
									}
									k++;
								}
								num8++;
							}
						}
					}
				}
				else if (componentsInChildren != null && componentsInChildren.Length > 0)
				{
					int num10 = 0;
					MeshRenderer[] array5 = componentsInChildren;
					for (int n = 0; n < array5.Length; n++)
					{
						MeshRenderer meshRenderer3 = array5[n];
						if (num4 < 0 || meshRenderer3.lightmapIndex < 0 || meshRenderer3.lightmapIndex > 253 || num4 == meshRenderer3.lightmapIndex)
						{
							MeshFilter component3 = meshRenderer3.gameObject.GetComponent<MeshFilter>();
							if (component3 != null && component3.sharedMesh != null)
							{
								if (k <= num10 && list2.Count + component3.sharedMesh.vertexCount <= 65534)
								{
									bool flag6 = GameObjectExtensions.MergeMeshInto(component3.sharedMesh, null, meshRenderer3.sharedMaterials, list2, list3, list4, list5, list6, list7, list8, list11, list9, list10, dictionary, component3.transform.localScale.x * component3.transform.localScale.y * component3.transform.localScale.z < 0f, meshRenderer3.lightmapScaleOffset, component3.transform, gameObject2.transform, component3.gameObject.name + "_" + component3.sharedMesh.name, skipSubmeshNames);
									if (flag6 && component3.gameObject != gameObject2)
									{
										component3.gameObject.SetActive(false);
										Transform parent = component3.gameObject.transform.parent;
										if (parent != null && parent.gameObject != gameObject2)
										{
											parent.gameObject.SetActive(false);
										}
									}
									k++;
								}
								num10++;
							}
						}
					}
				}
				LODMaker.RemoveUnusedVertices(list2, list3, list4, list5, list6, list7, list8, list11, dictionary);
				if (mesh == null)
				{
					mesh = new Mesh();
				}
				mesh.vertices = list2.ToArray();
				if (list3.Count > 0)
				{
					mesh.normals = list3.ToArray();
				}
				bool flag7 = false;
				for (int num11 = 0; num11 < list4.Count; num11++)
				{
					if (list4[num11].x != 0f || list4[num11].y != 0f)
					{
						flag7 = true;
						break;
					}
				}
				if (flag7)
				{
					mesh.uv = list4.ToArray();
				}
				flag7 = false;
				for (int num12 = 0; num12 < list5.Count; num12++)
				{
					if (list5[num12].x != 0f || list5[num12].y != 0f)
					{
						flag7 = true;
						break;
					}
				}
				if (flag7)
				{
					mesh.uv2 = list5.ToArray();
				}
				flag7 = false;
				for (int num13 = 0; num13 < list6.Count; num13++)
				{
					if (list6[num13].x != 0f || list6[num13].y != 0f)
					{
						flag7 = true;
						break;
					}
				}
				if (flag7)
				{
					mesh.uv3 = list6.ToArray();
				}
				flag7 = false;
				for (int num14 = 0; num14 < list7.Count; num14++)
				{
					if (list7[num14].x != 0f || list7[num14].y != 0f)
					{
						flag7 = true;
						break;
					}
				}
				if (flag7)
				{
					mesh.uv4 = list7.ToArray();
				}
				flag7 = false;
				for (int num15 = 0; num15 < list8.Count; num15++)
				{
					if (list8[num15].r > 0 || list8[num15].g > 0 || list8[num15].b > 0)
					{
						flag7 = true;
						break;
					}
				}
				if (flag7)
				{
					mesh.colors32 = list8.ToArray();
				}
				if (list10.Count > 0)
				{
					mesh.bindposes = list10.ToArray();
				}
				if (list11.Count > 0)
				{
					if (list11.Count == list2.Count)
					{
						mesh.boneWeights = list11.ToArray();
					}
					else
					{
						UnityEngine.Debug.LogWarning("Nr of bone weights not equal to nr of vertices.");
					}
				}
				mesh.subMeshCount = dictionary.Keys.Count;
				Material[] array6 = new Material[dictionary.Keys.Count];
				int num16 = 0;
				foreach (Material current in dictionary.Keys)
				{
					array6[num16] = current;
					mesh.SetTriangles(dictionary[current].ToArray(), num16++);
				}
				if (list3 == null || list3.Count <= 0)
				{
					mesh.RecalculateNormals();
				}
				mesh.RecalculateTangents();
				mesh.RecalculateBounds();
				if (componentsInChildren2 != null && componentsInChildren2.Length > 0)
				{
					SkinnedMeshRenderer skinnedMeshRenderer3 = gameObject2.GetComponent<SkinnedMeshRenderer>();
					if (skinnedMeshRenderer3 == null)
					{
						skinnedMeshRenderer3 = gameObject2.AddComponent<SkinnedMeshRenderer>();
					}
					skinnedMeshRenderer3.quality = componentsInChildren2[0].quality;
					skinnedMeshRenderer3.sharedMesh = mesh;
					skinnedMeshRenderer3.sharedMaterials = array6;
					skinnedMeshRenderer3.bones = list9.ToArray();
				}
				else if (componentsInChildren != null && componentsInChildren.Length > 0)
				{
					MeshRenderer meshRenderer4 = gameObject2.GetComponent<MeshRenderer>();
					if (meshRenderer4 == null)
					{
						meshRenderer4 = gameObject2.AddComponent<MeshRenderer>();
					}
					if (num4 >= 0 && num4 <= 253)
					{
						meshRenderer4.lightmapIndex = num4;
					}
					meshRenderer4.sharedMaterials = array6;
					MeshFilter meshFilter = gameObject2.GetComponent<MeshFilter>();
					if (meshFilter == null)
					{
						meshFilter = gameObject2.AddComponent<MeshFilter>();
					}
					meshFilter.sharedMesh = mesh;
				}
				list.Add(mesh);
			}
			return list.ToArray();
		}

		private static int GiveUniqueNameIfNeeded(GameObject aGo, GameObject topGO, int uniqueId)
		{
			if (topGO.IsChildWithNameUnique(aGo.name))
			{
				return uniqueId;
			}
			aGo.name = aGo.name + "_simpleLod" + ++uniqueId;
			return uniqueId;
		}

		public static void SetUpLODLevels(this GameObject go)
		{
			go.SetUpLODLevels(1f);
		}

		public static void SetUpLODLevels(this GameObject go, float maxWeight)
		{
			go.SetUpLODLevels(new float[]
			{
				0.6f,
				0.3f,
				0.15f
			}, new float[]
			{
				maxWeight * 0.65f,
				maxWeight,
				maxWeight * 1.5f
			});
		}

		public static void SetUpLODLevels(this GameObject go, float[] lodScreenSizes, float[] maxWeights)
		{
			MeshFilter[] componentsInChildren = go.GetComponentsInChildren<MeshFilter>(false);
			if (componentsInChildren == null || componentsInChildren.Length == 0)
			{
				return;
			}
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].gameObject.SetUpLODLevelsWithLODSwitcher(lodScreenSizes, maxWeights, true, 1f, 1f, 1f, 1f, 1f, 1);
			}
		}

		public static Mesh[] SetUpLODLevelsWithLODSwitcher(this GameObject go, float[] lodScreenSizes, float[] maxWeights, bool recalcNormals, float removeSmallParts = 1f, float protectNormals = 1f, float protectUvs = 1f, float protectSubMeshesAndSharpEdges = 1f, float smallTrianglesFirst = 1f, int nrOfSteps = 1)
		{
			Mesh mesh = null;
			LODSwitcher lODSwitcher = go.GetComponent<LODSwitcher>();
			if (lODSwitcher != null)
			{
				lODSwitcher.ReleaseFixedLODLevel();
				lODSwitcher.SetLODLevel(0);
			}
			SkinnedMeshRenderer component = go.GetComponent<SkinnedMeshRenderer>();
			if (component != null)
			{
				mesh = component.sharedMesh;
			}
			else
			{
				MeshFilter component2 = go.GetComponent<MeshFilter>();
				if (component2 != null)
				{
					mesh = component2.sharedMesh;
				}
			}
			if (mesh == null)
			{
				throw new ApplicationException("No mesh found in " + go.name + ". Maybe you need to select a child object?");
			}
			for (int i = 0; i < maxWeights.Length; i++)
			{
				if (maxWeights[i] <= 0f)
				{
					throw new ApplicationException("MaxWeight should be more that 0 or else this operation will have no effect");
				}
			}
			Mesh[] array = mesh.MakeLODMeshes(maxWeights, recalcNormals, removeSmallParts, protectNormals, protectUvs, protectSubMeshesAndSharpEdges, smallTrianglesFirst, nrOfSteps);
			if (array == null)
			{
				return null;
			}
			if (lODSwitcher == null)
			{
				lODSwitcher = go.AddComponent<LODSwitcher>();
			}
			Array.Resize<Mesh>(ref array, maxWeights.Length + 1);
			for (int j = maxWeights.Length; j > 0; j--)
			{
				array[j] = array[j - 1];
			}
			array[0] = mesh;
			lODSwitcher.lodMeshes = array;
			lODSwitcher.lodScreenSizes = lodScreenSizes;
			lODSwitcher.ComputeDimensions();
			lODSwitcher.enabled = true;
			return array;
		}

		[DebuggerHidden]
		public static IEnumerator SetUpLODLevelsWithLODSwitcherInBackground(this GameObject go, float[] lodScreenSizes, float[] maxWeights, bool recalcNormals, float removeSmallParts = 1f, float protectNormals = 1f, float protectUvs = 1f, float protectSubMeshesAndSharpEdges = 1f, float smallTrianglesFirst = 1f)
		{
			GameObjectExtensions.<SetUpLODLevelsWithLODSwitcherInBackground>c__Iterator1D2 <SetUpLODLevelsWithLODSwitcherInBackground>c__Iterator1D = new GameObjectExtensions.<SetUpLODLevelsWithLODSwitcherInBackground>c__Iterator1D2();
			<SetUpLODLevelsWithLODSwitcherInBackground>c__Iterator1D.go = go;
			<SetUpLODLevelsWithLODSwitcherInBackground>c__Iterator1D.maxWeights = maxWeights;
			<SetUpLODLevelsWithLODSwitcherInBackground>c__Iterator1D.removeSmallParts = removeSmallParts;
			<SetUpLODLevelsWithLODSwitcherInBackground>c__Iterator1D.recalcNormals = recalcNormals;
			<SetUpLODLevelsWithLODSwitcherInBackground>c__Iterator1D.lodScreenSizes = lodScreenSizes;
			<SetUpLODLevelsWithLODSwitcherInBackground>c__Iterator1D.<$>go = go;
			<SetUpLODLevelsWithLODSwitcherInBackground>c__Iterator1D.<$>maxWeights = maxWeights;
			<SetUpLODLevelsWithLODSwitcherInBackground>c__Iterator1D.<$>removeSmallParts = removeSmallParts;
			<SetUpLODLevelsWithLODSwitcherInBackground>c__Iterator1D.<$>recalcNormals = recalcNormals;
			<SetUpLODLevelsWithLODSwitcherInBackground>c__Iterator1D.<$>lodScreenSizes = lodScreenSizes;
			return <SetUpLODLevelsWithLODSwitcherInBackground>c__Iterator1D;
		}

		public static Mesh[] SetUpLODLevelsAndChildrenWithLODSwitcher(this GameObject go, float[] lodScreenSizes, float[] maxWeights, bool recalcNormals, float removeSmallParts, float protectNormals = 1f, float protectUvs = 1f, float protectSubMeshesAndSharpEdges = 1f, float smallTrianglesFirst = 1f, int nrOfSteps = 1)
		{
			Mesh mesh = null;
			LODSwitcher lODSwitcher = go.GetComponent<LODSwitcher>();
			if (lODSwitcher != null)
			{
				lODSwitcher.ReleaseFixedLODLevel();
				lODSwitcher.SetLODLevel(0);
			}
			SkinnedMeshRenderer component = go.GetComponent<SkinnedMeshRenderer>();
			Material[] sharedMaterials;
			if (component != null)
			{
				mesh = component.sharedMesh;
				sharedMaterials = component.sharedMaterials;
				component.enabled = false;
			}
			else
			{
				MeshFilter component2 = go.GetComponent<MeshFilter>();
				if (component2 != null)
				{
					mesh = component2.sharedMesh;
				}
				MeshRenderer component3 = go.GetComponent<MeshRenderer>();
				if (component3 == null)
				{
					throw new ApplicationException("No MeshRenderer found");
				}
				sharedMaterials = component3.sharedMaterials;
				component3.enabled = false;
			}
			if (mesh == null)
			{
				throw new ApplicationException("No mesh found in " + go.name + ". Maybe you need to select a child object?");
			}
			for (int i = 0; i < maxWeights.Length; i++)
			{
				if (maxWeights[i] <= 0f)
				{
					throw new ApplicationException("MaxWeight should be more that 0 or else this operation will have no effect");
				}
			}
			Mesh[] array = mesh.MakeLODMeshes(maxWeights, recalcNormals, removeSmallParts, protectNormals, protectUvs, protectSubMeshesAndSharpEdges, smallTrianglesFirst, nrOfSteps);
			if (array == null)
			{
				return null;
			}
			Mesh[] array2 = new Mesh[array.Length + 1];
			array2[0] = mesh;
			for (int j = 0; j < array.Length; j++)
			{
				array2[j + 1] = array[j];
			}
			if (lODSwitcher == null)
			{
				lODSwitcher = go.AddComponent<LODSwitcher>();
			}
			lODSwitcher.lodScreenSizes = lodScreenSizes;
			GameObject[] array3 = new GameObject[array2.Length];
			for (int k = 0; k < array2.Length; k++)
			{
				Transform transform = go.transform.FindFirstChildWithName(go.name + "_LOD" + k);
				if (transform != null)
				{
					array3[k] = transform.gameObject;
					array3[k].SetActive(true);
				}
				if (array3[k] == null)
				{
					array3[k] = new GameObject(go.name + "_LOD" + k);
					array3[k].transform.SetParent(go.transform);
					array3[k].transform.localPosition = Vector3.zero;
					array3[k].transform.localRotation = Quaternion.identity;
					array3[k].transform.localScale = Vector3.one;
				}
				if (component != null)
				{
					SkinnedMeshRenderer skinnedMeshRenderer = array3[k].GetComponent<SkinnedMeshRenderer>();
					if (skinnedMeshRenderer == null)
					{
						skinnedMeshRenderer = array3[k].AddComponent<SkinnedMeshRenderer>();
					}
					skinnedMeshRenderer.sharedMesh = array2[k];
					skinnedMeshRenderer.sharedMaterials = sharedMaterials;
				}
				else
				{
					MeshFilter meshFilter = array3[k].GetComponent<MeshFilter>();
					if (meshFilter == null)
					{
						meshFilter = array3[k].AddComponent<MeshFilter>();
					}
					meshFilter.sharedMesh = array2[k];
					MeshRenderer meshRenderer = array3[k].GetComponent<MeshRenderer>();
					if (meshRenderer == null)
					{
						meshRenderer = array3[k].AddComponent<MeshRenderer>();
					}
					meshRenderer.sharedMaterials = sharedMaterials;
				}
				array3[k].SetActive(k == 0);
			}
			lODSwitcher.lodGameObjects = array3;
			lODSwitcher.ComputeDimensions();
			lODSwitcher.enabled = true;
			return array2;
		}

		public static Mesh[] SetUpLODLevelsAndChildrenWithLODGroup(this GameObject go, float[] relativeTransitionHeights, float[] maxWeights, bool recalcNormals, float removeSmallParts, float protectNormals = 1f, float protectUvs = 1f, float protectSubMeshesAndSharpEdges = 1f, float smallTrianglesFirst = 1f, int nrOfSteps = 1)
		{
			LODGroup lODGroup = null;
			if (relativeTransitionHeights.Length < 0 || relativeTransitionHeights.Length != maxWeights.Length)
			{
				throw new ApplicationException("relativeTransitionHeights and maxWeights arrays need to have equal length and be longer than 0. Example: SetUpLODLevelsWithLODGroup(go, new float[2] {0.6f, 0.4f}, new float[2] {1f, 1.75f})");
			}
			for (int i = 0; i < maxWeights.Length; i++)
			{
				if (maxWeights[i] <= 0f)
				{
					throw new ApplicationException("MaxWeight should be more that 0 or else this operation will have no effect");
				}
			}
			GameObject gameObject = new GameObject(go.name + "_$LodGrp");
			gameObject.transform.position = go.transform.position;
			gameObject.transform.rotation = go.transform.rotation;
			gameObject.transform.localScale = go.transform.localScale;
			go.transform.SetParent(gameObject.transform);
			if (lODGroup == null)
			{
				lODGroup = gameObject.AddComponent<LODGroup>();
			}
			else
			{
				Transform transform = gameObject.transform.FindFirstChildWhereNameContains(go.name + "_$Lod:");
				int num = 0;
				while (transform != null && num++ < 10)
				{
					transform.SetParent(null);
					UnityEngine.Object.Destroy(transform.gameObject);
					transform = gameObject.transform.FindFirstChildWhereNameContains(go.name + "_$Lod:");
				}
			}
			LOD[] array = new LOD[maxWeights.Length + 1];
			array[0] = new LOD(relativeTransitionHeights[0], go.GetComponentsInChildren<MeshRenderer>(false));
			List<Mesh> list = new List<Mesh>();
			Mesh[] array2 = go.GetMeshes(false);
			float num2 = 0f;
			for (int j = 1; j < array.Length; j++)
			{
				Mesh[] array3 = new Mesh[array2.Length];
				for (int k = 0; k < array2.Length; k++)
				{
					Mesh mesh = array2[k];
					if (nrOfSteps < 1)
					{
						nrOfSteps = 1;
					}
					for (int l = 0; l < nrOfSteps; l++)
					{
						float num3 = maxWeights[j - 1] - num2;
						mesh = mesh.MakeLODMesh((float)(l + 1) * (num3 / (float)nrOfSteps) + num2, recalcNormals, removeSmallParts, protectNormals, protectUvs, protectSubMeshesAndSharpEdges, smallTrianglesFirst);
					}
					num2 = maxWeights[j - 1];
					array3[k] = mesh;
					mesh.name = string.Concat(new object[]
					{
						go.name,
						"_",
						k,
						"_LOD",
						j
					});
					list.Add(mesh);
				}
				GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(go);
				gameObject2.name = go.name + "_$Lod:" + j;
				gameObject2.transform.SetParent(gameObject.transform);
				gameObject2.transform.localPosition = go.transform.localPosition;
				gameObject2.transform.localRotation = go.transform.localRotation;
				gameObject2.transform.localScale = go.transform.localScale;
				gameObject2.SetMeshes(array3);
				float screenRelativeTransitionHeight = (j >= relativeTransitionHeights.Length) ? 0f : relativeTransitionHeights[j];
				array[j] = new LOD(screenRelativeTransitionHeight, gameObject2.GetComponentsInChildren<MeshRenderer>(false));
				array2 = array3;
			}
			lODGroup.SetLODs(array);
			lODGroup.RecalculateBounds();
			lODGroup.ForceLOD(-1);
			return list.ToArray();
		}

		public static Mesh GetSimplifiedMesh(this GameObject go, float maxWeight, bool recalcNormals, float removeSmallParts, float protectNormals = 1f, float protectUvs = 1f, float protectSubMeshesAndSharpEdges = 1f, float smallTrianglesFirst = 1f, int nrOfSteps = 1)
		{
			Mesh mesh = null;
			LODSwitcher component = go.GetComponent<LODSwitcher>();
			if (component != null)
			{
				component.ReleaseFixedLODLevel();
				component.SetLODLevel(0);
			}
			MeshFilter meshFilter = null;
			SkinnedMeshRenderer component2 = go.GetComponent<SkinnedMeshRenderer>();
			if (maxWeight <= 0f)
			{
				throw new ApplicationException("MaxWeight should be more that 0 or else this operation will have no effect");
			}
			if (component2 != null)
			{
				mesh = component2.sharedMesh;
			}
			else
			{
				meshFilter = go.GetComponent<MeshFilter>();
				if (meshFilter != null)
				{
					mesh = meshFilter.sharedMesh;
				}
			}
			if (mesh == null)
			{
				throw new ApplicationException("No mesh found. Maybe you need to select a child object?");
			}
			Mesh mesh2 = mesh;
			if (nrOfSteps < 1)
			{
				nrOfSteps = 1;
			}
			for (int i = 0; i < nrOfSteps; i++)
			{
				mesh2 = mesh2.MakeLODMesh((float)(i + 1) * (maxWeight / (float)nrOfSteps), recalcNormals, removeSmallParts, protectNormals, protectUvs, protectSubMeshesAndSharpEdges, smallTrianglesFirst);
			}
			if (component2 != null)
			{
				component2.sharedMesh = mesh2;
			}
			else if (meshFilter != null)
			{
				meshFilter.sharedMesh = mesh2;
			}
			return mesh2;
		}

		[DebuggerHidden]
		public static IEnumerator GetSimplifiedMeshInBackground(this GameObject go, float maxWeight, bool recalcNormals, float removeSmallParts, Action<Mesh> result)
		{
			GameObjectExtensions.<GetSimplifiedMeshInBackground>c__Iterator1D3 <GetSimplifiedMeshInBackground>c__Iterator1D = new GameObjectExtensions.<GetSimplifiedMeshInBackground>c__Iterator1D3();
			<GetSimplifiedMeshInBackground>c__Iterator1D.go = go;
			<GetSimplifiedMeshInBackground>c__Iterator1D.maxWeight = maxWeight;
			<GetSimplifiedMeshInBackground>c__Iterator1D.removeSmallParts = removeSmallParts;
			<GetSimplifiedMeshInBackground>c__Iterator1D.result = result;
			<GetSimplifiedMeshInBackground>c__Iterator1D.recalcNormals = recalcNormals;
			<GetSimplifiedMeshInBackground>c__Iterator1D.<$>go = go;
			<GetSimplifiedMeshInBackground>c__Iterator1D.<$>maxWeight = maxWeight;
			<GetSimplifiedMeshInBackground>c__Iterator1D.<$>removeSmallParts = removeSmallParts;
			<GetSimplifiedMeshInBackground>c__Iterator1D.<$>result = result;
			<GetSimplifiedMeshInBackground>c__Iterator1D.<$>recalcNormals = recalcNormals;
			return <GetSimplifiedMeshInBackground>c__Iterator1D;
		}

		private static bool MergeMeshInto(Mesh fromMesh, Transform[] fromBones, Material[] fromMaterials, List<Vector3> vertices, List<Vector3> normals, List<Vector2> uv1s, List<Vector2> uv2s, List<Vector2> uv3s, List<Vector2> uv4s, List<Color32> colors32, List<BoneWeight> boneWeights, List<Transform> bones, List<Matrix4x4> bindposes, Dictionary<Material, List<int>> subMeshes, bool usesNegativeScale, Vector4 lightmapScaleOffset, Transform fromTransform, Transform topTransform, string submeshName, string[] skipSubmeshNames)
		{
			if (fromMesh == null)
			{
				return false;
			}
			bool result = true;
			int count = vertices.Count;
			Vector3[] vertices2 = fromMesh.vertices;
			Vector3[] normals2 = fromMesh.normals;
			Vector2[] array = fromMesh.uv;
			Vector2[] array2 = fromMesh.uv2;
			Vector2[] array3 = fromMesh.uv3;
			Vector2[] array4 = fromMesh.uv4;
			Color32[] array5 = fromMesh.colors32;
			BoneWeight[] array6 = fromMesh.boneWeights;
			Matrix4x4[] array7 = fromMesh.bindposes;
			List<int> list = new List<int>();
			Vector3 localPosition = fromTransform.localPosition;
			Quaternion localRotation = fromTransform.localRotation;
			Vector3 localScale = fromTransform.localScale;
			bool flag = false;
			if (fromBones != null)
			{
				fromTransform.localPosition = Vector3.zero;
				fromTransform.localRotation = Quaternion.identity;
				fromTransform.localScale = Vector3.one;
			}
			if (fromBones == null || fromBones.Length == 0)
			{
				flag = true;
			}
			if ((fromBones == null || fromBones.Length == 0) && bones != null && bones.Count > 0)
			{
				fromBones = new Transform[]
				{
					fromTransform
				};
				Matrix4x4 matrix4x = fromTransform.worldToLocalMatrix * topTransform.localToWorldMatrix;
				array7 = new Matrix4x4[]
				{
					matrix4x
				};
				array6 = new BoneWeight[vertices2.Length];
				for (int i = 0; i < vertices2.Length; i++)
				{
					array6[i].boneIndex0 = 0;
					array6[i].weight0 = 1f;
				}
			}
			if (fromBones != null)
			{
				bool flag2 = false;
				for (int j = 0; j < fromBones.Length; j++)
				{
					int k = 0;
					list.Add(j);
					while (k < bones.Count)
					{
						if (fromBones[j] == bones[k])
						{
							list[j] = k;
							if (array7[j] != bindposes[k])
							{
								flag2 = true;
								if (fromBones[j] != null)
								{
									UnityEngine.Debug.Log(fromTransform.gameObject.name + ": The bindpose of " + fromBones[j].gameObject.name + " is different, vertices will be moved to match the bindpose of the merged mesh");
								}
								else
								{
									UnityEngine.Debug.LogError(fromTransform.gameObject.name + ": There is an error in the bonestructure. A bone could not be found.");
								}
							}
						}
						k++;
					}
					if (k >= bones.Count)
					{
						list[j] = bones.Count;
						bones.Add(fromBones[j]);
						bindposes.Add(array7[j]);
					}
				}
				if (flag2)
				{
					for (int l = 0; l < vertices2.Length; l++)
					{
						Vector3 vector = vertices2[l];
						BoneWeight boneWeight = array6[l];
						if (fromBones[boneWeight.boneIndex0] != null)
						{
							vector = GameObjectExtensions.ApplyBindPose(vertices2[l], fromBones[boneWeight.boneIndex0], array7[boneWeight.boneIndex0], boneWeight.weight0);
							if (boneWeight.weight1 > 0f)
							{
								vector += GameObjectExtensions.ApplyBindPose(vertices2[l], fromBones[boneWeight.boneIndex1], array7[boneWeight.boneIndex1], boneWeight.weight1);
							}
							if (boneWeight.weight2 > 0f)
							{
								vector += GameObjectExtensions.ApplyBindPose(vertices2[l], fromBones[boneWeight.boneIndex2], array7[boneWeight.boneIndex2], boneWeight.weight2);
							}
							if (boneWeight.weight3 > 0f)
							{
								vector += GameObjectExtensions.ApplyBindPose(vertices2[l], fromBones[boneWeight.boneIndex3], array7[boneWeight.boneIndex3], boneWeight.weight3);
							}
							Vector3 vertex = vector;
							vector = GameObjectExtensions.UnApplyBindPose(vertex, bones[list[boneWeight.boneIndex0]], bindposes[list[boneWeight.boneIndex0]], boneWeight.weight0);
							if (boneWeight.weight1 > 0f)
							{
								vector += GameObjectExtensions.UnApplyBindPose(vertex, bones[list[boneWeight.boneIndex1]], bindposes[list[boneWeight.boneIndex1]], boneWeight.weight1);
							}
							if (boneWeight.weight2 > 0f)
							{
								vector += GameObjectExtensions.UnApplyBindPose(vertex, bones[list[boneWeight.boneIndex2]], bindposes[list[boneWeight.boneIndex2]], boneWeight.weight2);
							}
							if (boneWeight.weight3 > 0f)
							{
								vector += GameObjectExtensions.UnApplyBindPose(vertex, bones[list[boneWeight.boneIndex3]], bindposes[list[boneWeight.boneIndex3]], boneWeight.weight3);
							}
							vertices2[l] = vector;
						}
					}
				}
			}
			if (boneWeights != null && array6 != null && array6.Length > 0)
			{
				for (int m = 0; m < array6.Length; m++)
				{
					boneWeights.Add(new BoneWeight
					{
						boneIndex0 = list[array6[m].boneIndex0],
						boneIndex1 = list[array6[m].boneIndex1],
						boneIndex2 = list[array6[m].boneIndex2],
						boneIndex3 = list[array6[m].boneIndex3],
						weight0 = array6[m].weight0,
						weight1 = array6[m].weight1,
						weight2 = array6[m].weight2,
						weight3 = array6[m].weight3
					});
				}
			}
			Matrix4x4 matrix4x2 = topTransform.worldToLocalMatrix * fromTransform.localToWorldMatrix;
			if (flag)
			{
				for (int n = 0; n < vertices2.Length; n++)
				{
					Vector3 v = vertices2[n];
					vertices2[n] = matrix4x2.MultiplyPoint3x4(v);
				}
			}
			vertices.AddRange(vertices2);
			Quaternion rotation = Quaternion.LookRotation(matrix4x2.GetColumn(2), matrix4x2.GetColumn(1));
			if (normals2 != null && normals2.Length > 0)
			{
				for (int num = 0; num < normals2.Length; num++)
				{
					normals2[num] = rotation * normals2[num];
				}
				normals.AddRange(normals2);
			}
			if (array == null || array.Length != vertices2.Length)
			{
				array = new Vector2[vertices2.Length];
			}
			if (array != null && array.Length > 0)
			{
				uv1s.AddRange(array);
			}
			if (array2 == null || array2.Length != vertices2.Length)
			{
				array2 = new Vector2[vertices2.Length];
			}
			int num2 = 0;
			while (array2 != null && num2 < array2.Length)
			{
				uv2s.Add(new Vector2(lightmapScaleOffset.z + array2[num2].x * lightmapScaleOffset.x, lightmapScaleOffset.w + array2[num2].y * lightmapScaleOffset.y));
				num2++;
			}
			if (array3 == null || array3.Length != vertices2.Length)
			{
				array3 = new Vector2[vertices2.Length];
			}
			if (array3 != null && array3.Length > 0)
			{
				uv3s.AddRange(array3);
			}
			if (array4 == null || array4.Length != vertices2.Length)
			{
				array4 = new Vector2[vertices2.Length];
			}
			if (array4 != null && array4.Length > 0)
			{
				uv4s.AddRange(array3);
			}
			if (array5 == null || array5.Length != vertices2.Length)
			{
				array5 = new Color32[vertices2.Length];
			}
			if (array5 != null && array5.Length > 0)
			{
				colors32.AddRange(array5);
			}
			int num3 = 0;
			for (int num4 = 0; num4 < fromMaterials.Length; num4++)
			{
				if (num4 < fromMesh.subMeshCount)
				{
					string a = submeshName + "_" + num4;
					int num5;
					for (num5 = 0; num5 < skipSubmeshNames.Length; num5++)
					{
						if (a == skipSubmeshNames[num5])
						{
							break;
						}
					}
					if (num5 >= skipSubmeshNames.Length)
					{
						int[] triangles = fromMesh.GetTriangles(num4);
						if (triangles.Length > 0)
						{
							if (fromMaterials[num4] != null && !subMeshes.ContainsKey(fromMaterials[num4]))
							{
								subMeshes.Add(fromMaterials[num4], new List<int>());
							}
							List<int> list2 = subMeshes[fromMaterials[num4]];
							for (int num6 = 0; num6 < triangles.Length; num6 += 3)
							{
								if (usesNegativeScale)
								{
									int num7 = triangles[num6 + 1];
									int num8 = triangles[num6 + 2];
									triangles[num6 + 1] = num8;
									triangles[num6 + 2] = num7;
									num3++;
								}
								triangles[num6] += count;
								triangles[num6 + 1] += count;
								triangles[num6 + 2] += count;
							}
							list2.AddRange(triangles);
						}
					}
					else
					{
						result = false;
					}
				}
			}
			if (fromBones != null)
			{
				fromTransform.localPosition = localPosition;
				fromTransform.localRotation = localRotation;
				fromTransform.localScale = localScale;
			}
			return result;
		}

		private static Vector3 ApplyBindPose(Vector3 vertex, Transform bone, Matrix4x4 bindpose, float boneWeight)
		{
			Matrix4x4 matrix4x = bone.localToWorldMatrix * bindpose;
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					int row;
					int expr_20 = row = i;
					int column;
					int expr_24 = column = j;
					float num = matrix4x[row, column];
					matrix4x[expr_20, expr_24] = num * boneWeight;
				}
			}
			return matrix4x.MultiplyPoint3x4(vertex);
		}

		private static Vector3 UnApplyBindPose(Vector3 vertex, Transform bone, Matrix4x4 bindpose, float boneWeight)
		{
			Matrix4x4 inverse = (bone.localToWorldMatrix * bindpose).inverse;
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					int row;
					int expr_29 = row = i;
					int column;
					int expr_2D = column = j;
					float num = inverse[row, column];
					inverse[expr_29, expr_2D] = num * boneWeight;
				}
			}
			return inverse.MultiplyPoint3x4(vertex);
		}
	}
}
