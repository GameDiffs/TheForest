using System;

namespace Inspector
{
	public class MinMaxSliderAttribute : InspectorAttribute
	{
		public float minValue;

		public float maxValue;

		public bool showFields = true;

		public MinMaxSliderAttribute() : base(null)
		{
		}

		public MinMaxSliderAttribute(string label) : base(label)
		{
		}

		public MinMaxSliderAttribute(string label, float minValue, float maxValue) : base(label)
		{
			this.minValue = minValue;
			this.maxValue = maxValue;
		}
	}
}
