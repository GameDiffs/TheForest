using System;

namespace Ceto
{
	public class PhillipsSpectrumConditionKey : WaveSpectrumConditionKey
	{
		public float WindSpeed
		{
			get;
			private set;
		}

		public PhillipsSpectrumConditionKey(float windSpeed, int size, float windDir, SPECTRUM_TYPE spectrumType, int numGrids) : base(size, windDir, spectrumType, numGrids)
		{
			this.WindSpeed = windSpeed;
		}

		protected override bool Matches(WaveSpectrumConditionKey k)
		{
			PhillipsSpectrumConditionKey phillipsSpectrumConditionKey = k as PhillipsSpectrumConditionKey;
			return !(phillipsSpectrumConditionKey == null) && this.WindSpeed == phillipsSpectrumConditionKey.WindSpeed;
		}

		protected override int AddToHashCode(int hashcode)
		{
			hashcode = hashcode * 37 + this.WindSpeed.GetHashCode();
			return hashcode;
		}
	}
}
