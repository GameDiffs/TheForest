using System;
using UnityEngine;

namespace Ceto
{
	public interface ICustomWaveSpectrum
	{
		bool MultiThreadTask
		{
			get;
		}

		WaveSpectrumConditionKey CreateKey(int size, float windDir, SPECTRUM_TYPE spectrumType, int numGrids);

		ISpectrum CreateSpectrum(WaveSpectrumConditionKey key);

		Vector4 GetGridSizes(int numGrids);

		Vector4 GetChoppyness(int numGrids);

		Vector4 GetWaveAmps(int numGrids);
	}
}
