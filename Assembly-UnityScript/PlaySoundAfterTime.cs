using System;
using UnityEngine;

[Serializable]
public class PlaySoundAfterTime : MonoBehaviour
{
	public int waitTime;

	public PlaySoundAfterTime()
	{
		this.waitTime = 2;
	}

	public override void Start()
	{
		this.Invoke("PlaySound", (float)this.waitTime);
	}

	public override void PlaySound()
	{
		this.GetComponent<AudioSource>().Play();
	}

	public override void Main()
	{
	}
}
