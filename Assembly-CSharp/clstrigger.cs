using System;
using UnityEngine;

public class clstrigger : MonoBehaviour
{
	private void OnTriggerEnter(Collider varsource)
	{
		if (varsource.name == "bumper")
		{
			clskinetify component = base.gameObject.GetComponent<clskinetify>();
			if (component != null)
			{
				component.metgodriven();
			}
		}
	}
}
