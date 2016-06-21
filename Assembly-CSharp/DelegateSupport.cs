using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using UniLinq;

public static class DelegateSupport
{
	public class Index<TK, TR> : Dictionary<TK, TR> where TR : class, new()
	{
		public new TR this[TK index]
		{
			get
			{
				TR result;
				if (!this.TryGetValue(index, out result))
				{
					result = (base[index] = Activator.CreateInstance<TR>());
				}
				return result;
			}
			set
			{
				base[index] = value;
			}
		}
	}

	private static Dictionary<MethodInfo, Type> delegateTypes = new Dictionary<MethodInfo, Type>();

	private static Dictionary<MethodInfo, Delegate> openDelegates = new Dictionary<MethodInfo, Delegate>();

	private static DelegateSupport.Index<Type, Dictionary<string, Func<object, object[], Delegate, object>>> _functions = new DelegateSupport.Index<Type, Dictionary<string, Func<object, object[], Delegate, object>>>();

	private static Dictionary<MethodInfo, Func<object, object[], object>> _methods = new Dictionary<MethodInfo, Func<object, object[], object>>();

	public static Delegate ToOpenDelegate(MethodInfo mi)
	{
		Delegate result;
		if (!DelegateSupport.openDelegates.TryGetValue(mi, out result))
		{
			Type type;
			if (!DelegateSupport.delegateTypes.TryGetValue(mi, out type))
			{
				List<Type> list = (from p in mi.GetParameters()
				select p.ParameterType).ToList<Type>();
				list.Insert(0, mi.DeclaringType);
				if (mi.ReturnType == typeof(void))
				{
					type = Expression.GetActionType(list.ToArray());
				}
				else
				{
					list.Add(mi.ReturnType);
					type = Expression.GetFuncType(list.ToArray());
				}
				DelegateSupport.delegateTypes[mi] = type;
			}
			result = (DelegateSupport.openDelegates[mi] = Delegate.CreateDelegate(type, mi));
		}
		return result;
	}

	public static Delegate ToDelegate(MethodInfo mi, object target)
	{
		Type type;
		if (!DelegateSupport.delegateTypes.TryGetValue(mi, out type))
		{
			List<Type> list = (from p in mi.GetParameters()
			select p.ParameterType).ToList<Type>();
			if (mi.ReturnType == typeof(void))
			{
				type = Expression.GetActionType(list.ToArray());
			}
			else
			{
				list.Add(mi.ReturnType);
				type = Expression.GetFuncType(list.ToArray());
			}
			DelegateSupport.delegateTypes[mi] = type;
		}
		return Delegate.CreateDelegate(type, target, mi);
	}

	public static void RegisterActionType<TType>()
	{
		DelegateSupport._functions[typeof(TType)][DelegateSupport.GetTypes(new Type[]
		{
			typeof(void)
		})] = delegate(object target, object[] parms, Delegate @delegate)
		{
			((Action<TType>)@delegate)((TType)((object)target));
			return null;
		};
	}

	public static void RegisterActionType<TType, T1>()
	{
		DelegateSupport._functions[typeof(TType)][DelegateSupport.GetTypes(new Type[]
		{
			typeof(T1),
			typeof(void)
		})] = delegate(object target, object[] parms, Delegate @delegate)
		{
			((Action<TType, T1>)@delegate)((TType)((object)target), (T1)((object)parms[0]));
			return null;
		};
	}

	public static void RegisterActionType<TType, T1, T2, T3>()
	{
		DelegateSupport._functions[typeof(TType)][DelegateSupport.GetTypes(new Type[]
		{
			typeof(T1),
			typeof(T2),
			typeof(T3),
			typeof(void)
		})] = delegate(object target, object[] parms, Delegate @delegate)
		{
			((Action<TType, T1, T2, T3>)@delegate)((TType)((object)target), (T1)((object)parms[0]), (T2)((object)parms[1]), (T3)((object)parms[2]));
			return null;
		};
	}

	public static void RegisterActionType<TType, T1, T2>()
	{
		DelegateSupport._functions[typeof(TType)][DelegateSupport.GetTypes(new Type[]
		{
			typeof(T1),
			typeof(T2),
			typeof(void)
		})] = delegate(object target, object[] parms, Delegate @delegate)
		{
			((Action<TType, T1, T2>)@delegate)((TType)((object)target), (T1)((object)parms[0]), (T2)((object)parms[1]));
			return null;
		};
	}

	public static void RegisterFunctionType<TType, TReturns>()
	{
		DelegateSupport._functions[typeof(TType)][DelegateSupport.GetTypes(new Type[]
		{
			typeof(TReturns)
		})] = ((object target, object[] parms, Delegate @delegate) => ((Func<TType, TReturns>)@delegate)((TType)((object)target)));
	}

	public static void RegisterFunctionType<TType, T1, TReturns>()
	{
		DelegateSupport._functions[typeof(TType)][DelegateSupport.GetTypes(new Type[]
		{
			typeof(T1),
			typeof(TReturns)
		})] = ((object target, object[] parms, Delegate @delegate) => ((Func<TType, T1, TReturns>)@delegate)((TType)((object)target), (T1)((object)parms[0])));
	}

	public static void RegisterFunctionType<TType, T1, T2, TReturns>()
	{
		DelegateSupport._functions[typeof(TType)][DelegateSupport.GetTypes(new Type[]
		{
			typeof(T1),
			typeof(T2),
			typeof(TReturns)
		})] = ((object target, object[] parms, Delegate @delegate) => ((Func<TType, T1, T2, TReturns>)@delegate)((TType)((object)target), (T1)((object)parms[0]), (T2)((object)parms[1])));
	}

	public static void RegisterFunctionType<TType, T1, T2, T3, TReturns>()
	{
		DelegateSupport._functions[typeof(TType)][DelegateSupport.GetTypes(new Type[]
		{
			typeof(T1),
			typeof(T2),
			typeof(T3),
			typeof(TReturns)
		})] = ((object target, object[] parms, Delegate @delegate) => ((Func<TType, T1, T2, T3, TReturns>)@delegate)((TType)((object)target), (T1)((object)parms[0]), (T2)((object)parms[1]), (T3)((object)parms[2])));
	}

	public static object FastInvoke(this MethodInfo mi, object target, params object[] parameters)
	{
		Func<object, object[], object> func;
		if (DelegateSupport._methods.TryGetValue(mi, out func))
		{
			return func(target, parameters);
		}
		string types;
		if (!mi.IsStatic && DelegateSupport._functions.ContainsKey(mi.DeclaringType) && DelegateSupport._functions[mi.DeclaringType].ContainsKey(types = DelegateSupport.GetTypes(mi)))
		{
			Delegate @delegate = DelegateSupport.ToOpenDelegate(mi);
			Func<object, object[], Delegate, object> inner = DelegateSupport._functions[mi.DeclaringType][types];
			Func<object, object[], object> func2 = (object t, object[] p) => inner(t, p, @delegate);
			DelegateSupport._methods[mi] = func2;
			Func<object, object[], object> func3 = func2;
			return func3(target, parameters);
		}
		return mi.Invoke(target, parameters);
	}

	private static string GetTypes(params Type[] types)
	{
		return (from t in types
		select t.FullName).Aggregate(string.Empty, (string v, string n) => v += n);
	}

	private static string GetTypes(MethodInfo mi)
	{
		return DelegateSupport.GetTypes((from p in mi.GetParameters()
		select p.ParameterType).Concat(new Type[]
		{
			mi.ReturnType
		}).ToArray<Type>());
	}
}
