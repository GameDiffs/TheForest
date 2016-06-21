using System;

namespace Inspector.Decorations
{
	public class ErrorBoxAttribute : HelpBoxAttribute
	{
		public ErrorBoxAttribute(int order, string message) : base(order, message)
		{
		}
	}
}
