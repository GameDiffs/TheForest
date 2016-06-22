using System;
using System.Xml.Serialization;
using UnityEngine;

namespace LibNoise.Unity
{
	public class Noise2D : IDisposable
	{
		public const double South = -90.0;

		public const double North = 90.0;

		public const double West = -180.0;

		public const double East = 180.0;

		public const double AngleMin = -180.0;

		public const double AngleMax = 180.0;

		public const double Left = -1.0;

		public const double Right = 1.0;

		public const double Top = -1.0;

		public const double Bottom = 1.0;

		private int m_width;

		private int m_height;

		private float m_borderValue = float.NaN;

		private float[,] m_data;

		private ModuleBase m_generator;

		[XmlIgnore]
		[NonSerialized]
		private bool m_disposed;

		public float this[int x, int y]
		{
			get
			{
				if (x < 0 && x >= this.m_width)
				{
					throw new ArgumentOutOfRangeException();
				}
				if (y < 0 && y >= this.m_height)
				{
					throw new ArgumentOutOfRangeException();
				}
				return this.m_data[x, y];
			}
			set
			{
				if (x < 0 && x >= this.m_width)
				{
					throw new ArgumentOutOfRangeException();
				}
				if (y < 0 && y >= this.m_height)
				{
					throw new ArgumentOutOfRangeException();
				}
				this.m_data[x, y] = value;
			}
		}

		public float Border
		{
			get
			{
				return this.m_borderValue;
			}
			set
			{
				this.m_borderValue = value;
			}
		}

		public ModuleBase Generator
		{
			get
			{
				return this.m_generator;
			}
			set
			{
				this.m_generator = value;
			}
		}

		public int Height
		{
			get
			{
				return this.m_height;
			}
		}

		public int Width
		{
			get
			{
				return this.m_width;
			}
		}

		public bool IsDisposed
		{
			get
			{
				return this.m_disposed;
			}
		}

		protected Noise2D()
		{
		}

		public Noise2D(int size) : this(size, size, null)
		{
		}

		public Noise2D(int size, ModuleBase generator) : this(size, size, generator)
		{
		}

		public Noise2D(int width, int height) : this(width, height, null)
		{
		}

		public Noise2D(int width, int height, ModuleBase generator)
		{
			this.m_generator = generator;
			this.m_width = width;
			this.m_height = height;
			this.m_data = new float[width, height];
		}

		public void Clear()
		{
			this.Clear(0f);
		}

		public void Clear(float value)
		{
			for (int i = 0; i < this.m_width; i++)
			{
				for (int j = 0; j < this.m_height; j++)
				{
					this.m_data[i, j] = value;
				}
			}
		}

		private double GenerateCylindrical(double angle, double height)
		{
			double x = Math.Cos(angle * 0.017453292519943295);
			double z = Math.Sin(angle * 0.017453292519943295);
			return this.m_generator.GetValue(x, height, z);
		}

		public void GenerateCylindrical(double angleMin, double angleMax, double heightMin, double heightMax)
		{
			if (angleMax <= angleMin || heightMax <= heightMin || this.m_generator == null)
			{
				throw new ArgumentException();
			}
			double num = angleMax - angleMin;
			double num2 = heightMax - heightMin;
			double num3 = num / (double)this.m_width;
			double num4 = num2 / (double)this.m_height;
			double num5 = angleMin;
			for (int i = 0; i < this.m_width; i++)
			{
				double num6 = heightMin;
				for (int j = 0; j < this.m_height; j++)
				{
					this.m_data[i, j] = (float)this.GenerateCylindrical(num5, num6);
					num6 += num4;
				}
				num5 += num3;
			}
		}

		private double GeneratePlanar(double x, double y)
		{
			return this.m_generator.GetValue(x, 0.0, y);
		}

		public void GeneratePlanar(double left, double right, double top, double bottom)
		{
			this.GeneratePlanar(left, right, top, bottom, false);
		}

		public void GeneratePlanar(double left, double right, double top, double bottom, bool seamless)
		{
			if (right <= left || bottom <= top || this.m_generator == null)
			{
				throw new ArgumentException();
			}
			double num = right - left;
			double num2 = bottom - top;
			double num3 = num / (double)this.m_width;
			double num4 = num2 / (double)this.m_height;
			double num5 = left;
			for (int i = 0; i < this.m_width; i++)
			{
				double num6 = top;
				for (int j = 0; j < this.m_height; j++)
				{
					float num7;
					if (!seamless)
					{
						num7 = (float)this.GeneratePlanar(num5, num6);
					}
					else
					{
						double a = this.GeneratePlanar(num5, num6);
						double b = this.GeneratePlanar(num5 + num, num6);
						double a2 = this.GeneratePlanar(num5, num6 + num2);
						double b2 = this.GeneratePlanar(num5 + num, num6 + num2);
						double position = 1.0 - (num5 - left) / num;
						double position2 = 1.0 - (num6 - top) / num2;
						double a3 = Utils.InterpolateLinear(a, b, position);
						double b3 = Utils.InterpolateLinear(a2, b2, position);
						num7 = (float)Utils.InterpolateLinear(a3, b3, position2);
					}
					this.m_data[i, j] = num7;
					num6 += num4;
				}
				num5 += num3;
			}
		}

		private double GenerateSpherical(double lat, double lon)
		{
			double num = Math.Cos(0.017453292519943295 * lat);
			return this.m_generator.GetValue(num * Math.Cos(0.017453292519943295 * lon), Math.Sin(0.017453292519943295 * lat), num * Math.Sin(0.017453292519943295 * lon));
		}

		public void GenerateSpherical(double south, double north, double west, double east)
		{
			if (east <= west || north <= south || this.m_generator == null)
			{
				throw new ArgumentException();
			}
			double num = east - west;
			double num2 = north - south;
			double num3 = num / (double)this.m_width;
			double num4 = num2 / (double)this.m_height;
			double num5 = west;
			for (int i = 0; i < this.m_width; i++)
			{
				double num6 = south;
				for (int j = 0; j < this.m_height; j++)
				{
					this.m_data[i, j] = (float)this.GenerateSpherical(num6, num5);
					num6 += num4;
				}
				num5 += num3;
			}
		}

		public Texture2D GetNormalMap(float scale)
		{
			Texture2D texture2D = new Texture2D(this.m_width, this.m_height);
			Color[] array = new Color[this.m_width * this.m_height];
			for (int i = 0; i < this.m_height; i++)
			{
				for (int j = 0; j < this.m_width; j++)
				{
					Vector3 zero = Vector3.zero;
					Vector3 zero2 = Vector3.zero;
					Vector3 vector = default(Vector3);
					if (j > 0 && i > 0 && j < this.m_width - 1 && i < this.m_height - 1)
					{
						zero = new Vector3((this.m_data[j - 1, i] - this.m_data[j + 1, i]) / 2f * scale, 0f, 1f);
						zero2 = new Vector3(0f, (this.m_data[j, i - 1] - this.m_data[j, i + 1]) / 2f * scale, 1f);
						vector = zero + zero2;
						vector.Normalize();
						Vector3 zero3 = Vector3.zero;
						zero3.x = (vector.x + 1f) / 2f;
						zero3.y = (vector.y + 1f) / 2f;
						zero3.z = (vector.z + 1f) / 2f;
						array[j + i * this.m_height] = new Color(zero3.x, zero3.y, zero3.z);
					}
					else
					{
						zero = new Vector3(0f, 0f, 1f);
						zero2 = new Vector3(0f, 0f, 1f);
						vector = zero + zero2;
						vector.Normalize();
						Vector3 zero4 = Vector3.zero;
						zero4.x = (vector.x + 1f) / 2f;
						zero4.y = (vector.y + 1f) / 2f;
						zero4.z = (vector.z + 1f) / 2f;
						array[j + i * this.m_height] = new Color(zero4.x, zero4.y, zero4.z);
					}
				}
			}
			texture2D.SetPixels(array);
			return texture2D;
		}

		public Texture2D GetTexture()
		{
			return this.GetTexture(Gradient.Grayscale);
		}

		public Texture2D GetTexture(Gradient gradient)
		{
			return this.GetTexture(ref gradient);
		}

		public Texture2D GetTexture(ref Gradient gradient)
		{
			Texture2D texture2D = new Texture2D(this.m_width, this.m_height);
			Color[] array = new Color[this.m_width * this.m_height];
			int num = 0;
			for (int i = 0; i < this.m_height; i++)
			{
				int j = 0;
				while (j < this.m_width)
				{
					float num2;
					if (!float.IsNaN(this.m_borderValue) && (j == 0 || j == this.m_width - 1 || i == 0 || i == this.m_height - 1))
					{
						num2 = this.m_borderValue;
					}
					else
					{
						num2 = this.m_data[j, i];
					}
					array[num] = gradient[(double)num2];
					j++;
					num++;
				}
			}
			texture2D.SetPixels(array);
			return texture2D;
		}

		public void Dispose()
		{
			if (!this.m_disposed)
			{
				this.m_disposed = this.Disposing();
			}
			GC.SuppressFinalize(this);
		}

		protected virtual bool Disposing()
		{
			if (this.m_data != null)
			{
				this.m_data = null;
			}
			this.m_width = 0;
			this.m_height = 0;
			return true;
		}
	}
}
