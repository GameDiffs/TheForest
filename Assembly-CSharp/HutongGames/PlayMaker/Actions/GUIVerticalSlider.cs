using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUI), HutongGames.PlayMaker.Tooltip("GUI Vertical Slider connected to a Float Variable.")]
	public class GUIVerticalSlider : GUIAction
	{
		[RequiredField, UIHint(UIHint.Variable)]
		public FsmFloat floatVariable;

		[RequiredField]
		public FsmFloat topValue;

		[RequiredField]
		public FsmFloat bottomValue;

		public FsmString sliderStyle;

		public FsmString thumbStyle;

		public override void Reset()
		{
			base.Reset();
			this.floatVariable = null;
			this.topValue = 100f;
			this.bottomValue = 0f;
			this.sliderStyle = "verticalslider";
			this.thumbStyle = "verticalsliderthumb";
			this.width = 0.1f;
		}

		public override void OnGUI()
		{
			base.OnGUI();
			if (this.floatVariable != null)
			{
				this.floatVariable.Value = GUI.VerticalSlider(this.rect, this.floatVariable.Value, this.topValue.Value, this.bottomValue.Value, (!(this.sliderStyle.Value != string.Empty)) ? "verticalslider" : this.sliderStyle.Value, (!(this.thumbStyle.Value != string.Empty)) ? "verticalsliderthumb" : this.thumbStyle.Value);
			}
		}
	}
}
