using System;
using UnityEngine;

[Serializable]
public class CharacterMotorJumping
{
	public bool enabled;

	public float baseHeight;

	public float extraHeight;

	public float perpAmount;

	public float steepPerpAmount;

	[NonSerialized]
	public bool jumping;

	[NonSerialized]
	public bool holdingJumpButton;

	[NonSerialized]
	public float lastStartTime;

	[NonSerialized]
	public float lastButtonDownTime;

	[NonSerialized]
	public Vector3 jumpDir;

	public CharacterMotorJumping()
	{
		this.enabled = true;
		this.baseHeight = 1f;
		this.extraHeight = 4.1f;
		this.steepPerpAmount = 0.5f;
		this.lastButtonDownTime = (float)-100;
		this.jumpDir = Vector3.up;
	}
}
