using System;

namespace Serialization
{
	public interface ISerializeObjectEx : ISerializeObject
	{
		bool CanSerialize(Type targetType, object instance);
	}
}
