using System;
using UnityEngine;

public class setTriggerHeight : MonoBehaviour
{
	public LayerMask mask;

	private void Start()
	{
		RaycastHit raycastHit;
		if (Physics.Raycast(base.transform.position + Vector3.up * 7f, Vector3.down, out raycastHit, 20f, this.mask))
		{
			base.transform.position = raycastHit.point;
		}
	}
}
