using System;
using UnityEngine;

namespace Pathfinding
{
	public class RandomPath : ABPath
	{
		public int searchLength;

		public int spread;

		public bool uniform;

		public float aimStrength;

		private PathNode chosenNodeR;

		private PathNode maxGScoreNodeR;

		private int maxGScore;

		public Vector3 aim;

		private int nodesEvaluatedRep;

		private readonly System.Random rnd = new System.Random();

		public override bool FloodingPath
		{
			get
			{
				return true;
			}
		}

		protected override bool hasEndPoint
		{
			get
			{
				return false;
			}
		}

		public RandomPath()
		{
		}

		public RandomPath(Vector3 start, int length, OnPathDelegate callback = null)
		{
			throw new Exception("This constructor is obsolete. Please use the pooling API and the Setup methods");
		}

		public override void Reset()
		{
			base.Reset();
			this.searchLength = 5000;
			this.spread = 5000;
			this.uniform = true;
			this.aimStrength = 0f;
			this.chosenNodeR = null;
			this.maxGScoreNodeR = null;
			this.maxGScore = 0;
			this.aim = Vector3.zero;
			this.nodesEvaluatedRep = 0;
		}

		protected override void Recycle()
		{
			PathPool<RandomPath>.Recycle(this);
		}

		public static RandomPath Construct(Vector3 start, int length, OnPathDelegate callback = null)
		{
			RandomPath path = PathPool<RandomPath>.GetPath();
			path.Setup(start, length, callback);
			return path;
		}

		protected RandomPath Setup(Vector3 start, int length, OnPathDelegate callback)
		{
			this.callback = callback;
			this.searchLength = length;
			this.originalStartPoint = start;
			this.originalEndPoint = Vector3.zero;
			this.startPoint = start;
			this.endPoint = Vector3.zero;
			this.startIntPoint = (Int3)start;
			return this;
		}

		public override void ReturnPath()
		{
			if (this.path != null && this.path.Count > 0)
			{
				this.endNode = this.path[this.path.Count - 1];
				this.endPoint = (Vector3)this.endNode.position;
				this.originalEndPoint = this.endPoint;
				this.hTarget = this.endNode.position;
			}
			if (this.callback != null)
			{
				this.callback(this);
			}
		}

		public override void Prepare()
		{
			this.nnConstraint.tags = this.enabledTags;
			NNInfo nearest = AstarPath.active.GetNearest(this.startPoint, this.nnConstraint, this.startHint);
			this.startPoint = nearest.clampedPosition;
			this.endPoint = this.startPoint;
			this.startIntPoint = (Int3)this.startPoint;
			this.hTarget = (Int3)this.aim;
			this.startNode = nearest.node;
			this.endNode = this.startNode;
			if (this.startNode == null || this.endNode == null)
			{
				base.LogError("Couldn't find close nodes to the start point");
				base.Error();
				return;
			}
			if (!this.startNode.Walkable)
			{
				base.LogError("The node closest to the start point is not walkable");
				base.Error();
				return;
			}
			this.heuristicScale = this.aimStrength;
		}

		public override void Initialize()
		{
			PathNode pathNode = base.pathHandler.GetPathNode(this.startNode);
			pathNode.node = this.startNode;
			if (this.searchLength + this.spread <= 0)
			{
				base.CompleteState = PathCompleteState.Complete;
				this.Trace(pathNode);
				return;
			}
			pathNode.pathID = base.pathID;
			pathNode.parent = null;
			pathNode.cost = 0u;
			pathNode.G = base.GetTraversalCost(this.startNode);
			pathNode.H = base.CalculateHScore(this.startNode);
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

		public override void CalculateStep(long targetTick)
		{
			int num = 0;
			while (base.CompleteState == PathCompleteState.NotCalculated)
			{
				this.searchedNodes++;
				if ((ulong)this.currentR.G >= (ulong)((long)this.searchLength))
				{
					this.nodesEvaluatedRep++;
					if (this.chosenNodeR == null)
					{
						this.chosenNodeR = this.currentR;
					}
					else if (this.rnd.NextDouble() <= (double)(1f / (float)this.nodesEvaluatedRep))
					{
						this.chosenNodeR = this.currentR;
					}
					if ((ulong)this.currentR.G >= (ulong)((long)(this.searchLength + this.spread)))
					{
						base.CompleteState = PathCompleteState.Complete;
						break;
					}
				}
				else if ((ulong)this.currentR.G > (ulong)((long)this.maxGScore))
				{
					this.maxGScore = (int)this.currentR.G;
					this.maxGScoreNodeR = this.currentR;
				}
				this.currentR.node.Open(this, this.currentR, base.pathHandler);
				if (base.pathHandler.HeapEmpty())
				{
					if (this.chosenNodeR != null)
					{
						base.CompleteState = PathCompleteState.Complete;
					}
					else if (this.maxGScoreNodeR != null)
					{
						this.chosenNodeR = this.maxGScoreNodeR;
						base.CompleteState = PathCompleteState.Complete;
					}
					else
					{
						base.LogError("Not a single node found to search");
						base.Error();
					}
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
					if (this.searchedNodes > 1000000)
					{
						throw new Exception("Probable infinite loop. Over 1,000,000 nodes searched");
					}
				}
				num++;
			}
			if (base.CompleteState == PathCompleteState.Complete)
			{
				this.Trace(this.chosenNodeR);
			}
		}
	}
}
