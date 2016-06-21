using System;

namespace Serialization
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class CreatorFor : Attribute
	{
		public readonly Type CreatesType;

		public CreatorFor(Type createsType)
		{
			this.CreatesType = createsType;
		}
	}
}
