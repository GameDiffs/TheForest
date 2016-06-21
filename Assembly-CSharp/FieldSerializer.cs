using System;
using System.Collections.Generic;
using System.Reflection;

public static class FieldSerializer
{
	public static void SerializeFields(Dictionary<string, object> storage, object obj, params string[] names)
	{
		Type type = obj.GetType();
		for (int i = 0; i < names.Length; i++)
		{
			string text = names[i];
			FieldInfo field = type.GetField(text, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetField);
			if (field != null)
			{
				storage[text] = field.GetValue(obj);
			}
		}
	}

	public static void DeserializeFields(Dictionary<string, object> storage, object obj)
	{
		Type type = obj.GetType();
		foreach (KeyValuePair<string, object> current in storage)
		{
			FieldInfo field = type.GetField(current.Key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetField);
			if (field != null)
			{
				field.SetValue(obj, current.Value);
			}
		}
	}
}
