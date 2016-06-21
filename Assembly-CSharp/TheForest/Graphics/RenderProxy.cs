using System;
using UnityEngine;

namespace TheForest.Graphics
{
	[AddComponentMenu("The Forest/Graphics/RenderProxy"), ExecuteInEditMode]
	public class RenderProxy : MonoBehaviour
	{
		private int frame;

		private static int globalFrame;

		private void OnWillRenderObject()
		{
			this.frame++;
			if (this.frame == RenderProxy.globalFrame)
			{
				return;
			}
			RenderProxy.globalFrame = this.frame;
			if (WaterEngine.RenderCameras != null)
			{
				WaterEngine.RenderCameras();
			}
		}
	}
}
