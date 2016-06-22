using System;
using UnityEngine;

namespace LibNoise.Unity.Generator
{
	public class BrownianMotion : ModuleBase
	{
		private double m_frequency = 1.0;

		private double m_lacunarity = 2.0;

		private QualityMode m_quality = QualityMode.Medium;

		private int m_octaveCount = 6;

		private int m_seed;

		private double[] m_weights = new double[30];

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

		public BrownianMotion() : base(0)
		{
			this.UpdateWeights();
		}

		public BrownianMotion(double frequency, double lacunarity, int octaves, int seed, QualityMode quality) : base(0)
		{
			this.Frequency = frequency;
			this.Lacunarity = lacunarity;
			this.OctaveCount = octaves;
			this.Seed = seed;
			this.Quality = quality;
		}

		private void UpdateWeights()
		{
			double num = 1.0;
			for (int i = 0; i < 30; i++)
			{
				this.m_weights[i] = Math.Pow(num, -1.0);
				num *= this.m_lacunarity;
			}
		}

		public override double GetValue(double x, double y, double z)
		{
			x *= this.Frequency;
			y *= this.Frequency;
			z *= this.Frequency;
			float num = 0f;
			ModuleBase moduleBase = new Perlin(this.Frequency, this.Lacunarity, 0.5, this.OctaveCount, this.Seed, QualityMode.Medium);
			int i;
			for (i = 0; i < this.OctaveCount; i++)
			{
				float num2 = (float)moduleBase.GetValue(x, y, z) * (float)this.m_weights[i];
				num += num2;
				x *= this.Lacunarity;
				y *= this.Lacunarity;
				z *= this.Lacunarity;
			}
			float num3 = (float)(this.OctaveCount - this.OctaveCount);
			if (num3 > 0f)
			{
				num += num3 * (float)moduleBase.GetValue(x, y, z) * (float)this.m_weights[i];
			}
			return (double)num;
		}
	}
}
