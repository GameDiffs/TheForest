using System;

namespace Serialization
{
	public interface ISerializeObject
	{
		object[] Serialize(object target);

		object Deserialize(object[] data, object instance);
	}
}
