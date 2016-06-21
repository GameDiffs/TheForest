using System;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Networking
{
	public class HostAltTabTutMP : MonoBehaviour
	{
		private void Awake()
		{
			if (!BoltNetwork.isServer)
			{
				UnityEngine.Object.Destroy(this);
			}
		}

		private void OnApplicationPause(bool pauseStatus)
		{
			if (pauseStatus && LocalPlayer.Tuts && Screen.fullScreen)
			{
				LocalPlayer.Tuts.ShowMPHostAltTabTut();
			}
		}

		private void OnApplicationFocus(bool pauseStatus)
		{
			if (pauseStatus && LocalPlayer.Tuts && Screen.fullScreen)
			{
				LocalPlayer.Tuts.ShowMPHostAltTabTut();
			}
		}
	}
}
