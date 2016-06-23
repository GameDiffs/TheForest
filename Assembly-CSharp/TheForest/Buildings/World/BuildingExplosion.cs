using Bolt;
using System;
using System.Collections;
using System.Diagnostics;
using TheForest.Utils.Enums;
using TheForest.World;
using UniLinq;
using UnityEngine;

namespace TheForest.Buildings.World
{
	public class BuildingExplosion : EntityBehaviour
	{
		public enum SelectionModes
		{
			Renderer,
			StructureTag,
			Both
		}

		public BuildingExplosion.SelectionModes _selectionMode;

		public bool _destroyThis;

		public CapsuleDirections _capsuleDirection = CapsuleDirections.Z;

		private bool _exploding;

		public bool Exploding
		{
			get
			{
				return this._exploding;
			}
		}

		public void UnlocalizedExplode()
		{
			base.SendMessage("lookAtExplosion", base.transform.position + Vector3.down);
		}

		[DebuggerHidden]
		public IEnumerator OnExplode(Explode.Data explodeData)
		{
			BuildingExplosion.<OnExplode>c__Iterator14A <OnExplode>c__Iterator14A = new BuildingExplosion.<OnExplode>c__Iterator14A();
			<OnExplode>c__Iterator14A.explodeData = explodeData;
			<OnExplode>c__Iterator14A.<$>explodeData = explodeData;
			<OnExplode>c__Iterator14A.<>f__this = this;
			return <OnExplode>c__Iterator14A;
		}

		public void OnExplodeFoundationTier(Explode.Data explodeData, FoundationChunkTier tier)
		{
			FoundationHealth component = base.GetComponent<FoundationHealth>();
			if (component)
			{
				component.LocalizedTierHit(new LocalizedHitData
				{
					_damage = explodeData.explode.damage * this.GetDamageRatio(explodeData.distance, explodeData.explode.radius),
					_position = explodeData.explode.transform.position,
					_distortRatio = 2.5f
				}, tier);
			}
		}

		private float GetDamageRatio(float distanceWithExplosion, float radius)
		{
			return 1f - Mathf.Clamp01((distanceWithExplosion - 2f) / radius);
		}

		private Transform[] GetTransforms()
		{
			if (this._selectionMode == BuildingExplosion.SelectionModes.StructureTag || this._selectionMode == BuildingExplosion.SelectionModes.Both)
			{
				return base.transform.GetComponentsInChildren<Transform>();
			}
			return (from r in base.transform.GetComponentsInChildren<Renderer>()
			select r.transform).ToArray<Transform>();
		}
	}
}
