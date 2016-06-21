using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform), HutongGames.PlayMaker.Tooltip("Gets the Angle between a GameObject's forward axis and a Target. The Target can be defined as a GameObject or a world Position. If you specify both, then the Position will be used as a local offset from the Target Object's position.")]
	public class GetAngleToTarget : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("The game object whose forward axis we measure from. If the target is dead ahead the angle will be 0.")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("The target object to measure the angle to. Or use target position.")]
		public FsmGameObject targetObject;

		[HutongGames.PlayMaker.Tooltip("The world position to measure an angle to. If Target Object is also specified, this vector is used as an offset from that object's position.")]
		public FsmVector3 targetPosition;

		[HutongGames.PlayMaker.Tooltip("Ignore height differences when calculating the angle.")]
		public FsmBool ignoreHeight;

		[RequiredField, HutongGames.PlayMaker.Tooltip("Store the angle in a float variable."), UIHint(UIHint.Variable)]
		public FsmFloat storeAngle;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			this.gameObject = null;
			this.targetObject = null;
			this.targetPosition = new FsmVector3
			{
				UseVariable = true
			};
			this.ignoreHeight = true;
			this.storeAngle = null;
			this.everyFrame = false;
		}

		public override void OnLateUpdate()
		{
			this.DoGetAngleToTarget();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		private void DoGetAngleToTarget()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			GameObject value = this.targetObject.Value;
			if (value == null && this.targetPosition.IsNone)
			{
				return;
			}
			Vector3 a;
			if (value != null)
			{
				a = (this.targetPosition.IsNone ? value.transform.position : value.transform.TransformPoint(this.targetPosition.Value));
			}
			else
			{
				a = this.targetPosition.Value;
			}
			if (this.ignoreHeight.Value)
			{
				a.y = ownerDefaultTarget.transform.position.y;
			}
			Vector3 from = a - ownerDefaultTarget.transform.position;
			this.storeAngle.Value = Vector3.Angle(from, ownerDefaultTarget.transform.forward);
		}
	}
}
