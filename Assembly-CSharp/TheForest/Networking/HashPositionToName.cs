using Bolt;
using System;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Networking
{
	public class HashPositionToName : EntityBehaviour
	{
		public override void Attached()
		{
			base.name = HashPositionToName.GetHash(base.transform.position);
			if (BoltNetwork.isServer && !this.entity.isOwner)
			{
				BoltNetwork.Instantiate(Prefabs.Instance.HashPositionToNamePrefab, base.transform.position, base.transform.rotation);
				BoltNetwork.Destroy(base.gameObject);
			}
			else
			{
				BreakStoneSimple[] array = UnityEngine.Object.FindObjectsOfType<BreakStoneSimple>();
				for (int i = 0; i < array.Length; i++)
				{
					array[i].ClientHashExplodeCheck(base.name);
				}
			}
		}

		public static string GetHash(Vector3 position)
		{
			int num = 9;
			return string.Concat(new object[]
			{
				(int)position.x / num,
				"_",
				(int)position.y / num,
				"_",
				(int)position.z / num
			});
		}
	}
}
