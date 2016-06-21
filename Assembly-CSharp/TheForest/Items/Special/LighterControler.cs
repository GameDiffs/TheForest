using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Items.Special
{
	public class LighterControler : SpecialItemControlerBase
	{
		public GameObject _lighterFlame;

		public int _maxSparksBeforeLight = 10;

		private float _buttonPressStart;

		private int _sparks;

		public bool _breakRoutine;

		public static bool HasLightableItem;

		public static bool IsBusy;

		private Coroutine _lightingHeldFireRoutine;

		protected override bool IsActive
		{
			get
			{
				return LocalPlayer.Inventory.HasInSlot(Item.EquipmentSlot.LeftHand, LocalPlayer.Inventory.LastLight._itemId);
			}
		}

		public bool IsReallyActive
		{
			get
			{
				return this.IsActive && LocalPlayer.Inventory.LastLight == this;
			}
		}

		private void Awake()
		{
			LocalPlayer.Inventory.DefaultLight = this;
			LocalPlayer.Inventory.LastLight = this;
			LighterControler.IsBusy = false;
		}

		protected override void Update()
		{
			if (LocalPlayer.Inventory.enabled && this.CurrentViewTest())
			{
				if (!LighterControler.HasLightableItem || !this.IsActive)
				{
					if (TheForest.Utils.Input.GetButtonDown(this._buttonCached))
					{
						if (!this.IsActive)
						{
							this.OnActivating();
						}
						else
						{
							this.OnDeactivating();
						}
					}
				}
				else
				{
					if (TheForest.Utils.Input.GetButtonDown(this._buttonCached))
					{
						this._buttonPressStart = Time.realtimeSinceStartup;
					}
					if (TheForest.Utils.Input.GetButtonUp(this._buttonCached) && Time.realtimeSinceStartup - this._buttonPressStart < 0.2f)
					{
						this.OnDeactivating();
					}
				}
			}
			if (this._checkRemoval)
			{
				this._checkRemoval = false;
				if (!LocalPlayer.Inventory.Owns(this._itemId))
				{
					base.enabled = false;
				}
			}
		}

		public override bool ToggleSpecial(bool enable)
		{
			base.CancelInvoke();
			if (enable)
			{
				if (LocalPlayer.Inventory.LastLight != this)
				{
					LocalPlayer.Inventory.StashLeftHand();
					LocalPlayer.Inventory.LastLight = this;
				}
				this.TurnLighterOn();
			}
			else
			{
				LocalPlayer.Animator.SetBoolReflected("lighterIgnite", false);
			}
			return true;
		}

		private void TurnLighterOn()
		{
			this._sparks = 0;
			base.InvokeRepeating("SparkLighter", 0.5f, 0.5f);
			this._lighterFlame.SetActive(false);
			LocalPlayer.Tuts.HideLighter();
		}

		public void equipLighterOnly()
		{
			base.StartCoroutine(this.equipLighterRoutine());
		}

		public void LightTheFire()
		{
			base.StartCoroutine(this.LightingFireRoutine());
		}

		public void LightHeldFire()
		{
			this._lightingHeldFireRoutine = base.StartCoroutine(this.LightingHeldFireRoutine());
		}

		public void CancelLightHeldFire()
		{
			if (this._lightingHeldFireRoutine != null)
			{
				LocalPlayer.Animator.SetBoolReflected("lightWeaponBool", false);
				LocalPlayer.Inventory.UnlockEquipmentSlot(Item.EquipmentSlot.LeftHand);
				base.StopCoroutine(this._lightingHeldFireRoutine);
				this._lightingHeldFireRoutine = null;
				LighterControler.IsBusy = false;
			}
		}

		[DebuggerHidden]
		private IEnumerator LightingFireRoutine()
		{
			LighterControler.<LightingFireRoutine>c__Iterator162 <LightingFireRoutine>c__Iterator = new LighterControler.<LightingFireRoutine>c__Iterator162();
			<LightingFireRoutine>c__Iterator.<>f__this = this;
			return <LightingFireRoutine>c__Iterator;
		}

		[DebuggerHidden]
		private IEnumerator LightingHeldFireRoutine()
		{
			LighterControler.<LightingHeldFireRoutine>c__Iterator163 <LightingHeldFireRoutine>c__Iterator = new LighterControler.<LightingHeldFireRoutine>c__Iterator163();
			<LightingHeldFireRoutine>c__Iterator.<>f__this = this;
			return <LightingHeldFireRoutine>c__Iterator;
		}

		private void SparkLighter()
		{
			if (!this._lighterFlame.activeSelf && LocalPlayer.Inventory.CurrentView == PlayerInventory.PlayerViews.World)
			{
				LocalPlayer.Sfx.PlayLighterSound();
				LocalPlayer.Animator.SetBoolReflected("lighterIgnite", true);
				if (UnityEngine.Random.Range(0, 2) == 0 || ++this._sparks == this._maxSparksBeforeLight)
				{
					this._sparks = 0;
					base.Invoke("TurnLighterOff", (float)UnityEngine.Random.Range(10, 35));
					LocalPlayer.Animator.SetBoolReflected("lighterIgnite", false);
					this._lighterFlame.SetActive(true);
				}
			}
		}

		private void TurnLighterOff()
		{
			this._lighterFlame.SetActive(false);
		}

		public void StashLighter()
		{
			base.StartCoroutine(this.StashLighterRoutine());
		}

		public void StashLighter2()
		{
			if (LighterControler.IsBusy)
			{
				return;
			}
			base.StartCoroutine(this.StashLighterRoutine());
		}

		[DebuggerHidden]
		private IEnumerator StashLighterRoutine()
		{
			LighterControler.<StashLighterRoutine>c__Iterator164 <StashLighterRoutine>c__Iterator = new LighterControler.<StashLighterRoutine>c__Iterator164();
			<StashLighterRoutine>c__Iterator.<>f__this = this;
			return <StashLighterRoutine>c__Iterator;
		}

		[DebuggerHidden]
		private IEnumerator equipLighterRoutine()
		{
			LighterControler.<equipLighterRoutine>c__Iterator165 <equipLighterRoutine>c__Iterator = new LighterControler.<equipLighterRoutine>c__Iterator165();
			<equipLighterRoutine>c__Iterator.<>f__this = this;
			return <equipLighterRoutine>c__Iterator;
		}

		protected override void OnActivating()
		{
			if (!LighterControler.IsBusy && LocalPlayer.Inventory.LastLight == this && !LocalPlayer.Animator.GetBool("drawBowBool") && !LocalPlayer.Inventory.IsSlotLocked(Item.EquipmentSlot.LeftHand))
			{
				LocalPlayer.Inventory.TurnOnLastLight();
			}
		}

		protected override void OnDeactivating()
		{
			if (this.IsReallyActive)
			{
				this.StashLighter();
			}
		}

		private void disableBreakRoutine()
		{
			this._breakRoutine = false;
		}

		private void stopLightHeldFire()
		{
			this._breakRoutine = true;
			base.StopCoroutine("LightingHeldFireRoutine");
			base.StopCoroutine("LightingHeldFireRoutine");
			LocalPlayer.Animator.SetBoolReflected("lightWeaponBool", false);
		}
	}
}
