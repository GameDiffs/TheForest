using System;

namespace Serialization
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class OnlyInterfaces : Attribute
	{
	}
}
