using System;

namespace FMOD
{
	public struct REVERB_PROPERTIES
	{
		public float DecayTime;

		public float EarlyDelay;

		public float LateDelay;

		public float HFReference;

		public float HFDecayRatio;

		public float Diffusion;

		public float Density;

		public float LowShelfFrequency;

		public float LowShelfGain;

		public float HighCut;

		public float EarlyLateMix;

		public float WetLevel;

		public REVERB_PROPERTIES(float decayTime, float earlyDelay, float lateDelay, float hfReference, float hfDecayRatio, float diffusion, float density, float lowShelfFrequency, float lowShelfGain, float highCut, float earlyLateMix, float wetLevel)
		{
			this.DecayTime = decayTime;
			this.EarlyDelay = earlyDelay;
			this.LateDelay = lateDelay;
			this.HFReference = hfReference;
			this.HFDecayRatio = hfDecayRatio;
			this.Diffusion = diffusion;
			this.Density = density;
			this.LowShelfFrequency = lowShelfFrequency;
			this.LowShelfGain = lowShelfGain;
			this.HighCut = highCut;
			this.EarlyLateMix = earlyLateMix;
			this.WetLevel = wetLevel;
		}
	}
}
