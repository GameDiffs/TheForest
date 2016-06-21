using System;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UnityEngine;

public class SurvivalBook : MonoBehaviour
{
	public GameObject Index;

	public GameObject Pages;

	public GameObject SurvivalPage1;

	public GameObject SurvivalPage2;

	public GameObject SurvivalPage3;

	public SelectPageNumber SurvivalPage1Tab;

	public SelectPageNumber SurvivalPage2Tab;

	public SelectPageNumber SurvivalPage3Tab;

	private PlayerTuts Tuts;

	private Transform CurrentPage;

	private Transform WantedPage;

	private Transform MyTransform;

	public Transform BookPos;

	private bool ShowStep1;

	private bool ShowStep2;

	private bool ShowStep3;

	private void Awake()
	{
		this.MyTransform = base.transform;
		this.Tuts = LocalPlayer.Tuts;
	}

	private void OnEnable()
	{
		if (LocalPlayer.Inventory && LocalPlayer.Inventory.CurrentView > PlayerInventory.PlayerViews.Loading)
		{
			Scene.HudGui.bookUICam.SetActive(true);
		}
	}

	private void OnDisable()
	{
		if (Scene.HudGui)
		{
			Scene.HudGui.bookUICam.SetActive(false);
		}
	}

	private void Update()
	{
		if (LocalPlayer.Inventory.CurrentView == PlayerInventory.PlayerViews.Book && (LocalPlayer.Animator.GetBool("lighterHeld") || LocalPlayer.Animator.GetBool("flashLightHeld")))
		{
			LocalPlayer.SpecialItems.SendMessage("StashLighter");
		}
		if (TheForest.Utils.Input.IsGamePad && TheForest.Utils.Input.GetButtonDown("Drop"))
		{
			LocalPlayer.Create.CloseTheBook();
		}
	}

	private void CloseBook()
	{
		base.BroadcastMessage("NotActive", SendMessageOptions.DontRequireReceiver);
	}

	public bool InitShowStep1()
	{
		this.ShowStep1 = true;
		return this.SurvivalPage1Tab.Highlight(this.SurvivalPage1);
	}

	public bool InitShowStep2()
	{
		this.ShowStep2 = true;
		return this.SurvivalPage1Tab.Highlight(this.SurvivalPage2);
	}

	public bool InitShowStep3()
	{
		this.ShowStep3 = true;
		return this.SurvivalPage1Tab.Highlight(this.SurvivalPage3);
	}

	private void CheckPage()
	{
		if (this.ShowStep1 && this.SurvivalPage1Tab.Highlight(this.SurvivalPage1))
		{
			this.ShowStep1 = false;
			this.Tuts.CloseAllBookTuts();
		}
		if (this.ShowStep2 && this.SurvivalPage2Tab.Highlight(this.SurvivalPage2))
		{
			this.ShowStep2 = false;
			this.Tuts.CloseAllBookTuts();
		}
		if (this.ShowStep3 && this.SurvivalPage3Tab.Highlight(this.SurvivalPage3))
		{
			this.ShowStep3 = false;
			this.Tuts.CloseAllBookTuts();
		}
	}

	private void TurnOffAllPages()
	{
		for (int i = this.Pages.transform.childCount - 1; i >= 0; i--)
		{
			this.Pages.transform.GetChild(i).gameObject.SetActive(false);
		}
	}
}
