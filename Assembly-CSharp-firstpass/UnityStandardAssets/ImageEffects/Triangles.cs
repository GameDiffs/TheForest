using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
	internal class Triangles
	{
		private static Mesh[] meshes;

		private static int currentTris;

		private static bool HasMeshes()
		{
			if (Triangles.meshes == null)
			{
				return false;
			}
			for (int i = 0; i < Triangles.meshes.Length; i++)
			{
				if (null == Triangles.meshes[i])
				{
					return false;
				}
			}
			return true;
		}

		private static void Cleanup()
		{
			if (Triangles.meshes == null)
			{
				return;
			}
			for (int i = 0; i < Triangles.meshes.Length; i++)
			{
				if (null != Triangles.meshes[i])
				{
					UnityEngine.Object.DestroyImmediate(Triangles.meshes[i]);
					Triangles.meshes[i] = null;
				}
			}
			Triangles.meshes = null;
		}

		private static Mesh[] GetMeshes(int totalWidth, int totalHeight)
		{
			if (Triangles.HasMeshes() && Triangles.currentTris == totalWidth * totalHeight)
			{
				return Triangles.meshes;
			}
			int num = 21666;
			int num2 = totalWidth * totalHeight;
			Triangles.currentTris = num2;
			int num3 = Mathf.CeilToInt(1f * (float)num2 / (1f * (float)num));
			Triangles.meshes = new Mesh[num3];
			int num4 = 0;
			for (int i = 0; i < num2; i += num)
			{
				int triCount = Mathf.FloorToInt((float)Mathf.Clamp(num2 - i, 0, num));
				Triangles.meshes[num4] = Triangles.GetMesh(triCount, i, totalWidth, totalHeight);
				num4++;
			}
			return Triangles.meshes;
		}

		private static Mesh GetMesh(int triCount, int triOffset, int totalWidth, int totalHeight)
		{
			Mesh mesh = new Mesh();
			mesh.hideFlags = HideFlags.DontSave;
			Vector3[] array = new Vector3[triCount * 3];
			Vector2[] array2 = new Vector2[triCount * 3];
			Vector2[] array3 = new Vector2[triCount * 3];
			int[] array4 = new int[triCount * 3];
			for (int i = 0; i < triCount; i++)
			{
				int num = i * 3;
				int num2 = triOffset + i;
				float num3 = Mathf.Floor((float)(num2 % totalWidth)) / (float)totalWidth;
				float num4 = Mathf.Floor((float)(num2 / totalWidth)) / (float)totalHeight;
				Vector3 vector = new Vector3(num3 * 2f - 1f, num4 * 2f - 1f, 1f);
				array[num] = vector;
				array[num + 1] = vector;
				array[num + 2] = vector;
				array2[num] = new Vector2(0f, 0f);
				array2[num + 1] = new Vector2(1f, 0f);
				array2[num + 2] = new Vector2(0f, 1f);
				array3[num] = new Vector2(num3, num4);
				array3[num + 1] = new Vector2(num3, num4);
				array3[num + 2] = new Vector2(num3, num4);
				array4[num] = num;
				array4[num + 1] = num + 1;
				array4[num + 2] = num + 2;
			}
			mesh.vertices = array;
			mesh.triangles = array4;
			mesh.uv = array2;
			mesh.uv2 = array3;
			return mesh;
		}
	}
}
