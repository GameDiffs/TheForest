using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Character), HutongGames.PlayMaker.Tooltip("Gets the Collision Flags from a Character Controller on a Game Object. Collision flags give you a broad overview of where the character collided with any other object.")]
	public class GetControllerCollisionFlags : FsmStateAction
	{
		[CheckForComponent(typeof(CharacterController)), RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject with a Character Controller component.")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("True if the Character Controller capsule is on the ground"), UIHint(UIHint.Variable)]
		public FsmBool isGrounded;

		[HutongGames.PlayMaker.Tooltip("True if no collisions in last move."), UIHint(UIHint.Variable)]
		public FsmBool none;

		[HutongGames.PlayMaker.Tooltip("True if the Character Controller capsule was hit on the sides."), UIHint(UIHint.Variable)]
		public FsmBool sides;

		[HutongGames.PlayMaker.Tooltip("True if the Character Controller capsule was hit from above."), UIHint(UIHint.Variable)]
		public FsmBool above;

		[HutongGames.PlayMaker.Tooltip("True if the Character Controller capsule was hit from below."), UIHint(UIHint.Variable)]
		public FsmBool below;

		private GameObject previousGo;

		private CharacterController controller;

		public override void Reset()
		{
			this.gameObject = null;
			this.isGrounded = null;
			this.none = null;
			this.sides = null;
			this.above = null;
			this.below = null;
		}

		public override void OnUpdate()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			if (ownerDefaultTarget != this.previousGo)
			{
				this.controller = ownerDefaultTarget.GetComponent<CharacterController>();
				this.previousGo = ownerDefaultTarget;
			}
			if (this.controller != null)
			{
				this.isGrounded.Value = this.controller.isGrounded;
				FsmBool arg_7D_0 = this.none;
				CollisionFlags arg_7B_0 = this.controller.collisionFlags;
				arg_7D_0.Value = false;
				this.sides.Value = ((this.controller.collisionFlags & CollisionFlags.Sides) != CollisionFlags.None);
				this.above.Value = ((this.controller.collisionFlags & CollisionFlags.Above) != CollisionFlags.None);
				this.below.Value = ((this.controller.collisionFlags & CollisionFlags.Below) != CollisionFlags.None);
			}
		}
	}
}
