using System;
using UnityEngine;

namespace TheForest.UI
{
	public class ActionIconUISprite : MonoBehaviour
	{
		public InputMappingIcons.Actions _action;

		public UISprite _sprite;

		public UILabel _label;

		public bool _invertScale;

		private int _version;

		private int _startHeight;

		private void Awake()
		{
			this._startHeight = this._sprite.height;
		}

		private void Update()
		{
			if (this._version != InputMappingIcons.Version)
			{
				this._sprite.height = this._startHeight;
				if (!InputMappingIcons.UsesText(this._action))
				{
					this._sprite.spriteName = InputMappingIcons.GetMappingFor(this._action);
					this._label.enabled = false;
					UISpriteData atlasSprite = this._sprite.GetAtlasSprite();
					if (atlasSprite != null)
					{
						float num = (float)atlasSprite.width / (float)atlasSprite.height;
						if (num > 1.2f)
						{
							this._sprite.height = Mathf.RoundToInt((float)this._sprite.height * 0.666f);
						}
						this._sprite.width = Mathf.RoundToInt(num * (float)this._sprite.height);
					}
					else
					{
						Debug.LogError("Missing sprite: " + this._sprite.spriteName);
					}
				}
				else
				{
					this._sprite.spriteName = InputMappingIcons.TextIconBacking.name;
					this._label.text = InputMappingIcons.GetMappingFor(this._action);
					this._label.enabled = true;
					float num2;
					if (this._invertScale)
					{
						num2 = (float)this._label.width * 1f / this._label.transform.localScale.x / (float)this._startHeight;
					}
					else
					{
						num2 = (float)this._label.width * 1f * this._label.transform.localScale.x / (float)this._startHeight;
					}
					if (num2 > 1.5f)
					{
						this._sprite.width = Mathf.RoundToInt((float)this._startHeight * num2);
					}
					else
					{
						this._sprite.width = this._startHeight;
					}
				}
				this._version = InputMappingIcons.Version;
			}
		}
	}
}
