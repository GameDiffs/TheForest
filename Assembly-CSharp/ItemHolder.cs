using Bolt;
using System;
using TheForest.Items;
using TheForest.Items.Craft;
using TheForest.Utils;
using UnityEngine;

[DoNotSerializePublic]
public class ItemHolder : EntityEventListener<IItemHolderState>
{
	[ItemIdPicker]
	public int _itemid;

	public GameObject[] ItemsRender;

	public GameObject TakeIcon;

	public GameObject AddIcon;

	[Header("FMOD")]
	public string addItemEvent;

	[SerializeThis]
	public int Items;

	private bool hasPreloaded;

	private void Awake()
	{
		base.enabled = false;
	}

	private void OnEnable()
	{
		FMODCommon.PreloadEvents(new string[]
		{
			this.addItemEvent
		});
		this.hasPreloaded = true;
	}

	private void OnDisable()
	{
		if (this.hasPreloaded)
		{
			FMODCommon.UnloadEvents(new string[]
			{
				this.addItemEvent
			});
			this.hasPreloaded = false;
		}
	}

	private void Update()
	{
		if (BoltNetwork.isServer)
		{
			base.state.ItemCount = this.Items;
		}
		if (BoltNetwork.isClient)
		{
			this.Items = base.state.ItemCount;
		}
		if (this.Items > 0)
		{
			this.TakeIcon.SetActive(true);
			if (TheForest.Utils.Input.GetButtonDown("Take") && LocalPlayer.Inventory.AddItem(this._itemid, 1, false, false, (WeaponStatUpgrade.Types)(-2)))
			{
				LocalPlayer.Sfx.PlayWhoosh();
				if (BoltNetwork.isRunning)
				{
					ItemHolderTakeItem itemHolderTakeItem = ItemHolderTakeItem.Create(GlobalTargets.OnlyServer);
					itemHolderTakeItem.Target = this.entity;
					itemHolderTakeItem.Player = LocalPlayer.Entity;
					itemHolderTakeItem.Send();
				}
				else
				{
					this.ItemsRender[this.Items - 1].SetActive(false);
					this.Items--;
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
				if (this.addItemEvent.Length > 0)
				{
					FMODCommon.PlayOneshot(this.addItemEvent, base.transform);
				}
				else
				{
					LocalPlayer.Sfx.PlayPutDown(base.gameObject);
				}
				if (LocalPlayer.Inventory.RemoveItem(this._itemid, 1, false))
				{
					if (BoltNetwork.isRunning)
					{
						ItemHolderAddItem itemHolderAddItem = ItemHolderAddItem.Create(GlobalTargets.OnlyServer);
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

	private void OnDeserialized()
	{
		if (!BoltNetwork.isClient)
		{
			for (int i = 0; i < this.ItemsRender.Length; i++)
			{
				GameObject gameObject = this.ItemsRender[i];
				bool flag = i < this.Items;
				if (gameObject.activeSelf != flag)
				{
					gameObject.SetActive(flag);
				}
			}
		}
	}

	private void GrabEnter()
	{
		base.enabled = (!BoltNetwork.isRunning || this.entity.isAttached);
	}

	private void GrabExit()
	{
		base.enabled = false;
		this.AddIcon.SetActive(false);
		this.TakeIcon.SetActive(false);
	}

	public override void Attached()
	{
		base.state.AddCallback("ItemCount", new PropertyCallbackSimple(this.ItemCountChangedMP));
		if (BoltNetwork.isServer)
		{
			base.state.ItemCount = this.Items;
		}
	}

	public void TakeItemMP(BoltEntity targetPlayer)
	{
		if (this.Items > 0)
		{
			this.entity.Freeze(false);
			base.state.ItemCount = (this.Items = Mathf.Max(0, this.Items - 1));
		}
		else
		{
			ItemRemoveFromPlayer itemRemoveFromPlayer;
			if (targetPlayer.isOwner)
			{
				itemRemoveFromPlayer = ItemRemoveFromPlayer.Create(GlobalTargets.OnlySelf);
			}
			else
			{
				itemRemoveFromPlayer = ItemRemoveFromPlayer.Create(targetPlayer.source);
			}
			itemRemoveFromPlayer.ItemId = this._itemid;
			itemRemoveFromPlayer.Send();
		}
	}

	public void AddItemMP()
	{
		if (this.Items < this.ItemsRender.Length)
		{
			base.state.ItemCount = (this.Items = Mathf.Min(this.Items + 1, this.ItemsRender.Length));
			this.entity.Freeze(false);
		}
	}

	private void ItemCountChangedMP()
	{
		if (BoltNetwork.isClient)
		{
			this.Items = base.state.ItemCount;
		}
		for (int i = 0; i < this.ItemsRender.Length; i++)
		{
			this.ItemsRender[i].SetActive(false);
		}
		for (int j = 0; j < this.Items; j++)
		{
			this.ItemsRender[j].SetActive(true);
		}
	}

	public void forceRemoveItem()
	{
		this.ItemsRender[this.Items - 1].SetActive(false);
		this.Items--;
		if (this.Items > 0)
		{
			LocalPlayer.Sfx.PlayWhoosh();
		}
	}
}
