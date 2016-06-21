using Pathfinding.ClipperLib;
using Pathfinding.Util;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Pathfinding
{
	[AddComponentMenu("Pathfinding/Navmesh/Navmesh Cut")]
	public class NavmeshCut : MonoBehaviour
	{
		public enum MeshType
		{
			Rectangle,
			Circle,
			CustomMesh
		}

		private static List<NavmeshCut> allCuts = new List<NavmeshCut>();

		public NavmeshCut.MeshType type;

		public Mesh mesh;

		public Vector2 rectangleSize = new Vector2(1f, 1f);

		public float circleRadius = 1f;

		public int circleResolution = 6;

		public float height = 1f;

		public float meshScale = 1f;

		public Vector3 center;

		public float updateDistance = 0.4f;

		public bool isDual;

		public bool cutsAddedGeom = true;

		public float updateRotationDistance = 10f;

		public bool useRotation;

		private Vector3[][] contours;

		protected Transform tr;

		private Mesh lastMesh;

		private Vector3 lastPosition;

		private Quaternion lastRotation;

		private bool wasEnabled;

		private Bounds lastBounds;

		private static readonly Dictionary<Int2, int> edges = new Dictionary<Int2, int>();

		private static readonly Dictionary<int, int> pointers = new Dictionary<int, int>();

		public static readonly Color GizmoColor = new Color(0.145098045f, 0.721568644f, 0.9372549f);

		public static event Action<NavmeshCut> OnDestroyCallback
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				NavmeshCut.OnDestroyCallback = (Action<NavmeshCut>)Delegate.Combine(NavmeshCut.OnDestroyCallback, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				NavmeshCut.OnDestroyCallback = (Action<NavmeshCut>)Delegate.Remove(NavmeshCut.OnDestroyCallback, value);
			}
		}

		public Bounds LastBounds
		{
			get
			{
				return this.lastBounds;
			}
		}

		private static void AddCut(NavmeshCut obj)
		{
			NavmeshCut.allCuts.Add(obj);
		}

		private static void RemoveCut(NavmeshCut obj)
		{
			NavmeshCut.allCuts.Remove(obj);
		}

		public static List<NavmeshCut> GetAllInRange(Bounds b)
		{
			List<NavmeshCut> list = ListPool<NavmeshCut>.Claim();
			for (int i = 0; i < NavmeshCut.allCuts.Count; i++)
			{
				if (NavmeshCut.allCuts[i].enabled && NavmeshCut.Intersects(b, NavmeshCut.allCuts[i].GetBounds()))
				{
					list.Add(NavmeshCut.allCuts[i]);
				}
			}
			return list;
		}

		private static bool Intersects(Bounds b1, Bounds b2)
		{
			Vector3 min = b1.min;
			Vector3 max = b1.max;
			Vector3 min2 = b2.min;
			Vector3 max2 = b2.max;
			return min.x <= max2.x && max.x >= min2.x && min.y <= max2.y && max.y >= min2.y && min.z <= max2.z && max.z >= min2.z;
		}

		public static List<NavmeshCut> GetAll()
		{
			return NavmeshCut.allCuts;
		}

		public void Awake()
		{
			NavmeshCut.AddCut(this);
		}

		public void OnEnable()
		{
			this.tr = base.transform;
			this.lastPosition = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
			this.lastRotation = this.tr.rotation;
		}

		public void OnDestroy()
		{
			if (NavmeshCut.OnDestroyCallback != null)
			{
				NavmeshCut.OnDestroyCallback(this);
			}
			NavmeshCut.RemoveCut(this);
		}

		public void ForceUpdate()
		{
			this.lastPosition = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
		}

		public bool RequiresUpdate()
		{
			return this.wasEnabled != base.enabled || (this.wasEnabled && ((this.tr.position - this.lastPosition).sqrMagnitude > this.updateDistance * this.updateDistance || (this.useRotation && Quaternion.Angle(this.lastRotation, this.tr.rotation) > this.updateRotationDistance)));
		}

		public virtual void UsedForCut()
		{
		}

		public void NotifyUpdated()
		{
			this.wasEnabled = base.enabled;
			if (this.wasEnabled)
			{
				this.lastPosition = this.tr.position;
				this.lastBounds = this.GetBounds();
				if (this.useRotation)
				{
					this.lastRotation = this.tr.rotation;
				}
			}
		}

		private void CalculateMeshContour()
		{
			if (this.mesh == null)
			{
				return;
			}
			NavmeshCut.edges.Clear();
			NavmeshCut.pointers.Clear();
			Vector3[] vertices = this.mesh.vertices;
			int[] triangles = this.mesh.triangles;
			for (int i = 0; i < triangles.Length; i += 3)
			{
				if (Polygon.IsClockwise(vertices[triangles[i]], vertices[triangles[i + 1]], vertices[triangles[i + 2]]))
				{
					int num = triangles[i];
					triangles[i] = triangles[i + 2];
					triangles[i + 2] = num;
				}
				NavmeshCut.edges[new Int2(triangles[i], triangles[i + 1])] = i;
				NavmeshCut.edges[new Int2(triangles[i + 1], triangles[i + 2])] = i;
				NavmeshCut.edges[new Int2(triangles[i + 2], triangles[i])] = i;
			}
			for (int j = 0; j < triangles.Length; j += 3)
			{
				for (int k = 0; k < 3; k++)
				{
					if (!NavmeshCut.edges.ContainsKey(new Int2(triangles[j + (k + 1) % 3], triangles[j + k % 3])))
					{
						NavmeshCut.pointers[triangles[j + k % 3]] = triangles[j + (k + 1) % 3];
					}
				}
			}
			List<Vector3[]> list = new List<Vector3[]>();
			List<Vector3> list2 = ListPool<Vector3>.Claim();
			for (int l = 0; l < vertices.Length; l++)
			{
				if (NavmeshCut.pointers.ContainsKey(l))
				{
					list2.Clear();
					int num2 = l;
					do
					{
						int num3 = NavmeshCut.pointers[num2];
						if (num3 == -1)
						{
							break;
						}
						NavmeshCut.pointers[num2] = -1;
						list2.Add(vertices[num2]);
						num2 = num3;
						if (num2 == -1)
						{
							goto Block_9;
						}
					}
					while (num2 != l);
					IL_20C:
					if (list2.Count > 0)
					{
						list.Add(list2.ToArray());
						goto IL_227;
					}
					goto IL_227;
					Block_9:
					Debug.LogError("Invalid Mesh '" + this.mesh.name + " in " + base.gameObject.name);
					goto IL_20C;
				}
				IL_227:;
			}
			ListPool<Vector3>.Release(list2);
			this.contours = list.ToArray();
		}

		public Bounds GetBounds()
		{
			Bounds result = default(Bounds);
			switch (this.type)
			{
			case NavmeshCut.MeshType.Rectangle:
				if (this.useRotation)
				{
					Matrix4x4 localToWorldMatrix = this.tr.localToWorldMatrix;
					result = new Bounds(localToWorldMatrix.MultiplyPoint3x4(this.center + new Vector3(-this.rectangleSize.x, -this.height, -this.rectangleSize.y) * 0.5f), Vector3.zero);
					result.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(this.center + new Vector3(this.rectangleSize.x, -this.height, -this.rectangleSize.y) * 0.5f));
					result.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(this.center + new Vector3(this.rectangleSize.x, -this.height, this.rectangleSize.y) * 0.5f));
					result.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(this.center + new Vector3(-this.rectangleSize.x, -this.height, this.rectangleSize.y) * 0.5f));
					result.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(this.center + new Vector3(-this.rectangleSize.x, this.height, -this.rectangleSize.y) * 0.5f));
					result.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(this.center + new Vector3(this.rectangleSize.x, this.height, -this.rectangleSize.y) * 0.5f));
					result.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(this.center + new Vector3(this.rectangleSize.x, this.height, this.rectangleSize.y) * 0.5f));
					result.Encapsulate(localToWorldMatrix.MultiplyPoint3x4(this.center + new Vector3(-this.rectangleSize.x, this.height, this.rectangleSize.y) * 0.5f));
				}
				else
				{
					result = new Bounds(this.tr.position + this.center, new Vector3(this.rectangleSize.x, this.height, this.rectangleSize.y));
				}
				break;
			case NavmeshCut.MeshType.Circle:
				if (this.useRotation)
				{
					result = new Bounds(this.tr.localToWorldMatrix.MultiplyPoint3x4(this.center), new Vector3(this.circleRadius * 2f, this.height, this.circleRadius * 2f));
				}
				else
				{
					result = new Bounds(base.transform.position + this.center, new Vector3(this.circleRadius * 2f, this.height, this.circleRadius * 2f));
				}
				break;
			case NavmeshCut.MeshType.CustomMesh:
				if (!(this.mesh == null))
				{
					Bounds bounds = this.mesh.bounds;
					if (this.useRotation)
					{
						Matrix4x4 localToWorldMatrix2 = this.tr.localToWorldMatrix;
						bounds.center *= this.meshScale;
						bounds.size *= this.meshScale;
						result = new Bounds(localToWorldMatrix2.MultiplyPoint3x4(this.center + bounds.center), Vector3.zero);
						Vector3 max = bounds.max;
						Vector3 min = bounds.min;
						result.Encapsulate(localToWorldMatrix2.MultiplyPoint3x4(this.center + new Vector3(max.x, max.y, max.z)));
						result.Encapsulate(localToWorldMatrix2.MultiplyPoint3x4(this.center + new Vector3(min.x, max.y, max.z)));
						result.Encapsulate(localToWorldMatrix2.MultiplyPoint3x4(this.center + new Vector3(min.x, max.y, min.z)));
						result.Encapsulate(localToWorldMatrix2.MultiplyPoint3x4(this.center + new Vector3(max.x, max.y, min.z)));
						result.Encapsulate(localToWorldMatrix2.MultiplyPoint3x4(this.center + new Vector3(max.x, min.y, max.z)));
						result.Encapsulate(localToWorldMatrix2.MultiplyPoint3x4(this.center + new Vector3(min.x, min.y, max.z)));
						result.Encapsulate(localToWorldMatrix2.MultiplyPoint3x4(this.center + new Vector3(min.x, min.y, min.z)));
						result.Encapsulate(localToWorldMatrix2.MultiplyPoint3x4(this.center + new Vector3(max.x, min.y, min.z)));
						Vector3 size = result.size;
						size.y = Mathf.Max(size.y, this.height * this.tr.lossyScale.y);
						result.size = size;
					}
					else
					{
						Vector3 size2 = bounds.size * this.meshScale;
						size2.y = Mathf.Max(size2.y, this.height);
						result = new Bounds(base.transform.position + this.center + bounds.center * this.meshScale, size2);
					}
				}
				break;
			default:
				throw new Exception("Invalid mesh type");
			}
			return result;
		}

		public void GetContour(List<List<IntPoint>> buffer)
		{
			if (this.circleResolution < 3)
			{
				this.circleResolution = 3;
			}
			Vector3 a = this.tr.position;
			switch (this.type)
			{
			case NavmeshCut.MeshType.Rectangle:
			{
				List<IntPoint> list = ListPool<IntPoint>.Claim();
				if (this.useRotation)
				{
					Matrix4x4 localToWorldMatrix = this.tr.localToWorldMatrix;
					list.Add(NavmeshCut.V3ToIntPoint(localToWorldMatrix.MultiplyPoint3x4(this.center + new Vector3(-this.rectangleSize.x, 0f, -this.rectangleSize.y) * 0.5f)));
					list.Add(NavmeshCut.V3ToIntPoint(localToWorldMatrix.MultiplyPoint3x4(this.center + new Vector3(this.rectangleSize.x, 0f, -this.rectangleSize.y) * 0.5f)));
					list.Add(NavmeshCut.V3ToIntPoint(localToWorldMatrix.MultiplyPoint3x4(this.center + new Vector3(this.rectangleSize.x, 0f, this.rectangleSize.y) * 0.5f)));
					list.Add(NavmeshCut.V3ToIntPoint(localToWorldMatrix.MultiplyPoint3x4(this.center + new Vector3(-this.rectangleSize.x, 0f, this.rectangleSize.y) * 0.5f)));
				}
				else
				{
					a += this.center;
					list.Add(NavmeshCut.V3ToIntPoint(a + new Vector3(-this.rectangleSize.x, 0f, -this.rectangleSize.y) * 0.5f));
					list.Add(NavmeshCut.V3ToIntPoint(a + new Vector3(this.rectangleSize.x, 0f, -this.rectangleSize.y) * 0.5f));
					list.Add(NavmeshCut.V3ToIntPoint(a + new Vector3(this.rectangleSize.x, 0f, this.rectangleSize.y) * 0.5f));
					list.Add(NavmeshCut.V3ToIntPoint(a + new Vector3(-this.rectangleSize.x, 0f, this.rectangleSize.y) * 0.5f));
				}
				buffer.Add(list);
				break;
			}
			case NavmeshCut.MeshType.Circle:
			{
				List<IntPoint> list = ListPool<IntPoint>.Claim(this.circleResolution);
				if (this.useRotation)
				{
					Matrix4x4 localToWorldMatrix2 = this.tr.localToWorldMatrix;
					for (int i = 0; i < this.circleResolution; i++)
					{
						list.Add(NavmeshCut.V3ToIntPoint(localToWorldMatrix2.MultiplyPoint3x4(this.center + new Vector3(Mathf.Cos((float)(i * 2) * 3.14159274f / (float)this.circleResolution), 0f, Mathf.Sin((float)(i * 2) * 3.14159274f / (float)this.circleResolution)) * this.circleRadius)));
					}
				}
				else
				{
					a += this.center;
					for (int j = 0; j < this.circleResolution; j++)
					{
						list.Add(NavmeshCut.V3ToIntPoint(a + new Vector3(Mathf.Cos((float)(j * 2) * 3.14159274f / (float)this.circleResolution), 0f, Mathf.Sin((float)(j * 2) * 3.14159274f / (float)this.circleResolution)) * this.circleRadius));
					}
				}
				buffer.Add(list);
				break;
			}
			case NavmeshCut.MeshType.CustomMesh:
				if (this.mesh != this.lastMesh || this.contours == null)
				{
					this.CalculateMeshContour();
					this.lastMesh = this.mesh;
				}
				if (this.contours != null)
				{
					a += this.center;
					bool flag = Vector3.Dot(this.tr.up, Vector3.up) < 0f;
					for (int k = 0; k < this.contours.Length; k++)
					{
						Vector3[] array = this.contours[k];
						List<IntPoint> list = ListPool<IntPoint>.Claim(array.Length);
						if (this.useRotation)
						{
							Matrix4x4 localToWorldMatrix3 = this.tr.localToWorldMatrix;
							for (int l = 0; l < array.Length; l++)
							{
								list.Add(NavmeshCut.V3ToIntPoint(localToWorldMatrix3.MultiplyPoint3x4(this.center + array[l] * this.meshScale)));
							}
						}
						else
						{
							for (int m = 0; m < array.Length; m++)
							{
								list.Add(NavmeshCut.V3ToIntPoint(a + array[m] * this.meshScale));
							}
						}
						if (flag)
						{
							list.Reverse();
						}
						buffer.Add(list);
					}
				}
				break;
			}
		}

		public static IntPoint V3ToIntPoint(Vector3 p)
		{
			Int3 @int = (Int3)p;
			return new IntPoint((long)@int.x, (long)@int.z);
		}

		public static Vector3 IntPointToV3(IntPoint p)
		{
			Int3 ob = new Int3((int)p.X, 0, (int)p.Y);
			return (Vector3)ob;
		}

		public void OnDrawGizmos()
		{
			if (this.tr == null)
			{
				this.tr = base.transform;
			}
			List<List<IntPoint>> list = ListPool<List<IntPoint>>.Claim();
			this.GetContour(list);
			Gizmos.color = NavmeshCut.GizmoColor;
			Bounds bounds = this.GetBounds();
			float y = bounds.min.y;
			Vector3 b = Vector3.up * (bounds.max.y - y);
			for (int i = 0; i < list.Count; i++)
			{
				List<IntPoint> list2 = list[i];
				for (int j = 0; j < list2.Count; j++)
				{
					Vector3 vector = NavmeshCut.IntPointToV3(list2[j]);
					vector.y = y;
					Vector3 vector2 = NavmeshCut.IntPointToV3(list2[(j + 1) % list2.Count]);
					vector2.y = y;
					Gizmos.DrawLine(vector, vector2);
					Gizmos.DrawLine(vector + b, vector2 + b);
					Gizmos.DrawLine(vector, vector + b);
					Gizmos.DrawLine(vector2, vector2 + b);
				}
			}
			ListPool<List<IntPoint>>.Release(list);
		}

		public void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.Lerp(NavmeshCut.GizmoColor, new Color(1f, 1f, 1f, 0.2f), 0.9f);
			Bounds bounds = this.GetBounds();
			Gizmos.DrawCube(bounds.center, bounds.size);
			Gizmos.DrawWireCube(bounds.center, bounds.size);
		}
	}
}
