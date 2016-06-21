using Pathfinding;
using System;
using UnityEngine;

public class navMeshChecker : MonoBehaviour
{
	private bool init;

	private void Start()
	{
		this.init = true;
	}

	private void OnEnable()
	{
		if (!this.init)
		{
			return;
		}
		GraphNode node = AstarPath.active.GetNearest(base.transform.position).node;
		if (!node.Walkable)
		{
			base.gameObject.SetActive(false);
		}
	}
}
