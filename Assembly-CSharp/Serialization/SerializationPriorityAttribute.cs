using System;

namespace Serialization
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class SerializationPriorityAttribute : Attribute
	{
		public readonly int Priority;

		public SerializationPriorityAttribute(int priority)
		{
			this.Priority = priority;
		}
	}
}
