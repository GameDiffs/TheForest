using System;

namespace Inspector.Decorations
{
	public class WarningBoxAttribute : HelpBoxAttribute
	{
		public WarningBoxAttribute(int order, string message) : base(order, message)
		{
		}
	}
}
