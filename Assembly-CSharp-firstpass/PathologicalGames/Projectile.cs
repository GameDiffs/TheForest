using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace PathologicalGames
{
	[AddComponentMenu("Path-o-logical/TargetPro/Projectile"), RequireComponent(typeof(Rigidbody))]
	public class Projectile : TargetTracker
	{
		public enum DETONATION_MODES
		{
			TargetOnly,
			HitLayers
		}

		public enum NOTIFY_TARGET_OPTIONS
		{
			Off,
			Direct,
			PassToDetonator
		}

		public delegate void OnLaunched();

		public delegate void OnLaunchedUpdate();

		public delegate void OnDetonation(TargetList targets);

		public List<HitEffectGUIBacker> _effectsOnTarget = new List<HitEffectGUIBacker>();

		public bool areaHit = true;

		public bool detonateOnRigidBodySleep = true;

		public Projectile.DETONATION_MODES detonationMode = Projectile.DETONATION_MODES.HitLayers;

		public float timer;

		public Projectile.NOTIFY_TARGET_OPTIONS notifyTargets = Projectile.NOTIFY_TARGET_OPTIONS.Direct;

		public Transform detonationPrefab;

		public FireController fireController;

		public Dictionary<object, bool> _editorListItemStates = new Dictionary<object, bool>();

		private float curTimer;

		private Rigidbody rbd;

		private Projectile.OnLaunched OnLaunchedDelegates;

		private Projectile.OnLaunchedUpdate OnLaunchedUpdateDelegates;

		private Projectile.OnDetonation OnDetonationDelegates;

		public Target target
		{
			get;
			internal set;
		}

		public HitEffectList effectsOnTarget
		{
			get
			{
				HitEffectList hitEffectList = new HitEffectList();
				foreach (HitEffectGUIBacker current in this._effectsOnTarget)
				{
					hitEffectList.Add(new HitEffect
					{
						name = current.name,
						value = current.value,
						duration = current.duration
					});
				}
				return hitEffectList;
			}
			set
			{
				this._effectsOnTarget.Clear();
				foreach (HitEffect current in value)
				{
					HitEffectGUIBacker item = new HitEffectGUIBacker(current);
					this._effectsOnTarget.Add(item);
				}
			}
		}

		protected override void Awake()
		{
			base.Awake();
			this.rbd = base.GetComponent<Rigidbody>();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			base.StartCoroutine(this.Launch());
		}

		[DebuggerHidden]
		private IEnumerator Launch()
		{
			Projectile.<Launch>c__Iterator12 <Launch>c__Iterator = new Projectile.<Launch>c__Iterator12();
			<Launch>c__Iterator.<>f__this = this;
			return <Launch>c__Iterator;
		}

		private void OnTriggerEnter(Collider other)
		{
			Projectile.DETONATION_MODES dETONATION_MODES = this.detonationMode;
			if (dETONATION_MODES == Projectile.DETONATION_MODES.TargetOnly)
			{
				if (this.target.isSpawned && this.target.gameObject == other.gameObject)
				{
					this.DetonateProjectile();
				}
				return;
			}
			if (dETONATION_MODES != Projectile.DETONATION_MODES.HitLayers)
			{
				return;
			}
			if ((1 << other.gameObject.layer & this.targetLayers) != 0)
			{
				if (!this.areaHit)
				{
					Transform transform = other.transform;
					Targetable component = transform.GetComponent<Targetable>();
					if (component != null)
					{
						this.target = new Target(transform, this);
					}
				}
				this.DetonateProjectile();
			}
		}

		public void DetonateProjectile()
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			TargetList targetList = new TargetList();
			if (this.areaHit)
			{
				base.perimeter.enabled = true;
				targetList.AddRange(this.targets);
			}
			else if (this.target != Target.Null)
			{
				targetList.Add(this.target);
			}
			if (this.debugLevel > DEBUG_LEVELS.Off)
			{
				string arg = string.Format("Detonating with targets: {0}", targetList);
				UnityEngine.Debug.Log(string.Format("Projectile ({0}): {1}", base.name, arg));
			}
			TargetList targetList2 = new TargetList();
			Projectile.NOTIFY_TARGET_OPTIONS nOTIFY_TARGET_OPTIONS;
			foreach (Target current in targetList)
			{
				if (!(current == Target.Null))
				{
					Target target = new Target(current);
					target.projectile = this;
					targetList2.Add(target);
					nOTIFY_TARGET_OPTIONS = this.notifyTargets;
					if (nOTIFY_TARGET_OPTIONS == Projectile.NOTIFY_TARGET_OPTIONS.Direct)
					{
						target.targetable.OnHit(this.effectsOnTarget, target, base.GetComponent<Collider>());
					}
					if (this.debugLevel > DEBUG_LEVELS.Off)
					{
						UnityEngine.Debug.DrawLine(this.xform.position, target.transform.position, Color.red);
					}
				}
			}
			nOTIFY_TARGET_OPTIONS = this.notifyTargets;
			if (nOTIFY_TARGET_OPTIONS != Projectile.NOTIFY_TARGET_OPTIONS.Direct)
			{
				if (nOTIFY_TARGET_OPTIONS == Projectile.NOTIFY_TARGET_OPTIONS.PassToDetonator)
				{
					this.SpawnDetonatorPrefab(true);
				}
			}
			else
			{
				this.SpawnDetonatorPrefab(false);
			}
			if (this.OnDetonationDelegates != null)
			{
				this.OnDetonationDelegates(targetList2);
			}
			this.target = Target.Null;
			InstanceManager.Despawn(base.transform);
		}

		private void SpawnDetonatorPrefab(bool passEffects)
		{
			if (this.detonationPrefab == null)
			{
				return;
			}
			Transform transform = InstanceManager.Spawn(this.detonationPrefab.transform, this.xform.position, this.xform.rotation);
			if (!passEffects)
			{
				return;
			}
			Detonator component = transform.GetComponent<Detonator>();
			if (component == null)
			{
				return;
			}
			component.effectsOnTarget = this.effectsOnTarget;
			component.perimeterShape = base.perimeterShape;
			component.range = base.range;
		}

		public void AddOnLaunchedDelegate(Projectile.OnLaunched del)
		{
			this.OnLaunchedDelegates = (Projectile.OnLaunched)Delegate.Combine(this.OnLaunchedDelegates, del);
		}

		public void SetOnLaunchedDelegate(Projectile.OnLaunched del)
		{
			this.OnLaunchedDelegates = del;
		}

		public void RemoveOnLaunchedDelegate(Projectile.OnLaunched del)
		{
			this.OnLaunchedDelegates = (Projectile.OnLaunched)Delegate.Remove(this.OnLaunchedDelegates, del);
		}

		public void AddOnLaunchedUpdateDelegate(Projectile.OnLaunchedUpdate del)
		{
			this.OnLaunchedUpdateDelegates = (Projectile.OnLaunchedUpdate)Delegate.Combine(this.OnLaunchedUpdateDelegates, del);
		}

		public void SetOnLaunchedUpdateDelegate(Projectile.OnLaunchedUpdate del)
		{
			this.OnLaunchedUpdateDelegates = del;
		}

		public void RemoveOnLaunchedUpdateDelegate(Projectile.OnLaunchedUpdate del)
		{
			this.OnLaunchedUpdateDelegates = (Projectile.OnLaunchedUpdate)Delegate.Remove(this.OnLaunchedUpdateDelegates, del);
		}

		public void AddOnDetonationDelegate(Projectile.OnDetonation del)
		{
			this.OnDetonationDelegates = (Projectile.OnDetonation)Delegate.Combine(this.OnDetonationDelegates, del);
		}

		public void SetOnDetonationDelegate(Projectile.OnDetonation del)
		{
			this.OnDetonationDelegates = del;
		}

		public void RemoveOnDetonationDelegate(Projectile.OnDetonation del)
		{
			this.OnDetonationDelegates = (Projectile.OnDetonation)Delegate.Remove(this.OnDetonationDelegates, del);
		}
	}
}
