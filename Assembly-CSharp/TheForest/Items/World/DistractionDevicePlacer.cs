using FMOD.Studio;
using System;
using TheForest.Items.Inventory;
using TheForest.Items.Special;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Items.World
{
	[AddComponentMenu("Items/World/Distraction Device Placer")]
	public class DistractionDevicePlacer : MonoBehaviour
	{
		[ItemIdPicker]
		public int _itemId;

		public Item.EquipmentSlot _slot;

		public LayerMask _layerMask;

		public GameObject _distractionDevicePrefab;

		public PlayerInventory _player;

		public GameObject _placeIconSheen;

		public GameObject _targetObject;

		public bool _playAudio;

		private PlayerSfx _playerSfx;

		private RaycastHit _hit;

		private void Start()
		{
			this._layerMask = 1050625;
			base.enabled = false;
			this._playerSfx = this._player.GetComponent<PlayerSfx>();
			this._placeIconSheen.SetActive(false);
		}

		private void FixedUpdate()
		{
			if (this._targetObject)
			{
				if (this._player.HasInSlot(this._slot, this._itemId))
				{
					if (Physics.Raycast(LocalPlayer.MainCamTr.position, LocalPlayer.MainCamTr.forward, out this._hit, 10f, this._layerMask.value))
					{
						if (!this._placeIconSheen.activeSelf)
						{
							this._placeIconSheen.transform.parent = null;
							this._placeIconSheen.SetActive(true);
						}
						if (this._targetObject.CompareTag("trapTrigger"))
						{
							this._placeIconSheen.transform.position = this._targetObject.transform.position;
						}
						else
						{
							this._placeIconSheen.transform.position = this._hit.point + LocalPlayer.MainCamTr.forward * -0.1f;
						}
						if (TheForest.Utils.Input.GetButtonDown("Craft") && this._player.RemoveItem(this._itemId, 1, false))
						{
							Vector3 forward = this._hit.point - this._hit.transform.position;
							forward.y = 0f;
							Vector3 position = this._hit.point;
							if (this._targetObject.CompareTag("trapTrigger"))
							{
								position = this._targetObject.transform.position;
							}
							GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this._distractionDevicePrefab, position, Quaternion.LookRotation(forward));
							if (BoltNetwork.isRunning)
							{
								BoltEntity component = BoltNetwork.Attach(gameObject).GetComponent<BoltEntity>();
								if (component && this._playAudio && WalkmanControler.HasCassetteReady)
								{
									component.GetState<IDistractionDevice>().MusicTrack = WalkmanControler.CurrentTrack + 10;
								}
							}
							else if (this._playAudio)
							{
								EventInstance eventInstance = this._playerSfx.RelinquishMusicTrack();
								if (eventInstance == null && WalkmanControler.HasCassetteReady)
								{
									eventInstance = this._playerSfx.InstantiateMusicTrack(WalkmanControler.CurrentTrack);
								}
								if (eventInstance != null)
								{
									gameObject.SendMessage("ActivateDevice", eventInstance);
								}
								gameObject.SendMessage("SetPlayerSfx", this._playerSfx);
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
			if ((1 << other.gameObject.layer & this._layerMask) > 0)
			{
				this._targetObject = other.gameObject;
				base.enabled = true;
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
			if (this._placeIconSheen)
			{
				this._placeIconSheen.SetActive(false);
				this._placeIconSheen.transform.parent = base.transform;
			}
			base.enabled = false;
		}
	}
}
