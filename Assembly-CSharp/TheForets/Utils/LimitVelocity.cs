using System;
using UnityEngine;

namespace TheForets.Utils
{
	public class LimitVelocity : MonoBehaviour
	{
		public float _maxVelocity = 7f;

		public float _maxAngularVelocity = 1f;

		private Rigidbody _rb;

		private void Awake()
		{
			this._rb = base.GetComponent<Rigidbody>();
			if (this._rb)
			{
				this._rb.maxAngularVelocity = this._maxAngularVelocity;
			}
			else
			{
				UnityEngine.Object.Destroy(this);
			}
		}

		private void Update()
		{
			if (this._rb)
			{
				if (this._rb.velocity.sqrMagnitude > this._maxVelocity * this._maxVelocity)
				{
					this._rb.velocity = this._rb.velocity.normalized * this._maxVelocity;
				}
			}
			else
			{
				UnityEngine.Object.Destroy(this);
			}
		}
	}
}
