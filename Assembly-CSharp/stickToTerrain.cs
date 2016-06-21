using System;
using UnityEngine;

public class stickToTerrain : MonoBehaviour
{
	private Transform Tr;

	private Vector3 pos;

	private int layer;

	private int layerMask;

	public bool onStart;

	public bool onPlane;

	public bool inCave;

	public bool rotateToTerrain;

	public bool parentToWorld;

	private RaycastHit hit;

	private void Start()
	{
		this.Tr = base.transform;
		if (this.onPlane)
		{
			this.layer = 20;
			this.layerMask = 1;
		}
		else if (this.inCave)
		{
			this.layerMask = 139264;
		}
		else
		{
			this.layer = 26;
			this.layerMask = 1 << this.layer;
		}
		if (this.onStart)
		{
			this.pos = new Vector3(this.Tr.position.x, this.Tr.position.y + 10f, this.Tr.position.z);
			if (Physics.Raycast(this.pos, Vector3.down, out this.hit, 60f, this.layerMask))
			{
				this.Tr.position = new Vector3(this.Tr.position.x, this.hit.point.y, this.Tr.position.z);
				if (this.rotateToTerrain)
				{
					this.Tr.rotation = Quaternion.LookRotation(Vector3.Cross(base.transform.right, this.hit.normal), this.hit.normal);
				}
			}
		}
		if (this.parentToWorld)
		{
			base.transform.parent = null;
		}
	}

	public void doAlignForEncounter()
	{
		this.pos = new Vector3(this.Tr.position.x, this.Tr.position.y + 10f, this.Tr.position.z);
		if (Physics.Raycast(this.pos, Vector3.down, out this.hit, 60f, this.layerMask))
		{
			this.Tr.position = new Vector3(this.Tr.position.x, this.hit.point.y, this.Tr.position.z);
			if (this.rotateToTerrain)
			{
				this.Tr.rotation = Quaternion.LookRotation(Vector3.Cross(base.transform.right, this.hit.normal), this.hit.normal);
			}
		}
	}

	private void LateUpdate()
	{
		if (!this.onStart)
		{
			this.pos = new Vector3(this.Tr.position.x, this.Tr.position.y + 3f, this.Tr.position.z);
			if (Physics.Raycast(this.pos, Vector3.down, out this.hit, 10f, this.layerMask))
			{
				this.Tr.position = new Vector3(this.Tr.position.x, this.hit.point.y, this.Tr.position.z);
				if (this.rotateToTerrain)
				{
					this.Tr.rotation = Quaternion.LookRotation(Vector3.Cross(base.transform.right, this.hit.normal), this.hit.normal);
				}
			}
		}
	}
}
