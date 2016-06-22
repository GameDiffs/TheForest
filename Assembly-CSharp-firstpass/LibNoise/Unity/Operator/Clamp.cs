using System;

namespace LibNoise.Unity.Operator
{
	public class Clamp : ModuleBase
	{
		private double m_min = -1.0;

		private double m_max = 1.0;

		public double Maximum
		{
			get
			{
				return this.m_max;
			}
			set
			{
				this.m_max = value;
			}
		}

		public double Minimum
		{
			get
			{
				return this.m_min;
			}
			set
			{
				this.m_min = value;
			}
		}

		public Clamp() : base(1)
		{
		}

		public Clamp(double min, double max, ModuleBase input) : base(1)
		{
			this.Minimum = min;
			this.Maximum = max;
			this.m_modules[0] = input;
		}

		public override double GetValue(double x, double y, double z)
		{
			if (this.m_min > this.m_max)
			{
				double min = this.m_min;
				this.m_min = this.m_max;
				this.m_max = min;
			}
			double value = this.m_modules[0].GetValue(x, y, z);
			if (value < this.m_min)
			{
				return this.m_min;
			}
			if (value > this.m_max)
			{
				return this.m_max;
			}
			return value;
		}
	}
}
