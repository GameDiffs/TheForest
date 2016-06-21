using System;
using System.Collections;
using UnityEngine;

public class FindEdges : MonoBehaviour
{
	public class Edge
	{
		public int[] vertexIndex = new int[2];

		public int[] faceIndex = new int[2];
	}

	public static FindEdges.Edge[] BuildManifoldEdges(Mesh mesh)
	{
		FindEdges.Edge[] array = FindEdges.BuildEdges(mesh.vertexCount, mesh.triangles);
		ArrayList arrayList = new ArrayList();
		FindEdges.Edge[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			FindEdges.Edge edge = array2[i];
			if (edge.faceIndex[0] == edge.faceIndex[1])
			{
				arrayList.Add(edge);
			}
		}
		return arrayList.ToArray(typeof(FindEdges.Edge)) as FindEdges.Edge[];
	}

	public static FindEdges.Edge[] BuildEdges(int vertexCount, int[] triangleArray)
	{
		int num = triangleArray.Length;
		int[] array = new int[vertexCount + num];
		int num2 = triangleArray.Length / 3;
		for (int i = 0; i < vertexCount; i++)
		{
			array[i] = -1;
		}
		FindEdges.Edge[] array2 = new FindEdges.Edge[num];
		int num3 = 0;
		for (int j = 0; j < num2; j++)
		{
			int num4 = triangleArray[j * 3 + 2];
			for (int k = 0; k < 3; k++)
			{
				int num5 = triangleArray[j * 3 + k];
				if (num4 < num5)
				{
					FindEdges.Edge edge = new FindEdges.Edge();
					edge.vertexIndex[0] = num4;
					edge.vertexIndex[1] = num5;
					edge.faceIndex[0] = j;
					edge.faceIndex[1] = j;
					array2[num3] = edge;
					int num6 = array[num4];
					if (num6 == -1)
					{
						array[num4] = num3;
					}
					else
					{
						while (true)
						{
							int num7 = array[vertexCount + num6];
							if (num7 == -1)
							{
								break;
							}
							num6 = num7;
						}
						array[vertexCount + num6] = num3;
					}
					array[vertexCount + num3] = -1;
					num3++;
				}
				num4 = num5;
			}
		}
		for (int l = 0; l < num2; l++)
		{
			int num8 = triangleArray[l * 3 + 2];
			for (int m = 0; m < 3; m++)
			{
				int num9 = triangleArray[l * 3 + m];
				if (num8 > num9)
				{
					bool flag = false;
					for (int num10 = array[num9]; num10 != -1; num10 = array[vertexCount + num10])
					{
						FindEdges.Edge edge2 = array2[num10];
						if (edge2.vertexIndex[1] == num8 && edge2.faceIndex[0] == edge2.faceIndex[1])
						{
							array2[num10].faceIndex[1] = l;
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						FindEdges.Edge edge3 = new FindEdges.Edge();
						edge3.vertexIndex[0] = num8;
						edge3.vertexIndex[1] = num9;
						edge3.faceIndex[0] = l;
						edge3.faceIndex[1] = l;
						array2[num3] = edge3;
						num3++;
					}
				}
				num8 = num9;
			}
		}
		FindEdges.Edge[] array3 = new FindEdges.Edge[num3];
		for (int n = 0; n < num3; n++)
		{
			array3[n] = array2[n];
		}
		return array3;
	}
}
