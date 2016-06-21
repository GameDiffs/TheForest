using System;
using UnityEngine;

namespace Ceto
{
	public class UnifiedPhillipsSpectrumCondition : WaveSpectrumCondition
	{
		public UnifiedPhillipsSpectrumCondition(int size, float windSpeed, float windDir, float waveAge, int numGrids) : base(size, numGrids)
		{
			if (numGrids < 1 || numGrids > 4)
			{
				throw new ArgumentException("UnifiedPhillipsSpectrumCondition must have 1 to 4 grids not " + numGrids);
			}
			base.Key = new UnifiedSpectrumConditionKey(windSpeed, waveAge, size, windDir, SPECTRUM_TYPE.UNIFIED_PHILLIPS, numGrids);
			if (numGrids == 1)
			{
				base.GridSizes = new Vector4(772f, 1f, 1f, 1f);
				base.Choppyness = new Vector4(2.3f, 1f, 1f, 1f);
				base.WaveAmps = new Vector4(1f, 1f, 1f, 1f);
			}
			else if (numGrids == 2)
			{
				base.GridSizes = new Vector4(772f, 97f, 1f, 1f);
				base.Choppyness = new Vector4(2.3f, 1.2f, 1f, 1f);
				base.WaveAmps = new Vector4(1f, 1f, 1f, 1f);
			}
			else if (numGrids == 3)
			{
				base.GridSizes = new Vector4(1372f, 392f, 31f, 1f);
				base.Choppyness = new Vector4(2.3f, 2.1f, 1f, 1f);
				base.WaveAmps = new Vector4(1f, 1f, 1f, 1f);
			}
			else if (numGrids == 4)
			{
				base.GridSizes = new Vector4(1372f, 392f, 31f, 4f);
				base.Choppyness = new Vector4(2.3f, 2.1f, 1f, 0.9f);
				base.WaveAmps = new Vector4(1f, 1f, 1f, 1f);
			}
		}

		public override SpectrumTask GetCreateSpectrumConditionTask()
		{
			UnifiedSpectrumConditionKey unifiedSpectrumConditionKey = base.Key as UnifiedSpectrumConditionKey;
			UnifiedSpectrum unifiedSpectrum = new UnifiedSpectrum(unifiedSpectrumConditionKey.WindSpeed, unifiedSpectrumConditionKey.WindDir, unifiedSpectrumConditionKey.WaveAge);
			PhillipsSpectrum phillipsSpectrum = new PhillipsSpectrum(unifiedSpectrumConditionKey.WindSpeed, unifiedSpectrumConditionKey.WindDir);
			if (base.Key.NumGrids == 1)
			{
				bool arg_53_1 = true;
				ISpectrum[] expr_4F = new ISpectrum[4];
				expr_4F[0] = unifiedSpectrum;
				return new SpectrumTask(this, arg_53_1, expr_4F);
			}
			if (base.Key.NumGrids == 2)
			{
				bool arg_7A_1 = true;
				ISpectrum[] expr_72 = new ISpectrum[4];
				expr_72[0] = unifiedSpectrum;
				expr_72[1] = phillipsSpectrum;
				return new SpectrumTask(this, arg_7A_1, expr_72);
			}
			if (base.Key.NumGrids == 3)
			{
				bool arg_A5_1 = true;
				ISpectrum[] expr_99 = new ISpectrum[4];
				expr_99[0] = unifiedSpectrum;
				expr_99[1] = unifiedSpectrum;
				expr_99[2] = phillipsSpectrum;
				return new SpectrumTask(this, arg_A5_1, expr_99);
			}
			return new SpectrumTask(this, true, new ISpectrum[]
			{
				unifiedSpectrum,
				unifiedSpectrum,
				phillipsSpectrum,
				unifiedSpectrum
			});
		}
	}
}
