using Pathfinding.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
	public class RichPath
	{
		private int currentPart;

		private readonly List<RichPathPart> parts = new List<RichPathPart>();

		public Seeker seeker;

		public void Initialize(Seeker s, Path p, bool mergePartEndpoints, RichFunnel.FunnelSimplification simplificationMode)
		{
			if (p.error)
			{
				throw new ArgumentException("Path has an error");
			}
			List<GraphNode> path = p.path;
			if (path.Count == 0)
			{
				throw new ArgumentException("Path traverses no nodes");
			}
			this.seeker = s;
			for (int i = 0; i < this.parts.Count; i++)
			{
				if (this.parts[i] is RichFunnel)
				{
					ObjectPool<RichFunnel>.Release(this.parts[i] as RichFunnel);
				}
				else if (this.parts[i] is RichSpecial)
				{
					ObjectPool<RichSpecial>.Release(this.parts[i] as RichSpecial);
				}
			}
			this.parts.Clear();
			this.currentPart = 0;
			for (int j = 0; j < path.Count; j++)
			{
				if (path[j] is TriangleMeshNode)
				{
					IFunnelGraph graph = AstarData.GetGraph(path[j]) as IFunnelGraph;
					RichFunnel richFunnel = ObjectPool<RichFunnel>.Claim().Initialize(this, graph);
					richFunnel.funnelSimplificationMode = simplificationMode;
					int num = j;
					uint graphIndex = path[num].GraphIndex;
					while (j < path.Count)
					{
						if (path[j].GraphIndex != graphIndex && !(path[j] is NodeLink3Node))
						{
							break;
						}
						j++;
					}
					j--;
					if (num == 0)
					{
						richFunnel.exactStart = p.vectorPath[0];
					}
					else
					{
						richFunnel.exactStart = (Vector3)path[(!mergePartEndpoints) ? num : (num - 1)].position;
					}
					if (j == path.Count - 1)
					{
						richFunnel.exactEnd = p.vectorPath[p.vectorPath.Count - 1];
					}
					else
					{
						richFunnel.exactEnd = (Vector3)path[(!mergePartEndpoints) ? j : (j + 1)].position;
					}
					richFunnel.BuildFunnelCorridor(path, num, j);
					this.parts.Add(richFunnel);
				}
				else if (NodeLink2.GetNodeLink(path[j]) != null)
				{
					NodeLink2 nodeLink = NodeLink2.GetNodeLink(path[j]);
					int num2 = j;
					uint graphIndex2 = path[num2].GraphIndex;
					for (j++; j < path.Count; j++)
					{
						if (path[j].GraphIndex != graphIndex2)
						{
							break;
						}
					}
					j--;
					if (j - num2 > 1)
					{
						throw new Exception("NodeLink2 path length greater than two (2) nodes. " + (j - num2));
					}
					if (j - num2 != 0)
					{
						RichSpecial item = ObjectPool<RichSpecial>.Claim().Initialize(nodeLink, path[num2]);
						this.parts.Add(item);
					}
				}
			}
		}

		public bool PartsLeft()
		{
			return this.currentPart < this.parts.Count;
		}

		public void NextPart()
		{
			this.currentPart++;
			if (this.currentPart >= this.parts.Count)
			{
				this.currentPart = this.parts.Count;
			}
		}

		public RichPathPart GetCurrentPart()
		{
			return (this.currentPart >= this.parts.Count) ? null : this.parts[this.currentPart];
		}
	}
}
