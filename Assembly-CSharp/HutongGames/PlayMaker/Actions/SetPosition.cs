using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform), HutongGames.PlayMaker.Tooltip("Sets the Position of a Game Object. To leave any axis unchanged, set variable to 'None'.")]
	public class SetPosition : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("The GameObject to position.")]
		public FsmOwnerDefault gameObject;

		[HutongGames.PlayMaker.Tooltip("Use a stored Vector3 position, and/or set individual axis below."), UIHint(UIHint.Variable)]
		public FsmVector3 vector;

		public FsmFloat x;

		public FsmFloat y;

		public FsmFloat z;

		[HutongGames.PlayMaker.Tooltip("Use local or world space.")]
		public Space space;

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
			this.space = Space.Self;
			this.everyFrame = false;
			this.lateUpdate = false;
		}

		public override void OnEnter()
		{
			if (!this.everyFrame && !this.lateUpdate)
			{
				this.DoSetPosition();
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			if (!this.lateUpdate)
			{
				this.DoSetPosition();
			}
		}

		public override void OnLateUpdate()
		{
			if (this.lateUpdate)
			{
				this.DoSetPosition();
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		private void DoSetPosition()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			Vector3 vector;
			if (this.vector.IsNone)
			{
				vector = ((this.space != Space.World) ? ownerDefaultTarget.transform.localPosition : ownerDefaultTarget.transform.position);
			}
			else
			{
				vector = this.vector.Value;
			}
			if (!this.x.IsNone)
			{
				vector.x = this.x.Value;
			}
			if (!this.y.IsNone)
			{
				vector.y = this.y.Value;
			}
			if (!this.z.IsNone)
			{
				vector.z = this.z.Value;
			}
			if (this.space == Space.World)
			{
				ownerDefaultTarget.transform.position = vector;
			}
			else
			{
				ownerDefaultTarget.transform.localPosition = vector;
			}
		}
	}
}
