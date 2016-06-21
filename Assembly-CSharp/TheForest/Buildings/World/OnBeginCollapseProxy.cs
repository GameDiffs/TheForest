using System;
using UnityEngine;

namespace TheForest.Buildings.World
{
	public class OnBeginCollapseProxy : MonoBehaviour
	{
		public GameObject _target;

		private void OnBeginCollapse()
		{
			if (this._target)
			{
				this._target.SendMessage("OnBeginCollapse", SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}
