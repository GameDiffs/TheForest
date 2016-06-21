using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic), Tooltip("Tests if the value of a Bool Variable has changed. Use this to send an event on change, or store a bool that can be used in other operations.")]
	public class BoolChanged : FsmStateAction
	{
		[RequiredField, Tooltip("The Bool variable to watch for changes."), UIHint(UIHint.Variable)]
		public FsmBool boolVariable;

		[Tooltip("Event to send if the variable changes.")]
		public FsmEvent changedEvent;

		[Tooltip("Set to True if changed."), UIHint(UIHint.Variable)]
		public FsmBool storeResult;

		private bool previousValue;

		public override void Reset()
		{
			this.boolVariable = null;
			this.changedEvent = null;
			this.storeResult = null;
		}

		public override void OnEnter()
		{
			if (this.boolVariable.IsNone)
			{
				base.Finish();
				return;
			}
			this.previousValue = this.boolVariable.Value;
		}

		public override void OnUpdate()
		{
			this.storeResult.Value = false;
			if (this.boolVariable.Value != this.previousValue)
			{
				this.storeResult.Value = true;
				base.Fsm.Event(this.changedEvent);
			}
		}
	}
}
