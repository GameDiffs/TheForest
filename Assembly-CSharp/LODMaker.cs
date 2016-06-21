using OrbCreationExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LODMaker
{
	public static Mesh MakeLODMesh(Mesh orig, float aMaxWeight, bool recalcNormals = true, float removeSmallParts = 1f, bool reuseOldMesh = false)
	{
		return LODMaker.MakeLODMesh(orig, aMaxWeight, removeSmallParts, 1f, 1f, 1f, 1f, recalcNormals, reuseOldMesh);
	}

	public static Mesh MakeLODMesh(Mesh orig, float aMaxWeight, float removeSmallParts, float protectNormals, float protectUvs, float smallTrianglesFirst, float protectSubMeshesAndSharpEdges, bool recalcNormals, bool reuseOldMesh = false)
	{
		if (!orig.isReadable)
		{
			Debug.LogError("Sorry, mesh was not marked for read/write upon import");
			return orig;
		}
		float sideLengthWeight;
		float oldAngleWeight;
		float newAngleWeight;
		float uvWeight;
		float areaDiffWeight;
		float normalWeight;
		float vertexWeight;
		float centerDistanceWeight;
		LODMaker.GetWeights(aMaxWeight, removeSmallParts, protectNormals, protectUvs, smallTrianglesFirst, protectSubMeshesAndSharpEdges, out sideLengthWeight, out oldAngleWeight, out newAngleWeight, out uvWeight, out areaDiffWeight, out normalWeight, out vertexWeight, out centerDistanceWeight);
		return LODMaker.MakeLODMesh(orig, aMaxWeight, removeSmallParts, sideLengthWeight, oldAngleWeight, newAngleWeight, uvWeight, areaDiffWeight, normalWeight, vertexWeight, centerDistanceWeight, recalcNormals, reuseOldMesh);
	}

	private static void GetWeights(float aMaxWeight, float removeSmallParts, float protectNormals, float protectUvs, float smallTrianglesFirst, float protectSubMeshesAndSharpEdges, out float sideLengthWeight, out float oldAngleWeight, out float newAngleWeight, out float uvWeight, out float areaDiffWeight, out float normalWeight, out float vertexWeight, out float centerDistanceWeight)
	{
		float num = 0.12f / (0.5f + aMaxWeight);
		sideLengthWeight = 3f * num * smallTrianglesFirst;
		oldAngleWeight = 0.1f * num * smallTrianglesFirst;
		newAngleWeight = 0.4f * num;
		uvWeight = 800f * num * protectUvs;
		areaDiffWeight = 10f * num;
		normalWeight = 50f * num * protectNormals;
		vertexWeight = 200f * num * protectSubMeshesAndSharpEdges;
		centerDistanceWeight = 5000f * num;
	}

	public static void MakeLODMeshInBackground(object data)
	{
		string text = "start " + DateTime.Now.ToString("yyy/MM/dd hh:mm:ss.fff");
		Hashtable hashtable = (Hashtable)data;
		float num = (float)hashtable["maxWeight"];
		Vector3[] array = (Vector3[])hashtable["vertices"];
		int[] array2 = (int[])hashtable["triangles"];
		int[] value = (int[])hashtable["subMeshOffsets"];
		Bounds meshBounds = (Bounds)hashtable["meshBounds"];
		float removeSmallParts = 1f;
		float protectNormals = 1f;
		float protectUvs = 1f;
		float protectSubMeshesAndSharpEdges = 1f;
		float smallTrianglesFirst = 1f;
		Vector3[] ns = null;
		Vector2[] uv1s = null;
		Vector2[] uv2s = null;
		Vector2[] uv3s = null;
		Vector2[] uv4s = null;
		Color32[] colors = null;
		Matrix4x4[] value2 = null;
		BoneWeight[] bws = null;
		if (hashtable.ContainsKey("removeSmallParts"))
		{
			removeSmallParts = (float)hashtable["removeSmallParts"];
		}
		if (hashtable.ContainsKey("protectNormals"))
		{
			protectNormals = (float)hashtable["protectNormals"];
		}
		if (hashtable.ContainsKey("protectUvs"))
		{
			protectUvs = (float)hashtable["protectUvs"];
		}
		if (hashtable.ContainsKey("protectSubMeshesAndSharpEdges"))
		{
			protectSubMeshesAndSharpEdges = (float)hashtable["protectSubMeshesAndSharpEdges"];
		}
		if (hashtable.ContainsKey("smallTrianglesFirst"))
		{
			smallTrianglesFirst = (float)hashtable["smallTrianglesFirst"];
		}
		if (hashtable.ContainsKey("normals"))
		{
			ns = (Vector3[])hashtable["normals"];
		}
		if (hashtable.ContainsKey("uv1s"))
		{
			uv1s = (Vector2[])hashtable["uv1s"];
		}
		if (hashtable.ContainsKey("uv2s"))
		{
			uv2s = (Vector2[])hashtable["uv2s"];
		}
		if (hashtable.ContainsKey("uv3s"))
		{
			uv3s = (Vector2[])hashtable["uv3s"];
		}
		if (hashtable.ContainsKey("uv4s"))
		{
			uv4s = (Vector2[])hashtable["uv4s"];
		}
		if (hashtable.ContainsKey("colors32"))
		{
			colors = (Color32[])hashtable["colors32"];
		}
		if (hashtable.ContainsKey("bindposes"))
		{
			value2 = (Matrix4x4[])hashtable["bindposes"];
		}
		if (hashtable.ContainsKey("boneWeights"))
		{
			bws = (BoneWeight[])hashtable["boneWeights"];
		}
		float sideLengthWeight;
		float oldAngleWeight;
		float newAngleWeight;
		float uvWeight;
		float areaDiffWeight;
		float normalWeight;
		float vertexWeight;
		float centerDistanceWeight;
		LODMaker.GetWeights(num, removeSmallParts, protectNormals, protectUvs, smallTrianglesFirst, protectSubMeshesAndSharpEdges, out sideLengthWeight, out oldAngleWeight, out newAngleWeight, out uvWeight, out areaDiffWeight, out normalWeight, out vertexWeight, out centerDistanceWeight);
		List<Vector3> list;
		List<Vector3> list2;
		List<Vector2> list3;
		List<Vector2> list4;
		List<Vector2> list5;
		List<Vector2> list6;
		List<Color32> list7;
		List<int> list8;
		List<BoneWeight> list9;
		LODMaker.MakeLODMesh(array, ns, uv1s, uv2s, uv3s, uv4s, colors, array2, ref value2, bws, ref value, meshBounds, num, removeSmallParts, sideLengthWeight, oldAngleWeight, newAngleWeight, uvWeight, areaDiffWeight, normalWeight, vertexWeight, centerDistanceWeight, out list, out list2, out list3, out list4, out list5, out list6, out list7, out list8, out list9);
		((Hashtable)data)["vertices"] = list.ToArray();
		((Hashtable)data)["normals"] = list2.ToArray();
		((Hashtable)data)["uv1s"] = list3.ToArray();
		((Hashtable)data)["uv2s"] = list4.ToArray();
		((Hashtable)data)["uv3s"] = list5.ToArray();
		((Hashtable)data)["uv4s"] = list6.ToArray();
		((Hashtable)data)["colors32"] = list7.ToArray();
		((Hashtable)data)["triangles"] = list8.ToArray();
		((Hashtable)data)["bindposes"] = value2;
		((Hashtable)data)["boneWeights"] = list9.ToArray();
		((Hashtable)data)["subMeshOffsets"] = value;
		((Hashtable)data)["ready"] = true;
		Debug.Log(string.Concat(new object[]
		{
			"compression:",
			num,
			", vertices:",
			array.Length,
			" -> ",
			list.Count,
			", triangles:",
			array2.Length / 3,
			" -> ",
			list8.Count / 3,
			"\n",
			text,
			"\nended ",
			DateTime.Now.ToString("yyy/MM/dd hh:mm:ss.fff")
		}));
	}

	private static Mesh MakeLODMesh(Mesh orig, float maxWeight, float removeSmallParts, float sideLengthWeight, float oldAngleWeight, float newAngleWeight, float uvWeight, float areaDiffWeight, float normalWeight, float vertexWeight, float centerDistanceWeight, bool recalcNormals, bool reuseOldMesh)
	{
		string text = "started " + DateTime.Now.ToString("yyy/MM/dd hh:mm:ss.fff");
		Vector3[] vertices = orig.vertices;
		if (vertices.Length <= 0)
		{
			LODMaker.Log("Mesh was empty");
			return orig;
		}
		Vector3[] normals = orig.normals;
		if (normals.Length == 0)
		{
			orig.RecalculateNormals();
			normals = orig.normals;
		}
		Vector2[] uv = orig.uv;
		Vector2[] uv2 = orig.uv2;
		Vector2[] uv3 = orig.uv3;
		Vector2[] uv4 = orig.uv4;
		Color32[] colors = orig.colors32;
		int[] triangles = orig.triangles;
		Matrix4x4[] bindposes = orig.bindposes;
		BoneWeight[] boneWeights = orig.boneWeights;
		int[] array = new int[orig.subMeshCount];
		if (orig.subMeshCount > 1)
		{
			for (int i = 0; i < orig.subMeshCount; i++)
			{
				int[] triangles2 = orig.GetTriangles(i);
				int j;
				for (j = 0; j < triangles2.Length; j++)
				{
					triangles[array[i] + j] = triangles2[j];
				}
				if (i + 1 < orig.subMeshCount)
				{
					array[i + 1] = array[i] + j;
				}
			}
		}
		Bounds bounds = orig.bounds;
		List<Vector3> list;
		List<Vector3> list2;
		List<Vector2> list3;
		List<Vector2> list4;
		List<Vector2> list5;
		List<Vector2> list6;
		List<Color32> list7;
		List<int> list8;
		List<BoneWeight> list9;
		LODMaker.MakeLODMesh(vertices, normals, uv, uv2, uv3, uv4, colors, triangles, ref bindposes, boneWeights, ref array, bounds, maxWeight, removeSmallParts, sideLengthWeight, oldAngleWeight, newAngleWeight, uvWeight, areaDiffWeight, normalWeight, vertexWeight, centerDistanceWeight, out list, out list2, out list3, out list4, out list5, out list6, out list7, out list8, out list9);
		Debug.Log(string.Concat(new object[]
		{
			"compression:",
			maxWeight,
			", vertices:",
			vertices.Length,
			" -> ",
			list.Count,
			", triangles:",
			triangles.Length / 3,
			" -> ",
			list8.Count / 3,
			"\n",
			text,
			"\nended ",
			DateTime.Now.ToString("yyy/MM/dd hh:mm:ss.fff")
		}));
		if (reuseOldMesh)
		{
			orig.uv4 = null;
			orig.uv3 = null;
			orig.uv2 = null;
			orig.uv = null;
			orig.colors = null;
			orig.tangents = null;
			orig.boneWeights = null;
			orig.bindposes = null;
			orig.triangles = null;
			orig.subMeshCount = 0;
			orig.normals = null;
			LODMaker.FillMesh(orig, list.ToArray(), list2.ToArray(), list3.ToArray(), list4.ToArray(), list5.ToArray(), list6.ToArray(), list7.ToArray(), list8.ToArray(), list9.ToArray(), bindposes, array, recalcNormals);
			return orig;
		}
		return LODMaker.CreateNewMesh(list.ToArray(), list2.ToArray(), list3.ToArray(), list4.ToArray(), list5.ToArray(), list6.ToArray(), list7.ToArray(), list8.ToArray(), list9.ToArray(), bindposes, array, recalcNormals);
	}

	private static void MakeLODMesh(Vector3[] vs, Vector3[] ns, Vector2[] uv1s, Vector2[] uv2s, Vector2[] uv3s, Vector2[] uv4s, Color32[] colors32, int[] ts, ref Matrix4x4[] bindposes, BoneWeight[] bws, ref int[] subMeshOffsets, Bounds meshBounds, float maxWeight, float removeSmallParts, float sideLengthWeight, float oldAngleWeight, float newAngleWeight, float uvWeight, float areaDiffWeight, float normalWeight, float vertexWeight, float centerDistanceWeight, out List<Vector3> newVs, out List<Vector3> newNs, out List<Vector2> newUv1s, out List<Vector2> newUv2s, out List<Vector2> newUv3s, out List<Vector2> newUv4s, out List<Color32> newColors32, out List<int> newTs, out List<BoneWeight> newBws)
	{
		int num = 1;
		Vector3 size = meshBounds.size;
		Vector3 center = meshBounds.center;
		Vector3 zero = Vector3.zero;
		if (size.x > 0f)
		{
			zero.x = 1f / size.x;
		}
		if (size.y > 0f)
		{
			zero.y = 1f / size.y;
		}
		if (size.z > 0f)
		{
			zero.z = 1f / size.z;
		}
		List<List<int>> trianglesPerGroup = new List<List<int>>();
		int[] array = new int[ts.Length / 3];
		List<List<int>> list = new List<List<int>>();
		int[] array2 = new int[vs.Length];
		List<int> list2 = new List<int>();
		Vector3[] array3 = new Vector3[vs.Length];
		bool[] array4 = new bool[vs.Length];
		bool[] array5 = new bool[vs.Length];
		int[] array6 = new int[vs.Length];
		int[] array7 = new int[vs.Length];
		int[] array8 = new int[uv1s.Length];
		int[] array9 = new int[uv2s.Length];
		int[] array10 = new int[uv3s.Length];
		int[] array11 = new int[uv4s.Length];
		int[] array12 = new int[colors32.Length];
		float[] array13 = new float[vs.Length];
		for (int i = 0; i < vs.Length; i++)
		{
			array4[i] = false;
			array6[i] = i;
			array7[i] = i;
			list.Add(new List<int>());
			array2[i] = -1;
		}
		for (int j = 0; j < uv1s.Length; j++)
		{
			array8[j] = j;
		}
		for (int k = 0; k < uv2s.Length; k++)
		{
			array9[k] = k;
		}
		for (int l = 0; l < uv3s.Length; l++)
		{
			array10[l] = l;
		}
		for (int m = 0; m < uv4s.Length; m++)
		{
			array11[m] = m;
		}
		for (int n = 0; n < colors32.Length; n++)
		{
			array12[n] = n;
		}
		for (int num2 = 0; num2 < array.Length; num2++)
		{
			array[num2] = -1;
		}
		float num3 = Mathf.Round(meshBounds.size.x * 10000f);
		float num4 = Mathf.Round(meshBounds.size.y * 10000f);
		float num5 = Mathf.Round(meshBounds.size.z * 10000f);
		if (num3 <= 0f)
		{
			num3 = 1f;
		}
		if (num4 <= 0f)
		{
			num4 = 1f;
		}
		if (num5 <= 0f)
		{
			num5 = 1f;
		}
		for (int num6 = 0; num6 < vs.Length; num6++)
		{
			vs[num6].x = Mathf.Round(vs[num6].x * num3) / num3;
			vs[num6].y = Mathf.Round(vs[num6].y * num4) / num4;
			vs[num6].z = Mathf.Round(vs[num6].z * num5) / num5;
		}
		for (int num7 = 0; num7 < vs.Length; num7++)
		{
			int num8 = LODMaker.GetLastVertexWithYSmaller(vs[num7].y, list2, vs, num7);
			for (num8++; num8 < list2.Count; num8++)
			{
				if (list2[num8] < 0)
				{
					break;
				}
				if (vs[list2[num8]].y > vs[num7].y)
				{
					break;
				}
				if (vs[list2[num8]].y == vs[num7].y)
				{
					if (vs[list2[num8]].z > vs[num7].z)
					{
						break;
					}
					if (vs[num7].z == vs[list2[num8]].z && vs[list2[num8]].x > vs[num7].x)
					{
						break;
					}
				}
			}
			list2.Insert(num8, num7);
		}
		int num9 = -1;
		for (int num10 = 0; num10 < ts.Length; num10++)
		{
			if (num10 + 1 < subMeshOffsets.Length && num10 >= subMeshOffsets[num9 + 1])
			{
				num9++;
			}
			int num11 = array2[ts[num10]];
			if (num11 < 0)
			{
				array2[ts[num10]] = num9;
			}
			else if (num11 != num9)
			{
				array13[ts[num10]] += (float)vs.Length;
			}
		}
		for (int num12 = 0; num12 < vs.Length; num12++)
		{
			array3[num12] = (vs[num12] - center).Product(zero);
			List<int> verticesEqualTo = LODMaker.GetVerticesEqualTo(vs[num12], list2, vs);
			if (verticesEqualTo.Count > 1)
			{
				array5[num12] = true;
				for (int num13 = 0; num13 < verticesEqualTo.Count; num13++)
				{
					if (num13 != num12)
					{
						if (array7[verticesEqualTo[num13]] == verticesEqualTo[num13])
						{
							array7[verticesEqualTo[num13]] = array7[num12];
						}
						if (ns != null && ns.Length != 0 && (ns[verticesEqualTo[0]].x != ns[verticesEqualTo[num13]].x || ns[verticesEqualTo[0]].y != ns[verticesEqualTo[num13]].y || ns[verticesEqualTo[0]].z != ns[verticesEqualTo[num13]].z))
						{
							array13[num12] += normalWeight * 0.05f;
							array13[verticesEqualTo[num13]] += normalWeight * 0.05f;
						}
						if (uv1s != null && uv1s.Length != 0 && (uv1s[verticesEqualTo[0]].x != uv1s[verticesEqualTo[num13]].x || uv1s[verticesEqualTo[0]].y != uv1s[verticesEqualTo[num13]].y))
						{
							array13[num12] += uvWeight * 0.02f;
							array13[verticesEqualTo[num13]] += uvWeight * 0.02f;
						}
						if (uv2s != null && uv2s.Length != 0 && (uv2s[verticesEqualTo[0]].x != uv2s[verticesEqualTo[num13]].x || uv2s[verticesEqualTo[0]].y != uv2s[verticesEqualTo[num13]].y))
						{
							array13[num12] += uvWeight * 0.013f;
							array13[verticesEqualTo[num13]] += uvWeight * 0.013f;
						}
					}
				}
			}
		}
		for (int num14 = 0; num14 < ts.Length; num14++)
		{
			list[array7[ts[num14]]].Add(num14 / 3);
		}
		float num15 = maxWeight * (0.8f + (float)vs.Length / 65536f * 0.2f);
		float num16 = 0f;
		int num17 = 0;
		for (int num18 = 0; num18 < ts.Length; num18 += 3)
		{
			int num19 = ts[num18];
			int num20 = ts[num18 + 1];
			int num21 = ts[num18 + 2];
			if (num19 != num20 && num20 != num21 && num21 != num19)
			{
				num16 += LODMaker.Area(vs[array6[num19]].Product(zero), vs[array6[num20]].Product(zero), vs[array6[num21]].Product(zero));
				num17++;
			}
		}
		if (num17 > 0)
		{
			num16 /= (float)num17;
		}
		int[] array14 = new int[]
		{
			1,
			0,
			2,
			1,
			0,
			2
		};
		int[] array15 = new int[]
		{
			0,
			1,
			1,
			2,
			2,
			0
		};
		for (int num22 = 0; num22 < 3; num22++)
		{
			float num23 = num16 * (float)num22 * 0.5f;
			float num24 = num16 * (float)(num22 + 1) * 0.5f;
			if (num22 >= 2)
			{
				num24 = float.PositiveInfinity;
			}
			for (int num25 = 0; num25 < ts.Length; num25 += 3)
			{
				int num26 = ts[num25];
				int num27 = ts[num25 + 1];
				int num28 = ts[num25 + 2];
				if (num26 != num27 && num27 != num28 && num28 != num26)
				{
					int[] array16 = new int[]
					{
						num26,
						num27,
						num27,
						num28,
						num28,
						num26
					};
					int[] array17 = new int[]
					{
						num27,
						num26,
						num28,
						num27,
						num26,
						num28
					};
					float[] array18 = new float[6];
					float[] array19 = new float[]
					{
						(vs[array6[num26]] - vs[array6[num27]]).Product(zero).magnitude,
						(vs[array6[num27]] - vs[array6[num28]]).Product(zero).magnitude,
						(vs[array6[num28]] - vs[array6[num26]]).Product(zero).magnitude
					};
					float num29 = LODMaker.Area(vs[array6[num26]].Product(zero), vs[array6[num27]].Product(zero), vs[array6[num28]].Product(zero));
					if (num29 >= num23 && num29 < num24)
					{
						num29 = Mathf.Sqrt(num29);
						for (int num30 = 0; num30 < 6; num30++)
						{
							array18[num30] += num29 * sideLengthWeight;
						}
						for (int num31 = 0; num31 < 6; num31++)
						{
							array18[num31] += array19[num31 / 2] * sideLengthWeight;
						}
						for (int num32 = 0; num32 < 6; num32++)
						{
							array18[num32] += array13[array16[num32]] * num29 * vertexWeight;
						}
						for (int num33 = 0; num33 < 6; num33++)
						{
							if (num33 / 2 * 2 == num33)
							{
								float num34 = LODMaker.GetNormalDiffForCorners(ns, array16[num33 / 2 * 2], array17[num33 / 2 * 2]) * array19[num33 / 2] * normalWeight;
								array18[num33] += num34;
								array18[num33 + 1] += num34;
							}
						}
						int[] adjacentTriangles = LODMaker.GetAdjacentTriangles(ts, num25, list, array7, array, trianglesPerGroup);
						if (LODMaker.AnyWeightOK(array18, num15))
						{
							float[] array20 = new float[3];
							Vector3[] array21 = new Vector3[3];
							for (int num35 = 0; num35 < 3; num35++)
							{
								LODMaker.GetTotalAngleAndCenterDistanceForCorner(adjacentTriangles, vs, array6, array16[num35 * 2], array3, ref array20[num35], ref array21[num35]);
							}
							for (int num36 = 0; num36 < 6; num36++)
							{
								if (array18[num36] < num15)
								{
									array18[num36] += LODMaker.AngleDiff(array20[array15[num36]]) * array19[num36 / 2] * num29 * oldAngleWeight;
								}
							}
							if (LODMaker.AnyWeightOK(array18, num15))
							{
								for (int num37 = 0; num37 < 6; num37++)
								{
									if (array18[num37] < num15)
									{
										float num38 = 0f;
										Vector3 zero2 = Vector3.zero;
										bool flag = false;
										LODMaker.GetTotalAngleAndCenterDistanceForNewCorner(adjacentTriangles, vs, array6, array7, array16[num37], array17[num37], array3, maxWeight, ref num38, ref zero2, ref flag);
										if (flag)
										{
											array18[num37] += 100f * num29;
										}
										if (Mathf.Abs(num38) < 10f)
										{
											array18[num37] += LODMaker.AngleCornerDiff(num38 - array20[array14[num37]]) * Mathf.Sqrt(array19[num37 / 2]) * newAngleWeight;
										}
										else
										{
											array18[num37] += LODMaker.AngleDiff(num38 - array20[array14[num37]]) * Mathf.Sqrt(array19[num37 / 2]) * newAngleWeight;
										}
										if (ns != null && ns.Length > 0)
										{
											array18[num37] += Vector3.Project(vs[array6[array16[num37]]] - vs[array6[array17[num37]]], ns[array16[num37]]).magnitude * (zero2 - array21[array14[num37]]).magnitude * centerDistanceWeight;
										}
									}
								}
								if (LODMaker.AnyWeightOK(array18, num15))
								{
									float num39 = LODMaker.Area(vs[array6[num26]], vs[array6[num27]], vs[array6[num28]]);
									for (int num40 = 0; num40 < 6; num40++)
									{
										if (array18[num40] < num15)
										{
											float num41 = 0f;
											float num42 = 0f;
											float num43 = 0f;
											float f = 0f;
											LODMaker.GetUVStretchAndAreaForCorner(adjacentTriangles, vs, array6, array7, uv1s, array16[num40], array17[num40], ref num42, ref num41, ref f, ref num43);
											array18[num40] += num42 * 10f * uvWeight;
											array18[num40] += (Mathf.Pow(Mathf.Abs(f) + 1f, 2f) - 1f) * 30f * uvWeight;
											if (num39 <= 0f)
											{
												num39 = Mathf.Max(num41, num43);
											}
											if (num39 > 0f)
											{
												if (num41 / num39 > 1f)
												{
													array18[num40] += num41 / num39 * 0.5f * areaDiffWeight;
												}
												array18[num40] += (Mathf.Pow(Mathf.Abs(num43 / num39) + 1f, 2f) - 1f) * 0.5f * areaDiffWeight;
											}
										}
									}
								}
							}
						}
						if (LODMaker.AnyWeightOK(array18, num15))
						{
							for (int num44 = 0; num44 < 6; num44++)
							{
								array18[num44] *= 0.05f + 0.95f * array19[num44 / 2];
							}
							int num45 = -1;
							float num46 = float.PositiveInfinity;
							for (int num47 = 0; num47 < 6; num47++)
							{
								if (array18[num47] < num46)
								{
									num46 = array18[num47];
									num45 = num47;
								}
							}
							switch (num45)
							{
							case 0:
								LODMaker.MergeVertices(ref num26, num27, array5, vs, ts, uv1s, uv2s, uv3s, uv4s, colors32, array4, array6, array7, array8, array9, array10, array11, array12, list, num > 1);
								break;
							case 1:
								LODMaker.MergeVertices(ref num27, num26, array5, vs, ts, uv1s, uv2s, uv3s, uv4s, colors32, array4, array6, array7, array8, array9, array10, array11, array12, list, num > 1);
								break;
							case 2:
								LODMaker.MergeVertices(ref num27, num28, array5, vs, ts, uv1s, uv2s, uv3s, uv4s, colors32, array4, array6, array7, array8, array9, array10, array11, array12, list, num > 1);
								break;
							case 3:
								LODMaker.MergeVertices(ref num28, num27, array5, vs, ts, uv1s, uv2s, uv3s, uv4s, colors32, array4, array6, array7, array8, array9, array10, array11, array12, list, num > 1);
								break;
							case 4:
								LODMaker.MergeVertices(ref num28, num26, array5, vs, ts, uv1s, uv2s, uv3s, uv4s, colors32, array4, array6, array7, array8, array9, array10, array11, array12, list, num > 1);
								break;
							case 5:
								LODMaker.MergeVertices(ref num26, num28, array5, vs, ts, uv1s, uv2s, uv3s, uv4s, colors32, array4, array6, array7, array8, array9, array10, array11, array12, list, num > 1);
								break;
							}
						}
					}
				}
			}
		}
		newVs = new List<Vector3>();
		newNs = new List<Vector3>();
		newUv1s = new List<Vector2>();
		newUv2s = new List<Vector2>();
		newUv3s = new List<Vector2>();
		newUv4s = new List<Vector2>();
		newColors32 = new List<Color32>();
		newTs = new List<int>();
		List<int> newTGrps = new List<int>();
		newBws = new List<BoneWeight>();
		int[] o2n = new int[vs.Length];
		LODMaker.FillNewMeshArray(vs, array4, array6, ns, uv1s, array8, uv2s, array9, uv3s, array10, uv4s, array11, colors32, array12, bws, newVs, newNs, newUv1s, newUv2s, newUv3s, newUv4s, newColors32, newBws, o2n);
		LODMaker.FillNewMeshTriangles(ts, o2n, newTs, subMeshOffsets, array, newTGrps);
		LODMaker.RemoveEmptyTriangles(newVs, newNs, newUv1s, newUv2s, newUv3s, newUv4s, newColors32, newTs, newBws, subMeshOffsets, newTGrps);
		if (removeSmallParts > 0f)
		{
			LODMaker.RemoveMiniTriangleGroups(removeSmallParts, zero, maxWeight, newVs, newTs, subMeshOffsets, newTGrps);
		}
		LODMaker.RemoveUnusedVertices(newVs, newNs, newUv1s, newUv2s, newUv3s, newUv4s, newColors32, newBws, newTs);
	}

	private static bool AnyWeightOK(float[] weights, float aMaxWeight)
	{
		for (int i = 0; i < 6; i++)
		{
			if (weights[i] < aMaxWeight)
			{
				return true;
			}
		}
		return false;
	}

	private static int[] GetAdjacentTriangles(int[] ts, int tIdx, List<List<int>> trianglesPerVertex, int[] uniqueVs, int[] triangleGroups, List<List<int>> trianglesPerGroup)
	{
		int num = ts[tIdx];
		int num2 = ts[tIdx + 1];
		int num3 = ts[tIdx + 2];
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		List<int> list3 = trianglesPerVertex[uniqueVs[num]];
		for (int i = 0; i < list3.Count; i++)
		{
			list2.Add(ts[list3[i] * 3]);
			list2.Add(ts[list3[i] * 3 + 1]);
			list2.Add(ts[list3[i] * 3 + 2]);
			list.Add(list3[i]);
			LODMaker.SetTriangleGroup(tIdx / 3, list3[i], triangleGroups, trianglesPerGroup);
		}
		list3 = trianglesPerVertex[uniqueVs[num2]];
		for (int j = 0; j < list3.Count; j++)
		{
			int k;
			for (k = 0; k < list.Count; k++)
			{
				if (list[k] == list3[j])
				{
					break;
				}
			}
			if (k >= list.Count)
			{
				list2.Add(ts[list3[j] * 3]);
				list2.Add(ts[list3[j] * 3 + 1]);
				list2.Add(ts[list3[j] * 3 + 2]);
				list.Add(list3[j]);
				LODMaker.SetTriangleGroup(tIdx / 3, list3[j], triangleGroups, trianglesPerGroup);
			}
		}
		list3 = trianglesPerVertex[uniqueVs[num3]];
		for (int l = 0; l < list3.Count; l++)
		{
			int m;
			for (m = 0; m < list.Count; m++)
			{
				if (list[m] == list3[l])
				{
					break;
				}
			}
			if (m >= list.Count)
			{
				list2.Add(ts[list3[l] * 3]);
				list2.Add(ts[list3[l] * 3 + 1]);
				list2.Add(ts[list3[l] * 3 + 2]);
				list.Add(list3[l]);
				LODMaker.SetTriangleGroup(tIdx / 3, list3[l], triangleGroups, trianglesPerGroup);
			}
		}
		return list2.ToArray();
	}

	private static void SetTriangleGroup(int tIdx0, int tIdx1, int[] triangleGroups, List<List<int>> trianglesPerGroup)
	{
		if (triangleGroups[tIdx0] < 0 && triangleGroups[tIdx1] < 0)
		{
			triangleGroups[tIdx0] = trianglesPerGroup.Count;
			triangleGroups[tIdx1] = trianglesPerGroup.Count;
			trianglesPerGroup.Add(new List<int>());
			trianglesPerGroup[triangleGroups[tIdx0]].Add(tIdx0);
			trianglesPerGroup[triangleGroups[tIdx0]].Add(tIdx1);
		}
		else if (triangleGroups[tIdx0] < 0 && triangleGroups[tIdx1] >= 0)
		{
			triangleGroups[tIdx0] = triangleGroups[tIdx1];
			trianglesPerGroup[triangleGroups[tIdx1]].Add(tIdx0);
		}
		else if (triangleGroups[tIdx0] >= 0 && triangleGroups[tIdx1] < 0)
		{
			triangleGroups[tIdx1] = triangleGroups[tIdx0];
			trianglesPerGroup[triangleGroups[tIdx0]].Add(tIdx1);
		}
		else if (triangleGroups[tIdx0] != triangleGroups[tIdx1])
		{
			List<int> list = trianglesPerGroup[triangleGroups[tIdx1]];
			trianglesPerGroup[triangleGroups[tIdx0]].AddRange(list);
			trianglesPerGroup[triangleGroups[tIdx1]] = new List<int>();
			for (int i = 0; i < list.Count; i++)
			{
				triangleGroups[list[i]] = triangleGroups[tIdx0];
			}
		}
	}

	private static void GetTotalAngleAndCenterDistanceForCorner(int[] ts, Vector3[] vs, int[] movedVs, int vertexIdx, Vector3[] centerDistances, ref float totalAngle, ref Vector3 totalCenterDist)
	{
		totalAngle = 0f;
		totalCenterDist = Vector3.zero;
		int num = 0;
		for (int i = 0; i < ts.Length; i++)
		{
			if (ts[i] == vertexIdx)
			{
				int num2 = i / 3 * 3;
				int num3 = ts[num2];
				int num4 = ts[num2 + 1];
				int num5 = ts[num2 + 2];
				i = num2 + 2;
				if (num3 != num4 && num4 != num5 && num5 != num3)
				{
					int num6 = vertexIdx;
					int num7 = vertexIdx;
					if (num3 != vertexIdx)
					{
						num6 = num3;
					}
					if (num4 != vertexIdx)
					{
						if (num6 == vertexIdx)
						{
							num6 = num4;
						}
						else
						{
							num7 = ts[num2 + 1];
						}
					}
					if (num5 != vertexIdx)
					{
						num7 = num5;
					}
					totalAngle += Vector3.Angle(vs[movedVs[num6]] - vs[movedVs[vertexIdx]], vs[movedVs[num7]] - vs[movedVs[vertexIdx]]);
				}
				totalCenterDist += centerDistances[num3] + centerDistances[num4] + centerDistances[num5];
				num += 3;
			}
		}
		totalCenterDist /= (float)num;
	}

	private static void GetTotalAngleAndCenterDistanceForNewCorner(int[] ts, Vector3[] vs, int[] movedVs, int[] uniqueVs, int vertexIdx, int newIdx, Vector3[] centerDistances, float maxWeight, ref float totalAngle, ref Vector3 totalCenterDist, ref bool flipsTriangles)
	{
		float num = 0.5f + Mathf.Clamp01(maxWeight * 0.75f);
		totalAngle = 0f;
		totalCenterDist = Vector3.zero;
		int num2 = 0;
		flipsTriangles = false;
		for (int i = 0; i < ts.Length; i++)
		{
			if (ts[i] == vertexIdx || ts[i] == newIdx)
			{
				int num3 = i / 3 * 3;
				int num4 = ts[num3];
				int num5 = ts[num3 + 1];
				int num6 = ts[num3 + 2];
				i = num3 + 2;
				if (num4 != num5 && num5 != num6 && num6 != num4)
				{
					int num7 = vertexIdx;
					int num8 = vertexIdx;
					if (num4 != vertexIdx)
					{
						num7 = num4;
					}
					if (num5 != vertexIdx)
					{
						if (num7 == vertexIdx)
						{
							num7 = num5;
						}
						else
						{
							num8 = ts[num3 + 1];
						}
					}
					if (num6 != vertexIdx)
					{
						num8 = num6;
					}
					Vector3 normalized = Vector3.Cross(vs[movedVs[uniqueVs[num7]]] - vs[movedVs[uniqueVs[vertexIdx]]], vs[movedVs[uniqueVs[num8]]] - vs[movedVs[uniqueVs[vertexIdx]]]).normalized;
					if (num4 == vertexIdx)
					{
						num4 = newIdx;
					}
					if (num5 == vertexIdx)
					{
						num5 = newIdx;
					}
					if (num6 == vertexIdx)
					{
						num6 = newIdx;
					}
					if (num4 != num5 && num5 != num6 && num6 != num4)
					{
						num7 = newIdx;
						num8 = newIdx;
						if (num4 != newIdx)
						{
							num7 = num4;
						}
						if (num5 != newIdx)
						{
							if (num7 == newIdx)
							{
								num7 = num5;
							}
							else
							{
								num8 = ts[num3 + 1];
							}
						}
						if (num6 != newIdx)
						{
							num8 = num6;
						}
						Vector3 normalized2 = Vector3.Cross(vs[movedVs[uniqueVs[num7]]] - vs[movedVs[uniqueVs[newIdx]]], vs[movedVs[uniqueVs[num8]]] - vs[movedVs[uniqueVs[newIdx]]]).normalized;
						if ((normalized2 + normalized).magnitude < num)
						{
							flipsTriangles = true;
						}
						totalAngle += Vector3.Angle(vs[movedVs[uniqueVs[num7]]] - vs[movedVs[uniqueVs[newIdx]]], vs[movedVs[uniqueVs[num8]]] - vs[movedVs[uniqueVs[newIdx]]]);
					}
				}
				totalCenterDist = totalCenterDist + centerDistances[num4] + centerDistances[num5] + centerDistances[num6];
				num2 += 3;
			}
		}
		totalCenterDist /= (float)num2;
	}

	private static void GetUVStretchAndAreaForCorner(int[] ts, Vector3[] vs, int[] movedVs, int[] uniqueVs, Vector2[] uvs, int cFrom, int cTo, ref float affectedUvAreaDiff, ref float affectedAreaDiff, ref float totalUvAreaDiff, ref float totalAreaDiff)
	{
		float num = 0f;
		int num2 = 0;
		float num3 = 0f;
		float num4 = 0f;
		float num5 = 0f;
		float num6 = 0f;
		float num7 = 0f;
		float num8 = 0f;
		float num9 = 0f;
		float num10 = 0f;
		int num11 = uniqueVs[cFrom];
		int num12 = uniqueVs[cTo];
		affectedUvAreaDiff = 0f;
		affectedAreaDiff = 0f;
		int num13 = 0;
		int num14 = 0;
		int num15 = 0;
		float num16 = 0f;
		float num17 = 0f;
		int i = 0;
		while (i < ts.Length)
		{
			if (i % 3 != 0)
			{
				goto IL_151;
			}
			num13 = ts[i];
			num14 = ts[i + 1];
			num15 = ts[i + 2];
			if (num13 != num14 && num14 != num15 && num15 != num13)
			{
				num16 = LODMaker.Area(vs[movedVs[num13]], vs[movedVs[num14]], vs[movedVs[num15]]);
				num7 += num16;
				if (uvs != null && uvs.Length > 0)
				{
					num17 = LODMaker.Area(uvs[num13], uvs[num14], uvs[num15]);
				}
				num5 += num17;
				goto IL_151;
			}
			i += 2;
			IL_1C0:
			i++;
			continue;
			IL_151:
			if (uniqueVs[ts[i]] == num11)
			{
				int num18 = 1;
				if (uniqueVs[num13] == num12 || uniqueVs[num14] == num12 || uniqueVs[num15] == num12)
				{
					num18++;
				}
				if (uvs != null && uvs.Length > 0 && num16 > 0f && num18 < 2)
				{
					num += num17 / num16;
					num2++;
				}
				num9 += num16;
				goto IL_1C0;
			}
			goto IL_1C0;
		}
		if (num2 > 0 && num > 0f)
		{
			num3 = num / (float)num2;
		}
		num = 0f;
		num2 = 0;
		int j = 0;
		while (j < ts.Length)
		{
			if (j % 3 != 0)
			{
				goto IL_2FC;
			}
			num13 = ts[j];
			num14 = ts[j + 1];
			num15 = ts[j + 2];
			if (uniqueVs[num13] == num11)
			{
				num13 = cTo;
			}
			if (uniqueVs[num14] == num11)
			{
				num14 = cTo;
			}
			if (uniqueVs[num15] == num11)
			{
				num15 = cTo;
			}
			if (num13 != num14 && num14 != num15 && num15 != num13)
			{
				num16 = LODMaker.Area(vs[movedVs[num13]], vs[movedVs[num14]], vs[movedVs[num15]]);
				num8 += num16;
				if (uvs != null && uvs.Length > 0)
				{
					num17 = LODMaker.Area(uvs[num13], uvs[num14], uvs[num15]);
				}
				num6 += num17;
				goto IL_2FC;
			}
			j += 2;
			IL_363:
			j++;
			continue;
			IL_2FC:
			if (uniqueVs[ts[j]] == num11)
			{
				int num19 = 1;
				if (uniqueVs[num13] == num12 || uniqueVs[num14] == num12 || uniqueVs[num15] == num12)
				{
					num19++;
				}
				if (uvs != null && uvs.Length > 0 && num16 > 0f)
				{
					num += num17 / num16;
					num2++;
				}
				num10 += num16;
				goto IL_363;
			}
			goto IL_363;
		}
		affectedAreaDiff = Mathf.Abs(num10 - num9);
		totalAreaDiff = num8 - num7;
		totalUvAreaDiff = num6 - num5;
		if (num2 > 0 && num > 0f)
		{
			num4 = num / (float)num2;
		}
		float num20 = Mathf.Abs(num4 - num3);
		if (num20 > 0f)
		{
			affectedUvAreaDiff = Mathf.Sqrt(num20) * num9;
		}
	}

	private static float GetNormalDiffForCorners(Vector3[] ns, int corner1, int corner2)
	{
		if (ns == null || ns.Length <= 0)
		{
			return 0f;
		}
		return Vector3.Angle(ns[corner1], ns[corner2]) / 180f;
	}

	private static void MergeVertices(ref int oldV, int newV, bool[] hasTwinVS, Vector3[] vs, int[] triangles, Vector2[] uv1s, Vector2[] uv2s, Vector2[] uv3s, Vector2[] uv4s, Color32[] colors32, bool[] deletedVertices, int[] movedVs, int[] uniqueVs, int[] movedUv1s, int[] movedUv2s, int[] movedUv3s, int[] movedUv4s, int[] movedColors, List<List<int>> trianglesPerVertex, bool logYN)
	{
		if (oldV == newV)
		{
			return;
		}
		deletedVertices[oldV] = true;
		int num = uniqueVs[oldV];
		int num2 = uniqueVs[newV];
		List<int> list = trianglesPerVertex[num];
		for (int i = 0; i < list.Count; i++)
		{
			int num3 = list[i] * 3;
			for (int j = 0; j < 3; j++)
			{
				if (triangles[num3 + j] == oldV)
				{
					triangles[num3 + j] = newV;
				}
			}
		}
		if (num != num2)
		{
			trianglesPerVertex[num2].AddRange(trianglesPerVertex[num]);
			trianglesPerVertex[num].Clear();
		}
		if (hasTwinVS[oldV] || hasTwinVS[movedVs[oldV]])
		{
			LODMaker.MoveVertex(oldV, newV, movedVs, uniqueVs, movedUv1s, movedUv2s, movedUv3s, movedUv4s, movedColors);
			for (int k = 0; k < vs.Length; k++)
			{
				if (k != oldV && vs[oldV].x == vs[movedVs[k]].x && vs[oldV].y == vs[movedVs[k]].y && vs[oldV].z == vs[movedVs[k]].z)
				{
					LODMaker.MoveVertex(k, newV, movedVs, uniqueVs, movedUv1s, movedUv2s, movedUv3s, movedUv4s, movedColors);
				}
			}
		}
		oldV = newV;
	}

	private static void MoveVertex(int oldV, int newV, int[] movedVs, int[] uniqueVs, int[] movedUv1s, int[] movedUv2s, int[] movedUv3s, int[] movedUv4s, int[] movedColors)
	{
		movedVs[oldV] = movedVs[newV];
		movedVs[movedVs[oldV]] = movedVs[newV];
		movedVs[uniqueVs[oldV]] = movedVs[newV];
		if (movedUv1s.Length > 0)
		{
			movedUv1s[oldV] = movedUv1s[newV];
			movedUv1s[movedVs[oldV]] = movedUv1s[newV];
			movedUv1s[uniqueVs[oldV]] = movedUv1s[newV];
		}
		if (movedUv2s.Length > 0)
		{
			movedUv2s[oldV] = movedUv2s[newV];
			movedUv2s[movedVs[oldV]] = movedUv2s[newV];
			movedUv2s[uniqueVs[oldV]] = movedUv2s[newV];
		}
		if (movedUv3s.Length > 0)
		{
			movedUv3s[oldV] = movedUv3s[newV];
			movedUv3s[movedVs[oldV]] = movedUv3s[newV];
			movedUv3s[uniqueVs[oldV]] = movedUv3s[newV];
		}
		if (movedUv4s.Length > 0)
		{
			movedUv4s[oldV] = movedUv4s[newV];
			movedUv4s[movedVs[oldV]] = movedUv4s[newV];
			movedUv4s[uniqueVs[oldV]] = movedUv4s[newV];
		}
		if (movedColors.Length > 0)
		{
			movedColors[oldV] = movedColors[newV];
			movedColors[movedVs[oldV]] = movedColors[newV];
			movedColors[uniqueVs[oldV]] = movedColors[newV];
		}
	}

	private static void FillNewMeshArray(Vector3[] vs, bool[] vdel, int[] movedVs, Vector3[] ns, Vector2[] uv1s, int[] movedUv1s, Vector2[] uv2s, int[] movedUv2s, Vector2[] uv3s, int[] movedUv3s, Vector2[] uv4s, int[] movedUv4s, Color32[] colors32, int[] movedColors, BoneWeight[] bws, List<Vector3> newVs, List<Vector3> newNs, List<Vector2> newUv1s, List<Vector2> newUv2s, List<Vector2> newUv3s, List<Vector2> newUv4s, List<Color32> newColors32, List<BoneWeight> newBws, int[] o2n)
	{
		bool flag = ns != null && ns.Length > 0;
		bool flag2 = false;
		int num = 0;
		while (uv1s != null && num < uv1s.Length)
		{
			if (uv1s[num].x != 0f || uv1s[num].y != 0f)
			{
				flag2 = true;
				break;
			}
			num++;
		}
		bool flag3 = false;
		int num2 = 0;
		while (uv2s != null && num2 < uv2s.Length)
		{
			if (uv2s[num2].x != 0f || uv2s[num2].y != 0f)
			{
				flag3 = true;
				break;
			}
			num2++;
		}
		bool flag4 = false;
		int num3 = 0;
		while (uv3s != null && num3 < uv3s.Length)
		{
			if (uv3s[num3].x != 0f || uv3s[num3].y != 0f)
			{
				flag4 = true;
				break;
			}
			num3++;
		}
		bool flag5 = false;
		int num4 = 0;
		while (uv4s != null && num4 < uv4s.Length)
		{
			if (uv4s[num4].x != 0f || uv4s[num4].y != 0f)
			{
				flag5 = true;
				break;
			}
			num4++;
		}
		bool flag6 = false;
		int num5 = 0;
		while (colors32 != null && num5 < colors32.Length)
		{
			if (colors32[num5].r > 0 || colors32[num5].g > 0 || colors32[num5].b > 0)
			{
				flag6 = true;
				break;
			}
			num5++;
		}
		bool flag7 = bws != null && bws.Length > 0;
		int num6 = 0;
		for (int i = 0; i < vs.Length; i++)
		{
			if (!vdel[i])
			{
				newVs.Add(vs[movedVs[i]]);
				if (flag)
				{
					newNs.Add(ns[i]);
				}
				if (flag2)
				{
					newUv1s.Add(uv1s[movedUv1s[i]]);
				}
				if (flag3)
				{
					newUv2s.Add(uv2s[movedUv2s[i]]);
				}
				if (flag4)
				{
					newUv3s.Add(uv3s[movedUv3s[i]]);
				}
				if (flag5)
				{
					newUv4s.Add(uv4s[movedUv4s[i]]);
				}
				if (flag6)
				{
					newColors32.Add(colors32[movedColors[i]]);
				}
				if (flag7)
				{
					newBws.Add(bws[i]);
				}
				o2n[i] = num6;
				num6++;
			}
			else
			{
				o2n[i] = -1;
			}
		}
	}

	private static void FillNewMeshTriangles(int[] oldTriangles, int[] o2n, List<int> newTriangles, int[] subMeshOffsets, int[] triangleGroups, List<int> newTGrps)
	{
		int num = -1;
		for (int i = 0; i < oldTriangles.Length; i += 3)
		{
			int num2 = oldTriangles[i];
			int num3 = oldTriangles[i + 1];
			int num4 = oldTriangles[i + 2];
			while (num + 1 < subMeshOffsets.Length && i == subMeshOffsets[num + 1])
			{
				num++;
				subMeshOffsets[num] = newTriangles.Count;
			}
			if (o2n[num2] >= 0 && o2n[num3] >= 0 && o2n[num4] >= 0 && o2n[num2] != o2n[num3] && o2n[num3] != o2n[num4] && o2n[num4] != o2n[num2])
			{
				newTriangles.Add(o2n[num2]);
				newTriangles.Add(o2n[num3]);
				newTriangles.Add(o2n[num4]);
				newTGrps.Add(triangleGroups[i / 3]);
			}
		}
		while (num + 1 < subMeshOffsets.Length)
		{
			num++;
			subMeshOffsets[num] = newTriangles.Count;
		}
	}

	public static void RemoveUnusedVertices(List<Vector3> vs, List<Vector3> ns, List<Vector2> uv1s, List<Vector2> uv2s, List<Vector2> uv3s, List<Vector2> uv4s, List<Color32> colors32, List<BoneWeight> bws, List<int> ts)
	{
		List<List<int>> list = new List<List<int>>();
		for (int i = 0; i < vs.Count; i++)
		{
			list.Add(new List<int>());
		}
		for (int j = 0; j < ts.Count; j++)
		{
			list[ts[j]].Add(j);
		}
		bool flag = ns != null && ns.Count > 0;
		bool flag2 = uv1s != null && uv1s.Count > 0;
		bool flag3 = uv2s != null && uv2s.Count > 0;
		bool flag4 = uv3s != null && uv3s.Count > 0;
		bool flag5 = uv4s != null && uv4s.Count > 0;
		bool flag6 = colors32 != null && colors32.Count > 0;
		bool flag7 = bws != null && bws.Count > 0;
		int num = 0;
		for (int k = 0; k < vs.Count; k++)
		{
			List<int> list2 = list[k];
			if (list2.Count > 0)
			{
				if (num > 0)
				{
					for (int l = 0; l < list2.Count; l++)
					{
						int num2;
						int expr_127 = num2 = list2[l];
						num2 = ts[num2];
						ts[expr_127] = num2 - num;
					}
				}
			}
			else
			{
				vs.RemoveAt(k);
				list.RemoveAt(k);
				if (flag)
				{
					ns.RemoveAt(k);
				}
				if (flag2)
				{
					uv1s.RemoveAt(k);
				}
				if (flag3)
				{
					uv2s.RemoveAt(k);
				}
				if (flag4)
				{
					uv3s.RemoveAt(k);
				}
				if (flag5)
				{
					uv4s.RemoveAt(k);
				}
				if (flag6)
				{
					colors32.RemoveAt(k);
				}
				if (flag7)
				{
					bws.RemoveAt(k);
				}
				num++;
				k--;
			}
		}
	}

	public static void RemoveUnusedVertices(List<Vector3> vs, List<Vector3> ns, List<Vector2> uv1s, List<Vector2> uv2s, List<Vector2> uv3s, List<Vector2> uv4s, List<Color32> colors32, List<BoneWeight> bws, List<List<int>> subMeshes)
	{
		List<List<int>> list = new List<List<int>>();
		List<List<int>> list2 = new List<List<int>>();
		for (int i = 0; i < vs.Count; i++)
		{
			list.Add(new List<int>());
			list2.Add(new List<int>());
		}
		for (int j = 0; j < subMeshes.Count; j++)
		{
			List<int> list3 = subMeshes[j];
			for (int k = 0; k < list3.Count; k++)
			{
				list[list3[k]].Add(j);
				list2[list3[k]].Add(k);
			}
		}
		bool flag = ns != null && ns.Count > 0;
		bool flag2 = uv1s != null && uv1s.Count > 0;
		bool flag3 = uv2s != null && uv2s.Count > 0;
		bool flag4 = uv3s != null && uv3s.Count > 0;
		bool flag5 = uv4s != null && uv4s.Count > 0;
		bool flag6 = colors32 != null && colors32.Count > 0;
		bool flag7 = bws != null && bws.Count > 0;
		int num = 0;
		for (int l = 0; l < vs.Count; l++)
		{
			List<int> list4 = list[l];
			List<int> list5 = list2[l];
			if (list5.Count > 0)
			{
				if (num > 0)
				{
					for (int m = 0; m < list4.Count; m++)
					{
						List<int> list6;
						List<int> expr_182 = list6 = subMeshes[list4[m]];
						int num2;
						int expr_18E = num2 = list5[m];
						num2 = list6[num2];
						expr_182[expr_18E] = num2 - num;
					}
				}
			}
			else
			{
				vs.RemoveAt(l);
				list.RemoveAt(l);
				list2.RemoveAt(l);
				if (flag)
				{
					ns.RemoveAt(l);
				}
				if (flag2)
				{
					uv1s.RemoveAt(l);
				}
				if (flag3)
				{
					uv2s.RemoveAt(l);
				}
				if (flag4)
				{
					uv3s.RemoveAt(l);
				}
				if (flag5)
				{
					uv4s.RemoveAt(l);
				}
				if (flag6)
				{
					colors32.RemoveAt(l);
				}
				if (flag7)
				{
					bws.RemoveAt(l);
				}
				num++;
				l--;
			}
		}
	}

	public static void RemoveUnusedVertices(List<Vector3> vs, List<Vector3> ns, List<Vector2> uv1s, List<Vector2> uv2s, List<Vector2> uv3s, List<Vector2> uv4s, List<Color32> colors32, List<BoneWeight> bws, Dictionary<Material, List<int>> subMeshes)
	{
		List<List<Material>> list = new List<List<Material>>();
		List<List<int>> list2 = new List<List<int>>();
		for (int i = 0; i < vs.Count; i++)
		{
			list.Add(new List<Material>());
			list2.Add(new List<int>());
		}
		foreach (Material current in subMeshes.Keys)
		{
			List<int> list3 = subMeshes[current];
			for (int j = 0; j < list3.Count; j++)
			{
				list[list3[j]].Add(current);
				list2[list3[j]].Add(j);
			}
		}
		bool flag = ns != null && ns.Count > 0;
		bool flag2 = uv1s != null && uv1s.Count > 0;
		bool flag3 = uv2s != null && uv2s.Count > 0;
		bool flag4 = uv3s != null && uv3s.Count > 0;
		bool flag5 = uv4s != null && uv4s.Count > 0;
		bool flag6 = colors32 != null && colors32.Count > 0;
		bool flag7 = bws != null && bws.Count > 0;
		int num = 0;
		for (int k = 0; k < vs.Count; k++)
		{
			List<Material> list4 = list[k];
			List<int> list5 = list2[k];
			if (list5.Count > 0)
			{
				if (num > 0)
				{
					for (int l = 0; l < list4.Count; l++)
					{
						List<int> list6;
						List<int> expr_1A3 = list6 = subMeshes[list4[l]];
						int num2;
						int expr_1AF = num2 = list5[l];
						num2 = list6[num2];
						expr_1A3[expr_1AF] = num2 - num;
					}
				}
			}
			else
			{
				vs.RemoveAt(k);
				list.RemoveAt(k);
				list2.RemoveAt(k);
				if (flag)
				{
					ns.RemoveAt(k);
				}
				if (flag2)
				{
					uv1s.RemoveAt(k);
				}
				if (flag3)
				{
					uv2s.RemoveAt(k);
				}
				if (flag4)
				{
					uv3s.RemoveAt(k);
				}
				if (flag5)
				{
					uv4s.RemoveAt(k);
				}
				if (flag6)
				{
					colors32.RemoveAt(k);
				}
				if (flag7)
				{
					bws.RemoveAt(k);
				}
				num++;
				k--;
			}
		}
	}

	private static void RemoveEmptyTriangles(List<Vector3> newVs, List<Vector3> newNs, List<Vector2> newUv1s, List<Vector2> newUv2s, List<Vector2> newUv3s, List<Vector2> newUv4s, List<Color32> newColors32, List<int> newTs, List<BoneWeight> newBws, int[] subMeshOffsets, List<int> newTGrps)
	{
		int num = subMeshOffsets.Length - 1;
		bool[] array = new bool[newVs.Count];
		for (int i = newTs.Count - 3; i >= 0; i -= 3)
		{
			while (num > 0 && i + 3 == subMeshOffsets[num])
			{
				num--;
			}
			if (LODMaker.Area(newVs[newTs[i]], newVs[newTs[i + 1]], newVs[newTs[i + 2]]) <= 0f)
			{
				newTs.RemoveAt(i + 2);
				newTs.RemoveAt(i + 1);
				newTs.RemoveAt(i);
				newTGrps.RemoveAt(i / 3);
				for (int j = num + 1; j < subMeshOffsets.Length; j++)
				{
					subMeshOffsets[j] -= 3;
				}
			}
			else
			{
				array[newTs[i]] = true;
				array[newTs[i + 1]] = true;
				array[newTs[i + 2]] = true;
			}
		}
		bool flag = newNs != null && newNs.Count > 0;
		bool flag2 = newUv1s != null && newUv1s.Count > 0;
		bool flag3 = newUv2s != null && newUv2s.Count > 0;
		bool flag4 = newUv3s != null && newUv3s.Count > 0;
		bool flag5 = newUv4s != null && newUv4s.Count > 0;
		bool flag6 = newColors32 != null && newColors32.Count > 0;
		bool flag7 = newBws != null && newBws.Count > 0;
		List<int> list = new List<int>();
		for (int k = array.Length - 1; k >= 0; k--)
		{
			if (!array[k])
			{
				newVs.RemoveAt(k);
				if (flag)
				{
					newNs.RemoveAt(k);
				}
				if (flag2)
				{
					newUv1s.RemoveAt(k);
				}
				if (flag3)
				{
					newUv2s.RemoveAt(k);
				}
				if (flag4)
				{
					newUv3s.RemoveAt(k);
				}
				if (flag5)
				{
					newUv4s.RemoveAt(k);
				}
				if (flag6)
				{
					newColors32.RemoveAt(k);
				}
				if (flag7)
				{
					newBws.RemoveAt(k);
				}
				list.Add(k);
			}
		}
		for (int l = 0; l < newTs.Count; l++)
		{
			int num2 = newTs[l];
			int num3 = 0;
			for (int m = list.Count - 1; m >= 0; m--)
			{
				if (num2 < list[m])
				{
					break;
				}
				num3++;
			}
			if (num3 > 0)
			{
				newTs[l] = num2 - num3;
			}
		}
	}

	private static void RemoveMiniTriangleGroups(float removeSmallParts, Vector3 sizeMultiplier, float aMaxWeight, List<Vector3> newVs, List<int> newTs, int[] subMeshOffsets, List<int> newTGrps)
	{
		float num = (aMaxWeight * 0.5f < 1f) ? Mathf.Pow(aMaxWeight * 0.5f, 3f) : (aMaxWeight * 0.5f);
		float num2 = 0f;
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		List<float> list3 = new List<float>();
		for (int i = 0; i < newTGrps.Count; i++)
		{
			int num3 = newTGrps[i];
			float num4 = LODMaker.Area(newVs[newTs[i * 3]].Product(sizeMultiplier), newVs[newTs[i * 3 + 1]].Product(sizeMultiplier), newVs[newTs[i * 3 + 2]].Product(sizeMultiplier));
			int j;
			for (j = 0; j < list.Count; j++)
			{
				if (list[j] == num3 && num3 >= 0)
				{
					break;
				}
			}
			if (j >= list.Count)
			{
				list.Add(num3);
				list3.Add(0f);
				list2.Add(0);
			}
			list3[j] += num4;
			list2[j]++;
			num2 += num4;
		}
		removeSmallParts = Mathf.Clamp(removeSmallParts, 0f, 5f) * 0.0028f * num;
		for (int k = 0; k < list.Count; k++)
		{
			if (list3[k] / Mathf.Pow((float)list2[k], 0.33f) / num2 < removeSmallParts)
			{
				int num5 = list[k];
				for (int l = newTGrps.Count - 1; l >= 0; l--)
				{
					if (newTGrps[l] == num5)
					{
						newTs.RemoveAt(l * 3);
						newTs.RemoveAt(l * 3);
						newTs.RemoveAt(l * 3);
						newTGrps.RemoveAt(l);
						for (int m = 0; m < subMeshOffsets.Length; m++)
						{
							if (subMeshOffsets[m] > l * 3)
							{
								subMeshOffsets[m] -= 3;
							}
						}
					}
				}
			}
		}
	}

	public static Mesh CreateNewMesh(Vector3[] vs, Vector3[] ns, Vector2[] uv1s, Vector2[] uv2s, Vector2[] uv3s, Vector2[] uv4s, Color32[] colors32, int[] ts, BoneWeight[] bws, Matrix4x4[] bindposes, int[] subMeshOffsets, bool recalcNormals)
	{
		Mesh mesh = new Mesh();
		LODMaker.FillMesh(mesh, vs, ns, uv1s, uv2s, uv3s, uv4s, colors32, ts, bws, bindposes, subMeshOffsets, recalcNormals);
		return mesh;
	}

	public static void FillMesh(Mesh mesh, Vector3[] vs, Vector3[] ns, Vector2[] uv1s, Vector2[] uv2s, Vector2[] uv3s, Vector2[] uv4s, Color32[] colors32, int[] ts, BoneWeight[] bws, Matrix4x4[] bindposes, int[] subMeshOffsets, bool recalcNormals)
	{
		mesh.vertices = vs;
		if (ns != null && ns.Length > 0)
		{
			mesh.normals = ns;
		}
		if (uv1s != null && uv1s.Length > 0)
		{
			mesh.uv = uv1s;
		}
		if (uv2s != null && uv2s.Length > 0)
		{
			mesh.uv2 = uv2s;
		}
		if (uv3s != null && uv2s.Length > 0)
		{
			mesh.uv3 = uv3s;
		}
		if (uv4s != null && uv2s.Length > 0)
		{
			mesh.uv4 = uv4s;
		}
		if (colors32 != null && colors32.Length > 0)
		{
			mesh.colors32 = colors32;
		}
		if (bws != null && bws.Length > 0)
		{
			mesh.boneWeights = bws;
		}
		if (bindposes != null && bindposes.Length > 0)
		{
			mesh.bindposes = bindposes;
		}
		if (subMeshOffsets.Length == 1)
		{
			mesh.triangles = ts;
		}
		else
		{
			mesh.subMeshCount = subMeshOffsets.Length;
			for (int i = 0; i < subMeshOffsets.Length; i++)
			{
				subMeshOffsets[i] = Mathf.Max(0, subMeshOffsets[i]);
				int num = (i + 1 >= subMeshOffsets.Length) ? ts.Length : subMeshOffsets[i + 1];
				if (num - subMeshOffsets[i] > 0)
				{
					int[] array = new int[num - subMeshOffsets[i]];
					Array.Copy(ts, subMeshOffsets[i], array, 0, num - subMeshOffsets[i]);
					mesh.SetTriangles(array, i);
				}
				else
				{
					mesh.SetTriangles(null, i);
				}
			}
		}
		if (recalcNormals || mesh.normals == null || mesh.normals.Length <= 0)
		{
			mesh.RecalculateNormals();
		}
		mesh.RecalculateTangents();
	}

	private static float AngleCornerDiff(float angle)
	{
		angle = Mathf.Abs(angle.To180Angle());
		float num = Mathf.Min(angle, Mathf.Min(Mathf.Abs((180f - angle).To180Angle()), Mathf.Abs((90f - angle).To180Angle())));
		return num * num * 10f;
	}

	private static float AngleDiff(float angle)
	{
		angle = Mathf.Abs(angle.To180Angle());
		float num = Mathf.Min(angle, Mathf.Abs((180f - angle).To180Angle()));
		return num * num;
	}

	private static float Area(Vector3 p0, Vector3 p1, Vector3 p2)
	{
		return (p1 - p0).magnitude * (Mathf.Sin(Vector3.Angle(p1 - p0, p2 - p0) * 0.0174532924f) * (p2 - p0).magnitude) * 0.5f;
	}

	private static int GetVertexEqualTo(Vector3 v, List<int> orderedVertices, Vector3[] vs)
	{
		int num = orderedVertices.Count / 2;
		int num2 = num;
		int num3 = 1;
		int num4 = 0;
		while (num >= 0 && num < orderedVertices.Count)
		{
			Vector3 vector = vs[orderedVertices[num]];
			int num5 = num2;
			int num6 = num3;
			num2 = Mathf.Max(num5 / 2, 1);
			if (vector.y < v.y)
			{
				num3 = 1;
			}
			else if (vector.y > v.y)
			{
				num3 = -1;
			}
			else if (vector.z < v.z)
			{
				num3 = 1;
			}
			else if (vector.z > v.z)
			{
				num3 = -1;
			}
			else if (vector.x < v.x)
			{
				num3 = 1;
			}
			else
			{
				if (vector.x <= v.x)
				{
					while (num >= 0 && vs[orderedVertices[num]].IsEqual(v))
					{
						num--;
					}
					return num + 1;
				}
				num3 = -1;
			}
			num += num3 * num2;
			if (num3 != num6 && num2 == 1 && num5 == 1 && !vs[orderedVertices[num]].IsEqual(v))
			{
				break;
			}
			if (++num4 > orderedVertices.Count)
			{
				break;
			}
		}
		return -1;
	}

	private static List<int> GetVerticesEqualTo(Vector3 v, List<int> orderedVertices, Vector3[] vs)
	{
		List<int> list = new List<int>();
		int num = LODMaker.GetVertexEqualTo(v, orderedVertices, vs);
		while (num >= 0 && num < orderedVertices.Count && vs[orderedVertices[num]].IsEqual(v))
		{
			list.Add(orderedVertices[num]);
			num++;
		}
		return list;
	}

	private static List<int> GetVerticesWithinBox(Vector3 from, Vector3 to, List<int> orderedVertices, Vector3[] vs)
	{
		List<int> list = new List<int>();
		int num = LODMaker.GetLastVertexWithYSmaller(from.y, orderedVertices, vs, orderedVertices.Count);
		if (num < 0)
		{
			num = 0;
		}
		while (num < orderedVertices.Count && vs[orderedVertices[num]].y <= to.y)
		{
			if (vs[orderedVertices[num]].y >= from.y && vs[orderedVertices[num]].x >= from.x && vs[orderedVertices[num]].x <= to.x && vs[orderedVertices[num]].z >= from.z && vs[orderedVertices[num]].z <= to.z)
			{
				list.Add(orderedVertices[num]);
			}
			num++;
		}
		return list;
	}

	private static int GetLastVertexWithYSmaller(float y, List<int> orderedVertices, Vector3[] vs, int limitSearchRange)
	{
		int i = Mathf.Min(orderedVertices.Count, limitSearchRange) / 2;
		int num = i;
		int num2 = 1;
		while (i >= 0 && i < limitSearchRange && i < orderedVertices.Count)
		{
			int num3 = orderedVertices[i];
			Vector3 vector = vs[num3];
			int num4 = num;
			int num5 = num2;
			num = Mathf.Max(num4 / 2, 1);
			if (vector.y < y)
			{
				if ((num5 == -1 && num4 == 1) || num4 == 0)
				{
					i++;
					while (i < limitSearchRange && i < orderedVertices.Count)
					{
						num3 = orderedVertices[i];
						if (vs[num3].y >= y)
						{
							break;
						}
						i++;
					}
					return i - 1;
				}
				num2 = 1;
			}
			else
			{
				if (vector.y <= y)
				{
					for (i--; i >= 0; i--)
					{
						num3 = orderedVertices[i];
						if (vs[num3].y < y)
						{
							break;
						}
					}
					return i;
				}
				num2 = -1;
			}
			i += num2 * num;
		}
		return -1;
	}

	private static bool IsVertexObscured(Vector3[] vs, Vector3[] ns, int[] ts, bool[] vObscured, int[] uniqueVs, Vector3 vertexBoxSize, List<int> orderedVertices, List<List<int>> trianglesPerVertex, int[] subMeshIdxPerVertex, float maxObscureDist, bool hiddenByOtherSubmesh, Vector3 vertex, Vector3 normal, int i)
	{
		List<int> verticesWithinBox = LODMaker.GetVerticesWithinBox(vertex - vertexBoxSize, vertex + vertexBoxSize, orderedVertices, vs);
		for (int j = 0; j < verticesWithinBox.Count; j++)
		{
			if (verticesWithinBox[j] != i)
			{
				List<int> list = trianglesPerVertex[verticesWithinBox[j]];
				for (int k = 0; k < list.Count; k++)
				{
					int num = list[k] * 3;
					if (vObscured == null || !vObscured[uniqueVs[ts[num]]] || !vObscured[uniqueVs[ts[num + 1]]] || !vObscured[uniqueVs[ts[num + 2]]])
					{
						if (ts[num] != i && ts[num + 1] != i && ts[num + 2] != i)
						{
							if (!hiddenByOtherSubmesh || subMeshIdxPerVertex[ts[num]] != subMeshIdxPerVertex[i])
							{
								Vector3 normalized = (ns[ts[num]] + ns[ts[num + 1]] + ns[ts[num + 2]]).normalized;
								float num2 = Vector3.Angle(normal, normalized);
								if (num2 < 60f)
								{
									float num3 = LODMaker.FindCollision(vertex, normal, vs[ts[num]], normalized);
									if (num3 > 0f && num3 < maxObscureDist)
									{
										Vector3 p = vertex + normal * num3;
										Vector2 v = p.Barycentric(vs[ts[num]], vs[ts[num + 1]], vs[ts[num + 2]]);
										if (v.IsBarycentricInTriangle())
										{
											if (vObscured != null)
											{
												vObscured[uniqueVs[i]] = true;
											}
											return true;
										}
									}
								}
							}
						}
					}
				}
			}
		}
		return false;
	}

	public static float FindCollision(Vector3 fromPos, Vector3 direction, Vector3 pointOnPlane, Vector3 normalPlane)
	{
		float result = float.PositiveInfinity;
		float num = direction.InProduct(normalPlane);
		if (num != 0f)
		{
			result = (pointOnPlane - fromPos).InProduct(normalPlane) / direction.InProduct(normalPlane);
		}
		return result;
	}

	private static void LogVectors(string msg, int[] idxs, Vector3[] vs, int decimals)
	{
		string text = string.Concat(new object[]
		{
			msg,
			" (",
			idxs.Length,
			"):\n"
		});
		for (int i = 0; i < idxs.Length; i++)
		{
			text = string.Concat(new object[]
			{
				text,
				string.Empty,
				i,
				": ",
				vs[idxs[i]].MakeString(decimals),
				"\n"
			});
		}
		LODMaker.Log(text);
	}

	private static void LogArray(string msg, List<Vector3> vs, int decimals)
	{
		string text = string.Concat(new object[]
		{
			msg,
			" (",
			vs.Count,
			"):\n"
		});
		for (int i = 0; i < vs.Count; i++)
		{
			text = string.Concat(new object[]
			{
				text,
				string.Empty,
				i,
				": ",
				vs[i].MakeString(decimals),
				"\n"
			});
		}
		LODMaker.Log(text);
	}

	private static void LogArray(string msg, Vector3[] vs, int decimals)
	{
		string text = string.Concat(new object[]
		{
			msg,
			" (",
			vs.Length,
			"):\n"
		});
		for (int i = 0; i < vs.Length; i++)
		{
			text = string.Concat(new object[]
			{
				text,
				string.Empty,
				i,
				": ",
				vs[i].MakeString(decimals),
				"\n"
			});
		}
		LODMaker.Log(text);
	}

	private static void LogArray(string msg, Vector2[] vs, int decimals)
	{
		string text = string.Concat(new object[]
		{
			msg,
			" (",
			vs.Length,
			"):\n"
		});
		for (int i = 0; i < vs.Length; i++)
		{
			text = string.Concat(new object[]
			{
				text,
				string.Empty,
				i,
				": ",
				vs[i].MakeString(decimals),
				"\n"
			});
		}
		LODMaker.Log(text);
	}

	private static void LogTriArray(string msg, List<int> ts)
	{
		string text = string.Concat(new object[]
		{
			msg,
			" (",
			ts.Count,
			"):\n"
		});
		for (int i = 0; i < ts.Count; i += 3)
		{
			text = string.Concat(new object[]
			{
				text,
				ts[i],
				", ",
				ts[i + 1],
				", ",
				ts[i + 2],
				"\n"
			});
		}
		LODMaker.Log(text);
	}

	private static void LogTriArray(string msg, int[] ts)
	{
		string text = string.Concat(new object[]
		{
			msg,
			" (",
			ts.Length,
			"):\n"
		});
		for (int i = 0; i < ts.Length; i += 3)
		{
			text = string.Concat(new object[]
			{
				text,
				ts[i],
				", ",
				ts[i + 1],
				", ",
				ts[i + 2],
				"\n"
			});
		}
		LODMaker.Log(text);
	}

	private static void LogArray(string msg, List<Color32> ts)
	{
		string text = string.Concat(new object[]
		{
			msg,
			" (",
			ts.Count,
			"):\n"
		});
		for (int i = 0; i < ts.Count; i++)
		{
			text = text + ts[i] + "\n";
		}
		LODMaker.Log(text);
	}

	private static void LogArray(string msg, Color32[] ts)
	{
		string text = string.Concat(new object[]
		{
			msg,
			" (",
			ts.Length,
			"):\n"
		});
		for (int i = 0; i < ts.Length; i++)
		{
			text = text + ts[i] + "\n";
		}
		LODMaker.Log(text);
	}

	private static void LogArray(string msg, List<int> ts)
	{
		string text = string.Concat(new object[]
		{
			msg,
			" (",
			ts.Count,
			"):\n"
		});
		for (int i = 0; i < ts.Count; i++)
		{
			text = text + ts[i] + "\n";
		}
		LODMaker.Log(text);
	}

	private static void LogArray(string msg, List<List<int>> ts)
	{
		string text = string.Concat(new object[]
		{
			msg,
			" (",
			ts.Count,
			"):\n"
		});
		for (int i = 0; i < ts.Count; i++)
		{
			List<int> list = ts[i];
			for (int j = 0; j < list.Count; j++)
			{
				text = text + list[j] + ",";
			}
			text += "\n";
		}
		LODMaker.Log(text);
	}

	private static void LogArray(string msg, int[] ts)
	{
		string text = string.Concat(new object[]
		{
			msg,
			" (",
			ts.Length,
			"):\n"
		});
		for (int i = 0; i < ts.Length; i++)
		{
			text = text + ts[i] + "\n";
		}
		LODMaker.Log(text);
	}

	private static void LogArray(string msg, float[] fs)
	{
		string text = string.Concat(new object[]
		{
			msg,
			" (",
			fs.Length,
			"):\n"
		});
		for (int i = 0; i < fs.Length; i++)
		{
			text = text + fs[i] + "\n";
		}
		LODMaker.Log(text);
	}

	private static void LogArray(string msg, bool[] ts)
	{
		string text = string.Concat(new object[]
		{
			msg,
			" (",
			ts.Length,
			"):\n"
		});
		for (int i = 0; i < ts.Length; i += 3)
		{
			text = text + ts[i] + "\n";
		}
		LODMaker.Log(text);
	}

	private static string LogWeights(string msg, float[] w)
	{
		string text = msg + ": ";
		text = string.Concat(new string[]
		{
			text,
			" weight 0-1: ",
			w[0].MakeString(1),
			" / ",
			w[1].MakeString(1)
		});
		text = string.Concat(new string[]
		{
			text,
			" | weight 1>2: ",
			w[2].MakeString(1),
			" / ",
			w[3].MakeString(1)
		});
		return string.Concat(new string[]
		{
			text,
			" | weight 2>0: ",
			w[4].MakeString(1),
			" / ",
			w[5].MakeString(1)
		});
	}

	private static void Log(string msg)
	{
		Debug.Log(msg + "\n" + DateTime.Now.ToString("yyy/MM/dd hh:mm:ss.fff"));
	}
}
