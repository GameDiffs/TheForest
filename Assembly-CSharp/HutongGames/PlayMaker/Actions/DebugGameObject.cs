using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Debug), Tooltip("Logs the value of a Game Object Variable in the PlayMaker Log Window.")]
	public class DebugGameObject : FsmStateAction
	{
		[Tooltip("Info, Warning, or Error.")]
		public LogLevel logLevel;

		[Tooltip("Prints the value of a GameObject variable in the PlayMaker log window."), UIHint(UIHint.Variable)]
		public FsmGameObject gameObject;

		public override void Reset()
		{
			this.logLevel = LogLevel.Info;
			this.gameObject = null;
		}

		public override void OnEnter()
		{
			string text = "None";
			if (!this.gameObject.IsNone)
			{
				text = this.gameObject.Name + ": " + this.gameObject;
			}
			ActionHelpers.DebugLog(base.Fsm, this.logLevel, text);
			base.Finish();
		}
	}
}
