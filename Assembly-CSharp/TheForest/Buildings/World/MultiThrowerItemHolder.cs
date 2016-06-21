using Bolt;
using System;
using TheForest.Items;
using TheForest.Items.Craft;
using TheForest.Items.Inventory;
using TheForest.Player;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Buildings.World
{
	[DoNotSerializePublic]
	public class MultiThrowerItemHolder : EntityBehaviour<IMultiThrowerState>
	{
		public Transform[] _renderSlots;

		public GameObject _takeIcon;

		public GameObject _addIcon;

		public Transform _lever;

		public rockThrowerAnimEvents _anim;

		[ItemIdPicker(Item.Types.Equipment)]
		public int[] _blackListedItemIds;

		[Header("FMOD")]
		public string _addItemEvent;

		[SerializeThis]
		private int[] _items;

		private int _nextItemIndex;

		private bool _hasPreloaded;

		private int[] _ammoLoaded = new int[3];

		public int[] AmmoLoaded
		{
			get
			{
				return this._ammoLoaded;
			}
		}

		private void Awake()
		{
			base.enabled = false;
			this._items = new int[this._renderSlots.Length];
		}

		private void OnEnable()
		{
			FMODCommon.PreloadEvents(new string[]
			{
				this._addItemEvent
			});
			this._hasPreloaded = true;
		}

		private void OnDisable()
		{
			if (this._hasPreloaded)
			{
				FMODCommon.UnloadEvents(new string[]
				{
					this._addItemEvent
				});
				this._hasPreloaded = false;
			}
		}

		private void Update()
		{
			if (BoltNetwork.isRunning)
			{
				this.UpdateMP();
			}
			if (this._nextItemIndex > 0)
			{
				if (!this._takeIcon.activeSelf)
				{
					this._takeIcon.SetActive(true);
				}
				if (TheForest.Utils.Input.GetButtonDown("Take") && LocalPlayer.Inventory.AddItem(this._items[this._nextItemIndex - 1], 1, false, false, (WeaponStatUpgrade.Types)(-2)))
				{
					if (BoltNetwork.isRunning)
					{
						ItemHolderTakeItem itemHolderTakeItem = ItemHolderTakeItem.Create(GlobalTargets.OnlyServer);
						itemHolderTakeItem.Target = this.entity;
						itemHolderTakeItem.ContentType = this._items[this._nextItemIndex - 1];
						itemHolderTakeItem.Player = LocalPlayer.Entity;
						itemHolderTakeItem.Send();
					}
					else
					{
						this._nextItemIndex--;
						UnityEngine.Object.Destroy(this._renderSlots[this._nextItemIndex].GetChild(0).gameObject);
						this._renderSlots[this._nextItemIndex].gameObject.SetActive(false);
						this._items[this._nextItemIndex] = -1;
					}
					if (DecayingInventoryItemView.LastUsed)
					{
						DecayingInventoryItemView.LastUsed.SetDecayState(DecayingInventoryItemView.DecayStates.Spoilt);
					}
				}
			}
			else if (this._takeIcon.activeSelf)
			{
				this._takeIcon.SetActive(false);
			}
			if (this._nextItemIndex < this._renderSlots.Length && !LocalPlayer.Inventory.IsRightHandEmpty() && !LocalPlayer.Inventory.RightHand.ItemCache.MatchType(Item.Types.Story) && !this.IsBlackListed(LocalPlayer.Inventory.RightHand._itemId))
			{
				if (!this._addIcon.activeSelf)
				{
					this._addIcon.SetActive(true);
				}
				if (TheForest.Utils.Input.GetButtonDown("Craft"))
				{
					if (this._addItemEvent.Length > 0)
					{
						FMODCommon.PlayOneshot(this._addItemEvent, base.transform);
					}
					else
					{
						LocalPlayer.Sfx.PlayPutDown(base.gameObject);
					}
					int itemId = LocalPlayer.Inventory.RightHand._itemId;
					LocalPlayer.Inventory.ShuffleRemoveRightHandItem();
					if (BoltNetwork.isRunning)
					{
						ItemHolderAddItem itemHolderAddItem = ItemHolderAddItem.Create(GlobalTargets.OnlyServer);
						itemHolderAddItem.ContentType = itemId;
						itemHolderAddItem.Target = this.entity;
						itemHolderAddItem.Send();
					}
					else
					{
						this.SpawnEquipmentItemView(this._renderSlots[this._nextItemIndex], itemId);
						this._items[this._nextItemIndex] = itemId;
						this._nextItemIndex++;
					}
				}
			}
			else if (this._addIcon.activeSelf)
			{
				this._addIcon.SetActive(false);
			}
		}

		private void OnDeserialized()
		{
			if (!BoltNetwork.isClient)
			{
				for (int i = 0; i < this._items.Length; i++)
				{
					Transform parent = this._renderSlots[i];
					int num = this._items[i];
					if (num <= 0)
					{
						break;
					}
					this.SpawnEquipmentItemView(parent, num);
					this._nextItemIndex++;
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
			this._addIcon.SetActive(false);
			this._takeIcon.SetActive(false);
		}

		private void UpdateMP()
		{
			if (BoltNetwork.isServer)
			{
				base.state.leverRotate = this._lever.localRotation.y;
			}
			if (!LocalPlayer.AnimControl.onRockThrower && BoltNetwork.isRunning)
			{
				Vector3 localEulerAngles = this._lever.localEulerAngles;
				localEulerAngles.y = base.state.leverRotate;
				this._lever.localEulerAngles = localEulerAngles;
			}
		}

		private bool IsBlackListed(int itemId)
		{
			for (int i = 0; i < this._blackListedItemIds.Length; i++)
			{
				if (this._blackListedItemIds[i] == itemId)
				{
					return true;
				}
			}
			return false;
		}

		private void SpawnEquipmentItemView(Transform parent, int itemId)
		{
			InventoryItemView inventoryItemView = LocalPlayer.Inventory.InventoryItemViewsCache[itemId][0];
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>((!inventoryItemView.ItemCache._throwerProjectilePrefab) ? inventoryItemView._held : inventoryItemView.ItemCache._throwerProjectilePrefab.gameObject);
			parent.gameObject.SetActive(true);
			gameObject.transform.parent = parent;
			gameObject.transform.localPosition = inventoryItemView._held.transform.localPosition;
			gameObject.transform.localRotation = inventoryItemView._held.transform.localRotation;
			gameObject.layer = base.gameObject.layer;
			gameObject.SetActive(true);
			if (!inventoryItemView.ItemCache._throwerProjectilePrefab)
			{
				foreach (Transform transform in gameObject.transform)
				{
					if (transform.name == "collide")
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
			}
		}

		private void TransferItemView(Transform itemView, Transform parent, int itemId)
		{
			InventoryItemView inventoryItemView = LocalPlayer.Inventory.InventoryItemViewsCache[itemId][0];
			itemView.parent = parent;
			itemView.localPosition = inventoryItemView._held.transform.localPosition;
			itemView.localRotation = inventoryItemView._held.transform.localRotation;
		}

		public override void Attached()
		{
			base.state.AddCallback("Items[]", new PropertyCallbackSimple(this.ItemsChangedMP));
			if (BoltNetwork.isClient)
			{
				base.state.AddCallback("Ammo[]", new PropertyCallbackSimple(this.AmmoChangedMP));
			}
			base.state.AddCallback("leverRotate", new PropertyCallbackSimple(this.leverRotateChangedMP));
			if (BoltNetwork.isServer)
			{
				for (int i = 0; i < this._items.Length; i++)
				{
					base.state.Items[i] = this._items[i];
				}
				for (int j = 0; j < this._ammoLoaded.Length; j++)
				{
					base.state.Ammo[j] = this._ammoLoaded[j];
				}
			}
		}

		public void loadItemIntoBasket(int type)
		{
			if (this._nextItemIndex > 0)
			{
				this._nextItemIndex--;
				Transform child = this._renderSlots[this._nextItemIndex].GetChild(0);
				int num = this._items[this._nextItemIndex];
				this._items[this._nextItemIndex] = -1;
				this._ammoLoaded[this._anim.ammoCount] = num;
				this._anim.rockAmmo[this._anim.ammoCount].SetActive(true);
				this.TransferItemView(child, this._anim.rockAmmo[this._anim.ammoCount].transform, num);
				FMODCommon.PlayOneshotNetworked(LocalPlayer.Sfx.WhooshEvent, base.transform, FMODCommon.NetworkRole.Server);
				if (BoltNetwork.isRunning)
				{
					this.entity.Freeze(false);
					base.state.Items[this._nextItemIndex] = -1;
					base.state.Ammo[this._anim.ammoCount] = num;
				}
				this._anim.ammoCount++;
			}
		}

		public void resetBasketAmmo()
		{
			for (int i = 0; i < this._ammoLoaded.Length; i++)
			{
				this._ammoLoaded[i] = 0;
				if (BoltNetwork.isServer)
				{
					base.state.Ammo[i] = 0;
				}
				if (!BoltNetwork.isClient && this._anim.rockAmmo[i].transform.childCount > 0)
				{
					UnityEngine.Object.Destroy(this._anim.rockAmmo[i].transform.GetChild(0).gameObject);
				}
				this._anim.rockAmmo[i].SetActive(false);
			}
			this._anim.ammoCount = 0;
		}

		public void TakeItemMP(BoltEntity targetPlayer, int itemId)
		{
			if (this._nextItemIndex > 0)
			{
				this.entity.Freeze(false);
				base.state.Items[--this._nextItemIndex] = (this._items[this._nextItemIndex] = -1);
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
				itemRemoveFromPlayer.ItemId = itemId;
				itemRemoveFromPlayer.Send();
			}
		}

		public void AddItemMP(int itemId)
		{
			if (this._nextItemIndex < this._items.Length)
			{
				NetworkArray_Values<int> arg_35_0 = base.state.Items;
				int arg_35_1 = this._nextItemIndex;
				this._items[this._nextItemIndex] = itemId;
				arg_35_0[arg_35_1] = itemId;
				this.entity.Freeze(false);
				this._nextItemIndex++;
			}
		}

		private void ItemsChangedMP()
		{
			if (BoltNetwork.isClient)
			{
				this._nextItemIndex = 0;
				for (int i = 0; i < this._items.Length; i++)
				{
					int num = base.state.Items[i];
					this._items[i] = num;
					if (num > 0)
					{
						this._nextItemIndex++;
					}
				}
			}
			for (int j = 0; j < this._items.Length; j++)
			{
				int num2 = this._items[j];
				Transform transform = this._renderSlots[j];
				bool flag = transform.childCount > 0;
				if (num2 > 0)
				{
					if (!flag)
					{
						this.SpawnEquipmentItemView(transform, num2);
					}
				}
				else if (flag)
				{
					UnityEngine.Object.Destroy(transform.GetChild(0).gameObject);
				}
			}
		}

		private void AmmoChangedMP()
		{
			if (BoltNetwork.isClient)
			{
				this._anim.ammoCount = 0;
				for (int i = 0; i < this._ammoLoaded.Length; i++)
				{
					if (this._ammoLoaded[i] != base.state.Ammo[i])
					{
						if (this._anim.rockAmmo[i].transform.childCount > 0)
						{
							this._anim.rockAmmo[i].SetActive(false);
							UnityEngine.Object.Destroy(this._anim.rockAmmo[i].transform.GetChild(0).gameObject);
						}
						this._ammoLoaded[i] = base.state.Ammo[i];
						if (this._ammoLoaded[i] > 0)
						{
							this._anim.rockAmmo[i].SetActive(true);
							this.SpawnEquipmentItemView(this._anim.rockAmmo[i].transform, this._ammoLoaded[i]);
						}
					}
					if (this._ammoLoaded[i] > 0)
					{
						this._anim.ammoCount++;
					}
				}
			}
		}

		private void leverRotateChangedMP()
		{
			if (!LocalPlayer.AnimControl.onRockThrower)
			{
				Vector3 localEulerAngles = this._lever.localEulerAngles;
				localEulerAngles.y = base.state.leverRotate;
				this._lever.localEulerAngles = localEulerAngles;
			}
		}

		public void disableTriggerMP()
		{
			if (BoltNetwork.isRunning)
			{
				RockThrowerActivated rockThrowerActivated = RockThrowerActivated.Create(GlobalTargets.Everyone);
				rockThrowerActivated.Target = this.entity;
				rockThrowerActivated.Send();
			}
		}

		public void enableTriggerMP()
		{
			if (BoltNetwork.isRunning)
			{
				RockThrowerDeActivated rockThrowerDeActivated = RockThrowerDeActivated.Create(GlobalTargets.Everyone);
				rockThrowerDeActivated.Target = this.entity;
				rockThrowerDeActivated.Send();
			}
		}

		public void forceRemoveItem()
		{
			if (this._nextItemIndex > 0)
			{
				if (BoltNetwork.isRunning)
				{
					RockThrowerRemoveItem rockThrowerRemoveItem = RockThrowerRemoveItem.Create(GlobalTargets.OnlyServer);
					rockThrowerRemoveItem.ContentType = this._items[this._nextItemIndex - 1];
					rockThrowerRemoveItem.Target = this.entity;
					rockThrowerRemoveItem.Player = LocalPlayer.Entity;
					rockThrowerRemoveItem.Send();
				}
				else
				{
					this.loadItemIntoBasket(this._items[this._nextItemIndex - 1]);
				}
			}
		}

		public void sendResetAmmoMP()
		{
			if (BoltNetwork.isRunning)
			{
				RockThrowerResetAmmo rockThrowerResetAmmo = RockThrowerResetAmmo.Create(GlobalTargets.Everyone);
				rockThrowerResetAmmo.Target = this.entity;
				rockThrowerResetAmmo.Send();
			}
			else
			{
				this.resetBasketAmmo();
			}
		}

		public void sendAnimVars(int var, bool onoff)
		{
			RockThrowerAnimate rockThrowerAnimate = RockThrowerAnimate.Create(GlobalTargets.Everyone);
			rockThrowerAnimate.animVar = var;
			rockThrowerAnimate.onoff = onoff;
			rockThrowerAnimate.Target = this.entity;
			rockThrowerAnimate.Send();
		}

		public void sendLandTarget()
		{
			RockThrowerLandTarget rockThrowerLandTarget = RockThrowerLandTarget.Create(GlobalTargets.OnlyServer);
			rockThrowerLandTarget.landPos = this._anim.throwPos.GetComponent<rockThrowerAimingReticle>()._currentLandTarget;
			rockThrowerLandTarget.Target = this.entity;
			rockThrowerLandTarget.Send();
		}
	}
}
