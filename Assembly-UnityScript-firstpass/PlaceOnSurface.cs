using System;
using UnityEngine;

[Serializable]
public class PlaceOnSurface : MonoBehaviour
{
	public float PivotOffset;

	public int RaycastOffset;

	public LayerMask GroundLayer;

	public PlaceOnSurface()
	{
		this.RaycastOffset = 100;
	}

	public override void Start()
	{
		RaycastHit raycastHit = default(RaycastHit);
		if (Physics.Raycast(this.transform.position + Vector3.up * (float)this.RaycastOffset, -Vector3.up, out raycastHit, float.PositiveInfinity, this.GroundLayer))
		{
			float d = raycastHit.distance - (float)this.RaycastOffset - this.PivotOffset;
			this.transform.Translate(-Vector3.up * d, Space.World);
		}
	}

	public override void Main()
	{
	}
}
