using System;
using UnityEngine;

namespace Ceto.Common.Unity.Utility
{
	public class Wireframe : MonoBehaviour
	{
		public bool on;

		public KeyCode toggleKey = KeyCode.F2;

		private void Start()
		{
		}

		private void Update()
		{
			if (Input.GetKeyDown(this.toggleKey))
			{
				this.on = !this.on;
			}
		}

		private void OnPreRender()
		{
			if (this.on)
			{
				GL.wireframe = true;
			}
		}

		private void OnPostRender()
		{
			if (this.on)
			{
				GL.wireframe = false;
			}
		}
	}
}
