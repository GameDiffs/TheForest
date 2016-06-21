using System;
using UnityEngine;

namespace Ceto
{
	public class PhillipsSpectrum : ISpectrum
	{
		private readonly float GRAVITY = 9.818286f;

		private readonly float AMP = 0.02f;

		private readonly float WindSpeed;

		private readonly Vector2 WindDir;

		private readonly float length2;

		private readonly float dampedLength2;

		public PhillipsSpectrum(float windSpeed, float windDir)
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
}
