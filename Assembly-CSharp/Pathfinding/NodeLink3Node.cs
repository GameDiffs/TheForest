using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
	public class NodeLink3Node : PointNode
	{
		public NodeLink3 link;

		public Vector3 portalA;

		public Vector3 portalB;

		public NodeLink3Node(AstarPath active) : base(active)
		{
		}

		public override bool GetPortal(GraphNode other, List<Vector3> left, List<Vector3> right, bool backwards)
		{
			if (this.connections.Length < 2)
			{
				return false;
			}
			if (this.connections.Length != 2)
			{
				throw new Exception("Invalid NodeLink3Node. Expected 2 connections, found " + this.connections.Length);
			}
			if (left != null)
			{
				left.Add(this.portalA);
				right.Add(this.portalB);
			}
			return true;
		}

		public GraphNode GetOther(GraphNode a)
		{
			if (this.connections.Length < 2)
			{
				return null;
			}
			if (this.connections.Length != 2)
			{
				throw new Exception("Invalid NodeLink3Node. Expected 2 connections, found " + this.connections.Length);
			}
			return (a != this.connections[0]) ? (this.connections[0] as NodeLink3Node).GetOtherInternal(this) : (this.connections[1] as NodeLink3Node).GetOtherInternal(this);
		}

		private GraphNode GetOtherInternal(GraphNode a)
		{
			if (this.connections.Length < 2)
			{
				return null;
			}
			return (a != this.connections[0]) ? this.connections[0] : this.connections[1];
		}
	}
}
