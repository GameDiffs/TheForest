using System;
using UnityEngine;

namespace TheForest.Utils
{
	public class CullingGridRenderer : MonoBehaviour
	{
		public Renderer _renderer;

		private int _token;

		private void OnEnable()
		{
			this._token = CullingGrid.Register(this._renderer);
		}

		private void OnDisable()
		{
			CullingGrid.Unregister(this._renderer, this._token);
		}
	}
}
