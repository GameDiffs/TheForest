using System;
using System.Collections.Generic;
using System.Reflection;
using TheForest.Items.Core;
using TheForest.Items.Inventory;
using TheForest.Items.Special;
using TheForest.Items.World;
using TheForest.Utils;
using UniLinq;
using UnityEngine;

namespace TheForest.Items.Craft
{
	[DoNotSerializePublic, AddComponentMenu("Items/Craft/Crafting Cog")]
	public class CraftingCog : MonoBehaviour, IItemStorage
	{
		[EnumFlags]
		public Item.Types _acceptedTypes;

		public ReceipeBook _receipeBook;

		public PlayerInventory _inventory;

		public Material _selectedMaterial;

		public GameObject _craftSfx;

		public GameObject _craftSfx2;

		public InventoryItemView[] _itemViews;

		public UpgradeCog _upgradeCog;

		private GameObject _clickToCombineButton;

		private Material _normalMaterial;

		private HashSet<ReceipeIngredient> _ingredients;

		private Dictionary<int, InventoryItemView> _itemViewsCache;

		private bool _validRecipeFull;

		private float _validRecipeFill;

		private Receipe _validRecipe;

		private int _upgradeCount;

		private FMOD_StudioEventEmitter _craftSfxEmitter;

		private FMOD_StudioEventEmitter _craftSfx2Emitter;

		private bool _initialized;

		private InventoryItemView _lambdaMultiView;

		public Item.Types AcceptedTypes
		{
			get
			{
				return (!this.Storage) ? this._acceptedTypes : this.Storage.AcceptedTypes;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this._ingredients.Count == 0;
			}
		}

		public HashSet<ReceipeIngredient> Ingredients
		{
			get
			{
				return this._ingredients;
			}
		}

		public float RecipeFill
		{
			get
			{
				return this._validRecipeFill;
			}
		}

		public bool RecipeProductFull
		{
			get
			{
				return this._validRecipeFull;
			}
		}

		private bool CanCraft
		{
			get
			{
				return this._validRecipe != null && !this._validRecipeFull && Mathf.Approximately(this._validRecipeFill, 1f);
			}
		}

		public Dictionary<int, InventoryItemView> ItemViewsCache
		{
			get
			{
				return this._itemViewsCache;
			}
		}

		public ItemStorage Storage
		{
			get;
			set;
		}

		public bool HasValideRecipe
		{
			get
			{
				return this._validRecipe != null;
			}
		}

		public void Awake()
		{
			if (!this._initialized)
			{
				this._initialized = true;
				this._validRecipeFill = 0f;
				this._validRecipe = null;
				this._normalMaterial = base.gameObject.GetComponent<Renderer>().sharedMaterial;
				this._upgradeCog = base.GetComponent<UpgradeCog>();
				this._ingredients = new HashSet<ReceipeIngredient>();
				this._itemViewsCache = this._itemViews.ToDictionary((InventoryItemView iv) => iv._itemId, (InventoryItemView iv) => iv);
				if (this._inventory && Scene.HudGui)
				{
					this._clickToCombineButton = Scene.HudGui.ClickToCombineInfo;
				}
				if (this._craftSfx)
				{
					this._craftSfxEmitter = this._craftSfx.GetComponent<FMOD_StudioEventEmitter>();
				}
				if (this._craftSfx2)
				{
					this._craftSfx2Emitter = this._craftSfx2.GetComponent<FMOD_StudioEventEmitter>();
				}
				for (int i = 0; i < this._itemViews.Length; i++)
				{
					this._itemViews[i].Init();
				}
			}
		}

		private void Start()
		{
			this._lambdaMultiView = new GameObject("LambdaMultiview").AddComponent<InventoryItemView>();
			this._lambdaMultiView.transform.parent = base.transform;
			this._lambdaMultiView.transform.localPosition = new Vector3(-2.5f, 2.5f, 2.5f);
			this._lambdaMultiView._isCraft = true;
			this._lambdaMultiView._allowMultiView = true;
			if (!LevelSerializer.IsDeserializing)
			{
				this.IngredientCleanUp();
			}
			base.enabled = false;
		}

		public void OnDeserialized()
		{
			this.Awake();
		}

		private void OnDisable()
		{
			if (this._clickToCombineButton.activeSelf)
			{
				this._clickToCombineButton.SetActive(false);
				base.gameObject.GetComponent<Renderer>().sharedMaterial = this._normalMaterial;
			}
		}

		private void OnMouseExitCollider()
		{
			base.enabled = false;
		}

		private void OnMouseEnterCollider()
		{
			bool flag = this.CheckStorage() && this._ingredients.Count > 0;
			bool hasValideRecipe = this.HasValideRecipe;
			if (!base.enabled && !this._upgradeCog.enabled && ((hasValideRecipe && (this._validRecipe._type != Receipe.Types.Upgrade || this._upgradeCount > 0)) || flag))
			{
				base.enabled = true;
				base.gameObject.GetComponent<Renderer>().sharedMaterial = this._selectedMaterial;
				if (this._clickToCombineButton.activeSelf != (hasValideRecipe || flag))
				{
					this._clickToCombineButton.SetActive(hasValideRecipe);
				}
			}
		}

		private void Update()
		{
			bool flag = this.CheckStorage() && this._ingredients.Count > 0;
			bool flag2 = this._validRecipe != null;
			if ((flag2 || flag) && (TheForest.Utils.Input.GetButtonDown("Combine") || TheForest.Utils.Input.GetButtonDown("Build")))
			{
				Receipe validRecipe = this._validRecipe;
				this._craftSfxEmitter.Play();
				LocalPlayer.Tuts.CloseRecipeTut();
				if (flag)
				{
					this.DoStorage();
				}
				else if (validRecipe._type.Equals(Receipe.Types.Upgrade))
				{
					WeaponStatUpgrade.Types types = (WeaponStatUpgrade.Types)(-1);
					for (int i = 0; i < validRecipe._ingredients.Length; i++)
					{
						ReceipeIngredient recipeIngredient = validRecipe._ingredients[i];
						ReceipeIngredient receipeIngredient = this._ingredients.FirstOrDefault((ReceipeIngredient ig) => ig._itemID == recipeIngredient._itemID);
						int num = recipeIngredient._amount * ((i != 0) ? this._upgradeCount : 1);
						receipeIngredient._amount -= num;
						if (i == 1)
						{
							types = ((!this._itemViewsCache[receipeIngredient._itemID]._allowMultiView) ? this._itemViewsCache[receipeIngredient._itemID].ActiveBonus : this._itemViewsCache[receipeIngredient._itemID].GetFirstViewBonus());
						}
						if (receipeIngredient._amount <= 0)
						{
							this._ingredients.Remove(receipeIngredient);
						}
						else if (recipeIngredient._amount == 0)
						{
							this._inventory.AddItem(recipeIngredient._itemID, receipeIngredient._amount, true, true, types);
							this.Remove(recipeIngredient._itemID, receipeIngredient._amount, (WeaponStatUpgrade.Types)(-2));
						}
						this.ToggleItemInventoryView(recipeIngredient._itemID, (WeaponStatUpgrade.Types)(-2));
					}
					int upgradeCount = this._upgradeCount;
					this.Add(validRecipe._productItemID, validRecipe._productItemAmount, (WeaponStatUpgrade.Types)(-2));
					this.ApplyUpgrade(validRecipe, types, upgradeCount);
				}
				else
				{
					GameStats.ItemCrafted.Invoke(validRecipe._productItemID);
					this.IngredientCleanUp();
					this.Add(validRecipe._productItemID, validRecipe._productItemAmount, (WeaponStatUpgrade.Types)(-2));
				}
			}
			else if (!flag2 && !flag)
			{
				base.enabled = false;
			}
		}

		public int Add(int itemId, int amount = 1, WeaponStatUpgrade.Types activeBonus = (WeaponStatUpgrade.Types)(-2))
		{
			ReceipeIngredient receipeIngredient = this._ingredients.FirstOrDefault((ReceipeIngredient i) => i._itemID == itemId);
			int num;
			if (this.Storage || !this._itemViewsCache.ContainsKey(itemId))
			{
				num = 2147483647;
			}
			else if (this._itemViewsCache[itemId]._allowMultiView)
			{
				num = this._itemViewsCache[itemId]._maxMultiViews;
			}
			else
			{
				num = 1;
			}
			if (receipeIngredient == null)
			{
				receipeIngredient = new ReceipeIngredient
				{
					_itemID = itemId
				};
				this._ingredients.Add(receipeIngredient);
			}
			int result = Mathf.Max(receipeIngredient._amount + amount - num, 0);
			receipeIngredient._amount = Mathf.Min(receipeIngredient._amount + amount, num);
			this.CheckForValidRecipe();
			this.ToggleItemInventoryView(itemId, activeBonus);
			return result;
		}

		public int Remove(int itemId, int amount = 1, WeaponStatUpgrade.Types activeBonus = (WeaponStatUpgrade.Types)(-2))
		{
			if (this._upgradeCog.enabled)
			{
				this._upgradeCog.Shutdown();
			}
			ReceipeIngredient receipeIngredient = this._ingredients.FirstOrDefault((ReceipeIngredient i) => i._itemID == itemId);
			if (receipeIngredient != null)
			{
				if (this._itemViewsCache.ContainsKey(itemId) && (this._itemViewsCache[itemId].ItemCache._maxAmount == 0 || this._itemViewsCache[itemId].ItemCache._maxAmount > LocalPlayer.Inventory.InventoryItemViewsCache[itemId].Count || !this._itemViewsCache[itemId]._allowMultiView))
				{
					if (activeBonus == (WeaponStatUpgrade.Types)(-2) || this._itemViewsCache[itemId].ActiveBonus == activeBonus)
					{
						int result = Mathf.Max(amount - receipeIngredient._amount, 0);
						if ((receipeIngredient._amount -= amount) <= 0)
						{
							this._ingredients.Remove(receipeIngredient);
						}
						this.CheckForValidRecipe();
						this.ToggleItemInventoryView(itemId, (WeaponStatUpgrade.Types)(-2));
						return result;
					}
				}
				else
				{
					if (this._itemViewsCache.ContainsKey(itemId))
					{
						int num = this._itemViewsCache[itemId].AmountOfMultiviewWithBonus(itemId, activeBonus);
						int num2 = Mathf.Max(amount - num, 0);
						if ((receipeIngredient._amount -= amount - num2) <= 0)
						{
							this._ingredients.Remove(receipeIngredient);
						}
						this.CheckForValidRecipe();
						this._itemViewsCache[itemId].RemovedMultiViews(itemId, amount, activeBonus);
						return num2;
					}
					if (LocalPlayer.Inventory.InventoryItemViewsCache.ContainsKey(itemId))
					{
						int num3 = this._lambdaMultiView.AmountOfMultiviewWithBonus(itemId, activeBonus);
						int num4 = Mathf.Max(amount - num3, 0);
						if ((receipeIngredient._amount -= amount - num4) <= 0)
						{
							this._ingredients.Remove(receipeIngredient);
						}
						this.CheckForValidRecipe();
						this._lambdaMultiView.RemovedMultiViews(itemId, amount - num4, activeBonus);
						return num4;
					}
				}
			}
			return amount;
		}

		public void Open()
		{
		}

		public void Close()
		{
			if (this._upgradeCog.enabled)
			{
				this._upgradeCog.Shutdown();
			}
			foreach (ReceipeIngredient current in this._ingredients)
			{
				if (LocalPlayer.Inventory.InventoryItemViewsCache[current._itemID][0].ItemCache.MatchType(Item.Types.Special))
				{
					LocalPlayer.Inventory.SpecialItemsControlers[current._itemID].ToggleSpecialCraft(false);
				}
				if (this._itemViewsCache.ContainsKey(current._itemID))
				{
					if (!this._itemViewsCache[current._itemID]._allowMultiView)
					{
						this._inventory.AddItem(current._itemID, current._amount, true, true, this._itemViewsCache[current._itemID].ActiveBonus);
					}
					else
					{
						int num;
						for (int i = current._amount; i > 0; i -= num)
						{
							WeaponStatUpgrade.Types firstViewBonus = this._itemViewsCache[current._itemID].GetFirstViewBonus();
							num = this._itemViewsCache[current._itemID].AmountOfMultiviewWithBonus(current._itemID, firstViewBonus);
							this._itemViewsCache[current._itemID].RemovedMultiViews(current._itemID, num, firstViewBonus);
							this._inventory.AddItem(current._itemID, num, true, true, firstViewBonus);
						}
					}
				}
				else
				{
					this._inventory.AddItem(current._itemID, current._amount, true, true, (WeaponStatUpgrade.Types)(-2));
				}
			}
			this.IngredientCleanUp();
			this.CheckForValidRecipe();
		}

		private bool CanCarryProduct(Receipe recipe)
		{
			int maxAmountOf = LocalPlayer.Inventory.GetMaxAmountOf(recipe._productItemID);
			return this._inventory.AmountOf(recipe._productItemID, true) < maxAmountOf;
		}

		private bool CheckStorage()
		{
			if (this.Storage)
			{
				this._validRecipe = null;
				Scene.HudGui.CraftingReceipeBacking.gameObject.SetActive(false);
				Scene.HudGui.ShowValidCraftingRecipes(null);
				Scene.HudGui.HideUpgradesDistribution();
				base.gameObject.GetComponent<Renderer>().enabled = (this._ingredients.Count > 1);
				return true;
			}
			if (this._validRecipe == null)
			{
				base.gameObject.GetComponent<Renderer>().enabled = false;
			}
			return false;
		}

		private void DoStorage()
		{
			List<ReceipeIngredient> list = new List<ReceipeIngredient>();
			foreach (ReceipeIngredient current in this._ingredients)
			{
				if (!this._itemViewsCache.ContainsKey(current._itemID) || !this._itemViewsCache[current._itemID].ItemCache.MatchType(Item.Types.Special))
				{
					int num = this.Storage.Add(current._itemID, current._amount, (WeaponStatUpgrade.Types)(-2));
					if (num == 0)
					{
						list.Add(current);
					}
					else
					{
						current._amount = num;
					}
				}
			}
			foreach (ReceipeIngredient current2 in list)
			{
				this._ingredients.Remove(current2);
				this.ToggleItemInventoryView(current2._itemID, (WeaponStatUpgrade.Types)(-2));
			}
			this.Storage.UpdateContentVersion();
			this.CheckStorage();
		}

		public void CheckForValidRecipe()
		{
			if (this.CheckStorage())
			{
				return;
			}
			IOrderedEnumerable<Receipe> orderedEnumerable = null;
			IOrderedEnumerable<Receipe> orderedEnumerable2 = null;
			if (this._ingredients.Count > 0)
			{
				orderedEnumerable = from ar in this._receipeBook.AvailableReceipesCache
				where this._ingredients.All((ReceipeIngredient i) => ar._ingredients.Any((ReceipeIngredient i2) => i._itemID == i2._itemID))
				where this.CanCarryProduct(ar)
				orderby ar._ingredients.Length
				select ar;
				orderedEnumerable2 = from ar in this._receipeBook.AvailableUpgradeCache
				where this._ingredients.All((ReceipeIngredient i) => ar._ingredients.Any((ReceipeIngredient i2) => i._itemID == i2._itemID))
				where this.CanCarryProduct(ar)
				orderby ar._ingredients.Length
				select ar;
				this._validRecipe = orderedEnumerable.FirstOrDefault<Receipe>();
			}
			else
			{
				this._validRecipe = null;
			}
			bool flag = this._validRecipe != null;
			if (flag)
			{
				if (!this.CanCarryProduct(this._validRecipe))
				{
					Scene.HudGui.CraftingReceipeBacking.gameObject.SetActive(false);
					this._validRecipeFull = true;
					flag = false;
				}
				else
				{
					int num = 0;
					int num2 = this._validRecipe._ingredients.Sum((ReceipeIngredient i) => i._amount);
					foreach (ReceipeIngredient cogIngredients in this._ingredients)
					{
						ReceipeIngredient receipeIngredient = this._validRecipe._ingredients.First((ReceipeIngredient i) => i._itemID == cogIngredients._itemID);
						if (cogIngredients._amount > receipeIngredient._amount)
						{
							this._validRecipe = null;
							flag = false;
							break;
						}
						num += cogIngredients._amount;
					}
					this._validRecipeFull = false;
					this._validRecipeFill = (float)num / (float)num2;
					HudGui arg_25D_0 = Scene.HudGui;
					IOrderedEnumerable<Receipe> arg_25D_1;
					if (flag)
					{
						IOrderedEnumerable<Receipe> orderedEnumerable3 = from r in orderedEnumerable.Concat(orderedEnumerable2)
						orderby r._type, r._ingredients.Length
						select r;
						arg_25D_1 = orderedEnumerable3;
					}
					else
					{
						arg_25D_1 = null;
					}
					arg_25D_0.ShowValidCraftingRecipes(arg_25D_1);
					Scene.HudGui.CraftingReceipeBacking.gameObject.SetActive(false);
					if (num != num2 && flag)
					{
						Scene.HudGui.CraftingReceipeBacking.gameObject.SetActive(true);
						Scene.HudGui.CraftingReceipeProgress.fillAmount = this._validRecipeFill;
						flag = false;
						this._validRecipe = null;
					}
				}
			}
			else
			{
				Scene.HudGui.ShowValidCraftingRecipes(orderedEnumerable2);
				Scene.HudGui.CraftingReceipeBacking.gameObject.SetActive(false);
			}
			if (flag)
			{
				Scene.HudGui.HideUpgradesDistribution();
				this._craftSfx2Emitter.Play();
				base.gameObject.GetComponent<Renderer>().enabled = true;
			}
			if (!flag || !this.CanCraft)
			{
				this.CheckForValidUpgrade();
			}
		}

		private void CheckForValidUpgrade()
		{
			Receipe receipe = null;
			string b = Receipe.IngredientsToRecipeHash(this._ingredients);
			foreach (Receipe current in this._receipeBook.AvailableUpgradeCache)
			{
				if (current.IngredientHash == b)
				{
					receipe = current;
					break;
				}
			}
			if (receipe != null)
			{
				if (!this.CanCarryProduct(receipe) && receipe._ingredients[0]._itemID != receipe._productItemID)
				{
					this._validRecipeFull = true;
					receipe = null;
				}
				else
				{
					IEnumerable<ReceipeIngredient> source = from vri in receipe._ingredients
					join i in this._ingredients on vri._itemID equals i._itemID
					select i;
					ReceipeIngredient[] array = source.ToArray<ReceipeIngredient>();
					if (array.Length > 1)
					{
						this._upgradeCount = ItemDatabase.ItemById(receipe._productItemID)._maxUpgradesAmount;
						int itemID = array[1]._itemID;
						if (this._upgradeCog.SupportedItemsCache.ContainsKey(itemID) && this._upgradeCog.SupportedItemsCache[itemID]._pattern != UpgradeCog.Patterns.NoView)
						{
							this._upgradeCount -= LocalPlayer.Inventory.GetAmountOfUpgrades(receipe._productItemID);
						}
						bool flag = this._upgradeCount == 0;
						for (int j = 1; j < receipe._ingredients.Length; j++)
						{
							ReceipeIngredient receipeIngredient = receipe._ingredients[j];
							if (receipeIngredient._amount > 0)
							{
								int num = array[j]._amount / receipeIngredient._amount;
								if (num < this._upgradeCount)
								{
									this._upgradeCount = num;
								}
							}
							else
							{
								this._upgradeCount = 1;
								flag = true;
							}
						}
						if (this._upgradeCount <= 0 && !flag)
						{
							receipe = null;
						}
					}
					else
					{
						receipe = null;
					}
				}
			}
			bool flag2 = receipe != null;
			if (flag2)
			{
				this._validRecipe = receipe;
				this._validRecipeFill = 1f;
				Scene.HudGui.CraftingReceipeBacking.gameObject.SetActive(false);
				Scene.HudGui.ShowValidCraftingRecipes(null);
				Scene.HudGui.ShowUpgradesDistribution(this._validRecipe._productItemID, this._validRecipe._ingredients[1]._itemID, this._upgradeCount);
			}
			else
			{
				Scene.HudGui.HideUpgradesDistribution();
				this._upgradeCount = 0;
			}
			if (base.gameObject.GetComponent<Renderer>().enabled != this._upgradeCount > 0)
			{
				if (!base.gameObject.GetComponent<Renderer>().enabled)
				{
					this._craftSfx2Emitter.Play();
				}
				base.gameObject.GetComponent<Renderer>().enabled = (this._upgradeCount > 0);
			}
		}

		private void ApplyUpgrade(Receipe craftedRecipe, WeaponStatUpgrade.Types lastIngredientActiveBonus, int upgradeCount)
		{
			bool flag = this._itemViewsCache[craftedRecipe._productItemID]._held && !this._itemViewsCache[craftedRecipe._productItemID]._held.activeInHierarchy;
			if (flag)
			{
				this._itemViewsCache[craftedRecipe._productItemID]._held.SetActive(true);
			}
			if (!this.ApplyWeaponStatsUpgrades(craftedRecipe._productItemID, craftedRecipe._ingredients[1]._itemID, craftedRecipe._weaponStatUpgrades, craftedRecipe._batchUpgrade, upgradeCount, lastIngredientActiveBonus))
			{
				this._upgradeCog.ApplyUpgradeRecipe(this._itemViewsCache[craftedRecipe._productItemID], craftedRecipe, upgradeCount);
			}
			if (flag)
			{
				this._itemViewsCache[craftedRecipe._productItemID]._held.SetActive(false);
			}
		}

		public WeaponStatUpgrade[] GetWeaponStatUpgradeForIngredient(int ingredientId)
		{
			UpgradeCogItems upgradeCogItems;
			if (this._upgradeCog.SupportedItemsCache.TryGetValue(ingredientId, out upgradeCogItems))
			{
				return upgradeCogItems._weaponStatUpgrades;
			}
			return null;
		}

		public bool ApplyWeaponStatsUpgrades(int productItemId, int ingredientItemId, WeaponStatUpgrade[] bonuses, bool batched, int upgradeCount, WeaponStatUpgrade.Types lastIngredientActiveBonus = (WeaponStatUpgrade.Types)(-2))
		{
			bool result = false;
			Type typeFromHandle = typeof(weaponInfo);
			weaponInfo heldWeaponInfo = this._itemViewsCache[productItemId]._heldWeaponInfo;
			int i = 0;
			while (i < bonuses.Length)
			{
				switch (bonuses[i]._type)
				{
				case WeaponStatUpgrade.Types.BurningWeapon:
				{
					BurnableCloth componentInChildren = this._itemViewsCache[productItemId]._held.GetComponentInChildren<BurnableCloth>();
					if (componentInChildren)
					{
						this._itemViewsCache[productItemId].ActiveBonus = WeaponStatUpgrade.Types.BurningWeapon;
						componentInChildren.EnableBurnableCloth();
					}
					result = true;
					break;
				}
				case WeaponStatUpgrade.Types.StickyProjectile:
					this._itemViewsCache[productItemId].ActiveBonus = WeaponStatUpgrade.Types.StickyProjectile;
					result = true;
					break;
				case WeaponStatUpgrade.Types.WalkmanTrack:
					WalkmanControler.LoadCassette(ingredientItemId);
					result = true;
					break;
				case WeaponStatUpgrade.Types.BurningAmmo:
					if (batched && this._itemViewsCache[productItemId]._allowMultiView)
					{
						this._itemViewsCache[productItemId].SetMultiviewsBonus(WeaponStatUpgrade.Types.BurningAmmo);
					}
					else
					{
						this._itemViewsCache[productItemId].ActiveBonus = WeaponStatUpgrade.Types.BurningAmmo;
					}
					result = true;
					break;
				case WeaponStatUpgrade.Types.Paint_Green:
				{
					EquipmentPainting componentInChildren2 = this._itemViewsCache[productItemId]._held.GetComponentInChildren<EquipmentPainting>();
					if (componentInChildren2)
					{
						componentInChildren2.PaintInGreen();
					}
					result = true;
					break;
				}
				case WeaponStatUpgrade.Types.Paint_Orange:
				{
					EquipmentPainting componentInChildren3 = this._itemViewsCache[productItemId]._held.GetComponentInChildren<EquipmentPainting>();
					if (componentInChildren3)
					{
						componentInChildren3.PaintInOrange();
					}
					result = true;
					break;
				}
				case WeaponStatUpgrade.Types.DirtyWater:
				case WeaponStatUpgrade.Types.CleanWater:
				case WeaponStatUpgrade.Types.Cooked:
				case WeaponStatUpgrade.Types.blockStaminaDrain:
					goto IL_3DD;
				case WeaponStatUpgrade.Types.ItemPart:
				{
					IItemPartInventoryView itemPartInventoryView = (IItemPartInventoryView)this._itemViewsCache[productItemId];
					itemPartInventoryView.AddPiece(Mathf.RoundToInt(bonuses[i]._amount));
					result = true;
					break;
				}
				case WeaponStatUpgrade.Types.BatteryCharge:
					LocalPlayer.Stats.BatteryCharge = Mathf.Clamp(LocalPlayer.Stats.BatteryCharge + bonuses[i]._amount, 0f, 100f);
					result = true;
					break;
				case WeaponStatUpgrade.Types.FlareGunAmmo:
					LocalPlayer.Inventory.AddItem(ItemDatabase.ItemByName("FlareGunAmmo")._id, Mathf.RoundToInt(bonuses[i]._amount * (float)upgradeCount), false, false, (WeaponStatUpgrade.Types)(-2));
					result = true;
					break;
				case WeaponStatUpgrade.Types.SetWeaponAmmoBonus:
					this._itemViewsCache[productItemId].ActiveBonus = ((lastIngredientActiveBonus != (WeaponStatUpgrade.Types)(-2)) ? lastIngredientActiveBonus : this._itemViewsCache[ingredientItemId].ActiveBonus);
					LocalPlayer.Inventory.SortInventoryViewsByBonus(LocalPlayer.Inventory.InventoryItemViewsCache[ingredientItemId][0], this._itemViewsCache[productItemId].ActiveBonus, false);
					result = true;
					break;
				case WeaponStatUpgrade.Types.AddMaxAmountBonus:
					LocalPlayer.Inventory.AddMaxAmountBonus(ingredientItemId, Mathf.RoundToInt(bonuses[i]._amount));
					result = true;
					break;
				case WeaponStatUpgrade.Types.SetMaxAmountBonus:
					LocalPlayer.Inventory.AddMaxAmountBonus(ingredientItemId, Mathf.RoundToInt(bonuses[i]._amount));
					result = true;
					break;
				case WeaponStatUpgrade.Types.PoisonnedAmmo:
					if (batched && this._itemViewsCache[productItemId]._allowMultiView)
					{
						this._itemViewsCache[productItemId].SetMultiviewsBonus(WeaponStatUpgrade.Types.PoisonnedAmmo);
					}
					else
					{
						this._itemViewsCache[productItemId].ActiveBonus = WeaponStatUpgrade.Types.PoisonnedAmmo;
					}
					result = true;
					break;
				case WeaponStatUpgrade.Types.SetArrowMaxAmountBonus:
					LocalPlayer.Inventory.AddMaxAmountBonus(ItemDatabase.ItemByName("Arrows")._id, Mathf.RoundToInt(bonuses[i]._amount));
					result = true;
					break;
				case WeaponStatUpgrade.Types.BurningWeaponExtra:
				{
					BurnableCloth componentInChildren4 = this._itemViewsCache[productItemId]._held.GetComponentInChildren<BurnableCloth>();
					if (componentInChildren4 && this._itemViewsCache[productItemId].ActiveBonus == WeaponStatUpgrade.Types.BurningWeapon)
					{
						this._itemViewsCache[productItemId].ActiveBonus = WeaponStatUpgrade.Types.BurningWeaponExtra;
						componentInChildren4.EnableBurnableClothExtra();
					}
					result = true;
					break;
				}
				case WeaponStatUpgrade.Types.Incendiary:
					this._itemViewsCache[productItemId].ActiveBonus = WeaponStatUpgrade.Types.Incendiary;
					result = true;
					break;
				default:
					goto IL_3DD;
				}
				IL_4C5:
				i++;
				continue;
				IL_3DD:
				if (heldWeaponInfo)
				{
					FieldInfo field = typeFromHandle.GetField(bonuses[i]._type.ToString(), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					float upgradeBonusAmount = this.GetUpgradeBonusAmount(productItemId, ingredientItemId, bonuses[i], upgradeCount);
					if (field.FieldType == typeof(float))
					{
						field.SetValue(heldWeaponInfo, (float)field.GetValue(heldWeaponInfo) + upgradeBonusAmount);
					}
					else if (field.FieldType == typeof(int))
					{
						field.SetValue(heldWeaponInfo, (int)field.GetValue(heldWeaponInfo) + Mathf.RoundToInt(upgradeBonusAmount));
					}
					result = true;
					GameStats.UpgradesAdded.Invoke(upgradeCount);
				}
				else
				{
					Debug.LogError("Attempting to upgrade " + this._itemViewsCache[productItemId].ItemCache._name + " which doesn't reference its weaponInfo component.");
				}
				goto IL_4C5;
			}
			return result;
		}

		public float GetUpgradeBonusAmount(int productItemId, int ingredientItemId, WeaponStatUpgrade bonus, int upgradeCount)
		{
			AnimationCurve bonusDecayCurve = this._upgradeCog.SupportedItemsCache[ingredientItemId]._bonusDecayCurve;
			float num = 0f;
			if (bonusDecayCurve == null)
			{
				num = bonus._amount * (float)upgradeCount;
			}
			else
			{
				float num2 = (float)LocalPlayer.Inventory.GetAmountOfUpgrades(productItemId, ingredientItemId);
				float num3 = (float)ItemDatabase.ItemById(productItemId)._maxUpgradesAmount;
				for (int i = 0; i < upgradeCount; i++)
				{
					num += bonus._amount * bonusDecayCurve.Evaluate((num2 + (float)i) / num3);
				}
			}
			return num;
		}

		private void ToggleItemInventoryView(int itemId, WeaponStatUpgrade.Types activeBonus)
		{
			ReceipeIngredient receipeIngredient = this._ingredients.FirstOrDefault((ReceipeIngredient i) => i._itemID == itemId);
			int num = (receipeIngredient == null) ? 0 : receipeIngredient._amount;
			bool flag = num > 0;
			if (this._itemViewsCache.ContainsKey(itemId))
			{
				if (this._itemViewsCache[itemId]._allowMultiView)
				{
					if (activeBonus == (WeaponStatUpgrade.Types)(-2))
					{
						this._itemViewsCache[itemId].SetMultiViewAmount(num, activeBonus);
					}
					else
					{
						this._itemViewsCache[itemId].SetMultiViewAmount(num - this._itemViewsCache[itemId].AmountOfMultiviewWithoutBonus(itemId, activeBonus), activeBonus);
					}
				}
				else
				{
					if (activeBonus != (WeaponStatUpgrade.Types)(-2))
					{
						this._itemViewsCache[itemId].ActiveBonus = activeBonus;
					}
					if (this._itemViewsCache[itemId].gameObject.activeSelf != flag)
					{
						this._itemViewsCache[itemId].gameObject.SetActive(flag);
					}
				}
			}
			else if (LocalPlayer.Inventory.InventoryItemViewsCache.ContainsKey(itemId))
			{
				this._lambdaMultiView.SetAnyMultiViewAmount(LocalPlayer.Inventory.InventoryItemViewsCache[itemId][0], this._lambdaMultiView.transform, num, activeBonus);
			}
		}

		private void IngredientCleanUp()
		{
			this._ingredients.Clear();
			IEnumerable<Item> enumerable = ItemDatabase.ItemsByType(Item.Types.CraftingMaterial);
			foreach (Item current in enumerable)
			{
				this.ToggleItemInventoryView(current._id, (WeaponStatUpgrade.Types)(-1));
			}
			this._lambdaMultiView.ClearMultiViews();
		}
	}
}
