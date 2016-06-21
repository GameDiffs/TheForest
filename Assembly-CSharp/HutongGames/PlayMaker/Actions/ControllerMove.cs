using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Character), HutongGames.PlayMaker.Tooltip("Moves a Game Object with a Character Controller. See also Controller Simple Move. NOTE: It is recommended that you make only one call to Move or SimpleMove per frame.")]
	public class ControllerMove : FsmStateAction
	{
		[CheckForComponent(typeof(CharacterController)), RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject to move.")]
		public FsmOwnerDefault gameObject;

		[RequiredField, HutongGames.PlayMaker.Tooltip("The movement vector.")]
		public FsmVector3 moveVector;

		[HutongGames.PlayMaker.Tooltip("Move in local or word space.")]
		public Space space;

		[HutongGames.PlayMaker.Tooltip("Movement vector is defined in units per second. Makes movement frame rate independent.")]
		public FsmBool perSecond;

		private GameObject previousGo;

		private CharacterController controller;

		public override void Reset()
		{
			this.gameObject = null;
			this.moveVector = new FsmVector3
			{
				UseVariable = true
			};
			this.space = Space.World;
			this.perSecond = true;
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
				Vector3 vector = (this.space != Space.World) ? ownerDefaultTarget.transform.TransformDirection(this.moveVector.Value) : this.moveVector.Value;
				if (this.perSecond.Value)
				{
					this.controller.Move(vector * Time.deltaTime);
				}
				else
				{
					this.controller.Move(vector);
				}
			}
		}
	}
}
