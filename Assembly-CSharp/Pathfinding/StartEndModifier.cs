using System;
using UnityEngine;

namespace Pathfinding
{
	[Serializable]
	public class StartEndModifier : PathModifier
	{
		public enum Exactness
		{
			SnapToNode,
			Original,
			Interpolate,
			ClosestOnNode
		}

		public bool addPoints;

		public StartEndModifier.Exactness exactStartPoint = StartEndModifier.Exactness.ClosestOnNode;

		public StartEndModifier.Exactness exactEndPoint = StartEndModifier.Exactness.ClosestOnNode;

		public bool useRaycasting;

		public LayerMask mask = -1;

		public bool useGraphRaycasting;

		public override ModifierData input
		{
			get
			{
				return ModifierData.Vector;
			}
		}

		public override ModifierData output
		{
			get
			{
				return ((!this.addPoints) ? ModifierData.StrictVectorPath : ModifierData.None) | ModifierData.VectorPath;
			}
		}

		public override void Apply(Path _p, ModifierData source)
		{
			ABPath aBPath = _p as ABPath;
			if (aBPath == null)
			{
				return;
			}
			if (aBPath.vectorPath.Count == 0)
			{
				return;
			}
			if (aBPath.vectorPath.Count == 1 && !this.addPoints)
			{
				aBPath.vectorPath.Add(aBPath.vectorPath[0]);
			}
			Vector3 vector = Vector3.zero;
			Vector3 vector2 = Vector3.zero;
			switch (this.exactStartPoint)
			{
			case StartEndModifier.Exactness.SnapToNode:
				vector = (Vector3)aBPath.path[0].position;
				break;
			case StartEndModifier.Exactness.Original:
				vector = this.GetClampedPoint((Vector3)aBPath.path[0].position, aBPath.originalStartPoint, aBPath.path[0]);
				break;
			case StartEndModifier.Exactness.Interpolate:
				vector = this.GetClampedPoint((Vector3)aBPath.path[0].position, aBPath.originalStartPoint, aBPath.path[0]);
				vector = AstarMath.NearestPointStrict((Vector3)aBPath.path[0].position, (Vector3)aBPath.path[(1 < aBPath.path.Count) ? 1 : 0].position, vector);
				break;
			case StartEndModifier.Exactness.ClosestOnNode:
				vector = this.GetClampedPoint((Vector3)aBPath.path[0].position, aBPath.startPoint, aBPath.path[0]);
				break;
			}
			switch (this.exactEndPoint)
			{
			case StartEndModifier.Exactness.SnapToNode:
				vector2 = (Vector3)aBPath.path[aBPath.path.Count - 1].position;
				break;
			case StartEndModifier.Exactness.Original:
				vector2 = this.GetClampedPoint((Vector3)aBPath.path[aBPath.path.Count - 1].position, aBPath.originalEndPoint, aBPath.path[aBPath.path.Count - 1]);
				break;
			case StartEndModifier.Exactness.Interpolate:
				vector2 = this.GetClampedPoint((Vector3)aBPath.path[aBPath.path.Count - 1].position, aBPath.originalEndPoint, aBPath.path[aBPath.path.Count - 1]);
				vector2 = AstarMath.NearestPointStrict((Vector3)aBPath.path[aBPath.path.Count - 1].position, (Vector3)aBPath.path[(aBPath.path.Count - 2 >= 0) ? (aBPath.path.Count - 2) : 0].position, vector2);
				break;
			case StartEndModifier.Exactness.ClosestOnNode:
				vector2 = this.GetClampedPoint((Vector3)aBPath.path[aBPath.path.Count - 1].position, aBPath.endPoint, aBPath.path[aBPath.path.Count - 1]);
				break;
			}
			if (!this.addPoints)
			{
				aBPath.vectorPath[0] = vector;
				aBPath.vectorPath[aBPath.vectorPath.Count - 1] = vector2;
			}
			else
			{
				if (this.exactStartPoint != StartEndModifier.Exactness.SnapToNode)
				{
					aBPath.vectorPath.Insert(0, vector);
				}
				if (this.exactEndPoint != StartEndModifier.Exactness.SnapToNode)
				{
					aBPath.vectorPath.Add(vector2);
				}
			}
		}

		public Vector3 GetClampedPoint(Vector3 from, Vector3 to, GraphNode hint)
		{
			Vector3 vector = to;
			RaycastHit raycastHit;
			if (this.useRaycasting && Physics.Linecast(from, to, out raycastHit, this.mask))
			{
				vector = raycastHit.point;
			}
			if (this.useGraphRaycasting && hint != null)
			{
				NavGraph graph = AstarData.GetGraph(hint);
				if (graph != null)
				{
					IRaycastableGraph raycastableGraph = graph as IRaycastableGraph;
					GraphHitInfo graphHitInfo;
					if (raycastableGraph != null && raycastableGraph.Linecast(from, vector, hint, out graphHitInfo))
					{
						vector = graphHitInfo.point;
					}
				}
			}
			return vector;
		}
	}
}
