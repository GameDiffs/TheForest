using System;
using UnityEngine;

public class CopyCam : MonoBehaviour
{
	private void Update()
	{
		base.GetComponent<Camera>().CopyFrom(Camera.main);
	}
}
