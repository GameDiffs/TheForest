using System;
using UnityEngine;

public class rabbitEvents : MonoBehaviour
{
	private animalAI ai;

	public AudioClip[] hops;

	private void Awake()
	{
		this.ai = base.transform.parent.GetComponent<animalAI>();
	}

	private void playHop()
	{
		if (this.ai.fsmPlayerDist.Value < 35f)
		{
			base.GetComponent<AudioSource>().PlayOneShot(this.hops[UnityEngine.Random.Range(0, this.hops.Length)], 0.35f);
		}
	}
}
