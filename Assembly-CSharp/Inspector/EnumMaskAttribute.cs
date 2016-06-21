using System;

namespace Inspector
{
	public class EnumMaskAttribute : InspectorAttribute
	{
		public EnumMaskAttribute() : base(null)
		{
		}

		public EnumMaskAttribute(string label) : base(label)
		{
		}
	}
}
