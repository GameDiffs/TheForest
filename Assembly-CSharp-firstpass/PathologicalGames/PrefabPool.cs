using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace PathologicalGames
{
	[Serializable]
	public class PrefabPool
	{
		public Transform prefab;

		internal GameObject prefabGO;

		public int preloadAmount = 1;

		public bool preloadTime;

		public int preloadFrames = 2;

		public float preloadDelay;

		public bool limitInstances;

		public int limitAmount = 100;

		public bool limitFIFO;

		public bool cullDespawned;

		public int cullAbove = 50;

		public int cullDelay = 60;

		public int cullMaxPerPass = 5;

		public bool _logMessages;

		private bool forceLoggingSilent;

		public SpawnPool spawnPool;

		private bool cullingActive;

		internal List<Transform> _spawned = new List<Transform>();

		internal List<Transform> _despawned = new List<Transform>();

		internal int _delayedDespawnedCount;

		private bool _preloaded;

		private WaitForSeconds delayedDespawn = new WaitForSeconds(1f);

		public bool logMessages
		{
			get
			{
				if (this.forceLoggingSilent)
				{
					return false;
				}
				if (this.spawnPool.logMessages)
				{
					return this.spawnPool.logMessages;
				}
				return this._logMessages;
			}
		}

		public List<Transform> spawned
		{
			get
			{
				return new List<Transform>(this._spawned);
			}
		}

		public List<Transform> despawned
		{
			get
			{
				return new List<Transform>(this._despawned);
			}
		}

		public int totalCount
		{
			get
			{
				int num = 0;
				num += this._spawned.Count;
				return num + (this._despawned.Count + this._delayedDespawnedCount);
			}
		}

		internal bool preloaded
		{
			get
			{
				return this._preloaded;
			}
			private set
			{
				this._preloaded = value;
			}
		}

		public PrefabPool(Transform prefab)
		{
			this.prefab = prefab;
			this.prefabGO = prefab.gameObject;
		}

		public PrefabPool()
		{
		}

		internal void inspectorInstanceConstructor()
		{
			this.prefabGO = this.prefab.gameObject;
			this._spawned = new List<Transform>();
			this._despawned = new List<Transform>();
		}

		internal void SelfDestruct()
		{
			this.prefab = null;
			this.prefabGO = null;
			this.spawnPool = null;
			foreach (Transform current in this._despawned)
			{
				if (current != null)
				{
					UnityEngine.Object.Destroy(current.gameObject);
				}
			}
			foreach (Transform current2 in this._spawned)
			{
				if (current2 != null)
				{
					UnityEngine.Object.Destroy(current2.gameObject);
				}
			}
			this._spawned.Clear();
			this._despawned.Clear();
		}

		internal bool DespawnInstance(Transform xform)
		{
			return this.DespawnInstance(xform, true);
		}

		internal bool DespawnInstance(Transform xform, bool sendEventMessage)
		{
			if (this.logMessages)
			{
				UnityEngine.Debug.Log(string.Format("SpawnPool {0} ({1}): Despawning '{2}'", this.spawnPool.poolName, this.prefab.name, xform.name));
			}
			this._spawned.Remove(xform);
			this.spawnPool.NextDisableTr = xform;
			if (sendEventMessage)
			{
				xform.gameObject.BroadcastMessage("OnDespawned", this.spawnPool, SendMessageOptions.DontRequireReceiver);
			}
			if (this.spawnPool.DelayNextDisable)
			{
				this.spawnPool.DelayNextDisable = false;
				this.spawnPool.StartCoroutine(this.DelayedlDespawned(xform));
			}
			else
			{
				this._despawned.Add(xform);
				PoolManagerUtils.SetActive(xform.gameObject, false);
			}
			if (!this.cullingActive && this.cullDespawned && this.totalCount > this.cullAbove)
			{
				this.cullingActive = true;
				this.spawnPool.StartCoroutine(this.CullDespawned());
			}
			return true;
		}

		[DebuggerHidden]
		internal IEnumerator DelayedlDespawned(Transform xform)
		{
			PrefabPool.<DelayedlDespawned>c__Iterator9 <DelayedlDespawned>c__Iterator = new PrefabPool.<DelayedlDespawned>c__Iterator9();
			<DelayedlDespawned>c__Iterator.xform = xform;
			<DelayedlDespawned>c__Iterator.<$>xform = xform;
			<DelayedlDespawned>c__Iterator.<>f__this = this;
			return <DelayedlDespawned>c__Iterator;
		}

		[DebuggerHidden]
		internal IEnumerator CullDespawned()
		{
			PrefabPool.<CullDespawned>c__IteratorA <CullDespawned>c__IteratorA = new PrefabPool.<CullDespawned>c__IteratorA();
			<CullDespawned>c__IteratorA.<>f__this = this;
			return <CullDespawned>c__IteratorA;
		}

		internal Transform SpawnInstance(Vector3 pos, Quaternion rot)
		{
			if (this.limitInstances && this.limitFIFO && this._spawned.Count >= this.limitAmount)
			{
				Transform transform = this._spawned[0];
				if (this.logMessages)
				{
					UnityEngine.Debug.Log(string.Format("SpawnPool {0} ({1}): LIMIT REACHED! FIFO=True. Calling despawning for {2}...", this.spawnPool.poolName, this.prefab.name, transform));
				}
				this.DespawnInstance(transform);
				this.spawnPool._spawned.Remove(transform);
			}
			Transform transform2;
			if (this._despawned.Count == 0)
			{
				transform2 = this.SpawnNew(pos, rot);
			}
			else
			{
				transform2 = this._despawned[0];
				this._despawned.RemoveAt(0);
				this._spawned.Add(transform2);
				if (transform2 == null)
				{
					string message = "Make sure you didn't delete a despawned instance directly.";
					throw new MissingReferenceException(message);
				}
				if (this.logMessages)
				{
					UnityEngine.Debug.Log(string.Format("SpawnPool {0} ({1}): respawning '{2}'.", this.spawnPool.poolName, this.prefab.name, transform2.name));
				}
				transform2.position = pos;
				transform2.rotation = rot;
				PoolManagerUtils.SetActive(transform2.gameObject, true);
			}
			return transform2;
		}

		public Transform SpawnNew()
		{
			return this.SpawnNew(Vector3.zero, Quaternion.identity);
		}

		public Transform SpawnNew(Vector3 pos, Quaternion rot)
		{
			if (this.limitInstances && this.totalCount >= this.limitAmount)
			{
				if (this.logMessages)
				{
					UnityEngine.Debug.Log(string.Format("SpawnPool {0} ({1}): LIMIT REACHED! Not creating new instances! (Returning null)", this.spawnPool.poolName, this.prefab.name));
				}
				return null;
			}
			if (pos == Vector3.zero)
			{
				pos = this.spawnPool.group.position;
			}
			if (rot == Quaternion.identity)
			{
				rot = this.spawnPool.group.rotation;
			}
			Transform transform = (Transform)UnityEngine.Object.Instantiate(this.prefab, pos, rot);
			this.nameInstance(transform);
			if (!this.spawnPool.dontReparent)
			{
				transform.parent = this.spawnPool.group;
			}
			if (this.spawnPool.matchPoolScale)
			{
				transform.localScale = Vector3.one;
			}
			if (this.spawnPool.matchPoolLayer)
			{
				this.SetRecursively(transform, this.spawnPool.gameObject.layer);
			}
			this._spawned.Add(transform);
			if (this.logMessages)
			{
				UnityEngine.Debug.Log(string.Format("SpawnPool {0} ({1}): Spawned new instance '{2}'.", this.spawnPool.poolName, this.prefab.name, transform.name));
			}
			return transform;
		}

		private void SetRecursively(Transform xform, int layer)
		{
			xform.gameObject.layer = layer;
			foreach (Transform xform2 in xform)
			{
				this.SetRecursively(xform2, layer);
			}
		}

		internal void AddUnpooled(Transform inst, bool despawn)
		{
			this.nameInstance(inst);
			if (despawn)
			{
				PoolManagerUtils.SetActive(inst.gameObject, false);
				this._despawned.Add(inst);
			}
			else
			{
				this._spawned.Add(inst);
			}
		}

		internal void PreloadInstances()
		{
			if (this.preloaded)
			{
				UnityEngine.Debug.Log(string.Format("SpawnPool {0} ({1}): Already preloaded! You cannot preload twice. If you are running this through code, make sure it isn't also defined in the Inspector.", this.spawnPool.poolName, this.prefab.name));
				return;
			}
			if (this.prefab == null)
			{
				UnityEngine.Debug.LogError(string.Format("SpawnPool {0} ({1}): Prefab cannot be null.", this.spawnPool.poolName, this.prefab.name));
				return;
			}
			if (this.limitInstances && this.preloadAmount > this.limitAmount)
			{
				UnityEngine.Debug.LogWarning(string.Format("SpawnPool {0} ({1}): You turned ON 'Limit Instances' and entered a 'Limit Amount' greater than the 'Preload Amount'! Setting preload amount to limit amount.", this.spawnPool.poolName, this.prefab.name));
				this.preloadAmount = this.limitAmount;
			}
			if (this.cullDespawned && this.preloadAmount > this.cullAbove)
			{
				UnityEngine.Debug.LogWarning(string.Format("SpawnPool {0} ({1}): You turned ON Culling and entered a 'Cull Above' threshold greater than the 'Preload Amount'! This will cause the culling feature to trigger immediatly, which is wrong conceptually. Only use culling for extreme situations. See the docs.", this.spawnPool.poolName, this.prefab.name));
			}
			if (this.preloadTime)
			{
				if (this.preloadFrames > this.preloadAmount)
				{
					UnityEngine.Debug.LogWarning(string.Format("SpawnPool {0} ({1}): Preloading over-time is on but the frame duration is greater than the number of instances to preload. The minimum spawned per frame is 1, so the maximum time is the same as the number of instances. Changing the preloadFrames value...", this.spawnPool.poolName, this.prefab.name));
					this.preloadFrames = this.preloadAmount;
				}
				this.spawnPool.StartCoroutine(this.PreloadOverTime());
			}
			else
			{
				this.forceLoggingSilent = true;
				while (this.totalCount < this.preloadAmount)
				{
					Transform xform = this.SpawnNew();
					this.DespawnInstance(xform, false);
				}
				this.forceLoggingSilent = false;
			}
		}

		[DebuggerHidden]
		private IEnumerator PreloadOverTime()
		{
			PrefabPool.<PreloadOverTime>c__IteratorB <PreloadOverTime>c__IteratorB = new PrefabPool.<PreloadOverTime>c__IteratorB();
			<PreloadOverTime>c__IteratorB.<>f__this = this;
			return <PreloadOverTime>c__IteratorB;
		}

		public bool Contains(Transform transform)
		{
			if (this.prefabGO == null)
			{
				UnityEngine.Debug.LogError(string.Format("SpawnPool {0}: PrefabPool.prefabGO is null", this.spawnPool.poolName));
			}
			bool flag = this.spawned.Contains(transform);
			return flag || this.despawned.Contains(transform);
		}

		private void nameInstance(Transform instance)
		{
			instance.name += (this.totalCount + 1).ToString("#000");
		}
	}
}
