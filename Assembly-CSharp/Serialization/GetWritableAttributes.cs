using System;
using System.Collections.Generic;
using System.Reflection;
using UniLinq;

namespace Serialization
{
	public class GetWritableAttributes
	{
		private static readonly Dictionary<Type, GetSet[][][]> PropertyAccess = new Dictionary<Type, GetSet[][][]>();

		private static readonly Dictionary<Type, object> Vanilla = new Dictionary<Type, object>();

		public static Entry[] GetProperties(object obj, bool seen)
		{
			Type type = obj.GetType();
			GetSet[][] accessors = GetWritableAttributes.GetAccessors(type);
			return (from a in accessors[(!seen) ? 2 : 0]
			select new
			{
				a = a,
				value = a.Get(obj)
			} into <>__TranspIdent1
			select new Entry
			{
				PropertyInfo = <>__TranspIdent1.a.Info,
				MustHaveName = true,
				Value = <>__TranspIdent1.value,
				IsStatic = <>__TranspIdent1.a.IsStatic
			}).ToArray<Entry>();
		}

		public static Entry[] GetFields(object obj, bool seen)
		{
			Type type = obj.GetType();
			GetSet[][] accessors = GetWritableAttributes.GetAccessors(type);
			return (from a in accessors[(!seen) ? 3 : 1]
			select new
			{
				a = a,
				value = a.Get(obj)
			} into <>__TranspIdent2
			select new Entry
			{
				FieldInfo = <>__TranspIdent2.a.FieldInfo,
				MustHaveName = true,
				Value = <>__TranspIdent2.value,
				IsStatic = <>__TranspIdent2.a.IsStatic
			}).ToArray<Entry>();
		}

		private static object GetVanilla(Type type)
		{
			object result;
			try
			{
				object obj = null;
				Dictionary<Type, object> vanilla = GetWritableAttributes.Vanilla;
				lock (vanilla)
				{
					if (!GetWritableAttributes.Vanilla.TryGetValue(type, out obj))
					{
						obj = UnitySerializer.CreateObject(type);
						GetWritableAttributes.Vanilla[type] = obj;
					}
				}
				result = obj;
			}
			catch
			{
				result = null;
			}
			return result;
		}

		private static GetSet[][] GetAccessors(Type type)
		{
			Dictionary<Type, GetSet[][][]> propertyAccess = GetWritableAttributes.PropertyAccess;
			GetSet[][] result;
			lock (propertyAccess)
			{
				int num = ((!UnitySerializer.IsChecksum) ? 0 : 1) + ((!UnitySerializer.IsChecksum || !UnitySerializer.IgnoreIds) ? 0 : 1);
				GetSet[][][] array;
				if (!GetWritableAttributes.PropertyAccess.TryGetValue(type, out array))
				{
					array = new GetSet[3][][];
					GetWritableAttributes.PropertyAccess[type] = array;
				}
				GetSet[][] array2 = array[num];
				if (array2 == null)
				{
					object vanilla = GetWritableAttributes.GetVanilla(type);
					bool flag = false;
					if (vanilla != null)
					{
						flag = !vanilla.Equals(null);
					}
					List<GetSet> list = new List<GetSet>();
					IEnumerable<PropertyInfo> enumerable = from p in UnitySerializer.GetPropertyInfo(type)
					where p.Name != "hideFlags"
					select new
					{
						priority = ((SerializationPriorityAttribute)p.GetCustomAttributes(false).FirstOrDefault((object a) => a is SerializationPriorityAttribute)) ?? new SerializationPriorityAttribute(100),
						info = p
					} into p
					orderby p.priority.Priority
					select p.info;
					foreach (PropertyInfo current in enumerable)
					{
						GetSetGeneric getSetGeneric = new GetSetGeneric(current);
						if (!flag)
						{
							getSetGeneric.Vanilla = null;
						}
						else
						{
							getSetGeneric.Vanilla = getSetGeneric.Get(vanilla);
						}
						list.Add(getSetGeneric);
					}
					array2 = new GetSet[4][];
					array2[0] = (from a in list
					where !a.IsStatic
					select a).ToArray<GetSet>();
					array2[2] = list.ToArray();
					list.Clear();
					IEnumerable<FieldInfo> enumerable2 = from f in UnitySerializer.GetFieldInfo(type)
					where f.Name != "hideFlags"
					select f into p
					select new
					{
						priority = ((SerializationPriorityAttribute)p.GetCustomAttributes(false).FirstOrDefault((object a) => a is SerializationPriorityAttribute)) ?? new SerializationPriorityAttribute(100),
						info = p
					} into p
					orderby p.priority.Priority
					select p.info;
					foreach (FieldInfo current2 in enumerable2)
					{
						GetSetGeneric getSetGeneric2 = new GetSetGeneric(current2);
						if (!flag)
						{
							getSetGeneric2.Vanilla = null;
						}
						else
						{
							getSetGeneric2.Vanilla = getSetGeneric2.Get(vanilla);
						}
						list.Add(getSetGeneric2);
					}
					array2[1] = (from a in list
					where !a.IsStatic
					select a).ToArray<GetSet>();
					array2[3] = list.ToArray();
					array[num] = array2;
				}
				result = array2;
			}
			return result;
		}
	}
}
