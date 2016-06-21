using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Rect), HutongGames.PlayMaker.Tooltip("Sets the individual fields of a Rect Variable. To leave any field unchanged, set variable to 'None'.")]
	public class SetRectFields : FsmStateAction
	{
		[RequiredField, UIHint(UIHint.Variable)]
		public FsmRect rectVariable;

		public FsmFloat x;

		public FsmFloat y;

		public FsmFloat width;

		public FsmFloat height;

		public bool everyFrame;

		public override void Reset()
		{
			this.rectVariable = null;
			this.x = new FsmFloat
			{
				UseVariable = true
			};
			this.y = new FsmFloat
			{
				UseVariable = true
			};
			this.width = new FsmFloat
			{
				UseVariable = true
			};
			this.height = new FsmFloat
			{
				UseVariable = true
			};
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoSetRectFields();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoSetRectFields();
		}

		private void DoSetRectFields()
		{
			if (this.rectVariable.IsNone)
			{
				return;
			}
			Rect value = this.rectVariable.Value;
			if (!this.x.IsNone)
			{
				value.x = this.x.Value;
			}
			if (!this.y.IsNone)
			{
				value.y = this.y.Value;
			}
			if (!this.width.IsNone)
			{
				value.width = this.width.Value;
			}
			if (!this.height.IsNone)
			{
				value.height = this.height.Value;
			}
			this.rectVariable.Value = value;
		}
	}
}
