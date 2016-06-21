using System;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.UI
{
	public class VirtualCursor : MonoBehaviour
	{
		public float _mouseSpeed = 5f;

		public Texture2D _mouseIcon;

		public static VirtualCursor Instance;

		private float _mouseSpeedScaled;

		private bool _active;

		private Vector3 _position;

		public Vector3 Position
		{
			get
			{
				return this._position;
			}
		}

		private void Awake()
		{
			VirtualCursor.Instance = this;
			Cursor.SetCursor(this._mouseIcon, new Vector2(45f, 24f), CursorMode.Auto);
		}

		private void OnDestroy()
		{
			if (VirtualCursor.Instance == this)
			{
				VirtualCursor.Instance = null;
			}
		}

		private void LateUpdate()
		{
			if (TheForest.Utils.Input.IsMouseLocked)
			{
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
				if (this._active)
				{
					this._active = false;
					Scene.HudGui.MouseSprite.gameObject.SetActive(false);
				}
			}
			else if (TheForest.Utils.Input.IsGamePad && LocalPlayer.Inventory && (LocalPlayer.Inventory.CurrentView == PlayerInventory.PlayerViews.Inventory || LocalPlayer.Inventory.CurrentView == PlayerInventory.PlayerViews.Book))
			{
				this._mouseSpeedScaled = this._mouseSpeed * (float)Screen.width / 900f;
				if (!this._active)
				{
					Cursor.lockState = CursorLockMode.Locked;
					Cursor.visible = false;
					Scene.HudGui.MouseSprite.gameObject.SetActive(true);
					this._active = true;
					this._position = TheForest.Utils.Input.mousePosition;
				}
				this._position.x = Mathf.Clamp(this._position.x + TheForest.Utils.Input.GetAxis("Mouse X") * this._mouseSpeedScaled, 0f, (float)Screen.width);
				this._position.y = Mathf.Clamp(this._position.y + TheForest.Utils.Input.GetAxis("Mouse Y") * this._mouseSpeedScaled, 0f, (float)Screen.height);
				this._position.z = 1f;
				if (LocalPlayer.Inventory.CurrentView == PlayerInventory.PlayerViews.Book)
				{
					Scene.HudGui.MouseSprite.position = Scene.HudGui.bookUICam.GetComponent<Camera>().ScreenToWorldPoint(this._position);
				}
				else
				{
					Scene.HudGui.MouseSprite.position = Scene.HudGui.GuiCamC.ScreenToWorldPoint(this._position);
				}
			}
			else if (this._active)
			{
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
				this._active = false;
				Scene.HudGui.MouseSprite.gameObject.SetActive(false);
			}
			else
			{
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
		}
	}
}
