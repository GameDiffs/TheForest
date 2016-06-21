using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Character), Tooltip("Gets info on the last Character Controller collision and store in variables.")]
	public class GetControllerHitInfo : FsmStateAction
	{
		[UIHint(UIHint.Variable)]
		public FsmGameObject gameObjectHit;

		[UIHint(UIHint.Variable)]
		public FsmVector3 contactPoint;

		[UIHint(UIHint.Variable)]
		public FsmVector3 contactNormal;

		[UIHint(UIHint.Variable)]
		public FsmVector3 moveDirection;

		[UIHint(UIHint.Variable)]
		public FsmFloat moveLength;

		[Tooltip("Useful for triggering different effects. Audio, particles..."), UIHint(UIHint.Variable)]
		public FsmString physicsMaterialName;

		public override void Reset()
		{
			this.gameObjectHit = null;
			this.contactPoint = null;
			this.contactNormal = null;
			this.moveDirection = null;
			this.moveLength = null;
			this.physicsMaterialName = null;
		}

		private void StoreTriggerInfo()
		{
			if (base.Fsm.ControllerCollider == null)
			{
				return;
			}
			this.gameObjectHit.Value = base.Fsm.ControllerCollider.gameObject;
			this.contactPoint.Value = base.Fsm.ControllerCollider.point;
			this.contactNormal.Value = base.Fsm.ControllerCollider.normal;
			this.moveDirection.Value = base.Fsm.ControllerCollider.moveDirection;
			this.moveLength.Value = base.Fsm.ControllerCollider.moveLength;
			this.physicsMaterialName.Value = base.Fsm.ControllerCollider.collider.material.name;
		}

		public override void OnEnter()
		{
			this.StoreTriggerInfo();
			base.Finish();
		}

		public override string ErrorCheck()
		{
			return ActionHelpers.CheckOwnerPhysicsSetup(base.Owner);
		}
	}
}
