using System;

namespace Serialization
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class SerializerAttribute : Attribute
	{
		internal readonly Type SerializesType;

		public SerializerAttribute(Type serializesType)
		{
			this.SerializesType = serializesType;
		}
	}
}
