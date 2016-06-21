using System;
using UnityEngine;

public class SunshineSampleMove : MonoBehaviour
{
	public bool Move = true;

	public Vector3 MoveVector = Vector3.up;

	public float MoveRange = 4f;

	public float MoveSpeed = 1f;

	public bool Spin;

	public float SpinSpeed = 20f;

	private Vector3 startPosition;

	private void Start()
	{
		this.startPosition = base.transform.position;
	}

	private void Update()
	{
		if (this.Move)
		{
			base.transform.position = this.startPosition + this.MoveVector * (this.MoveRange * Mathf.Sin(Time.timeSinceLevelLoad * this.MoveSpeed));
		}
		if (this.Spin)
		{
			base.transform.eulerAngles = new Vector3(0f, Time.timeSinceLevelLoad * this.SpinSpeed, 0f);
		}
	}
}
