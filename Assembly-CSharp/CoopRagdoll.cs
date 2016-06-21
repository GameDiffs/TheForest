using System;
using UnityEngine;

public class CoopRagdoll : CoopBase<IRagdollState>
{
	[SerializeField]
	private Rigidbody rigidbody;

	public override void Attached()
	{
	}
}
