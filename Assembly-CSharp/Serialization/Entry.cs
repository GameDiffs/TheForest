using System;
using System.Reflection;

namespace Serialization
{
	public class Entry
	{
		public string Name;

		private PropertyInfo _propertyInfo;

		private FieldInfo _fieldInfo;

		public Type StoredType;

		public object Value;

		public bool IsStatic;

		public bool MustHaveName;

		public Type OwningType;

		public GetSet Setter;

		public PropertyInfo PropertyInfo
		{
			get
			{
				return this._propertyInfo;
			}
			set
			{
				this.Name = value.Name;
				this.StoredType = value.PropertyType;
				this._propertyInfo = value;
			}
		}

		public FieldInfo FieldInfo
		{
			get
			{
				return this._fieldInfo;
			}
			set
			{
				this.Name = value.Name;
				this.StoredType = value.FieldType;
				this._fieldInfo = value;
			}
		}
	}
}
