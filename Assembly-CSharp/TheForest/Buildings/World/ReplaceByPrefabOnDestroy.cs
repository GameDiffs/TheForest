using System;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Buildings.World
{
	public class ReplaceByPrefabOnDestroy : MonoBehaviour
	{
		public Transform _replaceByPrefab;

		public bool _onlyIfActive = true;

		public bool _oneFrameDelay = true;

		private void OnDestroy()
		{
			if (this._replaceByPrefab && (!this._onlyIfActive || base.gameObject.activeSelf) && Prefabs.Instance && !BoltNetwork.isClient)
			{
				Transform parent = base.transform.parent;
				if (BoltNetwork.isServer && this._replaceByPrefab.GetComponent<BoltEntity>())
				{
					if (this._oneFrameDelay)
					{
						Prefabs.Instance.SpawnNextFrameMP(this._replaceByPrefab, base.transform.position, base.transform.rotation, parent);
					}
					else
					{
						BoltEntity boltEntity = BoltNetwork.Instantiate(this._replaceByPrefab.gameObject, base.transform.position, base.transform.rotation);
						if (parent)
						{
							boltEntity.transform.parent = parent;
						}
					}
				}
				else if (this._oneFrameDelay)
				{
					Prefabs.Instance.SpawnNextFrame(this._replaceByPrefab, base.transform.position, base.transform.rotation, parent);
				}
				else
				{
					Transform transform = (Transform)UnityEngine.Object.Instantiate(this._replaceByPrefab, base.transform.position, base.transform.rotation);
					if (parent)
					{
						transform.parent = parent;
					}
				}
			}
		}
	}
}
