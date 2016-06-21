using System;

namespace Serialization
{
	public interface ISpecialist
	{
		object Serialize(object value);

		object Deserialize(object value);
	}
}
