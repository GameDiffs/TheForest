using System;

namespace Ceto
{
	public class UnifiedSpectrumConditionKey : WaveSpectrumConditionKey
	{
		public float WindSpeed
		{
			get;
			private set;
		}

		public float WaveAge
		{
			get;
			private set;
		}

		public UnifiedSpectrumConditionKey(float windSpeed, float waveAge, int size, float windDir, SPECTRUM_TYPE spectrumType, int numGrids) : base(size, windDir, spectrumType, numGrids)
		{
			this.WindSpeed = windSpeed;
			this.WaveAge = waveAge;
		}

		protected override bool Matches(WaveSpectrumConditionKey k)
		{
			UnifiedSpectrumConditionKey unifiedSpectrumConditionKey = k as UnifiedSpectrumConditionKey;
			return !(unifiedSpectrumConditionKey == null) && this.WindSpeed == unifiedSpectrumConditionKey.WindSpeed && this.WaveAge == unifiedSpectrumConditionKey.WaveAge;
		}

		protected override int AddToHashCode(int hashcode)
		{
			hashcode = hashcode * 37 + this.WindSpeed.GetHashCode();
			hashcode = hashcode * 37 + this.WaveAge.GetHashCode();
			return hashcode;
		}
	}
}
