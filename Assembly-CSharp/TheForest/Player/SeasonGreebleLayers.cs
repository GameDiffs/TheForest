using System;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Player
{
	public class SeasonGreebleLayers : MonoBehaviour
	{
		public GameObject _standard;

		public GameObject _snow;

		private void Awake()
		{
			base.enabled = false;
			base.Invoke("Activate", 0.5f);
		}

		private void Update()
		{
			if (LocalPlayer.Inventory.enabled)
			{
				bool flag = LocalPlayer.Stats.IsInNorthColdArea();
				if (this._standard.activeSelf == flag)
				{
					this._standard.SetActive(!flag);
				}
				if (this._snow.activeSelf != flag)
				{
					this._snow.SetActive(flag);
				}
			}
		}

		private void Activate()
		{
			base.enabled = true;
		}
	}
}
