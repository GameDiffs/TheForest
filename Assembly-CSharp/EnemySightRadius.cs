using System;
using UnityEngine;

public class EnemySightRadius : MonoBehaviour
{
	public float fieldOfViewAngle = 200f;

	private Vector3 Target;

	private Vector3 dist;

	private DoneLastPlayerSighting lastPlayerSighting;

	private Transform player;

	private float distance;

	private int SoundRandom;

	private bool SoundPlaying;

	public GameObject HorrorSting1;

	public GameObject HorrorSting2;

	public GameObject Snarl;

	private void Awake()
	{
		this.player = GameObject.FindWithTag("Player").transform;
	}

	private void Update()
	{
		this.distance = Vector3.Distance(base.transform.position, this.player.position);
		if (this.distance < 60f)
		{
			this.CheckForPlayer();
		}
	}

	private void CheckForPlayer()
	{
		Vector3 from = this.player.transform.position - base.transform.position;
		float num = Vector3.Angle(from, base.transform.forward);
		RaycastHit raycastHit;
		if (num < this.fieldOfViewAngle * 1f && Physics.Raycast(base.transform.position, from.normalized, out raycastHit) && raycastHit.collider.gameObject == this.player)
		{
			this.PlayHorrorSting();
		}
	}

	private void PlayHorrorSting()
	{
		this.SoundRandom = UnityEngine.Random.Range(0, 5);
		if (!this.SoundPlaying)
		{
			this.SoundPlaying = true;
			base.Invoke("StopSound", 20f);
			if (this.SoundRandom == 0)
			{
				this.Snarl.GetComponent<AudioSource>().Play();
			}
			else if (this.SoundRandom == 1)
			{
				this.HorrorSting1.GetComponent<AudioSource>().Play();
			}
			else if (this.SoundRandom == 2)
			{
				this.HorrorSting2.GetComponent<AudioSource>().Play();
			}
		}
	}

	private void StopSound()
	{
		this.SoundPlaying = false;
	}
}
