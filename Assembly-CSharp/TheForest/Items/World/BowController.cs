using System;
using TheForest.Items.Craft;
using TheForest.Items.Inventory;
using TheForest.Items.Special;
using TheForest.Player;
using TheForest.Tools;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Items.World
{
	public class BowController : MonoBehaviour
	{
		public PlayerInventory _player;

		[ItemIdPicker(Item.Types.RangedWeapon)]
		public int _bowItemId;

		[ItemIdPicker(Item.Types.Ammo)]
		public int _ammoItemId;

		public GameObject _ammoAnimated;

		public Renderer _ammoAnimationRenderer;

		public float _reArmDelay = 0.5f;

		public MasterFireSpread _fireArrowPrefab;

		public GameObject _poisonArrowPrefab;

		public AimingReticle _aimingReticle;

		private InventoryItemView _currentAmmo;

		private bool _showRotateArrowType;

		private bool _lightingArrow;

		private int _attackHash;

		private Animator _animator;

		private Animator _bowAnimator;

		private float _nextReArm;

		private WeaponStatUpgrade.Types _activeAmmoBonus = (WeaponStatUpgrade.Types)(-1);

		private MasterFireSpread _activeFireArrowGO;

		private bool CanSetArrowOnFire
		{
			get
			{
				return !this._lightingArrow && this._activeAmmoBonus == (WeaponStatUpgrade.Types)(-1) && !LighterControler.IsBusy && this._player.AmountOf(this._ammoItemId, true) > 0 && this.CurrentArrowItemView.ActiveBonus == WeaponStatUpgrade.Types.BurningAmmo;
			}
		}

		private InventoryItemView BowItemView
		{
			get
			{
				return LocalPlayer.Inventory.InventoryItemViewsCache[this._bowItemId][0];
			}
		}

		private InventoryItemView CurrentArrowItemView
		{
			get
			{
				return LocalPlayer.Inventory.InventoryItemViewsCache[this._ammoItemId][Mathf.Max(LocalPlayer.Inventory.AmountOf(this._ammoItemId, true) - 1, 0)];
			}
		}

		private InventoryItemView PrevioustArrowItemView
		{
			get
			{
				return LocalPlayer.Inventory.InventoryItemViewsCache[this._ammoItemId][Mathf.Max(LocalPlayer.Inventory.AmountOf(this._ammoItemId, true) - 1, 0)];
			}
		}

		private void Start()
		{
			if (base.GetComponentInParent<PlayerInventory>())
			{
				this._nextReArm = 3.40282347E+38f;
				this._ammoAnimated.SetActive(true);
				this._attackHash = Animator.StringToHash("attacking");
				this._animator = LocalPlayer.Animator;
				this._bowAnimator = base.GetComponent<Animator>();
				if (this._aimingReticle)
				{
					this._aimingReticle.enabled = false;
				}
			}
		}

		private void OnEnable()
		{
			EventRegistry.Player.Subscribe(TfEvent.AddedItem, new EventRegistry.SubscriberCallback(this.OnItemAdded));
			if (this.CurrentArrowItemView.ActiveBonus == WeaponStatUpgrade.Types.BurningAmmo)
			{
				LighterControler.HasLightableItem = true;
			}
			this.SetActiveArrowBonus(this._activeAmmoBonus);
		}

		private void OnDisable()
		{
			EventRegistry.Player.Unsubscribe(TfEvent.AddedItem, new EventRegistry.SubscriberCallback(this.OnItemAdded));
			LighterControler.HasLightableItem = false;
			this._nextReArm = 3.40282347E+38f;
			this._ammoAnimated.SetActive(true);
			if (this._animator)
			{
				this._animator.SetBoolReflected("drawBowBool", false);
				if (base.gameObject.activeInHierarchy)
				{
					this._bowAnimator.SetBoolReflected("drawBool", false);
				}
				this.ShutDown(false);
				this.ShutDownFire();
			}
			if (this._activeFireArrowGO)
			{
				UnityEngine.Object.Destroy(this._activeFireArrowGO);
			}
			this._lightingArrow = false;
			if (Scene.HudGui)
			{
				Scene.HudGui.ToggleArrowBonusIcon.SetActive(false);
			}
		}

		private void Update()
		{
			if (this._player.CurrentView == PlayerInventory.PlayerViews.World)
			{
				if (!LocalPlayer.Create.Grabber.Target && LocalPlayer.MainCamTr.forward.y < -0.85f)
				{
					WeaponStatUpgrade.Types types = this.NextAvailableArrowBonus(this.BowItemView.ActiveBonus);
					if (types != this.BowItemView.ActiveBonus)
					{
						this._showRotateArrowType = true;
						if (!Scene.HudGui.ToggleArrowBonusIcon.activeSelf)
						{
							Scene.HudGui.ToggleArrowBonusIcon.SetActive(true);
						}
						if (TheForest.Utils.Input.GetButtonDown("Rotate"))
						{
							LocalPlayer.Sfx.PlayWhoosh();
							this.SetActiveBowBonus(types);
							Scene.HudGui.ToggleArrowBonusIcon.SetActive(false);
						}
					}
					else if (this._showRotateArrowType)
					{
						this._showRotateArrowType = false;
						Scene.HudGui.ToggleArrowBonusIcon.SetActive(false);
					}
				}
				else if (this._showRotateArrowType)
				{
					this._showRotateArrowType = false;
					Scene.HudGui.ToggleArrowBonusIcon.SetActive(false);
				}
				if (this.CurrentArrowItemView.ActiveBonus != this.BowItemView.ActiveBonus)
				{
					LocalPlayer.Inventory.SortInventoryViewsByBonus(this.CurrentArrowItemView, this.BowItemView.ActiveBonus, false);
					if (this.CurrentArrowItemView.ActiveBonus != this.BowItemView.ActiveBonus)
					{
						this.SetActiveBowBonus(this.CurrentArrowItemView.ActiveBonus);
					}
					this.UpdateArrowRenderer();
				}
				WeaponStatUpgrade.Types activeBonus = this.CurrentArrowItemView.ActiveBonus;
				bool canSetArrowOnFire = this.CanSetArrowOnFire;
				if (canSetArrowOnFire && TheForest.Utils.Input.GetButton("Lighter"))
				{
					Scene.HudGui.SetDelayedIconController(this);
				}
				else
				{
					Scene.HudGui.UnsetDelayedIconController(this);
				}
				if (canSetArrowOnFire)
				{
					if (TheForest.Utils.Input.GetButtonAfterDelay("Lighter", 0.5f))
					{
						this.SetArrowOnFire();
					}
				}
				else if (activeBonus != WeaponStatUpgrade.Types.BurningAmmo && this._activeAmmoBonus != activeBonus)
				{
					this.SetActiveArrowBonus(activeBonus);
				}
				if (!this._lightingArrow)
				{
					if (TheForest.Utils.Input.GetButtonDown("Fire1") && !LocalPlayer.Animator.GetBool("ballHeld"))
					{
						LocalPlayer.Inventory.CancelNextChargedAttack = false;
						if (this._aimingReticle)
						{
							this._aimingReticle.enabled = true;
						}
						this.ReArm();
						this._animator.SetBoolReflected("drawBowBool", true);
						this._bowAnimator.SetBoolReflected("drawBool", true);
						this._bowAnimator.SetBoolReflected("bowFireBool", false);
						this._animator.SetBoolReflected("bowFireBool", false);
						this._player.StashLeftHand();
						this._animator.SetBoolReflected("checkArms", false);
						this._animator.SetBoolReflected("onHand", false);
					}
					else if (TheForest.Utils.Input.GetButtonDown("AltFire") || LocalPlayer.Animator.GetBool("ballHeld"))
					{
						LocalPlayer.AnimControl.animEvents.enableSpine();
						this._player.CancelNextChargedAttack = true;
						this._animator.SetBool("drawBowBool", false);
						this._bowAnimator.SetBool("drawBool", false);
						this.ShutDown(false);
					}
					if (TheForest.Utils.Input.GetButtonUp("Fire1") || LocalPlayer.Animator.GetBool("ballHeld"))
					{
						this._currentAmmo = this.CurrentArrowItemView;
						if (this._aimingReticle)
						{
							this._aimingReticle.enabled = false;
						}
						base.CancelInvoke();
						if (this._animator.GetCurrentAnimatorStateInfo(1).tagHash == this._attackHash && this._animator.GetBool("drawBowBool") && !LocalPlayer.Animator.GetBool("ballHeld"))
						{
							this._animator.SetBoolReflected("bowFireBool", true);
							this._bowAnimator.SetBoolReflected("bowFireBool", true);
							this._animator.SetBoolReflected("drawBowBool", false);
							this._bowAnimator.SetBoolReflected("drawBool", false);
							LocalPlayer.TargetFunctions.sendPlayerAttacking();
							this.InitReArm();
						}
						else if (LocalPlayer.Animator.GetBool("ballHeld"))
						{
							LocalPlayer.AnimControl.animEvents.enableSpine();
							this._player.CancelNextChargedAttack = true;
							this._animator.SetBoolReflected("drawBowBool", false);
							this._bowAnimator.SetBoolReflected("drawBool", false);
							this.ShutDown(false);
						}
						else
						{
							this.ShutDown(true);
						}
					}
					else if (this._nextReArm < Time.time)
					{
						this.ReArm();
					}
				}
				else
				{
					LocalPlayer.Inventory.CancelNextChargedAttack = true;
				}
			}
		}

		private void OnItemAdded(object o)
		{
			this.OnItemAdded((int)o);
		}

		private void OnItemAdded(int itemId)
		{
			if (this._ammoItemId == itemId && this._player.AmountOf(this._ammoItemId, true) == 1)
			{
				this._player.ToggleAmmo(this._ammoItemId, true);
			}
		}

		private void ShutDownFire()
		{
			this.SetActiveArrowBonus((WeaponStatUpgrade.Types)(-1));
		}

		private void ShutDown(bool rearm)
		{
			base.CancelInvoke();
			if (base.gameObject.activeInHierarchy)
			{
				this._animator.SetBool("drawBowBool", false);
				this._bowAnimator.SetBool("drawBool", false);
				this._animator.SetBool("bowFireBool", false);
				this._bowAnimator.SetBool("bowFireBool", false);
			}
			if (rearm)
			{
				this.InitReArm();
			}
		}

		private void InitReArm()
		{
			this._ammoAnimated.SetActive(false);
			this._nextReArm = Time.time + this._reArmDelay;
		}

		private void ReArm()
		{
			this._nextReArm = 3.40282347E+38f;
			this._ammoAnimated.SetActive(true);
		}

		private void LightArrow()
		{
			GameStats.LitArrow.Invoke();
			this._lightingArrow = false;
			this.SetActiveArrowBonus(WeaponStatUpgrade.Types.BurningAmmo);
			this._activeFireArrowGO = UnityEngine.Object.Instantiate<MasterFireSpread>(this._fireArrowPrefab);
			this._activeFireArrowGO.enabled = false;
			this._activeFireArrowGO.transform.parent = this._ammoAnimated.transform;
			this._activeFireArrowGO.transform.localPosition = Vector3.zero;
			this._activeFireArrowGO.transform.localRotation = Quaternion.identity;
			this._activeFireArrowGO.owner = LocalPlayer.Transform;
			LighterControler.HasLightableItem = (this.CurrentArrowItemView.ActiveBonus == WeaponStatUpgrade.Types.BurningAmmo);
		}

		private void SetActiveBowBonus(WeaponStatUpgrade.Types bonusType)
		{
			if (this.BowItemView.ActiveBonus == WeaponStatUpgrade.Types.BurningAmmo && this.CurrentArrowItemView.ActiveBonus == WeaponStatUpgrade.Types.BurningAmmo && bonusType != WeaponStatUpgrade.Types.BurningAmmo && this._activeFireArrowGO)
			{
				this.CurrentArrowItemView.ActiveBonus = (WeaponStatUpgrade.Types)(-1);
				UnityEngine.Object.Destroy(this._activeFireArrowGO.gameObject);
				this._activeFireArrowGO = null;
			}
			this.BowItemView.ActiveBonus = bonusType;
		}

		private void SetActiveArrowBonus(WeaponStatUpgrade.Types bonusType)
		{
			if (this._activeAmmoBonus != bonusType)
			{
				this._activeAmmoBonus = bonusType;
				this.UpdateArrowRenderer();
			}
			if (this._activeFireArrowGO)
			{
				UnityEngine.Object.Destroy(this._activeFireArrowGO.gameObject);
				this._activeFireArrowGO = null;
			}
		}

		private void UpdateArrowRenderer()
		{
			this._ammoAnimationRenderer.sharedMaterials = this.CurrentArrowItemView.GetComponent<Renderer>().sharedMaterials;
		}

		private WeaponStatUpgrade.Types NextAvailableArrowBonus(WeaponStatUpgrade.Types current)
		{
			if (!LocalPlayer.Inventory.Owns(this._ammoItemId))
			{
				return (WeaponStatUpgrade.Types)(-1);
			}
			WeaponStatUpgrade.Types types;
			if (current != WeaponStatUpgrade.Types.BurningAmmo)
			{
				if (current != WeaponStatUpgrade.Types.PoisonnedAmmo)
				{
					types = WeaponStatUpgrade.Types.BurningAmmo;
				}
				else
				{
					types = (WeaponStatUpgrade.Types)(-1);
				}
			}
			else
			{
				types = WeaponStatUpgrade.Types.PoisonnedAmmo;
			}
			if (LocalPlayer.Inventory.OwnsItemWithBonus(this._ammoItemId, types))
			{
				return types;
			}
			return this.NextAvailableArrowBonus(types);
		}

		private void SetArrowOnFire()
		{
			this.ReArm();
			this._player.SpecialItems.SendMessage("LightHeldFire");
			base.CancelInvoke("LightArrow");
			base.Invoke("LightArrow", 2f);
			this._lightingArrow = true;
		}

		private void OnAmmoFired(GameObject Ammo)
		{
			GameStats.ArrowFired.Invoke();
			WeaponStatUpgrade.Types activeAmmoBonus = this._activeAmmoBonus;
			if (activeAmmoBonus != WeaponStatUpgrade.Types.BurningAmmo)
			{
				if (activeAmmoBonus == WeaponStatUpgrade.Types.PoisonnedAmmo)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this._poisonArrowPrefab);
					gameObject.transform.parent = Ammo.transform;
					gameObject.transform.localPosition = Vector3.zero;
					gameObject.transform.localRotation = Quaternion.identity;
				}
			}
			else if (this._activeFireArrowGO)
			{
				this._activeFireArrowGO.transform.parent = Ammo.transform;
				if (!this._activeFireArrowGO.GetComponent<destroyAfter>())
				{
					this._activeFireArrowGO.gameObject.AddComponent<destroyAfter>().destroyTime = 15f;
				}
				this._activeFireArrowGO = null;
			}
			this.SetActiveArrowBonus((WeaponStatUpgrade.Types)(-1));
			this._currentAmmo.ActiveBonus = (WeaponStatUpgrade.Types)(-1);
		}
	}
}
