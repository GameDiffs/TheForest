using Ceto.Common.Threading.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Ceto
{
	public class SpectrumTask : ThreadedTask
	{
		public const float GRAVITY = 9.818286f;

		public const float WAVE_CM = 0.23f;

		public const float WAVE_KM = 370f;

		private Color[] m_spectrum01;

		private Color[] m_spectrum23;

		private Color[] m_wtable;

		private ISpectrum[] m_spectrums;

		private System.Random m_rnd;

		private SPECTRUM_DISTRIBUTION m_distibution;

		public int Size
		{
			get;
			private set;
		}

		public int NumGrids
		{
			get;
			private set;
		}

		public Vector4 GridSizes
		{
			get;
			private set;
		}

		public Vector4 InverseGridSizes
		{
			get;
			private set;
		}

		public Vector4 WaveAmps
		{
			get;
			private set;
		}

		protected WaveSpectrumCondition Condition
		{
			get;
			private set;
		}

		public SpectrumTask(WaveSpectrumCondition condition, bool multiThreadTask, ISpectrum[] spectrums) : base(multiThreadTask)
		{
			if (spectrums == null || spectrums.Length != 4)
			{
				throw new ArgumentException("Spectrums array must have a length of 4");
			}
			this.m_spectrums = spectrums;
			this.Condition = condition;
			this.Size = condition.Key.Size;
			this.GridSizes = condition.GridSizes;
			this.WaveAmps = condition.WaveAmps;
			this.NumGrids = condition.Key.NumGrids;
			this.m_rnd = new System.Random(0);
			this.m_distibution = SPECTRUM_DISTRIBUTION.LINEAR;
			float num = 6.28318548f * (float)this.Size;
			this.InverseGridSizes = new Vector4(num / this.GridSizes.x, num / this.GridSizes.y, num / this.GridSizes.z, num / this.GridSizes.w);
			this.m_spectrum01 = new Color[this.Size * this.Size];
			if (this.NumGrids > 2)
			{
				this.m_spectrum23 = new Color[this.Size * this.Size];
			}
			this.m_wtable = new Color[this.Size * this.Size];
		}

		protected float RandomNumber()
		{
			SPECTRUM_DISTRIBUTION distibution = this.m_distibution;
			if (distibution == SPECTRUM_DISTRIBUTION.LINEAR)
			{
				return (float)this.m_rnd.NextDouble();
			}
			if (distibution != SPECTRUM_DISTRIBUTION.GAUSSIAN)
			{
				return (float)this.m_rnd.NextDouble();
			}
			return this.GaussianRandomNumber();
		}

		private float GaussianRandomNumber()
		{
			float num;
			float num3;
			do
			{
				num = 2f * (float)this.m_rnd.NextDouble() - 1f;
				float num2 = 2f * (float)this.m_rnd.NextDouble() - 1f;
				num3 = num * num + num2 * num2;
			}
			while (num3 >= 1f);
			num3 = Mathf.Sqrt(-2f * Mathf.Log(num3) / num3);
			return num * num3;
		}

		public override void Start()
		{
			base.Start();
			this.Condition.CreateTextures();
			this.Condition.Done = false;
		}

		public override IEnumerator Run()
		{
			this.CreateWTable();
			this.GenerateWavesSpectrum();
			this.FinishedRunning();
			return null;
		}

		public override void End()
		{
			base.End();
			this.Condition.Apply(this.m_spectrum01, this.m_spectrum23, this.m_wtable);
			this.Condition.LastUpdated = Time.frameCount;
			this.Condition.Done = true;
		}

		private float GetSpectrumSample(float i, float j, float dk, float kMin, float amp, ISpectrum spectrum)
		{
			if (spectrum == null)
			{
				return 0f;
			}
			float num = i * dk;
			float num2 = j * dk;
			float num3 = 0f;
			if (Math.Abs(num) >= kMin || Math.Abs(num2) >= kMin)
			{
				float num4 = spectrum.Spectrum(num, num2) * amp;
				num3 = Mathf.Sqrt(num4 * 0.5f) * dk;
				if (float.IsNaN(num3) || float.IsInfinity(num3))
				{
					num3 = 0f;
				}
			}
			return num3;
		}

		private void GenerateWavesSpectrum()
		{
			int size = this.Size;
			int num = size / 2;
			float num2 = (float)size;
			int numGrids = this.NumGrids;
			float num3 = 6.28318548f;
			float kMin = 3.14159274f / this.GridSizes.x;
			float kMin2 = 3.14159274f * num2 / this.GridSizes.x;
			float kMin3 = 3.14159274f * num2 / this.GridSizes.y;
			float kMin4 = 3.14159274f * num2 / this.GridSizes.z;
			float dk = num3 / this.GridSizes.x;
			float dk2 = num3 / this.GridSizes.y;
			float dk3 = num3 / this.GridSizes.z;
			float dk4 = num3 / this.GridSizes.w;
			float x = this.WaveAmps.x;
			float y = this.WaveAmps.y;
			float z = this.WaveAmps.z;
			float w = this.WaveAmps.w;
			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					if (this.Cancelled)
					{
						return;
					}
					int num4 = j + i * size;
					float i2 = (j < num) ? ((float)j) : ((float)j - num2);
					float j2 = (i < num) ? ((float)i) : ((float)i - num2);
					Vector4 vector;
					if (numGrids > 0)
					{
						float f = this.RandomNumber() * num3;
						vector.x = this.GetSpectrumSample(i2, j2, dk, kMin, x, this.m_spectrums[0]);
						this.m_spectrum01[num4].r = vector.x * Mathf.Cos(f) * 1.41421354f;
						this.m_spectrum01[num4].g = vector.x * Mathf.Sin(f) * 1.41421354f;
					}
					if (numGrids > 1)
					{
						float f = this.RandomNumber() * num3;
						vector.y = this.GetSpectrumSample(i2, j2, dk2, kMin2, y, this.m_spectrums[1]);
						this.m_spectrum01[num4].b = vector.y * Mathf.Cos(f) * 1.41421354f;
						this.m_spectrum01[num4].a = vector.y * Mathf.Sin(f) * 1.41421354f;
					}
					if (numGrids > 2)
					{
						float f = this.RandomNumber() * num3;
						vector.z = this.GetSpectrumSample(i2, j2, dk3, kMin3, z, this.m_spectrums[2]);
						this.m_spectrum23[num4].r = vector.z * Mathf.Cos(f) * 1.41421354f;
						this.m_spectrum23[num4].g = vector.z * Mathf.Sin(f) * 1.41421354f;
					}
					if (numGrids > 3)
					{
						float f = this.RandomNumber() * num3;
						vector.w = this.GetSpectrumSample(i2, j2, dk4, kMin4, w, this.m_spectrums[3]);
						this.m_spectrum23[num4].b = vector.w * Mathf.Cos(f) * 1.41421354f;
						this.m_spectrum23[num4].a = vector.w * Mathf.Sin(f) * 1.41421354f;
					}
				}
			}
		}

		private void CreateWTable()
		{
			int size = this.Size;
			float num = (float)size;
			float num2 = 1f / num;
			int numGrids = this.NumGrids;
			Vector4 vector;
			vector.x = this.InverseGridSizes.x * this.InverseGridSizes.x;
			vector.y = this.InverseGridSizes.y * this.InverseGridSizes.y;
			vector.z = this.InverseGridSizes.z * this.InverseGridSizes.z;
			vector.w = this.InverseGridSizes.w * this.InverseGridSizes.w;
			float num3 = 136900f;
			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					if (this.Cancelled)
					{
						return;
					}
					int num4 = j + i * size;
					Vector2 vector2;
					vector2.x = (float)j * num2;
					vector2.y = (float)i * num2;
					Vector2 vector3;
					vector3.x = ((vector2.x <= 0.5f) ? vector2.x : (vector2.x - 1f));
					vector3.y = ((vector2.y <= 0.5f) ? vector2.y : (vector2.y - 1f));
					Vector2 vector4;
					vector4.x = vector3.x * vector3.x;
					vector4.y = vector3.y * vector3.y;
					if (numGrids > 0)
					{
						float num5 = Mathf.Sqrt(vector4.x * vector.x + vector4.y * vector.x);
						float r = Mathf.Sqrt(9.818286f * num5 * (1f + num5 * num5 / num3));
						this.m_wtable[num4].r = r;
					}
					if (numGrids > 1)
					{
						float num6 = Mathf.Sqrt(vector4.x * vector.y + vector4.y * vector.y);
						float g = Mathf.Sqrt(9.818286f * num6 * (1f + num6 * num6 / num3));
						this.m_wtable[num4].g = g;
					}
					if (numGrids > 2)
					{
						float num7 = Mathf.Sqrt(vector4.x * vector.z + vector4.y * vector.z);
						float b = Mathf.Sqrt(9.818286f * num7 * (1f + num7 * num7 / num3));
						this.m_wtable[num4].b = b;
					}
					if (numGrids > 3)
					{
						float num8 = Mathf.Sqrt(vector4.x * vector.w + vector4.y * vector.w);
						float a = Mathf.Sqrt(9.818286f * num8 * (1f + num8 * num8 / num3));
						this.m_wtable[num4].a = a;
					}
				}
			}
		}
	}
}
