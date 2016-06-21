using System;
using UnityEngine;

public class birdAvoidance : MonoBehaviour
{
	private lb_Bird bird;

	private lb_BirdController controller;

	private bool structureCoolDown;

	private bool treeCoolDown;

	private bool init;

	private float updateTimer;

	private RaycastHit hit;

	public LayerMask layerMask;

	private int collideMask;

	private void Start()
	{
		this.bird = base.transform.parent.GetComponent<lb_Bird>();
		this.structureCoolDown = false;
		this.treeCoolDown = false;
		this.init = true;
		this.collideMask = 0;
	}

	private void OnEnable()
	{
		this.structureCoolDown = false;
		this.treeCoolDown = false;
	}

	private void Update()
	{
		if (Time.time > this.updateTimer)
		{
			this.updateTimer = Time.time + 0.5f;
		}
	}

	private void doCollideCheck()
	{
		if (!this.init)
		{
			return;
		}
		if (!this.bird)
		{
			return;
		}
		Debug.Log(base.gameObject.name + " is doing bird collide check");
		if (Physics.Raycast(base.transform.position, base.transform.forward, out this.hit, 5f, this.collideMask))
		{
			if (this.hit.collider.gameObject.CompareTag("FireTrigger") && this.bird.flying && !this.bird.landing && !this.bird.onGround && !this.structureCoolDown)
			{
				Debug.Log("bird in trigger " + this.hit.collider.gameObject.name);
				this.bird.FleeBehind();
				this.structureCoolDown = true;
				base.Invoke("resetStructureCoolDown", 1.5f);
			}
			if (!this.hit.collider.isTrigger && this.bird.flying && !this.bird.landing && !this.bird.onGround && !this.structureCoolDown)
			{
				Debug.Log("bird in trigger " + this.hit.collider.gameObject.name);
				this.bird.FleeBehind();
				this.structureCoolDown = true;
				base.Invoke("resetStructureCoolDown", 1.5f);
			}
		}
		if ((this.hit.collider.gameObject.CompareTag("Tree") || this.hit.collider.gameObject.layer == 11) && this.bird.flying && !this.bird.landing && !this.bird.onGround && !this.treeCoolDown)
		{
			Debug.Log("bird in trigger " + this.hit.collider.gameObject.name);
			this.bird.FleeDodgeTree();
			this.treeCoolDown = true;
			base.Invoke("resetTreeCoolDown", 0.5f);
		}
	}

	private void resetStructureCoolDown()
	{
		this.structureCoolDown = false;
	}

	private void resetTreeCoolDown()
	{
		this.treeCoolDown = false;
	}
}
