using System;
using UnityEngine;

namespace TheForest.Tools
{
	public class SendMessageTo : MonoBehaviour
	{
		public GameObject _target;

		public string _message;

		private void Start()
		{
			this._target.SendMessage(this._message);
		}
	}
}
