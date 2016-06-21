using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Character), HutongGames.PlayMaker.Tooltip("Moves a Game Object with a Character Controller. Velocity along the y-axis is ignored. Speed is in meters/s. Gravity is automatically applied.")]
	public class ControllerSimpleMove : FsmStateAction
	{
		[CheckForComponent(typeof(CharacterController)), RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject to move.")]
		public FsmOwnerDefault gameObject;

		[RequiredField, HutongGames.PlayMaker.Tooltip("The movement vector.")]
		public FsmVector3 moveVector;

		[HutongGames.PlayMaker.Tooltip("Multiply the movement vector by a speed factor.")]
		public FsmFloat speed;

		[HutongGames.PlayMaker.Tooltip("Move in local or word space.")]
		public Space space;

		private GameObject previousGo;

		private CharacterController controller;

		public override void Reset()
		{
			this.gameObject = null;
			this.moveVector = new FsmVector3
			{
				UseVariable = true
			};
			this.speed = 1f;
			this.space = Space.World;
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
				Vector3 a = (this.space != Space.World) ? ownerDefaultTarget.transform.TransformDirection(this.moveVector.Value) : this.moveVector.Value;
				this.controller.SimpleMove(a * this.speed.Value);
			}
		}
	}
}
