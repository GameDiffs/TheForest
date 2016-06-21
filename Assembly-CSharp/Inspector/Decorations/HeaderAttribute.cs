using System;

namespace Inspector.Decorations
{
	public class HeaderAttribute : DecorationAttribute
	{
		public readonly string header;

		public HeaderAttribute(int order, string header) : base(order)
		{
			this.header = header;
		}
	}
}
