using System;
using UnityEngine;

public class sharkGoRagdoll : MonoBehaviour
{
	public bool destroyParent;

	public GameObject ragDoll;

	private bool doneRagdoll;

	private void OnTriggerEnter(Collider other)
	{
		if (this.doneRagdoll)
		{
			return;
		}
		if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Weapon"))
		{
			this.doneRagdoll = true;
			UnityEngine.Object.Instantiate(this.ragDoll, base.transform.position, base.transform.rotation);
			if (this.destroyParent)
			{
				UnityEngine.Object.Destroy(base.transform.parent.gameObject);
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}
}
