using System;
using UnityEngine;

public struct PointerEventArgs
{
	public uint controllerIndex;

	public uint flags;

	public float distance;

	public Transform target;
}
