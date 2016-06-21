using System;
using UnityEngine;

public class animalAvoidance1 : MonoBehaviour
{
	public GameObject ControllerGo;

	private animalAI ai;

	private void Start()
	{
		this.ai = this.ControllerGo.GetComponent<animalAI>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other)
		{
			return;
		}
		if (!other.gameObject)
		{
			return;
		}
		if (other.gameObject.CompareTag("Tree") || other.gameObject.CompareTag("jumpObject"))
		{
			Vector3 vector = base.transform.InverseTransformPoint(other.transform.position);
			if (vector.x > 0f && this.ai.turnInt != 1)
			{
				this.ai.turnInt = -1;
			}
			else if (vector.x < 0f && this.ai.turnInt != -1)
			{
				this.ai.turnInt = 1;
			}
			base.Invoke("disableCloseTurn", 1f);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!other)
		{
			return;
		}
		if (!other.gameObject)
		{
			return;
		}
		if (other.gameObject.CompareTag("Tree") || other.gameObject.CompareTag("jumpObject"))
		{
			base.Invoke("disableCloseTurn", 0.1f);
		}
	}

	private void disableCloseTurn()
	{
		this.ai.turnInt = 0;
	}
}
