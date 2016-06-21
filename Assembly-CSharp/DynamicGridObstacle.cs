using Pathfinding;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DynamicGridObstacle : MonoBehaviour
{
	private Collider col;

	public float updateError = 1f;

	public float checkTime = 0.2f;

	private Bounds prevBounds;

	private bool isWaitingForUpdate;

	private void Start()
	{
		this.col = base.GetComponent<Collider>();
		if (this.col == null)
		{
			throw new Exception("A collider must be attached to the GameObject for DynamicGridObstacle to work");
		}
		base.StartCoroutine(this.UpdateGraphs());
	}

	[DebuggerHidden]
	private IEnumerator UpdateGraphs()
	{
		DynamicGridObstacle.<UpdateGraphs>c__Iterator11 <UpdateGraphs>c__Iterator = new DynamicGridObstacle.<UpdateGraphs>c__Iterator11();
		<UpdateGraphs>c__Iterator.<>f__this = this;
		return <UpdateGraphs>c__Iterator;
	}

	public void OnDestroy()
	{
		if (AstarPath.active != null)
		{
			GraphUpdateObject ob = new GraphUpdateObject(this.prevBounds);
			AstarPath.active.UpdateGraphs(ob);
		}
	}

	public void DoUpdateGraphs()
	{
		if (this.col == null)
		{
			return;
		}
		this.isWaitingForUpdate = false;
		Bounds bounds = this.col.bounds;
		Bounds bounds2 = bounds;
		bounds2.Encapsulate(this.prevBounds);
		if (DynamicGridObstacle.BoundsVolume(bounds2) < DynamicGridObstacle.BoundsVolume(bounds) + DynamicGridObstacle.BoundsVolume(this.prevBounds))
		{
			AstarPath.active.UpdateGraphs(bounds2);
		}
		else
		{
			AstarPath.active.UpdateGraphs(this.prevBounds);
			AstarPath.active.UpdateGraphs(bounds);
		}
		this.prevBounds = bounds;
	}

	private static float BoundsVolume(Bounds b)
	{
		return Math.Abs(b.size.x * b.size.y * b.size.z);
	}
}
