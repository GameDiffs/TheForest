using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class KeyValueList<K, V> : IList, IEnumerable, ICollection
{
	private List<K> keyList = new List<K>();

	private List<V> valList = new List<V>();

	object IList.this[int index]
	{
		get
		{
			throw new NotImplementedException();
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	public V this[K key]
	{
		get
		{
			V result;
			if (this.TryGetValue(key, out result))
			{
				return result;
			}
			throw new KeyNotFoundException();
		}
		set
		{
			int num = this.keyList.IndexOf(key);
			if (num == -1)
			{
				this.keyList.Add(key);
				this.valList.Add(value);
			}
			else
			{
				this.valList[num] = value;
			}
		}
	}

	public int Count
	{
		get
		{
			return this.valList.Count;
		}
	}

	public bool IsFixedSize
	{
		get
		{
			return false;
		}
	}

	public bool IsReadOnly
	{
		get
		{
			return false;
		}
	}

	public bool IsSynchronized
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public object SyncRoot
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public KeyValueList()
	{
	}

	public KeyValueList(ref List<K> keyListRef, ref List<V> valListRef)
	{
		this.keyList = keyListRef;
		this.valList = valListRef;
	}

	public KeyValueList(KeyValueList<K, V> otherKeyValueList)
	{
		this.AddRange(otherKeyValueList);
	}

	[DebuggerHidden]
	IEnumerator IEnumerable.GetEnumerator()
	{
		KeyValueList<K, V>.GetEnumerator>c__Iterator1 getEnumerator>c__Iterator = new KeyValueList<K, V>.GetEnumerator>c__Iterator1();
		getEnumerator>c__Iterator.<>f__this = this;
		return getEnumerator>c__Iterator;
	}

	public bool TryGetValue(K key, out V value)
	{
		int num = this.keyList.IndexOf(key);
		if (num == -1)
		{
			value = default(V);
			return false;
		}
		value = this.valList[num];
		return true;
	}

	public int Add(object value)
	{
		throw new NotImplementedException("Use KeyValueList[key] = value or insert");
	}

	public void Clear()
	{
		this.keyList.Clear();
		this.valList.Clear();
	}

	public bool Contains(V value)
	{
		return this.valList.Contains(value);
	}

	public bool ContainsKey(K key)
	{
		return this.keyList.Contains(key);
	}

	public int IndexOf(K key)
	{
		return this.keyList.IndexOf(key);
	}

	public void Insert(int index, K key, V value)
	{
		if (this.keyList.Contains(key))
		{
			throw new Exception("Cannot insert duplicate key.");
		}
		this.keyList.Insert(index, key);
		this.valList.Insert(index, value);
	}

	public void Insert(int index, KeyValuePair<K, V> kvp)
	{
		this.Insert(index, kvp.Key, kvp.Value);
	}

	public void Insert(int index, object value)
	{
		string message = "Use Insert(K key, V value) or Insert(KeyValuePair<K, V>)";
		throw new NotImplementedException(message);
	}

	public void Remove(K key)
	{
		int num = this.keyList.IndexOf(key);
		if (num == -1)
		{
			throw new KeyNotFoundException();
		}
		this.keyList.RemoveAt(num);
		this.valList.RemoveAt(num);
	}

	public void Remove(object value)
	{
		throw new NotImplementedException("Use Remove(K key)");
	}

	public void RemoveAt(int index)
	{
		this.keyList.RemoveAt(index);
		this.valList.RemoveAt(index);
	}

	public V GetAt(int index)
	{
		if (index >= this.valList.Count)
		{
			throw new IndexOutOfRangeException();
		}
		return this.valList[index];
	}

	public void SetAt(int index, V value)
	{
		if (index >= this.valList.Count)
		{
			throw new IndexOutOfRangeException();
		}
		this.valList[index] = value;
	}

	public void CopyTo(V[] array, int index)
	{
		this.valList.CopyTo(array, index);
	}

	public void CopyTo(KeyValueList<K, V> otherKeyValueList, int index)
	{
		foreach (KeyValuePair<K, V> current in this)
		{
			otherKeyValueList[current.Key] = current.Value;
		}
	}

	public void AddRange(KeyValueList<K, V> otherKeyValueList)
	{
		otherKeyValueList.CopyTo(this, 0);
	}

	[DebuggerHidden]
	public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
	{
		KeyValueList<K, V>.<GetEnumerator>c__Iterator2 <GetEnumerator>c__Iterator = new KeyValueList<K, V>.<GetEnumerator>c__Iterator2();
		<GetEnumerator>c__Iterator.<>f__this = this;
		return <GetEnumerator>c__Iterator;
	}

	public override string ToString()
	{
		string[] array = new string[this.keyList.Count];
		string format = "{0}:{1}";
		int num = 0;
		foreach (KeyValuePair<K, V> current in this)
		{
			array[num] = string.Format(format, current.Key, current.Value);
			num++;
		}
		return string.Format("[{0}]", string.Join(", ", array));
	}

	public bool Contains(object value)
	{
		throw new NotImplementedException();
	}

	public int IndexOf(object value)
	{
		throw new NotImplementedException();
	}

	public void CopyTo(Array array, int index)
	{
		throw new NotImplementedException();
	}
}
