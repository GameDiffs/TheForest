using System;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
	public class MB_Utility
	{
		public struct MeshAnalysisResult
		{
			public Rect uvRect;

			public bool hasOutOfBoundsUVs;

			public bool hasOverlappingSubmeshVerts;

			public bool hasOverlappingSubmeshTris;
		}

		private class MB_Triangle
		{
			private int submeshIdx;

			private int[] vs = new int[3];

			public bool isSame(object obj)
			{
				MB_Utility.MB_Triangle mB_Triangle = (MB_Utility.MB_Triangle)obj;
				return this.vs[0] == mB_Triangle.vs[0] && this.vs[1] == mB_Triangle.vs[1] && this.vs[2] == mB_Triangle.vs[2] && this.submeshIdx != mB_Triangle.submeshIdx;
			}

			public bool sharesVerts(MB_Utility.MB_Triangle obj)
			{
				return ((this.vs[0] == obj.vs[0] || this.vs[0] == obj.vs[1] || this.vs[0] == obj.vs[2]) && this.submeshIdx != obj.submeshIdx) || ((this.vs[1] == obj.vs[0] || this.vs[1] == obj.vs[1] || this.vs[1] == obj.vs[2]) && this.submeshIdx != obj.submeshIdx) || ((this.vs[2] == obj.vs[0] || this.vs[2] == obj.vs[1] || this.vs[2] == obj.vs[2]) && this.submeshIdx != obj.submeshIdx);
			}

			public void Initialize(int[] ts, int idx, int sIdx)
			{
				this.vs[0] = ts[idx];
				this.vs[1] = ts[idx + 1];
				this.vs[2] = ts[idx + 2];
				this.submeshIdx = sIdx;
				Array.Sort<int>(this.vs);
			}
		}

		public static Texture2D createTextureCopy(Texture2D source)
		{
			Texture2D texture2D = new Texture2D(source.width, source.height, TextureFormat.ARGB32, true);
			texture2D.SetPixels(source.GetPixels());
			return texture2D;
		}

		public static bool ArrayBIsSubsetOfA(object[] a, object[] b)
		{
			for (int i = 0; i < b.Length; i++)
			{
				bool flag = false;
				for (int j = 0; j < a.Length; j++)
				{
					if (a[j] == b[i])
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		public static Material[] GetGOMaterials(GameObject go)
		{
			if (go == null)
			{
				return null;
			}
			Material[] array = null;
			Mesh mesh = null;
			MeshRenderer component = go.GetComponent<MeshRenderer>();
			if (component != null)
			{
				array = component.sharedMaterials;
				MeshFilter component2 = go.GetComponent<MeshFilter>();
				if (component2 == null)
				{
					throw new Exception("Object " + go + " has a MeshRenderer but no MeshFilter.");
				}
				mesh = component2.sharedMesh;
			}
			SkinnedMeshRenderer component3 = go.GetComponent<SkinnedMeshRenderer>();
			if (component3 != null)
			{
				array = component3.sharedMaterials;
				mesh = component3.sharedMesh;
			}
			if (array == null)
			{
				Debug.LogError("Object " + go.name + " does not have a MeshRenderer or a SkinnedMeshRenderer component");
				return null;
			}
			if (mesh == null)
			{
				Debug.LogError("Object " + go.name + " has a MeshRenderer or SkinnedMeshRenderer but no mesh.");
				return null;
			}
			if (mesh.subMeshCount < array.Length)
			{
				Debug.LogWarning(string.Concat(new object[]
				{
					"Object ",
					go,
					" has only ",
					mesh.subMeshCount,
					" submeshes and has ",
					array.Length,
					" materials. Extra materials do nothing."
				}));
				Material[] array2 = new Material[mesh.subMeshCount];
				Array.Copy(array, array2, array2.Length);
				array = array2;
			}
			return array;
		}

		public static Mesh GetMesh(GameObject go)
		{
			if (go == null)
			{
				return null;
			}
			MeshFilter component = go.GetComponent<MeshFilter>();
			if (component != null)
			{
				return component.sharedMesh;
			}
			SkinnedMeshRenderer component2 = go.GetComponent<SkinnedMeshRenderer>();
			if (component2 != null)
			{
				return component2.sharedMesh;
			}
			Debug.LogError("Object " + go.name + " does not have a MeshFilter or a SkinnedMeshRenderer component");
			return null;
		}

		public static Renderer GetRenderer(GameObject go)
		{
			if (go == null)
			{
				return null;
			}
			MeshRenderer component = go.GetComponent<MeshRenderer>();
			if (component != null)
			{
				return component;
			}
			SkinnedMeshRenderer component2 = go.GetComponent<SkinnedMeshRenderer>();
			if (component2 != null)
			{
				return component2;
			}
			return null;
		}

		public static void DisableRendererInSource(GameObject go)
		{
			if (go == null)
			{
				return;
			}
			MeshRenderer component = go.GetComponent<MeshRenderer>();
			if (component != null)
			{
				component.enabled = false;
				return;
			}
			SkinnedMeshRenderer component2 = go.GetComponent<SkinnedMeshRenderer>();
			if (component2 != null)
			{
				component2.enabled = false;
				return;
			}
		}

		public static bool hasOutOfBoundsUVs(Mesh m, ref Rect uvBounds)
		{
			MB_Utility.MeshAnalysisResult meshAnalysisResult = default(MB_Utility.MeshAnalysisResult);
			return MB_Utility.hasOutOfBoundsUVs(m, ref uvBounds, ref meshAnalysisResult, -1);
		}

		public static bool hasOutOfBoundsUVs(Mesh m, ref Rect uvBounds, ref MB_Utility.MeshAnalysisResult putResultHere, int submeshIndex = -1)
		{
			if (m == null)
			{
				putResultHere.hasOutOfBoundsUVs = false;
				return putResultHere.hasOutOfBoundsUVs;
			}
			Vector2[] uv = m.uv;
			if (uv.Length == 0)
			{
				putResultHere.hasOutOfBoundsUVs = false;
				putResultHere.uvRect = uvBounds;
				return putResultHere.hasOutOfBoundsUVs;
			}
			if (submeshIndex >= m.subMeshCount)
			{
				putResultHere.hasOutOfBoundsUVs = false;
				putResultHere.uvRect = uvBounds;
				return putResultHere.hasOutOfBoundsUVs;
			}
			float x;
			float num;
			float y;
			float num2;
			if (submeshIndex >= 0)
			{
				int[] triangles = m.GetTriangles(submeshIndex);
				if (triangles.Length == 0)
				{
					putResultHere.hasOutOfBoundsUVs = false;
					putResultHere.uvRect = uvBounds;
					return putResultHere.hasOutOfBoundsUVs;
				}
				num = (x = uv[triangles[0]].x);
				num2 = (y = uv[triangles[0]].y);
				for (int i = 0; i < triangles.Length; i++)
				{
					int num3 = triangles[i];
					if (uv[num3].x < x)
					{
						x = uv[num3].x;
					}
					if (uv[num3].x > num)
					{
						num = uv[num3].x;
					}
					if (uv[num3].y < y)
					{
						y = uv[num3].y;
					}
					if (uv[num3].y > num2)
					{
						num2 = uv[num3].y;
					}
				}
			}
			else
			{
				num = (x = uv[0].x);
				num2 = (y = uv[0].y);
				for (int j = 0; j < uv.Length; j++)
				{
					if (uv[j].x < x)
					{
						x = uv[j].x;
					}
					if (uv[j].x > num)
					{
						num = uv[j].x;
					}
					if (uv[j].y < y)
					{
						y = uv[j].y;
					}
					if (uv[j].y > num2)
					{
						num2 = uv[j].y;
					}
				}
			}
			uvBounds.x = x;
			uvBounds.y = y;
			uvBounds.width = num - x;
			uvBounds.height = num2 - y;
			if (num > 1f || x < 0f || num2 > 1f || y < 0f)
			{
				putResultHere.hasOutOfBoundsUVs = true;
				putResultHere.uvRect = uvBounds;
				return putResultHere.hasOutOfBoundsUVs;
			}
			float num4 = 0f;
			uvBounds.y = num4;
			uvBounds.x = num4;
			num4 = 1f;
			uvBounds.height = num4;
			uvBounds.width = num4;
			putResultHere.uvRect = uvBounds;
			putResultHere.hasOutOfBoundsUVs = false;
			return putResultHere.hasOutOfBoundsUVs;
		}

		public static void setSolidColor(Texture2D t, Color c)
		{
			Color[] pixels = t.GetPixels();
			for (int i = 0; i < pixels.Length; i++)
			{
				pixels[i] = c;
			}
			t.SetPixels(pixels);
			t.Apply();
		}

		public static Texture2D resampleTexture(Texture2D source, int newWidth, int newHeight)
		{
			TextureFormat format = source.format;
			if (format == TextureFormat.ARGB32 || format == TextureFormat.RGBA32 || format == TextureFormat.BGRA32 || format == TextureFormat.RGB24 || format == TextureFormat.Alpha8 || format == TextureFormat.DXT1)
			{
				Texture2D texture2D = new Texture2D(newWidth, newHeight, TextureFormat.ARGB32, true);
				float num = (float)newWidth;
				float num2 = (float)newHeight;
				for (int i = 0; i < newWidth; i++)
				{
					for (int j = 0; j < newHeight; j++)
					{
						float u = (float)i / num;
						float v = (float)j / num2;
						texture2D.SetPixel(i, j, source.GetPixelBilinear(u, v));
					}
				}
				texture2D.Apply();
				return texture2D;
			}
			Debug.LogError("Can only resize textures in formats ARGB32, RGBA32, BGRA32, RGB24, Alpha8 or DXT");
			return null;
		}

		public static bool AreAllSharedMaterialsDistinct(Material[] sharedMaterials)
		{
			for (int i = 0; i < sharedMaterials.Length; i++)
			{
				for (int j = i + 1; j < sharedMaterials.Length; j++)
				{
					if (sharedMaterials[i] == sharedMaterials[j])
					{
						return false;
					}
				}
			}
			return true;
		}

		public static int doSubmeshesShareVertsOrTris(Mesh m, ref MB_Utility.MeshAnalysisResult mar)
		{
			MB_Utility.MB_Triangle mB_Triangle = new MB_Utility.MB_Triangle();
			MB_Utility.MB_Triangle mB_Triangle2 = new MB_Utility.MB_Triangle();
			int[][] array = new int[m.subMeshCount][];
			for (int i = 0; i < m.subMeshCount; i++)
			{
				array[i] = m.GetTriangles(i);
			}
			bool flag = false;
			bool flag2 = false;
			for (int j = 0; j < m.subMeshCount; j++)
			{
				int[] array2 = array[j];
				for (int k = j + 1; k < m.subMeshCount; k++)
				{
					int[] array3 = array[k];
					for (int l = 0; l < array2.Length; l += 3)
					{
						mB_Triangle.Initialize(array2, l, j);
						for (int n = 0; n < array3.Length; n += 3)
						{
							mB_Triangle2.Initialize(array3, n, k);
							if (mB_Triangle.isSame(mB_Triangle2))
							{
								flag2 = true;
								break;
							}
							if (mB_Triangle.sharesVerts(mB_Triangle2))
							{
								flag = true;
								break;
							}
						}
					}
				}
			}
			if (flag2)
			{
				mar.hasOverlappingSubmeshVerts = true;
				mar.hasOverlappingSubmeshTris = true;
				return 2;
			}
			if (flag)
			{
				mar.hasOverlappingSubmeshVerts = true;
				mar.hasOverlappingSubmeshTris = false;
				return 1;
			}
			mar.hasOverlappingSubmeshTris = false;
			mar.hasOverlappingSubmeshVerts = false;
			return 0;
		}

		public static bool GetBounds(GameObject go, out Bounds b)
		{
			if (go == null)
			{
				Debug.LogError("go paramater was null");
				b = new Bounds(Vector3.zero, Vector3.zero);
				return false;
			}
			Renderer renderer = MB_Utility.GetRenderer(go);
			if (renderer == null)
			{
				Debug.LogError("GetBounds must be called on an object with a Renderer");
				b = new Bounds(Vector3.zero, Vector3.zero);
				return false;
			}
			if (renderer is MeshRenderer)
			{
				b = renderer.bounds;
				return true;
			}
			if (renderer is SkinnedMeshRenderer)
			{
				b = renderer.bounds;
				return true;
			}
			Debug.LogError("GetBounds must be called on an object with a MeshRender or a SkinnedMeshRenderer.");
			b = new Bounds(Vector3.zero, Vector3.zero);
			return false;
		}

		public static void Destroy(UnityEngine.Object o)
		{
			if (Application.isPlaying)
			{
				UnityEngine.Object.Destroy(o);
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(o, false);
			}
		}
	}
}
