using System;
using UnityEngine;

namespace TheForest.Utils
{
	public class GamepadToggleGo : MonoBehaviour
	{
		public GameObject _gamepadGo;

		public GameObject _otherGo;

		private void Start()
		{
			this.Reset();
		}

		private void Update()
		{
			if (Input.IsGamePad != this._gamepadGo.activeSelf)
			{
				this.Reset();
			}
		}

		private void Reset()
		{
			this._gamepadGo.SetActive(Input.IsGamePad);
			if (this._otherGo)
			{
				this._otherGo.SetActive(!Input.IsGamePad);
			}
		}
	}
}
