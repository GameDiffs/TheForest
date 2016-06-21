using System;
using UnityEngine;

namespace TheForest.Utils
{
	[Serializable]
	public class SerializableTimer
	{
		public float _startTime;

		[SerializeThis]
		public float _doneTime;

		public void Start()
		{
			if (!LevelSerializer.IsDeserializing)
			{
				this._startTime = Time.time;
				this._doneTime = 0f;
			}
		}

		public void OnSerializing()
		{
			this._doneTime += Time.time - this._startTime;
			this._startTime = Time.time;
		}

		public void OnDeserialized()
		{
			this._startTime = Time.time;
		}
	}
}
