using System;

namespace LibNoise.Unity.Operator
{
	public class ScaleBias : ModuleBase
	{
		private double m_scale = 1.0;

		private double m_bias;

		public double Bias
		{
			get
			{
				return this.m_bias;
			}
			set
			{
				this.m_bias = value;
			}
		}

		public double Scale
		{
			get
			{
				return this.m_scale;
			}
			set
			{
				this.m_scale = value;
			}
		}

		public ScaleBias() : base(1)
		{
		}

		public ScaleBias(double scale, double bias, ModuleBase input) : base(1)
		{
			this.m_modules[0] = input;
			this.Bias = bias;
			this.Scale = scale;
		}

		public override double GetValue(double x, double y, double z)
		{
			return this.m_modules[0].GetValue(x, y, z) * this.m_scale + this.m_bias;
		}
	}
}
