using System;
using UnityEngine;

namespace Ceto
{
	[AddComponentMenu("Ceto/Overlays/AddShoreMask")]
	public class AddShoreMask : AddWaveOverlayBase
	{
		public enum ROTATION
		{
			RELATIVE_TO_PARENT,
			INDEPENDANT_TO_PARENT
		}

		public bool checkTextures = true;

		public bool ignoreQuerys;

		public Texture heightMask;

		[Range(0f, 1f)]
		public float heightAlpha = 1f;

		public Texture normalMask;

		[Range(0f, 1f)]
		public float normalAlpha = 1f;

		public Texture edgeFoam;

		[Range(0f, 10f)]
		public float edgeFoamAlpha = 1f;

		public bool textureFoam = true;

		public Texture foamMask;

		[Range(0f, 1f)]
		public float foamMaskAlpha = 1f;

		public Texture clipMask;

		public float width = 10f;

		public float height = 10f;

		public Vector3 offset;

		public AddShoreMask.ROTATION m_rotationMode;

		[Range(0f, 360f)]
		public float rotation;

		private bool m_registered;

		protected override void Start()
		{
			if (this.checkTextures && !this.ignoreQuerys)
			{
				base.CheckCanSampleTex(this.heightMask, "height mask");
				base.CheckCanSampleTex(this.clipMask, "clip mask");
			}
			this.m_overlays.Add(new WaveOverlay());
			this.UpdateOverlay();
		}

		protected override void Update()
		{
			if (this.m_overlays == null || this.m_overlays.Count != 1)
			{
				return;
			}
			this.UpdateOverlay();
		}

		private void UpdateOverlay()
		{
			if (!this.m_registered && Ocean.Instance != null)
			{
				Ocean.Instance.OverlayManager.Add(this.m_overlays[0]);
				this.m_registered = true;
			}
			this.m_overlays[0].Position = base.transform.position + this.offset;
			this.m_overlays[0].HalfSize = new Vector2(this.width * 0.5f, this.height * 0.5f);
			this.m_overlays[0].Rotation = this.Rotation();
			this.m_overlays[0].HeightTex.mask = this.heightMask;
			this.m_overlays[0].HeightTex.maskAlpha = this.heightAlpha;
			this.m_overlays[0].HeightTex.ignoreQuerys = this.ignoreQuerys;
			this.m_overlays[0].NormalTex.mask = this.normalMask;
			this.m_overlays[0].NormalTex.maskAlpha = this.normalAlpha;
			this.m_overlays[0].NormalTex.maskMode = OVERLAY_MASK_MODE.WAVES_AND_OVERLAY_BLEND;
			this.m_overlays[0].FoamTex.tex = this.edgeFoam;
			this.m_overlays[0].FoamTex.alpha = this.edgeFoamAlpha;
			this.m_overlays[0].FoamTex.textureFoam = this.textureFoam;
			this.m_overlays[0].FoamTex.mask = this.foamMask;
			this.m_overlays[0].FoamTex.maskAlpha = this.foamMaskAlpha;
			this.m_overlays[0].ClipTex.tex = this.clipMask;
			this.m_overlays[0].ClipTex.ignoreQuerys = this.ignoreQuerys;
			this.m_overlays[0].UpdateOverlay();
		}

		private float Rotation()
		{
			AddShoreMask.ROTATION rotationMode = this.m_rotationMode;
			if (rotationMode == AddShoreMask.ROTATION.RELATIVE_TO_PARENT)
			{
				return base.transform.eulerAngles.y + this.rotation;
			}
			if (rotationMode != AddShoreMask.ROTATION.INDEPENDANT_TO_PARENT)
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
			Vector3 vector = base.transform.position + this.offset;
			Matrix4x4 matrix = Matrix4x4.TRS(new Vector3(vector.x, 0f, vector.z), Quaternion.Euler(0f, this.Rotation(), 0f), s);
			Gizmos.color = Color.yellow;
			Gizmos.matrix = matrix;
			Gizmos.DrawWireCube(Vector3.zero, new Vector3(2f, 10f, 2f));
		}
	}
}
