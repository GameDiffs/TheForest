using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Debug), Tooltip("Adds a text area to the action list. NOTE: Doesn't do anything, just for notes...")]
	public class Comment : FsmStateAction
	{
		[UIHint(UIHint.Comment)]
		public string comment;

		public override void Reset()
		{
			this.comment = string.Empty;
		}

		public override void OnEnter()
		{
			base.Finish();
		}
	}
}
