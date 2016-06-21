using Bolt;
using System;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Items.World
{
	[AddComponentMenu("Items/World/Drawings Placer")]
	public class DrawingsPlacer : MonoBehaviour
	{
		[ItemIdPicker]
		public int _itemId;

		public LayerMask _layerMask;

		public DrawingsInventoryItemView _diiv;

		public GameObject _placeIconSheen;

		private Transform _targetObjectRoot;

		private GameObject _targetObject;

		private RaycastHit _hit;

		private void Start()
		{
			this._layerMask = 2097152;
			base.enabled = false;
			this._placeIconSheen.SetActive(false);
		}

		private void FixedUpdate()
		{
			if (this._targetObject)
			{
				if (LocalPlayer.Inventory.HasInSlot(Item.EquipmentSlot.RightHand, this._itemId))
				{
					if (Physics.Raycast(LocalPlayer.MainCamTr.position, LocalPlayer.MainCamTr.forward, out this._hit, 10f, this._layerMask.value))
					{
						if (!this._placeIconSheen.activeSelf)
						{
							this._placeIconSheen.transform.parent = null;
							this._placeIconSheen.SetActive(true);
						}
						this._placeIconSheen.transform.position = this._hit.point + LocalPlayer.MainCamTr.forward * -0.1f;
						if (TheForest.Utils.Input.GetButtonDown("Craft") && LocalPlayer.Inventory.ShuffleRemoveRightHandItem())
						{
							int num = this._diiv.PopLast();
							Vector3 normal = this._hit.normal;
							Vector3 point = this._hit.point;
							if (!BoltNetwork.isRunning)
							{
								Transform transform = (Transform)UnityEngine.Object.Instantiate(Prefabs.Instance.TimmyDrawingsPrefab[num], point, Quaternion.LookRotation(normal));
								transform.parent = this._targetObjectRoot;
							}
							else
							{
								PlaceConstruction placeConstruction = PlaceConstruction.Create(GlobalTargets.OnlyServer);
								placeConstruction.Parent = this._targetObjectRoot.GetComponent<BoltEntity>();
								placeConstruction.PrefabId = Prefabs.Instance.TimmyDrawingsPrefab[num].GetComponent<BoltEntity>().prefabId;
								placeConstruction.Position = point;
								placeConstruction.Rotation = Quaternion.LookRotation(normal);
								placeConstruction.Send();
							}
							this._placeIconSheen.SetActive(false);
							this._placeIconSheen.transform.parent = base.transform;
							this.Deactivate();
						}
					}
				}
				else if (this._placeIconSheen.activeSelf)
				{
					this._placeIconSheen.transform.parent = base.transform;
					this._placeIconSheen.SetActive(false);
				}
			}
			else
			{
				this.Deactivate();
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if ((1 << other.gameObject.layer & this._layerMask) > 0 && other.gameObject != this._targetObject)
			{
				BoltEntity componentInParent = other.gameObject.GetComponentInParent<BoltEntity>();
				if (componentInParent)
				{
					this._targetObject = other.gameObject;
					this._targetObjectRoot = componentInParent.transform;
					base.enabled = true;
				}
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.gameObject == this._targetObject)
			{
				this.Deactivate();
			}
		}

		private void Deactivate()
		{
			this._targetObject = null;
			this._targetObjectRoot = null;
			if (this._placeIconSheen)
			{
				this._placeIconSheen.SetActive(false);
				this._placeIconSheen.transform.parent = base.transform;
			}
			base.enabled = false;
		}
	}
}
