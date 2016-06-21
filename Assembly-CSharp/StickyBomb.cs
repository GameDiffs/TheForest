using System;
using UnityEngine;

public class StickyBomb : MonoBehaviour
{
	private void OnCollisionEnter(Collision other)
	{
		FollowTarget followTarget = base.gameObject.AddComponent<FollowTarget>();
		followTarget.target = other.transform;
		followTarget.offset = base.transform.position - other.transform.position;
		base.GetComponent<Rigidbody>().isKinematic = true;
		UnityEngine.Object.Destroy(this);
	}
}
