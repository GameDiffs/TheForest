using System;
using TheForest.Items.Core;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Buildings.World
{
	[DoNotSerializePublic, AddComponentMenu("Buildings/World/Item Stash")]
	public class ItemStash : ItemStorage
	{
		public GameObject _billboardSheen;

		public GameObject _billboardPickup;

		private void Update()
		{
			if (TheForest.Utils.Input.GetButtonDown("Take"))
			{
				LocalPlayer.Inventory.Open(this);
				base.enabled = false;
			}
		}

		private void GrabEnter()
		{
			base.enabled = true;
			this._billboardSheen.SetActive(false);
			this._billboardPickup.SetActive(true);
		}

		private void GrabExit()
		{
			base.enabled = false;
			this._billboardPickup.SetActive(false);
			this._billboardSheen.SetActive(true);
		}

		public override void Close()
		{
			base.Close();
			base.GetComponent<Collider>().enabled = false;
			base.GetComponent<Collider>().enabled = true;
		}
	}
}
