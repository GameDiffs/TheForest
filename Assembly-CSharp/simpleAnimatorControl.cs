using System;
using UnityEngine;

public class simpleAnimatorControl : MonoBehaviour
{
	private Animator animator;

	private CharacterController controller;

	private Transform thisTr;

	public float gravity;

	private float animGravity;

	private Vector3 moveDir = Vector3.zero;

	private float currYPos;

	public float velY;

	public float accelY;

	public Vector3 wantedDir;

	private bool ikBool;

	private bool initBool;

	private float terrainPosY;

	private void Start()
	{
		this.animator = base.GetComponent<Animator>();
		this.controller = base.transform.parent.GetComponent<CharacterController>();
		this.thisTr = base.transform;
		base.Invoke("initAnimator", 0.5f);
	}

	private void initAnimator()
	{
		this.initBool = true;
	}

	private void OnAnimatorMove()
	{
		this.controllerOn();
	}

	private void controllerOn()
	{
		if (this.initBool)
		{
			this.controller.enabled = true;
			this.animGravity = this.animator.GetFloat("Gravity");
			this.moveDir = this.animator.deltaPosition;
			this.moveDir.y = this.moveDir.y - this.gravity * Time.deltaTime * this.animGravity;
			this.controller.Move(this.moveDir);
			this.thisTr.rotation = this.animator.rootRotation;
		}
	}
}
