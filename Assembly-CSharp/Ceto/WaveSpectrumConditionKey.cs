using System;

namespace Ceto
{
	public abstract class WaveSpectrumConditionKey : IEquatable<WaveSpectrumConditionKey>
	{
		public int Size
		{
			get;
			private set;
		}

		public int NumGrids
		{
			get;
			private set;
		}

		public float WindDir
		{
			get;
			private set;
		}

		public SPECTRUM_TYPE SpectrumType
		{
			get;
			private set;
		}

		public WaveSpectrumConditionKey(int size, float windDir, SPECTRUM_TYPE spectrumType, int numGrids)
		{
			this.Size = size;
			this.NumGrids = numGrids;
			this.WindDir = windDir;
			this.SpectrumType = spectrumType;
		}

		protected abstract bool Matches(WaveSpectrumConditionKey k);

		protected abstract int AddToHashCode(int hashcode);

		public override bool Equals(object o)
		{
			WaveSpectrumConditionKey k = o as WaveSpectrumConditionKey;
			return !(k == null) && k == this;
		}

		public bool Equals(WaveSpectrumConditionKey k)
		{
			return k == this;
		}

		public override int GetHashCode()
		{
			int num = 23;
			num = num * 37 + this.Size.GetHashCode();
			num = num * 37 + this.NumGrids.GetHashCode();
			num = num * 37 + this.WindDir.GetHashCode();
			num = num * 37 + this.SpectrumType.GetHashCode();
			return this.AddToHashCode(num);
		}

		public static bool operator ==(WaveSpectrumConditionKey k1, WaveSpectrumConditionKey k2)
		{
			return object.ReferenceEquals(k1, k2) || (k1 != null && k2 != null && k1.Size == k2.Size && k1.NumGrids == k2.NumGrids && k1.WindDir == k2.WindDir && k1.SpectrumType == k2.SpectrumType && k1.Matches(k2));
		}

		public static bool operator !=(WaveSpectrumConditionKey k1, WaveSpectrumConditionKey k2)
		{
			return !(k1 == k2);
		}
	}
}
