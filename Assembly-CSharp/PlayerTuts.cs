using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Items;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UnityEngine;

[DoNotSerializePublic]
public class PlayerTuts : MonoBehaviour
{
	public SurvivalBook Book;

	[ItemIdPicker(Item.Types.Equipment)]
	public int[] _axesItemId;

	[ItemIdPicker(Item.Types.CraftingMaterial)]
	public int[] _craftTutReceiepeItemId;

	[ItemIdPicker(Item.Types.Equipment)]
	public int[] _worldMapPiecesItemIds;

	[ItemIdPicker(Item.Types.Equipment)]
	public int _molotovItemId;

	public float _lighterTutDelay = 60f;

	private float NextLightTutTime;

	private int TotalPlaneBodies = 130;

	private bool Showing;

	[SerializeThis]
	private int PlaneBodies;

	[SerializeThis]
	private bool bookStep1Done;

	[SerializeThis]
	private bool bookStep2Done;

	[SerializeThis]
	private bool bookStep3Done;

	[SerializeThis]
	private bool ShouldShowLighter;

	[SerializeThis]
	private bool bookTutDone;

	[SerializeThis]
	private bool craftingTutDone;

	[SerializeThis]
	private bool receipeTutDone;

	[SerializeThis]
	private bool axeTutDone;

	[SerializeThis]
	private bool treeStructureTutDone;

	[SerializeThis]
	private bool worldMapTutDone;

	[SerializeThis]
	private bool molotovTutDone;

	private void Start()
	{
		this.NextLightTutTime = Time.realtimeSinceStartup + this._lighterTutDelay * 3f;
		base.Invoke("ShowSprint", 300f);
	}

	private void Update()
	{
		bool flag = LocalPlayer.Inventory.HasInSlot(Item.EquipmentSlot.LeftHand, LocalPlayer.Inventory.LastLight._itemId) || LocalPlayer.Inventory.IsWeaponBurning || LocalPlayer.Stats.Dead;
		if (Clock.InCave && !flag && !LocalPlayer.WaterViz.InWater)
		{
			this.TryShowLighter();
		}
		else if (Clock.Dark && !flag && this.ShouldShowLighter)
		{
			this.TryShowLighter();
			this.ShouldShowLighter = false;
		}
		else if (((!Clock.InCave && !Clock.Dark) || LocalPlayer.WaterViz.InWater || LocalPlayer.Stats.Dead) && Scene.HudGui.Tut_Lighter.activeSelf)
		{
			this.HideLighter();
		}
		if (!this.molotovTutDone)
		{
			if (LocalPlayer.Inventory.HasInSlot(Item.EquipmentSlot.RightHand, this._molotovItemId))
			{
				this.ShowMolotovTut();
			}
			else
			{
				this.HideMolotovTut();
			}
		}
		if (!this.craftingTutDone || !this.receipeTutDone)
		{
			if (LocalPlayer.Inventory._craftingCog.HasValideRecipe)
			{
				this.CloseRecipeTut();
			}
			else
			{
				bool flag2 = true;
				for (int i = 0; i < this._craftTutReceiepeItemId.Length; i++)
				{
					if (!LocalPlayer.Inventory.Owns(this._craftTutReceiepeItemId[i]))
					{
						flag2 = false;
						break;
					}
				}
				if (flag2)
				{
					if (!this.craftingTutDone && LocalPlayer.Inventory.CurrentView == PlayerInventory.PlayerViews.World)
					{
						this.CraftingTut();
					}
					else if (!this.receipeTutDone && LocalPlayer.Inventory.CurrentView == PlayerInventory.PlayerViews.Inventory)
					{
						this.ShowReceipeTut();
					}
				}
			}
		}
		if (!this.axeTutDone)
		{
			for (int j = 0; j < this._axesItemId.Length; j++)
			{
				if (LocalPlayer.Inventory.HasInSlot(Item.EquipmentSlot.RightHand, this._axesItemId[j]))
				{
					this.ShowAxeTut();
					break;
				}
			}
		}
	}

	public void ToggleVisibility(bool visible)
	{
		Scene.HudGui.Grid.gameObject.SetActive(visible);
	}

	public void GotBook()
	{
		if (!this.Showing)
		{
			this.Showing = true;
			Scene.HudGui.Tut_OpenBook.SetActive(true);
			Scene.HudGui.Grid.Reposition();
		}
		else
		{
			base.Invoke("GotBook", 5f);
		}
	}

	public void ShowBookTut()
	{
		if (!this.bookTutDone)
		{
			this.bookTutDone = true;
			Scene.HudGui.bookTutorial.SetActive(true);
		}
	}

	public void HideBookTut()
	{
		if (Scene.HudGui.bookTutorial.activeSelf)
		{
			Scene.HudGui.bookTutorial.SetActive(false);
		}
	}

	public void ShowMolotovTut()
	{
		if (!this.molotovTutDone && !this.Showing)
		{
			this.Showing = true;
			Scene.HudGui.Tut_MolotovTutorial.SetActive(true);
		}
	}

	public void HideMolotovTut()
	{
		if (Scene.HudGui.Tut_MolotovTutorial.activeSelf)
		{
			this.Showing = false;
			Scene.HudGui.Tut_MolotovTutorial.SetActive(false);
		}
	}

	public void MolotovTutDone()
	{
		this.molotovTutDone = true;
		this.HideMolotovTut();
	}

	public void ShowStoryClueTut()
	{
		if (!Scene.HudGui.Tut_StoryClue.activeSelf)
		{
			Scene.HudGui.Tut_StoryClue.SetActive(true);
			Scene.HudGui.Grid.Reposition();
		}
	}

	public void HideStoryClueTut()
	{
		if (Scene.HudGui.Tut_StoryClue.activeSelf)
		{
			Scene.HudGui.Tut_StoryClue.SetActive(false);
			Scene.HudGui.Grid.Reposition();
		}
	}

	public void ShowSprint()
	{
	}

	public void CloseSprint()
	{
	}

	public void ShowStep1Tut()
	{
		if (!this.bookStep1Done && this.Book.InitShowStep1())
		{
			this.bookStep1Done = true;
			Scene.HudGui.Tut_BookStage1.SetActive(true);
			Scene.HudGui.Grid.Reposition();
		}
	}

	public void ShowStep2Tut()
	{
		if (!this.bookStep2Done && this.Book.InitShowStep2())
		{
			this.bookStep2Done = true;
			Scene.HudGui.Tut_BookStage2.SetActive(true);
			Scene.HudGui.Grid.Reposition();
		}
	}

	public void ShowStep3Tut()
	{
		if (!this.bookStep3Done && this.Book.InitShowStep3())
		{
			this.bookStep3Done = true;
			Scene.HudGui.Tut_BookStage3.SetActive(true);
			Scene.HudGui.Grid.Reposition();
		}
	}

	public void ShelterTutOff()
	{
		Scene.HudGui.Tut_Shelter.SetActive(false);
	}

	public void ShowDeathMP()
	{
		Scene.HudGui.Tut_DeathMP.SetActive(true);
		Scene.HudGui.Grid.Reposition();
	}

	public void HideDeathMP()
	{
		Scene.HudGui.Tut_DeathMP.SetActive(false);
		Scene.HudGui.Grid.Reposition();
	}

	public void ShowReviveMP()
	{
		if (!Scene.HudGui.Tut_ReviveMP.activeSelf)
		{
			Scene.HudGui.Tut_ReviveMP.SetActive(true);
			Scene.HudGui.Grid.Reposition();
		}
	}

	public void HideReviveMP()
	{
		if (Scene.HudGui && Scene.HudGui.Tut_ReviveMP.activeSelf)
		{
			Scene.HudGui.Tut_ReviveMP.SetActive(false);
			Scene.HudGui.Grid.Reposition();
		}
	}

	public void TryShowLighter()
	{
		if (this.NextLightTutTime < Time.realtimeSinceStartup && !LocalPlayer.WaterViz.InWater)
		{
			this.ShowLighter();
		}
	}

	public void ShowLighter()
	{
		this.NextLightTutTime = Time.realtimeSinceStartup + this._lighterTutDelay;
		Scene.HudGui.Tut_Lighter.SetActive(true);
	}

	public void HideLighter()
	{
		Scene.HudGui.Tut_Lighter.SetActive(false);
	}

	public void LowHealthTutorial()
	{
		if (!Scene.HudGui.Tut_Health.activeSelf)
		{
			Scene.HudGui.Tut_Health.SetActive(true);
			Scene.HudGui.Grid.Reposition();
		}
	}

	public void CloseLowHealthTutorial()
	{
		if (Scene.HudGui.Tut_Health.activeSelf)
		{
			Scene.HudGui.Tut_Health.SetActive(false);
			Scene.HudGui.Grid.Reposition();
		}
	}

	public void HungryTutorial()
	{
		if (!Scene.HudGui.Tut_Hungry.activeSelf)
		{
			Scene.HudGui.Tut_Hungry.SetActive(true);
			Scene.HudGui.Grid.Reposition();
		}
	}

	public void CloseHungryTutorial()
	{
		if (Scene.HudGui.Tut_Hungry.activeSelf)
		{
			Scene.HudGui.Tut_Hungry.SetActive(false);
			Scene.HudGui.Grid.Reposition();
		}
	}

	public void ShowThirstTut()
	{
		if (!this.Showing)
		{
			this.Showing = true;
			Scene.HudGui.Tut_Thirst.SetActive(true);
			base.Invoke("ThirstTutOff", 7f);
		}
		else
		{
			base.Invoke("ShowThirstTut", 5f);
		}
	}

	public void ThirstTutOff()
	{
		if (this.Showing)
		{
			this.Showing = false;
			Scene.HudGui.Tut_Thirst.SetActive(false);
		}
	}

	public void ShowThirstyTut()
	{
		if (!Scene.HudGui.Tut_Thirsty.activeSelf)
		{
			Scene.HudGui.Tut_Thirsty.SetActive(true);
			Scene.HudGui.Grid.Reposition();
		}
	}

	public void HideThirstyTut()
	{
		if (Scene.HudGui.Tut_Thirsty.activeSelf)
		{
			Scene.HudGui.Tut_Thirsty.SetActive(false);
			Scene.HudGui.Grid.Reposition();
		}
	}

	public void ShowStarvationTut()
	{
		if (!this.Showing)
		{
			this.Showing = true;
			Scene.HudGui.Tut_Starvation.SetActive(true);
			base.Invoke("StarvationTutOff", 7f);
		}
		else
		{
			base.Invoke("ShowStarvationTut", 5f);
		}
	}

	public void StarvationTutOff()
	{
		if (this.Showing)
		{
			this.Showing = false;
			Scene.HudGui.Tut_Starvation.SetActive(false);
		}
	}

	public void ShowColdDamageTut()
	{
		if (!this.Showing)
		{
			this.Showing = true;
			Scene.HudGui.Tut_ColdDamage.SetActive(true);
			base.Invoke("ColdDamageTutOff", 7f);
		}
		else
		{
			base.Invoke("ShowColdDamageTut", 5f);
		}
	}

	public void ColdDamageTutOff()
	{
		if (this.Showing)
		{
			this.Showing = false;
			Scene.HudGui.Tut_ColdDamage.SetActive(false);
		}
	}

	public void ShowMPHostAltTabTut()
	{
		if (!this.Showing)
		{
			this.Showing = true;
			Scene.HudGui.Tut_HostAltTabMP.SetActive(true);
			base.Invoke("MPHostAltTabOff", 7f);
		}
		else
		{
			base.Invoke("ShowMPHostAltTabTut", 5f);
		}
	}

	public void MPHostAltTabOff()
	{
		if (this.Showing)
		{
			this.Showing = false;
			Scene.HudGui.Tut_HostAltTabMP.SetActive(false);
		}
	}

	public void ShowNoInventoryUnderWater()
	{
		if (!Scene.HudGui.Tut_NoInventoryUnderwater.activeSelf)
		{
			Scene.HudGui.Tut_NoInventoryUnderwater.SetActive(true);
			Scene.HudGui.Grid.Reposition();
			base.Invoke("HideNoInventoryUnderWater", 7f);
		}
	}

	public void HideNoInventoryUnderWater()
	{
		Scene.HudGui.Tut_NoInventoryUnderwater.SetActive(false);
		Scene.HudGui.Grid.Reposition();
	}

	public void LowEnergyTutorial()
	{
		if (!Scene.HudGui.Tut_Energy.activeSelf)
		{
			Scene.HudGui.Tut_Energy.SetActive(true);
			Scene.HudGui.Grid.Reposition();
		}
	}

	public void CloseLowEnergyTutorial()
	{
		if (Scene.HudGui.Tut_Energy.activeSelf)
		{
			Scene.HudGui.Tut_Energy.SetActive(false);
			Scene.HudGui.Grid.Reposition();
		}
	}

	public void ColdTut()
	{
		if (!Scene.HudGui.Tut_Cold.activeSelf)
		{
			Scene.HudGui.Tut_Cold.SetActive(true);
			Scene.HudGui.Grid.Reposition();
		}
	}

	public void CloseColdTut()
	{
		if (Scene.HudGui.Tut_Cold.activeSelf)
		{
			Scene.HudGui.Tut_Cold.SetActive(false);
			Scene.HudGui.Grid.Reposition();
		}
	}

	public void BuildToolsTut()
	{
	}

	private void CraftingTut()
	{
		if (!this.Showing)
		{
			this.Showing = true;
			this.craftingTutDone = true;
			Scene.HudGui.Tut_Crafting.SetActive(true);
			Scene.HudGui.Grid.Reposition();
			base.Invoke("CloseCraftingTut", 10f);
		}
	}

	public void CloseCraftingTut()
	{
		if (Scene.HudGui.Tut_Crafting.activeSelf)
		{
			this.Showing = false;
			Scene.HudGui.Tut_Crafting.SetActive(false);
			Scene.HudGui.Grid.Reposition();
		}
	}

	private void ShowReceipeTut()
	{
		if (!Scene.HudGui.CraftingMessage.activeSelf)
		{
			Scene.HudGui.CraftingMessage.SetActive(true);
			base.Invoke("CloseRecipeTut", 6f);
		}
	}

	public void CloseRecipeTut()
	{
		if (Scene.HudGui.CraftingMessage.activeSelf)
		{
			this.receipeTutDone = true;
			Scene.HudGui.CraftingMessage.SetActive(false);
		}
	}

	public void ShowTreeStructureTut()
	{
		if (Scene.HudGui.Tut_AnchorAccessibleBuildings && !this.treeStructureTutDone && !Scene.HudGui.Tut_AnchorAccessibleBuildings.activeSelf)
		{
			this.treeStructureTutDone = true;
			Scene.HudGui.Tut_AnchorAccessibleBuildings.SetActive(true);
			base.Invoke("CloseTreeStructureTut", 6f);
		}
	}

	public void CloseTreeStructureTut()
	{
		if (Scene.HudGui.Tut_AnchorAccessibleBuildings.activeSelf)
		{
			Scene.HudGui.Tut_AnchorAccessibleBuildings.SetActive(false);
		}
	}

	public void RecordedBody()
	{
		base.CancelInvoke("TurnOffBodiesMessage");
		Scene.HudGui.PlaneBodiesMsg.SetActive(false);
		Scene.HudGui.Grid.Reposition();
		this.PlaneBodies++;
		Scene.HudGui.PlaneBodiesMsg.SetActive(true);
		Scene.HudGui.PlaneBodiesLabel.text = this.PlaneBodies + "/" + this.TotalPlaneBodies.ToString();
		Scene.HudGui.Grid.Reposition();
		base.Invoke("TurnOffBodiesMessage", 3f);
	}

	private void TurnOffBodiesMessage()
	{
		if (Scene.HudGui.PlaneBodiesMsg.activeSelf)
		{
			Scene.HudGui.PlaneBodiesMsg.SetActive(false);
			Scene.HudGui.Grid.Reposition();
		}
	}

	private void ShowAxeTut()
	{
		if (!this.Showing)
		{
			this.Showing = true;
			this.axeTutDone = true;
			Scene.HudGui.Tut_Axe.SetActive(true);
			Scene.HudGui.Grid.Reposition();
			base.Invoke("CloseAxeTut", 15f);
		}
	}

	private void CloseAxeTut()
	{
		if (Scene.HudGui.Tut_Axe.activeSelf)
		{
			this.Showing = false;
			Scene.HudGui.Tut_Axe.SetActive(false);
			Scene.HudGui.Grid.Reposition();
		}
	}

	private void ShowWorldMapTut(int mapId)
	{
		this.worldMapTutDone = true;
		base.StartCoroutine(this.ShowWorldMapTutDelayed(mapId));
	}

	[DebuggerHidden]
	private IEnumerator ShowWorldMapTutDelayed(int mapId)
	{
		PlayerTuts.<ShowWorldMapTutDelayed>c__Iterator1A2 <ShowWorldMapTutDelayed>c__Iterator1A = new PlayerTuts.<ShowWorldMapTutDelayed>c__Iterator1A2();
		<ShowWorldMapTutDelayed>c__Iterator1A.mapId = mapId;
		<ShowWorldMapTutDelayed>c__Iterator1A.<$>mapId = mapId;
		<ShowWorldMapTutDelayed>c__Iterator1A.<>f__this = this;
		return <ShowWorldMapTutDelayed>c__Iterator1A;
	}

	[DebuggerHidden]
	private IEnumerator CloseWorldMapTut(int mapId)
	{
		PlayerTuts.<CloseWorldMapTut>c__Iterator1A3 <CloseWorldMapTut>c__Iterator1A = new PlayerTuts.<CloseWorldMapTut>c__Iterator1A3();
		<CloseWorldMapTut>c__Iterator1A.mapId = mapId;
		<CloseWorldMapTut>c__Iterator1A.<$>mapId = mapId;
		return <CloseWorldMapTut>c__Iterator1A;
	}

	public void CloseBuildToolsTut()
	{
	}

	public void CaveTut()
	{
	}

	private void ResetCaveTutDelay()
	{
	}

	private void CloseCaveTut()
	{
	}

	public void ShowMushroomPage()
	{
	}

	private void CloseMushroomPage()
	{
	}

	private void ResetMushroomDelay()
	{
	}

	public void BloodyTut()
	{
		if (!Scene.HudGui.Tut_Bloody.activeSelf)
		{
			Scene.HudGui.Tut_Bloody.SetActive(true);
			Scene.HudGui.Grid.Reposition();
		}
	}

	public void CloseBloodyTut()
	{
		if (Scene.HudGui.Tut_Bloody.activeSelf)
		{
			Scene.HudGui.Tut_Bloody.SetActive(false);
			Scene.HudGui.Grid.Reposition();
		}
	}

	public void CloseAllBookTuts()
	{
		Scene.HudGui.Tut_BookStage1.SetActive(false);
		Scene.HudGui.Tut_BookStage2.SetActive(false);
		Scene.HudGui.Tut_BookStage3.SetActive(false);
		Scene.HudGui.Tut_OpenBook.SetActive(false);
		Scene.HudGui.Tut_Axe.SetActive(false);
		Scene.HudGui.Tut_Crafting.SetActive(false);
		Scene.HudGui.Grid.Reposition();
		this.Showing = false;
	}
}
