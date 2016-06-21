using Bolt;
using System;
using UnityEngine;

namespace TheForest.Buildings.World
{
	public class DestroyOnContactWithTag : MonoBehaviour
	{
		public int _hits = 1;

		public string _tag = "Weapon";

		public GameObject _destroyTarget;

		public GameObject _replaceByPrefab;

		public bool _mpOnly;

		protected int _remainingHits;

		private void OnEnable()
		{
			this._remainingHits = this._hits;
		}

		protected virtual void OnTriggerEnter(Collider other)
		{
			if (base.enabled && other.CompareTag(this._tag))
			{
				if (this._mpOnly && !BoltNetwork.isRunning)
				{
					return;
				}
				if (--this._remainingHits <= 0)
				{
					BoltEntity componentInParent = base.GetComponentInParent<BoltEntity>();
					if (BoltNetwork.isRunning && componentInParent)
					{
						DestroyWithTag destroyWithTag = DestroyWithTag.Create(GlobalTargets.OnlyServer);
						destroyWithTag.Entity = componentInParent;
						destroyWithTag.Send();
					}
					else
					{
						this.Perform(false);
					}
				}
			}
		}

		private void PerformDestroy(bool multiplayer)
		{
			this.Perform(multiplayer);
		}

		public virtual void Perform(bool multiplayer)
		{
			if (base.enabled)
			{
				base.enabled = false;
				if (this._replaceByPrefab)
				{
					GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this._replaceByPrefab, this._destroyTarget.transform.position, this._destroyTarget.transform.rotation);
					if (this._destroyTarget.transform.parent)
					{
						gameObject.transform.parent = this._destroyTarget.transform.parent;
					}
				}
				if (!multiplayer)
				{
					UnityEngine.Object.Destroy(this._destroyTarget);
				}
			}
		}
	}
}
