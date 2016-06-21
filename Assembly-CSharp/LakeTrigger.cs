using System;
using UnityEngine;

public class LakeTrigger : MonoBehaviour
{
	public Transform MyParticle;

	public GameObject BigSplash;

	private float posy;

	private Vector3 GetSplashPosition(Vector3 position)
	{
		position.y = base.transform.position.y;
		return position;
	}

	private void OnTriggerEnter(Collider other)
	{
		UnityEngine.Object.Instantiate(this.MyParticle, this.GetSplashPosition(other.transform.position), base.transform.rotation);
		if (other.gameObject.CompareTag("Player"))
		{
			other.SendMessage("GotClean");
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			this.BigSplash.SetActive(true);
			this.BigSplash.transform.position = this.GetSplashPosition(other.transform.position);
		}
		else
		{
			this.BigSplash.SetActive(false);
		}
	}
}
