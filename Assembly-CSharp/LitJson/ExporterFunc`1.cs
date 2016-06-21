using System;

namespace LitJson
{
	public delegate void ExporterFunc<T>(T obj, JsonWriter writer);
}
