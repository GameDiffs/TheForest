using System;

namespace Serialization
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class Specialist : Attribute
	{
		public readonly Type Type;

		public Specialist(Type type)
		{
			this.Type = type;
		}
	}
}
