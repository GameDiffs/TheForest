using Pathfinding.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;

namespace Pathfinding
{
	public abstract class Path
	{
		public OnPathDelegate callback;

		public OnPathDelegate immediateCallback;

		private PathState state;

		private object stateLock = new object();

		private PathCompleteState pathCompleteState;

		private string _errorLog = string.Empty;

		public List<GraphNode> path;

		public List<Vector3> vectorPath;

		protected float maxFrameTime;

		protected PathNode currentR;

		public float duration;

		public int searchIterations;

		public int searchedNodes;

		internal bool recycled;

		protected bool hasBeenReset;

		public NNConstraint nnConstraint = PathNNConstraint.Default;

		internal Path next;

		public Heuristic heuristic;

		public float heuristicScale = 1f;

		protected GraphNode hTargetNode;

		protected Int3 hTarget;

		public int enabledTags = -1;

		private static readonly int[] ZeroTagPenalties = new int[32];

		protected int[] internalTagPenalties;

		protected int[] manualTagPenalties;

		private List<object> claimed = new List<object>();

		private bool releasedNotSilent;

		public PathHandler pathHandler
		{
			get;
			private set;
		}

		public PathCompleteState CompleteState
		{
			get
			{
				return this.pathCompleteState;
			}
			protected set
			{
				this.pathCompleteState = value;
			}
		}

		public bool error
		{
			get
			{
				return this.CompleteState == PathCompleteState.Error;
			}
		}

		public string errorLog
		{
			get
			{
				return this._errorLog;
			}
		}

		public DateTime callTime
		{
			get;
			private set;
		}

		public ushort pathID
		{
			get;
			private set;
		}

		public int[] tagPenalties
		{
			get
			{
				return this.manualTagPenalties;
			}
			set
			{
				if (value == null || value.Length != 32)
				{
					this.manualTagPenalties = null;
					this.internalTagPenalties = Path.ZeroTagPenalties;
				}
				else
				{
					this.manualTagPenalties = value;
					this.internalTagPenalties = value;
				}
			}
		}

		public virtual bool FloodingPath
		{
			get
			{
				return false;
			}
		}

		public float GetTotalLength()
		{
			if (this.vectorPath == null)
			{
				return float.PositiveInfinity;
			}
			float num = 0f;
			for (int i = 0; i < this.vectorPath.Count - 1; i++)
			{
				num += Vector3.Distance(this.vectorPath[i], this.vectorPath[i + 1]);
			}
			return num;
		}

		[DebuggerHidden]
		public IEnumerator WaitForPath()
		{
			Path.<WaitForPath>c__Iterator9 <WaitForPath>c__Iterator = new Path.<WaitForPath>c__Iterator9();
			<WaitForPath>c__Iterator.<>f__this = this;
			return <WaitForPath>c__Iterator;
		}

		public uint CalculateHScore(GraphNode node)
		{
			switch (this.heuristic)
			{
			case Heuristic.Manhattan:
			{
				Int3 position = node.position;
				uint val = (uint)((float)(Math.Abs(this.hTarget.x - position.x) + Math.Abs(this.hTarget.y - position.y) + Math.Abs(this.hTarget.z - position.z)) * this.heuristicScale);
				uint val2 = (this.hTargetNode == null) ? 0u : AstarPath.active.euclideanEmbedding.GetHeuristic(node.NodeIndex, this.hTargetNode.NodeIndex);
				return Math.Max(val, val2);
			}
			case Heuristic.DiagonalManhattan:
			{
				Int3 @int = this.GetHTarget() - node.position;
				@int.x = Math.Abs(@int.x);
				@int.y = Math.Abs(@int.y);
				@int.z = Math.Abs(@int.z);
				int num = Math.Min(@int.x, @int.z);
				int num2 = Math.Max(@int.x, @int.z);
				uint val = (uint)((float)(14 * num / 10 + (num2 - num) + @int.y) * this.heuristicScale);
				uint val2 = (this.hTargetNode == null) ? 0u : AstarPath.active.euclideanEmbedding.GetHeuristic(node.NodeIndex, this.hTargetNode.NodeIndex);
				return Math.Max(val, val2);
			}
			case Heuristic.Euclidean:
			{
				uint val = (uint)((float)(this.GetHTarget() - node.position).costMagnitude * this.heuristicScale);
				uint val2 = (this.hTargetNode == null) ? 0u : AstarPath.active.euclideanEmbedding.GetHeuristic(node.NodeIndex, this.hTargetNode.NodeIndex);
				return Math.Max(val, val2);
			}
			default:
				return 0u;
			}
		}

		public uint GetTagPenalty(int tag)
		{
			return (uint)this.internalTagPenalties[tag];
		}

		public Int3 GetHTarget()
		{
			return this.hTarget;
		}

		public bool CanTraverse(GraphNode node)
		{
			return node.Walkable && (this.enabledTags >> (int)node.Tag & 1) != 0;
		}

		public uint GetTraversalCost(GraphNode node)
		{
			return this.GetTagPenalty((int)node.Tag) + node.Penalty;
		}

		public virtual uint GetConnectionSpecialCost(GraphNode a, GraphNode b, uint currentCost)
		{
			return currentCost;
		}

		public bool IsDone()
		{
			return this.CompleteState != PathCompleteState.NotCalculated;
		}

		public void AdvanceState(PathState s)
		{
			object obj = this.stateLock;
			lock (obj)
			{
				this.state = (PathState)Math.Max((int)this.state, (int)s);
			}
		}

		public PathState GetState()
		{
			return this.state;
		}

		public void LogError(string msg)
		{
			if (AstarPath.isEditor || AstarPath.active.logPathResults != PathLog.None)
			{
				this._errorLog += msg;
			}
			if (AstarPath.active.logPathResults != PathLog.None && AstarPath.active.logPathResults != PathLog.InGame)
			{
				UnityEngine.Debug.LogWarning(msg);
			}
		}

		public void ForceLogError(string msg)
		{
			this.Error();
			this._errorLog += msg;
			UnityEngine.Debug.LogError(msg);
		}

		public void Log(string msg)
		{
			if (AstarPath.isEditor || AstarPath.active.logPathResults != PathLog.None)
			{
				this._errorLog += msg;
			}
		}

		public void Error()
		{
			this.CompleteState = PathCompleteState.Error;
		}

		private void ErrorCheck()
		{
			if (!this.hasBeenReset)
			{
				throw new Exception("The path has never been reset. Use pooling API or call Reset() after creating the path with the default constructor.");
			}
			if (this.recycled)
			{
				throw new Exception("The path is currently in a path pool. Are you sending the path for calculation twice?");
			}
			if (this.pathHandler == null)
			{
				throw new Exception("Field pathHandler is not set. Please report this bug.");
			}
			if (this.GetState() > PathState.Processing)
			{
				throw new Exception("This path has already been processed. Do not request a path with the same path object twice.");
			}
		}

		public virtual void OnEnterPool()
		{
			if (this.vectorPath != null)
			{
				ListPool<Vector3>.Release(this.vectorPath);
			}
			if (this.path != null)
			{
				ListPool<GraphNode>.Release(this.path);
			}
			this.vectorPath = null;
			this.path = null;
		}

		public virtual void Reset()
		{
			if (object.ReferenceEquals(AstarPath.active, null))
			{
				throw new NullReferenceException("No AstarPath object found in the scene. Make sure there is one or do not create paths in Awake");
			}
			this.hasBeenReset = true;
			this.state = PathState.Created;
			this.releasedNotSilent = false;
			this.pathHandler = null;
			this.callback = null;
			this._errorLog = string.Empty;
			this.pathCompleteState = PathCompleteState.NotCalculated;
			this.path = ListPool<GraphNode>.Claim();
			this.vectorPath = ListPool<Vector3>.Claim();
			this.currentR = null;
			this.duration = 0f;
			this.searchIterations = 0;
			this.searchedNodes = 0;
			this.nnConstraint = PathNNConstraint.Default;
			this.next = null;
			this.heuristic = AstarPath.active.heuristic;
			this.heuristicScale = AstarPath.active.heuristicScale;
			this.enabledTags = -1;
			this.tagPenalties = null;
			this.callTime = DateTime.UtcNow;
			this.pathID = AstarPath.active.GetNextPathID();
			this.hTarget = Int3.zero;
			this.hTargetNode = null;
		}

		protected bool HasExceededTime(int searchedNodes, long targetTime)
		{
			return DateTime.UtcNow.Ticks >= targetTime;
		}

		protected abstract void Recycle();

		public void Claim(object o)
		{
			if (object.ReferenceEquals(o, null))
			{
				throw new ArgumentNullException("o");
			}
			for (int i = 0; i < this.claimed.Count; i++)
			{
				if (object.ReferenceEquals(this.claimed[i], o))
				{
					throw new ArgumentException("You have already claimed the path with that object (" + o + "). Are you claiming the path with the same object twice?");
				}
			}
			this.claimed.Add(o);
		}

		public void ReleaseSilent(object o)
		{
			if (o == null)
			{
				throw new ArgumentNullException("o");
			}
			for (int i = 0; i < this.claimed.Count; i++)
			{
				if (object.ReferenceEquals(this.claimed[i], o))
				{
					this.claimed.RemoveAt(i);
					if (this.releasedNotSilent && this.claimed.Count == 0)
					{
						this.Recycle();
					}
					return;
				}
			}
			if (this.claimed.Count == 0)
			{
				throw new ArgumentException("You are releasing a path which is not claimed at all (most likely it has been pooled already). Are you releasing the path with the same object (" + o + ") twice?");
			}
			throw new ArgumentException("You are releasing a path which has not been claimed with this object (" + o + "). Are you releasing the path with the same object twice?");
		}

		public void Release(object o)
		{
			if (o == null)
			{
				throw new ArgumentNullException("o");
			}
			for (int i = 0; i < this.claimed.Count; i++)
			{
				if (object.ReferenceEquals(this.claimed[i], o))
				{
					this.claimed.RemoveAt(i);
					this.releasedNotSilent = true;
					if (this.claimed.Count == 0)
					{
						this.Recycle();
					}
					return;
				}
			}
			if (this.claimed.Count == 0)
			{
				throw new ArgumentException("You are releasing a path which is not claimed at all (most likely it has been pooled already). Are you releasing the path with the same object (" + o + ") twice?");
			}
			throw new ArgumentException("You are releasing a path which has not been claimed with this object (" + o + "). Are you releasing the path with the same object twice?");
		}

		protected virtual void Trace(PathNode from)
		{
			int num = 0;
			PathNode pathNode = from;
			while (pathNode != null)
			{
				pathNode = pathNode.parent;
				num++;
				if (num > 2048)
				{
					UnityEngine.Debug.LogWarning("Infinite loop? >2048 node path. Remove this message if you really have that long paths (Path.cs, Trace method)");
					break;
				}
			}
			if (this.path.Capacity < num)
			{
				this.path.Capacity = num;
			}
			if (this.vectorPath.Capacity < num)
			{
				this.vectorPath.Capacity = num;
			}
			pathNode = from;
			for (int i = 0; i < num; i++)
			{
				this.path.Add(pathNode.node);
				pathNode = pathNode.parent;
			}
			int num2 = num / 2;
			for (int j = 0; j < num2; j++)
			{
				GraphNode value = this.path[j];
				this.path[j] = this.path[num - j - 1];
				this.path[num - j - 1] = value;
			}
			for (int k = 0; k < num; k++)
			{
				this.vectorPath.Add((Vector3)this.path[k].position);
			}
		}

		public virtual string DebugString(PathLog logMode)
		{
			if (logMode == PathLog.None || (!this.error && logMode == PathLog.OnlyErrors))
			{
				return string.Empty;
			}
			StringBuilder debugStringBuilder = this.pathHandler.DebugStringBuilder;
			debugStringBuilder.Length = 0;
			debugStringBuilder.Append((!this.error) ? "Path Completed : " : "Path Failed : ");
			debugStringBuilder.Append("Computation Time ");
			debugStringBuilder.Append(this.duration.ToString((logMode != PathLog.Heavy) ? "0.00 ms " : "0.000 ms "));
			debugStringBuilder.Append("Searched Nodes ");
			debugStringBuilder.Append(this.searchedNodes);
			if (!this.error)
			{
				debugStringBuilder.Append(" Path Length ");
				debugStringBuilder.Append((this.path != null) ? this.path.Count.ToString() : "Null");
				if (logMode == PathLog.Heavy)
				{
					debugStringBuilder.Append("\nSearch Iterations " + this.searchIterations);
				}
			}
			if (this.error)
			{
				debugStringBuilder.Append("\nError: ");
				debugStringBuilder.Append(this.errorLog);
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
			debugStringBuilder.Append(this.pathID);
			return debugStringBuilder.ToString();
		}

		public virtual void ReturnPath()
		{
			if (this.callback != null)
			{
				this.callback(this);
			}
		}

		public void PrepareBase(PathHandler pathHandler)
		{
			if (pathHandler.PathID > this.pathID)
			{
				pathHandler.ClearPathIDs();
			}
			this.pathHandler = pathHandler;
			pathHandler.InitializeForPath(this);
			if (this.internalTagPenalties == null || this.internalTagPenalties.Length != 32)
			{
				this.internalTagPenalties = Path.ZeroTagPenalties;
			}
			try
			{
				this.ErrorCheck();
			}
			catch (Exception ex)
			{
				this.ForceLogError(string.Concat(new object[]
				{
					"Exception in path ",
					this.pathID,
					"\n",
					ex
				}));
			}
		}

		public abstract void Prepare();

		public virtual void Cleanup()
		{
		}

		public abstract void Initialize();

		public abstract void CalculateStep(long targetTick);
	}
}
