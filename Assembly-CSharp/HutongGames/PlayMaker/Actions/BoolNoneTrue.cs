using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Logic), Tooltip("Tests if all the Bool Variables are False.\nSend an event or store the result.")]
	public class BoolNoneTrue : FsmStateAction
	{
		[RequiredField, Tooltip("The Bool variables to check."), UIHint(UIHint.Variable)]
		public FsmBool[] boolVariables;

		[Tooltip("Event to send if none of the Bool variables are True.")]
		public FsmEvent sendEvent;

		[Tooltip("Store the result in a Bool variable."), UIHint(UIHint.Variable)]
		public FsmBool storeResult;

		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;

		public override void Reset()
		{
			this.boolVariables = null;
			this.sendEvent = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoNoneTrue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoNoneTrue();
		}

		private void DoNoneTrue()
		{
			if (this.boolVariables.Length == 0)
			{
				return;
			}
			bool flag = true;
			for (int i = 0; i < this.boolVariables.Length; i++)
			{
				if (this.boolVariables[i].Value)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				base.Fsm.Event(this.sendEvent);
			}
			this.storeResult.Value = flag;
		}
	}
}
