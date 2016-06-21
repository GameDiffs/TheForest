using System;
using UnityEngine;

[Serializable]
public class FireDamagePoint
{
	public Vector3 Position = Vector3.zero;

	public float Radius = 1f;

	public bool isBurning;

	public FireParticle particleSystem;
}
