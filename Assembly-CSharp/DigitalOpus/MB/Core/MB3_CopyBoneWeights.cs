using System;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
	public class MB3_CopyBoneWeights
	{
		public static void CopyBoneWeightsFromSeamMeshToOtherMeshes(float radius, Mesh seamMesh, Mesh[] targetMeshes)
		{
			if (seamMesh == null)
			{
				Debug.LogError(string.Format("The SeamMesh cannot be null", new object[0]));
				return;
			}
			if (seamMesh.vertexCount == 0)
			{
				Debug.LogError("The seam mesh has no vertices. Check that the Asset Importer for the seam mesh does not have 'Optimize Mesh' checked.");
				return;
			}
			Vector3[] vertices = seamMesh.vertices;
			BoneWeight[] boneWeights = seamMesh.boneWeights;
			Debug.Log(string.Format("The seam mesh has {0} vertices.", seamMesh.vertices.Length));
			bool flag = false;
			for (int i = 0; i < targetMeshes.Length; i++)
			{
				if (seamMesh == null)
				{
					Debug.LogError(string.Format("The SeamMesh cannot be null", new object[0]));
					flag = true;
				}
				if (targetMeshes[i] == null)
				{
					Debug.LogError(string.Format("Mesh {0} was null", i));
					flag = true;
				}
				if (radius < 0f)
				{
					Debug.LogError("radius must be zero or positive.");
				}
			}
			if (flag)
			{
				return;
			}
			for (int j = 0; j < targetMeshes.Length; j++)
			{
				Mesh mesh = targetMeshes[j];
				Vector3[] vertices2 = mesh.vertices;
				BoneWeight[] boneWeights2 = mesh.boneWeights;
				int num = 0;
				for (int k = 0; k < vertices2.Length; k++)
				{
					for (int l = 0; l < vertices.Length; l++)
					{
						if (Vector3.Distance(vertices2[k], vertices[l]) <= radius)
						{
							num++;
							boneWeights2[k] = boneWeights[l];
						}
					}
				}
				if (num > 0)
				{
					targetMeshes[j].boneWeights = boneWeights2;
				}
				Debug.Log(string.Format("Copied boneweights for {1} vertices in mesh {0} that matched positions in the seam mesh.", targetMeshes[j].name, num));
			}
			Debug.Log(string.Format("The seam mesh has {0} vertices.", seamMesh.vertices.Length));
		}
	}
}
