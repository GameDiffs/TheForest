using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic), Tooltip("Tests if all the given Bool Variables are are equal to thier Bool States.")]
	public class BoolTestMulti : FsmStateAction
	{
		[RequiredField, Tooltip("This must be the same number used for Bool States."), UIHint(UIHint.Variable)]
		public FsmBool[] boolVariables;

		[RequiredField, Tooltip("This must be the same number used for Bool Variables.")]
		public FsmBool[] boolStates;

		public FsmEvent trueEvent;

		public FsmEvent falseEvent;

		[UIHint(UIHint.Variable)]
		public FsmBool storeResult;

		public bool everyFrame;

		public override void Reset()
		{
			this.boolVariables = null;
			this.boolStates = null;
			this.trueEvent = null;
			this.falseEvent = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoAllTrue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoAllTrue();
		}

		private void DoAllTrue()
		{
			if (this.boolVariables.Length == 0 || this.boolStates.Length == 0)
			{
				return;
			}
			if (this.boolVariables.Length != this.boolStates.Length)
			{
				return;
			}
			bool flag = true;
			for (int i = 0; i < this.boolVariables.Length; i++)
			{
				if (this.boolVariables[i].Value != this.boolStates[i].Value)
				{
					flag = false;
					break;
				}
			}
			this.storeResult.Value = flag;
			if (flag)
			{
				base.Fsm.Event(this.trueEvent);
			}
			else
			{
				base.Fsm.Event(this.falseEvent);
			}
		}
	}
}
