using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics), HutongGames.PlayMaker.Tooltip("Sets the gravity vector, or individual axis.")]
	public class SetGravity : FsmStateAction
	{
		public FsmVector3 vector;

		public FsmFloat x;

		public FsmFloat y;

		public FsmFloat z;

		public bool everyFrame;

		public override void Reset()
		{
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
		}

		public override void OnEnter()
		{
			this.DoSetGravity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoSetGravity();
		}

		private void DoSetGravity()
		{
			Vector3 value = this.vector.Value;
			if (!this.x.IsNone)
			{
				value.x = this.x.Value;
			}
			if (!this.y.IsNone)
			{
				value.y = this.y.Value;
			}
			if (!this.z.IsNone)
			{
				value.z = this.z.Value;
			}
			Physics.gravity = value;
		}
	}
}
