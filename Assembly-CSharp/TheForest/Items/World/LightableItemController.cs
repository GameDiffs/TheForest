using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Items.Special;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Items.World
{
	public class LightableItemController : MonoBehaviour
	{
		public GameObject fire;

		public bool isLit;

		[ItemIdPicker]
		public int _molotovId;

		[ItemIdPicker]
		public int _lighterId;

		public bool lighterWasEquipped;

		private bool checkForEquipped;

		public bool isActive;

		public string fmodEvent = "event:/combat/molotov_held";

		private bool isLighting;

		private void OnEnable()
		{
			if (LocalPlayer.Inventory.HasInSlot(Item.EquipmentSlot.LeftHand, this._lighterId) && !this.checkForEquipped)
			{
				this.checkForEquipped = true;
				this.lighterWasEquipped = true;
			}
			this.checkForEquipped = true;
			this.isActive = true;
			base.StartCoroutine("forceFireOff");
			LocalPlayer.Inventory.SpecialItems.SendMessage("disableBreakRoutine", SendMessageOptions.DontRequireReceiver);
			LocalPlayer.Inventory.SpecialItems.SendMessage("equipLighterOnly", SendMessageOptions.DontRequireReceiver);
			this.fire.SetActive(false);
			LighterControler.HasLightableItem = true;
			LocalPlayer.Inventory.UseAltProjectile = true;
		}

		private void Update()
		{
			if (LocalPlayer.Inventory.DefaultLight.IsReallyActive)
			{
				if (TheForest.Utils.Input.GetButton("Lighter") && !this.isLighting)
				{
					Scene.HudGui.SetDelayedIconController(this);
				}
				else
				{
					Scene.HudGui.UnsetDelayedIconController(this);
				}
				if (TheForest.Utils.Input.GetButtonAfterDelay("Lighter", 0.5f) && !this.isLighting)
				{
					base.CancelInvoke("ResetIsLighting");
					base.Invoke("ResetIsLighting", 4f);
					this.isLighting = true;
					LocalPlayer.SpecialItems.SendMessage("LightHeldFire");
					Scene.HudGui.UnsetDelayedIconController(this);
				}
			}
		}

		private void ResetIsLighting()
		{
			this.isLighting = false;
		}

		private void GotClean()
		{
			if (this.isLit)
			{
				FMODCommon.PlayOneshotNetworked("event:/player/actions/molotov_quench", base.transform, FMODCommon.NetworkRole.Any);
			}
			this.isLit = false;
			base.StartCoroutine("forceFireOff");
			LighterControler.HasLightableItem = true;
		}

		private void OnDisable()
		{
			base.Invoke("disableActive", 0.2f);
			base.Invoke("checkStashLighter", 0.55f);
			LocalPlayer.SpecialItems.SendMessage("stopLightHeldFire", SendMessageOptions.DontRequireReceiver);
			LighterControler.HasLightableItem = false;
			base.CancelInvoke("enableFire");
			this.isLit = false;
			this.fire.SetActive(false);
			LocalPlayer.Inventory.UseAltProjectile = false;
		}

		private void OnProjectileThrown(GameObject projectile)
		{
			if (this.fire.activeSelf)
			{
				FMOD_StudioEventEmitter[] components = this.fire.GetComponents<FMOD_StudioEventEmitter>();
				for (int i = 0; i < components.Length; i++)
				{
					if (components[i] != null && components[i].path == this.fmodEvent)
					{
						components[i].TransplantEventInstance(projectile.transform);
						break;
					}
				}
			}
		}

		[DebuggerHidden]
		private IEnumerator forceFireOff()
		{
			LightableItemController.<forceFireOff>c__Iterator2F <forceFireOff>c__Iterator2F = new LightableItemController.<forceFireOff>c__Iterator2F();
			<forceFireOff>c__Iterator2F.<>f__this = this;
			return <forceFireOff>c__Iterator2F;
		}

		private void enableFire()
		{
			this.isLighting = false;
			this.fire.SetActive(true);
			base.Invoke("disableBlock", 0.5f);
			LocalPlayer.Tuts.MolotovTutDone();
		}

		private void disableBlock()
		{
		}

		private void disableActive()
		{
			this.isActive = false;
		}

		private void setIsLit()
		{
			this.isLit = true;
			base.CancelInvoke("checkIfLighting");
		}

		private void checkStashLighter()
		{
			if (this.isActive)
			{
				return;
			}
			if (this.lighterWasEquipped)
			{
				this.lighterWasEquipped = false;
				this.checkForEquipped = false;
				return;
			}
			if (!LocalPlayer.Inventory.HasInSlot(Item.EquipmentSlot.RightHand, this._molotovId))
			{
				LocalPlayer.Inventory.SpecialItems.SendMessage("StashLighter2", SendMessageOptions.DontRequireReceiver);
				this.lighterWasEquipped = false;
				this.checkForEquipped = false;
			}
		}
	}
}
