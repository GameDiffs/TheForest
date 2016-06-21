using System;
using UnityEngine;

namespace Pathfinding
{
	public struct NNInfo
	{
		public GraphNode node;

		public GraphNode constrainedNode;

		public Vector3 clampedPosition;

		public Vector3 constClampedPosition;

		public NNInfo(GraphNode node)
		{
			this.node = node;
			this.constrainedNode = null;
			this.clampedPosition = Vector3.zero;
			this.constClampedPosition = Vector3.zero;
			this.UpdateInfo();
		}

		public void SetConstrained(GraphNode constrainedNode, Vector3 clampedPosition)
		{
			this.constrainedNode = constrainedNode;
			this.constClampedPosition = clampedPosition;
		}

		public void UpdateInfo()
		{
			this.clampedPosition = ((this.node == null) ? Vector3.zero : ((Vector3)this.node.position));
			this.constClampedPosition = ((this.constrainedNode == null) ? Vector3.zero : ((Vector3)this.constrainedNode.position));
		}

		public static explicit operator Vector3(NNInfo ob)
		{
			return ob.clampedPosition;
		}

		public static explicit operator GraphNode(NNInfo ob)
		{
			return ob.node;
		}

		public static explicit operator NNInfo(GraphNode ob)
		{
			return new NNInfo(ob);
		}
	}
}
