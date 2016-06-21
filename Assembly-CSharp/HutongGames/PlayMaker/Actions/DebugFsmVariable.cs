using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Debug), Tooltip("Print the value of an FSM Variable in the PlayMaker Log Window.")]
	public class DebugFsmVariable : FsmStateAction
	{
		[Tooltip("Info, Warning, or Error.")]
		public LogLevel logLevel;

		[HideTypeFilter, Tooltip("Variable to print to the PlayMaker log window."), UIHint(UIHint.Variable)]
		public FsmVar fsmVar;

		public override void Reset()
		{
			this.logLevel = LogLevel.Info;
			this.fsmVar = null;
		}

		public override void OnEnter()
		{
			ActionHelpers.DebugLog(base.Fsm, this.logLevel, this.fsmVar.DebugString());
			base.Finish();
		}
	}
}
