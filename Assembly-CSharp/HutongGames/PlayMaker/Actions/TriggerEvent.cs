using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics), HutongGames.PlayMaker.Tooltip("Detect collisions with objects that have RigidBody components. \nNOTE: The system events, TRIGGER ENTER, TRIGGER STAY, and TRIGGER EXIT are sent when any object collides with the trigger. Use this action to filter collisions by Tag.")]
	public class TriggerEvent : FsmStateAction
	{
		public TriggerType trigger;

		[UIHint(UIHint.Tag)]
		public FsmString collideTag;

		public FsmEvent sendEvent;

		[UIHint(UIHint.Variable)]
		public FsmGameObject storeCollider;

		public override void Reset()
		{
			this.trigger = TriggerType.OnTriggerEnter;
			this.collideTag = "Untagged";
			this.sendEvent = null;
			this.storeCollider = null;
		}

		public override void Awake()
		{
			switch (this.trigger)
			{
			case TriggerType.OnTriggerEnter:
				base.Fsm.HandleTriggerEnter = true;
				break;
			case TriggerType.OnTriggerStay:
				base.Fsm.HandleTriggerStay = true;
				break;
			case TriggerType.OnTriggerExit:
				base.Fsm.HandleTriggerExit = true;
				break;
			}
		}

		private void StoreCollisionInfo(Collider collisionInfo)
		{
			this.storeCollider.Value = collisionInfo.gameObject;
		}

		public override void DoTriggerEnter(Collider other)
		{
			if (this.trigger == TriggerType.OnTriggerEnter && other.gameObject.CompareTag(this.collideTag.Value))
			{
				this.StoreCollisionInfo(other);
				base.Fsm.Event(this.sendEvent);
			}
		}

		public override void DoTriggerStay(Collider other)
		{
			if (this.trigger == TriggerType.OnTriggerStay && other.gameObject.CompareTag(this.collideTag.Value))
			{
				this.StoreCollisionInfo(other);
				base.Fsm.Event(this.sendEvent);
			}
		}

		public override void DoTriggerExit(Collider other)
		{
			if (this.trigger == TriggerType.OnTriggerExit && other.gameObject.CompareTag(this.collideTag.Value))
			{
				this.StoreCollisionInfo(other);
				base.Fsm.Event(this.sendEvent);
			}
		}

		public override string ErrorCheck()
		{
			return ActionHelpers.CheckOwnerPhysicsSetup(base.Owner);
		}
	}
}
