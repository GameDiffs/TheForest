using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics), HutongGames.PlayMaker.Tooltip("Detect collisions between the Owner of this FSM and other Game Objects that have RigidBody components.\nNOTE: The system events, COLLISION ENTER, COLLISION STAY, and COLLISION EXIT are sent automatically on collisions with any object. Use this action to filter collisions by Tag.")]
	public class CollisionEvent : FsmStateAction
	{
		[HutongGames.PlayMaker.Tooltip("The type of collision to detect.")]
		public CollisionType collision;

		[HutongGames.PlayMaker.Tooltip("Filter by Tag."), UIHint(UIHint.Tag)]
		public FsmString collideTag;

		[HutongGames.PlayMaker.Tooltip("Event to send if a collision is detected.")]
		public FsmEvent sendEvent;

		[HutongGames.PlayMaker.Tooltip("Store the GameObject that collided with the Owner of this FSM."), UIHint(UIHint.Variable)]
		public FsmGameObject storeCollider;

		[HutongGames.PlayMaker.Tooltip("Store the force of the collision. NOTE: Use Get Collision Info to get more info about the collision."), UIHint(UIHint.Variable)]
		public FsmFloat storeForce;

		public override void Reset()
		{
			this.collision = CollisionType.OnCollisionEnter;
			this.collideTag = "Untagged";
			this.sendEvent = null;
			this.storeCollider = null;
			this.storeForce = null;
		}

		public override void Awake()
		{
			switch (this.collision)
			{
			case CollisionType.OnCollisionEnter:
				base.Fsm.HandleCollisionEnter = true;
				break;
			case CollisionType.OnCollisionStay:
				base.Fsm.HandleCollisionStay = true;
				break;
			case CollisionType.OnCollisionExit:
				base.Fsm.HandleCollisionExit = true;
				break;
			}
		}

		private void StoreCollisionInfo(Collision collisionInfo)
		{
			this.storeCollider.Value = collisionInfo.gameObject;
			this.storeForce.Value = collisionInfo.relativeVelocity.magnitude;
		}

		public override void DoCollisionEnter(Collision collisionInfo)
		{
			if (this.collision == CollisionType.OnCollisionEnter && collisionInfo.collider.gameObject.CompareTag(this.collideTag.Value))
			{
				this.StoreCollisionInfo(collisionInfo);
				base.Fsm.Event(this.sendEvent);
			}
		}

		public override void DoCollisionStay(Collision collisionInfo)
		{
			if (this.collision == CollisionType.OnCollisionStay && collisionInfo.collider.gameObject.CompareTag(this.collideTag.Value))
			{
				this.StoreCollisionInfo(collisionInfo);
				base.Fsm.Event(this.sendEvent);
			}
		}

		public override void DoCollisionExit(Collision collisionInfo)
		{
			if (this.collision == CollisionType.OnCollisionExit && collisionInfo.collider.gameObject.CompareTag(this.collideTag.Value))
			{
				this.StoreCollisionInfo(collisionInfo);
				base.Fsm.Event(this.sendEvent);
			}
		}

		public override void DoControllerColliderHit(ControllerColliderHit collisionInfo)
		{
			if (this.collision == CollisionType.OnControllerColliderHit && collisionInfo.collider.gameObject.CompareTag(this.collideTag.Value))
			{
				if (this.storeCollider != null)
				{
					this.storeCollider.Value = collisionInfo.gameObject;
				}
				this.storeForce.Value = 0f;
				base.Fsm.Event(this.sendEvent);
			}
		}

		public override string ErrorCheck()
		{
			return ActionHelpers.CheckOwnerPhysicsSetup(base.Owner);
		}
	}
}
