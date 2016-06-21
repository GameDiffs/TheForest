using System;
using UnityEngine;

public class gooseRagdollify : MonoBehaviour
{
	public Transform vargamragdoll;

	public bool burning;

	public bool spinRagdoll;

	public bool hackVelocity;

	public Transform hitTr;

	public Vector3 getVelocity;

	public Transform metgoragdoll(Vector3 varpvelocity = default(Vector3))
	{
		Transform transform = UnityEngine.Object.Instantiate(this.vargamragdoll, base.transform.position, base.transform.rotation) as Transform;
		transform.localScale = base.transform.localScale;
		Rigidbody[] componentsInChildren = transform.GetComponentsInChildren<Rigidbody>();
		if (!this.spinRagdoll)
		{
			transform.GetComponent<tempRagdoll>().blockRagdoll();
		}
		else
		{
			transform.GetComponent<Animator>().enabled = false;
		}
		Rigidbody[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			Rigidbody rigidbody = array[i];
			if (rigidbody != null && this.spinRagdoll)
			{
				rigidbody.velocity = transform.transform.forward * 13f;
				ConstantForce component = rigidbody.GetComponent<ConstantForce>();
				if (component)
				{
					component.torque = Vector3.up * 1E+10f + base.transform.right * 1E+08f;
				}
			}
		}
		if (this.burning)
		{
			transform.gameObject.SendMessage("enableFire", SendMessageOptions.DontRequireReceiver);
		}
		this.burning = false;
		return transform;
	}
}
