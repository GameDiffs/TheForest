using System;
using UnityEngine;

namespace TheForest.UI
{
	public class ActionIconGuiTexture : MonoBehaviour
	{
		public InputMappingIcons.Actions _action;

		public GUITexture _texture;

		public GUIText _text;

		public bool _useTextBacking = true;

		private int _version;

		private void Update()
		{
			if (this._version != InputMappingIcons.Version)
			{
				this._texture.texture = InputMappingIcons.GetTextureFor(this._action);
				if (!InputMappingIcons.UsesText(this._action))
				{
					this._text.enabled = false;
					this._texture.enabled = true;
				}
				else
				{
					this._text.text = InputMappingIcons.GetMappingFor(this._action);
					this._text.enabled = true;
					this._texture.enabled = this._useTextBacking;
				}
				this._version = InputMappingIcons.Version;
			}
		}
	}
}
