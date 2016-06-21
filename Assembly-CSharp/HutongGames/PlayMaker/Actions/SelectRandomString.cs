using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.String), Tooltip("Select a Random String from an array of Strings.")]
	public class SelectRandomString : FsmStateAction
	{
		[CompoundArray("Strings", "String", "Weight")]
		public FsmString[] strings;

		[HasFloatSlider(0f, 1f)]
		public FsmFloat[] weights;

		[RequiredField, UIHint(UIHint.Variable)]
		public FsmString storeString;

		public override void Reset()
		{
			this.strings = new FsmString[3];
			this.weights = new FsmFloat[]
			{
				1f,
				1f,
				1f
			};
			this.storeString = null;
		}

		public override void OnEnter()
		{
			this.DoSelectRandomString();
			base.Finish();
		}

		private void DoSelectRandomString()
		{
			if (this.strings == null)
			{
				return;
			}
			if (this.strings.Length == 0)
			{
				return;
			}
			if (this.storeString == null)
			{
				return;
			}
			int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(this.weights);
			if (randomWeightedIndex != -1)
			{
				this.storeString.Value = this.strings[randomWeightedIndex].Value;
			}
		}
	}
}
