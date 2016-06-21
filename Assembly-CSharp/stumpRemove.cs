using Pathfinding;
using System;
using UnityEngine;

public class stumpRemove : MonoBehaviour
{
	private void Start()
	{
		base.Invoke("doStumpRemove", UnityEngine.Random.Range(2f, 5f));
	}

	private void doStumpRemove()
	{
		if (!AstarPath.active)
		{
			return;
		}
		Terrain activeTerrain = Terrain.activeTerrain;
		if (activeTerrain)
		{
			Collider component = base.transform.GetComponent<Collider>();
			GraphUpdateObject ob = new GraphUpdateObject(component.bounds);
			AstarPath.active.UpdateGraphs(ob, 0f);
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
