using Pathfinding.Util;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Pathfinding
{
	public class MultiTargetPath : ABPath
	{
		public enum HeuristicMode
		{
			None,
			Average,
			MovingAverage,
			Midpoint,
			MovingMidpoint,
			Sequential
		}

		public OnPathDelegate[] callbacks;

		public GraphNode[] targetNodes;

		protected int targetNodeCount;

		public bool[] targetsFound;

		public Vector3[] targetPoints;

		public Vector3[] originalTargetPoints;

		public List<Vector3>[] vectorPaths;

		public List<GraphNode>[] nodePaths;

		public bool pathsForAll = true;

		public int chosenTarget = -1;

		private int sequentialTarget;

		public MultiTargetPath.HeuristicMode heuristicMode = MultiTargetPath.HeuristicMode.Sequential;

		public bool inverted = true;

		public static MultiTargetPath Construct(Vector3[] startPoints, Vector3 target, OnPathDelegate[] callbackDelegates, OnPathDelegate callback = null)
		{
			MultiTargetPath multiTargetPath = MultiTargetPath.Construct(target, startPoints, callbackDelegates, callback);
			multiTargetPath.inverted = true;
			return multiTargetPath;
		}

		public static MultiTargetPath Construct(Vector3 start, Vector3[] targets, OnPathDelegate[] callbackDelegates, OnPathDelegate callback = null)
		{
			MultiTargetPath path = PathPool<MultiTargetPath>.GetPath();
			path.Setup(start, targets, callbackDelegates, callback);
			return path;
		}

		protected void Setup(Vector3 start, Vector3[] targets, OnPathDelegate[] callbackDelegates, OnPathDelegate callback)
		{
			this.inverted = false;
			this.callback = callback;
			this.callbacks = callbackDelegates;
			this.targetPoints = targets;
			this.originalStartPoint = start;
			this.startPoint = start;
			this.startIntPoint = (Int3)start;
			if (targets.Length == 0)
			{
				base.Error();
				base.LogError("No targets were assigned to the MultiTargetPath");
				return;
			}
			this.endPoint = targets[0];
			this.originalTargetPoints = new Vector3[this.targetPoints.Length];
			for (int i = 0; i < this.targetPoints.Length; i++)
			{
				this.originalTargetPoints[i] = this.targetPoints[i];
			}
		}

		protected override void Recycle()
		{
			PathPool<MultiTargetPath>.Recycle(this);
		}

		public override void OnEnterPool()
		{
			if (this.vectorPaths != null)
			{
				for (int i = 0; i < this.vectorPaths.Length; i++)
				{
					if (this.vectorPaths[i] != null)
					{
						ListPool<Vector3>.Release(this.vectorPaths[i]);
					}
				}
			}
			this.vectorPaths = null;
			this.vectorPath = null;
			if (this.nodePaths != null)
			{
				for (int j = 0; j < this.nodePaths.Length; j++)
				{
					if (this.nodePaths[j] != null)
					{
						ListPool<GraphNode>.Release(this.nodePaths[j]);
					}
				}
			}
			this.nodePaths = null;
			this.path = null;
			base.OnEnterPool();
		}

		private void ChooseShortestPath()
		{
			this.chosenTarget = -1;
			if (this.nodePaths != null)
			{
				uint num = 2147483647u;
				for (int i = 0; i < this.nodePaths.Length; i++)
				{
					List<GraphNode> list = this.nodePaths[i];
					if (list != null)
					{
						uint g = base.pathHandler.GetPathNode(list[(!this.inverted) ? (list.Count - 1) : 0]).G;
						if (this.chosenTarget == -1 || g < num)
						{
							this.chosenTarget = i;
							num = g;
						}
					}
				}
			}
		}

		private void SetPathParametersForReturn(int target)
		{
			this.path = this.nodePaths[target];
			this.vectorPath = this.vectorPaths[target];
			if (this.inverted)
			{
				this.startNode = this.targetNodes[target];
				this.startPoint = this.targetPoints[target];
				this.originalStartPoint = this.originalTargetPoints[target];
			}
			else
			{
				this.endNode = this.targetNodes[target];
				this.endPoint = this.targetPoints[target];
				this.originalEndPoint = this.originalTargetPoints[target];
			}
		}

		public override void ReturnPath()
		{
			if (base.error)
			{
				if (this.callbacks != null)
				{
					for (int i = 0; i < this.callbacks.Length; i++)
					{
						if (this.callbacks[i] != null)
						{
							this.callbacks[i](this);
						}
					}
				}
				if (this.callback != null)
				{
					this.callback(this);
				}
				return;
			}
			bool flag = false;
			if (this.inverted)
			{
				this.endPoint = this.startPoint;
				this.endNode = this.startNode;
				this.originalEndPoint = this.originalStartPoint;
			}
			for (int j = 0; j < this.nodePaths.Length; j++)
			{
				if (this.nodePaths[j] != null)
				{
					base.CompleteState = PathCompleteState.Complete;
					flag = true;
				}
				else
				{
					base.CompleteState = PathCompleteState.Error;
				}
				if (this.callbacks != null && this.callbacks[j] != null)
				{
					this.SetPathParametersForReturn(j);
					this.callbacks[j](this);
					this.vectorPaths[j] = this.vectorPath;
				}
			}
			if (flag)
			{
				base.CompleteState = PathCompleteState.Complete;
				this.SetPathParametersForReturn(this.chosenTarget);
			}
			else
			{
				base.CompleteState = PathCompleteState.Error;
			}
			if (this.callback != null)
			{
				this.callback(this);
			}
		}

		protected void FoundTarget(PathNode nodeR, int i)
		{
			nodeR.flag1 = false;
			this.Trace(nodeR);
			this.vectorPaths[i] = this.vectorPath;
			this.nodePaths[i] = this.path;
			this.vectorPath = ListPool<Vector3>.Claim();
			this.path = ListPool<GraphNode>.Claim();
			this.targetsFound[i] = true;
			this.targetNodeCount--;
			if (!this.pathsForAll)
			{
				base.CompleteState = PathCompleteState.Complete;
				this.targetNodeCount = 0;
				return;
			}
			if (this.targetNodeCount <= 0)
			{
				base.CompleteState = PathCompleteState.Complete;
				return;
			}
			this.RecalculateHTarget(false);
		}

		protected void RebuildOpenList()
		{
			BinaryHeapM heap = base.pathHandler.GetHeap();
			for (int i = 0; i < heap.numberOfItems; i++)
			{
				PathNode node = heap.GetNode(i);
				node.H = base.CalculateHScore(node.node);
				heap.SetF(i, node.F);
			}
			base.pathHandler.RebuildHeap();
		}

		public override void Prepare()
		{
			this.nnConstraint.tags = this.enabledTags;
			NNInfo nearest = AstarPath.active.GetNearest(this.startPoint, this.nnConstraint, this.startHint);
			this.startNode = nearest.node;
			if (this.startNode == null)
			{
				base.LogError("Could not find start node for multi target path");
				base.Error();
				return;
			}
			if (!this.startNode.Walkable)
			{
				base.LogError("Nearest node to the start point is not walkable");
				base.Error();
				return;
			}
			PathNNConstraint pathNNConstraint = this.nnConstraint as PathNNConstraint;
			if (pathNNConstraint != null)
			{
				pathNNConstraint.SetStart(nearest.node);
			}
			this.vectorPaths = new List<Vector3>[this.targetPoints.Length];
			this.nodePaths = new List<GraphNode>[this.targetPoints.Length];
			this.targetNodes = new GraphNode[this.targetPoints.Length];
			this.targetsFound = new bool[this.targetPoints.Length];
			this.targetNodeCount = this.targetPoints.Length;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			for (int i = 0; i < this.targetPoints.Length; i++)
			{
				NNInfo nearest2 = AstarPath.active.GetNearest(this.targetPoints[i], this.nnConstraint);
				this.targetNodes[i] = nearest2.node;
				this.targetPoints[i] = nearest2.clampedPosition;
				if (this.targetNodes[i] != null)
				{
					flag3 = true;
					this.endNode = this.targetNodes[i];
				}
				bool flag4 = false;
				if (nearest2.node != null && nearest2.node.Walkable)
				{
					flag = true;
				}
				else
				{
					flag4 = true;
				}
				if (nearest2.node != null && nearest2.node.Area == this.startNode.Area)
				{
					flag2 = true;
				}
				else
				{
					flag4 = true;
				}
				if (flag4)
				{
					this.targetsFound[i] = true;
					this.targetNodeCount--;
				}
			}
			this.startPoint = nearest.clampedPosition;
			this.startIntPoint = (Int3)this.startPoint;
			if (this.startNode == null || !flag3)
			{
				base.LogError(string.Concat(new string[]
				{
					"Couldn't find close nodes to either the start or the end (start = ",
					(this.startNode == null) ? "not found" : "found",
					" end = ",
					(!flag3) ? "none found" : "at least one found",
					")"
				}));
				base.Error();
				return;
			}
			if (!this.startNode.Walkable)
			{
				base.LogError("The node closest to the start point is not walkable");
				base.Error();
				return;
			}
			if (!flag)
			{
				base.LogError("No target nodes were walkable");
				base.Error();
				return;
			}
			if (!flag2)
			{
				base.LogError("There are no valid paths to the targets");
				base.Error();
				return;
			}
			this.RecalculateHTarget(true);
		}

		private void RecalculateHTarget(bool firstTime)
		{
			if (!this.pathsForAll)
			{
				this.heuristic = Heuristic.None;
				this.heuristicScale = 0f;
				return;
			}
			switch (this.heuristicMode)
			{
			case MultiTargetPath.HeuristicMode.None:
				this.heuristic = Heuristic.None;
				this.heuristicScale = 0f;
				goto IL_269;
			case MultiTargetPath.HeuristicMode.Average:
				if (!firstTime)
				{
					return;
				}
				break;
			case MultiTargetPath.HeuristicMode.MovingAverage:
				break;
			case MultiTargetPath.HeuristicMode.Midpoint:
				if (!firstTime)
				{
					return;
				}
				goto IL_EF;
			case MultiTargetPath.HeuristicMode.MovingMidpoint:
				goto IL_EF;
			case MultiTargetPath.HeuristicMode.Sequential:
			{
				if (!firstTime && !this.targetsFound[this.sequentialTarget])
				{
					return;
				}
				float num = 0f;
				for (int i = 0; i < this.targetPoints.Length; i++)
				{
					if (!this.targetsFound[i])
					{
						float sqrMagnitude = (this.targetNodes[i].position - this.startNode.position).sqrMagnitude;
						if (sqrMagnitude > num)
						{
							num = sqrMagnitude;
							this.hTarget = (Int3)this.targetPoints[i];
							this.sequentialTarget = i;
						}
					}
				}
				goto IL_269;
			}
			default:
				goto IL_269;
			}
			Vector3 vector = Vector3.zero;
			int num2 = 0;
			for (int j = 0; j < this.targetPoints.Length; j++)
			{
				if (!this.targetsFound[j])
				{
					vector += (Vector3)this.targetNodes[j].position;
					num2++;
				}
			}
			if (num2 == 0)
			{
				throw new Exception("Should not happen");
			}
			vector /= (float)num2;
			this.hTarget = (Int3)vector;
			goto IL_269;
			IL_EF:
			Vector3 vector2 = Vector3.zero;
			Vector3 vector3 = Vector3.zero;
			bool flag = false;
			for (int k = 0; k < this.targetPoints.Length; k++)
			{
				if (!this.targetsFound[k])
				{
					if (!flag)
					{
						vector2 = (Vector3)this.targetNodes[k].position;
						vector3 = (Vector3)this.targetNodes[k].position;
						flag = true;
					}
					else
					{
						vector2 = Vector3.Min((Vector3)this.targetNodes[k].position, vector2);
						vector3 = Vector3.Max((Vector3)this.targetNodes[k].position, vector3);
					}
				}
			}
			Int3 hTarget = (Int3)((vector2 + vector3) * 0.5f);
			this.hTarget = hTarget;
			IL_269:
			if (!firstTime)
			{
				this.RebuildOpenList();
			}
		}

		public override void Initialize()
		{
			PathNode pathNode = base.pathHandler.GetPathNode(this.startNode);
			pathNode.node = this.startNode;
			pathNode.pathID = base.pathID;
			pathNode.parent = null;
			pathNode.cost = 0u;
			pathNode.G = base.GetTraversalCost(this.startNode);
			pathNode.H = base.CalculateHScore(this.startNode);
			for (int i = 0; i < this.targetNodes.Length; i++)
			{
				if (this.startNode == this.targetNodes[i])
				{
					this.FoundTarget(pathNode, i);
				}
				else if (this.targetNodes[i] != null)
				{
					base.pathHandler.GetPathNode(this.targetNodes[i]).flag1 = true;
				}
			}
			if (this.targetNodeCount <= 0)
			{
				base.CompleteState = PathCompleteState.Complete;
				return;
			}
			this.startNode.Open(this, pathNode, base.pathHandler);
			this.searchedNodes++;
			if (base.pathHandler.HeapEmpty())
			{
				base.LogError("No open points, the start node didn't open any nodes");
				base.Error();
				return;
			}
			this.currentR = base.pathHandler.PopNode();
		}

		public override void Cleanup()
		{
			this.ChooseShortestPath();
			this.ResetFlags();
		}

		private void ResetFlags()
		{
			if (this.targetNodes != null)
			{
				for (int i = 0; i < this.targetNodes.Length; i++)
				{
					if (this.targetNodes[i] != null)
					{
						base.pathHandler.GetPathNode(this.targetNodes[i]).flag1 = false;
					}
				}
			}
		}

		public override void CalculateStep(long targetTick)
		{
			int num = 0;
			while (base.CompleteState == PathCompleteState.NotCalculated)
			{
				this.searchedNodes++;
				if (this.currentR.flag1)
				{
					for (int i = 0; i < this.targetNodes.Length; i++)
					{
						if (!this.targetsFound[i] && this.currentR.node == this.targetNodes[i])
						{
							this.FoundTarget(this.currentR, i);
							if (base.CompleteState != PathCompleteState.NotCalculated)
							{
								break;
							}
						}
					}
					if (this.targetNodeCount <= 0)
					{
						base.CompleteState = PathCompleteState.Complete;
						break;
					}
				}
				this.currentR.node.Open(this, this.currentR, base.pathHandler);
				if (base.pathHandler.HeapEmpty())
				{
					base.CompleteState = PathCompleteState.Complete;
					break;
				}
				this.currentR = base.pathHandler.PopNode();
				if (num > 500)
				{
					if (DateTime.UtcNow.Ticks >= targetTick)
					{
						return;
					}
					num = 0;
				}
				num++;
			}
		}

		protected override void Trace(PathNode node)
		{
			base.Trace(node);
			if (this.inverted)
			{
				int num = this.path.Count / 2;
				for (int i = 0; i < num; i++)
				{
					GraphNode value = this.path[i];
					this.path[i] = this.path[this.path.Count - i - 1];
					this.path[this.path.Count - i - 1] = value;
				}
				for (int j = 0; j < num; j++)
				{
					Vector3 value2 = this.vectorPath[j];
					this.vectorPath[j] = this.vectorPath[this.vectorPath.Count - j - 1];
					this.vectorPath[this.vectorPath.Count - j - 1] = value2;
				}
			}
		}

		public override string DebugString(PathLog logMode)
		{
			if (logMode == PathLog.None || (!base.error && logMode == PathLog.OnlyErrors))
			{
				return string.Empty;
			}
			StringBuilder debugStringBuilder = base.pathHandler.DebugStringBuilder;
			debugStringBuilder.Length = 0;
			debugStringBuilder.Append((!base.error) ? "Path Completed : " : "Path Failed : ");
			debugStringBuilder.Append("Computation Time ");
			debugStringBuilder.Append(this.duration.ToString((logMode != PathLog.Heavy) ? "0.00" : "0.000"));
			debugStringBuilder.Append(" ms Searched Nodes ");
			debugStringBuilder.Append(this.searchedNodes);
			if (!base.error)
			{
				debugStringBuilder.Append("\nShortest path was ");
				debugStringBuilder.Append((this.chosenTarget != -1) ? this.nodePaths[this.chosenTarget].Count.ToString() : "undefined");
				debugStringBuilder.Append(" nodes long");
				if (logMode == PathLog.Heavy)
				{
					debugStringBuilder.Append("\nSearch Iterations " + this.searchIterations);
					debugStringBuilder.Append("\nPaths (").Append(this.targetsFound.Length).Append("):");
					for (int i = 0; i < this.targetsFound.Length; i++)
					{
						debugStringBuilder.Append("\n\n\tPath " + i).Append(" Found: ").Append(this.targetsFound[i]);
						if (this.nodePaths[i] != null)
						{
							debugStringBuilder.Append("\n\t\tLength: ");
							debugStringBuilder.Append(this.nodePaths[i].Count);
							GraphNode graphNode = this.nodePaths[i][this.nodePaths[i].Count - 1];
							if (graphNode != null)
							{
								PathNode pathNode = base.pathHandler.GetPathNode(this.endNode);
								if (pathNode != null)
								{
									debugStringBuilder.Append("\n\t\tEnd Node");
									debugStringBuilder.Append("\n\t\t\tG: ");
									debugStringBuilder.Append(pathNode.G);
									debugStringBuilder.Append("\n\t\t\tH: ");
									debugStringBuilder.Append(pathNode.H);
									debugStringBuilder.Append("\n\t\t\tF: ");
									debugStringBuilder.Append(pathNode.F);
									debugStringBuilder.Append("\n\t\t\tPoint: ");
									debugStringBuilder.Append(this.endPoint.ToString());
									debugStringBuilder.Append("\n\t\t\tGraph: ");
									debugStringBuilder.Append(this.endNode.GraphIndex);
								}
								else
								{
									debugStringBuilder.Append("\n\t\tEnd Node: Null");
								}
							}
						}
					}
					debugStringBuilder.Append("\nStart Node");
					debugStringBuilder.Append("\n\tPoint: ");
					debugStringBuilder.Append(this.endPoint.ToString());
					debugStringBuilder.Append("\n\tGraph: ");
					debugStringBuilder.Append(this.startNode.GraphIndex);
					debugStringBuilder.Append("\nBinary Heap size at completion: ");
					debugStringBuilder.AppendLine((base.pathHandler.GetHeap() != null) ? (base.pathHandler.GetHeap().numberOfItems - 2).ToString() : "Null");
				}
			}
			if (base.error)
			{
				debugStringBuilder.Append("\nError: ");
				debugStringBuilder.Append(base.errorLog);
				debugStringBuilder.AppendLine();
			}
			if (logMode == PathLog.Heavy && !AstarPath.IsUsingMultithreading)
			{
				debugStringBuilder.Append("\nCallback references ");
				if (this.callback != null)
				{
					debugStringBuilder.Append(this.callback.Target.GetType().FullName).AppendLine();
				}
				else
				{
					debugStringBuilder.AppendLine("NULL");
				}
			}
			debugStringBuilder.Append("\nPath Number ");
			debugStringBuilder.Append(base.pathID);
			return debugStringBuilder.ToString();
		}
	}
}
