using System;
using UnityEngine;

[Serializable]
public class CameraMove : MonoBehaviour
{
	public override void Update()
	{
		this.transform.Translate(Input.GetAxis("Horizontal"), (float)0, Input.GetAxis("Vertical"));
	}

	public override void Main()
	{
	}
}
