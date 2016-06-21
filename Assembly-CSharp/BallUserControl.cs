using System;
using UnityEngine;

public class BallUserControl : MonoBehaviour
{
	private Ball ball;

	private Vector3 move;

	private bool jump;

	private void Awake()
	{
		this.ball = base.GetComponent<Ball>();
	}

	private void Update()
	{
		this.move = new Vector3(CrossPlatformInput.GetAxis("Horizontal"), 0f, CrossPlatformInput.GetAxis("Vertical"));
		this.jump = CrossPlatformInput.GetButton("Jump");
	}

	private void FixedUpdate()
	{
		this.ball.Move(this.move, this.jump);
	}
}
