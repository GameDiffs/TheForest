using System;
using UnityEngine;

[Serializable]
public class HeadOnStickFlash : MonoBehaviour
{
	public GameObject MyLight;

	private bool Delay;

	public override void OnTriggerEnter(Collider otherObject)
	{
		if (otherObject.gameObject.CompareTag("Grabber") && !this.Delay)
		{
			this.MyLight.SetActive(true);
			this.GetComponent<AudioSource>().Play();
			this.Delay = true;
			this.Invoke("LightOff", 0.6f);
		}
	}

	public override void LightOff()
	{
		this.MyLight.SetActive(false);
		this.Invoke("NoDelay", (float)900);
	}

	public override void NoDelay()
	{
		this.Delay = false;
	}

	public override void Main()
	{
	}
}
