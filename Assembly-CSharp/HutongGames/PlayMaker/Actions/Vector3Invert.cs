using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Vector3), Tooltip("Reverses the direction of a Vector3 Variable. Same as multiplying by -1.")]
	public class Vector3Invert : FsmStateAction
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
			this.vector3Variable.Value = this.vector3Variable.Value * -1f;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.vector3Variable.Value = this.vector3Variable.Value * -1f;
		}
	}
}
