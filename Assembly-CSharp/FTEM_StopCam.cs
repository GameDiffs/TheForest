using System;
using UnityEngine;

public class FTEM_StopCam : MonoBehaviour
{
	public bool myCheck = true;

	public GameObject camObject;

	private Animator camAnim;

	private void Start()
	{
		this.camAnim = this.camObject.GetComponent<Animator>();
	}

	private void OnMouseDown()
	{
		if (this.myCheck)
		{
			this.camAnim.speed = 0f;
			this.myCheck = false;
			return;
		}
		if (!this.myCheck)
		{
			this.camAnim.speed = 1f;
			this.myCheck = true;
			return;
		}
	}
}
