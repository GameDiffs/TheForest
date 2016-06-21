using System;
using UnityEngine;

namespace Ceto
{
	[DisallowMultipleComponent]
	public abstract class WaveSpectrumBase : OceanComponent
	{
		public abstract Vector4 GridSizes
		{
			get;
		}

		public abstract Vector4 Choppyness
		{
			get;
		}

		public abstract float GridScale
		{
			get;
		}

		public abstract Vector2 MaxDisplacement
		{
			get;
			set;
		}

		public abstract bool DisableReadBack
		{
			get;
		}

		public abstract IDisplacementBuffer DisplacementBuffer
		{
			get;
		}

		public ICustomWaveSpectrum CustomWaveSpectrum
		{
			get;
			set;
		}

		public abstract void QueryWaves(WaveQuery query);
	}
}
