using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace PathologicalGames
{
	[AddComponentMenu("Path-o-logical/TargetPro/Fire Controller"), RequireComponent(typeof(TargetTracker))]
	public class FireController : MonoBehaviour
	{
		public enum NOTIFY_TARGET_OPTIONS
		{
			Off,
			Direct,
			PassToProjectile,
			UseProjectileEffects
		}

		public delegate void OnStartDelegate();

		public delegate void OnUpdateDelegate();

		public delegate void OnTargetUpdateDelegate(TargetList targets);

		public delegate void OnIdleUpdateDelegate();

		public delegate void OnStopDelegate();

		public delegate void OnPreFireDelegate(TargetList targets);

		public delegate void OnFireDelegate(TargetList targets);

		public float interval;

		public bool initIntervalCountdownAtZero = true;

		public FireController.NOTIFY_TARGET_OPTIONS notifyTargets = FireController.NOTIFY_TARGET_OPTIONS.Direct;

		public Transform ammoPrefab;

		public List<HitEffectGUIBacker> _effectsOnTarget = new List<HitEffectGUIBacker>();

		public bool waitForAlignment;

		public bool flatAngleCompare;

		public Transform emitter;

		public float lockOnAngleTolerance = 5f;

		public DEBUG_LEVELS debugLevel;

		public float fireIntervalCounter = 99999f;

		public Dictionary<object, bool> _editorListItemStates = new Dictionary<object, bool>();

		private TargetTracker targetTracker;

		private TargetList targets = new TargetList();

		private FireController.OnStartDelegate onStartDelegates;

		private FireController.OnUpdateDelegate onUpdateDelegates;

		private FireController.OnTargetUpdateDelegate onTargetUpdateDelegates;

		private FireController.OnIdleUpdateDelegate onIdleUpdateDelegates;

		private FireController.OnStopDelegate onStopDelegates;

		private FireController.OnPreFireDelegate onPreFireDelegates;

		private FireController.OnFireDelegate onFireDelegates;

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

		private bool isLockedOnTarget
		{
			get
			{
				if (!this.waitForAlignment)
				{
					return true;
				}
				Vector3 vector = this.targets[0].transform.position - this.emitter.position;
				Vector3 forward = this.emitter.forward;
				if (this.flatAngleCompare)
				{
					vector.y = (forward.y = 0f);
				}
				float num = Vector3.Angle(vector, forward);
				if (this.debugLevel > DEBUG_LEVELS.Off)
				{
					UnityEngine.Debug.DrawRay(this.emitter.position, vector * 3f, Color.cyan);
					UnityEngine.Debug.DrawRay(this.emitter.position, forward * 3f, Color.cyan);
				}
				return num < this.lockOnAngleTolerance;
			}
		}

		private string targetsString
		{
			get
			{
				string[] array = new string[this.targets.Count];
				int num = 0;
				foreach (Target current in this.targets)
				{
					array[num] = current.transform.name;
					num++;
				}
				return string.Join(", ", array);
			}
		}

		private void Awake()
		{
			if (this.emitter == null)
			{
				this.emitter = base.transform;
			}
			this.targetTracker = base.GetComponent<TargetTracker>();
		}

		private void OnEnable()
		{
			base.StartCoroutine(this.FiringSystem());
		}

		private void OnDisable()
		{
			this.OnStop();
			this.targets.Clear();
		}

		private void OnStart()
		{
			if (this.debugLevel > DEBUG_LEVELS.Off)
			{
				string arg = "Starting Firing System...";
				UnityEngine.Debug.Log(string.Format("{0}: {1}", this, arg));
			}
			if (this.onStartDelegates != null)
			{
				this.onStartDelegates();
			}
		}

		private void OnUpdate()
		{
			if (this.onUpdateDelegates != null)
			{
				this.onUpdateDelegates();
			}
		}

		private void OnTargetUpdate(TargetList targets)
		{
			if (this.onTargetUpdateDelegates != null)
			{
				this.onTargetUpdateDelegates(targets);
			}
		}

		private void OnIdleUpdate()
		{
			if (this.onIdleUpdateDelegates != null)
			{
				this.onIdleUpdateDelegates();
			}
		}

		private void OnStop()
		{
			if (this.debugLevel > DEBUG_LEVELS.Off)
			{
				string arg = "stopping Firing System...";
				UnityEngine.Debug.Log(string.Format("{0}: {1}", this, arg));
			}
			if (this.onStopDelegates != null)
			{
				this.onStopDelegates();
			}
		}

		private void OnFire()
		{
			if (this.debugLevel > DEBUG_LEVELS.Off)
			{
				string arg = string.Format("Firing on: {0}\nHitEffects{1}", this.targetsString, this.effectsOnTarget.ToString());
				UnityEngine.Debug.Log(string.Format("{0}: {1}", this, arg));
			}
			TargetList targetList = new TargetList();
			foreach (Target current in this.targets)
			{
				Target target = new Target(current);
				target.fireController = this;
				targetList.Add(target);
				switch (this.notifyTargets)
				{
				case FireController.NOTIFY_TARGET_OPTIONS.Direct:
					target.targetable.OnHit(this.effectsOnTarget, target);
					this.SpawnAmmunition(target, false, false);
					break;
				case FireController.NOTIFY_TARGET_OPTIONS.PassToProjectile:
					this.SpawnAmmunition(target, true, true);
					break;
				case FireController.NOTIFY_TARGET_OPTIONS.UseProjectileEffects:
					this.SpawnAmmunition(target, true, false);
					break;
				}
				if (this.notifyTargets > FireController.NOTIFY_TARGET_OPTIONS.Off && this.debugLevel > DEBUG_LEVELS.Off)
				{
					UnityEngine.Debug.DrawLine(this.emitter.position, target.transform.position, Color.red);
				}
			}
			this.targets = targetList;
			if (this.onFireDelegates != null)
			{
				this.onFireDelegates(this.targets);
			}
		}

		private void SpawnAmmunition(Target target, bool passTarget, bool passEffects)
		{
			if (this.ammoPrefab == null)
			{
				return;
			}
			Transform transform = InstanceManager.Spawn(this.ammoPrefab.transform, this.emitter.position, this.emitter.rotation);
			if (!passTarget)
			{
				return;
			}
			Projectile component = transform.GetComponent<Projectile>();
			if (component == null)
			{
				string arg = string.Format("Ammo '{0}' must have an Projectile component", transform.name);
				UnityEngine.Debug.Log(string.Format("{0}: {1}", this, arg));
				return;
			}
			component.fireController = this;
			component.target = target;
			if (passEffects)
			{
				component.effectsOnTarget = this.effectsOnTarget;
			}
		}

		public void AddOnStartDelegate(FireController.OnStartDelegate del)
		{
			this.onStartDelegates = (FireController.OnStartDelegate)Delegate.Combine(this.onStartDelegates, del);
		}

		public void SetOnStartDelegate(FireController.OnStartDelegate del)
		{
			this.onStartDelegates = del;
		}

		public void RemoveOnStartDelegate(FireController.OnStartDelegate del)
		{
			this.onStartDelegates = (FireController.OnStartDelegate)Delegate.Remove(this.onStartDelegates, del);
		}

		public void AddOnUpdateDelegate(FireController.OnUpdateDelegate del)
		{
			this.onUpdateDelegates = (FireController.OnUpdateDelegate)Delegate.Combine(this.onUpdateDelegates, del);
		}

		public void SetOnUpdateDelegate(FireController.OnUpdateDelegate del)
		{
			this.onUpdateDelegates = del;
		}

		public void RemoveOnUpdateDelegate(FireController.OnUpdateDelegate del)
		{
			this.onUpdateDelegates = (FireController.OnUpdateDelegate)Delegate.Remove(this.onUpdateDelegates, del);
		}

		public void AddOnTargetUpdateDelegate(FireController.OnTargetUpdateDelegate del)
		{
			this.onTargetUpdateDelegates = (FireController.OnTargetUpdateDelegate)Delegate.Combine(this.onTargetUpdateDelegates, del);
		}

		public void SetOnTargetUpdateDelegate(FireController.OnTargetUpdateDelegate del)
		{
			this.onTargetUpdateDelegates = del;
		}

		public void RemoveOnTargetUpdateDelegate(FireController.OnTargetUpdateDelegate del)
		{
			this.onTargetUpdateDelegates = (FireController.OnTargetUpdateDelegate)Delegate.Remove(this.onTargetUpdateDelegates, del);
		}

		public void AddOnIdleUpdateDelegate(FireController.OnIdleUpdateDelegate del)
		{
			this.onIdleUpdateDelegates = (FireController.OnIdleUpdateDelegate)Delegate.Combine(this.onIdleUpdateDelegates, del);
		}

		public void SetOnIdleUpdateDelegate(FireController.OnIdleUpdateDelegate del)
		{
			this.onIdleUpdateDelegates = del;
		}

		public void RemoveOnIdleUpdateDelegate(FireController.OnIdleUpdateDelegate del)
		{
			this.onIdleUpdateDelegates = (FireController.OnIdleUpdateDelegate)Delegate.Remove(this.onIdleUpdateDelegates, del);
		}

		public void AddOnStopDelegate(FireController.OnStopDelegate del)
		{
			this.onStopDelegates = (FireController.OnStopDelegate)Delegate.Combine(this.onStopDelegates, del);
		}

		public void SetOnStopDelegate(FireController.OnStopDelegate del)
		{
			this.onStopDelegates = del;
		}

		public void RemoveOnStopDelegate(FireController.OnStopDelegate del)
		{
			this.onStopDelegates = (FireController.OnStopDelegate)Delegate.Remove(this.onStopDelegates, del);
		}

		public void AddOnPreFireDelegate(FireController.OnPreFireDelegate del)
		{
			this.onPreFireDelegates = (FireController.OnPreFireDelegate)Delegate.Combine(this.onPreFireDelegates, del);
		}

		public void SetOnPreFireDelegate(FireController.OnPreFireDelegate del)
		{
			this.onPreFireDelegates = del;
		}

		public void RemoveOnPreFireDelegate(FireController.OnPreFireDelegate del)
		{
			this.onPreFireDelegates = (FireController.OnPreFireDelegate)Delegate.Remove(this.onPreFireDelegates, del);
		}

		public void AddOnFireDelegate(FireController.OnFireDelegate del)
		{
			this.onFireDelegates = (FireController.OnFireDelegate)Delegate.Combine(this.onFireDelegates, del);
		}

		public void SetOnFireDelegate(FireController.OnFireDelegate del)
		{
			this.onFireDelegates = del;
		}

		public void RemoveOnFireDelegate(FireController.OnFireDelegate del)
		{
			this.onFireDelegates = (FireController.OnFireDelegate)Delegate.Remove(this.onFireDelegates, del);
		}

		public void FireImmediately(bool resetIntervalCounter)
		{
			if (resetIntervalCounter)
			{
				this.fireIntervalCounter = this.interval;
			}
			if (this.onPreFireDelegates != null)
			{
				this.onPreFireDelegates(this.targets);
			}
			this.OnFire();
		}

		[DebuggerHidden]
		private IEnumerator FiringSystem()
		{
			FireController.<FiringSystem>c__IteratorE <FiringSystem>c__IteratorE = new FireController.<FiringSystem>c__IteratorE();
			<FiringSystem>c__IteratorE.<>f__this = this;
			return <FiringSystem>c__IteratorE;
		}
	}
}
