using System;
using System.Collections.Generic;
using System.Reflection;
using UniLinq;

public class Types
{
	private static Dictionary<Type, List<Type>> _types = new Dictionary<Type, List<Type>>();

	private static Dictionary<Assembly, bool> _requiredAssemblies = new Dictionary<Assembly, bool>();

	public static List<Type> GetTypes(Type attribute)
	{
		if (!Types._types.ContainsKey(attribute))
		{
			Types._types[attribute] = (from tp in AppDomain.CurrentDomain.GetAssemblies().SelectMany((Assembly asm) => asm.GetTypes())
			where tp.IsDefined(attribute, false)
			select tp).ToList<Type>();
		}
		return Types._types[attribute];
	}

	public static void RequireAssembly(Assembly asm)
	{
		Types._requiredAssemblies[asm] = true;
		Types.Refresh();
	}

	public static void Refresh()
	{
		Types._types = new Dictionary<Type, List<Type>>();
	}
}
