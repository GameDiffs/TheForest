using Serialization;
using System;
using UnityEngine;

[ComponentSerializerFor(typeof(Rigidbody))]
public class SerializeRigidBody : IComponentSerializer
{
	public class RigidBodyInfo
	{
		public bool isKinematic;

		public bool useGravity;

		public bool freezeRotation;

		public bool detectCollisions;

		public bool useConeFriction;

		public Vector3 velocity;

		public Vector3 position;

		public Vector3 angularVelocity;

		public Vector3 centerOfMass;

		public Vector3 inertiaTensor;

		public Quaternion rotation;

		public Quaternion inertiaTensorRotation;

		public float drag;

		public float angularDrag;

		public float mass;

		public float sleepVelocity;

		public float sleepAngularVelocity;

		public float maxAngularVelocity;

		public RigidbodyConstraints constraints;

		public CollisionDetectionMode collisionDetectionMode;

		public RigidbodyInterpolation interpolation;

		public int solverIterationCount;

		public RigidBodyInfo()
		{
		}

		public RigidBodyInfo(Rigidbody source)
		{
			this.isKinematic = source.isKinematic;
			this.useGravity = source.useGravity;
			this.freezeRotation = source.freezeRotation;
			this.detectCollisions = source.detectCollisions;
			this.useConeFriction = source.useConeFriction;
			this.velocity = source.velocity;
			this.position = source.position;
			this.rotation = source.rotation;
			this.angularVelocity = source.angularVelocity;
			this.centerOfMass = source.centerOfMass;
			this.inertiaTensor = source.inertiaTensor;
			this.inertiaTensorRotation = source.inertiaTensorRotation;
			this.drag = source.drag;
			this.angularDrag = source.angularDrag;
			this.mass = source.mass;
			this.sleepVelocity = source.sleepVelocity;
			this.sleepAngularVelocity = source.sleepAngularVelocity;
			this.maxAngularVelocity = source.maxAngularVelocity;
			this.constraints = source.constraints;
			this.collisionDetectionMode = source.collisionDetectionMode;
			this.interpolation = source.interpolation;
			this.solverIterationCount = source.solverIterationCount;
		}

		public void Configure(Rigidbody body)
		{
			body.isKinematic = true;
			body.freezeRotation = this.freezeRotation;
			body.useGravity = this.useGravity;
			body.detectCollisions = this.detectCollisions;
			body.useConeFriction = this.useConeFriction;
			if (this.centerOfMass != Vector3.zero)
			{
				body.centerOfMass = this.centerOfMass;
			}
			body.drag = this.drag;
			body.angularDrag = this.angularDrag;
			body.mass = this.mass;
			body.rotation = this.rotation;
			body.sleepVelocity = this.sleepVelocity;
			body.sleepAngularVelocity = this.sleepAngularVelocity;
			body.maxAngularVelocity = this.maxAngularVelocity;
			body.constraints = this.constraints;
			body.collisionDetectionMode = this.collisionDetectionMode;
			body.interpolation = this.interpolation;
			body.solverIterationCount = this.solverIterationCount;
			body.isKinematic = this.isKinematic;
			if (!this.isKinematic)
			{
				body.velocity = this.velocity;
				body.useGravity = this.useGravity;
				body.angularVelocity = this.angularVelocity;
				if (this.inertiaTensor != Vector3.zero)
				{
					body.inertiaTensor = this.inertiaTensor;
				}
				if (this.inertiaTensorRotation != SerializeRigidBody.zero)
				{
					body.inertiaTensorRotation = this.inertiaTensorRotation;
				}
			}
		}
	}

	private static Quaternion zero = new Quaternion(0f, 0f, 0f, 0f);

	public byte[] Serialize(Component component)
	{
		return UnitySerializer.Serialize(new SerializeRigidBody.RigidBodyInfo((Rigidbody)component));
	}

	public void Deserialize(byte[] data, Component instance)
	{
		SerializeRigidBody.RigidBodyInfo info = UnitySerializer.Deserialize<SerializeRigidBody.RigidBodyInfo>(data);
		info.Configure((Rigidbody)instance);
		UnitySerializer.AddFinalAction(delegate
		{
			info.Configure((Rigidbody)instance);
		});
	}
}
