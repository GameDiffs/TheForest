using System;
using UnityEngine;

namespace LibNoise.Unity.Generator
{
	public class HybridMulti : ModuleBase
	{
		private double m_frequency = 1.0;

		private double m_offset = 0.5;

		private double m_lacunarity = 2.0;

		private double m_gain = 0.5;

		private QualityMode m_quality = QualityMode.Medium;

		private int m_octaveCount = 6;

		private int m_seed;

		private double[] m_weights = new double[30];

		private double m_persistence = 0.5;

		public double Frequency
		{
			get
			{
				return this.m_frequency;
			}
			set
			{
				this.m_frequency = value;
			}
		}

		public double Lacunarity
		{
			get
			{
				return this.m_lacunarity;
			}
			set
			{
				this.m_lacunarity = value;
				this.UpdateWeights();
			}
		}

		public double Persistence
		{
			get
			{
				return this.m_persistence;
			}
			set
			{
				this.m_persistence = value;
			}
		}

		public QualityMode Quality
		{
			get
			{
				return this.m_quality;
			}
			set
			{
				this.m_quality = value;
			}
		}

		public int OctaveCount
		{
			get
			{
				return this.m_octaveCount;
			}
			set
			{
				this.m_octaveCount = Mathf.Clamp(value, 1, 30);
			}
		}

		public int Seed
		{
			get
			{
				return this.m_seed;
			}
			set
			{
				this.m_seed = value;
			}
		}

		public double Offset
		{
			get
			{
				return this.m_offset;
			}
			set
			{
				this.m_offset = value;
			}
		}

		public double Gain
		{
			get
			{
				return this.m_gain;
			}
			set
			{
				this.m_gain = value;
			}
		}

		public HybridMulti() : base(0)
		{
			this.UpdateWeights();
		}

		public HybridMulti(double frequency, double lacunarity, int octaves, double persistence, int seed, double offset, double gain, QualityMode quality) : base(0)
		{
			this.Frequency = frequency;
			this.Lacunarity = lacunarity;
			this.OctaveCount = octaves;
			this.Seed = seed;
			this.Offset = offset;
			this.Persistence = persistence;
			this.Quality = quality;
			this.Gain = gain;
		}

		private void UpdateWeights()
		{
			for (int i = 0; i < 30; i++)
			{
				this.m_weights[i] = Math.Pow(this.m_lacunarity, (double)((float)(-(float)i) * 0.25f));
			}
		}

		public override double GetValue(double x, double y, double z)
		{
			ModuleBase moduleBase = new Perlin(this.Frequency, this.Lacunarity, this.Persistence, this.OctaveCount, this.Seed, QualityMode.Medium);
			x *= this.m_frequency;
			y *= this.m_frequency;
			z *= this.m_frequency;
			float num = (float)this.m_offset + (float)Utils.GradientCoherentNoise3D(x, y, z, (long)this.m_seed, this.m_quality);
			float num2 = (float)this.m_gain * num;
			x *= this.m_lacunarity;
			y *= this.m_lacunarity;
			z *= this.m_lacunarity;
			int num3 = 1;
			while (num2 > 0.001f && num3 < this.m_octaveCount)
			{
				if (num2 > 1f)
				{
					num2 = 1f;
				}
				float num4 = (float)this.m_offset + (float)Utils.GradientCoherentNoise3D(x, y, z, (long)this.m_seed, this.m_quality);
				num4 *= (float)this.m_weights[num3];
				num4 *= num;
				num += num4;
				num2 *= (float)this.m_gain * num4;
				x *= this.m_lacunarity;
				y *= this.m_lacunarity;
				z *= this.m_lacunarity;
				num3++;
			}
			float num5 = (float)(this.m_octaveCount - this.m_octaveCount);
			if (num5 > 0f)
			{
				float num4 = (float)moduleBase.GetValue(x, y, z);
				num4 *= (float)this.m_weights[num3];
				num4 *= num;
				num4 *= num5;
				num += num4;
			}
			return (double)num;
		}
	}
}
