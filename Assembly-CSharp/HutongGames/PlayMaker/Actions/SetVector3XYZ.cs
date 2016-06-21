using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Vector3), HutongGames.PlayMaker.Tooltip("Sets the XYZ channels of a Vector3 Variable. To leave any channel unchanged, set variable to 'None'.")]
	public class SetVector3XYZ : FsmStateAction
	{
		[RequiredField, UIHint(UIHint.Variable)]
		public FsmVector3 vector3Variable;

		[UIHint(UIHint.Variable)]
		public FsmVector3 vector3Value;

		public FsmFloat x;

		public FsmFloat y;

		public FsmFloat z;

		public bool everyFrame;

		public override void Reset()
		{
			this.vector3Variable = null;
			this.vector3Value = null;
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
			this.DoSetVector3XYZ();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoSetVector3XYZ();
		}

		private void DoSetVector3XYZ()
		{
			if (this.vector3Variable == null)
			{
				return;
			}
			Vector3 value = this.vector3Variable.Value;
			if (!this.vector3Value.IsNone)
			{
				value = this.vector3Value.Value;
			}
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
			this.vector3Variable.Value = value;
		}
	}
}
