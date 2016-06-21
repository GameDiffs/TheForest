using Bolt;
using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Buildings.Interfaces;
using TheForest.Buildings.World;
using TheForest.Items;
using TheForest.Items.Craft;
using TheForest.Utils;
using UnityEngine;

[DoNotSerializePublic]
public class Fire2 : EntityBehaviour<IFireState>, IWetable
{
	[Serializable]
	public class FoodItem
	{
		[ItemIdPicker]
		public int _itemId;

		public Cook _cookPrefab;
	}

	public Fire2.FoodItem[] foodItems;

	public Fire2.FoodItem waterCleaningItem;

	[ItemIdPicker]
	public int LeafItemId;

	public bool CookingDisabled;

	public bool CurrentLit;

	public float CurrentFuel;

	public float CurrentLeafDelay;

	public float FuelStart = 120f;

	public float FuelMax = 120f;

	public float DrainRate;

	public float DrainRateRaining;

	public float LeafDelayTime = 2f;

	public float LeafAddedExtraScale = 1f;

	public float LeafAddedExtraIntensity = 1f;

	public Material MaterialBurnt;

	public Material MaterialAlight;

	public GameObject ModelLit;

	public GameObject ModelUnlit;

	public GameObject IconCookHeld;

	public GameObject IconAddLeaves;

	public GameObject IconLightFire;

	public GameObject FireLight;

	public GameObject FireDamage;

	public GameObject FlamesAll;

	public GameObject FlamesScale;

	public GameObject FearTrigger;

	[Header("FMOD")]
	public string addFuelEvent;

	private bool hasPreloaded;

	[SerializeThis]
	private bool _lit;

	private bool _proximity;

	[SerializeThis]
	private float _fuel;

	private float _leafDelay;

	private float _leafAdded;

	[SerializeThis]
	private int _putOutChance = 2;

	private Light _light;

	private ParticleScaler _scaler;

	[SerializeField]
	private DestroyOnContactWithTag _tag;

	private float _baseScale;

	private float _baseIntensity;

	public bool Lit
	{
		get
		{
			if (BoltNetwork.isRunning && this.entity && this.entity.isAttached)
			{
				return base.state.Lit;
			}
			return this._lit;
		}
		set
		{
			if (BoltNetwork.isRunning)
			{
				base.state.Lit = value;
			}
			else
			{
				this._lit = value;
			}
		}
	}

	private float Fuel
	{
		get
		{
			if (BoltNetwork.isRunning)
			{
				return base.state.Fuel;
			}
			return this._fuel;
		}
		set
		{
			value = Mathf.Clamp(value, 0.0001f, this.FuelMax);
			if (BoltNetwork.isRunning)
			{
				base.state.Fuel = value;
			}
			else
			{
				this._fuel = value;
			}
		}
	}

	[DebuggerHidden]
	private IEnumerator OnDeserialized()
	{
		Fire2.<OnDeserialized>c__Iterator146 <OnDeserialized>c__Iterator = new Fire2.<OnDeserialized>c__Iterator146();
		<OnDeserialized>c__Iterator.<>f__this = this;
		return <OnDeserialized>c__Iterator;
	}

	public override void Detached()
	{
		if (this.entity.detachToken is CoopDestroyTagToken)
		{
			this._tag.Perform(true);
		}
	}

	public override void Attached()
	{
		IFireState expr_06 = base.state;
		expr_06.OnFuelAdded = (Action)Delegate.Combine(expr_06.OnFuelAdded, new Action(this.AddFuel_Complete));
		base.state.AddCallback("Lit", delegate
		{
			if (BoltNetwork.isClient)
			{
				if (base.state.Lit)
				{
					this.On();
				}
				else
				{
					this.Off();
				}
			}
		});
		if (this.entity.isOwner)
		{
			base.state.Fuel = this.FuelStart;
		}
	}

	private void UpdateCooking()
	{
		if (this.CookingDisabled)
		{
			return;
		}
		if (LocalPlayer.Inventory.EquipmentSlots[0] != null)
		{
			int num = this.foodItems.FindIndex((Fire2.FoodItem fi) => fi._itemId == LocalPlayer.Inventory.EquipmentSlots[0]._itemId);
			if (this.IconCookHeld.activeSelf != num >= 0)
			{
				this.IconCookHeld.SetActive(num >= 0);
			}
			if (num >= 0)
			{
				if (TheForest.Utils.Input.GetButtonDown("Craft"))
				{
					LocalPlayer.Inventory.UnequipItemAtSlot(Item.EquipmentSlot.RightHand, false, false, true);
					this.SpawnFoodPrefab(this.foodItems[num]._cookPrefab, false);
					this.IconCookHeld.SetActive(false);
				}
			}
			else if (LocalPlayer.Inventory.EquipmentSlots[0]._itemId == this.waterCleaningItem._itemId && (LocalPlayer.Inventory.EquipmentSlots[0].ActiveBonus == WeaponStatUpgrade.Types.DirtyWater || LocalPlayer.Inventory.EquipmentSlots[0].ActiveBonus == WeaponStatUpgrade.Types.CleanWater))
			{
				this.IconCookHeld.SetActive(true);
				if (TheForest.Utils.Input.GetButtonDown("Craft"))
				{
					LocalPlayer.Inventory.EquipmentSlots[0].ActiveBonus = (WeaponStatUpgrade.Types)(-1);
					LocalPlayer.Inventory.UnequipItemAtSlot(Item.EquipmentSlot.RightHand, false, false, true);
					this.SpawnFoodPrefab(this.waterCleaningItem._cookPrefab, true);
					this.IconCookHeld.SetActive(false);
				}
			}
		}
	}

	private void SpawnFoodPrefab(Cook foodPrefab, bool center)
	{
		if (this.CookingDisabled)
		{
			return;
		}
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
		cook.transform.parent = base.transform.parent;
		cook.transform.Rotate(0f, (float)UnityEngine.Random.Range(0, 359), 0f);
	}

	private void Awake()
	{
		if (this.FireLight)
		{
			this._light = this.FireLight.GetComponent<Light>();
			this._baseIntensity = this._light.intensity;
		}
		if (this.FlamesScale)
		{
			this._scaler = this.FlamesScale.GetComponent<ParticleScaler>();
			this._baseScale = this._scaler.particleScale;
		}
		this._leafAdded = 0f;
		if (this.IconLightFire)
		{
			this.IconLightFire.SetActive(false);
		}
		base.enabled = false;
		if (!BoltNetwork.isRunning)
		{
			this.Fuel = this.FuelStart;
		}
		if (this.FuelStart == 0f)
		{
			if (this.ModelLit)
			{
				this.ModelLit.SetActive(true);
			}
			if (this.ModelUnlit)
			{
				this.ModelUnlit.SetActive(false);
			}
			if (this.ModelLit && this.MaterialBurnt)
			{
				this.ModelLit.GetComponent<Renderer>().sharedMaterial = this.MaterialBurnt;
			}
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		this.CurrentLit = this.Lit;
		this.CurrentFuel = this.Fuel;
		this.CurrentLeafDelay = this._leafDelay;
		if (this.Lit)
		{
			this.UpdateLit();
		}
		else
		{
			this.UpdateNotLit();
		}
	}

	private void UpdateLit()
	{
		if (!BoltNetwork.isClient)
		{
			if (Scene.WeatherSystem.Raining)
			{
				this.Fuel -= this.DrainRateRaining * Time.deltaTime;
			}
			else
			{
				this.Fuel -= this.DrainRate * Time.deltaTime;
			}
		}
		float num = Mathf.Clamp01(this.Fuel / (this.FuelMax * 0.5f));
		if (this._leafAdded > Time.time)
		{
			Vector2 from = new Vector2(0f, this.LeafAddedExtraScale);
			Vector2 to = new Vector2(this.LeafAddedExtraScale, 0f);
			Vector2 vector = Vector2.Lerp(from, to, Mathf.Clamp01((this._leafAdded - Time.time) / 2f));
			Vector2 from2 = new Vector2(0f, this.LeafAddedExtraIntensity);
			Vector2 to2 = new Vector2(this.LeafAddedExtraIntensity, 0f);
			Vector2 vector2 = Vector2.Lerp(from2, to2, Mathf.Clamp01((this._leafAdded - Time.time) / 2f));
			if (this._leafAdded - Time.time < 1f)
			{
				this._light.intensity = this._baseIntensity * num + vector2.x;
				this._scaler.particleScale = Mathf.Max(Mathf.Min(num, this._baseScale), 0.01f) + vector.x;
			}
			else
			{
				this._light.intensity = this._baseIntensity * num + vector2.y;
				this._scaler.particleScale = Mathf.Max(Mathf.Min(num, this._baseScale), 0.01f) + vector.y;
			}
		}
		else
		{
			this._light.intensity = this._baseIntensity * num;
			this._scaler.particleScale = Mathf.Max(Mathf.Min(num, this._baseScale), 0.01f);
		}
		if (!BoltNetwork.isClient && this.Fuel <= 0.01f)
		{
			if (this._putOutChance-- <= 0)
			{
				this.DestroyFire();
			}
			else
			{
				this.Off();
			}
		}
		if (this._proximity)
		{
			this.UpdateCooking();
		}
		bool flag = this._leafDelay + this.LeafDelayTime < Time.time;
		bool flag2 = LocalPlayer.Inventory.Owns(this.LeafItemId);
		this.IconAddLeaves.SetActive(flag && flag2 && this._proximity);
		if (this.IconAddLeaves.activeInHierarchy && TheForest.Utils.Input.GetButtonDown("Take") && LocalPlayer.Inventory.RemoveItem(this.LeafItemId, 1, false))
		{
			LocalPlayer.Sfx.PlayWhoosh();
			this._leafDelay = Time.time;
			if (BoltNetwork.isRunning)
			{
				FireAddFuelEvent fireAddFuelEvent = FireAddFuelEvent.Raise(GlobalTargets.Everyone);
				fireAddFuelEvent.Target = this.entity;
				fireAddFuelEvent.Send();
			}
			else
			{
				this.Action_AddFuel();
			}
		}
	}

	private void DestroyFire()
	{
		if (this.ModelLit && this.MaterialBurnt)
		{
			this.ModelLit.GetComponent<Renderer>().sharedMaterial = this.MaterialBurnt;
		}
		if (BoltNetwork.isRunning && this.entity)
		{
			DestroyWithTag destroyWithTag = DestroyWithTag.Create(GlobalTargets.OnlyServer);
			destroyWithTag.Entity = this.entity;
			destroyWithTag.Send();
		}
		else
		{
			this.FlamesAll.BroadcastMessage("PerformDestroy", false, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void UpdateNotLit()
	{
		if (this._proximity && TheForest.Utils.Input.GetButtonAfterDelay("Take", 0.5f) && this.IconLightFire.activeSelf)
		{
			LocalPlayer.Inventory.SpecialItems.SendMessage("LightTheFire");
			this.IconLightFire.SetActive(false);
			base.Invoke("BringBackLightFireIcon", 3f);
		}
	}

	private void BringBackLightFireIcon()
	{
		if (this._proximity && !this.Lit)
		{
			this.IconLightFire.SetActive(true);
		}
	}

	private void receiveLightFire()
	{
		if (base.enabled)
		{
			this.SetAlight();
		}
	}

	private void Burn()
	{
		if (!this.Lit)
		{
			this.SetAlight();
		}
	}

	private void SetAlight()
	{
		if (BoltNetwork.isRunning)
		{
			FireLightEvent fireLightEvent = FireLightEvent.Raise(GlobalTargets.OnlyServer);
			fireLightEvent.Target = this.entity;
			fireLightEvent.Send();
		}
		else
		{
			this.Action_LightFire();
		}
	}

	public void Action_LightFire()
	{
		if (BoltNetwork.isClient)
		{
			UnityEngine.Debug.LogError("Action_LightFire: Should never be called on the client!");
			return;
		}
		if (!this.Lit)
		{
			if (this.Fuel < 5f)
			{
				this.Fuel = 10f;
				this._scaler.Awake();
				this._scaler.ResetParticleScale(Mathf.Clamp01(this.Fuel / (this.FuelMax * 0.5f)));
			}
			FMODCommon.PlayOneshotNetworked(this.addFuelEvent, base.transform, FMODCommon.NetworkRole.Any);
			this.On();
		}
	}

	public void Action_AddFuel()
	{
		this.Burn();
		this.Fuel += (float)UnityEngine.Random.Range(10, 30);
		this.AddFuel_Complete();
	}

	public void AddFuel_Complete()
	{
		this._leafAdded = Time.time + 2f;
		FMODCommon.PlayOneshot(this.addFuelEvent, base.transform);
	}

	private void Action_EnableFireDamage()
	{
		if (this.Lit && this.FireDamage)
		{
			this.FireDamage.SetActive(true);
		}
	}

	private void GrabEnter()
	{
		if (!BoltNetwork.isRunning || this.entity.isAttached)
		{
			base.enabled = true;
			this._proximity = true;
			if (!this.Lit && this.IconLightFire)
			{
				this.IconLightFire.SetActive(true);
			}
		}
	}

	private void GrabExit()
	{
		if (!this.Lit)
		{
			base.CancelInvoke();
		}
		base.enabled = this.Lit;
		this._proximity = false;
		if (this.IconCookHeld)
		{
			this.IconCookHeld.SetActive(false);
		}
		if (this.IconAddLeaves)
		{
			this.IconAddLeaves.SetActive(false);
		}
		if (this.IconLightFire)
		{
			this.IconLightFire.SetActive(false);
		}
	}

	private void On()
	{
		base.CancelInvoke();
		if (this._light)
		{
			this._light.intensity = 0f;
		}
		bool flag = true;
		base.enabled = flag;
		this.Lit = flag;
		this.SetFlames(true);
		if (this.IconLightFire)
		{
			this.IconLightFire.SetActive(false);
		}
		if (this.ModelLit)
		{
			this.ModelLit.SetActive(true);
			if (this.MaterialAlight)
			{
				this.ModelLit.GetComponent<Renderer>().sharedMaterial = this.MaterialAlight;
			}
		}
		if (this.ModelUnlit)
		{
			this.ModelUnlit.SetActive(false);
		}
		base.Invoke("Action_EnableFireDamage", 2f);
		base.GetComponent<Collider>().enabled = false;
		base.GetComponent<Collider>().enabled = true;
	}

	private void Off()
	{
		bool flag = false;
		base.enabled = flag;
		this.Lit = (this.CurrentLit = flag);
		this.SetFlames(false);
		if (this.FireDamage)
		{
			this.FireDamage.SetActive(false);
		}
		if (this.ModelLit && this.MaterialBurnt)
		{
			this.ModelLit.GetComponent<Renderer>().sharedMaterial = this.MaterialBurnt;
		}
		base.GetComponent<Collider>().enabled = false;
		base.GetComponent<Collider>().enabled = true;
	}

	private void SetFlames(bool active)
	{
		if (this.FlamesAll)
		{
			this.FlamesAll.SetActive(active);
		}
		if (this.FearTrigger)
		{
			this.FearTrigger.SetActive(active);
		}
	}

	private void OnEnable()
	{
		FMODCommon.PreloadEvents(new string[]
		{
			this.addFuelEvent
		});
		this.hasPreloaded = true;
	}

	private void OnDisable()
	{
		if (this.IconAddLeaves)
		{
			this.IconAddLeaves.SetActive(false);
		}
		if (this.IconCookHeld)
		{
			this.IconCookHeld.SetActive(false);
		}
		if (this.hasPreloaded)
		{
			FMODCommon.UnloadEvents(new string[]
			{
				this.addFuelEvent
			});
			this.hasPreloaded = false;
		}
	}

	public void GotClean()
	{
		UnityEngine.Object.Destroy(this.IconLightFire);
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
