using Pathfinding;
using System;
using UnityEngine;

public class DoorController : MonoBehaviour
{
	private bool open;

	public int opentag = 1;

	public int closedtag = 1;

	public bool updateGraphsWithGUO = true;

	public float yOffset = 5f;

	private Bounds bounds;

	public void Start()
	{
		this.bounds = base.GetComponent<Collider>().bounds;
		this.SetState(this.open);
	}

	private void OnGUI()
	{
		if (GUI.Button(new Rect(5f, this.yOffset, 100f, 22f), "Toggle Door"))
		{
			this.SetState(!this.open);
		}
	}

	public void SetState(bool open)
	{
		this.open = open;
		if (this.updateGraphsWithGUO)
		{
			GraphUpdateObject graphUpdateObject = new GraphUpdateObject(this.bounds);
			int num = (!open) ? this.closedtag : this.opentag;
			if (num > 31)
			{
				Debug.LogError("tag > 31");
				return;
			}
			graphUpdateObject.modifyTag = true;
			graphUpdateObject.setTag = num;
			graphUpdateObject.updatePhysics = false;
			AstarPath.active.UpdateGraphs(graphUpdateObject);
		}
		if (open)
		{
			base.GetComponent<Animation>().Play("Open");
		}
		else
		{
			base.GetComponent<Animation>().Play("Close");
		}
	}
}
