using System;

namespace Serialization
{
	public interface IStorage
	{
		bool SupportsOnDemand
		{
			get;
		}

		void StartSerializing();

		void FinishedSerializing();

		void FinishedDeserializing();

		bool StartSerializing(Entry entry, int id);

		void FinishSerializing(Entry entry);

		object StartDeserializing(Entry entry);

		void DeserializeGetName(Entry entry);

		void FinishDeserializing(Entry entry);

		Entry[] ShouldWriteFields(Entry[] fields);

		Entry[] ShouldWriteProperties(Entry[] properties);

		void StartDeserializing();

		bool HasMore();

		Entry BeginReadProperty(Entry entry);

		void EndReadProperty();

		Entry BeginReadField(Entry entry);

		void EndReadField();

		int BeginReadProperties();

		int BeginReadFields();

		void EndReadProperties();

		void EndReadFields();

		T ReadSimpleValue<T>();

		object ReadSimpleValue(Type type);

		bool IsMultiDimensionalArray(out int length);

		void BeginReadMultiDimensionalArray(out int dimension, out int count);

		void EndReadMultiDimensionalArray();

		int ReadArrayDimension(int index);

		Array ReadSimpleArray(Type elementType, int count);

		int BeginReadObject(out bool isReference);

		void EndReadObject();

		int BeginReadList(Type valueType);

		object BeginReadListItem(int index, Entry entry);

		void EndReadListItem();

		void EndReadList();

		int BeginReadDictionary(Type keyType, Type valueType);

		void BeginReadDictionaryKeys();

		object BeginReadDictionaryKeyItem(int index, Entry entry);

		void EndReadDictionaryKeyItem();

		void EndReadDictionaryKeys();

		void BeginReadDictionaryValues();

		object BeginReadDictionaryValueItem(int index, Entry entry);

		void EndReadDictionaryValueItem();

		void EndReadDictionaryValues();

		void EndReadDictionary();

		int BeginReadObjectArray(Type valueType);

		object BeginReadObjectArrayItem(int index, Entry entry);

		void EndReadObjectArrayItem();

		void EndReadObjectArray();

		void BeginWriteObject(int id, Type objectType, bool wasSeen);

		void EndWriteObject();

		void BeginWriteList(int count, Type listType);

		bool BeginWriteListItem(int index, object value);

		void EndWriteListItem();

		void EndWriteList();

		void BeginWriteObjectArray(int count, Type arrayType);

		bool BeginWriteObjectArrayItem(int index, object value);

		void EndWriteObjectArrayItem();

		void EndWriteObjectArray();

		void BeginMultiDimensionArray(Type arrayType, int dimensions, int count);

		void EndMultiDimensionArray();

		void WriteArrayDimension(int index, int count);

		void WriteSimpleArray(int count, Array array);

		void WriteSimpleValue(object value);

		void BeginWriteDictionary(int count, Type dictionaryType);

		void BeginWriteDictionaryKeys();

		bool BeginWriteDictionaryKey(int id, object value);

		void EndWriteDictionaryKey();

		void EndWriteDictionaryKeys();

		void BeginWriteDictionaryValues();

		bool BeginWriteDictionaryValue(int id, object value);

		void EndWriteDictionaryValue();

		void EndWriteDictionaryValues();

		void EndWriteDictionary();

		void BeginWriteProperties(int count);

		void EndWriteProperties();

		void BeginWriteProperty(string name, Type type);

		void EndWriteProperty();

		void BeginWriteFields(int count);

		void EndWriteFields();

		void BeginWriteField(string name, Type type);

		void EndWriteField();

		void BeginOnDemand(int id);

		void EndOnDemand();
	}
}
