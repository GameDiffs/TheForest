using Bolt;
using System;
using TheForest.Items;
using TheForest.Items.Craft;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Buildings.World
{
	[DoNotSerializePublic, AddComponentMenu("Buildings/World/Item Cupboard")]
	public class ItemCupboard : EntityEventListener<IItemHolderState>
	{
		[ItemIdPicker]
		public int _itemid;

		public GameObject[] ItemsRender;

		public GameObject TakeIcon;

		public GameObject AddIcon;

		[SerializeThis]
		private int Items;

		private void Awake()
		{
			base.enabled = false;
		}

		private void Update()
		{
			if (this.Items > 0)
			{
				this.TakeIcon.SetActive(true);
				if (TheForest.Utils.Input.GetButtonDown("Take"))
				{
					LocalPlayer.Inventory.SendMessage("PlayWhoosh");
					if (LocalPlayer.Inventory.AddItem(this._itemid, 1, false, false, (WeaponStatUpgrade.Types)(-2)))
					{
						if (BoltNetwork.isRunning)
						{
							ItemHolderTakeItem itemHolderTakeItem = ItemHolderTakeItem.Raise(GlobalTargets.OnlyServer);
							itemHolderTakeItem.Target = this.entity;
							itemHolderTakeItem.Send();
						}
						else
						{
							this.ItemsRender[this.Items - 1].SetActive(false);
							this.Items--;
						}
					}
				}
			}
			else if (this.TakeIcon.activeSelf)
			{
				this.TakeIcon.SetActive(false);
			}
			if (this.Items < this.ItemsRender.Length && LocalPlayer.Inventory.Owns(this._itemid))
			{
				this.AddIcon.SetActive(true);
				if (TheForest.Utils.Input.GetButtonDown("Craft"))
				{
					if (base.GetComponent<AudioSource>())
					{
						base.GetComponent<AudioSource>().Play();
					}
					else
					{
						LocalPlayer.Sfx.PlayWhoosh();
					}
					if (LocalPlayer.Inventory.RemoveItem(this._itemid, 1, false))
					{
						if (BoltNetwork.isRunning)
						{
							ItemHolderAddItem itemHolderAddItem = ItemHolderAddItem.Raise(GlobalTargets.OnlyServer);
							itemHolderAddItem.Target = this.entity;
							itemHolderAddItem.Send();
						}
						else
						{
							this.Items++;
							this.ItemsRender[this.Items - 1].SetActive(true);
						}
					}
				}
			}
			else
			{
				this.AddIcon.SetActive(false);
			}
		}

		private void GrabEnter()
		{
			base.enabled = true;
		}

		private void GrabExit()
		{
			base.enabled = false;
		}

		public override void Attached()
		{
			base.state.AddCallback("ItemCount", new PropertyCallbackSimple(this.ItemCountChangedMP));
			if (BoltNetwork.isServer)
			{
				base.state.ItemCount = this.Items;
			}
		}

		public void TakeItemMP()
		{
			base.state.ItemCount = Mathf.Max(base.state.ItemCount - 1, 0);
		}

		public void AddItemMP()
		{
			base.state.ItemCount = Mathf.Min(base.state.ItemCount + 1, this.ItemsRender.Length);
		}

		private void ItemCountChangedMP()
		{
			this.Items = base.state.ItemCount;
			for (int i = 0; i < this.ItemsRender.Length; i++)
			{
				this.ItemsRender[i].SetActive(false);
			}
			for (int j = 0; j < this.Items; j++)
			{
				this.ItemsRender[j].SetActive(true);
			}
		}
	}
}
