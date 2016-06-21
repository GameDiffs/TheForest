using Bolt;
using PathologicalGames;
using System;
using TheForest.Items.Craft;
using TheForest.Items.Utils;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Items.World
{
	[AddComponentMenu("Items/World/PickUp")]
	public class PickUp : MonoBehaviour
	{
		[Serializable]
		public class LootTuple
		{
			public RandomRange _amount;

			[ItemIdPicker]
			public int _itemId;
		}

		public enum InputMethods
		{
			ButtonDown,
			ButtonHold
		}

		public RandomRange _amount;

		[ItemIdPicker]
		public int _itemId;

		public PickUp.LootTuple[] _bonusItems;

		public PickUp.InputMethods _inputMethod;

		public bool _holdInput;

		public bool _infinite;

		public bool _preventFakeDrop;

		public bool _preventAutoEquip;

		public bool _forceAutoEquip;

		public bool _bodyAutoCollect;

		public bool _grabberAutoCollect;

		public bool _destroyIfFull;

		public bool _disableInsteadOfDestroy;

		public bool _spiderDice;

		public bool _poolManagerDespawnCreature;

		public bool _destroyIfAlreadyOwned;

		public bool _hideIfAlreadyOwned;

		public bool _positionHashSaving;

		public string _poolManagerPool = "creatures";

		public float _enableDelayMp;

		public GameObject _destroyTarget;

		public GameObject _spawnAfterPickupPrefab;

		public GameObject _sheen;

		public GameObject _myPickUp;

		public bool _mpOnly;

		public bool _mpUseRequestReply;

		public bool _mpIsFlareFromCrate;

		private int _wsToken = -1;

		private float _enableTime;

		private bool _pendingdestroythroughbolt;

		public bool Used
		{
			get;
			set;
		}

		protected virtual void Awake()
		{
			if (this._mpOnly && !BoltNetwork.isRunning)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			base.enabled = false;
			this.ResetIcons();
			if (BoltNetwork.isRunning)
			{
				this._enableTime = Time.time + this._enableDelayMp;
			}
			if (this._destroyIfAlreadyOwned || this._hideIfAlreadyOwned || this._positionHashSaving)
			{
				this.OwnershipCheckRegistration();
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.transform.root.CompareTag("Player") && ((other.transform.CompareTag("Player") && this._bodyAutoCollect) || this._grabberAutoCollect))
			{
				this.Collect();
			}
		}

		private void Update()
		{
			if (this._enableTime < Time.time && ((this._inputMethod == PickUp.InputMethods.ButtonDown && TheForest.Utils.Input.GetButtonDown("Take")) || (this._inputMethod == PickUp.InputMethods.ButtonHold && TheForest.Utils.Input.GetButtonAfterDelay("Take", 0.5f))))
			{
				this.Collect();
			}
		}

		private void OnDisabled()
		{
			if (this._destroyTarget && this._destroyTarget.transform.parent && !this._destroyTarget.transform.parent.gameObject.activeInHierarchy && !this._destroyTarget.activeSelf)
			{
				this._destroyTarget.SetActive(true);
				this.OnSpawned();
			}
		}

		private void OnDestroy()
		{
			this.OwnershipCheckUnregister();
		}

		private void OnSpawned()
		{
			this.Used = false;
			base.enabled = false;
			if (this._pendingdestroythroughbolt)
			{
				if (this._destroyTarget)
				{
					this.ToggleRenderers(this._destroyTarget.transform, true);
				}
				this._pendingdestroythroughbolt = false;
			}
			this.ResetIcons();
		}

		protected virtual void GrabEnter()
		{
			if (this._sheen)
			{
				this._sheen.SetActive(false);
			}
			if (this._myPickUp)
			{
				this._myPickUp.SetActive(true);
			}
			base.enabled = true;
		}

		protected virtual void GrabExit()
		{
			if (this._sheen)
			{
				this._sheen.SetActive(true);
			}
			if (this._myPickUp)
			{
				this._myPickUp.SetActive(false);
			}
			base.enabled = false;
		}

		private void ResetIcons()
		{
			if (this._sheen)
			{
				this._sheen.SetActive(true);
			}
			if (this._myPickUp)
			{
				this._myPickUp.SetActive(false);
			}
		}

		private void OwnershipCheckRegistration()
		{
			this._wsToken = WorkScheduler.Register(new WorkScheduler.Task(this.OwnershipCheck), base.transform.position, true);
		}

		private void OwnershipCheckUnregister()
		{
			if (this._wsToken >= 0)
			{
				WorkScheduler.Unregister(new WorkScheduler.Task(this.OwnershipCheck), this._wsToken);
				this._wsToken = -1;
			}
		}

		protected virtual void OwnershipCheck()
		{
			if (LocalPlayer.Inventory)
			{
				if (this._positionHashSaving)
				{
					if (GlobalDataSaver.GetInt(SceneUtils.PositionToLongHash(base.transform.position), 0) == 1)
					{
						this.ClearOut(false);
					}
					else
					{
						this.OwnershipCheckUnregister();
					}
				}
				else if (LocalPlayer.Inventory.Owns(this._itemId))
				{
					if (this._destroyIfAlreadyOwned && !BoltNetwork.isRunning)
					{
						this.ClearOut(false);
					}
					else
					{
						this._destroyTarget.SetActive(false);
					}
				}
				else
				{
					this._destroyTarget.SetActive(true);
				}
			}
		}

		private bool DestroyThroughBolt(bool fakeDrop)
		{
			if (BoltNetwork.isRunning)
			{
				BoltEntity componentInParent = base.GetComponentInParent<BoltEntity>();
				if (componentInParent && componentInParent.isAttached && !componentInParent.StateIs<IPlayerState>() && !componentInParent.StateIs<IMutantState>())
				{
					if (this._disableInsteadOfDestroy)
					{
						DisablePickup disablePickup;
						if (componentInParent.source == null)
						{
							disablePickup = DisablePickup.Create(GlobalTargets.OnlySelf);
						}
						else
						{
							disablePickup = DisablePickup.Create(componentInParent.source);
						}
						disablePickup.Entity = componentInParent;
						disablePickup.Num = this._destroyTarget.transform.GetSiblingIndex();
						disablePickup.Send();
						this._pendingdestroythroughbolt = true;
						return false;
					}
					DestroyPickUp destroyPickUp;
					if (componentInParent.source == null)
					{
						destroyPickUp = DestroyPickUp.Create(GlobalTargets.OnlySelf);
					}
					else
					{
						destroyPickUp = DestroyPickUp.Create(componentInParent.source);
					}
					destroyPickUp.PickUpPlayer = LocalPlayer.Entity;
					destroyPickUp.PickUpEntity = componentInParent;
					destroyPickUp.ItemId = this._itemId;
					destroyPickUp.SibblingId = ((!this._mpIsFlareFromCrate) ? -1 : this._destroyTarget.transform.GetSiblingIndex());
					destroyPickUp.FakeDrop = fakeDrop;
					destroyPickUp.Send();
					if (this._destroyTarget)
					{
						this.ToggleRenderers(this._destroyTarget.transform, false);
					}
					this._pendingdestroythroughbolt = true;
					return true;
				}
			}
			return false;
		}

		private void Collect()
		{
			if (this._pendingdestroythroughbolt)
			{
				return;
			}
			if (this.MainEffect() || this._destroyIfFull)
			{
				if (this._positionHashSaving)
				{
					GlobalDataSaver.SetInt(SceneUtils.PositionToLongHash(base.transform.position), 1);
				}
				this.CollectBonuses();
				if (this._spiderDice && UnityEngine.Random.Range(0, 4) == 2)
				{
					UnityEngine.Object.Instantiate(Resources.Load("Spider"), base.transform.position, base.transform.rotation);
				}
				if (this._spawnAfterPickupPrefab)
				{
					if (!BoltNetwork.isRunning)
					{
						UnityEngine.Object.Instantiate(this._spawnAfterPickupPrefab, base.transform.position, base.transform.rotation);
					}
					else
					{
						BoltNetwork.Instantiate(this._spawnAfterPickupPrefab, base.transform.position, base.transform.rotation);
					}
				}
				if (!this.TryPool() && !this._infinite)
				{
					this.ClearOut(false);
				}
			}
			else if (!this._preventFakeDrop)
			{
				if (this._positionHashSaving)
				{
					GlobalDataSaver.SetInt(SceneUtils.PositionToLongHash(base.transform.position), 1);
				}
				if (this._spawnAfterPickupPrefab)
				{
					if (!BoltNetwork.isRunning)
					{
						UnityEngine.Object.Instantiate(this._spawnAfterPickupPrefab, base.transform.position, base.transform.rotation);
					}
					else
					{
						BoltNetwork.Instantiate(this._spawnAfterPickupPrefab, base.transform.position, base.transform.rotation);
					}
				}
				this.ClearOut(true);
			}
		}

		protected virtual bool MainEffect()
		{
			if (LocalPlayer.Inventory.AddItem(this._itemId, this._amount, this._preventAutoEquip, false, (WeaponStatUpgrade.Types)(-2)))
			{
				if (this._forceAutoEquip)
				{
					LocalPlayer.Inventory.UnlockEquipmentSlot(Item.EquipmentSlot.RightHand);
					LocalPlayer.Inventory.Equip(this._itemId, false);
				}
				return true;
			}
			return false;
		}

		private void CollectBonuses()
		{
			if (this._bonusItems != null && this._bonusItems.Length > 0)
			{
				PickUp.LootTuple[] bonusItems = this._bonusItems;
				for (int i = 0; i < bonusItems.Length; i++)
				{
					PickUp.LootTuple lootTuple = bonusItems[i];
					if (lootTuple._itemId > 0)
					{
						LocalPlayer.Inventory.AddItem(lootTuple._itemId, lootTuple._amount, this._preventAutoEquip, false, (WeaponStatUpgrade.Types)(-2));
					}
				}
			}
		}

		protected void ClearOut(bool fakeDrop)
		{
			if (!this.DestroyThroughBolt(fakeDrop))
			{
				if (fakeDrop)
				{
					LocalPlayer.Inventory.FakeDrop(this._itemId);
				}
				if (!this._disableInsteadOfDestroy)
				{
					this._destroyTarget.transform.parent = null;
					UnityEngine.Object.Destroy(this._destroyTarget);
				}
				else
				{
					this.Used = true;
					this.GrabExit();
					this._destroyTarget.SetActive(false);
				}
			}
		}

		public bool TryPool()
		{
			if (this._poolManagerDespawnCreature && !BoltNetwork.isClient && this._destroyTarget && PoolManager.Pools[this._poolManagerPool].IsSpawned(this._destroyTarget.transform))
			{
				base.enabled = false;
				this._destroyTarget.transform.parent = null;
				if (BoltNetwork.isServer)
				{
					BoltNetwork.Detach(this._destroyTarget.gameObject);
				}
				PoolManager.Pools[this._poolManagerPool].Despawn(this._destroyTarget.transform);
				this.OwnershipCheckUnregister();
				return true;
			}
			return false;
		}

		private void ToggleRenderers(Transform t, bool enabled)
		{
			Renderer component = t.gameObject.GetComponent<Renderer>();
			if (component)
			{
				component.enabled = enabled;
			}
			foreach (Transform t2 in t)
			{
				this.ToggleRenderers(t2, enabled);
			}
		}
	}
}
