using System;

namespace Serialization
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class SubTypeSerializerAttribute : Attribute
	{
		internal readonly Type SerializesType;

		public SubTypeSerializerAttribute(Type serializesType)
		{
			this.SerializesType = serializesType;
		}
	}
}
