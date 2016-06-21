using Ceto.Common.Containers.Interpolation;
using Ceto.Common.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ceto
{
	public class FindRangeTask : ThreadedTask
	{
		private IList<InterpolatedArray2f> m_displacements;

		private WaveSpectrumBase m_spectrum;

		private Vector4 m_max;

		private Vector4 m_choppyness;

		private Vector2 m_gridScale;

		public FindRangeTask(WaveSpectrumBase spectrum) : base(true)
		{
			this.m_spectrum = spectrum;
			this.m_choppyness = spectrum.Choppyness;
			this.m_gridScale = new Vector2(spectrum.GridScale, spectrum.GridScale);
			IDisplacementBuffer displacementBuffer = spectrum.DisplacementBuffer;
			displacementBuffer.CopyAndCreateDisplacements(out this.m_displacements);
		}

		public override void Reset()
		{
			base.Reset();
			this.m_choppyness = this.m_spectrum.Choppyness;
			this.m_gridScale = new Vector2(this.m_spectrum.GridScale, this.m_spectrum.GridScale);
			IDisplacementBuffer displacementBuffer = this.m_spectrum.DisplacementBuffer;
			displacementBuffer.CopyDisplacements(this.m_displacements);
		}

		public override IEnumerator Run()
		{
			this.m_max = QueryDisplacements.MaxRange(this.m_displacements, this.m_choppyness, this.m_gridScale, this);
			this.FinishedRunning();
			return null;
		}

		public override void End()
		{
			this.m_spectrum.MaxDisplacement = new Vector2(Mathf.Max(this.m_max.x, this.m_max.z), this.m_max.y);
			base.End();
		}
	}
}
