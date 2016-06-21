using System;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.UI.Multiplayer
{
	public class PlayerName : MonoBehaviour
	{
		public enum Modes
		{
			Hidden,
			Shown
		}

		public BoltEntity _entity;

		public bool _showing = true;

		public float _nearRange = 4f;

		public float _farRange = 9f;

		public float _nearRangeTextSize = 24f;

		public float _farRangeTextSize = 10f;

		private IPlayerState _playerState;

		private PlayerOverlay _overlay;

		private Transform _overlayTr;

		private PlayerName.Modes _mode;

		private void Awake()
		{
			base.enabled = false;
		}

		private void LateUpdate()
		{
			if (LocalPlayer.MainCam)
			{
				if (!LocalPlayer.Inventory.enabled || this._playerState.CurrentView <= 0 || this._playerState.crouch > 0f)
				{
					this._showing = false;
				}
				else if (LocalPlayer.Inventory.CurrentView == PlayerInventory.PlayerViews.World != this._showing)
				{
					this._showing = (PlayerPreferences.ShowPlayerNamesMP && !this._showing);
				}
				if (!this._showing || Vector3.Dot((base.transform.position - LocalPlayer.MainCamTr.position).normalized, LocalPlayer.MainCamTr.forward) < 0.6f)
				{
					if (this._mode != PlayerName.Modes.Hidden)
					{
						this._mode = PlayerName.Modes.Hidden;
						this._overlay.gameObject.SetActive(false);
					}
				}
				else
				{
					if (this._mode != PlayerName.Modes.Shown)
					{
						this._mode = PlayerName.Modes.Shown;
						this._overlay.gameObject.SetActive(true);
					}
					float num = Vector3.Distance(LocalPlayer.MainCamTr.position, base.transform.position);
					num = Mathf.Clamp(num, this._nearRange, this._farRange);
					float z;
					if (this._playerState.CurrentView != 7)
					{
						z = Mathf.Lerp(this._nearRangeTextSize, this._farRangeTextSize, (num - this._nearRange) / (this._farRange - this._nearRange));
					}
					else
					{
						z = this._nearRangeTextSize;
					}
					Vector3 position = LocalPlayer.MainCam.WorldToViewportPoint(base.transform.position);
					position.z = z;
					this._overlayTr.position = Scene.HudGui.ActionIconCams.ViewportToWorldPoint(position);
				}
			}
		}

		private void OnDestroy()
		{
			if (this._overlay)
			{
				UnityEngine.Object.Destroy(this._overlay.gameObject);
			}
		}

		public void Init(string name)
		{
			this._playerState = this._entity.GetState<IPlayerState>();
			PlayerOverlay playerOverlay = Scene.HudGui.PlayerOverlay;
			if (!this._overlay)
			{
				this._overlay = (PlayerOverlay)UnityEngine.Object.Instantiate(playerOverlay, playerOverlay.transform.position, playerOverlay.transform.rotation);
			}
			this._overlay._name.text = name;
			this._overlay.gameObject.SetActive(false);
			this._overlay.transform.parent = playerOverlay.transform.parent;
			this._overlay.transform.localScale = Vector3.one;
			this._overlayTr = this._overlay.transform;
			this.OnCurrentViewChanged();
			base.enabled = true;
			if (Scene.HudGui.MpPlayerList.gameObject.activeInHierarchy)
			{
				Scene.HudGui.MpPlayerList.Refresh();
			}
		}

		public void OnCurrentViewChanged()
		{
			if (this._playerState == null)
			{
				return;
			}
			PlayerInventory.PlayerViews currentView = (PlayerInventory.PlayerViews)this._playerState.CurrentView;
			this._overlay._bookIcon.enabled = (PlayerPreferences.ShowPlayerNamesMP && currentView == PlayerInventory.PlayerViews.Book);
			this._overlay._inventoryIcon.enabled = (PlayerPreferences.ShowPlayerNamesMP && currentView == PlayerInventory.PlayerViews.Inventory);
			this._overlay._menuIcon.enabled = (PlayerPreferences.ShowPlayerNamesMP && currentView == PlayerInventory.PlayerViews.Pause);
			this._overlay._deathIcon.enabled = (PlayerPreferences.ShowPlayerNamesMP && currentView == PlayerInventory.PlayerViews.Death);
			this._overlay._sleepIcon.enabled = (PlayerPreferences.ShowPlayerNamesMP && currentView == PlayerInventory.PlayerViews.Sleep);
		}
	}
}
