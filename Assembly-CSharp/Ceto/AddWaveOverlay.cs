using System;
using UnityEngine;

namespace Ceto
{
	[AddComponentMenu("Ceto/Overlays/AddWaveOverlay")]
	public class AddWaveOverlay : AddWaveOverlayBase
	{
		public enum ROTATION
		{
			RELATIVE_TO_PARENT,
			INDEPENDANT_TO_PARENT
		}

		public bool checkTextures = true;

		public OverlayHeightTexture heightTexture = new OverlayHeightTexture();

		public OverlayNormalTexture normalTexture = new OverlayNormalTexture();

		public OverlayFoamTexture foamTexture = new OverlayFoamTexture();

		public OverlayClipTexture clipTexture = new OverlayClipTexture();

		public float width = 10f;

		public float height = 10f;

		public AddWaveOverlay.ROTATION m_rotationMode;

		[Range(0f, 360f)]
		public float rotation;

		private bool m_registered;

		protected override void Start()
		{
			if (this.checkTextures)
			{
				if (!this.heightTexture.ignoreQuerys)
				{
					base.CheckCanSampleTex(this.heightTexture.tex, "height texture");
				}
				if (!this.heightTexture.ignoreQuerys)
				{
					base.CheckCanSampleTex(this.heightTexture.mask, "height mask");
				}
				if (!this.clipTexture.ignoreQuerys)
				{
					base.CheckCanSampleTex(this.clipTexture.tex, "clip texture");
				}
			}
			Vector2 halfSize = new Vector2(this.width * 0.5f, this.height * 0.5f);
			this.m_overlays.Add(new WaveOverlay(base.transform.position, this.Rotation(), halfSize, 0f));
			this.m_overlays[0].HeightTex = this.heightTexture;
			this.m_overlays[0].NormalTex = this.normalTexture;
			this.m_overlays[0].FoamTex = this.foamTexture;
			this.m_overlays[0].ClipTex = this.clipTexture;
			if (!this.m_registered && Ocean.Instance != null)
			{
				Ocean.Instance.OverlayManager.Add(this.m_overlays[0]);
				this.m_registered = true;
			}
		}

		protected override void Update()
		{
			if (this.m_overlays == null || this.m_overlays.Count != 1)
			{
				return;
			}
			if (!this.m_registered && Ocean.Instance != null)
			{
				Ocean.Instance.OverlayManager.Add(this.m_overlays[0]);
				this.m_registered = true;
			}
			this.m_overlays[0].Position = base.transform.position;
			this.m_overlays[0].HalfSize = new Vector2(this.width * 0.5f, this.height * 0.5f);
			this.m_overlays[0].Rotation = this.Rotation();
			this.m_overlays[0].UpdateOverlay();
		}

		private float Rotation()
		{
			AddWaveOverlay.ROTATION rotationMode = this.m_rotationMode;
			if (rotationMode == AddWaveOverlay.ROTATION.RELATIVE_TO_PARENT)
			{
				return base.transform.eulerAngles.y + this.rotation;
			}
			if (rotationMode != AddWaveOverlay.ROTATION.INDEPENDANT_TO_PARENT)
			{
				return this.rotation;
			}
			return this.rotation;
		}

		private void OnDrawGizmos()
		{
			if (!base.enabled)
			{
				return;
			}
			Vector3 s = new Vector3(this.width * 0.5f, 1f, this.height * 0.5f);
			Vector3 position = base.transform.position;
			Matrix4x4 matrix = Matrix4x4.TRS(new Vector3(position.x, 0f, position.z), Quaternion.Euler(0f, this.Rotation(), 0f), s);
			Gizmos.color = Color.yellow;
			Gizmos.matrix = matrix;
			Gizmos.DrawWireCube(Vector3.zero, new Vector3(2f, 10f, 2f));
		}
	}
}
