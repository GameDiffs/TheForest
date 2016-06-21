using System;
using UnityEngine;

namespace Ceto
{
	public class PhillipsSpectrumCondition : WaveSpectrumCondition
	{
		public PhillipsSpectrumCondition(int size, float windSpeed, float windDir, float waveAge, int numGrids) : base(size, numGrids)
		{
			if (numGrids < 1 || numGrids > 4)
			{
				throw new ArgumentException("PhillipsSpectrumCondition must have 1 to 4 grids not " + numGrids);
			}
			base.Key = new PhillipsSpectrumConditionKey(windSpeed, size, windDir, SPECTRUM_TYPE.PHILLIPS, numGrids);
			if (numGrids == 1)
			{
				base.GridSizes = new Vector4(217f, 1f, 1f, 1f);
				base.Choppyness = new Vector4(1.5f, 1f, 1f, 1f);
				base.WaveAmps = new Vector4(0.5f, 1f, 1f, 1f);
			}
			else if (numGrids == 2)
			{
				base.GridSizes = new Vector4(217f, 97f, 1f, 1f);
				base.Choppyness = new Vector4(1.5f, 1.2f, 1f, 1f);
				base.WaveAmps = new Vector4(0.5f, 1f, 1f, 1f);
			}
			else if (numGrids == 3)
			{
				base.GridSizes = new Vector4(217f, 97f, 31f, 1f);
				base.Choppyness = new Vector4(1.5f, 1.2f, 1f, 1f);
				base.WaveAmps = new Vector4(0.5f, 1f, 1f, 1f);
			}
			else if (numGrids == 4)
			{
				base.GridSizes = new Vector4(1372f, 217f, 97f, 31f);
				base.Choppyness = new Vector4(1.5f, 1.2f, 1f, 1f);
				base.WaveAmps = new Vector4(0.25f, 0.5f, 1f, 1f);
			}
		}

		public override SpectrumTask GetCreateSpectrumConditionTask()
		{
			PhillipsSpectrumConditionKey phillipsSpectrumConditionKey = base.Key as PhillipsSpectrumConditionKey;
			PhillipsSpectrum phillipsSpectrum = new PhillipsSpectrum(phillipsSpectrumConditionKey.WindSpeed, phillipsSpectrumConditionKey.WindDir);
			return new SpectrumTask(this, true, new ISpectrum[]
			{
				phillipsSpectrum,
				phillipsSpectrum,
				phillipsSpectrum,
				phillipsSpectrum
			});
		}
	}
}
