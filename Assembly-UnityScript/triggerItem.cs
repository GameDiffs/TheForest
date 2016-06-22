using System;
using UnityEngine;
using UnityScript.Lang;

[Serializable]
public class triggerItem : MonoBehaviour
{
	public GameObject MyObject;

	public GameObject Lid;

	public GameObject Base;

	public GameObject MyTrigger;

	public bool FirstAidKitInterior;

	public bool Food;

	public bool Lighter;

	public bool FlareGun;

	public bool BatteryPack;

	public bool Drink;

	public bool Health;

	public bool SuitCase;

	public bool TennisBall;

	public bool TennisBallDog;

	public bool ChocolateBar;

	public bool Choc0;

	public bool Choc1;

	public bool Choc2;

	public float Amount;

	public bool HairSpray;

	private bool Opened;

	public Transform InsidePos;

	public GameObject[] WorldItem;

	public GameObject Statue;

	public GameObject ShavingCream;

	public GameObject Sfx;

	private int Dice;

	private int Dice2;

	public GameObject Interior1;

	public GameObject Interior2;

	public Material SwitchMat;

	public bool IsLocked;

	private bool PlayerHere;

	private GameObject Player;

	public bool Usable;

	public bool IsSeen;

	public triggerItem()
	{
		this.Dice = 1;
		this.Dice2 = 1;
	}

	public override void Awake()
	{
		this.Player = GameObject.FindWithTag("Player");
		if (this.SuitCase)
		{
			this.RollDice();
			this.Interior1.SetActive(false);
			this.Interior2.SetActive(false);
			this.Statue.SetActive(false);
			this.ShavingCream.SetActive(false);
		}
	}

	public override void Update()
	{
		if (this.PlayerHere)
		{
			if (this.SuitCase && !this.Opened)
			{
				this.Player.SendMessage("ShowSuitcase");
			}
			if (this.TennisBall)
			{
				this.Player.SendMessage("ShowTennisBall");
			}
			if (this.Drink)
			{
				this.Player.SendMessage("ShowAddToBackPack");
			}
			if (this.FlareGun)
			{
				this.Player.SendMessage("ShowFlareGun");
			}
			if (this.ChocolateBar)
			{
				this.Player.SendMessage("ShowAddToBackPack");
			}
			if (Input.GetButtonDown("Take"))
			{
				if (this.SuitCase && !this.IsLocked && !this.Opened)
				{
					this.Opened = true;
					this.MyObject.GetComponent<Animation>().Play();
					this.Sfx.SetActive(true);
					this.Sfx.GetComponent<AudioSource>().Play();
					this.Lid.GetComponent<Renderer>().material.SetFloat("_SheenPower", this.Amount);
					this.Lid.GetComponent<Renderer>().material.SetFloat("_SheenSpeed", this.Amount);
					this.Base.GetComponent<Renderer>().material.SetFloat("_SheenSpeed", this.Amount);
					this.Base.GetComponent<Renderer>().material.SetFloat("_SheenPower", this.Amount);
					if (this.Dice == 1)
					{
						this.Interior1.SetActive(true);
					}
					if (this.Dice == 2)
					{
						this.Interior2.SetActive(true);
					}
					if (this.Dice == 3)
					{
						this.Interior1.SetActive(true);
						this.Interior2.SetActive(true);
					}
					if (this.Dice2 == 4)
					{
						this.Statue.SetActive(true);
					}
					if (this.Dice2 == 3)
					{
						this.ShavingCream.SetActive(true);
					}
					GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this.WorldItem[UnityEngine.Random.Range(0, Extensions.get_length(this.WorldItem))], this.InsidePos.position, this.InsidePos.rotation);
					gameObject.transform.parent = this.InsidePos.transform;
					gameObject.SendMessage("Delay");
					this.Player.SendMessage("CloseSuitcase");
					this.gameObject.SetActive(false);
				}
				if (this.Lighter)
				{
					this.Player.SendMessage("GotLighter");
					UnityEngine.Object.Destroy(this.MyObject);
				}
				if (this.FlareGun)
				{
					this.Player.SendMessage("GotFlareGun");
					this.Player.SendMessage("CloseFlareGun");
					UnityEngine.Object.Destroy(this.MyObject);
				}
				if (this.HairSpray)
				{
					this.Player.SendMessage("GotFlameThrower");
					UnityEngine.Object.Destroy(this.MyObject);
				}
				if (this.ChocolateBar)
				{
					this.Player.SendMessage("ShowAddToBackPack");
					if (this.Choc0)
					{
						this.Player.SendMessage("GetChocolateBar0");
						this.Player.SendMessage("CloseAddToBackPack");
					}
					if (this.Choc1)
					{
						this.Player.SendMessage("GetChocolateBar1");
						this.Player.SendMessage("CloseAddToBackPack");
					}
					if (this.Choc2)
					{
						this.Player.SendMessage("GetChocolateBar2");
						this.Player.SendMessage("CloseAddToBackPack");
					}
					this.Player.SendMessage("CloseItemPickUp");
					UnityEngine.Object.Destroy(this.MyObject);
				}
				if (this.Food)
				{
					this.Player.SendMessage("GetFood");
					this.Player.SendMessage("CloseItemPickUp");
					this.Player.SendMessage("NotInATrigger");
					UnityEngine.Object.Destroy(this.MyObject);
				}
				if (this.BatteryPack)
				{
					this.Player.SendMessage("GotBattery");
					this.Player.SendMessage("CloseItemPickUp");
					UnityEngine.Object.Destroy(this.MyObject);
				}
				if (this.TennisBall)
				{
					this.Player.SendMessage("GotTennisBall");
					this.Player.SendMessage("ShowDrop");
					this.Player.SendMessage("CloseTennisBall");
					this.Player.SendMessage("CloseItemPickUp");
					UnityEngine.Object.Destroy(this.MyObject);
				}
				if (this.TennisBallDog)
				{
					this.Player.SendMessage("GotTennisBall");
					this.Player.SendMessage("CloseItemPickUp");
					this.PlayerHere = false;
					this.MyObject.SetActive(false);
				}
				if (this.Drink)
				{
					this.Player.SendMessage("StashCan");
					this.Player.SendMessage("CloseAddToBackPack");
					UnityEngine.Object.Destroy(this.MyObject);
				}
				if (this.Health)
				{
					this.Sfx.SetActive(true);
					this.Sfx.GetComponent<AudioSource>().Play();
					this.MyObject.GetComponent<Animation>().Play();
					this.MyObject.GetComponent<Renderer>().material = this.SwitchMat;
					this.Base.GetComponent<Renderer>().material = this.SwitchMat;
					this.Player.SendMessage("CloseItemPickUp");
					this.Player.SendMessage("GetHealth");
					this.Player.SendMessage("CloseItemUse");
					this.gameObject.SetActive(false);
				}
			}
		}
	}

	public override void GrabEnter()
	{
		this.PlayerHere = true;
	}

	public override void GrabExit()
	{
		this.Player.SendMessage("CloseSuitcase");
		this.Player.SendMessage("CloseDrink");
		this.Player.SendMessage("CloseEat");
		this.Player.SendMessage("CloseFlareGun");
		this.Player.SendMessage("CloseTennisBall");
		this.Player.SendMessage("CloseAddToBackPack");
		this.Player.SendMessage("NotInATrigger");
		this.PlayerHere = false;
	}

	public override void SeeMe()
	{
	}

	public override void DontSeeMe()
	{
	}

	public override void RollDice()
	{
		this.Dice = UnityEngine.Random.Range(1, 4);
		this.Dice2 = UnityEngine.Random.Range(1, 6);
	}

	public override void Locked()
	{
		this.IsLocked = false;
	}

	public override void UnLock()
	{
		this.IsLocked = false;
	}

	public override void Main()
	{
	}
}
