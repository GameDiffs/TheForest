using Bolt;
using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Items.Special
{
	[DoNotSerializePublic]
	public class LogControler : EntityBehaviour<IPlayerState>
	{
		[ItemIdPicker]
		public int _logItemId;

		[ItemIdPicker]
		public int _lighterItemId;

		public PlayerInventory _player;

		public GameObject[] _logsHeld;

		public GameObject _logPrefab;

		[SerializeThis]
		private int _logs;

		private Item _itemCache;

		private bool _inDark;

		private bool _lightOff;

		public int Amount
		{
			get
			{
				return this._logs;
			}
		}

		public bool HasLogs
		{
			get
			{
				return this._logs > 0;
			}
		}

		private void Start()
		{
			this._player.Logs = this;
			this._itemCache = ItemDatabase.ItemById(this._logItemId);
			base.enabled = false;
		}

		private void Update()
		{
			if (TheForest.Utils.Input.GetButtonDown("Drop"))
			{
				this.PutDown(false, true, true);
			}
		}

		[DebuggerHidden]
		private IEnumerator OnDeserialized()
		{
			LogControler.<OnDeserialized>c__Iterator16E <OnDeserialized>c__Iterator16E = new LogControler.<OnDeserialized>c__Iterator16E();
			<OnDeserialized>c__Iterator16E.<>f__this = this;
			return <OnDeserialized>c__Iterator16E;
		}

		public bool Lift()
		{
			if (this._logs < 2 && LocalPlayer.FpCharacter.Grounded && !LocalPlayer.FpCharacter.PushingSled && !LocalPlayer.FpCharacter.SailingRaft)
			{
				this._logs++;
				this._logsHeld[this._logs - 1].SetActive(true);
				LocalPlayer.Sfx.PlayWhoosh();
				if (this._logs == 1)
				{
					if (!this._player.HasInSlot(Item.EquipmentSlot.LeftHand, this._lighterItemId))
					{
						this._player.MemorizeItem(Item.EquipmentSlot.LeftHand);
						this._player.UnequipItemAtSlot(Item.EquipmentSlot.LeftHand, false, true, false);
					}
					this._player.MemorizeItem(Item.EquipmentSlot.RightHand);
					this._player.UnequipItemAtSlot(Item.EquipmentSlot.RightHand, false, true, false);
					for (int i = 0; i < this._itemCache._equipedAnimVars.Length; i++)
					{
						LocalPlayer.Animator.SetBoolReflected(this._itemCache._equipedAnimVars[i].ToString(), true);
					}
					base.enabled = true;
				}
				this.UpdateLogCount();
				return true;
			}
			this.UpdateLogCount();
			return false;
		}

		public void RemoveLog(bool equipPrevious)
		{
			if (this._logs > 0)
			{
				this._logs--;
				this._logsHeld[this._logs].SetActive(false);
				if (this._logs == 0)
				{
					for (int i = 0; i < this._itemCache._equipedAnimVars.Length; i++)
					{
						LocalPlayer.Animator.SetBoolReflected(this._itemCache._equipedAnimVars[i].ToString(), false);
					}
					if (equipPrevious)
					{
						if (!this._player.HasInSlot(Item.EquipmentSlot.LeftHand, this._lighterItemId))
						{
							this._player.EquipPreviousUtility();
						}
						this._player.EquipPreviousWeaponDelayed();
					}
					base.enabled = false;
				}
			}
		}

		public bool PutDown(bool fake = false, bool drop = false, bool equipPrevious = true)
		{
			if (!fake)
			{
				if (this._logs <= 0)
				{
					return false;
				}
				this.RemoveLog(equipPrevious);
			}
			if (drop)
			{
				Transform transform = this._logsHeld[Mathf.Min(this._logs, 1)].transform;
				if (BoltNetwork.isRunning)
				{
					DropItem dropItem = DropItem.Raise(GlobalTargets.OnlyServer);
					dropItem.PrefabId = BoltPrefabs.Log;
					dropItem.Position = transform.position;
					dropItem.Rotation = transform.rotation;
					dropItem.Send();
				}
				else
				{
					UnityEngine.Object.Instantiate(this._logPrefab, transform.position, transform.rotation);
				}
				FMODCommon.PlayOneshotNetworked("event:/player/foley/log_drop_exert", transform, FMODCommon.NetworkRole.Any);
			}
			this.UpdateLogCount();
			return true;
		}

		private void UpdateLogCount()
		{
			if (BoltNetwork.isRunning)
			{
				int num = 0;
				for (int i = 0; i < this._logsHeld.Length; i++)
				{
					if (this._logsHeld[i] && this._logsHeld[i].activeInHierarchy)
					{
						num++;
					}
				}
				base.state.logsHeld = num;
			}
		}
	}
}
