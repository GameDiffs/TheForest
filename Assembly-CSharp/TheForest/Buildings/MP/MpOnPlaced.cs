using System;
using UnityEngine;

namespace TheForest.Buildings.MP
{
	[AddComponentMenu("Buildings/MP/Auto OnPlaced")]
	public class MpOnPlaced : MonoBehaviour
	{
		private void Start()
		{
			if (BoltNetwork.isClient && base.transform.parent == null)
			{
				base.SendMessage("OnPlaced");
			}
			UnityEngine.Object.Destroy(this);
		}
	}
}
