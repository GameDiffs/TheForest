using Bolt;
using FMOD.Studio;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TheForest.Items.Core;
using TheForest.Items.Craft;
using TheForest.Items.Special;
using TheForest.Items.World;
using TheForest.Tools;
using TheForest.Utils;
using UniLinq;
using UnityEngine;
using UnityEngine.Events;

namespace TheForest.Items.Inventory
{
	[DoNotSerializePublic, AddComponentMenu("Items/Inventory/Player Inventory")]
	public class PlayerInventory : MonoBehaviour
	{
		public class ItemEvent : UnityEvent<int>
		{
		}

		public enum PlayerViews
		{
			PlaneCrash = -1,
			Loading,
			WakingUp,
			World,
			Inventory,
			ClosingInventory,
			Book,
			Pause,
			Death,
			Loot,
			Sleep
		}

		public class ItemsUpgradeCountersDict : Dictionary<int, PlayerInventory.UpgradeCounterDict>
		{
		}

		public class UpgradeCounterDict : Dictionary<int, int>
		{
		}

		[Serializable]
		public class SerializableItemUpgradeCounters
		{
			public int _itemId;

			public int _count;

			public PlayerInventory.SerializableUpgradeCounter[] _counters;
		}

		[Serializable]
		public class SerializableUpgradeCounter
		{
			public int _upgradeItemId;

			public int _amount;
		}

		public ItemDatabase _itemDatabase;

		[SerializeThis]
		public List<InventoryItem> _possessedItems;

		[SerializeThis]
		public int _possessedItemsCount;

		public CraftingCog _craftingCog;

		public List<UpgradeViewReceiver> _upgradeViewReceivers;

		public InventoryItemView[] _itemViews;

		public GameObject _specialActions;

		public GameObject _specialItems;

		public GameObject _inventoryGO;

		[ItemIdPicker]
		public int _leafItemId;

		[ItemIdPicker]
		public int _seedItemId;

		[ItemIdPicker]
		public int _sapItemId;

		[ItemIdPicker(Item.Types.Equipment)]
		public int _defaultWeaponItemId;

		private PlayerInventory.PlayerViews _currentView;

		[SerializeThis]
		private int[] _equipmentSlotsIds;

		private bool[] _equipmentSlotsLocked;

		private InventoryItemView[] _equipmentSlots;

		private InventoryItemView[] _equipmentSlotsPrevious;

		private InventoryItemView[] _equipmentSlotsPreviousOverride;

		private InventoryItemView[] _equipmentSlotsNext;

		private InventoryItemView _noEquipedItem;

		[SerializeThis]
		private PlayerInventory.SerializableItemUpgradeCounters[] _upgradeCounters;

		[SerializeThis]
		private int _upgradeCountersCount;

		private Dictionary<int, InventoryItem> _possessedItemCache;

		private Dictionary<int, List<InventoryItemView>> _inventoryItemViewsCache;

		private PlayMakerFSM _pm;

		private ItemAnimatorHashHelper _itemAnimHash;

		private float _weaponChargeStartTime;

		private float _equipPreviousTime;

		private bool _isThrowing;

		private FMOD.Studio.EventInstance _pauseSnapshot;

		public PlayerInventory.ItemEvent Equiped = new PlayerInventory.ItemEvent();

		public PlayerInventory.ItemEvent RemovedItem = new PlayerInventory.ItemEvent();

		public UnityEvent StashedLeftHand = new UnityEvent();

		public UnityEvent StashedRightHand = new UnityEvent();

		public UnityEvent DroppedRightHand = new UnityEvent();

		public UnityEvent Attacked = new UnityEvent();

		public UnityEvent AttackEnded = new UnityEvent();

		public UnityEvent ReleasedAttack = new UnityEvent();

		public UnityEvent Blocked = new UnityEvent();

		public UnityEvent Unblocked = new UnityEvent();

		public Dictionary<int, List<InventoryItemView>> InventoryItemViewsCache
		{
			get
			{
				return this._inventoryItemViewsCache;
			}
		}

		public Dictionary<int, SpecialItemControlerBase> SpecialItemsControlers
		{
			get;
			set;
		}

		public GameObject SpecialItems
		{
			get
			{
				return this._specialItems;
			}
		}

		public GameObject SpecialActions
		{
			get
			{
				return this._specialActions;
			}
		}

		public PlayMakerFSM PM
		{
			get
			{
				return this._pm;
			}
		}

		public LighterControler DefaultLight
		{
			get;
			set;
		}

		public SpecialItemControlerBase LastLight
		{
			get;
			set;
		}

		public SpecialItemControlerBase LastUtility
		{
			get;
			set;
		}

		public InventoryItemView[] EquipmentSlots
		{
			get
			{
				return this._equipmentSlots;
			}
		}

		public InventoryItemView[] EquipmentSlotsPrevious
		{
			get
			{
				return this._equipmentSlotsPrevious;
			}
		}

		public InventoryItemView LeftHand
		{
			get
			{
				return this._equipmentSlots[1];
			}
		}

		public InventoryItemView RightHand
		{
			get
			{
				return this._equipmentSlots[0];
			}
		}

		public LogControler Logs
		{
			get;
			set;
		}

		public PlayerInventory.ItemsUpgradeCountersDict ItemsUpgradeCounters
		{
			get;
			set;
		}

		public bool BlockTogglingInventory
		{
			get;
			set;
		}

		public bool IsWeaponBurning
		{
			get;
			set;
		}

		public bool CancelNextChargedAttack
		{
			get;
			set;
		}

		public bool SkipNextAddItemWoosh
		{
			get;
			set;
		}

		public bool DontShowDrop
		{
			get;
			set;
		}

		public string PendingSendMessage
		{
			get;
			set;
		}

		public float WeaponChargeStartTime
		{
			get
			{
				return this._weaponChargeStartTime;
			}
		}

		public bool IsThrowing
		{
			get
			{
				return this._isThrowing;
			}
		}

		public bool UseAltProjectile
		{
			get;
			set;
		}

		public IItemStorage CurrentStorage
		{
			get;
			private set;
		}

		public IInventoryItemFilter ItemFilter
		{
			get;
			set;
		}

		public PlayerInventory.PlayerViews CurrentView
		{
			get
			{
				return this._currentView;
			}
			set
			{
				this._currentView = value;
				if (BoltNetwork.isRunning && LocalPlayer.Entity && LocalPlayer.Entity.IsAttached())
				{
					LocalPlayer.Entity.GetState<IPlayerState>().CurrentView = (int)value;
				}
			}
		}

		private void Awake()
		{
			this.SpecialItemsControlers = new Dictionary<int, SpecialItemControlerBase>();
			this._equipmentSlots = new InventoryItemView[Enum.GetValues(typeof(Item.EquipmentSlot)).Length];
			this._equipmentSlotsPrevious = new InventoryItemView[this._equipmentSlots.Length];
			this._equipmentSlotsPreviousOverride = new InventoryItemView[this._equipmentSlots.Length];
			this._equipmentSlotsNext = new InventoryItemView[this._equipmentSlots.Length];
			this._equipmentSlotsLocked = new bool[this._equipmentSlots.Length];
			this._equipPreviousTime = 3.40282347E+38f;
			this._noEquipedItem = this._inventoryGO.AddComponent<InventoryItemView>();
			this._noEquipedItem.enabled = false;
			for (int i = 0; i < this._equipmentSlots.Length; i++)
			{
				this._equipmentSlots[i] = this._noEquipedItem;
				this._equipmentSlotsPrevious[i] = this._noEquipedItem;
				this._equipmentSlotsPreviousOverride[i] = this._noEquipedItem;
				this._equipmentSlotsNext[i] = this._noEquipedItem;
			}
			this._itemAnimHash = new ItemAnimatorHashHelper();
			this.InitItemCache();
		}

		public void Start()
		{
			this._craftingCog._inventory = this;
			for (int i = 0; i < this._itemViews.Length; i++)
			{
				this._itemViews[i].Init();
			}
			this._pm = base.GetComponentInChildren<playerScriptSetup>().pmControl;
			this._inventoryGO.SetActive(false);
			if (!LevelSerializer.IsDeserializing)
			{
				for (int j = 0; j < ItemDatabase.Items.Length; j++)
				{
					Item item = ItemDatabase.Items[j];
					this.ToggleInventoryItemView(item._id, (WeaponStatUpgrade.Types)(-2), false);
				}
				DecayingInventoryItemView.LastUsed = null;
			}
			else
			{
				base.enabled = false;
			}
		}

		[DebuggerHidden]
		public IEnumerator OnDeserialized()
		{
			PlayerInventory.<OnDeserialized>c__Iterator167 <OnDeserialized>c__Iterator = new PlayerInventory.<OnDeserialized>c__Iterator167();
			<OnDeserialized>c__Iterator.<>f__this = this;
			return <OnDeserialized>c__Iterator;
		}

		private void Update()
		{
			if ((this.CurrentView != PlayerInventory.PlayerViews.Pause && !this.BlockTogglingInventory && !LocalPlayer.AnimControl.useRootMotion && !LocalPlayer.FpCharacter.jumping && TheForest.Utils.Input.GetButtonDown("Inventory")) || (this.CurrentView == PlayerInventory.PlayerViews.Inventory && TheForest.Utils.Input.GetButtonDown("Esc")))
			{
				this.ToggleInventory();
			}
			else if ((TheForest.Utils.Input.GetButtonDown("Esc") && this.CurrentView != PlayerInventory.PlayerViews.Book && !LocalPlayer.Create.CreateMode) || (this.CurrentView == PlayerInventory.PlayerViews.Pause && !Scene.HudGui.PauseMenu.activeSelf))
			{
				this.TogglePauseMenu();
			}
			else if (this.CurrentView == PlayerInventory.PlayerViews.World)
			{
				if (TheForest.Utils.Input.GetButtonDown("Fire1"))
				{
					this.Attack();
				}
				if (TheForest.Utils.Input.GetButtonUp("Fire1"))
				{
					this.ReleaseAttack();
				}
				else if (TheForest.Utils.Input.GetButtonDown("AltFire"))
				{
					this.Block();
				}
				else if (TheForest.Utils.Input.GetButtonUp("AltFire"))
				{
					this.UnBlock();
				}
				else if (TheForest.Utils.Input.GetButtonDown("Drop") && !LocalPlayer.AnimControl.upsideDown)
				{
					bool flag = this.HasInSlot(Item.EquipmentSlot.RightHand, this._defaultWeaponItemId);
					bool flag2 = this.HasInPreviousSlot(Item.EquipmentSlot.RightHand, this._defaultWeaponItemId);
					bool flag3 = true;
					if (flag && this._equipmentSlotsPrevious[0])
					{
						flag3 = this.HasInSlot(Item.EquipmentSlot.RightHand, this._equipmentSlotsPrevious[0]._itemId);
					}
					this.DropEquipedWeapon(!LocalPlayer.AnimControl.WaterBlock && (!flag || (!flag2 && !flag3)));
				}
				if (this._equipPreviousTime < Time.time && !LocalPlayer.AnimControl.upsideDown)
				{
					this.EquipPreviousWeapon();
				}
			}
			else if (this.CurrentView == PlayerInventory.PlayerViews.ClosingInventory)
			{
				this.CurrentView = PlayerInventory.PlayerViews.World;
			}
			this.RefreshDropIcon();
		}

		private void OnLevelWasLoaded()
		{
			if (Application.loadedLevelName == "TitleScene")
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		public void ToggleInventory()
		{
			if (this.CurrentView == PlayerInventory.PlayerViews.Inventory || LocalPlayer.Stats.Dead)
			{
				this.Close();
			}
			else if (!LocalPlayer.WaterViz.InWater)
			{
				this.Open(this._craftingCog);
			}
			else
			{
				LocalPlayer.Tuts.ShowNoInventoryUnderWater();
			}
		}

		public void Open(IItemStorage storage)
		{
			if (this.CurrentView != PlayerInventory.PlayerViews.Inventory || this.CurrentStorage != storage)
			{
				if (this.CurrentView == PlayerInventory.PlayerViews.Book)
				{
					LocalPlayer.Create.CloseBookForInventory();
				}
				this.CurrentStorage = storage;
				this.CurrentStorage.Open();
				this.CurrentView = PlayerInventory.PlayerViews.Inventory;
				LocalPlayer.Tuts.CloseCraftingTut();
				LocalPlayer.FpCharacter.LockView(true);
				Scene.HudGui.CheckHudState();
				this._inventoryGO.tag = "open";
				this._inventoryGO.SetActive(true);
				LocalPlayer.Sfx.PlayOpenInventory();
				base.Invoke("PauseTimeInInventory", 1f);
			}
		}

		private void PauseTimeInInventory()
		{
			if (this.CurrentView == PlayerInventory.PlayerViews.Inventory)
			{
				Time.timeScale = 0f;
				this._pauseSnapshot = FMODCommon.PlayOneshot("snapshot:/Inventory Pause", Vector3.zero, new object[0]);
			}
		}

		public void Close()
		{
			if (this.CurrentView == PlayerInventory.PlayerViews.Inventory || this._inventoryGO.activeSelf)
			{
				this.CurrentStorage.Close();
				this.CurrentStorage = null;
				this._inventoryGO.tag = "closed";
				this._inventoryGO.SetActive(false);
				Scene.HudGui.CheckHudState();
				LocalPlayer.Tuts.CloseRecipeTut();
				LocalPlayer.FpCharacter.UnLockView();
				this.CurrentView = PlayerInventory.PlayerViews.ClosingInventory;
				if (!string.IsNullOrEmpty(this.PendingSendMessage))
				{
					base.SendMessage(this.PendingSendMessage);
					this.PendingSendMessage = null;
				}
				LocalPlayer.Sfx.PlayCloseInventory();
				Time.timeScale = 1f;
				if (this._pauseSnapshot != null)
				{
					UnityUtil.ERRCHECK(this._pauseSnapshot.stop(STOP_MODE.ALLOWFADEOUT));
				}
			}
		}

		public void TogglePauseMenu()
		{
			if (this.CurrentView == PlayerInventory.PlayerViews.Pause)
			{
				LocalPlayer.FpCharacter.UnLockView();
				Scene.HudGui.CheckHudState();
				Scene.HudGui.PauseMenu.SetActive(false);
				this.CurrentView = PlayerInventory.PlayerViews.World;
				Time.timeScale = 1f;
				LocalPlayer.PauseMenuBlur.enabled = false;
				LocalPlayer.PauseMenuBlurPsCam.enabled = false;
				Scene.HudGui.CheckHudState();
			}
			else
			{
				LocalPlayer.FpCharacter.LockView(true);
				LocalPlayer.Create.CloseBuildMode();
				Scene.HudGui.PauseMenu.SetActive(true);
				this.CurrentView = PlayerInventory.PlayerViews.Pause;
				Time.timeScale = 0f;
				LocalPlayer.PauseMenuBlur.enabled = true;
				LocalPlayer.PauseMenuBlurPsCam.enabled = true;
				Scene.HudGui.GuiCam.SetActive(false);
			}
		}

		public void LockEquipmentSlot(Item.EquipmentSlot slot)
		{
			this._equipmentSlotsLocked[(int)slot] = true;
		}

		public void UnlockEquipmentSlot(Item.EquipmentSlot slot)
		{
			this._equipmentSlotsLocked[(int)slot] = false;
		}

		public bool IsSlotLocked(Item.EquipmentSlot slot)
		{
			return this._equipmentSlotsLocked[(int)slot];
		}

		public bool Equip(int itemId, bool pickedUpFromWorld)
		{
			if (itemId == this.Logs._logItemId)
			{
				if (this.Logs.Lift())
				{
					this.Equiped.Invoke(itemId);
					return true;
				}
				return false;
			}
			else
			{
				int num = this.AmountOf(itemId, false);
				if (pickedUpFromWorld)
				{
					num++;
					if (ItemDatabase.ItemById(itemId)._maxAmount > 0 && num > ItemDatabase.ItemById(itemId)._maxAmount)
					{
						return false;
					}
				}
				if (num > 0 && this.Equip(this._inventoryItemViewsCache[itemId][Mathf.Min(num, this._inventoryItemViewsCache[itemId].Count) - 1], pickedUpFromWorld))
				{
					this.Equiped.Invoke(itemId);
					return true;
				}
				return false;
			}
		}

		private bool Equip(InventoryItemView itemView, bool pickedUpFromWorld)
		{
			if (itemView != null)
			{
				this._equipPreviousTime = 3.40282347E+38f;
				Item.EquipmentSlot equipmentSlot = itemView.ItemCache._equipmentSlot;
				if ((this.Logs.HasLogs && equipmentSlot != Item.EquipmentSlot.LeftHand) || LocalPlayer.AnimControl.carry || !(itemView != this._equipmentSlots[(int)equipmentSlot]) || this.IsSlotLocked(equipmentSlot) || (itemView.ItemCache.MatchType(Item.Types.Special) && !this.SpecialItemsControlers[itemView._itemId].ToggleSpecial(true)))
				{
					return false;
				}
				if (pickedUpFromWorld || this.RemoveItem(itemView._itemId, 1, false))
				{
					this.LockEquipmentSlot(equipmentSlot);
					base.StartCoroutine(this.EquipSequence(equipmentSlot, itemView));
					return true;
				}
			}
			else
			{
				UnityEngine.Debug.LogWarning(base.name + " is trying to equip a null object");
			}
			return false;
		}

		[DebuggerHidden]
		private IEnumerator EquipSequence(Item.EquipmentSlot slot, InventoryItemView itemView)
		{
			PlayerInventory.<EquipSequence>c__Iterator168 <EquipSequence>c__Iterator = new PlayerInventory.<EquipSequence>c__Iterator168();
			<EquipSequence>c__Iterator.slot = slot;
			<EquipSequence>c__Iterator.itemView = itemView;
			<EquipSequence>c__Iterator.<$>slot = slot;
			<EquipSequence>c__Iterator.<$>itemView = itemView;
			<EquipSequence>c__Iterator.<>f__this = this;
			return <EquipSequence>c__Iterator;
		}

		public void MemorizeItem(Item.EquipmentSlot slot)
		{
			if (Mathf.Approximately(this._equipPreviousTime, 3.40282347E+38f))
			{
				this._equipmentSlotsPrevious[(int)slot] = this._equipmentSlots[(int)slot];
			}
			else
			{
				this._equipPreviousTime = 3.40282347E+38f;
			}
		}

		public void MemorizeOverrideItem(Item.EquipmentSlot slot)
		{
			if (Mathf.Approximately(this._equipPreviousTime, 3.40282347E+38f))
			{
				this._equipmentSlotsPreviousOverride[(int)slot] = this._equipmentSlots[(int)slot];
			}
			else
			{
				this._equipPreviousTime = 3.40282347E+38f;
			}
		}

		public void EquipPreviousUtility()
		{
			if (this._equipmentSlotsPrevious[1] != null && this._equipmentSlotsPrevious[1] != this._noEquipedItem)
			{
				this.Equip(this._equipmentSlotsPrevious[1]._itemId, false);
				this._equipmentSlotsPrevious[1] = null;
			}
		}

		public void EquipPreviousWeaponDelayed()
		{
			this._equipPreviousTime = Time.time + 0.45f;
		}

		public void CancelEquipPreviousWeaponDelayed()
		{
			this._equipPreviousTime = 3.40282347E+38f;
		}

		public void EquipPreviousWeapon()
		{
			if (!this.EquipPreviousOverride() && this._equipmentSlotsPrevious[0] != this._noEquipedItem)
			{
				this._equipPreviousTime = 3.40282347E+38f;
				if (this._equipmentSlotsPrevious[0] != null)
				{
					if (this.Equip(this._equipmentSlotsPrevious[0]._itemId, false))
					{
						this._equipmentSlotsPrevious[0] = null;
					}
					else
					{
						this.Equip(this._defaultWeaponItemId, false);
						this._equipmentSlotsPrevious[0] = this._inventoryItemViewsCache[this._defaultWeaponItemId][0];
					}
				}
				else
				{
					this.Equip(this._defaultWeaponItemId, false);
					this._equipmentSlotsPrevious[0] = this._inventoryItemViewsCache[this._defaultWeaponItemId][0];
				}
			}
		}

		private bool EquipPreviousOverride()
		{
			if (this._equipmentSlotsPreviousOverride[0] != this._noEquipedItem)
			{
				if (this.Equip(this._equipmentSlotsPreviousOverride[0]._itemId, false))
				{
					this._equipPreviousTime = 3.40282347E+38f;
					this._equipmentSlotsPreviousOverride[0] = this._noEquipedItem;
					return true;
				}
				this._equipmentSlotsPreviousOverride[0] = this._noEquipedItem;
			}
			return false;
		}

		public void DropEquipedWeapon(bool equipPrevious)
		{
			this.DroppedRightHand.Invoke();
			LocalPlayer.Animator.SetBoolReflected("lookAtItemRight", false);
			this.UnequipItemAtSlot(Item.EquipmentSlot.RightHand, true, false, equipPrevious);
		}

		public void StashEquipedWeapon(bool equipPrevious)
		{
			this.StashedRightHand.Invoke();
			this.UnequipItemAtSlot(Item.EquipmentSlot.RightHand, false, true, equipPrevious);
		}

		public void StashLeftHand()
		{
			this.StashedLeftHand.Invoke();
			if (this.HasInSlot(Item.EquipmentSlot.LeftHand, this.DefaultLight._itemId))
			{
				this.DefaultLight.StashLighter();
			}
			else
			{
				this.UnequipItemAtSlot(Item.EquipmentSlot.LeftHand, false, true, false);
			}
		}

		public void HideAllEquiped(bool hideOnly = false)
		{
			if (!hideOnly)
			{
				if (LocalPlayer.AnimControl.carry)
				{
					LocalPlayer.AnimControl.DropBody();
				}
				else if (this.Logs.HasLogs && this.Logs.PutDown(false, true, false))
				{
					this.Logs.PutDown(false, true, false);
				}
			}
			this.MemorizeItem(Item.EquipmentSlot.LeftHand);
			this.MemorizeItem(Item.EquipmentSlot.RightHand);
			this.StashLeftHand();
			this.StashEquipedWeapon(false);
		}

		public void ShowAllEquiped()
		{
			this.EquipPreviousUtility();
			this.EquipPreviousWeapon();
		}

		public void HideRightHand(bool hideOnly = false)
		{
			if (!hideOnly)
			{
				if (LocalPlayer.AnimControl.carry)
				{
					LocalPlayer.AnimControl.DropBody();
				}
				else if (this.Logs.HasLogs && this.Logs.PutDown(false, true, false))
				{
					this.Logs.PutDown(false, true, false);
				}
			}
			this.MemorizeItem(Item.EquipmentSlot.RightHand);
			this.StashEquipedWeapon(false);
		}

		public void ShowRightHand()
		{
			this.EquipPreviousWeapon();
		}

		public void Attack()
		{
			if (!this.IsRightHandEmpty() && !this._isThrowing && !this.IsSlotLocked(Item.EquipmentSlot.RightHand))
			{
				Item itemCache = this._equipmentSlots[0].ItemCache;
				if (itemCache._attackEvent != Item.FSMEvents.None)
				{
					LocalPlayer.Stats.UsedStick();
					this._pm.SendEvent(itemCache._attackEvent.ToString());
				}
				if (itemCache._attackSFX != Item.SFXCommands.None)
				{
					LocalPlayer.Sfx.SendMessage(itemCache._attackSFX.ToString());
				}
				if (itemCache.MatchType(Item.Types.Projectile))
				{
					this._isThrowing = true;
					base.Invoke("ThrowProjectile", itemCache._projectileThrowDelay);
					LocalPlayer.TargetFunctions.Invoke("sendPlayerAttacking", 0.5f);
					LocalPlayer.SpecialItems.SendMessage("stopLightHeldFire", SendMessageOptions.DontRequireReceiver);
				}
				else if (itemCache.MatchType(Item.Types.RangedWeapon))
				{
					if (itemCache.MatchRangedStyle(Item.RangedStyle.Instantaneous))
					{
						base.Invoke("FireRangedWeapon", itemCache._projectileThrowDelay);
					}
					else
					{
						this._weaponChargeStartTime = Time.time;
					}
				}
				this.Attacked.Invoke();
			}
		}

		public void ReleaseAttack()
		{
			if (!this.IsRightHandEmpty() && !this._isThrowing)
			{
				Item itemCache = this._equipmentSlots[0].ItemCache;
				if (this.CancelNextChargedAttack)
				{
					this.CancelNextChargedAttack = false;
					return;
				}
				if (itemCache.MatchType(Item.Types.RangedWeapon) && itemCache.MatchRangedStyle(Item.RangedStyle.Charged))
				{
					if (itemCache._attackReleaseEvent != Item.FSMEvents.None)
					{
						LocalPlayer.Stats.UsedStick();
						this._pm.SendEvent(itemCache._attackReleaseEvent.ToString());
					}
					this._isThrowing = true;
					base.Invoke("FireRangedWeapon", itemCache._projectileThrowDelay);
					this.ReleasedAttack.Invoke();
				}
			}
		}

		public void Block()
		{
			if (!this.IsRightHandEmpty() && !this._isThrowing)
			{
				Item itemCache = this._equipmentSlots[0].ItemCache;
				if (itemCache._blockEvent != Item.FSMEvents.None)
				{
					LocalPlayer.Stats.UsedStick();
					this._pm.SendEvent(itemCache._blockEvent.ToString());
					this.Blocked.Invoke();
				}
			}
		}

		public void UnBlock()
		{
			if (!this.IsRightHandEmpty())
			{
				Item itemCache = this._equipmentSlots[0].ItemCache;
				if (itemCache._unblockEvent != Item.FSMEvents.None)
				{
					LocalPlayer.Stats.UsedStick();
					this._pm.SendEvent(itemCache._unblockEvent.ToString());
					this.Unblocked.Invoke();
				}
			}
		}

		public bool AddItem(int itemId, int amount = 1, bool preventAutoEquip = false, bool fromCraftingCog = false, WeaponStatUpgrade.Types activeBonus = (WeaponStatUpgrade.Types)(-2))
		{
			if (this.ItemFilter != null)
			{
				return this.ItemFilter.AddItem(itemId, amount, preventAutoEquip, fromCraftingCog, activeBonus);
			}
			return this.AddItemNF(itemId, amount, preventAutoEquip, fromCraftingCog, activeBonus);
		}

		public bool AddItemNF(int itemId, int amount = 1, bool preventAutoEquip = false, bool fromCraftingCog = false, WeaponStatUpgrade.Types activeBonus = (WeaponStatUpgrade.Types)(-2))
		{
			if (this.Logs != null && itemId == this.Logs._logItemId)
			{
				return this.Logs.Lift();
			}
			Item item = ItemDatabase.ItemById(itemId);
			if (item._maxAmount >= 0)
			{
				if (amount < 1)
				{
					return true;
				}
				if (!Application.isPlaying || fromCraftingCog || Mathf.Approximately(item._weight, 0f) || item._weight + LocalPlayer.Stats.CarriedWeight.CurrentWeight <= 1f)
				{
					InventoryItem inventoryItem;
					if (!this._possessedItemCache.ContainsKey(itemId))
					{
						inventoryItem = new InventoryItem
						{
							_itemId = itemId,
							_maxAmount = (item._maxAmount != 0) ? item._maxAmount : 2147483647
						};
						this._possessedItems.Add(inventoryItem);
						this._possessedItemCache[itemId] = inventoryItem;
					}
					else
					{
						inventoryItem = this._possessedItemCache[itemId];
					}
					if (Application.isPlaying)
					{
						int num = inventoryItem.Add(amount, this.HasInSlot(item._equipmentSlot, itemId));
						if (num < amount && !fromCraftingCog)
						{
							this.RefreshCurrentWeight();
							Scene.HudGui.ToggleGotItemHud(itemId, amount - num);
						}
						if (num > 0)
						{
							Scene.HudGui.ToggleFullCapacityHud(itemId);
							if (num == amount && !fromCraftingCog)
							{
								return false;
							}
						}
						if (item.MatchType(Item.Types.Special) && this.SpecialItemsControlers.ContainsKey(itemId))
						{
							this.SpecialItemsControlers[itemId].PickedUpSpecialItem(itemId);
						}
						if (preventAutoEquip || LocalPlayer.AnimControl.swimming || !item.MatchType(Item.Types.Equipment) || (!(this._equipmentSlots[(int)item._equipmentSlot] == null) && !(this._equipmentSlots[0] == this._noEquipedItem)) || !this.Equip(itemId, false))
						{
							EventRegistry.Player.Publish(TfEvent.AddedItem, itemId);
						}
						this.ToggleInventoryItemView(itemId, activeBonus, false);
						if (this.SkipNextAddItemWoosh)
						{
							this.SkipNextAddItemWoosh = false;
						}
						else
						{
							LocalPlayer.Sfx.PlayWhoosh();
						}
					}
					else
					{
						inventoryItem.Add(amount, false);
					}
					return true;
				}
				Scene.HudGui.ToggleFullWeightHud(itemId);
				return false;
			}
			else
			{
				if (item.MatchType(Item.Types.Equipment) && !LocalPlayer.AnimControl.swimming)
				{
					bool flag = this._equipmentSlots[(int)item._equipmentSlot] == null || this._equipmentSlots[0] == this._noEquipedItem || this._equipmentSlots[(int)item._equipmentSlot]._itemId != itemId;
					if (flag && !this.Logs.HasLogs)
					{
						if (this.Equip(itemId, true))
						{
							EventRegistry.Player.Publish(TfEvent.AddedItem, itemId);
							return true;
						}
					}
					else if (this.Logs.HasLogs)
					{
						this.Logs.PutDown(false, true, false);
						this.Logs.PutDown(false, true, false);
						return this.Equip(itemId, true);
					}
					return false;
				}
				return false;
			}
		}

		public void RefreshCurrentWeight()
		{
		}

		public bool RemoveItem(int itemId, int amount = 1, bool allowAmountOverflow = false)
		{
			if (this.ItemFilter != null)
			{
				return this.ItemFilter.RemoveItem(itemId, amount, allowAmountOverflow);
			}
			return this.RemoveItemNF(itemId, amount, allowAmountOverflow);
		}

		public bool RemoveItemNF(int itemId, int amount = 1, bool allowAmountOverflow = false)
		{
			InventoryItem inventoryItem;
			if (this.Logs != null && itemId == this.Logs._logItemId)
			{
				if (this.Logs.PutDown(false, false, true))
				{
					if (this.RemovedItem != null)
					{
						this.RemovedItem.Invoke(itemId);
					}
					return true;
				}
			}
			else if (this._possessedItemCache.TryGetValue(itemId, out inventoryItem))
			{
				if (inventoryItem.Remove(amount))
				{
					if (Application.isPlaying)
					{
						this.RefreshCurrentWeight();
						this.ToggleInventoryItemView(itemId, (WeaponStatUpgrade.Types)(-2), false);
					}
					if (inventoryItem._amount == 0 && inventoryItem._maxAmountBonus == 0)
					{
						this._possessedItems.Remove(inventoryItem);
						this._possessedItemCache.Remove(itemId);
					}
					return true;
				}
				if (allowAmountOverflow)
				{
					if (this.HasInSlot(Item.EquipmentSlot.RightHand, itemId))
					{
						this.UnequipItemAtSlot(Item.EquipmentSlot.RightHand, false, false, true);
						this.RemovedItem.Invoke(itemId);
					}
					if (inventoryItem._maxAmountBonus == 0)
					{
						this._possessedItems.Remove(inventoryItem);
						this._possessedItemCache.Remove(itemId);
					}
					this.ToggleInventoryItemView(itemId, (WeaponStatUpgrade.Types)(-2), false);
					this.RefreshCurrentWeight();
				}
			}
			else
			{
				if (amount == 1 && this.HasInSlot(Item.EquipmentSlot.RightHand, itemId))
				{
					this.UnequipItemAtSlot(Item.EquipmentSlot.RightHand, false, false, true);
					this.RemovedItem.Invoke(itemId);
					this.RefreshCurrentWeight();
					return true;
				}
				if (amount == 1 && this.HasInSlot(Item.EquipmentSlot.LeftHand, itemId))
				{
					this.UnequipItemAtSlot(Item.EquipmentSlot.LeftHand, false, false, true);
					this.RemovedItem.Invoke(itemId);
					this.RefreshCurrentWeight();
					return true;
				}
				Item item = ItemDatabase.ItemById(itemId);
				if (item._fallbackItemIds.Length > 0)
				{
					for (int i = 0; i < item._fallbackItemIds.Length; i++)
					{
						if (this.RemoveItem(item._fallbackItemIds[i], amount, allowAmountOverflow))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public bool IsSlotEmpty(Item.EquipmentSlot slot)
		{
			return this._equipmentSlots[(int)slot] == null || this._equipmentSlots[(int)slot] == this._noEquipedItem;
		}

		public bool IsRightHandEmpty()
		{
			return this.IsSlotEmpty(Item.EquipmentSlot.RightHand);
		}

		public bool IsLeftHandEmpty()
		{
			return this.IsSlotEmpty(Item.EquipmentSlot.LeftHand);
		}

		public bool HasInSlot(Item.EquipmentSlot slot, int itemId)
		{
			return this._equipmentSlots[(int)slot] != null && this._equipmentSlots[(int)slot]._itemId == itemId;
		}

		public bool HasInPreviousSlot(Item.EquipmentSlot slot, int itemId)
		{
			return this._equipmentSlotsPrevious[(int)slot] != null && this._equipmentSlotsPrevious[(int)slot]._itemId == itemId;
		}

		public bool HasInNextSlot(Item.EquipmentSlot slot, int itemId)
		{
			return this._equipmentSlotsNext[(int)slot] != null && this._equipmentSlotsNext[(int)slot]._itemId == itemId;
		}

		public bool Owns(int itemId)
		{
			if (this.ItemFilter != null)
			{
				return this.ItemFilter.Owns(itemId);
			}
			return this.OwnsNF(itemId);
		}

		public bool OwnsNF(int itemId)
		{
			if (this.Logs != null && itemId == this.Logs._logItemId)
			{
				return this.Logs.HasLogs;
			}
			bool flag = this._possessedItemCache.ContainsKey(itemId) || this.HasInSlot(Item.EquipmentSlot.RightHand, itemId) || this.HasInSlot(Item.EquipmentSlot.LeftHand, itemId) || this.HasInSlot(Item.EquipmentSlot.Chest, itemId) || this.HasInSlot(Item.EquipmentSlot.Feet, itemId);
			if (!flag)
			{
				Item item = ItemDatabase.ItemById(itemId);
				if (item._fallbackItemIds.Length > 0)
				{
					for (int i = 0; i < item._fallbackItemIds.Length; i++)
					{
						if (this.Owns(item._fallbackItemIds[i]))
						{
							return true;
						}
					}
				}
			}
			return flag;
		}

		public int AmountOf(int itemId, bool allowFallback = true)
		{
			if (this.ItemFilter != null)
			{
				return this.ItemFilter.AmountOf(itemId, allowFallback);
			}
			return this.AmountOfNF(itemId, allowFallback);
		}

		public int AmountOfNF(int itemId, bool allowFallback = true)
		{
			if (this.Logs != null && itemId == this.Logs._logItemId)
			{
				return this.Logs.Amount;
			}
			int num = 0;
			if (this._possessedItemCache.ContainsKey(itemId))
			{
				num = this._possessedItemCache[itemId]._amount;
			}
			if (this.HasInSlot(Item.EquipmentSlot.RightHand, itemId) || this.HasInSlot(Item.EquipmentSlot.LeftHand, itemId) || this.HasInSlot(Item.EquipmentSlot.Chest, itemId) || this.HasInSlot(Item.EquipmentSlot.Feet, itemId))
			{
				num++;
			}
			if (num == 0 && allowFallback)
			{
				Item item = ItemDatabase.ItemById(itemId);
				if (item._fallbackItemIds.Length > 0)
				{
					for (int i = 0; i < item._fallbackItemIds.Length; i++)
					{
						num += this.AmountOf(item._fallbackItemIds[i], false);
					}
				}
			}
			return num;
		}

		public void AddMaxAmountBonus(int itemId, int amount)
		{
			if (this.Logs != null && itemId != this.Logs._logItemId)
			{
				InventoryItem inventoryItem;
				if (!this._possessedItemCache.ContainsKey(itemId))
				{
					Item item = ItemDatabase.ItemById(itemId);
					inventoryItem = new InventoryItem
					{
						_itemId = itemId,
						_maxAmount = (item._maxAmount != 0) ? item._maxAmount : 2147483647
					};
					this._possessedItems.Add(inventoryItem);
					this._possessedItemCache[itemId] = inventoryItem;
				}
				else
				{
					inventoryItem = this._possessedItemCache[itemId];
				}
				inventoryItem._maxAmountBonus += amount;
			}
		}

		public void SetMaxAmountBonus(int itemId, int amount)
		{
			if (this.Logs != null && itemId != this.Logs._logItemId)
			{
				InventoryItem inventoryItem;
				if (!this._possessedItemCache.ContainsKey(itemId))
				{
					Item item = ItemDatabase.ItemById(itemId);
					inventoryItem = new InventoryItem
					{
						_itemId = itemId,
						_maxAmount = (item._maxAmount != 0) ? item._maxAmount : 2147483647
					};
					this._possessedItems.Add(inventoryItem);
					this._possessedItemCache[itemId] = inventoryItem;
				}
				else
				{
					inventoryItem = this._possessedItemCache[itemId];
				}
				inventoryItem._maxAmountBonus = amount;
			}
		}

		public int GetMaxAmountOf(int itemId)
		{
			if (this._possessedItemCache.ContainsKey(itemId))
			{
				return this._possessedItemCache[itemId].MaxAmount;
			}
			int maxAmount = ItemDatabase.ItemById(itemId)._maxAmount;
			return (maxAmount != 0) ? maxAmount : 2147483647;
		}

		public void TurnOffLastLight()
		{
			if (this.HasInSlot(Item.EquipmentSlot.LeftHand, this.LastLight._itemId))
			{
				this.StashLeftHand();
			}
		}

		public void TurnOnLastLight()
		{
			if (!this.HasInSlot(Item.EquipmentSlot.LeftHand, this.LastLight._itemId))
			{
				if (!this.Equip(this.LastLight._itemId, false))
				{
					this.LastLight = this.DefaultLight;
					this.Equip(this.LastLight._itemId, false);
				}
				LocalPlayer.Tuts.HideLighter();
			}
		}

		public void TurnOffLastUtility()
		{
			if (this.HasInSlot(Item.EquipmentSlot.LeftHand, this.LastUtility._itemId))
			{
				this.StashLeftHand();
			}
		}

		public void TurnOnLastUtility()
		{
			if (!this.HasInSlot(Item.EquipmentSlot.LeftHand, this.LastUtility._itemId))
			{
				this.Equip(this.LastUtility._itemId, false);
			}
		}

		public void BloodyWeapon()
		{
			InventoryItemView[] equipmentSlots = this._equipmentSlots;
			for (int i = 0; i < equipmentSlots.Length; i++)
			{
				InventoryItemView inventoryItemView = equipmentSlots[i];
				if (inventoryItemView != null && inventoryItemView._held != null)
				{
					inventoryItemView._held.SendMessage("GotBloody", SendMessageOptions.DontRequireReceiver);
				}
			}
		}

		public void CleanWeapon()
		{
			InventoryItemView[] equipmentSlots = this._equipmentSlots;
			for (int i = 0; i < equipmentSlots.Length; i++)
			{
				InventoryItemView inventoryItemView = equipmentSlots[i];
				if (inventoryItemView != null && inventoryItemView._held != null)
				{
					inventoryItemView._held.SendMessage("GotClean", SendMessageOptions.DontRequireReceiver);
				}
			}
			Bloodify[] weaponBlood = LocalPlayer.HeldItemsData._weaponBlood;
			for (int j = 0; j < weaponBlood.Length; j++)
			{
				Bloodify bloodify = weaponBlood[j];
				if (bloodify)
				{
					bloodify.GotClean();
				}
			}
			BurnableCloth[] weaponFire = LocalPlayer.HeldItemsData._weaponFire;
			for (int k = 0; k < weaponFire.Length; k++)
			{
				BurnableCloth burnableCloth = weaponFire[k];
				if (burnableCloth)
				{
					burnableCloth.GotClean();
				}
			}
		}

		public void GotLeaf()
		{
			this.AddItem(this._leafItemId, UnityEngine.Random.Range(1, 3), false, false, (WeaponStatUpgrade.Types)(-2));
		}

		public void GotSap(int? amount = null)
		{
			int num = (!amount.HasValue) ? UnityEngine.Random.Range(-1, 3) : amount.Value;
			if (num > 0)
			{
				this.AddItem(this._sapItemId, num, false, false, (WeaponStatUpgrade.Types)(-2));
			}
		}

		public void BubbleUpInventoryView(InventoryItemView view)
		{
			int num = Mathf.Max(this.AmountOf(view._itemId, false), 1);
			List<InventoryItemView> list = this._inventoryItemViewsCache[view._itemId];
			if (num <= list.Count)
			{
				list.Remove(view);
				list.Insert(num - 1, view);
			}
		}

		public void BubbleDownInventoryView(InventoryItemView view)
		{
			List<InventoryItemView> list = this._inventoryItemViewsCache[view._itemId];
			list.Remove(view);
			list.Insert(0, view);
		}

		public void ShuffleInventoryView(InventoryItemView view)
		{
			List<InventoryItemView> list = this._inventoryItemViewsCache[view._itemId];
			int num = Mathf.Min(Mathf.Max(this.AmountOf(view._itemId, false), 1), list.Count) - 1;
			int num2 = list.IndexOf(view);
			if (num2 == num)
			{
				list.Remove(view);
				if (num > 0)
				{
					list.Insert(0, view);
				}
				else
				{
					list.Insert(list.Count, view);
				}
				this.ToggleInventoryItemView(view._itemId, (WeaponStatUpgrade.Types)(-2), false);
			}
		}

		public bool ShuffleRemoveRightHandItem()
		{
			if (!this.IsRightHandEmpty())
			{
				if (LocalPlayer.Inventory.AmountOf(this.RightHand._itemId, true) == 1)
				{
					LocalPlayer.Inventory.UnequipItemAtSlot(Item.EquipmentSlot.RightHand, false, false, true);
				}
				else
				{
					LocalPlayer.Inventory.MemorizeItem(Item.EquipmentSlot.RightHand);
					LocalPlayer.Inventory.ShuffleInventoryView(LocalPlayer.Inventory.RightHand);
					LocalPlayer.Inventory.UnequipItemAtSlot(Item.EquipmentSlot.RightHand, false, false, true);
				}
				return true;
			}
			return false;
		}

		public void SortInventoryViewsByBonus(InventoryItemView view, WeaponStatUpgrade.Types activeBonus, bool setTargetViewFirst)
		{
			List<InventoryItemView> list = this._inventoryItemViewsCache[view._itemId];
			int num = Mathf.Max(this.AmountOf(view._itemId, false), 1);
			int num2 = this.AmountOfItemWithBonus(view._itemId, activeBonus);
			int num3 = 0;
			int count = list.Count;
			for (int i = count - 1; i >= 0; i--)
			{
				InventoryItemView inventoryItemView = list[i + num3];
				if (inventoryItemView._activeBonus != activeBonus && inventoryItemView.gameObject.activeSelf)
				{
					list.RemoveAt(i + num3++);
					list.Insert(0, inventoryItemView);
				}
			}
			if (setTargetViewFirst && num <= list.Count)
			{
				list.Remove(view);
				list.Insert(num - 1, view);
			}
		}

		public bool OwnsItemWithBonus(int itemId, WeaponStatUpgrade.Types bonus)
		{
			List<InventoryItemView> list = this._inventoryItemViewsCache[itemId];
			for (int i = list.Count - 1; i >= 0; i--)
			{
				InventoryItemView inventoryItemView = list[i];
				if (inventoryItemView.gameObject.activeSelf && inventoryItemView.ActiveBonus == bonus)
				{
					return true;
				}
			}
			return false;
		}

		public int AmountOfItemWithBonus(int itemId, WeaponStatUpgrade.Types bonus)
		{
			int num = 0;
			List<InventoryItemView> list = this._inventoryItemViewsCache[itemId];
			if (list[0].ItemCache._maxAmount > 0 && list[0].ItemCache._maxAmount <= list.Count)
			{
				for (int i = list.Count - 1; i >= 0; i--)
				{
					InventoryItemView inventoryItemView = list[i];
					if (inventoryItemView.gameObject.activeSelf && inventoryItemView.ActiveBonus == bonus)
					{
						num++;
					}
				}
			}
			else
			{
				num = this.AmountOf(itemId, false);
			}
			return num;
		}

		public void AddUpgradeToCounter(int itemId, int upgradeItemId, int amount)
		{
			if (!this.ItemsUpgradeCounters.ContainsKey(itemId))
			{
				this.ItemsUpgradeCounters[itemId] = new PlayerInventory.UpgradeCounterDict();
			}
			int num;
			this.ItemsUpgradeCounters[itemId].TryGetValue(upgradeItemId, out num);
			this.ItemsUpgradeCounters[itemId][upgradeItemId] = num + amount;
		}

		public int GetAmountOfUpgrades(int itemId)
		{
			if (this.ItemsUpgradeCounters.ContainsKey(itemId))
			{
				return this.ItemsUpgradeCounters[itemId].Values.Sum();
			}
			return 0;
		}

		public int GetAmountOfUpgrades(int itemId, int upgradeItemId)
		{
			if (this.ItemsUpgradeCounters.ContainsKey(itemId) && this.ItemsUpgradeCounters[itemId].ContainsKey(upgradeItemId))
			{
				return this.ItemsUpgradeCounters[itemId][upgradeItemId];
			}
			return 0;
		}

		public void GatherWater(bool clean)
		{
			InventoryItemView inventoryItemView = this._equipmentSlots[0];
			inventoryItemView.ActiveBonus = ((!clean) ? WeaponStatUpgrade.Types.DirtyWater : WeaponStatUpgrade.Types.CleanWater);
			inventoryItemView.ActiveBonusValue = 2f;
		}

		[DebuggerHidden]
		private IEnumerator OnSerializing()
		{
			PlayerInventory.<OnSerializing>c__Iterator169 <OnSerializing>c__Iterator = new PlayerInventory.<OnSerializing>c__Iterator169();
			<OnSerializing>c__Iterator.<>f__this = this;
			return <OnSerializing>c__Iterator;
		}

		private void ToggleInventoryItemView(int itemId, WeaponStatUpgrade.Types activeBonus = (WeaponStatUpgrade.Types)(-2), bool forceInit = false)
		{
			if (this._inventoryItemViewsCache.ContainsKey(itemId))
			{
				int num = this.AmountOf(itemId, false);
				List<InventoryItemView> list = this._inventoryItemViewsCache[itemId];
				for (int i = 0; i < list.Count; i++)
				{
					GameObject gameObject = list[i].gameObject;
					bool flag = i < num;
					if (gameObject.activeSelf != flag || forceInit)
					{
						gameObject.SetActive(flag);
						if (flag)
						{
							list[i].OnItemAdded();
						}
						else
						{
							list[i].OnItemRemoved();
						}
						if (activeBonus != (WeaponStatUpgrade.Types)(-2))
						{
							list[i].ActiveBonus = activeBonus;
						}
					}
				}
			}
		}

		private void ThrowProjectile()
		{
			this._isThrowing = false;
			InventoryItemView inventoryItemView = this._equipmentSlots[0];
			if (inventoryItemView)
			{
				Item itemCache = inventoryItemView.ItemCache;
				bool flag = itemCache._maxAmount < 0;
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate((!this.UseAltProjectile) ? inventoryItemView._worldPrefab : inventoryItemView._altWorldPrefab, inventoryItemView._held.transform.position, inventoryItemView._held.transform.rotation);
				Rigidbody component = gameObject.GetComponent<Rigidbody>();
				Collider component2 = gameObject.GetComponent<Collider>();
				if (BoltNetwork.isRunning)
				{
					BoltEntity component3 = gameObject.GetComponent<BoltEntity>();
					if (component3)
					{
						BoltNetwork.Attach(gameObject);
					}
				}
				if (inventoryItemView.ActiveBonus == WeaponStatUpgrade.Types.StickyProjectile)
				{
					if (component2)
					{
						gameObject.AddComponent<StickyBomb>();
					}
					else
					{
						Collider componentInChildren = gameObject.GetComponentInChildren<Collider>();
						if (componentInChildren)
						{
							componentInChildren.gameObject.AddComponent<StickyBomb>();
						}
					}
				}
				component.AddForce((float)itemCache._projectileThrowForceRange * LocalPlayer.MainCamTr.forward);
				inventoryItemView._held.SendMessage("OnProjectileThrown", gameObject, SendMessageOptions.DontRequireReceiver);
				inventoryItemView.ActiveBonus = (WeaponStatUpgrade.Types)(-1);
				if (itemCache._rangedStyle == Item.RangedStyle.Bell)
				{
					component.AddTorque((float)itemCache._projectileThrowTorqueRange * base.transform.forward);
				}
				else if (itemCache._rangedStyle == Item.RangedStyle.Forward)
				{
					component.AddTorque((float)itemCache._projectileThrowTorqueRange * LocalPlayer.MainCamTr.forward);
				}
				if (base.transform.GetComponent<Collider>().enabled && component2 && component2.enabled)
				{
					Physics.IgnoreCollision(base.transform.GetComponent<Collider>(), component2);
				}
				if (!flag)
				{
					this.MemorizeOverrideItem(Item.EquipmentSlot.RightHand);
				}
				this.UnequipItemAtSlot(itemCache._equipmentSlot, false, false, true);
				LocalPlayer.Sfx.PlayThrow();
			}
		}

		private void FireRangedWeapon()
		{
			InventoryItemView inventoryItemView = this._equipmentSlots[0];
			Item itemCache = inventoryItemView.ItemCache;
			bool flag = itemCache._maxAmount < 0;
			if (flag || this.RemoveItem(itemCache._ammoItemId, 1, false))
			{
				InventoryItemView inventoryItemView2 = this._inventoryItemViewsCache[itemCache._ammoItemId][0];
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(inventoryItemView2._worldPrefab, inventoryItemView2._held.transform.position, inventoryItemView2._held.transform.rotation);
				if (gameObject.GetComponent<Rigidbody>())
				{
					if (itemCache.MatchRangedStyle(Item.RangedStyle.Shoot))
					{
						gameObject.GetComponent<Rigidbody>().AddForce(gameObject.transform.TransformDirection(Vector3.forward * (float)itemCache._projectileThrowForceRange), ForceMode.VelocityChange);
					}
					else
					{
						float num = Time.time - this._weaponChargeStartTime;
						gameObject.GetComponent<Rigidbody>().AddForce(inventoryItemView2._held.transform.up * Mathf.Clamp01(num / itemCache._projectileMaxChargeDuration) * (float)itemCache._projectileThrowForceRange);
					}
					inventoryItemView._held.SendMessage("OnAmmoFired", gameObject, SendMessageOptions.DontRequireReceiver);
				}
				if (itemCache._attackReleaseSFX != Item.SFXCommands.None)
				{
					LocalPlayer.Sfx.SendMessage(itemCache._attackReleaseSFX.ToString());
				}
			}
			else if (itemCache._dryFireSFX != Item.SFXCommands.None)
			{
				LocalPlayer.Sfx.SendMessage(itemCache._dryFireSFX.ToString());
			}
			if (flag)
			{
				this.UnequipItemAtSlot(itemCache._equipmentSlot, false, false, flag);
			}
			else
			{
				this.ToggleAmmo(inventoryItemView, true);
			}
			this._weaponChargeStartTime = 0f;
			this._isThrowing = false;
		}

		public void UnequipItemAtSlot(Item.EquipmentSlot slot, bool drop, bool stash, bool equipPrevious)
		{
			if (!this.IsSlotEmpty(slot))
			{
				InventoryItemView inventoryItemView = this._equipmentSlots[(int)slot];
				Item itemCache = inventoryItemView.ItemCache;
				this.UnlockEquipmentSlot(slot);
				bool useAltProjectile = this.UseAltProjectile;
				inventoryItemView._held.SetActive(false);
				inventoryItemView.ApplyEquipmentEffect(false);
				if (inventoryItemView.ItemCache.MatchType(Item.Types.Special))
				{
					this.SpecialItemsControlers[inventoryItemView._itemId].ToggleSpecial(false);
				}
				this._itemAnimHash.ApplyAnimVars(itemCache, false);
				this._equipmentSlots[(int)slot] = ((this._equipmentSlots[(int)slot]._itemId == this._defaultWeaponItemId) ? this._noEquipedItem : null);
				if ((drop && itemCache.MatchType(Item.Types.WorldObject)) || (stash && inventoryItemView.IsHeldOnly))
				{
					GameObject worldGo;
					if (!BoltNetwork.isRunning || !itemCache._pickupPrefabMP)
					{
						worldGo = (GameObject)UnityEngine.Object.Instantiate((!useAltProjectile && inventoryItemView._worldPrefab) ? inventoryItemView._worldPrefab : ((!itemCache._pickupPrefab) ? inventoryItemView._altWorldPrefab : itemCache._pickupPrefab.gameObject), inventoryItemView._held.transform.position, inventoryItemView._held.transform.rotation);
					}
					else
					{
						worldGo = BoltNetwork.Instantiate((!useAltProjectile && inventoryItemView._worldPrefab && inventoryItemView._worldPrefab.GetComponent<BoltEntity>()) ? inventoryItemView._worldPrefab : itemCache._pickupPrefabMP.gameObject, inventoryItemView._held.transform.position, inventoryItemView._held.transform.rotation);
					}
					inventoryItemView.OnItemDropped(worldGo);
				}
				else if ((stash && itemCache._maxAmount >= 0) || (drop && !itemCache.MatchType(Item.Types.WorldObject)))
				{
					this.AddItem(inventoryItemView._itemId, 1, true, true, (WeaponStatUpgrade.Types)(-2));
					if (inventoryItemView.ItemCache._stashSFX != Item.SFXCommands.None)
					{
						LocalPlayer.Sfx.SendMessage(inventoryItemView.ItemCache._stashSFX.ToString());
					}
				}
				if (inventoryItemView.ItemCache._maxAmount >= 0)
				{
					this.ToggleAmmo(inventoryItemView, false);
					this.ToggleInventoryItemView(inventoryItemView._itemId, (WeaponStatUpgrade.Types)(-2), false);
				}
				if (equipPrevious)
				{
					this.EquipPreviousWeaponDelayed();
				}
			}
		}

		public void FakeDrop(int itemId)
		{
			if (itemId == this.Logs._logItemId)
			{
				this.Logs.PutDown(true, true, true);
			}
			else if (this._inventoryItemViewsCache.ContainsKey(itemId))
			{
				InventoryItemView itemView = this._inventoryItemViewsCache[itemId][0];
				this.FakeDrop(itemView, false);
			}
		}

		public void FakeDrop(InventoryItemView itemView, bool sendOnDropEvent)
		{
			LocalPlayer.Sfx.PlayWhoosh();
			if (itemView.ItemCache._pickupPrefab || itemView._worldPrefab)
			{
				GameObject gameObject = (!itemView._held) ? this._inventoryItemViewsCache[this._defaultWeaponItemId][0]._held : itemView._held;
				if (BoltNetwork.isRunning)
				{
					BoltEntity component = ((!itemView.ItemCache._pickupPrefabMP) ? itemView._worldPrefab : itemView.ItemCache._pickupPrefabMP.gameObject).GetComponent<BoltEntity>();
					if (component)
					{
						DropItem dropItem = DropItem.Raise(GlobalTargets.OnlyServer);
						dropItem.PrefabId = component.prefabId;
						dropItem.Position = gameObject.transform.position;
						dropItem.Rotation = gameObject.transform.rotation;
						dropItem.Send();
					}
					else
					{
						GameObject worldGo = (GameObject)UnityEngine.Object.Instantiate((!itemView.ItemCache._pickupPrefab) ? itemView._worldPrefab : itemView.ItemCache._pickupPrefab.gameObject, gameObject.transform.position, gameObject.transform.rotation);
						if (sendOnDropEvent)
						{
							itemView.OnItemDropped(worldGo);
						}
					}
				}
				else
				{
					GameObject worldGo2 = (GameObject)UnityEngine.Object.Instantiate((!itemView.ItemCache._pickupPrefab) ? itemView._worldPrefab : itemView.ItemCache._pickupPrefab.gameObject, gameObject.transform.position, gameObject.transform.rotation);
					if (sendOnDropEvent)
					{
						itemView.OnItemDropped(worldGo2);
					}
				}
			}
			else
			{
				UnityEngine.Debug.LogWarning(ItemDatabase.ItemById(itemView._itemId)._name + " pickup settings are incorrect (missing prevent fake drop or destroy if full)");
			}
		}

		public void ToggleAmmo(int ammoItemId, bool enable)
		{
			this._inventoryItemViewsCache[ammoItemId][0]._held.SetActive(enable && this.Owns(ammoItemId));
		}

		public void ToggleAmmo(InventoryItemView itemView, bool enable)
		{
			if (itemView.ItemCache.MatchType(Item.Types.RangedWeapon))
			{
				this._inventoryItemViewsCache[itemView.ItemCache._ammoItemId][0]._held.SetActive(enable && this.Owns(itemView.ItemCache._ammoItemId));
			}
		}

		public void InitItemCache()
		{
			this._itemDatabase.OnEnable();
			this._possessedItemCache = this._possessedItems.ToDictionary((InventoryItem i) => i._itemId);
			if (this._itemViews != null)
			{
				this._inventoryItemViewsCache = (from i in this._itemViews
				where i._itemId > 0
				select i into iv
				group iv by iv._itemId).ToDictionary((IGrouping<int, InventoryItemView> g) => g.Key, (IGrouping<int, InventoryItemView> g) => g.ToList<InventoryItemView>());
			}
			this.ItemsUpgradeCounters = new PlayerInventory.ItemsUpgradeCountersDict();
			if (Application.isPlaying)
			{
				this._craftingCog.Awake();
			}
		}

		private void RefreshDropIcon()
		{
			bool flag = (this.Logs.Amount > 0 || LocalPlayer.AnimControl.carry || (!this.IsRightHandEmpty() && this._equipmentSlots[0].ItemCache._maxAmount < 0)) && !this.DontShowDrop && this.CurrentView == PlayerInventory.PlayerViews.World;
			if (Scene.HudGui.DropButton.activeSelf != flag)
			{
				Scene.HudGui.DropButton.SetActive(flag);
			}
		}
	}
}
