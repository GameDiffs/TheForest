using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Debug), Tooltip("Sends a log message to the PlayMaker Log Window.")]
	public class DebugLog : FsmStateAction
	{
		[Tooltip("Info, Warning, or Error.")]
		public LogLevel logLevel;

		[Tooltip("Text to print to the PlayMaker log window.")]
		public FsmString text;

		public override void Reset()
		{
			this.logLevel = LogLevel.Info;
			this.text = string.Empty;
		}

		public override void OnEnter()
		{
			if (!string.IsNullOrEmpty(this.text.Value))
			{
				ActionHelpers.DebugLog(base.Fsm, this.logLevel, this.text.Value);
			}
			base.Finish();
		}
	}
}
