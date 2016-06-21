using System;
using UnityEngine;

public class MouseLayerAlloc : MonoBehaviour
{
	private void Start()
	{
		base.GetComponent<Camera>().eventMask = ~base.GetComponent<Camera>().cullingMask;
	}
}
