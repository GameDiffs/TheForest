using System;
using System.Collections.Generic;
using System.Reflection;
using UniLinq;

namespace Serialization
{
	public class SerializePrivateFieldOfType
	{
		private static readonly Index<string, List<SerializePrivateFieldOfType>> privateFields = new Index<string, List<SerializePrivateFieldOfType>>();

		private readonly string _fieldName;

		public SerializePrivateFieldOfType(string typeName, string fieldName)
		{
			this._fieldName = fieldName;
			SerializePrivateFieldOfType.privateFields[typeName].Add(this);
		}

		public static IEnumerable<FieldInfo> GetFields(Type type)
		{
			string text = string.Empty;
			if (SerializePrivateFieldOfType.privateFields.ContainsKey(type.Name))
			{
				text = type.Name;
			}
			if (SerializePrivateFieldOfType.privateFields.ContainsKey(type.FullName))
			{
				text = type.FullName;
			}
			if (!string.IsNullOrEmpty(text))
			{
				List<SerializePrivateFieldOfType> fields = SerializePrivateFieldOfType.privateFields[text];
				return from f in type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.SetField)
				where fields.Any((SerializePrivateFieldOfType fld) => fld._fieldName == f.Name)
				select f;
			}
			return new FieldInfo[0];
		}
	}
}
