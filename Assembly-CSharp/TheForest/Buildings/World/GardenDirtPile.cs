using Bolt;
using PathologicalGames;
using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Items.World;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Buildings.World
{
	[DoNotSerializePublic]
	public class GardenDirtPile : EntityBehaviour<IGardenDirtPileState>, IEntityReplicationFilter
	{
		public float _growthDuration = 1f;

		public GameObject _plantRenderer;

		public PickUp[] _pickups;

		[SerializeThis]
		private float _plantedTime = -1f;

		[SerializeThis]
		private int _slotNum = -1;

		[SerializeThis]
		private bool[] _usedPickups;

		public int SlotNum
		{
			get
			{
				return this._slotNum;
			}
			set
			{
				this._slotNum = value;
			}
		}

		bool IEntityReplicationFilter.AllowReplicationTo(BoltConnection connection)
		{
			return this && base.state != null && base.state.Garden && connection.ExistsOnRemote(base.state.Garden) == ExistsResult.Yes;
		}

		private void Awake()
		{
			this._usedPickups = new bool[this._pickups.Length];
			if (BoltNetwork.isClient)
			{
				this.Growth();
			}
		}

		private void OnDeserialized()
		{
			if (!BoltNetwork.isClient)
			{
				this.Init();
			}
		}

		private void OnSpawned()
		{
			if (!LevelSerializer.IsDeserializing)
			{
				this._plantedTime = Scene.Clock.ElapsedGameTime;
			}
			this.Init();
		}

		private void Init()
		{
			if (!this._plantRenderer)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			else
			{
				this._plantRenderer.transform.localScale = Vector3.zero;
				base.InvokeRepeating("Growth", 0.1f, 30f);
			}
		}

		public void Growth()
		{
			float num = Scene.Clock.ElapsedGameTime - this._plantedTime;
			float num2 = 0.3f * Mathf.Min(num / this._growthDuration, 3f);
			this._plantRenderer.transform.localScale = new Vector3(num2, num2, num2);
			int num3 = Mathf.FloorToInt(num / this._growthDuration);
			if (num3 < 5)
			{
				num3 = Mathf.Min(num3, 3);
				for (int i = 0; i < this._pickups.Length; i++)
				{
					PickUp pickUp = this._pickups[i];
					if (num3 > 0)
					{
						if (this._usedPickups[i] || pickUp.Used)
						{
							pickUp.Used = true;
							this._usedPickups[i] = true;
							if (BoltNetwork.isRunning && this.entity.isAttached && this.entity.isOwner && base.state.UsedPickups[i] == 0)
							{
								base.state.UsedPickups[i] = 1;
							}
							if (pickUp._destroyTarget.gameObject.activeSelf)
							{
								pickUp._destroyTarget.gameObject.SetActive(false);
							}
						}
						else
						{
							pickUp._amount._min = num3;
							pickUp._amount._max = num3;
							if (!pickUp.gameObject.activeSelf)
							{
								pickUp.gameObject.SetActive(true);
							}
						}
					}
					else if (pickUp.gameObject.activeSelf)
					{
						pickUp.gameObject.SetActive(false);
					}
				}
			}
			else if (!BoltNetwork.isClient)
			{
				this.Clear();
			}
		}

		private void Clear()
		{
			if (PoolManager.Pools["misc"].IsSpawned(base.transform))
			{
				base.transform.parent = null;
				PoolManager.Pools["misc"].Despawn(base.transform);
				for (int i = 0; i < this._pickups.Length; i++)
				{
					this._pickups[i].transform.parent.gameObject.SetActive(true);
				}
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		private void SetPickupUsed(int num)
		{
			for (int i = 0; i < this._pickups.Length; i++)
			{
				PickUp pickUp = this._pickups[i];
				if (pickUp._destroyTarget.transform.GetSiblingIndex() == num)
				{
					if (!pickUp.Used)
					{
						pickUp.Used = true;
						this._usedPickups[i] = true;
						if (BoltNetwork.isRunning && this.entity.isAttached && this.entity.isOwner && base.state.UsedPickups[i] == 0)
						{
							this.entity.Freeze(false);
							base.state.UsedPickups[i] = 1;
						}
						if (pickUp._destroyTarget.gameObject.activeSelf)
						{
							pickUp._destroyTarget.gameObject.SetActive(false);
						}
					}
					break;
				}
			}
		}

		public override void Attached()
		{
			if (BoltNetwork.isClient)
			{
				if (base.state.PlantedTime >= 0f)
				{
					this.InitPlantedTimeFromStateData();
				}
				else
				{
					base.state.AddCallback("PlantedTime", new PropertyCallbackSimple(this.InitPlantedTimeFromStateData));
				}
				if (base.state.SlotNum >= 0)
				{
					this.InitSlotNumFromStateData();
				}
				else
				{
					base.state.AddCallback("SlotNum", new PropertyCallbackSimple(this.InitSlotNumFromStateData));
				}
				base.state.AddCallback("UsedPickups[]", new PropertyCallbackSimple(this.RefreshUsedPickups));
				base.StartCoroutine(this.WaitForGarden());
			}
			else
			{
				base.state.PlantedTime = this._plantedTime;
				base.state.SlotNum = this._slotNum;
				base.StartCoroutine(this.WaitForGardenEntity());
			}
		}

		private void InitPlantedTimeFromStateData()
		{
			this._plantedTime = base.state.PlantedTime;
			if (this._plantedTime >= 0f && this.SlotNum >= 0)
			{
				this.Init();
			}
		}

		private void InitSlotNumFromStateData()
		{
			this.SlotNum = base.state.SlotNum;
			if (this._plantedTime >= 0f && this.SlotNum >= 0)
			{
				this.Init();
			}
		}

		private void RefreshUsedPickups()
		{
			int num = Mathf.Min(this._usedPickups.Length, base.state.UsedPickups.Length);
			for (int i = 0; i < num; i++)
			{
				if (this._usedPickups[i] != (base.state.UsedPickups[i] == 1))
				{
					PickUp pickUp = this._pickups[i];
					this._usedPickups[i] = !this._usedPickups[i];
					pickUp._destroyTarget.SetActive(!this._usedPickups[i]);
					pickUp.Used = this._usedPickups[i];
				}
			}
		}

		[DebuggerHidden]
		private IEnumerator WaitForGarden()
		{
			GardenDirtPile.<WaitForGarden>c__Iterator151 <WaitForGarden>c__Iterator = new GardenDirtPile.<WaitForGarden>c__Iterator151();
			<WaitForGarden>c__Iterator.<>f__this = this;
			return <WaitForGarden>c__Iterator;
		}

		[DebuggerHidden]
		private IEnumerator WaitForGardenEntity()
		{
			GardenDirtPile.<WaitForGardenEntity>c__Iterator152 <WaitForGardenEntity>c__Iterator = new GardenDirtPile.<WaitForGardenEntity>c__Iterator152();
			<WaitForGardenEntity>c__Iterator.<>f__this = this;
			return <WaitForGardenEntity>c__Iterator;
		}
	}
}
