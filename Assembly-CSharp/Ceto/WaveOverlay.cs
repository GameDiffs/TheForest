using System;
using UnityEngine;

namespace Ceto
{
	public class WaveOverlay
	{
		private static readonly Vector4[] CORNERS = new Vector4[]
		{
			new Vector4(-1f, 0f, -1f, 1f),
			new Vector4(1f, 0f, -1f, 1f),
			new Vector4(1f, 0f, 1f, 1f),
			new Vector4(-1f, 0f, 1f, 1f)
		};

		public bool Kill
		{
			get;
			set;
		}

		public bool Hide
		{
			get;
			set;
		}

		public Vector3 Position
		{
			get;
			set;
		}

		public Vector2 HalfSize
		{
			get;
			set;
		}

		public float Rotation
		{
			get;
			set;
		}

		public float Creation
		{
			get;
			protected set;
		}

		public float Age
		{
			get
			{
				return this.OceanTime() - this.Creation;
			}
		}

		public float Duration
		{
			get;
			protected set;
		}

		public float NormalizedAge
		{
			get
			{
				return Mathf.Clamp01(this.Age / this.Duration);
			}
		}

		public Vector4[] Corners
		{
			get;
			private set;
		}

		public OverlayHeightTexture HeightTex
		{
			get;
			set;
		}

		public OverlayNormalTexture NormalTex
		{
			get;
			set;
		}

		public OverlayFoamTexture FoamTex
		{
			get;
			set;
		}

		public OverlayClipTexture ClipTex
		{
			get;
			set;
		}

		public Matrix4x4 LocalToWorld
		{
			get;
			protected set;
		}

		public Bounds BoundingBox
		{
			get;
			protected set;
		}

		public WaveOverlay(Vector3 pos, float rotation, Vector2 halfSize, float duration)
		{
			this.Position = pos;
			this.HalfSize = halfSize;
			this.Rotation = rotation;
			this.Creation = this.OceanTime();
			this.Duration = Mathf.Max(duration, 0.001f);
			this.Corners = new Vector4[4];
			this.HeightTex = new OverlayHeightTexture();
			this.NormalTex = new OverlayNormalTexture();
			this.FoamTex = new OverlayFoamTexture();
			this.ClipTex = new OverlayClipTexture();
			this.CalculateLocalToWorld();
			this.CalculateBounds();
		}

		public WaveOverlay()
		{
			this.HalfSize = Vector2.one;
			this.Rotation = 0f;
			this.Creation = this.OceanTime();
			this.Duration = 0.001f;
			this.Corners = new Vector4[4];
			this.HeightTex = new OverlayHeightTexture();
			this.NormalTex = new OverlayNormalTexture();
			this.FoamTex = new OverlayFoamTexture();
			this.ClipTex = new OverlayClipTexture();
			this.CalculateLocalToWorld();
			this.CalculateBounds();
		}

		private float OceanTime()
		{
			if (Ocean.Instance == null)
			{
				return 0f;
			}
			return Ocean.Instance.OceanTime.Now;
		}

		public void Reset(Vector3 pos, float rotation, Vector2 halfSize, float duration)
		{
			this.Position = pos;
			this.HalfSize = halfSize;
			this.Rotation = rotation;
			this.Creation = this.OceanTime();
			this.Duration = Mathf.Max(duration, 0.001f);
			this.Kill = false;
			this.Hide = false;
			this.CalculateLocalToWorld();
			this.CalculateBounds();
		}

		public virtual void UpdateOverlay()
		{
			this.CalculateLocalToWorld();
			this.CalculateBounds();
		}

		public virtual void CalculateLocalToWorld()
		{
			Vector3 s = new Vector3(this.HalfSize.x, 1f, this.HalfSize.y);
			this.LocalToWorld = Matrix4x4.TRS(new Vector3(this.Position.x, 0f, this.Position.z), Quaternion.Euler(0f, this.Rotation, 0f), s);
		}

		public virtual void CalculateBounds()
		{
			Vector3 vector = new Vector3(this.HalfSize.x, 1f, this.HalfSize.y);
			float y = 0f;
			if (Ocean.Instance != null)
			{
				y = Ocean.Instance.level;
				vector.y = Ocean.MAX_WAVE_HEIGHT;
			}
			float num = float.PositiveInfinity;
			float num2 = float.PositiveInfinity;
			float num3 = float.NegativeInfinity;
			float num4 = float.NegativeInfinity;
			for (int i = 0; i < 4; i++)
			{
				this.Corners[i] = this.LocalToWorld * WaveOverlay.CORNERS[i];
				this.Corners[i].y = y;
				if (this.Corners[i].x < num)
				{
					num = this.Corners[i].x;
				}
				if (this.Corners[i].z < num2)
				{
					num2 = this.Corners[i].z;
				}
				if (this.Corners[i].x > num3)
				{
					num3 = this.Corners[i].x;
				}
				if (this.Corners[i].z > num4)
				{
					num4 = this.Corners[i].z;
				}
			}
			Vector3 center = new Vector3(this.Position.x, y, this.Position.z);
			Vector3 size = new Vector3(num3 - num, vector.y, num4 - num2);
			this.BoundingBox = new Bounds(center, size);
		}

		public bool Contains(float x, float z)
		{
			float num;
			float num2;
			return this.Contains(x, z, out num, out num2);
		}

		public bool Contains(float x, float z, out float u, out float v)
		{
			u = 0f;
			v = 0f;
			float num = x - this.Position.x;
			float num2 = z - this.Position.z;
			float rotation = this.Rotation;
			float num3 = Mathf.Cos(rotation * 3.14159274f / 180f);
			float num4 = Mathf.Sin(rotation * 3.14159274f / 180f);
			u = num * num3 - num2 * num4;
			v = num * num4 + num2 * num3;
			if (u > -this.HalfSize.x && u < this.HalfSize.x && v > -this.HalfSize.y && v < this.HalfSize.y)
			{
				u /= this.HalfSize.x;
				v /= this.HalfSize.y;
				u = u * 0.5f + 0.5f;
				v = v * 0.5f + 0.5f;
				return true;
			}
			return false;
		}
	}
}
