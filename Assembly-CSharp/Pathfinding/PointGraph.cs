using Pathfinding.Serialization;
using Pathfinding.Serialization.JsonFx;
using Pathfinding.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
	[JsonOptIn]
	public class PointGraph : NavGraph, IUpdatableGraph
	{
		[JsonMember]
		public Transform root;

		[JsonMember]
		public string searchTag;

		[JsonMember]
		public float maxDistance;

		[JsonMember]
		public Vector3 limits;

		[JsonMember]
		public bool raycast = true;

		[JsonMember]
		public bool use2DPhysics;

		[JsonMember]
		public bool thickRaycast;

		[JsonMember]
		public float thickRaycastRadius = 1f;

		[JsonMember]
		public bool recursive = true;

		[JsonMember]
		public bool autoLinkNodes = true;

		[JsonMember]
		public LayerMask mask;

		[JsonMember]
		public bool optimizeForSparseGraph;

		[JsonMember]
		public bool optimizeFor2D;

		private static readonly Int3[] ThreeDNeighbours = new Int3[]
		{
			new Int3(-1, 0, -1),
			new Int3(0, 0, -1),
			new Int3(1, 0, -1),
			new Int3(-1, 0, 0),
			new Int3(0, 0, 0),
			new Int3(1, 0, 0),
			new Int3(-1, 0, 1),
			new Int3(0, 0, 1),
			new Int3(1, 0, 1),
			new Int3(-1, -1, -1),
			new Int3(0, -1, -1),
			new Int3(1, -1, -1),
			new Int3(-1, -1, 0),
			new Int3(0, -1, 0),
			new Int3(1, -1, 0),
			new Int3(-1, -1, 1),
			new Int3(0, -1, 1),
			new Int3(1, -1, 1),
			new Int3(-1, 1, -1),
			new Int3(0, 1, -1),
			new Int3(1, 1, -1),
			new Int3(-1, 1, 0),
			new Int3(0, 1, 0),
			new Int3(1, 1, 0),
			new Int3(-1, 1, 1),
			new Int3(0, 1, 1),
			new Int3(1, 1, 1)
		};

		private Dictionary<Int3, PointNode> nodeLookup;

		private Int3 minLookup;

		private Int3 maxLookup;

		private Int3 lookupCellSize;

		public PointNode[] nodes;

		public int nodeCount;

		private Int3 WorldToLookupSpace(Int3 p)
		{
			Int3 zero = Int3.zero;
			zero.x = ((this.lookupCellSize.x == 0) ? 0 : (p.x / this.lookupCellSize.x));
			zero.y = ((this.lookupCellSize.y == 0) ? 0 : (p.y / this.lookupCellSize.y));
			zero.z = ((this.lookupCellSize.z == 0) ? 0 : (p.z / this.lookupCellSize.z));
			return zero;
		}

		public override void GetNodes(GraphNodeDelegateCancelable del)
		{
			if (this.nodes == null)
			{
				return;
			}
			int num = 0;
			while (num < this.nodeCount && del(this.nodes[num]))
			{
				num++;
			}
		}

		public override NNInfo GetNearest(Vector3 position, NNConstraint constraint, GraphNode hint)
		{
			return this.GetNearestForce(position, constraint);
		}

		public override NNInfo GetNearestForce(Vector3 position, NNConstraint constraint)
		{
			if (this.nodes == null)
			{
				return default(NNInfo);
			}
			float num = (!constraint.constrainDistance) ? float.PositiveInfinity : AstarPath.active.maxNearestNodeDistanceSqr;
			float num2 = float.PositiveInfinity;
			GraphNode graphNode = null;
			float num3 = float.PositiveInfinity;
			GraphNode graphNode2 = null;
			if (this.optimizeForSparseGraph)
			{
				Int3 @int = this.WorldToLookupSpace((Int3)position);
				Int3 int2 = @int - this.minLookup;
				int num4 = 0;
				num4 = Math.Max(num4, Math.Abs(int2.x));
				num4 = Math.Max(num4, Math.Abs(int2.y));
				num4 = Math.Max(num4, Math.Abs(int2.z));
				int2 = @int - this.maxLookup;
				num4 = Math.Max(num4, Math.Abs(int2.x));
				num4 = Math.Max(num4, Math.Abs(int2.y));
				num4 = Math.Max(num4, Math.Abs(int2.z));
				PointNode next;
				if (this.nodeLookup.TryGetValue(@int, out next))
				{
					while (next != null)
					{
						float sqrMagnitude = (position - (Vector3)next.position).sqrMagnitude;
						if (sqrMagnitude < num2)
						{
							num2 = sqrMagnitude;
							graphNode = next;
						}
						if (constraint == null || (sqrMagnitude < num3 && sqrMagnitude < num && constraint.Suitable(next)))
						{
							num3 = sqrMagnitude;
							graphNode2 = next;
						}
						next = next.next;
					}
				}
				for (int i = 1; i <= num4; i++)
				{
					if (i >= 20)
					{
						Debug.LogWarning("Aborting GetNearest call at maximum distance because it has iterated too many times.\nIf you get this regularly, check your settings for PointGraph -> <b>Optimize For Sparse Graph</b> and PointGraph -> <b>Optimize For 2D</b>.\nThis happens when the closest node was very far away (20*link distance between nodes). When optimizing for sparse graphs, getting the nearest node from far away positions is <b>very slow</b>.\n");
						break;
					}
					if (this.lookupCellSize.y == 0)
					{
						Int3 lhs = @int + new Int3(-i, 0, -i);
						for (int j = 0; j <= 2 * i; j++)
						{
							if (this.nodeLookup.TryGetValue(lhs + new Int3(j, 0, 0), out next))
							{
								while (next != null)
								{
									float sqrMagnitude2 = (position - (Vector3)next.position).sqrMagnitude;
									if (sqrMagnitude2 < num2)
									{
										num2 = sqrMagnitude2;
										graphNode = next;
									}
									if (constraint == null || (sqrMagnitude2 < num3 && sqrMagnitude2 < num && constraint.Suitable(next)))
									{
										num3 = sqrMagnitude2;
										graphNode2 = next;
									}
									next = next.next;
								}
							}
							if (this.nodeLookup.TryGetValue(lhs + new Int3(j, 0, 2 * i), out next))
							{
								while (next != null)
								{
									float sqrMagnitude3 = (position - (Vector3)next.position).sqrMagnitude;
									if (sqrMagnitude3 < num2)
									{
										num2 = sqrMagnitude3;
										graphNode = next;
									}
									if (constraint == null || (sqrMagnitude3 < num3 && sqrMagnitude3 < num && constraint.Suitable(next)))
									{
										num3 = sqrMagnitude3;
										graphNode2 = next;
									}
									next = next.next;
								}
							}
						}
						for (int k = 1; k < 2 * i; k++)
						{
							if (this.nodeLookup.TryGetValue(lhs + new Int3(0, 0, k), out next))
							{
								while (next != null)
								{
									float sqrMagnitude4 = (position - (Vector3)next.position).sqrMagnitude;
									if (sqrMagnitude4 < num2)
									{
										num2 = sqrMagnitude4;
										graphNode = next;
									}
									if (constraint == null || (sqrMagnitude4 < num3 && sqrMagnitude4 < num && constraint.Suitable(next)))
									{
										num3 = sqrMagnitude4;
										graphNode2 = next;
									}
									next = next.next;
								}
							}
							if (this.nodeLookup.TryGetValue(lhs + new Int3(2 * i, 0, k), out next))
							{
								while (next != null)
								{
									float sqrMagnitude5 = (position - (Vector3)next.position).sqrMagnitude;
									if (sqrMagnitude5 < num2)
									{
										num2 = sqrMagnitude5;
										graphNode = next;
									}
									if (constraint == null || (sqrMagnitude5 < num3 && sqrMagnitude5 < num && constraint.Suitable(next)))
									{
										num3 = sqrMagnitude5;
										graphNode2 = next;
									}
									next = next.next;
								}
							}
						}
					}
					else
					{
						Int3 lhs2 = @int + new Int3(-i, -i, -i);
						for (int l = 0; l <= 2 * i; l++)
						{
							for (int m = 0; m <= 2 * i; m++)
							{
								if (this.nodeLookup.TryGetValue(lhs2 + new Int3(l, m, 0), out next))
								{
									while (next != null)
									{
										float sqrMagnitude6 = (position - (Vector3)next.position).sqrMagnitude;
										if (sqrMagnitude6 < num2)
										{
											num2 = sqrMagnitude6;
											graphNode = next;
										}
										if (constraint == null || (sqrMagnitude6 < num3 && sqrMagnitude6 < num && constraint.Suitable(next)))
										{
											num3 = sqrMagnitude6;
											graphNode2 = next;
										}
										next = next.next;
									}
								}
								if (this.nodeLookup.TryGetValue(lhs2 + new Int3(l, m, 2 * i), out next))
								{
									while (next != null)
									{
										float sqrMagnitude7 = (position - (Vector3)next.position).sqrMagnitude;
										if (sqrMagnitude7 < num2)
										{
											num2 = sqrMagnitude7;
											graphNode = next;
										}
										if (constraint == null || (sqrMagnitude7 < num3 && sqrMagnitude7 < num && constraint.Suitable(next)))
										{
											num3 = sqrMagnitude7;
											graphNode2 = next;
										}
										next = next.next;
									}
								}
							}
						}
						for (int n = 1; n < 2 * i; n++)
						{
							for (int num5 = 0; num5 <= 2 * i; num5++)
							{
								if (this.nodeLookup.TryGetValue(lhs2 + new Int3(0, num5, n), out next))
								{
									while (next != null)
									{
										float sqrMagnitude8 = (position - (Vector3)next.position).sqrMagnitude;
										if (sqrMagnitude8 < num2)
										{
											num2 = sqrMagnitude8;
											graphNode = next;
										}
										if (constraint == null || (sqrMagnitude8 < num3 && sqrMagnitude8 < num && constraint.Suitable(next)))
										{
											num3 = sqrMagnitude8;
											graphNode2 = next;
										}
										next = next.next;
									}
								}
								if (this.nodeLookup.TryGetValue(lhs2 + new Int3(2 * i, num5, n), out next))
								{
									while (next != null)
									{
										float sqrMagnitude9 = (position - (Vector3)next.position).sqrMagnitude;
										if (sqrMagnitude9 < num2)
										{
											num2 = sqrMagnitude9;
											graphNode = next;
										}
										if (constraint == null || (sqrMagnitude9 < num3 && sqrMagnitude9 < num && constraint.Suitable(next)))
										{
											num3 = sqrMagnitude9;
											graphNode2 = next;
										}
										next = next.next;
									}
								}
							}
						}
						for (int num6 = 1; num6 < 2 * i; num6++)
						{
							for (int num7 = 1; num7 < 2 * i; num7++)
							{
								if (this.nodeLookup.TryGetValue(lhs2 + new Int3(num6, 0, num7), out next))
								{
									while (next != null)
									{
										float sqrMagnitude10 = (position - (Vector3)next.position).sqrMagnitude;
										if (sqrMagnitude10 < num2)
										{
											num2 = sqrMagnitude10;
											graphNode = next;
										}
										if (constraint == null || (sqrMagnitude10 < num3 && sqrMagnitude10 < num && constraint.Suitable(next)))
										{
											num3 = sqrMagnitude10;
											graphNode2 = next;
										}
										next = next.next;
									}
								}
								if (this.nodeLookup.TryGetValue(lhs2 + new Int3(num6, 2 * i, num7), out next))
								{
									while (next != null)
									{
										float sqrMagnitude11 = (position - (Vector3)next.position).sqrMagnitude;
										if (sqrMagnitude11 < num2)
										{
											num2 = sqrMagnitude11;
											graphNode = next;
										}
										if (constraint == null || (sqrMagnitude11 < num3 && sqrMagnitude11 < num && constraint.Suitable(next)))
										{
											num3 = sqrMagnitude11;
											graphNode2 = next;
										}
										next = next.next;
									}
								}
							}
						}
					}
					if (graphNode2 != null)
					{
						num4 = Math.Min(num4, i + 1);
					}
				}
			}
			else
			{
				for (int num8 = 0; num8 < this.nodeCount; num8++)
				{
					PointNode pointNode = this.nodes[num8];
					float sqrMagnitude12 = (position - (Vector3)pointNode.position).sqrMagnitude;
					if (sqrMagnitude12 < num2)
					{
						num2 = sqrMagnitude12;
						graphNode = pointNode;
					}
					if (constraint == null || (sqrMagnitude12 < num3 && sqrMagnitude12 < num && constraint.Suitable(pointNode)))
					{
						num3 = sqrMagnitude12;
						graphNode2 = pointNode;
					}
				}
			}
			NNInfo result = new NNInfo(graphNode);
			result.constrainedNode = graphNode2;
			if (graphNode2 != null)
			{
				result.constClampedPosition = (Vector3)graphNode2.position;
			}
			else if (graphNode != null)
			{
				result.constrainedNode = graphNode;
				result.constClampedPosition = (Vector3)graphNode.position;
			}
			return result;
		}

		public PointNode AddNode(Int3 position)
		{
			return this.AddNode<PointNode>(new PointNode(this.active), position);
		}

		public T AddNode<T>(T node, Int3 position) where T : PointNode
		{
			if (this.nodes == null || this.nodeCount == this.nodes.Length)
			{
				PointNode[] array = new PointNode[(this.nodes == null) ? 4 : Math.Max(this.nodes.Length + 4, this.nodes.Length * 2)];
				for (int i = 0; i < this.nodeCount; i++)
				{
					array[i] = this.nodes[i];
				}
				this.nodes = array;
			}
			node.SetPosition(position);
			node.GraphIndex = this.graphIndex;
			node.Walkable = true;
			this.nodes[this.nodeCount] = node;
			this.nodeCount++;
			this.AddToLookup(node);
			return node;
		}

		public static int CountChildren(Transform tr)
		{
			int num = 0;
			foreach (Transform tr2 in tr)
			{
				num++;
				num += PointGraph.CountChildren(tr2);
			}
			return num;
		}

		public void AddChildren(ref int c, Transform tr)
		{
			foreach (Transform transform in tr)
			{
				this.nodes[c].SetPosition((Int3)transform.position);
				this.nodes[c].Walkable = true;
				this.nodes[c].gameObject = transform.gameObject;
				c++;
				this.AddChildren(ref c, transform);
			}
		}

		public void RebuildNodeLookup()
		{
			if (!this.optimizeForSparseGraph)
			{
				return;
			}
			if (this.maxDistance == 0f)
			{
				this.lookupCellSize = (Int3)this.limits;
			}
			else
			{
				this.lookupCellSize.x = Mathf.CeilToInt(1000f * ((this.limits.x == 0f) ? this.maxDistance : Mathf.Min(this.maxDistance, this.limits.x)));
				this.lookupCellSize.y = Mathf.CeilToInt(1000f * ((this.limits.y == 0f) ? this.maxDistance : Mathf.Min(this.maxDistance, this.limits.y)));
				this.lookupCellSize.z = Mathf.CeilToInt(1000f * ((this.limits.z == 0f) ? this.maxDistance : Mathf.Min(this.maxDistance, this.limits.z)));
			}
			if (this.optimizeFor2D)
			{
				this.lookupCellSize.y = 0;
			}
			if (this.nodeLookup == null)
			{
				this.nodeLookup = new Dictionary<Int3, PointNode>();
			}
			this.nodeLookup.Clear();
			for (int i = 0; i < this.nodeCount; i++)
			{
				PointNode node = this.nodes[i];
				this.AddToLookup(node);
			}
		}

		public void AddToLookup(PointNode node)
		{
			if (this.nodeLookup == null)
			{
				return;
			}
			Int3 key = this.WorldToLookupSpace(node.position);
			if (this.nodeLookup.Count == 0)
			{
				this.minLookup = key;
				this.maxLookup = key;
			}
			else
			{
				this.minLookup = new Int3(Math.Min(this.minLookup.x, key.x), Math.Min(this.minLookup.y, key.y), Math.Min(this.minLookup.z, key.z));
				this.maxLookup = new Int3(Math.Max(this.minLookup.x, key.x), Math.Max(this.minLookup.y, key.y), Math.Max(this.minLookup.z, key.z));
			}
			if (node.next != null)
			{
				throw new Exception("This node has already been added to the lookup structure");
			}
			PointNode pointNode;
			if (this.nodeLookup.TryGetValue(key, out pointNode))
			{
				node.next = pointNode.next;
				pointNode.next = node;
			}
			else
			{
				this.nodeLookup[key] = node;
			}
		}

		public override void ScanInternal(OnScanStatus statusCallback)
		{
			if (this.root == null)
			{
				GameObject[] array = (this.searchTag == null) ? null : GameObject.FindGameObjectsWithTag(this.searchTag);
				if (array == null)
				{
					this.nodes = new PointNode[0];
					this.nodeCount = 0;
					return;
				}
				this.nodes = new PointNode[array.Length];
				this.nodeCount = this.nodes.Length;
				for (int i = 0; i < this.nodes.Length; i++)
				{
					this.nodes[i] = new PointNode(this.active);
				}
				for (int j = 0; j < array.Length; j++)
				{
					this.nodes[j].SetPosition((Int3)array[j].transform.position);
					this.nodes[j].Walkable = true;
					this.nodes[j].gameObject = array[j].gameObject;
				}
			}
			else if (!this.recursive)
			{
				this.nodes = new PointNode[this.root.childCount];
				this.nodeCount = this.nodes.Length;
				for (int k = 0; k < this.nodes.Length; k++)
				{
					this.nodes[k] = new PointNode(this.active);
				}
				int num = 0;
				foreach (Transform transform in this.root)
				{
					this.nodes[num].SetPosition((Int3)transform.position);
					this.nodes[num].Walkable = true;
					this.nodes[num].gameObject = transform.gameObject;
					num++;
				}
			}
			else
			{
				this.nodes = new PointNode[PointGraph.CountChildren(this.root)];
				this.nodeCount = this.nodes.Length;
				for (int l = 0; l < this.nodes.Length; l++)
				{
					this.nodes[l] = new PointNode(this.active);
				}
				int num2 = 0;
				this.AddChildren(ref num2, this.root);
			}
			if (this.optimizeForSparseGraph)
			{
				this.RebuildNodeLookup();
			}
			if (this.maxDistance >= 0f)
			{
				List<PointNode> list = new List<PointNode>(3);
				List<uint> list2 = new List<uint>(3);
				for (int m = 0; m < this.nodes.Length; m++)
				{
					list.Clear();
					list2.Clear();
					PointNode pointNode = this.nodes[m];
					if (this.optimizeForSparseGraph)
					{
						Int3 lhs = this.WorldToLookupSpace(pointNode.position);
						int num3 = (this.lookupCellSize.y != 0) ? PointGraph.ThreeDNeighbours.Length : 9;
						for (int n = 0; n < num3; n++)
						{
							Int3 key = lhs + PointGraph.ThreeDNeighbours[n];
							PointNode next;
							if (this.nodeLookup.TryGetValue(key, out next))
							{
								while (next != null)
								{
									float num4;
									if (this.IsValidConnection(pointNode, next, out num4))
									{
										list.Add(next);
										list2.Add((uint)Mathf.RoundToInt(num4 * 1000f));
									}
									next = next.next;
								}
							}
						}
					}
					else
					{
						for (int num5 = 0; num5 < this.nodes.Length; num5++)
						{
							if (m != num5)
							{
								PointNode pointNode2 = this.nodes[num5];
								float num6;
								if (this.IsValidConnection(pointNode, pointNode2, out num6))
								{
									list.Add(pointNode2);
									list2.Add((uint)Mathf.RoundToInt(num6 * 1000f));
								}
							}
						}
					}
					pointNode.connections = list.ToArray();
					pointNode.connectionCosts = list2.ToArray();
				}
			}
		}

		public virtual bool IsValidConnection(GraphNode a, GraphNode b, out float dist)
		{
			dist = 0f;
			if (!a.Walkable || !b.Walkable)
			{
				return false;
			}
			Vector3 vector = (Vector3)(a.position - b.position);
			if ((!Mathf.Approximately(this.limits.x, 0f) && Mathf.Abs(vector.x) > this.limits.x) || (!Mathf.Approximately(this.limits.y, 0f) && Mathf.Abs(vector.y) > this.limits.y) || (!Mathf.Approximately(this.limits.z, 0f) && Mathf.Abs(vector.z) > this.limits.z))
			{
				return false;
			}
			dist = vector.magnitude;
			if (this.maxDistance == 0f || dist < this.maxDistance)
			{
				if (!this.raycast)
				{
					return true;
				}
				Ray ray = new Ray((Vector3)a.position, (Vector3)(b.position - a.position));
				Ray ray2 = new Ray((Vector3)b.position, (Vector3)(a.position - b.position));
				if (this.use2DPhysics)
				{
					if (this.thickRaycast)
					{
						if (!Physics2D.CircleCast(ray.origin, this.thickRaycastRadius, ray.direction, dist, this.mask) && !Physics2D.CircleCast(ray2.origin, this.thickRaycastRadius, ray2.direction, dist, this.mask))
						{
							return true;
						}
					}
					else if (!Physics2D.Linecast((Vector3)a.position, (Vector3)b.position, this.mask) && !Physics2D.Linecast((Vector3)b.position, (Vector3)a.position, this.mask))
					{
						return true;
					}
				}
				else if (this.thickRaycast)
				{
					if (!Physics.SphereCast(ray, this.thickRaycastRadius, dist, this.mask) && !Physics.SphereCast(ray2, this.thickRaycastRadius, dist, this.mask))
					{
						return true;
					}
				}
				else if (!Physics.Linecast((Vector3)a.position, (Vector3)b.position, this.mask) && !Physics.Linecast((Vector3)b.position, (Vector3)a.position, this.mask))
				{
					return true;
				}
			}
			return false;
		}

		public GraphUpdateThreading CanUpdateAsync(GraphUpdateObject o)
		{
			return GraphUpdateThreading.UnityThread;
		}

		public void UpdateAreaInit(GraphUpdateObject o)
		{
		}

		public void UpdateArea(GraphUpdateObject guo)
		{
			if (this.nodes == null)
			{
				return;
			}
			for (int i = 0; i < this.nodeCount; i++)
			{
				if (guo.bounds.Contains((Vector3)this.nodes[i].position))
				{
					guo.WillUpdateNode(this.nodes[i]);
					guo.Apply(this.nodes[i]);
				}
			}
			if (guo.updatePhysics)
			{
				Bounds bounds = guo.bounds;
				if (this.thickRaycast)
				{
					bounds.Expand(this.thickRaycastRadius * 2f);
				}
				List<GraphNode> list = ListPool<GraphNode>.Claim();
				List<uint> list2 = ListPool<uint>.Claim();
				for (int j = 0; j < this.nodeCount; j++)
				{
					PointNode pointNode = this.nodes[j];
					Vector3 a = (Vector3)pointNode.position;
					List<GraphNode> list3 = null;
					List<uint> list4 = null;
					for (int k = 0; k < this.nodeCount; k++)
					{
						if (k != j)
						{
							Vector3 b = (Vector3)this.nodes[k].position;
							if (Polygon.LineIntersectsBounds(bounds, a, b))
							{
								PointNode pointNode2 = this.nodes[k];
								bool flag = pointNode.ContainsConnection(pointNode2);
								float num;
								bool flag2 = this.IsValidConnection(pointNode, pointNode2, out num);
								if (!flag && flag2)
								{
									if (list3 == null)
									{
										list.Clear();
										list2.Clear();
										list3 = list;
										list4 = list2;
										list3.AddRange(pointNode.connections);
										list4.AddRange(pointNode.connectionCosts);
									}
									uint item = (uint)Mathf.RoundToInt(num * 1000f);
									list3.Add(pointNode2);
									list4.Add(item);
								}
								else if (flag && !flag2)
								{
									if (list3 == null)
									{
										list.Clear();
										list2.Clear();
										list3 = list;
										list4 = list2;
										list3.AddRange(pointNode.connections);
										list4.AddRange(pointNode.connectionCosts);
									}
									int num2 = list3.IndexOf(pointNode2);
									if (num2 != -1)
									{
										list3.RemoveAt(num2);
										list4.RemoveAt(num2);
									}
								}
							}
						}
					}
					if (list3 != null)
					{
						pointNode.connections = list3.ToArray();
						pointNode.connectionCosts = list4.ToArray();
					}
				}
				ListPool<GraphNode>.Release(list);
				ListPool<uint>.Release(list2);
			}
		}

		public override void PostDeserialization()
		{
			this.RebuildNodeLookup();
		}

		public override void RelocateNodes(Matrix4x4 oldMatrix, Matrix4x4 newMatrix)
		{
			base.RelocateNodes(oldMatrix, newMatrix);
			this.RebuildNodeLookup();
		}

		public override void SerializeExtraInfo(GraphSerializationContext ctx)
		{
			if (this.nodes == null)
			{
				ctx.writer.Write(-1);
			}
			ctx.writer.Write(this.nodeCount);
			for (int i = 0; i < this.nodeCount; i++)
			{
				if (this.nodes[i] == null)
				{
					ctx.writer.Write(-1);
				}
				else
				{
					ctx.writer.Write(0);
					this.nodes[i].SerializeNode(ctx);
				}
			}
		}

		public override void DeserializeExtraInfo(GraphSerializationContext ctx)
		{
			int num = ctx.reader.ReadInt32();
			if (num == -1)
			{
				this.nodes = null;
				return;
			}
			this.nodes = new PointNode[num];
			this.nodeCount = num;
			for (int i = 0; i < this.nodes.Length; i++)
			{
				if (ctx.reader.ReadInt32() != -1)
				{
					this.nodes[i] = new PointNode(this.active);
					this.nodes[i].DeserializeNode(ctx);
				}
			}
		}
	}
}
