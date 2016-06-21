using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.String), Tooltip("Gets the Right n characters from a String.")]
	public class GetStringRight : FsmStateAction
	{
		[RequiredField, UIHint(UIHint.Variable)]
		public FsmString stringVariable;

		public FsmInt charCount;

		[RequiredField, UIHint(UIHint.Variable)]
		public FsmString storeResult;

		public bool everyFrame;

		public override void Reset()
		{
			this.stringVariable = null;
			this.charCount = 0;
			this.storeResult = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoGetStringRight();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoGetStringRight();
		}

		private void DoGetStringRight()
		{
			if (this.stringVariable == null)
			{
				return;
			}
			if (this.storeResult == null)
			{
				return;
			}
			string value = this.stringVariable.Value;
			this.storeResult.Value = value.Substring(value.Length - this.charCount.Value, this.charCount.Value);
		}
	}
}
