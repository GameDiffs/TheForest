using System;
using UnityEngine;

namespace TheForest.UI
{
	public class ActionIconUITexture : MonoBehaviour
	{
		public InputMappingIcons.Actions _action;

		public UITexture _texture;

		public UILabel _label;

		private int _version;

		private int _startHeight;

		private void Awake()
		{
			this._startHeight = this._texture.height;
		}

		private void Update()
		{
			if (this._version != InputMappingIcons.Version)
			{
				this._texture.height = this._startHeight;
				if (!InputMappingIcons.UsesText(this._action))
				{
					this._texture.mainTexture = InputMappingIcons.GetTextureFor(this._action);
					this._label.enabled = false;
					float num = (float)this._texture.mainTexture.width / (float)this._texture.mainTexture.height;
					if (num > 1.2f)
					{
						this._texture.height = Mathf.RoundToInt((float)this._texture.height * 0.666f);
					}
					this._texture.width = Mathf.RoundToInt(num * (float)this._texture.height);
				}
				else
				{
					this._texture.mainTexture = InputMappingIcons.GetTextureFor(this._action);
					this._label.text = InputMappingIcons.GetMappingFor(this._action);
					this._label.enabled = true;
					float num2 = (float)this._label.width / (float)this._label.height;
					if (num2 > 1.5f)
					{
						this._texture.width = Mathf.RoundToInt((float)this._startHeight * num2);
					}
					else
					{
						this._texture.width = this._startHeight;
					}
				}
				this._version = InputMappingIcons.Version;
			}
		}
	}
}
