using System;
using UnityEngine;

public class touchBendingPlayerListener : MonoBehaviour
{
	public float maxSpeed = 8f;

	public float Player_DampSpeed = 0.75f;

	private Transform myTransform;

	private Vector3 Player_Position;

	private Vector3 Player_OldPosition;

	public float Player_Speed;

	private float Player_NewSpeed;

	public Vector3 Player_Direction;

	private void Awake()
	{
		this.myTransform = base.transform;
	}

	private void Start()
	{
		this.Player_Position = base.transform.position;
		this.Player_OldPosition = this.Player_Position;
	}

	private void LateUpdate()
	{
		if (Time.deltaTime <= 1.401298E-45f)
		{
			this.Player_Speed = 0f;
			return;
		}
		this.Player_Position = this.myTransform.position;
		this.Player_NewSpeed = (this.Player_Position - this.Player_OldPosition).magnitude / Time.deltaTime / this.maxSpeed;
		float num = 1f - Mathf.Exp(-20f * Time.deltaTime);
		float num2 = 0.25f * num;
		num *= 0.125f;
		if (this.Player_NewSpeed < this.Player_Speed)
		{
			this.Player_Speed = Mathf.Lerp(this.Player_Speed, this.Player_NewSpeed, num * this.Player_DampSpeed);
		}
		else
		{
			this.Player_Speed = Mathf.Lerp(this.Player_Speed, this.Player_NewSpeed, num2 * this.Player_DampSpeed);
		}
		if (this.Player_Position != this.Player_OldPosition)
		{
			this.Player_Direction = Vector3.Normalize(this.Player_Position - this.Player_OldPosition);
		}
		this.Player_OldPosition = this.Player_Position;
	}
}
