using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Color), HutongGames.PlayMaker.Tooltip("Samples a Color on a continuous Colors gradient.")]
	public class ColorRamp : FsmStateAction
	{
		[RequiredField, HutongGames.PlayMaker.Tooltip("Array of colors to defining the gradient.")]
		public FsmColor[] colors;

		[RequiredField, HutongGames.PlayMaker.Tooltip("Point on the gradient to sample. Should be between 0 and the number of colors in the gradient.")]
		public FsmFloat sampleAt;

		[RequiredField, HutongGames.PlayMaker.Tooltip("Store the sampled color in a Color variable."), UIHint(UIHint.Variable)]
		public FsmColor storeColor;

		[HutongGames.PlayMaker.Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;

		public override void Reset()
		{
			this.colors = new FsmColor[3];
			this.sampleAt = 0f;
			this.storeColor = null;
			this.everyFrame = false;
		}

		public override void OnEnter()
		{
			this.DoColorRamp();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		public override void OnUpdate()
		{
			this.DoColorRamp();
		}

		private void DoColorRamp()
		{
			if (this.colors == null)
			{
				return;
			}
			if (this.colors.Length == 0)
			{
				return;
			}
			if (this.sampleAt == null)
			{
				return;
			}
			if (this.storeColor == null)
			{
				return;
			}
			float num = Mathf.Clamp(this.sampleAt.Value, 0f, (float)(this.colors.Length - 1));
			Color value;
			if (num == 0f)
			{
				value = this.colors[0].Value;
			}
			else if (num == (float)this.colors.Length)
			{
				value = this.colors[this.colors.Length - 1].Value;
			}
			else
			{
				Color value2 = this.colors[Mathf.FloorToInt(num)].Value;
				Color value3 = this.colors[Mathf.CeilToInt(num)].Value;
				num -= Mathf.Floor(num);
				value = Color.Lerp(value2, value3, num);
			}
			this.storeColor.Value = value;
		}

		public override string ErrorCheck()
		{
			if (this.colors.Length < 2)
			{
				return "Define at least 2 colors to make a gradient.";
			}
			return null;
		}
	}
}
