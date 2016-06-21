using System;
using UnityEngine;

public class SimpleAnim : MonoBehaviour
{
	private Animator anim;

	private void Start()
	{
		this.anim = base.GetComponent<Animator>();
		base.Invoke("Crawl", UnityEngine.Random.Range(0f, 2f));
	}

	private void Crawl()
	{
		this.anim.SetBool("Crawl", true);
	}
}
