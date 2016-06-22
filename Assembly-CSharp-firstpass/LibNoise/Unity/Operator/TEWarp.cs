using System;

namespace LibNoise.Unity.Operator
{
	public class TEWarp : ModuleBase
	{
		public TEWarp() : base(1)
		{
		}

		public TEWarp(ModuleBase input) : base(1)
		{
			this.m_modules[0] = input;
		}

		public override double GetValue(double x, double y, double z)
		{
			return (this.m_modules[0].GetValue(x + 1.0, y, z + 1.0) + this.m_modules[0].GetValue(x + 1.0, y, z) + this.m_modules[0].GetValue(x, y, z + 1.0)) * 0.33;
		}
	}
}
