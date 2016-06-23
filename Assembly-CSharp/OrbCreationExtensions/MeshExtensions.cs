using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace OrbCreationExtensions
{
	public static class MeshExtensions
	{
		public static void RecalculateTangents(this Mesh mesh)
		{
			int vertexCount = mesh.vertexCount;
			Vector3[] vertices = mesh.vertices;
			Vector3[] normals = mesh.normals;
			Vector2[] uv = mesh.uv;
			int[] triangles = mesh.triangles;
			int num = triangles.Length / 3;
			Vector4[] array = new Vector4[vertexCount];
			Vector3[] array2 = new Vector3[vertexCount];
			Vector3[] array3 = new Vector3[vertexCount];
			int num2 = 0;
			if (uv.Length <= 0)
			{
				return;
			}
			for (int i = 0; i < num; i++)
			{
				int num3 = triangles[num2];
				int num4 = triangles[num2 + 1];
				int num5 = triangles[num2 + 2];
				Vector3 vector = vertices[num3];
				Vector3 vector2 = vertices[num4];
				Vector3 vector3 = vertices[num5];
				Vector2 vector4 = uv[num3];
				Vector2 vector5 = uv[num4];
				Vector2 vector6 = uv[num5];
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
				float num17 = (num16 != 0f) ? (1f / num16) : 0f;
				Vector3 b = new Vector3((num15 * num6 - num14 * num7) * num17, (num15 * num8 - num14 * num9) * num17, (num15 * num10 - num14 * num11) * num17);
				Vector3 b2 = new Vector3((num12 * num7 - num13 * num6) * num17, (num12 * num9 - num13 * num8) * num17, (num12 * num11 - num13 * num10) * num17);
				array2[num3] += b;
				array2[num4] += b;
				array2[num5] += b;
				array3[num3] += b2;
				array3[num4] += b2;
				array3[num5] += b2;
				num2 += 3;
			}
			for (int j = 0; j < vertexCount; j++)
			{
				Vector3 lhs = normals[j];
				Vector3 rhs = array2[j];
				Vector3.OrthoNormalize(ref lhs, ref rhs);
				array[j].x = rhs.x;
				array[j].y = rhs.y;
				array[j].z = rhs.z;
				array[j].w = ((Vector3.Dot(Vector3.Cross(lhs, rhs), array3[j]) >= 0f) ? 1f : -1f);
			}
			mesh.tangents = array;
		}

		public static Mesh ScaledRotatedTranslatedMesh(this Mesh mesh, Vector3 scale, Quaternion rotate, Vector3 translate)
		{
			Mesh mesh2 = UnityEngine.Object.Instantiate<Mesh>(mesh);
			Vector3[] vertices = mesh2.vertices;
			Vector3[] normals = mesh2.normals;
			bool flag = true;
			if (normals == null || normals.Length < vertices.Length || rotate == Quaternion.identity)
			{
				flag = false;
			}
			for (int i = 0; i < vertices.Length; i++)
			{
				Vector3[] expr_4A_cp_0 = vertices;
				int expr_4A_cp_1 = i;
				expr_4A_cp_0[expr_4A_cp_1].x = expr_4A_cp_0[expr_4A_cp_1].x * scale.x;
				Vector3[] expr_65_cp_0 = vertices;
				int expr_65_cp_1 = i;
				expr_65_cp_0[expr_65_cp_1].y = expr_65_cp_0[expr_65_cp_1].y * scale.y;
				Vector3[] expr_80_cp_0 = vertices;
				int expr_80_cp_1 = i;
				expr_80_cp_0[expr_80_cp_1].z = expr_80_cp_0[expr_80_cp_1].z * scale.z;
				vertices[i] = rotate * vertices[i];
				if (flag)
				{
					normals[i] = rotate * normals[i];
				}
				vertices[i] += translate;
			}
			mesh2.vertices = vertices;
			if (flag)
			{
				mesh2.normals = normals;
			}
			mesh2.RecalculateBounds();
			return mesh2;
		}

		public static bool IsSkinnedMesh(this Mesh mesh)
		{
			return mesh.blendShapeCount > 0 || (mesh.bindposes != null && mesh.bindposes.Length > 0);
		}

		public static int GetTriangleCount(this Mesh orig)
		{
			return orig.triangles.Length / 3;
		}

		public static Mesh MakeLODMesh(this Mesh orig, float aMaxWeight, bool recalcNormals, float removeSmallParts = 1f, float protectNormals = 1f, float protectUvs = 1f, float protectSubMeshesAndSharpEdges = 1f, float smallTrianglesFirst = 1f)
		{
			return LODMaker.MakeLODMesh(orig, aMaxWeight, removeSmallParts, protectNormals, protectUvs, protectSubMeshesAndSharpEdges, smallTrianglesFirst, recalcNormals, false);
		}

		[DebuggerHidden]
		public static IEnumerator MakeLODMeshInBackground(this Mesh mesh, float maxWeight, bool recalcNormals, float removeSmallParts, Action<Mesh> result)
		{
			MeshExtensions.<MakeLODMeshInBackground>c__Iterator1DD <MakeLODMeshInBackground>c__Iterator1DD = new MeshExtensions.<MakeLODMeshInBackground>c__Iterator1DD();
			<MakeLODMeshInBackground>c__Iterator1DD.maxWeight = maxWeight;
			<MakeLODMeshInBackground>c__Iterator1DD.removeSmallParts = removeSmallParts;
			<MakeLODMeshInBackground>c__Iterator1DD.mesh = mesh;
			<MakeLODMeshInBackground>c__Iterator1DD.result = result;
			<MakeLODMeshInBackground>c__Iterator1DD.recalcNormals = recalcNormals;
			<MakeLODMeshInBackground>c__Iterator1DD.<$>maxWeight = maxWeight;
			<MakeLODMeshInBackground>c__Iterator1DD.<$>removeSmallParts = removeSmallParts;
			<MakeLODMeshInBackground>c__Iterator1DD.<$>mesh = mesh;
			<MakeLODMeshInBackground>c__Iterator1DD.<$>result = result;
			<MakeLODMeshInBackground>c__Iterator1DD.<$>recalcNormals = recalcNormals;
			return <MakeLODMeshInBackground>c__Iterator1DD;
		}

		public static Mesh[] MakeLODMeshes(this Mesh mesh, float[] maxWeights, bool recalcNormals, float removeSmallParts = 1f, float protectNormals = 1f, float protectUvs = 1f, float protectSubMeshesAndSharpEdges = 1f, float smallTrianglesFirst = 1f, int nrOfSteps = 1)
		{
			if (maxWeights.Length < 1)
			{
				UnityEngine.Debug.LogError("Mesh.GetLODLevelMeshes: maxWeights arrays is empty");
				return null;
			}
			Mesh[] array = new Mesh[maxWeights.Length];
			float num = 0f;
			for (int i = 0; i < maxWeights.Length; i++)
			{
				if (nrOfSteps < 1)
				{
					nrOfSteps = 1;
				}
				for (int j = 0; j < nrOfSteps; j++)
				{
					float num2 = maxWeights[i] - num;
					mesh = mesh.MakeLODMesh((float)(j + 1) * (num2 / (float)nrOfSteps) + num, recalcNormals, removeSmallParts, protectNormals, protectUvs, protectSubMeshesAndSharpEdges, smallTrianglesFirst);
				}
				num = maxWeights[i];
				array[i] = mesh;
			}
			return array;
		}

		public static Vector4 GetUvRange(this Mesh mesh)
		{
			Vector4 result = new Vector4(0f, 0f, 1f, 1f);
			Vector2[] uv = mesh.uv;
			for (int i = 0; i < uv.Length; i++)
			{
				Vector2 vector = uv[i];
				if (vector.x < result.x)
				{
					result.x = vector.x;
				}
				if (vector.y < result.y)
				{
					result.y = vector.y;
				}
				if (vector.x > result.z)
				{
					result.z = vector.x;
				}
				if (vector.y > result.w)
				{
					result.w = vector.y;
				}
			}
			return result;
		}

		public static bool CheckUvsWithin01Range(this Mesh mesh)
		{
			Vector2[] uv = mesh.uv;
			for (int i = 0; i < uv.Length; i++)
			{
				Vector2 vector = uv[i];
				if (vector.x < 0f || vector.x > 1f || vector.y < 0f || vector.y > 1f)
				{
					return false;
				}
			}
			return true;
		}

		public static void ClampUvs(this Mesh mesh)
		{
			Vector2[] uv = mesh.uv;
			for (int i = 0; i < uv.Length; i++)
			{
				Vector2 vector = uv[i];
				vector.x = Mathf.Clamp01(vector.x);
				vector.y = Mathf.Clamp01(vector.y);
				uv[i] = vector;
			}
			mesh.uv = uv;
		}

		public static void WrapUvs(this Mesh mesh)
		{
			Vector2[] uv = mesh.uv;
			for (int i = 0; i < uv.Length; i++)
			{
				Vector2 vector = uv[i];
				while (vector.x > 1f)
				{
					vector.x -= 1f;
				}
				while (vector.x < 0f)
				{
					vector.x += 1f;
				}
				while (vector.y > 1f)
				{
					vector.y -= 1f;
				}
				while (vector.y < 0f)
				{
					vector.y += 1f;
				}
				uv[i] = vector;
			}
			mesh.uv = uv;
		}

		public static void SetAtlasRectForSubmesh(this Mesh mesh, Rect atlasRect, int submeshIndex)
		{
			if (submeshIndex >= mesh.subMeshCount)
			{
				return;
			}
			int[] triangles = mesh.GetTriangles(submeshIndex);
			List<int> list = new List<int>();
			for (int i = 0; i < triangles.Length; i++)
			{
				int j;
				for (j = 0; j < list.Count; j++)
				{
					if (list[j] == triangles[i])
					{
						break;
					}
				}
				if (j >= list.Count)
				{
					list.Add(triangles[i]);
				}
			}
			Vector2[] uv = mesh.uv;
			for (int k = 0; k < list.Count; k++)
			{
				Vector2 vector = uv[list[k]];
				vector.x = Mathf.Clamp01(vector.x) * atlasRect.width + atlasRect.x;
				vector.y = Mathf.Clamp01(vector.y) * atlasRect.height + atlasRect.y;
				uv[list[k]] = vector;
			}
			mesh.uv = uv;
		}

		public static void MergeSubmeshInto(this Mesh mesh, int from, int to)
		{
			int[] triangles = mesh.GetTriangles(from);
			int[] triangles2 = mesh.GetTriangles(to);
			List<int> list = new List<int>();
			for (int i = 0; i < triangles2.Length; i++)
			{
				list.Add(triangles2[i]);
			}
			for (int j = 0; j < triangles.Length; j++)
			{
				list.Add(triangles[j]);
			}
			mesh.SetTriangles(list.ToArray(), to);
			for (int k = from + 1; k < mesh.subMeshCount; k++)
			{
				mesh.SetTriangles(mesh.GetTriangles(k), k - 1);
			}
			mesh.SetTriangles(null, mesh.subMeshCount - 1);
			mesh.subMeshCount--;
		}

		public static Mesh CopyAndRemoveSubmeshes(this Mesh orig, int[] submeshesToRemove)
		{
			Mesh mesh = UnityEngine.Object.Instantiate<Mesh>(orig);
			List<List<int>> list = new List<List<int>>(mesh.subMeshCount);
			List<Vector3> list2 = new List<Vector3>(orig.vertices);
			List<Vector3> list3 = new List<Vector3>(orig.vertexCount);
			List<Vector2> list4 = new List<Vector2>(orig.vertexCount);
			List<Vector2> list5 = new List<Vector2>();
			List<Vector2> list6 = new List<Vector2>();
			List<Vector2> list7 = new List<Vector2>();
			List<Color32> list8 = new List<Color32>();
			List<BoneWeight> list9 = new List<BoneWeight>();
			list3.AddRange(orig.normals);
			list4.AddRange(orig.uv);
			list5.AddRange(orig.uv2);
			list6.AddRange(orig.uv3);
			list7.AddRange(orig.uv4);
			list8.AddRange(orig.colors32);
			list9.AddRange(orig.boneWeights);
			for (int i = 0; i < mesh.subMeshCount; i++)
			{
				bool flag = false;
				for (int j = 0; j < submeshesToRemove.Length; j++)
				{
					if (submeshesToRemove[j] == i)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					List<int> list10 = new List<int>();
					list10.AddRange(mesh.GetTriangles(i));
					list.Add(list10);
				}
			}
			LODMaker.RemoveUnusedVertices(list2, list3, list4, list5, list6, list7, list8, list9, list);
			mesh.uv4 = null;
			mesh.uv3 = null;
			mesh.uv2 = null;
			mesh.uv2 = null;
			mesh.boneWeights = null;
			mesh.colors32 = null;
			mesh.normals = null;
			mesh.tangents = null;
			mesh.triangles = null;
			mesh.vertices = list2.ToArray();
			if (list3.Count > 0)
			{
				mesh.normals = list3.ToArray();
			}
			if (list4.Count > 0)
			{
				mesh.uv = list4.ToArray();
			}
			if (list5.Count > 0)
			{
				mesh.uv2 = list5.ToArray();
			}
			if (list6.Count > 0)
			{
				mesh.uv3 = list6.ToArray();
			}
			if (list7.Count > 0)
			{
				mesh.uv4 = list7.ToArray();
			}
			if (list8.Count > 0)
			{
				mesh.colors32 = list8.ToArray();
			}
			if (list9.Count > 0)
			{
				mesh.boneWeights = list9.ToArray();
			}
			mesh.subMeshCount = list.Count;
			for (int k = 0; k < list.Count; k++)
			{
				mesh.SetTriangles(list[k].ToArray(), k);
			}
			if (list3 == null || list3.Count <= 0)
			{
				mesh.RecalculateNormals();
			}
			mesh.RecalculateTangents();
			mesh.RecalculateBounds();
			return mesh;
		}

		public static Mesh CopyAndRemoveHiddenTriangles(this Mesh orig, int subMeshIdx, Matrix4x4 localToWorldMatrix, Mesh[] hidingMeshes, int[] hidingSubMeshes, Matrix4x4[] hidingLocalToWorldMatrices, float maxRemoveDistance = 0.01f)
		{
			if (subMeshIdx >= orig.subMeshCount)
			{
				return null;
			}
			if (hidingMeshes.Length <= 0)
			{
				return null;
			}
			if (hidingMeshes.Length != hidingSubMeshes.Length || hidingMeshes.Length != hidingLocalToWorldMatrices.Length)
			{
				return null;
			}
			Mesh mesh = UnityEngine.Object.Instantiate<Mesh>(orig);
			List<List<int>> list = new List<List<int>>(mesh.subMeshCount);
			List<Vector3> list2 = new List<Vector3>(orig.vertices);
			List<Vector3> list3 = new List<Vector3>(orig.vertexCount);
			List<Vector2> list4 = new List<Vector2>(orig.vertexCount);
			List<Vector2> list5 = new List<Vector2>();
			List<Vector2> list6 = new List<Vector2>();
			List<Vector2> list7 = new List<Vector2>();
			List<Color32> list8 = new List<Color32>();
			List<BoneWeight> list9 = new List<BoneWeight>();
			list3.AddRange(orig.normals);
			if (list3 == null || list3.Count <= 0)
			{
				orig.RecalculateNormals();
				list3.AddRange(orig.normals);
			}
			list4.AddRange(orig.uv);
			list5.AddRange(orig.uv2);
			list6.AddRange(orig.uv3);
			list7.AddRange(orig.uv4);
			list8.AddRange(orig.colors32);
			list9.AddRange(orig.boneWeights);
			for (int i = 0; i < orig.subMeshCount; i++)
			{
				List<int> list10 = new List<int>();
				list10.AddRange(orig.GetTriangles(i));
				list.Add(list10);
			}
			List<int> list11 = list[subMeshIdx];
			List<Vector3> list12 = new List<Vector3>();
			List<int> list13 = new List<int>();
			Mesh x = null;
			int num = 0;
			int num2 = 0;
			for (int j = 0; j < hidingMeshes.Length; j++)
			{
				Mesh mesh2 = hidingMeshes[j];
				int[] triangles = mesh2.GetTriangles(hidingSubMeshes[j]);
				if (x != mesh2)
				{
					num += num2;
				}
				for (int k = 0; k < triangles.Length; k++)
				{
					list13.Add(triangles[k] + num);
				}
				if (x != mesh2)
				{
					Matrix4x4 matrix4x = hidingLocalToWorldMatrices[j];
					Vector3[] vertices = mesh2.vertices;
					for (int l = 0; l < vertices.Length; l++)
					{
						list12.Add(matrix4x.MultiplyPoint3x4(vertices[l]));
					}
					num2 = vertices.Length;
					x = mesh2;
				}
			}
			List<Vector3> list14 = new List<Vector3>();
			List<Vector3> list15 = new List<Vector3>();
			for (int m = 0; m < list13.Count; m += 3)
			{
				Vector3 vector = list12[list13[m]];
				Vector3 vector2 = list12[list13[m + 1]];
				Vector3 vector3 = list12[list13[m + 2]];
				list14.Add(new Vector3(Mathf.Min(Mathf.Min(vector.x, vector2.x), vector3.x), Mathf.Min(Mathf.Min(vector.y, vector2.y), vector3.y), Mathf.Min(Mathf.Min(vector.z, vector2.z), vector3.z)));
				list15.Add(new Vector3(Mathf.Max(Mathf.Max(vector.x, vector2.x), vector3.x), Mathf.Max(Mathf.Max(vector.y, vector2.y), vector3.y), Mathf.Max(Mathf.Max(vector.z, vector2.z), vector3.z)));
			}
			List<int> list16 = new List<int>();
			for (int n = 0; n < list11.Count; n += 3)
			{
				Vector3 v = localToWorldMatrix.MultiplyPoint3x4(list2[list11[n]]);
				Vector3 v2 = localToWorldMatrix.MultiplyPoint3x4(list2[list11[n + 1]]);
				Vector3 v3 = localToWorldMatrix.MultiplyPoint3x4(list2[list11[n + 2]]);
				if (!MeshExtensions.IsTriangleHidden(v, v2, v3, maxRemoveDistance, list14, list15, list12, list13))
				{
					list16.Add(list11[n]);
					list16.Add(list11[n + 1]);
					list16.Add(list11[n + 2]);
				}
			}
			list[subMeshIdx] = list16;
			LODMaker.RemoveUnusedVertices(list2, list3, list4, list5, list6, list7, list8, list9, list);
			mesh.uv4 = null;
			mesh.uv3 = null;
			mesh.uv2 = null;
			mesh.uv2 = null;
			mesh.boneWeights = null;
			mesh.colors32 = null;
			mesh.normals = null;
			mesh.tangents = null;
			mesh.triangles = null;
			mesh.vertices = list2.ToArray();
			if (list3.Count > 0)
			{
				mesh.normals = list3.ToArray();
			}
			if (list4.Count > 0)
			{
				mesh.uv = list4.ToArray();
			}
			if (list5.Count > 0)
			{
				mesh.uv2 = list5.ToArray();
			}
			if (list6.Count > 0)
			{
				mesh.uv3 = list6.ToArray();
			}
			if (list7.Count > 0)
			{
				mesh.uv4 = list7.ToArray();
			}
			if (list8.Count > 0)
			{
				mesh.colors32 = list8.ToArray();
			}
			if (list9.Count > 0)
			{
				mesh.boneWeights = list9.ToArray();
			}
			mesh.subMeshCount = list.Count;
			for (int num3 = 0; num3 < list.Count; num3++)
			{
				if (num3 == subMeshIdx)
				{
					mesh.SetTriangles(list16.ToArray(), num3);
				}
				else
				{
					mesh.SetTriangles(list[num3].ToArray(), num3);
				}
			}
			if (list3 == null || list3.Count <= 0)
			{
				mesh.RecalculateNormals();
			}
			mesh.RecalculateTangents();
			mesh.RecalculateBounds();
			return mesh;
		}

		private static bool IsTriangleHidden(Vector3 v0, Vector3 v1, Vector3 v2, float maxDistance, List<Vector3> triMinCorners, List<Vector3> triMaxCorners, List<Vector3> hidingVs, List<int> hidingTs)
		{
			Vector3 normal = MeshExtensions.GetNormal(v0, v1, v2);
			List<int> trianglesWithinRange = MeshExtensions.GetTrianglesWithinRange(v0, v1, v2, maxDistance, triMinCorners, triMaxCorners);
			return MeshExtensions.IsHidden((v0 + v1 + v2) / 3f, normal, maxDistance, hidingVs, hidingTs, trianglesWithinRange) && MeshExtensions.IsHidden(v0, normal, maxDistance, hidingVs, hidingTs, trianglesWithinRange) && MeshExtensions.IsHidden(v1, normal, maxDistance, hidingVs, hidingTs, trianglesWithinRange) && MeshExtensions.IsHidden(v2, normal, maxDistance, hidingVs, hidingTs, trianglesWithinRange);
		}

		private static bool IsHidden(Vector3 v, Vector3 n, float maxDistance, List<Vector3> hidingVs, List<int> hidingTs, List<int> trianglesToCheck)
		{
			for (int i = 0; i < trianglesToCheck.Count; i++)
			{
				int num = trianglesToCheck[i] * 3;
				Vector3 vector = hidingVs[hidingTs[num]];
				Vector3 vector2 = hidingVs[hidingTs[num + 1]];
				Vector3 vector3 = hidingVs[hidingTs[num + 2]];
				Vector3 normal = MeshExtensions.GetNormal(vector, vector2, vector3);
				float num2 = Vector3.Angle(n, normal);
				if (num2 < 60f)
				{
					float num3 = MeshExtensions.DistanceToPlane(v, n, vector, normal);
					if (num3 > 0f && num3 <= maxDistance)
					{
						Vector3 p = v + n * num3;
						Vector2 v2 = p.Barycentric(vector, vector2, vector3);
						if (v2.IsBarycentricInTriangle())
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		private static List<int> GetTrianglesWithinRange(Vector3 v0, Vector3 v1, Vector3 v2, float maxDistance, List<Vector3> triMinCorners, List<Vector3> triMaxCorners)
		{
			List<int> list = new List<int>();
			Vector3 vector = new Vector3(Mathf.Min(Mathf.Min(v0.x, v1.x), v2.x) - maxDistance, Mathf.Min(Mathf.Min(v0.y, v1.y), v2.y) - maxDistance, Mathf.Min(Mathf.Min(v0.z, v1.z), v2.z) - maxDistance);
			Vector3 vector2 = new Vector3(Mathf.Max(Mathf.Max(v0.x, v1.x), v2.x) + maxDistance, Mathf.Max(Mathf.Max(v0.y, v1.y), v2.y) + maxDistance, Mathf.Max(Mathf.Max(v0.z, v1.z), v2.z) + maxDistance);
			for (int i = 0; i < triMaxCorners.Count; i++)
			{
				Vector3 vector3 = triMaxCorners[i];
				if (vector3.x > vector.x && vector3.y > vector.y && vector3.z > vector.z)
				{
					Vector3 vector4 = triMinCorners[i];
					if (vector4.x < vector2.x && vector4.y < vector2.y && vector4.z < vector2.z)
					{
						list.Add(i);
					}
				}
			}
			return list;
		}

		public static float DistanceToPlane(Vector3 fromPos, Vector3 direction, Vector3 pointOnPlane, Vector3 normalPlane)
		{
			float result = float.PositiveInfinity;
			float num = direction.InProduct(normalPlane);
			if (num != 0f)
			{
				result = (pointOnPlane - fromPos).InProduct(normalPlane) / direction.InProduct(normalPlane);
			}
			return result;
		}

		public static Vector3 GetNormal(Vector3 v0, Vector3 v1, Vector3 v2)
		{
			return Vector3.Cross(v1 - v0, v2 - v0).normalized;
		}
	}
}
