using System;
using UnityEngine;

namespace TheForest.Utils
{
	public class PooledTransformReset : MonoBehaviour
	{
		private Vector3 _localPosition;

		private Quaternion _localRotation;

		private void Awake()
		{
			this._localPosition = base.transform.localPosition;
			this._localRotation = base.transform.localRotation;
		}

		private void OnSpawned()
		{
			base.transform.localPosition = this._localPosition;
			base.transform.localRotation = this._localRotation;
		}
	}
}
