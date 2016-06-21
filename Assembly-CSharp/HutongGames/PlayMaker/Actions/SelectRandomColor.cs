using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Color), Tooltip("Select a random Color from an array of Colors.")]
	public class SelectRandomColor : FsmStateAction
	{
		[CompoundArray("Colors", "Color", "Weight")]
		public FsmColor[] colors;

		[HasFloatSlider(0f, 1f)]
		public FsmFloat[] weights;

		[RequiredField, UIHint(UIHint.Variable)]
		public FsmColor storeColor;

		public override void Reset()
		{
			this.colors = new FsmColor[3];
			this.weights = new FsmFloat[]
			{
				1f,
				1f,
				1f
			};
			this.storeColor = null;
		}

		public override void OnEnter()
		{
			this.DoSelectRandomColor();
			base.Finish();
		}

		private void DoSelectRandomColor()
		{
			if (this.colors == null)
			{
				return;
			}
			if (this.colors.Length == 0)
			{
				return;
			}
			if (this.storeColor == null)
			{
				return;
			}
			int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(this.weights);
			if (randomWeightedIndex != -1)
			{
				this.storeColor.Value = this.colors[randomWeightedIndex].Value;
			}
		}
	}
}
