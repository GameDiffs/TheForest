using Bolt;
using System;
using TheForest.Items;
using TheForest.Utils;
using UniLinq;
using UnityEngine;

namespace TheForest.Buildings.Creation
{
	public class TrophyMaker : EntityBehaviour
	{
		[ItemIdPicker]
		public int[] _itemIdWhiteList;

		public GameObject _sheenBillboard;

		public GameObject _pickupBillboard;

		private void Awake()
		{
			base.enabled = false;
		}

		private void Update()
		{
			bool flag = LocalPlayer.Inventory.RightHand && this._itemIdWhiteList.Contains(LocalPlayer.Inventory.RightHand._itemId);
			if (flag)
			{
				this._sheenBillboard.SetActive(false);
				this._pickupBillboard.SetActive(true);
				int itemId = LocalPlayer.Inventory.RightHand._itemId;
				if (TheForest.Utils.Input.GetButtonAfterDelay("Take", 0.5f) && LocalPlayer.Inventory.RemoveItem(itemId, 1, false))
				{
					LocalPlayer.Sfx.PlayTwinkle();
					if (BoltNetwork.isRunning)
					{
						PlaceTrophy placeTrophy = global::PlaceTrophy.Create(GlobalTargets.OnlyServer);
						placeTrophy.Maker = this.entity;
						placeTrophy.ItemId = itemId;
						placeTrophy.Send();
					}
					else
					{
						this.PlaceTrophy(itemId);
					}
				}
			}
			else
			{
				this._sheenBillboard.SetActive(true);
				this._pickupBillboard.SetActive(false);
				base.enabled = false;
			}
		}

		private void GrabEnter()
		{
			base.enabled = (LocalPlayer.Inventory.RightHand && this._itemIdWhiteList.Contains(LocalPlayer.Inventory.RightHand._itemId));
		}

		private void GrabExit()
		{
			this._sheenBillboard.SetActive(true);
			this._pickupBillboard.SetActive(false);
			base.enabled = false;
		}

		public void PlaceTrophy(int itemId)
		{
			Transform transform = (Transform)UnityEngine.Object.Instantiate(Prefabs.Instance.TrophyPrefabs.First((Prefabs.TrophyPrefab tp) => tp._itemId == itemId)._prefab, base.transform.parent.position, base.transform.parent.rotation);
			transform.parent = base.transform.parent.parent;
			if (BoltNetwork.isRunning)
			{
				BoltNetwork.Attach(transform.gameObject);
			}
			UnityEngine.Object.Destroy(base.transform.parent.gameObject);
		}
	}
}
