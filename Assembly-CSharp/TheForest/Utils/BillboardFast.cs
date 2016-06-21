using System;
using UnityEngine;

namespace TheForest.Utils
{
	public class BillboardFast : MonoBehaviour
	{
		private void Update()
		{
			base.transform.rotation = LocalPlayer.MainCamTr.rotation;
		}
	}
}
