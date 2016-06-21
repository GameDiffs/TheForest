using Serialization;
using System;

public static class Storage
{
	public static string SerializeToString(this object obj)
	{
		return Convert.ToBase64String(UnitySerializer.Serialize(obj));
	}

	public static T Deserialize<T>(this string data) where T : class
	{
		return Storage.Deserialize(data) as T;
	}

	public static object Deserialize(string data)
	{
		return UnitySerializer.Deserialize(Convert.FromBase64String(data));
	}
}
