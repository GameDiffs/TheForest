using System;
using TheForest.Utils;
using TheForest.World;
using UnityEngine;

namespace TheForest.Buildings.World
{
	[DoNotSerializePublic]
	public class BuildingHealthChunkHitRelay : MonoBehaviour
	{
		public void LocalizedHit(LocalizedHitData data)
		{
			BuildingHealthChunk componentInParent = base.transform.GetComponentInParent<BuildingHealthChunk>();
			if (componentInParent)
			{
				componentInParent.LocalizedHit(data);
			}
		}

		public void OnExplode(Explode explode)
		{
			BuildingExplosion componentInParent = base.transform.GetComponentInParent<BuildingExplosion>();
			if (componentInParent)
			{
				Scene.ActiveMB.StartCoroutine(componentInParent.OnExplode(explode));
			}
		}
	}
}
