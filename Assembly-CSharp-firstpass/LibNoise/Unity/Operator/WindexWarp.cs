using System;

namespace LibNoise.Unity.Operator
{
	public class WindexWarp : ModuleBase
	{
		public WindexWarp() : base(1)
		{
		}

		public WindexWarp(ModuleBase input) : base(1)
		{
			this.m_modules[0] = input;
		}

		public override double GetValue(double x, double y, double z)
		{
			return (this.m_modules[0].GetValue(x + 1.0, y + 1.0, z) + this.m_modules[0].GetValue(x + 1.0, y, z) + this.m_modules[0].GetValue(x, y + 1.0, z)) * 0.33;
		}
	}
}
