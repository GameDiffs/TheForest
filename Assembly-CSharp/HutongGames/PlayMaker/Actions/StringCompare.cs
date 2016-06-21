using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic), Tooltip("Compares 2 Strings and sends Events based on the result.")]
	public class StringCompare : FsmStateAction
	{
		[RequiredField, UIHint(UIHint.Variable)]
		public FsmString stringVariable;

		public FsmString compareTo;

		public FsmEvent equalEvent;

		public FsmEvent notEqualEvent;

		[Tooltip("Store the true/false result in a bool variable."), UIHint(UIHint.Variable)]
		public FsmBool storeResult;

		[Tooltip("Repeat every frame. Useful if any of the strings are changing over time.")]
		public bool everyFrame;

		public override void Reset()
		{
			this.stringVariable = null;
			this.compareTo = string.Empty;
			this.equalEvent = null;
			this.notEqualEvent = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoStringCompare();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoStringCompare();
		}

		private void DoStringCompare()
		{
			if (this.stringVariable == null || this.compareTo == null)
			{
				return;
			}
			bool flag = this.stringVariable.Value == this.compareTo.Value;
			if (this.storeResult != null)
			{
				this.storeResult.Value = flag;
			}
			if (flag && this.equalEvent != null)
			{
				base.Fsm.Event(this.equalEvent);
				return;
			}
			if (!flag && this.notEqualEvent != null)
			{
				base.Fsm.Event(this.notEqualEvent);
			}
		}
	}
}
