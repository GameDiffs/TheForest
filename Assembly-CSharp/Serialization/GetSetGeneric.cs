using System;
using System.Reflection;

namespace Serialization
{
	public class GetSetGeneric : GetSet
	{
		public GetSetGeneric(PropertyInfo info)
		{
			this.Name = info.Name;
			this.Info = info;
			this.CollectionType = (this.Info.PropertyType.GetInterface("IEnumerable", true) != null);
			object[] customAttributes = info.GetCustomAttributes(typeof(Specialist), true);
			if (customAttributes.Length > 0)
			{
				Specialist specialist = (Specialist)customAttributes[0];
				this.Get = ((object o) => UnitySerializer.Specialists[specialist.Type].Serialize(info.GetValue(o, null)));
				this.Set = delegate(object o, object v)
				{
					info.SetValue(o, UnitySerializer.Specialists[specialist.Type].Deserialize(v), null);
				};
			}
			else
			{
				MethodInfo getMethod = info.GetGetMethod(true);
				MethodInfo setMethod = info.GetSetMethod(true);
				if (getMethod == null)
				{
					this.Get = ((object o) => info.GetValue(o, null));
					this.Set = delegate(object o, object v)
					{
						info.SetValue(o, v, null);
					};
					return;
				}
				this.IsStatic = getMethod.IsStatic;
				this.Get = ((object o) => getMethod.FastInvoke(o, null));
				this.Set = delegate(object o, object v)
				{
					try
					{
						setMethod.FastInvoke(o, new object[]
						{
							v
						});
					}
					catch (Exception ex)
					{
						Radical.LogWarning(string.Format("When setting {0} to {1} found {2}:", (o == null) ? "null" : o.ToString(), (v == null) ? "null" : v.ToString(), ex.ToString()));
					}
				};
			}
		}

		public GetSetGeneric(FieldInfo info)
		{
			this.Name = info.Name;
			this.FieldInfo = info;
			object[] customAttributes = info.GetCustomAttributes(typeof(Specialist), true);
			if (customAttributes.Length > 0)
			{
				Specialist specialist = (Specialist)customAttributes[0];
				this.Get = ((object o) => UnitySerializer.Specialists[specialist.Type].Serialize(info.GetValue(o)));
				this.Set = delegate(object o, object v)
				{
					info.SetValue(o, UnitySerializer.Specialists[specialist.Type].Deserialize(v));
				};
			}
			else
			{
				this.Get = new Func<object, object>(info.GetValue);
				this.Set = new Action<object, object>(info.SetValue);
			}
			this.IsStatic = info.IsStatic;
			this.CollectionType = (this.FieldInfo.FieldType.GetInterface("IEnumerable", true) != null);
		}

		public GetSetGeneric(Type t, string name)
		{
			GetSetGeneric <>f__this = this;
			this.Name = name;
			PropertyInfo property = t.GetProperty(name);
			if (property == null)
			{
				this.FieldInfo = t.GetField(this.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				this.Get = new Func<object, object>(this.FieldInfo.GetValue);
				this.Set = new Action<object, object>(this.FieldInfo.SetValue);
				this.IsStatic = this.FieldInfo.IsStatic;
				this.CollectionType = (this.FieldInfo.FieldType.GetInterface("IEnumerable", true) != null);
				return;
			}
			this.Info = property;
			this.CollectionType = (this.Info.PropertyType.GetInterface("IEnumerable", true) != null);
			MethodInfo getMethod = property.GetGetMethod(true);
			MethodInfo setMethod = property.GetSetMethod(true);
			this.IsStatic = getMethod.IsStatic;
			this.Get = ((object o) => getMethod.Invoke(<>f__this.IsStatic ? null : o, null));
			this.Set = delegate(object o, object v)
			{
				setMethod.Invoke(<>f__this.IsStatic ? null : o, new object[]
				{
					v
				});
			};
		}
	}
}
