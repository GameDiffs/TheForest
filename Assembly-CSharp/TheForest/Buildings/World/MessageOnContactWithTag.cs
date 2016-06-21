using System;
using UnityEngine;

namespace TheForest.Buildings.World
{
	public class MessageOnContactWithTag : DestroyOnContactWithTag
	{
		public string _message;

		public GameObject _messageTarget;

		public override void Perform(bool multiplayer)
		{
			this._messageTarget.SendMessage(this._message, SendMessageOptions.DontRequireReceiver);
		}
	}
}
