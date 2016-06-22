using System;

namespace LibNoise.Unity.Operator
{
	public class Min : ModuleBase
	{
		public Min() : base(2)
		{
		}

		public Min(ModuleBase lhs, ModuleBase rhs) : base(2)
		{
			this.m_modules[0] = lhs;
			this.m_modules[1] = rhs;
		}

		public override double GetValue(double x, double y, double z)
		{
			double value = this.m_modules[0].GetValue(x, y, z);
			double value2 = this.m_modules[1].GetValue(x, y, z);
			return Math.Min(value, value2);
		}
	}
}
