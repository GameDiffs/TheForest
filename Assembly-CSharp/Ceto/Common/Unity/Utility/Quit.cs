using System;
using UnityEngine;

namespace Ceto.Common.Unity.Utility
{
	public class Quit : MonoBehaviour
	{
		public KeyCode quitKey = KeyCode.Escape;

		private void OnGUI()
		{
			if (Input.GetKeyDown(this.quitKey))
			{
				Application.Quit();
			}
		}
	}
}
