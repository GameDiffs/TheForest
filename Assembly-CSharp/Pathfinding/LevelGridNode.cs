using Pathfinding.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
	public class LevelGridNode : GridNodeBase
	{
		private const int GridFlagsWalkableErosionOffset = 8;

		private const int GridFlagsWalkableErosionMask = 256;

		private const int GridFlagsWalkableTmpOffset = 9;

		private const int GridFlagsWalkableTmpMask = 512;

		public const int NoConnection = 255;

		public const int ConnectionMask = 255;

		private const int ConnectionStride = 8;

		public const int MaxLayerCount = 255;

		private static LayerGridGraph[] _gridGraphs = new LayerGridGraph[0];

		protected ushort gridFlags;

		protected uint gridConnections;

		protected static LayerGridGraph[] gridGraphs;

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

		public LevelGridNode(AstarPath astar) : base(astar)
		{
		}

		public static LayerGridGraph GetGridGraph(uint graphIndex)
		{
			return LevelGridNode._gridGraphs[(int)graphIndex];
		}

		public static void SetGridGraph(int graphIndex, LayerGridGraph graph)
		{
			if (LevelGridNode._gridGraphs.Length <= graphIndex)
			{
				LayerGridGraph[] array = new LayerGridGraph[graphIndex + 1];
				for (int i = 0; i < LevelGridNode._gridGraphs.Length; i++)
				{
					array[i] = LevelGridNode._gridGraphs[i];
				}
				LevelGridNode._gridGraphs = array;
			}
			LevelGridNode._gridGraphs[graphIndex] = graph;
		}

		public void ResetAllGridConnections()
		{
			this.gridConnections = 4294967295u;
		}

		public bool HasAnyGridConnections()
		{
			return this.gridConnections != 4294967295u;
		}

		public void SetPosition(Int3 position)
		{
			this.position = position;
		}

		public override void ClearConnections(bool alsoReverse)
		{
			if (alsoReverse)
			{
				LayerGridGraph gridGraph = LevelGridNode.GetGridGraph(base.GraphIndex);
				int[] neighbourOffsets = gridGraph.neighbourOffsets;
				LevelGridNode[] nodes = gridGraph.nodes;
				for (int i = 0; i < 4; i++)
				{
					int connectionValue = this.GetConnectionValue(i);
					if (connectionValue != 255)
					{
						LevelGridNode levelGridNode = nodes[base.NodeInGridIndex + neighbourOffsets[i] + gridGraph.lastScannedWidth * gridGraph.lastScannedDepth * connectionValue];
						if (levelGridNode != null)
						{
							levelGridNode.SetConnectionValue((i >= 4) ? 7 : ((i + 2) % 4), 255);
						}
					}
				}
			}
			this.ResetAllGridConnections();
		}

		public override void GetConnections(GraphNodeDelegate del)
		{
			int nodeInGridIndex = base.NodeInGridIndex;
			LayerGridGraph gridGraph = LevelGridNode.GetGridGraph(base.GraphIndex);
			int[] neighbourOffsets = gridGraph.neighbourOffsets;
			LevelGridNode[] nodes = gridGraph.nodes;
			for (int i = 0; i < 4; i++)
			{
				int connectionValue = this.GetConnectionValue(i);
				if (connectionValue != 255)
				{
					LevelGridNode levelGridNode = nodes[nodeInGridIndex + neighbourOffsets[i] + gridGraph.lastScannedWidth * gridGraph.lastScannedDepth * connectionValue];
					if (levelGridNode != null)
					{
						del(levelGridNode);
					}
				}
			}
		}

		public override void FloodFill(Stack<GraphNode> stack, uint region)
		{
			int nodeInGridIndex = base.NodeInGridIndex;
			LayerGridGraph gridGraph = LevelGridNode.GetGridGraph(base.GraphIndex);
			int[] neighbourOffsets = gridGraph.neighbourOffsets;
			LevelGridNode[] nodes = gridGraph.nodes;
			for (int i = 0; i < 4; i++)
			{
				int connectionValue = this.GetConnectionValue(i);
				if (connectionValue != 255)
				{
					LevelGridNode levelGridNode = nodes[nodeInGridIndex + neighbourOffsets[i] + gridGraph.lastScannedWidth * gridGraph.lastScannedDepth * connectionValue];
					if (levelGridNode != null && levelGridNode.Area != region)
					{
						levelGridNode.Area = region;
						stack.Push(levelGridNode);
					}
				}
			}
		}

		public override void AddConnection(GraphNode node, uint cost)
		{
			throw new NotImplementedException("Layered Grid Nodes do not have support for adding manual connections");
		}

		public override void RemoveConnection(GraphNode node)
		{
			throw new NotImplementedException("Layered Grid Nodes do not have support for adding manual connections");
		}

		public bool GetConnection(int i)
		{
			return (this.gridConnections >> i * 8 & 255u) != 255u;
		}

		public void SetConnectionValue(int dir, int value)
		{
			this.gridConnections = ((this.gridConnections & ~(255u << dir * 8)) | (uint)((uint)value << dir * 8));
		}

		public int GetConnectionValue(int dir)
		{
			return (int)(this.gridConnections >> dir * 8 & 255u);
		}

		public override bool GetPortal(GraphNode other, List<Vector3> left, List<Vector3> right, bool backwards)
		{
			if (backwards)
			{
				return true;
			}
			LayerGridGraph gridGraph = LevelGridNode.GetGridGraph(base.GraphIndex);
			int[] neighbourOffsets = gridGraph.neighbourOffsets;
			LevelGridNode[] nodes = gridGraph.nodes;
			int nodeInGridIndex = base.NodeInGridIndex;
			for (int i = 0; i < 4; i++)
			{
				int connectionValue = this.GetConnectionValue(i);
				if (connectionValue != 255 && other == nodes[nodeInGridIndex + neighbourOffsets[i] + gridGraph.lastScannedWidth * gridGraph.lastScannedDepth * connectionValue])
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
			return false;
		}

		public override void UpdateRecursiveG(Path path, PathNode pathNode, PathHandler handler)
		{
			handler.PushNode(pathNode);
			base.UpdateG(path, pathNode);
			LayerGridGraph gridGraph = LevelGridNode.GetGridGraph(base.GraphIndex);
			int[] neighbourOffsets = gridGraph.neighbourOffsets;
			LevelGridNode[] nodes = gridGraph.nodes;
			int nodeInGridIndex = base.NodeInGridIndex;
			for (int i = 0; i < 4; i++)
			{
				int connectionValue = this.GetConnectionValue(i);
				if (connectionValue != 255)
				{
					LevelGridNode levelGridNode = nodes[nodeInGridIndex + neighbourOffsets[i] + gridGraph.lastScannedWidth * gridGraph.lastScannedDepth * connectionValue];
					PathNode pathNode2 = handler.GetPathNode(levelGridNode);
					if (pathNode2 != null && pathNode2.parent == pathNode && pathNode2.pathID == handler.PathID)
					{
						levelGridNode.UpdateRecursiveG(path, pathNode2, handler);
					}
				}
			}
		}

		public override void Open(Path path, PathNode pathNode, PathHandler handler)
		{
			LayerGridGraph gridGraph = LevelGridNode.GetGridGraph(base.GraphIndex);
			int[] neighbourOffsets = gridGraph.neighbourOffsets;
			uint[] neighbourCosts = gridGraph.neighbourCosts;
			LevelGridNode[] nodes = gridGraph.nodes;
			int nodeInGridIndex = base.NodeInGridIndex;
			for (int i = 0; i < 4; i++)
			{
				int connectionValue = this.GetConnectionValue(i);
				if (connectionValue != 255)
				{
					GraphNode graphNode = nodes[nodeInGridIndex + neighbourOffsets[i] + gridGraph.lastScannedWidth * gridGraph.lastScannedDepth * connectionValue];
					if (path.CanTraverse(graphNode))
					{
						PathNode pathNode2 = handler.GetPathNode(graphNode);
						if (pathNode2.pathID != handler.PathID)
						{
							pathNode2.parent = pathNode;
							pathNode2.pathID = handler.PathID;
							pathNode2.cost = neighbourCosts[i];
							pathNode2.H = path.CalculateHScore(graphNode);
							graphNode.UpdateG(path, pathNode2);
							handler.PushNode(pathNode2);
						}
						else
						{
							uint num = neighbourCosts[i];
							if (pathNode.G + num + path.GetTraversalCost(graphNode) < pathNode2.G)
							{
								pathNode2.cost = num;
								pathNode2.parent = pathNode;
								graphNode.UpdateRecursiveG(path, pathNode2, handler);
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
			}
		}

		public override void SerializeNode(GraphSerializationContext ctx)
		{
			base.SerializeNode(ctx);
			ctx.writer.Write(this.position.x);
			ctx.writer.Write(this.position.y);
			ctx.writer.Write(this.position.z);
			ctx.writer.Write(this.gridFlags);
			ctx.writer.Write(this.gridConnections);
		}

		public override void DeserializeNode(GraphSerializationContext ctx)
		{
			base.DeserializeNode(ctx);
			this.position = new Int3(ctx.reader.ReadInt32(), ctx.reader.ReadInt32(), ctx.reader.ReadInt32());
			this.gridFlags = ctx.reader.ReadUInt16();
			this.gridConnections = ctx.reader.ReadUInt32();
		}
	}
}
