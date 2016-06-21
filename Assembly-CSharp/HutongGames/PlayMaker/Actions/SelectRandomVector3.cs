using System;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Vector3), Tooltip("Select a Random Vector3 from a Vector3 array.")]
	public class SelectRandomVector3 : FsmStateAction
	{
		[CompoundArray("Vectors", "Vector", "Weight")]
		public FsmVector3[] vector3Array;

		[HasFloatSlider(0f, 1f)]
		public FsmFloat[] weights;

		[RequiredField, UIHint(UIHint.Variable)]
		public FsmVector3 storeVector3;

		public override void Reset()
		{
			this.vector3Array = new FsmVector3[3];
			this.weights = new FsmFloat[]
			{
				1f,
				1f,
				1f
			};
			this.storeVector3 = null;
		}

		public override void OnEnter()
		{
			this.DoSelectRandomColor();
			base.Finish();
		}

		private void DoSelectRandomColor()
		{
			if (this.vector3Array == null)
			{
				return;
			}
			if (this.vector3Array.Length == 0)
			{
				return;
			}
			if (this.storeVector3 == null)
			{
				return;
			}
			int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(this.weights);
			if (randomWeightedIndex != -1)
			{
				this.storeVector3.Value = this.vector3Array[randomWeightedIndex].Value;
			}
		}
	}
}
