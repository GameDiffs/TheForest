using System;
using UnityEngine;

public class RotateGameObject : MonoBehaviour
{
	public float rot_speed_x;

	public float rot_speed_y;

	public float rot_speed_z;

	public bool local;

	private void Start()
	{
	}

	private void FixedUpdate()
	{
		if (this.local)
		{
			base.transform.Rotate(Time.fixedDeltaTime * new Vector3(this.rot_speed_x, this.rot_speed_y, this.rot_speed_z), Space.Self);
		}
		else
		{
			base.transform.Rotate(Time.fixedDeltaTime * new Vector3(this.rot_speed_x, this.rot_speed_y, this.rot_speed_z), Space.World);
		}
	}
}
