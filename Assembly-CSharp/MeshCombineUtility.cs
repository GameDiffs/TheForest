using System;
using UnityEngine;

public class MeshCombineUtility
{
	public struct MeshInstance
	{
		public Mesh mesh;

		public int subMeshIndex;

		public Matrix4x4 transform;
	}

	public static Mesh Combine(MeshCombineUtility.MeshInstance[] combines, bool generateStrips)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < combines.Length; i++)
		{
			MeshCombineUtility.MeshInstance meshInstance = combines[i];
			if (meshInstance.mesh)
			{
				num += meshInstance.mesh.vertexCount;
				if (generateStrips)
				{
					int num4 = meshInstance.mesh.GetTriangles(meshInstance.subMeshIndex).Length;
					if (num4 != 0)
					{
						if (num3 != 0)
						{
							if ((num3 & 1) == 1)
							{
								num3 += 3;
							}
							else
							{
								num3 += 2;
							}
						}
						num3 += num4;
					}
					else
					{
						generateStrips = false;
					}
				}
			}
		}
		if (!generateStrips)
		{
			for (int j = 0; j < combines.Length; j++)
			{
				MeshCombineUtility.MeshInstance meshInstance2 = combines[j];
				if (meshInstance2.mesh)
				{
					num2 += meshInstance2.mesh.GetTriangles(meshInstance2.subMeshIndex).Length;
				}
			}
		}
		Vector3[] array = new Vector3[num];
		Vector3[] array2 = new Vector3[num];
		Vector4[] array3 = new Vector4[num];
		Vector2[] array4 = new Vector2[num];
		Vector2[] array5 = new Vector2[num];
		Color[] array6 = new Color[num];
		int[] array7 = new int[num2];
		int[] array8 = new int[num3];
		int num5 = 0;
		for (int k = 0; k < combines.Length; k++)
		{
			MeshCombineUtility.MeshInstance meshInstance3 = combines[k];
			if (meshInstance3.mesh)
			{
				MeshCombineUtility.Copy(meshInstance3.mesh.vertexCount, meshInstance3.mesh.vertices, array, ref num5, meshInstance3.transform);
			}
		}
		num5 = 0;
		for (int l = 0; l < combines.Length; l++)
		{
			MeshCombineUtility.MeshInstance meshInstance4 = combines[l];
			if (meshInstance4.mesh)
			{
				Matrix4x4 transform = meshInstance4.transform;
				transform = transform.inverse.transpose;
				MeshCombineUtility.CopyNormal(meshInstance4.mesh.vertexCount, meshInstance4.mesh.normals, array2, ref num5, transform);
			}
		}
		num5 = 0;
		for (int m = 0; m < combines.Length; m++)
		{
			MeshCombineUtility.MeshInstance meshInstance5 = combines[m];
			if (meshInstance5.mesh)
			{
				Matrix4x4 transform2 = meshInstance5.transform;
				transform2 = transform2.inverse.transpose;
				MeshCombineUtility.CopyTangents(meshInstance5.mesh.vertexCount, meshInstance5.mesh.tangents, array3, ref num5, transform2);
			}
		}
		num5 = 0;
		for (int n = 0; n < combines.Length; n++)
		{
			MeshCombineUtility.MeshInstance meshInstance6 = combines[n];
			if (meshInstance6.mesh)
			{
				MeshCombineUtility.Copy(meshInstance6.mesh.vertexCount, meshInstance6.mesh.uv, array4, ref num5);
			}
		}
		num5 = 0;
		for (int num6 = 0; num6 < combines.Length; num6++)
		{
			MeshCombineUtility.MeshInstance meshInstance7 = combines[num6];
			if (meshInstance7.mesh)
			{
				MeshCombineUtility.Copy(meshInstance7.mesh.vertexCount, meshInstance7.mesh.uv2, array5, ref num5);
			}
		}
		num5 = 0;
		for (int num7 = 0; num7 < combines.Length; num7++)
		{
			MeshCombineUtility.MeshInstance meshInstance8 = combines[num7];
			if (meshInstance8.mesh)
			{
				MeshCombineUtility.CopyColors(meshInstance8.mesh.vertexCount, meshInstance8.mesh.colors, array6, ref num5);
			}
		}
		int num8 = 0;
		int num9 = 0;
		int num10 = 0;
		for (int num11 = 0; num11 < combines.Length; num11++)
		{
			MeshCombineUtility.MeshInstance meshInstance9 = combines[num11];
			if (meshInstance9.mesh)
			{
				if (generateStrips)
				{
					int[] triangles = meshInstance9.mesh.GetTriangles(meshInstance9.subMeshIndex);
					if (num9 != 0)
					{
						if ((num9 & 1) == 1)
						{
							array8[num9] = array8[num9 - 1];
							array8[num9 + 1] = triangles[0] + num10;
							array8[num9 + 2] = triangles[0] + num10;
							num9 += 3;
						}
						else
						{
							array8[num9] = array8[num9 - 1];
							array8[num9 + 1] = triangles[0] + num10;
							num9 += 2;
						}
					}
					for (int num12 = 0; num12 < triangles.Length; num12++)
					{
						array8[num12 + num9] = triangles[num12] + num10;
					}
					num9 += triangles.Length;
				}
				else
				{
					int[] triangles2 = meshInstance9.mesh.GetTriangles(meshInstance9.subMeshIndex);
					for (int num13 = 0; num13 < triangles2.Length; num13++)
					{
						array7[num13 + num8] = triangles2[num13] + num10;
					}
					num8 += triangles2.Length;
				}
				num10 += meshInstance9.mesh.vertexCount;
			}
		}
		Mesh mesh = new Mesh();
		mesh.name = "Combined Mesh";
		mesh.vertices = array;
		mesh.normals = array2;
		mesh.colors = array6;
		mesh.uv = array4;
		mesh.uv2 = array5;
		mesh.tangents = array3;
		if (generateStrips)
		{
			mesh.SetTriangles(array8, 0);
		}
		else
		{
			mesh.triangles = array7;
		}
		return mesh;
	}

	private static void Copy(int vertexcount, Vector3[] src, Vector3[] dst, ref int offset, Matrix4x4 transform)
	{
		for (int i = 0; i < src.Length; i++)
		{
			dst[i + offset] = transform.MultiplyPoint(src[i]);
		}
		offset += vertexcount;
	}

	private static void CopyNormal(int vertexcount, Vector3[] src, Vector3[] dst, ref int offset, Matrix4x4 transform)
	{
		for (int i = 0; i < src.Length; i++)
		{
			dst[i + offset] = transform.MultiplyVector(src[i]).normalized;
		}
		offset += vertexcount;
	}

	private static void Copy(int vertexcount, Vector2[] src, Vector2[] dst, ref int offset)
	{
		for (int i = 0; i < src.Length; i++)
		{
			dst[i + offset] = src[i];
		}
		offset += vertexcount;
	}

	private static void CopyColors(int vertexcount, Color[] src, Color[] dst, ref int offset)
	{
		for (int i = 0; i < src.Length; i++)
		{
			dst[i + offset] = src[i];
		}
		offset += vertexcount;
	}

	private static void CopyTangents(int vertexcount, Vector4[] src, Vector4[] dst, ref int offset, Matrix4x4 transform)
	{
		for (int i = 0; i < src.Length; i++)
		{
			Vector4 vector = src[i];
			Vector3 normalized = new Vector3(vector.x, vector.y, vector.z);
			normalized = transform.MultiplyVector(normalized).normalized;
			dst[i + offset] = new Vector4(normalized.x, normalized.y, normalized.z, vector.w);
		}
		offset += vertexcount;
	}
}
