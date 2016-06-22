using Bolt;
using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Items;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Buildings.World
{
	[DoNotSerializePublic, AddComponentMenu("Buildings/World/Water Source")]
	public class WaterSource : EntityBehaviour<IWaterSourceState>
	{
		public enum IconModes
		{
			FixedPosition,
			AutoPosition
		}

		[SerializeThis]
		public float _amount;

		public float _minAmount;

		public float _maxAmount;

		public int _pollutedDamage = 10;

		[SerializeThis]
		public bool _polluted;

		[ItemIdPicker]
		public int _potItemId;

		[ItemIdPicker]
		public int _waterSkinItemId;

		public GameObject _billboardDrink;

		public GameObject _billboardDrinkSheen;

		public GameObject _billboardGather;

		public GameObject _billboardGatherSheen;

		public WaterSource.IconModes _iconMode;

		private RaycastHit _hit;

		private LayerMask _layerMask;

		private bool _terrainBlockDrink;

		private bool _atLakeWater;

		[SerializeThis]
		private float _lastUseTime = -1f;

		public float AmountReal
		{
			get
			{
				if (BoltNetwork.isRunning && this.entity && this.entity.isAttached)
				{
					return base.state.amount;
				}
				return this._amount;
			}
			set
			{
				if (BoltNetwork.isRunning && this.entity && this.entity.isAttached && this.entity.isOwner && base.state.amount != value)
				{
					base.state.amount = value;
				}
				this._amount = value;
			}
		}

		private bool CanDrink
		{
			get
			{
				return base.enabled && (!LocalPlayer.FpCharacter.swimming || LocalPlayer.Transform.position.y - LocalPlayer.WaterViz.WaterLevel > 1.2f) && (this.AmountReal > this._minAmount || this._maxAmount == 0f) && (this._iconMode == WaterSource.IconModes.FixedPosition || (double)LocalPlayer.AnimControl.normCamX > 0.37) && !LocalPlayer.FpCharacter.jumping && !this._terrainBlockDrink && (double)LocalPlayer.Ridigbody.velocity.sqrMagnitude < 0.01;
			}
		}

		private bool CanGather
		{
			get
			{
				return this.CanDrink && LocalPlayer.Inventory && (LocalPlayer.Inventory.HasInSlot(Item.EquipmentSlot.RightHand, this._potItemId) || LocalPlayer.Inventory.HasInSlot(Item.EquipmentSlot.RightHand, this._waterSkinItemId));
			}
		}

		private void Awake()
		{
			this._billboardDrink.SetActive(false);
			this._billboardDrinkSheen.SetActive(false);
			this._billboardGather.SetActive(false);
			this._billboardGatherSheen.SetActive(false);
			this._layerMask = 67108880;
			base.enabled = false;
		}

		private void FixedUpdate()
		{
			this._terrainBlockDrink = false;
			bool canDrink = this.CanDrink;
			if (canDrink)
			{
				bool canGather = this.CanGather;
				if (canGather)
				{
					if (TheForest.Utils.Input.GetButtonAfterDelay("Craft", 0.5f))
					{
						LocalPlayer.Sfx.PlayTwinkle();
						LocalPlayer.Inventory.GatherWater(!this._polluted);
						if (this._maxAmount > 0f)
						{
							this.RemoveWater(1f);
						}
					}
				}
				else if (TheForest.Utils.Input.GetButtonAfterDelay("Take", 0.5f))
				{
					float num;
					if (this._maxAmount == 0f)
					{
						num = LocalPlayer.Stats.Thirst;
					}
					else
					{
						num = Mathf.Min(this.AmountReal, LocalPlayer.Stats.Thirst);
						this.RemoveWater(num);
					}
					LocalPlayer.Stats.Thirst -= num * ((!this._polluted) ? 1f : 0.5f);
					if (this._atLakeWater)
					{
						this.enableDrinkParams();
						base.Invoke("playDrinkSFX", 1f);
						base.Invoke("doPollutedDamage", 2.2f);
						return;
					}
					this.playDrinkSFX();
					this.doPollutedDamage();
				}
			}
			this.ToggleIcons(true);
		}

		private void GrabEnter()
		{
			base.enabled = true;
			this._terrainBlockDrink = false;
			this.ToggleIcons(true);
		}

		private void GrabExit()
		{
			base.enabled = false;
			this.ToggleIcons(false);
		}

		private void DeadBodyEnteredArea()
		{
			this._polluted = true;
		}

		private void playDrinkSFX()
		{
			if (this._iconMode != WaterSource.IconModes.AutoPosition)
			{
				LocalPlayer.Sfx.PlayDrinkFromWaterSource();
			}
		}

		private void doPollutedDamage()
		{
			if (this._polluted)
			{
				LocalPlayer.Stats.HitFoodDelayed(this._pollutedDamage);
			}
		}

		public void AddWater(float amount)
		{
			this.AmountReal = Mathf.Min(this.AmountReal + amount, this._maxAmount);
			if (base.enabled)
			{
				this.ToggleIcons(base.enabled);
			}
			base.SendMessage("UpdateWater", SendMessageOptions.DontRequireReceiver);
		}

		public void RemoveWater(float amount)
		{
			if (BoltNetwork.isClient && base.GetComponentInParent<BoltEntity>() && this.entity.isAttached)
			{
				RemoveWater removeWater = global::RemoveWater.Create(GlobalTargets.OnlyServer);
				removeWater.Amount = amount;
				removeWater.Entity = this.entity;
				removeWater.Send();
			}
			else
			{
				this.AmountReal = Mathf.Max(this.AmountReal - amount, 0f);
			}
			this.ToggleIcons(base.enabled);
			base.SendMessage("UpdateWater", SendMessageOptions.DontRequireReceiver);
		}

		private void ToggleIcons(bool sheen)
		{
			bool canGather = this.CanGather;
			bool flag = !canGather && this.CanDrink;
			if (sheen)
			{
				if (this._billboardDrink.activeSelf)
				{
					this._billboardDrink.SetActive(false);
				}
				if (this._billboardDrinkSheen.activeSelf != flag)
				{
					this._billboardDrinkSheen.SetActive(flag);
				}
				if (flag)
				{
					this.AutoPositionIcon(this._billboardDrinkSheen);
				}
				if (this._billboardGather.activeSelf)
				{
					this._billboardGather.SetActive(false);
				}
				if (this._billboardGatherSheen.activeSelf != canGather)
				{
					this._billboardGatherSheen.SetActive(canGather);
				}
				if (canGather)
				{
					this.AutoPositionIcon(this._billboardGatherSheen);
				}
			}
			else
			{
				if (this._billboardDrink.activeSelf != flag)
				{
					this._billboardDrink.SetActive(flag);
				}
				if (this._billboardDrinkSheen.activeSelf)
				{
					this._billboardDrinkSheen.SetActive(false);
				}
				if (flag)
				{
					this.AutoPositionIcon(this._billboardDrink);
				}
				if (this._billboardGather.activeSelf != canGather)
				{
					this._billboardGather.SetActive(canGather);
				}
				if (this._billboardGatherSheen.activeSelf)
				{
					this._billboardGatherSheen.SetActive(false);
				}
				if (canGather)
				{
					this.AutoPositionIcon(this._billboardGather);
				}
			}
		}

		private void AutoPositionIcon(GameObject icon)
		{
			if (this._iconMode == WaterSource.IconModes.AutoPosition && LocalPlayer.Transform)
			{
				Vector3 position = Vector3.zero;
				if (Physics.Raycast(LocalPlayer.MainCamTr.position, LocalPlayer.MainCamTr.forward, out this._hit, 8f, this._layerMask.value))
				{
					if (this._hit.transform.gameObject.CompareTag("Water"))
					{
						this._atLakeWater = true;
					}
					if (this._hit.transform.gameObject.CompareTag("TerrainMain"))
					{
						this._terrainBlockDrink = true;
						this._atLakeWater = false;
						return;
					}
					this._terrainBlockDrink = false;
					position = LocalPlayer.MainCamTr.position + LocalPlayer.MainCamTr.forward * 2.5f;
					position.y -= 1f;
				}
				icon.transform.position = position;
			}
			else
			{
				this._atLakeWater = false;
			}
		}

		private void enableDrinkParams()
		{
			LocalPlayer.Animator.SetBoolReflected("drinkWater", true);
			LocalPlayer.AnimControl.playerHeadCollider.enabled = false;
			LocalPlayer.Inventory.HideAllEquiped(false);
			LocalPlayer.Animator.SetLayerWeightReflected(1, 1f);
			LocalPlayer.Animator.SetLayerWeightReflected(2, 1f);
			LocalPlayer.Animator.SetLayerWeightReflected(3, 0f);
			LocalPlayer.Transform.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
			LocalPlayer.MainRotator.enabled = false;
			LocalPlayer.CamRotator.stopInput = true;
			LocalPlayer.CamRotator.rotationRange = new Vector2(0f, 0f);
			LocalPlayer.FpCharacter.drinking = true;
			LocalPlayer.FpCharacter.enabled = false;
			LocalPlayer.CamFollowHead.smoothLock = true;
			LocalPlayer.CamFollowHead.lockYCam = true;
			LocalPlayer.AnimControl.animEvents.StartCoroutine("smoothDisableSpine");
			base.StartCoroutine("forceStop");
			base.enabled = false;
			this._billboardDrink.SetActive(false);
			this._billboardDrinkSheen.SetActive(false);
		}

		[DebuggerHidden]
		private IEnumerator forceStop()
		{
			WaterSource.<forceStop>c__Iterator158 <forceStop>c__Iterator = new WaterSource.<forceStop>c__Iterator158();
			<forceStop>c__Iterator.<>f__this = this;
			return <forceStop>c__Iterator;
		}

		public override void Attached()
		{
			if (BoltNetwork.isServer)
			{
				base.state.amount = this._amount;
			}
			base.state.AddCallback("amount", new PropertyCallbackSimple(this.RefreshAmount));
		}

		private void RefreshAmount()
		{
			this.AmountReal = base.state.amount;
			base.SendMessage("CheckForWater", SendMessageOptions.DontRequireReceiver);
		}
	}
}
