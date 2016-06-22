using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace PathologicalGames
{
	[AddComponentMenu("Path-o-logical/TargetPro/Detonator")]
	public class Detonator : TargetTracker
	{
		public delegate void OnDetonating();

		public delegate void OnDetonatingUpdate(float progress);

		public delegate void OnDetonation(TargetList targets);

		public float durration = 2f;

		public Vector3 maxRange;

		public List<HitEffectGUIBacker> _effectsOnTarget = new List<HitEffectGUIBacker>();

		public Dictionary<object, bool> _editorListItemStates = new Dictionary<object, bool>();

		private Detonator.OnDetonating OnDetonatingDelegates;

		private Detonator.OnDetonatingUpdate OnDetonatingUpdateDelegates;

		private Detonator.OnDetonation OnDetonationDelegates;

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
			this.maxRange = base.range;
			base.range = Vector3.zero;
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			base.StartCoroutine(this.Detonate());
		}

		[DebuggerHidden]
		private IEnumerator Detonate()
		{
			Detonator.<Detonate>c__IteratorD <Detonate>c__IteratorD = new Detonator.<Detonate>c__IteratorD();
			<Detonate>c__IteratorD.<>f__this = this;
			return <Detonate>c__IteratorD;
		}

		public void AddOnDetonatingDelegate(Detonator.OnDetonating del)
		{
			this.OnDetonatingDelegates = (Detonator.OnDetonating)Delegate.Combine(this.OnDetonatingDelegates, del);
		}

		public void SetOnDetonatingDelegate(Detonator.OnDetonating del)
		{
			this.OnDetonatingDelegates = del;
		}

		public void RemoveOnDetonatingDelegate(Detonator.OnDetonating del)
		{
			this.OnDetonatingDelegates = (Detonator.OnDetonating)Delegate.Remove(this.OnDetonatingDelegates, del);
		}

		public void AddOnDetonatingUpdateDelegate(Detonator.OnDetonatingUpdate del)
		{
			this.OnDetonatingUpdateDelegates = (Detonator.OnDetonatingUpdate)Delegate.Combine(this.OnDetonatingUpdateDelegates, del);
		}

		public void SetOnDetonatingUpdateDelegate(Detonator.OnDetonatingUpdate del)
		{
			this.OnDetonatingUpdateDelegates = del;
		}

		public void RemoveOnDetonatingUpdateDelegate(Detonator.OnDetonatingUpdate del)
		{
			this.OnDetonatingUpdateDelegates = (Detonator.OnDetonatingUpdate)Delegate.Remove(this.OnDetonatingUpdateDelegates, del);
		}

		public void AddOnDetonationDelegate(Detonator.OnDetonation del)
		{
			this.OnDetonationDelegates = (Detonator.OnDetonation)Delegate.Combine(this.OnDetonationDelegates, del);
		}

		public void SetOnDetonationDelegate(Detonator.OnDetonation del)
		{
			this.OnDetonationDelegates = del;
		}

		public void RemoveOnDetonationDelegate(Detonator.OnDetonation del)
		{
			this.OnDetonationDelegates = (Detonator.OnDetonation)Delegate.Remove(this.OnDetonationDelegates, del);
		}
	}
}
