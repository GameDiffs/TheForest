using System;
using UnityEngine;

namespace TheForest.Networking.UI
{
	public class KickedOrBannedMessage : MonoBehaviour
	{
		public GameObject _root;

		public UILabel _label;

		private void Awake()
		{
			if (!string.IsNullOrEmpty(CoopKick.Client_KickMessage))
			{
				Debug.Log("MP Kick Message: " + CoopKick.Client_KickMessage);
				this._label.text = CoopKick.Client_KickMessage;
				this._root.SetActive(true);
				CoopKick.Client_KickMessage = string.Empty;
			}
		}

		public void Close()
		{
			this._root.SetActive(false);
		}
	}
}
