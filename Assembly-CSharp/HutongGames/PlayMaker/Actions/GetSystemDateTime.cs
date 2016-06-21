using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Time), Tooltip("Gets system date and time info and stores it in a string variable. An optional format string gives you a lot of control over the formatting (see online docs for format syntax).")]
	public class GetSystemDateTime : FsmStateAction
	{
		[Tooltip("Store System DateTime as a string."), UIHint(UIHint.Variable)]
		public FsmString storeString;

		[Tooltip("Optional format string. E.g., MM/dd/yyyy HH:mm")]
		public FsmString format;

		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			this.storeString = null;
			this.format = "MM/dd/yyyy HH:mm";
		}

		public override void OnEnter()
		{
			this.storeString.Value = DateTime.Now.ToString(this.format.Value);
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.storeString.Value = DateTime.Now.ToString(this.format.Value);
		}
	}
}
