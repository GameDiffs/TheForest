using System;
using UnityEngine;

public class effectPlayVolume : MonoBehaviour
{
	public ParticleSystem effectToPlay;

	public bool armed;

	private void OnTriggerEnter(Collider other)
	{
		this.armed = true;
	}

	private void OnTriggerExit(Collider other)
	{
		this.armed = false;
	}

	private void Update()
	{
		if (this.armed && Input.GetKeyDown(KeyCode.Space))
		{
			if (this.effectToPlay.isPlaying)
			{
				this.effectToPlay.Clear();
				this.effectToPlay.Play();
			}
			else
			{
				this.effectToPlay.Play();
			}
		}
	}
}
