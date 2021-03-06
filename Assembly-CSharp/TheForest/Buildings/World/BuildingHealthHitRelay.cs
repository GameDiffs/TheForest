using System;
using TheForest.Utils;
using TheForest.World;
using UnityEngine;

namespace TheForest.Buildings.World
{
	[DoNotSerializePublic]
	public class BuildingHealthHitRelay : MonoBehaviour
	{
		public void LocalizedHit(LocalizedHitData data)
		{
			PrefabIdentifier componentInParent = base.transform.GetComponentInParent<PrefabIdentifier>();
			if (componentInParent)
			{
				BuildingHealth component = componentInParent.GetComponent<BuildingHealth>();
				if (component)
				{
					component.LocalizedHit(data);
				}
			}
		}

		public void OnExplode(Explode.Data explodeData)
		{
			BuildingExplosion componentInParent = base.transform.GetComponentInParent<BuildingExplosion>();
			if (componentInParent && !componentInParent.Exploding)
			{
				Scene.ActiveMB.StartCoroutine(componentInParent.OnExplode(explodeData));
			}
		}
	}
}
