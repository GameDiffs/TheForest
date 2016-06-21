using System;
using UnityEngine;

public class Craft_Item : MonoBehaviour
{
	public GameObject MyRock;

	public GameObject MyStick;

	public GameObject MyBottle;

	public GameObject MyCloth;

	public GameObject MyAloe;

	public GameObject MyMariGold;

	public GameObject MyCBoard;

	public GameObject MyRope;

	public GameObject MyCoins;

	public GameObject[] MyFeathersModel;

	public int MyFeathers;

	public GameObject CraftedMolotov;

	public GameObject CraftedBombTimed;

	public GameObject CraftedAxe;

	public GameObject CraftedBow;

	public GameObject CraftedArrow;

	public GameObject CraftedFireStick;

	public GameObject CraftedMeds;

	public GameObject CraftButton;

	public Material SelectedMaterial;

	private Material NormalMaterial;

	private bool CanCraft;

	private bool CanCraftBow;

	private bool CanCraftMeds;

	private bool CanCraftArrow;

	private bool CanCraftFireStick;

	private bool CanCraftAxe;

	private bool CanCraftMolotov;

	private bool CanCraftBombTimed;

	public GameObject CraftSfx;

	public GameObject CraftSfx2;

	public int Coins;

	public int Feathers;

	private void Awake()
	{
		this.NormalMaterial = base.gameObject.GetComponent<Renderer>().material;
	}

	private void Update()
	{
		if (this.MyAloe.activeSelf && this.MyMariGold.activeSelf && !this.CraftedMeds.activeSelf && !this.MyBottle.activeSelf && !this.MyStick.activeSelf && !this.MyCloth.activeSelf && !this.MyRope.activeSelf && !this.MyRock.activeSelf && !this.MyCBoard.activeSelf && !this.MyCoins.activeSelf && this.Feathers == 0 && this.Coins == 0)
		{
			if (!this.CanCraftMeds)
			{
				this.CraftSfx2.GetComponent<AudioSource>().Play();
			}
			this.CanCraftAxe = false;
			this.CanCraftMolotov = false;
			this.CanCraftBombTimed = false;
			this.CanCraftBow = false;
			this.CanCraftFireStick = false;
			this.CanCraftMeds = true;
			this.CanCraftArrow = false;
		}
		else
		{
			this.CanCraftMeds = false;
		}
		if (this.MyStick.activeSelf && this.MyFeathers == 5 && !this.MyAloe.activeSelf && !this.MyMariGold.activeSelf && !this.MyBottle.activeSelf && !this.MyRope.activeSelf && !this.MyRock.activeSelf && !this.MyCBoard.activeSelf && !this.MyCoins.activeSelf && !this.MyCloth.activeSelf)
		{
			if (!this.CanCraftArrow)
			{
				this.CraftSfx2.GetComponent<AudioSource>().Play();
			}
			this.CanCraftAxe = false;
			this.CanCraftMolotov = false;
			this.CanCraftBombTimed = false;
			this.CanCraftBow = false;
			this.CanCraftFireStick = false;
			this.CanCraftMeds = false;
			this.CanCraftArrow = true;
		}
		else
		{
			this.CanCraftArrow = false;
		}
		if (this.MyStick.activeSelf && this.MyCloth.activeSelf && !this.CraftedFireStick.activeSelf && !this.MyAloe.activeSelf && !this.MyMariGold.activeSelf && !this.MyBottle.activeSelf && !this.MyRope.activeSelf && !this.MyRock.activeSelf && !this.MyCBoard.activeSelf && !this.MyCoins.activeSelf && this.Feathers == 0)
		{
			if (!this.CanCraftFireStick)
			{
				this.CraftSfx2.GetComponent<AudioSource>().Play();
			}
			this.CanCraftAxe = false;
			this.CanCraftMolotov = false;
			this.CanCraftBombTimed = false;
			this.CanCraftBow = false;
			this.CanCraftFireStick = true;
			this.CanCraftMeds = false;
			this.CanCraftArrow = false;
		}
		else
		{
			this.CanCraftFireStick = false;
		}
		if (this.MyStick.activeSelf && this.MyCloth.activeSelf && this.MyRope.activeSelf && !this.CraftedBow.activeSelf && !this.MyAloe.activeSelf && !this.MyMariGold.activeSelf && !this.MyBottle.activeSelf && !this.MyRock.activeSelf && !this.MyCBoard.activeSelf && !this.MyCoins.activeSelf && this.Feathers == 0)
		{
			if (!this.CanCraftBow)
			{
				this.CraftSfx2.GetComponent<AudioSource>().Play();
			}
			this.CanCraftAxe = false;
			this.CanCraftMolotov = false;
			this.CanCraftBombTimed = false;
			this.CanCraftBow = true;
			this.CanCraftFireStick = false;
			this.CanCraftMeds = false;
			this.CanCraftArrow = false;
		}
		else
		{
			this.CanCraftBow = false;
		}
		if (this.MyBottle.activeSelf && this.MyCBoard.activeSelf && !this.CraftedBombTimed.activeSelf && this.Coins > 0 && !this.MyAloe.activeSelf && !this.MyMariGold.activeSelf && !this.MyStick.activeSelf && !this.MyCloth.activeSelf && !this.MyRope.activeSelf && !this.MyRock.activeSelf && this.Feathers == 0)
		{
			if (!this.CanCraftBombTimed)
			{
				this.CraftSfx2.GetComponent<AudioSource>().Play();
			}
			this.CanCraftAxe = false;
			this.CanCraftMolotov = false;
			this.CanCraftBombTimed = true;
			this.CanCraftBow = false;
			this.CanCraftFireStick = false;
			this.CanCraftMeds = false;
			this.CanCraftArrow = false;
		}
		else
		{
			this.CanCraftBombTimed = false;
		}
		if (this.MyRock.activeSelf && this.MyStick.activeSelf && !this.CraftedAxe.activeSelf && !this.MyAloe.activeSelf && !this.MyMariGold.activeSelf && !this.MyBottle.activeSelf && !this.MyRope.activeSelf && !this.MyCBoard.activeSelf && !this.MyCloth.activeSelf && !this.MyCoins.activeSelf && this.Feathers == 0)
		{
			if (!this.CanCraftAxe)
			{
				this.CraftSfx2.GetComponent<AudioSource>().Play();
			}
			this.CanCraftAxe = true;
			this.CanCraftMolotov = false;
			this.CanCraftBombTimed = false;
			this.CanCraftBow = false;
			this.CanCraftFireStick = false;
			this.CanCraftMeds = false;
			this.CanCraftArrow = false;
		}
		else
		{
			this.CanCraftAxe = false;
		}
		if (this.MyBottle.activeSelf && this.MyCloth.activeSelf && !this.CraftedMolotov.activeSelf && !this.MyAloe.activeSelf && !this.MyMariGold.activeSelf && !this.MyRope.activeSelf && !this.MyStick.activeSelf && !this.MyRock.activeSelf && !this.MyCBoard.activeSelf && !this.MyCoins.activeSelf && this.Feathers == 0)
		{
			if (!this.CanCraftMolotov)
			{
				this.CraftSfx2.GetComponent<AudioSource>().Play();
			}
			this.CanCraftAxe = false;
			this.CanCraftMolotov = true;
			this.CanCraftBombTimed = false;
			this.CanCraftBow = false;
			this.CanCraftFireStick = false;
			this.CanCraftMeds = false;
			this.CanCraftArrow = false;
		}
		else
		{
			this.CanCraftMolotov = false;
		}
		if (this.CanCraftAxe || this.CanCraftMolotov || this.CanCraftBombTimed || this.CanCraftFireStick || this.CanCraftBow || this.CanCraftArrow || this.CanCraftMeds)
		{
			base.gameObject.GetComponent<Renderer>().enabled = true;
			base.gameObject.GetComponent<Collider>().enabled = true;
		}
		else
		{
			base.gameObject.GetComponent<Renderer>().enabled = false;
			base.gameObject.GetComponent<Collider>().enabled = false;
		}
	}

	private void OnMouseExitCollider()
	{
		this.CraftButton.SetActive(false);
		base.gameObject.GetComponent<Renderer>().material = this.NormalMaterial;
	}

	private void OnMouseOverCollider()
	{
		base.gameObject.GetComponent<Renderer>().material = this.SelectedMaterial;
		if (this.CanCraftAxe || this.CanCraftMolotov || this.CanCraftBombTimed || this.CanCraftFireStick || this.CanCraftBow || this.CanCraftArrow || this.CanCraftMeds)
		{
			this.CraftButton.SetActive(true);
		}
		else
		{
			this.CraftButton.SetActive(false);
		}
		if (Input.GetButtonDown("AltFire"))
		{
			this.CraftSfx.GetComponent<AudioSource>().Play();
			if (this.CanCraftMeds)
			{
				this.CraftMeds();
			}
			if (this.CanCraftArrow)
			{
				this.CraftArrow();
			}
			if (this.CanCraftAxe)
			{
				this.CraftAxe();
			}
			else if (this.CanCraftMolotov)
			{
				this.CraftMolotov();
			}
			else if (this.CanCraftBombTimed)
			{
				this.CraftBombTimed();
			}
			else if (this.CanCraftBow)
			{
				this.CraftBow();
			}
			else if (this.CanCraftFireStick)
			{
				this.CraftFireStick();
			}
		}
	}

	private void CraftFireStick()
	{
		this.CraftButton.SetActive(false);
		this.MyStick.SetActive(false);
		this.MyCloth.SetActive(false);
		this.CraftedFireStick.SetActive(true);
	}

	private void CraftBow()
	{
		this.CraftButton.SetActive(false);
		this.MyStick.SetActive(false);
		this.MyCloth.SetActive(false);
		this.MyRope.SetActive(false);
		this.CraftedBow.SetActive(true);
	}

	private void CraftArrow()
	{
		this.CraftButton.SetActive(false);
		this.MyStick.SetActive(false);
		this.MyFeathersModel[this.MyFeathers - 1].SetActive(false);
		this.MyFeathers--;
		this.MyFeathersModel[this.MyFeathers - 1].SetActive(false);
		this.MyFeathers--;
		this.MyFeathersModel[this.MyFeathers - 1].SetActive(false);
		this.MyFeathers--;
		this.MyFeathersModel[this.MyFeathers - 1].SetActive(false);
		this.MyFeathers--;
		this.MyFeathersModel[this.MyFeathers - 1].SetActive(false);
		this.MyFeathers--;
		this.CraftedArrow.SetActive(true);
	}

	private void CraftMeds()
	{
		this.CraftButton.SetActive(false);
		this.MyAloe.SetActive(false);
		this.MyMariGold.SetActive(false);
		this.CraftedMeds.SetActive(true);
	}

	private void CraftAxe()
	{
		this.CraftButton.SetActive(false);
		this.MyRock.SetActive(false);
		this.MyStick.SetActive(false);
		this.CraftedAxe.SetActive(true);
	}

	private void CraftMolotov()
	{
		this.CraftButton.SetActive(false);
		this.MyCloth.SetActive(false);
		this.MyBottle.SetActive(false);
		this.CraftedMolotov.SetActive(true);
	}

	private void CraftBombTimed()
	{
		this.CraftButton.SetActive(false);
		this.MyCBoard.SetActive(false);
		this.MyBottle.SetActive(false);
		this.MyCoins.SetActive(false);
		this.Coins = 0;
		this.CraftedBombTimed.SetActive(true);
	}
}
