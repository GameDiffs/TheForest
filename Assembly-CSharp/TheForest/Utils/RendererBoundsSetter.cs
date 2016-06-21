using System;
using UnityEngine;

namespace TheForest.Utils
{
	public class RendererBoundsSetter : MonoBehaviour
	{
		public Vector3 _boundSize;

		private void Reset()
		{
			MeshFilter component = base.GetComponent<MeshFilter>();
			if (component)
			{
				this._boundSize = component.sharedMesh.bounds.size * 10f;
			}
		}

		private void Awake()
		{
			MeshFilter component = base.GetComponent<MeshFilter>();
			if (component)
			{
				component.mesh.bounds = new Bounds(Vector3.zero, this._boundSize);
			}
		}
	}
}
