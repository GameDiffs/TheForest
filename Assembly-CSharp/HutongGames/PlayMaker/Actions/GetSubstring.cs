using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.String), Tooltip("Gets a sub-string from a String Variable.")]
	public class GetSubstring : FsmStateAction
	{
		[RequiredField, UIHint(UIHint.Variable)]
		public FsmString stringVariable;

		[RequiredField]
		public FsmInt startIndex;

		[RequiredField]
		public FsmInt length;

		[RequiredField, UIHint(UIHint.Variable)]
		public FsmString storeResult;

		public bool everyFrame;

		public override void Reset()
		{
			this.stringVariable = null;
			this.startIndex = 0;
			this.length = 1;
			this.storeResult = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoGetSubstring();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoGetSubstring();
		}

		private void DoGetSubstring()
		{
			if (this.stringVariable == null)
			{
				return;
			}
			if (this.storeResult == null)
			{
				return;
			}
			this.storeResult.Value = this.stringVariable.Value.Substring(this.startIndex.Value, this.length.Value);
		}
	}
}
