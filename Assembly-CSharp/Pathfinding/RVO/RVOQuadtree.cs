using Pathfinding.RVO.Sampled;
using System;
using UnityEngine;

namespace Pathfinding.RVO
{
	public class RVOQuadtree
	{
		private struct Node
		{
			public int child00;

			public int child01;

			public int child10;

			public int child11;

			public byte count;

			public Agent linkedList;

			public void Add(Agent agent)
			{
				agent.next = this.linkedList;
				this.linkedList = agent;
			}

			public void Distribute(RVOQuadtree.Node[] nodes, Rect r)
			{
				Vector2 center = r.center;
				while (this.linkedList != null)
				{
					Agent next = this.linkedList.next;
					if (this.linkedList.position.x > center.x)
					{
						if (this.linkedList.position.z > center.y)
						{
							nodes[this.child11].Add(this.linkedList);
						}
						else
						{
							nodes[this.child10].Add(this.linkedList);
						}
					}
					else if (this.linkedList.position.z > center.y)
					{
						nodes[this.child01].Add(this.linkedList);
					}
					else
					{
						nodes[this.child00].Add(this.linkedList);
					}
					this.linkedList = next;
				}
				this.count = 0;
			}
		}

		private const int LeafSize = 15;

		private float maxRadius;

		private RVOQuadtree.Node[] nodes = new RVOQuadtree.Node[42];

		private int filledNodes = 1;

		private Rect bounds;

		public void Clear()
		{
			this.nodes[0] = default(RVOQuadtree.Node);
			this.filledNodes = 1;
			this.maxRadius = 0f;
		}

		public void SetBounds(Rect r)
		{
			this.bounds = r;
		}

		public int GetNodeIndex()
		{
			if (this.filledNodes == this.nodes.Length)
			{
				RVOQuadtree.Node[] array = new RVOQuadtree.Node[this.nodes.Length * 2];
				for (int i = 0; i < this.nodes.Length; i++)
				{
					array[i] = this.nodes[i];
				}
				this.nodes = array;
			}
			this.nodes[this.filledNodes] = default(RVOQuadtree.Node);
			this.nodes[this.filledNodes].child00 = this.filledNodes;
			this.filledNodes++;
			return this.filledNodes - 1;
		}

		public void Insert(Agent agent)
		{
			int num = 0;
			Rect r = this.bounds;
			Vector2 vector = new Vector2(agent.position.x, agent.position.z);
			agent.next = null;
			this.maxRadius = Math.Max(agent.radius, this.maxRadius);
			int num2 = 0;
			while (true)
			{
				num2++;
				if (this.nodes[num].child00 == num)
				{
					if (this.nodes[num].count < 15 || num2 > 10)
					{
						break;
					}
					RVOQuadtree.Node node = this.nodes[num];
					node.child00 = this.GetNodeIndex();
					node.child01 = this.GetNodeIndex();
					node.child10 = this.GetNodeIndex();
					node.child11 = this.GetNodeIndex();
					this.nodes[num] = node;
					this.nodes[num].Distribute(this.nodes, r);
				}
				if (this.nodes[num].child00 != num)
				{
					Vector2 center = r.center;
					if (vector.x > center.x)
					{
						if (vector.y > center.y)
						{
							num = this.nodes[num].child11;
							r = Rect.MinMaxRect(center.x, center.y, r.xMax, r.yMax);
						}
						else
						{
							num = this.nodes[num].child10;
							r = Rect.MinMaxRect(center.x, r.yMin, r.xMax, center.y);
						}
					}
					else if (vector.y > center.y)
					{
						num = this.nodes[num].child01;
						r = Rect.MinMaxRect(r.xMin, center.y, center.x, r.yMax);
					}
					else
					{
						num = this.nodes[num].child00;
						r = Rect.MinMaxRect(r.xMin, r.yMin, center.x, center.y);
					}
				}
			}
			this.nodes[num].Add(agent);
			RVOQuadtree.Node[] expr_9F_cp_0 = this.nodes;
			int expr_9F_cp_1 = num;
			expr_9F_cp_0[expr_9F_cp_1].count = expr_9F_cp_0[expr_9F_cp_1].count + 1;
		}

		public void Query(Vector2 p, float radius, Agent agent)
		{
			this.QueryRec(0, p, radius, agent, this.bounds);
		}

		private float QueryRec(int i, Vector2 p, float radius, Agent agent, Rect r)
		{
			if (this.nodes[i].child00 == i)
			{
				for (Agent agent2 = this.nodes[i].linkedList; agent2 != null; agent2 = agent2.next)
				{
					float num = agent.InsertAgentNeighbour(agent2, radius * radius);
					if (num < radius * radius)
					{
						radius = Mathf.Sqrt(num);
					}
				}
			}
			else
			{
				Vector2 center = r.center;
				if (p.x - radius < center.x)
				{
					if (p.y - radius < center.y)
					{
						radius = this.QueryRec(this.nodes[i].child00, p, radius, agent, Rect.MinMaxRect(r.xMin, r.yMin, center.x, center.y));
					}
					if (p.y + radius > center.y)
					{
						radius = this.QueryRec(this.nodes[i].child01, p, radius, agent, Rect.MinMaxRect(r.xMin, center.y, center.x, r.yMax));
					}
				}
				if (p.x + radius > center.x)
				{
					if (p.y - radius < center.y)
					{
						radius = this.QueryRec(this.nodes[i].child10, p, radius, agent, Rect.MinMaxRect(center.x, r.yMin, r.xMax, center.y));
					}
					if (p.y + radius > center.y)
					{
						radius = this.QueryRec(this.nodes[i].child11, p, radius, agent, Rect.MinMaxRect(center.x, center.y, r.xMax, r.yMax));
					}
				}
			}
			return radius;
		}

		public void DebugDraw()
		{
			this.DebugDrawRec(0, this.bounds);
		}

		private void DebugDrawRec(int i, Rect r)
		{
			Debug.DrawLine(new Vector3(r.xMin, 0f, r.yMin), new Vector3(r.xMax, 0f, r.yMin), Color.white);
			Debug.DrawLine(new Vector3(r.xMax, 0f, r.yMin), new Vector3(r.xMax, 0f, r.yMax), Color.white);
			Debug.DrawLine(new Vector3(r.xMax, 0f, r.yMax), new Vector3(r.xMin, 0f, r.yMax), Color.white);
			Debug.DrawLine(new Vector3(r.xMin, 0f, r.yMax), new Vector3(r.xMin, 0f, r.yMin), Color.white);
			if (this.nodes[i].child00 != i)
			{
				Vector2 center = r.center;
				this.DebugDrawRec(this.nodes[i].child11, Rect.MinMaxRect(center.x, center.y, r.xMax, r.yMax));
				this.DebugDrawRec(this.nodes[i].child10, Rect.MinMaxRect(center.x, r.yMin, r.xMax, center.y));
				this.DebugDrawRec(this.nodes[i].child01, Rect.MinMaxRect(r.xMin, center.y, center.x, r.yMax));
				this.DebugDrawRec(this.nodes[i].child00, Rect.MinMaxRect(r.xMin, r.yMin, center.x, center.y));
			}
			for (Agent agent = this.nodes[i].linkedList; agent != null; agent = agent.next)
			{
				Debug.DrawLine(this.nodes[i].linkedList.position + Vector3.up, agent.position + Vector3.up, new Color(1f, 1f, 0f, 0.5f));
			}
		}
	}
}
