using System;
using UnityEngine;

public class net_bookController : MonoBehaviour
{
	public Animator animator;

	public GameObject bookHeld;

	private Animator bookAnim;

	private void Start()
	{
		if (this.bookHeld)
		{
			this.bookAnim = this.bookHeld.GetComponent<Animator>();
		}
	}

	private void Update()
	{
		if (!this.bookHeld)
		{
			return;
		}
		if (this.animator.GetBool("bookHeld"))
		{
			this.bookHeld.SetActive(true);
			this.bookAnim.SetBool("bookHeld", true);
		}
		else
		{
			this.bookHeld.SetActive(false);
		}
	}
}
