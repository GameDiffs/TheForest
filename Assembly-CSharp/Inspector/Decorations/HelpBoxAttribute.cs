using System;

namespace Inspector.Decorations
{
	public abstract class HelpBoxAttribute : DecorationAttribute
	{
		public readonly string message;

		public float width = 420f;

		public HelpBoxAttribute(int order, string message) : base(order)
		{
			this.message = message;
		}
	}
}
