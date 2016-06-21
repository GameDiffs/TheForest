using System;
using System.Reflection;
using UniLinq;

public static class TypeHelper
{
	public static T Attribute<T>(this Type tp) where T : Attribute
	{
		return System.Attribute.GetCustomAttribute(tp, typeof(T)) as T;
	}

	public static T Attribute<T>(this object o) where T : Attribute, new()
	{
		if (o is MemberInfo)
		{
			return ((T)((object)(o as MemberInfo).GetCustomAttributes(typeof(T), false).FirstOrDefault<object>())) ?? Activator.CreateInstance<T>();
		}
		if (o is ParameterInfo)
		{
			return ((T)((object)(o as ParameterInfo).GetCustomAttributes(typeof(T), false).FirstOrDefault<object>())) ?? Activator.CreateInstance<T>();
		}
		return o.GetType().Attribute<T>() ?? Activator.CreateInstance<T>();
	}
}
