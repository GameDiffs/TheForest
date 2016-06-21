using System;
using UnityEngine;

namespace TheForest.Utils
{
	public class GlobalDataBinding : MonoBehaviour
	{
		public string _dataName;

		public GlobalDataSaver.DataTypes _dataType;

		public string _message;

		private void Start()
		{
			switch (this._dataType)
			{
			case GlobalDataSaver.DataTypes.Int:
				base.SendMessage(this._message, GlobalDataSaver.GetInt(this._dataName, 0));
				break;
			case GlobalDataSaver.DataTypes.Float:
				base.SendMessage(this._message, GlobalDataSaver.GetFloat(this._dataName, 0f));
				break;
			case GlobalDataSaver.DataTypes.String:
				base.SendMessage(this._message, GlobalDataSaver.GetString(this._dataName, string.Empty));
				break;
			}
		}

		public void SetIntData(int value)
		{
			GlobalDataSaver.SetInt(this._dataName, value);
		}

		public void SetFloatData(float value)
		{
			GlobalDataSaver.SetFloat(this._dataName, value);
		}

		public void SetStringData(string value)
		{
			GlobalDataSaver.SetString(this._dataName, value);
		}

		public void ClearIntData()
		{
			GlobalDataSaver.ClearInt(this._dataName);
		}

		public void ClearFloatData()
		{
			GlobalDataSaver.ClearFloat(this._dataName);
		}

		public void ClearStringData()
		{
			GlobalDataSaver.ClearString(this._dataName);
		}
	}
}
