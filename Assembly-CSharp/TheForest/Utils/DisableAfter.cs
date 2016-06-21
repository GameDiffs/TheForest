using System;
using UnityEngine;

namespace TheForest.Utils
{
	public class DisableAfter : MonoBehaviour
	{
		public float _delay;

		private void OnEnable()
		{
			base.Invoke("DisableNow", this._delay);
		}

		private void DisableNow()
		{
			base.gameObject.SetActive(false);
		}
	}
}
