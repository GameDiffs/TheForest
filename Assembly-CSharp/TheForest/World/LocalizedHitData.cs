using System;
using UnityEngine;

namespace TheForest.World
{
	public struct LocalizedHitData
	{
		public Vector3 _position;

		public float _damage;

		public float _distortRatio;

		public LocalizedHitData(Vector3 position, float damage)
		{
			this._position = position;
			this._damage = damage;
			this._distortRatio = 1f;
		}

		public LocalizedHitData(Vector3 position, float damage, float distortRatio)
		{
			this._position = position;
			this._damage = damage;
			this._distortRatio = distortRatio;
		}
	}
}
