using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform), HutongGames.PlayMaker.Tooltip("Transforms position from world space to a Game Object's local space. The opposite of TransformPoint.")]
	public class InverseTransformPoint : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[RequiredField]
		public FsmVector3 worldPosition;

		[RequiredField, UIHint(UIHint.Variable)]
		public FsmVector3 storeResult;

		public bool everyFrame;

		public override void Reset()
		{
			this.gameObject = null;
			this.worldPosition = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoInverseTransformPoint();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoInverseTransformPoint();
		}

		private void DoInverseTransformPoint()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this.storeResult.Value = ownerDefaultTarget.transform.InverseTransformPoint(this.worldPosition.Value);
		}
	}
}
