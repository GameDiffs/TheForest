using System;
using UnityEngine;

public class batScareTrigger : MonoBehaviour
{
	public GameObject batGameObject;

	private bool doBatScare;

	[Header("ANIMATION PLAYED")]
	public AnimationClip batAnimation;

	private void Start()
	{
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			this.doBatScare = true;
			if (this.doBatScare)
			{
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		this.doBatScare = false;
	}
}
