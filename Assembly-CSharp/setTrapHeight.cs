using System;
using UnityEngine;

public class setTrapHeight : MonoBehaviour
{
	public LayerMask mask;

	private void OnEnable()
	{
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position, Vector3.down, out raycastHit, 20f, this.mask) && raycastHit.distance < 11f)
		{
			Vector3 point = raycastHit.point;
			point.y += 11f;
			base.transform.position = point;
		}
	}
}
