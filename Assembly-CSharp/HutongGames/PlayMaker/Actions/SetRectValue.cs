using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Rect), Tooltip("Sets the value of a Rect Variable.")]
	public class SetRectValue : FsmStateAction
	{
		[RequiredField, UIHint(UIHint.Variable)]
		public FsmRect rectVariable;

		[RequiredField]
		public FsmRect rectValue;

		public bool everyFrame;

		public override void Reset()
		{
			this.rectVariable = null;
			this.rectValue = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.rectVariable.Value = this.rectValue.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.rectVariable.Value = this.rectValue.Value;
		}
	}
}
