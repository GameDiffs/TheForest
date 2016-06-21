using System;

namespace Serialization
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class SerializerId : Attribute
	{
	}
}
