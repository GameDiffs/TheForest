using System;
using UnityEngine;

namespace Ceto
{
	[RequireComponent(typeof(Camera))]
	public class AddRenderTarget : MonoBehaviour
	{
		public int scale = 2;

		private void Start()
		{
			Camera component = base.GetComponent<Camera>();
			component.targetTexture = new RenderTexture(Screen.width / this.scale, Screen.height / this.scale, 24);
		}

		private void OnGUI()
		{
			Camera component = base.GetComponent<Camera>();
			if (component.targetTexture == null)
			{
				return;
			}
			int width = component.targetTexture.width;
			int height = component.targetTexture.height;
			GUI.DrawTexture(new Rect(10f, 10f, (float)width, (float)height), component.targetTexture);
		}
	}
}
