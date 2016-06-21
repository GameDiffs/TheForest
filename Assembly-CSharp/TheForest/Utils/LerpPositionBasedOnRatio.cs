using System;
using UnityEngine;

namespace TheForest.Utils
{
	public class LerpPositionBasedOnRatio : MonoBehaviour
	{
		public Transform _from;

		public Transform _to;

		public float _fromAspectRatio = 1.25f;

		public float _toAspectRatio = 1.7f;

		private void OnEnable()
		{
			float num = (float)Screen.width / (float)Screen.height;
			float t = (num - this._fromAspectRatio) / (this._toAspectRatio - this._fromAspectRatio);
			base.transform.position = Vector3.Lerp(this._from.position, this._to.position, t);
		}
	}
}
