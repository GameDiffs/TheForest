using Pathfinding.Serialization;
using Pathfinding.Serialization.JsonFx;
using Pathfinding.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
	[JsonOptIn]
	[Serializable]
	public class NavMeshGraph : NavGraph, IUpdatableGraph, IRaycastableGraph, INavmesh, INavmeshHolder, IFunnelGraph
	{
		[JsonMember]
		public Mesh sourceMesh;

		[JsonMember]
		public Vector3 offset;

		[JsonMember]
		public Vector3 rotation;

		[JsonMember]
		public float scale = 1f;

		[JsonMember]
		public bool accurateNearestNode = true;

		public TriangleMeshNode[] nodes;

		private BBTree _bbTree;

		[NonSerialized]
		private Int3[] _vertices;

		[NonSerialized]
		private Vector3[] originalVertices;

		[NonSerialized]
		public int[] triangles;

		public TriangleMeshNode[] TriNodes
		{
			get
			{
				return this.nodes;
			}
		}

		public BBTree bbTree
		{
			get
			{
				return this._bbTree;
			}
			set
			{
				this._bbTree = value;
			}
		}

		public Int3[] vertices
		{
			get
			{
				return this._vertices;
			}
			set
			{
				this._vertices = value;
			}
		}

		public override void GetNodes(GraphNodeDelegateCancelable del)
		{
			if (this.nodes == null)
			{
				return;
			}
			int num = 0;
			while (num < this.nodes.Length && del(this.nodes[num]))
			{
				num++;
			}
		}

		public override void OnDestroy()
		{
			base.OnDestroy();
			TriangleMeshNode.SetNavmeshHolder(this.active.astarData.GetGraphIndex(this), null);
		}

		public Int3 GetVertex(int index)
		{
			return this.vertices[index];
		}

		public int GetVertexArrayIndex(int index)
		{
			return index;
		}

		public void GetTileCoordinates(int tileIndex, out int x, out int z)
		{
			x = (z = 0);
		}

		public void GenerateMatrix()
		{
			base.SetMatrix(Matrix4x4.TRS(this.offset, Quaternion.Euler(this.rotation), new Vector3(this.scale, this.scale, this.scale)));
		}

		public override void RelocateNodes(Matrix4x4 oldMatrix, Matrix4x4 newMatrix)
		{
			if (this.vertices == null || this.vertices.Length == 0 || this.originalVertices == null || this.originalVertices.Length != this.vertices.Length)
			{
				return;
			}
			for (int i = 0; i < this._vertices.Length; i++)
			{
				this._vertices[i] = (Int3)newMatrix.MultiplyPoint3x4(this.originalVertices[i]);
			}
			for (int j = 0; j < this.nodes.Length; j++)
			{
				TriangleMeshNode triangleMeshNode = this.nodes[j];
				triangleMeshNode.UpdatePositionFromVertices();
				if (triangleMeshNode.connections != null)
				{
					for (int k = 0; k < triangleMeshNode.connections.Length; k++)
					{
						triangleMeshNode.connectionCosts[k] = (uint)(triangleMeshNode.position - triangleMeshNode.connections[k].position).costMagnitude;
					}
				}
			}
			base.SetMatrix(newMatrix);
			NavMeshGraph.RebuildBBTree(this);
		}

		public static NNInfo GetNearest(NavMeshGraph graph, GraphNode[] nodes, Vector3 position, NNConstraint constraint, bool accurateNearestNode)
		{
			if (nodes == null || nodes.Length == 0)
			{
				Debug.LogError("NavGraph hasn't been generated yet or does not contain any nodes");
				return default(NNInfo);
			}
			if (constraint == null)
			{
				constraint = NNConstraint.None;
			}
			Int3[] vertices = graph.vertices;
			if (graph.bbTree == null)
			{
				return NavMeshGraph.GetNearestForce(graph, graph, position, constraint, accurateNearestNode);
			}
			float num = (graph.bbTree.Size.width + graph.bbTree.Size.height) * 0.5f * 0.02f;
			NNInfo result = graph.bbTree.QueryCircle(position, num, constraint);
			if (result.node == null)
			{
				for (int i = 1; i <= 8; i++)
				{
					result = graph.bbTree.QueryCircle(position, (float)(i * i) * num, constraint);
					if (result.node != null || (float)((i - 1) * (i - 1)) * num > AstarPath.active.maxNearestNodeDistance * 2f)
					{
						break;
					}
				}
			}
			if (result.node != null)
			{
				result.clampedPosition = NavMeshGraph.ClosestPointOnNode(result.node as TriangleMeshNode, vertices, position);
			}
			if (result.constrainedNode != null)
			{
				if (constraint.constrainDistance && ((Vector3)result.constrainedNode.position - position).sqrMagnitude > AstarPath.active.maxNearestNodeDistanceSqr)
				{
					result.constrainedNode = null;
				}
				else
				{
					result.constClampedPosition = NavMeshGraph.ClosestPointOnNode(result.constrainedNode as TriangleMeshNode, vertices, position);
				}
			}
			return result;
		}

		public override NNInfo GetNearest(Vector3 position, NNConstraint constraint, GraphNode hint)
		{
			return NavMeshGraph.GetNearest(this, this.nodes, position, constraint, this.accurateNearestNode);
		}

		public override NNInfo GetNearestForce(Vector3 position, NNConstraint constraint)
		{
			return NavMeshGraph.GetNearestForce(this, this, position, constraint, this.accurateNearestNode);
		}

		public static NNInfo GetNearestForce(NavGraph graph, INavmeshHolder navmesh, Vector3 position, NNConstraint constraint, bool accurateNearestNode)
		{
			NNInfo nearestForceBoth = NavMeshGraph.GetNearestForceBoth(graph, navmesh, position, constraint, accurateNearestNode);
			nearestForceBoth.node = nearestForceBoth.constrainedNode;
			nearestForceBoth.clampedPosition = nearestForceBoth.constClampedPosition;
			return nearestForceBoth;
		}

		public static NNInfo GetNearestForceBoth(NavGraph graph, INavmeshHolder navmesh, Vector3 position, NNConstraint constraint, bool accurateNearestNode)
		{
			Int3 pos = (Int3)position;
			float minDist = -1f;
			GraphNode minNode = null;
			float minConstDist = -1f;
			GraphNode minConstNode = null;
			float maxDistSqr = (!constraint.constrainDistance) ? float.PositiveInfinity : AstarPath.active.maxNearestNodeDistanceSqr;
			GraphNodeDelegateCancelable del = delegate(GraphNode _node)
			{
				TriangleMeshNode triangleMeshNode3 = _node as TriangleMeshNode;
				if (accurateNearestNode)
				{
					Vector3 b = triangleMeshNode3.ClosestPointOnNode(position);
					float sqrMagnitude = ((Vector3)pos - b).sqrMagnitude;
					if (minNode == null || sqrMagnitude < minDist)
					{
						minDist = sqrMagnitude;
						minNode = triangleMeshNode3;
					}
					if (sqrMagnitude < maxDistSqr && constraint.Suitable(triangleMeshNode3) && (minConstNode == null || sqrMagnitude < minConstDist))
					{
						minConstDist = sqrMagnitude;
						minConstNode = triangleMeshNode3;
					}
				}
				else if (!triangleMeshNode3.ContainsPoint((Int3)position))
				{
					float sqrMagnitude2 = (triangleMeshNode3.position - pos).sqrMagnitude;
					if (minNode == null || sqrMagnitude2 < minDist)
					{
						minDist = sqrMagnitude2;
						minNode = triangleMeshNode3;
					}
					if (sqrMagnitude2 < maxDistSqr && constraint.Suitable(triangleMeshNode3) && (minConstNode == null || sqrMagnitude2 < minConstDist))
					{
						minConstDist = sqrMagnitude2;
						minConstNode = triangleMeshNode3;
					}
				}
				else
				{
					int num = AstarMath.Abs(triangleMeshNode3.position.y - pos.y);
					if (minNode == null || (float)num < minDist)
					{
						minDist = (float)num;
						minNode = triangleMeshNode3;
					}
					if ((float)num < maxDistSqr && constraint.Suitable(triangleMeshNode3) && (minConstNode == null || (float)num < minConstDist))
					{
						minConstDist = (float)num;
						minConstNode = triangleMeshNode3;
					}
				}
				return true;
			};
			graph.GetNodes(del);
			NNInfo result = new NNInfo(minNode);
			if (result.node != null)
			{
				TriangleMeshNode triangleMeshNode = result.node as TriangleMeshNode;
				Vector3 clampedPosition = triangleMeshNode.ClosestPointOnNode(position);
				result.clampedPosition = clampedPosition;
			}
			result.constrainedNode = minConstNode;
			if (result.constrainedNode != null)
			{
				TriangleMeshNode triangleMeshNode2 = result.constrainedNode as TriangleMeshNode;
				Vector3 constClampedPosition = triangleMeshNode2.ClosestPointOnNode(position);
				result.constClampedPosition = constClampedPosition;
			}
			return result;
		}

		public void BuildFunnelCorridor(List<GraphNode> path, int startIndex, int endIndex, List<Vector3> left, List<Vector3> right)
		{
			NavMeshGraph.BuildFunnelCorridor(this, path, startIndex, endIndex, left, right);
		}

		public static void BuildFunnelCorridor(INavmesh graph, List<GraphNode> path, int startIndex, int endIndex, List<Vector3> left, List<Vector3> right)
		{
			if (graph == null)
			{
				Debug.LogError("Couldn't cast graph to the appropriate type (graph isn't a Navmesh type graph, it doesn't implement the INavmesh interface)");
				return;
			}
			for (int i = startIndex; i < endIndex; i++)
			{
				TriangleMeshNode triangleMeshNode = path[i] as TriangleMeshNode;
				TriangleMeshNode triangleMeshNode2 = path[i + 1] as TriangleMeshNode;
				bool flag = true;
				int j;
				for (j = 0; j < 3; j++)
				{
					for (int k = 0; k < 3; k++)
					{
						if (triangleMeshNode.GetVertexIndex(j) == triangleMeshNode2.GetVertexIndex((k + 1) % 3) && triangleMeshNode.GetVertexIndex((j + 1) % 3) == triangleMeshNode2.GetVertexIndex(k))
						{
							flag = false;
							break;
						}
					}
					if (!flag)
					{
						break;
					}
				}
				if (j == 3)
				{
					left.Add((Vector3)triangleMeshNode.position);
					right.Add((Vector3)triangleMeshNode.position);
					left.Add((Vector3)triangleMeshNode2.position);
					right.Add((Vector3)triangleMeshNode2.position);
				}
				else
				{
					left.Add((Vector3)triangleMeshNode.GetVertex(j));
					right.Add((Vector3)triangleMeshNode.GetVertex((j + 1) % 3));
				}
			}
		}

		public void AddPortal(GraphNode n1, GraphNode n2, List<Vector3> left, List<Vector3> right)
		{
		}

		public bool Linecast(Vector3 origin, Vector3 end)
		{
			return this.Linecast(origin, end, base.GetNearest(origin, NNConstraint.None).node);
		}

		public bool Linecast(Vector3 origin, Vector3 end, GraphNode hint, out GraphHitInfo hit)
		{
			return NavMeshGraph.Linecast(this, origin, end, hint, out hit, null);
		}

		public bool Linecast(Vector3 origin, Vector3 end, GraphNode hint)
		{
			GraphHitInfo graphHitInfo;
			return NavMeshGraph.Linecast(this, origin, end, hint, out graphHitInfo, null);
		}

		public bool Linecast(Vector3 origin, Vector3 end, GraphNode hint, out GraphHitInfo hit, List<GraphNode> trace)
		{
			return NavMeshGraph.Linecast(this, origin, end, hint, out hit, trace);
		}

		public static bool Linecast(INavmesh graph, Vector3 tmp_origin, Vector3 tmp_end, GraphNode hint, out GraphHitInfo hit)
		{
			return NavMeshGraph.Linecast(graph, tmp_origin, tmp_end, hint, out hit, null);
		}

		public static bool Linecast(INavmesh graph, Vector3 tmp_origin, Vector3 tmp_end, GraphNode hint, out GraphHitInfo hit, List<GraphNode> trace)
		{
			Int3 @int = (Int3)tmp_end;
			Int3 int2 = (Int3)tmp_origin;
			hit = default(GraphHitInfo);
			if (float.IsNaN(tmp_origin.x + tmp_origin.y + tmp_origin.z))
			{
				throw new ArgumentException("origin is NaN");
			}
			if (float.IsNaN(tmp_end.x + tmp_end.y + tmp_end.z))
			{
				throw new ArgumentException("end is NaN");
			}
			TriangleMeshNode triangleMeshNode = hint as TriangleMeshNode;
			if (triangleMeshNode == null)
			{
				triangleMeshNode = ((graph as NavGraph).GetNearest(tmp_origin, NNConstraint.None).node as TriangleMeshNode);
				if (triangleMeshNode == null)
				{
					Debug.LogError("Could not find a valid node to start from");
					hit.point = tmp_origin;
					return true;
				}
			}
			if (int2 == @int)
			{
				hit.node = triangleMeshNode;
				return false;
			}
			int2 = (Int3)triangleMeshNode.ClosestPointOnNode((Vector3)int2);
			hit.origin = (Vector3)int2;
			if (!triangleMeshNode.Walkable)
			{
				hit.point = (Vector3)int2;
				hit.tangentOrigin = (Vector3)int2;
				return true;
			}
			List<Vector3> list = ListPool<Vector3>.Claim();
			List<Vector3> list2 = ListPool<Vector3>.Claim();
			int num = 0;
			while (true)
			{
				num++;
				if (num > 2000)
				{
					break;
				}
				TriangleMeshNode triangleMeshNode2 = null;
				if (trace != null)
				{
					trace.Add(triangleMeshNode);
				}
				if (triangleMeshNode.ContainsPoint(@int))
				{
					goto Block_9;
				}
				for (int i = 0; i < triangleMeshNode.connections.Length; i++)
				{
					if (triangleMeshNode.connections[i].GraphIndex == triangleMeshNode.GraphIndex)
					{
						list.Clear();
						list2.Clear();
						if (triangleMeshNode.GetPortal(triangleMeshNode.connections[i], list, list2, false))
						{
							Vector3 vector = list[0];
							Vector3 vector2 = list2[0];
							if (Polygon.LeftNotColinear(vector, vector2, hit.origin) || !Polygon.LeftNotColinear(vector, vector2, tmp_end))
							{
								float num2;
								float num3;
								if (Polygon.IntersectionFactor(vector, vector2, hit.origin, tmp_end, out num2, out num3))
								{
									if (num3 >= 0f)
									{
										if (num2 >= 0f && num2 <= 1f)
										{
											triangleMeshNode2 = (triangleMeshNode.connections[i] as TriangleMeshNode);
											break;
										}
									}
								}
							}
						}
					}
				}
				if (triangleMeshNode2 == null)
				{
					goto Block_18;
				}
				triangleMeshNode = triangleMeshNode2;
			}
			Debug.LogError("Linecast was stuck in infinite loop. Breaking.");
			ListPool<Vector3>.Release(list);
			ListPool<Vector3>.Release(list2);
			return true;
			Block_9:
			ListPool<Vector3>.Release(list);
			ListPool<Vector3>.Release(list2);
			return false;
			Block_18:
			int vertexCount = triangleMeshNode.GetVertexCount();
			for (int j = 0; j < vertexCount; j++)
			{
				Vector3 vector3 = (Vector3)triangleMeshNode.GetVertex(j);
				Vector3 vector4 = (Vector3)triangleMeshNode.GetVertex((j + 1) % vertexCount);
				if (Polygon.LeftNotColinear(vector3, vector4, hit.origin) || !Polygon.LeftNotColinear(vector3, vector4, tmp_end))
				{
					float num4;
					float num5;
					if (Polygon.IntersectionFactor(vector3, vector4, hit.origin, tmp_end, out num4, out num5))
					{
						if (num5 >= 0f)
						{
							if (num4 >= 0f && num4 <= 1f)
							{
								Vector3 point = vector3 + (vector4 - vector3) * num4;
								hit.point = point;
								hit.node = triangleMeshNode;
								hit.tangent = vector4 - vector3;
								hit.tangentOrigin = vector3;
								ListPool<Vector3>.Release(list);
								ListPool<Vector3>.Release(list2);
								return true;
							}
						}
					}
				}
			}
			Debug.LogWarning("Linecast failing because point not inside node, and line does not hit any edges of it");
			ListPool<Vector3>.Release(list);
			ListPool<Vector3>.Release(list2);
			return false;
		}

		public GraphUpdateThreading CanUpdateAsync(GraphUpdateObject o)
		{
			return GraphUpdateThreading.UnityThread;
		}

		public void UpdateAreaInit(GraphUpdateObject o)
		{
		}

		public void UpdateArea(GraphUpdateObject o)
		{
			NavMeshGraph.UpdateArea(o, this);
		}

		public static void UpdateArea(GraphUpdateObject o, INavmesh graph)
		{
			Bounds bounds = o.bounds;
			Rect r = Rect.MinMaxRect(bounds.min.x, bounds.min.z, bounds.max.x, bounds.max.z);
			IntRect r2 = new IntRect(Mathf.FloorToInt(bounds.min.x * 1000f), Mathf.FloorToInt(bounds.min.z * 1000f), Mathf.FloorToInt(bounds.max.x * 1000f), Mathf.FloorToInt(bounds.max.z * 1000f));
			Int3 a = new Int3(r2.xmin, 0, r2.ymin);
			Int3 b = new Int3(r2.xmin, 0, r2.ymax);
			Int3 c = new Int3(r2.xmax, 0, r2.ymin);
			Int3 d = new Int3(r2.xmax, 0, r2.ymax);
			graph.GetNodes(delegate(GraphNode _node)
			{
				TriangleMeshNode triangleMeshNode = _node as TriangleMeshNode;
				bool flag = false;
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				for (int i = 0; i < 3; i++)
				{
					Int3 vertex = triangleMeshNode.GetVertex(i);
					Vector3 vector = (Vector3)vertex;
					if (r2.Contains(vertex.x, vertex.z))
					{
						flag = true;
						break;
					}
					if (vector.x < r.xMin)
					{
						num++;
					}
					if (vector.x > r.xMax)
					{
						num2++;
					}
					if (vector.z < r.yMin)
					{
						num3++;
					}
					if (vector.z > r.yMax)
					{
						num4++;
					}
				}
				if (!flag && (num == 3 || num2 == 3 || num3 == 3 || num4 == 3))
				{
					return true;
				}
				for (int j = 0; j < 3; j++)
				{
					int i2 = (j <= 1) ? (j + 1) : 0;
					Int3 vertex2 = triangleMeshNode.GetVertex(j);
					Int3 vertex3 = triangleMeshNode.GetVertex(i2);
					if (Polygon.Intersects(a, b, vertex2, vertex3))
					{
						flag = true;
						break;
					}
					if (Polygon.Intersects(a, c, vertex2, vertex3))
					{
						flag = true;
						break;
					}
					if (Polygon.Intersects(c, d, vertex2, vertex3))
					{
						flag = true;
						break;
					}
					if (Polygon.Intersects(d, b, vertex2, vertex3))
					{
						flag = true;
						break;
					}
				}
				if (triangleMeshNode.ContainsPoint(a) || triangleMeshNode.ContainsPoint(b) || triangleMeshNode.ContainsPoint(c) || triangleMeshNode.ContainsPoint(d))
				{
					flag = true;
				}
				if (!flag)
				{
					return true;
				}
				o.WillUpdateNode(triangleMeshNode);
				o.Apply(triangleMeshNode);
				return true;
			});
		}

		private static Vector3 ClosestPointOnNode(TriangleMeshNode node, Int3[] vertices, Vector3 pos)
		{
			return Polygon.ClosestPointOnTriangle((Vector3)vertices[node.v0], (Vector3)vertices[node.v1], (Vector3)vertices[node.v2], pos);
		}

		[Obsolete("Use TriangleMeshNode.ContainsPoint instead")]
		public bool ContainsPoint(TriangleMeshNode node, Vector3 pos)
		{
			return Polygon.IsClockwise((Vector3)this.vertices[node.v0], (Vector3)this.vertices[node.v1], pos) && Polygon.IsClockwise((Vector3)this.vertices[node.v1], (Vector3)this.vertices[node.v2], pos) && Polygon.IsClockwise((Vector3)this.vertices[node.v2], (Vector3)this.vertices[node.v0], pos);
		}

		[Obsolete("Use TriangleMeshNode.ContainsPoint instead")]
		public static bool ContainsPoint(TriangleMeshNode node, Vector3 pos, Int3[] vertices)
		{
			if (!Polygon.IsClockwiseMargin((Vector3)vertices[node.v0], (Vector3)vertices[node.v1], (Vector3)vertices[node.v2]))
			{
				Debug.LogError("Noes!");
			}
			return Polygon.IsClockwiseMargin((Vector3)vertices[node.v0], (Vector3)vertices[node.v1], pos) && Polygon.IsClockwiseMargin((Vector3)vertices[node.v1], (Vector3)vertices[node.v2], pos) && Polygon.IsClockwiseMargin((Vector3)vertices[node.v2], (Vector3)vertices[node.v0], pos);
		}

		public void ScanInternal(string objMeshPath)
		{
			Mesh x = ObjImporter.ImportFile(objMeshPath);
			if (x == null)
			{
				Debug.LogError("Couldn't read .obj file at '" + objMeshPath + "'");
				return;
			}
			this.sourceMesh = x;
			base.ScanInternal();
		}

		public override void ScanInternal(OnScanStatus statusCallback)
		{
			if (this.sourceMesh == null)
			{
				return;
			}
			this.GenerateMatrix();
			Vector3[] vertices = this.sourceMesh.vertices;
			this.triangles = this.sourceMesh.triangles;
			TriangleMeshNode.SetNavmeshHolder(this.active.astarData.GetGraphIndex(this), this);
			this.GenerateNodes(vertices, this.triangles, out this.originalVertices, out this._vertices);
		}

		private void GenerateNodes(Vector3[] vectorVertices, int[] triangles, out Vector3[] originalVertices, out Int3[] vertices)
		{
			if (vectorVertices.Length == 0 || triangles.Length == 0)
			{
				originalVertices = vectorVertices;
				vertices = new Int3[0];
				this.nodes = new TriangleMeshNode[0];
				return;
			}
			vertices = new Int3[vectorVertices.Length];
			int num = 0;
			for (int i = 0; i < vertices.Length; i++)
			{
				vertices[i] = (Int3)this.matrix.MultiplyPoint3x4(vectorVertices[i]);
			}
			Dictionary<Int3, int> dictionary = new Dictionary<Int3, int>();
			int[] array = new int[vertices.Length];
			for (int j = 0; j < vertices.Length; j++)
			{
				if (!dictionary.ContainsKey(vertices[j]))
				{
					array[num] = j;
					dictionary.Add(vertices[j], num);
					num++;
				}
			}
			for (int k = 0; k < triangles.Length; k++)
			{
				Int3 key = vertices[triangles[k]];
				triangles[k] = dictionary[key];
			}
			Int3[] array2 = vertices;
			vertices = new Int3[num];
			originalVertices = new Vector3[num];
			for (int l = 0; l < num; l++)
			{
				vertices[l] = array2[array[l]];
				originalVertices[l] = vectorVertices[array[l]];
			}
			this.nodes = new TriangleMeshNode[triangles.Length / 3];
			int graphIndex = this.active.astarData.GetGraphIndex(this);
			for (int m = 0; m < this.nodes.Length; m++)
			{
				this.nodes[m] = new TriangleMeshNode(this.active);
				TriangleMeshNode triangleMeshNode = this.nodes[m];
				triangleMeshNode.GraphIndex = (uint)graphIndex;
				triangleMeshNode.Penalty = this.initialPenalty;
				triangleMeshNode.Walkable = true;
				triangleMeshNode.v0 = triangles[m * 3];
				triangleMeshNode.v1 = triangles[m * 3 + 1];
				triangleMeshNode.v2 = triangles[m * 3 + 2];
				if (!Polygon.IsClockwise(vertices[triangleMeshNode.v0], vertices[triangleMeshNode.v1], vertices[triangleMeshNode.v2]))
				{
					int v = triangleMeshNode.v0;
					triangleMeshNode.v0 = triangleMeshNode.v2;
					triangleMeshNode.v2 = v;
				}
				if (Polygon.IsColinear(vertices[triangleMeshNode.v0], vertices[triangleMeshNode.v1], vertices[triangleMeshNode.v2]))
				{
					Debug.DrawLine((Vector3)vertices[triangleMeshNode.v0], (Vector3)vertices[triangleMeshNode.v1], Color.red);
					Debug.DrawLine((Vector3)vertices[triangleMeshNode.v1], (Vector3)vertices[triangleMeshNode.v2], Color.red);
					Debug.DrawLine((Vector3)vertices[triangleMeshNode.v2], (Vector3)vertices[triangleMeshNode.v0], Color.red);
				}
				triangleMeshNode.UpdatePositionFromVertices();
			}
			Dictionary<Int2, TriangleMeshNode> dictionary2 = new Dictionary<Int2, TriangleMeshNode>();
			int n = 0;
			int num2 = 0;
			while (n < triangles.Length)
			{
				dictionary2[new Int2(triangles[n], triangles[n + 1])] = this.nodes[num2];
				dictionary2[new Int2(triangles[n + 1], triangles[n + 2])] = this.nodes[num2];
				dictionary2[new Int2(triangles[n + 2], triangles[n])] = this.nodes[num2];
				num2++;
				n += 3;
			}
			List<MeshNode> list = new List<MeshNode>();
			List<uint> list2 = new List<uint>();
			int num3 = 0;
			int num4 = 0;
			while (num3 < triangles.Length)
			{
				list.Clear();
				list2.Clear();
				TriangleMeshNode triangleMeshNode2 = this.nodes[num4];
				for (int num5 = 0; num5 < 3; num5++)
				{
					TriangleMeshNode triangleMeshNode3;
					if (dictionary2.TryGetValue(new Int2(triangles[num3 + (num5 + 1) % 3], triangles[num3 + num5]), out triangleMeshNode3))
					{
						list.Add(triangleMeshNode3);
						list2.Add((uint)(triangleMeshNode2.position - triangleMeshNode3.position).costMagnitude);
					}
				}
				triangleMeshNode2.connections = list.ToArray();
				triangleMeshNode2.connectionCosts = list2.ToArray();
				num4++;
				num3 += 3;
			}
			NavMeshGraph.RebuildBBTree(this);
		}

		public static void RebuildBBTree(NavMeshGraph graph)
		{
			BBTree bBTree = graph.bbTree;
			bBTree = (bBTree ?? new BBTree());
			bBTree.RebuildFrom(graph.nodes);
			graph.bbTree = bBTree;
		}

		public void PostProcess()
		{
		}

		public override void OnDrawGizmos(bool drawNodes)
		{
			if (!drawNodes)
			{
				return;
			}
			Matrix4x4 matrix = this.matrix;
			this.GenerateMatrix();
			if (this.nodes == null)
			{
			}
			if (this.nodes == null)
			{
				return;
			}
			if (matrix != this.matrix)
			{
				this.RelocateNodes(matrix, this.matrix);
			}
			PathHandler debugPathData = AstarPath.active.debugPathData;
			for (int i = 0; i < this.nodes.Length; i++)
			{
				TriangleMeshNode triangleMeshNode = this.nodes[i];
				Gizmos.color = this.NodeColor(triangleMeshNode, AstarPath.active.debugPathData);
				if (triangleMeshNode.Walkable)
				{
					if (AstarPath.active.showSearchTree && debugPathData != null && debugPathData.GetPathNode(triangleMeshNode).parent != null)
					{
						Gizmos.DrawLine((Vector3)triangleMeshNode.position, (Vector3)debugPathData.GetPathNode(triangleMeshNode).parent.node.position);
					}
					else
					{
						for (int j = 0; j < triangleMeshNode.connections.Length; j++)
						{
							Gizmos.DrawLine((Vector3)triangleMeshNode.position, Vector3.Lerp((Vector3)triangleMeshNode.position, (Vector3)triangleMeshNode.connections[j].position, 0.45f));
						}
					}
					Gizmos.color = AstarColor.MeshEdgeColor;
				}
				else
				{
					Gizmos.color = AstarColor.UnwalkableNode;
				}
				Gizmos.DrawLine((Vector3)this.vertices[triangleMeshNode.v0], (Vector3)this.vertices[triangleMeshNode.v1]);
				Gizmos.DrawLine((Vector3)this.vertices[triangleMeshNode.v1], (Vector3)this.vertices[triangleMeshNode.v2]);
				Gizmos.DrawLine((Vector3)this.vertices[triangleMeshNode.v2], (Vector3)this.vertices[triangleMeshNode.v0]);
			}
		}

		public override void DeserializeExtraInfo(GraphSerializationContext ctx)
		{
			uint graphIndex = (uint)ctx.graphIndex;
			TriangleMeshNode.SetNavmeshHolder((int)graphIndex, this);
			int num = ctx.reader.ReadInt32();
			int num2 = ctx.reader.ReadInt32();
			if (num == -1)
			{
				this.nodes = new TriangleMeshNode[0];
				this._vertices = new Int3[0];
				this.originalVertices = new Vector3[0];
			}
			this.nodes = new TriangleMeshNode[num];
			this._vertices = new Int3[num2];
			this.originalVertices = new Vector3[num2];
			for (int i = 0; i < num2; i++)
			{
				this._vertices[i] = new Int3(ctx.reader.ReadInt32(), ctx.reader.ReadInt32(), ctx.reader.ReadInt32());
				this.originalVertices[i] = new Vector3(ctx.reader.ReadSingle(), ctx.reader.ReadSingle(), ctx.reader.ReadSingle());
			}
			this.bbTree = new BBTree();
			for (int j = 0; j < num; j++)
			{
				this.nodes[j] = new TriangleMeshNode(this.active);
				TriangleMeshNode triangleMeshNode = this.nodes[j];
				triangleMeshNode.DeserializeNode(ctx);
				triangleMeshNode.UpdatePositionFromVertices();
			}
			this.bbTree.RebuildFrom(this.nodes);
		}

		public override void SerializeExtraInfo(GraphSerializationContext ctx)
		{
			if (this.nodes == null || this.originalVertices == null || this._vertices == null || this.originalVertices.Length != this._vertices.Length)
			{
				ctx.writer.Write(-1);
				ctx.writer.Write(-1);
				return;
			}
			ctx.writer.Write(this.nodes.Length);
			ctx.writer.Write(this._vertices.Length);
			for (int i = 0; i < this._vertices.Length; i++)
			{
				ctx.writer.Write(this._vertices[i].x);
				ctx.writer.Write(this._vertices[i].y);
				ctx.writer.Write(this._vertices[i].z);
				ctx.writer.Write(this.originalVertices[i].x);
				ctx.writer.Write(this.originalVertices[i].y);
				ctx.writer.Write(this.originalVertices[i].z);
			}
			for (int j = 0; j < this.nodes.Length; j++)
			{
				this.nodes[j].SerializeNode(ctx);
			}
		}
	}
}
