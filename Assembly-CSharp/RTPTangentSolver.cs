using System;
using UnityEngine;

public class RTPTangentSolver
{
	public static void Solve(Mesh theMesh, bool planetFlag = false)
	{
		int vertexCount = theMesh.vertexCount;
		Vector3[] vertices = theMesh.vertices;
		Vector3[] normals = theMesh.normals;
		Vector2[] uv = theMesh.uv;
		int[] triangles = theMesh.triangles;
		Vector4[] array = new Vector4[vertexCount];
		Vector3[] array2 = new Vector3[vertexCount];
		Vector3[] array3 = new Vector3[vertexCount];
		for (int i = 0; i < triangles.Length; i += 3)
		{
			int num = triangles[i];
			int num2 = triangles[i + 1];
			int num3 = triangles[i + 2];
			Vector3 vector = vertices[num];
			Vector3 vector2 = vertices[num2];
			Vector3 vector3 = vertices[num3];
			Vector2 vector4 = uv[num];
			Vector2 vector5 = uv[num2];
			Vector2 vector6 = uv[num3];
			float num4 = vector2.x - vector.x;
			float num5 = vector3.x - vector.x;
			float num6 = vector2.y - vector.y;
			float num7 = vector3.y - vector.y;
			float num8 = vector2.z - vector.z;
			float num9 = vector3.z - vector.z;
			float num10 = vector5.x - vector4.x;
			float num11 = vector6.x - vector4.x;
			float num12 = vector5.y - vector4.y;
			float num13 = vector6.y - vector4.y;
			float num14 = num10 * num13 - num11 * num12;
			float num15 = (num14 != 0f) ? (1f / num14) : 0f;
			Vector3 b = new Vector3((num13 * num4 - num12 * num5) * num15, (num13 * num6 - num12 * num7) * num15, (num13 * num8 - num12 * num9) * num15);
			Vector3 b2 = new Vector3((num10 * num5 - num11 * num4) * num15, (num10 * num7 - num11 * num6) * num15, (num10 * num9 - num11 * num8) * num15);
			array2[num] += b;
			array2[num2] += b;
			array2[num3] += b;
			array3[num] += b2;
			array3[num2] += b2;
			array3[num3] += b2;
		}
		for (int j = 0; j < vertexCount; j++)
		{
			Vector3 lhs;
			if (planetFlag)
			{
				lhs = Vector3.Normalize(vertices[j]);
			}
			else
			{
				lhs = normals[j];
			}
			Vector3 rhs = array2[j];
			Vector3.OrthoNormalize(ref lhs, ref rhs);
			array[j].x = rhs.x;
			array[j].y = rhs.y;
			array[j].z = rhs.z;
			array[j].w = ((Vector3.Dot(Vector3.Cross(lhs, rhs), array3[j]) >= 0f) ? 1f : -1f);
		}
		theMesh.tangents = array;
	}
}
