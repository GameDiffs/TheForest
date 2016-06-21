using Bolt;
using System;
using TheForest.Items;
using TheForest.Items.Craft;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UnityEngine;

public class Fire : EntityBehaviour
{
	[Serializable]
	public class FoodItem
	{
		[ItemIdPicker]
		public int _itemId;

		public Cook _cookPrefab;
	}

	public Fire.FoodItem[] foodItems;

	public Fire.FoodItem waterCleaningItem;

	public bool FireSmall;

	public bool FireRockPit;

	public bool FireStanding;

	public Material Burnt;

	public Material Alight;

	public GameObject FlamesScale;

	public GameObject LitModel;

	public GameObject UnLitModel;

	public GameObject FireLight;

	public GameObject FlamesAll;

	public GameObject AddLeavesIcon;

	public GameObject CookHeldIcon;

	public GameObject FireDamage;

	public GameObject fearTrigger;

	[ItemIdPicker]
	public int _leafItemId;

	private PlayerInventory Player;

	private bool LeafDelay;

	private bool Lit;

	private float Fuel;

	private float FlameSize;

	private ParticleScaler ParticleScaler;

	private void Start()
	{
		this.ParticleScaler = this.FlamesScale.GetComponent<ParticleScaler>();
		this.Fuel = 120f;
		if (!this.Lit)
		{
			base.enabled = false;
		}
	}

	private void GrabEnter()
	{
		base.enabled = true;
		this.Player = LocalPlayer.Inventory;
	}

	private void GrabExit()
	{
		this.Player = null;
		this.AddLeavesIcon.SetActive(false);
		this.CookHeldIcon.SetActive(false);
		if (!this.Lit)
		{
			base.CancelInvoke();
			base.enabled = false;
		}
	}

	private void Update()
	{
		if (this.Lit)
		{
			if (BoltNetwork.isClient)
			{
				this.LeafDelay = false;
			}
			this.ParticleScaler.particleScale = this.Fuel / 150f;
			this.FireLight.GetComponent<Light>().intensity = this.Fuel / 320f;
			if (this.Player != null)
			{
				if (this.Player.EquipmentSlots[0] != null)
				{
					int num = this.foodItems.FindIndex((Fire.FoodItem fi) => fi._itemId == this.Player.EquipmentSlots[0]._itemId);
					if (this.CookHeldIcon.activeSelf != num >= 0)
					{
						this.CookHeldIcon.SetActive(num >= 0);
					}
					if (num >= 0)
					{
						if (TheForest.Utils.Input.GetButtonDown("Craft"))
						{
							this.Player.UnequipItemAtSlot(Item.EquipmentSlot.RightHand, false, false, true);
							this.SpawnFoodPrefab(this.foodItems[num]._cookPrefab, false);
							this.CookHeldIcon.SetActive(false);
						}
					}
					else if (this.Player.EquipmentSlots[0]._itemId == this.waterCleaningItem._itemId && (this.Player.EquipmentSlots[0].ActiveBonus == WeaponStatUpgrade.Types.DirtyWater || this.Player.EquipmentSlots[0].ActiveBonus == WeaponStatUpgrade.Types.CleanWater))
					{
						this.CookHeldIcon.SetActive(true);
						if (TheForest.Utils.Input.GetButtonDown("Craft"))
						{
							this.Player.EquipmentSlots[0].ActiveBonus = (WeaponStatUpgrade.Types)(-1);
							this.Player.UnequipItemAtSlot(Item.EquipmentSlot.RightHand, false, false, true);
							this.SpawnFoodPrefab(this.waterCleaningItem._cookPrefab, true);
							this.CookHeldIcon.SetActive(false);
						}
					}
				}
				if (!this.LeafDelay && this.Player.Owns(this._leafItemId))
				{
					this.AddLeavesIcon.SetActive(true);
					if (TheForest.Utils.Input.GetButtonDown("Take") && this.Player.RemoveItem(this._leafItemId, 1, false))
					{
						if (this.Player)
						{
							LocalPlayer.Sfx.PlayWhoosh();
						}
						this.LeafDelay = true;
						this.StartAddToFuel();
					}
				}
				else
				{
					this.AddLeavesIcon.SetActive(false);
				}
			}
		}
		else if (this.Player == null)
		{
			base.enabled = false;
		}
		else if (TheForest.Utils.Input.GetButtonDown("Take"))
		{
			this.Player.SpecialItems.SendMessage("LightTheFire");
			if (this.Fuel < 5f)
			{
				this.Fuel = 10f;
			}
			this.StartLightFire();
		}
	}

	public override void Attached()
	{
		IFireState state = this.entity.GetState<IFireState>();
		state.AddCallback("Lit", delegate
		{
			if (state.Lit)
			{
				this.LightFire();
			}
			else
			{
				this.FireOut();
			}
		});
	}

	public void StartLightFire()
	{
		if (BoltNetwork.isRunning)
		{
			FireLightEvent fireLightEvent = FireLightEvent.Raise(GlobalTargets.OnlyServer);
			fireLightEvent.Target = this.entity;
			fireLightEvent.Send();
		}
		else
		{
			base.Invoke("LightFire", 2f);
		}
	}

	public void StartAddToFuel()
	{
		base.GetComponent<AudioSource>().Play();
		if (BoltNetwork.isRunning)
		{
			FireAddFuelEvent fireAddFuelEvent = FireAddFuelEvent.Raise(GlobalTargets.OnlyServer);
			fireAddFuelEvent.Target = this.entity;
			fireAddFuelEvent.Send();
		}
		else
		{
			this.Fuel += 150f;
			base.Invoke("AddToFuel", 1f);
		}
	}

	private void SpawnFoodPrefab(Cook foodPrefab, bool center)
	{
		Vector3 position = base.transform.position + new Vector3((!center) ? UnityEngine.Random.Range(-0.2f, 0.2f) : 0f, 1.25f, (!center) ? UnityEngine.Random.Range(-0.2f, 0.2f) : 0f);
		Cook cook;
		if (!BoltNetwork.isRunning)
		{
			cook = (Cook)UnityEngine.Object.Instantiate(foodPrefab, position, Quaternion.identity);
		}
		else
		{
			BoltEntity boltEntity = BoltNetwork.Instantiate(foodPrefab.gameObject, position, Quaternion.identity);
			cook = boltEntity.GetComponent<Cook>();
		}
		cook.transform.Rotate(0f, (float)UnityEngine.Random.Range(0, 359), 0f);
	}

	private void LightFire()
	{
		base.enabled = true;
		this.Lit = true;
		base.Invoke("TurnDamageOn", 2f);
		this.LitModel.GetComponent<Renderer>().material = this.Alight;
		this.fearTrigger.SetActive(true);
		this.UnLitModel.SetActive(false);
		this.LitModel.SetActive(true);
		this.FlamesAll.SetActive(true);
		base.InvokeRepeating("Drain", 4f, 4f);
	}

	private void TurnDamageOn()
	{
		this.FireDamage.SetActive(true);
	}

	private void AddToFuel()
	{
		this.LeafDelay = false;
		this.Fuel += (float)UnityEngine.Random.Range(10, 30);
		if (this.Fuel > 120f)
		{
			this.Fuel = 120f;
		}
	}

	public void LightFireMP()
	{
		this.entity.GetState<IFireState>().Lit = true;
	}

	public void AddToFuelMP()
	{
		base.Invoke("AddToFuel", 1f);
	}

	private void FireOut()
	{
		this.fearTrigger.SetActive(false);
		this.LitModel.GetComponent<Renderer>().material = this.Burnt;
		this.FlamesAll.SetActive(false);
		this.Lit = false;
		base.CancelInvoke("Drain");
	}

	private void Drain()
	{
		if (BoltNetwork.isClient)
		{
			return;
		}
		if (this.FireSmall && this.Fuel >= 0f)
		{
			this.Fuel -= 0.8f;
			if (Scene.WeatherSystem.Raining)
			{
				this.Fuel -= 4f;
			}
		}
		if (this.FireRockPit && this.Fuel >= 0f)
		{
			this.Fuel -= 0.3f;
			if (Scene.WeatherSystem.Raining)
			{
				this.Fuel -= 2f;
			}
		}
		if (this.Fuel <= 0f)
		{
			if (BoltNetwork.isRunning)
			{
				this.entity.GetState<IFireState>().Lit = false;
			}
			else
			{
				this.FireOut();
			}
		}
	}
}
