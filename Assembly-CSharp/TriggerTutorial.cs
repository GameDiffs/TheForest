using System;
using UnityEngine;

public class TriggerTutorial : MonoBehaviour
{
	public GameObject MyTut;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Grabber"))
		{
			this.MyTut.SetActive(true);
			this.MyTut.transform.parent.SendMessage("Reposition");
			base.Invoke("TurnOff", 5f);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("Grabber"))
		{
			this.MyTut.SetActive(false);
		}
	}

	private void TurnOff()
	{
		this.MyTut.SetActive(false);
	}
}
