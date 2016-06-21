using Pathfinding;
using System;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
	public GameObject go;

	public bool direct;

	public bool issueGUOs = true;

	private void Start()
	{
	}

	private void Update()
	{
		if (Input.GetKeyDown("p"))
		{
			this.PlaceObject();
		}
		if (Input.GetKeyDown("r"))
		{
			this.RemoveObject();
		}
	}

	public void PlaceObject()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit raycastHit;
		if (Physics.Raycast(ray, out raycastHit, float.PositiveInfinity))
		{
			Vector3 point = raycastHit.point;
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this.go, point, Quaternion.identity);
			if (this.issueGUOs)
			{
				Bounds bounds = gameObject.GetComponent<Collider>().bounds;
				GraphUpdateObject ob = new GraphUpdateObject(bounds);
				AstarPath.active.UpdateGraphs(ob);
				if (this.direct)
				{
					AstarPath.active.FlushGraphUpdates();
				}
			}
		}
	}

	public void RemoveObject()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit raycastHit;
		if (Physics.Raycast(ray, out raycastHit, float.PositiveInfinity))
		{
			if (raycastHit.collider.isTrigger || raycastHit.transform.gameObject.name == "Ground")
			{
				return;
			}
			Bounds bounds = raycastHit.collider.bounds;
			UnityEngine.Object.Destroy(raycastHit.collider);
			UnityEngine.Object.Destroy(raycastHit.collider.gameObject);
			if (this.issueGUOs)
			{
				GraphUpdateObject ob = new GraphUpdateObject(bounds);
				AstarPath.active.UpdateGraphs(ob, 0f);
				if (this.direct)
				{
					AstarPath.active.FlushGraphUpdates();
				}
			}
		}
	}
}
