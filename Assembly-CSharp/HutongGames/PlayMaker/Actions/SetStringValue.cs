using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.String), Tooltip("Sets the value of a String Variable.")]
	public class SetStringValue : FsmStateAction
	{
		[RequiredField, UIHint(UIHint.Variable)]
		public FsmString stringVariable;

		[RequiredField]
		public FsmString stringValue;

		public bool everyFrame;

		public override void Reset()
		{
			this.stringVariable = null;
			this.stringValue = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoSetStringValue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoSetStringValue();
		}

		private void DoSetStringValue()
		{
			if (this.stringVariable == null)
			{
				return;
			}
			if (this.stringValue == null)
			{
				return;
			}
			this.stringVariable.Value = this.stringValue.Value;
		}
	}
}
