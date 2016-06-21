using System;
using UnityEngine;

namespace Ceto.Common.Unity.Utility
{
	public class RotateLight : MonoBehaviour
	{
		public float speed = 50f;

		public Vector3 axis = new Vector3(1f, 0f, 0f);

		public KeyCode decrementKey = KeyCode.KeypadMinus;

		public KeyCode incrementKey = KeyCode.KeypadPlus;

		private void Start()
		{
		}

		private void Update()
		{
			float num = Time.deltaTime * this.speed;
			Vector3 eulerAngles = new Vector3(num, num, num);
			if (Input.GetKey(this.decrementKey))
			{
				eulerAngles.x *= -this.axis.x;
				eulerAngles.y *= -this.axis.y;
				eulerAngles.z *= -this.axis.z;
				base.transform.Rotate(eulerAngles);
			}
			if (Input.GetKey(this.incrementKey))
			{
				eulerAngles.x *= this.axis.x;
				eulerAngles.y *= this.axis.y;
				eulerAngles.z *= this.axis.z;
				base.transform.Rotate(eulerAngles);
			}
		}
	}
}
