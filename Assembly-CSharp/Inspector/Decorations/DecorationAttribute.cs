using System;

namespace Inspector.Decorations
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public abstract class DecorationAttribute : Attribute
	{
		public readonly int order;

		public string visibleCheck;

		public DecorationAttribute(int order)
		{
			this.order = order;
		}
	}
}
