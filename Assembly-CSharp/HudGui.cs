using PathologicalGames;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TheForest.Buildings.Creation;
using TheForest.Items;
using TheForest.Items.Craft;
using TheForest.Items.Inventory;
using TheForest.Items.UI;
using TheForest.Player;
using TheForest.UI;
using TheForest.UI.Crafting;
using TheForest.UI.Multiplayer;
using TheForest.Utils;
using UniLinq;
using UnityEngine;

public class HudGui : MonoBehaviour
{
	[Serializable]
	public class InventoryItemInfo
	{
		public enum LeftClickCommands
		{
			none,
			equip,
			play,
			drink,
			read,
			take,
			Charge_Flashlight,
			wear,
			eat,
			select
		}

		public enum RightClickCommands
		{
			none,
			combine,
			remove,
			unequip
		}

		public enum AmountDisplay
		{
			none,
			Amount,
			Pedometer,
			Battery,
			Air,
			WaterFill
		}

		[ItemIdPicker]
		public int _itemId;

		public Texture _icon;

		public string _titleText;

		public string _titlePluralText;

		public string _effectText;

		public string _descriptionText;

		public HudGui.InventoryItemInfo.LeftClickCommands _leftClick;

		public HudGui.InventoryItemInfo.RightClickCommands _rightClick;

		public HudGui.InventoryItemInfo.AmountDisplay _amountDisplay;

		public bool _showGotItem;

		public bool _showCollectedItem;

		public bool _showCantCarryMoreItem;
	}

	[Serializable]
	public class InventoryItemInfoView
	{
		[HideInInspector]
		public int _itemId;

		public GameObject _root;

		public UITexture _icon;

		public UILabel _title;

		public UILabel _effect;

		public UILabel _description;

		public UILabel _weight;

		public ItemAmountLabel _amountText;

		public UISprite _leftClickIcon;

		public UISprite _rightClickIcon;

		public UILabel _leftClickText;

		public UILabel _rightClickText;

		public UIGrid _upgradeCounterGrid;

		[NameFromProperty("_root")]
		public List<HudGui.UpgradeCounterView> _upgradeCounterViews;

		[NameFromProperty("_stat")]
		public List<HudGui.UsableUpgradeView> _usableUpgradeViews;

		public UIGrid _upgradeBonusGrid;

		public HudGui.UpgradeBonusView _speedBonusView;

		public HudGui.UpgradeBonusView _damageBonusView;

		public HudGui.UpgradeBonusView _blockView;

		public int ViewCounter
		{
			get;
			set;
		}

		public bool IsCraft
		{
			get;
			set;
		}
	}

	[Serializable]
	public class UpgradeCounterView
	{
		[ItemIdPicker]
		public int _itemId;

		public GameObject _root;

		public UILabel _label;
	}

	[Serializable]
	public class UsableUpgradeView
	{
		public WeaponStatUpgrade.Types _stat;

		public GameObject _root;
	}

	[Serializable]
	public class UpgradeBonusView
	{
		public GameObject _root;

		public UISprite _amount;
	}

	[Serializable]
	public class UpgradeBonusPreview
	{
		public GameObject _root;

		public UISprite _amountLeft;

		public UISprite _amountRight;

		public Color _base;

		public Color _negative;

		public Color _positive;
	}

	private class TimedGameObject
	{
		public int _itemId;

		public int _amount;

		public float _endTime;

		public GameObject _GO;

		public UILabel _label;
	}

	[Serializable]
	public class UpgradePreview
	{
		public GameObject _upgradesDistributionBacking;

		public HudGui.UpgradesDistributionView[] _upgradesDistributionViews;

		public HudGui.UpgradeBonusPreview _speedBonusPreview;

		public HudGui.UpgradeBonusPreview _damageBonusView;

		public HudGui.UpgradeBonusPreview _blockBonusView;
	}

	[Serializable]
	public class UpgradesDistributionView
	{
		[ItemIdPicker(Item.Types.CraftingMaterial)]
		public int _itemId;

		public Transform _sprite;

		public GameObject _icon;
	}

	[Serializable]
	public class CarriedWeightInfoView
	{
		public GameObject _root;

		public UILabel _weightOverMax;

		public UILabel _weightPercentage;
	}

	[Serializable]
	public class ModTimerView
	{
		public GameObject _root;

		public UILabel _title;

		public UILabel _timer;

		public UILabel _subtitle;
	}

	[Serializable]
	public class LoadingScreen
	{
		public GameObject _cam;

		public GameObject _message;

		public TweenAlpha _backgroundTween;
	}

	private const float MaxWeaponSpeed = 12f;

	private const float MaxWeaponDamage = 12f;

	private const float MaxWeaponBlockPercent = 1f;

	private const float MaxWeaponBlockStaminDrain = 26f;

	public GameObject PauseMenu;

	[Space(15f)]
	public GameObject StomachOutline;

	public UISprite Stomach;

	public UISprite StomachStarvation;

	public GameObject ThirstOutline;

	public UISprite Thirst;

	public GameObject TimmyStomachOutline;

	public UISprite TimmyStomach;

	public UISprite HealthBar;

	public UISprite ArmorBar;

	public UISprite ColdArmorBar;

	public UISprite StaminaBar;

	public UISprite EnergyBar;

	public GameObject EnergyFlash;

	public GameObject HealthBarOutline;

	public GameObject StaminaBarOutline;

	public GameObject EnergyBarOutline;

	public UISprite AirReserve;

	public GameObject AirReserveOutline;

	[Space(15f)]
	public UIGrid Grid;

	public GameObject PlaneBodiesMsg;

	public UILabel PlaneBodiesLabel;

	[Space(15f)]
	public UILabel MpRespawnLabel;

	public UISprite MpRespawnMaxTimer;

	public PlayerOverlay PlayerOverlay;

	public GameObject MpPlayerListCamGo;

	public MpPlayerList MpPlayerList;

	public MpBannedPlayerList MpBannedPlayerList;

	[Space(15f)]
	public GameObject ToggleNoWallIcon;

	public GameObject ToggleWallIcon;

	public GameObject ToggleDoor1Icon;

	public GameObject ToggleDoor2Icon;

	public GameObject ToggleWindowIcon;

	public GameObject ToggleGate1Icon;

	public GameObject ToggleGate2Icon;

	public GameObject PlacePartIcon;

	public GameObject PlaceIcon;

	public GameObject PlaceWallIcon;

	public GameObject BatchPlaceIcon;

	public GameObject RotateIcon;

	public GameObject ToggleArrowBonusIcon;

	public GameObject SnapIcon;

	public GameObject LockPositionIcon;

	public GameObject UnlockPositionIcon;

	public GameObject ToggleAutoFillIcon;

	public GameObject ToggleManualPlacementIcon;

	public GameObject AutoFillPointsIcon;

	public GameObject AutoCompletePointsIcon;

	public GameObject AddIcon;

	public GameObject DestroyIcon;

	public GameObject MapIcon;

	public GameObject EyeIcon;

	public GameObject EyeIconLine;

	public UiFollowTarget SleepDelayIcon;

	public UILabel MpSleepLabel;

	[Space(15f)]
	public GameObject bookUICam;

	public GameObject bookTutorial;

	public GameObject GuiCam;

	public GameObject PedCam;

	public HudGui.LoadingScreen Loading;

	public Camera GuiCamC;

	public Camera ActionIconCams;

	public GameObject SaveSlotSelectionScreen;

	[Space(15f)]
	public GameObject Tut_Lighter;

	public GameObject Tut_Shelter;

	public GameObject Tut_OpenBag;

	public GameObject Tut_OpenBook;

	public GameObject Tut_DeathMP;

	public GameObject Tut_ReviveMP;

	public GameObject Tut_Health;

	public GameObject Tut_Energy;

	public GameObject Tut_Bloody;

	public GameObject Tut_Cold;

	public GameObject Tut_ColdDamage;

	public GameObject Tut_Hungry;

	public GameObject Tut_Starvation;

	public GameObject Tut_Thirst;

	public GameObject Tut_Thirsty;

	public GameObject Tut_BookStage1;

	public GameObject Tut_BookStage2;

	public GameObject Tut_BookStage3;

	public GameObject Tut_Axe;

	public GameObject Tut_Crafting;

	public GameObject Tut_AnchorAccessibleBuildings;

	public GameObject Tut_StoryClue;

	public GameObject Tut_HostAltTabMP;

	public GameObject Tut_NoInventoryUnderwater;

	public GameObject Tut_MolotovTutorial;

	[Space(15f)]
	public GameObject DropButton;

	public GameObject DelayedActionIcon;

	public GameObject CraftingMessage;

	public UISprite CraftingReceipeProgress;

	public UISprite CraftingReceipeBacking;

	public GameObject ClickToCombineInfo;

	public GameObject ClickToRemoveInfo;

	public GameObject ClickToEquipInfo;

	public GameObject ManualUpgradingInfo;

	public GameObject TalkyWalkyInfo;

	public UITexture StorageFill;

	[Space(15f)]
	public BuildLog[] BuildMissionLogs;

	[Space(15f)]
	public GameObject[] StoryMapIcons;

	public GameObject[] StarIcons;

	[Space(15f)]
	public HudGui.ModTimerView ModTimer;

	[Space(15f)]
	public Transform AngleAndDistanceGizmo;

	public Transform AngleAndDistanceGizmoWall;

	public Transform SnappingGridGizmo;

	public Transform UpgradePlacementGizmo;

	public LineRenderer RangedWeaponTrajectory;

	public GameObject RangedWeaponHitTarget;

	public GameObject RangedWeaponHitGroundTarget;

	public Transform MouseSprite;

	[NameFromProperty("_titleText"), Space(15f)]
	public List<HudGui.InventoryItemInfo> _inventoryItemsInfo;

	public HudGui.InventoryItemInfoView _inventoryItemInfoView;

	public HudGui.CarriedWeightInfoView _carriedWeightInfoView;

	public GameObject _cantCarryItemView;

	public GameObject _cantCarryItemWeightView;

	public GameObject _gotItemView;

	public string _gotItemText;

	public GameObject _foundPassengerView;

	public GameObject _todoListMessageView;

	public ReceipeProductView[] _receipeProductViews;

	public GameObject _upgradesDistributionBacking;

	public HudGui.UpgradesDistributionView[] _upgradesDistributionViews;

	public HudGui.UpgradePreview _upgradePreview;

	private Dictionary<int, HudGui.InventoryItemInfo> _inventoryItemsInfoCache;

	private Dictionary<int, HudGui.TimedGameObject> _cantCarryItemViewGOs;

	private Dictionary<int, HudGui.TimedGameObject> _gotItemViewGOs;

	private Dictionary<int, HudGui.TimedGameObject> _foundPassengerViewGOs;

	private Dictionary<int, HudGui.TimedGameObject> _todoListMessagesGOs;

	private Queue<HudGui.TimedGameObject> _tgoPool;

	private int _screenResolutionHash;

	public MonoBehaviour DelayedActionIconController
	{
		get;
		set;
	}

	public Dictionary<int, HudGui.InventoryItemInfo> InventoryItemsInfoCache
	{
		get
		{
			return this._inventoryItemsInfoCache;
		}
	}

	[DebuggerHidden]
	private IEnumerator Start()
	{
		HudGui.<Start>c__Iterator15C <Start>c__Iterator15C = new HudGui.<Start>c__Iterator15C();
		<Start>c__Iterator15C.<>f__this = this;
		return <Start>c__Iterator15C;
	}

	private void Update()
	{
		this.CheckTimedGOs(this._cantCarryItemViewGOs);
		this.CheckTimedGOs(this._gotItemViewGOs);
		this.CheckTimedGOs(this._foundPassengerViewGOs);
		this.CheckTimedGOs(this._todoListMessagesGOs);
		if (BoltNetwork.isRunning && TheForest.Utils.Input.GetButtonDown("PlayerList"))
		{
			this.ToggleMpPlayerList();
		}
		if (this._screenResolutionHash != this.GetScreenResolutionHash())
		{
			this._screenResolutionHash = this.GetScreenResolutionHash();
			Scene.ActiveMB.StartCoroutine(this.RefreshHud());
		}
		this.CheckDelayedActionController();
	}

	private void InitPrefabPool(Transform prefab)
	{
		PrefabPool prefabPool = new PrefabPool(prefab);
		prefabPool.cullAbove = 5;
		prefabPool.cullDelay = 10;
		prefabPool.cullMaxPerPass = 10;
		prefabPool.cullDespawned = true;
		PoolManager.Pools["misc"].CreatePrefabPool(prefabPool);
	}

	public void ShowMpPlayerList()
	{
		this.MpPlayerListCamGo.SetActive(true);
	}

	public void HideMpPlayerList()
	{
		this.MpPlayerListCamGo.SetActive(false);
	}

	public void ToggleMpPlayerList()
	{
		this.MpPlayerListCamGo.SetActive(!this.MpPlayerListCamGo.activeInHierarchy);
	}

	private void OnApplicationFocus(bool focusStatus)
	{
		UnityEngine.Debug.Log("OnApplicationFocus: focusStatus=" + focusStatus);
		if (Scene.ActiveMB)
		{
			Scene.ActiveMB.StartCoroutine(this.RefreshHud());
		}
	}

	[DebuggerHidden]
	private IEnumerator RefreshHud()
	{
		return new HudGui.<RefreshHud>c__Iterator15D();
	}

	private int GetScreenResolutionHash()
	{
		return Screen.width * 100000 + Screen.height;
	}

	public void CheckHudState()
	{
		bool flag = PlayerPreferences.ShowHud && LocalPlayer.Inventory.CurrentView != PlayerInventory.PlayerViews.Pause && !Scene.Atmosphere.Sleeping;
		if (this.GuiCam.activeSelf != flag)
		{
			this.GuiCam.SetActive(flag);
			base.transform.FindChild("iconsAndText").gameObject.SetActive(flag);
		}
	}

	private void CheckTimedGOs(Dictionary<int, HudGui.TimedGameObject> gos)
	{
		Dictionary<int, HudGui.TimedGameObject>.Enumerator enumerator = gos.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<int, HudGui.TimedGameObject> current = enumerator.Current;
			HudGui.TimedGameObject value = current.Value;
			if (value._endTime < Time.time)
			{
				if (PoolManager.Pools["misc"].IsSpawned(value._GO.transform))
				{
					PoolManager.Pools["misc"].Despawn(value._GO.transform);
				}
				else
				{
					UnityEngine.Object.Destroy(value._GO);
				}
				value._GO = null;
				value._label = null;
				this._tgoPool.Enqueue(value);
				gos.Remove(value._itemId);
				this.Grid.repositionNow = true;
				break;
			}
		}
	}

	public void SetDelayedIconController(MonoBehaviour mb)
	{
		if (this.DelayedActionIconController != mb)
		{
			this.DelayedActionIconController = mb;
			this.CheckDelayedActionController();
		}
	}

	public void UnsetDelayedIconController(MonoBehaviour mb)
	{
		if (this.DelayedActionIconController == mb)
		{
			this.SetDelayedIconController(null);
		}
	}

	public void CheckDelayedActionController()
	{
		if (this.DelayedActionIconController && this.DelayedActionIconController.enabled && this.DelayedActionIconController.gameObject.activeSelf)
		{
			if (!this.DelayedActionIcon.activeSelf)
			{
				this.DelayedActionIcon.SetActive(true);
			}
		}
		else if (this.DelayedActionIcon.activeSelf)
		{
			this.DelayedActionIcon.SetActive(false);
			this.DelayedActionIconController = null;
		}
	}

	public void ShowItemInfoView(InventoryItemView itemView, Vector3 viewportPos, bool isCraft)
	{
		if (this._inventoryItemInfoView._itemId == itemView._itemId && this._inventoryItemInfoView.IsCraft == isCraft)
		{
			this._inventoryItemInfoView.ViewCounter++;
		}
		else if (!isCraft)
		{
			if (this._inventoryItemsInfoCache.ContainsKey(itemView._itemId))
			{
				this._inventoryItemInfoView.ViewCounter = 1;
				this._inventoryItemInfoView._itemId = itemView._itemId;
				this._inventoryItemInfoView.IsCraft = false;
				if (viewportPos.y > 0.6f)
				{
					viewportPos.y -= 0.1f;
				}
				else
				{
					viewportPos.y += 0.1f;
				}
				if (viewportPos.x > 0.7f)
				{
					viewportPos.x -= 0.075f;
				}
				else if (viewportPos.x < 0.3f)
				{
					viewportPos.x += 0.075f;
				}
				viewportPos.z = 2f;
				Vector3 position = this.GuiCamC.ViewportToWorldPoint(viewportPos);
				this._inventoryItemInfoView._root.transform.position = position;
				HudGui.InventoryItemInfo inventoryItemInfo = this._inventoryItemsInfoCache[itemView._itemId];
				this._inventoryItemInfoView._icon.mainTexture = inventoryItemInfo._icon;
				if (itemView is DecayingInventoryItemView)
				{
					this._inventoryItemInfoView._title.text = string.Format("{0} {1}", ((DecayingInventoryItemView)itemView)._state, inventoryItemInfo._titleText);
				}
				else
				{
					this._inventoryItemInfoView._title.text = inventoryItemInfo._titleText;
				}
				this._inventoryItemInfoView._effect.text = inventoryItemInfo._effectText;
				this._inventoryItemInfoView._description.text = inventoryItemInfo._descriptionText;
				this._inventoryItemInfoView._effect.gameObject.SetActive(true);
				this._inventoryItemInfoView._description.gameObject.SetActive(true);
				switch (inventoryItemInfo._amountDisplay)
				{
				case HudGui.InventoryItemInfo.AmountDisplay.none:
					this._inventoryItemInfoView._amountText._label.enabled = false;
					break;
				case HudGui.InventoryItemInfo.AmountDisplay.Amount:
					this._inventoryItemInfoView._amountText._label.enabled = true;
					this._inventoryItemInfoView._amountText._itemId = itemView._itemId;
					break;
				case HudGui.InventoryItemInfo.AmountDisplay.Pedometer:
					this._inventoryItemInfoView._amountText._label.enabled = false;
					this._inventoryItemInfoView._description.text = inventoryItemInfo._descriptionText.Replace("%", LocalPlayer.FpHeadBob.Steps.ToString());
					break;
				case HudGui.InventoryItemInfo.AmountDisplay.Battery:
					this._inventoryItemInfoView._amountText._label.enabled = false;
					this._inventoryItemInfoView._description.text = inventoryItemInfo._descriptionText.Replace("%", Mathf.FloorToInt(LocalPlayer.Stats.BatteryCharge) + "%");
					break;
				case HudGui.InventoryItemInfo.AmountDisplay.Air:
					this._inventoryItemInfoView._amountText._label.enabled = false;
					this._inventoryItemInfoView._description.text = inventoryItemInfo._descriptionText.Replace("%", Mathf.FloorToInt(LocalPlayer.Stats.AirBreathing.CurrentRebreatherAir) + "s");
					if (LocalPlayer.Inventory.HasInSlot(Item.EquipmentSlot.Chest, inventoryItemInfo._itemId))
					{
						UILabel expr_376 = this._inventoryItemInfoView._description;
						expr_376.text += " (Equiped)";
					}
					break;
				case HudGui.InventoryItemInfo.AmountDisplay.WaterFill:
				{
					this._inventoryItemInfoView._amountText._label.enabled = false;
					WeaponStatUpgrade.Types activeBonus = itemView.ActiveBonus;
					string newValue;
					if (activeBonus != WeaponStatUpgrade.Types.DirtyWater)
					{
						if (activeBonus != WeaponStatUpgrade.Types.CleanWater)
						{
							newValue = "empty";
						}
						else
						{
							newValue = "clean water";
						}
					}
					else
					{
						newValue = "polluted water";
					}
					this._inventoryItemInfoView._description.text = inventoryItemInfo._descriptionText.Replace("%", newValue);
					break;
				}
				}
				this._inventoryItemInfoView._weight.enabled = false;
				if (this._inventoryItemInfoView._upgradeCounterGrid)
				{
					if (itemView.ItemCache.MatchType(Item.Types.Equipment))
					{
						this._inventoryItemInfoView._upgradeCounterGrid.gameObject.SetActive(true);
						for (int i = 0; i < this._inventoryItemInfoView._upgradeCounterViews.Count; i++)
						{
							HudGui.UpgradeCounterView upgradeCounterView = this._inventoryItemInfoView._upgradeCounterViews[i];
							int amountOfUpgrades = LocalPlayer.Inventory.GetAmountOfUpgrades(itemView._itemId, upgradeCounterView._itemId);
							if (amountOfUpgrades > 0)
							{
								upgradeCounterView._root.SetActive(true);
								upgradeCounterView._label.text = amountOfUpgrades.ToString();
							}
							else
							{
								upgradeCounterView._root.SetActive(false);
							}
						}
						this._inventoryItemInfoView._upgradeCounterGrid.Reposition();
					}
					else
					{
						this._inventoryItemInfoView._upgradeCounterGrid.gameObject.SetActive(false);
					}
				}
				if (this._inventoryItemInfoView._upgradeBonusGrid)
				{
					if (itemView.ItemCache.MatchType(Item.Types.Equipment) && itemView._heldWeaponInfo)
					{
						this._inventoryItemInfoView._effect.gameObject.SetActive(false);
						this._inventoryItemInfoView._description.gameObject.SetActive(false);
						this._inventoryItemInfoView._upgradeBonusGrid.gameObject.SetActive(true);
						this._inventoryItemInfoView._speedBonusView._root.SetActive(true);
						float fillAmount = itemView._heldWeaponInfo.weaponSpeed / 12f;
						this._inventoryItemInfoView._speedBonusView._amount.fillAmount = fillAmount;
						this._inventoryItemInfoView._damageBonusView._root.SetActive(true);
						float fillAmount2 = itemView._heldWeaponInfo.weaponDamage / 12f;
						this._inventoryItemInfoView._damageBonusView._amount.fillAmount = fillAmount2;
						this._inventoryItemInfoView._blockView._root.SetActive(true);
						float fillAmount3 = 1f - itemView._heldWeaponInfo.blockDamagePercent / 1f;
						this._inventoryItemInfoView._blockView._amount.fillAmount = fillAmount3;
						this._inventoryItemInfoView._upgradeBonusGrid.Reposition();
					}
					else
					{
						this._inventoryItemInfoView._upgradeBonusGrid.gameObject.SetActive(false);
					}
				}
				for (int j = 0; j < this._inventoryItemInfoView._usableUpgradeViews.Count; j++)
				{
					HudGui.UsableUpgradeView usableUpgradeView = this._inventoryItemInfoView._usableUpgradeViews[j];
					if (usableUpgradeView._stat == itemView.ActiveBonus != usableUpgradeView._root.activeSelf)
					{
						usableUpgradeView._root.SetActive(!usableUpgradeView._root.activeSelf);
					}
				}
				if (inventoryItemInfo._leftClick == HudGui.InventoryItemInfo.LeftClickCommands.none)
				{
					this._inventoryItemInfoView._leftClickIcon.enabled = false;
					this._inventoryItemInfoView._leftClickText.enabled = false;
				}
				else
				{
					this._inventoryItemInfoView._leftClickIcon.enabled = true;
					this._inventoryItemInfoView._leftClickText.enabled = true;
					if (itemView.ItemCache._edibleCondition != (WeaponStatUpgrade.Types)(-1) && (itemView.ActiveBonus == WeaponStatUpgrade.Types.CleanWater || itemView._activeBonus == WeaponStatUpgrade.Types.DirtyWater))
					{
						this._inventoryItemInfoView._leftClickText.text = "Drink";
					}
					else
					{
						this._inventoryItemInfoView._leftClickText.text = inventoryItemInfo._leftClick.ToString();
					}
				}
				if (LocalPlayer.Inventory.CurrentStorage == LocalPlayer.Inventory._craftingCog)
				{
					if (itemView._canDropFromInventory)
					{
						this._inventoryItemInfoView._rightClickIcon.enabled = true;
						this._inventoryItemInfoView._rightClickText.enabled = true;
						this._inventoryItemInfoView._rightClickText.text = "Drop";
					}
					else if (inventoryItemInfo._rightClick == HudGui.InventoryItemInfo.RightClickCommands.none)
					{
						this._inventoryItemInfoView._rightClickIcon.enabled = false;
						this._inventoryItemInfoView._rightClickText.enabled = false;
					}
					else
					{
						this._inventoryItemInfoView._rightClickIcon.enabled = true;
						this._inventoryItemInfoView._rightClickText.enabled = true;
						this._inventoryItemInfoView._rightClickText.text = inventoryItemInfo._rightClick.ToString();
					}
				}
				else if (itemView.CanBeStored)
				{
					this._inventoryItemInfoView._rightClickIcon.enabled = true;
					this._inventoryItemInfoView._rightClickText.enabled = true;
					this._inventoryItemInfoView._rightClickText.text = "Store";
				}
				else
				{
					this._inventoryItemInfoView._rightClickIcon.enabled = false;
					this._inventoryItemInfoView._rightClickText.enabled = false;
				}
				this._inventoryItemInfoView._root.SetActive(true);
			}
			else
			{
				UnityEngine.Debug.LogWarning("Missing Inventory Item Info data in HUDGui for " + itemView.ItemCache._name);
			}
		}
		else
		{
			if (!this.ClickToRemoveInfo.activeSelf)
			{
				this._inventoryItemInfoView.ViewCounter = 1;
				this._inventoryItemInfoView._itemId = itemView._itemId;
				this._inventoryItemInfoView.IsCraft = true;
				this.ClickToRemoveInfo.SetActive(true);
			}
			if (this.ClickToEquipInfo.activeSelf != itemView._canEquipFromCraft)
			{
				this.ClickToEquipInfo.SetActive(itemView._canEquipFromCraft);
			}
		}
	}

	public void HideItemInfoView(int itemId, bool isCraft)
	{
		if (this._inventoryItemInfoView._itemId == itemId && this._inventoryItemInfoView.IsCraft == isCraft)
		{
			this._inventoryItemInfoView.ViewCounter--;
			if (this._inventoryItemInfoView.ViewCounter == 0)
			{
				this.ClickToRemoveInfo.SetActive(false);
				this.ClickToEquipInfo.SetActive(false);
				if (this._inventoryItemInfoView._root.activeSelf)
				{
					this._inventoryItemInfoView._root.SetActive(false);
				}
				this._inventoryItemInfoView._itemId = 0;
			}
		}
	}

	public void ShowCarriedWeightInfo(Vector3 viewportPos)
	{
	}

	public void HideCarriedWeightInfo()
	{
		this._carriedWeightInfoView._root.SetActive(false);
	}

	private HudGui.TimedGameObject SpawnTimedGameObject(int itemId, int amount, float endTime, GameObject go, UILabel label)
	{
		HudGui.TimedGameObject timedGameObject;
		if (this._tgoPool.Count > 0)
		{
			timedGameObject = this._tgoPool.Dequeue();
		}
		else
		{
			timedGameObject = new HudGui.TimedGameObject();
		}
		timedGameObject._itemId = itemId;
		timedGameObject._amount = amount;
		timedGameObject._endTime = endTime;
		timedGameObject._GO = go;
		timedGameObject._label = label;
		return timedGameObject;
	}

	public void ToggleFullCapacityHud(int itemId)
	{
		if (this._inventoryItemsInfoCache.ContainsKey(itemId) && this._inventoryItemsInfoCache[itemId]._showCantCarryMoreItem && this.GuiCam.activeSelf)
		{
			if (!this._cantCarryItemViewGOs.ContainsKey(itemId))
			{
				base.enabled = true;
				GameObject gameObject = PoolManager.Pools["misc"].Spawn(this._cantCarryItemView.transform, this._cantCarryItemView.transform.position, this._cantCarryItemView.transform.rotation).gameObject;
				gameObject.transform.parent = this.Grid.transform;
				gameObject.transform.localScale = this._cantCarryItemView.transform.localScale;
				gameObject.SetActive(true);
				UILabel componentInChildren = gameObject.GetComponentInChildren<UILabel>();
				if (componentInChildren)
				{
					componentInChildren.text = (string.IsNullOrEmpty(this._inventoryItemsInfoCache[itemId]._titlePluralText) ? this._inventoryItemsInfoCache[itemId]._titleText : this._inventoryItemsInfoCache[itemId]._titlePluralText);
				}
				this.Grid.Reposition();
				this._cantCarryItemViewGOs[itemId] = this.SpawnTimedGameObject(itemId, 0, Time.time + 5f, gameObject, null);
			}
			else
			{
				this._cantCarryItemViewGOs[itemId]._endTime = Time.time + 5f;
				this._cantCarryItemViewGOs[itemId]._GO.SetActive(false);
				this.Grid.Reposition();
				this._cantCarryItemViewGOs[itemId]._GO.SetActive(true);
				this.Grid.Reposition();
			}
		}
	}

	public void ToggleFullWeightHud(int itemId)
	{
		if (this.GuiCam.activeSelf)
		{
			if (!this._cantCarryItemViewGOs.ContainsKey(itemId))
			{
				base.enabled = true;
				GameObject gameObject = PoolManager.Pools["misc"].Spawn(this._cantCarryItemWeightView.transform, this._cantCarryItemWeightView.transform.position, this._cantCarryItemWeightView.transform.rotation).gameObject;
				gameObject.transform.parent = this.Grid.transform;
				gameObject.transform.localScale = this._cantCarryItemWeightView.transform.localScale;
				gameObject.SetActive(true);
				UILabel componentInChildren = gameObject.GetComponentInChildren<UILabel>();
				if (componentInChildren)
				{
					componentInChildren.text = componentInChildren.text.Replace("{ITEM}", string.IsNullOrEmpty(this._inventoryItemsInfoCache[itemId]._titlePluralText) ? this._inventoryItemsInfoCache[itemId]._titleText : this._inventoryItemsInfoCache[itemId]._titlePluralText);
				}
				this.Grid.Reposition();
				this._cantCarryItemViewGOs[itemId] = this.SpawnTimedGameObject(itemId, 0, Time.time + 5f, gameObject, null);
			}
			else
			{
				this._cantCarryItemViewGOs[itemId]._endTime = Time.time + 5f;
				this._cantCarryItemViewGOs[itemId]._GO.SetActive(false);
				this.Grid.Reposition();
				this._cantCarryItemViewGOs[itemId]._GO.SetActive(true);
				this.Grid.Reposition();
			}
		}
	}

	public void ToggleGotItemHud(int itemId, int amount)
	{
		if (base.gameObject.activeSelf && this._inventoryItemsInfoCache.ContainsKey(itemId) && this._inventoryItemsInfoCache[itemId]._showGotItem && this.GuiCam.activeSelf)
		{
			if (!this._gotItemViewGOs.ContainsKey(itemId))
			{
				GameObject gameObject = PoolManager.Pools["misc"].Spawn(this._gotItemView.transform, this._gotItemView.transform.position, this._gotItemView.transform.rotation).gameObject;
				gameObject.transform.parent = this.Grid.transform;
				gameObject.transform.localScale = this._gotItemView.transform.localScale;
				gameObject.SetActive(true);
				UILabel componentInChildren = gameObject.GetComponentInChildren<UILabel>();
				if (componentInChildren)
				{
					componentInChildren.text = this.GotItemText(this._inventoryItemsInfoCache[itemId], amount);
				}
				this.Grid.Reposition();
				this._gotItemViewGOs[itemId] = this.SpawnTimedGameObject(itemId, amount, Time.time + 2f, gameObject, componentInChildren);
			}
			else
			{
				this._gotItemViewGOs[itemId]._amount += amount;
				this._gotItemViewGOs[itemId]._endTime = Time.time + 2f;
				this._gotItemViewGOs[itemId]._label.text = this.GotItemText(this._inventoryItemsInfoCache[itemId], this._gotItemViewGOs[itemId]._amount);
				this._gotItemViewGOs[itemId]._GO.SetActive(false);
				this.Grid.Reposition();
				this._gotItemViewGOs[itemId]._GO.SetActive(true);
				this.Grid.Reposition();
			}
		}
	}

	public void ShowFoundPassenger(int passengerId)
	{
		if (!this._foundPassengerViewGOs.ContainsKey(passengerId))
		{
			GameObject gameObject = PoolManager.Pools["misc"].Spawn(this._foundPassengerView.transform, this._foundPassengerView.transform.position, this._foundPassengerView.transform.rotation).gameObject;
			gameObject.transform.parent = this.Grid.transform;
			gameObject.transform.localScale = this._foundPassengerView.transform.localScale;
			gameObject.SetActive(true);
			UILabel componentInChildren = gameObject.GetComponentInChildren<UILabel>();
			if (componentInChildren)
			{
				string passengerName = PassengerDatabase.Instance.GetPassengerName(passengerId);
				componentInChildren.text = "Found passenger, seat " + passengerId;
			}
			this.Grid.Reposition();
			this._foundPassengerViewGOs[passengerId] = this.SpawnTimedGameObject(passengerId, 0, Time.time + 10f, gameObject, componentInChildren);
		}
	}

	public void ShowTodoListMessage(string message)
	{
		if (!string.IsNullOrEmpty(message) && !this._todoListMessagesGOs.Any((KeyValuePair<int, HudGui.TimedGameObject> tlm) => tlm.Value._label.text.Equals(message)))
		{
			GameObject gameObject = PoolManager.Pools["misc"].Spawn(this._todoListMessageView.transform, this._todoListMessageView.transform.position, this._todoListMessageView.transform.rotation).gameObject;
			gameObject.transform.parent = this.Grid.transform;
			gameObject.transform.localScale = this._todoListMessageView.transform.localScale;
			gameObject.SetActive(true);
			UILabel componentInChildren = gameObject.GetComponentInChildren<UILabel>();
			if (componentInChildren)
			{
				componentInChildren.text = message;
			}
			this.Grid.Reposition();
			int num = (this._todoListMessagesGOs.Keys.Count != 0) ? (this._todoListMessagesGOs.Keys.Max() + 1) : 0;
			this._todoListMessagesGOs[num] = this.SpawnTimedGameObject(num, 0, Time.time + 16f, gameObject, componentInChildren);
		}
	}

	private string GotItemText(HudGui.InventoryItemInfo iii, int amount)
	{
		string text;
		if (amount > 1)
		{
			text = amount + " " + (string.IsNullOrEmpty(iii._titlePluralText) ? iii._titleText : iii._titlePluralText);
		}
		else
		{
			text = iii._titleText;
		}
		if (iii._showCollectedItem)
		{
			text += " collected";
		}
		else
		{
			text += this._gotItemText;
		}
		return text;
	}

	public void ShowValidCraftingRecipes(IOrderedEnumerable<Receipe> receipes)
	{
		int i = 0;
		if (receipes != null)
		{
			foreach (Receipe current in receipes)
			{
				this._receipeProductViews[i].gameObject.SetActive(true);
				this._receipeProductViews[i].ShowReceipe(current, (!this._inventoryItemsInfoCache.ContainsKey(current._productItemID)) ? null : this._inventoryItemsInfoCache[current._productItemID]);
				if (++i == this._receipeProductViews.Length)
				{
					break;
				}
			}
		}
		while (i < this._receipeProductViews.Length)
		{
			this._receipeProductViews[i].gameObject.SetActive(false);
			i++;
		}
	}

	public void ShowUpgradesDistribution(int itemId, int addingItemid, int addingAmount)
	{
		weaponInfo heldWeaponInfo = LocalPlayer.Inventory.InventoryItemViewsCache[itemId][0]._heldWeaponInfo;
		if (heldWeaponInfo)
		{
			WeaponStatUpgrade[] weaponStatUpgradeForIngredient = LocalPlayer.Inventory._craftingCog.GetWeaponStatUpgradeForIngredient(addingItemid);
			if (weaponStatUpgradeForIngredient != null)
			{
				int maxUpgradesAmount = ItemDatabase.ItemById(itemId)._maxUpgradesAmount;
				this._upgradePreview._upgradesDistributionBacking.SetActive(true);
				for (int i = 0; i < this._upgradePreview._upgradesDistributionViews.Length; i++)
				{
					float num = (float)LocalPlayer.Inventory.GetAmountOfUpgrades(itemId, this._upgradePreview._upgradesDistributionViews[i]._itemId);
					if (this._upgradePreview._upgradesDistributionViews[i]._itemId == addingItemid)
					{
						num += (float)addingAmount;
					}
					if (num > 0f)
					{
						this._upgradePreview._upgradesDistributionViews[i]._icon.SetActive(true);
						this._upgradePreview._upgradesDistributionViews[i]._sprite.localScale = new Vector3(num / (float)maxUpgradesAmount, 0.5f, 1f);
					}
					else
					{
						this._upgradePreview._upgradesDistributionViews[i]._icon.SetActive(false);
						this._upgradePreview._upgradesDistributionViews[i]._sprite.localScale = Vector3.zero;
					}
				}
				float num2 = 0f;
				float num3 = 0f;
				float num4 = 0f;
				float num5 = 0f;
				for (int j = 0; j < weaponStatUpgradeForIngredient.Length; j++)
				{
					WeaponStatUpgrade.Types type = weaponStatUpgradeForIngredient[j]._type;
					switch (type)
					{
					case WeaponStatUpgrade.Types.weaponDamage:
						num3 += LocalPlayer.Inventory._craftingCog.GetUpgradeBonusAmount(itemId, addingItemid, weaponStatUpgradeForIngredient[j], addingAmount);
						goto IL_1F0;
					case WeaponStatUpgrade.Types.smashDamage:
						IL_182:
						if (type != WeaponStatUpgrade.Types.blockStaminaDrain)
						{
							goto IL_1F0;
						}
						num5 += LocalPlayer.Inventory._craftingCog.GetUpgradeBonusAmount(itemId, addingItemid, weaponStatUpgradeForIngredient[j], addingAmount);
						goto IL_1F0;
					case WeaponStatUpgrade.Types.weaponSpeed:
						num2 += LocalPlayer.Inventory._craftingCog.GetUpgradeBonusAmount(itemId, addingItemid, weaponStatUpgradeForIngredient[j], addingAmount);
						goto IL_1F0;
					}
					goto IL_182;
					IL_1F0:;
				}
				this._inventoryItemInfoView._speedBonusView._root.SetActive(true);
				float num6;
				float num7;
				if (num2 >= 0f)
				{
					num6 = heldWeaponInfo.weaponSpeed / 12f;
					num7 = num2 / 12f;
					this._upgradePreview._speedBonusPreview._amountRight.color = this._upgradePreview._speedBonusPreview._positive;
				}
				else
				{
					num6 = (heldWeaponInfo.weaponSpeed + num2) / 12f;
					num7 = Mathf.Abs(num2) / 12f;
					this._upgradePreview._speedBonusPreview._amountRight.color = this._upgradePreview._speedBonusPreview._negative;
				}
				this._upgradePreview._speedBonusPreview._amountLeft.fillAmount = num6;
				this._upgradePreview._speedBonusPreview._amountRight.fillAmount = num7;
				this._upgradePreview._speedBonusPreview._amountRight.transform.localPosition = new Vector3((num6 + num7) * (float)this._upgradePreview._speedBonusPreview._amountRight.width, 0f, 0f);
				float num8;
				float num9;
				if (num3 >= 0f)
				{
					num8 = heldWeaponInfo.weaponDamage / 12f;
					num9 = num3 / 12f;
					this._upgradePreview._damageBonusView._amountRight.color = this._upgradePreview._damageBonusView._positive;
				}
				else
				{
					num8 = (heldWeaponInfo.weaponDamage + num3) / 12f;
					num9 = Mathf.Abs(num3) / 12f;
					this._upgradePreview._damageBonusView._amountRight.color = this._upgradePreview._damageBonusView._negative;
				}
				this._upgradePreview._damageBonusView._amountLeft.fillAmount = num8;
				this._upgradePreview._damageBonusView._amountRight.fillAmount = num9;
				this._upgradePreview._damageBonusView._amountRight.transform.localPosition = new Vector3((num8 + num9) * (float)this._upgradePreview._damageBonusView._amountRight.width, 0f, 0f);
				float num10;
				float num11;
				if (num3 >= 0f)
				{
					num10 = 1f - heldWeaponInfo.blockDamagePercent / 1f;
					num11 = num4 / 1f;
					this._upgradePreview._blockBonusView._amountRight.color = this._upgradePreview._blockBonusView._positive;
				}
				else
				{
					num10 = 1f - (heldWeaponInfo.blockDamagePercent + num4) / 1f;
					num11 = Mathf.Abs(num4) / 1f;
					this._upgradePreview._blockBonusView._amountRight.color = this._upgradePreview._blockBonusView._negative;
				}
				this._upgradePreview._blockBonusView._amountLeft.fillAmount = num10;
				this._upgradePreview._blockBonusView._amountRight.fillAmount = num11;
				this._upgradePreview._blockBonusView._amountRight.transform.localPosition = new Vector3((num10 + num11) * (float)this._upgradePreview._blockBonusView._amountRight.width, 0f, 0f);
			}
			else
			{
				this._upgradePreview._upgradesDistributionBacking.SetActive(false);
			}
		}
		else
		{
			UnityEngine.Debug.Log("Missing weaponInfo reference for: " + LocalPlayer.Inventory.InventoryItemViewsCache[itemId][0].ItemCache._name);
			this._upgradePreview._upgradesDistributionBacking.SetActive(false);
		}
	}

	public void HideUpgradesDistribution()
	{
		this._upgradePreview._upgradesDistributionBacking.SetActive(false);
	}

	public BuildLog GetBuildMissionLogForItem(int itemId)
	{
		return this.BuildMissionLogs.FirstOrDefault((BuildLog bml) => bml._itemId == itemId);
	}
}
