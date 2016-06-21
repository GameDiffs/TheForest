using Pathfinding.Serialization;
using System;
using UnityEngine;

namespace Pathfinding
{
	public class PointNode : GraphNode
	{
		public GraphNode[] connections;

		public uint[] connectionCosts;

		public GameObject gameObject;

		public PointNode next;

		public PointNode(AstarPath astar) : base(astar)
		{
		}

		public void SetPosition(Int3 value)
		{
			this.position = value;
		}

		public override void GetConnections(GraphNodeDelegate del)
		{
			if (this.connections == null)
			{
				return;
			}
			for (int i = 0; i < this.connections.Length; i++)
			{
				del(this.connections[i]);
			}
		}

		public override void ClearConnections(bool alsoReverse)
		{
			if (alsoReverse && this.connections != null)
			{
				for (int i = 0; i < this.connections.Length; i++)
				{
					this.connections[i].RemoveConnection(this);
				}
			}
			this.connections = null;
			this.connectionCosts = null;
		}

		public override void UpdateRecursiveG(Path path, PathNode pathNode, PathHandler handler)
		{
			base.UpdateG(path, pathNode);
			handler.PushNode(pathNode);
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

		public override bool ContainsConnection(GraphNode node)
		{
			for (int i = 0; i < this.connections.Length; i++)
			{
				if (this.connections[i] == node)
				{
					return true;
				}
			}
			return false;
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

		public override void Open(Path path, PathNode pathNode, PathHandler handler)
		{
			if (this.connections == null)
			{
				return;
			}
			for (int i = 0; i < this.connections.Length; i++)
			{
				GraphNode graphNode = this.connections[i];
				if (path.CanTraverse(graphNode))
				{
					PathNode pathNode2 = handler.GetPathNode(graphNode);
					if (pathNode2.pathID != handler.PathID)
					{
						pathNode2.parent = pathNode;
						pathNode2.pathID = handler.PathID;
						pathNode2.cost = this.connectionCosts[i];
						pathNode2.H = path.CalculateHScore(graphNode);
						graphNode.UpdateG(path, pathNode2);
						handler.PushNode(pathNode2);
					}
					else
					{
						uint num = this.connectionCosts[i];
						if (pathNode.G + num + path.GetTraversalCost(graphNode) < pathNode2.G)
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

		public override void SerializeNode(GraphSerializationContext ctx)
		{
			base.SerializeNode(ctx);
			ctx.writer.Write(this.position.x);
			ctx.writer.Write(this.position.y);
			ctx.writer.Write(this.position.z);
		}

		public override void DeserializeNode(GraphSerializationContext ctx)
		{
			base.DeserializeNode(ctx);
			this.position = new Int3(ctx.reader.ReadInt32(), ctx.reader.ReadInt32(), ctx.reader.ReadInt32());
		}

		public override void SerializeReferences(GraphSerializationContext ctx)
		{
			if (this.connections == null)
			{
				ctx.writer.Write(-1);
			}
			else
			{
				ctx.writer.Write(this.connections.Length);
				for (int i = 0; i < this.connections.Length; i++)
				{
					ctx.writer.Write(ctx.GetNodeIdentifier(this.connections[i]));
					ctx.writer.Write(this.connectionCosts[i]);
				}
			}
		}

		public override void DeserializeReferences(GraphSerializationContext ctx)
		{
			int num = ctx.reader.ReadInt32();
			if (num == -1)
			{
				this.connections = null;
				this.connectionCosts = null;
			}
			else
			{
				this.connections = new GraphNode[num];
				this.connectionCosts = new uint[num];
				for (int i = 0; i < num; i++)
				{
					this.connections[i] = ctx.GetNodeFromIdentifier(ctx.reader.ReadInt32());
					this.connectionCosts[i] = ctx.reader.ReadUInt32();
				}
			}
		}
	}
}
