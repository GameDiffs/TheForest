using System;
using UnityEngine;

namespace TheForest.Utils
{
	[DoNotSerializePublic]
	public class OnEnableProxy : MonoBehaviour
	{
		public MonoBehaviour _todo;

		private void OnEnable()
		{
			this._todo.SendMessage("OnEnable");
		}
	}
}
