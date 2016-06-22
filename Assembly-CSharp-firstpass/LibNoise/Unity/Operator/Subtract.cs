using System;

namespace LibNoise.Unity.Operator
{
	public class Subtract : ModuleBase
	{
		public Subtract() : base(2)
		{
		}

		public Subtract(ModuleBase lhs, ModuleBase rhs) : base(2)
		{
			this.m_modules[0] = lhs;
			this.m_modules[1] = rhs;
		}

		public override double GetValue(double x, double y, double z)
		{
			return this.m_modules[0].GetValue(x, y, z) - this.m_modules[1].GetValue(x, y, z);
		}
	}
}
