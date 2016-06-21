using System;
using UnityEngine;

namespace Rewired.Demos
{
	[AddComponentMenu("")]
	public class Bullet : MonoBehaviour
	{
		public float lifeTime = 3f;

		private bool die;

		private float deathTime;

		private void Start()
		{
			if (this.lifeTime > 0f)
			{
				this.deathTime = Time.time + this.lifeTime;
				this.die = true;
			}
		}

		private void Update()
		{
			if (this.die && Time.time >= this.deathTime)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}
}
