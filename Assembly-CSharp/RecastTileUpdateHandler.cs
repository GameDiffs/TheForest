using Pathfinding;
using System;
using UnityEngine;

[AddComponentMenu("Pathfinding/Navmesh/RecastTileUpdateHandler")]
public class RecastTileUpdateHandler : MonoBehaviour
{
	private RecastGraph graph;

	private bool[] dirtyTiles;

	private bool anyDirtyTiles;

	private float earliestDirty = float.NegativeInfinity;

	public float maxThrottlingDelay = 0.5f;

	public void SetGraph(RecastGraph graph)
	{
		this.graph = graph;
		if (graph == null)
		{
			return;
		}
		this.dirtyTiles = new bool[graph.tileXCount * graph.tileZCount];
		this.anyDirtyTiles = false;
	}

	public void ScheduleUpdate(Bounds bounds)
	{
		if (this.graph == null)
		{
			if (AstarPath.active != null)
			{
				this.SetGraph(AstarPath.active.astarData.recastGraph);
			}
			if (this.graph == null)
			{
				Debug.LogError("Received tile update request (from RecastTileUpdate), but no RecastGraph could be found to handle it");
				return;
			}
		}
		int num = Mathf.CeilToInt(this.graph.characterRadius / this.graph.cellSize);
		int num2 = num + 3;
		bounds.Expand(new Vector3((float)num2, 0f, (float)num2) * this.graph.cellSize * 2f);
		IntRect touchingTiles = this.graph.GetTouchingTiles(bounds);
		if (touchingTiles.Width * touchingTiles.Height > 0)
		{
			if (!this.anyDirtyTiles)
			{
				this.earliestDirty = Time.time;
				this.anyDirtyTiles = true;
			}
			for (int i = touchingTiles.ymin; i <= touchingTiles.ymax; i++)
			{
				for (int j = touchingTiles.xmin; j <= touchingTiles.xmax; j++)
				{
					this.dirtyTiles[i * this.graph.tileXCount + j] = true;
				}
			}
		}
	}

	private void OnEnable()
	{
		RecastTileUpdate.OnNeedUpdates += new Action<Bounds>(this.ScheduleUpdate);
	}

	private void OnDisable()
	{
		RecastTileUpdate.OnNeedUpdates -= new Action<Bounds>(this.ScheduleUpdate);
	}

	private void Update()
	{
		if (this.anyDirtyTiles && Time.time - this.earliestDirty >= this.maxThrottlingDelay && this.graph != null)
		{
			this.UpdateDirtyTiles();
		}
	}

	public void UpdateDirtyTiles()
	{
		if (this.graph == null)
		{
			new InvalidOperationException("No graph is set on this object");
		}
		if (this.graph.tileXCount * this.graph.tileZCount != this.dirtyTiles.Length)
		{
			Debug.LogError("Graph has changed dimensions. Clearing queued graph updates and resetting.");
			this.SetGraph(this.graph);
			return;
		}
		for (int i = 0; i < this.graph.tileZCount; i++)
		{
			for (int j = 0; j < this.graph.tileXCount; j++)
			{
				if (this.dirtyTiles[i * this.graph.tileXCount + j])
				{
					this.dirtyTiles[i * this.graph.tileXCount + j] = false;
					Bounds tileBounds = this.graph.GetTileBounds(j, i, 1, 1);
					tileBounds.extents *= 0.5f;
					GraphUpdateObject graphUpdateObject = new GraphUpdateObject(tileBounds);
					graphUpdateObject.nnConstraint.graphMask = 1 << (int)this.graph.graphIndex;
					AstarPath.active.UpdateGraphs(graphUpdateObject);
				}
			}
		}
		this.anyDirtyTiles = false;
	}
}
