using System;
using UnityEngine;

public class StartGeese : MonoBehaviour
{
	public GameObject[] geese;

	private FMOD_StudioEventEmitter emitter;

	private void Start()
	{
		this.emitter = base.GetComponent<FMOD_StudioEventEmitter>();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Grabber"))
		{
			this.emitter.Play();
			GameObject[] array = this.geese;
			for (int i = 0; i < array.Length; i++)
			{
				GameObject gameObject = array[i];
				gameObject.SendMessage("Fly", SendMessageOptions.DontRequireReceiver);
			}
			base.Invoke("resetGeese", 600f);
			base.gameObject.GetComponent<Collider>().enabled = false;
		}
	}

	private void resetGeese()
	{
		GameObject[] array = this.geese;
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject = array[i];
			gameObject.SetActive(true);
			base.gameObject.GetComponent<Collider>().enabled = true;
		}
	}
}
