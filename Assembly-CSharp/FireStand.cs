using Bolt;
using System;
using TheForest.Buildings.Creation;
using TheForest.Items;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UnityEngine;

[DoNotSerializePublic]
public class FireStand : EntityBehaviour<IFireState>, IEventListener
{
	public GameObject fearTrigger;

	public GameObject BillboardLight;

	public GameObject FlamesScale;

	public Light FireLight;

	public GameObject FlamesAll;

	public GameObject AddLeavesIcon;

	public GameObject FireDamage;

	[ItemIdPicker]
	public int _leafItemId;

	private ParticleScaler ParticleScaler;

	private int Dice;

	private bool LeafDelay;

	[SerializeThis]
	private bool Lit;

	[SerializeThis]
	private float Fuel;

	private PlayerInventory Player;

	bool IEventListener.InvokeIfDisabled
	{
		get
		{
			return true;
		}
	}

	bool IEventListener.InvokeIfGameObjectIsInactive
	{
		get
		{
			return true;
		}
	}

	private void Awake()
	{
		if (!LevelSerializer.IsDeserializing)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(LocalPlayer.Create._blueprints.Find((Create.BuildingBlueprint bp) => bp._type == Create.BuildingTypes.FireStand)._builtPrefab);
			gameObject.transform.position = base.transform.parent.position;
			gameObject.transform.rotation = base.transform.parent.rotation;
			gameObject.transform.parent = base.transform.parent.parent;
			if (BoltNetwork.isRunning)
			{
				BoltNetwork.Attach(gameObject);
			}
			base.transform.parent.parent = null;
			UnityEngine.Object.Destroy(base.transform.parent.gameObject);
		}
	}

	private void OnDeserialized()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(LocalPlayer.Create._blueprints.Find((Create.BuildingBlueprint bp) => bp._type == Create.BuildingTypes.FireStand)._builtPrefab);
		gameObject.transform.position = base.transform.parent.position;
		gameObject.transform.rotation = base.transform.parent.rotation;
		gameObject.transform.parent = base.transform.parent.parent;
		if (BoltNetwork.isRunning)
		{
			BoltNetwork.Attach(gameObject);
		}
		base.transform.parent.parent = null;
		UnityEngine.Object.Destroy(base.transform.parent.gameObject);
	}

	public override void Attached()
	{
		IFireState state = this.entity.GetState<IFireState>();
		state.AddCallback("Lit", delegate
		{
			if (state.Lit)
			{
				this.Invoke("LightFire", 0.75f);
			}
			else
			{
				this.Off();
			}
		});
	}

	private void Start()
	{
	}

	private void GrabEnter()
	{
		base.enabled = true;
		this.Player = LocalPlayer.Inventory;
	}

	private void GrabExit()
	{
		if (!this.Lit)
		{
			base.enabled = false;
		}
		this.Player = null;
		this.AddLeavesIcon.SetActive(false);
	}

	private void Update()
	{
		if (BoltNetwork.isClient)
		{
			this.LeafDelay = false;
		}
		if (!this.Lit)
		{
			this.BillboardLight.SetActive(true);
			if (this.Player == null)
			{
				base.enabled = false;
				return;
			}
		}
		else
		{
			this.BillboardLight.SetActive(false);
			this.ParticleScaler.particleScale = this.Fuel / 400f;
			if (this.FireLight)
			{
				this.FireLight.intensity = this.Fuel / 300f;
			}
		}
		if (this.Player != null)
		{
			if (!this.Lit && TheForest.Utils.Input.GetButtonDown("Take"))
			{
				this.Player.SpecialItems.SendMessage("LightTheFire");
				if (this.Fuel < 5f)
				{
					this.Fuel = 10f;
				}
				this.StartLightFire();
			}
			if (this.Lit && !this.LeafDelay && this.Player.Owns(this._leafItemId))
			{
				this.AddLeavesIcon.SetActive(true);
				if (TheForest.Utils.Input.GetButtonDown("Take") && this.Player.RemoveItem(this._leafItemId, 1, false))
				{
					LocalPlayer.Sfx.PlayWhoosh();
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

	public void AddToFuelMP()
	{
		base.Invoke("AddToFuel", 0.75f);
	}

	public void LightFireMP()
	{
		base.state.Lit = true;
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
		this.Fuel += 150f;
		if (BoltNetwork.isRunning)
		{
			FireAddFuelEvent fireAddFuelEvent = FireAddFuelEvent.Raise(GlobalTargets.OnlyServer);
			fireAddFuelEvent.Target = this.entity;
			fireAddFuelEvent.Send();
		}
		else
		{
			base.Invoke("AddToFuel", 1f);
		}
	}

	private void LightFire()
	{
		base.enabled = true;
		this.Lit = true;
		this.fearTrigger.SetActive(true);
		this.FlamesAll.SetActive(true);
		base.Invoke("TurnDamageOn", 2f);
		if (!BoltNetwork.isClient)
		{
			base.InvokeRepeating("Drain", 4f, 4f);
		}
	}

	private void TurnDamageOn()
	{
		this.FireDamage.SetActive(true);
	}

	private void AddToFuel()
	{
		this.LeafDelay = false;
		this.Fuel -= 150f;
		this.Fuel += (float)UnityEngine.Random.Range(10, 30);
		if (this.Fuel > 120f)
		{
			this.Fuel = 120f;
		}
	}

	private void Drain()
	{
		if (this.Fuel >= 0f)
		{
			this.Fuel -= 0.1f;
			if (Scene.WeatherSystem.Raining)
			{
				this.Fuel -= 1f;
			}
		}
		if (this.Fuel <= 0f)
		{
			if (BoltNetwork.isRunning)
			{
				base.state.Lit = false;
			}
			this.Off();
		}
	}

	private void Off()
	{
		this.fearTrigger.SetActive(false);
		this.FlamesAll.SetActive(false);
		this.Lit = false;
		Debug.Log("LIT == FALSE");
		if (this.Player == null)
		{
			base.enabled = false;
		}
		base.CancelInvoke("Drain");
	}
}
