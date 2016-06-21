using System;
using UnityEngine;

namespace Ceto
{
	public static class OceanUtility
	{
		public static int ShowLayer(int mask, string layer)
		{
			return mask | 1 << LayerMask.NameToLayer(layer);
		}

		public static int HideLayer(int mask, string layer)
		{
			return mask & ~(1 << LayerMask.NameToLayer(layer));
		}

		public static int ToggleLayer(int mask, string layer)
		{
			return mask ^ 1 << LayerMask.NameToLayer(layer);
		}

		public static int ShowLayer(int mask, LayerMask layer)
		{
			return mask | 1 << layer;
		}

		public static int HideLayer(int mask, LayerMask layer)
		{
			return mask & ~(1 << layer);
		}

		public static int ToggleLayer(int mask, LayerMask layer)
		{
			return mask ^ 1 << layer;
		}

		public static Mesh CreateQuadMesh()
		{
			Vector3[] array = new Vector3[4];
			Vector2[] array2 = new Vector2[4];
			int[] triangles = new int[]
			{
				0,
				2,
				1,
				2,
				3,
				1
			};
			array[0] = new Vector3(-1f, 0f, -1f);
			array[1] = new Vector3(1f, 0f, -1f);
			array[2] = new Vector3(-1f, 0f, 1f);
			array[3] = new Vector3(1f, 0f, 1f);
			array2[0] = new Vector2(0f, 0f);
			array2[1] = new Vector2(1f, 0f);
			array2[2] = new Vector2(0f, 1f);
			array2[3] = new Vector2(1f, 1f);
			return new Mesh
			{
				vertices = array,
				uv = array2,
				triangles = triangles
			};
		}
	}
}
