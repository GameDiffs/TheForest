using Ceto.Common.Containers.Interpolation;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ceto
{
	public class DisplacementBufferGPU : WaveSpectrumBufferGPU, IDisplacementBuffer
	{
		private static readonly int NUM_BUFFERS = 3;

		private InterpolatedArray2f[] m_displacements;

		public DisplacementBufferGPU(int size, Shader fourierSdr) : base(size, fourierSdr, DisplacementBufferGPU.NUM_BUFFERS)
		{
			int gRIDS = QueryDisplacements.GRIDS;
			int cHANNELS = QueryDisplacements.CHANNELS;
			this.m_displacements = new InterpolatedArray2f[gRIDS];
			for (int i = 0; i < gRIDS; i++)
			{
				this.m_displacements[i] = new InterpolatedArray2f(size, size, cHANNELS, true);
			}
		}

		public InterpolatedArray2f[] GetReadDisplacements()
		{
			return this.m_displacements;
		}

		public void CopyAndCreateDisplacements(out IList<InterpolatedArray2f> displacements)
		{
			QueryDisplacements.CopyAndCreateDisplacements(this.m_displacements, out displacements);
		}

		public void CopyDisplacements(IList<InterpolatedArray2f> displacements)
		{
			QueryDisplacements.CopyDisplacements(this.m_displacements, displacements);
		}

		public Vector4 MaxRange(Vector4 choppyness, Vector2 gridScale)
		{
			return QueryDisplacements.MaxRange(this.m_displacements, choppyness, gridScale, null);
		}

		public void QueryWaves(WaveQuery query, QueryGridScaling scaling)
		{
			int num = this.EnabledBuffers();
			if (num == 0)
			{
				return;
			}
			QueryDisplacements.QueryWaves(query, num, this.m_displacements, scaling);
		}
	}
}
