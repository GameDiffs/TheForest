using System;

namespace Serialization
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class AttributeListProvider : Attribute
	{
		public readonly Type AttributeListType;

		public AttributeListProvider(Type attributeListType)
		{
			this.AttributeListType = attributeListType;
		}
	}
}
