using System;
using UnityEngine;

namespace Ceto
{
	[AddComponentMenu("Ceto/Components/CustomWaveSpectrumExample"), DisallowMultipleComponent, RequireComponent(typeof(Ocean)), RequireComponent(typeof(WaveSpectrum))]
	public class CustomWaveSpectrumExample : MonoBehaviour, ICustomWaveSpectrum
	{
		public class CustomSpectrumConditionKey : WaveSpectrumConditionKey
		{
			public float WindSpeed
			{
				get;
				private set;
			}

			public CustomSpectrumConditionKey(float windSpeed, int size, float windDir, SPECTRUM_TYPE spectrumType, int numGrids) : base(size, windDir, spectrumType, numGrids)
			{
				this.WindSpeed = windSpeed;
			}

			protected override bool Matches(WaveSpectrumConditionKey k)
			{
				CustomWaveSpectrumExample.CustomSpectrumConditionKey customSpectrumConditionKey = k as CustomWaveSpectrumExample.CustomSpectrumConditionKey;
				return !(customSpectrumConditionKey == null) && this.WindSpeed == customSpectrumConditionKey.WindSpeed;
			}

			protected override int AddToHashCode(int hashcode)
			{
				hashcode = hashcode * 37 + this.WindSpeed.GetHashCode();
				return hashcode;
			}
		}

		public class CustomSpectrum : ISpectrum
		{
			private readonly float GRAVITY = 9.818286f;

			private readonly float AMP = 0.02f;

			private readonly float WindSpeed;

			private readonly Vector2 WindDir;

			private readonly float length2;

			private readonly float dampedLength2;

			public CustomSpectrum(float windSpeed, float windDir)
			{
				this.WindSpeed = windSpeed;
				float f = windDir * 3.14159274f / 180f;
				this.WindDir = new Vector2(Mathf.Cos(f), Mathf.Sin(f));
				float num = this.WindSpeed * this.WindSpeed / this.GRAVITY;
				this.length2 = num * num;
				float num2 = 0.001f;
				this.dampedLength2 = this.length2 * num2 * num2;
			}

			public float Spectrum(float kx, float kz)
			{
				float num = kx * this.WindDir.x - kz * this.WindDir.y;
				float num2 = kx * this.WindDir.y + kz * this.WindDir.x;
				kx = num;
				kz = num2;
				float num3 = Mathf.Sqrt(kx * kx + kz * kz);
				if (num3 < 1E-06f)
				{
					return 0f;
				}
				float num4 = num3 * num3;
				float num5 = num4 * num4;
				kx /= num3;
				kz /= num3;
				float num6 = kx * 1f + kz * 0f;
				float num7 = num6 * num6 * num6 * num6 * num6 * num6;
				return this.AMP * Mathf.Exp(-1f / (num4 * this.length2)) / num5 * num7 * Mathf.Exp(-num4 * this.dampedLength2);
			}
		}

		[Range(0f, 30f)]
		public float windSpeed = 10f;

		public bool MultiThreadTask
		{
			get
			{
				return true;
			}
		}

		private void Awake()
		{
			WaveSpectrum component = base.GetComponent<WaveSpectrum>();
			component.CustomWaveSpectrum = this;
		}

		private void Start()
		{
		}

		private void Update()
		{
		}

		public WaveSpectrumConditionKey CreateKey(int size, float windDir, SPECTRUM_TYPE spectrumType, int numGrids)
		{
			return new CustomWaveSpectrumExample.CustomSpectrumConditionKey(this.windSpeed, size, windDir, spectrumType, numGrids);
		}

		public ISpectrum CreateSpectrum(WaveSpectrumConditionKey key)
		{
			CustomWaveSpectrumExample.CustomSpectrumConditionKey customSpectrumConditionKey = key as CustomWaveSpectrumExample.CustomSpectrumConditionKey;
			if (customSpectrumConditionKey == null)
			{
				throw new InvalidCastException("Spectrum condition key is null or not the correct type");
			}
			float num = customSpectrumConditionKey.WindSpeed;
			float windDir = customSpectrumConditionKey.WindDir;
			return new CustomWaveSpectrumExample.CustomSpectrum(num, windDir);
		}

		public Vector4 GetGridSizes(int numGrids)
		{
			if (numGrids == 4)
			{
				return new Vector4(1372f, 217f, 97f, 31f);
			}
			return new Vector4(217f, 97f, 31f, 1f);
		}

		public Vector4 GetChoppyness(int numGrids)
		{
			return new Vector4(1.5f, 1.2f, 1f, 1f);
		}

		public Vector4 GetWaveAmps(int numGrids)
		{
			if (numGrids == 4)
			{
				return new Vector4(0.5f, 1f, 1f, 1f);
			}
			return new Vector4(0.25f, 0.5f, 1f, 1f);
		}
	}
}
