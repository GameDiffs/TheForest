using Pathfinding;
using System;
using UnityEngine;

public class navRemoveReceiver : MonoBehaviour
{
	private Bounds getBounds;

	private void doDummyNavRemove(Bounds b)
	{
		this.getBounds = b;
		base.Invoke("dummyRemove", 8f);
	}

	private void dummyRemove()
	{
		if (!AstarPath.active)
		{
			return;
		}
		Terrain activeTerrain = Terrain.activeTerrain;
		if (activeTerrain)
		{
			GraphUpdateObject ob = new GraphUpdateObject(this.getBounds);
			AstarPath.active.UpdateGraphs(ob, 0f);
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
