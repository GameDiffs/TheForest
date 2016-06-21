using Bolt;
using System;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Buildings.World
{
	[DoNotSerializePublic]
	public class GuiTextureColor : EntityBehaviour<IStickMarkerState>
	{
		[SerializeThis]
		public Color _color;

		public GUITexture _target;

		public Renderer _targetRenderer;

		public GameObject _sheenIcon;

		public GameObject _pickupIcon;

		private void Awake()
		{
			base.enabled = false;
		}

		private void Update()
		{
			if (TheForest.Utils.Input.GetButtonDown("Rotate"))
			{
				LocalPlayer.Sfx.PlayWhoosh();
				this._color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), this._color.a);
				this.SetTargetColor();
				if (BoltNetwork.isRunning)
				{
					this.SetMpStateColor();
				}
			}
		}

		private void OnDeserialized()
		{
			if (this._color.a > 0.5f)
			{
				this._color.a = 0.196078435f;
			}
			this.SetTargetColor();
		}

		private void GrabEnter()
		{
			this._sheenIcon.SetActive(false);
			this._pickupIcon.SetActive(true);
			base.enabled = true;
		}

		private void GrabExit()
		{
			this._sheenIcon.SetActive(true);
			this._pickupIcon.SetActive(false);
			base.enabled = false;
		}

		private void SetTargetColor()
		{
			this._target.color = this._color;
			if (this._targetRenderer)
			{
				MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
				this._targetRenderer.GetPropertyBlock(materialPropertyBlock);
				materialPropertyBlock.SetColor("_Color", this._color);
				this._targetRenderer.SetPropertyBlock(materialPropertyBlock);
			}
		}

		public override void Attached()
		{
			if (BoltNetwork.isServer)
			{
				this.SetMpStateColor();
			}
			base.state.AddCallback("Color", new PropertyCallbackSimple(this.SetTargetColorMp));
		}

		private void SetTargetColorMp()
		{
			this._color = new Color(base.state.Color.x, base.state.Color.y, base.state.Color.z, this._color.a);
			this.SetTargetColor();
		}

		private void SetMpStateColor()
		{
			Vector3 color = new Vector3(this._color.r, this._color.g, this._color.b);
			base.state.Color = color;
		}
	}
}
