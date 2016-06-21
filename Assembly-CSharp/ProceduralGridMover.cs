using Pathfinding;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class ProceduralGridMover : MonoBehaviour
{
	public float updateDistance = 10f;

	public Transform target;

	public bool floodFill;

	private GridGraph graph;

	private GridNode[] tmp;

	public bool updatingGraph
	{
		get;
		private set;
	}

	public void Start()
	{
		if (AstarPath.active == null)
		{
			throw new Exception("There is no AstarPath object in the scene");
		}
		this.graph = AstarPath.active.astarData.gridGraph;
		if (this.graph == null)
		{
			throw new Exception("The AstarPath object has no GridGraph");
		}
		this.UpdateGraph();
	}

	private void Update()
	{
		Vector3 a = this.PointToGraphSpace(this.graph.center);
		Vector3 b = this.PointToGraphSpace(this.target.position);
		if (AstarMath.SqrMagnitudeXZ(a, b) > this.updateDistance * this.updateDistance)
		{
			this.UpdateGraph();
		}
	}

	private Vector3 PointToGraphSpace(Vector3 p)
	{
		return this.graph.inverseMatrix.MultiplyPoint(p);
	}

	public void UpdateGraph()
	{
		if (this.updatingGraph)
		{
			return;
		}
		this.updatingGraph = true;
		IEnumerator ie = this.UpdateGraphCoroutine();
		AstarPath.active.AddWorkItem(new AstarPath.AstarWorkItem(delegate(bool force)
		{
			if (force)
			{
				while (ie.MoveNext())
				{
				}
			}
			bool flag = !ie.MoveNext();
			if (flag)
			{
				this.updatingGraph = false;
			}
			return flag;
		}));
	}

	[DebuggerHidden]
	private IEnumerator UpdateGraphCoroutine()
	{
		ProceduralGridMover.<UpdateGraphCoroutine>c__IteratorB <UpdateGraphCoroutine>c__IteratorB = new ProceduralGridMover.<UpdateGraphCoroutine>c__IteratorB();
		<UpdateGraphCoroutine>c__IteratorB.<>f__this = this;
		return <UpdateGraphCoroutine>c__IteratorB;
	}
}
