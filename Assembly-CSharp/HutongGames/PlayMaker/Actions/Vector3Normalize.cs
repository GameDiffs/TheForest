using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Vector3), Tooltip("Normalizes a Vector3 Variable.")]
	public class Vector3Normalize : FsmStateAction
	{
		[RequiredField, UIHint(UIHint.Variable)]
		public FsmVector3 vector3Variable;

		public bool everyFrame;

		public override void Reset()
		{
			this.vector3Variable = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.vector3Variable.Value = this.vector3Variable.Value.normalized;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.vector3Variable.Value = this.vector3Variable.Value.normalized;
		}
	}
}
