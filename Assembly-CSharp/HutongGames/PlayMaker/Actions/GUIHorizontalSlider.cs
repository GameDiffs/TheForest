using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GUI), HutongGames.PlayMaker.Tooltip("GUI Horizontal Slider connected to a Float Variable.")]
	public class GUIHorizontalSlider : GUIAction
	{
		[RequiredField, UIHint(UIHint.Variable)]
		public FsmFloat floatVariable;

		[RequiredField]
		public FsmFloat leftValue;

		[RequiredField]
		public FsmFloat rightValue;

		public FsmString sliderStyle;

		public FsmString thumbStyle;

		public override void Reset()
		{
			base.Reset();
			this.floatVariable = null;
			this.leftValue = 0f;
			this.rightValue = 100f;
			this.sliderStyle = "horizontalslider";
			this.thumbStyle = "horizontalsliderthumb";
		}

		public override void OnGUI()
		{
			base.OnGUI();
			if (this.floatVariable != null)
			{
				this.floatVariable.Value = GUI.HorizontalSlider(this.rect, this.floatVariable.Value, this.leftValue.Value, this.rightValue.Value, (!(this.sliderStyle.Value != string.Empty)) ? "horizontalslider" : this.sliderStyle.Value, (!(this.thumbStyle.Value != string.Empty)) ? "horizontalsliderthumb" : this.thumbStyle.Value);
			}
		}
	}
}
