using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform), HutongGames.PlayMaker.Tooltip("Sets the Rotation of a Game Object. To leave any axis unchanged, set variable to 'None'.")]
	public class SetRotation : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject to rotate.")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("Use a stored quaternion, or vector angles below."), UIHint(UIHint.Variable)]
		public FsmQuaternion quaternion;

		[Title("Euler Angles"), HutongGames.PlayMaker.Tooltip("Use euler angles stored in a Vector3 variable, and/or set each axis below."), UIHint(UIHint.Variable)]
		public FsmVector3 vector;

		public FsmFloat xAngle;

		public FsmFloat yAngle;

		public FsmFloat zAngle;

		[HutongGames.PlayMaker.Tooltip("Use local or world space.")]
		public Space space;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
		public bool everyFrame;

		[HutongGames.PlayMaker.Tooltip("Perform in LateUpdate. This is useful if you want to override the position of objects that are animated or otherwise positioned in Update.")]
		public bool lateUpdate;

		public override void Reset()
		{
			this.gameObject = null;
			this.quaternion = null;
			this.vector = null;
			this.xAngle = new FsmFloat
			{
				UseVariable = true
			};
			this.yAngle = new FsmFloat
			{
				UseVariable = true
			};
			this.zAngle = new FsmFloat
			{
				UseVariable = true
			};
			this.space = Space.World;
			this.everyFrame = false;
			this.lateUpdate = false;
		}

		public override void OnEnter()
		{
			if (!this.everyFrame && !this.lateUpdate)
			{
				this.DoSetRotation();
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			if (!this.lateUpdate)
			{
				this.DoSetRotation();
			}
		}

		public override void OnLateUpdate()
		{
			if (this.lateUpdate)
			{
				this.DoSetRotation();
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		private void DoSetRotation()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			Vector3 vector;
			if (!this.quaternion.IsNone)
			{
				vector = this.quaternion.Value.eulerAngles;
			}
			else if (!this.vector.IsNone)
			{
				vector = this.vector.Value;
			}
			else
			{
				vector = ((this.space != Space.Self) ? ownerDefaultTarget.transform.eulerAngles : ownerDefaultTarget.transform.localEulerAngles);
			}
			if (!this.xAngle.IsNone)
			{
				vector.x = this.xAngle.Value;
			}
			if (!this.yAngle.IsNone)
			{
				vector.y = this.yAngle.Value;
			}
			if (!this.zAngle.IsNone)
			{
				vector.z = this.zAngle.Value;
			}
			if (this.space == Space.Self)
			{
				ownerDefaultTarget.transform.localEulerAngles = vector;
			}
			else
			{
				ownerDefaultTarget.transform.eulerAngles = vector;
			}
		}
	}
}
