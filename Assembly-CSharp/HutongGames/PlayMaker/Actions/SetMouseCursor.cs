using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUI), Tooltip("Controls the appearance of Mouse Cursor.")]
	public class SetMouseCursor : FsmStateAction
	{
		public FsmTexture cursorTexture;

		public FsmBool hideCursor;

		public FsmBool lockCursor;

		public override void Reset()
		{
			this.cursorTexture = null;
			this.hideCursor = false;
			this.lockCursor = false;
		}

		public override void OnEnter()
		{
			PlayMakerGUI.LockCursor = this.lockCursor.Value;
			PlayMakerGUI.HideCursor = this.hideCursor.Value;
			PlayMakerGUI.MouseCursor = this.cursorTexture.Value;
			base.Finish();
		}
	}
}
