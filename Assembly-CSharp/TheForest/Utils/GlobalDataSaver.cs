using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheForest.Utils
{
	public class GlobalDataSaver : MonoBehaviour
	{
		public enum DataTypes
		{
			Int,
			Float,
			String
		}

		[SerializeThis]
		private Dictionary<string, int> _intData;

		[SerializeThis]
		private int _intDataCount;

		[SerializeThis]
		private Dictionary<string, float> _floatData;

		[SerializeThis]
		private int _floatDataCount;

		[SerializeThis]
		private Dictionary<string, string> _stringData;

		[SerializeThis]
		private int _stringDataCount;

		[SerializeThis]
		private Dictionary<long, int> _longIntData;

		[SerializeThis]
		private int _longIntDataCount;

		[SerializeThis]
		private Dictionary<long, float> _longFloatData;

		[SerializeThis]
		private int _longFloatDataCount;

		[SerializeThis]
		private Dictionary<long, string> _longStringData;

		[SerializeThis]
		private int _longStringDataCount;

		private static GlobalDataSaver Instance;

		private void Awake()
		{
			GlobalDataSaver.Instance = this;
			this._intData = new Dictionary<string, int>(1000);
			this._floatData = new Dictionary<string, float>();
			this._stringData = new Dictionary<string, string>();
			this._longIntData = new Dictionary<long, int>(200);
			this._longFloatData = new Dictionary<long, float>();
			this._longStringData = new Dictionary<long, string>();
		}

		private void OnDestroy()
		{
			if (GlobalDataSaver.Instance == this)
			{
				GlobalDataSaver.Instance = null;
			}
		}

		private void OnSerializing()
		{
			this._intDataCount = this._intData.Count;
			this._floatDataCount = this._floatData.Count;
			this._stringDataCount = this._stringData.Count;
			this._longIntDataCount = this._longIntData.Count;
			this._longFloatDataCount = this._longFloatData.Count;
			this._longStringDataCount = this._longStringData.Count;
		}

		private void OnDeserialized()
		{
			if (this._intData.Count != this._intDataCount)
			{
				Debug.LogError(string.Concat(new object[]
				{
					"GlobalDataSaver this._intData.Count missmatch (",
					this._intData.Count,
					"/",
					this._intDataCount,
					")"
				}));
			}
		}

		public static void SetInt(string name, int value)
		{
			if (GlobalDataSaver.Instance)
			{
				GlobalDataSaver.Instance._intData[name] = value;
			}
		}

		public static void SetFloat(string name, float value)
		{
			if (GlobalDataSaver.Instance)
			{
				GlobalDataSaver.Instance._floatData[name] = value;
			}
		}

		public static void SetString(string name, string value)
		{
			if (GlobalDataSaver.Instance)
			{
				GlobalDataSaver.Instance._stringData[name] = value;
			}
		}

		public static void SetInt(long key, int value)
		{
			if (GlobalDataSaver.Instance)
			{
				GlobalDataSaver.Instance._longIntData[key] = value;
			}
		}

		public static void SetFloat(long key, float value)
		{
			if (GlobalDataSaver.Instance)
			{
				GlobalDataSaver.Instance._longFloatData[key] = value;
			}
		}

		public static void SetString(long key, string value)
		{
			if (GlobalDataSaver.Instance)
			{
				GlobalDataSaver.Instance._longStringData[key] = value;
			}
		}

		public static int GetInt(string name, int defaultValue = 0)
		{
			int result;
			if (GlobalDataSaver.Instance._intData.TryGetValue(name, out result))
			{
				return result;
			}
			return defaultValue;
		}

		public static float GetFloat(string name, float defaultValue = 0f)
		{
			float result;
			if (GlobalDataSaver.Instance._floatData.TryGetValue(name, out result))
			{
				return result;
			}
			return defaultValue;
		}

		public static string GetString(string name, string defaultValue = "")
		{
			string result;
			if (GlobalDataSaver.Instance._stringData.TryGetValue(name, out result))
			{
				return result;
			}
			return defaultValue;
		}

		public static int GetInt(long key, int defaultValue = 0)
		{
			int result;
			if (GlobalDataSaver.Instance._longIntData.TryGetValue(key, out result))
			{
				return result;
			}
			return defaultValue;
		}

		public static float GetFloat(long key, float defaultValue = 0f)
		{
			float result;
			if (GlobalDataSaver.Instance._longFloatData.TryGetValue(key, out result))
			{
				return result;
			}
			return defaultValue;
		}

		public static string GetString(long key, string defaultValue = "")
		{
			string result;
			if (GlobalDataSaver.Instance._longStringData.TryGetValue(key, out result))
			{
				return result;
			}
			return defaultValue;
		}

		public static void ClearInt(string name)
		{
			GlobalDataSaver.Instance._intData.Remove(name);
		}

		public static void ClearFloat(string name)
		{
			GlobalDataSaver.Instance._floatData.Remove(name);
		}

		public static void ClearString(string name)
		{
			GlobalDataSaver.Instance._stringData.Remove(name);
		}

		public static void ClearLongInt(long key)
		{
			GlobalDataSaver.Instance._longIntData.Remove(key);
		}

		public static void ClearLongFloat(long key)
		{
			GlobalDataSaver.Instance._longFloatData.Remove(key);
		}

		public static void ClearLongString(long key)
		{
			GlobalDataSaver.Instance._longStringData.Remove(key);
		}
	}
}
