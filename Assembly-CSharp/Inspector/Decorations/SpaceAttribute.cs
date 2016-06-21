using System;

namespace Inspector.Decorations
{
	public class SpaceAttribute : DecorationAttribute
	{
		public readonly int height;

		public SpaceAttribute(int order, int height) : base(order)
		{
			this.height = height;
		}
	}
}
