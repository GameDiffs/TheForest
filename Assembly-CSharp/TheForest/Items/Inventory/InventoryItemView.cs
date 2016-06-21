using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TheForest.Items.Craft;
using TheForest.Items.Utils;
using TheForest.Utils;
using UniLinq;
using UnityEngine;

namespace TheForest.Items.Inventory
{
	[DoNotSerializePublic, AddComponentMenu("Items/Inventory/Item Inventory View")]
	public class InventoryItemView : MonoBehaviour
	{
		[ItemIdPicker]
		public int _itemId;

		public bool _isCraft;

		public bool _canEquipFromCraft;

		public bool _canDropFromInventory;

		public bool _addMaxToCraft;

		public bool _allowMultiView;

		public int _maxMultiViews = 20;

		public Material _selectedMaterial;

		public GameObject _held;

		public weaponInfo _heldWeaponInfo;

		public GameObject _worldPrefab;

		public GameObject _altWorldPrefab;

		[HideInInspector]
		public PlayerInventory _inventory;

		public ActiveBonusListenerMat _bonusListener;

		protected bool _hovered;

		private List<InventoryItemView> _multiViews;

		private Item _item;

		private Material _normalMaterial;

		[SerializeThis]
		public WeaponStatUpgrade.Types _activeBonus = (WeaponStatUpgrade.Types)(-1);

		[SerializeThis]
		public float _activeBonusValue;

		public event Action<WeaponStatUpgrade.Types> ActiveBonusChanged
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				this.ActiveBonusChanged = (Action<WeaponStatUpgrade.Types>)Delegate.Combine(this.ActiveBonusChanged, value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				this.ActiveBonusChanged = (Action<WeaponStatUpgrade.Types>)Delegate.Remove(this.ActiveBonusChanged, value);
			}
		}

		public Item ItemCache
		{
			get
			{
				return this._item;
			}
		}

		public bool CanBeStored
		{
			get
			{
				return this._item.MatchType(LocalPlayer.Inventory.CurrentStorage.AcceptedTypes);
			}
		}

		public bool IsHeldOnly
		{
			get
			{
				return this._item != null && this._item._maxAmount < 0;
			}
		}

		public Material NormalMaterial
		{
			get
			{
				return this._normalMaterial;
			}
			set
			{
				this._normalMaterial = value;
			}
		}

		public InventoryItemView MultiViewOwner
		{
			get;
			set;
		}

		public virtual WeaponStatUpgrade.Types ActiveBonus
		{
			get
			{
				return this._activeBonus;
			}
			set
			{
				this._activeBonus = value;
				this.ActiveBonusChanged(this._activeBonus);
				if (this._bonusListener)
				{
					this._bonusListener.ToggleItemViewMat(this);
				}
			}
		}

		public float ActiveBonusValue
		{
			get
			{
				return this._activeBonusValue;
			}
			set
			{
				this._activeBonusValue = value;
			}
		}

		public InventoryItemView()
		{
			this.ActiveBonusChanged = delegate
			{
			};
			base..ctor();
		}

		private void Start()
		{
			if (this._isCraft && this._allowMultiView)
			{
				this._multiViews = new List<InventoryItemView>();
			}
			if (!this._hovered)
			{
				base.enabled = false;
			}
		}

		private void OnDisable()
		{
			if (this._hovered)
			{
				if (Scene.HudGui)
				{
					Scene.HudGui.HideItemInfoView(this._itemId, this._isCraft);
				}
				base.gameObject.GetComponent<Renderer>().sharedMaterial = this._normalMaterial;
				this._hovered = false;
			}
			base.enabled = false;
		}

		private void OnMouseExitCollider()
		{
			if (base.enabled)
			{
				base.enabled = false;
			}
		}

		private void OnMouseEnterCollider()
		{
			if (!base.enabled && !this._inventory._craftingCog._upgradeCog.enabled)
			{
				this._hovered = true;
				base.enabled = true;
				Renderer component = base.gameObject.GetComponent<Renderer>();
				if (component && this._normalMaterial != component.sharedMaterial)
				{
					this._normalMaterial = component.sharedMaterial;
				}
				base.gameObject.GetComponent<Renderer>().sharedMaterial = this._selectedMaterial;
				Scene.HudGui.ShowItemInfoView(this, LocalPlayer.InventoryCam.ScreenToViewportPoint(TheForest.Utils.Input.mousePosition), this._isCraft);
			}
		}

		private void Update()
		{
			if (this._inventory._craftingCog._upgradeCog.enabled)
			{
				return;
			}
			if (this._isCraft)
			{
				if (TheForest.Utils.Input.GetButtonDown("Combine"))
				{
					if (this._item.MatchType(Item.Types.Special))
					{
						LocalPlayer.Inventory.SpecialItemsControlers[this._itemId].ToggleSpecialCraft(false);
					}
					if (this._allowMultiView && this.MultiViewOwner != null)
					{
						Scene.HudGui.HideItemInfoView(this._itemId, this._isCraft);
						this.MultiViewOwner.BubbleDownMultiview(this);
					}
					LocalPlayer.Sfx.PlayInventorySound(Item.SFXCommands.PlayWhoosh);
					int num = (!TheForest.Utils.Input.GetButton("Batch")) ? 1 : 10;
					if (this._addMaxToCraft)
					{
						num = 2147483647;
					}
					WeaponStatUpgrade.Types activeBonus = this._activeBonus;
					if (this._inventory._craftingCog.Storage && base.GetComponentInParent<ItemStorageProxy>())
					{
						num -= this._inventory._craftingCog.Storage.Remove(this._itemId, num, (WeaponStatUpgrade.Types)(-2));
					}
					else
					{
						num -= this._inventory.CurrentStorage.Remove(this._itemId, num, activeBonus);
					}
					this._inventory.AddItem(this._itemId, num, true, true, activeBonus);
					this._activeBonus = (WeaponStatUpgrade.Types)(-1);
				}
				if (this._canEquipFromCraft && TheForest.Utils.Input.GetButtonUp("Equip") && this._item.MatchType(Item.Types.Equipment))
				{
					this._inventory.Equip(this._itemId, true);
					this._inventory.CurrentStorage.Remove(this._itemId, 1, (WeaponStatUpgrade.Types)(-2));
					this._inventory.Close();
				}
			}
			else
			{
				if (TheForest.Utils.Input.GetButtonDown("Combine"))
				{
					if (this.CanBeStored && this._item._equipmentSlot < Item.EquipmentSlot.Chest)
					{
						if (!this._item.MatchType(Item.Types.Special) || LocalPlayer.Inventory.SpecialItemsControlers[this._itemId].ToggleSpecialCraft(true))
						{
							if (this._allowMultiView && this.MultiViewOwner != null)
							{
								Scene.HudGui.HideItemInfoView(this._itemId, this._isCraft);
								this.MultiViewOwner.RemovedMultiView(this);
							}
							LocalPlayer.Sfx.PlayInventorySound(Item.SFXCommands.PlayWhoosh);
							if (this._inventory.HasInSlot(this._item._equipmentSlot, this._itemId) && this._inventory.AmountOf(this._itemId, false) == 1)
							{
								this._inventory.MemorizeItem(this._item._equipmentSlot);
							}
							int num2 = 1;
							if (this._addMaxToCraft)
							{
								num2 = this._inventory.AmountOf(this._itemId, false);
							}
							else if (TheForest.Utils.Input.GetButton("Batch"))
							{
								num2 = Mathf.Min(this._inventory.AmountOfItemWithBonus(this._itemId, this._activeBonus), 10);
							}
							if (num2 > 1)
							{
								LocalPlayer.Inventory.SortInventoryViewsByBonus(this, this._activeBonus, true);
							}
							else
							{
								this._inventory.BubbleUpInventoryView(this);
							}
							WeaponStatUpgrade.Types activeBonus2 = this._activeBonus;
							int num3 = this._inventory.CurrentStorage.Add(this._itemId, num2, activeBonus2);
							this._inventory.RemoveItem(this._itemId, num2 - num3, true);
						}
					}
					else if (this._canDropFromInventory && this._item.MatchType(Item.Types.WorldObject))
					{
						GameObject worldGo;
						if (!BoltNetwork.isRunning || !this.ItemCache._pickupPrefabMP)
						{
							worldGo = (GameObject)UnityEngine.Object.Instantiate((!this.ItemCache._pickupPrefab) ? this._worldPrefab : this.ItemCache._pickupPrefab.gameObject, this._held.transform.position, this._held.transform.rotation);
						}
						else
						{
							worldGo = BoltNetwork.Instantiate(this.ItemCache._pickupPrefabMP.gameObject, this._held.transform.position, this._held.transform.rotation).gameObject;
						}
						this.OnItemDropped(worldGo);
						this._inventory.BubbleUpInventoryView(this);
						this._inventory.RemoveItem(this._itemId, 1, false);
					}
				}
				if (TheForest.Utils.Input.GetButtonUp("Equip"))
				{
					if (this._item.MatchType(Item.Types.Edible) && (this._activeBonus == this._item._edibleCondition || this._activeBonus == this._item._altEdibleCondition))
					{
						this.UseEdible();
					}
					else if (this._item.MatchType(Item.Types.Equipment))
					{
						if (!this._item._inventoryTooltipOnly)
						{
							this._inventory.BubbleUpInventoryView(this);
							this._inventory.Equip(this._itemId, false);
							this._inventory.Close();
						}
					}
					else if (this._item.MatchType(Item.Types.Special))
					{
						this._inventory.SpecialItemsControlers[this._itemId].ToggleSpecial(true);
						if (this._item._equipedSFX != Item.SFXCommands.None)
						{
							this._inventory.SendMessage(this._item._equipedSFX.ToString());
						}
						this._inventory.Close();
					}
				}
				else if ((this._item._equipmentSlot == Item.EquipmentSlot.Chest || this._item._equipmentSlot == Item.EquipmentSlot.Feet) && TheForest.Utils.Input.GetButtonDown("Combine") && LocalPlayer.Inventory.EquipmentSlots[(int)this._item._equipmentSlot] == this)
				{
					LocalPlayer.Inventory.UnequipItemAtSlot(this._item._equipmentSlot, false, true, false);
				}
			}
		}

		public virtual void OnDeserialized()
		{
			if (base.GetComponent<EmptyObjectIdentifier>())
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
		}

		public virtual void OnSerializing()
		{
		}

		public virtual void OnItemAdded()
		{
		}

		public virtual void OnItemRemoved()
		{
		}

		public virtual void OnItemDropped(GameObject worldGo)
		{
		}

		public virtual void OnItemEquipped()
		{
		}

		public virtual void Init()
		{
			if (this._itemId > 0)
			{
				this._item = ItemDatabase.ItemById(this._itemId);
				Renderer component = base.gameObject.GetComponent<Renderer>();
				if (component)
				{
					this._normalMaterial = base.gameObject.GetComponent<Renderer>().sharedMaterial;
				}
				this._canEquipFromCraft = this._item.MatchType(Item.Types.Equipment);
				return;
			}
			Debug.Log("InventoryItemView not setup : " + base.name);
			if (Application.isPlaying)
			{
				UnityEngine.Object.Destroy(base.gameObject, 0.5f);
			}
		}

		protected virtual void UseEdible()
		{
			if (this._activeBonus == WeaponStatUpgrade.Types.DirtyWater)
			{
				LocalPlayer.Stats.HitFoodDelayed(10);
			}
			this.ActiveBonus = (WeaponStatUpgrade.Types)(-1);
			this._inventory.BubbleUpInventoryView(this);
			if (!this._item.MatchType(Item.Types.Equipment))
			{
				this._inventory.RemoveItem(this._itemId, 1, false);
				GameStats.EdibleItemUsed.Invoke(this._itemId);
			}
			ItemUtils.ApplyEffectsToStats(this._item._usedStatEffect, true);
			if (this._item._usedSFX != Item.SFXCommands.None)
			{
				this._inventory.SendMessage("PlayInventorySound", this._item._usedSFX);
			}
		}

		public void ApplyEquipmentEffect(bool forward)
		{
			ItemUtils.ApplyEffectsToStats(this._item._equipedStatEffect, forward);
		}

		public WeaponStatUpgrade.Types GetFirstViewBonus()
		{
			if (this._multiViews != null && this._multiViews.Count > 0)
			{
				return this._multiViews[0].ActiveBonus;
			}
			return (WeaponStatUpgrade.Types)(-2);
		}

		public int AmountOfMultiviewWithBonus(int itemId, WeaponStatUpgrade.Types bonus)
		{
			if (this._allowMultiView && this._multiViews != null)
			{
				int num = 0;
				for (int i = this._multiViews.Count - 1; i >= 0; i--)
				{
					if (this._multiViews[i]._itemId == itemId && (bonus == (WeaponStatUpgrade.Types)(-2) || this._multiViews[i].ActiveBonus == bonus))
					{
						num++;
					}
				}
				return num;
			}
			return 0;
		}

		public int AmountOfMultiviewWithoutBonus(int itemId, WeaponStatUpgrade.Types bonus)
		{
			if (this._allowMultiView && this._multiViews != null)
			{
				int num = 0;
				for (int i = this._multiViews.Count - 1; i >= 0; i--)
				{
					if (this._multiViews[i]._itemId == itemId && this._multiViews[i].ActiveBonus != bonus)
					{
						num++;
					}
				}
				return num;
			}
			return 0;
		}

		public void SetMultiViewAmount(int amount, WeaponStatUpgrade.Types activeBonus)
		{
			if (this._allowMultiView)
			{
				int i = Mathf.Clamp(amount, 0, this._maxMultiViews);
				if (this._multiViews == null)
				{
					this._multiViews = new List<InventoryItemView>();
				}
				base.gameObject.SetActive(true);
				int num = (activeBonus != (WeaponStatUpgrade.Types)(-2)) ? this.AmountOfMultiviewWithBonus(this._itemId, activeBonus) : this._multiViews.Count;
				while (i > num)
				{
					num++;
					this.SpawnAnyMultiview(this, base.transform.parent, true, activeBonus);
				}
				if (i < num)
				{
					this.RemovedMultiViews(this._itemId, num - i, activeBonus);
				}
				base.gameObject.SetActive(false);
			}
		}

		public void SetAnyMultiViewAmount(InventoryItemView source, Transform parent, int amount, WeaponStatUpgrade.Types activeBonus)
		{
			if (this._allowMultiView)
			{
				int i = Mathf.Clamp(amount, 0, this._maxMultiViews);
				if (this._multiViews == null)
				{
					this._multiViews = new List<InventoryItemView>();
				}
				base.gameObject.SetActive(true);
				int num = (activeBonus != (WeaponStatUpgrade.Types)(-2)) ? this.AmountOfMultiviewWithBonus(source._itemId, activeBonus) : this._multiViews.Count;
				while (i > num)
				{
					num++;
					this.SpawnAnyMultiview(source, parent, false, activeBonus);
				}
				if (i < num)
				{
					this.RemovedMultiViews(source._itemId, num - i, activeBonus);
				}
			}
		}

		public void SpawnAnyMultiview(InventoryItemView source, Transform parent, bool randomRotation, WeaponStatUpgrade.Types activeBonus)
		{
			Vector3 euler = new Vector3(0f, (float)UnityEngine.Random.Range(-50, 50), 0f);
			bool flag = source.Equals(this);
			Collider component = source.GetComponent<Collider>();
			Vector3 size;
			if (component is BoxCollider)
			{
				size = ((BoxCollider)component).size;
			}
			else if (component is CapsuleCollider)
			{
				CapsuleCollider capsuleCollider = (CapsuleCollider)component;
				int direction = capsuleCollider.direction;
				if (direction != 0)
				{
					if (direction != 1)
					{
						size = new Vector3(capsuleCollider.radius * 2f, capsuleCollider.radius * 2f, capsuleCollider.height);
					}
					else
					{
						size = new Vector3(capsuleCollider.radius * 2f, capsuleCollider.height, capsuleCollider.radius * 2f);
					}
				}
				else
				{
					size = new Vector3(capsuleCollider.height, capsuleCollider.radius * 2f, capsuleCollider.radius * 2f);
				}
			}
			else
			{
				size = component.bounds.size;
			}
			Vector3 vector = UnityEngine.Random.insideUnitSphere;
			if (!flag)
			{
				vector *= 3f;
			}
			if (size.x * 2f < size.z)
			{
				vector /= 15f;
				vector *= size.x * 0.75f;
				vector.x /= 2f;
			}
			else if (size.z * 2f < size.x)
			{
				vector /= 15f;
				vector *= size.z * 0.75f;
				vector.z /= 2f;
			}
			else
			{
				vector /= 8f;
				bool flag2 = size.z * 2f > size.y;
				bool flag3 = size.x * 2f > size.y;
				if (flag2 && !flag3)
				{
					euler.z = (float)UnityEngine.Random.Range(-45, 45);
				}
				if (flag3 && !flag2)
				{
					euler.x = (float)UnityEngine.Random.Range(-45, 45);
				}
			}
			vector.y = 0f;
			InventoryItemView inventoryItemView = (InventoryItemView)UnityEngine.Object.Instantiate(source, base.transform.position + vector, (!randomRotation) ? source.transform.rotation : (source.transform.rotation * Quaternion.Euler(euler)));
			inventoryItemView.transform.parent = parent;
			inventoryItemView.transform.localScale = source.transform.localScale;
			inventoryItemView._isCraft = true;
			inventoryItemView.MultiViewOwner = this;
			inventoryItemView.enabled = true;
			inventoryItemView.gameObject.SetActive(true);
			inventoryItemView.Init();
			if (activeBonus != (WeaponStatUpgrade.Types)(-2))
			{
				inventoryItemView.ActiveBonus = activeBonus;
			}
			this._multiViews.Add(inventoryItemView);
		}

		public void SetMultiviewsBonus(WeaponStatUpgrade.Types bonus)
		{
			if (this._multiViews != null && this._multiViews.Count > 0)
			{
				for (int i = 0; i < this._multiViews.Count; i++)
				{
					this._multiViews[i].ActiveBonus = bonus;
				}
			}
		}

		public bool ContainsMultiView(int itemId)
		{
			return this._multiViews.Any((InventoryItemView mv) => mv._itemId == itemId);
		}

		private void BubbleDownMultiview(InventoryItemView iiv)
		{
			if (this._multiViews.Contains(iiv))
			{
				this._multiViews.Remove(iiv);
				this._multiViews.Insert(0, iiv);
			}
		}

		public int AmountOfMultiView(int itemId)
		{
			return this._multiViews.Count((InventoryItemView mv) => mv._itemId == itemId);
		}

		private void RemovedMultiView(InventoryItemView view)
		{
			if (this._multiViews.Contains(view))
			{
				this._multiViews.Remove(view);
				UnityEngine.Object.Destroy(view.gameObject);
			}
		}

		public void RemovedMultiViews(int itemId, int amount, WeaponStatUpgrade.Types bonus)
		{
			if (this._multiViews == null)
			{
				this._multiViews = new List<InventoryItemView>();
			}
			int num = this._multiViews.FindIndex((InventoryItemView mv) => mv._itemId == itemId && (bonus == (WeaponStatUpgrade.Types)(-2) || mv.ActiveBonus == bonus));
			while (num >= 0 && amount > 0)
			{
				amount--;
				if (this._multiViews[num])
				{
					UnityEngine.Object.Destroy(this._multiViews[num].gameObject);
				}
				this._multiViews.RemoveAt(num);
				num = this._multiViews.FindIndex((InventoryItemView mv) => mv._itemId == itemId && (bonus == (WeaponStatUpgrade.Types)(-2) || mv.ActiveBonus == bonus));
			}
		}

		public void ClearMultiViews()
		{
			if (this._multiViews == null)
			{
				this._multiViews = new List<InventoryItemView>();
			}
			while (this._multiViews.Count > 0)
			{
				if (this._multiViews[0])
				{
					UnityEngine.Object.Destroy(this._multiViews[0].gameObject);
				}
				this._multiViews.RemoveAt(0);
			}
		}
	}
}
