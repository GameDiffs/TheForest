using System;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Graphics
{
	[AddComponentMenu("The Forest/Graphics/Lake"), ExecuteInEditMode]
	public class Lake : Water
	{
		public Renderer newRenderer;

		public Collider newCollider;

		public Utility.TextureResolution cubemapResolution = Utility.TextureResolution._512;

		public LayerMask cubemapLayers = -1;

		public bool cubemapSettingsFoldout;

		public bool cubemapMipMaps = true;

		public float cubemapNearClipPlane = 0.5f;

		public float cubemapFarClipPlane = 4000f;

		private Material normalQualityMaterial;

		public override Material SharedMaterial
		{
			get
			{
				if (this.newRenderer == null)
				{
					return null;
				}
				return this.newRenderer.sharedMaterial;
			}
		}

		public override Material InstanceMaterial
		{
			get
			{
				if (this.newRenderer == null)
				{
					return null;
				}
				return this.newRenderer.material;
			}
		}

		public float BoundsSize
		{
			get
			{
				if (this.newCollider)
				{
					return this.newCollider.bounds.size.magnitude;
				}
				if (this.newRenderer)
				{
					return this.newRenderer.bounds.size.magnitude;
				}
				return 0f;
			}
		}

		public bool IsInBounds(Vector3 position)
		{
			if (this.newCollider)
			{
				RaycastHit raycastHit;
				return this.newCollider.Raycast(new Ray(position, Vector3.down), out raycastHit, 0f);
			}
			if (!this.newRenderer)
			{
				return false;
			}
			if (Vector3.Distance(position, this.newRenderer.bounds.center) > this.newRenderer.bounds.size.magnitude / 2f)
			{
				return false;
			}
			position.y = this.newRenderer.bounds.center.y;
			return this.newRenderer.bounds.Contains(position);
		}

		public Vector3 ClosestPoint(Vector3 position)
		{
			if (this.newCollider)
			{
				return this.newCollider.ClosestPointOnBounds(position);
			}
			if (this.newRenderer)
			{
				return this.newRenderer.bounds.ClosestPoint(position);
			}
			return base.transform.position;
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (this.newRenderer == null)
			{
				this.newRenderer = base.GetComponent<Renderer>();
			}
			if (this.newRenderer && !this.normalQualityMaterial)
			{
				this.normalQualityMaterial = this.newRenderer.sharedMaterial;
			}
			WaterEngine.Lakes.Add(this);
			this.InitMaterial();
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			if (WaterEngine.Lakes.Contains(this))
			{
				WaterEngine.Lakes.Remove(this);
			}
		}

		public void InitMaterial()
		{
			if (this.newRenderer && this.normalQualityMaterial && Prefabs.Instance)
			{
				if (TheForestQualitySettings.UserSettings.MaterialQuality == TheForestQualitySettings.MaterialQualities.Low)
				{
					this.newRenderer.sharedMaterial = Prefabs.Instance.LowQualityWaterMaterial;
				}
				else
				{
					this.newRenderer.sharedMaterial = this.normalQualityMaterial;
				}
			}
		}
	}
}
