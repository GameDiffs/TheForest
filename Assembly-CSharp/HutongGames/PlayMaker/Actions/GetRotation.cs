using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform), HutongGames.PlayMaker.Tooltip("Gets the Rotation of a Game Object and stores it in a Vector3 Variable or each Axis in a Float Variable")]
	public class GetRotation : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
		public FsmQuaternion quaternion;

		[Title("Euler Angles"), UIHint(UIHint.Variable)]
		public FsmVector3 vector;

		[UIHint(UIHint.Variable)]
		public FsmFloat xAngle;

		[UIHint(UIHint.Variable)]
		public FsmFloat yAngle;

		[UIHint(UIHint.Variable)]
		public FsmFloat zAngle;

		public Space space;

		public bool everyFrame;

		public override void Reset()
		{
			this.gameObject = null;
			this.quaternion = null;
			this.vector = null;
			this.xAngle = null;
			this.yAngle = null;
			this.zAngle = null;
			this.space = Space.World;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoGetRotation();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoGetRotation();
		}

		private void DoGetRotation()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			if (this.space == Space.World)
			{
				this.quaternion.Value = ownerDefaultTarget.transform.rotation;
				Vector3 eulerAngles = ownerDefaultTarget.transform.eulerAngles;
				this.vector.Value = eulerAngles;
				this.xAngle.Value = eulerAngles.x;
				this.yAngle.Value = eulerAngles.y;
				this.zAngle.Value = eulerAngles.z;
			}
			else
			{
				Vector3 localEulerAngles = ownerDefaultTarget.transform.localEulerAngles;
				this.quaternion.Value = Quaternion.Euler(localEulerAngles);
				this.vector.Value = localEulerAngles;
				this.xAngle.Value = localEulerAngles.x;
				this.yAngle.Value = localEulerAngles.y;
				this.zAngle.Value = localEulerAngles.z;
			}
		}
	}
}
