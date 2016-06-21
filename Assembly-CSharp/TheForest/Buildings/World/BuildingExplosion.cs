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

		public void UnlocalizedExplode()
		{
			base.SendMessage("lookAtExplosion", base.transform.position + Vector3.down);
		}

		[DebuggerHidden]
		public IEnumerator OnExplode(Explode explode)
		{
			BuildingExplosion.<OnExplode>c__Iterator143 <OnExplode>c__Iterator = new BuildingExplosion.<OnExplode>c__Iterator143();
			<OnExplode>c__Iterator.explode = explode;
			<OnExplode>c__Iterator.<$>explode = explode;
			<OnExplode>c__Iterator.<>f__this = this;
			return <OnExplode>c__Iterator;
		}

		public void OnExplodeFoundationTier(Explode explode, FoundationChunkTier tier)
		{
			FoundationHealth component = base.GetComponent<FoundationHealth>();
			if (component)
			{
				float num = Vector3.Distance(tier.transform.position, explode.transform.position);
				if (num < explode.radius)
				{
					component.LocalizedTierHit(new LocalizedHitData
					{
						_damage = explode.damage * this.GetDamageRatio(num, explode.radius),
						_position = explode.transform.position,
						_distortRatio = 2.5f
					}, tier);
				}
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
