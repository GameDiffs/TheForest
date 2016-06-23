using Bolt;
using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Items;
using TheForest.Items.Craft;
using TheForest.Items.Inventory;
using TheForest.Items.World;
using TheForest.Utils;
using UniLinq;
using UnityEngine;
using UnityEngine.Events;

namespace TheForest.Buildings.World
{
	[DoNotSerializePublic, AddComponentMenu("Buildings/World/Weapon Rack Slot")]
	public class WeaponRackSlot : MonoBehaviour
	{
		[Serializable]
		public class OnItemChanged : UnityEvent<int>
		{
		}

		public enum FillMode
		{
			EquipedInRightHand,
			AutoFromInventory
		}

		public enum PositionningSource
		{
			Held,
			Inventory
		}

		public WeaponRackSlot.FillMode _fillMode;

		public WeaponRackSlot.PositionningSource _positionningSource;

		[EnumFlags]
		public Item.Types _acceptedItemTypes = (Item.Types)(-1);

		[EnumFlags]
		public Item.Types _forbidenItemTypes = Item.Types.Projectile;

		[ItemIdPicker]
		public int[] _itemWhiteList;

		public GameObject _billboardAdd;

		public GameObject _billboardRemove;

		public bool _offsetIcons;

		[SerializeThis]
		private int _storedItemId = -1;

		private GameObject _storedItemGO;

		private readonly Vector3 IconsOffset = Vector3.down * 0.5f;

		public bool _added;

		public bool _removed;

		public WeaponRackSlot.OnItemChanged OnItemAdded;

		public WeaponRackSlot.OnItemChanged OnItemRemoved;

		public bool hellDoorSlot;

		public int StoredItemId
		{
			get
			{
				return this._storedItemId;
			}
		}

		private void Start()
		{
			base.enabled = false;
		}

		private void Update()
		{
			if (!this._storedItemGO)
			{
				if (TheForest.Utils.Input.GetButtonDown("Craft"))
				{
					if (this._fillMode == WeaponRackSlot.FillMode.EquipedInRightHand)
					{
						this.PlaceEquipment();
					}
					else
					{
						this.PlaceNonEquipment();
					}
				}
			}
			else if (TheForest.Utils.Input.GetButtonDown("Take"))
			{
				this.TakeCurrentItem();
			}
		}

		[DebuggerHidden]
		private IEnumerator OnDeserialized()
		{
			WeaponRackSlot.<OnDeserialized>c__Iterator15A <OnDeserialized>c__Iterator15A = new WeaponRackSlot.<OnDeserialized>c__Iterator15A();
			<OnDeserialized>c__Iterator15A.<>f__this = this;
			return <OnDeserialized>c__Iterator15A;
		}

		private void GrabEnter()
		{
			if (this._storedItemGO)
			{
				if (LocalPlayer.Inventory.AmountOf(this._storedItemId, false) < ItemDatabase.ItemById(this._storedItemId)._maxAmount)
				{
					this._billboardRemove.transform.position = base.transform.position;
					this._billboardRemove.SetActive(true);
					base.enabled = true;
				}
			}
			else
			{
				if (this._fillMode != WeaponRackSlot.FillMode.EquipedInRightHand || LocalPlayer.Inventory.IsRightHandEmpty() || !this.IsValidItem(LocalPlayer.Inventory.RightHand.ItemCache))
				{
					if (this._fillMode != WeaponRackSlot.FillMode.AutoFromInventory)
					{
						return;
					}
					if (!this._itemWhiteList.Any((int wli) => LocalPlayer.Inventory._possessedItems.Any((InventoryItem pi) => pi._itemId == wli)))
					{
						return;
					}
				}
				if (!this._offsetIcons)
				{
					this._billboardAdd.transform.position = base.transform.position;
				}
				else
				{
					this._billboardAdd.transform.position = base.transform.position + this.IconsOffset;
				}
				this._billboardAdd.SetActive(true);
				base.enabled = true;
			}
		}

		private void GrabExit()
		{
			base.enabled = false;
			this._billboardAdd.SetActive(false);
			this._billboardRemove.SetActive(false);
		}

		private void PlaceEquipment()
		{
			InventoryItemView inventoryItemView = LocalPlayer.Inventory.IsRightHandEmpty() ? null : LocalPlayer.Inventory.RightHand;
			if (inventoryItemView != null && this.IsValidItem(inventoryItemView.ItemCache))
			{
				LocalPlayer.Inventory.ShuffleRemoveRightHandItem();
				if (BoltNetwork.isRunning && !this.hellDoorSlot)
				{
					RackAdd rackAdd = RackAdd.Create(GlobalTargets.OnlyServer);
					rackAdd.Slot = base.GetComponentInParent<CoopRack>().GetSlotIndex(this);
					rackAdd.Rack = base.GetComponentInParent<BoltEntity>();
					rackAdd.ItemId = inventoryItemView._itemId;
					rackAdd.Send();
				}
				else
				{
					if (this.hellDoorSlot)
					{
						this._removed = false;
						this._added = true;
					}
					this._storedItemId = inventoryItemView._itemId;
					this.SpawnItemView();
				}
				LocalPlayer.Sfx.PlayPutDown(base.gameObject);
				this._billboardAdd.SetActive(false);
				if (!this._offsetIcons)
				{
					this._billboardAdd.transform.position = base.transform.position;
				}
				else
				{
					this._billboardRemove.transform.position = base.transform.position + this.IconsOffset;
				}
				this._billboardRemove.SetActive(true);
			}
		}

		private void SpawnItemView()
		{
			if (this._positionningSource == WeaponRackSlot.PositionningSource.Held)
			{
				this.SpawnEquipmentItemView();
			}
			else
			{
				this.SpawnNonEquipmentItemView();
			}
		}

		private void SpawnEquipmentItemView()
		{
			InventoryItemView inventoryItemView = LocalPlayer.Inventory.InventoryItemViewsCache[this._storedItemId][0];
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(inventoryItemView._held);
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = inventoryItemView._held.transform.localPosition;
			gameObject.transform.localRotation = inventoryItemView._held.transform.localRotation;
			gameObject.layer = base.gameObject.layer;
			gameObject.SetActive(true);
			foreach (Transform transform in gameObject.transform)
			{
				if (transform.name == "collide" || transform.GetComponent<BurnableCloth>())
				{
					UnityEngine.Object.Destroy(transform.gameObject);
				}
			}
			MonoBehaviour[] components = gameObject.GetComponents<MonoBehaviour>();
			for (int i = 0; i < components.Length; i++)
			{
				MonoBehaviour obj = components[i];
				UnityEngine.Object.Destroy(obj);
			}
			this._storedItemGO = gameObject;
			this.OnItemAdded.Invoke(this._storedItemId);
		}

		private void PlaceNonEquipment()
		{
			int num = this._itemWhiteList.FirstOrDefault((int wli) => LocalPlayer.Inventory._possessedItems.Any((InventoryItem pi) => pi._itemId == wli));
			if (num > 0 && LocalPlayer.Inventory.RemoveItem(num, 1, false))
			{
				if (BoltNetwork.isRunning)
				{
					RackAdd rackAdd = RackAdd.Create(GlobalTargets.OnlyServer);
					rackAdd.Slot = base.GetComponentInParent<CoopRack>().GetSlotIndex(this);
					rackAdd.Rack = base.GetComponentInParent<BoltEntity>();
					rackAdd.ItemId = num;
					rackAdd.Send();
				}
				else
				{
					this._storedItemId = num;
					this.SpawnItemView();
				}
				LocalPlayer.Sfx.PlayPutDown(base.gameObject);
				this._billboardAdd.SetActive(false);
				if (!this._offsetIcons)
				{
					this._billboardAdd.transform.position = base.transform.position;
				}
				else
				{
					this._billboardRemove.transform.position = base.transform.position + this.IconsOffset;
				}
				this._billboardRemove.SetActive(true);
			}
		}

		private void SpawnNonEquipmentItemView()
		{
			InventoryItemView inventoryItemView = LocalPlayer.Inventory.InventoryItemViewsCache[this._storedItemId][0];
			InventoryItemView inventoryItemView2 = UnityEngine.Object.Instantiate<InventoryItemView>(inventoryItemView);
			Vector3 position = base.transform.position;
			position.y += inventoryItemView.transform.position.y - LocalPlayer.Inventory._inventoryGO.transform.position.y;
			inventoryItemView2.transform.localScale = inventoryItemView.transform.lossyScale;
			inventoryItemView2.transform.parent = base.transform;
			inventoryItemView2.transform.position = position;
			inventoryItemView2.transform.rotation = inventoryItemView.transform.rotation;
			inventoryItemView2.gameObject.layer = base.gameObject.layer;
			inventoryItemView2.gameObject.SetActive(true);
			this._storedItemGO = inventoryItemView2.gameObject;
			UnityEngine.Object.Destroy(inventoryItemView2.GetComponent<Collider>());
			UnityEngine.Object.Destroy(inventoryItemView2);
			this.OnItemAdded.Invoke(this._storedItemId);
		}

		private void TakeCurrentItem()
		{
			if (this._fillMode == WeaponRackSlot.FillMode.EquipedInRightHand)
			{
				LocalPlayer.Inventory.MemorizeItem(Item.EquipmentSlot.RightHand);
				if (!LocalPlayer.Inventory.Equip(this._storedItemId, true))
				{
					return;
				}
			}
			else if (!LocalPlayer.Inventory.AddItem(this._storedItemId, 1, false, false, (WeaponStatUpgrade.Types)(-2)))
			{
				return;
			}
			UnityEngine.Object.Destroy(this._storedItemGO);
			this._storedItemGO = null;
			this._storedItemId = -1;
			if (this.hellDoorSlot)
			{
				this._removed = true;
				this._added = false;
			}
			this.OnItemRemoved.Invoke(this._storedItemId);
			if (BoltNetwork.isRunning && !this.hellDoorSlot)
			{
				RackRemove rackRemove = RackRemove.Create(GlobalTargets.OnlyServer);
				rackRemove.Slot = base.GetComponentInParent<CoopRack>().GetSlotIndex(this);
				rackRemove.Rack = base.GetComponentInParent<BoltEntity>();
				rackRemove.Send();
			}
			if (!this._offsetIcons)
			{
				this._billboardAdd.transform.position = base.transform.position;
			}
			else
			{
				this._billboardAdd.transform.position = base.transform.position + this.IconsOffset;
			}
			this._billboardAdd.SetActive(true);
			this._billboardRemove.SetActive(false);
		}

		private bool IsValidItem(Item item)
		{
			return item.MatchType(this._acceptedItemTypes) && (this._forbidenItemTypes == (Item.Types)0 || !item.MatchType(this._forbidenItemTypes)) && item._maxAmount >= 0 && (this._itemWhiteList.Length == 0 || this._itemWhiteList.Contains(item._id));
		}

		public void ItemIdChanged_Network(int newItemId)
		{
			if (this._storedItemId == newItemId)
			{
				return;
			}
			this._storedItemId = newItemId;
			if (newItemId > 0)
			{
				this.SpawnItemView();
			}
			else if (this._storedItemGO)
			{
				UnityEngine.Object.Destroy(this._storedItemGO);
				this._storedItemGO = null;
				this.OnItemRemoved.Invoke(this._storedItemId);
			}
		}
	}
}
