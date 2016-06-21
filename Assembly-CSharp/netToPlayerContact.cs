using System;
using UnityEngine;

public class netToPlayerContact : MonoBehaviour
{
	private bool playerContactCheck;

	private bool startUp;

	private Animator animator;

	private void Start()
	{
		this.animator = base.transform.GetComponent<Animator>();
		base.Invoke("setStartUp", 1f);
	}

	private void setStartUp()
	{
		this.startUp = true;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!this.startUp)
		{
			return;
		}
		if (other.gameObject.CompareTag("enemyBlocker"))
		{
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.CompareTag("enemyBlocker"))
		{
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!this.startUp)
		{
			return;
		}
	}
}
