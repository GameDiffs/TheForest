using System;
using UnityEngine;

public class clskinetify : MonoBehaviour
{
	public Transform varsource;

	public void metgodriven()
	{
		Rigidbody[] componentsInChildren = base.GetComponentsInChildren<Rigidbody>();
		Rigidbody[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			Rigidbody rigidbody = array[i];
			rigidbody.isKinematic = false;
			if (this.varsource != null && this.varsource.GetComponent<Rigidbody>() != null)
			{
				this.varsource.GetComponent<Rigidbody>().AddForce(Vector3.up, ForceMode.VelocityChange);
			}
		}
	}
}
