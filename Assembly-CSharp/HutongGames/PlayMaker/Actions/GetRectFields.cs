using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Rect), Tooltip("Get the individual fields of a Rect Variable and store them in Float Variables.")]
	public class GetRectFields : FsmStateAction
	{
		[RequiredField, UIHint(UIHint.Variable)]
		public FsmRect rectVariable;

		[UIHint(UIHint.Variable)]
		public FsmFloat storeX;

		[UIHint(UIHint.Variable)]
		public FsmFloat storeY;

		[UIHint(UIHint.Variable)]
		public FsmFloat storeWidth;

		[UIHint(UIHint.Variable)]
		public FsmFloat storeHeight;

		public bool everyFrame;

		public override void Reset()
		{
			this.rectVariable = null;
			this.storeX = null;
			this.storeY = null;
			this.storeWidth = null;
			this.storeHeight = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoGetRectFields();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoGetRectFields();
		}

		private void DoGetRectFields()
		{
			if (this.rectVariable.IsNone)
			{
				return;
			}
			this.storeX.Value = this.rectVariable.Value.x;
			this.storeY.Value = this.rectVariable.Value.y;
			this.storeWidth.Value = this.rectVariable.Value.width;
			this.storeHeight.Value = this.rectVariable.Value.height;
		}
	}
}
