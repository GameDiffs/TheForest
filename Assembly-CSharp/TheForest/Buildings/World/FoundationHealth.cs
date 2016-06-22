using Bolt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TheForest.Buildings.Creation;
using TheForest.Buildings.Interfaces;
using TheForest.Utils;
using TheForest.World;
using UniLinq;
using UnityEngine;

namespace TheForest.Buildings.World
{
	[DoNotSerializePublic]
	public class FoundationHealth : EntityBehaviour<IFoundationState>, IRepairableStructure
	{
		[Serializable]
		public class Chunk
		{
			[Serializable]
			public class Tier
			{
				public float _hp;
			}

			public FoundationHealth.Chunk.Tier[] _tiers;
		}

		public int _detachedLayer;

		public FoundationArchitect _foundation;

		public float _perLogBonusHP = 2f;

		public float _chunkTierHP = 300f;

		public float _collapseFallDuration = 2f;

		public Create.BuildingTypes _type;

		[SerializeThis]
		private FoundationHealth.Chunk[] _chunks;

		[SerializeThis]
		private int _collapsedLogs;

		[SerializeThis]
		private int _repairMaterial;

		[SerializeThis]
		private int _repairLogs;

		private int _bonusHp;

		private bool _collapsing;

		private float _lastHit;

		private BuildingRepair _repairTrigger;

		public int RepairMaterial
		{
			get
			{
				if (BoltNetwork.isRunning && this.entity && this.entity.isAttached)
				{
					return base.state.repairMaterial;
				}
				return this._repairMaterial;
			}
			set
			{
				if (BoltNetwork.isRunning && this.entity && this.entity.isAttached && this.entity.isOwner)
				{
					base.state.repairMaterial = value;
				}
				this._repairMaterial = value;
			}
		}

		public int RepairLogs
		{
			get
			{
				if (BoltNetwork.isRunning && this.entity && this.entity.isAttached)
				{
					return base.state.repairLogs;
				}
				return this._repairLogs;
			}
			set
			{
				if (BoltNetwork.isRunning && this.entity && this.entity.isAttached && this.entity.isOwner)
				{
					base.state.repairLogs = value;
				}
				this._repairLogs = value;
			}
		}

		public int CollapsedLogs
		{
			get
			{
				return this._collapsedLogs / 3;
			}
		}

		public bool Collapsing
		{
			get
			{
				return this._collapsing;
			}
		}

		private void Awake()
		{
			this._foundation.SegmentTierValidation = new FoundationArchitect.SegmentTierValidator(this.ChunkTierValidation);
			base.StartCoroutine(this.DelayedAwake());
		}

		[DebuggerHidden]
		private IEnumerator DelayedAwake()
		{
			FoundationHealth.<DelayedAwake>c__Iterator14F <DelayedAwake>c__Iterator14F = new FoundationHealth.<DelayedAwake>c__Iterator14F();
			<DelayedAwake>c__Iterator14F.<>f__this = this;
			return <DelayedAwake>c__Iterator14F;
		}

		private void OnDestroy()
		{
			if (this._repairTrigger)
			{
				UnityEngine.Object.Destroy(this._repairTrigger.gameObject);
			}
		}

		public void TierDestroyed(FoundationChunkTier tier, Vector3 position)
		{
			if (!this._collapsing && !BoltNetwork.isClient)
			{
				this.TierDestroyed_Actual(tier);
			}
		}

		public void LocalizedTierHit(LocalizedHitData data, FoundationChunkTier tier)
		{
			if (!Cheats.NoDestruction && !BoltNetwork.isClient)
			{
				this._lastHit = Time.realtimeSinceStartup;
				FoundationArchitect.Edge edge = this._foundation.Edges[tier._edgeNum];
				int num = this._foundation.Edges.Take(tier._edgeNum).Sum((FoundationArchitect.Edge e) => e._segments.Length);
				int num2 = tier._segmentNum + num;
				this._chunks[num2]._tiers[tier._tierNum]._hp -= data._damage;
				float hp = this._chunks[num2]._tiers[tier._tierNum]._hp;
				if (hp <= 0f)
				{
					if (tier.transform.parent)
					{
						tier.transform.parent = null;
						Renderer[] componentsInChildren = tier.GetComponentsInChildren<Renderer>();
						for (int i = 0; i < componentsInChildren.Length; i++)
						{
							Transform transform = componentsInChildren[i].transform;
							GameObject gameObject = transform.gameObject;
							transform.parent = null;
							gameObject.layer = this._detachedLayer;
							CapsuleCollider capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
							capsuleCollider.radius = 0.5f;
							capsuleCollider.height = 4.5f;
							capsuleCollider.direction = 2;
							Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
							if (rigidbody)
							{
								rigidbody.AddForce((transform.position - data._position).normalized * 2f, ForceMode.Impulse);
								rigidbody.AddRelativeTorque(Vector3.up * 2f, ForceMode.Impulse);
							}
							destroyAfter destroyAfter = gameObject.AddComponent<destroyAfter>();
							destroyAfter.destroyTime = 1.75f;
							this._collapsedLogs++;
						}
						LocalPlayer.Sfx.PlayStructureBreak(base.gameObject, 0.2f);
						UnityEngine.Object.Destroy(tier.gameObject);
						if (BoltNetwork.isServer)
						{
							this.PublishDestroyedTierData(this.LightWeightExport());
						}
						this.IntegrityCheck(num2, tier._tierNum);
					}
				}
				else
				{
					this.Distort(data);
				}
				if (!this._collapsing)
				{
					this.CheckSpawnRepairTrigger();
				}
				Prefabs.Instance.SpawnWoodHitPS(data._position, Quaternion.LookRotation(tier.transform.right));
			}
		}

		private void Collapse(int collapseChunkNum)
		{
			if (!this._collapsing)
			{
				int index = 0;
				while (collapseChunkNum >= this._foundation.Edges[index]._segments.Length)
				{
					collapseChunkNum -= this._foundation.Edges[index++]._segments.Length;
				}
				Vector3 p = this._foundation.Edges[index]._segments[collapseChunkNum]._p1;
				this.Collapse(p);
			}
		}

		public void Collapse(Vector3 collapsePoint)
		{
			if (BoltNetwork.isRunning)
			{
				if (this.entity.isAttached && this.entity.isOwner && base.state != null)
				{
					base.state.BuildingCollapsePoint = collapsePoint;
				}
			}
			else if (base.gameObject.activeInHierarchy)
			{
				base.StartCoroutine(this.CollapseRoutine(collapsePoint));
			}
		}

		public int CalcMissingRepairMaterial()
		{
			return Mathf.Max(this.CalcTotalRepairMaterial() - this.RepairMaterial, 0);
		}

		public int CalcTotalRepairMaterial()
		{
			int num = 3;
			int num2 = 3;
			float num3 = 0f;
			if (!BoltNetwork.isClient)
			{
				for (int i = 0; i < this._chunks.Length; i++)
				{
					for (int j = 0; j < 3; j++)
					{
						num3 += this._chunkTierHP + (float)this._bonusHp - Mathf.Max(this._chunks[i]._tiers[j]._hp, 0f);
					}
				}
				if (num3 > 0f)
				{
					int num4 = num + Mathf.RoundToInt(num3 / 500f * (float)num2);
					if (BoltNetwork.isServer && this.entity && this.entity.isAttached)
					{
						base.state.hp = (float)num4;
					}
					return num4;
				}
				if (BoltNetwork.isServer && this.entity && this.entity.isAttached)
				{
					base.state.hp = 0f;
				}
			}
			else if (BoltNetwork.isRunning && this.entity && this.entity.isAttached)
			{
				return (int)base.state.hp;
			}
			return 0;
		}

		public int CalcMissingRepairLogs()
		{
			return Mathf.Max(this._collapsedLogs / 3 - this.RepairLogs, 0);
		}

		public void AddRepairMaterial(bool isLog)
		{
			if (BoltNetwork.isRunning)
			{
				AddRepairMaterial addRepairMaterial = global::AddRepairMaterial.Create(GlobalTargets.OnlyServer);
				addRepairMaterial.Building = this.entity;
				addRepairMaterial.IsLog = isLog;
				addRepairMaterial.Send();
			}
			else
			{
				this.AddRepairMaterialReal(isLog);
			}
		}

		public void AddRepairMaterialReal(bool isLog)
		{
			if (this._foundation._mode != FoundationArchitect.Modes.Auto)
			{
				if (isLog)
				{
					this.RepairLogs++;
				}
				else
				{
					this.RepairMaterial++;
				}
				if (this.CalcMissingRepairMaterial() == 0 && this.CalcMissingRepairLogs() == 0)
				{
					this.ResetHP();
					if (!BoltNetwork.isClient)
					{
						this.PublishDestroyedTierData(this.LightWeightExport());
						this.RespawnFoundation(false);
					}
					GameStats.RepairedStructure.Invoke();
				}
			}
		}

		public void ResetHP()
		{
			for (int i = 0; i < this._chunks.Length; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					this._chunks[i]._tiers[j]._hp = this._chunkTierHP + (float)this._bonusHp;
				}
			}
		}

		public void RespawnFoundation(bool respawnOnly)
		{
			if (BoltNetwork.isServer && !respawnOnly)
			{
				base.state.repairTrigger = false;
			}
			if (this._type != Create.BuildingTypes.None)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(LocalPlayer.Create._blueprints.Find((Create.BuildingBlueprint bp) => bp._type == this._type)._builtPrefab);
				gameObject.transform.position = base.transform.position;
				gameObject.transform.rotation = base.transform.rotation;
				for (int i = base.transform.childCount - 1; i >= 0; i--)
				{
					Transform child = base.transform.GetChild(i);
					if (!child.GetComponent<PrefabIdentifier>() && !child.name.Contains("Anchor"))
					{
						UnityEngine.Object.Destroy(child.gameObject);
					}
				}
				for (int j = gameObject.transform.childCount - 1; j >= 0; j--)
				{
					Transform child2 = gameObject.transform.GetChild(j);
					if (!child2.name.Contains("Anchor"))
					{
						child2.parent = base.transform;
					}
				}
				UnityEngine.Object.Destroy(gameObject);
			}
			base.SendMessage("CreateStructure", true);
			if (!respawnOnly)
			{
				this.RepairMaterial = 0;
				this.RepairLogs = 0;
				this._collapsedLogs = 0;
				if (this._repairTrigger)
				{
					UnityEngine.Object.Destroy(this._repairTrigger.gameObject);
					this._repairTrigger = null;
				}
			}
		}

		private void Initialize()
		{
			if (this._foundation.Edges != null)
			{
				this._bonusHp = Mathf.RoundToInt((float)this._foundation.Edges.Sum((FoundationArchitect.Edge e) => e._totalLogs) * this._perLogBonusHP);
			}
			if (this._chunks == null || this._chunks.Length == 0)
			{
				this._chunks = new FoundationHealth.Chunk[this._foundation.ChunkCount];
				for (int i = 0; i < this._chunks.Length; i++)
				{
					this._chunks[i] = new FoundationHealth.Chunk
					{
						_tiers = new FoundationHealth.Chunk.Tier[3]
					};
					for (int j = 0; j < 3; j++)
					{
						this._chunks[i]._tiers[j] = new FoundationHealth.Chunk.Tier
						{
							_hp = this._chunkTierHP + (float)this._bonusHp
						};
					}
				}
			}
			else
			{
				this.CheckSpawnRepairTrigger();
			}
		}

		private bool ChunkTierValidation(int chunkNum, int tierNum)
		{
			return this._chunks == null || this._chunks.Length == 0 || this._chunks[chunkNum]._tiers[tierNum]._hp > 0f;
		}

		public void TierDestroyed_Actual(FoundationChunkTier tier)
		{
			if (!Cheats.NoDestruction)
			{
				try
				{
					if (!this._collapsing)
					{
						int num = tier._segmentNum + this._foundation.Edges.Take(tier._edgeNum).Sum((FoundationArchitect.Edge e) => e._segments.Length);
						this._chunks[num]._tiers[tier._tierNum]._hp = -42f;
						Transform exists = this._foundation.FoundationRoot.transform.FindChild("Edge" + tier._edgeNum);
						if (exists)
						{
							Renderer[] componentsInChildren = tier.GetComponentsInChildren<Renderer>();
							for (int i = 0; i < componentsInChildren.Length; i++)
							{
								Transform transform = componentsInChildren[i].transform;
								GameObject gameObject = transform.gameObject;
								transform.parent = null;
								gameObject.layer = this._detachedLayer;
								CapsuleCollider capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
								capsuleCollider.radius = 0.5f;
								capsuleCollider.height = 4.5f;
								capsuleCollider.direction = 2;
								Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
								if (rigidbody)
								{
									rigidbody.AddForce((transform.position - tier.transform.position).normalized * 2.5f, ForceMode.Impulse);
									rigidbody.AddRelativeTorque(Vector3.up * 2f, ForceMode.Impulse);
								}
								destroyAfter destroyAfter = gameObject.AddComponent<destroyAfter>();
								destroyAfter.destroyTime = 1.75f;
								this._collapsedLogs++;
							}
						}
						if (BoltNetwork.isServer)
						{
							this.PublishDestroyedTierData(this.LightWeightExport());
						}
						this.IntegrityCheck(num, tier._tierNum);
						if (!this._collapsing)
						{
							this.CheckSpawnRepairTrigger();
						}
					}
				}
				catch (Exception exception)
				{
					UnityEngine.Debug.LogException(exception);
				}
			}
		}

		public void Distort(LocalizedHitData data)
		{
			if (data._damage > 0f)
			{
				Renderer[] componentsInChildren = base.transform.GetComponentsInChildren<Renderer>();
				float num = Mathf.Clamp(data._damage * data._distortRatio * 10f / (this._chunkTierHP + (float)this._bonusHp), 1f, 10f);
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					if (componentsInChildren[i].enabled)
					{
						Transform transform = componentsInChildren[i].transform;
						GameObject gameObject = transform.gameObject;
						if (Vector3.Distance(componentsInChildren[i].bounds.center, data._position) < 12f)
						{
							transform.localRotation *= Quaternion.Euler(UnityEngine.Random.Range(-0.6f, 0.6f) * num, UnityEngine.Random.Range(-0.6f, 0.6f) * num, UnityEngine.Random.Range(-0.6f, 0.6f) * num);
						}
					}
				}
			}
		}

		private void IntegrityCheck(int brokenChunkNum, int brokenTierNum)
		{
			int num = brokenChunkNum - 1;
			int num2 = brokenChunkNum + 1;
			for (int i = 0; i < this._chunks.Length; i++)
			{
				int num3 = (num + this._chunks.Length * 2) % this._chunks.Length;
				if (this._chunks[num3]._tiers[brokenTierNum]._hp > 0f && ((brokenTierNum == 1 && (this._chunks[num3]._tiers[0]._hp > 0f || this._chunks[num3]._tiers[2]._hp > 0f)) || ((brokenTierNum == 0 || brokenTierNum == 2) && this._chunks[num3]._tiers[1]._hp > 0f)))
				{
					break;
				}
				num--;
				if ((float)(Mathf.Abs(num - num2) - 1) > (float)this._chunks.Length * 0.25f)
				{
					this.Collapse((int)Mathf.Repeat((float)(num + Mathf.RoundToInt((float)Mathf.Abs(num - num2) / 2f)), (float)this._chunks.Length));
					return;
				}
			}
			for (int j = 0; j < this._chunks.Length; j++)
			{
				int num4 = num2 % this._chunks.Length;
				if (this._chunks[num4]._tiers[brokenTierNum]._hp > 0f && ((brokenTierNum == 1 && (this._chunks[num4]._tiers[0]._hp > 0f || this._chunks[num4]._tiers[2]._hp > 0f)) || ((brokenTierNum == 0 || brokenTierNum == 2) && this._chunks[num4]._tiers[1]._hp > 0f)))
				{
					break;
				}
				num2++;
				if ((float)(Mathf.Abs(num - num2) - 1) > (float)this._chunks.Length * 0.25f)
				{
					this.Collapse((int)Mathf.Repeat((float)(num2 + Mathf.RoundToInt((float)Mathf.Abs(num - num2) / 2f)), (float)this._chunks.Length));
					return;
				}
			}
			if ((float)(Mathf.Abs(num - num2) - 1) > (float)this._chunks.Length * 0.25f)
			{
				this.Collapse((int)Mathf.Repeat((float)(num + Mathf.RoundToInt((float)Mathf.Abs(num - num2) / 2f)), (float)this._chunks.Length));
				return;
			}
		}

		[DebuggerHidden]
		private IEnumerator CollapseRoutine(Vector3 collapsePoint)
		{
			FoundationHealth.<CollapseRoutine>c__Iterator150 <CollapseRoutine>c__Iterator = new FoundationHealth.<CollapseRoutine>c__Iterator150();
			<CollapseRoutine>c__Iterator.collapsePoint = collapsePoint;
			<CollapseRoutine>c__Iterator.<$>collapsePoint = collapsePoint;
			<CollapseRoutine>c__Iterator.<>f__this = this;
			return <CollapseRoutine>c__Iterator;
		}

		private Vector3 GetCenterPosition()
		{
			List<Vector3> multiPointsPositions = this._foundation.MultiPointsPositions;
			float num = 0f;
			float num2 = 0f;
			foreach (Vector3 current in multiPointsPositions)
			{
				num += current.x;
				num2 += current.z;
			}
			float x = num / (float)multiPointsPositions.Count;
			float z = num2 / (float)multiPointsPositions.Count;
			return new Vector3(x, this._foundation.MultiPointsPositions[0].y, z);
		}

		private void CheckSpawnRepairTrigger()
		{
			if (!this._repairTrigger && this._foundation._mode != FoundationArchitect.Modes.Auto && (this.CalcMissingRepairMaterial() > 0 || this.CalcMissingRepairLogs() > 0))
			{
				this.SpawnRepairTrigger();
				if (BoltNetwork.isServer && this.entity.isAttached)
				{
					base.state.repairTrigger = true;
				}
			}
		}

		private void SpawnRepairTrigger()
		{
			if (this._foundation._mode != FoundationArchitect.Modes.Auto && !this._repairTrigger)
			{
				this._repairTrigger = UnityEngine.Object.Instantiate<BuildingRepair>(Prefabs.Instance.BuildingRepairTriggerPrefab);
				this._repairTrigger._target = this;
				this._repairTrigger.transform.position = this.GetCenterPosition() + Vector3.one;
			}
		}

		public override void Attached()
		{
			base.state.AddCallback("BuildingCollapsePoint", delegate
			{
				base.StartCoroutine(this.CollapseRoutine(base.state.BuildingCollapsePoint));
			});
			if (BoltNetwork.isClient)
			{
				base.state.AddCallback("DestroyedTiers[]", new PropertyCallbackSimple(this.ReceivedDestroyedTierData));
				if (this._foundation._mode != FoundationArchitect.Modes.Auto)
				{
					base.state.AddCallback("repairTrigger", delegate
					{
						if (base.state.repairTrigger)
						{
							this.SpawnRepairTrigger();
						}
						else if (this._repairTrigger)
						{
							this.RespawnFoundation(false);
							UnityEngine.Object.Destroy(this._repairTrigger);
							this._repairTrigger = null;
						}
					});
				}
			}
			if (BoltNetwork.isServer && this._repairTrigger)
			{
				this.CalcTotalRepairMaterial();
				base.state.repairTrigger = true;
			}
		}

		public override void Detached()
		{
			for (int i = base.transform.childCount - 1; i >= 0; i--)
			{
				Transform child = base.transform.GetChild(i);
				if (child.GetComponent<BoltEntity>())
				{
					child.parent = null;
				}
			}
		}

		public int[] LightWeightExport()
		{
			int num = Mathf.Min(this._chunks.Length * 3, 320);
			int[] array = new int[Mathf.CeilToInt((float)num / 32f)];
			for (int i = 0; i < num; i++)
			{
				if (this._chunks[i / 3]._tiers[i % 3]._hp > 0f)
				{
					array[i / 32] |= 1 << i % 32;
				}
			}
			return array;
		}

		public void ImportLightWeight(int[] data)
		{
			bool flag = false;
			int num = Mathf.Min(this._chunks.Length * 3, 320);
			for (int i = 0; i < num; i++)
			{
				bool flag2 = this._chunks[i / 3]._tiers[i % 3]._hp > 0f;
				bool flag3 = (data[i / 32] & 1 << i % 32) != 0;
				if (flag2 != flag3)
				{
					this._chunks[i / 3]._tiers[i % 3]._hp = ((!flag3) ? 0f : (this._chunkTierHP + (float)this._bonusHp));
					if (!flag3)
					{
						FoundationChunkTier chunk = this._foundation.GetChunk(i);
						if (chunk)
						{
							this.TierDestroyed_Actual(chunk);
						}
					}
					else
					{
						flag = true;
					}
				}
			}
			if (flag)
			{
				this.RespawnFoundation(true);
			}
		}

		private void PublishDestroyedTierData(int[] data)
		{
			if (this.entity.isAttached)
			{
				for (int i = 0; i < data.Length; i++)
				{
					if (base.state.DestroyedTiers[i] != data[i])
					{
						base.state.DestroyedTiers[i] = data[i];
					}
				}
			}
		}

		public void ReceivedDestroyedTierData()
		{
			for (int i = 0; i < base.state.DestroyedTiers.Length; i++)
			{
				this.ImportLightWeight(base.state.DestroyedTiers.ToArray<int>());
			}
		}
	}
}
