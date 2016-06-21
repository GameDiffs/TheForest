using System;
using UnityEngine;

namespace TheForest.Graphics
{
	public class CGUtility : MonoBehaviour
	{
		[Serializable]
		public struct float2
		{
			public float x;

			public float y;

			public CGUtility.float2 normalized
			{
				get
				{
					CGUtility.float2 @float = new CGUtility.float2(this.x, this.y);
					float magnitude = @float.magnitude;
					if (magnitude > 1E-05f)
					{
						@float /= magnitude;
					}
					else
					{
						@float = new CGUtility.float2(0f, 0f);
					}
					return @float;
				}
			}

			public float magnitude
			{
				get
				{
					return Mathf.Sqrt(this.x * this.x + this.y * this.y);
				}
			}

			public float r
			{
				get
				{
					return this.x;
				}
				set
				{
					this.x = value;
				}
			}

			public float g
			{
				get
				{
					return this.y;
				}
				set
				{
					this.y = value;
				}
			}

			public CGUtility.float2 xy
			{
				get
				{
					return new CGUtility.float2(this.x, this.y);
				}
				set
				{
					this.x = value.x;
					this.y = value.y;
				}
			}

			public float2(float x, float y)
			{
				this.x = x;
				this.y = y;
			}

			public float2(CGUtility.float3 v)
			{
				this.x = v.x;
				this.y = v.y;
			}

			public float2(CGUtility.float4 v)
			{
				this.x = v.x;
				this.y = v.y;
			}

			public float2(Color c)
			{
				this.x = c.r;
				this.y = c.g;
			}

			public static implicit operator CGUtility.float2(float v)
			{
				return new CGUtility.float2(v, v);
			}

			public static implicit operator CGUtility.float2(CGUtility.float3 v)
			{
				return new CGUtility.float2(v);
			}

			public static implicit operator CGUtility.float2(CGUtility.float4 v)
			{
				return new CGUtility.float2(v);
			}

			public static implicit operator CGUtility.float2(Color c)
			{
				return new CGUtility.float2(c);
			}

			public static implicit operator CGUtility.float2(Vector4 v)
			{
				return new CGUtility.float2(v.x, v.y);
			}

			public static implicit operator CGUtility.float2(Vector3 v)
			{
				return new CGUtility.float2(v.x, v.y);
			}

			public static implicit operator CGUtility.float2(Vector2 v)
			{
				return new CGUtility.float2(v.x, v.y);
			}

			public static implicit operator CGUtility.float3(CGUtility.float2 v)
			{
				return new CGUtility.float3(v, 0f);
			}

			public static implicit operator CGUtility.float4(CGUtility.float2 v)
			{
				return new CGUtility.float4(v, 0f, 0f);
			}

			public static CGUtility.float2 operator +(CGUtility.float2 a, CGUtility.float2 b)
			{
				return new CGUtility.float2(a.x + b.x, a.y + b.y);
			}

			public static CGUtility.float2 operator -(CGUtility.float2 a, CGUtility.float2 b)
			{
				return new CGUtility.float2(a.x - b.x, a.y - b.y);
			}

			public static CGUtility.float2 operator -(CGUtility.float2 a)
			{
				return new CGUtility.float2(-a.x, -a.y);
			}

			public static CGUtility.float2 operator *(CGUtility.float2 a, float d)
			{
				return new CGUtility.float2(a.x * d, a.y * d);
			}

			public static CGUtility.float2 operator *(float d, CGUtility.float2 a)
			{
				return new CGUtility.float2(a.x * d, a.y * d);
			}

			public static CGUtility.float2 operator /(CGUtility.float2 a, float d)
			{
				return new CGUtility.float2(a.x / d, a.y / d);
			}

			public static CGUtility.float2 operator /(CGUtility.float2 a, CGUtility.float2 b)
			{
				return new CGUtility.float2(a.x / b.x, a.y / b.y);
			}
		}

		[Serializable]
		public struct float3
		{
			public float x;

			public float y;

			public float z;

			public CGUtility.float3 normalized
			{
				get
				{
					CGUtility.float3 @float = new CGUtility.float3(this.x, this.y, this.z);
					float magnitude = @float.magnitude;
					if (magnitude > 1E-05f)
					{
						@float /= magnitude;
					}
					else
					{
						@float = new CGUtility.float3(0f, 0f, 0f);
					}
					return @float;
				}
			}

			public float magnitude
			{
				get
				{
					return Mathf.Sqrt(this.x * this.x + this.y * this.y + this.z * this.z);
				}
			}

			public float r
			{
				get
				{
					return this.x;
				}
				set
				{
					this.x = value;
				}
			}

			public float g
			{
				get
				{
					return this.y;
				}
				set
				{
					this.y = value;
				}
			}

			public float b
			{
				get
				{
					return this.z;
				}
				set
				{
					this.z = value;
				}
			}

			public CGUtility.float2 xy
			{
				get
				{
					return new CGUtility.float2(this.x, this.y);
				}
				set
				{
					this.x = value.x;
					this.y = value.y;
				}
			}

			public CGUtility.float2 xz
			{
				get
				{
					return new CGUtility.float2(this.x, this.z);
				}
				set
				{
					this.x = value.x;
					this.z = value.y;
				}
			}

			public CGUtility.float2 yx
			{
				get
				{
					return new CGUtility.float2(this.y, this.x);
				}
				set
				{
					this.y = value.x;
					this.x = value.y;
				}
			}

			public CGUtility.float2 yz
			{
				get
				{
					return new CGUtility.float2(this.y, this.z);
				}
				set
				{
					this.y = value.x;
					this.z = value.y;
				}
			}

			public CGUtility.float2 zx
			{
				get
				{
					return new CGUtility.float2(this.z, this.x);
				}
				set
				{
					this.z = value.x;
					this.x = value.y;
				}
			}

			public CGUtility.float2 zy
			{
				get
				{
					return new CGUtility.float2(this.z, this.y);
				}
				set
				{
					this.z = value.x;
					this.y = value.y;
				}
			}

			public CGUtility.float3 xyz
			{
				get
				{
					return new CGUtility.float3(this.x, this.y, this.z);
				}
				set
				{
					this.x = value.x;
					this.y = value.y;
					this.z = value.z;
				}
			}

			public float3(float x, float y, float z)
			{
				this.x = x;
				this.y = y;
				this.z = z;
			}

			public float3(CGUtility.float2 v, float z = 0f)
			{
				this.x = v.x;
				this.y = v.y;
				this.z = z;
			}

			public float3(CGUtility.float4 v)
			{
				this.x = v.x;
				this.y = v.y;
				this.z = v.z;
			}

			public float3(Color c)
			{
				this.x = c.r;
				this.y = c.g;
				this.z = c.b;
			}

			public static implicit operator CGUtility.float3(float v)
			{
				return new CGUtility.float3(v, v, v);
			}

			public static implicit operator CGUtility.float3(CGUtility.float2 v)
			{
				return new CGUtility.float3(v, 0f);
			}

			public static implicit operator CGUtility.float3(CGUtility.float4 v)
			{
				return new CGUtility.float3(v);
			}

			public static implicit operator CGUtility.float3(Color c)
			{
				return new CGUtility.float3(c);
			}

			public static implicit operator CGUtility.float3(Vector4 v)
			{
				return new CGUtility.float3(v.x, v.y, v.z);
			}

			public static implicit operator CGUtility.float3(Vector3 v)
			{
				return new CGUtility.float3(v.x, v.y, v.z);
			}

			public static implicit operator CGUtility.float3(Vector2 v)
			{
				return new CGUtility.float3(v.x, v.y, 0f);
			}

			public static implicit operator CGUtility.float2(CGUtility.float3 v)
			{
				return new CGUtility.float2(v);
			}

			public static implicit operator CGUtility.float4(CGUtility.float3 v)
			{
				return new CGUtility.float4(v, 0f);
			}

			public static CGUtility.float3 operator +(CGUtility.float3 a, CGUtility.float3 b)
			{
				return new CGUtility.float3(a.x + b.x, a.y + b.y, a.z + b.z);
			}

			public static CGUtility.float3 operator -(CGUtility.float3 a, CGUtility.float3 b)
			{
				return new CGUtility.float3(a.x - b.x, a.y - b.y, a.z - b.z);
			}

			public static CGUtility.float3 operator -(CGUtility.float3 a)
			{
				return new CGUtility.float3(-a.x, -a.y, -a.z);
			}

			public static CGUtility.float3 operator *(CGUtility.float3 a, float d)
			{
				return new CGUtility.float3(a.x * d, a.y * d, a.z * d);
			}

			public static CGUtility.float3 operator *(float d, CGUtility.float3 a)
			{
				return new CGUtility.float3(a.x * d, a.y * d, a.z * d);
			}

			public static CGUtility.float3 operator /(CGUtility.float3 a, float d)
			{
				return new CGUtility.float3(a.x / d, a.y / d, a.z / d);
			}

			public static CGUtility.float3 operator /(CGUtility.float3 a, CGUtility.float3 d)
			{
				return new CGUtility.float3(a.x / d.x, a.y / d.y, a.z / d.z);
			}
		}

		[Serializable]
		public struct float4
		{
			public float x;

			public float y;

			public float z;

			public float w;

			public CGUtility.float4 normalized
			{
				get
				{
					CGUtility.float4 @float = new CGUtility.float4(this.x, this.y, this.z, this.w);
					float magnitude = @float.magnitude;
					if (magnitude > 1E-05f)
					{
						@float /= magnitude;
					}
					else
					{
						@float = new CGUtility.float4(0f, 0f, 0f, 0f);
					}
					return @float;
				}
			}

			public float magnitude
			{
				get
				{
					return Mathf.Sqrt(this.x * this.x + this.y * this.y + this.z * this.z + this.w * this.w);
				}
			}

			public float r
			{
				get
				{
					return this.x;
				}
				set
				{
					this.x = value;
				}
			}

			public float g
			{
				get
				{
					return this.y;
				}
				set
				{
					this.y = value;
				}
			}

			public float b
			{
				get
				{
					return this.z;
				}
				set
				{
					this.z = value;
				}
			}

			public float a
			{
				get
				{
					return this.w;
				}
				set
				{
					this.w = value;
				}
			}

			public CGUtility.float2 xy
			{
				get
				{
					return new CGUtility.float2(this.x, this.y);
				}
				set
				{
					this.x = value.x;
					this.y = value.y;
				}
			}

			public CGUtility.float2 xz
			{
				get
				{
					return new CGUtility.float2(this.x, this.z);
				}
				set
				{
					this.x = value.x;
					this.z = value.y;
				}
			}

			public CGUtility.float2 yx
			{
				get
				{
					return new CGUtility.float2(this.y, this.x);
				}
				set
				{
					this.y = value.x;
					this.x = value.y;
				}
			}

			public CGUtility.float2 yz
			{
				get
				{
					return new CGUtility.float2(this.y, this.z);
				}
				set
				{
					this.y = value.x;
					this.z = value.y;
				}
			}

			public CGUtility.float2 zx
			{
				get
				{
					return new CGUtility.float2(this.z, this.x);
				}
				set
				{
					this.z = value.x;
					this.x = value.y;
				}
			}

			public CGUtility.float2 zy
			{
				get
				{
					return new CGUtility.float2(this.z, this.y);
				}
				set
				{
					this.z = value.x;
					this.y = value.y;
				}
			}

			public CGUtility.float2 wy
			{
				get
				{
					return new CGUtility.float2(this.w, this.y);
				}
				set
				{
					this.w = value.x;
					this.y = value.y;
				}
			}

			public CGUtility.float2 rg
			{
				get
				{
					return new CGUtility.float2(this.x, this.y);
				}
				set
				{
					this.x = value.x;
					this.y = value.y;
				}
			}

			public CGUtility.float3 xyz
			{
				get
				{
					return new CGUtility.float3(this.x, this.y, this.z);
				}
				set
				{
					this.x = value.x;
					this.y = value.y;
					this.z = value.z;
				}
			}

			public float4(float x, float y, float z, float w)
			{
				this.x = x;
				this.y = y;
				this.z = z;
				this.w = w;
			}

			public float4(CGUtility.float2 v, float z = 0f, float w = 0f)
			{
				this.x = v.x;
				this.y = v.y;
				this.z = z;
				this.w = w;
			}

			public float4(CGUtility.float3 v, float w = 0f)
			{
				this.x = v.x;
				this.y = v.y;
				this.z = v.z;
				this.w = w;
			}

			public float4(Color c)
			{
				this.x = c.r;
				this.y = c.g;
				this.z = c.b;
				this.w = c.a;
			}

			public static implicit operator CGUtility.float4(float v)
			{
				return new CGUtility.float4(v, v, v, v);
			}

			public static implicit operator CGUtility.float4(CGUtility.float2 v)
			{
				return new CGUtility.float4(v, 0f, 0f);
			}

			public static implicit operator CGUtility.float4(CGUtility.float3 v)
			{
				return new CGUtility.float4(v, 0f);
			}

			public static implicit operator CGUtility.float4(Color c)
			{
				return new CGUtility.float4(c);
			}

			public static implicit operator CGUtility.float4(Vector4 v)
			{
				return new CGUtility.float4(v.x, v.y, v.z, v.w);
			}

			public static implicit operator CGUtility.float4(Vector3 v)
			{
				return new CGUtility.float4(v.x, v.y, v.z, 0f);
			}

			public static implicit operator CGUtility.float4(Vector2 v)
			{
				return new CGUtility.float4(v.x, v.y, 0f, 0f);
			}

			public static implicit operator CGUtility.float2(CGUtility.float4 v)
			{
				return new CGUtility.float2(v);
			}

			public static implicit operator CGUtility.float3(CGUtility.float4 v)
			{
				return new CGUtility.float3(v);
			}

			public static CGUtility.float4 operator +(CGUtility.float4 a, CGUtility.float4 b)
			{
				return new CGUtility.float4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
			}

			public static CGUtility.float4 operator -(CGUtility.float4 a, CGUtility.float4 b)
			{
				return new CGUtility.float4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
			}

			public static CGUtility.float4 operator -(CGUtility.float4 a)
			{
				return new CGUtility.float4(-a.x, -a.y, -a.z, -a.w);
			}

			public static CGUtility.float4 operator *(CGUtility.float4 a, float d)
			{
				return new CGUtility.float4(a.x * d, a.y * d, a.z * d, a.w * d);
			}

			public static CGUtility.float4 operator *(float d, CGUtility.float4 a)
			{
				return new CGUtility.float4(a.x * d, a.y * d, a.z * d, a.w * d);
			}

			public static CGUtility.float4 operator /(CGUtility.float4 a, float d)
			{
				return new CGUtility.float4(a.x / d, a.y / d, a.z / d, a.w / d);
			}

			public static CGUtility.float4 operator /(CGUtility.float4 a, CGUtility.float4 d)
			{
				return new CGUtility.float4(a.x / d.x, a.y / d.y, a.z / d.z, a.w / d.w);
			}
		}

		protected CGUtility.float4 _Time
		{
			get
			{
				float num = 0f;
				if (Application.isPlaying)
				{
					num = Time.time;
				}
				return new CGUtility.float4(num / 20f, num, num * 2f, num * 3f);
			}
		}

		private CGUtility.float3 UnpackNormalDXT5nm(CGUtility.float4 packednormal)
		{
			CGUtility.float3 result = new CGUtility.float3(0f, 0f, 0f);
			result.xy = packednormal.wy * 2f - 1f;
			result.z = Mathf.Sqrt(1f - this.saturate(this.dot(result.xy, result.xy)));
			return result;
		}

		protected CGUtility.float3 UnpackNormal(CGUtility.float4 packednormal)
		{
			return this.UnpackNormalDXT5nm(packednormal);
		}

		protected CGUtility.float3 BlendNormals(CGUtility.float3 n1, CGUtility.float3 n2)
		{
			return this.normalize(new CGUtility.float3(n1.x + n2.x, n1.y + n2.y, n1.z * n2.z));
		}

		protected float abs(float value)
		{
			return Mathf.Abs(value);
		}

		protected float acos(float x)
		{
			return Mathf.Acos(x);
		}

		protected float asin(float x)
		{
			return Mathf.Asin(x);
		}

		protected bool all(CGUtility.float2 v)
		{
			return v.x * v.y != 0f;
		}

		protected bool all(CGUtility.float3 v)
		{
			return v.x * v.y * v.z != 0f;
		}

		protected bool all(CGUtility.float4 v)
		{
			return v.x * v.y * v.z * v.w != 0f;
		}

		protected bool any(CGUtility.float2 v)
		{
			return v.x != 0f || v.y != 0f;
		}

		protected bool any(CGUtility.float3 v)
		{
			return v.x != 0f || v.y != 0f || v.z != 0f;
		}

		protected bool any(CGUtility.float4 v)
		{
			return v.x != 0f || v.y != 0f || v.z != 0f || v.w != 0f;
		}

		protected void clip(float x)
		{
		}

		protected float distance(CGUtility.float2 a, CGUtility.float2 b)
		{
			return (a - b).magnitude;
		}

		protected float distance(CGUtility.float3 a, CGUtility.float3 b)
		{
			return (a - b).magnitude;
		}

		protected float distance(CGUtility.float4 a, CGUtility.float4 b)
		{
			return (a - b).magnitude;
		}

		protected float dot(CGUtility.float2 a, CGUtility.float2 b)
		{
			return a.x * b.x + a.y * b.y;
		}

		protected float dot(CGUtility.float3 a, CGUtility.float3 b)
		{
			return a.x * b.x + a.y * b.y + a.x * b.z;
		}

		protected float dot(CGUtility.float4 a, CGUtility.float4 b)
		{
			return a.x * b.x + a.y * b.y + a.x * b.z + a.w * b.w;
		}

		protected float degrees(float x)
		{
			return x * 57.29578f;
		}

		protected float lerp(float a, float b, float x)
		{
			return a + (b - a) * x;
		}

		protected CGUtility.float2 lerp(CGUtility.float2 a, CGUtility.float2 b, float x)
		{
			return new CGUtility.float2(a.x + (b.x - a.x) * x, a.y + (b.y - a.y) * x);
		}

		protected CGUtility.float3 lerp(CGUtility.float3 a, CGUtility.float3 b, float x)
		{
			return new CGUtility.float3(a.x + (b.x - a.x) * x, a.y + (b.y - a.y) * x, a.z + (b.z - a.z) * x);
		}

		protected CGUtility.float4 lerp(CGUtility.float4 a, CGUtility.float4 b, float x)
		{
			return new CGUtility.float4(a.x + (b.x - a.x) * x, a.y + (b.y - a.y) * x, a.z + (b.z - a.z) * x, a.w + (b.w - a.w) * x);
		}

		protected float length(CGUtility.float2 v)
		{
			return v.magnitude;
		}

		protected float length(CGUtility.float3 v)
		{
			return v.magnitude;
		}

		protected float length(CGUtility.float4 v)
		{
			return v.magnitude;
		}

		protected CGUtility.float2 normalize(CGUtility.float2 v)
		{
			return v.normalized;
		}

		protected CGUtility.float3 normalize(CGUtility.float3 v)
		{
			return v.normalized;
		}

		protected CGUtility.float4 normalize(CGUtility.float4 v)
		{
			return v.normalized;
		}

		protected float radians(float x)
		{
			return x * 0.0174532924f;
		}

		protected float saturate(float x)
		{
			return Mathf.Clamp01(x);
		}

		protected float sign(float x)
		{
			return Mathf.Sign(x);
		}

		protected void sincos(float x, out float sin, out float cos)
		{
			sin = Mathf.Sin(x);
			cos = Mathf.Cos(x);
		}

		protected float frac(float x)
		{
			return x - (float)((int)x);
		}

		protected float max(float a, float b)
		{
			return Mathf.Max(a, b);
		}

		private Color tex2Dlod_internal(Texture2D sampler, CGUtility.float4 uv)
		{
			if (sampler == null)
			{
				return Color.clear;
			}
			uv.x -= (float)((int)uv.x);
			uv.y -= (float)((int)uv.y);
			return sampler.GetPixelBilinear(uv.x, uv.y);
		}

		protected CGUtility.float4 tex2Dlod(Texture2D sampler, CGUtility.float4 uv)
		{
			Color color = this.tex2Dlod_internal(sampler, uv);
			return new CGUtility.float4(color.r, color.g, color.b, color.a);
		}
	}
}
