using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic), Tooltip("Tests if the value of a string variable has changed. Use this to send an event on change, or store a bool that can be used in other operations.")]
	public class StringChanged : FsmStateAction
	{
		[RequiredField, UIHint(UIHint.Variable)]
		public FsmString stringVariable;

		public FsmEvent changedEvent;

		[UIHint(UIHint.Variable)]
		public FsmBool storeResult;

		private string previousValue;

		public override void Reset()
		{
			this.stringVariable = null;
			this.changedEvent = null;
			this.storeResult = null;
		}

		public override void OnEnter()
		{
			if (this.stringVariable.IsNone)
			{
				base.Finish();
				return;
			}
			this.previousValue = this.stringVariable.Value;
		}

		public override void OnUpdate()
		{
			if (this.stringVariable.Value != this.previousValue)
			{
				this.storeResult.Value = true;
				base.Fsm.Event(this.changedEvent);
			}
		}
	}
}
