using Bolt;
using System;
using TheForest.Utils;
using TheForest.World;
using UnityEngine;

namespace TheForest.Buildings.World
{
	[DoNotSerializePublic]
	public class BuildingHealthChunk : EntityEventListener<IBuildingChunkDestructibleState>
	{
		public int _detachedLayer;

		public float _maxHP = 100f;

		public GameObject _destroyTarget;

		public GameObject _dustPrefab;

		[SerializeThis]
		private float _hp;

		private float _nextHit;

		private int _distorts;

		private bool _collapsed;

		private BuildingHealth _bh;

		private void Awake()
		{
			if (!LevelSerializer.IsDeserializing || this._hp == 0f)
			{
				this._hp = this._maxHP;
			}
		}

		private void Start()
		{
			if (!this._bh)
			{
				this._bh = base.transform.parent.GetComponentInParent<BuildingHealth>();
			}
		}

		private void OnWillDestroy()
		{
			this.LocalizedHit(new LocalizedHitData(base.transform.position, this._hp));
		}

		public void LocalizedHit(LocalizedHitData data)
		{
			if (this._nextHit < Time.realtimeSinceStartup)
			{
				this._nextHit = Time.realtimeSinceStartup + 0.5f;
				if (BoltNetwork.isRunning)
				{
					LocalizedHit localizedHit = global::LocalizedHit.Create(GlobalTargets.OnlyServer);
					localizedHit.Damage = data._damage;
					localizedHit.Position = data._position;
					localizedHit.Building = this.entity;
					if (base.transform.parent)
					{
						localizedHit.Chunk = this._bh.GetChunkIndex(this);
					}
					else
					{
						localizedHit.Chunk = -1;
					}
					localizedHit.Send();
					if (BoltNetwork.isClient)
					{
						Prefabs.Instance.SpawnWoodHitPS(data._position, Quaternion.LookRotation(base.transform.right));
						this.DistortReal(data);
					}
				}
				else
				{
					this.LocalizedHitReal(data);
				}
			}
		}

		private int Collapse(Vector3 origin)
		{
			int result = 0;
			if (BoltNetwork.isRunning)
			{
				int chunkIndex = this._bh.GetChunkIndex(this);
				base.state.ChunkCollapsePoints[chunkIndex] = origin;
			}
			else
			{
				result = this.CollapseReal(origin);
			}
			return result;
		}

		public void LocalizedHitReal(LocalizedHitData data)
		{
			if (!Cheats.NoDestruction && this._hp > 0f)
			{
				this._hp -= data._damage;
				int collapsedLogs = 0;
				Prefabs.Instance.SpawnWoodHitPS(data._position, Quaternion.LookRotation(data._position - base.transform.position));
				if (this._hp <= 0f)
				{
					collapsedLogs = this.Collapse(data._position);
				}
				else
				{
					this.Distort(data);
				}
				if (base.transform.parent)
				{
					this._bh.DamageOnly(data, collapsedLogs);
				}
			}
		}

		private void DistortRandom()
		{
		}

		private void DistortReal(LocalizedHitData data)
		{
			Renderer[] componentsInChildren = base.transform.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i].enabled)
				{
					Transform transform = componentsInChildren[i].transform;
					GameObject gameObject = transform.gameObject;
					if (Vector3.Distance(transform.position, data._position) < 4f)
					{
						transform.localRotation *= Quaternion.Euler((float)UnityEngine.Random.Range(-1, 1), (float)UnityEngine.Random.Range(-1, 1), (float)UnityEngine.Random.Range(-1, 1));
					}
				}
			}
		}

		private void Distort(LocalizedHitData data)
		{
			if (BoltNetwork.isRunning)
			{
				int chunkIndex = this._bh.GetChunkIndex(this);
				NetworkArray_Integer chunkHits;
				NetworkArray_Values<int> expr_22 = chunkHits = base.state.ChunkHits;
				int num;
				int expr_25 = num = chunkIndex;
				num = chunkHits[num];
				expr_22[expr_25] = num + 1;
			}
			else
			{
				this.DistortReal(data);
			}
		}

		private int CollapseReal(Vector3 origin)
		{
			if (base.transform.parent && base.transform.parent.parent)
			{
				LODGroup component = base.transform.parent.parent.GetComponent<LODGroup>();
				component.ForceLOD(0);
			}
			int num = 0;
			Renderer[] array = (!this._destroyTarget) ? base.GetComponentsInChildren<Renderer>() : this._destroyTarget.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < array.Length; i++)
			{
				Renderer renderer = array[i];
				GameObject gameObject = renderer.gameObject;
				if (gameObject.activeInHierarchy)
				{
					Transform transform = renderer.transform;
					transform.parent = null;
					gameObject.layer = this._detachedLayer;
					CapsuleCollider capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
					capsuleCollider.radius = 0.1f;
					capsuleCollider.height = 1f;
					capsuleCollider.direction = 2;
					Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
					if (rigidbody)
					{
						rigidbody.AddForce(((transform.position - origin).normalized + Vector3.up) * 2f, ForceMode.Impulse);
						rigidbody.AddRelativeTorque(Vector3.up * 2f, ForceMode.Impulse);
						if (gameObject.CompareTag("Log"))
						{
							num++;
						}
					}
					destroyAfter destroyAfter = gameObject.AddComponent<destroyAfter>();
					destroyAfter.destroyTime = 2f;
				}
			}
			if (this._dustPrefab)
			{
				UnityEngine.Object.Instantiate(this._dustPrefab, base.transform.position, base.transform.rotation);
			}
			UnityEngine.Object.Destroy((!this._destroyTarget) ? base.gameObject : this._destroyTarget);
			return num;
		}

		public override void Attached()
		{
			this.Start();
			int index = this._bh.GetChunkIndex(this);
			base.state.AddCallback("ChunkHits[]", delegate
			{
				while (this._distorts < this.state.ChunkHits[index])
				{
					this.DistortRandom();
					this._distorts++;
				}
			});
			base.state.AddCallback("ChunkCollapsePoints[]", delegate
			{
				if (this._collapsed)
				{
					return;
				}
				if (this.state.ChunkCollapsePoints[index] != Vector3.zero)
				{
					this._collapsed = true;
					this.CollapseReal(this.state.ChunkCollapsePoints[index]);
				}
			});
		}
	}
}
