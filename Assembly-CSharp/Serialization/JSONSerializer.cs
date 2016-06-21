using LitJson;
using System;
using System.Collections.Generic;
using System.Text;

namespace Serialization
{
	public class JSONSerializer : IStorage
	{
		public static string source = string.Empty;

		private readonly JsonReader _reader;

		private StringBuilder _json;

		private static bool _isReference;

		private static int _reference;

		private static string _currentType;

		private static int _multiDimensional;

		private static int _arrayCount;

		public string Data
		{
			get
			{
				return this._json.ToString().Replace(",]", "]").Replace(",}", "}");
			}
		}

		public bool SupportsOnDemand
		{
			get
			{
				return false;
			}
		}

		public JSONSerializer()
		{
		}

		public JSONSerializer(string json)
		{
			this._reader = new JsonReader(json);
		}

		public void StartSerializing()
		{
			this._json = new StringBuilder();
		}

		public void FinishedSerializing()
		{
		}

		public void FinishedDeserializing()
		{
		}

		public bool StartSerializing(Entry entry, int id)
		{
			return false;
		}

		public void FinishSerializing(Entry entry)
		{
		}

		public object StartDeserializing(Entry entry)
		{
			this._reader.Read();
			if (this._reader.Token == JsonToken.ObjectStart)
			{
				this._reader.Read();
				if ((string)this._reader.Value == "___o")
				{
					this._reader.Read();
					JSONSerializer._isReference = true;
					JSONSerializer._reference = (int)this._reader.Value;
				}
				else
				{
					JSONSerializer._isReference = false;
					JSONSerializer._reference = -1;
					this._reader.Read();
					JSONSerializer._currentType = (string)this._reader.Value;
					entry.StoredType = UnitySerializer.GetTypeEx(JSONSerializer._currentType);
				}
				return null;
			}
			return this._reader.Value;
		}

		public void DeserializeGetName(Entry entry)
		{
		}

		public void FinishDeserializing(Entry entry)
		{
		}

		public Entry[] ShouldWriteFields(Entry[] fields)
		{
			return fields;
		}

		public Entry[] ShouldWriteProperties(Entry[] properties)
		{
			return properties;
		}

		public void StartDeserializing()
		{
		}

		public Entry BeginReadProperty(Entry entry)
		{
			this._reader.Read();
			entry.Name = (string)this._reader.Value;
			return entry;
		}

		public void EndReadProperty()
		{
		}

		public Entry BeginReadField(Entry entry)
		{
			this._reader.Read();
			entry.Name = (string)this._reader.Value;
			return entry;
		}

		public void EndReadField()
		{
		}

		public int BeginReadProperties()
		{
			return 0;
		}

		public bool HasMore()
		{
			this._reader.Read();
			if (this._reader.Token == JsonToken.ArrayEnd || this._reader.Token == JsonToken.ObjectEnd)
			{
				return false;
			}
			this._reader.reReadToken = true;
			return true;
		}

		public int BeginReadFields()
		{
			return -1;
		}

		public void EndReadProperties()
		{
		}

		public void EndReadFields()
		{
			this._reader.reReadToken = true;
		}

		public T ReadSimpleValue<T>()
		{
			return (T)((object)this.ReadSimpleValue(typeof(T)));
		}

		public object ReadSimpleValue(Type type)
		{
			string name = type.Name;
			if (name != null)
			{
				if (JSONSerializer.<>f__switch$map1E == null)
				{
					JSONSerializer.<>f__switch$map1E = new Dictionary<string, int>(2)
					{
						{
							"DateTime",
							0
						},
						{
							"String",
							1
						}
					};
				}
				int num;
				if (JSONSerializer.<>f__switch$map1E.TryGetValue(name, out num))
				{
					if (num == 0)
					{
						return DateTime.Parse((string)this._reader.Value);
					}
					if (num == 1)
					{
						return UnitySerializer.UnEscape((string)this._reader.Value);
					}
				}
			}
			return this._reader.Value;
		}

		public bool IsMultiDimensionalArray(out int length)
		{
			length = -1;
			this._reader.Read();
			this._reader.reReadToken = true;
			return (string)this._reader.Value == "dimensions";
		}

		public void BeginReadMultiDimensionalArray(out int dimension, out int count)
		{
			this._reader.Read();
			this._reader.Read();
			dimension = (int)this._reader.Value;
			this._reader.Read();
			this._reader.Read();
			count = (int)this._reader.Value;
		}

		public void EndReadMultiDimensionalArray()
		{
		}

		public int ReadArrayDimension(int index)
		{
			this._reader.Read();
			this._reader.Read();
			return (int)this._reader.Value;
		}

		public Array ReadSimpleArray(Type elementType, int count)
		{
			this._reader.Read();
			this._reader.Read();
			count = (int)this._reader.Value;
			Array array = Array.CreateInstance(elementType, new long[]
			{
				(long)count
			});
			this._reader.Read();
			this._reader.Read();
			if (this._reader.Token == JsonToken.ArrayStart)
			{
				for (int i = 0; i < count; i++)
				{
					this._reader.Read();
					array.SetValue(Convert.ChangeType(this._reader.Value, elementType), i);
				}
				this._reader.Read();
			}
			return array;
		}

		public int BeginReadObject(out bool isReference)
		{
			if (JSONSerializer._isReference)
			{
				isReference = true;
				return JSONSerializer._reference;
			}
			isReference = false;
			return 0;
		}

		public void EndReadObject()
		{
			this._reader.Read();
		}

		public int BeginReadList(Type valueType)
		{
			this._reader.Read();
			if ((string)this._reader.Value == "___contents")
			{
				this._reader.Read();
				return -1;
			}
			return 0;
		}

		public object BeginReadListItem(int index, Entry entry)
		{
			return null;
		}

		public void EndReadListItem()
		{
		}

		public void EndReadList()
		{
		}

		public int BeginReadDictionary(Type keyType, Type valueType)
		{
			return -1;
		}

		public object BeginReadDictionaryKeyItem(int index, Entry entry)
		{
			return null;
		}

		public void EndReadDictionaryKeyItem()
		{
		}

		public object BeginReadDictionaryValueItem(int index, Entry entry)
		{
			return null;
		}

		public void EndReadDictionaryValueItem()
		{
		}

		public void EndReadDictionary()
		{
		}

		public int BeginReadObjectArray(Type valueType)
		{
			this._reader.Read();
			this._reader.Read();
			int result = (int)this._reader.Value;
			this._reader.Read();
			this._reader.Read();
			return result;
		}

		public object BeginReadObjectArrayItem(int index, Entry entry)
		{
			return null;
		}

		public void EndReadObjectArrayItem()
		{
		}

		public void EndReadObjectArray()
		{
			this._reader.Read();
		}

		public void BeginWriteObject(int id, Type objectType, bool wasSeen)
		{
			if (!wasSeen)
			{
				this._json.AppendFormat("{{\"___i\":\"{0}\",", objectType.FullName);
			}
			else
			{
				this._json.AppendFormat("{{\"___o\":{0},", id);
			}
		}

		public void EndWriteObject()
		{
			this._json.Append("}");
		}

		public void BeginWriteList(int count, Type listType)
		{
			this._json.Append("\"___contents\":[");
		}

		public bool BeginWriteListItem(int index, object value)
		{
			return false;
		}

		public void EndWriteListItem()
		{
			this._json.Append(",");
		}

		public void EndWriteList()
		{
			this._json.Append("],");
		}

		public void BeginWriteObjectArray(int count, Type arrayType)
		{
			if (JSONSerializer._multiDimensional > 0)
			{
				this._json.AppendFormat("\"count\":{1},\"contents{0}\":[", JSONSerializer._multiDimensional, count);
			}
			else
			{
				this._json.Append("\"count\":" + count + ",\"contents\":[");
			}
		}

		public bool BeginWriteObjectArrayItem(int index, object value)
		{
			return false;
		}

		public void EndWriteObjectArrayItem()
		{
			this._json.Append(",");
		}

		public void EndWriteObjectArray()
		{
			this._json.Append("],");
		}

		public void BeginMultiDimensionArray(Type arrayType, int dimensions, int count)
		{
			JSONSerializer._multiDimensional++;
			this._json.AppendFormat("\"dimensions\":{0},\"count\":{1},", dimensions, count);
		}

		public void EndMultiDimensionArray()
		{
			JSONSerializer._multiDimensional--;
		}

		public void WriteArrayDimension(int index, int count)
		{
			this._json.AppendFormat("\"dimension{0}\":{1},", index, count);
		}

		public void WriteSimpleArray(int count, Array array)
		{
			if (JSONSerializer._multiDimensional > 0)
			{
				this._json.AppendFormat("\"count{1}\":{0},", count, JSONSerializer._arrayCount);
				this._json.AppendFormat("\"contents{0}\":[", JSONSerializer._arrayCount++);
			}
			else
			{
				this._json.AppendFormat("\"count\":{0},", count);
				this._json.Append("\"contents\":[");
			}
			bool flag = true;
			foreach (object current in array)
			{
				if (!flag)
				{
					this._json.Append(",");
				}
				flag = false;
				this.WriteSimpleValue(current);
			}
			this._json.Append("],");
		}

		public void WriteSimpleValue(object value)
		{
			if (value is string)
			{
				this._json.AppendFormat("\"{0}\"", UnitySerializer.Escape((string)value));
			}
			else if (value is DateTime)
			{
				this._json.AppendFormat("\"{0}\"", value);
			}
			else if (value is bool)
			{
				this._json.Append((!(bool)value) ? "false" : "true");
			}
			else if (value is float || value is double)
			{
				this._json.AppendFormat("{0:0.00000000}", value);
			}
			else
			{
				this._json.AppendFormat("{0}", value);
			}
		}

		public void BeginWriteDictionary(int count, Type dictionaryType)
		{
		}

		public bool BeginWriteDictionaryKey(int id, object value)
		{
			return false;
		}

		public void EndWriteDictionaryKey()
		{
			this._json.AppendFormat(",", new object[0]);
		}

		public bool BeginWriteDictionaryValue(int id, object value)
		{
			return false;
		}

		public void EndWriteDictionaryValue()
		{
			this._json.AppendFormat(",", new object[0]);
		}

		public void EndWriteDictionary()
		{
		}

		public void BeginWriteProperties(int count)
		{
		}

		public void EndWriteProperties()
		{
		}

		public void BeginWriteProperty(string name, Type type)
		{
			this._json.AppendFormat("\"{0}\":", name);
		}

		public void EndWriteProperty()
		{
			this._json.AppendFormat(",", new object[0]);
		}

		public void BeginWriteFields(int count)
		{
		}

		public void EndWriteFields()
		{
		}

		public void BeginWriteField(string name, Type type)
		{
			this._json.AppendFormat("\"{0}\":", name);
		}

		public void EndWriteField()
		{
			this._json.AppendFormat(",", new object[0]);
		}

		public void BeginOnDemand(int id)
		{
		}

		public void EndOnDemand()
		{
		}

		public void BeginReadDictionaryKeys()
		{
			this._reader.Read();
			if ((string)this._reader.Value == "___keys")
			{
				this._reader.Read();
			}
		}

		public void EndReadDictionaryKeys()
		{
		}

		public void BeginReadDictionaryValues()
		{
			this._reader.Read();
			if ((string)this._reader.Value == "___values")
			{
				this._reader.Read();
			}
		}

		public void EndReadDictionaryValues()
		{
		}

		public void BeginWriteDictionaryKeys()
		{
			this._json.Append("\"___keys\": [");
		}

		public void EndWriteDictionaryKeys()
		{
			this._json.Append("],");
		}

		public void BeginWriteDictionaryValues()
		{
			this._json.Append("\"___values\": [");
		}

		public void EndWriteDictionaryValues()
		{
			this._json.Append("],");
		}
	}
}
