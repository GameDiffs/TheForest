using System;

namespace Ceto
{
	public class CustomWaveSpectrumCondition : WaveSpectrumCondition
	{
		private ICustomWaveSpectrum m_custom;

		public CustomWaveSpectrumCondition(ICustomWaveSpectrum custom, int size, float windDir, int numGrids) : base(size, numGrids)
		{
			if (numGrids < 1 || numGrids > 4)
			{
				throw new ArgumentException("UCustomSpectrumCondition must have 1 to 4 grids not " + numGrids);
			}
			this.m_custom = custom;
			base.Key = this.m_custom.CreateKey(size, windDir, SPECTRUM_TYPE.CUSTOM, numGrids);
			base.GridSizes = this.m_custom.GetGridSizes(numGrids);
			base.Choppyness = this.m_custom.GetChoppyness(numGrids);
			base.WaveAmps = this.m_custom.GetWaveAmps(numGrids);
		}

		public override SpectrumTask GetCreateSpectrumConditionTask()
		{
			ISpectrum spectrum = this.m_custom.CreateSpectrum(base.Key);
			bool multiThreadTask = this.m_custom.MultiThreadTask;
			return new SpectrumTask(this, multiThreadTask, new ISpectrum[]
			{
				spectrum,
				spectrum,
				spectrum,
				spectrum
			});
		}
	}
}
