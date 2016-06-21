using System;
using UniLinq;

namespace AssemblyCSharp
{
	public static class TypeExtensions
	{
		public static bool ImplementsInterface(this Type type, Type interfaceType)
		{
			return type.GetInterfaces().Contains(interfaceType);
		}
	}
}
