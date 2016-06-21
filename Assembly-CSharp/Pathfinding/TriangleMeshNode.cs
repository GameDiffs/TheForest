using Pathfinding.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
	public class TriangleMeshNode : MeshNode
	{
		public int v0;

		public int v1;

		public int v2;

		protected static INavmeshHolder[] _navmeshHolders = new INavmeshHolder[0];

		public TriangleMeshNode(AstarPath astar) : base(astar)
		{
		}

		public static INavmeshHolder GetNavmeshHolder(uint graphIndex)
		{
			return TriangleMeshNode._navmeshHolders[(int)graphIndex];
		}

		public static void SetNavmeshHolder(int graphIndex, INavmeshHolder graph)
		{
			if (TriangleMeshNode._navmeshHolders.Length <= graphIndex)
			{
				INavmeshHolder[] array = new INavmeshHolder[graphIndex + 1];
				for (int i = 0; i < TriangleMeshNode._navmeshHolders.Length; i++)
				{
					array[i] = TriangleMeshNode._navmeshHolders[i];
				}
				TriangleMeshNode._navmeshHolders = array;
			}
			TriangleMeshNode._navmeshHolders[graphIndex] = graph;
		}

		public void UpdatePositionFromVertices()
		{
			INavmeshHolder navmeshHolder = TriangleMeshNode.GetNavmeshHolder(base.GraphIndex);
			this.position = (navmeshHolder.GetVertex(this.v0) + navmeshHolder.GetVertex(this.v1) + navmeshHolder.GetVertex(this.v2)) * 0.333333f;
		}

		public int GetVertexIndex(int i)
		{
			return (i != 0) ? ((i != 1) ? this.v2 : this.v1) : this.v0;
		}

		public int GetVertexArrayIndex(int i)
		{
			return TriangleMeshNode.GetNavmeshHolder(base.GraphIndex).GetVertexArrayIndex((i != 0) ? ((i != 1) ? this.v2 : this.v1) : this.v0);
		}

		public override Int3 GetVertex(int i)
		{
			return TriangleMeshNode.GetNavmeshHolder(base.GraphIndex).GetVertex(this.GetVertexIndex(i));
		}

		public override int GetVertexCount()
		{
			return 3;
		}

		public override Vector3 ClosestPointOnNode(Vector3 p)
		{
			INavmeshHolder navmeshHolder = TriangleMeshNode.GetNavmeshHolder(base.GraphIndex);
			return Polygon.ClosestPointOnTriangle((Vector3)navmeshHolder.GetVertex(this.v0), (Vector3)navmeshHolder.GetVertex(this.v1), (Vector3)navmeshHolder.GetVertex(this.v2), p);
		}

		public override Vector3 ClosestPointOnNodeXZ(Vector3 _p)
		{
			INavmeshHolder navmeshHolder = TriangleMeshNode.GetNavmeshHolder(base.GraphIndex);
			Int3 vertex = navmeshHolder.GetVertex(this.v0);
			Int3 vertex2 = navmeshHolder.GetVertex(this.v1);
			Int3 vertex3 = navmeshHolder.GetVertex(this.v2);
			Int3 point = (Int3)_p;
			int y = point.y;
			vertex.y = 0;
			vertex2.y = 0;
			vertex3.y = 0;
			point.y = 0;
			if ((long)(vertex2.x - vertex.x) * (long)(point.z - vertex.z) - (long)(point.x - vertex.x) * (long)(vertex2.z - vertex.z) > 0L)
			{
				float num = Mathf.Clamp01(AstarMath.NearestPointFactor(vertex, vertex2, point));
				return new Vector3((float)vertex.x + (float)(vertex2.x - vertex.x) * num, (float)y, (float)vertex.z + (float)(vertex2.z - vertex.z) * num) * 0.001f;
			}
			if ((long)(vertex3.x - vertex2.x) * (long)(point.z - vertex2.z) - (long)(point.x - vertex2.x) * (long)(vertex3.z - vertex2.z) > 0L)
			{
				float num2 = Mathf.Clamp01(AstarMath.NearestPointFactor(vertex2, vertex3, point));
				return new Vector3((float)vertex2.x + (float)(vertex3.x - vertex2.x) * num2, (float)y, (float)vertex2.z + (float)(vertex3.z - vertex2.z) * num2) * 0.001f;
			}
			if ((long)(vertex.x - vertex3.x) * (long)(point.z - vertex3.z) - (long)(point.x - vertex3.x) * (long)(vertex.z - vertex3.z) > 0L)
			{
				float num3 = Mathf.Clamp01(AstarMath.NearestPointFactor(vertex3, vertex, point));
				return new Vector3((float)vertex3.x + (float)(vertex.x - vertex3.x) * num3, (float)y, (float)vertex3.z + (float)(vertex.z - vertex3.z) * num3) * 0.001f;
			}
			return _p;
		}

		public override bool ContainsPoint(Int3 p)
		{
			INavmeshHolder navmeshHolder = TriangleMeshNode.GetNavmeshHolder(base.GraphIndex);
			Int3 vertex = navmeshHolder.GetVertex(this.v0);
			Int3 vertex2 = navmeshHolder.GetVertex(this.v1);
			Int3 vertex3 = navmeshHolder.GetVertex(this.v2);
			return (long)(vertex2.x - vertex.x) * (long)(p.z - vertex.z) - (long)(p.x - vertex.x) * (long)(vertex2.z - vertex.z) <= 0L && (long)(vertex3.x - vertex2.x) * (long)(p.z - vertex2.z) - (long)(p.x - vertex2.x) * (long)(vertex3.z - vertex2.z) <= 0L && (long)(vertex.x - vertex3.x) * (long)(p.z - vertex3.z) - (long)(p.x - vertex3.x) * (long)(vertex.z - vertex3.z) <= 0L;
		}

		public override void UpdateRecursiveG(Path path, PathNode pathNode, PathHandler handler)
		{
			base.UpdateG(path, pathNode);
			handler.PushNode(pathNode);
			if (this.connections == null)
			{
				return;
			}
			for (int i = 0; i < this.connections.Length; i++)
			{
				GraphNode graphNode = this.connections[i];
				PathNode pathNode2 = handler.GetPathNode(graphNode);
				if (pathNode2.parent == pathNode && pathNode2.pathID == handler.PathID)
				{
					graphNode.UpdateRecursiveG(path, pathNode2, handler);
				}
			}
		}

		public override void Open(Path path, PathNode pathNode, PathHandler handler)
		{
			if (this.connections == null)
			{
				return;
			}
			bool flag = pathNode.flag2;
			for (int i = this.connections.Length - 1; i >= 0; i--)
			{
				GraphNode graphNode = this.connections[i];
				if (path.CanTraverse(graphNode))
				{
					PathNode pathNode2 = handler.GetPathNode(graphNode);
					if (pathNode2 != pathNode.parent)
					{
						uint num = this.connectionCosts[i];
						if (flag || pathNode2.flag2)
						{
							num = path.GetConnectionSpecialCost(this, graphNode, num);
						}
						if (pathNode2.pathID != handler.PathID)
						{
							pathNode2.node = graphNode;
							pathNode2.parent = pathNode;
							pathNode2.pathID = handler.PathID;
							pathNode2.cost = num;
							pathNode2.H = path.CalculateHScore(graphNode);
							graphNode.UpdateG(path, pathNode2);
							handler.PushNode(pathNode2);
						}
						else if (pathNode.G + num + path.GetTraversalCost(graphNode) < pathNode2.G)
						{
							pathNode2.cost = num;
							pathNode2.parent = pathNode;
							graphNode.UpdateRecursiveG(path, pathNode2, handler);
						}
						else if (pathNode2.G + num + path.GetTraversalCost(this) < pathNode.G && graphNode.ContainsConnection(this))
						{
							pathNode.parent = pathNode2;
							pathNode.cost = num;
							this.UpdateRecursiveG(path, pathNode, handler);
						}
					}
				}
			}
		}

		public int SharedEdge(GraphNode other)
		{
			int result;
			int num;
			this.GetPortal(other, null, null, false, out result, out num);
			return result;
		}

		public override bool GetPortal(GraphNode _other, List<Vector3> left, List<Vector3> right, bool backwards)
		{
			int num;
			int num2;
			return this.GetPortal(_other, left, right, backwards, out num, out num2);
		}

		public bool GetPortal(GraphNode _other, List<Vector3> left, List<Vector3> right, bool backwards, out int aIndex, out int bIndex)
		{
			aIndex = -1;
			bIndex = -1;
			if (_other.GraphIndex != base.GraphIndex)
			{
				return false;
			}
			TriangleMeshNode triangleMeshNode = _other as TriangleMeshNode;
			int num = this.GetVertexIndex(0) >> 12 & 524287;
			int num2 = triangleMeshNode.GetVertexIndex(0) >> 12 & 524287;
			if (num != num2 && TriangleMeshNode.GetNavmeshHolder(base.GraphIndex) is RecastGraph)
			{
				for (int i = 0; i < this.connections.Length; i++)
				{
					if (this.connections[i].GraphIndex != base.GraphIndex)
					{
						NodeLink3Node nodeLink3Node = this.connections[i] as NodeLink3Node;
						if (nodeLink3Node != null && nodeLink3Node.GetOther(this) == triangleMeshNode && left != null)
						{
							nodeLink3Node.GetPortal(triangleMeshNode, left, right, false);
							return true;
						}
					}
				}
				INavmeshHolder navmeshHolder = TriangleMeshNode.GetNavmeshHolder(base.GraphIndex);
				int num3;
				int num4;
				navmeshHolder.GetTileCoordinates(num, out num3, out num4);
				int num5;
				int num6;
				navmeshHolder.GetTileCoordinates(num2, out num5, out num6);
				int num7;
				if (Math.Abs(num3 - num5) == 1)
				{
					num7 = 0;
				}
				else
				{
					if (Math.Abs(num4 - num6) != 1)
					{
						throw new Exception(string.Concat(new object[]
						{
							"Tiles not adjacent (",
							num3,
							", ",
							num4,
							") (",
							num5,
							", ",
							num6,
							")"
						}));
					}
					num7 = 2;
				}
				int vertexCount = this.GetVertexCount();
				int vertexCount2 = triangleMeshNode.GetVertexCount();
				int num8 = -1;
				int num9 = -1;
				for (int j = 0; j < vertexCount; j++)
				{
					int num10 = this.GetVertex(j)[num7];
					for (int k = 0; k < vertexCount2; k++)
					{
						if (num10 == triangleMeshNode.GetVertex((k + 1) % vertexCount2)[num7] && this.GetVertex((j + 1) % vertexCount)[num7] == triangleMeshNode.GetVertex(k)[num7])
						{
							num8 = j;
							num9 = k;
							j = vertexCount;
							break;
						}
					}
				}
				aIndex = num8;
				bIndex = num9;
				if (num8 != -1)
				{
					Int3 vertex = this.GetVertex(num8);
					Int3 vertex2 = this.GetVertex((num8 + 1) % vertexCount);
					int i2 = (num7 != 2) ? 2 : 0;
					int num11 = Math.Min(vertex[i2], vertex2[i2]);
					int num12 = Math.Max(vertex[i2], vertex2[i2]);
					num11 = Math.Max(num11, Math.Min(triangleMeshNode.GetVertex(num9)[i2], triangleMeshNode.GetVertex((num9 + 1) % vertexCount2)[i2]));
					num12 = Math.Min(num12, Math.Max(triangleMeshNode.GetVertex(num9)[i2], triangleMeshNode.GetVertex((num9 + 1) % vertexCount2)[i2]));
					if (vertex[i2] < vertex2[i2])
					{
						vertex[i2] = num11;
						vertex2[i2] = num12;
					}
					else
					{
						vertex[i2] = num12;
						vertex2[i2] = num11;
					}
					if (left != null)
					{
						left.Add((Vector3)vertex);
						right.Add((Vector3)vertex2);
					}
					return true;
				}
			}
			else if (!backwards)
			{
				int num13 = -1;
				int num14 = -1;
				int vertexCount3 = this.GetVertexCount();
				int vertexCount4 = triangleMeshNode.GetVertexCount();
				for (int l = 0; l < vertexCount3; l++)
				{
					int vertexIndex = this.GetVertexIndex(l);
					for (int m = 0; m < vertexCount4; m++)
					{
						if (vertexIndex == triangleMeshNode.GetVertexIndex((m + 1) % vertexCount4) && this.GetVertexIndex((l + 1) % vertexCount3) == triangleMeshNode.GetVertexIndex(m))
						{
							num13 = l;
							num14 = m;
							l = vertexCount3;
							break;
						}
					}
				}
				aIndex = num13;
				bIndex = num14;
				if (num13 == -1)
				{
					for (int n = 0; n < this.connections.Length; n++)
					{
						if (this.connections[n].GraphIndex != base.GraphIndex)
						{
							NodeLink3Node nodeLink3Node2 = this.connections[n] as NodeLink3Node;
							if (nodeLink3Node2 != null && nodeLink3Node2.GetOther(this) == triangleMeshNode && left != null)
							{
								nodeLink3Node2.GetPortal(triangleMeshNode, left, right, false);
								return true;
							}
						}
					}
					return false;
				}
				if (left != null)
				{
					left.Add((Vector3)this.GetVertex(num13));
					right.Add((Vector3)this.GetVertex((num13 + 1) % vertexCount3));
				}
			}
			return true;
		}

		public override void SerializeNode(GraphSerializationContext ctx)
		{
			base.SerializeNode(ctx);
			ctx.writer.Write(this.v0);
			ctx.writer.Write(this.v1);
			ctx.writer.Write(this.v2);
		}

		public override void DeserializeNode(GraphSerializationContext ctx)
		{
			base.DeserializeNode(ctx);
			this.v0 = ctx.reader.ReadInt32();
			this.v1 = ctx.reader.ReadInt32();
			this.v2 = ctx.reader.ReadInt32();
		}
	}
}
