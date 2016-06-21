using System;
using TheForest.Items.Craft;
using TheForest.Items.Inventory;
using TheForest.Items.Special;
using TheForest.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace TheForest.Items.World
{
	[DoNotSerializePublic]
	public class BurnableCloth : MonoBehaviour
	{
		public enum States
		{
			Disabled,
			PutOutIdle,
			Idle,
			PutOutLighting,
			Lighting,
			Burning,
			Burnt,
			Dissolving,
			Dissolved
		}

		public PlayerInventory _player;

		public float _lightingDuration = 1.5f;

		public float _burnDuration = 60f;

		public float _fuelRatioAttacking = 20f;

		public float _dissolveDuration = 1.5f;

		public float _fireParticleSize = 1.2f;

		public Material _burningMat;

		public GameObject _weaponFirePrefab;

		public Material _clothDisolveMat;

		public GameObject _weaponFireSpawn;

		public GameObject _inventoryMirror;

		public GameObject _craftMirror;

		private BurnableCloth.States _state;

		private bool _extraBurn;

		private bool _attacking;

		private float _putOutFuel;

		private float _fuel;

		private Material _normalMat;

		private GameObject _weaponFire;

		private ParticleScaler _fireParticleScale;

		private Light _firelight;

		private FMOD_StudioEventEmitter _fireAudioEmitter;

		public BurnableCloth.States State
		{
			get
			{
				return this._state;
			}
		}

		private void Awake()
		{
			this._normalMat = base.GetComponent<Renderer>().sharedMaterial;
			base.GetComponent<Renderer>().enabled = false;
			base.enabled = false;
		}

		private void OnEnable()
		{
			if (this._state == BurnableCloth.States.Idle || this._state == BurnableCloth.States.PutOutIdle)
			{
				this._attacking = false;
				LighterControler.HasLightableItem = true;
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
				this.GotClean();
				if (this._state > BurnableCloth.States.Idle)
				{
					this.Burnt();
					this.Dissolved();
				}
			}
		}

		private void Update()
		{
			switch (this._state)
			{
			case BurnableCloth.States.PutOutIdle:
			case BurnableCloth.States.Idle:
				this.Idle();
				break;
			case BurnableCloth.States.PutOutLighting:
			case BurnableCloth.States.Lighting:
				this.Light();
				break;
			case BurnableCloth.States.Burning:
				this.Burning();
				break;
			case BurnableCloth.States.Burnt:
				this.Burnt();
				break;
			case BurnableCloth.States.Dissolving:
				this.Dissolving();
				break;
			case BurnableCloth.States.Dissolved:
				this.Dissolved();
				break;
			}
		}

		public void OnDeserialized()
		{
			WeaponStatUpgrade.Types activeBonus = this._inventoryMirror.transform.parent.GetComponent<InventoryItemView>().ActiveBonus;
			if (activeBonus == WeaponStatUpgrade.Types.BurningWeapon || activeBonus == WeaponStatUpgrade.Types.BurningWeaponExtra)
			{
				this.EnableBurnableCloth();
			}
		}

		public void EnableBurnableCloth()
		{
			if (this._inventoryMirror)
			{
				this._inventoryMirror.SetActive(true);
			}
			if (this._craftMirror)
			{
				this._craftMirror.SetActive(true);
			}
			this._state = BurnableCloth.States.Idle;
			base.GetComponent<Renderer>().enabled = true;
			base.enabled = true;
		}

		public void EnableBurnableClothExtra()
		{
		}

		public void GotClean()
		{
			if (this._state == BurnableCloth.States.Burning)
			{
				this._putOutFuel = this._fuel;
				this.Burnt();
				this._state = BurnableCloth.States.PutOutIdle;
				this._player.IsWeaponBurning = false;
				FMODCommon.PlayOneshotNetworked("event:/player/actions/molotov_quench", base.transform, FMODCommon.NetworkRole.Any);
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
					this._state = ((this._state != BurnableCloth.States.PutOutIdle) ? BurnableCloth.States.Lighting : BurnableCloth.States.PutOutLighting);
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
				InventoryItemView component = this._inventoryMirror.transform.parent.GetComponent<InventoryItemView>();
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
				this._fuel = ((this._state != BurnableCloth.States.PutOutLighting) ? this._burnDuration : this._putOutFuel);
				if (component.ActiveBonus == WeaponStatUpgrade.Types.BurningWeaponExtra)
				{
					this._extraBurn = true;
					this._fuel *= 3f;
				}
				else
				{
					this._extraBurn = false;
				}
				this._state = BurnableCloth.States.Burning;
				this._player.IsWeaponBurning = true;
				this._attacking = false;
				component.ActiveBonus = (WeaponStatUpgrade.Types)(-1);
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
				if (this._extraBurn)
				{
					num *= 1.2f;
				}
				if (this._fireParticleScale)
				{
					this._fireParticleScale.particleScale = num;
				}
				if (this._firelight)
				{
					this._firelight.intensity = num * 0.428571433f * ((!this._attacking) ? 1f : 0.8f);
				}
				if (this._fireAudioEmitter)
				{
					this._fireAudioEmitter.SetVolume(num);
				}
			}
			else
			{
				this._state = BurnableCloth.States.Burnt;
			}
		}

		private void Burnt()
		{
			if (this._weaponFire)
			{
				UnityEngine.Object.Destroy(this._weaponFire);
				this._weaponFire = null;
			}
			base.GetComponent<Renderer>().sharedMaterial = this._clothDisolveMat;
			this._clothDisolveMat.SetFloat("_BurnAmount", 1f);
			this._fireParticleScale = null;
			this._firelight = null;
			this._fuel = this._dissolveDuration;
			this._state = BurnableCloth.States.Dissolving;
		}

		private void Dissolving()
		{
			this._fuel -= Time.deltaTime;
			if (this._fuel > 0f)
			{
				this._clothDisolveMat.SetFloat("_BurnAmount", this._fuel / this._dissolveDuration);
			}
			else
			{
				this._state = BurnableCloth.States.Dissolved;
			}
		}

		private void Dissolved()
		{
			if (this._inventoryMirror)
			{
				this._inventoryMirror.SetActive(false);
			}
			if (this._craftMirror)
			{
				this._craftMirror.SetActive(false);
			}
			base.GetComponent<Renderer>().enabled = false;
			base.GetComponent<Renderer>().sharedMaterial = this._normalMat;
			this._clothDisolveMat.SetFloat("_BurnAmount", 1f);
			this._state = BurnableCloth.States.Disabled;
			this._player.IsWeaponBurning = false;
			base.enabled = false;
		}

		public float ExpoEaseIn(float t, float b, float c, float d)
		{
			return (t != 0f) ? (c * Mathf.Pow(2f, 10f * (t / d - 1f)) + b) : b;
		}
	}
}
