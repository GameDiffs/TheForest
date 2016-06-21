using System;
using UnityEngine;

namespace TheForest.Utils
{
	public class PlayerTrigger : MonoBehaviour
	{
		public string _message;

		public GameObject _target;

		public bool _destroyAfter;

		private void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.CompareTag("Player"))
			{
				this._target.SendMessage(this._message);
				if (this._destroyAfter)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
		}
	}
}
