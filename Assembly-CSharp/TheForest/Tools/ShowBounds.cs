using System;
using UnityEngine;

namespace TheForest.Tools
{
	public class ShowBounds : MonoBehaviour
	{
		public Bounds b;

		private void OnDrawGizmos()
		{
			Renderer component = base.GetComponent<Renderer>();
			if (component)
			{
				this.b = component.bounds;
				Vector3 vector = default(Vector3);
				vector.x = (component.bounds.max.x - component.bounds.min.x) / base.transform.localScale.x;
				vector.y = (component.bounds.max.y - component.bounds.min.y) / base.transform.localScale.y;
				vector.z = (component.bounds.max.z - component.bounds.min.z) / base.transform.localScale.z;
				vector = component.bounds.size;
				Gizmos.color = Color.white;
				Gizmos.DrawWireCube(component.bounds.center, component.bounds.size);
				Gizmos.color = Color.red;
				Gizmos.DrawWireCube(component.bounds.center, base.transform.parent.InverseTransformDirection(vector));
				Gizmos.color = Color.blue;
				Gizmos.DrawWireCube(component.bounds.center, base.transform.parent.InverseTransformVector(vector) * 0.99f);
				Gizmos.color = Color.magenta;
				Gizmos.DrawWireCube(component.bounds.center, base.transform.parent.TransformDirection(vector) * 0.98f);
				Gizmos.color = Color.cyan;
				Gizmos.DrawWireCube(component.bounds.center, base.transform.parent.TransformVector(vector) * 0.97f);
			}
		}
	}
}
