using System;
using UnityEngine;

[ExecuteInEditMode]
public class RotateClouds : MonoBehaviour
{
	public bool rotate;

	public float rotationY;

	public float Speed = 0.5f;

	public float rotationY_B;

	public float Speed_B = 1f;

	private float time;

	private void Start()
	{
	}

	private void Update()
	{
		if (this.rotate)
		{
			this.time += Time.deltaTime;
		}
		base.GetComponent<Renderer>().sharedMaterial.SetFloat("_RotationA", this.rotationY + this.time * this.Speed);
		base.GetComponent<Renderer>().sharedMaterial.SetFloat("_RotationB", this.rotationY_B + this.time * this.Speed_B);
	}
}
