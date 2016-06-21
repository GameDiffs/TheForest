using System;
using TheForest.Items.Craft;
using TheForest.Items.Inventory;
using UnityEngine;

namespace TheForest.Items.World
{
	public class ProjectileThrownListener : MonoBehaviour
	{
		public InventoryItemView _targetView;

		public WeaponStatUpgrade.Types _bonus;

		public GameObject _prefab;

		public Transform _position;

		private void OnProjectileThrown(GameObject projectileGO)
		{
			if (this._targetView.ActiveBonus == this._bonus)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this._prefab);
				gameObject.transform.parent = projectileGO.transform;
				gameObject.transform.localPosition = this._position.localPosition;
				gameObject.transform.localRotation = this._position.localRotation;
				Rigidbody component = gameObject.GetComponent<Rigidbody>();
				if (component)
				{
					component.useGravity = false;
					component.isKinematic = true;
				}
				if (!gameObject.GetComponent<destroyAfter>())
				{
					gameObject.AddComponent<destroyAfter>().destroyTime = 20f;
				}
				if (BoltNetwork.isRunning)
				{
					BoltEntity component2 = gameObject.GetComponent<BoltEntity>();
					if (component2 && !component2.isAttached)
					{
						BoltNetwork.Attach(component2);
					}
				}
				this._targetView.ActiveBonus = (WeaponStatUpgrade.Types)(-1);
			}
		}
	}
}
