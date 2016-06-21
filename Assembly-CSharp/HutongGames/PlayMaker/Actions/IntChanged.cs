using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic), Tooltip("Tests if the value of an integer variable changed. Use this to send an event on change, or store a bool that can be used in other operations.")]
	public class IntChanged : FsmStateAction
	{
		[RequiredField, UIHint(UIHint.Variable)]
		public FsmInt intVariable;

		public FsmEvent changedEvent;

		[UIHint(UIHint.Variable)]
		public FsmBool storeResult;

		private int previousValue;

		public override void Reset()
		{
			this.intVariable = null;
			this.changedEvent = null;
			this.storeResult = null;
		}

		public override void OnEnter()
		{
			if (this.intVariable.IsNone)
			{
				base.Finish();
				return;
			}
			this.previousValue = this.intVariable.Value;
		}

		public override void OnUpdate()
		{
			this.storeResult.Value = false;
			if (this.intVariable.Value != this.previousValue)
			{
				this.previousValue = this.intVariable.Value;
				this.storeResult.Value = true;
				base.Fsm.Event(this.changedEvent);
			}
		}
	}
}
