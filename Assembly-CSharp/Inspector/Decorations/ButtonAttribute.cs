using System;

namespace Inspector.Decorations
{
	public class ButtonAttribute : DecorationAttribute
	{
		public readonly string label;

		public readonly string callback;

		public float width = 200f;

		public float height = 28f;

		public string tooltip;

		public ButtonAttribute(int order, string label, string callback) : base(order)
		{
			this.label = label;
			this.callback = callback;
		}
	}
}
