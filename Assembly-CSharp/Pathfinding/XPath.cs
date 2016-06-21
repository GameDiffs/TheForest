using System;
using UnityEngine;

namespace Pathfinding
{
	public class XPath : ABPath
	{
		public PathEndingCondition endingCondition;

		public new static XPath Construct(Vector3 start, Vector3 end, OnPathDelegate callback = null)
		{
			XPath path = PathPool<XPath>.GetPath();
			path.Setup(start, end, callback);
			path.endingCondition = new ABPathEndingCondition(path);
			return path;
		}

		protected override void Recycle()
		{
			PathPool<XPath>.Recycle(this);
		}

		public override void Reset()
		{
			base.Reset();
			this.endingCondition = null;
		}

		protected override void CompletePathIfStartIsValidTarget()
		{
			PathNode pathNode = base.pathHandler.GetPathNode(this.startNode);
			if (this.endingCondition.TargetFound(pathNode))
			{
				this.endNode = pathNode.node;
				this.endPoint = (Vector3)this.endNode.position;
				this.Trace(pathNode);
				base.CompleteState = PathCompleteState.Complete;
			}
		}

		public override void CalculateStep(long targetTick)
		{
			int num = 0;
			while (base.CompleteState == PathCompleteState.NotCalculated)
			{
				this.searchedNodes++;
				if (this.endingCondition.TargetFound(this.currentR))
				{
					base.CompleteState = PathCompleteState.Complete;
					break;
				}
				this.currentR.node.Open(this, this.currentR, base.pathHandler);
				if (base.pathHandler.HeapEmpty())
				{
					base.Error();
					base.LogError("Searched whole area but could not find target");
					return;
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
				this.endNode = this.currentR.node;
				this.endPoint = (Vector3)this.endNode.position;
				this.Trace(this.currentR);
			}
		}
	}
}
