using System;

namespace LibNoise.Unity.Operator
{
	public class Power : ModuleBase
	{
		public Power() : base(2)
		{
		}

		public Power(ModuleBase lhs, ModuleBase rhs) : base(2)
		{
			this.m_modules[0] = lhs;
			this.m_modules[1] = rhs;
		}

		public override double GetValue(double x, double y, double z)
		{
			return Math.Pow(this.m_modules[0].GetValue(x, y, z), this.m_modules[1].GetValue(x, y, z));
		}
	}
}
