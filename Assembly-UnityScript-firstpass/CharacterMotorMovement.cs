using System;
using UnityEngine;

[Serializable]
public class CharacterMotorMovement
{
	public float maxForwardSpeed;

	public float maxSidewaysSpeed;

	public float maxBackwardsSpeed;

	public AnimationCurve slopeSpeedMultiplier;

	public float maxGroundAcceleration;

	public float maxAirAcceleration;

	public float gravity;

	public float maxFallSpeed;

	[NonSerialized]
	public CollisionFlags collisionFlags;

	[NonSerialized]
	public Vector3 velocity;

	[NonSerialized]
	public Vector3 frameVelocity;

	[NonSerialized]
	public Vector3 hitPoint;

	[NonSerialized]
	public Vector3 lastHitPoint;

	public CharacterMotorMovement()
	{
		this.maxForwardSpeed = 10f;
		this.maxSidewaysSpeed = 10f;
		this.maxBackwardsSpeed = 10f;
		this.slopeSpeedMultiplier = new AnimationCurve(new Keyframe[]
		{
			new Keyframe((float)-90, (float)1),
			new Keyframe((float)0, (float)1),
			new Keyframe((float)90, (float)0)
		});
		this.maxGroundAcceleration = 30f;
		this.maxAirAcceleration = 20f;
		this.gravity = 10f;
		this.maxFallSpeed = 20f;
		this.frameVelocity = Vector3.zero;
		this.hitPoint = Vector3.zero;
		this.lastHitPoint = new Vector3(float.PositiveInfinity, (float)0, (float)0);
	}
}
