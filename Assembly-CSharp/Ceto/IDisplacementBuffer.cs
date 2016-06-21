using Ceto.Common.Containers.Interpolation;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ceto
{
	public interface IDisplacementBuffer
	{
		bool IsGPU
		{
			get;
		}

		InterpolatedArray2f[] GetReadDisplacements();

		void CopyAndCreateDisplacements(out IList<InterpolatedArray2f> displacements);

		void CopyDisplacements(IList<InterpolatedArray2f> des);

		Vector4 MaxRange(Vector4 choppyness, Vector2 gridScale);

		void QueryWaves(WaveQuery query, QueryGridScaling scaling);

		int EnabledBuffers();
	}
}
