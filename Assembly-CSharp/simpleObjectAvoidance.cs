using System;
using UnityEngine;

public class simpleObjectAvoidance : MonoBehaviour
{
	private mutantScriptSetup setup;

	private CharacterController controller;

	private Transform orientTr;

	private bool slideMove;

	private int dir;

	public float speed;

	private void Start()
	{
		this.controller = base.GetComponent<CharacterController>();
		this.setup = base.transform.GetComponentInChildren<mutantScriptSetup>();
		this.orientTr = this.setup.thisGo.transform;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Tree"))
		{
			Debug.Log("hit a tree");
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.CompareTag("Tree"))
		{
			Debug.Log("collided with a tree!");
			Vector3 vector = this.orientTr.InverseTransformPoint(other.transform.position);
			float num = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
			if (num < 0f)
			{
				this.dir = -1;
			}
			else
			{
				this.dir = 1;
			}
			this.slideMove = true;
			base.Invoke("resetSlideMove", 0.5f);
		}
	}

	private void resetSlideMove()
	{
		this.slideMove = false;
	}

	private void LateUpdate()
	{
		if (this.slideMove)
		{
			base.transform.Translate(this.orientTr.right * (float)this.dir * this.speed * Time.deltaTime, Space.World);
		}
	}
}
