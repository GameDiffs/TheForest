using System;
using UnityEngine;

public class CentreAndPointMe : MonoBehaviour
{
	public Transform ball1;

	public Transform ball2;

	private void Start()
	{
	}

	private void Update()
	{
		Vector3 vector = this.ball1.position - this.ball2.position;
		base.transform.position = this.ball2.position + 0.5f * vector;
		Vector3 up = Vector3.up;
		base.transform.rotation = Quaternion.LookRotation(Quaternion.AngleAxis(90f, up) * vector, up);
	}
}
