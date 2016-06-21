using System;
using UnityEngine;

namespace Pathfinding
{
	[RequireComponent(typeof(Seeker))]
	public class MineBotAI : AIPath
	{
		public Animation anim;

		public float sleepVelocity = 0.4f;

		public float animationSpeed = 0.2f;

		public GameObject endOfPathEffect;

		protected Vector3 lastTarget;

		public new void Start()
		{
			this.anim["forward"].layer = 10;
			this.anim.Play("awake");
			this.anim.Play("forward");
			this.anim["awake"].wrapMode = WrapMode.Once;
			this.anim["awake"].speed = 0f;
			this.anim["awake"].normalizedTime = 1f;
			base.Start();
		}

		public override void OnTargetReached()
		{
			if (this.endOfPathEffect != null && Vector3.Distance(this.tr.position, this.lastTarget) > 1f)
			{
				UnityEngine.Object.Instantiate(this.endOfPathEffect, this.tr.position, this.tr.rotation);
				this.lastTarget = this.tr.position;
			}
		}

		public override Vector3 GetFeetPosition()
		{
			return this.tr.position;
		}

		protected new void Update()
		{
			Vector3 direction;
			if (this.canMove)
			{
				Vector3 vector = base.CalculateVelocity(this.GetFeetPosition());
				this.RotateTowards(this.targetDirection);
				vector.y = 0f;
				if (vector.sqrMagnitude <= this.sleepVelocity * this.sleepVelocity)
				{
					vector = Vector3.zero;
				}
				if (this.rvoController != null)
				{
					this.rvoController.Move(vector);
					direction = this.rvoController.velocity;
				}
				else if (this.navController != null)
				{
					direction = Vector3.zero;
				}
				else if (this.controller != null)
				{
					this.controller.SimpleMove(vector);
					direction = this.controller.velocity;
				}
				else
				{
					Debug.LogWarning("No NavmeshController or CharacterController attached to GameObject");
					direction = Vector3.zero;
				}
			}
			else
			{
				direction = Vector3.zero;
			}
			Vector3 vector2 = this.tr.InverseTransformDirection(direction);
			vector2.y = 0f;
			if (direction.sqrMagnitude <= this.sleepVelocity * this.sleepVelocity)
			{
				this.anim.Blend("forward", 0f, 0.2f);
			}
			else
			{
				this.anim.Blend("forward", 1f, 0.2f);
				AnimationState animationState = this.anim["forward"];
				float z = vector2.z;
				animationState.speed = z * this.animationSpeed;
			}
		}
	}
}
