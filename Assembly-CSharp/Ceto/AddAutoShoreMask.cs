using Ceto.Common.Containers.Interpolation;
using System;
using UnityEngine;

namespace Ceto
{
	[AddComponentMenu("Ceto/Overlays/AddAutoShoreMask"), DisallowMultipleComponent]
	public class AddAutoShoreMask : AddWaveOverlayBase
	{
		public bool ignoreQuerys;

		public bool textureFoam = true;

		[Range(0.1f, 100f)]
		public float heightSpread = 10f;

		[Range(0.1f, 10f)]
		public float foamSpread = 2f;

		[Range(0.1f, 10f)]
		public float clipOffset = 4f;

		public int resolution = 1024;

		public bool useHeightMask = true;

		[Range(0f, 1f)]
		public float heightAlpha = 0.9f;

		public bool useNormalMask = true;

		[Range(0f, 1f)]
		public float normalAlpha = 0.8f;

		public bool useEdgeFoam = true;

		[Range(0f, 10f)]
		public float edgeFoamAlpha = 1f;

		public bool useFoamMask = true;

		[Range(0f, 1f)]
		public float foamMaskAlpha = 1f;

		public bool useClipMask = true;

		private bool m_registered;

		private float m_width;

		private float m_height;

		private Texture2D m_heightMask;

		private Texture2D m_edgeFoam;

		private Texture2D m_clipMask;

		private float m_heightSpread;

		private float m_foamSpread;

		private float m_clipOffset;

		private float m_resolution;

		protected override void Start()
		{
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

		protected override void OnDestroy()
		{
			base.OnDestroy();
			this.Release();
		}

		private void UpdateOverlay()
		{
			if (Ocean.Instance != null && (!this.m_registered || this.SettingsChanged()))
			{
				this.CreateShoreMasks();
			}
			Vector2 halfSize = new Vector2(this.m_width * 0.5f, this.m_height * 0.5f);
			Vector3 position = base.transform.position;
			position.x += halfSize.x;
			position.z += halfSize.y;
			this.m_overlays[0].Position = position;
			this.m_overlays[0].HalfSize = halfSize;
			this.m_overlays[0].HeightTex.maskAlpha = ((!this.useHeightMask) ? 0f : this.heightAlpha);
			this.m_overlays[0].HeightTex.ignoreQuerys = this.ignoreQuerys;
			this.m_overlays[0].NormalTex.maskAlpha = ((!this.useNormalMask) ? 0f : this.normalAlpha);
			this.m_overlays[0].NormalTex.maskMode = OVERLAY_MASK_MODE.WAVES_AND_OVERLAY_BLEND;
			this.m_overlays[0].FoamTex.alpha = ((!this.useEdgeFoam) ? 0f : this.edgeFoamAlpha);
			this.m_overlays[0].FoamTex.textureFoam = this.textureFoam;
			this.m_overlays[0].FoamTex.maskAlpha = ((!this.useFoamMask) ? 0f : this.foamMaskAlpha);
			this.m_overlays[0].ClipTex.alpha = ((!this.useClipMask) ? 0f : 1f);
			this.m_overlays[0].ClipTex.ignoreQuerys = this.ignoreQuerys;
			this.m_overlays[0].UpdateOverlay();
		}

		private void CreateShoreMasks()
		{
			this.Release();
			Terrain component = base.GetComponent<Terrain>();
			if (component == null)
			{
				Ocean.LogWarning("The AddAutoShoreMask script must be attached to a component with a Terrain. The shore mask will not be created.");
				base.enabled = false;
				return;
			}
			if (component.terrainData == null)
			{
				Ocean.LogWarning("The terrain data is null. The shore mask will not be created.");
				base.enabled = false;
				return;
			}
			Vector3 size = component.terrainData.size;
			this.resolution = Mathf.Clamp(this.resolution, 32, 4096);
			this.m_width = size.x;
			this.m_height = size.z;
			float level = Ocean.Instance.level;
			float[] data = ShoreMaskGenerator.CreateHeightMap(component);
			int heightmapResolution = component.terrainData.heightmapResolution;
			InterpolatedArray2f heightMap = new InterpolatedArray2f(data, heightmapResolution, heightmapResolution, 1, false);
			if (this.useHeightMask || this.useNormalMask || this.useFoamMask)
			{
				this.m_heightMask = ShoreMaskGenerator.CreateMask(heightMap, this.resolution, this.resolution, level, this.heightSpread, TextureFormat.ARGB32);
			}
			if (this.useEdgeFoam)
			{
				this.m_edgeFoam = ShoreMaskGenerator.CreateMask(heightMap, this.resolution, this.resolution, level, this.foamSpread, TextureFormat.ARGB32);
			}
			if (this.useClipMask)
			{
				this.m_clipMask = ShoreMaskGenerator.CreateClipMask(heightMap, this.resolution, this.resolution, level + this.clipOffset, TextureFormat.ARGB32);
			}
			if (this.useHeightMask)
			{
				this.m_overlays[0].HeightTex.mask = this.m_heightMask;
			}
			if (this.useNormalMask)
			{
				this.m_overlays[0].NormalTex.mask = this.m_heightMask;
			}
			if (this.useFoamMask)
			{
				this.m_overlays[0].FoamTex.mask = this.m_heightMask;
			}
			if (this.useEdgeFoam)
			{
				this.m_overlays[0].FoamTex.tex = this.m_edgeFoam;
			}
			if (this.useClipMask)
			{
				this.m_overlays[0].ClipTex.tex = this.m_clipMask;
			}
			if (!this.m_registered)
			{
				Ocean.Instance.OverlayManager.Add(this.m_overlays[0]);
				this.m_registered = true;
			}
			this.m_heightSpread = this.heightSpread;
			this.m_foamSpread = this.foamSpread;
			this.m_clipOffset = this.clipOffset;
			this.m_resolution = (float)this.resolution;
		}

		private bool SettingsChanged()
		{
			return this.m_heightSpread != this.heightSpread || this.m_foamSpread != this.foamSpread || this.m_clipOffset != this.clipOffset || this.m_resolution != (float)this.resolution;
		}

		private void Release()
		{
			if (this.m_heightMask != null)
			{
				UnityEngine.Object.Destroy(this.m_heightMask);
				this.m_heightMask = null;
			}
			if (this.m_edgeFoam != null)
			{
				UnityEngine.Object.Destroy(this.m_edgeFoam);
				this.m_edgeFoam = null;
			}
			if (this.m_clipMask != null)
			{
				UnityEngine.Object.Destroy(this.m_clipMask);
				this.m_clipMask = null;
			}
			if (this.m_overlays != null && this.m_overlays.Count == 1)
			{
				this.m_overlays[0].HeightTex.mask = null;
				this.m_overlays[0].NormalTex.mask = null;
				this.m_overlays[0].FoamTex.mask = null;
				this.m_overlays[0].FoamTex.tex = null;
				this.m_overlays[0].ClipTex.tex = null;
			}
		}
	}
}
