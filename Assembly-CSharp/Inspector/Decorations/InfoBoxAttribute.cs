using System;

namespace Inspector.Decorations
{
	public class InfoBoxAttribute : HelpBoxAttribute
	{
		public InfoBoxAttribute(int order, string message) : base(order, message)
		{
		}
	}
}
