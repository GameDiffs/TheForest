using System;
using UnityEngine;

namespace Ceto
{
	public class UnifiedSpectrum : ISpectrum
	{
		private readonly float GRAVITY = 9.818286f;

		private readonly float WAVE_CM = 0.23f;

		private readonly float WAVE_KM = 370f;

		private readonly float U10;

		private readonly float PI_2;

		private readonly float SQRT_10;

		private readonly float G_SQ_OMEGA_U10;

		private readonly float Z_SQ_U10_G;

		private readonly float LOG_OMEGA_6;

		private readonly float SIGMA;

		private readonly float SQ_SIGMA_2;

		private readonly float ALPHA_P;

		private readonly float LOG_2_4;

		private readonly float kp;

		private readonly float cp;

		private readonly float z0;

		private readonly float u_star;

		private readonly float gamma;

		private readonly float HALF_ALPHA_P_CP;

		private readonly float alpham;

		private readonly float HALF_ALPHAM_WAVE_CM;

		private readonly float am;

		private readonly float WindSpeed;

		private readonly float WaveAge;

		private readonly Vector2 WindDir;

		public UnifiedSpectrum(float windSpeed, float windDir, float waveAge)
		{
			this.WindSpeed = windSpeed;
			this.WaveAge = waveAge;
			float f = windDir * 3.14159274f / 180f;
			this.WindDir = new Vector2(Mathf.Cos(f), Mathf.Sin(f));
			this.U10 = this.WindSpeed;
			this.PI_2 = 6.28318548f;
			this.SQRT_10 = Mathf.Sqrt(10f);
			this.G_SQ_OMEGA_U10 = this.GRAVITY * this.sqr(this.WaveAge / this.U10);
			this.Z_SQ_U10_G = 3.7E-05f * this.sqr(this.U10) / 9.81f;
			this.LOG_OMEGA_6 = Mathf.Log(this.WaveAge) * 6f;
			this.SIGMA = 0.08f * (1f + 4f / Mathf.Pow(this.WaveAge, 3f));
			this.SQ_SIGMA_2 = this.sqr(this.SIGMA) * 2f;
			this.ALPHA_P = 0.006f * Mathf.Sqrt(this.WaveAge);
			this.LOG_2_4 = Mathf.Log(2f) / 4f;
			this.kp = this.G_SQ_OMEGA_U10;
			this.cp = this.omega(this.kp) / this.kp;
			this.z0 = this.Z_SQ_U10_G * Mathf.Pow(this.U10 / this.cp, 0.9f);
			this.u_star = 0.41f * this.U10 / Mathf.Log(10f / this.z0);
			this.gamma = ((this.WaveAge >= 1f) ? (1.7f + this.LOG_OMEGA_6) : 1.7f);
			this.HALF_ALPHA_P_CP = 0.5f * this.ALPHA_P * this.cp;
			this.alpham = 0.01f * ((this.u_star >= this.WAVE_CM) ? (1f + 3f * Mathf.Log(this.u_star / this.WAVE_CM)) : (1f + Mathf.Log(this.u_star / this.WAVE_CM)));
			this.HALF_ALPHAM_WAVE_CM = 0.5f * this.alpham * this.WAVE_CM;
			this.am = 0.13f * this.u_star / this.WAVE_CM;
		}

		private float sqr(float x)
		{
			return x * x;
		}

		private float omega(float k)
		{
			return Mathf.Sqrt(this.GRAVITY * k * (1f + this.sqr(k / this.WAVE_KM)));
		}

		public float Spectrum(float kx, float ky)
		{
			float num = kx * this.WindDir.x - ky * this.WindDir.y;
			float num2 = kx * this.WindDir.y + ky * this.WindDir.x;
			kx = num;
			ky = num2;
			float num3 = Mathf.Sqrt(kx * kx + ky * ky);
			float num4 = this.omega(num3) / num3;
			float num5 = Mathf.Exp(-1.25f * this.sqr(this.kp / num3));
			float p = Mathf.Exp(-1f / this.SQ_SIGMA_2 * this.sqr(Mathf.Sqrt(num3 / this.kp) - 1f));
			float num6 = Mathf.Pow(this.gamma, p);
			float num7 = num5 * num6 * Mathf.Exp(-this.WaveAge / this.SQRT_10 * (Mathf.Sqrt(num3 / this.kp) - 1f));
			float num8 = this.HALF_ALPHA_P_CP / num4 * num7;
			float num9 = Mathf.Exp(-0.25f * this.sqr(num3 / this.WAVE_KM - 1f));
			float num10 = this.HALF_ALPHAM_WAVE_CM / num4 * num9 * num5;
			float num11 = (float)Math.Tanh((double)(this.LOG_2_4 + 4f * Mathf.Pow(num4 / this.cp, 2.5f) + this.am * Mathf.Pow(this.WAVE_CM / num4, 2.5f)));
			float num12 = Mathf.Atan2(ky, kx);
			if (kx < 0f)
			{
				return 0f;
			}
			num8 *= 2f;
			num10 *= 2f;
			float num13 = Mathf.Sqrt(Mathf.Max(kx / num3, 0f));
			return (num8 + num10) * (1f + num11 * Mathf.Cos(2f * num12)) / (this.PI_2 * num3 * num3 * num3 * num3) * num13;
		}
	}
}
