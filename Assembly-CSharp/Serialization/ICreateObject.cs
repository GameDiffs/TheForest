using System;

namespace Serialization
{
	public interface ICreateObject
	{
		object Create(Type itemType);
	}
}
