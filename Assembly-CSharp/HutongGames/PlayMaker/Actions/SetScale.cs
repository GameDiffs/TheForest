using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform), HutongGames.PlayMaker.Tooltip("Sets the Scale of a Game Object. To leave any axis unchanged, set variable to 'None'.")]
	public class SetScale : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject to scale.")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("Use stored Vector3 value, and/or set each axis below."), UIHint(UIHint.Variable)]
		public FsmVector3 vector;

		public FsmFloat x;

		public FsmFloat y;

		public FsmFloat z;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
		public bool everyFrame;

		[HutongGames.PlayMaker.Tooltip("Perform in LateUpdate. This is useful if you want to override the position of objects that are animated or otherwise positioned in Update.")]
		public bool lateUpdate;

		public override void Reset()
		{
			this.gameObject = null;
			this.vector = null;
			this.x = new FsmFloat
			{
				UseVariable = true
			};
			this.y = new FsmFloat
			{
				UseVariable = true
			};
			this.z = new FsmFloat
			{
				UseVariable = true
			};
			this.everyFrame = false;
			this.lateUpdate = false;
		}

		public override void OnEnter()
		{
			this.DoSetScale();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			if (!this.lateUpdate)
			{
				this.DoSetScale();
			}
		}

		public override void OnLateUpdate()
		{
			if (this.lateUpdate)
			{
				this.DoSetScale();
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		private void DoSetScale()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			Vector3 localScale = (!this.vector.IsNone) ? this.vector.Value : ownerDefaultTarget.transform.localScale;
			if (!this.x.IsNone)
			{
				localScale.x = this.x.Value;
			}
			if (!this.y.IsNone)
			{
				localScale.y = this.y.Value;
			}
			if (!this.z.IsNone)
			{
				localScale.z = this.z.Value;
			}
			ownerDefaultTarget.transform.localScale = localScale;
		}
	}
}
