using Serialization;
using System;
using System.IO;
using System.Xml.Serialization;

public static class XmlSupport
{
	public static T DeserializeXml<T>(this string xml) where T : class
	{
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
		T result;
		using (MemoryStream memoryStream = new MemoryStream(UnitySerializer.TextEncoding.GetBytes(xml)))
		{
			result = (T)((object)xmlSerializer.Deserialize(memoryStream));
		}
		return result;
	}

	public static object DeserializeXml(this string xml, Type tp)
	{
		XmlSerializer xmlSerializer = new XmlSerializer(tp);
		object result;
		using (MemoryStream memoryStream = new MemoryStream(UnitySerializer.TextEncoding.GetBytes(xml)))
		{
			result = xmlSerializer.Deserialize(memoryStream);
		}
		return result;
	}

	public static string SerializeXml(this object item)
	{
		XmlSerializer xmlSerializer = new XmlSerializer(item.GetType());
		string @string;
		using (MemoryStream memoryStream = new MemoryStream())
		{
			xmlSerializer.Serialize(memoryStream, item);
			memoryStream.Flush();
			@string = UnitySerializer.TextEncoding.GetString(memoryStream.GetBuffer());
		}
		return @string;
	}
}
