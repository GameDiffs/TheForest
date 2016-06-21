using System;
using UnityEngine;

public class SimpleLODClickObject : MonoBehaviour
{
	public Light emphasisLight;

	public bool sendNullInstead;

	private void Start()
	{
		if (this.emphasisLight != null)
		{
			this.emphasisLight.enabled = false;
		}
	}

	private void OnMouseEnter()
	{
		if (this.emphasisLight != null)
		{
			this.emphasisLight.enabled = true;
		}
	}

	private void OnMouseExit()
	{
		if (this.emphasisLight != null)
		{
			this.emphasisLight.enabled = false;
		}
	}

	private void OnMouseUpAsButton()
	{
		if (this.sendNullInstead)
		{
			Camera.main.gameObject.GetComponent<SimpleLODDemoCamera>().SetClickedObject(null);
		}
		else
		{
			Camera.main.gameObject.GetComponent<SimpleLODDemoCamera>().SetClickedObject(base.gameObject);
		}
	}
}
