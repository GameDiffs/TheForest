using Bolt;
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
		if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("PlayerNet") || other.gameObject.CompareTag("Weapon"))
		{
			if (BoltNetwork.isClient)
			{
				deadSharkDestroy deadSharkDestroy = deadSharkDestroy.Create(GlobalTargets.OnlyServer);
				deadSharkDestroy.target = base.transform.parent.GetComponent<BoltEntity>();
				deadSharkDestroy.switchToRagdoll = true;
				deadSharkDestroy.Send();
				this.doneRagdoll = true;
			}
			else
			{
				this.enableRagDoll();
			}
		}
	}

	public void enableRagDoll()
	{
		this.doneRagdoll = true;
		UnityEngine.Object.Instantiate(this.ragDoll, base.transform.position, base.transform.rotation);
		if (!BoltNetwork.isClient)
		{
			SkinnedMeshRenderer[] componentsInChildren = base.transform.GetComponentsInChildren<SkinnedMeshRenderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = false;
			}
		}
		if (this.destroyParent)
		{
			UnityEngine.Object.Destroy(base.transform.parent.gameObject, 0.1f);
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject, 0.1f);
		}
	}
}
