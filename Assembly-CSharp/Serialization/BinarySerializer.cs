using System;
using System.IO;

namespace Serialization
{
	public class BinarySerializer : IStorage
	{
		private MemoryStream _myStream;

		private BinaryWriter _writer;

		private BinaryReader _reader;

		public byte[] Data
		{
			get;
			private set;
		}

		public bool SupportsOnDemand
		{
			get
			{
				return false;
			}
		}

		public BinarySerializer()
		{
		}

		public BinarySerializer(byte[] data)
		{
			this.Data = data;
		}

		private void EncodeType(object item, Type storedType)
		{
			if (item == null)
			{
				this.WriteSimpleValue(65534);
				return;
			}
			Type type = item.GetType();
			if (storedType == null || storedType != item.GetType() || UnitySerializer.Verbose)
			{
				ushort typeId = UnitySerializer.GetTypeId(type);
				this.WriteSimpleValue(typeId);
			}
			else
			{
				this.WriteSimpleValue(65535);
			}
		}

		public bool StartSerializing(Entry entry, int id)
		{
			if (entry.MustHaveName)
			{
				ushort propertyDefinitionId = UnitySerializer.GetPropertyDefinitionId(entry.Name);
				this.WriteSimpleValue(propertyDefinitionId);
			}
			object item = entry.Value ?? new UnitySerializer.Nuller();
			this.EncodeType(item, entry.StoredType);
			return false;
		}

		public void StartSerializing()
		{
			this._myStream = new MemoryStream();
			this._writer = new BinaryWriter(this._myStream);
			UnitySerializer.PushKnownTypes();
			UnitySerializer.PushPropertyNames();
		}

		public void FinishedSerializing()
		{
			this._writer.Flush();
			this._writer.Close();
			this._myStream.Flush();
			byte[] array = this._myStream.ToArray();
			this._myStream.Close();
			this._myStream = null;
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			binaryWriter.Write("SerV10");
			binaryWriter.Write(UnitySerializer.Verbose);
			if (UnitySerializer.SerializationScope.IsPrimaryScope)
			{
				binaryWriter.Write(UnitySerializer._knownTypesLookup.Count);
				foreach (Type current in UnitySerializer._knownTypesLookup.Keys)
				{
					binaryWriter.Write(current.FullName);
				}
				binaryWriter.Write(UnitySerializer._propertyLookup.Count);
				foreach (string current2 in UnitySerializer._propertyLookup.Keys)
				{
					binaryWriter.Write(current2);
				}
			}
			else
			{
				binaryWriter.Write(0);
				binaryWriter.Write(0);
			}
			binaryWriter.Write(array.Length);
			binaryWriter.Write(array);
			binaryWriter.Flush();
			binaryWriter.Close();
			memoryStream.Flush();
			this.Data = memoryStream.ToArray();
			memoryStream.Close();
			this._writer = null;
			this._reader = null;
			UnitySerializer.PopKnownTypes();
			UnitySerializer.PopPropertyNames();
		}

		public void BeginOnDemand(int id)
		{
		}

		public void EndOnDemand()
		{
		}

		public void BeginWriteObject(int id, Type objectType, bool wasSeen)
		{
			if (objectType == null)
			{
				this.WriteSimpleValue('X');
			}
			else if (wasSeen)
			{
				this.WriteSimpleValue('S');
				this.WriteSimpleValue(id);
			}
			else
			{
				this.WriteSimpleValue('O');
			}
		}

		public void BeginWriteProperties(int count)
		{
			if (count > 250)
			{
				this.WriteSimpleValue(255);
				this.WriteSimpleValue(count);
			}
			else
			{
				this.WriteSimpleValue((byte)count);
			}
		}

		public void BeginWriteFields(int count)
		{
			if (count > 250)
			{
				this.WriteSimpleValue(255);
				this.WriteSimpleValue(count);
			}
			else
			{
				this.WriteSimpleValue((byte)count);
			}
		}

		public void WriteSimpleValue(object value)
		{
			UnitySerializer.WriteValue(this._writer, value);
		}

		public void BeginWriteList(int count, Type listType)
		{
			this.WriteSimpleValue(count);
		}

		public void BeginWriteDictionary(int count, Type dictionaryType)
		{
			this.WriteSimpleValue(count);
		}

		public void WriteSimpleArray(int count, Array array)
		{
			this.WriteSimpleValue(count);
			Type elementType = array.GetType().GetElementType();
			if (elementType == typeof(byte))
			{
				UnitySerializer.WriteValue(this._writer, array);
			}
			else if (elementType.IsPrimitive)
			{
				byte[] array2 = new byte[Buffer.ByteLength(array)];
				Buffer.BlockCopy(array, 0, array2, 0, array2.Length);
				UnitySerializer.WriteValue(this._writer, array2);
			}
			else
			{
				for (int i = 0; i < count; i++)
				{
					object value = array.GetValue(i);
					if (value == null)
					{
						UnitySerializer.WriteValue(this._writer, 0);
					}
					else
					{
						UnitySerializer.WriteValue(this._writer, 1);
						UnitySerializer.WriteValue(this._writer, value);
					}
				}
			}
		}

		public void BeginMultiDimensionArray(Type arrayType, int dimensions, int count)
		{
			this.WriteSimpleValue(-1);
			this.WriteSimpleValue(dimensions);
			this.WriteSimpleValue(count);
		}

		public void WriteArrayDimension(int dimension, int count)
		{
			this.WriteSimpleValue(count);
		}

		public void BeginWriteObjectArray(int count, Type arrayType)
		{
			this.WriteSimpleValue(count);
		}

		public Entry[] ShouldWriteFields(Entry[] fields)
		{
			return fields;
		}

		public Entry[] ShouldWriteProperties(Entry[] properties)
		{
			return properties;
		}

		private Type DecodeType(Type storedType)
		{
			Type result;
			try
			{
				ushort num = this.ReadSimpleValue<ushort>();
				if (num == 65535)
				{
					result = storedType;
				}
				else if (num == 65534)
				{
					result = null;
				}
				else
				{
					if (num >= 60000)
					{
						try
						{
							result = UnitySerializer.PrewarmLookup[(int)(num - 60000)];
							return result;
						}
						catch
						{
							throw new Exception("Data stream appears corrupt, found a TYPE ID of " + num.ToString());
						}
					}
					storedType = UnitySerializer._knownTypesList[(int)num];
					result = storedType;
				}
			}
			catch
			{
				result = null;
			}
			return result;
		}

		public void FinishedDeserializing()
		{
			this._reader.Close();
			this._myStream.Close();
			this._reader = null;
			this._myStream = null;
			this._writer = null;
			UnitySerializer.PopKnownTypes();
			UnitySerializer.PopPropertyNames();
		}

		public void DeserializeGetName(Entry entry)
		{
			if (!entry.MustHaveName)
			{
				return;
			}
			ushort num = this.ReadSimpleValue<ushort>();
			try
			{
				entry.Name = ((num < 50000) ? UnitySerializer._propertyList[(int)num] : PreWarm.PrewarmNames[(int)(num - 50000)]);
			}
			catch
			{
				throw new Exception(string.Concat(new object[]
				{
					"Data stream may be corrupt, found an id of ",
					num,
					" when looking a property name id (out of UnitySerializer._propertyList.Count=",
					UnitySerializer._propertyList.Count,
					")"
				}));
			}
		}

		public object StartDeserializing(Entry entry)
		{
			Type storedType = this.DecodeType(entry.StoredType);
			entry.StoredType = storedType;
			return null;
		}

		public Entry BeginReadProperty(Entry entry)
		{
			return entry;
		}

		public void EndReadProperty()
		{
		}

		public Entry BeginReadField(Entry entry)
		{
			return entry;
		}

		public void EndReadField()
		{
		}

		public void StartDeserializing()
		{
			UnitySerializer.PushKnownTypes();
			UnitySerializer.PushPropertyNames();
			MemoryStream memoryStream = new MemoryStream(this.Data);
			BinaryReader binaryReader = new BinaryReader(memoryStream);
			string text = binaryReader.ReadString();
			UnitySerializer.currentVersion = int.Parse(text.Substring(4));
			if (UnitySerializer.currentVersion >= 3)
			{
				UnitySerializer.Verbose = binaryReader.ReadBoolean();
			}
			int num = binaryReader.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				string text2 = binaryReader.ReadString();
				Type type = UnitySerializer.GetTypeEx(text2);
				if (type == null)
				{
					UnitySerializer.TypeMappingEventArgs typeMappingEventArgs = new UnitySerializer.TypeMappingEventArgs
					{
						TypeName = text2
					};
					UnitySerializer.InvokeMapMissingType(typeMappingEventArgs);
					type = typeMappingEventArgs.UseType;
				}
				if (type == null)
				{
					throw new ArgumentException(string.Format("Cannot reference type {0} in this context", text2));
				}
				UnitySerializer._knownTypesList.Add(type);
			}
			num = binaryReader.ReadInt32();
			for (int j = 0; j < num; j++)
			{
				UnitySerializer._propertyList.Add(binaryReader.ReadString());
			}
			byte[] buffer = binaryReader.ReadBytes(binaryReader.ReadInt32());
			this._myStream = new MemoryStream(buffer);
			this._reader = new BinaryReader(this._myStream);
			binaryReader.Close();
			memoryStream.Close();
		}

		public void FinishDeserializing(Entry entry)
		{
		}

		public Array ReadSimpleArray(Type elementType, int count)
		{
			if (count == -1)
			{
				count = this.ReadSimpleValue<int>();
			}
			if (elementType == typeof(byte))
			{
				return this.ReadSimpleValue<byte[]>();
			}
			if (elementType.IsPrimitive && UnitySerializer.currentVersion >= 6)
			{
				byte[] array = this.ReadSimpleValue<byte[]>();
				Array array2 = Array.CreateInstance(elementType, count);
				Buffer.BlockCopy(array, 0, array2, 0, array.Length);
				return array2;
			}
			Array array3 = Array.CreateInstance(elementType, count);
			if (UnitySerializer.currentVersion >= 8)
			{
				for (int i = 0; i < count; i++)
				{
					byte b = (byte)this.ReadSimpleValue(typeof(byte));
					array3.SetValue((b == 0) ? null : this.ReadSimpleValue(elementType), i);
				}
			}
			else
			{
				for (int j = 0; j < count; j++)
				{
					array3.SetValue(this.ReadSimpleValue(elementType), j);
				}
			}
			return array3;
		}

		public int BeginReadProperties()
		{
			byte b = this.ReadSimpleValue<byte>();
			return (b != 255) ? ((int)b) : this.ReadSimpleValue<int>();
		}

		public int BeginReadFields()
		{
			byte b = this.ReadSimpleValue<byte>();
			return (b != 255) ? ((int)b) : this.ReadSimpleValue<int>();
		}

		public T ReadSimpleValue<T>()
		{
			return (T)((object)this.ReadSimpleValue(typeof(T)));
		}

		public object ReadSimpleValue(Type type)
		{
			UnitySerializer.ReadAValue readAValue;
			if (!UnitySerializer.Readers.TryGetValue(type, out readAValue))
			{
				return this._reader.ReadInt32();
			}
			return readAValue(this._reader);
		}

		public bool IsMultiDimensionalArray(out int length)
		{
			int num = this.ReadSimpleValue<int>();
			if (num == -1)
			{
				length = -1;
				return true;
			}
			length = num;
			return false;
		}

		public int BeginReadDictionary(Type keyType, Type valueType)
		{
			return this.ReadSimpleValue<int>();
		}

		public void EndReadDictionary()
		{
		}

		public int BeginReadObjectArray(Type valueType)
		{
			return this.ReadSimpleValue<int>();
		}

		public void EndReadObjectArray()
		{
		}

		public void BeginReadMultiDimensionalArray(out int dimension, out int count)
		{
			dimension = this.ReadSimpleValue<int>();
			count = this.ReadSimpleValue<int>();
		}

		public void EndReadMultiDimensionalArray()
		{
		}

		public int ReadArrayDimension(int index)
		{
			return this.ReadSimpleValue<int>();
		}

		public int BeginReadList(Type valueType)
		{
			return this.ReadSimpleValue<int>();
		}

		public void EndReadList()
		{
		}

		public int BeginReadObject(out bool isReference)
		{
			char c = this.ReadSimpleValue<char>();
			if (c == 'X')
			{
				isReference = false;
				return -1;
			}
			int result;
			if (c == 'O')
			{
				result = -1;
				isReference = false;
			}
			else
			{
				result = this.ReadSimpleValue<int>();
				isReference = true;
			}
			return result;
		}

		public void EndWriteObjectArray()
		{
		}

		public void EndWriteList()
		{
		}

		public void EndWriteDictionary()
		{
		}

		public bool BeginWriteDictionaryKey(int id, object value)
		{
			return false;
		}

		public void EndWriteDictionaryKey()
		{
		}

		public bool BeginWriteDictionaryValue(int id, object value)
		{
			return false;
		}

		public void EndWriteDictionaryValue()
		{
		}

		public void EndMultiDimensionArray()
		{
		}

		public void EndReadObject()
		{
		}

		public bool BeginWriteListItem(int index, object value)
		{
			return false;
		}

		public void EndWriteListItem()
		{
		}

		public bool BeginWriteObjectArrayItem(int index, object value)
		{
			return false;
		}

		public void EndWriteObjectArrayItem()
		{
		}

		public void EndReadProperties()
		{
		}

		public void EndReadFields()
		{
		}

		public object BeginReadListItem(int index, Entry entry)
		{
			return null;
		}

		public void EndReadListItem()
		{
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

		public object BeginReadObjectArrayItem(int index, Entry entry)
		{
			return null;
		}

		public void EndReadObjectArrayItem()
		{
		}

		public void EndWriteObject()
		{
		}

		public void BeginWriteProperty(string name, Type type)
		{
		}

		public void EndWriteProperty()
		{
		}

		public void BeginWriteField(string name, Type type)
		{
		}

		public void EndWriteField()
		{
		}

		public void EndWriteProperties()
		{
		}

		public void EndWriteFields()
		{
		}

		public void FinishSerializing(Entry entry)
		{
		}

		public void BeginReadDictionaryKeys()
		{
		}

		public void EndReadDictionaryKeys()
		{
		}

		public void BeginReadDictionaryValues()
		{
		}

		public void EndReadDictionaryValues()
		{
		}

		public void BeginWriteDictionaryKeys()
		{
		}

		public void EndWriteDictionaryKeys()
		{
		}

		public void BeginWriteDictionaryValues()
		{
		}

		public void EndWriteDictionaryValues()
		{
		}

		public bool HasMore()
		{
			throw new NotImplementedException();
		}
	}
}
