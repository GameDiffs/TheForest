using Bolt;
using System;
using System.Collections.Generic;
using TheForest.Items;
using TheForest.Utils;
using UniLinq;
using UnityEngine;

namespace TheForest.Buildings.World
{
	[DoNotSerializePublic]
	public class EffigyArchitect : MonoBehaviour
	{
		[Serializable]
		public class Part
		{
			public Vector3 _position;

			public Vector3 _rotation;

			[ItemIdPicker]
			public int _itemId;
		}

		private const int TorsoFakeId = -2;

		private const float MaxEffigyRange = 80f;

		[ItemIdPicker(Item.Types.Equipment)]
		public int _baseItemId;

		[ItemIdPicker(Item.Types.Equipment)]
		public int[] _partsItemIds;

		public LayerMask _baseLayers;

		public LayerMask _partsLayers;

		public SphereCollider _effigyRange;

		public enableEffigy _enableEffigy;

		[SerializeThis]
		public List<EffigyArchitect.Part> _parts;

		private int _currentPreviewItemId = -1;

		private Transform _currentPreviewTr;

		private float _rotationAngle;

		[SerializeThis]
		private int _partCount;

		private bool _canLight;

		private void Awake()
		{
			this._effigyRange.radius = 0f;
			this._enableEffigy.lightBool = false;
			base.enabled = false;
			this._parts = new List<EffigyArchitect.Part>();
		}

		private void Update()
		{
			if (!this._enableEffigy.lightBool || !this._canLight)
			{
				if (LocalPlayer.AnimControl.carry)
				{
					if (this.CheckTorsoIsClearedOut())
					{
						this.CastForItem(-2, this._partsLayers);
					}
				}
				else if (LocalPlayer.Inventory.RightHand)
				{
					if (LocalPlayer.Inventory.RightHand._itemId == this._baseItemId)
					{
						this.CastForItem(LocalPlayer.Inventory.RightHand._itemId, this._baseLayers);
					}
					else if (this._partsItemIds.Any((int i) => i == LocalPlayer.Inventory.RightHand._itemId))
					{
						this.CastForItem(LocalPlayer.Inventory.RightHand._itemId, this._partsLayers);
					}
					else
					{
						this.UpdateCurrentPreviewItem(-1);
					}
				}
				else
				{
					this.UpdateCurrentPreviewItem(-1);
				}
				this.CheckRotate();
				this.CheckPlace();
			}
		}

		private void OnSerializing()
		{
			this._partCount = this._parts.Count;
		}

		private void OnDeserialized()
		{
			this._parts.RemoveRange(this._partCount, this._parts.Count - this._partCount);
			for (int i = 0; i < this._parts.Count; i++)
			{
				this.SpawnPartReal(this._parts[i], false);
			}
		}

		private void GrabEnter()
		{
			base.enabled = true;
		}

		private void GrabExit()
		{
			base.enabled = false;
			this.UpdateCurrentPreviewItem(-1);
			Scene.HudGui.PlacePartIcon.SetActive(false);
			Scene.HudGui.RotateIcon.SetActive(false);
		}

		private void OnBeginCollapse()
		{
			for (int i = base.transform.childCount - 1; i >= 0; i--)
			{
				Transform child = base.transform.GetChild(i);
				child.parent = null;
				if (!child.GetComponent<Collider>())
				{
					CapsuleCollider capsuleCollider = child.gameObject.AddComponent<CapsuleCollider>();
					capsuleCollider.radius = 0.1f;
					capsuleCollider.height = 1.5f;
					capsuleCollider.direction = 1;
				}
				Rigidbody rigidbody = child.gameObject.AddComponent<Rigidbody>();
				if (rigidbody)
				{
					rigidbody.AddForce((child.position.normalized + Vector3.up) * 2.5f, ForceMode.Impulse);
					rigidbody.AddRelativeTorque(Vector3.up * 2f, ForceMode.Impulse);
				}
				destroyAfter destroyAfter = child.gameObject.AddComponent<destroyAfter>();
				destroyAfter.destroyTime = 2.5f;
			}
		}

		private void CastForItem(int itemId, LayerMask layers)
		{
			RaycastHit hit;
			if (Physics.SphereCast(LocalPlayer.MainCamTr.position, 0.4f, LocalPlayer.MainCamTr.forward, out hit, 4f, layers) && hit.transform.GetComponentInParent<EffigyArchitect>() == this)
			{
				this.UpdateCurrentPreviewItem(itemId);
				this.PositionCurrentPreviewItem(hit);
			}
		}

		private void UpdateCurrentPreviewItem(int itemId)
		{
			if (this._currentPreviewItemId != itemId)
			{
				if (this._currentPreviewTr)
				{
					UnityEngine.Object.Destroy(this._currentPreviewTr.gameObject);
				}
				this._currentPreviewTr = null;
				Item item = (itemId <= 0) ? null : ItemDatabase.ItemById(itemId);
				if (item != null && item._bareItemPrefab)
				{
					this._rotationAngle = 0f;
					this._currentPreviewTr = UnityEngine.Object.Instantiate<Transform>(item._bareItemPrefab);
					this._currentPreviewTr.GetComponentInChildren<Renderer>().sharedMaterial = Prefabs.Instance.GhostClear;
					UnityEngine.Object.Destroy(this._currentPreviewTr.GetComponentInChildren<Collider>());
					this._currentPreviewItemId = itemId;
				}
				else if (itemId == -2)
				{
					this._currentPreviewTr = UnityEngine.Object.Instantiate<Transform>(Prefabs.Instance.TorsoGhostPrefab);
					this._currentPreviewItemId = itemId;
				}
				else if (itemId == -1)
				{
					this._currentPreviewItemId = itemId;
				}
			}
		}

		private void PositionCurrentPreviewItem(RaycastHit hit)
		{
			if (this._currentPreviewTr)
			{
				this._currentPreviewTr.position = hit.point;
				this._currentPreviewTr.LookAt(hit.point + hit.normal);
				if (!Mathf.Approximately(this._rotationAngle, 0f))
				{
					this._currentPreviewTr.rotation = Quaternion.AngleAxis(this._rotationAngle, hit.normal) * this._currentPreviewTr.rotation;
				}
			}
		}

		private bool CheckTorsoIsClearedOut()
		{
			if (LocalPlayer.AnimControl.placedBodyGo)
			{
				CoopSliceAndDiceMutant component = LocalPlayer.AnimControl.placedBodyGo.GetComponent<CoopSliceAndDiceMutant>();
				if (component)
				{
					if (component.BodyParts.Sum((DamageCorpse b) => (!(b == null)) ? 0 : 1) == 5)
					{
						return true;
					}
				}
			}
			return false;
		}

		private void CheckRotate()
		{
			if (this._currentPreviewItemId != -1)
			{
				Scene.HudGui.RotateIcon.SetActive(true);
				if (TheForest.Utils.Input.GetButton("Rotate"))
				{
					this._rotationAngle += (float)(((TheForest.Utils.Input.GetAxis("Rotate") <= 0f) ? -1 : 1) * 75) * Time.deltaTime;
				}
			}
			else
			{
				Scene.HudGui.RotateIcon.SetActive(false);
			}
		}

		private void CheckPlace()
		{
			if (this._currentPreviewItemId != -1)
			{
				Scene.HudGui.PlacePartIcon.SetActive(true);
				if (TheForest.Utils.Input.GetButtonDown("Take"))
				{
					LocalPlayer.Sfx.PlayHammer();
					if (LocalPlayer.AnimControl.carry)
					{
						if (BoltNetwork.isRunning)
						{
							LocalPlayer.AnimControl.DropBody(true);
						}
						else
						{
							UnityEngine.Object.Destroy(LocalPlayer.AnimControl.DropBody());
						}
					}
					else
					{
						LocalPlayer.Inventory.ShuffleRemoveRightHandItem();
					}
					EffigyArchitect.Part part = new EffigyArchitect.Part
					{
						_itemId = this._currentPreviewItemId,
						_position = this._currentPreviewTr.position,
						_rotation = this._currentPreviewTr.rotation.eulerAngles
					};
					if (!BoltNetwork.isRunning)
					{
						this._parts.Add(part);
					}
					this.SpawnPart(part);
					this.UpdateCurrentPreviewItem(-1);
					Scene.HudGui.PlacePartIcon.SetActive(false);
				}
			}
			else
			{
				Scene.HudGui.PlacePartIcon.SetActive(false);
			}
		}

		private void SpawnPart(EffigyArchitect.Part part)
		{
			if (BoltNetwork.isRunning)
			{
				AddEffigyPart addEffigyPart = AddEffigyPart.Create(GlobalTargets.OnlyServer);
				addEffigyPart.Effigy = base.GetComponentInParent<BoltEntity>();
				addEffigyPart.ItemId = part._itemId;
				addEffigyPart.Position = part._position;
				addEffigyPart.Rotation = part._rotation;
				addEffigyPart.Send();
			}
			else
			{
				this.SpawnPartReal(part, false);
			}
		}

		public void SpawnPartReal(EffigyArchitect.Part part, bool mp_addpart)
		{
			if (BoltNetwork.isRunning && mp_addpart)
			{
				this._parts.Add(part);
			}
			Item item = (part._itemId <= 0) ? null : ItemDatabase.ItemById(part._itemId);
			Transform transform = null;
			if (item != null && item._bareItemPrefab)
			{
				transform = item._bareItemPrefab;
				if (transform && item._id != this._baseItemId)
				{
					if (this._effigyRange.radius < 10f)
					{
						this._effigyRange.radius = 10f;
					}
					if (this._effigyRange.radius < 80f)
					{
						this._canLight = true;
						this._enableEffigy.lightBool = false;
						this._effigyRange.radius += 1.5f;
						this._enableEffigy.duration = Mathf.Clamp(this._enableEffigy.duration + 15f, 600f, 1200f);
					}
					if (this._enableEffigy.duration < 1200f)
					{
						this._enableEffigy.duration += 50f;
					}
				}
			}
			else if (part._itemId == -2)
			{
				transform = Prefabs.Instance.TorsoPrefab;
				if (transform)
				{
					if (this._effigyRange.radius < 10f)
					{
						this._effigyRange.radius = 10f;
					}
					if (this._effigyRange.radius < 80f)
					{
						this._canLight = true;
						this._enableEffigy.lightBool = false;
						this._effigyRange.radius += 3f;
					}
					if (this._enableEffigy.duration < 1200f)
					{
						this._enableEffigy.duration += 50f;
					}
				}
			}
			if (transform)
			{
				Transform transform2 = UnityEngine.Object.Instantiate<Transform>(transform);
				transform2.position = part._position;
				transform2.rotation = Quaternion.Euler(part._rotation.x, part._rotation.y, part._rotation.z);
				transform2.parent = base.transform;
				if (BoltNetwork.isServer)
				{
					BoltNetwork.Attach(transform2.gameObject).GetComponent<BoltEntity>().GetState<IPartState>().Effigy = base.GetComponentInParent<BoltEntity>();
				}
			}
		}
	}
}
