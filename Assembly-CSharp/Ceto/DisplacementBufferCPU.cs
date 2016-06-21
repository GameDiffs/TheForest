using Ceto.Common.Containers.Interpolation;
using Ceto.Common.Threading.Scheduling;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ceto
{
	public class DisplacementBufferCPU : WaveSpectrumBufferCPU, IDisplacementBuffer
	{
		private const int NUM_BUFFERS = 3;

		private IList<InterpolatedArray2f[]> m_displacements;

		public DisplacementBufferCPU(int size, Scheduler scheduler) : base(size, 3, scheduler)
		{
			int gRIDS = QueryDisplacements.GRIDS;
			int cHANNELS = QueryDisplacements.CHANNELS;
			this.m_displacements = new List<InterpolatedArray2f[]>(2);
			this.m_displacements.Add(new InterpolatedArray2f[gRIDS]);
			this.m_displacements.Add(new InterpolatedArray2f[gRIDS]);
			for (int i = 0; i < gRIDS; i++)
			{
				this.m_displacements[0][i] = new InterpolatedArray2f(size, size, cHANNELS, true);
				this.m_displacements[1][i] = new InterpolatedArray2f(size, size, cHANNELS, true);
			}
		}

		protected override void Initilize(WaveSpectrumCondition condition, float time)
		{
			InterpolatedArray2f[] writeDisplacements = this.GetWriteDisplacements();
			writeDisplacements[0].Clear();
			writeDisplacements[1].Clear();
			writeDisplacements[2].Clear();
			writeDisplacements[3].Clear();
			if (this.m_initTask == null)
			{
				this.m_initTask = condition.GetInitSpectrumDisplacementsTask(this, time);
			}
			else if (this.m_initTask.SpectrumType != condition.Key.SpectrumType || this.m_initTask.NumGrids != condition.Key.NumGrids)
			{
				this.m_initTask = condition.GetInitSpectrumDisplacementsTask(this, time);
			}
			else
			{
				this.m_initTask.Reset(condition, time);
			}
		}

		public InterpolatedArray2f[] GetWriteDisplacements()
		{
			return this.m_displacements[0];
		}

		public InterpolatedArray2f[] GetReadDisplacements()
		{
			return this.m_displacements[1];
		}

		public override void Run(WaveSpectrumCondition condition, float time)
		{
			this.SwapDisplacements();
			base.Run(condition, time);
		}

		public void CopyAndCreateDisplacements(out IList<InterpolatedArray2f> displacements)
		{
			InterpolatedArray2f[] readDisplacements = this.GetReadDisplacements();
			QueryDisplacements.CopyAndCreateDisplacements(readDisplacements, out displacements);
		}

		public void CopyDisplacements(IList<InterpolatedArray2f> displacements)
		{
			InterpolatedArray2f[] readDisplacements = this.GetReadDisplacements();
			QueryDisplacements.CopyDisplacements(readDisplacements, displacements);
		}

		private void SwapDisplacements()
		{
			InterpolatedArray2f[] value = this.m_displacements[0];
			this.m_displacements[0] = this.m_displacements[1];
			this.m_displacements[1] = value;
		}

		public Vector4 MaxRange(Vector4 choppyness, Vector2 gridScale)
		{
			InterpolatedArray2f[] readDisplacements = this.GetReadDisplacements();
			return QueryDisplacements.MaxRange(readDisplacements, choppyness, gridScale, null);
		}

		public void QueryWaves(WaveQuery query, QueryGridScaling scaling)
		{
			int num = this.EnabledBuffers();
			if (num == 0)
			{
				return;
			}
			InterpolatedArray2f[] readDisplacements = this.GetReadDisplacements();
			QueryDisplacements.QueryWaves(query, num, readDisplacements, scaling);
		}
	}
}
