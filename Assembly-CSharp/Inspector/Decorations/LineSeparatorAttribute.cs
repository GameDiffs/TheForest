using System;

namespace Inspector.Decorations
{
	public class LineSeparatorAttribute : DecorationAttribute
	{
		public float padding = 60f;

		public LineSeparatorAttribute(int order) : base(order)
		{
		}
	}
}
