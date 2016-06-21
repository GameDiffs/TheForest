using System;
using TheForest.Items.Special;
using TheForest.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace TheForest.Items.World
{
	[DoNotSerializePublic]
	public class BurnableItem : MonoBehaviour
	{
		private enum States
		{
			Idle,
			Lighting,
			Burning,
			Burnt,
			Dissolving,
			Dissolved
		}

		public float _lightingDuration = 1.5f;

		public float _burnDuration = 60f;

		public float _fuelRatioAttacking = 20f;

		public float _dissolveDuration = 1.5f;

		public float _fireParticleSize = 1.2f;

		public Material _burningMat;

		public Material _disolveMat;

		public GameObject _weaponFirePrefab;

		public GameObject _weaponFireSpawn;

		[ItemIdPicker]
		public int _itemId;

		private BurnableItem.States _state;

		private bool _attacking;

		private float _startTime;

		private float _fuel;

		private Material _normalMat;

		private GameObject _weaponFire;

		private ParticleScaler _fireParticleScale;

		private Light _firelight;

		private FMOD_StudioEventEmitter _fireAudioEmitter;

		private void Awake()
		{
			this._normalMat = base.GetComponent<Renderer>().sharedMaterial;
		}

		private void OnEnable()
		{
			if (this._state == BurnableItem.States.Idle && LocalPlayer.Inventory)
			{
				this._attacking = false;
				if (LocalPlayer.Inventory.HasInSlot(Item.EquipmentSlot.LeftHand, LocalPlayer.Inventory.DefaultLight._itemId))
				{
					LighterControler.HasLightableItem = true;
				}
				LocalPlayer.Inventory.Attacked.AddListener(new UnityAction(this.OnAttacking));
				LocalPlayer.Inventory.AttackEnded.AddListener(new UnityAction(this.OnAttackEnded));
			}
		}

		private void OnDisable()
		{
			if (Scene.HudGui)
			{
				this._attacking = false;
				Scene.HudGui.UnsetDelayedIconController(this);
				LocalPlayer.Inventory.Attacked.RemoveListener(new UnityAction(this.OnAttacking));
				LocalPlayer.Inventory.AttackEnded.RemoveListener(new UnityAction(this.OnAttackEnded));
				LighterControler.HasLightableItem = false;
				if (this._state > BurnableItem.States.Idle)
				{
					this.Burnt();
					this.Dissolved(true);
				}
			}
		}

		private void Update()
		{
			switch (this._state)
			{
			case BurnableItem.States.Idle:
				this.Idle();
				break;
			case BurnableItem.States.Lighting:
				this.Light();
				break;
			case BurnableItem.States.Burning:
				this.Burning();
				break;
			case BurnableItem.States.Burnt:
				this.Burnt();
				break;
			case BurnableItem.States.Dissolving:
				this.Dissolving();
				break;
			case BurnableItem.States.Dissolved:
				this.Dissolved(false);
				break;
			}
		}

		private void OnProjectileThrown(GameObject projectile)
		{
			if (this._state == BurnableItem.States.Burning)
			{
				if (this._weaponFire)
				{
					this._weaponFire.transform.parent = projectile.transform;
					this._weaponFire = null;
				}
				destroyAfter destroyAfter = projectile.AddComponent<destroyAfter>();
				destroyAfter.destroyTime = this._fuel;
				Renderer component = projectile.GetComponent<Renderer>();
				if (component)
				{
					component.sharedMaterial = base.GetComponent<Renderer>().sharedMaterial;
				}
				MasterFireSpread component2 = projectile.GetComponent<MasterFireSpread>();
				if (component2)
				{
					component2.Fuel = this._fuel * 2f;
				}
				PickUp componentInChildren = projectile.GetComponentInChildren<PickUp>();
				if (componentInChildren)
				{
					UnityEngine.Object.Destroy(componentInChildren.gameObject);
				}
				this.Burnt();
				this.Dissolved(true);
			}
			else if (this._state > BurnableItem.States.Burning)
			{
				destroyAfter destroyAfter2 = projectile.AddComponent<destroyAfter>();
				destroyAfter2.destroyTime = this._fuel;
				Renderer component3 = projectile.GetComponent<Renderer>();
				if (component3)
				{
					component3.sharedMaterial = base.GetComponent<Renderer>().sharedMaterial;
				}
			}
		}

		private void OnAttacking()
		{
			this._attacking = true;
		}

		private void OnAttackEnded()
		{
			this._attacking = false;
		}

		private void Idle()
		{
			if (LocalPlayer.Inventory.DefaultLight.IsReallyActive)
			{
				if (TheForest.Utils.Input.GetButton("Lighter"))
				{
					Scene.HudGui.SetDelayedIconController(this);
				}
				else
				{
					Scene.HudGui.UnsetDelayedIconController(this);
				}
				if (TheForest.Utils.Input.GetButtonAfterDelay("Lighter", 0.5f))
				{
					Scene.HudGui.UnsetDelayedIconController(this);
					LocalPlayer.Inventory.SpecialItems.SendMessage("LightHeldFire");
					this._fuel = Time.time + this._lightingDuration;
					this._state = BurnableItem.States.Lighting;
					LighterControler.HasLightableItem = false;
				}
			}
		}

		private void Light()
		{
			if (this._fuel < Time.time)
			{
				GameStats.LitWeapon.Invoke();
				LocalPlayer.Inventory.DefaultLight.StashLighter();
				Transform transform = (!this._weaponFireSpawn) ? base.transform : this._weaponFireSpawn.transform;
				this._weaponFire = (GameObject)UnityEngine.Object.Instantiate(this._weaponFirePrefab, transform.position, transform.rotation);
				this._weaponFire.transform.parent = transform;
				if (!this._weaponFire.activeSelf)
				{
					this._weaponFire.gameObject.SetActive(true);
				}
				this._fireParticleScale = this._weaponFire.GetComponentInChildren<ParticleScaler>();
				this._firelight = this._weaponFire.GetComponentInChildren<Light>();
				this._fireAudioEmitter = this._weaponFire.GetComponent<FMOD_StudioEventEmitter>();
				base.GetComponent<Renderer>().sharedMaterial = this._burningMat;
				this._fuel = this._burnDuration;
				this._startTime = Time.time;
				this._state = BurnableItem.States.Burning;
				this._attacking = false;
				LocalPlayer.Inventory.IsWeaponBurning = true;
				FMODCommon.PlayOneshot("event:/fire/fire_built_start", transform);
			}
		}

		private void Burning()
		{
			if (this._fuel >= 0f)
			{
				if (Scene.WeatherSystem.Raining)
				{
					this._fuel -= Time.deltaTime * ((!this._attacking) ? 5f : (this._fuelRatioAttacking + 5f));
				}
				else
				{
					this._fuel -= Time.deltaTime * ((!this._attacking) ? 1f : this._fuelRatioAttacking);
				}
				float num = this.ExpoEaseIn(1f - this._fuel / this._burnDuration, 1f, 0f, 1f) * this._fireParticleSize * ((!this._attacking) ? 1f : 0.75f);
				if (this._fireParticleScale)
				{
					this._fireParticleScale.particleScale = num;
				}
				if (this._firelight)
				{
					this._firelight.intensity = num * 6f / 7f;
				}
				if (this._fireAudioEmitter)
				{
					this._fireAudioEmitter.SetVolume(num);
				}
			}
			else
			{
				this._state = BurnableItem.States.Burnt;
			}
		}

		private void Burnt()
		{
			if (this._weaponFire)
			{
				UnityEngine.Object.Destroy(this._weaponFire);
				this._weaponFire = null;
			}
			if (this._disolveMat)
			{
				base.GetComponent<Renderer>().sharedMaterial = this._disolveMat;
				this._disolveMat.SetFloat("_BurnAmount", 1f);
				this._fireParticleScale = null;
				this._firelight = null;
				this._fuel = this._dissolveDuration;
				this._state = BurnableItem.States.Dissolving;
			}
			else
			{
				this.Dissolved(false);
			}
		}

		private void Dissolving()
		{
			this._fuel -= Time.deltaTime;
			if (this._fuel > 0f)
			{
				this._disolveMat.SetFloat("_BurnAmount", this._fuel / this._dissolveDuration);
			}
			else
			{
				this._state = BurnableItem.States.Dissolved;
			}
		}

		private void Dissolved(bool thrown)
		{
			this._state = BurnableItem.States.Idle;
			if (!thrown && LocalPlayer.Inventory.HasInSlot(Item.EquipmentSlot.RightHand, this._itemId))
			{
				LocalPlayer.Inventory.UnequipItemAtSlot(Item.EquipmentSlot.RightHand, false, false, true);
			}
			base.GetComponent<Renderer>().sharedMaterial = this._normalMat;
			if (this._disolveMat)
			{
				this._disolveMat.SetFloat("_BurnAmount", 1f);
			}
			LocalPlayer.Inventory.IsWeaponBurning = false;
		}

		public float ExpoEaseIn(float t, float b, float c, float d)
		{
			return (t != 0f) ? (c * Mathf.Pow(2f, 10f * (t / d - 1f)) + b) : b;
		}
	}
}
