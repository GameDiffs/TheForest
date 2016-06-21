using System;
using UnityEngine;

namespace CetoTF
{
	public class OceanQualityTrigger : MonoBehaviour
	{
		public float width = 1f;

		public float height = 1f;

		public float length = 1f;

		private void Start()
		{
		}

		public bool Contains(Vector3 point)
		{
			Vector3 a = new Vector3(this.width, this.height, this.length);
			Vector3 position = base.transform.position;
			float y = base.transform.eulerAngles.y;
			if (point.y < position.y - a.y || point.y > position.y + a.y)
			{
				return false;
			}
			float num = point.x - position.x;
			float num2 = point.z - position.z;
			float num3 = Mathf.Cos(y * 3.14159274f / 180f);
			float num4 = Mathf.Sin(y * 3.14159274f / 180f);
			float num5 = num * num3 - num2 * num4;
			float num6 = num * num4 + num2 * num3;
			Vector3 vector = a * 0.5f;
			return num5 > -vector.x && num5 < vector.x && num6 > -vector.z && num6 < vector.z;
		}

		private void OnDrawGizmos()
		{
			if (!base.enabled)
			{
				return;
			}
			Vector3 s = new Vector3(this.width, this.height, this.length);
			Vector3 position = base.transform.position;
			Matrix4x4 matrix = Matrix4x4.TRS(new Vector3(position.x, 0f, position.z), Quaternion.Euler(0f, base.transform.eulerAngles.y, 0f), s);
			Gizmos.color = Color.yellow;
			Gizmos.matrix = matrix;
			Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
		}
	}
}
