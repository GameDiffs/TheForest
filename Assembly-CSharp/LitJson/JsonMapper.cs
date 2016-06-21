using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace LitJson
{
	public class JsonMapper
	{
		private static int max_nesting_depth;

		private static IFormatProvider datetime_format;

		private static IDictionary<Type, ExporterFunc> base_exporters_table;

		private static IDictionary<Type, ExporterFunc> custom_exporters_table;

		private static IDictionary<Type, IDictionary<Type, ImporterFunc>> base_importers_table;

		private static IDictionary<Type, IDictionary<Type, ImporterFunc>> custom_importers_table;

		private static IDictionary<Type, ArrayMetadata> array_metadata;

		private static readonly object array_metadata_lock;

		private static IDictionary<Type, IDictionary<Type, MethodInfo>> conv_ops;

		private static readonly object conv_ops_lock;

		private static IDictionary<Type, ObjectMetadata> object_metadata;

		private static readonly object object_metadata_lock;

		private static IDictionary<Type, IList<PropertyMetadata>> type_properties;

		private static readonly object type_properties_lock;

		private static JsonWriter static_writer;

		private static readonly object static_writer_lock;

		static JsonMapper()
		{
			JsonMapper.array_metadata_lock = new object();
			JsonMapper.conv_ops_lock = new object();
			JsonMapper.object_metadata_lock = new object();
			JsonMapper.type_properties_lock = new object();
			JsonMapper.static_writer_lock = new object();
			JsonMapper.max_nesting_depth = 100;
			JsonMapper.array_metadata = new Dictionary<Type, ArrayMetadata>();
			JsonMapper.conv_ops = new Dictionary<Type, IDictionary<Type, MethodInfo>>();
			JsonMapper.object_metadata = new Dictionary<Type, ObjectMetadata>();
			JsonMapper.type_properties = new Dictionary<Type, IList<PropertyMetadata>>();
			JsonMapper.static_writer = new JsonWriter();
			JsonMapper.datetime_format = DateTimeFormatInfo.InvariantInfo;
			JsonMapper.base_exporters_table = new Dictionary<Type, ExporterFunc>();
			JsonMapper.custom_exporters_table = new Dictionary<Type, ExporterFunc>();
			JsonMapper.base_importers_table = new Dictionary<Type, IDictionary<Type, ImporterFunc>>();
			JsonMapper.custom_importers_table = new Dictionary<Type, IDictionary<Type, ImporterFunc>>();
			JsonMapper.RegisterBaseExporters();
			JsonMapper.RegisterBaseImporters();
		}

		private static void AddArrayMetadata(Type type)
		{
			if (JsonMapper.array_metadata.ContainsKey(type))
			{
				return;
			}
			ArrayMetadata value = default(ArrayMetadata);
			value.IsArray = type.IsArray;
			if (type.GetInterface("System.Collections.IList") != null)
			{
				value.IsList = true;
			}
			PropertyInfo[] properties = type.GetProperties();
			for (int i = 0; i < properties.Length; i++)
			{
				PropertyInfo propertyInfo = properties[i];
				if (!(propertyInfo.Name != "Item"))
				{
					ParameterInfo[] indexParameters = propertyInfo.GetIndexParameters();
					if (indexParameters.Length == 1)
					{
						if (indexParameters[0].ParameterType == typeof(int))
						{
							value.ElementType = propertyInfo.PropertyType;
						}
					}
				}
			}
			object obj = JsonMapper.array_metadata_lock;
			lock (obj)
			{
				try
				{
					JsonMapper.array_metadata.Add(type, value);
				}
				catch (ArgumentException)
				{
				}
			}
		}

		private static void AddObjectMetadata(Type type)
		{
			if (JsonMapper.object_metadata.ContainsKey(type))
			{
				return;
			}
			ObjectMetadata value = default(ObjectMetadata);
			if (type.GetInterface("System.Collections.IDictionary") != null)
			{
				value.IsDictionary = true;
			}
			value.Properties = new Dictionary<string, PropertyMetadata>();
			PropertyInfo[] properties = type.GetProperties();
			for (int i = 0; i < properties.Length; i++)
			{
				PropertyInfo propertyInfo = properties[i];
				if (propertyInfo.Name == "Item")
				{
					ParameterInfo[] indexParameters = propertyInfo.GetIndexParameters();
					if (indexParameters.Length == 1)
					{
						if (indexParameters[0].ParameterType == typeof(string))
						{
							value.ElementType = propertyInfo.PropertyType;
						}
					}
				}
				else
				{
					PropertyMetadata value2 = default(PropertyMetadata);
					value2.Info = propertyInfo;
					value2.Type = propertyInfo.PropertyType;
					value.Properties.Add(propertyInfo.Name, value2);
				}
			}
			FieldInfo[] fields = type.GetFields();
			for (int j = 0; j < fields.Length; j++)
			{
				FieldInfo fieldInfo = fields[j];
				PropertyMetadata value3 = default(PropertyMetadata);
				value3.Info = fieldInfo;
				value3.IsField = true;
				value3.Type = fieldInfo.FieldType;
				value.Properties.Add(fieldInfo.Name, value3);
			}
			object obj = JsonMapper.object_metadata_lock;
			lock (obj)
			{
				try
				{
					JsonMapper.object_metadata.Add(type, value);
				}
				catch (ArgumentException)
				{
				}
			}
		}

		private static void AddTypeProperties(Type type)
		{
			if (JsonMapper.type_properties.ContainsKey(type))
			{
				return;
			}
			IList<PropertyMetadata> list = new List<PropertyMetadata>();
			PropertyInfo[] properties = type.GetProperties();
			for (int i = 0; i < properties.Length; i++)
			{
				PropertyInfo propertyInfo = properties[i];
				if (!(propertyInfo.Name == "Item"))
				{
					list.Add(new PropertyMetadata
					{
						Info = propertyInfo,
						IsField = false
					});
				}
			}
			FieldInfo[] fields = type.GetFields();
			for (int j = 0; j < fields.Length; j++)
			{
				FieldInfo info = fields[j];
				list.Add(new PropertyMetadata
				{
					Info = info,
					IsField = true
				});
			}
			object obj = JsonMapper.type_properties_lock;
			lock (obj)
			{
				try
				{
					JsonMapper.type_properties.Add(type, list);
				}
				catch (ArgumentException)
				{
				}
			}
		}

		private static MethodInfo GetConvOp(Type t1, Type t2)
		{
			object obj = JsonMapper.conv_ops_lock;
			lock (obj)
			{
				if (!JsonMapper.conv_ops.ContainsKey(t1))
				{
					JsonMapper.conv_ops.Add(t1, new Dictionary<Type, MethodInfo>());
				}
			}
			if (JsonMapper.conv_ops[t1].ContainsKey(t2))
			{
				return JsonMapper.conv_ops[t1][t2];
			}
			MethodInfo method = t1.GetMethod("op_Implicit", new Type[]
			{
				t2
			});
			object obj2 = JsonMapper.conv_ops_lock;
			lock (obj2)
			{
				try
				{
					JsonMapper.conv_ops[t1].Add(t2, method);
				}
				catch (ArgumentException)
				{
					return JsonMapper.conv_ops[t1][t2];
				}
			}
			return method;
		}

		private static object ReadValue(Type inst_type, JsonReader reader)
		{
			reader.Read();
			if (reader.Token == JsonToken.ArrayEnd)
			{
				return null;
			}
			if (reader.Token == JsonToken.Null)
			{
				if (!inst_type.IsClass)
				{
					throw new JsonException(string.Format("Can't assign null to an instance of type {0}", inst_type));
				}
				return null;
			}
			else
			{
				if (reader.Token != JsonToken.Double && reader.Token != JsonToken.Int && reader.Token != JsonToken.Long && reader.Token != JsonToken.String && reader.Token != JsonToken.Boolean)
				{
					object obj = null;
					if (reader.Token == JsonToken.ArrayStart)
					{
						JsonMapper.AddArrayMetadata(inst_type);
						ArrayMetadata arrayMetadata = JsonMapper.array_metadata[inst_type];
						if (!arrayMetadata.IsArray && !arrayMetadata.IsList)
						{
							throw new JsonException(string.Format("Type {0} can't act as an array", inst_type));
						}
						IList list;
						Type elementType;
						if (!arrayMetadata.IsArray)
						{
							list = (IList)Activator.CreateInstance(inst_type);
							elementType = arrayMetadata.ElementType;
						}
						else
						{
							list = new ArrayList();
							elementType = inst_type.GetElementType();
						}
						while (true)
						{
							object value = JsonMapper.ReadValue(elementType, reader);
							if (reader.Token == JsonToken.ArrayEnd)
							{
								break;
							}
							list.Add(value);
						}
						if (arrayMetadata.IsArray)
						{
							int count = list.Count;
							obj = Array.CreateInstance(elementType, count);
							for (int i = 0; i < count; i++)
							{
								((Array)obj).SetValue(list[i], i);
							}
						}
						else
						{
							obj = list;
						}
					}
					else if (reader.Token == JsonToken.ObjectStart)
					{
						JsonMapper.AddObjectMetadata(inst_type);
						ObjectMetadata objectMetadata = JsonMapper.object_metadata[inst_type];
						obj = Activator.CreateInstance(inst_type);
						string text;
						while (true)
						{
							reader.Read();
							if (reader.Token == JsonToken.ObjectEnd)
							{
								break;
							}
							text = (string)reader.Value;
							if (objectMetadata.Properties.ContainsKey(text))
							{
								PropertyMetadata propertyMetadata = objectMetadata.Properties[text];
								if (propertyMetadata.IsField)
								{
									((FieldInfo)propertyMetadata.Info).SetValue(obj, JsonMapper.ReadValue(propertyMetadata.Type, reader));
								}
								else
								{
									PropertyInfo propertyInfo = (PropertyInfo)propertyMetadata.Info;
									if (propertyInfo.CanWrite)
									{
										propertyInfo.SetValue(obj, JsonMapper.ReadValue(propertyMetadata.Type, reader), null);
									}
									else
									{
										JsonMapper.ReadValue(propertyMetadata.Type, reader);
									}
								}
							}
							else
							{
								if (!objectMetadata.IsDictionary)
								{
									goto Block_27;
								}
								((IDictionary)obj).Add(text, JsonMapper.ReadValue(objectMetadata.ElementType, reader));
							}
						}
						return obj;
						Block_27:
						throw new JsonException(string.Format("The type {0} doesn't have the property '{1}'", inst_type, text));
					}
					return obj;
				}
				Type type = reader.Value.GetType();
				if (inst_type.IsAssignableFrom(type))
				{
					return reader.Value;
				}
				if (JsonMapper.custom_importers_table.ContainsKey(type) && JsonMapper.custom_importers_table[type].ContainsKey(inst_type))
				{
					ImporterFunc importerFunc = JsonMapper.custom_importers_table[type][inst_type];
					return importerFunc(reader.Value);
				}
				if (JsonMapper.base_importers_table.ContainsKey(type) && JsonMapper.base_importers_table[type].ContainsKey(inst_type))
				{
					ImporterFunc importerFunc2 = JsonMapper.base_importers_table[type][inst_type];
					return importerFunc2(reader.Value);
				}
				if (inst_type.IsEnum)
				{
					return Enum.ToObject(inst_type, reader.Value);
				}
				MethodInfo convOp = JsonMapper.GetConvOp(inst_type, type);
				if (convOp != null)
				{
					return convOp.Invoke(null, new object[]
					{
						reader.Value
					});
				}
				throw new JsonException(string.Format("Can't assign value '{0}' (type {1}) to type {2}", reader.Value, type, inst_type));
			}
		}

		private static IJsonWrapper ReadValue(WrapperFactory factory, JsonReader reader)
		{
			reader.Read();
			if (reader.Token == JsonToken.ArrayEnd || reader.Token == JsonToken.Null)
			{
				return null;
			}
			IJsonWrapper jsonWrapper = factory();
			if (reader.Token == JsonToken.String)
			{
				jsonWrapper.SetString((string)reader.Value);
				return jsonWrapper;
			}
			if (reader.Token == JsonToken.Double)
			{
				jsonWrapper.SetDouble((double)reader.Value);
				return jsonWrapper;
			}
			if (reader.Token == JsonToken.Int)
			{
				jsonWrapper.SetInt((int)reader.Value);
				return jsonWrapper;
			}
			if (reader.Token == JsonToken.Long)
			{
				jsonWrapper.SetLong((long)reader.Value);
				return jsonWrapper;
			}
			if (reader.Token == JsonToken.Boolean)
			{
				jsonWrapper.SetBoolean((bool)reader.Value);
				return jsonWrapper;
			}
			if (reader.Token == JsonToken.ArrayStart)
			{
				jsonWrapper.SetJsonType(JsonType.Array);
				while (true)
				{
					IJsonWrapper value = JsonMapper.ReadValue(factory, reader);
					if (reader.Token == JsonToken.ArrayEnd)
					{
						break;
					}
					jsonWrapper.Add(value);
				}
			}
			else if (reader.Token == JsonToken.ObjectStart)
			{
				jsonWrapper.SetJsonType(JsonType.Object);
				while (true)
				{
					reader.Read();
					if (reader.Token == JsonToken.ObjectEnd)
					{
						break;
					}
					string key = (string)reader.Value;
					jsonWrapper[key] = JsonMapper.ReadValue(factory, reader);
				}
			}
			return jsonWrapper;
		}

		private static void RegisterBaseExporters()
		{
			JsonMapper.base_exporters_table[typeof(byte)] = delegate(object obj, JsonWriter writer)
			{
				writer.Write(Convert.ToInt32((byte)obj));
			};
			JsonMapper.base_exporters_table[typeof(char)] = delegate(object obj, JsonWriter writer)
			{
				writer.Write(Convert.ToString((char)obj));
			};
			JsonMapper.base_exporters_table[typeof(DateTime)] = delegate(object obj, JsonWriter writer)
			{
				writer.Write(Convert.ToString((DateTime)obj, JsonMapper.datetime_format));
			};
			JsonMapper.base_exporters_table[typeof(decimal)] = delegate(object obj, JsonWriter writer)
			{
				writer.Write((decimal)obj);
			};
			JsonMapper.base_exporters_table[typeof(sbyte)] = delegate(object obj, JsonWriter writer)
			{
				writer.Write(Convert.ToInt32((sbyte)obj));
			};
			JsonMapper.base_exporters_table[typeof(short)] = delegate(object obj, JsonWriter writer)
			{
				writer.Write(Convert.ToInt32((short)obj));
			};
			JsonMapper.base_exporters_table[typeof(ushort)] = delegate(object obj, JsonWriter writer)
			{
				writer.Write(Convert.ToInt32((ushort)obj));
			};
			JsonMapper.base_exporters_table[typeof(uint)] = delegate(object obj, JsonWriter writer)
			{
				writer.Write(Convert.ToUInt64((uint)obj));
			};
			JsonMapper.base_exporters_table[typeof(ulong)] = delegate(object obj, JsonWriter writer)
			{
				writer.Write((ulong)obj);
			};
		}

		private static void RegisterBaseImporters()
		{
			ImporterFunc importer = (object input) => Convert.ToByte((int)input);
			JsonMapper.RegisterImporter(JsonMapper.base_importers_table, typeof(int), typeof(byte), importer);
			importer = ((object input) => Convert.ToUInt64((int)input));
			JsonMapper.RegisterImporter(JsonMapper.base_importers_table, typeof(int), typeof(ulong), importer);
			importer = ((object input) => Convert.ToSByte((int)input));
			JsonMapper.RegisterImporter(JsonMapper.base_importers_table, typeof(int), typeof(sbyte), importer);
			importer = ((object input) => Convert.ToInt16((int)input));
			JsonMapper.RegisterImporter(JsonMapper.base_importers_table, typeof(int), typeof(short), importer);
			importer = ((object input) => Convert.ToUInt16((int)input));
			JsonMapper.RegisterImporter(JsonMapper.base_importers_table, typeof(int), typeof(ushort), importer);
			importer = ((object input) => Convert.ToUInt32((int)input));
			JsonMapper.RegisterImporter(JsonMapper.base_importers_table, typeof(int), typeof(uint), importer);
			importer = ((object input) => Convert.ToSingle((int)input));
			JsonMapper.RegisterImporter(JsonMapper.base_importers_table, typeof(int), typeof(float), importer);
			importer = ((object input) => Convert.ToDouble((int)input));
			JsonMapper.RegisterImporter(JsonMapper.base_importers_table, typeof(int), typeof(double), importer);
			importer = ((object input) => Convert.ToDecimal((double)input));
			JsonMapper.RegisterImporter(JsonMapper.base_importers_table, typeof(double), typeof(decimal), importer);
			importer = ((object input) => Convert.ToUInt32((long)input));
			JsonMapper.RegisterImporter(JsonMapper.base_importers_table, typeof(long), typeof(uint), importer);
			importer = ((object input) => Convert.ToChar((string)input));
			JsonMapper.RegisterImporter(JsonMapper.base_importers_table, typeof(string), typeof(char), importer);
			importer = ((object input) => Convert.ToDateTime((string)input, JsonMapper.datetime_format));
			JsonMapper.RegisterImporter(JsonMapper.base_importers_table, typeof(string), typeof(DateTime), importer);
		}

		private static void RegisterImporter(IDictionary<Type, IDictionary<Type, ImporterFunc>> table, Type json_type, Type value_type, ImporterFunc importer)
		{
			if (!table.ContainsKey(json_type))
			{
				table.Add(json_type, new Dictionary<Type, ImporterFunc>());
			}
			table[json_type][value_type] = importer;
		}

		private static void WriteValue(object obj, JsonWriter writer, bool writer_is_private, int depth)
		{
			if (depth > JsonMapper.max_nesting_depth)
			{
				throw new JsonException(string.Format("Max allowed object depth reached while trying to export from type {0}", obj.GetType()));
			}
			if (obj == null)
			{
				writer.Write(null);
				return;
			}
			if (obj is IJsonWrapper)
			{
				if (writer_is_private)
				{
					writer.TextWriter.Write(((IJsonWrapper)obj).ToJson());
				}
				else
				{
					((IJsonWrapper)obj).ToJson(writer);
				}
				return;
			}
			if (obj is string)
			{
				writer.Write((string)obj);
				return;
			}
			if (obj is double)
			{
				writer.Write((double)obj);
				return;
			}
			if (obj is int)
			{
				writer.Write((int)obj);
				return;
			}
			if (obj is bool)
			{
				writer.Write((bool)obj);
				return;
			}
			if (obj is long)
			{
				writer.Write((long)obj);
				return;
			}
			if (obj is Array)
			{
				writer.WriteArrayStart();
				foreach (object current in ((Array)obj))
				{
					JsonMapper.WriteValue(current, writer, writer_is_private, depth + 1);
				}
				writer.WriteArrayEnd();
				return;
			}
			if (obj is IList)
			{
				writer.WriteArrayStart();
				foreach (object current2 in ((IList)obj))
				{
					JsonMapper.WriteValue(current2, writer, writer_is_private, depth + 1);
				}
				writer.WriteArrayEnd();
				return;
			}
			if (obj is IDictionary)
			{
				writer.WriteObjectStart();
				foreach (DictionaryEntry dictionaryEntry in ((IDictionary)obj))
				{
					writer.WritePropertyName((string)dictionaryEntry.Key);
					JsonMapper.WriteValue(dictionaryEntry.Value, writer, writer_is_private, depth + 1);
				}
				writer.WriteObjectEnd();
				return;
			}
			Type type = obj.GetType();
			if (JsonMapper.custom_exporters_table.ContainsKey(type))
			{
				ExporterFunc exporterFunc = JsonMapper.custom_exporters_table[type];
				exporterFunc(obj, writer);
				return;
			}
			if (JsonMapper.base_exporters_table.ContainsKey(type))
			{
				ExporterFunc exporterFunc2 = JsonMapper.base_exporters_table[type];
				exporterFunc2(obj, writer);
				return;
			}
			if (obj is Enum)
			{
				Type underlyingType = Enum.GetUnderlyingType(type);
				if (underlyingType == typeof(long) || underlyingType == typeof(uint) || underlyingType == typeof(ulong))
				{
					writer.Write((ulong)obj);
				}
				else
				{
					writer.Write((int)obj);
				}
				return;
			}
			JsonMapper.AddTypeProperties(type);
			IList<PropertyMetadata> list = JsonMapper.type_properties[type];
			writer.WriteObjectStart();
			foreach (PropertyMetadata current3 in list)
			{
				if (current3.IsField)
				{
					writer.WritePropertyName(current3.Info.Name);
					JsonMapper.WriteValue(((FieldInfo)current3.Info).GetValue(obj), writer, writer_is_private, depth + 1);
				}
				else
				{
					PropertyInfo propertyInfo = (PropertyInfo)current3.Info;
					if (propertyInfo.CanRead)
					{
						writer.WritePropertyName(current3.Info.Name);
						JsonMapper.WriteValue(propertyInfo.GetValue(obj, null), writer, writer_is_private, depth + 1);
					}
				}
			}
			writer.WriteObjectEnd();
		}

		public static string ToJson(object obj)
		{
			object obj2 = JsonMapper.static_writer_lock;
			string result;
			lock (obj2)
			{
				JsonMapper.static_writer.Reset();
				JsonMapper.WriteValue(obj, JsonMapper.static_writer, true, 0);
				result = JsonMapper.static_writer.ToString();
			}
			return result;
		}

		public static void ToJson(object obj, JsonWriter writer)
		{
			JsonMapper.WriteValue(obj, writer, false, 0);
		}

		public static JsonData ToObject(JsonReader reader)
		{
			return (JsonData)JsonMapper.ToWrapper(() => new JsonData(), reader);
		}

		public static JsonData ToObject(TextReader reader)
		{
			JsonReader reader2 = new JsonReader(reader);
			return (JsonData)JsonMapper.ToWrapper(() => new JsonData(), reader2);
		}

		public static JsonData ToObject(string json)
		{
			return (JsonData)JsonMapper.ToWrapper(() => new JsonData(), json);
		}

		public static T ToObject<T>(JsonReader reader)
		{
			return (T)((object)JsonMapper.ReadValue(typeof(T), reader));
		}

		public static T ToObject<T>(TextReader reader)
		{
			JsonReader reader2 = new JsonReader(reader);
			return (T)((object)JsonMapper.ReadValue(typeof(T), reader2));
		}

		public static T ToObject<T>(string json)
		{
			JsonReader reader = new JsonReader(json);
			return (T)((object)JsonMapper.ReadValue(typeof(T), reader));
		}

		public static IJsonWrapper ToWrapper(WrapperFactory factory, JsonReader reader)
		{
			return JsonMapper.ReadValue(factory, reader);
		}

		public static IJsonWrapper ToWrapper(WrapperFactory factory, string json)
		{
			JsonReader reader = new JsonReader(json);
			return JsonMapper.ReadValue(factory, reader);
		}

		public static void RegisterExporter<T>(ExporterFunc<T> exporter)
		{
			ExporterFunc value = delegate(object obj, JsonWriter writer)
			{
				exporter((T)((object)obj), writer);
			};
			JsonMapper.custom_exporters_table[typeof(T)] = value;
		}

		public static void RegisterImporter<TJson, TValue>(ImporterFunc<TJson, TValue> importer)
		{
			ImporterFunc importer2 = (object input) => importer((TJson)((object)input));
			JsonMapper.RegisterImporter(JsonMapper.custom_importers_table, typeof(TJson), typeof(TValue), importer2);
		}

		public static void UnregisterExporters()
		{
			JsonMapper.custom_exporters_table.Clear();
		}

		public static void UnregisterImporters()
		{
			JsonMapper.custom_importers_table.Clear();
		}
	}
}
