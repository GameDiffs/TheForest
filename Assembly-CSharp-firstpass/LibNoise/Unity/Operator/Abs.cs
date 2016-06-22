using System;

namespace LibNoise.Unity.Operator
{
	public class Abs : ModuleBase
	{
		public Abs() : base(1)
		{
		}

		public Abs(ModuleBase input) : base(1)
		{
			this.m_modules[0] = input;
		}

		public override double GetValue(double x, double y, double z)
		{
			return Math.Abs(this.m_modules[0].GetValue(x, y, z));
		}
	}
}
