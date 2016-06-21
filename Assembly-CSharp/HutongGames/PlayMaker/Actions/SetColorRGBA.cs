using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Color), HutongGames.PlayMaker.Tooltip("Sets the RGBA channels of a Color Variable. To leave any channel unchanged, set variable to 'None'.")]
	public class SetColorRGBA : FsmStateAction
	{
		[RequiredField, UIHint(UIHint.Variable)]
		public FsmColor colorVariable;

		[HasFloatSlider(0f, 1f)]
		public FsmFloat red;

		[HasFloatSlider(0f, 1f)]
		public FsmFloat green;

		[HasFloatSlider(0f, 1f)]
		public FsmFloat blue;

		[HasFloatSlider(0f, 1f)]
		public FsmFloat alpha;

		public bool everyFrame;

		public override void Reset()
		{
			this.colorVariable = null;
			this.red = 0f;
			this.green = 0f;
			this.blue = 0f;
			this.alpha = 1f;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoSetColorRGBA();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoSetColorRGBA();
		}

		private void DoSetColorRGBA()
		{
			if (this.colorVariable == null)
			{
				return;
			}
			Color value = this.colorVariable.Value;
			if (!this.red.IsNone)
			{
				value.r = this.red.Value;
			}
			if (!this.green.IsNone)
			{
				value.g = this.green.Value;
			}
			if (!this.blue.IsNone)
			{
				value.b = this.blue.Value;
			}
			if (!this.alpha.IsNone)
			{
				value.a = this.alpha.Value;
			}
			this.colorVariable.Value = value;
		}
	}
}
