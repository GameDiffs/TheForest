using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Debug), Tooltip("Logs the value of an Integer Variable in the PlayMaker Log Window.")]
	public class DebugInt : FsmStateAction
	{
		[Tooltip("Info, Warning, or Error.")]
		public LogLevel logLevel;

		[Tooltip("Prints the value of an Int variable in the PlayMaker log window."), UIHint(UIHint.Variable)]
		public FsmInt intVariable;

		public override void Reset()
		{
			this.logLevel = LogLevel.Info;
			this.intVariable = null;
		}

		public override void OnEnter()
		{
			string text = "None";
			if (!this.intVariable.IsNone)
			{
				text = this.intVariable.Name + ": " + this.intVariable.Value;
			}
			ActionHelpers.DebugLog(base.Fsm, this.logLevel, text);
			base.Finish();
		}
	}
}
