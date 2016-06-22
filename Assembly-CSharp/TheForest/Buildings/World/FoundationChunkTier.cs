using Bolt;
using System;
using TheForest.Buildings.Creation;
using TheForest.Utils;
using TheForest.World;
using UnityEngine;

namespace TheForest.Buildings.World
{
	public class FoundationChunkTier : MonoBehaviour
	{
		public int _edgeNum;

		public int _segmentNum;

		public int _tierNum;

		public FoundationHealth _owner;

		private void Start()
		{
			this._owner = base.transform.GetComponentInParent<FoundationHealth>();
		}

		private void OnWillDestroy(Vector3 position)
		{
			if (this._owner)
			{
				this._owner.TierDestroyed(this, position);
			}
			else
			{
				Debug.LogError("Missing FoundationHealth in chunk");
			}
		}

		public void LocalizedHitReal(LocalizedHitData data)
		{
			if (this._owner)
			{
				this._owner.LocalizedTierHit(data, this);
			}
		}

		public void LocalizedHit(LocalizedHitData data)
		{
			if (BoltNetwork.isClient)
			{
				if (this._owner)
				{
					FoundationArchitect foundation = this._owner._foundation;
					if (foundation)
					{
						FoundationExLocalizedHit foundationExLocalizedHit = FoundationExLocalizedHit.Create(GlobalTargets.OnlyServer);
						foundationExLocalizedHit.Entity = base.GetComponentInParent<BoltEntity>();
						foundationExLocalizedHit.Chunk = foundation.GetChunkIndex(this);
						foundationExLocalizedHit.HitDamage = data._damage;
						foundationExLocalizedHit.HitPosition = data._position;
						foundationExLocalizedHit.Send();
						Prefabs.Instance.SpawnWoodHitPS(data._position, Quaternion.LookRotation(base.transform.right));
						this._owner.Distort(data);
					}
				}
			}
			else
			{
				this.LocalizedHitReal(data);
			}
		}

		public void OnExplode(Explode.Data explodeData)
		{
			if (this._owner)
			{
				BuildingExplosion component = this._owner.GetComponent<BuildingExplosion>();
				if (component)
				{
					component.OnExplodeFoundationTier(explodeData, this);
				}
			}
		}

		public void LookAtExplosionReal(Vector3 position)
		{
			if (this._owner)
			{
				BuildingExplosion component = this._owner.GetComponent<BuildingExplosion>();
				if (component && base.transform.parent && !base.GetComponentInChildren<Renderer>())
				{
					this.OnWillDestroy(position);
				}
			}
		}
	}
}
