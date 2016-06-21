using System;
using UnityEngine;

public class startPlaying_stopPlaying : MonoBehaviour
{
	public Animator anim;

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
			this.StartPlaying();
		}
	}

	private void StartPlaying()
	{
		this.anim.speed = 1f;
	}

	private void StopPlaying()
	{
		this.anim.speed = 0f;
	}
}
