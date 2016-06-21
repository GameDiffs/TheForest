using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Transform), HutongGames.PlayMaker.Tooltip("Gets the Position of a Game Object and stores it in a Vector3 Variable or each Axis in a Float Variable")]
	public class GetPosition : FsmStateAction
	{
		[RequiredField]
		public FsmOwnerDefault gameObject;

		[UIHint(UIHint.Variable)]
		public FsmVector3 vector;

		[UIHint(UIHint.Variable)]
		public FsmFloat x;

		[UIHint(UIHint.Variable)]
		public FsmFloat y;

		[UIHint(UIHint.Variable)]
		public FsmFloat z;

		public Space space;

		public bool everyFrame;

		public override void Reset()
		{
			this.gameObject = null;
			this.vector = null;
			this.x = null;
			this.y = null;
			this.z = null;
			this.space = Space.World;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoGetPosition();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoGetPosition();
		}

		private void DoGetPosition()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			Vector3 value = (this.space != Space.World) ? ownerDefaultTarget.transform.localPosition : ownerDefaultTarget.transform.position;
			this.vector.Value = value;
			this.x.Value = value.x;
			this.y.Value = value.y;
			this.z.Value = value.z;
		}
	}
}
