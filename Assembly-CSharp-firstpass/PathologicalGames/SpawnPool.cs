using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace PathologicalGames
{
	[AddComponentMenu("Path-o-logical/PoolManager/SpawnPool")]
	public sealed class SpawnPool : MonoBehaviour, IList<Transform>, ICollection<Transform>, IEnumerable<Transform>, IEnumerable
	{
		[HideInInspector]
		public bool DelayNextDisable;

		[HideInInspector]
		public Transform NextDisableTr;

		public bool disableOnClient;

		public bool disableOnServer;

		public string poolName = string.Empty;

		public bool matchPoolScale;

		public bool matchPoolLayer;

		public bool dontReparent;

		public bool _dontDestroyOnLoad;

		public bool logMessages;

		public List<PrefabPool> _perPrefabPoolOptions = new List<PrefabPool>();

		public Dictionary<object, bool> prefabsFoldOutStates = new Dictionary<object, bool>();

		public float maxParticleDespawnTime = 300f;

		public PrefabsDict prefabs = new PrefabsDict();

		public Dictionary<object, bool> _editorListItemStates = new Dictionary<object, bool>();

		private List<PrefabPool> _prefabPools = new List<PrefabPool>();

		internal List<Transform> _spawned = new List<Transform>();

		public bool dontDestroyOnLoad
		{
			get
			{
				return this._dontDestroyOnLoad;
			}
			set
			{
				this._dontDestroyOnLoad = value;
				if (this.group != null && !base.transform.parent)
				{
					UnityEngine.Object.DontDestroyOnLoad(this.group.gameObject);
				}
			}
		}

		public Transform group
		{
			get;
			private set;
		}

		public Dictionary<string, PrefabPool> prefabPools
		{
			get
			{
				Dictionary<string, PrefabPool> dictionary = new Dictionary<string, PrefabPool>();
				for (int i = 0; i < this._prefabPools.Count; i++)
				{
					dictionary[this._prefabPools[i].prefabGO.name] = this._prefabPools[i];
				}
				return dictionary;
			}
		}

		public Transform this[int index]
		{
			get
			{
				return this._spawned[index];
			}
			set
			{
				throw new NotImplementedException("Read-only.");
			}
		}

		public int Count
		{
			get
			{
				return this._spawned.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		[DebuggerHidden]
		IEnumerator IEnumerable.GetEnumerator()
		{
			SpawnPool.GetEnumerator>c__Iterator3 getEnumerator>c__Iterator = new SpawnPool.GetEnumerator>c__Iterator3();
			getEnumerator>c__Iterator.<>f__this = this;
			return getEnumerator>c__Iterator;
		}

		bool ICollection<Transform>.Remove(Transform item)
		{
			throw new NotImplementedException();
		}

		private void Awake()
		{
			if (BoltNetwork.isClient && this.disableOnClient)
			{
				base.gameObject.SetActive(false);
				return;
			}
			if (BoltNetwork.isServerOrNotRunning && this.disableOnServer)
			{
				base.gameObject.SetActive(false);
				return;
			}
			if (this._dontDestroyOnLoad && !base.transform.parent)
			{
				UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			}
			this.group = base.transform;
			if (this.poolName == string.Empty)
			{
				this.poolName = this.group.name.Replace("Pool", string.Empty);
				this.poolName = this.poolName.Replace("(Clone)", string.Empty);
			}
			if (this.logMessages)
			{
				UnityEngine.Debug.Log(string.Format("SpawnPool {0}: Initializing..", this.poolName));
			}
			for (int i = 0; i < this._perPrefabPoolOptions.Count; i++)
			{
				if (this._perPrefabPoolOptions[i].prefab == null)
				{
					UnityEngine.Debug.LogWarning(string.Format("Initialization Warning: Pool '{0}' contains a PrefabPool with no prefab reference. Skipping.", this.poolName));
				}
				else
				{
					this._perPrefabPoolOptions[i].inspectorInstanceConstructor();
					this.CreatePrefabPool(this._perPrefabPoolOptions[i]);
				}
			}
			PoolManager.Pools.Add(this);
		}

		private void OnDestroy()
		{
			if (BoltNetwork.isClient && this.disableOnClient)
			{
				return;
			}
			if (BoltNetwork.isServer && this.disableOnServer)
			{
				return;
			}
			if (this.logMessages)
			{
				UnityEngine.Debug.Log(string.Format("SpawnPool {0}: Destroying...", this.poolName));
			}
			PoolManager.Pools.Remove(this);
			base.StopAllCoroutines();
			this._spawned.Clear();
			foreach (PrefabPool current in this._prefabPools)
			{
				current.SelfDestruct();
			}
			this._prefabPools.Clear();
			this.prefabs._Clear();
		}

		public void CreatePrefabPool(PrefabPool prefabPool)
		{
			if (this.GetPrefabPool(prefabPool.prefab) == null)
			{
				prefabPool.spawnPool = this;
				this._prefabPools.Add(prefabPool);
				this.prefabs._Add(prefabPool.prefab.name, prefabPool.prefab);
			}
			if (!prefabPool.preloaded)
			{
				if (this.logMessages)
				{
					UnityEngine.Debug.Log(string.Format("SpawnPool {0}: Preloading {1} {2}", this.poolName, prefabPool.preloadAmount, prefabPool.prefab.name));
				}
				prefabPool.PreloadInstances();
			}
		}

		public void Add(Transform instance, string prefabName, bool despawn, bool parent)
		{
			for (int i = 0; i < this._prefabPools.Count; i++)
			{
				if (this._prefabPools[i].prefabGO == null)
				{
					UnityEngine.Debug.LogError("Unexpected Error: PrefabPool.prefabGO is null");
					return;
				}
				if (this._prefabPools[i].prefabGO.name == prefabName)
				{
					this._prefabPools[i].AddUnpooled(instance, despawn);
					if (this.logMessages)
					{
						UnityEngine.Debug.Log(string.Format("SpawnPool {0}: Adding previously unpooled instance {1}", this.poolName, instance.name));
					}
					if (parent)
					{
						instance.parent = this.group;
					}
					if (!despawn)
					{
						this._spawned.Add(instance);
					}
					return;
				}
			}
			UnityEngine.Debug.LogError(string.Format("SpawnPool {0}: PrefabPool {1} not found.", this.poolName, prefabName));
		}

		public void Add(Transform item)
		{
			string message = "Use SpawnPool.Spawn() to properly add items to the pool.";
			throw new NotImplementedException(message);
		}

		public void Remove(Transform item)
		{
			string message = "Use Despawn() to properly manage items that should remain in the pool but be deactivated.";
			throw new NotImplementedException(message);
		}

		public Transform Spawn(Transform prefab, Vector3 pos, Quaternion rot, Transform parent)
		{
			int i = 0;
			Transform transform;
			while (i < this._prefabPools.Count)
			{
				if (this._prefabPools[i].prefabGO == prefab.gameObject)
				{
					transform = this._prefabPools[i].SpawnInstance(pos, rot);
					if (transform == null)
					{
						return null;
					}
					if (parent != null)
					{
						transform.parent = parent;
					}
					else if (!this.dontReparent && transform.parent != this.group)
					{
						transform.parent = this.group;
					}
					this._spawned.Add(transform);
					transform.gameObject.BroadcastMessage("OnSpawned", this, SendMessageOptions.DontRequireReceiver);
					return transform;
				}
				else
				{
					i++;
				}
			}
			PrefabPool prefabPool = new PrefabPool(prefab);
			this.CreatePrefabPool(prefabPool);
			transform = prefabPool.SpawnInstance(pos, rot);
			if (parent != null)
			{
				transform.parent = parent;
			}
			else
			{
				transform.parent = this.group;
			}
			this._spawned.Add(transform);
			transform.gameObject.BroadcastMessage("OnSpawned", this, SendMessageOptions.DontRequireReceiver);
			return transform;
		}

		public Transform Spawn(Transform prefab, Vector3 pos, Quaternion rot)
		{
			Transform transform = this.Spawn(prefab, pos, rot, null);
			if (transform == null)
			{
				return null;
			}
			return transform;
		}

		public Transform Spawn(Transform prefab)
		{
			return this.Spawn(prefab, Vector3.zero, Quaternion.identity);
		}

		public Transform Spawn(Transform prefab, Transform parent)
		{
			return this.Spawn(prefab, Vector3.zero, Quaternion.identity, parent);
		}

		public Transform Spawn(string prefabName)
		{
			Transform prefab = this.prefabs[prefabName];
			return this.Spawn(prefab);
		}

		public Transform Spawn(string prefabName, Transform parent)
		{
			Transform prefab = this.prefabs[prefabName];
			return this.Spawn(prefab, parent);
		}

		public Transform Spawn(string prefabName, Vector3 pos, Quaternion rot)
		{
			Transform prefab = this.prefabs[prefabName];
			return this.Spawn(prefab, pos, rot);
		}

		public Transform Spawn(string prefabName, Vector3 pos, Quaternion rot, Transform parent)
		{
			Transform prefab = this.prefabs[prefabName];
			return this.Spawn(prefab, pos, rot, parent);
		}

		public AudioSource Spawn(AudioSource prefab, Vector3 pos, Quaternion rot)
		{
			return this.Spawn(prefab, pos, rot, null);
		}

		public AudioSource Spawn(AudioSource prefab)
		{
			return this.Spawn(prefab, Vector3.zero, Quaternion.identity, null);
		}

		public AudioSource Spawn(AudioSource prefab, Transform parent)
		{
			return this.Spawn(prefab, Vector3.zero, Quaternion.identity, parent);
		}

		public AudioSource Spawn(AudioSource prefab, Vector3 pos, Quaternion rot, Transform parent)
		{
			Transform transform = this.Spawn(prefab.transform, pos, rot, parent);
			if (transform == null)
			{
				return null;
			}
			AudioSource component = transform.GetComponent<AudioSource>();
			component.Play();
			base.StartCoroutine(this.ListForAudioStop(component));
			return component;
		}

		public ParticleSystem Spawn(ParticleSystem prefab, Vector3 pos, Quaternion rot)
		{
			return this.Spawn(prefab, pos, rot, null);
		}

		public ParticleSystem Spawn(ParticleSystem prefab, Vector3 pos, Quaternion rot, Transform parent)
		{
			Transform transform = this.Spawn(prefab.transform, pos, rot, parent);
			if (transform == null)
			{
				return null;
			}
			ParticleSystem component = transform.GetComponent<ParticleSystem>();
			base.StartCoroutine(this.ListenForEmitDespawn(component));
			return component;
		}

		public ParticleEmitter Spawn(ParticleEmitter prefab, Vector3 pos, Quaternion rot)
		{
			Transform transform = this.Spawn(prefab.transform, pos, rot);
			if (transform == null)
			{
				return null;
			}
			ParticleAnimator component = transform.GetComponent<ParticleAnimator>();
			if (component != null)
			{
				component.autodestruct = false;
			}
			ParticleEmitter component2 = transform.GetComponent<ParticleEmitter>();
			component2.emit = true;
			base.StartCoroutine(this.ListenForEmitDespawn(component2));
			return component2;
		}

		public ParticleEmitter Spawn(ParticleEmitter prefab, Vector3 pos, Quaternion rot, string colorPropertyName, Color color)
		{
			Transform transform = this.Spawn(prefab.transform, pos, rot);
			if (transform == null)
			{
				return null;
			}
			ParticleAnimator component = transform.GetComponent<ParticleAnimator>();
			if (component != null)
			{
				component.autodestruct = false;
			}
			ParticleEmitter component2 = transform.GetComponent<ParticleEmitter>();
			component2.GetComponent<Renderer>().material.SetColor(colorPropertyName, color);
			component2.emit = true;
			base.StartCoroutine(this.ListenForEmitDespawn(component2));
			return component2;
		}

		public void Despawn(Transform instance)
		{
			bool flag = false;
			for (int i = 0; i < this._prefabPools.Count; i++)
			{
				if (this._prefabPools[i]._spawned.Contains(instance))
				{
					flag = this._prefabPools[i].DespawnInstance(instance);
					break;
				}
				if (this._prefabPools[i]._despawned.Contains(instance))
				{
					return;
				}
			}
			if (!flag)
			{
				UnityEngine.Debug.LogError(string.Format("SpawnPool {0}: {1} not found in SpawnPool", this.poolName, instance.name));
				return;
			}
			this._spawned.Remove(instance);
		}

		public void KillInstance(Transform instance)
		{
			for (int i = 0; i < this._prefabPools.Count; i++)
			{
				if (this._prefabPools[i]._spawned.Contains(instance))
				{
					this._prefabPools[i]._spawned.Remove(instance);
				}
				else if (this._prefabPools[i]._despawned.Contains(instance))
				{
					this._prefabPools[i]._despawned.Remove(instance);
				}
			}
			if (this._spawned.Contains(instance))
			{
				this._spawned.Remove(instance);
			}
		}

		public void Despawn(Transform instance, Transform parent)
		{
			instance.parent = parent;
			this.Despawn(instance);
		}

		public void Despawn(Transform instance, float seconds)
		{
			base.StartCoroutine(this.DoDespawnAfterSeconds(instance, seconds, false, null));
		}

		public void Despawn(Transform instance, float seconds, Transform parent)
		{
			base.StartCoroutine(this.DoDespawnAfterSeconds(instance, seconds, true, parent));
		}

		[DebuggerHidden]
		private IEnumerator DoDespawnAfterSeconds(Transform instance, float seconds, bool useParent, Transform parent)
		{
			SpawnPool.<DoDespawnAfterSeconds>c__Iterator4 <DoDespawnAfterSeconds>c__Iterator = new SpawnPool.<DoDespawnAfterSeconds>c__Iterator4();
			<DoDespawnAfterSeconds>c__Iterator.instance = instance;
			<DoDespawnAfterSeconds>c__Iterator.seconds = seconds;
			<DoDespawnAfterSeconds>c__Iterator.useParent = useParent;
			<DoDespawnAfterSeconds>c__Iterator.parent = parent;
			<DoDespawnAfterSeconds>c__Iterator.<$>instance = instance;
			<DoDespawnAfterSeconds>c__Iterator.<$>seconds = seconds;
			<DoDespawnAfterSeconds>c__Iterator.<$>useParent = useParent;
			<DoDespawnAfterSeconds>c__Iterator.<$>parent = parent;
			<DoDespawnAfterSeconds>c__Iterator.<>f__this = this;
			return <DoDespawnAfterSeconds>c__Iterator;
		}

		public void DespawnAll()
		{
			List<Transform> list = new List<Transform>(this._spawned);
			for (int i = 0; i < list.Count; i++)
			{
				this.Despawn(list[i]);
			}
		}

		public bool IsSpawned(Transform instance)
		{
			return this._spawned.Contains(instance);
		}

		public PrefabPool GetPrefabPool(Transform prefab)
		{
			for (int i = 0; i < this._prefabPools.Count; i++)
			{
				if (this._prefabPools[i].prefabGO == null)
				{
					UnityEngine.Debug.LogError(string.Format("SpawnPool {0}: PrefabPool.prefabGO is null", this.poolName));
				}
				if (this._prefabPools[i].prefabGO == prefab.gameObject)
				{
					return this._prefabPools[i];
				}
			}
			return null;
		}

		public PrefabPool GetPrefabPool(GameObject prefab)
		{
			for (int i = 0; i < this._prefabPools.Count; i++)
			{
				if (this._prefabPools[i].prefabGO == null)
				{
					UnityEngine.Debug.LogError(string.Format("SpawnPool {0}: PrefabPool.prefabGO is null", this.poolName));
				}
				if (this._prefabPools[i].prefabGO == prefab)
				{
					return this._prefabPools[i];
				}
			}
			return null;
		}

		public Transform GetPrefab(Transform instance)
		{
			for (int i = 0; i < this._prefabPools.Count; i++)
			{
				if (this._prefabPools[i].Contains(instance))
				{
					return this._prefabPools[i].prefab;
				}
			}
			return null;
		}

		public GameObject GetPrefab(GameObject instance)
		{
			for (int i = 0; i < this._prefabPools.Count; i++)
			{
				if (this._prefabPools[i].Contains(instance.transform))
				{
					return this._prefabPools[i].prefabGO;
				}
			}
			return null;
		}

		[DebuggerHidden]
		private IEnumerator ListForAudioStop(AudioSource src)
		{
			SpawnPool.<ListForAudioStop>c__Iterator5 <ListForAudioStop>c__Iterator = new SpawnPool.<ListForAudioStop>c__Iterator5();
			<ListForAudioStop>c__Iterator.src = src;
			<ListForAudioStop>c__Iterator.<$>src = src;
			<ListForAudioStop>c__Iterator.<>f__this = this;
			return <ListForAudioStop>c__Iterator;
		}

		[DebuggerHidden]
		private IEnumerator ListenForEmitDespawn(ParticleEmitter emitter)
		{
			SpawnPool.<ListenForEmitDespawn>c__Iterator6 <ListenForEmitDespawn>c__Iterator = new SpawnPool.<ListenForEmitDespawn>c__Iterator6();
			<ListenForEmitDespawn>c__Iterator.emitter = emitter;
			<ListenForEmitDespawn>c__Iterator.<$>emitter = emitter;
			<ListenForEmitDespawn>c__Iterator.<>f__this = this;
			return <ListenForEmitDespawn>c__Iterator;
		}

		[DebuggerHidden]
		private IEnumerator ListenForEmitDespawn(ParticleSystem emitter)
		{
			SpawnPool.<ListenForEmitDespawn>c__Iterator7 <ListenForEmitDespawn>c__Iterator = new SpawnPool.<ListenForEmitDespawn>c__Iterator7();
			<ListenForEmitDespawn>c__Iterator.emitter = emitter;
			<ListenForEmitDespawn>c__Iterator.<$>emitter = emitter;
			<ListenForEmitDespawn>c__Iterator.<>f__this = this;
			return <ListenForEmitDespawn>c__Iterator;
		}

		public override string ToString()
		{
			List<string> list = new List<string>();
			foreach (Transform current in this._spawned)
			{
				list.Add(current.name);
			}
			return string.Join(", ", list.ToArray());
		}

		public bool Contains(Transform item)
		{
			string message = "Use IsSpawned(Transform instance) instead.";
			throw new NotImplementedException(message);
		}

		public void CopyTo(Transform[] array, int arrayIndex)
		{
			this._spawned.CopyTo(array, arrayIndex);
		}

		[DebuggerHidden]
		public IEnumerator<Transform> GetEnumerator()
		{
			SpawnPool.<GetEnumerator>c__Iterator8 <GetEnumerator>c__Iterator = new SpawnPool.<GetEnumerator>c__Iterator8();
			<GetEnumerator>c__Iterator.<>f__this = this;
			return <GetEnumerator>c__Iterator;
		}

		public int IndexOf(Transform item)
		{
			throw new NotImplementedException();
		}

		public void Insert(int index, Transform item)
		{
			throw new NotImplementedException();
		}

		public void RemoveAt(int index)
		{
			throw new NotImplementedException();
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}
	}
}
