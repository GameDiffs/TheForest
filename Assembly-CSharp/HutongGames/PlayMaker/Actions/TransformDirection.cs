using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform), HutongGames.PlayMaker.Tooltip("Transforms a Direction from a Game Object's local space to world space.")]
	public class TransformDirection : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		public FsmVector3 localDirection;

		[RequiredField, UIHint(UIHint.Variable)]
		public FsmVector3 storeResult;

		public bool everyFrame;

		public override void Reset()
		{
			this.gameObject = null;
			this.localDirection = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoTransformDirection();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoTransformDirection();
		}

		private void DoTransformDirection()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this.storeResult.Value = ownerDefaultTarget.transform.TransformDirection(this.localDirection.Value);
		}
	}
}
