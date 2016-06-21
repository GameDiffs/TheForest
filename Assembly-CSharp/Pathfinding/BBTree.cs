using System;
using UnityEngine;

namespace Pathfinding
{
	public class BBTree
	{
		private struct BBTreeBox
		{
			public IntRect rect;

			public MeshNode node;

			public int left;

			public int right;

			public bool IsLeaf
			{
				get
				{
					return this.node != null;
				}
			}

			public BBTreeBox(IntRect rect)
			{
				this.node = null;
				this.rect = rect;
				this.left = (this.right = -1);
			}

			public BBTreeBox(MeshNode node)
			{
				this.node = node;
				Int3 vertex = node.GetVertex(0);
				Int2 @int = new Int2(vertex.x, vertex.z);
				Int2 int2 = @int;
				for (int i = 1; i < node.GetVertexCount(); i++)
				{
					Int3 vertex2 = node.GetVertex(i);
					@int.x = Math.Min(@int.x, vertex2.x);
					@int.y = Math.Min(@int.y, vertex2.z);
					int2.x = Math.Max(int2.x, vertex2.x);
					int2.y = Math.Max(int2.y, vertex2.z);
				}
				this.rect = new IntRect(@int.x, @int.y, int2.x, int2.y);
				this.left = (this.right = -1);
			}

			public bool Contains(Vector3 p)
			{
				Int3 @int = (Int3)p;
				return this.rect.Contains(@int.x, @int.z);
			}
		}

		private BBTree.BBTreeBox[] arr = new BBTree.BBTreeBox[6];

		private int count;

		public Rect Size
		{
			get
			{
				if (this.count == 0)
				{
					return new Rect(0f, 0f, 0f, 0f);
				}
				IntRect rect = this.arr[0].rect;
				return Rect.MinMaxRect((float)rect.xmin * 0.001f, (float)rect.ymin * 0.001f, (float)rect.xmax * 0.001f, (float)rect.ymax * 0.001f);
			}
		}

		public void Clear()
		{
			this.count = 0;
		}

		private void EnsureCapacity(int c)
		{
			if (this.arr.Length < c)
			{
				BBTree.BBTreeBox[] array = new BBTree.BBTreeBox[Math.Max(c, (int)((float)this.arr.Length * 1.5f))];
				for (int i = 0; i < this.count; i++)
				{
					array[i] = this.arr[i];
				}
				this.arr = array;
			}
		}

		private int GetBox(MeshNode node)
		{
			if (this.count >= this.arr.Length)
			{
				this.EnsureCapacity(this.count + 1);
			}
			this.arr[this.count] = new BBTree.BBTreeBox(node);
			this.count++;
			return this.count - 1;
		}

		private int GetBox(IntRect rect)
		{
			if (this.count >= this.arr.Length)
			{
				this.EnsureCapacity(this.count + 1);
			}
			this.arr[this.count] = new BBTree.BBTreeBox(rect);
			this.count++;
			return this.count - 1;
		}

		public void RebuildFrom(MeshNode[] nodes)
		{
			this.Clear();
			if (nodes.Length == 0)
			{
				return;
			}
			if (nodes.Length == 1)
			{
				this.GetBox(nodes[0]);
				return;
			}
			this.EnsureCapacity(Mathf.CeilToInt((float)nodes.Length * 2.1f));
			MeshNode[] array = new MeshNode[nodes.Length];
			for (int i = 0; i < nodes.Length; i++)
			{
				array[i] = nodes[i];
			}
			this.RebuildFromInternal(array, 0, nodes.Length, false);
		}

		private static int SplitByX(MeshNode[] nodes, int from, int to, int divider)
		{
			int num = to;
			for (int i = from; i < num; i++)
			{
				if (nodes[i].position.x > divider)
				{
					num--;
					MeshNode meshNode = nodes[num];
					nodes[num] = nodes[i];
					nodes[i] = meshNode;
					i--;
				}
			}
			return num;
		}

		private static int SplitByZ(MeshNode[] nodes, int from, int to, int divider)
		{
			int num = to;
			for (int i = from; i < num; i++)
			{
				if (nodes[i].position.z > divider)
				{
					num--;
					MeshNode meshNode = nodes[num];
					nodes[num] = nodes[i];
					nodes[i] = meshNode;
					i--;
				}
			}
			return num;
		}

		private int RebuildFromInternal(MeshNode[] nodes, int from, int to, bool odd)
		{
			if (to - from <= 0)
			{
				throw new ArgumentException();
			}
			if (to - from == 1)
			{
				return this.GetBox(nodes[from]);
			}
			IntRect rect = BBTree.NodeBounds(nodes, from, to);
			int box = this.GetBox(rect);
			if (to - from == 2)
			{
				this.arr[box].left = this.GetBox(nodes[from]);
				this.arr[box].right = this.GetBox(nodes[from + 1]);
				return box;
			}
			int num;
			if (odd)
			{
				int divider = (rect.xmin + rect.xmax) / 2;
				num = BBTree.SplitByX(nodes, from, to, divider);
			}
			else
			{
				int divider2 = (rect.ymin + rect.ymax) / 2;
				num = BBTree.SplitByZ(nodes, from, to, divider2);
			}
			if (num == from || num == to)
			{
				if (!odd)
				{
					int divider3 = (rect.xmin + rect.xmax) / 2;
					num = BBTree.SplitByX(nodes, from, to, divider3);
				}
				else
				{
					int divider4 = (rect.ymin + rect.ymax) / 2;
					num = BBTree.SplitByZ(nodes, from, to, divider4);
				}
				if (num == from || num == to)
				{
					num = (from + to) / 2;
				}
			}
			this.arr[box].left = this.RebuildFromInternal(nodes, from, num, !odd);
			this.arr[box].right = this.RebuildFromInternal(nodes, num, to, !odd);
			return box;
		}

		private static IntRect NodeBounds(MeshNode[] nodes, int from, int to)
		{
			if (to - from <= 0)
			{
				throw new ArgumentException();
			}
			Int3 vertex = nodes[from].GetVertex(0);
			Int2 @int = new Int2(vertex.x, vertex.z);
			Int2 int2 = @int;
			for (int i = from; i < to; i++)
			{
				MeshNode meshNode = nodes[i];
				for (int j = 1; j < meshNode.GetVertexCount(); j++)
				{
					Int3 vertex2 = meshNode.GetVertex(j);
					@int.x = Math.Min(@int.x, vertex2.x);
					@int.y = Math.Min(@int.y, vertex2.z);
					int2.x = Math.Max(int2.x, vertex2.x);
					int2.y = Math.Max(int2.y, vertex2.z);
				}
			}
			return new IntRect(@int.x, @int.y, int2.x, int2.y);
		}

		public void Insert(MeshNode node)
		{
			int box = this.GetBox(node);
			if (box == 0)
			{
				return;
			}
			BBTree.BBTreeBox bBTreeBox = this.arr[box];
			int num = 0;
			BBTree.BBTreeBox bBTreeBox2;
			while (true)
			{
				bBTreeBox2 = this.arr[num];
				bBTreeBox2.rect = BBTree.ExpandToContain(bBTreeBox2.rect, bBTreeBox.rect);
				if (bBTreeBox2.node != null)
				{
					break;
				}
				this.arr[num] = bBTreeBox2;
				int num2 = BBTree.ExpansionRequired(this.arr[bBTreeBox2.left].rect, bBTreeBox.rect);
				int num3 = BBTree.ExpansionRequired(this.arr[bBTreeBox2.right].rect, bBTreeBox.rect);
				if (num2 < num3)
				{
					num = bBTreeBox2.left;
				}
				else if (num3 < num2)
				{
					num = bBTreeBox2.right;
				}
				else
				{
					num = ((BBTree.RectArea(this.arr[bBTreeBox2.left].rect) >= BBTree.RectArea(this.arr[bBTreeBox2.right].rect)) ? bBTreeBox2.right : bBTreeBox2.left);
				}
			}
			bBTreeBox2.left = box;
			int box2 = this.GetBox(bBTreeBox2.node);
			bBTreeBox2.right = box2;
			bBTreeBox2.node = null;
			this.arr[num] = bBTreeBox2;
		}

		public NNInfo Query(Vector3 p, NNConstraint constraint)
		{
			if (this.count == 0)
			{
				return new NNInfo(null);
			}
			NNInfo result = default(NNInfo);
			this.SearchBox(0, p, constraint, ref result);
			result.UpdateInfo();
			return result;
		}

		public NNInfo QueryCircle(Vector3 p, float radius, NNConstraint constraint)
		{
			if (this.count == 0)
			{
				return new NNInfo(null);
			}
			NNInfo result = new NNInfo(null);
			this.SearchBoxCircle(0, p, radius, constraint, ref result);
			result.UpdateInfo();
			return result;
		}

		public NNInfo QueryClosest(Vector3 p, NNConstraint constraint, out float distance)
		{
			distance = float.PositiveInfinity;
			return this.QueryClosest(p, constraint, ref distance, new NNInfo(null));
		}

		public NNInfo QueryClosestXZ(Vector3 p, NNConstraint constraint, ref float distance, NNInfo previous)
		{
			if (this.count == 0)
			{
				return previous;
			}
			this.SearchBoxClosestXZ(0, p, ref distance, constraint, ref previous);
			return previous;
		}

		private void SearchBoxClosestXZ(int boxi, Vector3 p, ref float closestDist, NNConstraint constraint, ref NNInfo nnInfo)
		{
			BBTree.BBTreeBox bBTreeBox = this.arr[boxi];
			if (bBTreeBox.node != null)
			{
				Vector3 constClampedPosition = bBTreeBox.node.ClosestPointOnNodeXZ(p);
				if (constraint == null || constraint.Suitable(bBTreeBox.node))
				{
					float num = (constClampedPosition.x - p.x) * (constClampedPosition.x - p.x) + (constClampedPosition.z - p.z) * (constClampedPosition.z - p.z);
					if (nnInfo.constrainedNode == null)
					{
						nnInfo.constrainedNode = bBTreeBox.node;
						nnInfo.constClampedPosition = constClampedPosition;
						closestDist = (float)Math.Sqrt((double)num);
					}
					else if (num < closestDist * closestDist)
					{
						nnInfo.constrainedNode = bBTreeBox.node;
						nnInfo.constClampedPosition = constClampedPosition;
						closestDist = (float)Math.Sqrt((double)num);
					}
				}
			}
			else
			{
				if (BBTree.RectIntersectsCircle(this.arr[bBTreeBox.left].rect, p, closestDist))
				{
					this.SearchBoxClosestXZ(bBTreeBox.left, p, ref closestDist, constraint, ref nnInfo);
				}
				if (BBTree.RectIntersectsCircle(this.arr[bBTreeBox.right].rect, p, closestDist))
				{
					this.SearchBoxClosestXZ(bBTreeBox.right, p, ref closestDist, constraint, ref nnInfo);
				}
			}
		}

		public NNInfo QueryClosest(Vector3 p, NNConstraint constraint, ref float distance, NNInfo previous)
		{
			if (this.count == 0)
			{
				return previous;
			}
			this.SearchBoxClosest(0, p, ref distance, constraint, ref previous);
			return previous;
		}

		private void SearchBoxClosest(int boxi, Vector3 p, ref float closestDist, NNConstraint constraint, ref NNInfo nnInfo)
		{
			BBTree.BBTreeBox bBTreeBox = this.arr[boxi];
			if (bBTreeBox.node != null)
			{
				if (BBTree.NodeIntersectsCircle(bBTreeBox.node, p, closestDist))
				{
					Vector3 vector = bBTreeBox.node.ClosestPointOnNode(p);
					if (constraint == null || constraint.Suitable(bBTreeBox.node))
					{
						float sqrMagnitude = (vector - p).sqrMagnitude;
						if (nnInfo.constrainedNode == null)
						{
							nnInfo.constrainedNode = bBTreeBox.node;
							nnInfo.constClampedPosition = vector;
							closestDist = (float)Math.Sqrt((double)sqrMagnitude);
						}
						else if (sqrMagnitude < closestDist * closestDist)
						{
							nnInfo.constrainedNode = bBTreeBox.node;
							nnInfo.constClampedPosition = vector;
							closestDist = (float)Math.Sqrt((double)sqrMagnitude);
						}
					}
				}
			}
			else
			{
				if (BBTree.RectIntersectsCircle(this.arr[bBTreeBox.left].rect, p, closestDist))
				{
					this.SearchBoxClosest(bBTreeBox.left, p, ref closestDist, constraint, ref nnInfo);
				}
				if (BBTree.RectIntersectsCircle(this.arr[bBTreeBox.right].rect, p, closestDist))
				{
					this.SearchBoxClosest(bBTreeBox.right, p, ref closestDist, constraint, ref nnInfo);
				}
			}
		}

		public MeshNode QueryInside(Vector3 p, NNConstraint constraint)
		{
			return (this.count == 0) ? null : this.SearchBoxInside(0, p, constraint);
		}

		private MeshNode SearchBoxInside(int boxi, Vector3 p, NNConstraint constraint)
		{
			BBTree.BBTreeBox bBTreeBox = this.arr[boxi];
			if (bBTreeBox.node != null)
			{
				if (bBTreeBox.node.ContainsPoint((Int3)p))
				{
					if (constraint == null || constraint.Suitable(bBTreeBox.node))
					{
						return bBTreeBox.node;
					}
				}
			}
			else
			{
				if (this.arr[bBTreeBox.left].Contains(p))
				{
					MeshNode meshNode = this.SearchBoxInside(bBTreeBox.left, p, constraint);
					if (meshNode != null)
					{
						return meshNode;
					}
				}
				if (this.arr[bBTreeBox.right].Contains(p))
				{
					MeshNode meshNode = this.SearchBoxInside(bBTreeBox.right, p, constraint);
					if (meshNode != null)
					{
						return meshNode;
					}
				}
			}
			return null;
		}

		private void SearchBoxCircle(int boxi, Vector3 p, float radius, NNConstraint constraint, ref NNInfo nnInfo)
		{
			BBTree.BBTreeBox bBTreeBox = this.arr[boxi];
			if (bBTreeBox.node != null)
			{
				if (BBTree.NodeIntersectsCircle(bBTreeBox.node, p, radius))
				{
					Vector3 vector = bBTreeBox.node.ClosestPointOnNode(p);
					float sqrMagnitude = (vector - p).sqrMagnitude;
					if (nnInfo.node == null)
					{
						nnInfo.node = bBTreeBox.node;
						nnInfo.clampedPosition = vector;
					}
					else if (sqrMagnitude < (nnInfo.clampedPosition - p).sqrMagnitude)
					{
						nnInfo.node = bBTreeBox.node;
						nnInfo.clampedPosition = vector;
					}
					if (constraint == null || constraint.Suitable(bBTreeBox.node))
					{
						if (nnInfo.constrainedNode == null)
						{
							nnInfo.constrainedNode = bBTreeBox.node;
							nnInfo.constClampedPosition = vector;
						}
						else if (sqrMagnitude < (nnInfo.constClampedPosition - p).sqrMagnitude)
						{
							nnInfo.constrainedNode = bBTreeBox.node;
							nnInfo.constClampedPosition = vector;
						}
					}
				}
				return;
			}
			if (BBTree.RectIntersectsCircle(this.arr[bBTreeBox.left].rect, p, radius))
			{
				this.SearchBoxCircle(bBTreeBox.left, p, radius, constraint, ref nnInfo);
			}
			if (BBTree.RectIntersectsCircle(this.arr[bBTreeBox.right].rect, p, radius))
			{
				this.SearchBoxCircle(bBTreeBox.right, p, radius, constraint, ref nnInfo);
			}
		}

		private void SearchBox(int boxi, Vector3 p, NNConstraint constraint, ref NNInfo nnInfo)
		{
			BBTree.BBTreeBox bBTreeBox = this.arr[boxi];
			if (bBTreeBox.node != null)
			{
				if (bBTreeBox.node.ContainsPoint((Int3)p))
				{
					if (nnInfo.node == null)
					{
						nnInfo.node = bBTreeBox.node;
					}
					else if (Mathf.Abs(((Vector3)bBTreeBox.node.position).y - p.y) < Mathf.Abs(((Vector3)nnInfo.node.position).y - p.y))
					{
						nnInfo.node = bBTreeBox.node;
					}
					if (constraint.Suitable(bBTreeBox.node))
					{
						if (nnInfo.constrainedNode == null)
						{
							nnInfo.constrainedNode = bBTreeBox.node;
						}
						else if (Mathf.Abs((float)bBTreeBox.node.position.y - p.y) < Mathf.Abs((float)nnInfo.constrainedNode.position.y - p.y))
						{
							nnInfo.constrainedNode = bBTreeBox.node;
						}
					}
				}
				return;
			}
			if (this.arr[bBTreeBox.left].Contains(p))
			{
				this.SearchBox(bBTreeBox.left, p, constraint, ref nnInfo);
			}
			if (this.arr[bBTreeBox.right].Contains(p))
			{
				this.SearchBox(bBTreeBox.right, p, constraint, ref nnInfo);
			}
		}

		public void OnDrawGizmos()
		{
			Gizmos.color = new Color(1f, 1f, 1f, 0.5f);
			if (this.count == 0)
			{
				return;
			}
			this.OnDrawGizmos(0, 0);
		}

		private void OnDrawGizmos(int boxi, int depth)
		{
			BBTree.BBTreeBox bBTreeBox = this.arr[boxi];
			Vector3 a = (Vector3)new Int3(bBTreeBox.rect.xmin, 0, bBTreeBox.rect.ymin);
			Vector3 vector = (Vector3)new Int3(bBTreeBox.rect.xmax, 0, bBTreeBox.rect.ymax);
			Vector3 vector2 = (a + vector) * 0.5f;
			Vector3 size = (vector - vector2) * 2f;
			size = new Vector3(size.x, 1f, size.z);
			vector2.y += (float)(depth * 2);
			Gizmos.color = AstarMath.IntToColor(depth, 1f);
			Gizmos.DrawCube(vector2, size);
			if (bBTreeBox.node == null)
			{
				this.OnDrawGizmos(bBTreeBox.left, depth + 1);
				this.OnDrawGizmos(bBTreeBox.right, depth + 1);
			}
		}

		private static bool NodeIntersectsCircle(MeshNode node, Vector3 p, float radius)
		{
			return float.IsPositiveInfinity(radius) || (p - node.ClosestPointOnNode(p)).sqrMagnitude < radius * radius;
		}

		private static bool RectIntersectsCircle(IntRect r, Vector3 p, float radius)
		{
			if (float.IsPositiveInfinity(radius))
			{
				return true;
			}
			Vector3 vector = p;
			p.x = Math.Max(p.x, (float)r.xmin * 0.001f);
			p.x = Math.Min(p.x, (float)r.xmax * 0.001f);
			p.z = Math.Max(p.z, (float)r.ymin * 0.001f);
			p.z = Math.Min(p.z, (float)r.ymax * 0.001f);
			return (p.x - vector.x) * (p.x - vector.x) + (p.z - vector.z) * (p.z - vector.z) < radius * radius;
		}

		private static int ExpansionRequired(IntRect r, IntRect r2)
		{
			int num = Math.Min(r.xmin, r2.xmin);
			int num2 = Math.Max(r.xmax, r2.xmax);
			int num3 = Math.Min(r.ymin, r2.ymin);
			int num4 = Math.Max(r.ymax, r2.ymax);
			return (num2 - num) * (num4 - num3) - BBTree.RectArea(r);
		}

		private static IntRect ExpandToContain(IntRect r, IntRect r2)
		{
			return IntRect.Union(r, r2);
		}

		private static int RectArea(IntRect r)
		{
			return r.Width * r.Height;
		}
	}
}
