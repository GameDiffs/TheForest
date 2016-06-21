using System;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Items.World
{
	[DoNotSerializePublic]
	public class ActivateWithItem : MonoBehaviour
	{
		[ItemIdPicker]
		public int _itemId;

		public GameObject _target;

		public GameObject _itemIcon;

		public GameObject _itemText;

		public Color _greyColor;

		public Color _redColor;

		[SerializeThis]
		private bool _filled;

		private void Awake()
		{
			if (!LevelSerializer.IsDeserializing)
			{
				this.SetUpIcons();
			}
			base.enabled = false;
		}

		private void Update()
		{
			if (LocalPlayer.Inventory.Owns(this._itemId))
			{
				Scene.HudGui.AddIcon.SetActive(true);
				this.SetIconColor(this._greyColor);
				if (TheForest.Utils.Input.GetButtonDown("Take") && LocalPlayer.Inventory.RemoveItem(this._itemId, 1, false))
				{
					this._target.SetActive(true);
					this._filled = true;
					base.enabled = false;
					this.ToggleIcons(false);
					Scene.HudGui.AddIcon.SetActive(false);
					UnityEngine.Object.Destroy(base.GetComponent<Collider>());
				}
			}
			else
			{
				this.SetIconColor(this._redColor);
			}
		}

		private void OnDeserialized()
		{
			if (this._filled)
			{
				this._target.SetActive(true);
				UnityEngine.Object.Destroy(base.GetComponent<Collider>());
			}
			else
			{
				this.SetUpIcons();
			}
		}

		private void GrabEnter()
		{
			if (!this._filled)
			{
				base.enabled = true;
				this.ToggleIcons(true);
			}
		}

		private void GrabExit()
		{
			if (base.enabled)
			{
				base.enabled = false;
				this.ToggleIcons(false);
				Scene.HudGui.AddIcon.SetActive(false);
			}
		}

		private void SetUpIcons()
		{
			if (Application.isPlaying)
			{
				float num = 1f / ((float)Screen.width / 70f);
				Vector3 position = new Vector3(0.5f - num * -0.5f, 0.15f, 0f);
				if (this._itemIcon.transform.parent != base.transform)
				{
					this._itemIcon = UnityEngine.Object.Instantiate<GameObject>(this._itemIcon);
					this._itemIcon.transform.parent = base.transform;
					this._itemIcon.transform.position = position;
					this._itemText = UnityEngine.Object.Instantiate<GameObject>(this._itemText);
					this._itemText.transform.parent = base.transform;
					position.z += 1f;
					this._itemText.transform.position = position;
					this._itemText.GetComponent<GUIText>().text = "0/1";
					position.z -= 1f;
					position.x += num;
				}
				this._itemIcon.SetActive(false);
				this._itemText.SetActive(false);
			}
		}

		private void ToggleIcons(bool onoff)
		{
			this._itemIcon.SetActive(onoff);
			this._itemText.SetActive(onoff);
		}

		private void SetIconColor(Color c)
		{
			this._itemIcon.GetComponent<GUITexture>().color = c;
		}
	}
}
