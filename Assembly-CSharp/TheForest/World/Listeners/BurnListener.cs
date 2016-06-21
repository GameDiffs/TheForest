using System;
using UnityEngine;

namespace TheForest.World.Listeners
{
	public class BurnListener : MonoBehaviour
	{
		public string _message;

		public GameObject _target;

		private void Burn()
		{
			if (this._target)
			{
				this._target.SendMessage(this._message, SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				base.SendMessage(this._message, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}
