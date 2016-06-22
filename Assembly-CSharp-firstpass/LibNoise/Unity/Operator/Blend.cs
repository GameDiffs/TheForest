using System;

namespace LibNoise.Unity.Operator
{
	public class Blend : ModuleBase
	{
		public ModuleBase Controller
		{
			get
			{
				return this.m_modules[2];
			}
			set
			{
				this.m_modules[2] = value;
			}
		}

		public Blend() : base(3)
		{
		}

		public Blend(ModuleBase lhs, ModuleBase rhs, ModuleBase controller) : base(3)
		{
			this.m_modules[0] = lhs;
			this.m_modules[1] = rhs;
			this.m_modules[2] = controller;
		}

		public override double GetValue(double x, double y, double z)
		{
			double value = this.m_modules[0].GetValue(x, y, z);
			double value2 = this.m_modules[1].GetValue(x, y, z);
			double position = (this.m_modules[2].GetValue(x, y, z) + 1.0) / 2.0;
			return Utils.InterpolateLinear(value, value2, position);
		}
	}
}
