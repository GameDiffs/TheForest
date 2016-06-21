using Pathfinding.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
	public class GridNode : GridNodeBase
	{
		private const int GridFlagsConnectionOffset = 0;

		private const int GridFlagsConnectionBit0 = 1;

		private const int GridFlagsConnectionMask = 255;

		private const int GridFlagsWalkableErosionOffset = 8;

		private const int GridFlagsWalkableErosionMask = 256;

		private const int GridFlagsWalkableTmpOffset = 9;

		private const int GridFlagsWalkableTmpMask = 512;

		private const int GridFlagsEdgeNodeOffset = 10;

		private const int GridFlagsEdgeNodeMask = 1024;

		private static GridGraph[] _gridGraphs = new GridGraph[0];

		public GraphNode[] connections;

		public uint[] connectionCosts;

		protected ushort gridFlags;

		internal ushort InternalGridFlags
		{
			get
			{
				return this.gridFlags;
			}
			set
			{
				this.gridFlags = value;
			}
		}

		public bool EdgeNode
		{
			get
			{
				return (this.gridFlags & 1024) != 0;
			}
			set
			{
				this.gridFlags = (ushort)(((int)this.gridFlags & -1025) | ((!value) ? 0 : 1024));
			}
		}

		public bool WalkableErosion
		{
			get
			{
				return (this.gridFlags & 256) != 0;
			}
			set
			{
				this.gridFlags = (ushort)(((int)this.gridFlags & -257) | ((!value) ? 0 : 256));
			}
		}

		public bool TmpWalkable
		{
			get
			{
				return (this.gridFlags & 512) != 0;
			}
			set
			{
				this.gridFlags = (ushort)(((int)this.gridFlags & -513) | ((!value) ? 0 : 512));
			}
		}

		public GridNode(AstarPath astar) : base(astar)
		{
		}

		public static GridGraph GetGridGraph(uint graphIndex)
		{
			return GridNode._gridGraphs[(int)graphIndex];
		}

		public static void SetGridGraph(int graphIndex, GridGraph graph)
		{
			if (GridNode._gridGraphs.Length <= graphIndex)
			{
				GridGraph[] array = new GridGraph[graphIndex + 1];
				for (int i = 0; i < GridNode._gridGraphs.Length; i++)
				{
					array[i] = GridNode._gridGraphs[i];
				}
				GridNode._gridGraphs = array;
			}
			GridNode._gridGraphs[graphIndex] = graph;
		}

		public bool GetConnectionInternal(int dir)
		{
			return (this.gridFlags >> dir & 1) != 0;
		}

		public void SetConnectionInternal(int dir, bool value)
		{
			this.gridFlags = (ushort)(((int)this.gridFlags & ~(1 << dir)) | ((!value) ? 0 : 1) << (dir & 31));
		}

		public void ResetConnectionsInternal()
		{
			this.gridFlags = (ushort)((int)this.gridFlags & -256);
		}

		public override void ClearConnections(bool alsoReverse)
		{
			if (alsoReverse)
			{
				GridGraph gridGraph = GridNode.GetGridGraph(base.GraphIndex);
				for (int i = 0; i < 8; i++)
				{
					GridNode nodeConnection = gridGraph.GetNodeConnection(this, i);
					if (nodeConnection != null)
					{
						nodeConnection.SetConnectionInternal((i >= 4) ? 7 : ((i + 2) % 4), false);
					}
				}
			}
			this.ResetConnectionsInternal();
			if (alsoReverse && this.connections != null)
			{
				for (int j = 0; j < this.connections.Length; j++)
				{
					this.connections[j].RemoveConnection(this);
				}
			}
			this.connections = null;
			this.connectionCosts = null;
		}

		public override void GetConnections(GraphNodeDelegate del)
		{
			GridGraph gridGraph = GridNode.GetGridGraph(base.GraphIndex);
			int[] neighbourOffsets = gridGraph.neighbourOffsets;
			GridNode[] nodes = gridGraph.nodes;
			for (int i = 0; i < 8; i++)
			{
				if (this.GetConnectionInternal(i))
				{
					GridNode gridNode = nodes[this.nodeInGridIndex + neighbourOffsets[i]];
					if (gridNode != null)
					{
						del(gridNode);
					}
				}
			}
			if (this.connections != null)
			{
				for (int j = 0; j < this.connections.Length; j++)
				{
					del(this.connections[j]);
				}
			}
		}

		public override bool GetPortal(GraphNode other, List<Vector3> left, List<Vector3> right, bool backwards)
		{
			if (backwards)
			{
				return true;
			}
			GridGraph gridGraph = GridNode.GetGridGraph(base.GraphIndex);
			int[] neighbourOffsets = gridGraph.neighbourOffsets;
			GridNode[] nodes = gridGraph.nodes;
			for (int i = 0; i < 4; i++)
			{
				if (this.GetConnectionInternal(i) && other == nodes[this.nodeInGridIndex + neighbourOffsets[i]])
				{
					Vector3 a = (Vector3)(this.position + other.position) * 0.5f;
					Vector3 vector = Vector3.Cross(gridGraph.collision.up, (Vector3)(other.position - this.position));
					vector.Normalize();
					vector *= gridGraph.nodeSize * 0.5f;
					left.Add(a - vector);
					right.Add(a + vector);
					return true;
				}
			}
			for (int j = 4; j < 8; j++)
			{
				if (this.GetConnectionInternal(j) && other == nodes[this.nodeInGridIndex + neighbourOffsets[j]])
				{
					bool flag = false;
					bool flag2 = false;
					if (this.GetConnectionInternal(j - 4))
					{
						GridNode gridNode = nodes[this.nodeInGridIndex + neighbourOffsets[j - 4]];
						if (gridNode.Walkable && gridNode.GetConnectionInternal((j - 4 + 1) % 4))
						{
							flag = true;
						}
					}
					if (this.GetConnectionInternal((j - 4 + 1) % 4))
					{
						GridNode gridNode2 = nodes[this.nodeInGridIndex + neighbourOffsets[(j - 4 + 1) % 4]];
						if (gridNode2.Walkable && gridNode2.GetConnectionInternal(j - 4))
						{
							flag2 = true;
						}
					}
					Vector3 a2 = (Vector3)(this.position + other.position) * 0.5f;
					Vector3 vector2 = Vector3.Cross(gridGraph.collision.up, (Vector3)(other.position - this.position));
					vector2.Normalize();
					vector2 *= gridGraph.nodeSize * 1.4142f;
					left.Add(a2 - ((!flag2) ? Vector3.zero : vector2));
					right.Add(a2 + ((!flag) ? Vector3.zero : vector2));
					return true;
				}
			}
			return false;
		}

		public override void FloodFill(Stack<GraphNode> stack, uint region)
		{
			GridGraph gridGraph = GridNode.GetGridGraph(base.GraphIndex);
			int[] neighbourOffsets = gridGraph.neighbourOffsets;
			GridNode[] nodes = gridGraph.nodes;
			for (int i = 0; i < 8; i++)
			{
				if (this.GetConnectionInternal(i))
				{
					GridNode gridNode = nodes[this.nodeInGridIndex + neighbourOffsets[i]];
					if (gridNode != null && gridNode.Area != region)
					{
						gridNode.Area = region;
						stack.Push(gridNode);
					}
				}
			}
			if (this.connections != null)
			{
				for (int j = 0; j < this.connections.Length; j++)
				{
					GraphNode graphNode = this.connections[j];
					if (graphNode.Area != region)
					{
						graphNode.Area = region;
						stack.Push(graphNode);
					}
				}
			}
		}

		public override void AddConnection(GraphNode node, uint cost)
		{
			if (this.connections != null)
			{
				for (int i = 0; i < this.connections.Length; i++)
				{
					if (this.connections[i] == node)
					{
						this.connectionCosts[i] = cost;
						return;
					}
				}
			}
			int num = (this.connections == null) ? 0 : this.connections.Length;
			GraphNode[] array = new GraphNode[num + 1];
			uint[] array2 = new uint[num + 1];
			for (int j = 0; j < num; j++)
			{
				array[j] = this.connections[j];
				array2[j] = this.connectionCosts[j];
			}
			array[num] = node;
			array2[num] = cost;
			this.connections = array;
			this.connectionCosts = array2;
		}

		public override void RemoveConnection(GraphNode node)
		{
			if (this.connections == null)
			{
				return;
			}
			for (int i = 0; i < this.connections.Length; i++)
			{
				if (this.connections[i] == node)
				{
					int num = this.connections.Length;
					GraphNode[] array = new GraphNode[num - 1];
					uint[] array2 = new uint[num - 1];
					for (int j = 0; j < i; j++)
					{
						array[j] = this.connections[j];
						array2[j] = this.connectionCosts[j];
					}
					for (int k = i + 1; k < num; k++)
					{
						array[k - 1] = this.connections[k];
						array2[k - 1] = this.connectionCosts[k];
					}
					this.connections = array;
					this.connectionCosts = array2;
					return;
				}
			}
		}

		public override void UpdateRecursiveG(Path path, PathNode pathNode, PathHandler handler)
		{
			GridGraph gridGraph = GridNode.GetGridGraph(base.GraphIndex);
			int[] neighbourOffsets = gridGraph.neighbourOffsets;
			GridNode[] nodes = gridGraph.nodes;
			base.UpdateG(path, pathNode);
			handler.PushNode(pathNode);
			ushort pathID = handler.PathID;
			for (int i = 0; i < 8; i++)
			{
				if (this.GetConnectionInternal(i))
				{
					GridNode gridNode = nodes[this.nodeInGridIndex + neighbourOffsets[i]];
					PathNode pathNode2 = handler.GetPathNode(gridNode);
					if (pathNode2.parent == pathNode && pathNode2.pathID == pathID)
					{
						gridNode.UpdateRecursiveG(path, pathNode2, handler);
					}
				}
			}
			if (this.connections != null)
			{
				for (int j = 0; j < this.connections.Length; j++)
				{
					GraphNode graphNode = this.connections[j];
					PathNode pathNode3 = handler.GetPathNode(graphNode);
					if (pathNode3.parent == pathNode && pathNode3.pathID == pathID)
					{
						graphNode.UpdateRecursiveG(path, pathNode3, handler);
					}
				}
			}
		}

		public override void Open(Path path, PathNode pathNode, PathHandler handler)
		{
			GridGraph gridGraph = GridNode.GetGridGraph(base.GraphIndex);
			ushort pathID = handler.PathID;
			int[] neighbourOffsets = gridGraph.neighbourOffsets;
			uint[] neighbourCosts = gridGraph.neighbourCosts;
			GridNode[] nodes = gridGraph.nodes;
			for (int i = 0; i < 8; i++)
			{
				if (this.GetConnectionInternal(i))
				{
					GridNode gridNode = nodes[this.nodeInGridIndex + neighbourOffsets[i]];
					if (path.CanTraverse(gridNode))
					{
						PathNode pathNode2 = handler.GetPathNode(gridNode);
						uint num = neighbourCosts[i];
						if (pathNode2.pathID != pathID)
						{
							pathNode2.parent = pathNode;
							pathNode2.pathID = pathID;
							pathNode2.cost = num;
							pathNode2.H = path.CalculateHScore(gridNode);
							gridNode.UpdateG(path, pathNode2);
							handler.PushNode(pathNode2);
						}
						else if (pathNode.G + num + path.GetTraversalCost(gridNode) < pathNode2.G)
						{
							pathNode2.cost = num;
							pathNode2.parent = pathNode;
							gridNode.UpdateRecursiveG(path, pathNode2, handler);
						}
						else if (pathNode2.G + num + path.GetTraversalCost(this) < pathNode.G)
						{
							pathNode.parent = pathNode2;
							pathNode.cost = num;
							this.UpdateRecursiveG(path, pathNode, handler);
						}
					}
				}
			}
			if (this.connections != null)
			{
				for (int j = 0; j < this.connections.Length; j++)
				{
					GraphNode graphNode = this.connections[j];
					if (path.CanTraverse(graphNode))
					{
						PathNode pathNode3 = handler.GetPathNode(graphNode);
						uint num2 = this.connectionCosts[j];
						if (pathNode3.pathID != pathID)
						{
							pathNode3.parent = pathNode;
							pathNode3.pathID = pathID;
							pathNode3.cost = num2;
							pathNode3.H = path.CalculateHScore(graphNode);
							graphNode.UpdateG(path, pathNode3);
							handler.PushNode(pathNode3);
						}
						else if (pathNode.G + num2 + path.GetTraversalCost(graphNode) < pathNode3.G)
						{
							pathNode3.cost = num2;
							pathNode3.parent = pathNode;
							graphNode.UpdateRecursiveG(path, pathNode3, handler);
						}
						else if (pathNode3.G + num2 + path.GetTraversalCost(this) < pathNode.G && graphNode.ContainsConnection(this))
						{
							pathNode.parent = pathNode3;
							pathNode.cost = num2;
							this.UpdateRecursiveG(path, pathNode, handler);
						}
					}
				}
			}
		}

		public override void SerializeNode(GraphSerializationContext ctx)
		{
			base.SerializeNode(ctx);
			ctx.writer.Write(this.position.x);
			ctx.writer.Write(this.position.y);
			ctx.writer.Write(this.position.z);
			ctx.writer.Write(this.gridFlags);
		}

		public override void DeserializeNode(GraphSerializationContext ctx)
		{
			base.DeserializeNode(ctx);
			this.position = new Int3(ctx.reader.ReadInt32(), ctx.reader.ReadInt32(), ctx.reader.ReadInt32());
			this.gridFlags = ctx.reader.ReadUInt16();
		}
	}
}
