using System;

namespace Inspector
{
	public class ToggleAttribute : InspectorAttribute
	{
		public bool flipped;

		public ToggleAttribute() : base(null)
		{
		}

		public ToggleAttribute(string label) : base(label)
		{
		}
	}
}
