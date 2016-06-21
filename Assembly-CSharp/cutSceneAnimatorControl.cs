using System;
using UnityEngine;

public class cutSceneAnimatorControl : MonoBehaviour
{
	private Animator animator;

	private CharacterController controller;

	private Transform thisTr;

	private float colHeight;

	private float colSize;

	public float gravity;

	private float animGravity;

	private float currYPos;

	private float lastYPos;

	public float velY;

	public float accelY;

	public Vector3 wantedDir;

	private bool ikBool;

	private bool initBool;

	private float terrainPosY;

	private void Start()
	{
		this.animator = base.GetComponent<Animator>();
		this.thisTr = base.transform;
		base.Invoke("initAnimator", 0.1f);
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
			Vector3 deltaPosition = this.animator.deltaPosition;
			this.thisTr.Translate(deltaPosition, Space.World);
			this.thisTr.rotation = this.animator.rootRotation;
		}
	}
}
